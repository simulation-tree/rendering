using Data;
using Simulation;
using System.Numerics;

namespace Rendering.Components
{
    public struct CameraOutput
    {
        public rint destinationReference;
        public Vector4 region;
        public Color clearColor;
        public sbyte order;

        public CameraOutput(rint destinationReference, Vector4 region, Color clearColor, sbyte order)
        {
            this.destinationReference = destinationReference;
            this.region = region;
            this.clearColor = clearColor;
            this.order = order;
        }
    }
}
