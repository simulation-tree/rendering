using Shaders;
using Simulation;

namespace Rendering.Components
{
    public struct IsMaterial
    {
        public eint shader;

        public IsMaterial(Shader shader)
        {
            this.shader = shader.GetEntityValue();
        }
    }
}
