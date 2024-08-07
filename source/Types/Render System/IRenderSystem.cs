using Unmanaged;

namespace Rendering
{
    public interface IRenderSystem
    {
        static abstract FixedString Label { get; }

        /// <summary>
        /// Retrieve the functions that compose a "render system".
        /// <para>Called just once, and balanced by the <see cref="FinishFunction"/>.</para>
        /// </summary>
        static abstract (CreateFunction, DestroyFunction, RenderFunction, FinishFunction) GetFunctions();
    }
}