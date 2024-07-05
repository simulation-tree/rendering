using Rendering.Components;
using Simulation;
using System;
using System.Numerics;
using Transforms.Components;

namespace Rendering
{
    public readonly struct Destination : IDisposable
    {
        public readonly Entity entity;

        public readonly bool IsDestroyed => entity.IsDestroyed;
        public readonly uint Width => entity.GetComponent<IsDestination>().width;
        public readonly uint Height => entity.GetComponent<IsDestination>().height;
        public readonly Vector4 Region => entity.GetComponent<IsDestination>().region;

        public Destination(World world, EntityID existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Destination(World world, uint width, uint height)
        {
            entity = new(world);
            entity.AddComponent(new IsDestination(width, height, new Vector4(0, 0, 1, 1)));
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public static Destination GetOrCreate(World world, EntityID window)
        {
            if (!world.ContainsComponent<IsDestination>(window))
            {
                Scale scale = world.GetComponent<Scale>(window);
                uint width = (uint)scale.value.X;
                uint height = (uint)scale.value.Y;
                world.AddComponent(window, new IsDestination(width, height, new Vector4(0, 0, 1, 1)));
            }

            return new(world, window);
        }
    }
}
