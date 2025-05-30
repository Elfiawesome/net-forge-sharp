using System.IO;

namespace NetForge.Shared.Network.Packet.Clientbound.Authentication;

public class S2CLoginSuccessPacket : BasePacket
{
	public override PacketId Id => PacketId.S2CLoginSuccessPacket;

	public PlayerId PlayerId = new("null");

	public S2CLoginSuccessPacket()
	{
	}

	public S2CLoginSuccessPacket(PlayerId playerId)
	{
		PlayerId = playerId;
	}

	public override void DeserializePayload(BinaryReader reader)
	{
		PlayerId = new PlayerId(reader.ReadString());
	}

	public override void SerializePayload(BinaryWriter writer)
	{
		writer.Write(PlayerId.ToString());
	}
}