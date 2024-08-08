using Unmanaged;

namespace Rendering
{
    public unsafe readonly struct DisposeFunction
    {
        private readonly delegate* unmanaged<Allocation, void> function;

        public DisposeFunction(delegate* unmanaged<Allocation, void> function)
        {
            this.function = function;
        }

        public readonly void Invoke(Allocation system)
        {
            function(system);
        }
    }
}