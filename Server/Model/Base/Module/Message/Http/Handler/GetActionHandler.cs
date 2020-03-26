﻿using System;
using System.Buffers;
using System.Net;
using System.Reflection;

namespace Sining.Network
{
    public class GetActionHandler : ActionHandler
    {
        public GetActionHandler(Type type, MethodInfo methodInfo) : base(type, methodInfo) { }

        protected override ActionResult Handler(HttpListenerContext context)
        {
            if (context.Request.HttpMethod.ToLower() != "post" ||
                context.Request.ContentType != "application/x-www-form-urlencoded")
            {
                return null;
            }

            var parametersLength = context.Request.QueryString.Count;
            var objectArray = ArrayPool<object>.Shared.Rent(parametersLength);

            try
            {
                for (var i = 0; i < context.Request.QueryString.Count; i++)
                {
                    objectArray[i] = context.Request.QueryString[i];
                }

                var obj = MethodInfo.Invoke(HttpControllerBase,
                    objectArray.AsSpan(0, parametersLength).ToArray());
               
                return (ActionResult) MethodInfo.Invoke(HttpControllerBase,
                    objectArray.AsSpan(0, parametersLength).ToArray());
            }
            finally
            {
                ArrayPool<object>.Shared.Return(objectArray, true);
            }
        }
    }
}