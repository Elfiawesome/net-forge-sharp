using System.IO;

namespace Shared.Network.Packets.Clientbound.Root;

public class S2CDisconnectPacket : BasePacket
{
	public override PacketId Id => PacketId.S2CDisconnect;
	public string DisconnectMessage = string.Empty;

	public S2CDisconnectPacket() {}

	public S2CDisconnectPacket(string disconnectMessage)
	{
		DisconnectMessage = disconnectMessage;
	}

	public override void DeserializePayload(BinaryReader reader)
	{
		DisconnectMessage = reader.ReadString();
	}

	public override void SerializePayload(BinaryWriter writer)
	{
		writer.Write(DisconnectMessage);
	}
}