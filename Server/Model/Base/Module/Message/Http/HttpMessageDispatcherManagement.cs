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
            foreach (var type in AssemblyManagement.AllType.Where(d =>
                d.IsDefined(typeof(HTTPApiControllerAttribute), false)))
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    if (methodInfo.IsDefined(typeof(PostAttribute), false))
                    {
                        var url = methodInfo.GetCustomAttribute<PostAttribute>(false).Url;

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
                        var url = methodInfo.GetCustomAttribute<GetAttribute>(true).Url;

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
                        var url = methodInfo.GetCustomAttribute<PostJsonAttribute>(false).Url;

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
                        var url = methodInfo.GetCustomAttribute<GetJsonAttribute>(false).Url;

                        if (string.IsNullOrWhiteSpace(url))
                        {
                            Log.Error($"class:{type.Name} method:{methodInfo.Name} PostAttribute Url not null");
                            continue;
                        }

                        _actionHandler[url] = new GetJsonActionHandler(type, methodInfo);
                    }
                }
            }

            Instance = this;
        }

        public ActionResult Handler(HttpListenerContext context)
        {
            return _actionHandler.TryGetValue(context.Request.RawUrl.Split('?')[0], out var handler)
                ? handler.Run(context)
                : null;
        }
    }
}