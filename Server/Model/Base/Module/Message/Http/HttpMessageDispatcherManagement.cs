using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Reflection;
using Sining.Tools;

namespace Sining.Network
{
    public class HttpMessageDispatcherManagement : Component
    {
        public static HttpMessageDispatcherManagement Instance;
        private readonly ConcurrentDictionary<string, ActionHandler> _actionHandler =
            new ConcurrentDictionary<string, ActionHandler>();
        public void Init()
        {
            foreach (var allTypes in AssemblyManagement.AllType.Values)
            {
                foreach (var type in allTypes.Where(d =>
                    d.IsDefined(typeof(HTTPApiControllerAttribute), false)))
                {
                    foreach (var methodInfo in type.GetMethods())
                    {
                        if (methodInfo.IsDefined(typeof(PostAttribute), false))
                        {
                            var url = methodInfo.GetCustomAttribute<PostAttribute>(false)?.Url;

                            if (string.IsNullOrWhiteSpace(url))
                            {
                                Log.Error($"class:{type.Name} method:{methodInfo.Name} PostAttribute Url not null");
                                continue;
                            }

                            _actionHandler[url] = new PostActionHandler(type, methodInfo);

                            continue;
                        }

                        if (methodInfo.IsDefined(typeof(GetAttribute), true))
                        {
                            var url = methodInfo.GetCustomAttribute<GetAttribute>(true)?.Url;

                            if (string.IsNullOrWhiteSpace(url))
                            {
                                Log.Error($"class:{type.Name} method:{methodInfo.Name} PostAttribute Url not null");
                                continue;
                            }

                            _actionHandler[url] = new GetActionHandler(type, methodInfo);

                            continue;
                        }

                        if (methodInfo.IsDefined(typeof(PostJsonAttribute), false))
                        {
                            var url = methodInfo.GetCustomAttribute<PostJsonAttribute>(false)?.Url;

                            if (string.IsNullOrWhiteSpace(url))
                            {
                                Log.Error($"class:{type.Name} method:{methodInfo.Name} PostAttribute Url not null");
                                continue;
                            }

                            _actionHandler[url] = new PostJsonActionHandler(type, methodInfo);

                            continue;
                        }

                        if (!methodInfo.IsDefined(typeof(GetJsonAttribute), false)) continue;
                        {
                            var url = methodInfo.GetCustomAttribute<GetJsonAttribute>(false)?.Url;

                            if (string.IsNullOrWhiteSpace(url))
                            {
                                Log.Error($"class:{type.Name} method:{methodInfo.Name} PostAttribute Url not null");
                                continue;
                            }

                            _actionHandler[url] = new GetJsonActionHandler(type, methodInfo);
                        }
                    }
                }
            }

            Instance = this;
        }
        public object Handler(Scene scene, HttpListenerContext context)
        {
            if (!_actionHandler.TryGetValue(context.Request.RawUrl.Split('?')[0], out var handler)) return null;
            
            try
            {
                if (context.Request.HttpMethod.ToLower() != "options")
                {
                    return handler.Run(scene, context);
                }
                
                context.Response.Headers.Add("Access-Control-Max-Age", "2592000");
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "*");
                context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                context.Response.Headers.Add("X-Powered-By", "Jetty");
                return ObjectPool<ActionResult>.Rent().Init(204, context.Request.ContentType, null);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
        public void ReLoad()
        {
            _actionHandler.Clear();
            Init();
        }
    }
}