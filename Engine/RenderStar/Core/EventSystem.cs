

namespace RenderStar.Core
{
    public abstract class Event
    {
        public bool IsCancelled { get; private set; }
        public int Priority { get; private set; }

        protected Event(int priority = 0)
        {
            Priority = priority;
        }

        public void Cancel()
        {
            IsCancelled = true;
        }
    }

    public static class EventManager
    {
        private class EventHandlerWrapper
        {
            public Delegate Handler { get; set; } = null!;
            public int Priority { get; set; }
        }

        private static Dictionary<Type, List<EventHandlerWrapper>> RegisteredHandlers { get; } = [];

        public delegate void EventHandler<TEvent>(TEvent @event) where TEvent : Event;

        public static void Register<TEvent>(EventHandler<TEvent> handler, int priority = 0) where TEvent : Event
        {
            Type eventType = typeof(TEvent);

            if (!RegisteredHandlers.ContainsKey(eventType))
                RegisteredHandlers[eventType] = [];

            RegisteredHandlers[eventType].Add(new() { Handler = handler, Priority = priority });
            
            RegisteredHandlers[eventType] = RegisteredHandlers[eventType].OrderByDescending(wrapper => wrapper.Priority).ToList();
        }

        public static void Unregister<TEvent>(EventHandler<TEvent> handler) where TEvent : Event
        {
            Type eventType = typeof(TEvent);

            if (RegisteredHandlers.TryGetValue(eventType, out List<EventHandlerWrapper>? wrappers))
            {
                EventHandlerWrapper? wrapperToRemove = wrappers.FirstOrDefault(wrapper => wrapper.Handler.Equals(handler));

                if (wrapperToRemove != null)
                    wrappers.Remove(wrapperToRemove);
            }
        }

        public static void Broadcast<TEvent>(TEvent eventToDispatch) where TEvent : Event
        {
            Type eventType = typeof(TEvent);

            if (RegisteredHandlers.TryGetValue(eventType, out List<EventHandlerWrapper>? wrappers))
            {
                foreach (EventHandlerWrapper wrapper in wrappers)
                {
                    ((EventHandler<TEvent>)wrapper.Handler)(eventToDispatch);

                    if (eventToDispatch.IsCancelled)
                        break;
                }
            }
        }
    }
}
