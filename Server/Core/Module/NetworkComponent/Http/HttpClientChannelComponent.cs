using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Sining.Core;
using Sining.DataStructure;
using Sining.Event;
using Sining.Module;
using Sining.Tools;

namespace Sining.Network
{
    [ComponentSystem]
    public class HttpClientChannelComponentAwakeSystem : AwakeSystem<HttpClientChannelComponent, Session, string>
    {
        protected override void Awake(HttpClientChannelComponent self, Session session, string url)
        {
            self.Awake(session, url);
        }
    }

    public class HttpClientChannelComponent : NetworkChannel
    {
        private string _url;
        private PacketParser _parser;
        private readonly CircularBuffer _recvBuffer = new CircularBuffer();
        private readonly HttpClient _client = new HttpClient();

        public void Awake(Session session, string url)
        {
            MemoryStream = MemoryStreamPool.Instance.GetStream("Message", short.MaxValue);
            _parser = new PacketParser(_recvBuffer, MemoryStream);
            session.MemoryStream = MemoryStream;
            _url = url;
        }

        public override void Send(Session session, MemoryStream memoryStream)
        {
            SendAsync(session, memoryStream);
        }

        private async void SendAsync(Session session, MemoryStream memoryStream)
        {
            var content = new ByteArrayContent(memoryStream.GetBuffer());
            var response = await _client.PostAsync(_url, content);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }

            TaskProcessingComponent.Instance.Add(() => OnRecvComplete(response, session));
        }

        private void OnRecvComplete(HttpResponseMessage response, Session session)
        {
            var stream = response.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult();

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
                Log.Warning($"包的格式不正确 {e}");

                return;
            }

            session.Receive(_parser.MessageProtocolCode, MemoryStream);
            _parser.Clear();
        }
    }
}