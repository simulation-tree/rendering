using Collections;
using Data;
using Simulation;
using System;
using Unmanaged;

namespace Rendering
{
    /// <summary>
    /// Represents an unmanaged <see cref="IRenderer"/> instance.
    /// </summary>
    public struct RenderSystem : IDisposable
    {
        private bool hasSurface;
        public readonly nint library;
        public readonly List<Entity> cameras;
        public readonly Dictionary<Entity, Dictionary<int, List<uint>>> renderersPerCamera;
        public readonly Dictionary<int, Entity> materials;
        public readonly Dictionary<int, Entity> shaders;
        public readonly Dictionary<int, Entity> meshes;

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

            cameras = new();
            renderersPerCamera = new();
            materials = new();
            shaders = new();
            meshes = new();
        }

        public readonly void Dispose()
        {
            type.destroy.Invoke(system);
            materials.Dispose();
            shaders.Dispose();
            meshes.Dispose();
            cameras.Dispose();

            foreach (Entity camera in renderersPerCamera.Keys)
            {
                Dictionary<int, List<uint>> groups = renderersPerCamera[camera];
                foreach (int hash in groups.Keys)
                {
                    List<uint> renderers = groups[hash];
                    renderers.Dispose();
                }

                groups.Dispose();
            }

            renderersPerCamera.Dispose();
        }

        public void SurfaceCreated(nint surface)
        {
            type.surfaceCreated.Invoke(system, surface);
            hasSurface = true;
        }

        public readonly uint BeginRender(Color clearColor)
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