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

        public unsafe RenderEnginePlugin(World world, delegate* unmanaged<RenderEnginePluginFunction.Input, void> function)
        {
            this.world = world;
            value = world.CreateEntity(new RenderEnginePluginFunction(function));
        }

        void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<RenderEnginePluginFunction>();
        }

        public static RenderEnginePlugin Create(World world, RenderEnginePluginFunction function)
        {
            return new(world, function);
        }

        public unsafe static RenderEnginePlugin Create(World world, delegate* unmanaged<RenderEnginePluginFunction.Input, void> function)
        {
            return new(world, function);
        }
    }
}