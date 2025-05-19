using System;
using System.Collections.Generic;

namespace Shared.Network;

public class PacketHandler<TContext>
{
	private readonly Dictionary<PacketId, Action<TContext?, BasePacket>> _packetHandlers = [];

	protected void RegisterHandler<TPacket>(PacketId id, Action<TPacket> handler) where TPacket : BasePacket
	{
		if (_packetHandlers.ContainsKey(id)) { return; }

		_packetHandlers[id] = (context, basePacket) =>
		{
			if (basePacket is TPacket specificPacket)
			{
				handler(specificPacket);
			}
		};
	}

	protected void RegisterHandler<TPacket>(PacketId id, Action<TContext, TPacket> handler) where TPacket : BasePacket
	{
		if (_packetHandlers.ContainsKey(id)) { return; }

		_packetHandlers[id] = (context, basePacket) =>
		{
			if (context != null)
			{
				if (basePacket is TPacket specificPacket)
				{
					handler(context, specificPacket);
				}
			}
		};
	}


	public void HandlePacket(BasePacket? packet, TContext? context = default)
	{
		if (packet == null) { return; }
		if (_packetHandlers.TryGetValue(packet.Id, out var handlerAction))
		{
			handlerAction.Invoke(context, packet);
		}
	}
}

