using Simulation;
using Unmanaged;

namespace Rendering
{
    /// <summary>
    /// Renders a batch of entities using the same material, mesh and camera combination.
    /// </summary>
    public unsafe readonly struct RenderFunction
    {
        private readonly delegate* unmanaged<Allocation, nint, int, eint, eint, eint, void> function;

        public RenderFunction(delegate* unmanaged<Allocation, nint, int, eint, eint, eint, void> function)
        {
            this.function = function;
        }

        public readonly void Invoke(Allocation system, nint entities, int entityCount, eint material, eint shader, eint mesh)
        {
            function(system, entities, entityCount, material, shader, mesh);
        }
    }
}