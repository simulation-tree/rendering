using Data;
using Unmanaged;

namespace Rendering.Functions
{
    public unsafe readonly struct BeginRenderFunction
    {
#if NET
        private readonly delegate* unmanaged<Allocation, Color, uint> function;

        public BeginRenderFunction(delegate* unmanaged<Allocation, Color, uint> function)
        {
            this.function = function;
        }
#else
        private readonly delegate*<Allocation, Color, uint> function;

        public BeginRenderFunction(delegate*<Allocation, Color, uint> function)
        {
            this.function = function;
        }
#endif

        public readonly uint Invoke(Allocation system, Color color)
        {
            return function(system, color);
        }
    }
}