using System.Collections.Generic;
using NetForge.ServerCore.Network.Connection;
using NetForge.Shared.Network.Packet;

namespace NetForge.ServerCore.Network.PacketHandler;

public class BasePacketHandler<TContext>
{
	public delegate void HandlerSignautre<TPacket>(TContext context, TPacket packet, BaseConnection connection) where TPacket : BasePacket;

	private readonly Dictionary<PacketId, HandlerSignautre<BasePacket>> _handlers = [];

	public void RegisterHandler<TPacket>(PacketId packetId, HandlerSignautre<TPacket> handler) where TPacket : BasePacket
	{
		if (_handlers.ContainsKey(packetId)) { return; }
		_handlers[packetId] = (TContext context, BasePacket packet, BaseConnection connection) =>
		{
			if (packet is TPacket tPacket)
			{
				handler.Invoke(context, tPacket, connection);
			}
		};
	}

	public void Handle(TContext context, BasePacket packet, BaseConnection connection)
	{
		_handlers[packet.Id].Invoke(context, packet, connection);
	}
}