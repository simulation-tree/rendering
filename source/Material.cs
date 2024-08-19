using Data.Components;
using Data.Events;
using Rendering.Components;
using Shaders;
using Simulation;
using System;
using System.Numerics;
using Textures;
using Unmanaged;
using Unmanaged.Collections;

namespace Rendering
{
    public readonly struct Material : IMaterial, IDisposable
    {
        private readonly Entity entity;

        public readonly Shader Shader
        {
            get
            {
                IsMaterial component = entity.GetComponent<IsMaterial>();
                return new(entity.world, entity.GetReference(component.shaderReference));
            }
            set
            {
                ref IsMaterial component = ref entity.GetComponentRef<IsMaterial>();
                if (entity.ContainsReference(component.shaderReference))
                {
                    entity.SetReference(component.shaderReference, value);
                }
                else
                {
                    component.shaderReference = entity.AddReference(value);
                }
            }
        }

        eint IEntity.Value => entity.value;
        World IEntity.World => entity.world;

#if NET
        [Obsolete("Default constructor not available", true)]
        public Material()
        {
            throw new InvalidOperationException("Cannot create a material without a world.");
        }
#endif

        public Material(World world, eint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Material(World world, Shader shader)
        {
            entity = new(world);
            rint materialReference = entity.AddReference(shader);
            entity.AddComponent(new IsMaterial(materialReference));
            entity.CreateList<MaterialPushBinding>();
            entity.CreateList<MaterialComponentBinding>();
            entity.CreateList<MaterialTextureBinding>();
        }

        public Material(World world, ReadOnlySpan<char> address)
        {
            entity = new(world);
            entity.AddComponent(new IsMaterial(default));
            entity.AddComponent(new IsDataRequest(address));
            entity.CreateList<MaterialPushBinding>();
            entity.CreateList<MaterialComponentBinding>();
            entity.CreateList<MaterialTextureBinding>();
        }

        public Material(World world, FixedString address)
        {
            entity = new(world);
            entity.AddComponent(new IsMaterial(default));
            entity.AddComponent(new IsDataRequest(address));
            entity.CreateList<MaterialPushBinding>();
            entity.CreateList<MaterialComponentBinding>();
            entity.CreateList<MaterialTextureBinding>();
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        readonly Query IEntity.GetQuery(World world)
        {
            return new(world, RuntimeType.Get<IsMaterial>());
        }

        public readonly bool IsRequesting()
        {
            return entity.ContainsComponent<IsDataRequest>();
        }

        public readonly FixedString GetRequestAddress()
        {
            return entity.GetComponent<IsDataRequest>().address;
        }

        /// <summary>
        /// All values that bind an entity component to a uniform property.
        /// </summary>
        public readonly ReadOnlySpan<MaterialComponentBinding> GetComponentBindings()
        {
            UnmanagedList<MaterialComponentBinding> bindings = entity.GetList<MaterialComponentBinding>();
            return bindings.AsSpan();
        }

        /// <summary>
        /// All values that bind a texture entity to a sampler property.
        /// </summary>
        public readonly ReadOnlySpan<MaterialTextureBinding> GetTextureBindings()
        {
            UnmanagedList<MaterialTextureBinding> bindings = entity.GetList<MaterialTextureBinding>();
            return bindings.AsSpan();
        }

        public readonly ReadOnlySpan<MaterialPushBinding> GetPushBindings()
        {
            UnmanagedList<MaterialPushBinding> bindings = entity.GetList<MaterialPushBinding>();
            return bindings.AsSpan();
        }

        /// <summary>
        /// Adds a binding that links a component on the render entity, to the shader.
        /// </summary>
        public readonly void AddPushBinding(RuntimeType componentType, ShaderStage stage = ShaderStage.Vertex)
        {
            UnmanagedList<MaterialPushBinding> componentBindings = entity.GetList<MaterialPushBinding>();
            uint start = 0;
            foreach (MaterialPushBinding existingBinding in componentBindings)
            {
                if (existingBinding.componentType == componentType)
                {
                    throw new InvalidOperationException($"Push binding `{componentType}` already exists on `{entity}`.");
                }

                start += existingBinding.componentType.Size;
            }

            componentBindings.Add(new(start, componentType, stage));
        }

        public readonly void SetPushBinding(RuntimeType componentType, byte start, ShaderStage stage = ShaderStage.Vertex)
        {
            UnmanagedList<MaterialPushBinding> componentBindings = entity.GetList<MaterialPushBinding>();
            for (uint i = 0; i < componentBindings.Count; i++)
            {
                ref MaterialPushBinding existingBinding = ref componentBindings.GetRef(i);
                if (existingBinding.componentType == componentType)
                {
                    existingBinding.start = start;
                    existingBinding.stage = stage;
                    return;
                }
            }

            //todo: qol: check if it overlaps with another push binding? but what if thats desired on purpose for unions?
            componentBindings.Add(new(start, componentType, stage));
        }

        public readonly void AddComponentBinding(byte binding, byte set, eint entity, RuntimeType componentType, ShaderStage stage = ShaderStage.Vertex)
        {
            DescriptorResourceKey key = new(binding, set);
            UnmanagedList<MaterialComponentBinding> componentBindings = this.entity.GetList<MaterialComponentBinding>();
            for (uint i = 0; i < componentBindings.Count; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings.GetRef(i);
                if (existingBinding.key.Equals(key) && existingBinding.stage == stage)
                {
                    throw new InvalidOperationException($"Component with binding `{binding}` already exists on `{this.entity}`.");
                }
            }

            componentBindings.Add(new(key, entity, componentType, stage));
        }

        public readonly void AddComponentBinding<T>(byte binding, byte set, T entity, RuntimeType componentType, ShaderStage stage = ShaderStage.Vertex) where T : IEntity
        {
            AddComponentBinding(binding, set, entity.Value, componentType, stage);
        }

        public readonly void AddComponentBinding<C>(byte binding, byte set, eint entity, ShaderStage stage = ShaderStage.Vertex) where C : unmanaged
        {
            RuntimeType componentType = RuntimeType.Get<C>();
            AddComponentBinding(binding, set, entity, componentType, stage);
        }

        public readonly bool SetComponentBinding(byte binding, byte set, eint entity, RuntimeType componentType, ShaderStage stage = ShaderStage.Vertex)
        {
            DescriptorResourceKey key = new(binding, set);
            UnmanagedList<MaterialComponentBinding> componentBindings = this.entity.GetList<MaterialComponentBinding>();
            for (uint i = 0; i < componentBindings.Count; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings.GetRef(i);
                if (existingBinding.key.Equals(key) && existingBinding.stage == stage)
                {
                    existingBinding.componentType = componentType;
                    return true;
                }
            }

            componentBindings.Add(new(key, entity, componentType, stage));
            return false;
        }

        public readonly bool SetComponentBinding<C>(byte binding, byte set, eint entity, ShaderStage stage = ShaderStage.Vertex) where C : unmanaged
        {
            RuntimeType componentType = RuntimeType.Get<C>();
            return SetComponentBinding(binding, set, entity, componentType, stage);
        }

        public readonly bool RemoveComponentBinding(byte binding, byte set, ShaderStage stage)
        {
            DescriptorResourceKey key = new(binding, set);
            UnmanagedList<MaterialComponentBinding> componentBindings = entity.GetList<MaterialComponentBinding>();
            for (uint i = 0; i < componentBindings.Count; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings.GetRef(i);
                if (existingBinding.key.Equals(key) && existingBinding.stage == stage)
                {
                    componentBindings.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public readonly void AddTextureBinding(byte binding, byte set, Texture texture, Vector4 region)
        {
            DescriptorResourceKey key = new(binding, set);
            UnmanagedList<MaterialTextureBinding> textureBindings = entity.GetList<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Count; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
                if (existingBinding.key.Equals(key))
                {
                    throw new InvalidOperationException($"Texture with binding `{binding}` already exists on `{entity}`.");
                }
            }

            textureBindings.Add(new(0, key, texture, region));
        }

        public readonly void AddTextureBinding(byte binding, byte set, Texture texture)
        {
            AddTextureBinding(binding, set, texture, new(0, 0, 1, 1));
        }

        public readonly bool SetTextureBinding(byte binding, byte set, Texture texture, Vector4 region)
        {
            DescriptorResourceKey key = new(binding, set);
            UnmanagedList<MaterialTextureBinding> textureBindings = entity.GetList<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Count; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
                if (existingBinding.key.Equals(key))
                {
                    existingBinding.SetTexture(texture);
                    existingBinding.SetRegion(region);
                    return true;
                }
            }

            AddTextureBinding(binding, set, texture, region);
            return false;
        }

        public readonly void SetTextureRegion(Texture texture, Vector4 region)
        {
            UnmanagedList<MaterialTextureBinding> textureBindings = entity.GetList<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Count; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
                if (existingBinding.TextureEntity == ((Entity)texture).value)
                {
                    existingBinding.SetRegion(region);
                    return;
                }
            }

            throw new InvalidOperationException($"Texture binding referencing texture `{texture}` does not exist to update region of.");
        }

        public readonly bool TryGetTextureBinding(eint texture, out MaterialTextureBinding binding)
        {
            UnmanagedList<MaterialTextureBinding> textureBindings = entity.GetList<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Count; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
                if (existingBinding.TextureEntity == texture)
                {
                    binding = existingBinding;
                    return true;
                }
            }

            binding = default;
            return false;
        }

        public readonly bool TryGetTextureBinding(byte binding, byte set, out MaterialTextureBinding textureBinding)
        {
            DescriptorResourceKey key = new(binding, set);
            UnmanagedList<MaterialTextureBinding> textureBindings = entity.GetList<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Count; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
                if (existingBinding.key.Equals(key))
                {
                    textureBinding = existingBinding;
                    return true;
                }
            }

            textureBinding = default;
            return false;
        }

        public readonly ref MaterialTextureBinding GetTextureBindingRef(byte binding, byte set)
        {
            DescriptorResourceKey key = new(binding, set);
            UnmanagedList<MaterialTextureBinding> textureBindings = entity.GetList<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Count; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
                if (existingBinding.key == key)
                {
                    return ref existingBinding;
                }
            }

            throw new InvalidOperationException($"Texture binding `{binding}` does not exist on `{entity}`.");
        }

