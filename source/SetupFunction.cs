using Simulation;
using Unmanaged;

namespace Rendering
{
    public unsafe readonly struct SetupFunction
    {
        private readonly delegate* unmanaged<World, nint, int, Allocation> function;

        public SetupFunction(delegate* unmanaged<World, nint, int, Allocation> function)
        {
            this.function = function;
        }

        public readonly Allocation Invoke(World world, nint names, int nameCount)
        {
            return function(world, names, nameCount);
        }
    }
}