using System.IO;

namespace Shared.Network.Packets.Clientbound.Authentication;

public class S2CDisconnect : BasePacket
{
	public override PacketId Id => PacketId.S2CDisconnect;
	public string DisconnectReason = string.Empty;

	public S2CDisconnect() {}

	public S2CDisconnect(string disconnectReason)
	{
		DisconnectReason = disconnectReason;
	}

	public override void DeserializePayload(BinaryReader reader)
	{
		DisconnectReason = reader.ReadString();
	}

	public override void SerializePayload(BinaryWriter writer)
	{
		writer.Write(DisconnectReason);
	}
}
