using NetForge.ServerCore.Network.Connection;
using NetForge.ServerCore.Network.PacketHandler;
using NetForge.Shared.Debugging;
using NetForge.Shared.Network.Packet;
using NetForge.Shared.Network.Packet.Serverbound.Authentication;

namespace NetForge.ServerCore.Network;


// To take a connection and convert them into player obejcts for game logic
public class Handshake
{
	private readonly BasePacketHandler<Handshake> _packetHandler = new();

	public Handshake()
	{
		_packetHandler.RegisterHandler<C2SLoginResponsePacket>(PacketId.C2SLoginResponsePacket, HandleLoginResponse);
	}

	public void HandleNewConnection(BaseConnection connection)
	{
		connection.PacketReceivedEvent += (packet) =>
		{
			_packetHandler.Handle(this, packet, connection);
		};
	}

	private void HandleLoginResponse(Handshake context, C2SLoginResponsePacket packet, BaseConnection connection)
	{
		Logger.Log($"Server received login response of username {packet.Username}");
	}
}