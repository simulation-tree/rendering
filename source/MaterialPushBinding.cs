using Unmanaged;

namespace Rendering
{
    /// <summary>
    /// Links a component on the render entity to a push constant in the shader.
    /// </summary>
    public struct MaterialPushBinding
    {
        public uint start;
        public RuntimeType componentType;
        public RenderStage stage;

        public MaterialPushBinding(uint start, RuntimeType componentType, RenderStage stage)
        {
            this.start = start;
            this.componentType = componentType;
            this.stage = stage;
        }
    }
}
