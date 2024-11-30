using Worlds;

namespace Rendering.Components
{
    [Component]
    public readonly struct SurfaceReference
    {
        public readonly nint address;

        public SurfaceReference(nint address)
        {
            this.address = address;
        }
    }
}