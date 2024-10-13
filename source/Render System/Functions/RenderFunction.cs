using Unmanaged;

namespace Rendering.Functions
{
    /// <summary>
    /// Renders a batch of entities using the same material, mesh and camera combination.
    /// </summary>
    public unsafe readonly struct RenderFunction
    {
#if NET
        private readonly delegate* unmanaged<Input, void> function;

        public RenderFunction(delegate* unmanaged<Input, void> function)
        {
            this.function = function;
        }
#else
        private readonly delegate*<Input, void> function;

        public RenderFunction(delegate*<Input, void> function)
        {
            this.function = function;
        }
#endif

        public readonly void Invoke(Allocation system, USpan<uint> renderers, uint material, uint shader, uint mesh)
        {
            function(new(system, renderers, material, shader, mesh));
        }

        public readonly struct Input
        {
            public readonly Allocation system;
            public readonly uint material;
            public readonly uint shader;
            public readonly uint mesh;

            private readonly nint address;
            private readonly uint length;

            /// <summary>
            /// All entities containing the same filter, callback and identifier combinations.
            /// </summary>
            public readonly USpan<uint> Renderers => new(address, length);

            public Input(Allocation system, USpan<uint> renderers, uint material, uint shader, uint mesh)
            {
                this.system = system;
                this.material = material;
                this.shader = shader;
                this.mesh = mesh;
                address = renderers.Address;
                length = renderers.Length;
            }
        }
    }
}