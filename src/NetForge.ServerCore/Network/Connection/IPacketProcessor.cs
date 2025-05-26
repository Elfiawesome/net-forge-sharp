using NetForge.Shared.Network.Packet;

namespace NetForge.ServerCore.Network.Connection;

public interface IPacketProcessor
{
	void ProcessPacket(BasePacket packet);

	void OnConnectionLost();

	void OnForcefullyClosed(string reason);
}