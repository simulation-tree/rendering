using Simulation.Tests;
using Types;
using Worlds;

namespace Rendering.Tests
{
    public abstract class RenderingTests : SimulationTests
    {
        static RenderingTests()
        {
            TypeRegistry.Load<Data.TypeBank>();
            TypeRegistry.Load<Meshes.TypeBank>();
            TypeRegistry.Load<Rendering.TypeBank>();
            TypeRegistry.Load<Shaders.TypeBank>();
        }

        protected override Schema CreateSchema()
        {
            Schema schema = base.CreateSchema();
            schema.Load<Data.SchemaBank>();
            schema.Load<Meshes.SchemaBank>();
            schema.Load<Rendering.SchemaBank>();
            schema.Load<Shaders.SchemaBank>();
            return schema;
        }
    }
}