using RenderStar.Math;
using SharpDX;

namespace RenderStar.ECS
{
    public class GameObject
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        private Dictionary<Type, Component> Components { get; } = [];

        public void AddComponent(Component component)
        {
            Type type = component.GetType();

            foreach (RequireComponentAttribute requiredComponent in requiredComponents)
            {
                if (!HasComponent(requiredComponent.RequiredComponent))
                    AddComponent((Activator.CreateInstance(requiredComponent.RequiredComponent) as Component)!);
            }

            Components[type] = component;

            Components[type].GameObject = this;
            Components[type].Initialize();
        }

        public T GetComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (Components.TryGetValue(type, out Component? component))
                return (component as T)!;
            
            return null!;
        }

        public bool RemoveComponent<T>() where T : Component
        {
            return Components.Remove(typeof(T));
        }

        public void SetComponent<T>(T component) where T : Component
        {
            Type type = typeof(T);

            Components[type] = component;
        }
        }

        public bool HasComponent<T>() where T : Component
        {
            return Components.ContainsKey(typeof(T));
        }

        public void Update()
        {
            if (!IsActive)
                return;

            foreach (Component component in Components.Values)
                component.Update();
        }

        public void Render()
        {
            if (!IsActive)
                return;

            foreach (Component component in Components.Values)
                component.Render();
        }

        public void CleanUp()
        {
            foreach (Component component in Components.Values)
                component.CleanUp();
            
            Components.Clear();
        }
    }

    public abstract class Component
    {
        public Transform Transform => GameObject.GetComponent<Transform>();
        public GameObject GameObject { get; set; } = null!;

        public virtual void Initialize() { }

        public virtual void Update() { }

        public virtual void Render() { }

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

        public static void Render()
        {
            foreach (GameObject gameObject in RegisteredGameObjects.Values)
                gameObject.Render();
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
