using Rendering.Components;
using Simulation;
using System;
using System.Numerics;
using Unmanaged;

namespace Rendering
{
    public readonly struct Destination : IEntity
    {
        private readonly Entity entity;

        public readonly bool IsDestroyed => entity.IsDestroyed;

        /// <summary>
        /// Retrieves the size of the destination.
        /// </summary>
        public readonly (uint width, uint height) DestinationSize
        {
            get
            {
                IsDestination isDestination = entity.GetComponent<IsDestination>();
                return (isDestination.width, isDestination.height);
            }
            set
            {
                ref IsDestination isDestination = ref entity.GetComponent<IsDestination>();
                isDestination.width = value.width;
                isDestination.height = value.height;
            }
        }

        public readonly uint DestinationArea
        {
            get
            {
                (uint width, uint height) = DestinationSize;
                return width * height;
            }
        }

        public readonly float AspectRatio
        {
            get
            {
                (uint width, uint height) = DestinationSize;
                return (float)width / height;
            }
        }

        public readonly Vector4 DestinationRegion => entity.GetComponent<IsDestination>().region;


        World IEntity.World => entity;
        uint IEntity.Value => entity;

        public Destination(World world, uint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Destination(World world, Vector2 size, FixedString renderer)
        {
            entity = new(world);
            entity.AddComponent(new IsDestination(size, new Vector4(0, 0, 1, 1), renderer));
            entity.CreateArray<Extension>(0);
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        Query IEntity.GetQuery(World world)
        {
            return new(world, RuntimeType.Get<IsDestination>());
        }

        public readonly int CopyExtensionNamesTo(Span<FixedString> buffer)
        {
            //todo: should be possible to cast the unmanaged list directly into the desired type, the extension and FixedString are same size
            Span<Extension> extensions = entity.GetArray<Extension>();
            int count = (int)Math.Min(extensions.Length, buffer.Length);
            for (int i = 0; i < count; i++)
            {
                buffer[i] = extensions[i].text;
            }

            return count;
        }

        public readonly void AddExtension(ReadOnlySpan<char> extension)
        {
            Span<Extension> extensions = entity.GetArray<Extension>();
            for (uint i = 0; i < extensions.Length; i++)
            {
                if (extensions[(int)i].text == extension)
                {
                    throw new InvalidOperationException($"Extension `{extension.ToString()}` is already attached to destination `{entity}`");
                }
            }

            uint extensionCount = (uint)extensions.Length;
            extensions = entity.ResizeArray<Extension>(extensionCount + 1);
            extensions[(int)extensionCount] = new Extension(extension);
        }

        public readonly void AddExtension(FixedString extension)
        {
            Span<char> span = stackalloc char[FixedString.MaxLength];
            int length = extension.ToString(span);
            AddExtension(span[..length]);
        }

        public static implicit operator Entity(Destination destination)
        {
            return destination.entity;
        }

        public readonly struct Extension
        {
            public readonly FixedString text;

            public Extension(FixedString text)
            {
                this.text = text;
            }

            public Extension(ReadOnlySpan<char> text)
            {
                this.text = new(text);
            }
        }
    }
}
