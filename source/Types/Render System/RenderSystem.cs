using Simulation;
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
        private bool surfaceCreated;
        public readonly nint library;
        public readonly UnmanagedList<eint> cameras;
        public readonly UnmanagedDictionary<eint, UnmanagedDictionary<int, UnmanagedList<eint>>> renderers;
        public readonly UnmanagedDictionary<int, eint> materials;
        public readonly UnmanagedDictionary<int, eint> meshes;

        private readonly Allocation system;
        private readonly Allocation buffer;
        private readonly RenderSystemType type;

        public readonly bool IsSurfaceCreated => surfaceCreated;

        [Obsolete("Default constructor not supported", true)]
        public RenderSystem()
        {
            throw new NotImplementedException();
        }

        internal RenderSystem(CreateResult result, RenderSystemType type)
        {
            this.system = result.system;
            this.buffer = result.buffer;
            this.library = result.library;
            this.type = type;

            cameras = new();
            renderers = new();
            materials = new();
            meshes = new();
        }

        public readonly void Dispose()
        {
            type.destroy.Invoke(system);
            buffer.Dispose();
            materials.Dispose();
            meshes.Dispose();
            cameras.Dispose();

            foreach (eint cameraEntity in renderers.Keys)
            {
                UnmanagedDictionary<int, UnmanagedList<eint>> groups = renderers[cameraEntity];
                foreach (int hash in groups.Keys)
                {
                    UnmanagedList<eint> renderers = groups[hash];
                    renderers.Dispose();
                }

                groups.Dispose();
            }

            renderers.Dispose();
        }

        public void SurfaceCreated(nint surface)
        {
            type.surfaceCreated.Invoke(system, surface);
            surfaceCreated = true;
        }

        public readonly void BeginRender()
        {
            type.beginRender.Invoke(system, buffer);
        }

        public unsafe readonly void Render(ReadOnlySpan<eint> renderers, eint material, eint mesh, eint camera)
        {
            fixed (eint* entitiesPtr = renderers)
            {
                type.render.Invoke(system, (nint)entitiesPtr, renderers.Length, material, mesh, camera);
            }
        }

        public readonly void EndRender()
        {
            type.endRender.Invoke(system, buffer);
        }
    }
}