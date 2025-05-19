using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Shared.Network;
using Server.Connection;
using System.Collections.Generic;
using System;
using Shared.Network.Packets.Clientbound.Authentication;
using Shared;
using Server.Listener;

namespace Server;

public class Server
{
	private readonly CancellationTokenSource _cancellationTokenSource;
	private readonly CancellationToken _cancellationToken;
	private readonly PacketHandlerServer _packetHandlerServer;
	public readonly Dictionary<Guid, BaseServerConnection> Connections = [];
	public BaseListener? mainListener;

	public Server()
	{
		// This cancellation token will be the only one used throughout the entire program
		_cancellationTokenSource = new();
		_cancellationToken = _cancellationTokenSource.Token;

		_packetHandlerServer = new(this);
		// Bootstrap any registries we need
		BasePacket.Register();
	}

	public void AttachListener(BaseListener baseListener)
	{
		if (mainListener != null)
		{
			mainListener = baseListener;
			_ = baseListener.StartListening(_cancellationToken);
			baseListener.ConnectionAccepted += OnConnectionRequest;
		}
	}

	// When Connection wants to join game but is required to do the handshake login first
	public void OnConnectionRequest(BaseServerConnection connection)
	{
		connection.PacketReceived += OnConnectionPacketReceived;
		connection.Disconnected += OnConnectionDisconnected;
		connection.SendPacket(new S2CRequestLoginPacket());
	}


	public void OnConnectionConnected(BaseServerConnection connection, Guid clientId)
	{
		// Do whatever for client entrance
		DebugLogger.Log("Client success connected!");
	}

	public void OnConnectionDisconnected(BaseServerConnection connection)
	{
		// Do whatever for client exit
	}

	public void OnConnectionPacketReceived(BaseServerConnection connection, BasePacket packet)
	{
		_packetHandlerServer.HandlePacket(packet, connection);
	}
}