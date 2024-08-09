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
            this.mesh = mesh.GetEntityValue();
            this.material = material.GetEntityValue();
            this.camera = camera.GetEntityValue();
        }
    }
}