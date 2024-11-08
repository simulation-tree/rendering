using Collections;
using Data.Components;
using Rendering.Components;
using Shaders;
using Simulation;
using Simulation.Functions;
using System;
using System.Runtime.InteropServices;
using Unmanaged;
using Unmanaged.JSON;

namespace Rendering.Systems
{
    public readonly struct MaterialImportSystem : ISystem
    {
        private readonly ComponentQuery<IsMaterial, IsDataRequest> query;
        private readonly Dictionary<FixedString, Shader> cachedShaders;

        readonly unsafe InitializeFunction ISystem.Initialize => new(&Initialize);
        readonly unsafe IterateFunction ISystem.Iterate => new(&Update);
        readonly unsafe FinalizeFunction ISystem.Finalize => new(&Finalize);

        [UnmanagedCallersOnly]
        private static void Initialize(SystemContainer container, World world)
        {
        }

        [UnmanagedCallersOnly]
        private static void Update(SystemContainer container, World world, TimeSpan delta)
        {
            ref MaterialImportSystem system = ref container.Read<MaterialImportSystem>();
            system.Update(world);
        }

        [UnmanagedCallersOnly]
        private static void Finalize(SystemContainer container, World world)
        {
            if (container.World == world)
            {
                ref MaterialImportSystem system = ref container.Read<MaterialImportSystem>();
                system.CleanUp();
            }
        }

        public MaterialImportSystem()
        {
            query = new();
            cachedShaders = new();
        }

        private void CleanUp()
        {
            query.Dispose();
            cachedShaders.Dispose();
        }

        private void Update(World world)
        {
            query.Update(world);
            foreach (var x in query)
            {
                ref IsMaterial component = ref x.Component1;
                if (component.shaderReference == default)
                {
                    FixedString address = x.Component2.address;
                    if (!cachedShaders.TryGetValue(address, out Shader shader))
                    {
                        if (world.ContainsArray<byte>(x.entity))
                        {
                            using BinaryReader reader = new(world.GetArray<byte>(x.entity));
                            using JSONObject jsonObject = reader.ReadObject<JSONObject>();
                            bool hasVertexProperty = jsonObject.Contains("vertex");
                            bool hasFragmentProperty = jsonObject.Contains("fragment");
                            if (hasVertexProperty && hasFragmentProperty)
                            {
                                //todo: test materials and shaders loading from json
                                USpan<char> vertexAddress = jsonObject.GetText("vertex");
                                USpan<char> fragmentAddress = jsonObject.GetText("fragment");
                                shader = new(world, vertexAddress, fragmentAddress);
                                cachedShaders.Add(address, shader);
                            }
                            else if (!hasVertexProperty && !hasFragmentProperty)
                            {
                                throw new InvalidOperationException($"JSON data for material `{x.entity}` has no vertex or fragment properties");
                            }
                            else if (!hasVertexProperty)
                            {
                                throw new InvalidOperationException($"JSON data for material `{x.entity}` has no vertex property");
                            }
                            else
                            {
                                throw new InvalidOperationException($"JSON data for material `{x.entity}` has no fragment property");
                            }
                        }
                        else
                        {
                            continue; //waiting for data to become available
                        }
                    }

                    component.shaderReference = world.AddReference(x.entity, shader);
                }
            }
        }
    }
}