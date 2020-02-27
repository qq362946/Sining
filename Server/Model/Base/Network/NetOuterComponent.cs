using Sining.Event;
using Sining.Network;

namespace Sining.Module
{
    [ComponentSystem]
    public class NetOuterComponentServiceAwakeSystem : AwakeSystem<NetOuterComponent, string>
    {
        protected override void Awake(NetOuterComponent self, string address)
        {
            self.MessagePacker = self.AddComponent<ProtobufMessagePacker>();
            self.Awake(self.NetworkProtocolType, address);
        }
    }
    
    [ComponentSystem]
    public class NetOuterComponentConnectAwakeSystem : AwakeSystem<NetOuterComponent>
    {
        protected override void Awake(NetOuterComponent self)
        {
            self.MessagePacker = self.AddComponent<ProtobufMessagePacker>();
            self.Awake(self.NetworkProtocolType);
        }
    }

    public class NetOuterComponent : NetworkComponent
    {
        public NetworkProtocolType NetworkProtocolType { get; } = NetworkProtocolType.HTTP;
    }
}