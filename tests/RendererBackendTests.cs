using Data.Components;
using Meshes;
using Meshes.Components;
using Rendering.Components;
using Rendering.Systems;
using Shaders;
using Shaders.Components;
using Simulation.Tests;
using Worlds;

namespace Rendering.Tests
{
    public class RendererBackendTests : SimulationTests
    {
        static RendererBackendTests()
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

        [Test]
        public void CheckInitialization()
        {
            Assert.That(TestRendererBackend.initialized, Is.False);

            ref RenderingSystems renderingSystems = ref simulator.AddSystem<RenderingSystems>().Value;
            renderingSystems.RegisterRenderingBackend<TestRendererBackend>();

            Assert.That(TestRendererBackend.initialized, Is.True);

            simulator.RemoveSystem<RenderingSystems>();

            Assert.That(TestRendererBackend.initialized, Is.False);
        }

        [Test]
        public void CreatesRendererForDestination()
        {
            Destination testDestination = new(world, new(200, 200), RenderingBackend.GetLabel<TestRendererBackend>());
            ref RenderingSystems renderingSystems = ref simulator.AddSystem<RenderingSystems>().Value;
            renderingSystems.RegisterRenderingBackend<TestRendererBackend>();

            simulator.Update();

            Assert.That(TestRendererBackend.renderingMachines.Count, Is.EqualTo(1));
            TestRenderer testRenderer = TestRendererBackend.renderingMachines[0].Read<TestRenderer>();
            Assert.That(testRenderer.destination, Is.EqualTo(testDestination));
        }

        [Test]
        public void IterateThroughRendererObjects()
        {
            Destination destination = new(world, new(200, 200), RenderingBackend.GetLabel<TestRendererBackend>());
            destination.AddComponent(new SurfaceInUse());

            ref RenderingSystems renderingSystems = ref simulator.AddSystem<RenderingSystems>().Value;
            renderingSystems.RegisterRenderingBackend<TestRendererBackend>();

            Mesh mesh = new(world);
            Shader shader = new(world, "vertex", "fragment");
            shader.AddComponent(new IsShader());
            Material material = new(world, shader);
            MeshRenderer meshRenderer = new(world, mesh, material);
            Viewport viewport = new(world, destination);

            simulator.Update();

            Assert.That(TestRendererBackend.renderingMachines.Count, Is.EqualTo(1));
            TestRenderer testRenderer = TestRendererBackend.renderingMachines[0].Read<TestRenderer>();
            Assert.That(testRenderer.destination, Is.EqualTo(destination));
            Assert.That(testRenderer.entities.Count, Is.EqualTo(1));

            meshRenderer.SetEnabled(false);
            simulator.Update();

            Assert.That(testRenderer.entities.Count, Is.EqualTo(0));
        }
    }
}
