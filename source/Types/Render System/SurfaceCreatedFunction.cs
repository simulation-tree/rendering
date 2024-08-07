using Unmanaged;

namespace Rendering
{
    public unsafe readonly struct SurfaceCreatedFunction
    {
        private readonly delegate* unmanaged<Allocation, nint, void> function;

        public SurfaceCreatedFunction(delegate* unmanaged<Allocation, nint, void> function)
        {
            this.function = function;
        }

        public readonly void Invoke(Allocation system, nint surface)
        {
            function(system, surface);
        }
    }
}