using Unmanaged;

namespace Rendering.Functions
{
    public unsafe readonly struct SystemFunction
    {
#if NET
        private readonly delegate* unmanaged<Allocation, uint> function;

        public SystemFunction(delegate* unmanaged<Allocation, uint> function)
        {
            this.function = function;
        }
#else
        private readonly delegate*<Allocation, uint> function;

        public SystemFunction(delegate*<Allocation, uint> function)
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