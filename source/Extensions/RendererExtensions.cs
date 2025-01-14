using Rendering.Components;
using Worlds;

namespace Rendering
{
    public static class RendererExtensions
    {
        public static ref LayerMask GetRenderMask<T>(this T renderer) where T : unmanaged, IRenderer
        {
            Entity entity = renderer.AsEntity();
            return ref entity.GetComponent<IsRenderer>().renderMask;
        }

        public static void SetRenderMask<T>(this T renderer, LayerMask renderMask) where T : unmanaged, IRenderer
        {
            Entity entity = renderer.AsEntity();
            ref IsRenderer component = ref entity.GetComponent<IsRenderer>();
            component.renderMask = renderMask;
        }
    }
}