        public readonly ref MaterialComponentBinding GetComponentBindingRef(byte binding, byte set)
        {
            DescriptorResourceKey key = new(binding, set);
            UnmanagedList<MaterialComponentBinding> componentBindings = entity.GetList<MaterialComponentBinding>();
            for (uint i = 0; i < componentBindings.Count; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings.GetRef(i);
                if (existingBinding.key == key)
                {
                    return ref existingBinding;
                }
            }

            throw new InvalidOperationException($"Component binding `{binding}` does not exist on `{entity}`.");
        }

        public readonly bool RemoveTextureBinding(byte binding, byte set)
        {
            DescriptorResourceKey key = new(binding, set);
            UnmanagedList<MaterialTextureBinding> textureBindings = entity.GetList<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Count; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
                if (existingBinding.key.Equals(key))
                {
                    textureBindings.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public unsafe readonly ref MaterialComponentBinding TryGetProperty(ShaderUniformProperty uniform, out bool contains)
        {
            UnmanagedList<MaterialComponentBinding> componentBindings = entity.GetList<MaterialComponentBinding>();
            for (uint i = 0; i < componentBindings.Count; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings.GetRef(i);
                if (existingBinding.key == uniform.key)
                {
                    if (existingBinding.componentType.Size == uniform.size)
                    {
                        contains = true;
                        return ref existingBinding;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Component binding `{existingBinding.key}` found but has mismatching size, expected {uniform.size} but was {existingBinding.componentType.Size}.");
                    }
                }
            }

            contains = false;
            return ref *(MaterialComponentBinding*)null;
        }

        public unsafe readonly ref MaterialTextureBinding TryGetProperty(ShaderSamplerProperty sampler, out bool contains)
        {
            UnmanagedList<MaterialTextureBinding> textureBindings = entity.GetList<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Count; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
                if (existingBinding.key == sampler.key)
                {
                    contains = true;
                    return ref existingBinding;
                }
            }

            contains = false;
            return ref *(MaterialTextureBinding*)null;
        }

        public static implicit operator Entity(Material material)
        {
            return material.entity;
        }
    }
}
