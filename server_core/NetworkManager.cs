using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace NetForge
{
	class NetworkManager
	{
		public event Action<BaseServerConnection, BasePacket> PacketReceived = delegate {};
		
		private TcpListener ?_tcpListener;
		private readonly CancellationTokenSource _networkManagerCts;
		private ConcurrentDictionary<Guid, BaseServerConnection> _connections;

		public NetworkManager()
		{
			_connections = new ConcurrentDictionary<Guid, BaseServerConnection>();
			_networkManagerCts = new CancellationTokenSource();
		}

		public void StartServer()
		{
			_tcpListener = new TcpListener(IPAddress.Any, 3115);
			_tcpListener.Start();
			Task.Run(() => acceptClientTask(_networkManagerCts.Token));
		}

		public void StopServer()
		{
			_networkManagerCts.Cancel();

			foreach(var item in _connections)
			{
				item.Value.Disconnect();
			}
			_connections.Clear();

			_tcpListener?.Stop();
		}

		private async Task acceptClientTask(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				if (_tcpListener == null) { continue; }
				TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync(token);
				var newConnection = new TCPServerConnection(tcpClient);
				_connections.TryAdd(newConnection.ClientId, newConnection);
				newConnection.PacketReceived += (BaseServerConnection connection, BasePacket packet) => {
					GD.Print("Server received packet: "+packet);
				};
				newConnection.Disconnected += (BaseServerConnection connection) => {
					GD.Print("Client disconnected");
					GD.Print($"{_connections.Count} left");
				};
			}
		}
	}
}