using Meshes;
using Rendering.Components;
using Simulation;
using Unmanaged;

namespace Rendering
{
    public readonly struct Renderer : IEntity
    {
        public readonly Entity entity;

        public readonly bool IsEnabled
        {
            get => entity.IsEnabled;
            set => entity.IsEnabled = value;
        }

        public readonly Entity Parent
        {
            get => entity.Parent;
            set => entity.Parent = value;
        }

        public readonly Material Material
        {
            get
            {
                IsRenderer component = entity.GetComponentRef<IsRenderer>();
                uint materialEntity = entity.GetReference(component.material);
                return new(entity.world, materialEntity);
            }
            set
            {
                ref IsRenderer component = ref entity.GetComponentRef<IsRenderer>();
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
                IsRenderer component = entity.GetComponentRef<IsRenderer>();
                uint meshEntity = entity.GetReference(component.mesh);
                return new Entity(entity.world, meshEntity).As<Mesh>();
            }
            set
            {
                ref IsRenderer component = ref entity.GetComponentRef<IsRenderer>();
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
                IsRenderer component = entity.GetComponentRef<IsRenderer>();
                uint cameraEntity = entity.GetReference(component.camera);
                return new(entity.world, cameraEntity);
            }
            set
            {
                ref IsRenderer component = ref entity.GetComponentRef<IsRenderer>();
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

        readonly uint IEntity.Value => entity.value;
        readonly World IEntity.World => entity.world;
        readonly Definition IEntity.Definition => new([RuntimeType.Get<IsRenderer>()], []);

        public Renderer(World world, uint existingEntity)
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

        public readonly override string ToString()
        {
            return entity.ToString();
        }
    }
}
