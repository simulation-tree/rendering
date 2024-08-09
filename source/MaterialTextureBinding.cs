using Shaders;
using Simulation;
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

        public readonly byte Binding => key.Binding;
        public readonly byte Set => key.Set;

        public MaterialTextureBinding(DescriptorResourceKey key, eint texture, Vector4 region)
        {
            this.key = key;
            this.texture = texture;
            this.region = region;
        }
    }
}
