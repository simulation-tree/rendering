using Unmanaged;

namespace Rendering.Components
{
    /// <summary>
    /// The instance of whatever API is used to handle rendering.
    /// <para>
    /// Created when a backend handles the creation of a renderer for this entity.
    /// </para>
    /// </summary>
    public readonly struct RendererInstanceInUse
    {
        public readonly MemoryAddress value;

        public RendererInstanceInUse(MemoryAddress value)
        {
            this.value = value;
        }
    }
}