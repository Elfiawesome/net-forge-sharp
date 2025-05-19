using System;
using System.Collections.Generic;

namespace Shared.Network;

public class PacketHandler
{
	private readonly Dictionary<PacketId, Action<BasePacket>> _packetHandlers = [];

	protected void RegisterHandler<T>(PacketId id, Action<T> handler) where T : BasePacket
	{
		if (_packetHandlers.ContainsKey(id)) { return; }

		_packetHandlers[id] = (basePacket) =>
		{
			if (basePacket is T specificPacket)
			{
				handler(specificPacket);
			}
		};
	}

	public void HandlePacket(BasePacket? packet)
	{
		if (packet == null) { return; }

		if (_packetHandlers.TryGetValue(packet.Id, out var handlerAction))
		{
			handlerAction.Invoke(packet);
		}
	}
}