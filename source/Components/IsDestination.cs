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
        public IsDestination(Vector2 size, FixedString rendererLabel)
        {
            width = (uint)size.X;
            height = (uint)size.Y;
            region = new Vector4(0, 0, 1, 1);
            clearColor = new Vector4(0, 0, 0, 1);
            this.rendererLabel = rendererLabel;
        }

        public IsDestination(uint width, uint height, FixedString rendererLabel, Vector4 region, Vector4 clearColor)
        {
            this.width = width;
            this.height = height;
            this.region = region;
            this.clearColor = clearColor;
            this.rendererLabel = rendererLabel;
        }

        public IsDestination(Vector2 size, FixedString rendererLabel, Vector4 region, Vector4 clearColor)
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