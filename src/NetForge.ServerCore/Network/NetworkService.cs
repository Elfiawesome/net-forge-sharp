
using System;
using System.Collections.Generic;
using System.Threading;
using NetForge.ServerCore.Network.Connection;
using NetForge.ServerCore.Network.Listener;
using NetForge.Shared;
using NetForge.Shared.Debugging;
using NetForge.Shared.Network.Packet;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;
using NetForge.Shared.Network.Packet.Serverbound.Authentication;

namespace NetForge.ServerCore.Network;

public class NetworkService
{
	public readonly int ProtocolNumber = 1;
	public event Action<PlayerId> NewConnectionEvent = delegate { };
	private readonly CancellationTokenSource _CancellationTokenSource;
	private readonly CancellationToken _CancellationToken;
	private readonly List<BaseListener> _listeners = [];
	private readonly Dictionary<PlayerId, BaseConnection> _connections = [];

	public NetworkService(CancellationToken _parentCancellationToken)
	{
		_CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentCancellationToken);
		_CancellationToken = _CancellationTokenSource.Token;
	}

	public void AddListener(BaseListener listener)
	{
		listener.NewConnectionEvent += OnNewConnection;
		_listeners.Add(listener);
	}

	public void RemoveListener(BaseListener listener)
	{
		listener.NewConnectionEvent -= OnNewConnection;
		_listeners.Remove(listener);
	}

	public void StartListeners()
	{
		foreach (var listener in _listeners)
		{
			listener.Listen(_CancellationToken);
		}
	}

	public void StopListeners()
	{
		foreach (var listener in _listeners) { listener.Stop(); }
		_CancellationTokenSource.Cancel();
	}

	private void OnNewConnection(BaseConnection connection)
	{
		// 1. S->C Request player for login request
		// 2. C->S Send login request with username
		// 3. S->C Send login success packet with player ID
		// 4. NetworkService will notify GameService that a new player has connected using PlayerID and store Connection inside of a dictionary.
		// Meaning to say Connection object will not leave NetworkService.
	}
}