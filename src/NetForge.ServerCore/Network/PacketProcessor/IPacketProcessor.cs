using NetForge.ServerCore.Network.Connection;
using NetForge.Shared.Network.Packet;

namespace NetForge.ServerCore.Network.PacketProcessor;

public interface IPacketProcessor
{
	void ProcessPacket(BaseConnection connection, BasePacket packet);

	void OnConnectionLost(string reason);
}