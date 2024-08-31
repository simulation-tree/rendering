using Meshes.Components;
using Rendering.Components;
using Rendering.Events;
using Shaders.Components;
using Simulation;
using System;
using Unmanaged;
using Unmanaged.Collections;

namespace Rendering.Systems
{
    public class RenderEngineSystem : SystemBase
    {
        private readonly Query<IsDestination> destinationQuery;
        private readonly Query<IsRenderer> rendererQuery;
        private readonly Query<IsCamera> cameraQuery;
        private readonly UnmanagedList<uint> knownDestinations;
        private readonly UnmanagedDictionary<FixedString, RenderSystemType> availableSystemTypes;
        private readonly UnmanagedDictionary<uint, RenderSystem> renderSystems;

        public RenderEngineSystem(World world) : base(world)
        {
            destinationQuery = new(world);
            rendererQuery = new(world);
            cameraQuery = new(world);
            knownDestinations = new();
            availableSystemTypes = new();
            renderSystems = new();
            Subscribe<RenderUpdate>(Update);
        }

        public override void Dispose()
        {
            //foreach (uint destinationEntity in renderSystems.Keys)
            //{
            //    RenderSystem system = renderSystems[destinationEntity];
            //    system.Dispose();
            //}

            for (uint i = knownDestinations.Count - 1; i != uint.MaxValue; i--)
            {
                uint destinationEntity = knownDestinations[i];
                RenderSystem destinationRenderer = renderSystems[destinationEntity];
                destinationRenderer.Dispose();
            }

            renderSystems.Dispose();
            destinationQuery.Dispose();
            rendererQuery.Dispose();
            cameraQuery.Dispose();
            knownDestinations.Dispose();

            foreach (FixedString label in availableSystemTypes.Keys)
            {
                availableSystemTypes[label].Dispose();
            }

            availableSystemTypes.Dispose();

            base.Dispose();
        }

        /// <summary>
        /// Makes the given render system type available for use at runtime,
        /// for destinations that reference its label.
        /// </summary>
        public void RegisterSystem<T>() where T : unmanaged, IRenderSystem
        {
            FixedString label = default(T).Label;
            if (availableSystemTypes.ContainsKey(label))
            {
                throw new InvalidOperationException($"Label '{label}' already has a render system registered for.");
            }

            RenderSystemType systemCreator = RenderSystemType.Create<T>();
            availableSystemTypes.Add(label, systemCreator);
        }

