using Rendering.Components;
using System;
using System.Diagnostics;
using System.Numerics;
using Unmanaged;
using Worlds;

namespace Rendering
{
    public readonly partial struct Destination : IEntity
    {
        /// <summary>
        /// Retrieves the size of the destination.
        /// </summary>
        public readonly (uint width, uint height) Size
        {
            get
            {
                IsDestination component = GetComponent<IsDestination>();
                return (component.width, component.height);
            }
            set
            {
                ref IsDestination component = ref GetComponent<IsDestination>();
                component.width = value.width;
                component.height = value.height;
            }
        }

        public readonly Vector2 SizeAsVector2
        {
            get
            {
                IsDestination component = GetComponent<IsDestination>();
                return new Vector2(component.width, component.height);
            }
            set
            {
                ref IsDestination component = ref GetComponent<IsDestination>();
                component.width = (uint)value.X;
                component.height = (uint)value.Y;
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

        public readonly ref Vector4 Region => ref GetComponent<IsDestination>().region;
        public readonly ref Vector4 ClearColor => ref GetComponent<IsDestination>().clearColor;
        public readonly ref ASCIIText256 RendererLabel => ref GetComponent<IsDestination>().rendererLabel;

        public Destination(World world, Vector2 size, ASCIIText256 renderer)
        {
            this.world = world;
            value = world.CreateEntity(new IsDestination(size, renderer));
            CreateArray<DestinationExtension>();
        }

        public Destination(World world, Vector2 size, System.Span<char> renderer)
        {
            this.world = world;
            value = world.CreateEntity(new IsDestination(size, renderer));
            CreateArray<DestinationExtension>();
        }

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsDestination>();
            archetype.AddArrayType<DestinationExtension>();
        }

        public readonly override string ToString()
        {
            return value.ToString();
        }

        public readonly int CopyExtensionNamesTo(Span<ASCIIText256> destination)
        {
            Span<DestinationExtension> array = GetArray<DestinationExtension>().AsSpan();
            int length = Math.Min(array.Length, destination.Length);
            array.As<DestinationExtension, ASCIIText256>().Slice(0, length).CopyTo(destination);
            return length;
        }

        public readonly bool ContainsExtension(ASCIIText256 extension)
        {
            Span<DestinationExtension> array = GetArray<DestinationExtension>().AsSpan();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].value.Equals(extension))
                {
                    return true;
                }
            }

            return false;
        }

        public readonly void AddExtension(ASCIIText256 extension)
        {
            ThrowIfExtensionAlreadyPresent(extension);

            Values<DestinationExtension> array = GetArray<DestinationExtension>();
            int length = array.Length;
            array.Length++;
            array[length] = new DestinationExtension(extension);
        }

        /// <summary>
        /// Converts the given position on the destination surface to [0, 1] screen coordinates.
        /// </summary>
        public readonly Vector2 GetScreenPointFromPosition(Vector2 position)
        {
            (uint width, uint height) = Size;
            return position / new Vector2(width, height);
        }

        public readonly bool TryGetRendererInstanceInUse(out MemoryAddress instance)
        {
            ref RendererInstanceInUse component = ref TryGetComponent<RendererInstanceInUse>(out bool contains);
            if (contains)
            {
                instance = component.value;
                return true;
            }
            else
            {
                instance = default;
                return false;
            }
        }

        public readonly bool TryGetSurfaceInUse(out MemoryAddress surface)
        {
            ref SurfaceInUse component = ref TryGetComponent<SurfaceInUse>(out bool contains);
            if (contains)
            {
                surface = component.value;
                return true;
            }
            else
            {
                surface = default;
                return false;
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfExtensionAlreadyPresent(ASCIIText256 extension)
        {
            if (ContainsExtension(extension))
            {
                throw new InvalidOperationException($"Extension `{extension}` is already present on the destination.");
            }
        }
    }
}