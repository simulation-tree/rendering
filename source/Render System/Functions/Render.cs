using Unmanaged;

namespace Rendering.Functions
{
    /// <summary>
    /// Renders a batch of entities using the same material, mesh and camera combination.
    /// </summary>
    public unsafe readonly struct Render
    {
#if NET
        private readonly delegate* unmanaged<Input, void> function;

        public Render(delegate* unmanaged<Input, void> function)
        {
            this.function = function;
        }
#else
        private readonly delegate*<Input, void> function;

        public Render(delegate*<Input, void> function)
        {
            this.function = function;
        }
#endif

        public readonly void Invoke(Allocation backend, Allocation renderer, USpan<uint> entities, uint material, uint shader, uint mesh)
        {
            function(new(backend, renderer, entities, material, shader, mesh));
        }

        public readonly struct Input
        {
            public readonly Allocation backend;
            public readonly Allocation renderer;
            public readonly uint material;
            public readonly uint shader;
            public readonly uint mesh;

            private readonly nint entities;
            private readonly uint count;

            /// <summary>
            /// All entities containing the same filter, callback and identifier combinations.
            /// </summary>
            public readonly USpan<uint> Entities => new(entities, count);

            public Input(Allocation backend, Allocation renderer, USpan<uint> entities, uint material, uint shader, uint mesh)
            {
                this.backend = backend;
                this.renderer = renderer;
                this.material = material;
                this.shader = shader;
                this.mesh = mesh;
                this.entities = entities.Address;
                count = entities.Length;
            }
        }
    }
}