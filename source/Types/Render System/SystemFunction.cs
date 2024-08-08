using Unmanaged;

namespace Rendering
{
    public unsafe readonly struct SystemFunction
    {
        private readonly delegate* unmanaged<Allocation, bool> function;

        public SystemFunction(delegate* unmanaged<Allocation, bool> function)
        {
            this.function = function;
        }

        public readonly bool Invoke(Allocation system)
        {
            return function(system);
        }
    }
}