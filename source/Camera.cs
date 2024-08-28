using Rendering.Components;
using Simulation;
using System;
using System.Diagnostics;
using System.Numerics;

namespace Rendering
{
    public readonly struct Camera : IEntity
    {
        private readonly Entity entity;

        public readonly (float min, float max) Depth
        {
            get
            {
                IsCamera component = entity.GetComponent<IsCamera>();
                return (component.minDepth, component.maxDepth);
            }
            set
            {
                ref IsCamera component = ref entity.GetComponent<IsCamera>();
                component = new(value.min, value.max);
            }
        }

        public readonly float FieldOfView
        {
            get
            {
                ThrowIfOrthographic();
                return entity.GetComponent<CameraFieldOfView>().value;
            }
            set
            {
                ThrowIfOrthographic();
                ref CameraFieldOfView fieldOfView = ref entity.GetComponent<CameraFieldOfView>();
                fieldOfView = new(value);
            }
        }

        public readonly float OrthographicSize
        {
            get
            {
                ThrowIfPerspective();
                return entity.GetComponent<CameraOrthographicSize>().value;
            }
            set
            {
                ThrowIfPerspective();
                ref CameraOrthographicSize orthographicSize = ref entity.GetComponent<CameraOrthographicSize>();
                orthographicSize = new(value);
            }
        }

        public readonly bool IsOrthographic
        {
            get
            {
                return entity.ContainsComponent<CameraOrthographicSize>();
            }
        }

        public readonly bool IsPerspective
        {
            get
            {
                return entity.ContainsComponent<CameraFieldOfView>();
            }
        }

        public readonly sbyte Order
        {
            get
            {
                return entity.GetComponent<CameraOutput>().order;
            }
            set
            {
                ref CameraOutput output = ref entity.GetComponent<CameraOutput>();
                output.order = value;
            }
        }

        public readonly Vector4 OutputRegion
        {
            get
            {
                return entity.GetComponent<CameraOutput>().region;
            }
            set
            {
                ref CameraOutput output = ref entity.GetComponent<CameraOutput>();
                output.region = value;
            }
        }

        public readonly Destination Destination
        {
            get
            {
                return new(entity, entity.GetComponent<CameraOutput>().destination);
            }
            set
            {
                ref CameraOutput output = ref entity.GetComponent<CameraOutput>();
                output.destination = (Entity)value;
            }
        }

        World IEntity.World => entity;
        eint IEntity.Value => entity;

#if NET
        [Obsolete("Default constructor not available", true)]
        public Camera()
        {
            throw new InvalidOperationException("Cannot create a camera without a world.");
        }
#endif

        public Camera(World world, eint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Camera(World world, Destination destination, bool isOrthographic, float size, float minDepth = 0.1f, float maxDepth = 1000f)
        {
            uint cameraCount = world.CountEntities<IsCamera>();
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

            entity.AddComponent(new CameraOutput(destination, new(0, 0, 1, 1), new(0, 0, 0, 1), order));
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

        Query IEntity.GetQuery(World world)
        {
            return Query.Create<IsCamera, CameraOutput>(world);
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

        public static implicit operator Entity(Camera camera)
        {
            return camera.entity;
        }
    }
}