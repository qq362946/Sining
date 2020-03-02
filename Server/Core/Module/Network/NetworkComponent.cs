using System.Collections.Generic;
using System.Net;
using Sining.Network;
using Sining.Tools;

namespace Sining.Module
{
    public enum NetworkProtocolType
    {
        TCP = 1,
        WebSocket = 2,
        HTTP = 3
    }

    public class NetworkComponent : Component
    {
        public MessagePacker MessagePacker;
        private NetworkProtocol _networkProtocol;
        public int Count => Children.Count;

        public void Awake(NetworkProtocolType networkProtocolType, string address)
        {
            switch (networkProtocolType)
            {
                case NetworkProtocolType.TCP:
                    _networkProtocol = AddComponent<TCPComponent, EndPoint>(NetworkHelper.ToIPEndPoint(address));
                    break;
                case NetworkProtocolType.WebSocket:
                    _networkProtocol = AddComponent<WebSocketComponent, List<string>>(new List<string>() {address});
                    break;
                case NetworkProtocolType.HTTP:
                    _networkProtocol = AddComponent<HttpComponent, List<string>>(new List<string>() {address});
                    break;
            }
        }
        
        public void Awake(NetworkProtocolType networkProtocolType)
        {
            switch (networkProtocolType)
            {
                case NetworkProtocolType.TCP:
                    _networkProtocol = AddComponent<TCPComponent>();
                    break;
                case NetworkProtocolType.WebSocket:
                    _networkProtocol = AddComponent<WebSocketComponent>();
                    break;
                case NetworkProtocolType.HTTP:
                    break;
            }
        }

        public Session Create(string address)
        {
            var session = Create();

            session.Channel = _networkProtocol.ConnectChannel(session, address);

            return session;
        }

        public Session Create()
        {
            return ComponentFactory.Create<Session, NetworkComponent>(this, this, true);
        }

        public Session Get(long instanceId)
        {
            return GetChild<Session>(instanceId);
        }

        public virtual void Remove(long instanceId)
        {
            RemoveChild(instanceId);
        }

        public override void Dispose()
        {
            if (IsDispose) return;
            
            base.Dispose();

            MessagePacker = null;
            _networkProtocol = null;
        }
    }
}