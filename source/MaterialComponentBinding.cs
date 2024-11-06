using Shaders;
using System;
using Unmanaged;

namespace Rendering
{
    /// <summary>
    /// Links a component to a uniform buffer object.
    /// </summary>
    public struct MaterialComponentBinding : IEquatable<MaterialComponentBinding>
    {
        public DescriptorResourceKey key;
        public uint entity;
        public RuntimeType componentType;
        public RenderStage stage;

        public readonly byte Binding => key.Binding;
        public readonly byte Set => key.Set;

        public MaterialComponentBinding(DescriptorResourceKey key, uint entity, RuntimeType componentType, RenderStage stage)
        {
            this.key = key;
            this.entity = entity;
            this.componentType = componentType;
            this.stage = stage;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is MaterialComponentBinding property && Equals(property);
        }

        public readonly bool Equals(MaterialComponentBinding other)
        {
            return componentType.Equals(other.componentType) && key == other.key && stage == other.stage;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(componentType, key, stage);
        }

        /// <summary>
        /// Gets a property element that references a component of type <typeparamref name="T"/>.
        /// </summary>
        public static MaterialComponentBinding Create<T>(DescriptorResourceKey key, uint entity, RenderStage stage) where T : unmanaged
        {
            return new MaterialComponentBinding(key, entity, RuntimeType.Get<T>(), stage);
        }

        public static bool operator ==(MaterialComponentBinding left, MaterialComponentBinding right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MaterialComponentBinding left, MaterialComponentBinding right)
        {
            return !(left == right);
        }
    }
}
