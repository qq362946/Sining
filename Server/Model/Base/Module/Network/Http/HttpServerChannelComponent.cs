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
        private HttpListener _httpListener;
        public void Awake(IEnumerable<string> urls)
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
        public override void Send(Session session, MemoryStream memoryStream)
        {
            throw new Exception("Send messages using HttpHelper");
        }
        private void OnRecvComplete(object obj)
        {
            var context = (HttpListenerContext) obj;
            var result = HttpMessageDispatcherManagement.Instance.Handler(context);

            context.Response.StatusCode = result.StatusCode;

            if (!string.IsNullOrWhiteSpace(result.Response))
            {
                context.Response.OutputStream.Write(Encoding.UTF8.GetBytes(result.Response));
            }

            context.Response.Close();
        }
        public override void Dispose()
        {
            if (IsDispose) return;

            _httpListener.Close();

            base.Dispose();
        }
    }
}