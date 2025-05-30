using System.Threading;
using NetForge.ServerCore.GameCore;
using NetForge.ServerCore.Network;
using NetForge.Shared.Debugging;

namespace NetForge.ServerCore;

public class Server
{
	private readonly CancellationTokenSource _serverCancellationTokenSource;
	public readonly CancellationToken _serverCancellationToken;
	public readonly GameService GameService;
	public readonly NetworkService NetworkService;

	public Server()
	{
		_serverCancellationTokenSource = new();
		_serverCancellationToken = _serverCancellationTokenSource.Token;

		NetworkService = new(_serverCancellationToken);
		GameService = new(_serverCancellationToken);
	}

	public void Start()
	{
		Logger.Log("[Server] Starting server...");
		// Start all listeners
		NetworkService.StartListeners();
	}

	public void Stop()
	{
		Logger.Log("[Server] Stopping server...");
		NetworkService.StopListeners();
		_serverCancellationTokenSource.Cancel();
	}
}
