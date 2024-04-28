using RenderStar.Core;

namespace RenderStar.ECS
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequireComponentAttribute : Attribute
    {
        public Type RequiredComponent { get; private set; }

        public RequireComponentAttribute(Type requiredComponent)
        {
            if (!typeof(Component).IsAssignableFrom(requiredComponent))
                throw new ArgumentException("Required component must be a subclass of Component", nameof(requiredComponent));
            
            RequiredComponent = requiredComponent;
        }
    }
}