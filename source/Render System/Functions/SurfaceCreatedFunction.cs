using Unmanaged;

namespace Rendering.Functions
{
    public unsafe readonly struct SurfaceCreatedFunction
    {
#if NET
        private readonly delegate* unmanaged<Allocation, nint, void> function;

        public SurfaceCreatedFunction(delegate* unmanaged<Allocation, nint, void> function)
        {
            this.function = function;
        }
#else
        private readonly delegate*<Allocation, nint, void> function;

        public SurfaceCreatedFunction(delegate*<Allocation, nint, void> function)
        {
            this.function = function;
        }
#endif

        public readonly void Invoke(Allocation system, nint surface)
        {
            function(system, surface);
        }
    }
}