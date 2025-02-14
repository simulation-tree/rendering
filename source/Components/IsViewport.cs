using System.Numerics;
using Worlds;

namespace Rendering.Components
{
    public struct IsViewport
    {
        public rint destinationReference;
        public Vector4 region;
        public sbyte order;
        public LayerMask renderMask;

        public IsViewport(rint destinationReference, Vector4 region, sbyte order, LayerMask renderMask)
        {
            this.destinationReference = destinationReference;
            this.region = region;
            this.order = order;
            this.renderMask = renderMask;
        }
    }
}