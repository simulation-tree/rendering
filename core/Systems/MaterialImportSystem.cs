using Collections;
using Data;
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
        private readonly Dictionary<ShaderKey, Shader> cachedShaders;

        private MaterialImportSystem(Dictionary<ShaderKey, Shader> cachedShaders)
        {
            this.cachedShaders = cachedShaders;
        }

        void ISystem.Start(in SystemContainer systemContainer, in World world)
        {
            if (systemContainer.World == world)
            {
                Dictionary<ShaderKey, Shader> cachedShaders = new();
                systemContainer.Write(new MaterialImportSystem(cachedShaders));
            }
        }

        void ISystem.Update(in SystemContainer systemContainer, in World world, in TimeSpan delta)
        {
            LoadMaterials(world);
        }

        void ISystem.Finish(in SystemContainer systemContainer, in World world)
        {
            if (systemContainer.World == world)
            {
                cachedShaders.Dispose();
            }
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
                    Address address = request.address;
                    ShaderKey key = new(address.value, world);
                    if (!cachedShaders.TryGetValue(key, out Shader shader))
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
                                Address vertexAddress = new(jsonObject.GetText("vertex"));
                                Address fragmentAddress = new(jsonObject.GetText("fragment"));
                                shader = new(world, vertexAddress, fragmentAddress);
                                cachedShaders.Add(key, shader);
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

        public readonly struct ShaderKey : IEquatable<ShaderKey>
        {
            public readonly FixedString address;
            public readonly World world;

            public ShaderKey(FixedString address, World world)
            {
                this.address = address;
                this.world = world;
            }

            public readonly override bool Equals(object? obj)
            {
                return obj is ShaderKey key && Equals(key);
            }

            public readonly bool Equals(ShaderKey other)
            {
                return address.Equals(other.address) && world.Equals(other.world);
            }

            public readonly override int GetHashCode()
            {
                return HashCode.Combine(address, world);
            }

            public static bool operator ==(ShaderKey left, ShaderKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ShaderKey left, ShaderKey right)
            {
                return !(left == right);
            }
        }
    }
}