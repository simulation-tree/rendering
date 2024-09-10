using Rendering.Components;
using Simulation;
using System;
using System.Diagnostics;
using System.Numerics;
using Unmanaged;

namespace Rendering
{
    public readonly struct Camera : IEntity
    {
        public readonly Entity entity;

        public readonly (float min, float max) Depth
        {
            get
            {
                IsCamera component = entity.GetComponentRef<IsCamera>();
                return (component.minDepth, component.maxDepth);
            }
            set
            {
                ref IsCamera component = ref entity.GetComponentRef<IsCamera>();
                component = new(value.min, value.max);
            }
        }

        public readonly ref float FieldOfView
        {
            get
            {
                ThrowIfOrthographic();
                return ref entity.GetComponentRef<CameraFieldOfView>().value;
            }
        }

        public readonly ref float OrthographicSize
        {
            get
            {
                ThrowIfPerspective();
                return ref entity.GetComponentRef<CameraOrthographicSize>().value;
            }
        }

        public readonly bool IsOrthographic => entity.ContainsComponent<CameraOrthographicSize>();
        public readonly bool IsPerspective => entity.ContainsComponent<CameraFieldOfView>();
        public readonly ref sbyte Order => ref entity.GetComponentRef<CameraOutput>().order;
        public readonly ref Vector4 OutputRegion => ref entity.GetComponentRef<CameraOutput>().region;

        public readonly Destination Destination
        {
            get
            {
                rint destinationReference = entity.GetComponent<CameraOutput>().destinationReference;
                uint destinationEntity = entity.GetReference(destinationReference);
                return new(entity.world, destinationEntity);
            }
            set
            {
                ref CameraOutput output = ref entity.GetComponentRef<CameraOutput>();
                ref rint destinationReference = ref output.destinationReference;
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

        readonly uint IEntity.Value => entity.value;
        readonly World IEntity.World => entity.world;
        readonly Definition IEntity.Definition => new([RuntimeType.Get<IsCamera>(), RuntimeType.Get<CameraOutput>()], []);

#if NET
        [Obsolete("Default constructor not available", true)]
        public Camera()
        {
            throw new InvalidOperationException("Cannot create a camera without a world.");
        }
#endif

        public Camera(World world, uint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Camera(World world, Destination destination, bool isOrthographic, float size, float minDepth = 0.1f, float maxDepth = 1000f)
        {
            uint cameraCount = world.CountEntitiesWithComponent<IsCamera>();
            sbyte order = (sbyte)cameraCount;

            entity = new(world);
            if (isOrthographic)
            {
                entity.AddComponent(new CameraOrthographicSize(size));
            }
            else
            {
                entity.AddComponent(new CameraFieldOfView(size));
            }

            rint destinationReference = entity.AddReference(destination);
            entity.AddComponent(new CameraOutput(destinationReference, new(0, 0, 1, 1), new(0, 0, 0, 1), order));
            entity.AddComponent(new IsCamera(minDepth, maxDepth));
        }

        public Camera(World world, Destination destination, CameraFieldOfView fieldOfView, float minDepth = 0.1f, float maxDepth = 1000f) :
            this(world, destination, false, fieldOfView.value, minDepth, maxDepth)
        {
        }

        public Camera(World world, Destination destination, CameraOrthographicSize orthographicSize, float minDepth = 0.1f, float maxDepth = 1000f) :
            this(world, destination, true, orthographicSize.value, minDepth, maxDepth)
        {

        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfOrthographic()
        {
            if (IsOrthographic)
            {
                throw new InvalidOperationException("Cannot get field of view for an orthographic camera.");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfPerspective()
        {
            if (IsPerspective)
            {
                throw new InvalidOperationException("Cannot get orthographic size for a perspective camera.");
            }
        }
    }
}