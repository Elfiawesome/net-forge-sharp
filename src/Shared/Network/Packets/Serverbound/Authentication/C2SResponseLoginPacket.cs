using System.IO;

namespace Shared.Network.Packets.Serverbound.Authentication;

public class C2SResponseLoginPacket : BasePacket
{
	public override PacketId Id => PacketId.C2SResponseLoginPacket;
	public int ProtocolVersion = -1;
	public string Username = string.Empty;

	public C2SResponseLoginPacket() {}
	public C2SResponseLoginPacket(string username, int protocolVersion)
	{
		ProtocolVersion = protocolVersion;
		Username = username;
	}

	public override void DeserializePayload(BinaryReader reader)
	{
		ProtocolVersion = reader.ReadInt32();
		Username = reader.ReadString();
	}

	public override void SerializePayload(BinaryWriter writer)
	{
		writer.Write(ProtocolVersion);
		writer.Write(Username);
	}
}
