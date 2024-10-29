using Simulation;
using Simulation.Functions;
using System;
using System.Runtime.InteropServices;

namespace Rendering.Systems
{
    public struct RenderingSystems : ISystem
    {
        private Simulator simulator;

        readonly unsafe InitializeFunction ISystem.Initialize => new(&Initialize);
        readonly unsafe IterateFunction ISystem.Update => new(&Update);
        readonly unsafe FinalizeFunction ISystem.Finalize => new(&Finalize);

        [UnmanagedCallersOnly]
        private static void Initialize(SystemContainer container, World world)
        {
            if (container.World == world)
            {
                Simulator simulator = container.Simulator;
                ref RenderingSystems system = ref container.Read<RenderingSystems>();
                system.simulator = simulator;
                simulator.AddSystem<RenderEngineSystem>();
                simulator.AddSystem<MaterialImportSystem>();
            }
        }

        [UnmanagedCallersOnly]
        private static void Update(SystemContainer container, World world, TimeSpan delta)
        {
        }

        [UnmanagedCallersOnly]
        private static void Finalize(SystemContainer container, World world)
        {
            if (container.World == world)
            {
                Simulator simulator = container.Simulator;
                simulator.RemoveSystem<MaterialImportSystem>();
                simulator.RemoveSystem<RenderEngineSystem>();
            }
        }

        public readonly void RegisterRenderSystem<T>() where T : unmanaged, IRenderer
        {
            ref RenderEngineSystem renderEngineSystem = ref simulator.GetSystem<RenderEngineSystem>().Value;
            renderEngineSystem.RegisterRenderSystem<T>();
        }
    }
}