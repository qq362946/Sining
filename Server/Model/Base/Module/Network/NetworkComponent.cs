using System;
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
        public IMessageDispatcher MessageDispatcher;
        public int Count => Children.Count;

        public void Awake(NetworkProtocolType networkProtocolType, string address)
        {
            if (networkProtocolType != NetworkProtocolType.TCP)
                throw new Exception("Find an unsupported network protocol");

            _networkProtocol = AddComponent<TCPComponent, EndPoint>(NetworkHelper.ToIPEndPoint(address));
        }

        public void Awake(NetworkProtocolType networkProtocolType, IEnumerable<string> urls)
        {
            _networkProtocol = networkProtocolType switch
            {
                NetworkProtocolType.WebSocket => AddComponent<WebSocketComponent, IEnumerable<string>>(
                    urls),
                NetworkProtocolType.HTTP => AddComponent<HttpComponent, IEnumerable<string>>(urls),
                _ => _networkProtocol
            };
        }

        public void Awake(NetworkProtocolType networkProtocolType)
        {
            _networkProtocol = networkProtocolType switch
            {
                NetworkProtocolType.TCP => AddComponent<TCPComponent>(),
                NetworkProtocolType.WebSocket => AddComponent<WebSocketComponent>(),
                NetworkProtocolType.HTTP => AddComponent<HttpComponent>(),
                _ => _networkProtocol
            };
        }

        public Session Create(string address)
        {
            var session = Create();

            session.Channel = _networkProtocol.ConnectChannel(session, address);

            return session;
        }

        public Session Create()
        {
            return ComponentFactory.Create<Session, NetworkComponent>(Scene, this, this, true);
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