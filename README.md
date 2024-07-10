# Rendering

### Behaviours
1. Camera entities will have a `CameraProjection` component with projection and view matrices for shaders
3. Material entities pass component data to shaders

### Cameras
Camera entities are entities with an `IsCamera` component, and either a `CameraOrthographicSize` or
`CameraFieldOfView` component (cannot have both).

### Materials
```cs
EntityID material = world.CreateEntity();
world.AddComponent(material, new IsMaterial());
world.AddComponent(material, new MaterialComponentBinding());
```
