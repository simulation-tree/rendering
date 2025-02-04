using Unmanaged;
using Worlds;

namespace Rendering.Components
{
    [Component]
    public readonly struct SurfaceInUse
    {
        public readonly Allocation value;

        public SurfaceInUse(Allocation value)
        {
            this.value = value;
        }
    }
}