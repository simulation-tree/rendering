using System.Numerics;
using Unmanaged;

namespace Rendering.Components
{
    public struct IsDestination
    {
        public uint width;
        public uint height;
        public Vector4 region;
        public FixedString rendererLabel;

        public readonly uint Area => width * height;

        public IsDestination(uint width, uint height, Vector4 region, FixedString rendererLabel)
        {
            this.width = width;
            this.height = height;
            this.region = region;
            this.rendererLabel = rendererLabel;
        }

        public IsDestination(Vector2 size, Vector4 region, FixedString rendererLabel)
        {
            width = (uint)size.X;
            height = (uint)size.Y;
            this.region = region;
            this.rendererLabel = rendererLabel;
        }
    }
}