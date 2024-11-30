using Worlds;

namespace Rendering.Components
{
    [Component]
    public struct IsMaterial
    {
        public rint shaderReference;

        public IsMaterial(rint shaderReference)
        {
            this.shaderReference = shaderReference;
        }
    }
}
