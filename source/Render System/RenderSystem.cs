using Data;
using System;
using Unmanaged;
using Unmanaged.Collections;

namespace Rendering
{
    /// <summary>
    /// Represents an unmanaged <see cref="IRenderSystem"/> instance.
    /// </summary>
    public struct RenderSystem : IDisposable
    {
        private bool hasSurface;
        public readonly nint library;
        public readonly UnmanagedList<uint> cameras;
        public readonly UnmanagedDictionary<uint, UnmanagedDictionary<int, UnmanagedList<uint>>> renderersPerCamera;
        public readonly UnmanagedDictionary<int, uint> materials;
        public readonly UnmanagedDictionary<int, uint> shaders;
        public readonly UnmanagedDictionary<int, uint> meshes;

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

            foreach (uint cameraEntity in renderersPerCamera.Keys)
            {
                UnmanagedDictionary<int, UnmanagedList<uint>> groups = renderersPerCamera[cameraEntity];
                foreach (int hash in groups.Keys)
                {
                    UnmanagedList<uint> renderers = groups[hash];
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