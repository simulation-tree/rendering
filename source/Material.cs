using Data.Components;
using Rendering.Components;
using Shaders;
using Simulation;
using System;
using System.Numerics;
using Textures;
using Unmanaged;

namespace Rendering
{
    public readonly struct Material : IEntity
    {
        public readonly Entity entity;

        public readonly Shader Shader
        {
            get
            {
                IsMaterial component = entity.GetComponentRef<IsMaterial>();
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

        /// <summary>
        /// All values that bind an entity component to a uniform property.
        /// </summary>
        public readonly USpan<MaterialComponentBinding> ComponentBindings => entity.GetArray<MaterialComponentBinding>();

        /// <summary>
        /// All values that bind a texture entity to a sampler property.
        /// </summary>
        public readonly USpan<MaterialTextureBinding> TextureBindings => entity.GetArray<MaterialTextureBinding>();

        public readonly USpan<MaterialPushBinding> PushBindings => entity.GetArray<MaterialPushBinding>();

        readonly uint IEntity.Value => entity.value;
        readonly World IEntity.World => entity.world;
        readonly Definition IEntity.Definition => new Definition().AddComponentType<IsMaterial>().AddArrayTypes<MaterialPushBinding, MaterialComponentBinding, MaterialTextureBinding>();

#if NET
        [Obsolete("Default constructor not available", true)]
        public Material()
        {
            throw new InvalidOperationException("Cannot create a material without a world.");
        }
#endif

        public Material(World world, uint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Material(World world, Shader shader)
        {
            entity = new(world);
            rint materialReference = entity.AddReference(shader);
            entity.AddComponent(new IsMaterial(materialReference));
            entity.CreateArray<MaterialPushBinding>(0);
            entity.CreateArray<MaterialComponentBinding>(0);
            entity.CreateArray<MaterialTextureBinding>(0);
        }

        public Material(World world, USpan<char> address)
        {
            entity = new(world);
            entity.AddComponent(new IsMaterial(default));
            entity.AddComponent(new IsDataRequest(address));
            entity.CreateArray<MaterialPushBinding>(0);
            entity.CreateArray<MaterialComponentBinding>(0);
            entity.CreateArray<MaterialTextureBinding>(0);
        }

        public Material(World world, FixedString address)
        {
            entity = new(world);
            entity.AddComponent(new IsMaterial(default));
            entity.AddComponent(new IsDataRequest(address));
            entity.CreateArray<MaterialPushBinding>(0);
            entity.CreateArray<MaterialComponentBinding>(0);
            entity.CreateArray<MaterialTextureBinding>(0);
        }

        public Material(World world, string address)
        {
            entity = new(world);
            entity.AddComponent(new IsMaterial(default));
            entity.AddComponent(new IsDataRequest(address));
            entity.CreateArray<MaterialPushBinding>(0);
            entity.CreateArray<MaterialComponentBinding>(0);
            entity.CreateArray<MaterialTextureBinding>(0);
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        /// <summary>
        /// Adds a binding that links a component on the render entity, to the shader.
        /// </summary>
        public readonly void AddPushBinding(RuntimeType componentType, ShaderStage stage = ShaderStage.Vertex)
        {
            USpan<MaterialPushBinding> componentBindings = entity.GetArray<MaterialPushBinding>();
            uint start = 0;
            foreach (MaterialPushBinding existingBinding in componentBindings)
            {
                if (existingBinding.componentType == componentType)
                {
                    throw new InvalidOperationException($"Push binding `{componentType}` already exists on `{entity}`.");
                }

                start += existingBinding.componentType.Size;
            }

            uint bindingCount = componentBindings.Length;
            componentBindings = entity.ResizeArray<MaterialPushBinding>(bindingCount + 1);
            componentBindings[bindingCount] = new(start, componentType, stage);
        }

        public readonly void AddPushBinding<T>(ShaderStage stage = ShaderStage.Vertex) where T : unmanaged
        {
            RuntimeType componentType = RuntimeType.Get<T>();
            AddPushBinding(componentType, stage);
        }

        public readonly void SetPushBinding(RuntimeType componentType, byte start, ShaderStage stage = ShaderStage.Vertex)
        {
            USpan<MaterialPushBinding> componentBindings = entity.GetArray<MaterialPushBinding>();
            for (uint i = 0; i < componentBindings.Length; i++)
            {
                ref MaterialPushBinding existingBinding = ref componentBindings[i];
                if (existingBinding.componentType == componentType)
                {
                    existingBinding.start = start;
                    existingBinding.stage = stage;
                    return;
                }
            }

            //todo: qol: check if it overlaps with another push binding? but what if thats desired on purpose for unions?
            uint bindingCount = componentBindings.Length;
            componentBindings = entity.ResizeArray<MaterialPushBinding>(bindingCount + 1);
            componentBindings[bindingCount] = new(start, componentType, stage);
        }

        public readonly void AddComponentBinding(byte binding, byte set, uint entity, RuntimeType componentType, ShaderStage stage = ShaderStage.Vertex)
        {
            DescriptorResourceKey key = new(binding, set);
            USpan<MaterialComponentBinding> componentBindings = this.entity.GetArray<MaterialComponentBinding>();
            for (uint i = 0; i < componentBindings.Length; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings[i];
                if (existingBinding.key.Equals(key) && existingBinding.stage == stage)
                {
                    throw new InvalidOperationException($"Component with binding `{binding}` already exists on `{this.entity}`.");
                }
            }

            uint bindingCount = componentBindings.Length;
            componentBindings = this.entity.ResizeArray<MaterialComponentBinding>(bindingCount + 1);
            componentBindings[bindingCount] = new(key, entity, componentType, stage);
        }

        public readonly void AddComponentBinding<E>(byte binding, byte set, E entity, RuntimeType componentType, ShaderStage stage = ShaderStage.Vertex) where E : IEntity
        {
            AddComponentBinding(binding, set, entity.Value, componentType, stage);
        }

        public readonly void AddComponentBinding<C>(byte binding, byte set, uint entity, ShaderStage stage = ShaderStage.Vertex) where C : unmanaged
        {
            RuntimeType componentType = RuntimeType.Get<C>();
            AddComponentBinding(binding, set, entity, componentType, stage);
        }

        public readonly void AddComponentBinding<C>(byte binding, byte set, Entity entity, ShaderStage stage = ShaderStage.Vertex) where C : unmanaged
        {
            RuntimeType componentType = RuntimeType.Get<C>();
            AddComponentBinding(binding, set, entity, componentType, stage);
        }

        public readonly bool SetComponentBinding(byte binding, byte set, uint entity, RuntimeType componentType, ShaderStage stage = ShaderStage.Vertex)
        {
            DescriptorResourceKey key = new(binding, set);
            USpan<MaterialComponentBinding> componentBindings = this.entity.GetArray<MaterialComponentBinding>();
            for (uint i = 0; i < componentBindings.Length; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings[i];
                if (existingBinding.key.Equals(key) && existingBinding.stage == stage)
                {
                    existingBinding.componentType = componentType;
                    existingBinding.entity = entity;
                    return true;
                }
            }

            //todo: why the add here? when the Add function is already present
            uint bindingCount = componentBindings.Length;
            componentBindings = this.entity.ResizeArray<MaterialComponentBinding>(bindingCount + 1);
            componentBindings[bindingCount] = new(key, entity, componentType, stage);
            return false;
        }

        public readonly bool SetComponentBinding<C>(byte binding, byte set, uint entity, ShaderStage stage = ShaderStage.Vertex) where C : unmanaged
        {
            RuntimeType componentType = RuntimeType.Get<C>();
            return SetComponentBinding(binding, set, entity, componentType, stage);
        }

        public readonly bool SetComponentBinding<C>(byte binding, byte set, Entity entity, ShaderStage stage = ShaderStage.Vertex) where C : unmanaged
        {
            RuntimeType componentType = RuntimeType.Get<C>();
            return SetComponentBinding(binding, set, entity.GetEntityValue(), componentType, stage);
        }

        public readonly bool RemoveComponentBinding(byte binding, byte set, ShaderStage stage)
        {
            DescriptorResourceKey key = new(binding, set);
            USpan<MaterialComponentBinding> componentBindings = entity.GetArray<MaterialComponentBinding>();
            for (uint i = 0; i < componentBindings.Length; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings[i];
                if (existingBinding.key.Equals(key) && existingBinding.stage == stage)
                {
                    //move last element into this index
                    uint lastIndex = componentBindings.Length - 1;
                    ref MaterialComponentBinding lastBinding = ref componentBindings[lastIndex];
                    existingBinding = lastBinding;
                    this.entity.ResizeArray<MaterialComponentBinding>(lastIndex);
                    return true;
                }
            }

            return false;
        }

        public readonly void AddTextureBinding(byte binding, byte set, Texture texture, Vector4 region, TextureFiltering filtering = TextureFiltering.Linear)
        {
            DescriptorResourceKey key = new(binding, set);
            USpan<MaterialTextureBinding> textureBindings = entity.GetArray<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Length; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings[i];
                if (existingBinding.key.Equals(key))
                {
                    throw new InvalidOperationException($"Texture with binding `{binding}` already exists on `{entity}`.");
                }
            }

            uint bindingCount = textureBindings.Length;
            textureBindings = entity.ResizeArray<MaterialTextureBinding>(bindingCount + 1);
            textureBindings[bindingCount] = new(0, key, texture, region, filtering);
        }

        public readonly void AddTextureBinding(byte binding, byte set, Texture texture, TextureFiltering filtering = TextureFiltering.Linear)
        {
            AddTextureBinding(binding, set, texture, new(0, 0, 1, 1), filtering);
        }

        public readonly bool SetTextureBinding(byte binding, byte set, Texture texture, Vector4 region, TextureFiltering filtering = TextureFiltering.Linear)
        {
            DescriptorResourceKey key = new(binding, set);
            USpan<MaterialTextureBinding> textureBindings = entity.GetArray<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Length; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings[i];
                if (existingBinding.key.Equals(key))
                {
                    existingBinding.SetTexture(texture);
                    existingBinding.SetRegion(region);
                    existingBinding.SetFiltering(filtering);
                    return true;
                }
            }

            AddTextureBinding(binding, set, texture, region);
            return false;
        }

        public readonly void SetTextureRegion(Texture texture, Vector4 region)
        {
            USpan<MaterialTextureBinding> textureBindings = entity.GetArray<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Length; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings[i];
                if (existingBinding.TextureEntity == texture.entity.value)
                {
                    existingBinding.SetRegion(region);
                    return;
                }
            }

            throw new InvalidOperationException($"Texture binding referencing texture `{texture}` does not exist to update region of.");
        }

        public readonly bool TryGetTextureBinding(uint textureEntity, out MaterialTextureBinding binding)
        {
            USpan<MaterialTextureBinding> textureBindings = entity.GetArray<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Length; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings[i];
                if (existingBinding.TextureEntity == textureEntity)
                {
                    binding = existingBinding;
                    return true;
                }
            }

            binding = default;
            return false;
        }

        public readonly bool TryGetTextureBinding(byte binding, byte set, out uint index)
        {
            DescriptorResourceKey key = new(binding, set);
            USpan<MaterialTextureBinding> textureBindings = entity.GetArray<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Length; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings[i];
                if (existingBinding.key.Equals(key))
                {
                    index = i;
                    return true;
                }
            }

            index = default;
            return false;
        }

