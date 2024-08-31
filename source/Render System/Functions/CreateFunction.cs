namespace Rendering.Functions
{
    /// <summary>
    /// Creates a system instance for an existing destination entity.
    /// </summary>
    public unsafe readonly struct CreateFunction
    {
#if NET
        private readonly delegate* unmanaged<Destination, void*, int, CreateResult> function;

        public CreateFunction(delegate* unmanaged<Destination, void*, int, CreateResult> function)
        {
            this.function = function;
        }
#else
        private readonly delegate*<Destination, void*, int, CreateResult> function;

        public CreateFunction(delegate*<Destination, void*, int, CreateResult> function)
        {
            this.function = function;
        }
#endif

        public readonly CreateResult Invoke(Destination destination, void* names, int nameCount)
        {
            return function(destination, names, nameCount);
        }
    }
}