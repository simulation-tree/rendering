using Meshes;
using Rendering.Components;
using Worlds;

namespace Rendering
{
    public readonly struct MeshRenderer : IRenderer
    {
        private readonly Entity entity;

        public readonly Material Material
        {
            get
            {
                ref IsRenderer component = ref entity.GetComponent<IsRenderer>();
                uint materialEntity = entity.GetReference(component.materialReference);
                return new(entity.world, materialEntity);
            }
            set
            {
                ref IsRenderer component = ref entity.GetComponent<IsRenderer>();
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
                ref IsRenderer component = ref entity.GetComponent<IsRenderer>();
                uint meshEntity = entity.GetReference(component.meshReference);
                return new Entity(entity.world, meshEntity).As<Mesh>();
            }
            set
            {
                ref IsRenderer component = ref entity.GetComponent<IsRenderer>();
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

        public readonly ref LayerMask RenderMask
        {
            get
            {
                ref IsRenderer component = ref entity.GetComponent<IsRenderer>();
                return ref component.renderMask;
            }
        }

        readonly uint IEntity.Value => entity.GetEntityValue();
        readonly World IEntity.World => entity.GetWorld();

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsRenderer>();
        }

        public MeshRenderer(World world, uint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public MeshRenderer(World world, Mesh mesh, Material material, LayerMask renderMask)
        {
            entity = new Entity<IsRenderer>(world, new IsRenderer((rint)1, (rint)2, renderMask));
            entity.AddReference(mesh);
            entity.AddReference(material);
        }

        public MeshRenderer(World world, Mesh mesh, Material material)
        {
            LayerMask firstLayerOnly = default;
            firstLayerOnly.Set(0);
            entity = new Entity<IsRenderer>(world, new IsRenderer((rint)1, (rint)2, firstLayerOnly));
            entity.AddReference(mesh);
            entity.AddReference(material);
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