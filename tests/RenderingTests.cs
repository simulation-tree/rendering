using Simulation.Tests;
using Types;
using Worlds;

namespace Rendering.Tests
{
    public abstract class RenderingTests : SimulationTests
    {
        static RenderingTests()
        {
            TypeRegistry.Load<Data.Core.TypeBank>();
            TypeRegistry.Load<Meshes.TypeBank>();
            TypeRegistry.Load<Rendering.Core.TypeBank>();
            TypeRegistry.Load<Shaders.TypeBank>();
        }

        protected override Schema CreateSchema()
        {
            Schema schema = base.CreateSchema();
            schema.Load<Data.Core.SchemaBank>();
            schema.Load<Meshes.SchemaBank>();
            schema.Load<Rendering.Core.SchemaBank>();
            schema.Load<Shaders.SchemaBank>();
            return schema;
        }
    }
}