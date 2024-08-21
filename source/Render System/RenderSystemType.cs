using Rendering.Functions;
using System;
using Unmanaged;

namespace Rendering
{
    /// <summary>
    /// Defines a <see cref="IRenderSystem"/> type using functions.
    /// </summary>
    public readonly struct RenderSystemType : IDisposable
    {
        public readonly FixedString label;
        public readonly CreateFunction create;
        public readonly DisposeFunction destroy;
        public readonly RenderFunction render;
        public readonly FinishFunction finish;
        public readonly SurfaceCreatedFunction surfaceCreated;
        public readonly SystemFunction beginRender;
        public readonly SystemFunction endRender;

        private RenderSystemType(FixedString label, (CreateFunction, DisposeFunction, RenderFunction, FinishFunction, SurfaceCreatedFunction, SystemFunction, SystemFunction) functions)
        {
            this.label = label;
            this.create = functions.Item1;
            this.destroy = functions.Item2;
            this.render = functions.Item3;
            this.finish = functions.Item4;
            this.surfaceCreated = functions.Item5;
            this.beginRender = functions.Item6;
            this.endRender = functions.Item7;
        }

        /// <summary>
        /// Cleans up and disposes by invoking the <see cref="FinishFunction"/>
        /// on the render system type itself.
        /// </summary>
        public readonly void Dispose()
        {
            finish.Invoke();
        }

        /// <summary>
        /// Constructs an instance of this type.
        /// </summary>
        public unsafe readonly RenderSystem Create(Destination destination, ReadOnlySpan<FixedString> extensionNames)
        {
            fixed (FixedString* names = extensionNames)
            {
                CreateResult result = create.Invoke(destination, (nint)names, extensionNames.Length);
                return new(result, this);
            }
        }

        public static RenderSystemType Create<T>() where T : unmanaged, IRenderSystem
        {
            T v = default;
            return new(v.Label, v.GetFunctions());
        }
    }
}