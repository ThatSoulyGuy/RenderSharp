using RenderStar.Math;
using RenderStar.Render;
using SharpDX;

namespace RenderStar.ECS
{
    public class GameObject
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public GameObject Parent { get; private set; } = null!;
        public List<GameObject> Children { get; } = [];

        public Transform Transform => GetComponent<Transform>();

        private Dictionary<Type, Component> Components { get; } = [];

        public Component AddComponent(Component component)
        {
            Type type = component.GetType();
            List<RequireComponentAttribute> requiredComponents = type.GetCustomAttributes(typeof(RequireComponentAttribute), true).Cast<RequireComponentAttribute>().ToList();

            foreach (RequireComponentAttribute requiredComponent in requiredComponents)
            {
                if (!HasComponent(requiredComponent.RequiredComponent))
                    AddComponent((Activator.CreateInstance(requiredComponent.RequiredComponent) as Component)!);
            }

            Components[type] = component;
            component.GameObject = this;
            component.Initialize();

            return component;
        }

        public bool HasComponent(Type type)
        {
            return Components.ContainsKey(type);
        }

        public bool HasComponent<T>() where T : Component
        {
            return Components.ContainsKey(typeof(T));
        }

        public T GetComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (Components.TryGetValue(type, out Component? component))
                return (component as T)!;

            return null!;
        }

        public T GetComponentInChildren<T>() where T : Component
        {
            foreach (GameObject child in Children)
            {
                if (child.HasComponent<T>())
                    return child.GetComponent<T>();
            }

            return null!;
        }

        public bool RemoveComponent<T>() where T : Component
        {
            return Components.Remove(typeof(T));
        }

        public void AddChild(GameObject child)
        {
            if (!Children.Contains(child))
            {
                Children.Add(child);
                child.Parent = this;
            }
        }

        public void RemoveChild(GameObject child)
        {
            if (Children.Remove(child))
                child.Parent = null!;
        }

        public void Update()
        {
            if (!IsActive)
                return;

            foreach (Component component in Components.Values)
                component.Update();

            foreach (GameObject child in Children)
                child.Update();
        }

        public void Render(Camera camera)
        {
            if (!IsActive)
                return;

            foreach (Component component in Components.Values)
                component.Render(camera);

            foreach (GameObject child in Children)
                child.Render(camera);
        }

        public void CleanUp()
        {
            foreach (Component component in Components.Values)
                component.CleanUp();

            Components.Clear();

            foreach (GameObject child in Children)
                child.CleanUp();

            Children.Clear();
        }
    }

    public abstract class Component
    {
        public Transform Transform => GameObject.GetComponent<Transform>();
        public GameObject GameObject { get; set; } = null!;

        public virtual void Initialize() { }

        public virtual void Update() { }

        public virtual void Render(Camera camera) { }

        public virtual void CleanUp() { }
    }

    public abstract class Manager<T> where T : IManageable
    {
        public Dictionary<string, T> RegisteredObjects { get; } = [];

        public virtual void Register(T value)
        {
            RegisteredObjects[value.Name] = value;
        }

        public virtual void Unregister(string name)
        {
            RegisteredObjects.Remove(name);
        }

        public virtual T Get(string name)
        {
            if(RegisteredObjects.TryGetValue(name, out T? value))
                return value;

            return default!;
        }

        public virtual void CleanUp()
        {
            foreach (T value in RegisteredObjects.Values)
                value.CleanUp();

            RegisteredObjects.Clear();
        }
    }

    public static class GameObjectManager
    {
        private static Dictionary<string, GameObject> RegisteredGameObjects { get; } = [];

        public static GameObject Create(string name)
        {
            GameObject gameObject = new()
            {
                Name = name
            };

            gameObject.AddComponent(Transform.Create(Vector3.Zero, Vector3.Zero, Vector3.One));

            RegisteredGameObjects.Add(name, gameObject);

            return gameObject;
        }
        
        public static GameObject Get(string name)
        {
            if(RegisteredGameObjects.TryGetValue(name, out GameObject? value))
                return value;

            return null!;
        }

        public static void Update()
        {
            foreach (GameObject gameObject in RegisteredGameObjects.Values)
                gameObject.Update();
        }

        public static void Render(Camera camera)
        {
            foreach (GameObject gameObject in RegisteredGameObjects.Values)
                gameObject.Render(camera);
        }

        public static void Remove(GameObject gameObject)
        {
            if (RegisteredGameObjects.TryGetValue(gameObject.Name, out var value))
            {
                value.CleanUp();
                RegisteredGameObjects.Remove(value.Name);
            }
        }
    }
}
