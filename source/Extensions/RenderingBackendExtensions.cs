using Simulation;
using System.Numerics;
using Unmanaged;

namespace Rendering
{
    public static class RenderingBackendExtensions
    {
        public static void Initialize<T>(this ref T backend) where T : unmanaged, IRenderingBackend
        {
            backend.Initialize();
        }

        public static void PerformFinalize<T>(this ref T backend) where T : unmanaged, IRenderingBackend
        {
            backend.Finalize();
        }

        public static (Allocation renderer, Allocation instance) Create<T>(this ref T backend, in Destination destination, in USpan<FixedString> extensionNames) where T : unmanaged, IRenderingBackend
        {
            return backend.Create(destination, extensionNames);
        }

        public static void Dispose<T>(this ref T backend, in Allocation renderer) where T : unmanaged, IRenderingBackend
        {
            backend.Dispose(renderer);
        }

        public static void SurfaceCreated<T>(this ref T backend, in Allocation renderer, Allocation surface) where T : unmanaged, IRenderingBackend
        {
            backend.SurfaceCreated(renderer, surface);
        }

        public static StatusCode BeginRender<T>(this ref T backend, in Allocation renderer, in Vector4 clearColor) where T : unmanaged, IRenderingBackend
        {
            return backend.BeginRender(renderer, clearColor);
        }

        public static void Render<T>(this ref T backend, in Allocation renderer, in USpan<uint> entities, in uint material, in uint shader, in uint mesh) where T : unmanaged, IRenderingBackend
        {
            backend.Render(renderer, entities, material, shader, mesh);
        }

        public static void EndRender<T>(this ref T backend, in Allocation renderer) where T : unmanaged, IRenderingBackend
        {
            backend.EndRender(renderer);
        }
    }
}
