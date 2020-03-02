using System.Linq;
using Sining.DataStructure;
using Sining.Event;
using Sining.Network;

namespace Sining.Module
{
    [ComponentSystem]
    public class NetInnerComponentServiceAwakeSystem : AwakeSystem<NetInnerComponent, string>
    {
        protected override void Awake(NetInnerComponent self, string address)
        {
            self.MessagePacker = self.AddComponent<ProtobufMessagePacker>();
            self.Awake(self.NetworkProtocolType, address);
        }
    }
    
    [ComponentSystem]
    public class NetInnerComponentConnectAwakeSystem : AwakeSystem<NetInnerComponent>
    {
        protected override void Awake(NetInnerComponent self)
        {
            self.MessagePacker = self.AddComponent<ProtobufMessagePacker>();
            self.Awake(self.NetworkProtocolType);
        }
    }
    
    public class NetInnerComponent : NetworkComponent
    {
        private readonly DoubleMapDictionary<string, Session> _sessions = new DoubleMapDictionary<string, Session>();
        public NetworkProtocolType NetworkProtocolType { get; } = NetworkProtocolType.TCP;

        public Session GetSession(string address)
        {
            var session = _sessions.GetValueByKey(address);

            if (session != null)
            {
                return session;
            }

            session = Create(address);

            _sessions.Add(address, session);

            return session;
        }

        public override void Remove(long instanceId)
        {
            var session = Get(instanceId);
            
            if (session == null)
            {
                return;
            }
            
            _sessions.RemoveByValue(session);
            
            base.Remove(instanceId);
        }

        public override void Dispose()
        {
            if (IsDispose) return;
            
            foreach (var session in _sessions.Values.ToList())
            {
                Remove(session.InstanceId);
            }
            
            _sessions.Clear();
            
            base.Dispose();
        }
    }
}