using Shaders;
using Simulation;
using System;
using System.Numerics;

namespace Materials
{
    /// <summary>
    /// A shader property that refers to a texture entity.
    /// </summary>
    public struct MaterialTextureBinding : IEquatable<MaterialTextureBinding>
    {
        public DescriptorResourceKey key;
        public eint texture;
        public Vector4 region;

        public readonly byte Binding => key.Binding;
        public readonly byte Set => key.Set;

        public MaterialTextureBinding(DescriptorResourceKey key, eint texture, Vector4 region)
        {
            this.key = key;
            this.texture = texture;
            this.region = region;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is MaterialTextureBinding binding && Equals(binding);
        }

        public readonly bool Equals(MaterialTextureBinding other)
        {
            return key.Equals(other.key) && texture.Equals(other.texture) && region.Equals(other.region);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(key, texture, region);
        }

        public static bool operator ==(MaterialTextureBinding left, MaterialTextureBinding right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MaterialTextureBinding left, MaterialTextureBinding right)
        {
            return !(left == right);
        }
    }
}
