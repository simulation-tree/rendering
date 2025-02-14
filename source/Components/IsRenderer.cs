using Worlds;

namespace Rendering.Components
{
    public struct IsRenderer
    {
        public rint meshReference;
        public rint materialReference;
        public LayerMask renderMask;

        public IsRenderer(rint mesh, rint material, LayerMask renderMask)
        {
            this.meshReference = mesh;
            this.materialReference = material;
            this.renderMask = renderMask;
        }
    }
}