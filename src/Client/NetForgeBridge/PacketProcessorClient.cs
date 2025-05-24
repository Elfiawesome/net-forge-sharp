using System;
using System.Collections.Generic;
using Godot;
using Shared.Network;
using Shared.Network.Packets.Clientbound.Handshake;

namespace Client.NetForgeBridge;

public class PacketProcessorClient
{
	public delegate void HandlerSignature<TPacket>(ClientNode client, TPacket basePacket) where TPacket : BasePacket;
	private Dictionary<PacketId, Action<ClientNode, BasePacket>> _map = [];

	public PacketProcessorClient()
	{
		Register<S2CLoginRequestPacket>(PacketId.S2CLoginRequest, HandleS2CLoginRequest);
	}

	public void Register<TPacket>(PacketId packetId, HandlerSignature<TPacket> handler) where TPacket : BasePacket
	{
		_map[packetId] = (client, packet) =>
		{
			if (packet is TPacket tPacket)
			{
				handler.Invoke(client, tPacket);
			}
		};
	}

	public void ProcessPacket(ClientNode server, BasePacket packet)
	{
		_map[packet.Id].Invoke(server, packet);
	}

	public void HandleS2CLoginRequest(ClientNode client, S2CLoginRequestPacket packet)
	{
		client.SendData(new C2SLoginRsponsePacket("Elfiawesome23"));
	}
}