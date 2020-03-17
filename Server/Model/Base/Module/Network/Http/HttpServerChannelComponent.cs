using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Server.Network;
using Sining.DataStructure;
using Sining.Event;
using Sining.Message;
using Sining.Module;
using Sining.Tools;

namespace Sining.Network
{
    [ComponentSystem]
    public class HttpServerChannelComponentAwakeSystem : AwakeSystem<HttpServerChannelComponent, IEnumerable<string>>
    {
        protected override void Awake(HttpServerChannelComponent self, IEnumerable<string> urls)
        {
            self.Awake(urls);
        }
    }

    public class HttpServerChannelComponent : NetworkChannel
    {
        private PacketParser _parser;
        private NetworkComponent _networkComponent;
        private HttpListener _httpListener;
        private readonly CircularBuffer _recvBuffer = new CircularBuffer();

        public void Awake(IEnumerable<string> urls)
        {
            _networkComponent = Parent.GetParent<NetworkComponent>();
            MemoryStream = MemoryStreamPool.Instance.GetStream("Message", short.MaxValue);
            _parser = new PacketParser(_recvBuffer, MemoryStream);

            Start(urls);
        }

        private void Start(IEnumerable<string> urls)
        {
            try
            {
                _httpListener = new HttpListener();
                
                foreach (var prefix in urls)
                {
                    _httpListener.Prefixes.Add(prefix);
                }
                    
                _httpListener.Start();

                _httpListener.BeginGetContext(Recv, _httpListener);
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
        
        private void Recv(IAsyncResult result)
        {
            if (!(result.AsyncState is HttpListener httpListener)) return;

            OneThreadSynchronizationContext.Instance.Post(OnRecvComplete,httpListener.EndGetContext(result));
            
            httpListener.BeginGetContext(Recv, httpListener);
        }

        private void OnRecvComplete(object o)
        {
            var context = (HttpListenerContext) o;
            object message;

            try
            {
                _parser.JsonParse(context.Request.InputStream);

                var messageType = NetworkProtocolManagement.Instance.GetType(context.Request.RawUrl);

                message = _networkComponent.MessagePacker.DeserializeFrom(messageType, MemoryStream);
            }
            catch (Exception e)
            {
                // 出现这样错误，肯定是其他人恶意发送封包造成的。

                Log.Warning($"Likely to be attacked IP: {context.Request.RemoteEndPoint?.Address} {e}");

                return;
            }

            var session = _networkComponent.Create();
            session.Channel = this;
            session.MemoryStream = MemoryStream;
            _parser.Clear();

            session.Receive(context, message);
            if (!(message is IRequest)) SendEmpty(session);
        }

        private void SendEmpty(Session session)
        {
            var context = session.Context;
            context.Response.StatusCode = (int) HttpStatusCode.OK;
            context.Response.Close();
            session.Channel = null;
            session.Dispose();
        }

        public override void Send(Session session, MemoryStream memoryStream)
        {
            var context = session.Context;

            context.Response.StatusCode = (int) HttpStatusCode.OK;
            context.Response.ContentType = "application/json";
            context.Response.OutputStream.Write(MemoryStream.GetBuffer());
            context.Response.Close();
            session.Channel = null;
            session.Dispose();
        }

        public override void Dispose()
        {
            if (IsDispose) return;

            _httpListener.Close();
            MemoryStream.Dispose();
            _parser = null;
            _networkComponent = null;
            _recvBuffer.Clear();

            base.Dispose();
        }
    }
}