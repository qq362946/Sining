using System;
using System.Collections.Generic;
using Sining.Config;
using Sining.Module;
using Sining.Network;

namespace Sining.Tools
{
    public static partial class MessageHelper
    {
        private static readonly Dictionary<int, string> AddressCache = new Dictionary<int, string>();

        public static void Send(this IMessage message, int serverId)
        {
            NetInnerComponent.Instance.GetSession(GetAddress(serverId)).Send(message);
        }
        
        public static void Send(this IMessage message, string address)
        {
            NetInnerComponent.Instance.GetSession(address).Send(message);
        }

        public static STask<TResponse> Call<TResponse>(this IRequest request, int serverId) where TResponse : IResponse
        {
            return NetInnerComponent.Instance.GetSession(GetAddress(serverId)).Call<TResponse>(request);
        }

        public static void SendActor()
        {
            
        }

        public static void CallActor()
        {
            
        }

        private static string GetAddress(int serverId)
        {
            if (AddressCache.TryGetValue(serverId, out var address)) return address;

            // address = NetInnerComponent.Instance.Get(serverId);
            //
            // if (address == null)
            // {
            //     throw new Exception("没有找到该服务器的配置文件");
            // }
            //
            // AddressCache.Add(serverId, address);

            return address;
        }
    }
}