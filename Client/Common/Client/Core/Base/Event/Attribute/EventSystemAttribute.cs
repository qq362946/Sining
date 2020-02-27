using System;

namespace Sining.Event
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventSystemAttribute : BaseAttribute
    {
        public string EventType { get; }

        public EventSystemAttribute(string eventType)
        {
            EventType = eventType;
        }
    }
}