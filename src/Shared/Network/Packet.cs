using System;
using System.IO;
using Shared.Network.Packets.Clientbound;
using Shared.Network.Packets.Serverbound;
using Shared.Registry;

namespace Shared.Network;

public enum PacketId : ushort
{
	HelloWorld = 0,
	S2CTest,
	C2STest,

	
}

public abstract class BasePacket
{
	public static SimpleRegistry<PacketId, Func<BasePacket>> REGISTRY = new();

	public static void Register()
	{
		REGISTRY.Register(PacketId.S2CTest, () => new S2CTestPacket());
		REGISTRY.Register(PacketId.C2STest, () => new C2STestPacket());
	}

	public abstract PacketId Id { get; }
	public abstract void SerializePayload(BinaryWriter writer);
	public abstract void DeserializePayload(BinaryReader reader);
}