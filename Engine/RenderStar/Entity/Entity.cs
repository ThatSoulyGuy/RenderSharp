using RenderStar.ECS;
using SharpDX;

namespace RenderStar.Entity
{
    public struct EntityRegistration
    {
        public string RegistryName { get; private set; } = string.Empty;
        public int MaxHealth { get; private set; } = 0;
        public float MovementSpeed { get; private set; } = 0.0f;

        private EntityRegistration(byte _) { }

        public static EntityRegistration Create(string registryName, int maxHealth, float movementSpeed)
        {
            return new(0)
            {
                RegistryName = registryName,
                MaxHealth = maxHealth,
                MovementSpeed = movementSpeed
            };
        }
    }

    public abstract class Entity : Component
    {
        public string CustomName { get; set; } = string.Empty;
        public string RegistryName { get; private set; } = string.Empty;

        public int MaxHealth { get; private set; } = 0;
        public int Health { get; set; } = 0;

        public float MovementSpeed { get; private set; } = 0.0f;

        public bool IsDead => Health <= 0;

        public abstract EntityRegistration Registration { get; }

        public override void Initialize()
        {
            RegistryName = Registration.RegistryName;
            MaxHealth = Registration.MaxHealth;
            Health = MaxHealth;
            MovementSpeed = Registration.MovementSpeed;
        }

        public static T Create<T>() where T : Entity, new()
        {
            return new();
        }
    }
}