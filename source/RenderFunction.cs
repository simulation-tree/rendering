using Meshes;
using Simulation;
using Unmanaged;

namespace Rendering
{
    public unsafe readonly struct RenderFunction
    {
        private readonly delegate* unmanaged<Allocation, nint, int, Material, Mesh, Camera, eint, void> function;

        public RenderFunction(delegate* unmanaged<Allocation, nint, int, Material, Mesh, Camera, eint, void> function)
        {
            this.function = function;
        }

        public readonly void Invoke(Allocation renderer, nint entities, int entityCount, Material material, Mesh mesh, Camera camera, eint destination)
        {
            function(renderer, entities, entityCount, material, mesh, camera, destination);
        }
    }
}