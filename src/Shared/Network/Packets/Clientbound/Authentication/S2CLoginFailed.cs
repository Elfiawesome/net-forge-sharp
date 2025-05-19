using System.IO;

namespace Shared.Network.Packets.Clientbound.Authentication;

public class S2CLoginFailed : BasePacket
{
	public override PacketId Id => PacketId.S2CLoginFailed;
	public override void DeserializePayload(BinaryReader reader){}
	public override void SerializePayload(BinaryWriter writer){}
}
