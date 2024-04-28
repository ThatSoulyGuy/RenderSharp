using RenderStar.ECS;

namespace RenderStar.Render
{
    public class TextureManager : Manager<Texture>
    {
        public static TextureManager Instance
        {
            get
            {
                _instance ??= new();
                return _instance;
            }
        }

        private static TextureManager _instance = null!;
    }
}
