using Meshes;
using Shaders;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Types;

namespace Rendering
{
    [SkipLocalsInit]
    public static class ShaderVertexInputAttributeExtensions
    {
        private static readonly TypeMetadata vector2Type;
        private static readonly TypeMetadata vector3Type;
        private static readonly TypeMetadata vector4Type;

        static ShaderVertexInputAttributeExtensions()
        {
            vector2Type = MetadataRegistry.GetType<Vector2>();
            vector3Type = MetadataRegistry.GetType<Vector3>();
            vector4Type = MetadataRegistry.GetType<Vector4>();
        }

        /// <summary>
        /// Tries to retrieve the <paramref name="meshChannel"/> for the shader attribute based on its name.
        /// </summary>
        public static bool TryDeduceMeshChannel(this ShaderVertexInputAttribute attribute, out MeshChannel meshChannel)
        {
            //get lowercase version
            int length = attribute.name.Length;
            Span<char> nameBuffer = stackalloc char[length];
            attribute.name.CopyTo(nameBuffer);
            for (int i = 0; i < length; i++)
            {
                nameBuffer[i] = char.ToLower(nameBuffer[i]);
            }

            if (attribute.type == vector2Type)
            {
                if (nameBuffer.IndexOf("uv") != -1)
                {
                    meshChannel = MeshChannel.UV;
                    return true;
                }
            }
            else if (attribute.type == vector3Type)
            {
                if (nameBuffer.IndexOf("normal") != -1)
                {
                    meshChannel = MeshChannel.Normal;
                    return true;
                }
                else if (nameBuffer.IndexOf("tangent") != -1)
                {
                    meshChannel = MeshChannel.Tangent;
                    return true;
                }
                else if (nameBuffer.IndexOf("position") != -1)
                {
                    meshChannel = MeshChannel.Position;
                    return true;
                }
                else if (nameBuffer.IndexOf("bitangent") != -1)
                {
                    meshChannel = MeshChannel.BiTangent;
                    return true;
                }
            }
            else if (attribute.type == vector4Type)
            {
                if (nameBuffer.IndexOf("color") != -1)
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