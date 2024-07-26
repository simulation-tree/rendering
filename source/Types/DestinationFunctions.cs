using Rendering;
using Rendering.Components;
using System;
using System.Numerics;
using Unmanaged;
using Unmanaged.Collections;

public static class DestinationFunctions
{
    public static (uint width, uint height) GetDestinationSize<T>(this T entity) where T : unmanaged,  IDestination
    {
        IsDestination isDestination = entity.GetComponent<T, IsDestination>();
        return (isDestination.width, isDestination.height);
    }

    public static float GetAspectRatio<T>(this T entity) where T : unmanaged, IDestination
    {
        (uint width, uint height) = entity.GetDestinationSize();
        return (float)width / height;
    }

    public static Vector4 GetDestinationRegion<T>(this T entity) where T : unmanaged, IDestination
    {
        return entity.GetComponent<T, IsDestination>().region;
    }

    public static int GetExtensions<T>(this T entity, Span<FixedString> buffer) where T : unmanaged, IDestination
    {
        UnmanagedList<Destination.Extension> extensions = entity.GetList<T, Destination.Extension>();
        int count = (int)Math.Min(extensions.Count, buffer.Length);
        for (int i = 0; i < count; i++)
        {
            buffer[i] = extensions[(uint)i].value;
        }

        return count;
    }

    public static void AddExtension<T>(this T entity, ReadOnlySpan<char> extension) where T : unmanaged, IDestination
    {
        UnmanagedList<Destination.Extension> extensions = entity.GetList<T, Destination.Extension>();
        for (uint i = 0; i < extensions.Count; i++)
        {
            if (extensions[i].value == extension)
            {
                throw new InvalidOperationException($"Extension '{extension}' already attached to destination");
            }
        }

        extensions.Add(new Destination.Extension(extension));
    }

    public static void AddExtension<T>(this T entity, FixedString extension) where T : unmanaged, IDestination
    {
        Span<char> span = stackalloc char[extension.Length];
        extension.CopyTo(span);
        entity.AddExtension(span);
    }
}