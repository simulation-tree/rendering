using Simulation;

namespace Rendering.Systems
{
    public class RenderingSystems : SystemBase
    {
        public readonly RenderEngineSystem renderEngine;
        public readonly MaterialImportSystem materials;

        public RenderingSystems(World world) : base(world)
        {
            renderEngine = new(world);
            materials = new(world);
        }

        public override void Dispose()
        {
            materials.Dispose();
            renderEngine.Dispose();
            base.Dispose();
        }
    }
}