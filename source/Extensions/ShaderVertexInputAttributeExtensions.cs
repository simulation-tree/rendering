using Meshes;
using Shaders;
using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Types;
using Unmanaged;

namespace Rendering
{
    [SkipLocalsInit]
    public static class ShaderVertexInputAttributeExtensions
    {
        private static readonly TypeLayout vector2Type;
        private static readonly TypeLayout vector3Type;
        private static readonly TypeLayout vector4Type;

        static ShaderVertexInputAttributeExtensions()
        {
            vector2Type = TypeRegistry.Get<Vector2>();
            vector3Type = TypeRegistry.Get<Vector3>();
            vector4Type = TypeRegistry.Get<Vector4>();
        }

        /// <summary>
        /// Tries to retrieve the <paramref name="meshChannel"/> for the shader attribute based on its name.
        /// </summary>
        public static bool TryDeduceMeshChannel(this ShaderVertexInputAttribute attribute, out MeshChannel meshChannel)
        {
            //get lowercase version
            byte length = attribute.name.Length;
            USpan<char> nameBuffer = stackalloc char[length];
            attribute.name.CopyTo(nameBuffer);
            for (uint i = 0; i < length; i++)
            {
                nameBuffer[i] = char.ToLower(nameBuffer[i]);
            }

            TypeLayout attributeType = attribute.Type;
            if (attributeType == vector2Type)
            {
                if (nameBuffer.Contains("uv".AsSpan()))
                {
                    meshChannel = MeshChannel.UV;
                    return true;
                }
            }
            else if (attributeType == vector3Type)
            {
                if (nameBuffer.Contains("normal".AsSpan()))
                {
                    meshChannel = MeshChannel.Normal;
                    return true;
                }
                else if (nameBuffer.Contains("tangent".AsSpan()))
                {
                    meshChannel = MeshChannel.Tangent;
                    return true;
                }
                else if (nameBuffer.Contains("position".AsSpan()))
                {
                    meshChannel = MeshChannel.Position;
                    return true;
                }
                else if (nameBuffer.Contains("bitangent".AsSpan()))
                {
                    meshChannel = MeshChannel.BiTangent;
                    return true;
                }
            }
            else if (attributeType == vector4Type)
            {
                if (nameBuffer.Contains("color".AsSpan()))
                {
                    meshChannel = MeshChannel.Color;
                    return true;
                }
            }

            meshChannel = default;
            return false;
        }
    }
}