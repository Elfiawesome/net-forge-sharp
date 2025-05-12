using System.Threading;
using System.Threading.Tasks;
using Godot;
using NetForge.Network;

namespace NetForge
{
	class Server
	{
		private readonly Network.NetworkManager _networkManager;
		private Task ?_gameLoopTask;
		private readonly CancellationTokenSource _serverCts;
		private bool _isRunning = false;
		public Server()
		{
			_serverCts = new CancellationTokenSource();
			_networkManager = new Network.NetworkManager();
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
				_networkManager.Broadcast(new HelloWorldPacket("HOLA"));
				await Task.Delay(500, token);
			}
		}
	}
}