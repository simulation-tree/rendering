using Materials;
using Rendering;
using Rendering.Components;
using Shaders;
using Simulation;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Textures;
using Unmanaged;
using Unmanaged.Collections;

public static class MaterialFunctions
{
    public static Shader GetShader<T>(this T material) where T : IMaterial
    {
        IsMaterial component = material.GetComponent<T, IsMaterial>();
        return new(material.World, component.shader);
    }

    public static void SetShader<T>(this T material, Shader shader) where T : IMaterial
    {
        ref IsMaterial component = ref material.GetComponentRef<T, IsMaterial>();
        component = new(shader);
    }

    /// <summary>
    /// All values that bind an entity component to a uniform property.
    /// </summary>
    public static ReadOnlySpan<MaterialComponentBinding> GetComponentBindings<T>(this T material) where T : IMaterial
    {
        UnmanagedList<MaterialComponentBinding> bindings = material.GetList<T, MaterialComponentBinding>();
        return bindings.AsSpan();
    }

    /// <summary>
    /// All values that bind a texture entity to a sampler property.
    /// </summary>
    public static ReadOnlySpan<MaterialTextureBinding> GetTextureBindings<T>(this T material) where T : IMaterial
    {
        UnmanagedList<MaterialTextureBinding> bindings = material.GetList<T, MaterialTextureBinding>();
        return bindings.AsSpan();
    }

    public static void AddComponentBinding<T>(this T material, DescriptorResourceKey key, eint entity, RuntimeType componentType, ShaderStage stage) where T : IMaterial
    {
        UnmanagedList<MaterialComponentBinding> componentBindings = material.GetList<T, MaterialComponentBinding>();
        for (uint i = 0; i < componentBindings.Count; i++)
        {
            ref MaterialComponentBinding existingBinding = ref componentBindings.GetRef(i);
            if (existingBinding.key == key && existingBinding.stage == stage)
            {
                throw new InvalidOperationException($"Component binding `{key}` already exists on `{material}`.");
            }
        }

        componentBindings.Add(new(key, entity, componentType, stage));
    }

    public static void AddComponentBinding<T, C>(this T material, DescriptorResourceKey key, eint entity, ShaderStage stage) where T : IMaterial where C : unmanaged
    {
        RuntimeType componentType = RuntimeType.Get<C>();
        AddComponentBinding(material, key, entity, componentType, stage);
    }

    public static void AddComponentBinding<T>(this T material, byte binding, byte set, eint entity, RuntimeType componentType, ShaderStage stage) where T : IMaterial
    {
        AddComponentBinding(material, new(binding, set), entity, componentType, stage);
    }

    public static void AddComponentBinding<T, E>(this T material, byte binding, byte set, E entity, RuntimeType componentType, ShaderStage stage) where T : IMaterial where E : IEntity
    {
        AddComponentBinding(material, new(binding, set), entity.Value, componentType, stage);
    }

    public static void AddComponentBinding<T, C>(this T material, byte binding, byte set, eint entity, ShaderStage stage) where T : IMaterial where C : unmanaged
    {
        RuntimeType componentType = RuntimeType.Get<C>();
        AddComponentBinding(material, new(binding, set), entity, componentType, stage);
    }

    public static bool SetComponentBinding<T>(this T material, DescriptorResourceKey key, eint entity, RuntimeType componentType, ShaderStage stage) where T : IMaterial
    {
        UnmanagedList<MaterialComponentBinding> componentBindings = material.GetList<T, MaterialComponentBinding>();
        for (uint i = 0; i < componentBindings.Count; i++)
        {
            ref MaterialComponentBinding existingBinding = ref componentBindings.GetRef(i);
            if (existingBinding.key == key && existingBinding.stage == stage)
            {
                existingBinding.componentType = componentType;
                return true;
            }
        }

        AddComponentBinding<T>(material, key, entity, componentType, stage);
        return false;
    }

    public static bool SetComponentBinding<T, C>(this T material, DescriptorResourceKey key, eint entity, ShaderStage stage) where T : IMaterial where C : unmanaged
    {
        RuntimeType componentType = RuntimeType.Get<C>();
        return SetComponentBinding(material, key, entity, componentType, stage);
    }

