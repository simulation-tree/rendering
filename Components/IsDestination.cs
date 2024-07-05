using System.Numerics;

namespace Rendering.Components
{
    public struct IsDestination
    {
        public uint width;
        public uint height;
        public Vector4 region;

        public IsDestination(uint width, uint height, Vector4 region)
        {
            this.width = width;
            this.height = height;
            this.region = region;
        }
    }
}