using Sining.Module;

namespace Sining.Network
{
    public interface IMessageDispatcher
    {
        SVoid Dispatch(Session session, ushort code, object message);
    }
}