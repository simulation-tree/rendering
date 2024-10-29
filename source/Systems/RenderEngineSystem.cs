using Meshes.Components;
using Rendering.Components;
using Shaders.Components;
using Simulation;
using Simulation.Functions;
using System;
using System.Runtime.InteropServices;
using Unmanaged;
using Unmanaged.Collections;

namespace Rendering.Systems
{
    public struct RenderEngineSystem : ISystem
    {
        private readonly ComponentQuery<IsDestination> destinationQuery;
        private readonly ComponentQuery<IsRenderer> rendererQuery;
        private readonly ComponentQuery<IsCamera> cameraQuery;
        private readonly UnmanagedList<Entity> knownDestinations;
        private readonly UnmanagedDictionary<FixedString, RenderSystemType> availableSystemTypes;
        private readonly UnmanagedDictionary<Entity, RenderSystem> renderSystems;

        readonly unsafe InitializeFunction ISystem.Initialize => new(&Initialize);
        readonly unsafe IterateFunction ISystem.Update => new(&Update);
        readonly unsafe FinalizeFunction ISystem.Finalize => new(&Finalize);

        [UnmanagedCallersOnly]
        private static void Initialize(SystemContainer container, World world)
        {
        }

        [UnmanagedCallersOnly]
        private static void Update(SystemContainer container, World world, TimeSpan delta)
        {
            ref RenderEngineSystem system = ref container.Read<RenderEngineSystem>();
            if (container.World == world)
            {
                system.RemoveOldSystems();
                system.RenderAll();
            }

            system.Update(world);
        }

        [UnmanagedCallersOnly]
        private static void Finalize(SystemContainer container, World world)
        {
            if (container.World == world)
            {
                ref RenderEngineSystem system = ref container.Read<RenderEngineSystem>();
                system.CleanUp();
            }
        }

        public RenderEngineSystem()
        {
            destinationQuery = new();
            rendererQuery = new();
            cameraQuery = new();
            knownDestinations = new();
            availableSystemTypes = new();
            renderSystems = new();
        }

