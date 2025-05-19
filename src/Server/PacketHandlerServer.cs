
using System;
using Server.Connection;
using Shared.Network;
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
		if (_server.Connections.ContainsKey(clientId))
		{
			connection.Close("This username is already connected in the game!");
			return;
		}
		if (_server.ProtocolVersion != packet.ProtocolVersion)
		{
			connection.Close($"Not same Protocol version. Server is on v{_server.ProtocolVersion} and client is on v{packet.ProtocolVersion}");
			return;
		}

		_server.OnConnectionConnected(connection, clientId);
	}
}