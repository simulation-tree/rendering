using Meshes;
using Simulation;

namespace Rendering.Components
{
    public readonly struct IsRenderer
    {
        public readonly eint mesh;
        public readonly eint material;
        public readonly eint camera;

        public IsRenderer(Mesh mesh, Material material, Camera camera)
        {
            this.mesh = mesh.entity.value;
            this.material = material.entity.value;
            this.camera = camera.entity.value;
        }
    }
}