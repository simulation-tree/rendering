namespace Rendering
{
    public readonly struct RendererData
    {
        public readonly uint entity;
        public readonly uint version;

        public RendererData(uint entity, uint version)
        {
            this.entity = entity;
            this.version = version;
        }
    }
}