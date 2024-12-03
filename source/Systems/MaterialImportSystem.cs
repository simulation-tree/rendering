using Collections;
using Data.Components;
using Rendering.Components;
using Shaders;
using Simulation;
using System;
using Unmanaged;
using Unmanaged.JSON;
using Worlds;

namespace Rendering.Systems
{
    public readonly partial struct MaterialImportSystem : ISystem
    {
        private readonly ComponentQuery<IsMaterial, IsDataRequest> query;
        private readonly Dictionary<FixedString, Shader> cachedShaders;

        void ISystem.Start(in SystemContainer systemContainer, in World world)
        {
        }

        void ISystem.Update(in SystemContainer systemContainer, in World world, in TimeSpan delta)
        {
            Update(world);
        }

        void ISystem.Finish(in SystemContainer systemContainer, in World world)
        {
            if (systemContainer.World == world)
            {
                CleanUp();
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
                        if (world.ContainsArray<BinaryData>(x.entity))
                        {
                            using BinaryReader reader = new(world.GetArray<BinaryData>(x.entity).As<byte>());
                            using JSONObject jsonObject = reader.ReadObject<JSONObject>();
                            bool hasVertexProperty = jsonObject.Contains("vertex");
                            bool hasFragmentProperty = jsonObject.Contains("fragment");
                            if (hasVertexProperty && hasFragmentProperty)
                            {
                                //todo: test materials and shaders loading from json
                                USpan<char> vertexAddress = jsonObject.GetText("vertex");
                                USpan<char> fragmentAddress = jsonObject.GetText("fragment");
                                shader = new(world, vertexAddress, fragmentAddress);
                                cachedShaders.TryAdd(address, shader);
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