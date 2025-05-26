using System.Collections.Generic;
using System.Threading;
using NetForge.ServerCore.Network;
using NetForge.ServerCore.Network.Connection;
using NetForge.ServerCore.Network.Listener;
using NetForge.Shared.Debugging;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;

namespace NetForge.ServerCore;

public class Server
{
	public readonly int ProtocolNumber = 1;
	private readonly CancellationTokenSource _serverCancellationTokenSource;
	private readonly CancellationToken _serverCancellationToken;
	private readonly List<BaseListener> _listeners = [];
	private readonly PacketProcessorServer _packetProcessorServer;

	public Server()
	{
		// I've come to the conclusion that if an object is just and extension of it's parent, we can just have circular reference.
		_packetProcessorServer = new(this); // note circular reference here
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

	public void RemoveListener(BaseListener listener)
	{
		listener.NewConnectionEvent -= OnNewConnection;
		_listeners.Remove(listener);
	}

	private void OnNewConnection(BaseConnection connection)
	{
		connection.PacketProcessor = _packetProcessorServer;
		// Do Handshake
		connection.SendPacket(new S2CRequestLoginPacket());
		// The rest of the handshake will be handled by the PacketProcessorServer
	}
}
