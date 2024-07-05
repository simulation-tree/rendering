using System.Numerics;

namespace Rendering.Components
{
    public readonly struct CameraProjection
    {
        public readonly Matrix4x4 projection;
        public readonly Matrix4x4 view;

        public CameraProjection(Matrix4x4 projection, Matrix4x4 view)
        {
            this.projection = projection;
            this.view = view;
        }
    }
}
