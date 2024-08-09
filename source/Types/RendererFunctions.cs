using Meshes;
using Rendering;
using Rendering.Components;

public static class RendererFunctions
{
    public static Material GetMaterial<T>(this T entity) where T : IRenderer
    {
        IsRenderer component = entity.GetComponent<T, IsRenderer>();
        return new(entity.World, component.material);
    }

    public static void SetMaterial<T>(this T entity, Material material) where T : IRenderer
    {
        ref IsRenderer component = ref entity.GetComponentRef<T, IsRenderer>();
        Mesh existingMesh = new(entity.World, component.mesh);
        Camera existingCamera = new(entity.World, component.camera);
        component = new(existingMesh, material, existingCamera);
    }

    public static Mesh GetMesh<T>(this T entity) where T : IRenderer
    {
        IsRenderer component = entity.GetComponent<T, IsRenderer>();
        return new(entity.World, component.mesh);
    }

    public static void SetMesh<T>(this T entity, Mesh mesh) where T : IRenderer
    {
        ref IsRenderer component = ref entity.GetComponentRef<T, IsRenderer>();
        Material existingMaterial = new(entity.World, component.material);
        Camera existingCamera = new(entity.World, component.camera);
        component = new(mesh, existingMaterial, existingCamera);
    }

    /// <summary>
    /// The camera entity that the renderer targets.
    /// </summary>
    public static Camera GetCamera<T>(this T entity) where T : IRenderer
    {
        IsRenderer component = entity.GetComponent<T, IsRenderer>();
        return new(entity.World, component.camera);
    }

    public static void SetCamera<T>(this T entity, Camera camera) where T : IRenderer
    {
        ref IsRenderer component = ref entity.GetComponentRef<T, IsRenderer>();
        Mesh existingMesh = new(entity.World, component.mesh);
        Material existingMaterial = new(entity.World, component.material);
        component = new(existingMesh, existingMaterial, camera);
    }
}