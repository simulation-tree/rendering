using System;
using Unmanaged;

namespace Rendering
{
    public readonly struct CreateResult : IDisposable
    {
        public readonly Allocation system;
        public readonly Allocation buffer;
        public readonly nint library;

        private CreateResult(Allocation system, Allocation buffer, nint library)
        {
            this.system = system;
            this.buffer = buffer;
            this.library = library;
        }

        public readonly void Dispose()
        {
            system.Dispose();
        }

        public static CreateResult Create<T>(T system, Allocation buffer, nint library) where T : unmanaged
        {
            Allocation systemAlloc = Allocation.Create(system);
            return new(systemAlloc, buffer, library);
        }
    }
}