using Game;
using Shaders;
using System;
using System.Numerics;
using Textures;
using Unmanaged;
using Unmanaged.Collections;

namespace Materials
{
    public readonly struct Material : IDisposable
    {
        public readonly Entity entity;

        private readonly UnmanagedList<MaterialComponentBinding> componentBindings;
        private readonly UnmanagedList<MaterialTextureBinding> textureBindings;

        public readonly Shader Shader
        {
            get => new(entity.world, entity.GetComponent<IsMaterial>().shader);
            set
            {
                ref IsMaterial material = ref entity.GetComponentRef<IsMaterial>();
                material.shader = value.entity;
            }
        }

        public readonly ReadOnlySpan<MaterialComponentBinding> ComponentBindings => componentBindings.AsSpan();
        public readonly ReadOnlySpan<MaterialTextureBinding> TextureBindings => textureBindings.AsSpan();

        public Material()
        {
            throw new InvalidOperationException("Cannot create a material without a world.");
        }

        public Material(World world, EntityID existingEntity)
        {
            entity = new(world, existingEntity);
            componentBindings = entity.GetCollection<MaterialComponentBinding>();
            textureBindings = entity.GetCollection<MaterialTextureBinding>();
        }

        public Material(World world, Shader shader)
        {
            entity = new(world);
            entity.AddComponent(new IsMaterial(shader.entity));
            componentBindings = entity.CreateCollection<MaterialComponentBinding>();
            textureBindings = entity.CreateCollection<MaterialTextureBinding>();
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        public readonly void AddComponentBinding(ResourceKey key, ShaderStage stage, RuntimeType componentType)
        {
            for (uint i = 0; i < componentBindings.Count; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings.GetRef(i);
                if (existingBinding.key == key && existingBinding.stage == stage)
                {
                    throw new InvalidOperationException($"Component binding {key} already exists.");
                }
            }

            componentBindings.Add(new(key, stage, componentType));
        }

        public readonly void AddComponentBinding<T>(ResourceKey key, ShaderStage stage) where T : unmanaged
        {
            RuntimeType componentType = RuntimeType.Get<T>();
            AddComponentBinding(key, stage, componentType);
        }

        public readonly bool SetComponentBinding(ResourceKey key, RuntimeType componentType, ShaderStage stage)
        {
            for (uint i = 0; i < componentBindings.Count; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings.GetRef(i);
                if (existingBinding.key == key && existingBinding.stage == stage)
                {
                    existingBinding.componentType = componentType;
                    return true;
                }
            }

            AddComponentBinding(key, stage, componentType);
            return false;
        }

        public readonly bool SetComponentBinding<T>(ResourceKey key, ShaderStage stage) where T : unmanaged
        {
            RuntimeType componentType = RuntimeType.Get<T>();
            return SetComponentBinding(key, componentType, stage);
        }

        public readonly bool RemoveComponentBinding(ResourceKey key, ShaderStage stage)
        {
            for (uint i = 0; i < componentBindings.Count; i++)
            {
                ref MaterialComponentBinding existingBinding = ref componentBindings.GetRef(i);
                if (existingBinding.key == key && existingBinding.stage == stage)
                {
                    componentBindings.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public readonly void AddTextureBinding(ResourceKey key, Texture texture)
        {
            AddTextureBinding(key, texture, new(0, 0, 1, 1));
        }

        public readonly void AddTextureBinding(ResourceKey key, Texture texture, Vector4 region)
        {
            for (uint i = 0; i < textureBindings.Count; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
                if (existingBinding.key == key)
                {
                    throw new InvalidOperationException($"Texture binding {key} already exists.");
                }
            }

            textureBindings.Add(new(key, texture.entity, region));
        }

        public readonly void SetTextureBinding(ResourceKey key, Texture texture)
        {
            SetTextureBinding(key, texture, new(0, 0, 1, 1));
        }

        public readonly void SetTextureBinding(ResourceKey key, Texture texture, Vector4 region)
        {
            for (uint i = 0; i < textureBindings.Count; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
                if (existingBinding.key == key)
                {
                    existingBinding.texture = texture.entity;
                    existingBinding.region = region;
                    return;
                }
            }

            AddTextureBinding(key, texture, region);
        }

        public readonly bool RemoveTextureBinding(ResourceKey key)
        {
            for (uint i = 0; i < textureBindings.Count; i++)
            {
                ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
                if (existingBinding.key == key)
                {
                    textureBindings.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public readonly ref MaterialComponentBinding TryGetProperty(ShaderUniformProperty uniform, out bool contains)
        {
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
                        throw new InvalidOperationException($"Component binding {existingBinding.key} has an invalid size.");
                    }
                }
            }

            contains = false;
            return ref System.Runtime.CompilerServices.Unsafe.NullRef<MaterialComponentBinding>();
        }

        public readonly ref MaterialTextureBinding TryGetProperty(ShaderSamplerProperty sampler, out bool contains)
        {
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
            return ref System.Runtime.CompilerServices.Unsafe.NullRef<MaterialTextureBinding>();
        }
    }
}
