using System;
using System.Diagnostics;

namespace Rendering
{
    /// <summary>
    /// Represents a single layer inside a <see cref="LayerMask"/>.
    /// </summary>
    public readonly struct Layer : IEquatable<Layer>
    {
        public static readonly Layer MinValue = new(0);
        public static readonly Layer MaxValue = new(LayerMask.Capacity - 1);

        private readonly byte value;

        public Layer(byte value)
        {
            ThrowIfOutOfRange(value);

            this.value = value;
        }

        public Layer(uint value)
        {
            ThrowIfOutOfRange(value);

            this.value = (byte)value;
        }

        public readonly override string ToString()
        {
            return value.ToString();
        }

        public readonly int ToString(Span<char> destination)
        {
            return value.ToString(destination);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is Layer layer && Equals(layer);
        }

        public readonly bool Equals(Layer other)
        {
            return value == other.value;
        }

        public readonly override int GetHashCode()
        {
            return value;
        }

        public static bool operator ==(Layer left, Layer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Layer left, Layer right)
        {
            return !(left == right);
        }

        public static Layer operator +(Layer left, byte right)
        {
            return new((byte)(left.value + right));
        }

        public static Layer operator -(Layer left, byte right)
        {
            return new((byte)(left.value - right));
        }

        public static implicit operator Layer(byte value)
        {
            ThrowIfOutOfRange(value);

            return new(value);
        }

        public static implicit operator byte(Layer layer)
        {
            return layer.value;
        }

        [Conditional("DEBUG")]
        private static void ThrowIfOutOfRange(uint value)
        {
            if (value >= LayerMask.Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"The layer must be no greater than {LayerMask.Capacity - 1}");
            }
        }
    }
}