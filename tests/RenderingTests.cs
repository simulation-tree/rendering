using Data.Components;
using Meshes;
using Meshes.Components;
using Rendering.Components;
using Shaders.Components;
using Simulation.Tests;
using Types;

namespace Rendering.Tests
{
    public abstract class RenderingTests : SimulationTests
    {
        static RenderingTests()
        {
            TypeLayout.Register<IsDestination>();
            TypeLayout.Register<IsMaterial>();
            TypeLayout.Register<IsDataRequest>();
            TypeLayout.Register<RendererScissor>();
            TypeLayout.Register<WorldRendererScissor>();
            TypeLayout.Register<RendererInstanceInUse>();
            TypeLayout.Register<SurfaceInUse>();
            TypeLayout.Register<IsViewport>();
            TypeLayout.Register<IsRenderer>();
            TypeLayout.Register<IsMesh>();
            TypeLayout.Register<IsShader>();
            TypeLayout.Register<IsShaderRequest>();
            TypeLayout.Register<MeshVertexIndex>();
            TypeLayout.Register<MaterialPushBinding>();
            TypeLayout.Register<MaterialComponentBinding>();
            TypeLayout.Register<MaterialTextureBinding>();
            TypeLayout.Register<DestinationExtension>();
        }

        protected override void SetUp()
        {
            base.SetUp();
            world.Schema.RegisterComponent<IsDestination>();
            world.Schema.RegisterComponent<IsMaterial>();
            world.Schema.RegisterComponent<IsDataRequest>();
            world.Schema.RegisterComponent<RendererScissor>();
            world.Schema.RegisterComponent<WorldRendererScissor>();
            world.Schema.RegisterComponent<RendererInstanceInUse>();
            world.Schema.RegisterComponent<SurfaceInUse>();
            world.Schema.RegisterComponent<IsViewport>();
            world.Schema.RegisterComponent<IsRenderer>();
            world.Schema.RegisterComponent<IsMesh>();
            world.Schema.RegisterComponent<IsShader>();
            world.Schema.RegisterComponent<IsShaderRequest>();
            world.Schema.RegisterArrayElement<MeshVertexIndex>();
            world.Schema.RegisterArrayElement<MaterialPushBinding>();
            world.Schema.RegisterArrayElement<MaterialComponentBinding>();
            world.Schema.RegisterArrayElement<MaterialTextureBinding>();
            world.Schema.RegisterArrayElement<DestinationExtension>();
        }
    }
}
