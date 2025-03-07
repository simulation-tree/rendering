using Unmanaged;

namespace Rendering
{
    public readonly struct DestinationExtension
    {
        public readonly ASCIIText256 value;

        public DestinationExtension(ASCIIText256 value)
        {
            this.value = value;
        }

        public DestinationExtension(USpan<char> value)
        {
            this.value = new(value);
        }
    }
}