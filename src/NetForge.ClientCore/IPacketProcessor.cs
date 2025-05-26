using NetForge.Shared.Network.Packet;

namespace NetForge.ClientCore;

public interface IPacketProcessor
{
	public void ProcessPacket(BasePacket packet);
}