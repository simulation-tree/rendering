namespace Rendering
{
    public unsafe readonly struct DisposeFunction
    {
        private readonly delegate* unmanaged<nint, void> function;

        public DisposeFunction(delegate* unmanaged<nint, void> function)
        {
            this.function = function;
        }

        public readonly void Invoke(nint instance)
        {
            function(instance);
        }
    }
}