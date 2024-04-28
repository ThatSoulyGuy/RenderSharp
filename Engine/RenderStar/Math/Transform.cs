using RenderStar.ECS;
using RenderStar.Util;
using SharpDX;

namespace RenderStar.Math
{
    public class Transform : Component
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public Vector3 Forward => new((float)(MathF.Sin(MathHelper.DegreesToRadians(Rotation.Y)) * MathF.Cos(MathHelper.DegreesToRadians(Rotation.X))), (float)MathF.Sin(MathHelper.DegreesToRadians(Rotation.X)), (float)(MathF.Cos(MathHelper.DegreesToRadians(Rotation.Y)) * MathF.Cos(MathHelper.DegreesToRadians(Rotation.X))));

        public Vector3 Right => new((float)MathF.Cos(MathHelper.DegreesToRadians(Rotation.Y)), 0, (float)- MathF.Sin(MathHelper.DegreesToRadians(Rotation.Y)));

        public Vector3 Up => Vector3.Cross(Forward, Right);

        public static Vector3 WorldUp => new(0, 1, 0);

        public Matrix Matrix => Matrix.Scaling(Scale) * (Matrix.RotationX(MathHelper.DegreesToRadians(Rotation.X)) * Matrix.RotationY(MathHelper.DegreesToRadians(Rotation.Y)) * Matrix.RotationZ(MathHelper.DegreesToRadians(Rotation.Z))) * Matrix.Translation(Position);

        public Vector3 Apply(Vector3 position)
        {
            return Vector3.TransformCoordinate(position, Matrix);
        }

        public void Translate(Vector3 translation)
        {
            Position += translation;
        }

        public void Rotate(Vector3 rotation)
        {
            Rotation += rotation;
        }

        public void ScaleBy(Vector3 scale)
        {
            Scale += scale;
        }

        public static Transform Create(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            return new Transform
            {
                Position = position,
                Rotation = rotation,
                Scale = scale
            };
        }
    }
}
