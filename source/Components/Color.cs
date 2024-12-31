using System.Numerics;
using Worlds;

namespace Rendering
{
    [Component]
    public struct Color
    {
        public static readonly Color White = new(1, 1, 1, 1);
        public static readonly Color Black = new(0, 0, 0, 1);
        public static readonly Color Grey = new(0.5f, 0.5f, 0.5f, 1);
        public static readonly Color Red = new(1, 0, 0, 1);
        public static readonly Color Green = new(0, 1, 0, 1);
        public static readonly Color Blue = new(0, 0, 1, 1);
        public static readonly Color Yellow = new(1, 1, 0, 1);
        public static readonly Color Cyan = new(0, 1, 1, 1);
        public static readonly Color Magenta = new(1, 0, 1, 1);
        public static readonly Color Orange = new(1, 0.5f, 0, 1);
        public static readonly Color Chartreuse = new(0.5f, 1, 0f, 1);
        public static readonly Color SpringGreen = new(0, 1, 0.5f, 1);
        public static readonly Color SkyBlue = new(0, 0.5f, 1, 1);
        public static readonly Color Violet = new(0.5f, 0, 1, 1);
        public static readonly Color Rose = new(1, 0, 0.5f, 1);

        public Vector4 value;

        public Color(Vector4 value)
        {
            this.value = value;
        }

        public Color(float r, float g, float b, float a = 1f)
        {
            value = new(r, g, b, a);
        }
    }
}