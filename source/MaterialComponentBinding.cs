using Shaders;
using System;
using Unmanaged;

namespace Materials
{
    /// <summary>
    /// Links a component to a uniform buffer object.
    /// </summary>
    public struct MaterialComponentBinding : IEquatable<MaterialComponentBinding>
    {
        public DescriptorResourceKey key;
        public ShaderStage stage;
        public RuntimeType componentType;
        public Flags flags;

        public readonly byte Binding => key.Binding;
        public readonly byte Set => key.Set;

        public bool Changed
        {
            readonly get => (flags & Flags.Changed) != 0;
            set
            {
                if (value)
                {
                    flags |= Flags.Changed;
                }
                else
                {
                    flags &= ~Flags.Changed;
                }
            }
        }

        public bool AlwaysUpdated
        {
            readonly get => (flags & Flags.AlwaysUpdate) != 0;
            set
            {
                if (value)
                {
                    flags |= Flags.AlwaysUpdate;
                    flags &= ~Flags.RequireManualUpdate;
                }
                else
                {
                    flags &= ~Flags.AlwaysUpdate;
                    flags |= Flags.RequireManualUpdate;
                }
            }
        }

        public bool RequireManualUpdate
        {
            readonly get => (flags & Flags.RequireManualUpdate) != 0;
            set
            {
                if (value)
                {
                    flags |= Flags.RequireManualUpdate;
                    flags &= ~Flags.AlwaysUpdate;
                }
                else
                {
                    flags &= ~Flags.RequireManualUpdate;
                    flags |= Flags.AlwaysUpdate;
                }
            }
        }

        public MaterialComponentBinding(DescriptorResourceKey key, ShaderStage stage, RuntimeType componentType)
        {
            this.key = key;
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
        public static MaterialComponentBinding Create<T>(DescriptorResourceKey key, ShaderStage stage) where T : unmanaged
        {
            return new MaterialComponentBinding(key, stage, RuntimeType.Get<T>());
        }

        public static bool operator ==(MaterialComponentBinding left, MaterialComponentBinding right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MaterialComponentBinding left, MaterialComponentBinding right)
        {
            return !(left == right);
        }

        [Flags]
        public enum Flags : byte
        {
            None = 0,
            AlwaysUpdate = 1,
            RequireManualUpdate = 2,
            Changed = 4
        }
    }
}
