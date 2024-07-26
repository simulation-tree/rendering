namespace Rendering.Components
{
    public readonly struct CameraFieldOfView
    {
        /// <summary>
        /// Value in radians.
        /// </summary>
        public readonly float value;

        public CameraFieldOfView(float value)
        {
            this.value = value;
        }
    }
}
