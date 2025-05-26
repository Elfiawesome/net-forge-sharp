using System;
using System.Collections.Generic;
using NetForge.ServerCore.Network.Connection;
using NetForge.ServerCore.Network.PacketProcessor;
using NetForge.Shared.Debugging;
using NetForge.Shared.Network.Packet;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;
using NetForge.Shared.Network.Packet.Serverbound.Authentication;

namespace NetForge.ServerCore.Network;

public class PacketProcessorServer : IPacketProcessor
{
	private readonly Dictionary<PacketId, Action<BaseConnection, BasePacket>> _handlers = [];
	private readonly Server server;

	public PacketProcessorServer(Server server)
	{
		this.server = server;
		// Register packet handlers
		RegisterPacketHandler<C2SLoginResponsePacket>(PacketId.C2SLoginResponsePacket, OnLoginResponsePacket);
	}

	private void RegisterPacketHandler<TPacket>(PacketId packetId, Action<BaseConnection, TPacket> handler)
	{
		if (_handlers.ContainsKey(packetId)) { return; }
		_handlers[packetId] = (connection, packet) =>
		{
			if (packet is TPacket typedPacket) // Should always be true unless we registered a wrong id with a wrong packet object
			{
				handler(connection, typedPacket);
			}
		};
	}

	// Packet Processing
	public void ProcessPacket(BaseConnection connection, BasePacket packet)
	{
		// Handle Packet
		if (_handlers.TryGetValue(packet.Id, out var handler))
		{
			handler.Invoke(connection, packet);
		}
	}

	public void OnConnectionLost(string reason)
	{
		// Handle connection loss
		Logger.Log($"[Server] Connection lost: {reason}");
	}

	// Handle specific packets

	// Handshake Sequence
	private void OnLoginResponsePacket(BaseConnection connection, C2SLoginResponsePacket packet)
	{
		Logger.Log($"[Server] received response packet: Protocol-{packet.ProtocolNumber} from user: {packet.Username}");
		if (packet.ProtocolNumber != server.ProtocolNumber)
		{
			connection.InitiateClose("Protocol mismatch. Server is running version " + server.ProtocolNumber + ", but client is running version " + packet.ProtocolNumber);
			return;
		}

		connection.SendPacket(new S2CLoginSuccessPacket());
	}
}