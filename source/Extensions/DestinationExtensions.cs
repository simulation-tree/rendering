using Rendering.Components;
using System.Numerics;
using Unmanaged;
using Worlds;

namespace Rendering
{
    public static class DestinationExtensions
    {
        public static bool TryGetRenderSystemInUse<T>(this T destination, out RenderSystemInUse renderSystemInUse) where T : unmanaged, IDestination
        {
            Entity entity = destination.AsEntity();
            ref RenderSystemInUse component = ref entity.TryGetComponent<RenderSystemInUse>(out bool contains);
            if (contains)
            {
                renderSystemInUse = component;
                return true;
            }
            else
            {
                renderSystemInUse = default;
                return false;
            }
        }

        public static bool TryGetSurfaceReference<T>(this T window, out SurfaceReference reference) where T : unmanaged, IDestination
        {
            Entity entity = window.AsEntity();
            ref SurfaceReference component = ref entity.TryGetComponent<SurfaceReference>(out bool contains);
            if (contains)
            {
                reference = component;
                return true;
            }
            else
            {
                reference = default;
                return false;
            }
        }

        public static Vector2 SizeAsVector2<T>(this T destination) where T : unmanaged, IDestination
        {
            Entity entity = destination.AsEntity();
            IsDestination component = entity.GetComponent<IsDestination>();
            return new Vector2(component.width, component.height);
        }

        public static ref Vector4 GetClearColor<T>(this T destination) where T : unmanaged, IDestination
        {
            Entity entity = destination.AsEntity();
            return ref entity.GetComponent<IsDestination>().clearColor;
        }

        public static FixedString GetRendererLabel<T>(this T destination) where T : unmanaged, IDestination
        {
            Entity entity = destination.AsEntity();
            return entity.GetComponent<IsDestination>().rendererLabel;
        }
    }
}
