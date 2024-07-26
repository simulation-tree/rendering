using Rendering;
using Rendering.Components;
using System;
using System.Diagnostics;
using System.Numerics;

public static class CameraFunctions
{
    [Conditional("DEBUG")]
    public static void ThrowIfOrthographic<T>(this T camera) where T : unmanaged, ICamera
    {
        if (!camera.IsOrthographic())
        {
            throw new InvalidOperationException("The camera is not orthographic.");
        }
    }

    [Conditional("DEBUG")]
    public static void ThrowIfPerspective<T>(this T camera) where T : unmanaged, ICamera
    {
        if (camera.IsOrthographic())
        {
            throw new InvalidOperationException("The camera is not perspective.");
        }
    }

    public static bool IsOrthographic<T>(this T camera) where T : unmanaged, ICamera
    {
        return camera.ContainsComponent<T, CameraOrthographicSize>();
    }

    /// <summary>
    /// The destination entity that the camera outputs to.
    /// </summary>
    public static Destination GetDestination<T>(this T camera) where T : unmanaged, ICamera
    {
        return new(camera.World, camera.Value);
    }

    public static Vector4 GetOutputRegion<T>(this T camera) where T : unmanaged, ICamera
    {
        return camera.GetComponent<T, CameraOutput>().region;
    }

    public static sbyte GetOutputOrder<T>(this T camera) where T : unmanaged, ICamera
    {
        return camera.GetComponent<T, CameraOutput>().order;
    }

    public static float GetOrthographicSize<T>(this T camera) where T : unmanaged, ICamera
    {
        ThrowIfPerspective(camera);
        return camera.GetComponent<T, CameraOrthographicSize>().value;
    }

    public static float GetFieldOfView<T>(this T camera) where T : unmanaged, ICamera
    {
        ThrowIfOrthographic(camera);
        return camera.GetComponent<T, CameraFieldOfView>().value;
    }

    public static (float min, float max) GetDepth<T>(this T camera) where T : unmanaged, ICamera
    {
        IsCamera proof = camera.GetComponent<T, IsCamera>();
        return (proof.minDepth, proof.maxDepth);
    }
}