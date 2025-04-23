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
            MetadataRegistry.Load<DataMetadataBank>();
            MetadataRegistry.Load<MeshesMetadataBank>();
            MetadataRegistry.Load<MaterialsMetadataBank>();
            MetadataRegistry.Load<RenderingMetadataBank>();
            MetadataRegistry.Load<ShadersMetadataBank>();
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