using System.Numerics;
using Worlds;

namespace Rendering.Tests
{
    public class DestinationTests : RenderingTests
    {
        [Test]
        public void VerifyCompliance()
        {
            using World world = CreateWorld();
            Destination destination = new(world, new Vector2(1920, 1080), "Renderer");
            Assert.That(destination.IsCompliant, Is.True);
            Assert.That(destination.RendererLabel.ToString(), Is.EqualTo("Renderer"));
        }
    }
}
