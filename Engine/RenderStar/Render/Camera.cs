using RenderStar.ECS;
using RenderStar.Math;
using RenderStar.Util;
using SharpDX;

namespace RenderStar.Render
{
    public class Camera : Component
    {
        public float FieldOfView { get; private set; }
        public float NearPlane { get; private set; }
        public float FarPlane { get; private set; }

        public Matrix ViewMatrix => Matrix.LookAtLH(Transform.WorldPosition, Transform.WorldPosition + Transform.Forward, Transform.Up);
        public Matrix ProjectionMatrix => Matrix.PerspectiveFovLH(MathHelper.DegreesToRadians(FieldOfView), Renderer.AspectRatio, NearPlane, FarPlane);

        private Camera() { }

        public static Camera Create(float fieldOfView = 45.0f, float nearPlane = 0.01f, float farPlane = 500.0f)
        {    
            Camera camera = new()
            {
                FieldOfView = fieldOfView,
                NearPlane = nearPlane,
                FarPlane = farPlane
            };

            return camera;
        }
    }
}
