using Engine;

using Timer = System.Windows.Forms.Timer;

namespace RenderStar
{
    public partial class Window : Form
    {
        public Timer UpdateTimer { get; private set; } = null!;

        public Window()
        {
            RenderStarEngine.EngineLoader.CallMethod("RenderStar.Engine", "PreInitialize", this);

            InitializeComponent();
            RenderStarEngine.EngineLoader.CallMethod("RenderStar.Engine", "Initialize");
        }

        protected override void OnLoad(EventArgs arguments)
        {
            base.OnLoad(arguments);

            UpdateTimer = new()
            {
                Interval = 1
            };

            UpdateTimer.Tick += new(OnTimerTick);
            UpdateTimer.Start();
        }

        private void OnTimerTick(object? sender, EventArgs arguments)
        {
            RenderStarEngine.EngineLoader.CallMethod("RenderStar.Engine", "Update");
            RenderStarEngine.EngineLoader.CallMethod("RenderStar.Engine", "Render");
        }

        protected override void OnResize(EventArgs arguments)
        {
            base.OnResize(arguments);

            if (WindowState != FormWindowState.Minimized)
                RenderStarEngine.EngineLoader.CallMethod("RenderStar.Engine", "Resize", this);
        }

        protected override void OnFormClosing(FormClosingEventArgs arguments)
        {
            base.OnFormClosing(arguments);

            RenderStarEngine.EngineLoader.CallMethod("RenderStar.Engine", "CleanUp");
        }
    }
}