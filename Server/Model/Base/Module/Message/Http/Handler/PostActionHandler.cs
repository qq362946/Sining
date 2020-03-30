using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace Sining.Network
{
    public class PostActionHandler : ActionHandler
    {
        public PostActionHandler(Type type, MethodInfo methodInfo) : base(type, methodInfo) { }
        private static void Parsing(IList<object> objects, HttpListenerContext context)
        {
            using var body = context.Request.InputStream;
            var length = (int) context.Request.ContentLength64;
            var numArray = ArrayPool<byte>.Shared.Rent(length);
            if (length == 0) return;

            try
            {
                body.Read(numArray, 0, length);
                var parameters = Encoding.UTF8.GetString(numArray, 0, length);
                var parameterSplits = parameters.Split('&');

                for (var i = 0; i < parameterSplits.Length; i++)
                {
                    objects[i] = parameterSplits[i].Split('=')[1];
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(numArray, true);
            }
        }

        protected override object Handler(Scene scene, HttpListenerContext context)
        {
            if (context.Request.HttpMethod.ToLower() != "post" ||
                !context.Request.HasEntityBody ||
                context.Request.ContentType != "application/x-www-form-urlencoded")
            {
                return null;
            }

            var parametersLength = MethodInfo.GetParameters().Length;
            var objectArray = ArrayPool<object>.Shared.Rent(parametersLength);

            try
            {
                if (parametersLength == 0)
                {
                    return MethodInfo.Invoke(HttpControllerBase, null);
                }
                
                Parsing(objectArray, context);

                return MethodInfo.Invoke(HttpControllerBase,
                    objectArray.AsSpan(0, parametersLength).ToArray());
            }
            finally
            {
                ArrayPool<object>.Shared.Return(objectArray, true);
            }
        }
    }
}