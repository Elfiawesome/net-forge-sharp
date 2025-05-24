using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Server.Connection;
using Server.Listener;
using Server.PacketProcessor;
using Server.Space;
using Shared;
using Shared.Network;
using Shared.Network.Packets.Clientbound.Handshake;

namespace Server;

public class Server
{
	private readonly CancellationTokenSource _serverCancelationTokenSource = new();
	private readonly CancellationToken _serverCancelationToken;
	private readonly List<BaseListener> _listeners = new();
	private readonly PacketProcessorServer _packetProcessor = new();
	
	public Server()
	{
		_serverCancelationToken = _serverCancelationTokenSource.Token;
		BasePacket.Register();
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
		
		

		// Stops all running async functions 
		_serverCancelationTokenSource.Cancel();
	}

	private void OnNewConnection(BaseConnection connection)
	{
		connection.PacketReceived += (BasePacket packet) =>
		{
			_packetProcessor.ProcessPacket(this, packet, connection);
		};
		connection.Disconnected += () =>
		{
			// Remove this player from the game etc etc.
		};
		connection.SendPacket(new S2CLoginRequestPacket());
	}
}