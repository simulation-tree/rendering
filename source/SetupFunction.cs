namespace Rendering
{
    public unsafe readonly struct SetupFunction
    {
        private readonly delegate* unmanaged<nint, int, nint> function;

        public SetupFunction(delegate* unmanaged<nint, int, nint> function)
        {
            this.function = function;
        }

        public readonly nint Invoke(nint names, int nameCount)
        {
            return function(names, nameCount);
        }
    }
}