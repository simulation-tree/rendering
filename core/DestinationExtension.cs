using Unmanaged;
using Worlds;

namespace Rendering
{
    [ArrayElement]
    public readonly struct DestinationExtension
    {
        public readonly FixedString text;

        public DestinationExtension(FixedString text)
        {
            this.text = text;
        }

        public DestinationExtension(USpan<char> text)
        {
            this.text = new(text);
        }
    }
}