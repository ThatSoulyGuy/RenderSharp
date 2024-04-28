

namespace RenderStar.Util
{
    public static class MathHelper
    {
        public static float DegreesToRadians(float angleInDegrees)
        {
            return (float)(MathF.PI * angleInDegrees / 180.0);
        }
    }
}
