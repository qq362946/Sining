namespace Sining.Event
{
    public class EventType
    {
        public const string OnLogin = "OnLogin";
    }

    [EventSystem(EventType.OnLogin)]
    public class OnLogin : IEvent
    {
        public void Run()
        {
            throw new System.NotImplementedException();
        }
    }
}