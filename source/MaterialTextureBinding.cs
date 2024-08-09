using Shaders;
using Simulation;
using System;
using System.Numerics;

namespace Materials
{
    /// <summary>
    /// A shader property that refers to a texture entity.
    /// </summary>
    public struct MaterialTextureBinding
    {
        public DescriptorResourceKey key;
        public eint texture;
        public Vector4 region;
        public Flags flags;

        public readonly byte Binding => key.Binding;
        public readonly byte Set => key.Set;

        public bool Changed
        {
            get => (flags & Flags.Changed) == Flags.Changed;
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

        public MaterialTextureBinding(DescriptorResourceKey key, eint texture, Vector4 region)
        {
            this.key = key;
            this.texture = texture;
            this.region = region;
        }

        [Flags]
        public enum Flags : byte
        {
            None = 0,
            Changed = 1
        }
    }
}
