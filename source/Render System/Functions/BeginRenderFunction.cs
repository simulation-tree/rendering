using System.Numerics;
using Unmanaged;

namespace Rendering.Functions
{
    public unsafe readonly struct BeginRenderFunction
    {
#if NET
        private readonly delegate* unmanaged<Allocation, Vector4, uint> function;

        public BeginRenderFunction(delegate* unmanaged<Allocation, Vector4, uint> function)
        {
            this.function = function;
        }
#else
        private readonly delegate*<Allocation, Vector4, uint> function;

        public BeginRenderFunction(delegate*<Allocation, Vector4, uint> function)
        {
            this.function = function;
        }
#endif

        public readonly uint Invoke(Allocation system, Vector4 color)
        {
            return function(system, color);
        }
    }
}