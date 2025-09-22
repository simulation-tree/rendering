using System;
using Unmanaged;

namespace Rendering.Arrays
{
    public readonly struct DestinationExtension
    {
        public readonly ASCIIText256 value;

        public DestinationExtension(ASCIIText256 value)
        {
            this.value = value;
        }

        public DestinationExtension(ReadOnlySpan<char> value)
        {
            this.value = new(value);
        }
    }
}