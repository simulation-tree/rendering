using Simulation;

namespace Rendering.Components
{
    public struct IsRenderer
    {
        public rint mesh;
        public rint material;
        public rint camera;

        public IsRenderer(rint mesh, rint material, rint camera)
        {
            this.mesh = mesh;
            this.material = material;
            this.camera = camera;
        }
    }
}