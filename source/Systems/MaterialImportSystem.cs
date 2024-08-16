using Data.Components;
using Data.Events;
using Rendering.Components;
using Shaders;
using Shaders.Components;
using Shaders.Events;
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
            Span<(FixedString name, FixedString value)> defaultValues = stackalloc (FixedString, FixedString)[4];
            bool askForData = false;
            foreach (var x in query)
            {
                ref IsMaterial component = ref x.Component1;
                if (component.shader == default)
                {
                    FixedString address = x.Component2.address;
                    if (!cachedShaders.TryGetValue(address, out Shader shader))
                    {
                        if (world.ContainsList<byte>(x.entity))
                        {
                            using BinaryReader reader = new(world.GetList<byte>(x.entity).AsSpan());
                            using JSONObject jsonObject = reader.ReadObject<JSONObject>();
                            bool hasVertexProperty = jsonObject.Contains("vertex");
                            bool hasFragmentProperty = jsonObject.Contains("fragment");
                            if (hasVertexProperty && hasFragmentProperty)
                            {
                                ReadOnlySpan<char> vertexAddress = jsonObject.GetText("vertex");
                                ReadOnlySpan<char> fragmentAddress = jsonObject.GetText("fragment");
                                eint vertexRequest = world.CreateEntity();
                                world.AddComponent(vertexRequest, new IsDataRequest(vertexAddress));
                                eint fragmentRequest = world.CreateEntity();
                                world.AddComponent(fragmentRequest, new IsDataRequest(fragmentAddress));
                                eint shaderEntity = world.CreateEntity();
                                world.AddComponent(shaderEntity, new IsShader(vertexRequest, fragmentRequest));
                                world.CreateList<ShaderPushConstant>(shaderEntity);
                                world.CreateList<ShaderVertexInputAttribute>(shaderEntity);
                                world.CreateList<ShaderUniformProperty>(shaderEntity);
                                world.CreateList<ShaderSamplerProperty>(shaderEntity);
                                askForData = true;
                                shader = new(world, shaderEntity);
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

                    component.shader = shader.GetEntityValue();
                }
            }

            if (askForData)
            {
                world.Submit(new DataUpdate());
                world.Submit(new ShaderUpdate());
                world.Poll();
            }
        }

        private int ReadDefaultPushBindings(eint dataEntity, Span<(FixedString name, FixedString value)> defaultValues)
        {
            int count = 0;
            using BinaryReader reader = new(world.GetList<byte>(dataEntity).AsSpan());
            using JSONObject jsonObject = reader.ReadObject<JSONObject>();
            if (jsonObject.Contains("pushBindings"))
            {
                JSONArray pushBindingsArray = jsonObject.GetArray("pushBindings");
                for (uint i = 0; i < pushBindingsArray.Count; i++)
                {
                    JSONObject pushBindingObject = pushBindingsArray[i].Object;
                    FixedString name = pushBindingObject.GetText("name");
                    FixedString defaultValue = pushBindingObject.GetText("default");
                    defaultValues[count++] = (name, defaultValue);
                }
            }

            return count;
        }
    }
}