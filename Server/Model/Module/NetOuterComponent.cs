using System;
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
    public class NetOuterComponentNetworkProtocolAwakeSystem : AwakeSystem<NetOuterComponent, string, string>
    {
        protected override void Awake(NetOuterComponent self, string address, string networkProtocol)
        {
            self.MessagePacker = self.AddComponent<ProtobufMessagePacker>();
            self.SetNetworkProtocol(networkProtocol);
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
        public NetworkProtocolType NetworkProtocolType { get; private set; } = NetworkProtocolType.HTTP;

        public void SetNetworkProtocol(string networkProtocol)
        {
            switch (networkProtocol)
            {
                case "TCP":
                    NetworkProtocolType = NetworkProtocolType.TCP;
                    break;
                case "WebSocket":
                    NetworkProtocolType = NetworkProtocolType.WebSocket;
                    break;
                case "HTTP":
                    NetworkProtocolType = NetworkProtocolType.HTTP;
                    break;
                default:
                    throw new Exception($"No ServerType found for {networkProtocol}");
            }
        }
    }
}