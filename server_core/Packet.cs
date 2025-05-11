
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace NetForge
{
	public enum PacketType : ushort
	{
		ExamplePacket = 1,
	}
	public abstract class BasePacket
    {
        public abstract PacketType PacketId { get; }
        public abstract void Serialize(MemoryStream stream);
        public abstract void Deserialize(MemoryStream stream);

		public static BasePacket? CreatePacket(PacketType packetType)
		{
			switch (packetType)
			{
				case PacketType.ExamplePacket: return new ExamplePacket();
			}
			return null;
		}
    }

	public class ExamplePacket : BasePacket
	{
        public override PacketType PacketId => PacketType.ExamplePacket;
		private string _welcomeMessage;
		
		public ExamplePacket(string welcomeMessage)
		{
			_welcomeMessage = welcomeMessage;
		}
		
		public ExamplePacket()
		{
			_welcomeMessage = string.Empty; // default
		}

		public override void Serialize(MemoryStream stream)
		{
			stream.Write(_welcomeMessage.ToUtf8Buffer());
		}

        public override void Deserialize(MemoryStream stream)
		{
			var buffer = new byte[stream.Length];
			stream.Read(buffer, 0, buffer.Length);
			_welcomeMessage = Encoding.UTF8.GetString(buffer);
		}

	}

	public class PacketStream
	{
		private NetworkStream _stream;

		const int PACKET_HEADER_SIZE = 4;
		const int PACKET_TYPE_SIZE = 2;
		const int MAX_PACKET_SIZE = 2 * 1024 * 1024;

		public PacketStream(NetworkStream stream)
		{
			_stream = stream;
		}

		public async Task<BasePacket?> GetPacketAsync(CancellationToken token)
		{
			// Get the packet type + payload size
			var lengthBuffer = new byte[PACKET_HEADER_SIZE];
			int bytesRead = await _stream.ReadAsync(lengthBuffer, 0, PACKET_HEADER_SIZE, token);
			if (bytesRead == 0) return null;
			int totalPacketSize = BitConverter.ToInt32(lengthBuffer, 0);
			
			if (totalPacketSize <= 0 || totalPacketSize > MAX_PACKET_SIZE)
			{
				return null;
			}

			// Get packet type
			var typeBuffer = new byte[PACKET_TYPE_SIZE];
			bytesRead = await _stream.ReadAsync(typeBuffer, 0, PACKET_TYPE_SIZE, token);
			if (bytesRead == 0) return null;

			PacketType packetType = (PacketType)BitConverter.ToUInt16(typeBuffer, 0);
			
			// Get Payload
			int payloadSize = totalPacketSize - PACKET_TYPE_SIZE;
			byte[] payloadBuffer = new byte[payloadSize];
			if (payloadSize > 0)
			{
				bytesRead = await _stream.ReadAsync(payloadBuffer, 0, payloadSize, token);
				if (bytesRead == 0) return null;
			}

			// Create the packet
			var packet = BasePacket.CreatePacket(packetType);
			if (packet == null) return null;
			packet.Deserialize(new MemoryStream(payloadBuffer));
			return packet;
		}

		public async Task SendPacketAsync(BasePacket packet, CancellationToken token)
		{
			using (var ms = new MemoryStream())
			{
				packet.Serialize(ms);
				var payloadBuffer = ms.ToArray();
				
				var packetSizeBuffer = BitConverter.GetBytes(payloadBuffer.Length + PACKET_TYPE_SIZE);
				var typeBuffer = BitConverter.GetBytes((int)packet.PacketId);

				await _stream.WriteAsync(packetSizeBuffer, 0, PACKET_HEADER_SIZE, token);
				await _stream.WriteAsync(typeBuffer, 0, PACKET_TYPE_SIZE, token);
				await _stream.WriteAsync(payloadBuffer, 0, payloadBuffer.Length, token);
				await _stream.FlushAsync(token);
			}
		}
	}
}