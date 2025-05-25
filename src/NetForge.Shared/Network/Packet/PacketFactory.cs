using System;
using System.Collections.Generic;

namespace NetForge.Shared.Network.Packet;

public static class PacketFactory
{
	private static readonly Dictionary<PacketId, Func<BasePacket>> _packetFactory = [];

	public static void Initialize()
	{
		// Register here...
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