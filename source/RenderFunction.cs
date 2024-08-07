using Meshes;
using Simulation;

namespace Rendering
{
    public unsafe readonly struct RenderFunction
    {
        private readonly delegate* unmanaged<World, nint, nint, int, Material, Mesh, Camera, eint, void> function;

        public RenderFunction(delegate* unmanaged<World, nint, nint, int, Material, Mesh, Camera, eint, void> function)
        {
            this.function = function;
        }

        public readonly void Invoke(World world, nint instance, nint entities, int entityCount, Material material, Mesh mesh, Camera camera, eint destination)
        {
            function(world, instance, entities, entityCount, material, mesh, camera, destination);
        }
    }
}