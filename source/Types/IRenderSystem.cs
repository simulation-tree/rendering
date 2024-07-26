using System;
using Unmanaged;

namespace Rendering
{
    public interface IRenderSystem
    {
        FixedString Label { get; }
        
        void Initialize(ReadOnlySpan<FixedString> extensionName);
        void CreateInstance(Destination destination);
    }
}