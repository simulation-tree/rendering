using Rendering.Functions;
using Unmanaged;

namespace Rendering
{
    public interface IRenderer
    {
        FixedString Label { get; }
        CreateFunction Create { get; }
        DisposeFunction Dispose { get; }
        FinishFunction Finish { get; }
        SurfaceCreatedFunction SurfaceCreated { get; }
        BeginRenderFunction BeginRender { get; }
        RenderFunction Render { get; }
        SystemFunction EndRender { get; }
    }
}