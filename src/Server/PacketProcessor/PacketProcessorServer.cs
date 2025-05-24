using System;
using System.Collections.Generic;
using Server.Connection;
using Shared.Network;
using Shared.Network.Packets.Clientbound.Handshake;

namespace Server.PacketProcessor;

public class PacketProcessorServer
{
	public delegate void HandlerSignature<TPacket>(Server server, TPacket basePacket, BaseConnection baseConnection) where TPacket : BasePacket;
	private Dictionary<PacketId, Action<Server, BasePacket, BaseConnection>> _map = [];
	private PacketHandlerHandshake _handshake = new();

	public PacketProcessorServer()
	{
		Register<C2SLoginRsponsePacket>(PacketId.C2SLoginRsponse, _handshake.HandleC2SLoginRsponse);
	}

	public void Register<TPacket>(PacketId packetId, HandlerSignature<TPacket> handler) where TPacket : BasePacket
	{
		_map[packetId] = (server, packet, connection) =>
		{
			if (packet is TPacket tPacket)
			{
				handler.Invoke(server, tPacket, connection);
			}
		};
	}

	public void ProcessPacket(Server server, BasePacket packet, BaseConnection connection)
	{
		_map[packet.Id].Invoke(server, packet, connection);
	}
}