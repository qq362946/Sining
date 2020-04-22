using System;
using System.Collections.Generic;
using Sining.Event;
using Sining.Network;

namespace Sining.Module
{
    [ComponentSystem]
    public class
        NetOuterComponentNetworkProtocolAwakeSystem : AwakeSystem<NetOuterComponent, MessagePacker, NetworkProtocolType>
    {
        protected override void Awake(NetOuterComponent self, MessagePacker messagePacker,
            NetworkProtocolType networkProtocol)
        {
            self.MessagePacker = (MessagePacker) self.AddComponent(messagePacker);
            self.SetNetworkProtocol(Enum.GetName(typeof(NetworkProtocolType), networkProtocol));
            self.Awake(self.NetworkProtocolType);
        }
    }

    [ComponentSystem]
    public class
        NetOuterComponentNetworkProtocolStringAwakeSystem : AwakeSystem<NetOuterComponent, MessagePacker, string, string
        >
    {
        protected override void Awake(NetOuterComponent self, MessagePacker messagePacker, string address,
            string networkProtocol)
        {
            self.MessagePacker = (MessagePacker) self.AddComponent(messagePacker);
            self.SetNetworkProtocol(networkProtocol);
            self.Awake(self.NetworkProtocolType, address);
        }
    }

    [ComponentSystem]
    public class
        NetOuterComponentNetworkUrlsProtocolAwakeSystem : AwakeSystem<NetOuterComponent, MessagePacker,
            IEnumerable<string>, string>
    {
        protected override void Awake(NetOuterComponent self, MessagePacker messagePacker, IEnumerable<string> urls,
            string networkProtocol)
        {
            self.MessagePacker = (MessagePacker) self.AddComponent(messagePacker);
            self.SetNetworkProtocol(networkProtocol);
            self.Awake(self.NetworkProtocolType, urls);
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