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

        public object Run(Scene scene, HttpListenerContext context)
        {
            HttpControllerBase.SetContext(context,scene);
            return Handler(scene, context);
        }

        protected abstract object Handler(Scene scene, HttpListenerContext context);
    }
}