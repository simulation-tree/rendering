using Worlds;

namespace Rendering.Components
{
    [Component]
    public struct IsRenderer
    {
        public rint meshReference;
        public rint materialReference;
        public uint mask;

        public IsRenderer(rint mesh, rint material, uint mask)
        {
            this.meshReference = mesh;
            this.materialReference = material;
            this.mask = mask;
        }
    }
}