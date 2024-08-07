using Unmanaged;

namespace Rendering
{
    public interface IRenderSystem
    {
        static abstract FixedString Label { get; }

        /// <summary>
        /// Initializes the renderer.
        /// </summary>
        static abstract (SetupFunction setup, DisposeFunction tearDown, RenderFunction render) GetFunctions();
    }
}