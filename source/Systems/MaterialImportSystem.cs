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
        private readonly Dictionary<FixedString, Shader> cachedShaders;

        public MaterialImportSystem()
        {
            cachedShaders = new();
        }

        void ISystem.Start(in SystemContainer systemContainer, in World world)
        {
        }

        void ISystem.Update(in SystemContainer systemContainer, in World world, in TimeSpan delta)
        {
            LoadMaterials(world);
        }

        void ISystem.Finish(in SystemContainer systemContainer, in World world)
        {
        }

        void IDisposable.Dispose()
        {
            cachedShaders.Dispose();
        }

        private readonly void LoadMaterials(World world)
        {
            ComponentQuery<IsMaterial, IsDataRequest> requestQuery = new(world);
            foreach (var r in requestQuery)
            {
                ref IsMaterial component = ref r.component1;
                ref IsDataRequest request = ref r.component2;
                uint entity = r.entity;
                if (component.shaderReference == default)
                {
                    FixedString address = request.address;
                    if (!cachedShaders.TryGetValue(address, out Shader shader))
                    {
                        if (world.ContainsArray<BinaryData>(entity))
                        {
                            using BinaryReader reader = new(world.GetArray<BinaryData>(entity).As<byte>());
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
                                throw new InvalidOperationException($"JSON data for material `{entity}` has no vertex or fragment properties");
                            }
                            else if (!hasVertexProperty)
                            {
                                throw new InvalidOperationException($"JSON data for material `{entity}` has no vertex property");
                            }
                            else
                            {
                                throw new InvalidOperationException($"JSON data for material `{entity}` has no fragment property");
                            }
                        }
                        else
                        {
                            return; //waiting for data to become available
                        }
                    }

                    component.shaderReference = world.AddReference(entity, shader);
                }
            }
        }
    }
}