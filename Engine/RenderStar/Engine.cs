using RenderStar.Core;
using RenderStar.ECS;
using RenderStar.Mod;
using RenderStar.Render;

namespace RenderStar
{
    public static class Engine
    {
        public static void PreInitialize(Form form)
        {
            Renderer.Initialize(form);
            
            ShaderManager.Instance.Register(Shader.Create("default", "Shader/Default"));
            TextureManager.Instance.Register(Texture.Create("brick", "Texture/Brick.png"));

            InputManager.Initialize(form);

            ModManager.LoadFrom("CoreMods/");
            ModManager.LoadFrom("Mods/");

            ModManager.PreInitialize();
        }

        public static void Initialize()
        {
            ModManager.Initialize();
        }

        public static void Update()
        {
            ModManager.Update();

            GameObjectManager.Update();
            InputManager.Update();
        }

        public static void Render()
        {
            Renderer.PreRender();

            GameObjectManager.Render(Renderer.RenderCamera);

            Renderer.PostRender();
        }

        public static void Resize(Form form)
        {
            Renderer.Resize(form);
        }

        public static void CleanUp()
        {
            ShaderManager.Instance.CleanUp();
            TextureManager.Instance.CleanUp();

            Renderer.CleanUp();
        }
    }
}