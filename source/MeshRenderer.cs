using Materials;
using Meshes;
using Rendering.Components;
using System.Numerics;
using Worlds;

namespace Rendering
{
    public readonly partial struct MeshRenderer : IEntity
    {
        public readonly Material Material
        {
            get
            {
                ref IsRenderer component = ref GetComponent<IsRenderer>();
                uint materialEntity = GetReference(component.materialReference);
                return new Entity(world, materialEntity).As<Material>();
            }
            set
            {
                ref IsRenderer component = ref GetComponent<IsRenderer>();
                if (ContainsReference(component.materialReference))
                {
                    SetReference(component.materialReference, value);
                }
                else
                {
                    component.materialReference = AddReference(value);
                }
            }
        }

        public readonly ref Vector4 LocalScissor
        {
            get
            {
                ref RendererScissor component = ref TryGetComponent<RendererScissor>(out bool contains);
                if (contains)
                {
                    return ref component.value;
                }
                else
                {
                    return ref AddComponent(new RendererScissor()).value;
                }
            }
        }

        public readonly Vector4 WorldScissor
        {
            get
            {
                ref WorldRendererScissor component = ref TryGetComponent<WorldRendererScissor>(out bool contains);
                if (contains)
                {
                    return component.value;
                }
                else
                {
                    return AddComponent(new WorldRendererScissor()).value;
                }
            }
        }

        public readonly Mesh Mesh
        {
            get
            {
                ref IsRenderer component = ref GetComponent<IsRenderer>();
                uint meshEntity = GetReference(component.meshReference);
                return new Entity(world, meshEntity).As<Mesh>();
            }
            set
            {
                ref IsRenderer component = ref GetComponent<IsRenderer>();
                if (ContainsReference(component.meshReference))
                {
                    SetReference(component.meshReference, value);
                }
                else
                {
                    component.meshReference = AddReference(value);
                }
            }
        }

        public readonly ref LayerMask RenderMask
        {
            get
            {
                ref IsRenderer component = ref GetComponent<IsRenderer>();
                return ref component.renderMask;
            }
        }

        public MeshRenderer(World world, Mesh mesh, Material material, LayerMask renderMask)
        {
            this.world = world;
            value = world.CreateEntity(new IsRenderer((rint)1, (rint)2, renderMask));
            AddReference(mesh);
            AddReference(material);
        }

        public MeshRenderer(World world, Mesh mesh, Material material)
        {
            LayerMask renderMask = default;
            renderMask.Set(0);

            this.world = world;
            value = world.CreateEntity(new IsRenderer((rint)1, (rint)2, renderMask));
            AddReference(mesh);
            AddReference(material);
        }

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsRenderer>();
        }

        public readonly override string ToString()
        {
            return value.ToString();
        }
    }
}