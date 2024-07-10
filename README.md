# Rendering

### Behaviours
1. Camera entities will have a `CameraProjection` component with projection and view matrices for shaders
3. Material entities pass component data to shaders

### Cameras and destinations
When entities need to be rendered, they reference a camera. Camera entities are ones with an `IsCamera`
component, and either a `CameraOrthographicSize` or `CameraFieldOfView` component (cannot have both).

Each camera can have a `CameraOutput` component that points towards a destination entity, with a
configurable order for sorting the cameras.

### Materials
```cs
EntityID material = world.CreateEntity();
world.AddComponent(material, new IsMaterial());
world.AddComponent(material, new MaterialComponentBinding());
```
