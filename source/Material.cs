using Materials;
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

        public Material()
        {
            throw new InvalidOperationException("Cannot create a material without a world.");
        }

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

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        public static Query GetQuery(World world)
        {
            return new(world, RuntimeType.Get<IsMaterial>());
        }
    }
}
