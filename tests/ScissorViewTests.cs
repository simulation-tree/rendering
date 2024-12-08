using Rendering.Components;
using Rendering.Systems;
using Simulation.Tests;
using System.Numerics;
using Worlds;

namespace Rendering.Tests
{
    public class ScissorViewTests : SimulationTests
    {
        protected override void SetUp()
        {
            base.SetUp();
            ComponentType.Register<RendererScissor>();
            ComponentType.Register<WorldRendererScissor>();
            Simulator.AddSystem<ClampNestedScissorViews>();
        }

        [Test]
        public void VerifyClampedScissor()
        {
            uint parentScissor = World.CreateEntity();
            uint childScissor = World.CreateEntity();
            World.SetParent(childScissor, parentScissor);
            World.AddComponent(parentScissor, new RendererScissor(0, 0, 100, 100));
            World.AddComponent(childScissor, new RendererScissor(50, 50, 100, 100));

            Simulator.Update();

            WorldRendererScissor parent = World.GetComponent<WorldRendererScissor>(parentScissor);
            WorldRendererScissor child = World.GetComponent<WorldRendererScissor>(childScissor);
            Assert.That(parent.value, Is.EqualTo(new Vector4(0, 0, 100, 100)));
            Assert.That(child.value, Is.EqualTo(new Vector4(50, 50, 50, 50)));
        }

        [Test]
        public void OutOfBoundsChildScissor()
        {
            uint parentScissor = World.CreateEntity();
            uint childScissor = World.CreateEntity();
            World.SetParent(childScissor, parentScissor);
            World.AddComponent(parentScissor, new RendererScissor(0, 0, 100, 100));
            World.AddComponent(childScissor, new RendererScissor(0, 200, 100, 20));

            Simulator.Update();

            WorldRendererScissor parent = World.GetComponent<WorldRendererScissor>(parentScissor);
            WorldRendererScissor child = World.GetComponent<WorldRendererScissor>(childScissor);
            Assert.That(parent.value, Is.EqualTo(new Vector4(0, 0, 100, 100)));
            Assert.That(child.value, Is.EqualTo(new Vector4(0, 100, 100, 0)));
        }

        [Test]
        public void DeepNestedChildren()
        {
            uint rootScissor = World.CreateEntity();
            uint parentScissor = World.CreateEntity();
            uint childScissor = World.CreateEntity();
            World.SetParent(parentScissor, rootScissor);
            World.SetParent(childScissor, parentScissor);
            World.AddComponent(rootScissor, new RendererScissor(0, 0, 100, 100));
            World.AddComponent(parentScissor, new RendererScissor(50, 50, 100, 100));
            World.AddComponent(childScissor, new RendererScissor(25, 25, 50, 50));

            Simulator.Update();

            WorldRendererScissor root = World.GetComponent<WorldRendererScissor>(rootScissor);
            WorldRendererScissor parent = World.GetComponent<WorldRendererScissor>(parentScissor);
            WorldRendererScissor child = World.GetComponent<WorldRendererScissor>(childScissor);
            Assert.That(root.value, Is.EqualTo(new Vector4(0, 0, 100, 100)));
            Assert.That(parent.value, Is.EqualTo(new Vector4(50, 50, 50, 50)));
            Assert.That(child.value, Is.EqualTo(new Vector4(50, 50, 25, 25)));
        }

        [Test]
        public void VerifyDeepDescendant()
        {
            uint rootEntity = World.CreateEntity();
            uint parentScissor = World.CreateEntity();
            uint childScissor = World.CreateEntity();
            World.SetParent(parentScissor, rootEntity);
            World.SetParent(childScissor, parentScissor);
            World.AddComponent(rootEntity, new RendererScissor(0, 0, 100, 100));
            World.AddComponent(childScissor, new RendererScissor(50, 50, 100, 100));

            Simulator.Update();

            WorldRendererScissor root = World.GetComponent<WorldRendererScissor>(rootEntity);
            WorldRendererScissor child = World.GetComponent<WorldRendererScissor>(childScissor);
            Assert.That(root.value, Is.EqualTo(new Vector4(0, 0, 100, 100)));
            Assert.That(child.value, Is.EqualTo(new Vector4(50, 50, 50, 50)));
        }
    }
}
