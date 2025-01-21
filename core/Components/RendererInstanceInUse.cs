using Unmanaged;
using Worlds;

namespace Rendering.Components
{
    /// <summary>
    /// The instance of whatever API is used to handle rendering.
    /// <para>
    /// Created when <see cref="IRenderingBackend.Create"/> is called.
    /// </para>
    /// </summary>
    [Component]
    public readonly struct RendererInstanceInUse
    {
        public readonly Allocation value;

        public RendererInstanceInUse(Allocation value)
        {
            this.value = value;
        }
    }
}