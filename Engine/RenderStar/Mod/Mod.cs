

namespace RenderStar.Mod
{
    public struct ModRegistration
    {
        public string DisplayName { get; private set; }
        public string RegistryName { get; private set; }
        public string Description { get; private set; }
        public string Author { get; private set; }
        public string Version { get; private set; }

        public static ModRegistration Create(string displayName, string registryName, string description = "", string author = "Unknown", string version = "Unknown")
        {
            return new()
            {
                DisplayName = displayName,
                RegistryName = registryName,
                Description = description,
                Author = author,
                Version = version
            };
        }
    }

    public abstract class Mod
    {
        public string DisplayName { get; private set; } = "";
        public string RegistryName { get; private set; } = "";
        public string Description { get; private set; } = "";
        public string Author { get; private set; } = "";
        public string Version { get; private set; } = "";

        public abstract ModRegistration Registration { get; }

        public abstract void PreInitialize();

        public abstract void Initialize();

        public abstract void Update();

        public abstract void CleanUp();

        public void Setup()
        {
            DisplayName = Registration.DisplayName;
            RegistryName = Registration.RegistryName;
            Description = Registration.Description;
            Author = Registration.Author;
            Version = Registration.Version;
        }
    }
}
