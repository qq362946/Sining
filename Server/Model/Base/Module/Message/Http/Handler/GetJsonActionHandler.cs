using System;
using System.Buffers;
using System.Net;
using System.Reflection;
using System.Text;
using Sining.Tools;

namespace Sining.Network
{
    public class GetJsonActionHandler : ActionHandler
    {
        public GetJsonActionHandler(Type type, MethodInfo methodInfo) : base(type, methodInfo) { }
        private static string Parsing(HttpListenerContext context)
        {
            using var body = context.Request.InputStream;
            var length = (int) context.Request.ContentLength64;
            var numArray = ArrayPool<byte>.Shared.Rent(length);
            if (length == 0) return null;

            try
            {
                body.Read(numArray, 0, length);
                return Encoding.UTF8.GetString(numArray, 0, length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(numArray, true);
            }
        }

        protected override object Handler(Scene scene, HttpListenerContext context)
        {
            if (context.Request.HttpMethod.ToLower() != "get" ||
                !context.Request.HasEntityBody ||
                context.Request.ContentType != "application/x-www-form-urlencoded")
            {
                return null;
            }

            var parametersLength = MethodInfo.GetParameters().Length;
            var objectArray = ArrayPool<object>.Shared.Rent(parametersLength);

            try
            {
                var component = (Component) Parsing(context).Deserialize(MethodInfo.GetParameters()[0].ParameterType);
                component.Initialization(scene, isFromPool: false);
                objectArray[0] = component;

                return MethodInfo.Invoke(HttpControllerBase, objectArray.AsSpan(0, 1).ToArray());
            }
            finally
            {
                ArrayPool<object>.Shared.Return(objectArray, true);
            }
        }
    }
}