    public static bool RemoveComponentBinding<T>(this T material, DescriptorResourceKey key, ShaderStage stage) where T : IMaterial
    {
        UnmanagedList<MaterialComponentBinding> componentBindings = material.GetList<T, MaterialComponentBinding>();
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

    public static void AddTextureBinding<T>(this T material, DescriptorResourceKey key, Texture texture) where T : IMaterial
    {
        AddTextureBinding(material, key, texture, new(0, 0, 1, 1));
    }

    public static void AddTextureBinding<T>(this T material, DescriptorResourceKey key, Texture texture, Vector4 region) where T : IMaterial
    {
        UnmanagedList<MaterialTextureBinding> textureBindings = material.GetList<T, MaterialTextureBinding>();
        for (uint i = 0; i < textureBindings.Count; i++)
        {
            ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
            if (existingBinding.key == key)
            {
                throw new InvalidOperationException($"Texture binding `{key}` already exists on `{material}`.");
            }
        }

        textureBindings.Add(new(key, texture.GetEntityValue(), region));
    }

    public static void AddTextureBinding<T>(this T material, byte binding, byte set, Texture texture) where T : IMaterial
    {
        AddTextureBinding(material, new(binding, set), texture);
    }

    public static void AddTextureBinding<T>(this T material, byte binding, byte set, Texture texture, Vector4 region) where T : IMaterial
    {
        AddTextureBinding(material, new(binding, set), texture, region);
    }

    public static bool SetTextureBinding<T>(this T material, DescriptorResourceKey key, Texture texture) where T : IMaterial
    {
        return SetTextureBinding(material, key, texture, new(0, 0, 1, 1));
    }

    public static bool SetTextureBinding<T>(this T material, DescriptorResourceKey key, Texture texture, Vector4 region) where T : IMaterial
    {
        UnmanagedList<MaterialTextureBinding> textureBindings = material.GetList<T, MaterialTextureBinding>();
        for (uint i = 0; i < textureBindings.Count; i++)
        {
            ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
            if (existingBinding.key == key)
            {
                existingBinding.texture = texture.GetEntityValue();
                existingBinding.region = region;
                return true;
            }
        }

        AddTextureBinding(material, key, texture, region);
        return false;
    }

    public static void SetTextureRegion<T>(this T material, Texture texture, Vector4 region) where T : IMaterial
    {
        UnmanagedList<MaterialTextureBinding> textureBindings = material.GetList<T, MaterialTextureBinding>();
        for (uint i = 0; i < textureBindings.Count; i++)
        {
            ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
            if (existingBinding.texture == texture.GetEntityValue())
            {
                existingBinding.region = region;
                return;
            }
        }

        throw new InvalidOperationException($"Texture binding referencing texture `{texture}` does not exist to update region of.");
    }

    public static bool TryGetTextureBinding<T>(this T material, eint texture, out MaterialTextureBinding binding) where T : IMaterial
    {
        UnmanagedList<MaterialTextureBinding> textureBindings = material.GetList<T, MaterialTextureBinding>();
        for (uint i = 0; i < textureBindings.Count; i++)
        {
            ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
            if (existingBinding.texture == texture)
            {
                binding = existingBinding;
                return true;
            }
        }

        binding = default;
        return false;
    }

    public static MaterialTextureBinding GetTextureBinding<T>(this T material, uint binding) where T : IMaterial
    {
        UnmanagedList<MaterialTextureBinding> textureBindings = material.GetList<T, MaterialTextureBinding>();
        for (uint i = 0; i < textureBindings.Count; i++)
        {
            ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
            if (existingBinding.Binding == binding)
            {
                return existingBinding;
            }
        }

        throw new InvalidOperationException($"Texture binding `{binding}` does not exist on `{material}`.");
    }

    public static MaterialComponentBinding GetComponentBinding<T>(this T material, uint binding) where T : IMaterial
    {
        UnmanagedList<MaterialComponentBinding> componentBindings = material.GetList<T, MaterialComponentBinding>();
        for (uint i = 0; i < componentBindings.Count; i++)
        {
            ref MaterialComponentBinding existingBinding = ref componentBindings.GetRef(i);
            if (existingBinding.Binding == binding)
            {
                return existingBinding;
            }
        }

        throw new InvalidOperationException($"Component binding `{binding}` does not exist on `{material}`.");
    }

    public static bool RemoveTextureBinding<T>(this T material, DescriptorResourceKey key) where T : IMaterial
    {
        UnmanagedList<MaterialTextureBinding> textureBindings = material.GetList<T, MaterialTextureBinding>();
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

    public static ref MaterialComponentBinding TryGetProperty<T>(this T material, ShaderUniformProperty uniform, out bool contains) where T : IMaterial
    {
        UnmanagedList<MaterialComponentBinding> componentBindings = material.GetList<T, MaterialComponentBinding>();
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
        return ref Unsafe.NullRef<MaterialComponentBinding>();
    }

    public static ref MaterialTextureBinding TryGetProperty<T>(this T material, ShaderSamplerProperty sampler, out bool contains) where T : IMaterial
    {
        UnmanagedList<MaterialTextureBinding> textureBindings = material.GetList<T, MaterialTextureBinding>();
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
        return ref Unsafe.NullRef<MaterialTextureBinding>();
    }
}