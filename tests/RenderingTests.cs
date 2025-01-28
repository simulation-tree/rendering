using Data;
using Meshes;
using Shaders;
using Simulation.Tests;
using Types;
using Worlds;

namespace Rendering.Tests
{
    public abstract class RenderingTests : SimulationTests
    {
        static RenderingTests()
        {
            TypeRegistry.Load<DataTypeBank>();
            TypeRegistry.Load<MeshesTypeBank>();
            TypeRegistry.Load<RenderingTypeBank>();
            TypeRegistry.Load<ShadersTypeBank>();
        }

        protected override Schema CreateSchema()
        {
            Schema schema = base.CreateSchema();
            schema.Load<DataSchemaBank>();
            schema.Load<MeshesSchemaBank>();
            schema.Load<RenderingSchemaBank>();
            schema.Load<ShadersSchemaBank>();
            return schema;
        }
    }
}