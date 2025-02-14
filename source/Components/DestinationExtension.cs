using Unmanaged;

namespace Rendering
{
    public readonly struct DestinationExtension
    {
        public readonly FixedString value;

        public DestinationExtension(FixedString value)
        {
            this.value = value;
        }

        public DestinationExtension(USpan<char> value)
        {
            this.value = new(value);
        }
    }
}