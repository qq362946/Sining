using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Sining.Core;
using Sining.DataStructure;
using Sining.Event;
using Sining.Module;
using Sining.Tools;

namespace Sining.Network
{
    [ComponentSystem]
    public class HttpComponentAwakeSystem : AwakeSystem<HttpComponent, List<string>>
    {
        protected override void Awake(HttpComponent self, List<string> urls)
        {
            self.Awake(urls);
        }
    }

    public class HttpComponent : NetworkProtocol
    {
        private Thread _taskThread;
        private NetworkComponent _networkComponent;
        private PacketParser _parser;
        public MemoryStream MemoryStream;
        private readonly CircularBuffer _recvBuffer = new CircularBuffer();
        private readonly CircularBuffer _sendBuffer = new CircularBuffer();
        private HttpListener _httpListener = new HttpListener();

        public void Awake(IEnumerable<string> urls)
        {
            _networkComponent = GetParent<NetworkComponent>();
            MemoryStream = MemoryStreamPool.Instance.GetStream("Message", short.MaxValue);
            _parser = new PacketParser(_recvBuffer, MemoryStream);

            _taskThread = new Thread(() => Start(urls))
            {
                IsBackground = true
            };

            _taskThread.Start();
        }

        private void Start(IEnumerable<string> urls)
        {
            try
            {
                foreach (var prefix in urls)
                {
                    _httpListener.Prefixes.Add(prefix);
                }
                    
                _httpListener.Start();

                _httpListener.BeginGetContext(ListenerHandle, _httpListener);
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
        }

        private void ListenerHandle(IAsyncResult result)
        {
            _httpListener = result.AsyncState as HttpListener;

            if (_httpListener == null) return;

            TaskProcessingComponent.Instance.Add(() => Request(_httpListener.EndGetContext(result)));

            _httpListener.BeginGetContext(ListenerHandle, _httpListener);
        }

        private void Request(HttpListenerContext context)
        {
            var stream = context.Request.InputStream;

            try
            {
                if (!_parser.Parse(stream))
                {
                    Log.Error("无法解析的数据包");

                    return;
                }
            }
            catch (Exception e)
            {
                // 出现这样错误，肯定是其他人恶意发送封包造成的。

                Log.Warning($"Likely to be attacked IP: {context.Request.RemoteEndPoint?.Address} {e}");

                return;
            }

            var session = _networkComponent.Create();

            session.MemoryStream = MemoryStream;

            session.Receive(context, _parser.MessageProtocolCode, MemoryStream);
            
            _parser.Clear();
        }
        
        public override NetworkChannel GetChannel(long channelId)
        {
            return default;
        }

        public override NetworkChannel ConnectChannel(Session session, string address)
        {
            return default;
        }

        public override void RemoveChannel(long channelId) { }
    }
}