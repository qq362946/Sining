using System;
using System.Net;
using System.Net.Http;
using Sining.Event;
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
        private readonly HttpClient _client = new HttpClient();
        public void Awake()
        {
            Instance = this;
        }
        public async STask<string> CallNotDeserialize(string url, HttpContent content)
        {
            var response = await _client.PostAsync(url, content);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"Unable to connect to server url {(object) url} HttpStatusCode:{(object) response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }
        public async STask<T> Call<T>(string url, HttpContent content)
        {
            return await Deserialize<T>(url, await _client.PostAsync(url, content));
        }
        public async STask<T> Call<T>(string url, HttpMethod method)
        {
            return await Deserialize<T>(url, await _client.SendAsync(new HttpRequestMessage(method, url)));
        }
        private async STask<T> Deserialize<T>(string url, HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"Unable to connect to server url {(object) url} HttpStatusCode:{(object) response.StatusCode}");
            }

            return (await response.Content.ReadAsStringAsync()).Deserialize<T>();
        }
        public override void Dispose()
        {
            Instance = null;
            base.Dispose();
        }
    }
}