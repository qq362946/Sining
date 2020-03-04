using System;
using System.Buffers;
using System.Net.Sockets;
using Sining.Tools;

namespace Sining.Network
{
    public class SocketAsyncEventArg : IDisposable
    {
        public SocketAsyncEventArgs Arg { get; private set; }
        public IMemoryOwner<byte> Owner { get; private set; }

        public void Initialization(int ownerSize = 0)
        {
            Arg = ObjectPool<SocketAsyncEventArgs>.Rent();
            Arg.UserToken = this;
            Owner = MemoryPool<byte>.Shared.Rent(ownerSize);
        }

        public void Dispose()
        {
            if (Arg != null)
            {
                Arg.UserToken = null;
                Arg.RemoteEndPoint = null;
                ObjectPool<SocketAsyncEventArgs>.Return(Arg);
                Arg = null;
            }

            Owner?.Dispose();
            Owner = null;
        }
    }
}