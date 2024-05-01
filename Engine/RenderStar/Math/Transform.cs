using RenderStar.ECS;
using RenderStar.Util;
using SharpDX;

namespace RenderStar.Math
{
    public class Transform : Component
    {
        public Vector3 LocalPosition { get; set; }
        public Vector3 LocalRotation { get; set; }
        public Vector3 LocalScale { get; set; } = Vector3.One;

        public Vector3 Forward
        {
            get => Vector3.TransformNormal(new Vector3(0, 0, 1), WorldMatrix);
        }

        public Vector3 Right
        {
            get => Vector3.TransformNormal(new Vector3(1, 0, 0), WorldMatrix);
        }

        public Vector3 Up
        {
            get => Vector3.TransformNormal(new Vector3(0, 1, 0), WorldMatrix);
        }

        public Vector3 WorldPosition
        {
            get
            {
                return ParentTransform != null ? Vector3.TransformCoordinate(LocalPosition, ParentTransform.WorldMatrix) : LocalPosition;
            }
        }

        public Matrix WorldMatrix
        {
            get
            {
                Matrix rotationMatrix = Matrix.RotationX(MathHelper.DegreesToRadians(LocalRotation.X)) *
                                     Matrix.RotationY(MathHelper.DegreesToRadians(LocalRotation.Y)) *
                                     Matrix.RotationZ(MathHelper.DegreesToRadians(LocalRotation.Z));

                Matrix scaleMatrix = Matrix.Scaling(LocalScale);
                Matrix translationMatrix = Matrix.Translation(WorldPosition);

                return scaleMatrix * rotationMatrix * translationMatrix;
            }
        }

        private Transform ParentTransform => GameObject.Parent?.GetComponent<Transform>()!;

        public void Translate(Vector3 translation)
        {
            LocalPosition += translation;
        }

        public void Rotate(Vector3 rotation)
        {
            LocalRotation += rotation;
        }

        public void ScaleBy(Vector3 scale)
        {
            LocalScale += scale;
        }

        public static Transform Create(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            return new Transform
            {
                LocalPosition = position,
                LocalRotation = rotation,
                LocalScale = scale
            };
        }
    }
}
