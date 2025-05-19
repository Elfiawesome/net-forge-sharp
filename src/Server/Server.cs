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

namespace Server;

public class Server
{
	private readonly CancellationTokenSource _cancellationTokenSource;
	private readonly CancellationToken _cancellationToken;
	private readonly PacketHandlerServer _packetHandlerServer;
	public readonly Dictionary<Guid, BaseServerConnection> Connections = [];
	
	public Server()
	{
		// This cancellation token will be the only one used throughout the entire program
		_cancellationTokenSource = new();
		_cancellationToken = _cancellationTokenSource.Token;

		_packetHandlerServer = new(this);
		// Bootstrap any registries we need
		BasePacket.Register();
	}

	public async Task StartListeningAsync()
	{
		var listener = new TcpListener(IPAddress.Any, 3115);
		listener.Start();
		while (!_cancellationToken.IsCancellationRequested)
		{
			var tcpClient = await listener.AcceptTcpClientAsync(_cancellationToken);
			var tcpServerConnection = new TCPServerConnection(tcpClient, _cancellationToken);
			tcpServerConnection.PacketReceived += OnConnectionPacketReceived;
			tcpServerConnection.Disconnected += OnConnectionDisconnected;

			// Ask connecting client for login packet
			tcpServerConnection.SendPacket(new S2CRequestLoginPacket());
		}
	}

	public void OnConnectionPacketReceived(BaseServerConnection connection, BasePacket packet)
	{
		DebugLogger.Log($"packet received from connection {packet}|{connection}");
		_packetHandlerServer.HandlePacket(packet, connection);
	}

	public void OnConnectionConnected(BaseServerConnection connection, Guid clientId)
	{
		// Do whatever for client entrance
	}

	public void OnConnectionDisconnected(BaseServerConnection connection)
	{
		// Do whatever for client exit
	}
}