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

        public readonly ref float FieldOfView
        {
            get
            {
                ThrowIfOrthographic();
                return ref entity.GetComponent<CameraFieldOfView>().value;
            }
        }

        public readonly ref float OrthographicSize
        {
            get
            {
                ThrowIfPerspective();
                return ref entity.GetComponent<CameraOrthographicSize>().value;
            }
        }

        public readonly bool IsOrthographic => entity.ContainsComponent<CameraOrthographicSize>();
        public readonly bool IsPerspective => entity.ContainsComponent<CameraFieldOfView>();
        public readonly ref sbyte Order => ref entity.GetComponent<CameraOutput>().order;
        public readonly ref Vector4 OutputRegion => ref entity.GetComponent<CameraOutput>().region;

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
        uint IEntity.Value => entity;

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

        readonly Query IEntity.GetQuery(World world)
        {
            return Query.Create<IsCamera, CameraOutput>(world);
        }

        /// <summary>
        /// Calculates raycast inputs from the given mouse positon.
        /// </summary>
        public readonly (Vector3 origin, Vector3 direction) GetRayFromMousePosition(Vector2 mousePosition)
        {
            (uint width, uint height) = Destination.DestinationSize;
            Vector2 normalizedMousePosition = mousePosition / new Vector2(width, height);
            normalizedMousePosition.X = normalizedMousePosition.X * 2 - 1;
            normalizedMousePosition.Y = 1 - normalizedMousePosition.Y * 2;
            CameraProjection cameraMatrices = entity.GetComponent<CameraProjection>();
            Matrix4x4 projection = cameraMatrices.projection;
            Matrix4x4 view = cameraMatrices.view;
            Matrix4x4.Invert(projection, out Matrix4x4 invProjection);
            Matrix4x4.Invert(view, out Matrix4x4 invView);
            Vector4 nearPointNDC = new(normalizedMousePosition, 0f, 1f);
            Vector4 farPointNDC = new(normalizedMousePosition, 1f, 1f);
            Vector4 nearPointView = Vector4.Transform(nearPointNDC, invProjection);
            Vector4 farPointView = Vector4.Transform(farPointNDC, invProjection);
            nearPointView /= nearPointView.W;
            farPointView /= farPointView.W;
            Vector4 nearPointWorld = Vector4.Transform(nearPointView, invView);
            Vector4 farPointWorld = Vector4.Transform(farPointView, invView);
            Vector3 origin = new(nearPointWorld.X, nearPointWorld.Y, nearPointWorld.Z);
            Vector3 direction = Vector3.Normalize(new Vector3(farPointWorld.X, farPointWorld.Y, farPointWorld.Z) - origin);
            return (origin, direction);
        }

        /// <summary>
        /// Calculates position in world space from the given screen point,
        /// with an optional distance parameter.
        /// </summary>
        public readonly Vector3 GetWorldPositionFromScreenPoint(Vector2 screenPoint, float distance = 0f)
        {
            CameraProjection cameraMatrices = entity.GetComponent<CameraProjection>();
            Matrix4x4 projection = cameraMatrices.projection;
            Matrix4x4 view = cameraMatrices.view;
            Matrix4x4.Invert(projection, out Matrix4x4 invProjection);
            Matrix4x4.Invert(view, out Matrix4x4 invView);
            Vector4 nearPointNDC = new(screenPoint, 0f, 1f);
            Vector4 farPointNDC = new(screenPoint, 1f, 1f);
            Vector4 nearPointView = Vector4.Transform(nearPointNDC, invProjection);
            Vector4 farPointView = Vector4.Transform(farPointNDC, invProjection);
            nearPointView /= nearPointView.W;
            farPointView /= farPointView.W;
            Vector4 nearPointWorld = Vector4.Transform(nearPointView, invView);
            Vector4 farPointWorld = Vector4.Transform(farPointView, invView);
            Vector3 origin = new(nearPointWorld.X, nearPointWorld.Y, nearPointWorld.Z);
            Vector3 direction = Vector3.Normalize(new Vector3(farPointWorld.X, farPointWorld.Y, farPointWorld.Z) - origin);
            return origin + direction * distance;
        }

        /// <summary>
        /// Retrieves the given mouse position as 0-1 coordinates on the screen.
        /// </summary>
        public readonly Vector2 GetScreenPointFromMousePosition(Vector2 mousePosition)
        {
            (uint width, uint height) = Destination.DestinationSize;
            Vector2 screenPoint = mousePosition / new Vector2(width, height);
            screenPoint.Y = 1 - screenPoint.Y;
            return screenPoint;
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