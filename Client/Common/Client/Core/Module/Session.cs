using Sining.Network;
using Sining.Tools;

namespace Sining.Module
{
    public class Session : Component
    {
        private NetworkComponent _networkComponent;
        public long LastRecvTime { get; private set; }
        public long LastSendTime { get; private set; }

        public void Awake(NetworkComponent networkComponent)
        {
            _networkComponent = networkComponent;
        }

        public void Receive(object obj)
        {
            LastRecvTime = TimeHelper.Now;
            
            MessageDispatcher.Handle(this, obj);
        }

        public override void Dispose()
        {
            if (IsDispose) return;

            _networkComponent.Remove(InstanceId);

            base.Dispose();

            LastRecvTime = 0;
        }
    }
}