using Unmanaged;

namespace Rendering
{
    public unsafe readonly struct SystemFunction
    {
        private readonly delegate* unmanaged<Allocation, Allocation, void> function;

        public SystemFunction(delegate* unmanaged<Allocation, Allocation, void> function)
        {
            this.function = function;
        }

        public readonly void Invoke(Allocation system, Allocation buffer)
        {
            function(system, buffer);
        }
    }
}