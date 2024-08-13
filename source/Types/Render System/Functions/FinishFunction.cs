namespace Rendering
{
    /// <summary>
    /// Disposes and cleans up any resources for this render system type.
    /// <para>Called just once and is balanced by the <see cref="IRenderSystem.GetFunctions"/> method.</para>
    /// </summary>
    public unsafe readonly struct FinishFunction
    {
#if NET
        private readonly delegate* unmanaged<void> function;

        public FinishFunction(delegate* unmanaged<void> function)
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