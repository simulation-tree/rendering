using Meshes;
using Rendering.Components;
using Simulation;
using System;
using Unmanaged;

namespace Rendering
{
    public readonly struct Renderer : IEntity, IDisposable
    {
        private readonly Entity entity;

        public readonly bool IsEnabled
        {
            get => entity.IsEnabled;
            set => entity.IsEnabled = value;
        }

        public readonly Material Material
        {
            get
            {
                IsRenderer component = entity.GetComponent<IsRenderer>();
                eint materialEntity = entity.GetReference(component.material);
                return new(entity, materialEntity);
            }
            set
            {
                ref IsRenderer component = ref entity.GetComponent<IsRenderer>();
                if (entity.ContainsReference(component.material))
                {
                    entity.SetReference(component.material, value);
                }
                else
                {
                    component.material = entity.AddReference(value);
                }
            }
        }

        public readonly Mesh Mesh
        {
            get
            {
                IsRenderer component = entity.GetComponent<IsRenderer>();
                eint meshEntity = entity.GetReference(component.mesh);
                return new Entity(entity, meshEntity).As<Mesh>();
            }
            set
            {
                ref IsRenderer component = ref entity.GetComponent<IsRenderer>();
                if (entity.ContainsReference(component.mesh))
                {
                    entity.SetReference(component.mesh, value);
                }
                else
                {
                    component.mesh = entity.AddReference(value);
                }
            }
        }

        public readonly Camera Camera
        {
            get
            {
                IsRenderer component = entity.GetComponent<IsRenderer>();
                eint cameraEntity = entity.GetReference(component.camera);
                return new(entity, cameraEntity);
            }
            set
            {
                ref IsRenderer component = ref entity.GetComponent<IsRenderer>();
                if (entity.ContainsReference(component.camera))
                {
                    entity.SetReference(component.camera, value);
                }
                else
                {
                    component.camera = entity.AddReference(value);
                }
            }
        }

        eint IEntity.Value => entity;
        World IEntity.World => entity;

        public Renderer(World world, eint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Renderer(World world, Mesh mesh, Material material, Camera camera)
        {
            entity = new(world);
            rint meshReference = entity.AddReference(mesh);
            rint materialReference = entity.AddReference(material);
            rint cameraReference = entity.AddReference(camera);
            entity.AddComponent(new IsRenderer(meshReference, materialReference, cameraReference));
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
            return new(world, RuntimeType.Get<IsRenderer>());
        }

        public static implicit operator Entity(Renderer renderer)
        {
            return renderer.entity;
        }
    }
}
