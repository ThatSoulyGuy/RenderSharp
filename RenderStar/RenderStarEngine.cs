using Engine.Core;
using RenderStar;

namespace Engine
{
    public static class RenderStarEngine
    {
        public static AssemblyLoader EngineLoader { get; private set; } = null!;

        [STAThread]
        static void Main()
        {
#if DEBUG
            EngineLoader = AssemblyLoader.Create("Engine", "DLLs/Debug/net8.0-windows/Engine.dll");
#else
            EngineLoader = AssemblyLoader.Create("Engine", "Engine.dll");
#endif

            ApplicationConfiguration.Initialize();
            Application.Run(new Window());
        }
    }
}