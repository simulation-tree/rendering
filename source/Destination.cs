using Rendering.Components;
using Simulation;
using System;
using System.Numerics;
using Unmanaged;

namespace Rendering
{
    public readonly struct Destination : IDestination, IDisposable
    {
        private readonly Entity entity;

        World IEntity.World => entity.world;
        eint IEntity.Value => entity.value;

        public Destination(World world, eint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Destination(World world, Vector2 size, FixedString renderer)
        {
            entity = new(world);
            entity.AddComponent(new IsDestination(size, new Vector4(0, 0, 1, 1), renderer));
            world.CreateList<Extension>(entity.value);
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        Query IEntity.GetQuery(World world)
        {
            return new(world, RuntimeType.Get<IsDestination>());
        }

        public readonly struct Extension
        {
            public readonly FixedString text;

            public Extension(FixedString text)
            {
                this.text = text;
            }

            public Extension(ReadOnlySpan<char> text)
            {
                this.text = new(text);
            }
        }
    }
}
