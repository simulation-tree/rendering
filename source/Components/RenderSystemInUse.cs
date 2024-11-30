using Worlds;

namespace Rendering.Components
{
    /// <summary>
    /// The address of the library used to handle rendering of a destination.
    /// </summary>
    [Component]
    public readonly struct RenderSystemInUse
    {
        public readonly nint address;

        public RenderSystemInUse(nint address)
        {
            this.address = address;
        }
    }
}