using Worlds;

namespace Rendering
{
    /// <summary>
    /// Links a component on the render entity to a push constant in the shader.
    /// </summary>
    [ArrayElement]
    public struct MaterialPushBinding
    {
        public uint start;
        public ComponentType componentType;
        public RenderStage stage;

        public MaterialPushBinding(uint start, ComponentType componentType, RenderStage stage)
        {
            this.start = start;
            this.componentType = componentType;
            this.stage = stage;
        }
    }
}
