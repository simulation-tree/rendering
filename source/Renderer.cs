using Meshes;
using Rendering.Components;
using Simulation;
using System;
using Unmanaged;

namespace Rendering
{
    public readonly struct Renderer : IRenderer, IDisposable
    {
        private readonly Entity entity;

        eint IEntity.Value => entity.value;
        World IEntity.World => entity.world;

        public Renderer(World world, eint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Renderer(World world, Mesh mesh, Material material, Camera camera)
        {
            entity = new(world);
            entity.AddComponent(new IsRenderer(mesh, material, camera));
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
            return new(world, RuntimeType.Get<IsRenderer>());
        }
    }
}
