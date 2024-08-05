using System;
using Unmanaged;

namespace Rendering
{
    public interface IRenderSystem : IDisposable
    {
        static abstract FixedString Label { get; }

        void Initialize(ReadOnlySpan<FixedString> extensionName);
        void Render(Destination destination, Camera camera, ReadOnlySpan<Renderer> entities);
    }
}