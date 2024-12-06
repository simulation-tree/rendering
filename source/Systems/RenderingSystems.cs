using Simulation;
using System;
using Worlds;

namespace Rendering.Systems
{
    public partial struct RenderingSystems : ISystem
    {
        private readonly Simulator simulator;

        void ISystem.Start(in SystemContainer systemContainer, in World world)
        {
            if (systemContainer.World == world)
            {
                systemContainer.allocation.Write(new RenderingSystems(systemContainer.Simulator));
            }
        }

        void ISystem.Update(in SystemContainer systemContainer, in World world, in TimeSpan delta)
        {
        }

        void ISystem.Finish(in SystemContainer systemContainer, in World world)
        {
        }

        private RenderingSystems(Simulator simulator)
        {
            this.simulator = simulator;
            simulator.AddSystem(new RenderEngineSystem());
            simulator.AddSystem(new MaterialImportSystem());
        }

        public readonly void Dispose()
        {
            simulator.RemoveSystem<MaterialImportSystem>();
            simulator.RemoveSystem<RenderEngineSystem>();
        }

        public readonly void RegisterRenderSystem<T>() where T : unmanaged, IRenderer
        {
            ref RenderEngineSystem renderEngineSystem = ref simulator.GetSystem<RenderEngineSystem>().Value;
            renderEngineSystem.RegisterRenderSystem<T>();
        }
    }
}