using System.Numerics;

namespace Rendering.Components
{
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
