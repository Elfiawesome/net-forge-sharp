using System.IO;

namespace Shared.Network.Packets.Clientbound.Handshake;

public class S2CLoginRequestPacket : BasePacket
{
	public override PacketId Id => PacketId.S2CLoginRequest;

	public override void DeserializePayload(BinaryReader reader)
	{
	}

	public override void SerializePayload(BinaryWriter writer)
	{
	}
}