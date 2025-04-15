using System;

namespace Rendering
{
    public readonly struct RenderEntity : IEquatable<RenderEntity>
    {
        public readonly uint entity;
        public readonly uint meshEntity;
        public readonly uint vertexShaderEntity;
        public readonly uint fragmentShaderEntity;
        public readonly ushort meshVersion;
        public readonly ushort vertexShaderVersion;
        public readonly ushort fragmentShaderVersion;

        public RenderEntity(uint entity, uint meshEntity, uint vertexShaderEntity, uint fragmentShaderEntity, ushort meshVersion, ushort vertexShaderVersion, ushort fragmentShaderVersion)
        {
            this.entity = entity;
            this.meshEntity = meshEntity;
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
                   vertexShaderEntity == other.vertexShaderEntity &&
                   fragmentShaderEntity == other.fragmentShaderEntity &&
                   meshVersion == other.meshVersion &&
                   vertexShaderVersion == other.vertexShaderVersion &&
                   fragmentShaderVersion == other.fragmentShaderVersion;
        }

        public readonly override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(entity);
            hash.Add(meshEntity);
            hash.Add(vertexShaderEntity);
            hash.Add(fragmentShaderEntity);
            hash.Add(meshVersion);
            hash.Add(vertexShaderVersion);
            hash.Add(fragmentShaderVersion);
            return hash.ToHashCode();
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