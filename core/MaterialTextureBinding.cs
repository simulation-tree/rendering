using System;
using System.Numerics;
using Worlds;

namespace Rendering
{
    /// <summary>
    /// A shader property that refers to a texture entity.
    /// </summary>
    [ArrayElement]
    public struct MaterialTextureBinding : IEquatable<MaterialTextureBinding>
    {
        public DescriptorResourceKey key;

        private uint version;
        private Vector4 region;
        private uint textureEntity;
        private TextureFiltering filtering;

        public readonly byte Binding => key.Binding;
        public readonly byte Set => key.Set;
        public readonly uint TextureEntity => textureEntity;

        /// <summary>
        /// The version of this binding, updated when region or texture is changed.
        /// </summary>
        public readonly uint Version => version;

        public readonly Vector4 Region => region;

        public readonly TextureFiltering Filter => filtering;

        public MaterialTextureBinding(uint version, DescriptorResourceKey key, uint texture, Vector4 region, TextureFiltering filtering)
        {
            this.version = version;
            this.key = key;
            textureEntity = texture;
            this.region = region;
            this.filtering = filtering;
        }

        public MaterialTextureBinding(uint version, DescriptorResourceKey key, Entity texture, Vector4 region, TextureFiltering filtering)
        {
            this.version = version;
            this.key = key;
            textureEntity = texture.GetEntityValue();
            this.region = region;
            this.filtering = filtering;
        }

        public void SetTexture(Entity texture)
        {
            uint textureEntity = texture.GetEntityValue();
            if (this.textureEntity != textureEntity)
            {
                this.textureEntity = textureEntity;
                version++;
            }
        }

        public void SetRegion(Vector4 region)
        {
            if (this.region != region)
            {
                this.region = region;
                version++;
            }
        }

        public void SetRegion(float x, float y, float width, float height)
        {
            SetRegion(new Vector4(x, y, width, height));
        }

        public void SetFiltering(TextureFiltering filtering)
        {
            if (this.filtering != filtering)
            {
                this.filtering = filtering;
                version++;
            }
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
