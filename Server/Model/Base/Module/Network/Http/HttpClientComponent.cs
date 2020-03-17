using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Server.Network;
using Sining.Event;
using Sining.Module;
using Sining.Tools;

namespace Sining.Network
{
    [ComponentSystem]
    public class HttpClientComponentAwake : AwakeSystem<HttpClientComponent>
    {
        protected override void Awake(HttpClientComponent self)
        {
            self.Awake();
        }
    }

    public class HttpClientComponent : Component
    {
        public static HttpClientComponent Instance { get; private set; }
        private MessagePacker _messagePacker;
        private readonly HttpClient _client = new HttpClient();

        public void Awake()
        {
            _messagePacker = AddComponent<JsonMessagePacker>();
            Instance = this;
        }

        public void Send<T>(T t, string url) where T : IMessage
        {
            try
            {
                SendAsync(url, t).Coroutine();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private async SVoid SendAsync<T>(string url, T t) where T : IMessage
        {
            var content = new StringContent(t.Serialize(), Encoding.UTF8);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await _client.PostAsync($"{url}{NetworkProtocolManagement.Instance.GetRawUrl(typeof(T))}",
                content);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Unable to connect to server url {url} HttpStatusCode:{response.StatusCode}");
            }
        }

        public async STask<TResponse> Call<TResponse>(IRequest request, string url) where TResponse : IResponse
        {
            var content = new StringContent(request.Serialize(), Encoding.UTF8);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response =
                await _client.PostAsync($"{url}{NetworkProtocolManagement.Instance.GetRawUrl(request.GetType())}",
                    content);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Unable to connect to server url {url} HttpStatusCode:{response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();

            return _messagePacker.DeserializeFrom<TResponse>(json);
        }

        public override void Dispose()
        {
            Instance = null;
            _messagePacker = null;

            base.Dispose();
        }
    }
}