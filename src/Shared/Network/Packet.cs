using System;
using System.IO;
using Shared.Network.Packets.Clientbound.Handshake;
using Shared.Network.Packets.Clientbound.Root;
using Shared.Registry;

namespace Shared.Network;

public enum PacketId : ushort
{
	// Debug testing
	HelloWorld = 0,

	// Authentication (Serverbound)
	C2SLoginRsponse,

	// Authentication (Clientbound)
	S2CLoginRequest,
	S2CDisconnect,
}

public abstract class BasePacket
{
	public static SimpleRegistry<PacketId, Func<BasePacket>> REGISTRY = new();

	public static void Register()
	{
		REGISTRY.Register(PacketId.C2SLoginRsponse, () => new C2SLoginRsponsePacket());
		REGISTRY.Register(PacketId.S2CLoginRequest, () => new S2CLoginRequestPacket());
		REGISTRY.Register(PacketId.S2CDisconnect, () => new S2CDisconnectPacket());
	}


	public abstract PacketId Id { get; }
	public abstract void SerializePayload(BinaryWriter writer);
	public abstract void DeserializePayload(BinaryReader reader);
}