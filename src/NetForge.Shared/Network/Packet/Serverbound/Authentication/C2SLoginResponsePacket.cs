using System.IO;

namespace NetForge.Shared.Network.Packet.Serverbound.Authentication;

public class C2SLoginResponsePacket : BasePacket
{
	public override PacketId Id => PacketId.C2SLoginResponsePacket;
	public int ProtocolNumber = -1;
	public string Username = string.Empty;
	
	public C2SLoginResponsePacket() { }

	public C2SLoginResponsePacket(int protocolNumber, string username)
	{
		Username = username;
		ProtocolNumber = protocolNumber;
	}

	public override void DeserializePayload(BinaryReader reader)
	{
		ProtocolNumber = reader.ReadInt32();
		Username = reader.ReadString();
	}

	public override void SerializePayload(BinaryWriter writer)
	{
		writer.Write(ProtocolNumber);
		writer.Write(Username);
	}
}