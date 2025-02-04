using System.Numerics;
using Worlds;

namespace Rendering.Components
{
    [Component]
    public struct RendererScissor
    {
        public Vector4 value;

        public RendererScissor(Vector4 value)
        {
            this.value = value;
        }

        public RendererScissor(float x, float y, float width, float height)
        {
            value = new(x, y, width, height);
        }
    }
}
