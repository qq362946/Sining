using System;
using System.Net;
using System.Reflection;

namespace Sining.Network
{
    public abstract class ActionHandler
    {
        protected readonly MethodInfo MethodInfo;
        protected readonly HTTPControllerBase HttpControllerBase;

        protected ActionHandler(Type type, MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            HttpControllerBase = (HTTPControllerBase) Activator.CreateInstance(type);
        }

        public ActionResult Run(HttpListenerContext context)
        {
            HttpControllerBase.SetContext(context);
            return Handler(context);
        }

        protected abstract ActionResult Handler(HttpListenerContext context);
    }
}