using System;
using Sining.Event;

namespace Sining.Network
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageSystemAttribute : BaseAttribute
    {
        public MessageSystemAttribute() { }
    }
}