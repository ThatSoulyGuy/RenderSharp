using RenderStar.ECS;
using RenderStar.Render;

namespace RenderStar
{
    public static class Engine
    {
        public static GameObject Square { get; } = GameObjectManager.Create("Square");

        public static void PreInitialize(Form form)
        {
            Renderer.Initialize(form);
            
            ShaderManager.Instance.Register(Shader.Create("default", "Shader/Default"));
            TextureManager.Instance.Register(Texture.Create("brick", "Texture/Brick.png"));
        }

        public static void Initialize()
        {
            Square.AddComponent(ShaderManager.Instance.Get("default"));
            Square.AddComponent(TextureManager.Instance.Get("brick"));
            Square.AddComponent(Mesh.Create([], []));
            Square.GetComponent<Mesh>().GenerateSquare();
            Square.GetComponent<Mesh>().Generate(TextureFilter.MinMagMipPoint);
        }

        public static void Update()
        {
            GameObjectManager.Update();
        }

        public static void Render()
        {
            Renderer.PreRender();

            GameObjectManager.Render();

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