        public readonly ref MaterialTextureBinding GetTextureBindingRef(byte binding, byte set)
        {
            DescriptorResourceKey key = new(binding, set);
            USpan<MaterialTextureBinding> textureBindings = entity.GetArray<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Length; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings[i];
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
            USpan<MaterialComponentBinding> componentBindings = entity.GetArray<MaterialComponentBinding>();
            for (uint i = 0; i < componentBindings.Length; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings[i];
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
            USpan<MaterialTextureBinding> textureBindings = entity.GetArray<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Length; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings[i];
                if (existingBinding.key.Equals(key))
                {
                    uint lastIndex = textureBindings.Length - 1;
                    ref MaterialTextureBinding lastBinding = ref textureBindings[lastIndex];
                    existingBinding = lastBinding;
                    entity.ResizeArray<MaterialTextureBinding>(lastIndex);
                    return true;
                }
            }

            return false;
        }

        public unsafe readonly ref MaterialComponentBinding TryGetProperty(ShaderUniformProperty uniform, out bool contains)
        {
            USpan<MaterialComponentBinding> componentBindings = entity.GetArray<MaterialComponentBinding>();
            for (uint i = 0; i < componentBindings.Length; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings[i];
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
            USpan<MaterialTextureBinding> textureBindings = entity.GetArray<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Length; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings[i];
                if (existingBinding.key == sampler.key)
                {
                    contains = true;
                    return ref existingBinding;
                }
            }

            contains = false;
            return ref *(MaterialTextureBinding*)null;
        }
    }
}
