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
		Logger.Log("TCP Connection process started!");
		IsConnected = true;
		while (!_connectionCancellationToken.IsCancellationRequested)
		{
			try
			{
				BasePacket? packet = await _packetStream.GetPacketAsync(_connectionCancellationToken);
			}
			catch (Exception)
			{

			}
		}
		Logger.Log("TCP Connection process stopped");
		IsConnected = false;
		
		ForcefullyClose();
	}

	public override void SendData(BasePacket packet)
	{
		_ = _packetStream.SendPacketAsync(packet, _connectionCancellationToken);
	}

	public override void ForcefullyClose(string disconnectReason = "Disconnected from server for unkown reason.")
	{
		Logger.Log("TCP Connection Forcefully closed for reason: " + disconnectReason);
		base.ForcefullyClose(disconnectReason);
		_connectionCancellationTokenSource.Cancel();
	}
}