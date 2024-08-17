using Data;
using Simulation;
using System.Numerics;

namespace Rendering.Components
{
    public struct CameraOutput
    {
        public eint destination;
        public Vector4 region;
        public Color clearColor;
        public sbyte order;

        public CameraOutput(eint destination, Vector4 region, Color clearColor, sbyte order)
        {
            this.destination = destination;
            this.region = region;
            this.clearColor = clearColor;
            this.order = order;
        }

        public CameraOutput(Destination destination, Vector4 region, Color clearColor, sbyte order)
        {
            this.destination = ((Entity)destination).value;
            this.region = region;
            this.clearColor = clearColor;
            this.order = order;
        }
    }
}
