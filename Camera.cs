using Rendering.Components;
using Simulation;
using System;
using System.Diagnostics;
using System.Numerics;
using Transforms.Components;

namespace Rendering
{
    public readonly struct Camera : IDisposable
    {
        public readonly Entity entity;

        public readonly bool IsDestroyed => entity.IsDestroyed;

        public readonly bool IsOrthographic => entity.ContainsComponent<CameraOrthographicSize>();

        /// <summary>
        /// The destination surface that the camera outputs to.
        /// </summary>
        public readonly Destination Destination
        {
            get => new(entity.world, entity.GetComponent<CameraOutput>().destination);
            set
            {
                ref CameraOutput output = ref entity.GetComponentRef<CameraOutput>();
                output.destination = value.entity;
            }
        }

        public readonly Vector4 OutputRegion
        {
            get => entity.GetComponent<CameraOutput>().region;
            set
            {
                ref CameraOutput output = ref entity.GetComponentRef<CameraOutput>();
                output.region = value;
            }
        }

        public readonly sbyte OutputOrder
        {
            get => entity.GetComponent<CameraOutput>().order;
            set
            {
                ref CameraOutput output = ref entity.GetComponentRef<CameraOutput>();
                output.order = value;
            }
        }

        public readonly Vector3 Position
        {
            get => entity.GetComponent<Position>().value;
            set
            {
                ref Position position = ref entity.GetComponentRef<Position>();
                position.value = value;
            }
        }

        public readonly Quaternion Rotation
        {
            get => entity.GetComponent<Rotation>().value;
            set
            {
                ref Rotation rotation = ref entity.GetComponentRef<Rotation>();
                rotation.value = value;
            }
        }

        /// <summary>
        /// Field of view in radians.
        /// </summary>
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
                ref CameraFieldOfView fieldOfView = ref entity.GetComponentRef<CameraFieldOfView>();
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
                ref CameraOrthographicSize orthographicSize = ref entity.GetComponentRef<CameraOrthographicSize>();
                orthographicSize = new(value);
            }
        }

        public readonly float MinDepth
        {
            get => entity.GetComponent<IsCamera>().minDepth;
            set
            {
                ref IsCamera camera = ref entity.GetComponentRef<IsCamera>();
                camera.minDepth = value;
            }
        }

        public readonly float MaxDepth
        {
            get => entity.GetComponent<IsCamera>().maxDepth;
            set
            {
                ref IsCamera camera = ref entity.GetComponentRef<IsCamera>();
                camera.maxDepth = value;
            }
        }

        public Camera()
        {
            throw new InvalidOperationException("Cannot create a camera without a world.");
        }

        public Camera(World world, EntityID existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Camera(World world, Vector3 position, Quaternion rotation, Destination destination, bool isOrthographic, float size, float minDepth = 0.1f, float maxDepth = 1000f)
        {
            uint cameraCount = world.CountEntities<IsCamera>();
            sbyte order = (sbyte)cameraCount;

            entity = new(world);
            entity.AddComponent(new IsCamera(minDepth, maxDepth));
            entity.AddComponent(new IsTransform());
            entity.AddComponent(new CameraOutput(destination.entity, new(0, 0, 1, 1), new(0, 0, 0, 1), order));
            if (isOrthographic)
            {
                entity.AddComponent(new CameraOrthographicSize(size));
            }
            else
            {
                entity.AddComponent(new CameraFieldOfView(size));
            }

            entity.AddComponent(new Position(position));
            entity.AddComponent(new Rotation(rotation));
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfOrthographic()
        {
            if (IsOrthographic)
            {
                throw new InvalidOperationException("Cannot interact with the camera when it's expected to be orthographic.");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfPerspective()
        {
            if (!IsOrthographic)
            {
                throw new InvalidOperationException("Cannot interact with the camera when it's expected to be perspective.");
            }
        }

        public readonly void MakeOrthographic(float orthographicSize)
        {
            ThrowIfOrthographic();
            entity.AddComponent(new CameraOrthographicSize(orthographicSize));
            entity.RemoveComponent<CameraFieldOfView>();
        }

        public readonly void MakePerspective(float fieldOfView)
        {
            ThrowIfPerspective();
            entity.AddComponent(new CameraFieldOfView(fieldOfView));
            entity.RemoveComponent<CameraOrthographicSize>();
        }
    }
}