        private readonly void CleanUp()
        {
            for (uint i = knownDestinations.Count - 1; i != uint.MaxValue; i--)
            {
                Entity destination = knownDestinations[i];
                RenderSystem destinationRenderer = renderSystems[destination];
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
        }

        /// <summary>
        /// Makes the given render system type available for use at runtime,
        /// for destinations that reference its label.
        /// </summary>
        public readonly void RegisterRenderSystem<T>() where T : unmanaged, IRenderer
        {
            FixedString label = default(T).Label;
            if (availableSystemTypes.ContainsKey(label))
            {
                throw new InvalidOperationException($"Label '{label}' already has a render system registered for.");
            }

            RenderSystemType systemCreator = RenderSystemType.Create<T>();
            availableSystemTypes.Add(label, systemCreator);
        }

        private readonly void Update(World world)
        {
            CreateNewSystems(world);

            //reset lists
            foreach (Entity destination in knownDestinations)
            {
                if (destination.GetWorld() == world)
                {
                    ref RenderSystem renderSystem = ref renderSystems[destination];
                    renderSystem.cameras.Clear();

                    foreach (Entity camera in renderSystem.renderersPerCamera.Keys)
                    {
                        UnmanagedDictionary<int, UnmanagedList<uint>> renderersPerCamera = renderSystem.renderersPerCamera[camera];
                        foreach (int hash in renderersPerCamera.Keys)
                        {
                            UnmanagedList<uint> renderers = renderersPerCamera[hash];
                            renderers.Clear();
                        }
                    }

                    //notify that surface has been created
                    if (!renderSystem.IsSurfaceAvailable && destination.TryGetComponent(out SurfaceReference surface))
                    {
                        renderSystem.SurfaceCreated(surface.address);
                    }
                }
            }

            //find cameras
            cameraQuery.Update(world);
            foreach (var r in cameraQuery)
            {
                //todo: efficiency: asking to fetch a camera component more than once
                uint cameraEntity = r.entity;
                Entity camera = new(world, cameraEntity);
                rint destinationReference = camera.GetComponent<CameraOutput>().destinationReference;
                uint destinationEntity = camera.GetReference(destinationReference);
                Entity destination = new(world, destinationEntity);
                if (renderSystems.TryGetValue(destination, out RenderSystem destinationRenderer))
                {
                    destinationRenderer.cameras.Add(camera);
                    renderSystems[destination] = destinationRenderer;
                }
                else
                {
                    //system with label not found
                }
            }

            //find renderers
            rendererQuery.Update(world, true);
            foreach (var r in rendererQuery)
            {
                IsRenderer component = r.Component1;
                rint cameraReference = component.cameraReference;
                rint materialReference = component.materialReference;
                uint cameraEntity = world.GetReference(r.entity, cameraReference);
                uint materialEntity = world.GetReference(r.entity, materialReference);
                Entity camera = new(world, cameraEntity);
                if (!camera.IsDestroyed() && camera.TryGetComponent(out CameraOutput output))
                {
                    rint destinationReference = output.destinationReference;
                    uint destinationEntity = camera.GetReference(destinationReference);
                    Entity destination = new(world, destinationEntity);
                    if (renderSystems.TryGetValue(destination, out RenderSystem renderSystem))
                    {
                        //todo: fault: material or mesh entities are allowed to change, but the hash will remains the same
                        rint meshReference = component.meshReference;
                        uint meshEntity = world.GetReference(r.entity, meshReference);
                        rint shaderReference = world.GetComponent<IsMaterial>(materialEntity).shaderReference;
                        uint shaderEntity = world.GetReference(materialEntity, shaderReference);

                        if (shaderEntity == default || !world.ContainsEntity(shaderEntity) || !world.ContainsComponent<IsShader>(shaderEntity)) continue; //shader not yet loaded
                        if (meshEntity == default || !world.ContainsComponent<IsMesh>(meshEntity)) continue; //mesh not yet loaded

                        if (!renderSystem.renderersPerCamera.TryGetValue(camera, out UnmanagedDictionary<int, UnmanagedList<uint>> groups))
                        {
                            groups = new();
                            renderSystem.renderersPerCamera.Add(camera, groups);
                        }

                        int hash = HashCode.Combine(materialEntity, meshEntity);
                        if (!groups.TryGetValue(hash, out UnmanagedList<uint> renderers))
                        {
                            renderers = new();
                            groups.Add(hash, renderers);
                            renderSystem.materials.AddOrSet(hash, new(world, materialEntity));
                            renderSystem.shaders.AddOrSet(hash, new(world, shaderEntity));
                            renderSystem.meshes.AddOrSet(hash, new(world, meshEntity));
                        }

                        renderers.Add(r.entity);
                    }
                }
            }
        }

        private readonly void RenderAll()
        {
            foreach (Entity destination in knownDestinations)
            {
                if (!destination.ContainsComponent<SurfaceReference>()) continue;

                IsDestination component = destination.GetComponent<IsDestination>();
                if (component.Area == 0) continue;

                RenderSystem renderSystem = renderSystems[destination];
                if (renderSystem.BeginRender(component.clearColor) == 1) continue;

                //todo: iterate with respect to each camera's sorting order
                World world = destination.GetWorld();
                foreach (Entity camera in renderSystem.cameras)
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
                            Entity material = renderSystem.materials[hash];
                            Entity mesh = renderSystem.meshes[hash];
                            Entity shader = renderSystem.shaders[hash];
                            renderSystem.Render(renderers.AsSpan(), material.GetEntityValue(), shader.GetEntityValue(), mesh.GetEntityValue());
                        }
                    }
                }

                renderSystem.EndRender();
            }
        }

        private readonly void CreateNewSystems(World world)
        {
            destinationQuery.Update(world);
            USpan<FixedString> extensionNames = stackalloc FixedString[32];
            uint extensionNamesLength = 0;
            foreach (var r in destinationQuery)
            {
                uint destinationEntity = r.entity;
                Entity destination = new(world, destinationEntity);
                if (knownDestinations.Contains(destination))
                {
                    continue;
                }

                IsDestination component = r.Component1;
                FixedString label = component.rendererLabel;
                extensionNamesLength = destination.As<Destination>().CopyExtensionNamesTo(extensionNames);
                if (availableSystemTypes.TryGetValue(label, out RenderSystemType systemCreator))
                {
                    RenderSystem system = systemCreator.Create(destination.As<Destination>(), extensionNames.Slice(0, extensionNamesLength));
                    renderSystems.Add(destination, system);
                    knownDestinations.Add(destination);
                    destination.AddComponent(new RenderSystemInUse(system.library));
                }
                else
                {
                    throw new InvalidOperationException($"Unknown renderer label '{label}'");
                }
            }
        }

        private readonly void RemoveOldSystems()
        {
            for (uint i = knownDestinations.Count - 1; i != uint.MaxValue; i--)
            {
                Entity destination = knownDestinations[i];
                if (destination.IsDestroyed())
                {
                    RenderSystem destinationRenderer = renderSystems.Remove(destination);
                    destinationRenderer.Dispose();
                    knownDestinations.RemoveAt(i);
                }
            }
        }
    }
}