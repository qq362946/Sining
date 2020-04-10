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
        private readonly DoubleMapDictionary<ushort, Type> _messageEvents = new DoubleMapDictionary<ushort, Type>();
        public void Init()
        {
            foreach (var allTypes in AssemblyManagement.AllType.Values)
            {
                foreach (var type in allTypes.Where(d =>
                    d.IsDefined(typeof(MessageAttribute), true)))
                {
                    var messageAttribute = type.GetCustomAttribute<MessageAttribute>();

                    _messageEvents.Add(messageAttribute.Opcode, type);
                }
            }

            Instance = this;
        }
        public ushort GetOpCode(Type type)
        {
            return _messageEvents.GetKeyByValue(type);
        }
        public Type GetType(ushort code)
        {
            return _messageEvents.GetValueByKey(code);
        }
        public void ReLoad()
        {
            Clear();
            Init();
        }
        private void Clear()
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