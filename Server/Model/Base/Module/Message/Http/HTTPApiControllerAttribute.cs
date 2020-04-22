using System;
using System.Net;
using Sining.Event;

namespace Sining.Network
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HTTPApiControllerAttribute : BaseAttribute
    { }
    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : Attribute
    {
        public string Url { get; }

        public PostAttribute(string url)
        {
            Url = url;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class PostJsonAttribute : Attribute
    {
        public string Url { get; }

        public PostJsonAttribute(string url)
        {
            Url = url;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : Attribute
    {
        public string Url { get; }

        public GetAttribute(string url)
        {
            Url = url;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class GetJsonAttribute : Attribute
    {
        public string Url { get; }

        public GetJsonAttribute(string url)
        {
            Url = url;
        }
    }
}