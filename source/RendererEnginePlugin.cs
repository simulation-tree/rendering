using Rendering.Functions;
using Worlds;

namespace Rendering
{
    public readonly partial struct RenderEnginePlugin : IEntity
    {
        public RenderEnginePlugin(World world, RenderEnginePluginFunction function)
        {
            this.world = world;
            value = world.CreateEntity(function);
        }

        void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<RenderEnginePluginFunction>();
        }
    }
}