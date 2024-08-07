namespace Rendering
{
    /// <summary>
    /// Creates a system instance for an existing destination entity.
    /// </summary>
    public unsafe readonly struct CreateFunction
    {
        private readonly delegate* unmanaged<Destination, nint, int, CreateResult> function;

        public CreateFunction(delegate* unmanaged<Destination, nint, int, CreateResult> function)
        {
            this.function = function;
        }

        public readonly CreateResult Invoke(Destination destination, nint names, int nameCount)
        {
            return function(destination, names, nameCount);
        }
    }
}