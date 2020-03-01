using System;
using System.Linq;
using System.Reflection;
using Sining;
using Sining.DataStructure;
using Sining.Network;
using Sining.Tools;

namespace Server.Network
{
    public class NetworkProtocolManagement : Component
    {
        public static NetworkProtocolManagement Instance;
        
        private readonly DoubleMapDIc<ushort, Type> _messageEvents = new DoubleMapDIc<ushort, Type>();

        public void Init()
        {
            foreach (var type in AssemblyManagement.AllType.Where(d =>
                d.IsDefined(typeof(MessageAttribute), true)))
            {
                Add(type.GetCustomAttribute<MessageAttribute>().Opcode, type);
            }

            Instance = this;
        }
        
        public void Add(ushort code, Type type)
        {
            _messageEvents.Add(code, type);
        }
        
        public ushort GetOpCode(Type type)
        {
            return _messageEvents.GetKeyByValue(type);
        }

        public Type GetType(ushort code)
        {
            return _messageEvents.GetValueByKey(code);
        }

        public void Clear()
        {
            _messageEvents.Clear();
        }

        public override void Dispose()
        {
            if (IsDispose) return;

            Clear();
            
            base.Dispose();
        }
    }
}