using Meshes;
using Simulation;

namespace Rendering.Components
{
    public struct IsRenderer
    {
        public eint mesh;
        public eint material;
        public eint camera;

        public IsRenderer(Mesh mesh, Material material, Camera camera)
        {
            this.mesh = mesh.GetEntityValue();
            this.material = material.GetEntityValue();
            this.camera = camera.GetEntityValue();
        }
    }
}