using Rendering.Functions;
using Unmanaged;

namespace Rendering
{
    public interface IRenderSystem
    {
        FixedString Label { get; }

        /// <summary>
        /// Retrieve the functions that compose a "render system".
        /// <para>Called just once, and balanced by the <see cref="FinishFunction"/>.</para>
        /// </summary>
        (CreateFunction, DisposeFunction, RenderFunction, FinishFunction, SurfaceCreatedFunction, BeginRenderFunction, SystemFunction) GetFunctions();
    }
}