
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace NetForge
{
	class Server
	{
		private readonly NetworkManager _networkManager;
		private Task ?_gameLoopTask;
		private readonly CancellationTokenSource _serverCts;
		private bool _isRunning = false;
		public Server()
		{
			_serverCts = new CancellationTokenSource();
			_networkManager = new NetworkManager();
		}


		public void Start()
		{
			if (_isRunning) { return; }
			GD.Print("Server started");
			_isRunning = true;
			_gameLoopTask = Task.Run(() => MainGameLoop(_serverCts.Token));
			_networkManager.StartServer();
		}

		public void Stop()
		{
			_isRunning = false;
			_serverCts.Cancel();
			
			_networkManager.StopServer();
			
			GD.Print("Server stopped.");
		}

		public async Task MainGameLoop(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				// GD.Print("Game Ticked...");
				await Task.Delay(500);
			}
		}
	}

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

	public class BaseServerConnection
	{
		public event Action<BaseServerConnection, BasePacket> PacketReceived = delegate {};
		public event Action<BaseServerConnection> Disconnected = delegate {};
		public readonly Guid ClientId = Guid.NewGuid();
		public virtual void Disconnect() {}

		protected virtual void OnPacketReceived(BasePacket packet)
		{
			PacketReceived.Invoke(this, packet);
		}

		protected virtual void OnDisconnected()
		{
			Disconnected.Invoke(this);
		}
	}

	public class TCPServerConnection : BaseServerConnection
	{
		private readonly TcpClient _tcpClient;
		private readonly CancellationTokenSource _cts;
		public TCPServerConnection(TcpClient tcpClient)
		{
			_cts = new CancellationTokenSource();
			_tcpClient = tcpClient;
			Task.Run(() => receiveDataTask(_cts.Token));
		}

		public override void Disconnect()
		{
			if (_cts.IsCancellationRequested) return;
			// Need to get manager to delete me
			OnDisconnected();
			// Stop all other processes
			_cts.Cancel();
			_tcpClient.Close();
		}

		private async Task receiveDataTask(CancellationToken token)
		{
			var packetStream = new PacketStream(_tcpClient.GetStream());
			while (!token.IsCancellationRequested && _tcpClient.Connected)
			{
				var packet = await packetStream.GetPacketAsync(token);
				if (packet != null)
				{
					OnPacketReceived(packet);
				}
				else
				{
					Disconnect();
					break;
				}
			}
			GD.Print($"Client connection closed");
		}
	}
}