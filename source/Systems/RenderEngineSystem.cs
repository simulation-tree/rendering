using Collections;
using Meshes.Components;
using Rendering.Components;
using Shaders.Components;
using Simulation;
using System;
using System.Diagnostics;
using Unmanaged;
using Worlds;

namespace Rendering.Systems
{
    public readonly partial struct RenderEngineSystem : ISystem
    {
        private readonly List<Destination> knownDestinations;
        private readonly Dictionary<FixedString, RenderSystemType> availableSystemTypes;
        private readonly Dictionary<Destination, RenderSystem> renderSystems;
        private readonly Array<List<Viewport>> viewportEntities;

        public RenderEngineSystem()
        {
            knownDestinations = new();
            availableSystemTypes = new();
            renderSystems = new();
            viewportEntities = new(32);
            for (uint i = 0; i < viewportEntities.Length; i++)
            {
                viewportEntities[i] = new(32);
            }
        }

        void ISystem.Start(in SystemContainer systemContainer, in World world)
        {
        }

        void ISystem.Update(in SystemContainer systemContainer, in World world, in TimeSpan delta)
        {
            if (systemContainer.World == world)
            {
                RemoveOldSystems();
                RenderAll();
            }

            Update(world);
        }

        void ISystem.Finish(in SystemContainer systemContainer, in World world)
        {
            if (systemContainer.World == world)
            {
                for (uint i = viewportEntities.Length - 1; i != uint.MaxValue; i--)
                {
                    viewportEntities[i].Dispose();
                }

                for (uint i = knownDestinations.Count - 1; i != uint.MaxValue; i--)
                {
                    Destination destination = knownDestinations[i];
                    RenderSystem destinationRenderer = renderSystems[destination];
                    destinationRenderer.Dispose();
                }

                viewportEntities.Dispose();
                renderSystems.Dispose();
                knownDestinations.Dispose();

                foreach (FixedString label in availableSystemTypes.Keys)
                {
                    availableSystemTypes[label].Dispose();
                }

                availableSystemTypes.Dispose();
            }
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
                throw new InvalidOperationException($"Label `{label}` already has a render system registered for");
            }

            RenderSystemType systemCreator = RenderSystemType.Create<T>();
            availableSystemTypes.Add(label, systemCreator);
        }

        private readonly void CreateNewSystems(World world)
        {
            USpan<FixedString> extensionNames = stackalloc FixedString[32];
            ComponentQuery<IsDestination> query = new(world);
            foreach (var r in query)
            {
                ref IsDestination component = ref r.component1;
                Destination destination = new(world, r.entity);
                if (knownDestinations.Contains(destination))
                {
                    return;
                }

                FixedString label = component.rendererLabel;
                if (availableSystemTypes.TryGetValue(label, out RenderSystemType systemCreator))
                {
                    uint extensionNamesLength = destination.CopyExtensionNamesTo(extensionNames);
                    RenderSystem newRenderSystem = systemCreator.Create(destination, extensionNames.Slice(0, extensionNamesLength));
                    renderSystems.Add(destination, newRenderSystem);
                    knownDestinations.Add(destination);
                    destination.AddComponent(new RenderSystemInUse(newRenderSystem.library));
                    Trace.WriteLine($"Created render system for destination `{destination}` with label `{label}`");
                }
                else
                {
                    throw new InvalidOperationException($"Unknown renderer label `{label}`");
                }
            }
        }

