using System;

namespace Rendering.Tests
{
    public class LayerMaskTests
    {
        [Test]
        public void VerifyLayerMaskContainsLayers()
        {
            LayerMask layerMask = new();
            Layer a = 5;
            layerMask.Set(a);

            Assert.That(layerMask.Contains(a), Is.True);
        }

        [Test]
        public void CheckIfContainsAny()
        {
            LayerMask a = new();
            a.Set(0);
            a.Set(2);
            a.Set(4);

            LayerMask b = new();
            b.Set(2);
            b.Set(6);

            Assert.That(a.ContainsAny(b), Is.True);
            Assert.That(a.ContainsAll(b), Is.False);
        }

#if DEBUG
        [Test]
        public void ThrowIfOutOfRange()
        {
            Layer a = 1;
            Layer b = Layer.MaxValue;
            Assert.Throws<ArgumentOutOfRangeException>(() => new Layer(Layer.MaxValue + 1));
        }
#endif
    }
}