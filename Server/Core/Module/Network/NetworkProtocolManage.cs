using System;
using Sining.DataStructure;

namespace Server.Network
{
    public static class NetworkProtocolManage
    {
        private static readonly DoubleMapDIc<ushort, Type> MessageEvents = new DoubleMapDIc<ushort, Type>();

        public static void Add(ushort code, Type type)
        {
            MessageEvents.Add(code, type);
        }
        
        public static ushort GetOpCode(Type type)
        {
            return MessageEvents.GetKeyByValue(type);
        }

        public static Type GetType(ushort code)
        {
            return MessageEvents.GetValueByKey(code);
        }

        public static void Clear()
        {
            MessageEvents.Clear();
        }
    }
}