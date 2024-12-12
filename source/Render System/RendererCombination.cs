namespace Rendering
{
    public struct RendererCombination
    {
        public uint material;
        public uint shader;
        public uint mesh;

        public RendererCombination(uint material, uint shader, uint mesh)
        {
            this.material = material;
            this.shader = shader;
            this.mesh = mesh;
        }
    }
}