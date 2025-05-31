using System.IO;

namespace NetForge.Shared.Network.Packet.Clientbound.Authentication;

public class S2CDisconnectPacket : BasePacket
{
	public override PacketId Id => PacketId.S2CDisconnectPacket;
	public string Reason = string.Empty;

	public S2CDisconnectPacket() { }

	public S2CDisconnectPacket(string reason)
	{
		Reason = reason;
	}


	public override void DeserializePayload(BinaryReader reader)
	{
		Reason = reader.ReadString();
	}

	public override void SerializePayload(BinaryWriter writer)
	{
		writer.Write(Reason);
	}
}