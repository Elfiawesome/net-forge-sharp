using System.Collections.Generic;
using NetForge.ServerCore.Network.Connection;
using NetForge.Shared.Network.Packet;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;

namespace NetForge.ServerCore.Network;

public class Handshake
{
	// When a client connects, we need to first do a handshake to determine its id
	// We will first request the client for its data
	// The client will send a response with its username and protocol version
	// The server will determine if the protocol version match, then assign a hashed/GUid of the username to the client id
	// However still need to manage how should packets be handled around. Or should refactor the packet architecture

	private readonly List<BaseConnection> _connections = [];

	public Handshake()
	{

	}

	public void AddNewConnection(BaseConnection connection)
	{
		_connections.Add(connection);
		connection.PacketReceivedEvent += (packet) => OnConnectionPacketReceived(connection, packet);
		connection.DisconnectedEvent += () => OnConnectionDisconnected(connection);
		connection.SendData(new S2CRequestLoginPacket());
	}

	private void OnConnectionPacketReceived(BaseConnection connection, BasePacket packet)
	{
		
	}

	private void OnConnectionDisconnected(BaseConnection connection)
	{
		_connections.Remove(connection);
	}
}