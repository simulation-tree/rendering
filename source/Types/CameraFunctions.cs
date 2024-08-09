using Rendering;
using Rendering.Components;
using System;
using System.Diagnostics;
using System.Numerics;

public static class CameraFunctions
{
    [Conditional("DEBUG")]
    public static void ThrowIfOrthographic<T>(this T camera) where T : ICamera
    {
        if (!camera.IsOrthographic())
        {
            throw new InvalidOperationException("The camera is not orthographic.");
        }
    }

    [Conditional("DEBUG")]
    public static void ThrowIfPerspective<T>(this T camera) where T : ICamera
    {
        if (camera.IsOrthographic())
        {
            throw new InvalidOperationException("The camera is not perspective.");
        }
    }

    public static bool IsOrthographic<T>(this T camera) where T : ICamera
    {
        return camera.ContainsComponent<T, CameraOrthographicSize>();
    }

    /// <summary>
    /// The destination entity that the camera outputs to.
    /// </summary>
    public static Destination GetDestination<T>(this T camera) where T : ICamera
    {
        CameraOutput output = camera.GetComponent<T, CameraOutput>();
        return new(camera.World, output.destination);
    }

    public static void SetDestination<T>(this T camera, Destination destination) where T : ICamera
    {
        ref CameraOutput output = ref camera.GetComponentRef<T, CameraOutput>();
        output.destination = destination.GetEntityValue();
    }

    public static Vector4 GetOutputRegion<T>(this T camera) where T : ICamera
    {
        return camera.GetComponent<T, CameraOutput>().region;
    }

    public static void SetOutputRegion<T>(this T camera, Vector4 region) where T : ICamera
    {
        ref CameraOutput output = ref camera.GetComponentRef<T, CameraOutput>();
        output.region = region;
    }

    public static sbyte GetOutputOrder<T>(this T camera) where T : ICamera
    {
        return camera.GetComponent<T, CameraOutput>().order;
    }

    public static void SetOutputOrder<T>(this T camera, sbyte order) where T : ICamera
    {
        ref CameraOutput output = ref camera.GetComponentRef<T, CameraOutput>();
        output.order = order;
    }

    public static float GetOrthographicSize<T>(this T camera) where T : ICamera
    {
        ThrowIfPerspective(camera);
        return camera.GetComponent<T, CameraOrthographicSize>().value;
    }

    public static void SetOrthographicSize<T>(this T camera, float size) where T : ICamera
    {
        ThrowIfPerspective(camera);
        ref CameraOrthographicSize orthographicSize = ref camera.GetComponentRef<T, CameraOrthographicSize>();
        orthographicSize = new(size);
    }

    public static float GetFieldOfView<T>(this T camera) where T : ICamera
    {
        ThrowIfOrthographic(camera);
        return camera.GetComponent<T, CameraFieldOfView>().value;
    }

    public static void SetFieldOfView<T>(this T camera, float fieldOfView) where T : ICamera
    {
        ThrowIfOrthographic(camera);
        ref CameraFieldOfView fieldOfViewComponent = ref camera.GetComponentRef<T, CameraFieldOfView>();
        fieldOfViewComponent = new(fieldOfView);
    }

    public static (float min, float max) GetDepth<T>(this T camera) where T : ICamera
    {
        IsCamera component = camera.GetComponent<T, IsCamera>();
        return (component.minDepth, component.maxDepth);
    }

    public static void SetDepthMin<T>(this T camera, float minDepth) where T : ICamera
    {
        ref IsCamera component = ref camera.GetComponentRef<T, IsCamera>();
        component.minDepth = minDepth;
    }

    public static void SetDepthMax<T>(this T camera, float maxDepth) where T : ICamera
    {
        ref IsCamera component = ref camera.GetComponentRef<T, IsCamera>();
        component.maxDepth = maxDepth;
    }

    public static void SetDepth<T>(this T camera, float minDepth, float maxDepth) where T : ICamera
    {
        ref IsCamera component = ref camera.GetComponentRef<T, IsCamera>();
        component = new(minDepth, maxDepth);
    }
}