using System.Numerics;
using Worlds;

namespace Rendering.Components
{
    [Component]
    public struct RendererScissor
    {
        public Vector4 region;

        public RendererScissor(Vector4 region)
        {
            this.region = region;
        }

        public RendererScissor(float x, float y, float width, float height)
        {
            region = new(x, y, width, height);
        }
    }
}
