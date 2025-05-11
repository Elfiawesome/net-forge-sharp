
using System;
using System.IO;

namespace NetForgeServerSharp
{
	public enum PacketType : ushort {
		// Initialization & Handshake
		SB_ClientInitRequest = 1, 	// Server -> Client: Request client init data
		CB_ClientInitResponse,		// Client -> Server: Response client init data
		CB_JoinAcknowledged,		// Server -> Client: Join success
		CB_JoinFailed,				// Server -> Client: Join failed

		// Generic & Utility
		CB_Ping,
		SB_Pong,
		CB_Disconnect,
	}

	public abstract class BasePacket
	{
		public abstract PacketType Type { get; }

		protected abstract void SerializePayload(BinaryWriter writer);
		protected abstract void DeserializePayload(BinaryReader reader);

		public byte[] ToBytes()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter payloadWriter = new BinaryWriter(new MemoryStream()))
				{
					// Serialize payload into bytes first
					SerializePayload(payloadWriter);
					byte[] payloadBytes = ((MemoryStream)payloadWriter.BaseStream).ToArray();

					using (BinaryWriter writer = new BinaryWriter(memoryStream))
                    {
						writer.Write(4 + sizeof(ushort) + payloadBytes.Length); // Write size of total packet + (the size itself 4 bytes)
                        writer.Write((ushort)Type); // Write type of packet
                        writer.Write(payloadBytes); // write the payload data itself
					}
				}
				return memoryStream.ToArray();
			}
		}

		public static BasePacket? CreateFromBytes(byte[] packetData)
		{
			if (packetData.Length < sizeof(ushort))
			{
				// Packet cant be smaller than the type itself
				return null;
			}

			using (MemoryStream memoryStream = new MemoryStream(packetData))
			{
				using (BinaryReader reader = new BinaryReader(memoryStream))
				{
					PacketType type = (PacketType)reader.ReadUInt16();
					BasePacket ?packet = InstantiatePacket(type);

					if (packet != null)
					{
						try
						{
							packet.DeserializePayload(reader);
							return packet;
						}
						catch (Exception ex)
						{
							Console.WriteLine($"Error deserializing packet: {ex.Message}");
							return null;
						}
					}
					else
					{
						Console.WriteLine($"Received Unkown Packet Type: {type}");
						return null;
					}
				}
			}
		}

		public static BasePacket? InstantiatePacket(PacketType type)
		{
			switch (type)
			{
				case PacketType.SB_ClientInitRequest: return new SB_ClientInitRequest();
				case PacketType.CB_ClientInitResponse: return new CB_ClientInitResponse();
				case PacketType.CB_JoinAcknowledged: return new CB_JoinAcknowledged();
				case PacketType.CB_JoinFailed: return new CB_JoinFailed();
				default: return null;
			}
		}
	}

	public class SB_ClientInitRequest : BasePacket
	{
		public override PacketType Type => PacketType.SB_ClientInitRequest;
		protected override void SerializePayload(BinaryWriter writer) { }
		protected override void DeserializePayload(BinaryReader reader) { }
	}

	public class CB_ClientInitResponse : BasePacket
	{
		public override PacketType Type => PacketType.SB_ClientInitRequest;
		protected override void SerializePayload(BinaryWriter writer) { }
		protected override void DeserializePayload(BinaryReader reader) { }
	}

	public class CB_JoinAcknowledged : BasePacket
	{
		public override PacketType Type => PacketType.SB_ClientInitRequest;
		protected override void SerializePayload(BinaryWriter writer) { }
		protected override void DeserializePayload(BinaryReader reader) { }
	}

	public class CB_JoinFailed : BasePacket
	{
		public override PacketType Type => PacketType.SB_ClientInitRequest;
		protected override void SerializePayload(BinaryWriter writer) { }
		protected override void DeserializePayload(BinaryReader reader) { }
	}
}