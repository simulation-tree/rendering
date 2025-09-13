using System;

namespace Rendering
{
    public readonly struct RenderEntity : IEquatable<RenderEntity>
    {
        public readonly uint entity;
        public readonly uint meshEntity;
        public readonly uint materialEntity;
        public readonly uint vertexShaderEntity;
        public readonly uint fragmentShaderEntity;
        public readonly ushort meshVersion;
        public readonly ushort vertexShaderVersion;
        public readonly ushort fragmentShaderVersion;

        public RenderEntity(uint entity, uint meshEntity, uint materialEntity, uint vertexShaderEntity, uint fragmentShaderEntity, ushort meshVersion, ushort vertexShaderVersion, ushort fragmentShaderVersion)
        {
            this.entity = entity;
            this.meshEntity = meshEntity;
            this.materialEntity = materialEntity;
            this.vertexShaderEntity = vertexShaderEntity;
            this.fragmentShaderEntity = fragmentShaderEntity;
            this.meshVersion = meshVersion;
            this.vertexShaderVersion = vertexShaderVersion;
            this.fragmentShaderVersion = fragmentShaderVersion;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is RenderEntity entity && Equals(entity);
        }

        public readonly bool Equals(RenderEntity other)
        {
            return entity == other.entity &&
                    meshEntity == other.meshEntity &&
                    materialEntity == other.materialEntity &&
                    vertexShaderEntity == other.vertexShaderEntity &&
                    fragmentShaderEntity == other.fragmentShaderEntity &&
                    meshVersion == other.meshVersion &&
                    vertexShaderVersion == other.vertexShaderVersion &&
                    fragmentShaderVersion == other.fragmentShaderVersion;
        }

        public readonly override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + (int)entity;
            hash = hash * 31 + (int)meshEntity;
            hash = hash * 31 + (int)materialEntity;
            hash = hash * 31 + (int)vertexShaderEntity;
            hash = hash * 31 + (int)fragmentShaderEntity;
            hash = hash * 31 + meshVersion;
            hash = hash * 31 + vertexShaderVersion;
            hash = hash * 31 + fragmentShaderVersion;
            return hash;
        }

        public static bool operator ==(RenderEntity left, RenderEntity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RenderEntity left, RenderEntity right)
        {
            return !(left == right);
        }
    }
}