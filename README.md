# Rendering

### Behaviours
1. Camera entities will have a `CameraProjection` component with projection and view matrices
for shaders, using only `Position` and `Rotation` component values.
2. Material entities pass component data to shaders

### Cameras and destinations
When entities need to be rendered, they reference a camera. Camera entities are ones with an `IsCamera`
component, and either a `CameraOrthographicSize` or `CameraFieldOfView` component (cannot have both).

Each camera can have a `CameraOutput` component that points to a destination entity, with a
manual sorting order.

### Materials
```cs
EntityID material = world.CreateEntity();
world.AddComponent(material, new IsMaterial());
world.AddComponent(material, new MaterialComponentBinding());
```
