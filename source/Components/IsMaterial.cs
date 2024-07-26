using Simulation;

namespace Rendering.Components
{
    public struct IsMaterial
    {
        public eint shader;

        public IsMaterial(eint shader)
        {
            this.shader = shader;
        }
    }
}
