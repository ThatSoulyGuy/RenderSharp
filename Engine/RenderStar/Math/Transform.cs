using RenderStar.ECS;
using RenderStar.Util;
using SharpDX;

namespace RenderStar.Math
{
    public class Transform : Component
    {
        private Vector3 localPosition;
        private Vector3 localRotation;
        private Vector3 localScale = Vector3.One;
        private Matrix worldMatrix;
        private bool isDirty = true;

        public Vector3 LocalPosition
        {
            get => localPosition;
            set { localPosition = value; MarkDirty(); }
        }

        public Vector3 LocalRotation
        {
            get => localRotation;
            set { localRotation = value; MarkDirty(); }
        }

        public Vector3 LocalScale
        {
            get => localScale;
            set { localScale = value; MarkDirty(); }
        }

        public Vector3 WorldPosition
        {
            get
            {
                UpdateWorldMatrixIfNeeded();
                return worldMatrix.TranslationVector;
            }
        }

        public Vector3 Forward
        {
            get => Vector3.TransformNormal(new(0, 0, 1), WorldMatrix);
        }

        public Vector3 Right
        {
            get => Vector3.TransformNormal(new(1, 0, 0), WorldMatrix);
        }

        public Vector3 Up
        {
            get => Vector3.TransformNormal(new(0, 1, 0), WorldMatrix);
        }

        public Matrix WorldMatrix
        {
            get
            {
                UpdateWorldMatrixIfNeeded();
                return worldMatrix;
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
            WrapAngles();
        }

        public void ScaleBy(Vector3 scale)
        {
            LocalScale += scale;
        }

        private void MarkDirty()
        {
            if (!isDirty)
            {
                isDirty = true;
                foreach (var child in GameObject.Children)
                {
                    child.Transform.MarkDirty();
                }
            }
        }

        private void UpdateWorldMatrixIfNeeded()
        {
            if (isDirty)
            {
                RecalculateWorldMatrix();
                isDirty = false;
            }
        }

        private void RecalculateWorldMatrix()
        {
            Matrix rotationMatrix = Matrix.RotationYawPitchRoll(
                MathHelper.DegreesToRadians(LocalRotation.Y),
                MathHelper.DegreesToRadians(LocalRotation.X),
                MathHelper.DegreesToRadians(LocalRotation.Z));

            Matrix scaleMatrix = Matrix.Scaling(LocalScale);
            Matrix translationMatrix = Matrix.Translation(LocalPosition);

            worldMatrix = scaleMatrix * rotationMatrix * translationMatrix;

            if (ParentTransform != null)
                worldMatrix *= ParentTransform.WorldMatrix;
        }

        private void WrapAngles()
        {
            LocalRotation = new Vector3(WrapAngle(LocalRotation.X), WrapAngle(LocalRotation.Y), WrapAngle(LocalRotation.Z));
        }

        private static float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle < 0)
                angle += 360;
            return angle;
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