using Unmanaged;

namespace Rendering.Components
{
    public readonly struct SurfaceInUse
    {
        public readonly MemoryAddress value;

        public SurfaceInUse(MemoryAddress value)
        {
            this.value = value;
        }
    }
}