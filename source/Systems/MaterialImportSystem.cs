using Data.Components;
using Data.Events;
using Rendering.Components;
using Shaders;
using Simulation;
using System;
using Unmanaged;
using Unmanaged.Collections;
using Unmanaged.JSON;

namespace Rendering.Systems
{
    public class MaterialImportSystem : SystemBase
    {
        private readonly Query<IsMaterial, IsDataRequest> query;
        private readonly UnmanagedDictionary<FixedString, Shader> cachedShaders;

        public MaterialImportSystem(World world) : base(world)
        {
            query = new(world);
            cachedShaders = new();
            Subscribe<DataUpdate>(OnUpdate);
        }

        public override void Dispose()
        {
            query.Dispose();
            cachedShaders.Dispose();
            base.Dispose();
        }

        private void OnUpdate(DataUpdate update)
        {
            query.Update();
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
                                ReadOnlySpan<char> vertexAddress = jsonObject.GetText("vertex");
                                ReadOnlySpan<char> fragmentAddress = jsonObject.GetText("fragment");
                                shader = new(world, vertexAddress, fragmentAddress);
                                cachedShaders.Add(address, shader);
                            }
                            else if (!hasVertexProperty && !hasFragmentProperty)
                            {
                                throw new InvalidOperationException($"JSON data for material `{x.entity}` has no vertex or fragment properties.");
                            }
                            else if (!hasVertexProperty)
                            {
                                throw new InvalidOperationException($"JSON data for material `{x.entity}` has no vertex property.");
                            }
                            else
                            {
                                throw new InvalidOperationException($"JSON data for material `{x.entity}` has no fragment property.");
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