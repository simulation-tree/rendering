using System;
using Unmanaged;

namespace Rendering
{
    public readonly struct RendererLabel : IEquatable<RendererLabel>
    {
        public readonly long hash;

        public RendererLabel(ReadOnlySpan<char> text)
        {
            hash = text.GetLongHashCode();
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is RendererLabel label && Equals(label);
        }

        public readonly bool Equals(RendererLabel other)
        {
            return hash == other.hash;
        }

        public override int GetHashCode()
        {
            return (int)hash;
        }

        public static bool operator ==(RendererLabel left, RendererLabel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RendererLabel left, RendererLabel right)
        {
            return !(left == right);
        }

        public static implicit operator RendererLabel(string text)
        {
            return new RendererLabel(text.AsSpan());
        }
    }
}