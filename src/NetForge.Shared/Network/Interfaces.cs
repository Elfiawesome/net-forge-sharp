
using NetForge.Shared.Network.Packet;

namespace NetForge.Shared.Network;

public interface IConnection
{
	public bool HandlePacket<TPacket>(TPacket packet) where TPacket : BasePacket;
}