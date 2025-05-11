using System;
using System.Dynamic;
using System.IO;
using System.Net.Sockets;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace NetForgeServerSharp
{
	public interface IConnection
	{
		public Guid ConnectionId { get; } // Connection stuff
		public Guid ClientId { get; set; } // Identifier as player
		public bool IsConnected { get; }

		public Task SendPacketAsync(BasePacket packet);
		public void StartReceiving(Action<IConnection, BasePacket> onPacketReceivedCallback, Action<IConnection> onDisconnectedCallback);
		public void Close(string reason = "Connection closed");
	}

	public class TCPConnection : IConnection
	{
		private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly CancellationTokenSource _connectionCts;
        private Task? _receiveTask;
        private Action<IConnection, BasePacket>? _onPacketReceived;
        private Action<IConnection>? _onDisconnected;

		public Guid ConnectionId { get; }
        public Guid ClientId { get; set; }
        // What does this mean bruh...
		public bool IsConnected => _tcpClient.Connected && _connectionCts?.IsCancellationRequested == false;

		private const int MAX_PACKET_SIZE = 1024 * 1024 * 2; // 2 MB max packet size

		public TCPConnection(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();
            _connectionCts = new CancellationTokenSource();
            ConnectionId = Guid.NewGuid(); // Unique internal ID for this TCP connection instance
        }

		public async Task SendPacketAsync(BasePacket packet)
		{
			if (!IsConnected) return;
			try
			{
				byte[] data = packet.ToBytes(); 
                await _stream.WriteAsync(data, 0, data.Length, _connectionCts.Token);
                await _stream.FlushAsync(_connectionCts.Token);
			}
			catch (ObjectDisposedException) 
			{
				Close("Stream or client was disposed during send."); 
			}
            catch (IOException ex)
			{
                Console.WriteLine($"[TCPConnection {ConnectionId}] IO Error sending packet: {ex.Message}");
                Close($"IO Error during send: {ex.Message}");
            }
            catch (Exception ex)
			{
                Console.WriteLine($"[TCPConnection {ConnectionId}] Unexpected error sending packet: {ex.Message}");
                Close($"Unexpected error during send: {ex.Message}");
            }
		}

		public void StartReceiving(Action<IConnection, BasePacket> onPacketReceivedCallback, Action<IConnection> onDisconnectedCallback)
		{
			_onPacketReceived = onPacketReceivedCallback;
			_onDisconnected = onDisconnectedCallback;
			_receiveTask = Task.Run(() => ReceiveLoopAsync(_connectionCts.Token));
		}

		private async Task ReceiveLoopAsync(CancellationToken token)
		{
			byte[] lengthBuffer = new byte[4]; // buffer to read packet size
			try
			{
				// Get Packet size
				int totalBytesRead = 0;
				while (!token.IsCancellationRequested && IsConnected)
				{
					int bytesRead = await _stream.ReadAsync(lengthBuffer, totalBytesRead, lengthBuffer.Length - totalBytesRead, token);
					if (bytesRead == 0) throw new IOException("Connection closed by remote while reading packet length.");
					totalBytesRead += bytesRead;
				}
				int packetFullLength = BitConverter.ToInt32(lengthBuffer, 0);
				if (packetFullLength <= 0 || packetFullLength > MAX_PACKET_SIZE)
				{
					throw new IOException($"Invalid packet length received: {packetFullLength}. Max allowed: {MAX_PACKET_SIZE}");
				}

				// Get Packet type and content
				int packetBodyLength = packetFullLength - 4;
				if (packetBodyLength < sizeof(ushort)) // Minimum body is PacketType (ushort)
				{
						throw new IOException($"Packet body length ({packetBodyLength}) is too small.");
				}
				byte[] packetBodyBuffer = new byte[packetBodyLength]; // buffer to read packet content
				
				totalBytesRead = 0;
				while (totalBytesRead < packetBodyLength)
				{
					int bytesRead = await _stream.ReadAsync(packetBodyBuffer, totalBytesRead, packetBodyLength - totalBytesRead, token);
					if (bytesRead == 0) throw new IOException("Connection closed by remote while reading packet body.");
					totalBytesRead += bytesRead;
				}

				// Deserialize Packet
				BasePacket? packet = BasePacket.CreateFromBytes(packetBodyBuffer);
				if (packet != null)
				{
					_onPacketReceived?.Invoke(this, packet);
				}
			}
			catch (Exception ex)
			{
				// Aint no way im trying to write all the exceptions
				Console.WriteLine($"Error receiving packet: {ex.Message}");
				Close($"Error receiving packet: {ex.Message}");
			}
		}

		public void Close(string reason = "Connection closed")
		{
			if (_connectionCts.IsCancellationRequested){ return; }

			Console.WriteLine($"[TCPConnection {ConnectionId} Client: {ClientId}] Closing. Reason: {reason}");
            _connectionCts.Cancel(); 

			try
            {
                _tcpClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TCPConnection {ConnectionId}] Error during TcpClient.Close(): {ex.Message}");
            }
            
            // Signal disconnect to server
			_onDisconnected?.Invoke(this);
		}
		
	}


	// Do later...
	public class IntegratedConnection
	{

	}
}