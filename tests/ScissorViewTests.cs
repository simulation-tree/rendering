using Rendering.Components;
using Rendering.Systems;
using Simulation.Tests;
using System.Numerics;
using Worlds;

namespace Rendering.Tests
{
    public class ScissorViewTests : SimulationTests
    {
        static ScissorViewTests()
        {
            TypeLayout.Register<RendererScissor>("RendererScissor");
            TypeLayout.Register<WorldRendererScissor>("WorldRendererScissor");
        }

        protected override void SetUp()
        {
            base.SetUp();
            world.Schema.RegisterComponent<RendererScissor>();
            world.Schema.RegisterComponent<WorldRendererScissor>();
            simulator.AddSystem<ClampNestedScissorViews>();
        }

        [Test]
        public void VerifyClampedScissor()
        {
            uint parentScissor = world.CreateEntity();
            uint childScissor = world.CreateEntity();
            world.SetParent(childScissor, parentScissor);
            world.AddComponent(parentScissor, new RendererScissor(0, 0, 100, 100));
            world.AddComponent(childScissor, new RendererScissor(50, 50, 100, 100));

            simulator.Update();

            WorldRendererScissor parent = world.GetComponent<WorldRendererScissor>(parentScissor);
            WorldRendererScissor child = world.GetComponent<WorldRendererScissor>(childScissor);
            Assert.That(parent.value, Is.EqualTo(new Vector4(0, 0, 100, 100)));
            Assert.That(child.value, Is.EqualTo(new Vector4(50, 50, 50, 50)));
        }

        [Test]
        public void OutOfBoundsChildScissor()
        {
            uint parentScissor = world.CreateEntity();
            uint childScissor = world.CreateEntity();
            world.SetParent(childScissor, parentScissor);
            world.AddComponent(parentScissor, new RendererScissor(0, 0, 100, 100));
            world.AddComponent(childScissor, new RendererScissor(0, 200, 100, 20));

            simulator.Update();

            WorldRendererScissor parent = world.GetComponent<WorldRendererScissor>(parentScissor);
            WorldRendererScissor child = world.GetComponent<WorldRendererScissor>(childScissor);
            Assert.That(parent.value, Is.EqualTo(new Vector4(0, 0, 100, 100)));
            Assert.That(child.value, Is.EqualTo(new Vector4(0, 100, 100, 0)));
        }

        [Test]
        public void DeepNestedChildren()
        {
            uint rootScissor = world.CreateEntity();
            uint parentScissor = world.CreateEntity();
            uint childScissor = world.CreateEntity();
            world.SetParent(parentScissor, rootScissor);
            world.SetParent(childScissor, parentScissor);
            world.AddComponent(rootScissor, new RendererScissor(0, 0, 100, 100));
            world.AddComponent(parentScissor, new RendererScissor(50, 50, 100, 100));
            world.AddComponent(childScissor, new RendererScissor(25, 25, 50, 50));

            simulator.Update();

            WorldRendererScissor root = world.GetComponent<WorldRendererScissor>(rootScissor);
            WorldRendererScissor parent = world.GetComponent<WorldRendererScissor>(parentScissor);
            WorldRendererScissor child = world.GetComponent<WorldRendererScissor>(childScissor);
            Assert.That(root.value, Is.EqualTo(new Vector4(0, 0, 100, 100)));
            Assert.That(parent.value, Is.EqualTo(new Vector4(50, 50, 50, 50)));
            Assert.That(child.value, Is.EqualTo(new Vector4(50, 50, 25, 25)));
        }

        [Test]
        public void VerifyDeepDescendant()
        {
            uint rootEntity = world.CreateEntity();
            uint parentScissor = world.CreateEntity();
            uint childScissor = world.CreateEntity();
            world.SetParent(parentScissor, rootEntity);
            world.SetParent(childScissor, parentScissor);
            world.AddComponent(rootEntity, new RendererScissor(0, 0, 100, 100));
            world.AddComponent(childScissor, new RendererScissor(50, 50, 100, 100));

            simulator.Update();

            WorldRendererScissor root = world.GetComponent<WorldRendererScissor>(rootEntity);
            WorldRendererScissor child = world.GetComponent<WorldRendererScissor>(childScissor);
            Assert.That(root.value, Is.EqualTo(new Vector4(0, 0, 100, 100)));
            Assert.That(child.value, Is.EqualTo(new Vector4(50, 50, 50, 50)));
        }
    }
}
