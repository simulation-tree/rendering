using Data;
using Rendering.Components;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Unmanaged;
using Worlds;

namespace Rendering
{
    public readonly struct Destination : IEntity, IEquatable<Destination>
    {
        private readonly Entity entity;

        /// <summary>
        /// Retrieves the size of the destination.
        /// </summary>
        public readonly (uint width, uint height) Size
        {
            get
            {
                IsDestination isDestination = entity.GetComponentRef<IsDestination>();
                return (isDestination.width, isDestination.height);
            }
            set
            {
                ref IsDestination isDestination = ref entity.GetComponentRef<IsDestination>();
                isDestination.width = value.width;
                isDestination.height = value.height;
            }
        }

        public readonly uint Area
        {
            get
            {
                (uint width, uint height) = Size;
                return width * height;
            }
        }

        public readonly float AspectRatio
        {
            get
            {
                (uint width, uint height) = Size;
                return (float)width / height;
            }
        }

        public readonly ref Vector4 DestinationRegion => ref entity.GetComponentRef<IsDestination>().region;
        public readonly ref Vector4 ClearColor => ref entity.GetComponentRef<IsDestination>().clearColor;

        readonly uint IEntity.Value => entity.value;
        readonly World IEntity.World => entity.world;
        readonly Definition IEntity.Definition => new Definition().AddComponentType<IsDestination>().AddArrayType<DestinationExtension>();

        public Destination(World world, uint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Destination(World world, Vector2 size, FixedString renderer)
        {
            entity = new(world);
            entity.AddComponent(new IsDestination(size, new Vector4(0, 0, 1, 1), Color.Black.value, renderer));
            entity.CreateArray<DestinationExtension>();
        }

        public Destination(World world, Vector2 size, USpan<char> renderer)
        {
            entity = new(world);
            entity.AddComponent(new IsDestination(size, new Vector4(0, 0, 1, 1), Color.Black.value, new(renderer)));
            entity.CreateArray<DestinationExtension>();
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly Vector2 SizeAsVector2()
        {
            (uint width, uint height) = Size;
            return new Vector2(width, height);
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Destination destination && entity == destination.entity;
        }

        public readonly override int GetHashCode()
        {
            return entity.GetHashCode();
        }

        public readonly uint CopyExtensionNamesTo(USpan<FixedString> buffer)
        {
            //todo: should be possible to cast the unmanaged list directly into the desired type, the extension and FixedString are same size
            USpan<DestinationExtension> extensions = entity.GetArray<DestinationExtension>();
            uint count = Math.Min(extensions.Length, buffer.Length);
            for (uint i = 0; i < count; i++)
            {
                buffer[i] = extensions[i].text;
            }

            return count;
        }

        public readonly void AddExtension(USpan<char> extension)
        {
            USpan<DestinationExtension> extensions = entity.GetArray<DestinationExtension>();
            for (uint i = 0; i < extensions.Length; i++)
            {
                if (extensions[i].text.Equals(extension))
                {
                    throw new InvalidOperationException($"Extension `{extension.ToString()}` is already attached to destination `{entity}`");
                }
            }

            uint extensionCount = extensions.Length;
            extensions = entity.ResizeArray<DestinationExtension>(extensionCount + 1);
            extensions[extensionCount] = new DestinationExtension(extension);
        }

        public readonly void AddExtension(FixedString extension)
        {
            USpan<char> span = stackalloc char[(int)FixedString.Capacity];
            uint length = extension.CopyTo(span);
            AddExtension(span.Slice(0, length));
        }

        /// <summary>
        /// Converts the given position on the destination surface to [0, 1] screen coordinates.
        /// </summary>
        public readonly Vector2 GetScreenPointFromPosition(Vector2 position)
        {
            (uint width, uint height) = Size;
            Vector2 screenPoint = position / new Vector2(width, height);
            return screenPoint;
        }

        public readonly bool Equals(Destination other)
        {
            return entity == other.entity;
        }

        public static bool operator ==(Destination a, Destination b)
        {
            return a.entity == b.entity;
        }

        public static bool operator !=(Destination a, Destination b)
        {
            return a.entity != b.entity;
        }

        public static implicit operator Entity(Destination destination)
        {
            return destination.entity;
        }
    }
}