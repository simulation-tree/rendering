using Shaders;
using Simulation;

namespace Rendering.Components
{
    public struct IsMaterial
    {
        private eint shader;

        public IsMaterial(Shader shader)
        {
            this.shader = shader.entity.value;
        }

        public readonly Shader Get(World world)
        {
            return new(world, shader);
        }
    }
}
