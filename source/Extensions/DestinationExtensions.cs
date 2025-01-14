using Rendering.Components;
using System.Numerics;
using Unmanaged;
using Worlds;

namespace Rendering
{
    public static class DestinationExtensions
    {
        public static bool TryGetRendererInstanceInUse<T>(this T destination, out Allocation instance) where T : unmanaged, IDestination
        {
            Entity entity = destination.AsEntity();
            ref RendererInstanceInUse component = ref entity.TryGetComponent<RendererInstanceInUse>(out bool contains);
            if (contains)
            {
                instance = component.value;
                return true;
            }
            else
            {
                instance = default;
                return false;
            }
        }

        public static bool TryGetSurfaceInUse<T>(this T window, out Allocation surface) where T : unmanaged, IDestination
        {
            Entity entity = window.AsEntity();
            ref SurfaceInUse component = ref entity.TryGetComponent<SurfaceInUse>(out bool contains);
            if (contains)
            {
                surface = component.value;
                return true;
            }
            else
            {
                surface = default;
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

        public static void SetClearColor<T>(this T destination, Vector4 clearColor) where T : unmanaged, IDestination
        {
            Entity entity = destination.AsEntity();
            ref IsDestination component = ref entity.GetComponent<IsDestination>();
            component.clearColor = clearColor;
        }

        public static FixedString GetRendererLabel<T>(this T destination) where T : unmanaged, IDestination
        {
            Entity entity = destination.AsEntity();
            return entity.GetComponent<IsDestination>().rendererLabel;
        }
    }
}
