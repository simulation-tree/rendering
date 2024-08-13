using Rendering.Components;
using Simulation;
using System;
using System.Numerics;
using Unmanaged;

namespace Rendering
{
    public readonly struct Camera : ICamera, IDisposable
    {
        private readonly Entity entity;

        World IEntity.World => entity.world;
        eint IEntity.Value => entity.value;

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

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        Query IEntity.GetQuery(World world)
        {
            return Query.Create<IsCamera, CameraOutput>(world);
        }
    }
}