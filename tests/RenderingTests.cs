using Data;
using Materials;
using Meshes;
using Shaders;
using Types;
using Worlds;
using Worlds.Tests;

namespace Rendering.Tests
{
    public abstract class RenderingTests : WorldTests
    {
        static RenderingTests()
        {
            TypeRegistry.Load<DataTypeBank>();
            TypeRegistry.Load<MeshesTypeBank>();
            TypeRegistry.Load<MaterialsTypeBank>();
            TypeRegistry.Load<RenderingTypeBank>();
            TypeRegistry.Load<ShadersTypeBank>();
        }

        protected override Schema CreateSchema()
        {
            Schema schema = base.CreateSchema();
            schema.Load<DataSchemaBank>();
            schema.Load<MeshesSchemaBank>();
            schema.Load<MaterialsSchemaBank>();
            schema.Load<RenderingSchemaBank>();
            schema.Load<ShadersSchemaBank>();
            return schema;
        }
    }
}