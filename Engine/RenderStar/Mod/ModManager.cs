using RenderStar.Core;
using System.Reflection;

namespace RenderStar.Mod
{
    public static class ModManager
    {
        private static Dictionary<string, Mod> LoadedMods { get; } = [];

        public static void LoadFrom(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string[] files = Directory.GetFiles(path, "*.dll");

            foreach (string file in files)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    Type[] types = assembly.GetTypes();

                    foreach (Type type in types)
                    {
                        if (type.IsSubclassOf(typeof(Mod)) && !type.IsAbstract)
                        {
                            Mod modInstance = (Mod)Activator.CreateInstance(type)!;

                            modInstance.Setup();

                            if (!LoadedMods.TryAdd(modInstance.RegistryName, modInstance))
                                Logger.ThrowError("null", $"Mod with registry name '{modInstance.RegistryName}' already loaded.", true);

                            Logger.WriteConsole($"Loaded mod '{modInstance.DisplayName}' by {modInstance.Author} ({modInstance.Version})", LogLevel.Information);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.ThrowError("null", $"Failed to load assembly or instantiate mods from {file}: {exception.Message}");
                }
            }
        }

        public static void PreInitialize()
        {
            foreach (Mod mod in LoadedMods.Values)
                mod.PreInitialize();
        }

        public static void Initialize()
        {
            foreach (Mod mod in LoadedMods.Values)
                mod.Initialize();
        }

        public static void Update()
        {
            foreach (Mod mod in LoadedMods.Values)
                mod.Update();
        }

        public static void CleanUp()
        {
            foreach (Mod mod in LoadedMods.Values)
                mod.CleanUp();

            LoadedMods.Clear();
        }
    }
}