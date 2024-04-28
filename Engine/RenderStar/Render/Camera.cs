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

        public Matrix ViewMatrix => Matrix.LookAtLH(Transform.LocalPosition, Transform.LocalPosition + Transform.Forward, Transform.WorldUp);
        public Matrix ProjectionMatrix => Matrix.PerspectiveFovLH(MathHelper.DegreesToRadians(FieldOfView), Renderer.AspectRatio, NearPlane, FarPlane);

        public static Camera Create(Vector3 position, Vector3 rotation, float fieldOfView = 45.0f, float nearPlane = 0.01f, float farPlane = 500.0f)
        {    
            Camera camera = new()
            {
                FieldOfView = fieldOfView,
                NearPlane = nearPlane,
                FarPlane = farPlane
            };

            camera.GameObject.GetComponent<Transform>().LocalPosition = position;
            camera.GameObject.GetComponent<Transform>().LocalRotation = rotation;

            return camera;
        }
    }
}
