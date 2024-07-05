namespace Rendering.Components
{
    public struct IsCamera
    {
        public float minDepth;
        public float maxDepth;

        public IsCamera(float minDepth, float maxDepth)
        {
            this.minDepth = minDepth;
            this.maxDepth = maxDepth;
        }
    }
}
