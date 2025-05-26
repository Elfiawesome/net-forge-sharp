
using System.Collections.Generic;
using NetForge.ServerCore.Network.Connection;
using NetForge.ServerCore.Network.Listener;

namespace NetForge.ServerCore.Network;

public class NetworkService : BaseServerService
{
	private readonly List<BaseListener> _listeners = [];

	public NetworkService(Server server) : base(server)
	{
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
			listener.Listen(server._serverCancellationToken);
		}
	}

	public void StopListeners()
	{
		foreach (var listener in _listeners) { listener.Stop(); }
	}

	private void OnNewConnection(BaseConnection connection)
	{
		server.OnNewConnection(connection);
	}
	
}