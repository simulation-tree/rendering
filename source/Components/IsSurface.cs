namespace Rendering.Components
{
    public readonly struct IsSurface
    {
        public readonly nint address;

        public IsSurface(nint address)
        {
            this.address = address;
        }
    }
}