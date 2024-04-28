

namespace RenderStar.ECS
{
    public interface IManageable
    {
        string Name { get; }

        public virtual void CleanUp() { }
    }
}
