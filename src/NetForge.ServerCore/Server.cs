using System.Collections.Generic;
using System.Threading;
using NetForge.ServerCore.Network;
using NetForge.ServerCore.Network.Listener;
using NetForge.Shared.Debugging;

namespace NetForge.ServerCore;

public class Server
{
	private readonly CancellationTokenSource _serverCancellationTokenSource;
	private readonly CancellationToken _serverCancellationToken;
	private readonly List<BaseListener> _listeners = [];

	private readonly Handshake _handshake = new();
	
	// Manager system? Not sure how to abstract
	// private readonly GameLogicManager _gameLogicManager = new();
	// private readonly NetworkManager _networkManager = new();

	public Server()
	{
		_serverCancellationTokenSource = new();
		_serverCancellationToken = _serverCancellationTokenSource.Token;
	}

	public void Start()
	{
		Logger.Log("Starting server...");
		// Start all listeners
		foreach (var listener in _listeners)
		{
			_ = listener.Listen(_serverCancellationToken);
		}
	}

	public void Stop()
	{
		Logger.Log("Stopping server...");
		// This will stop all BaseListener.Listen and BaseConnection.Process function
		// BaseConnection Should be eligible for GD since no reference to it + no async left
		_serverCancellationTokenSource.Cancel();
	}

	public void AddListener(BaseListener listener)
	{
		_listeners.Add(listener);
		listener.NewConnectionEvent += _handshake.AddNewConnection;
	}
}
