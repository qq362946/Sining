using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using Sining.Event;
using Sining.Module;

namespace Sining.Network
{
    [ComponentSystem]
    public class WebSocketComponentAwakeSystem : AwakeSystem<WebSocketComponent, List<string>>
    {
        protected override void Awake(WebSocketComponent self, List<string> urls)
        {
            self.Awake(urls);
        }
    }

    public class WebSocketComponent : NetworkProtocol
    {
        private readonly HttpListener _httpListener = new HttpListener();
        private Thread _taskThread;
        private NetworkComponent _networkComponent;

        public void Awake(IEnumerable<string> urls)
        {
            _networkComponent = GetParent<NetworkComponent>();
            
            _taskThread = new Thread(async () =>
            {
                try
                {
                    foreach (var prefix in urls)
                    {
                        _httpListener.Prefixes.Add(prefix);
                    }
                
                    _httpListener.Start();

                    for (;;)
                    {
                        var httpListenerContext = await _httpListener.GetContextAsync();
                       
                        var webSocketContext = await httpListenerContext.AcceptWebSocketAsync(null);

                        AddChannel(webSocketContext);
                    }
                }
                catch (HttpListenerException e)
                {
                    if (e.ErrorCode == 5)
                    {
                        throw new Exception($"CMD管理员中输入: netsh http add urlacl url=http://*:8080/ user=Everyone", e);
                    }

                    Log.Error(e);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            })
            {
                IsBackground = true
            };
            
            _taskThread.Start();
        }

        private void AddChannel(HttpListenerWebSocketContext webSocketContext)
        {
            var session = _networkComponent.Create();

            session.Channel = ComponentFactory.Create<
                WebSocketChannelComponent,
                Session,
                HttpListenerWebSocketContext>(session,
                webSocketContext, this, true);
            
        }

        public override NetworkChannel GetChannel(long channelId)
        {
            return GetChild<NetworkChannel>(channelId);
        }

        public override NetworkChannel ConnectChannel(Session session, string address)
        {
            return ComponentFactory.Create<WebSocketChannelComponent, Session, string>
                (session, address, this, true);
        }

        public override void RemoveChannel(long channelId)
        {
            RemoveChild(channelId);
        }

        public override void Dispose()
        {
            if(IsDispose) return;
            
            _taskThread.Abort();
            _taskThread.Join();
            _taskThread = null;
            _httpListener.Close();
            
            base.Dispose();
        }
    }
}