        private readonly void FindRenderers(World world)
        {
            ComponentQuery<IsRenderer> query = new(world);
            foreach (var r in query)
            {
                uint entity = r.entity;
                if (!world.IsEnabled(entity)) continue;

                ref IsRenderer component = ref r.component1;
                uint mask = component.mask;
                for (uint l = 0; l < 32; l++)
                {
                    if ((mask & (1 << (int)l)) != 0)
                    {
                        foreach (Viewport viewport in viewportEntities[l])
                        {
                            Destination destination = viewport.Destination;
                            if (renderSystems.TryGetValue(destination, out RenderSystem renderSystem))
                            {
                                rint materialReference = component.materialReference;
                                uint materialEntity = world.GetReference(entity, materialReference);
                                rint meshReference = component.meshReference;
                                uint meshEntity = world.GetReference(entity, meshReference);
                                rint shaderReference = world.GetComponent<IsMaterial>(materialEntity).shaderReference;
                                if (shaderReference == default) continue;

                                uint shaderEntity = world.GetReference(materialEntity, shaderReference);

                                if (shaderEntity == default || !world.ContainsEntity(shaderEntity) || !world.ContainsComponent<IsShader>(shaderEntity)) continue; //shader not yet loaded
                                if (meshEntity == default || !world.ContainsComponent<IsMesh>(meshEntity)) continue; //mesh not yet loaded

                                if (!renderSystem.renderers.TryGetValue(viewport, out Dictionary<int, List<uint>> groups))
                                {
                                    groups = new();
                                    renderSystem.renderers.Add(viewport, groups);
                                }

                                int hash = HashCode.Combine(materialEntity, meshEntity);
                                if (!groups.TryGetValue(hash, out List<uint> renderers))
                                {
                                    renderers = new();
                                    groups.Add(hash, renderers);
                                    renderSystem.materials.AddOrSet(hash, new(world, materialEntity));
                                    renderSystem.shaders.AddOrSet(hash, new(world, shaderEntity));
                                    renderSystem.meshes.AddOrSet(hash, new(world, meshEntity));
                                }

                                renderers.Add(entity);
                            }
                        }
                    }
                }
            }
        }

        private readonly void FindViewports(World world)
        {
            //reset viewport lists
            for (uint i = 0; i < viewportEntities.Length; i++)
            {
                viewportEntities[i].Clear();
            }

            ComponentQuery<IsViewport> query = new(world);
            foreach (var r in query)
            {
                Viewport viewport = new Entity(world, r.entity).As<Viewport>();
                Destination destination = viewport.Destination;
                if (renderSystems.TryGetValue(destination, out RenderSystem destinationRenderer))
                {
                    destinationRenderer.viewports.Add(viewport);
                    renderSystems[destination] = destinationRenderer;
                }
                else
                {
                    //system with label not found
                }

                uint mask = viewport.GetMask();
                for (uint l = 0; l < 32; l++)
                {
                    if ((mask & (1 << (int)l)) != 0)
                    {
                        viewportEntities[l].Add(viewport);
                    }
                }
            }
        }

        private readonly void Update(World world)
        {
            CreateNewSystems(world);

            //reset lists
            foreach (Destination destination in knownDestinations)
            {
                if (destination.GetWorld() == world)
                {
                    ref RenderSystem renderSystem = ref renderSystems[destination];
                    renderSystem.viewports.Clear();

                    foreach (Viewport viewport in renderSystem.renderers.Keys)
                    {
                        Dictionary<int, List<uint>> renderersPerCamera = renderSystem.renderers[viewport];
                        foreach (int hash in renderersPerCamera.Keys)
                        {
                            List<uint> renderers = renderersPerCamera[hash];
                            renderers.Clear();
                        }
                    }

                    //notify that surface has been created
                    if (!renderSystem.IsSurfaceAvailable && destination.TryGetSurfaceReference(out SurfaceReference surface))
                    {
                        renderSystem.SurfaceCreated(surface.address);
                    }
                }
            }

            FindViewports(world);
            FindRenderers(world);
        }

        private readonly void RenderAll()
        {
            foreach (Destination destination in knownDestinations)
            {
                if (!destination.AsEntity().ContainsComponent<SurfaceReference>()) continue;

                IsDestination component = destination.AsEntity().GetComponent<IsDestination>();
                if (component.Area == 0) continue;

                RenderSystem renderSystem = renderSystems[destination];
                if (renderSystem.BeginRender(component.clearColor) == 1) continue;

                //todo: iterate with respect to each camera's sorting order
                World world = destination.GetWorld();
                foreach (Viewport viewport in renderSystem.viewports)
                {
                    if (renderSystem.renderers.TryGetValue(viewport, out Dictionary<int, List<uint>> groups))
                    {
                        foreach (int hash in groups.Keys)
                        {
                            List<uint> renderers = groups[hash];
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
                }

                renderSystem.EndRender();
            }
        }

        private readonly void RemoveOldSystems()
        {
            for (uint i = knownDestinations.Count - 1; i != uint.MaxValue; i--)
            {
                Destination destination = knownDestinations[i];
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