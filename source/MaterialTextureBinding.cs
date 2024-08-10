using Shaders;
using Simulation;
using System;
using System.Numerics;
using Textures;

namespace Materials
{
    /// <summary>
    /// A shader property that refers to a texture entity.
    /// </summary>
    public struct MaterialTextureBinding : IEquatable<MaterialTextureBinding>
    {
        public DescriptorResourceKey key;

        private uint version;
        private Vector4 region;
        private eint textureEntity;

        public readonly byte Binding => key.Binding;
        public readonly byte Set => key.Set;
        public readonly eint TextureEntity => textureEntity;

        /// <summary>
        /// The version of this binding, updated when region or texture is changed.
        /// </summary>
        public readonly uint Version => version;

        public readonly Vector4 Region => region;

        public MaterialTextureBinding(uint version, DescriptorResourceKey key, eint texture, Vector4 region)
        {
            this.version = version;
            this.key = key;
            this.textureEntity = texture;
            this.region = region;
        }

        public void SetTexture<T>(T texture) where T : ITexture
        {
            textureEntity = texture.GetEntityValue();
            version++;
        }

        public void SetRegion(Vector4 region)
        {
            this.region = region;
            version++;
        }

        public void SetRegion(float x, float y, float width, float height)
        {
            region = new Vector4(x, y, width, height);
            version++;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is MaterialTextureBinding binding && Equals(binding);
        }

        public readonly bool Equals(MaterialTextureBinding other)
        {
            return key.Equals(other.key) && textureEntity.Equals(other.textureEntity) && region.Equals(other.region);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(key, textureEntity, region);
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
