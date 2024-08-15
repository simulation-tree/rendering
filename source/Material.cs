using Data.Components;
using Data.Events;
using Rendering.Components;
using Shaders;
using Simulation;
using System;
using Unmanaged;

namespace Rendering
{
    public readonly struct Material : IMaterial, IDisposable
    {
        private readonly Entity entity;

        eint IEntity.Value => entity.value;
        World IEntity.World => entity.world;

#if NET
        [Obsolete("Default constructor not available", true)]
        public Material()
        {
            throw new InvalidOperationException("Cannot create a material without a world.");
        }
#endif

        public Material(World world, eint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Material(World world, Shader shader)
        {
            entity = new(world);
            entity.AddComponent(new IsMaterial(shader));
            entity.CreateList<Entity, MaterialPushBinding>();
            entity.CreateList<Entity, MaterialComponentBinding>();
            entity.CreateList<Entity, MaterialTextureBinding>();
        }

        public Material(World world, ReadOnlySpan<char> address)
        {
            entity = new(world);
            entity.AddComponent(new IsMaterial(default));
            entity.AddComponent(new IsDataRequest(address));
            entity.CreateList<Entity, MaterialPushBinding>();
            entity.CreateList<Entity, MaterialComponentBinding>();
            entity.CreateList<Entity, MaterialTextureBinding>();

            world.Submit(new DataUpdate());
            world.Poll();
        }

        public Material(World world, FixedString address)
        {
            entity = new(world);
            entity.AddComponent(new IsMaterial(default));
            entity.AddComponent(new IsDataRequest(address));
            entity.CreateList<Entity, MaterialPushBinding>();
            entity.CreateList<Entity, MaterialComponentBinding>();
            entity.CreateList<Entity, MaterialTextureBinding>();

            world.Submit(new DataUpdate());
            world.Poll();
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        Query IEntity.GetQuery(World world)
        {
            return new(world, RuntimeType.Get<IsMaterial>());
        }
    }
}
