using System;
using System.Collections.Generic;
using System.Net;
using Sining.Event;
using Sining.Network;
using Sining.Tools;

namespace Sining.Module
{
    public enum NetworkProtocolType
    {
        TCP = 1,
        KCP = 2,
        WebSocket = 3,
        HTTP = 4
    }

    [ComponentSystem]
    public class NetworkComponentAwakeSystem : AwakeSystem<NetworkComponent, NetworkProtocolType, string>
    {
        protected override void Awake(NetworkComponent self, NetworkProtocolType networkProtocolType, string address)
        {
            self.Awake(networkProtocolType, address);
        }
    }

    public class NetworkComponent : Component
    {
        public MessagePacker MessagePacker;
        
        public readonly Dictionary<long, Session> Sessions = new Dictionary<long, Session>();
        
        public int Count => Sessions.Count;

        public void Awake(NetworkProtocolType networkProtocolType, string address)
        {
            switch (networkProtocolType)
            {
                case NetworkProtocolType.TCP:
                    AddComponent<TCPComponent, EndPoint>(NetworkHelper.ToIPEndPoint(address));
                    break;
                case NetworkProtocolType.KCP:
                    break;
                case NetworkProtocolType.WebSocket:
                    break;
                case NetworkProtocolType.HTTP:
                    break;
            }
        }

        public Session Create(NetworkChannel channel)
        {
            this.Lock();

            var session = ComponentFactory.Create<Session, NetworkComponent>(this, channel);
            Sessions.Add(session.InstanceId, session);

            this.UnLock();

            return session;
        }

        public Session Get(long instanceId)
        {
            this.Lock();
            
            Sessions.TryGetValue(instanceId, out var session);
            
            this.UnLock();

            return session;
        }

        public void Remove(long instanceId)
        {
            this.Lock();

            if (!Sessions.TryGetValue(instanceId, out var session)) return;

            Sessions.Remove(instanceId);

            session.Dispose();

            Console.WriteLine($"Sessions删除了一个{Sessions.Count}");

            this.UnLock();
        }

        public override void Dispose()
        {
            if (IsDispose) return;
            
            base.Dispose();

            MessagePacker = null;
        }
    }
}