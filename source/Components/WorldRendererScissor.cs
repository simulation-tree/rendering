using System.Numerics;

namespace Rendering.Components
{
    public struct WorldRendererScissor
    {
        public Vector4 value;

        public WorldRendererScissor(Vector4 value)
        {
            this.value = value;
        }

        public WorldRendererScissor(float x, float y, float width, float height)
        {
            value = new(x, y, width, height);
        }
    }
}
