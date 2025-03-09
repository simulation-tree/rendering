using Rendering.Components;
using System.Numerics;
using Worlds;

namespace Rendering
{
    public readonly partial struct Viewport : IEntity
    {
        public readonly Destination Destination
        {
            get
            {
                IsViewport component = GetComponent<IsViewport>();
                uint destinationEntity = GetReference(component.destinationReference);
                if (world.ContainsEntity(destinationEntity))
                {
                    //todo: this branch means the destination reference should be 0 instead
                    return new Entity(world, destinationEntity).As<Destination>();
                }
                else
                {
                    return default;
                }
            }
            set
            {
                ref IsViewport component = ref GetComponent<IsViewport>();
                ref rint destinationReference = ref component.destinationReference;
                if (destinationReference == default)
                {
                    destinationReference = AddReference(value);
                }
                else
                {
                    uint destinationEntity = GetReference(destinationReference);
                    if (destinationEntity != value.value)
                    {
                        SetReference(destinationReference, value);
                    }
                    else
                    {
                        //same destination
                    }
                }
            }
        }

        public readonly ref Vector4 Region => ref GetComponent<IsViewport>().region;
        public readonly ref sbyte Order => ref GetComponent<IsViewport>().order;
        public readonly ref LayerMask RenderMask => ref GetComponent<IsViewport>().renderMask;

        public Viewport(World world, Destination destination, LayerMask renderMask)
        {
            int cameraCount = world.CountEntitiesWith<IsViewport>();
            sbyte order = (sbyte)cameraCount;

            this.world = world;
            value = world.CreateEntity(new IsViewport((rint)1, new(0, 0, 1, 1), order, renderMask));
            AddReference(destination);
        }

        public Viewport(World world, Destination destination)
        {
            int cameraCount = world.CountEntitiesWith<IsViewport>();
            sbyte order = (sbyte)cameraCount;

            this.world = world;
            value = world.CreateEntity(new IsViewport((rint)1, new(0, 0, 1, 1), order, LayerMask.All));
            AddReference(destination);
        }

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsViewport>();
        }

        public readonly override string ToString()
        {
            return value.ToString();
        }
    }
}