        private void Update(RenderUpdate update)
        {
            RemoveOldSystems();
            CreateNewSystems();

            //reset lists
            foreach (uint destinationEntity in knownDestinations)
            {
                ref RenderSystem renderSystem = ref renderSystems[destinationEntity];
                renderSystem.cameras.Clear();

                foreach (uint cameraEntity in renderSystem.renderersPerCamera.Keys)
                {
                    UnmanagedDictionary<int, UnmanagedList<uint>> renderersPerCamera = renderSystem.renderersPerCamera[cameraEntity];
                    foreach (int hash in renderersPerCamera.Keys)
                    {
                        UnmanagedList<uint> renderers = renderersPerCamera[hash];
                        renderers.Clear();
                    }
                }

                //notify that surface has been created
                if (!renderSystem.IsSurfaceAvailable && world.TryGetComponent(destinationEntity, out SurfaceReference surface))
                {
                    renderSystem.SurfaceCreated(surface.address);
                }
            }

            //find cameras
            cameraQuery.Update();
            foreach (var r in cameraQuery)
            {
                //todo: efficiency: asking to fetch a camera component more than once
                uint cameraEntity = r.entity;
                uint destination = world.GetComponent<CameraOutput>(cameraEntity).destination;
                if (renderSystems.TryGetValue(destination, out RenderSystem destinationRenderer))
                {
                    destinationRenderer.cameras.Add(cameraEntity);
                }
                else
                {
                    //system with label not found
                }
            }

            //find renderers
            rendererQuery.Update(Query.Option.OnlyEnabledEntities);
            foreach (var r in rendererQuery)
            {
                IsRenderer component = r.Component1;
                rint cameraReference = component.camera;
                rint materialReference = component.material;
                uint cameraEntity = world.GetReference(r.entity, cameraReference);
                uint materialEntity = world.GetReference(r.entity, materialReference);
                if (world.ContainsEntity(cameraEntity) && world.TryGetComponent(cameraEntity, out CameraOutput output))
                {
                    uint destinationEntity = output.destination;
                    if (renderSystems.TryGetValue(destinationEntity, out RenderSystem renderSystem))
                    {
                        //todo: fault: material or mesh entities are allowed to change, but the hash will remains the same
                        rint meshReference = component.mesh;
                        uint meshEntity = world.GetReference(r.entity, meshReference);
                        rint shaderReference = world.GetComponent<IsMaterial>(materialEntity).shaderReference;
                        uint shaderEntity = world.GetReference(materialEntity, shaderReference);

                        if (shaderEntity == default || !world.ContainsComponent<IsShader>(shaderEntity)) continue; //shader not yet loaded
                        if (meshEntity == default || !world.ContainsComponent<IsMesh>(meshEntity)) continue; //mesh not yet loaded

                        if (!renderSystem.renderersPerCamera.TryGetValue(cameraEntity, out UnmanagedDictionary<int, UnmanagedList<uint>> groups))
                        {
                            groups = new();
                            renderSystem.renderersPerCamera.Add(cameraEntity, groups);
                        }

                        int hash = HashCode.Combine(materialEntity, meshEntity);
                        if (!groups.TryGetValue(hash, out UnmanagedList<uint> renderers))
                        {
                            renderers = new();
                            groups.Add(hash, renderers);
                            renderSystem.materials.AddOrSet(hash, materialEntity);
                            renderSystem.shaders.AddOrSet(hash, shaderEntity);
                            renderSystem.meshes.AddOrSet(hash, meshEntity);
                        }

                        renderers.Add(r.entity);
                    }
                }
            }

            //render all
            foreach (uint destinationEntity in knownDestinations)
            {
                if (!world.ContainsComponent<SurfaceReference>(destinationEntity)) continue;
                if (world.GetComponent<IsDestination>(destinationEntity).Area == 0) continue;

                RenderSystem renderSystem = renderSystems[destinationEntity];
                if (renderSystem.BeginRender() == 1) continue;

                //todo: iterate with respect to each camera's sorting order
                foreach (uint camera in renderSystem.cameras)
                {
                    if (!renderSystem.renderersPerCamera.TryGetValue(camera, out UnmanagedDictionary<int, UnmanagedList<uint>> groups)) continue;

                    foreach (int hash in groups.Keys)
                    {
                        UnmanagedList<uint> renderers = groups[hash];
                        uint rendererCount = renderers.Count;

                        //make sure renderer entries that no longer exist are not in this list
                        for (uint r = rendererCount - 1; r != uint.MaxValue; r--)
                        {
                            uint rendererEntity = renderers[r];
                            if (!world.ContainsEntity(rendererEntity))
                            {
                                renderers.RemoveAt(r);
                            }
                        }

                        rendererCount = renderers.Count;
                        if (rendererCount > 0)
                        {
                            uint material = renderSystem.materials[hash];
                            uint mesh = renderSystem.meshes[hash];
                            uint shader = renderSystem.shaders[hash];
                            renderSystem.Render(renderers.AsSpan(), material, shader, mesh);
                        }
                    }
                }

                renderSystem.EndRender();
            }
        }

        private void CreateNewSystems()
        {
            destinationQuery.Update();
            Span<FixedString> extensionNames = stackalloc FixedString[32];
            int extensionNamesLength = 0;
            foreach (var r in destinationQuery)
            {
                uint destinationEntity = r.entity;
                if (knownDestinations.Contains(destinationEntity)) continue;

                IsDestination component = r.Component1;
                FixedString label = component.rendererLabel;
                Destination destination = new(world, destinationEntity);
                extensionNamesLength = destination.CopyExtensionNamesTo(extensionNames);
                if (availableSystemTypes.TryGetValue(label, out RenderSystemType systemCreator))
                {
                    RenderSystem system = systemCreator.Create(destination, extensionNames[..extensionNamesLength]);
                    renderSystems.Add(destinationEntity, system);
                    knownDestinations.Add(destinationEntity);
                    world.AddComponent(destinationEntity, new RenderSystemInUse(system.library));
                }
                else
                {
                    //throw new InvalidOperationException($"Unknown renderer label '{label}'");
                }
            }
        }

        private void RemoveOldSystems()
        {
            for (uint i = knownDestinations.Count - 1; i != uint.MaxValue; i--)
            {
                uint destinationEntity = knownDestinations[i];
                if (!world.ContainsEntity(destinationEntity))
                {
                    RenderSystem destinationRenderer = renderSystems.Remove(destinationEntity);
                    destinationRenderer.Dispose();
                    knownDestinations.RemoveAt(i);
                }
            }
        }
    }
}