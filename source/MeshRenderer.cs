using Meshes;
using Rendering.Components;
using Simulation;

namespace Rendering
{
    public readonly struct MeshRenderer : IEntity
    {
        private readonly Entity entity;

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
                IsRenderer component = entity.GetComponent<IsRenderer>();
                uint materialEntity = entity.GetReference(component.materialReference);
                return new(entity.world, materialEntity);
            }
            set
            {
                ref IsRenderer component = ref entity.GetComponentRef<IsRenderer>();
                if (entity.ContainsReference(component.materialReference))
                {
                    entity.SetReference(component.materialReference, value);
                }
                else
                {
                    component.materialReference = entity.AddReference(value);
                }
            }
        }

        public readonly Mesh Mesh
        {
            get
            {
                IsRenderer component = entity.GetComponentRef<IsRenderer>();
                uint meshEntity = entity.GetReference(component.meshReference);
                return new Entity(entity.world, meshEntity).As<Mesh>();
            }
            set
            {
                ref IsRenderer component = ref entity.GetComponentRef<IsRenderer>();
                if (entity.ContainsReference(component.meshReference))
                {
                    entity.SetReference(component.meshReference, value);
                }
                else
                {
                    component.meshReference = entity.AddReference(value);
                }
            }
        }

        public readonly ref uint Mask
        {
            get
            {
                ref IsRenderer component = ref entity.GetComponentRef<IsRenderer>();
                return ref component.mask;
            }
        }

        readonly uint IEntity.Value => entity.GetEntityValue();
        readonly World IEntity.World => entity.GetWorld();
        readonly Definition IEntity.Definition => new Definition().AddComponentType<IsRenderer>();

        public MeshRenderer(World world, uint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public MeshRenderer(World world, Mesh mesh, Material material, uint mask = 1)
        {
            entity = new(world);
            rint meshReference = entity.AddReference(mesh);
            rint materialReference = entity.AddReference(material);
            entity.AddComponent(new IsRenderer(meshReference, materialReference, mask));
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        public static implicit operator Entity(MeshRenderer renderer)
        {
            return renderer.entity;
        }
    }
}
