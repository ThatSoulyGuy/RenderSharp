using RenderStar.Core;
using RenderStar.ECS;
using RenderStar.Entity;
using RenderStar.Math;
using RenderStar.Render;
using SharpDX;

namespace RenderStar
{
    public class EntityPlayer : Entity.Entity
    {
        private Queue<Vector2> MouseDeltaBuffer { get; } = [];
        private const int BufferLength = 5;

        public override EntityRegistration Registration => EntityRegistration.Create("entity_player", 100, 5.0f);

        public override void Initialize()
        {
            GameObject.AddChild(GameObjectManager.Create("Camera"));
            GameObject.Children[0].AddComponent(Camera.Create());

            InputManager.SetCursorMode(true);

            Renderer.RenderCamera = GameObject.Children[0].GetComponent<Camera>();
        }

        public override void Update()
        {
            UpdateMouselook();
            UpdateMovement();
        }

        private void UpdateMouselook()
        {
            MouseDeltaBuffer.Enqueue(InputManager.MouseDelta);

            while (MouseDeltaBuffer.Count > BufferLength)
                MouseDeltaBuffer.Dequeue();

            Vector2 averageDelta = new(MouseDeltaBuffer.Average(position => position.X), MouseDeltaBuffer.Average(positon => positon.Y));

            float sensitivity = 80.0f;
            float yawChange = averageDelta.X * sensitivity * MathF.PI / 180;
            float pitchChange = averageDelta.Y * sensitivity * MathF.PI / 180;

            Vector3 currentRotation = GameObject.Children[0].Transform.LocalRotation;

            currentRotation.Y += yawChange;
            currentRotation.X += pitchChange;

            float pitchLimit = 89.9f;
            currentRotation.X = System.Math.Clamp(currentRotation.X, -pitchLimit, pitchLimit);

            GameObject.Children[0].Transform.LocalRotation = currentRotation;
        }

        private void UpdateMovement()
        {
            Vector3 movementVector = Vector3.Zero;

            if (InputManager.GetKeyHeld(Keys.W, KeyState.Pressed))
                movementVector += Transform.Forward * MovementSpeed;

            if (InputManager.GetKeyHeld(Keys.S, KeyState.Pressed))
                movementVector -= Transform.Forward * MovementSpeed;

            if (InputManager.GetKeyHeld(Keys.A, KeyState.Pressed))
                movementVector -= Transform.Right * MovementSpeed;

            if (InputManager.GetKeyHeld(Keys.D, KeyState.Pressed))
                movementVector += Transform.Right * MovementSpeed;

            Transform.LocalPosition += movementVector;
        }
    }

    public static class Engine
    {
        public static GameObject Square { get; } = GameObjectManager.Create("Square");
        public static GameObject Player { get; } = GameObjectManager.Create("Player");

        public static void PreInitialize(Form form)
        {
            Renderer.Initialize(form);
            
            ShaderManager.Instance.Register(Shader.Create("default", "Shader/Default"));
            TextureManager.Instance.Register(Texture.Create("brick", "Texture/Brick.png"));

            InputManager.Initialize(form);
        }

        public static void Initialize()
        {
            Square.AddComponent(ShaderManager.Instance.Get("default"));
            Square.AddComponent(TextureManager.Instance.Get("brick"));
            Square.AddComponent(Mesh.Create([], []));
            Square.GetComponent<Mesh>().GenerateSquare();
            Square.GetComponent<Mesh>().Generate(TextureFilter.MinMagMipPoint);

            Player.AddComponent(Entity.Entity.Create<EntityPlayer>());
            Player.Transform.LocalPosition = new(0.0f, 0.0f, -5.0f);

            Renderer.RenderCamera = Player.Children[0].GetComponent<Camera>();
        }

        public static void Update()
        {
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