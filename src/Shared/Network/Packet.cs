using System;
using System.IO;
using Shared.Registry;

namespace Shared.Network;

public enum PacketId : ushort
{
	// Debug testing
	HelloWorld = 0,

	// Authentication (Serverbound)
	C2SResponseLoginPacket,

	// Authentication (Clientbound)
	S2CDisconnect,
}

public abstract class BasePacket
{
	public static SimpleRegistry<PacketId, Func<BasePacket>> REGISTRY = new();

	public static void Register()
	{
		// REGISTRY.Register(PacketId.ID, () => new PacketObject());
	}


	public abstract PacketId Id { get; }
	public abstract void SerializePayload(BinaryWriter writer);
	public abstract void DeserializePayload(BinaryReader reader);
}