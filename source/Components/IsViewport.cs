using System.Numerics;
using Worlds;

namespace Rendering.Components
{
    [Component]
    public struct IsViewport
    {
        public rint destinationReference;
        public Vector4 region;
        public sbyte order;
        public uint mask;

        public IsViewport(rint destinationReference, Vector4 region, sbyte order, uint mask)
        {
            this.destinationReference = destinationReference;
            this.region = region;
            this.order = order;
            this.mask = mask;
        }
    }
}