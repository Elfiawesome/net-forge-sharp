using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace NetForge.Network
{
	public class PacketStream
	{
		private readonly NetworkStream _stream;
		// Change if i want to i guess lmao
		private static readonly Encoding _stringEncoding = Encoding.UTF8;

		// Size of the field that stores the total length of (Type + Payload).
        const int PACKET_LENGTH_FIELD_SIZE = sizeof(int); // 4
		// Size of the field that stores the packet type.
		const int PACKET_TYPE_FIELD_SIZE = sizeof(ushort); //2
		// Max size for (Type + Payload) 2MB
		const int MAX_PACKET_PAYLOAD_SIZE = 2 * 1024 * 1024;

		public PacketStream(NetworkStream stream)
		{
			_stream = stream;
		}

		public async Task<BasePacket?> GetPacketAsync(CancellationToken token)
		{
			try
			{
				//  Read the total length of (Type + Payload)
				var lengthBuffer = new byte[PACKET_LENGTH_FIELD_SIZE];
				int bytesRead = await ReadExactlyAsync(lengthBuffer, 0, PACKET_LENGTH_FIELD_SIZE, token); // NOTE:  Need to check if theres problems using readasync on fixed buffer size
				if (bytesRead == 0) {throw new Exception("Connection Closed on Field Size");}
				if (bytesRead < PACKET_LENGTH_FIELD_SIZE) {throw new Exception("Incomplete Packet Field Size");}
				int totalPacketSize = BitConverter.ToInt32(lengthBuffer, 0);
				
				if (totalPacketSize <= 0 || totalPacketSize > MAX_PACKET_PAYLOAD_SIZE)
				{
					throw new Exception($"Received a packet size of {totalPacketSize}. Max is {MAX_PACKET_PAYLOAD_SIZE}.");
				}
				
				// Read the entire (Type + Payload)
				byte[] typeAndPayloadBuffer = new byte[totalPacketSize];
				bytesRead = await ReadExactlyAsync(typeAndPayloadBuffer, 0, totalPacketSize, token);
				if (bytesRead == 0) {throw new Exception("Connection Closed on Packet");}
				if (bytesRead < totalPacketSize) {throw new Exception("Incomplete Packet on Packet");}
				using (var ms = new MemoryStream(typeAndPayloadBuffer))
				{
					using (var br = new BinaryReader(ms, _stringEncoding))
					{
						// Get Packet Type
						if (ms.Length < PACKET_TYPE_FIELD_SIZE)
						{
							throw new Exception("Packet size is less than type field size");
						}
						PacketType packetType = (PacketType)br.ReadUInt16();

						// Create Packet from type
						BasePacket? packet = PacketFactory.CreatePacket(packetType);
						if (packet == null)
						{
							throw new Exception("Packet does not exist in factory");
						}
						
						// Deserialize Packet with payload
						packet.DeserializePayload(br);
						if (ms.Position != ms.Length) { /* NOTE: The packet didnt read the whole reader. For now i'll ignore :/ */ }

						return packet;
					}
				}
			}
			catch (Exception ex)
			{
				GD.Print($"Encountered error getting packet: {ex.Message}");
				return null;
			}
		}

		private async Task<int> ReadExactlyAsync(byte[] buffer, int offset, int count, CancellationToken token)
		{
			int totalBytesRead = 0;
			while (totalBytesRead < count)
			{
				int bytesRead = await _stream.ReadAsync(buffer, offset + totalBytesRead, count - totalBytesRead, token);
				if (bytesRead == 0) // Stream closed prematurely
                {
                    return totalBytesRead; // Return what was read, could be 0
                }
				totalBytesRead += bytesRead;
			}
			return totalBytesRead;
		}

		public async Task SendPacketAsync(BasePacket packet, CancellationToken token)
		{
			using (var ms = new MemoryStream())
			{
				using (var bw = new BinaryWriter(ms, _stringEncoding, true))
				{
					// Packet Type
					bw.Write((ushort)packet.PacketId);
					// Packet Payload
					packet.SerializePayload(bw);
					bw.Flush();

					byte[] packetBuffer = ms.ToArray();
					if (packetBuffer.Length > MAX_PACKET_PAYLOAD_SIZE)
					{
						// Packet too big
						return;
					}

					// Get the length of (Type + Payload)
					var lengthBuffer = BitConverter.GetBytes(packetBuffer.Length);

					// Write to the network stream: Length, then (Type + Payload)
					await _stream.WriteAsync(lengthBuffer, 0, PACKET_LENGTH_FIELD_SIZE, token);
					await _stream.WriteAsync(packetBuffer, 0, packetBuffer.Length, token);
					await _stream.FlushAsync(token);
				}
			}
		}
	}
}