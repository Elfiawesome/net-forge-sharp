using System;
using System.Collections.Generic;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;
using NetForge.Shared.Network.Packet.Serverbound.Authentication;

namespace NetForge.Shared.Network.Packet;

public static class PacketFactory
{
	private static readonly Dictionary<PacketId, Func<BasePacket>> _packetFactory = [];

	public static void Initialize()
	{
		Register<S2CDisconnectPacket>(PacketId.S2CDisconnectPacket);
		Register<S2CRequestLoginPacket>(PacketId.S2CRequestLoginPacket);
		Register<C2SLoginResponsePacket>(PacketId.C2SLoginResponsePacket);
		Register<S2CLoginSuccessPacket>(PacketId.S2CLoginSuccessPacket);
		
	}

	public static void Register<TPacket>(PacketId packetId) where TPacket : BasePacket, new()
	{
		if (_packetFactory.ContainsKey(packetId))
		{
			return;
		}
		_packetFactory[packetId] = () => new TPacket();
	}

	public static BasePacket? Create(PacketId packetId)
	{
		if (!_packetFactory.ContainsKey(packetId)) { return null; }

		BasePacket? packet = _packetFactory[packetId]?.Invoke();
		return packet;
	}
}