using System.IO;

namespace Shared.Network.Packets.Serverbound;

public class C2STestPacket : BasePacket
{
	public override PacketId Id => PacketId.C2STest;
	public override void DeserializePayload(BinaryReader reader) { }
	public override void SerializePayload(BinaryWriter writer) { }
}