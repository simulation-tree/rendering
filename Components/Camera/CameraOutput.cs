using Simulation;
using System.Numerics;

namespace Rendering.Components
{
    public struct CameraOutput
    {
        public EntityID destination;
        public Vector4 region;
        public Vector4 clearColor;
        public sbyte order;

        public CameraOutput(EntityID destination, Vector4 region, Vector4 clearColor, sbyte order)
        {
            this.destination = destination;
            this.region = region;
            this.clearColor = clearColor;
            this.order = order;
        }
    }
}
