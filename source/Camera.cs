using Rendering.Components;
using Simulation;
using System;
using System.Numerics;
using Transforms.Components;
using Unmanaged;

namespace Rendering
{
    public readonly struct Camera : ICamera, IDisposable
    {
        private readonly Entity entity;

        World IEntity.World => entity.world;
        eint IEntity.Value => entity.value;

        public Camera()
        {
            throw new InvalidOperationException("Cannot create a camera without a world.");
        }

        public Camera(World world, eint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Camera(World world, Destination destination, bool isOrthographic, float size, float minDepth = 0.1f, float maxDepth = 1000f) :
            this(world, default, Quaternion.Identity, destination, isOrthographic, size, minDepth, maxDepth)
        {
        }

        public Camera(World world, Destination destination, CameraFieldOfView fieldOfView, float minDepth = 0.1f, float maxDepth = 1000f) : 
            this(world, destination, false, fieldOfView.value, minDepth, maxDepth)
        {
        }

        public Camera(World world, Destination destination, CameraOrthographicSize orthographicSize, float minDepth = 0.1f, float maxDepth = 1000f) :
            this(world, destination, true, orthographicSize.value, minDepth, maxDepth)
        {
        }

        public Camera(World world, Vector3 position, Quaternion rotation, Destination destination, bool isOrthographic,
            float size, float minDepth = 0.1f, float maxDepth = 1000f)
        {
            uint cameraCount = world.CountEntities<IsCamera>();
            sbyte order = (sbyte)cameraCount;

            entity = new(world);
            entity.AddComponent(new IsCamera(minDepth, maxDepth));
            entity.AddComponent(new IsTransform());
            entity.AddComponent(new CameraOutput(destination, new(0, 0, 1, 1), new(0, 0, 0, 1), order));
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

        public static Query GetQuery(World world)
        {
            return new(world, RuntimeType.Get<IsCamera>());
        }
    }
}