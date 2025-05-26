using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetForge.Shared.Debugging;
using NetForge.Shared.Network.Packet;
using NetForge.Shared.Network.Stream;

namespace NetForge.ServerCore.Network.Connection;

public class TCPConnection : BaseConnection
{
	private readonly TcpClient _tcpClient;
	private readonly PacketStream _packetStream;
	public bool IsConnected;

	private readonly Task _processTask;
	private readonly CancellationTokenSource _connectionCancellationTokenSource;
	private readonly CancellationToken _connectionCancellationToken;

	public TCPConnection(TcpClient tcpClient, CancellationToken serverCancellationToken)
	{
		_tcpClient = tcpClient;
		_packetStream = new(_tcpClient.GetStream());

		_connectionCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(serverCancellationToken);
		_connectionCancellationToken = _connectionCancellationTokenSource.Token;

		_processTask = Process();
	}

	private async Task Process()
	{
		Logger.Log($"[Server] [TCP Connection] New connection form - {_tcpClient.Client.RemoteEndPoint}");
		IsConnected = true;
		while (!_connectionCancellationToken.IsCancellationRequested && _tcpClient.Connected)
		{
			try
			{
				BasePacket? packet = await _packetStream.GetPacketAsync(_connectionCancellationToken);
				if (packet != null)
				{
					// Process Packet
				}
				else
				{
					break;
				}
			}
			catch (Exception ex)
			{
				Logger.Log($"[Server] [TCP Connection] Error while getting packet {ex.Message}");
			}
		}
		Logger.Log($"[Server] [TCP Connection] stopped - {_tcpClient.Client.RemoteEndPoint}");
		IsConnected = false;

		// This will also call PacketProcessor.OnConnectionLost
		// This will also mostly not matter if we already called ForcefullyClose already. It is only 
		// here in the case that the while loop stops (ie when server cancellation token or tcp stops abrubtly)
		ForcefullyClose("Connection unexpectedly ended :("); 
		
		_tcpClient.Close();
	}

	public override void SendData(BasePacket packet)
	{
		_ = _packetStream.SendPacketAsync(packet);
	}

	public override void ForcefullyClose(string disconnectReason = "Disconnected from server for unkown reason.")
	{
		base.ForcefullyClose(disconnectReason);
		_connectionCancellationTokenSource.Cancel();
	}
}