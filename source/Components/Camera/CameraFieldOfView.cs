using System;

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

        public static CameraFieldOfView FromDegrees(float valueInDegrees)
        {
            return new CameraFieldOfView(MathF.PI * valueInDegrees / 180.0f);
        }
    }
}
