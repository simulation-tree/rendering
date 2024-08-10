using Shaders;
using Unmanaged;

namespace Materials
{
    /// <summary>
    /// Links a component on the render entity to a push constant in the shader.
    /// </summary>
    public struct MaterialPushBinding
    {
        public uint start;
        public RuntimeType componentType;
        public ShaderStage stage;

        public MaterialPushBinding(uint start, RuntimeType componentType, ShaderStage stage)
        {
            this.start = start;
            this.componentType = componentType;
            this.stage = stage;
        }
    }
}
