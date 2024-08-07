using Unmanaged;

namespace Rendering
{
    /// <summary>
    /// Disposes a previously created system instance, for a destination
    /// entity that has already been destroyed from its world.
    /// </summary>
    public unsafe readonly struct DestroyFunction
    {
        private readonly delegate* unmanaged<Allocation, void> function;

        public DestroyFunction(delegate* unmanaged<Allocation, void> function)
        {
            this.function = function;
        }

        public readonly void Invoke(Allocation system)
        {
            function(system);
        }
    }
}