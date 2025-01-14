using System;
using System.Diagnostics;
using Unmanaged;

namespace Rendering
{
    /// <summary>
    /// Represents a layer part of a <see cref="LayerMask"/>.
    /// </summary>
    public readonly struct Layer : IEquatable<Layer>
    {
        public static readonly Layer MinValue = new(0);
        public static readonly Layer MaxValue = new(31);

        private readonly byte value;

        public Layer(byte value)
        {
            ThrowIfOutOfRange(value);

            this.value = value;
        }

        public readonly override string ToString()
        {
            USpan<char> buffer = stackalloc char[2];
            uint length = ToString(buffer);
            return buffer.Slice(0, length).ToString();
        }

        public readonly uint ToString(USpan<char> buffer)
        {
            return value.ToString(buffer);
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
            return HashCode.Combine(value);
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
        private static void ThrowIfOutOfRange(byte value)
        {
            if (value > 31)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"The layer must be no greater than 31");
            }
        }
    }
}