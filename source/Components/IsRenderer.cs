using Meshes;
using Simulation;

namespace Rendering.Components
{
    public struct IsRenderer
    {
        public eint mesh;
        public eint material;
        public eint camera;

        private IsRenderer(eint mesh, eint material, eint camera)
        {
            this.mesh = mesh;
            this.material = material;
            this.camera = camera;
        }

        public static IsRenderer Create<MS, MT, C>(MS mesh, MT material, C camera) where MS : IMesh where MT : IMaterial where C : ICamera
        {
            return new(mesh.Value, material.Value, camera.Value);
        }
    }
}