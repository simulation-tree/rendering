using System;
using Worlds;

namespace Rendering.Components
{
    public struct IsRenderer : IEquatable<IsRenderer>
    {
        public rint meshReference;
        public rint materialReference;
        public LayerMask renderMask;

        public IsRenderer(rint mesh, rint material, LayerMask renderMask)
        {
            this.meshReference = mesh;
            this.materialReference = material;
            this.renderMask = renderMask;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is IsRenderer renderer && Equals(renderer);
        }

        public readonly bool Equals(IsRenderer other)
        {
            return meshReference.Equals(other.meshReference) && materialReference.Equals(other.materialReference) && renderMask.Equals(other.renderMask);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(meshReference, materialReference, renderMask);
        }

        public static bool operator ==(IsRenderer left, IsRenderer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IsRenderer left, IsRenderer right)
        {
            return !(left == right);
        }
    }
}