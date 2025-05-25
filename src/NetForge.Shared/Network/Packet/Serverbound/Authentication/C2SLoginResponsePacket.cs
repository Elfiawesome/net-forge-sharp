using System;
using System.IO;

namespace NetForge.Shared.Network.Packet.Serverbound.Authentication;

public class C2SLoginResponsePacket : BasePacket
{
	public override PacketId Id => PacketId.C2SLoginResponsePacket;
	public string Username = string.Empty;
	
	public C2SLoginResponsePacket() { }

	public C2SLoginResponsePacket(string username)
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