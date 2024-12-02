# Rendering
Defines the types needed for 3D rendering implementations.

### Destinations
These are entities that are known to be rendered to, and are referenced by cameras. They're expected
to be implemented with individual window libraries, like in [`windows`](https://github.com/game-simulations/windows).

Destinations are then presented separately from appearing. They also require a label on them, with that label's handler registered with the `RenderEngineSystem`. The render engine system then updates upon the `RenderUpdate` event (which should be submitted last) and draws all enabled destinations:
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

### Binding data to shaders
Its the materials entities that contain information about how to bind entity or texture data to [`shaders`](https://github.com/game-simulations/shaders):
```cs
Shader shader = new(world, "Assets/Shaders/shader.vert.glsl", "Assets/Shaders/shader.frag.glsl");
Texture texture = new(world, "Assets/Textures/texture.png");
Material material = new(world, shader);
material.AddPushBinding<Color>(); //component expected on the renderer entity
material.AddComponentBinding<CameraProjection>(0, 0, camera);
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

### Loading materials from files
The included method expects material files to be json, with a "vertex" and "fragment" key pointing
to shader addresses:
```cs
Material material = new(world, "*/Materials/Unlit.material.json");
```
```json
{
    "vertex": "Assets/Shaders/unlit.vert.glsl",
    "fragment": "Assets/Shaders/unlit.frag.glsl"
}
```

### Renderers
Finally the entities that cause rendering. They reference the previously mentioned materials
and cameras, as well as [`meshes`](https://github.com/game-simulations/meshes).
```cs
Mesh mesh = ...
Renderer renderer = new(world, mesh, material, camera);
renderer.AddComponent(Color.Yellow); //the push binding requires a `Color`
```
