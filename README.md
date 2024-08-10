# Rendering
Defines the types needed for 3D rendering implementations.

### Destinations
These are entities that are known to be rendered to, and are referenced by cameras. They're expected
to be implemented by extending them into actual render destinations such as [`windows`](https://github.com/game-simulations/windows).

Destinations are first presented separately from rendering. They also require a label on them, with that label's handler registered with the `RenderEngineSystem`. This system updates upon the `RenderUpdate` event (which should be submitted last):
```cs
using RenderEngineSystem renderEngine = new(world);
renderEngine.RegisterSystem<CustomRenderer>();
```

### Cameras
Cameras are used by individual renderer entities, and they must always point towards a destination.
```cs
Destination destination = new("customRenderer", ...).AsDestination();
Camera camera = new(world, destination, CameraFieldOfView.FromDegrees(90f));
camera.SetPosition(0, 0, -10);
```
Whenever they get updated, they will have a `CameraProjection` component added to them to reflect
the view and projection matrices.

### Materials and shader binding
Materials contain information about how to bind entity or texture data to [`shaders`](https://github.com/game-simulations/shaders).
```cs
Shader shader = new(world, "Program/shader.vert", "Program/shader.frag");
Texture texture = new(world, "Program/texture.png");
Material material = new(world, shader);
material.AddPushBinding(RuntimeType.Get<Color>());
material.AddComponentBinding(0, 0, camera, RuntimeType.Get<CameraProjection>());
material.AddTextureBinding(1, 0, texture);
```
Where the shader has these uniform buffers and push constants defined:
```glsl
layout(push_constant) uniform EntityData {
    vec4 color; //bound to Color, from the renderer
} entity;

layout(binding = 0) uniform CameraInfo { //bound to CameraProjection, from the camera
	mat4 proj;
    mat4 view;
} cameraInfo;
```
```glsl
layout(binding = 1) uniform sampler2D textureSampler; //bound to the texture entity
```

### Renderers
Finally the entities that cause rendering. They reference the previously mentioned materials
and cameras, as well as [`meshes`](https://github.com/game-simulations/meshes).
```cs
Mesh mesh = ...
Renderer renderer = new(world, mesh, material, camera);
renderer.AddComponent(Color.Yellow); //the push binding requires a `Color`
```