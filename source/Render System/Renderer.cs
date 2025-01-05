using Collections;
using Simulation;
using System;
using System.Numerics;
using Unmanaged;

namespace Rendering
{
    /// <summary>
    /// Represents a handler of <see cref="IRenderingBackend"/> functions.
    /// </summary>
    public struct Renderer : IDisposable
    {
        private bool hasSurface;
        public readonly List<Viewport> viewports;
        public readonly Dictionary<Viewport, Dictionary<RendererKey, List<uint>>> renderers;
        public readonly Dictionary<RendererKey, RendererCombination> infos;

        private readonly Allocation allocation;
        private readonly RenderingBackend backend;

        public readonly bool IsSurfaceAvailable => hasSurface;

#if NET
        [Obsolete("Default constructor not supported", true)]
        public Renderer()
        {
            throw new NotImplementedException();
        }
#endif

        internal Renderer(Allocation allocation, RenderingBackend backend)
        {
            this.allocation = allocation;
            this.backend = backend;
            hasSurface = false;

            viewports = new();
            renderers = new();
            infos = new();
        }

        public readonly void Dispose()
        {
            backend.dispose.Invoke(backend.allocation, allocation);
            infos.Dispose();
            viewports.Dispose();

            foreach (Viewport viewport in renderers.Keys)
            {
                Dictionary<RendererKey, List<uint>> groups = renderers[viewport];
                foreach (RendererKey hash in groups.Keys)
                {
                    List<uint> renderers = groups[hash];
                    renderers.Dispose();
                }

                groups.Dispose();
            }

            renderers.Dispose();
        }

        public void SurfaceCreated(Allocation surface)
        {
            backend.surfaceCreated.Invoke(backend.allocation, allocation, surface);
            hasSurface = true;
        }

        public readonly StatusCode BeginRender(Vector4 clearColor)
        {
            return backend.beginRender.Invoke(backend.allocation, allocation, clearColor);
        }

        public unsafe readonly void Render(USpan<uint> renderers, uint material, uint shader, uint mesh)
        {
            backend.render.Invoke(backend.allocation, allocation, renderers, material, shader, mesh);
        }

        public readonly void EndRender()
        {
            backend.endRender.Invoke(backend.allocation, allocation);
        }
    }
}