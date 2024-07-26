using Simulation;
using System;
using Unmanaged;

namespace Rendering
{
    public interface IRenderSystem
    {
        static abstract FixedString Label { get; }

        void Initialize(eint entity, ReadOnlySpan<FixedString> extensionName);
    }
}