using Simulation;
using System;
using System.Formats.Asn1;
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
        public readonly UnmanagedList<eint> cameras;
        public readonly UnmanagedDictionary<eint, UnmanagedDictionary<int, UnmanagedList<eint>>> renderersPerCamera;
        public readonly UnmanagedDictionary<int, eint> materials;
        public readonly UnmanagedDictionary<int, eint> shaders;
        public readonly UnmanagedDictionary<int, eint> meshes;

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

            foreach (eint cameraEntity in renderersPerCamera.Keys)
            {
                UnmanagedDictionary<int, UnmanagedList<eint>> groups = renderersPerCamera[cameraEntity];
                foreach (int hash in groups.Keys)
                {
                    UnmanagedList<eint> renderers = groups[hash];
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

        public readonly uint BeginRender()
        {
            return type.beginRender.Invoke(system);
        }

        public unsafe readonly void Render(ReadOnlySpan<eint> renderers, eint material, eint shader, eint mesh)
        {
            fixed (eint* entitiesPtr = renderers)
            {
                type.render.Invoke(system, (nint)entitiesPtr, renderers.Length, material, shader, mesh);
            }
        }

        public readonly uint EndRender()
        {
            return type.endRender.Invoke(system);
        }
    }
}