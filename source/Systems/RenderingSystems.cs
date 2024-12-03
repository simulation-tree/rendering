using Simulation;
using System;
using Worlds;

namespace Rendering.Systems
{
    public partial struct RenderingSystems : ISystem
    {
        private Simulator simulator;

        void ISystem.Start(in SystemContainer systemContainer, in World world)
        {
            if (systemContainer.World == world)
            {
                Simulator simulator = systemContainer.Simulator;
                this.simulator = simulator;
                simulator.AddSystem<RenderEngineSystem>();
                simulator.AddSystem<MaterialImportSystem>();
            }
        }

        void ISystem.Update(in SystemContainer systemContainer, in World world, in TimeSpan delta)
        {
        }

        void ISystem.Finish(in SystemContainer systemContainer, in World world)
        {
            if (systemContainer.World == world)
            {
                Simulator simulator = systemContainer.Simulator;
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