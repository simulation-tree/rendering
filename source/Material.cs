using Data;
using Data.Components;
using Rendering.Components;
using Shaders;
using System;
using System.Diagnostics;
using System.Numerics;
using Unmanaged;
using Worlds;

namespace Rendering
{
    public readonly struct Material : IEntity
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
                ref IsMaterial component = ref entity.GetComponent<IsMaterial>();
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

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsMaterial>();
            archetype.AddArrayElementType<MaterialPushBinding>();
            archetype.AddArrayElementType<MaterialComponentBinding>();
            archetype.AddArrayElementType<MaterialTextureBinding>();
        }

#if NET
        [Obsolete("Default constructor not available", true)]
        public Material()
        {
            throw new NotSupportedException();
        }
#endif

        public Material(World world, uint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Material(World world, Shader shader)
        {
            entity = new Entity<IsMaterial>(world, new IsMaterial((rint)1));
            entity.AddReference(shader);
            entity.CreateArray<MaterialPushBinding>(0);
            entity.CreateArray<MaterialComponentBinding>(0);
            entity.CreateArray<MaterialTextureBinding>(0);
        }

        public Material(World world, Address address)
        {
            entity = new Entity<IsDataRequest, IsMaterial>(world, new(address), default);
            entity.CreateArray<MaterialPushBinding>(0);
            entity.CreateArray<MaterialComponentBinding>(0);
            entity.CreateArray<MaterialTextureBinding>(0);
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        /// <summary>
        /// Adds a binding that links a component on the render entity, to the shader.
        /// </summary>
        public readonly void AddPushBinding(DataType componentType, RenderStage stage = RenderStage.Vertex)
        {
            Schema schema = this.GetWorld().Schema;
            USpan<MaterialPushBinding> componentBindings = entity.GetArray<MaterialPushBinding>();
            uint start = 0;
            foreach (MaterialPushBinding existingBinding in componentBindings)
            {
                if (existingBinding.componentType == componentType)
                {
                    throw new InvalidOperationException($"Push binding `{componentType.ToString(schema)}` already exists on `{entity}`");
                }

                start += existingBinding.componentType.Size;
            }

            uint bindingCount = componentBindings.Length;
            componentBindings = entity.ResizeArray<MaterialPushBinding>(bindingCount + 1);
            componentBindings[bindingCount] = new(start, componentType, stage);
        }

        public readonly void AddPushBinding<C>(RenderStage stage = RenderStage.Vertex) where C : unmanaged
        {
            Schema schema = this.GetWorld().Schema;
            DataType componentType = schema.GetComponentDataType<C>();
            AddPushBinding(componentType, stage);
        }

        public readonly void SetPushBinding(DataType componentType, byte start, RenderStage stage = RenderStage.Vertex)
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

        public readonly void AddComponentBinding(byte binding, byte set, uint entity, DataType componentType, RenderStage stage = RenderStage.Vertex)
        {
            ThrowIfComponentBindingIsAlreadyPresent(binding, set, stage);

            DescriptorResourceKey key = new(binding, set);
            USpan<MaterialComponentBinding> componentBindings = this.entity.GetArray<MaterialComponentBinding>();
            uint bindingCount = componentBindings.Length;
            componentBindings = this.entity.ResizeArray<MaterialComponentBinding>(bindingCount + 1);
            componentBindings[bindingCount] = new(key, entity, componentType, stage);
        }

        public readonly void AddComponentBinding<E>(byte binding, byte set, E entity, DataType componentType, RenderStage stage = RenderStage.Vertex) where E : IEntity
        {
            AddComponentBinding(binding, set, entity.Value, componentType, stage);
        }

        public readonly void AddComponentBinding<C>(byte binding, byte set, uint entity, RenderStage stage = RenderStage.Vertex) where C : unmanaged
        {
            Schema schema = this.GetWorld().Schema;
            DataType componentType = schema.GetComponentDataType<C>();
            AddComponentBinding(binding, set, entity, componentType, stage);
        }

        public readonly void AddComponentBinding<C>(byte binding, byte set, Entity entity, RenderStage stage = RenderStage.Vertex) where C : unmanaged
        {
            Schema schema = this.GetWorld().Schema;
            DataType componentType = schema.GetComponentDataType<C>();
            AddComponentBinding(binding, set, entity, componentType, stage);
        }

        public readonly void SetComponentBinding(byte binding, byte set, uint entity, DataType componentType, RenderStage stage = RenderStage.Vertex)
        {
            ThrowIfComponentBindingIsMissing(binding, set, stage);

            DescriptorResourceKey key = new(binding, set);
            USpan<MaterialComponentBinding> componentBindings = this.entity.GetArray<MaterialComponentBinding>();
            for (uint i = 0; i < componentBindings.Length; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings[i];
                if (existingBinding.key.Equals(key) && existingBinding.stage == stage)
                {
                    existingBinding.componentType = componentType;
                    existingBinding.entity = entity;
                }
            }
        }

        public readonly void SetComponentBinding<C>(byte binding, byte set, uint entity, RenderStage stage = RenderStage.Vertex) where C : unmanaged
        {
            Schema schema = this.GetWorld().Schema;
            DataType componentType = schema.GetComponentDataType<C>();
            SetComponentBinding(binding, set, entity, componentType, stage);
        }

        public readonly void SetComponentBinding<C>(byte binding, byte set, Entity entity, RenderStage stage = RenderStage.Vertex) where C : unmanaged
        {
            Schema schema = this.GetWorld().Schema;
            DataType componentType = schema.GetComponentDataType<C>();
            SetComponentBinding(binding, set, entity.GetEntityValue(), componentType, stage);
        }

        public readonly bool TryRemoveComponentBinding(byte binding, byte set, RenderStage stage)
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

        public readonly void AddTextureBinding(byte binding, byte set, Entity texture, Vector4 region, TextureFiltering filtering = TextureFiltering.Linear)
        {
            DescriptorResourceKey key = new(binding, set);
            USpan<MaterialTextureBinding> textureBindings = entity.GetArray<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Length; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings[i];
                if (existingBinding.key.Equals(key))
                {
                    throw new InvalidOperationException($"Texture with binding `{binding}` already exists on `{entity}`");
                }
            }

            uint bindingCount = textureBindings.Length;
            textureBindings = entity.ResizeArray<MaterialTextureBinding>(bindingCount + 1);
            textureBindings[bindingCount] = new(0, key, texture, region, filtering);
        }

        public readonly void AddTextureBinding(byte binding, byte set, Entity texture, TextureFiltering filtering = TextureFiltering.Linear)
        {
            AddTextureBinding(binding, set, texture, new(0, 0, 1, 1), filtering);
        }

        public readonly bool SetTextureBinding(byte binding, byte set, Entity texture, Vector4 region, TextureFiltering filtering = TextureFiltering.Linear)
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

        public readonly void SetTextureRegion(Entity texture, Vector4 region)
        {
            USpan<MaterialTextureBinding> textureBindings = entity.GetArray<MaterialTextureBinding>();
            for (uint i = 0; i < textureBindings.Length; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings[i];
                if (existingBinding.TextureEntity == texture.GetEntityValue())
                {
                    existingBinding.SetRegion(region);
                    return;
                }
            }

            throw new InvalidOperationException($"Texture binding referencing texture `{texture}` does not exist to update region of");
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

            throw new InvalidOperationException($"Texture binding `{binding}` does not exist on `{entity}`");
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

            throw new InvalidOperationException($"Component binding `{binding}` does not exist on `{entity}`");
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

        [Conditional("DEBUG")]
        private readonly void ThrowIfComponentBindingIsMissing(byte binding, byte set, RenderStage stage)
        {
            DescriptorResourceKey key = new(binding, set);
            USpan<MaterialComponentBinding> componentBindings = entity.GetArray<MaterialComponentBinding>();
            for (uint i = 0; i < componentBindings.Length; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings[i];
                if (existingBinding.key == key && existingBinding.stage == stage)
                {
                    return;
                }
            }

            throw new InvalidOperationException($"Component binding `{binding}` does not exist on `{entity}`");
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfComponentBindingIsAlreadyPresent(byte binding, byte set, RenderStage stage)
        {
            DescriptorResourceKey key = new(binding, set);
            USpan<MaterialComponentBinding> componentBindings = entity.GetArray<MaterialComponentBinding>();
            for (uint i = 0; i < componentBindings.Length; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings[i];
                if (existingBinding.key == key && existingBinding.stage == stage)
                {
                    throw new InvalidOperationException($"Component binding `{binding}` already exists on `{entity}`");
                }
            }
        }

        public static implicit operator Entity(Material material)
        {
            return material.entity;
        }
    }
}
