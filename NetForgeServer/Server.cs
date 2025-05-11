using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetForgeServerSharp
{
	public class Server
	{
		private CancellationTokenSource _serverCts = new CancellationTokenSource();
		private Task ?_acceptClientTask;
		private Task ?_gameLoopTask;

		private ConcurrentDictionary<Guid, IConnection> _pendingConnections = new ConcurrentDictionary<Guid, IConnection>();
		private ConcurrentDictionary<Guid, IConnection> _connections = new ConcurrentDictionary<Guid, IConnection>();
		
		private TcpListener ?_tcpListener;
		private bool _isRunning = false;

		public Server()
		{
			// Create the default space here
		}

		public void Start()
		{
			if (_isRunning)
			{
				Console.WriteLine("Server is already running.");
				return;
			}
			_isRunning = true;
			_acceptClientTask = Task.Run(() => AcceptClientsAsync(_serverCts.Token));
			_gameLoopTask = Task.Run(() => MainGameLoop(_serverCts.Token));
		}

		public async Task Stop()
		{
			if (!_isRunning)
			{
				Console.WriteLine("Server is not running.");
				return;
			}
			_isRunning = false;
			_serverCts.Cancel();
			
			if (_acceptClientTask != null) { await _acceptClientTask; }
			if (_gameLoopTask != null) { await _gameLoopTask; }

			Console.WriteLine("Server stopped.");
		}

		private void MainGameLoop(CancellationToken token)
		{
			while (_isRunning && !token.IsCancellationRequested)
			{
				// Console.WriteLine("Game Looped...");
				Thread.Sleep(100);
			}
			Console.WriteLine("[Exit] Exiting main game loop");
		}

		private async Task AcceptClientsAsync(CancellationToken token)
		{
			_tcpListener = new TcpListener(IPAddress.Any, 3115);
			_tcpListener.Start();
			Console.WriteLine($"Server listening on port {3115}...");

			while (_isRunning && !token.IsCancellationRequested)
			{
				try
				{					
					TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync(token);
					Console.WriteLine($"New TCP Connection");
					var connection = new TCPConnection(tcpClient);
					_pendingConnections.TryAdd(connection.ConnectionId, connection);

					connection.StartReceiving(
						(IConnection connection, BasePacket packet) => {
							Console.WriteLine("Received data");
						},
						(IConnection connection) => {

						}
					);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error accepting client: {ex.Message}");
				}
			}
			_tcpListener.Stop();
			Console.WriteLine("[Exit] Exiting listening loop");
		}

		private void attachConnection(IConnection connection)
		{
			
		}
	}
}