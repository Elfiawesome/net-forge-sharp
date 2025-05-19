
using System;
using Server.Connection;
using Shared;
using Shared.Network;
using Shared.Network.Packets.Clientbound.Authentication;
using Shared.Network.Packets.Serverbound.Authentication;

namespace Server;

public class PacketHandlerServer : PacketHandler<BaseServerConnection>
{
	private readonly Server _server;

	public PacketHandlerServer(Server server)
	{
		_server = server;

		RegisterHandler<C2SResponseLoginPacket>(PacketId.C2SResponseLoginPacket, HandleC2SResponseLoginPacket);
	}

	private void HandleC2SResponseLoginPacket(BaseServerConnection connection, C2SResponseLoginPacket packet)
	{
		var clientId = Guid.NewGuid();
		if (!_server.Connections.ContainsKey(clientId))
		{
			_server.OnConnectionConnected(connection, clientId);
		}
		else
		{
			connection.SendPacket(new S2CDisconnect("This username is already connected in the game!"));
		}
	}
}