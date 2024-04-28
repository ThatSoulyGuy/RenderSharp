using RenderStar.ECS;

namespace RenderStar.Render
{
    public class ShaderManager : Manager<Shader>
    {
        public static ShaderManager Instance
        {
            get
            {
                _instance ??= new();
                return _instance;
            }
        }
        
        private static ShaderManager _instance = null!;
    }
}