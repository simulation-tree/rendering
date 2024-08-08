using System;
using Unmanaged;

namespace Rendering
{
    public readonly struct CreateResult : IDisposable
    {
        public readonly Allocation system;
        public readonly nint library;

        private CreateResult(Allocation system, nint library)
        {
            this.system = system;
            this.library = library;
        }

        public readonly void Dispose()
        {
            system.Dispose();
        }

        public static CreateResult Create<T>(T system, nint library) where T : unmanaged
        {
            Allocation systemAlloc = Allocation.Create(system);
            return new(systemAlloc, library);
        }
    }
}