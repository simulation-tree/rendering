using System;
using Unmanaged;

namespace Rendering
{
    /// <summary>
    /// Represents a mask containing up to 32 <see cref="Layer"/>s.
    /// </summary>
    public struct LayerMask : IEquatable<LayerMask>
    {
        /// <summary>
        /// Maximum amount of <see cref="Layer"/>s that can be stored.
        /// </summary>
        public const byte Capacity = 32;

        public static readonly LayerMask None = new(0);
        public static readonly LayerMask All = new(uint.MaxValue);

        private uint value;

#if NET
        /// <summary>
        /// Creates an empty layer mask containing no <see cref="Layer"/>s.
        /// </summary>
        public LayerMask()
        {
        }
#endif

        public LayerMask(uint value)
        {
            this.value = value;
        }

        public readonly override string ToString()
        {
            USpan<char> buffer = stackalloc char[32];
            uint length = ToString(buffer);
            return buffer.Slice(0, length).ToString();
        }

        public readonly uint ToString(USpan<char> buffer)
        {
            uint length = 0;
            for (byte i = 0; i < Layer.MaxValue; i++)
            {
                if (Contains(new(i)))
                {
                    length += i.ToString(buffer.Slice(length));
                    buffer[length++] = ',';
                }
            }

            if (length > 0)
            {
                length--;
            }

            return length;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is LayerMask mask && Equals(mask);
        }

        public readonly bool Equals(LayerMask other)
        {
            return value == other.value;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        /// <summary>
        /// Checks if the given <paramref name="layer"/> is present.
        /// </summary>
        public readonly bool Contains(Layer layer)
        {
            return (value & (1u << layer)) != 0;
        }

        /// <summary>
        /// Checks if this layer mask contains all layers from the given <paramref name="layerMask"/>.
        /// </summary>
        public readonly bool ContainsAll(LayerMask layerMask)
        {
            return (value & layerMask.value) == layerMask.value;
        }

        /// <summary>
        /// Checks if the layer mask contains any of the layers from the given <paramref name="layerMask"/>.
        /// </summary>
        public readonly bool ContainsAny(LayerMask layerMask)
        {
            return (value & layerMask.value) != 0;
        }

        /// <summary>
        /// Makes the given <paramref name="layer"/> present in the layer mask.
        /// </summary>
        public LayerMask Set(Layer layer)
        {
            value |= 1u << layer;
            return this;
        }

        /// <summary>
        /// Removes the given <paramref name="layer"/> from the layer mask.
        /// </summary>
        public LayerMask Clear(Layer layer)
        {
            value &= ~(1u << layer);
            return this;
        }

        public static bool operator ==(LayerMask left, LayerMask right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(LayerMask left, LayerMask right)
        {
            return !(left == right);
        }

        public static LayerMask operator |(LayerMask left, LayerMask right)
        {
            return new(left.value | right.value);
        }

        public static LayerMask operator &(LayerMask left, LayerMask right)
        {
            return new(left.value & right.value);
        }

        public static LayerMask operator ^(LayerMask left, LayerMask right)
        {
            return new(left.value ^ right.value);
        }

        public static LayerMask operator ~(LayerMask mask)
        {
            return new(~mask.value);
        }
    }
}