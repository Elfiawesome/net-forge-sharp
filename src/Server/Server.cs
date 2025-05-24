using System.Collections.Generic;
using System.Threading;
using Server.Connection;
using Server.Listener;
using Server.PacketHandler;
using Shared.Network;

namespace Server;

public class Server
{
	private readonly CancellationTokenSource _serverCancelationTokenSource = new();
	private readonly CancellationToken _serverCancelationToken;
	private readonly List<BaseListener> _listeners = new();
	private readonly PacketHandlerServer packetHandlerServer = new();

	public Server()
	{
		_serverCancelationToken = _serverCancelationTokenSource.Token;
	}

	// Note: Attaching a listener after the server has started will not activate the listener
	public void AttachListener(BaseListener listener)
	{
		_listeners.Add(listener);
		listener.ConnectionAccepted += OnNewConnection;
	}

	public void DetachListener(BaseListener listener)
	{
		_listeners.Remove(listener);
		listener.ConnectionAccepted -= OnNewConnection;
	}

	// Starts the server
	public void Start()
	{
		foreach (var listener in _listeners)
		{
			listener.StartListening(_serverCancelationToken);
		}
	}

	// Stops the server
	public void Stop()
	{
		foreach (var listener in _listeners)
		{
			listener.StopListening();
		}
		// Note that the listener still has a reference to this server object (via .ConnectionAccepted). Should remove this reference
	}

	private void OnNewConnection(BaseConnection connection)
	{
		connection.PacketReceived += (BasePacket packet) =>
		{
			
		};
	}
}