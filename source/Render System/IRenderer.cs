using Rendering.Functions;
using Unmanaged;

namespace Rendering
{
    public interface IRenderer
    {
        FixedString Label { get; }
        CreateFunction Create { get; }
        DisposeFunction Dispose { get; }
        FinishRenderer Finish { get; }
        SurfaceCreatedFunction SurfaceCreated { get; }
        BeginRenderFunction BeginRender { get; }
        RenderFunction Render { get; }
        EndRenderFunction EndRender { get; }
    }
}