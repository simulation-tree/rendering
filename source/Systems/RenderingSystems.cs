using Simulation;
using Simulation.Functions;
using System;
using System.Runtime.InteropServices;
using Worlds;

namespace Rendering.Systems
{
    public struct RenderingSystems : ISystem
    {
        private Simulator simulator;

        readonly unsafe StartSystem ISystem.Start => new(&Start);
        readonly unsafe UpdateSystem ISystem.Update => new(&Update);
        readonly unsafe FinishSystem ISystem.Finish => new(&Finish);

        [UnmanagedCallersOnly]
        private static void Start(SystemContainer container, World world)
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
        private static void Finish(SystemContainer container, World world)
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