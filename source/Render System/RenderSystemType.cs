using Rendering.Functions;
using System;
using Unmanaged;

namespace Rendering
{
    /// <summary>
    /// Defines a <see cref="IRenderingBackend"/> type using functions.
    /// </summary>
    public readonly struct RenderSystemType : IDisposable
    {
        public readonly FixedString label;
        public readonly CreateFunction create;
        public readonly DisposeFunction destroy;
        public readonly RenderFunction render;
        public readonly FinishRenderer finish;
        public readonly SurfaceCreatedFunction surfaceCreated;
        public readonly BeginRenderFunction beginRender;
        public readonly EndRenderFunction endRender;

        private RenderSystemType(FixedString label, CreateFunction create, DisposeFunction dispose, RenderFunction render, FinishRenderer finish, SurfaceCreatedFunction surfaceCreated, BeginRenderFunction beginRender, EndRenderFunction endRender)
        {
            this.label = label;
            this.create = create;
            this.destroy = dispose;
            this.render = render;
            this.finish = finish;
            this.surfaceCreated = surfaceCreated;
            this.beginRender = beginRender;
            this.endRender = endRender;
        }

        /// <summary>
        /// Cleans up and disposes by invoking the <see cref="FinishRenderer"/>
        /// on the render system type itself.
        /// </summary>
        public readonly void Dispose()
        {
            finish.Invoke();
        }

        /// <summary>
        /// Constructs an instance of this type.
        /// </summary>
        public unsafe readonly RenderSystem Create(Destination destination, USpan<FixedString> extensionNames)
        {
            CreateResult result = create.Invoke(destination, extensionNames);
            return new(result, this);
        }

        public static RenderSystemType Create<T>() where T : unmanaged, IRenderingBackend
        {
            T v = default;
            return new(v.Label, v.Create, v.Dispose, v.Render, v.Finish, v.SurfaceCreated, v.BeginRender, v.EndRender);
        }
    }
}