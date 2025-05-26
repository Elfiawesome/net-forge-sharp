using System.IO;

namespace NetForge.Shared.Network.Packet.Clientbound.Authentication;

public class S2CLoginSuccessPacket : BasePacket
{
	public override PacketId Id => PacketId.S2CLoginSuccessPacket;

	public override void DeserializePayload(BinaryReader reader)
	{
	}

	public override void SerializePayload(BinaryWriter writer)
	{
	}
}