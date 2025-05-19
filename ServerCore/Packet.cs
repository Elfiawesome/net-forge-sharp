using System;
using System.Data;
using System.IO;

namespace NetForgeSharp.ServerCore;

public enum PacketId : ushort
{
	HelloWorldMessage = 0,
	ServerInitRequest,
	ClientInitResponse,
}

public abstract class BasePacket
{
	public static SimpleRegistry<PacketId, Func<BasePacket>> REGISTRY = new();

	public static void Register()
	{
		REGISTRY.Register(PacketId.HelloWorldMessage, () => new HelloWorldMessage());
		REGISTRY.Register(PacketId.ServerInitRequest, () => new ServerInitRequest());
		REGISTRY.Register(PacketId.ClientInitResponse, () => new ClientInitResponse());
	}

	public abstract PacketId Id { get; }
	public abstract void SerializePayload(BinaryWriter writer);
	public abstract void DeserializePayload(BinaryReader reader);
	public virtual void Handle(BaseServerConnection connection, PacketHandlerServer handler) {}
	public virtual void Handle(PacketHandlerClient handler) {}
}

public class HelloWorldMessage : BasePacket
{
	public override PacketId Id => PacketId.HelloWorldMessage;
	public string TextMessage = string.Empty;
	public HelloWorldMessage() { }
	public HelloWorldMessage(string textMessage) { TextMessage = textMessage; }

	public override void SerializePayload(BinaryWriter writer)
	{
		writer.Write(TextMessage);
	}
	public override void DeserializePayload(BinaryReader reader)
	{
		TextMessage = reader.ReadString();
	}
}

public class ServerInitRequest : BasePacket
{
	public override PacketId Id => PacketId.ServerInitRequest;
	public override void DeserializePayload(BinaryReader reader) { }
	public override void SerializePayload(BinaryWriter writer) { }
	public override void Handle(PacketHandlerClient handler) { handler.HandleServerInitRequest(this); }
}

public class ClientInitResponse : BasePacket
{
	public override PacketId Id => PacketId.ClientInitResponse;
	public override void DeserializePayload(BinaryReader reader) { }
	public override void SerializePayload(BinaryWriter writer) { }
	public override void Handle(BaseServerConnection connection, PacketHandlerServer handler) { handler.HandleClientInitResponse(connection, this); }
}