using Simulation;

namespace Rendering.Components
{
    public struct IsRenderer
    {
        public eint mesh;
        public eint material;
        public eint camera;

        public IsRenderer(eint mesh, eint material, eint camera)
        {
            this.mesh = mesh;
            this.material = material;
            this.camera = camera;
        }
    }
}