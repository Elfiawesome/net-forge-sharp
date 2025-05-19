using System;
using Client.NetForgeBridge;
using Godot;
using Shared.Network;
using Shared.Network.Packets.Clientbound.Authentication;
using Shared.Network.Packets.Serverbound.Authentication;

namespace Client;

public class PacketHandlerClient : PacketHandler<object>
{
	private readonly ClientNode _client;
	public PacketHandlerClient(ClientNode clientNode)
	{
		_client = clientNode;

		RegisterHandler<S2CRequestLoginPacket>(PacketId.S2CRequestLoginPacket, HandleS2CRequestLoginPacket);
	}

	private void HandleS2CRequestLoginPacket(S2CRequestLoginPacket packet)
	{
		_client.SendData(new C2SResponseLoginPacket("Elfiawesome23"));
	}
}