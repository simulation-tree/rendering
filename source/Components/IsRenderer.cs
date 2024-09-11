using Simulation;

namespace Rendering.Components
{
    public struct IsRenderer
    {
        public rint meshReference;
        public rint materialReference;
        public rint cameraReference;

        public IsRenderer(rint mesh, rint material, rint camera)
        {
            this.meshReference = mesh;
            this.materialReference = material;
            this.cameraReference = camera;
        }
    }
}