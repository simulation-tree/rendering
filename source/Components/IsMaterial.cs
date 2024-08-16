namespace Rendering.Components
{
    public struct IsMaterial
    {
        public uint shaderReference;

        public IsMaterial(uint shaderReference)
        {
            this.shaderReference = shaderReference;
        }
    }
}
