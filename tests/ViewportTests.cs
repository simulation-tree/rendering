using System.Numerics;
using Worlds;

namespace Rendering.Tests
{
    public class ViewportTests : RenderingTests
    {
        [Test]
        public void VerifyCompliance()
        {
            using World world = CreateWorld();
            Destination destination = new(world, new Vector2(1920, 1080), "Renderer");
            Viewport viewport = new(world, destination);
            Assert.That(viewport.IsCompliant, Is.True);
            Assert.That(viewport.Destination, Is.EqualTo(destination));
        }
    }
}
