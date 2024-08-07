using Simulation;
using Unmanaged;

namespace Rendering
{
    /// <summary>
    /// Renders a batch of entities using the same material, mesh and camera combination.
    /// </summary>
    public unsafe readonly struct RenderFunction
    {
        private readonly delegate* unmanaged<Allocation, nint, nint, int, eint, eint, eint, void> function;

        public RenderFunction(delegate* unmanaged<Allocation, nint, nint, int, eint, eint, eint, void> function)
        {
            this.function = function;
        }

        public readonly void Invoke(Allocation system, nint surface, nint entities, int entityCount, eint material, eint mesh, eint camera)
        {
            function(system, surface, entities, entityCount, material, mesh, camera);
        }
    }
}