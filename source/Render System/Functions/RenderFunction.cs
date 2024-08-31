using Unmanaged;

namespace Rendering.Functions
{
    /// <summary>
    /// Renders a batch of entities using the same material, mesh and camera combination.
    /// </summary>
    public unsafe readonly struct RenderFunction
    {
#if NET
        private readonly delegate* unmanaged<Allocation, void*, int, uint, uint, uint, void> function;

        public RenderFunction(delegate* unmanaged<Allocation, void*, int, uint, uint, uint, void> function)
        {
            this.function = function;
        }
#else
        private readonly delegate*<Allocation, void*, int, uint, uint, uint, void> function;

        public RenderFunction(delegate*<Allocation, void*, int, uint, uint, uint, void> function)
        {
            this.function = function;
        }
#endif

        public readonly void Invoke(Allocation system, void* entities, int entityCount, uint material, uint shader, uint mesh)
        {
            function(system, entities, entityCount, material, shader, mesh);
        }
    }
}