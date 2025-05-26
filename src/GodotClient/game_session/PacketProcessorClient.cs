using System;
using System.Collections.Generic;
using NetForge.ClientCore;
using NetForge.Shared.Network.Packet;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;

namespace NetForge.game_session;

public class PacketProcessorClient : IPacketProcessor
{

	private readonly Dictionary<PacketId, Action<BasePacket>> _handlers = [];
	private readonly Client client;

	public PacketProcessorClient(Client client)
	{
		this.client = client;
		// Register packet handlers
		RegisterPacketHandler<S2CDisconnectPacket>(PacketId.S2CDisconnectPacket, OnDisconnectPacket);
		RegisterPacketHandler<S2CLoginSuccessPacket>(PacketId.S2CLoginSuccessPacket, OnLoginSuccessPacket);
		RegisterPacketHandler<S2CRequestLoginPacket>(PacketId.S2CRequestLoginPacket, OnRequestLoginPacket);
	}

	private void RegisterPacketHandler<TPacket>(PacketId packetId, Action<TPacket> handler)
	{
		if (_handlers.ContainsKey(packetId)) { return; }
		_handlers[packetId] = (packet) =>
		{
			if (packet is TPacket typedPacket) // Should always be true unless we registered a wrong id with a wrong packet object
			{
				handler(typedPacket);
			}
		};
	}

	public void ProcessPacket(BasePacket packet)
	{
		if (_handlers.TryGetValue(packet.Id, out var handler))
		{
			handler.Invoke(packet);
		}
	}

	private void OnDisconnectPacket(S2CDisconnectPacket packet)
	{

	}

	private void OnLoginSuccessPacket(S2CLoginSuccessPacket packet)
	{

	}
	private void OnRequestLoginPacket(S2CRequestLoginPacket packet)
	{

	}
}