using System.IO;

namespace NetForge.Shared.Network.Packet;

public enum PacketId : ushort
{
	TestPacket,
	S2CRequestLoginPacket,
	C2SLoginResponsePacket,
}

public abstract class BasePacket
{
	public abstract PacketId Id { get; }
	public abstract void SerializePayload(BinaryWriter writer);
	public abstract void DeserializePayload(BinaryReader reader);
}