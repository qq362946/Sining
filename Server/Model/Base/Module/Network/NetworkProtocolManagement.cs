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
        private readonly DoubleMapDictionary<string, Type> _apiMessageEvents = new DoubleMapDictionary<string, Type>();

        public void Init()
        {
            foreach (var type in AssemblyManagement.AllType.Where(d =>
                d.IsDefined(typeof(MessageAttribute), true)))
            {
                var messageAttribute = type.GetCustomAttribute<MessageAttribute>();

                _messageEvents.Add(messageAttribute.Opcode, type);

                if (!string.IsNullOrWhiteSpace(messageAttribute.RawUrl))
                {
                    _apiMessageEvents.Add(messageAttribute.RawUrl, type);
                }
            }

            Instance = this;
        }

        public ushort GetOpCode(Type type)
        {
            return _messageEvents.GetKeyByValue(type);
        }
        
        public string GetRawUrl(Type type)
        {
            return _apiMessageEvents.GetKeyByValue(type);
        }

        public Type GetType(ushort code)
        {
            return _messageEvents.GetValueByKey(code);
        }
        
        public Type GetType(string rawUrl)
        {
            return _apiMessageEvents.GetValueByKey(rawUrl);
        }

        public void Clear()
        {
            _messageEvents.Clear();
            _apiMessageEvents.Clear();
        }

        public override void Dispose()
        {
            if (IsDispose) return;

            Clear();
            
            base.Dispose();
        }
    }
}