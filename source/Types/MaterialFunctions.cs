using Materials;
using Rendering;
using Rendering.Components;
using Shaders;
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
        return component.Get(material.World);
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

    public static void AddComponentBinding<T>(this T material, DescriptorResourceKey key, ShaderStage stage, RuntimeType componentType) where T : IMaterial
    {
        UnmanagedList<MaterialComponentBinding> componentBindings = material.GetList<T, MaterialComponentBinding>();
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

    public static void AddComponentBinding<T, C>(this T material, DescriptorResourceKey key, ShaderStage stage) where T : IMaterial where C : unmanaged
    {
        RuntimeType componentType = RuntimeType.Get<C>();
        AddComponentBinding(material, key, stage, componentType);
    }

    public static void AddComponentBinding<T>(this T material, byte binding, byte set, ShaderStage stage, RuntimeType componentType) where T : IMaterial
    {
        AddComponentBinding(material, new(binding, set), stage, componentType);
    }

    public static void AddComponentBinding<T, C>(this T material, byte binding, byte set, ShaderStage stage) where T : IMaterial where C : unmanaged
    {
        RuntimeType componentType = RuntimeType.Get<C>();
        AddComponentBinding(material, new(binding, set), stage, componentType);
    }

    public static bool SetComponentBinding<T>(this T material, DescriptorResourceKey key, RuntimeType componentType, ShaderStage stage) where T : IMaterial
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

        AddComponentBinding<T>(material, key, stage, componentType);
        return false;
    }

    public static bool SetComponentBinding<T, C>(this T material, DescriptorResourceKey key, ShaderStage stage) where T : IMaterial where C : unmanaged
    {
        RuntimeType componentType = RuntimeType.Get<C>();
        return SetComponentBinding(material, key, componentType, stage);
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
                throw new InvalidOperationException($"Texture binding {key} already exists.");
            }
        }

        textureBindings.Add(new(key, texture.entity.value, region));
    }

    public static void AddTextureBinding<T>(this T material, byte binding, byte set, Texture texture) where T : IMaterial
    {
        AddTextureBinding(material, new(binding, set), texture);
    }

    public static void AddTextureBinding<T>(this T material, byte binding, byte set, Texture texture, Vector4 region) where T : IMaterial
    {
        AddTextureBinding(material, new(binding, set), texture, region);
    }

    public static void SetTextureBinding<T>(this T material, DescriptorResourceKey key, Texture texture) where T : IMaterial
    {
        SetTextureBinding(material, key, texture, new(0, 0, 1, 1));
    }

    public static void SetTextureBinding<T>(this T material, DescriptorResourceKey key, Texture texture, Vector4 region) where T : IMaterial
    {
        UnmanagedList<MaterialTextureBinding> textureBindings = material.GetList<T, MaterialTextureBinding>();
        for (uint i = 0; i < textureBindings.Count; i++)
        {
            ref MaterialTextureBinding existingBinding = ref textureBindings.GetRef(i);
            if (existingBinding.key == key)
            {
                existingBinding.texture = texture.entity.value;
                existingBinding.region = region;
                return;
            }
        }

        AddTextureBinding(material, key, texture, region);
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
                    throw new InvalidOperationException(
                        $"Component binding {existingBinding.key} has an invalid size.");
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