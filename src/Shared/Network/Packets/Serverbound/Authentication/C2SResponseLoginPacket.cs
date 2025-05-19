using System.IO;

namespace Shared.Network.Packets.Serverbound.Authentication;

public class C2SResponseLoginPacket : BasePacket
{
	public override PacketId Id => PacketId.C2SResponseLoginPacket;
	public string Username = string.Empty;

	public C2SResponseLoginPacket() {}
	public C2SResponseLoginPacket(string username)
	{
		Username = username;
	}

	public override void DeserializePayload(BinaryReader reader)
	{
		Username = reader.ReadString();
	}

	public override void SerializePayload(BinaryWriter writer)
	{
		writer.Write(Username);
	}
}
