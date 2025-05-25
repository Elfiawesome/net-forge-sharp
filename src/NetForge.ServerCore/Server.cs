using System.Collections.Generic;
using System.Threading;
using NetForge.ServerCore.Network.Connection;
using NetForge.ServerCore.Network.Listener;
using NetForge.Shared.Debugging;

namespace NetForge.ServerCore;

public class Server
{
	private readonly CancellationTokenSource _serverCancellationTokenSource;
	private readonly CancellationToken _serverCancellationToken;
	private readonly List<BaseListener> _listeners = [];

	public Server()
	{
		_serverCancellationTokenSource = new();
		_serverCancellationToken = _serverCancellationTokenSource.Token;
	}

	public void Start()
	{
		Logger.Log("[Server] Starting server...");
		// Start all listeners
		foreach (var listener in _listeners)
		{
			_ = listener.Listen(_serverCancellationToken);
		}
	}

	public void Stop()
	{
		Logger.Log("[Server] Stopping server...");
		// This will stop all BaseListener.Listen and BaseConnection.Process function
		// BaseConnection Should be eligible for GD since no reference to it + no async left
		_serverCancellationTokenSource.Cancel();
	}

	public void AddListener(BaseListener listener)
	{
		listener.NewConnectionEvent += OnNewConnection;
		_listeners.Add(listener);
	}

	private void OnNewConnection(BaseConnection connection)
	{
		// Do Handshake
		
	}
}
