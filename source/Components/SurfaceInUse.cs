using Unmanaged;

namespace Rendering.Components
{
    public readonly struct SurfaceInUse
    {
        public readonly Allocation value;

        public SurfaceInUse(Allocation value)
        {
            this.value = value;
        }
    }
}