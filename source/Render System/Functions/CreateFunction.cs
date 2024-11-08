using Unmanaged;

namespace Rendering
{
    /// <summary>
    /// Creates a system instance for an existing destination entity.
    /// </summary>
    public unsafe readonly struct CreateFunction
    {
#if NET
        private readonly delegate* unmanaged<Destination, FixedString*, uint, CreateResult> function;

        public CreateFunction(delegate* unmanaged<Destination, FixedString*, uint, CreateResult> function)
        {
            this.function = function;
        }
#else
        private readonly delegate*<Destination, FixedString*, uint, CreateResult> function;

        public CreateFunction(delegate*<Destination, FixedString*, uint, CreateResult> function)
        {
            this.function = function;
        }
#endif

        public readonly CreateResult Invoke(Destination destination, USpan<FixedString> extensionNames)
        {
            return function(destination, (FixedString*)extensionNames.Address, extensionNames.Length);
        }
    }
}