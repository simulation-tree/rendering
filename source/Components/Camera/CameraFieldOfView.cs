using System;

namespace Rendering.Components
{
    public struct CameraFieldOfView
    {
        /// <summary>
        /// Value in radians.
        /// </summary>
        public float value;

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
