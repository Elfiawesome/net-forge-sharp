using System.IO;

namespace NetForge.Shared.Network.Packet.Clientbound.Authentication;

public class S2CRequestLoginPacket : BasePacket
{
	public override PacketId Id => PacketId.S2CRequestLoginPacket;

	public override void DeserializePayload(BinaryReader reader)
	{
	}

	public override void SerializePayload(BinaryWriter writer)
	{
	}
}