
using Server.Connection;
using Shared.Network;

namespace Server;

public class PacketHandlerServer : PacketHandler<BaseServerConnection>
{
	private readonly Server _server;

	public PacketHandlerServer(Server server)
	{
		// insert Server context
		_server = server;

		// Register serverbound packets
	}
}