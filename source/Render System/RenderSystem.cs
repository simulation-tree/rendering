using Collections;
using System;
using System.Numerics;
using Unmanaged;

namespace Rendering
{
    /// <summary>
    /// Represents an unmanaged <see cref="IRenderingBackend"/> instance.
    /// </summary>
    public struct RenderSystem : IDisposable
    {
        private bool hasSurface;
        public readonly nint library;
        public readonly List<Viewport> viewports;
        public readonly Dictionary<Viewport, Dictionary<RendererKey, List<uint>>> renderers;
        public readonly Dictionary<RendererKey, RendererCombination> infos;

        private readonly Allocation system;
        private readonly RenderSystemType type;

        public readonly bool IsSurfaceAvailable => hasSurface;

#if NET
        [Obsolete("Default constructor not supported", true)]
        public RenderSystem()
        {
            throw new NotImplementedException();
        }
#endif

        internal RenderSystem(CreateResult result, RenderSystemType type)
        {
            this.system = result.system;
            this.library = result.library;
            this.type = type;
            hasSurface = false;

            viewports = new();
            renderers = new();
            infos = new();
        }

        public readonly void Dispose()
        {
            type.destroy.Invoke(system);
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

        public void SurfaceCreated(nint surface)
        {
            type.surfaceCreated.Invoke(system, surface);
            hasSurface = true;
        }

        public readonly uint BeginRender(Vector4 clearColor)
        {
            return type.beginRender.Invoke(system, clearColor);
        }

        public unsafe readonly void Render(USpan<uint> renderers, uint material, uint shader, uint mesh)
        {
            type.render.Invoke(system, renderers, material, shader, mesh);
        }

        public readonly uint EndRender()
        {
            return type.endRender.Invoke(system);
        }
    }
}