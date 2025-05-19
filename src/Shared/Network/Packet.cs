using System;
using System.IO;
using Shared.Network.Packets.Clientbound.Authentication;
using Shared.Network.Packets.Serverbound.Authentication;
using Shared.Registry;

namespace Shared.Network;

public enum PacketId : ushort
{
	HelloWorld = 0,

	// Authentication (Serverbound)
	C2SResponseLoginPacket,

	// Authentication (Clientbound)
	S2CDisconnect,
	S2CLoginSuccess, // Probably won't be using
	S2CLoginFailed, // Probably won't be using
	S2CRequestLoginPacket,
}

public abstract class BasePacket
{
	public static SimpleRegistry<PacketId, Func<BasePacket>> REGISTRY = new();

	public static void Register()
	{
		REGISTRY.Register(PacketId.C2SResponseLoginPacket, () => new C2SResponseLoginPacket());
		REGISTRY.Register(PacketId.S2CLoginFailed, () => new S2CLoginFailed());
		REGISTRY.Register(PacketId.S2CLoginSuccess, () => new S2CLoginSuccess());
		REGISTRY.Register(PacketId.S2CRequestLoginPacket, () => new S2CRequestLoginPacket());
	}


	public abstract PacketId Id { get; }
	public abstract void SerializePayload(BinaryWriter writer);
	public abstract void DeserializePayload(BinaryReader reader);
}