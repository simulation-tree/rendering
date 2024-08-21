using Unmanaged;

namespace Rendering.Functions
{
    public unsafe readonly struct DisposeFunction
    {
#if NET
        private readonly delegate* unmanaged<Allocation, void> function;

        public DisposeFunction(delegate* unmanaged<Allocation, void> function)
        {
            this.function = function;
        }
#else
        private readonly delegate*<Allocation, void> function;

        public DisposeFunction(delegate*<Allocation, void> function)
        {
            this.function = function;
        }
#endif

        public readonly void Invoke(Allocation system)
        {
            function(system);
        }
    }
}