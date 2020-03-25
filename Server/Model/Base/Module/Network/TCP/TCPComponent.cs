using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Sining.Event;
using Sining.Module;
using Sining.Tools;

namespace Sining.Network
{
    [ComponentSystem]
    public class TCPComponentAwakeSystem : AwakeSystem<TCPComponent, EndPoint>
    {
        protected override void Awake(TCPComponent self, EndPoint ipEndPoint)
        {
            self.Awake(ipEndPoint);
        }
    }

    public class TCPComponent : NetworkProtocol
    {
        private Socket _socket;
        private const int MaxClient = 2000;
        private volatile SocketAsyncEventArgs _acceptAsync = new SocketAsyncEventArgs();
        private NetworkComponent _networkComponent;
       
        public void Awake(EndPoint ipEndPoint)
        {
            _networkComponent = GetParent<NetworkComponent>();
            _socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            
            _acceptAsync.Completed += OnCompleted;
            
            if (ipEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                _socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            }

            _socket.Bind(ipEndPoint);
            _socket.Listen(MaxClient);

            StartAcceptAsync();
        }

        private void StartAcceptAsync()
        {
            _acceptAsync.AcceptSocket = null;

            if (_socket.AcceptAsync(_acceptAsync)) return;

            AcceptComplete(_acceptAsync);
        }
        
        private void OnCompleted(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            if (asyncEventArgs.LastOperation != SocketAsyncOperation.Accept)
                throw new Exception($"Socket Accept Error: {asyncEventArgs.LastOperation}");

            AcceptComplete(asyncEventArgs);
        }
        
        private void AcceptComplete(SocketAsyncEventArgs asyncEventArgs)
        {
            if (_acceptAsync == null) return;

            if (asyncEventArgs.SocketError != SocketError.Success)
            {
                Log.Error($"Socket Accept Error: {asyncEventArgs.SocketError}");
            }
            else
            {
                AddChannel(asyncEventArgs);
            }

            StartAcceptAsync();
        }

        private void AddChannel(SocketAsyncEventArgs asyncEventArgs)
        {
            var session = _networkComponent.Create();

            ComponentFactory.Create<TCPChannelComponent, Session, SocketAsyncEventArgs>(
                Scene, session, asyncEventArgs, this, true);
        }

        public override NetworkChannel GetChannel(long channelId)
        {
            return GetChild<NetworkChannel>(channelId);
        }

        public override NetworkChannel ConnectChannel(Session session, string address)
        {
            return ComponentFactory.Create<TCPChannelComponent, Session, IPEndPoint>(
                Scene, session, NetworkHelper.ToIPEndPoint(address), this, true);
        }

        public override void RemoveChannel(long channelId)
        {
            RemoveChild(channelId);
        }

        public override void Dispose()
        {
            if (IsDispose) return;

            base.Dispose();

            _socket.Dispose();
            _socket = null;
            _acceptAsync.AcceptSocket = null;
            _acceptAsync.Completed -= OnCompleted;
        }
    }
}