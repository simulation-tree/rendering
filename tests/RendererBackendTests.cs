using Data.Components;
using Rendering.Components;
using Rendering.Systems;
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

            Assert.That(TestRendererBackend.renderers.Count, Is.EqualTo(1));
            TestRenderer testRenderer = TestRendererBackend.renderers[0].Read<TestRenderer>();
            Assert.That(testRenderer.destination, Is.EqualTo(testDestination));
        }
    }
}
