using System;
using Worlds;

namespace Rendering.Functions
{
    public unsafe readonly struct RenderEnginePluginFunction : IEquatable<RenderEnginePluginFunction>
    {
#if NET
        private readonly delegate* unmanaged<Input, void> function;

        public RenderEnginePluginFunction(delegate* unmanaged<Input, void> function)
        {
            this.function = function;
        }
#else
        private readonly delegate*<Input, void> function;

        public PluginBeforeRendering(delegate*<Input, void> function)
        {
            this.function = function;
        }
#endif

        public readonly void Invoke(World world, sbyte renderGroup, Span<RenderEntity> entities)
        {
            function(new(world, renderGroup, entities));
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is RenderEnginePluginFunction plugin && Equals(plugin);
        }

        public readonly bool Equals(RenderEnginePluginFunction other)
        {
            return (nint)function == (nint)other.function;
        }

        public readonly override int GetHashCode()
        {
            return ((nint)function).GetHashCode();
        }

        public static bool operator ==(RenderEnginePluginFunction left, RenderEnginePluginFunction right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RenderEnginePluginFunction left, RenderEnginePluginFunction right)
        {
            return !(left == right);
        }

        public readonly struct Input
        {
            public readonly World world;
            public readonly sbyte renderGroup;

            private readonly RenderEntity* entities;
            private readonly int count;

            public readonly Span<RenderEntity> Entities => new(entities, count);

            public Input(World world, sbyte renderGroup, Span<RenderEntity> entities)
            {
                this.world = world;
                this.renderGroup = renderGroup;
                this.entities = entities.GetPointer();
                count = entities.Length;
            }
        }
    }
}