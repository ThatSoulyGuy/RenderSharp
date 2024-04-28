using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;

using Device = SharpDX.Direct3D11.Device;

namespace RenderStar.Render
{
    public static class Renderer
    {
        public static Device Device { get; private set; } = null!;
        public static SwapChain SwapChain { get; private set; } = null!;
        public static DeviceContext Context { get; private set; } = null!;

        public static float AspectRatio => SwapChain.Description.ModeDescription.Width / (float)SwapChain.Description.ModeDescription.Height;

        public static Vector2 ClientSize => new(SwapChain.Description.ModeDescription.Width, SwapChain.Description.ModeDescription.Height);

        public static Vector3 WindowColor { get; set; } = new(0.0f, 0.45f, 0.75f);

        private static RenderTargetView RenderTargetView { get; set; } = null!;

        public static void Initialize(Form form)
        {
            SwapChainDescription swapChainDescription = new()
            {
                BufferCount = 1,
                ModeDescription = new(form.ClientSize.Width, form.ClientSize.Height, new(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = form.Handle,
                SampleDescription = new(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDescription, out Device device, out SwapChain swapChain);
            Device = device;
            SwapChain = swapChain;

            Context = Device.ImmediateContext;

            using (Texture2D backBuffer = SwapChain.GetBackBuffer<Texture2D>(0))
            {
                RenderTargetView = new(Device, backBuffer);
            }

            Context.OutputMerger.SetRenderTargets(RenderTargetView);

            Viewport viewport = new(0, 0, form.ClientSize.Width, form.ClientSize.Height);
            Context.Rasterizer.SetViewport(viewport);
        }

        public static void Resize(Form form)
        {
            if (Device == null)
                return;

            RenderTargetView.Dispose();

            SwapChain.ResizeBuffers(1, form.ClientSize.Width, form.ClientSize.Height, Format.R8G8B8A8_UNorm, SwapChainFlags.None);

            using (Texture2D backBuffer = SwapChain.GetBackBuffer<Texture2D>(0))
            {
                RenderTargetView = new(Device, backBuffer);
            }

            Context.OutputMerger.SetRenderTargets(RenderTargetView);

            Viewport viewport = new(0, 0, form.ClientSize.Width, form.ClientSize.Height);
            Context.Rasterizer.SetViewport(viewport);
        }

        public static void PreRender()
        {
            if (Context == null)
                return;

            Context.ClearRenderTargetView(RenderTargetView, new Color4(WindowColor, 1.0f));
        }

        public static void PostRender()
        {
            SwapChain.Present(1, PresentFlags.None);
        }

        public static void CleanUp()
        {
            RenderTargetView.Dispose();
            SwapChain.Dispose();
            Device.Dispose();
        }
    }
}