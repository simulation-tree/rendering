namespace Rendering.Functions
{
    /// <summary>
    /// Disposes and cleans up any resources for this render system type.
    /// <para>Called just once and is balanced by the <see cref="IRenderingBackend"/> method.</para>
    /// </summary>
    public unsafe readonly struct FinishRenderer
    {
#if NET
        private readonly delegate* unmanaged<void> function;

        public FinishRenderer(delegate* unmanaged<void> function)
        {
            this.function = function;
        }
#else
        private readonly delegate*<void> function;

        public FinishFunction(delegate*<void> function)
        {
            this.function = function;
        }
#endif

        public readonly void Invoke()
        {
            function();
        }
    }
}