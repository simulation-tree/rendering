using Rendering.Components;
using Simulation;
using System;
using System.Numerics;

namespace Rendering
{
    public readonly struct Viewport : IEntity, IEquatable<Viewport>
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
                ref IsViewport component = ref entity.GetComponentRef<IsViewport>();
                ref rint destinationReference = ref component.destinationReference;
                if (destinationReference == default)
                {
                    destinationReference = entity.AddReference(value);
                }
                else
                {
                    uint destinationEntity = entity.GetReference(destinationReference);
                    if (destinationEntity != value.entity.value)
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

        public readonly ref uint Mask => ref entity.GetComponentRef<IsViewport>().mask;
        public readonly ref Vector4 Region => ref entity.GetComponentRef<IsViewport>().region;
        public readonly ref sbyte Order => ref entity.GetComponentRef<IsViewport>().order;

        readonly uint IEntity.Value => entity.GetEntityValue();
        readonly World IEntity.World => entity.GetWorld();
        readonly Definition IEntity.Definition => new Definition().AddComponentType<IsViewport>();

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

        public Viewport(World world, Destination destination, uint mask)
        {
            uint cameraCount = world.CountEntitiesWithComponent<IsViewport>();
            sbyte order = (sbyte)cameraCount;

            entity = new(world);

            rint destinationReference = entity.AddReference(destination);
            entity.AddComponent(new IsViewport(destinationReference, new(0, 0, 1, 1), order, mask));
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
            return HashCode.Combine(entity);
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