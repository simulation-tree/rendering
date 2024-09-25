using Simulation;
using System.Numerics;

namespace Rendering.Components
{
    public struct CameraOutput
    {
        public rint destinationReference;
        public Vector4 region;
        public sbyte order;

        public CameraOutput(rint destinationReference, Vector4 region, sbyte order)
        {
            this.destinationReference = destinationReference;
            this.region = region;
            this.order = order;
        }
    }
}
