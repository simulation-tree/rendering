using System;
using System.Numerics;
using Unmanaged;
using Worlds;

namespace Rendering.Components
{
    [Component]
    public struct IsDestination
    {
        public uint width;
        public uint height;
        public Vector4 region;
        public Vector4 clearColor;
        public FixedString rendererLabel;

        public readonly uint Area => width * height;

        public (uint x, uint y) Size
        {
            readonly get => (width, height);
            set
            {
                width = value.x;
                height = value.y;
            }
        }

        public readonly float AspectRatio => width / (float)height;

#if NET
        [Obsolete("Default constructor not supported")]
        public IsDestination()
        {
            throw new NotImplementedException();
        }
#endif

        public IsDestination(uint width, uint height, Vector4 region, Vector4 clearColor, FixedString rendererLabel)
        {
            this.width = width;
            this.height = height;
            this.region = region;
            this.clearColor = clearColor;
            this.rendererLabel = rendererLabel;
        }

        public IsDestination(Vector2 size, Vector4 region, Vector4 clearColor, FixedString rendererLabel)
        {
            width = (uint)size.X;
            height = (uint)size.Y;
            this.region = region;
            this.clearColor = clearColor;
            this.rendererLabel = rendererLabel;
        }

        public readonly Vector2 SizeAsVector2()
        {
            return new(width, height);
        }
    }
}