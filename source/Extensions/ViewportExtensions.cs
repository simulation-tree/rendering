using Rendering.Components;
using System.Numerics;
using Worlds;

namespace Rendering
{
    public static class ViewportExtensions
    {
        public static ref sbyte GetOrder<T>(this ref T viewport) where T : unmanaged, IViewport
        {
            Entity entity = viewport.AsEntity();
            return ref entity.GetComponent<IsViewport>().order;
        }

        public static ref Vector4 GetOutputRegion<T>(this ref T viewport) where T : unmanaged, IViewport
        {
            Entity entity = viewport.AsEntity();
            return ref entity.GetComponent<IsViewport>().region;
        }

        public static ref uint GetMask<T>(this ref T viewport) where T : unmanaged, IViewport
        {
            Entity entity = viewport.AsEntity();
            return ref entity.GetComponent<IsViewport>().mask;
        }
    }
}
