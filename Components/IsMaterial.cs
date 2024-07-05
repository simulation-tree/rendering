using Game;

namespace Materials
{
    public struct IsMaterial
    {
        public EntityID shader;

        public IsMaterial(EntityID shader)
        {
            this.shader = shader;
        }
    }
}
