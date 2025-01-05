using Collections;
using Simulation;
using System;
using System.Numerics;
using Unmanaged;

namespace Rendering.Tests
{
    public readonly partial struct TestRendererBackend : IRenderingBackend
    {
        public static bool initialized;
        public static readonly System.Collections.Generic.List<Allocation> renderers = new();

        FixedString IRenderingBackend.Label => "test";

        void IRenderingBackend.Initialize()
        {
            initialized = true;
        }

        void IRenderingBackend.Finalize()
        {
            initialized = false;
        }

        (Allocation renderer, Allocation instance) IRenderingBackend.Create(in Destination destination, in USpan<FixedString> extensionNames)
        {
            Allocation renderer = Allocation.Create(new TestRenderer(destination, extensionNames));
            renderers.Add(renderer);
            return (renderer, renderer);
        }

        void IRenderingBackend.Dispose(in Allocation renderer)
        {
            ref TestRenderer testRenderer = ref renderer.Read<TestRenderer>();
            testRenderer.Dispose();
            renderers.Remove(renderer);
            renderer.Dispose();
        }

        void IRenderingBackend.SurfaceCreated(in Allocation renderer, Allocation surface)
        {
            throw new System.NotImplementedException();
        }

        StatusCode IRenderingBackend.BeginRender(in Allocation renderer, in Vector4 clearColor)
        {
            throw new System.NotImplementedException();
        }

        void IRenderingBackend.Render(in Allocation renderer, in USpan<uint> entities, in uint material, in uint shader, in uint mesh)
        {
            throw new System.NotImplementedException();
        }

        void IRenderingBackend.EndRender(in Allocation renderer)
        {
            throw new System.NotImplementedException();
        }
    }

    public readonly struct TestRenderer : IDisposable
    {
        public readonly Destination destination;
        public readonly Array<FixedString> extensionNames;

        public TestRenderer(Destination destination, USpan<FixedString> extensionNames)
        {
            this.destination = destination;
            this.extensionNames = new(extensionNames);
        }

        public void Dispose()
        {
            extensionNames.Dispose();
        }
    }
}
