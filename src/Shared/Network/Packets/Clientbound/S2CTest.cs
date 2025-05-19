using System.IO;

namespace Shared.Network.Packets.Clientbound;

public class S2CTestPacket : BasePacket
{
	public override PacketId Id => PacketId.S2CTest;
	public override void DeserializePayload(BinaryReader reader) { }
	public override void SerializePayload(BinaryWriter writer) { }
}