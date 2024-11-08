using Unmanaged;

namespace Rendering.Functions
{
    public unsafe readonly struct EndRenderFunction
    {
#if NET
        private readonly delegate* unmanaged<Allocation, uint> function;

        public EndRenderFunction(delegate* unmanaged<Allocation, uint> function)
        {
            this.function = function;
        }
#else
        private readonly delegate*<Allocation, uint> function;

        public EndRenderFunction(delegate*<Allocation, uint> function)
        {
            this.function = function;
        }
#endif

        public readonly uint Invoke(Allocation system)
        {
            return function(system);
        }
    }
}