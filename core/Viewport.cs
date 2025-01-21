using Rendering.Components;
using System;
using Worlds;

namespace Rendering
{
    public readonly struct Viewport : IViewport, IEquatable<Viewport>
    {
        private readonly Entity entity;

        public readonly Destination Destination
        {
            get
            {
                IsViewport component = entity.GetComponent<IsViewport>();
                uint destinationEntity = entity.GetReference(component.destinationReference);
                return new(entity.world, destinationEntity);
            }
            set
            {
                ref IsViewport component = ref entity.GetComponent<IsViewport>();
                ref rint destinationReference = ref component.destinationReference;
                if (destinationReference == default)
                {
                    destinationReference = entity.AddReference(value);
                }
                else
                {
                    uint destinationEntity = entity.GetReference(destinationReference);
                    if (destinationEntity != value.GetEntityValue())
                    {
                        entity.SetReference(destinationReference, value);
                    }
                    else
                    {
                        //same destination
                    }
                }
            }
        }

        public readonly ref LayerMask RenderMask => ref entity.GetComponent<IsViewport>().renderMask;

        readonly uint IEntity.Value => entity.GetEntityValue();
        readonly World IEntity.World => entity.GetWorld();

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsViewport>();
        }

#if NET
        [Obsolete("Default constructor not supported", true)]
        public Viewport()
        {
            throw new NotSupportedException();
        }
#endif

        public Viewport(World world, uint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Viewport(World world, Destination destination, LayerMask renderMask)
        {
            uint cameraCount = world.CountEntitiesWith<IsViewport>();
            sbyte order = (sbyte)cameraCount;

            entity = new Entity<IsViewport>(world, new IsViewport((rint)1, new(0, 0, 1, 1), order, renderMask));
            entity.AddReference(destination);
        }

        public Viewport(World world, Destination destination)
        {
            uint cameraCount = world.CountEntitiesWith<IsViewport>();
            sbyte order = (sbyte)cameraCount;

            entity = new Entity<IsViewport>(world, new IsViewport((rint)1, new(0, 0, 1, 1), order, LayerMask.All));
            entity.AddReference(destination);
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is Viewport viewport && Equals(viewport);
        }

        public readonly bool Equals(Viewport other)
        {
            return entity.Equals(other.entity);
        }

        public readonly override int GetHashCode()
        {
            return entity.GetHashCode();
        }

        public static implicit operator Entity(Viewport viewport)
        {
            return viewport.entity;
        }

        public static bool operator ==(Viewport left, Viewport right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Viewport left, Viewport right)
        {
            return !(left == right);
        }
    }
}