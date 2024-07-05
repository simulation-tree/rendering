using Game;
using Shaders;
using System;
using System.Numerics;

namespace Materials
{
    /// <summary>
    /// A shader property that refers to a texture entity.
    /// </summary>
    public struct MaterialTextureBinding
    {
        public ResourceKey key;
        public EntityID texture;
        public Vector4 region;
        public Flags flags;

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

        public MaterialTextureBinding(ResourceKey key, EntityID texture, Vector4 region)
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
