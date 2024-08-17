using Simulation;

namespace Rendering.Components
{
    public struct IsMaterial
    {
        public rint shaderReference;

        public IsMaterial(rint shaderReference)
        {
            this.shaderReference = shaderReference;
        }
    }
}
