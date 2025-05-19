
using Shared.Network;
using Shared.Network.Packets.Serverbound;

namespace Server;

public class PacketHandlerServer : PacketHandler
{
	public PacketHandlerServer()
	{
		// Add server context here
		RegisterHandler<C2STestPacket>(PacketId.C2STest, HandleC2STest);
	}

	private void HandleC2STest(C2STestPacket packet)
	{

	}
}