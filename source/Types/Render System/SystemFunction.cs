using Unmanaged;

namespace Rendering
{
    public unsafe readonly struct SystemFunction
    {
        private readonly delegate* unmanaged<Allocation, uint> function;

        public SystemFunction(delegate* unmanaged<Allocation, uint> function)
        {
            this.function = function;
        }

        public readonly uint Invoke(Allocation system)
        {
            return function(system);
        }
    }
}