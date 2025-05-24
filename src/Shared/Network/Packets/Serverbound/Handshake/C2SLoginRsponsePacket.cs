using System;
using System.IO;

namespace Shared.Network.Packets.Clientbound.Handshake;

public class C2SLoginRsponsePacket : BasePacket
{
	public override PacketId Id => PacketId.C2SLoginRsponse;
	public string Username = string.Empty;

	public C2SLoginRsponsePacket() { }

	public C2SLoginRsponsePacket(string username)
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