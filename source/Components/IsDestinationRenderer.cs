using Unmanaged;

namespace Rendering.Components
{
    public readonly struct IsDestinationRenderer
    {
        public readonly FixedString label;

        public IsDestinationRenderer(FixedString label)
        {
            this.label = label;
        }
    }
}