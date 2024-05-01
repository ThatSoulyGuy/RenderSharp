using SharpDX;

namespace RenderStar.Core
{
    public enum KeyState
    {
        Pressed,
        Released,
    }

    public static class InputManager
    {
        public static Vector2 MousePosition { get; private set; }
        public static bool MouseLeftPressed { get; private set; }
        public static bool MouseRightPressed { get; private set; }
        public static bool MouseLeftClick { get; private set; }
        public static bool MouseRightClick { get; private set; }
        public static int MouseWheelDelta { get; private set; }

        public static Vector2 MouseDelta { get; private set; }

        private static Vector2 lastMousePosition;
        private static bool lastMouseLeftState;
        private static bool lastMouseRightState;

        private static Form Form { get; set; } = null!;

        public static bool CursorMode { get; private set; } = true;

        private static HashSet<Keys> CurrentPressedKeys { get; } = [];
        private static HashSet<Keys> NewlyPressedKeys { get; } = [];
        private static HashSet<Keys> NewlyReleasedKeys { get; } = [];

        private static bool cursorIsHidden = false;

        public static void Initialize(Form form)
        {
            Form = form;

            form.KeyDown += (sender, arguments) =>
            {
                if (!CurrentPressedKeys.Contains(arguments.KeyCode))
                    NewlyPressedKeys.Add(arguments.KeyCode);

                CurrentPressedKeys.Add(arguments.KeyCode);
            };

            form.KeyUp += (sender, arguments) =>
            {
                CurrentPressedKeys.Remove(arguments.KeyCode);
                NewlyReleasedKeys.Add(arguments.KeyCode);
            };

            form.MouseMove += (sender, arguments) =>
            {
                Vector2 newMousePosition = new(arguments.X, arguments.Y);

                MouseDelta = newMousePosition - lastMousePosition;
                lastMousePosition = newMousePosition;

                if (!CursorMode)
                {
                    Cursor.Position = Form.PointToScreen(new System.Drawing.Point(Form.ClientSize.Width / 2, Form.ClientSize.Height / 2));
                    lastMousePosition = new Vector2(Form.ClientSize.Width / 2, Form.ClientSize.Height / 2);
                }
                else
                    MouseDelta = Vector2.Zero;
            };

            form.MouseDown += (sender, arguments) =>
            {
                if (arguments.Button == MouseButtons.Left)
                    MouseLeftPressed = true;
                else if (arguments.Button == MouseButtons.Right)
                    MouseRightPressed = true;
            };

            form.MouseUp += (sender, arguments) =>
            {
                if (arguments.Button == MouseButtons.Left)
                {
                    MouseLeftPressed = false;
                    MouseLeftClick = !lastMouseLeftState;
                }
                else if (arguments.Button == MouseButtons.Right)
                {
                    MouseRightPressed = false;
                    MouseRightClick = !lastMouseRightState;
                }
            };

            form.MouseWheel += (sender, arguments) =>
            {
                MouseWheelDelta += arguments.Delta;
            };
        }

        public static void SetCursorMode(bool hidden)
        {
            if (hidden && !cursorIsHidden)
            {
                Cursor.Hide();

                cursorIsHidden = true;
                Cursor.Position = Form.PointToScreen(new System.Drawing.Point(Form.ClientSize.Width / 2, Form.ClientSize.Height / 2));

                lastMousePosition = new Vector2(Form.ClientSize.Width / 2, Form.ClientSize.Height / 2);
            }
            else if (!hidden && cursorIsHidden)
            {
                Cursor.Show();
                cursorIsHidden = false;
            }

            CursorMode = !hidden;
        }

        public static bool GetKeyHeld(Keys key, KeyState state)
        {
            if (state == KeyState.Pressed)
                return CurrentPressedKeys.Contains(key);
            else
                return !CurrentPressedKeys.Contains(key);
        }

        public static bool GetKey(Keys key, KeyState state)
        {
            if (state == KeyState.Pressed)
                return NewlyPressedKeys.Contains(key);
            else
                return NewlyReleasedKeys.Contains(key);
        }

        public static bool IsCursorWithinBounds(Vector3 min, Vector3 max)
        {
            System.Drawing.Point cursorPosition = Cursor.Position;

            return (cursorPosition.X >= min.X && cursorPosition.X <= max.X &&
                    cursorPosition.Y >= min.Y && cursorPosition.Y <= max.Y);
        }

        public static void Update()
        {
            NewlyPressedKeys.Clear();
            NewlyReleasedKeys.Clear();
            MouseWheelDelta = 0;
            lastMouseLeftState = MouseLeftPressed;
            lastMouseRightState = MouseRightPressed;
            MouseLeftClick = false;
            MouseRightClick = false;
        }
    }
}
