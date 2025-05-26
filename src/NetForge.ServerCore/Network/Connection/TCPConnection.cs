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
		string disconnectReason = "Connection ended normally by client or stream."; // Default reason
		try
		{
			while (!_connectionCancellationToken.IsCancellationRequested && _tcpClient.Connected)
			{
				BasePacket? packet = await _packetStream.GetPacketAsync(_connectionCancellationToken);
				if (packet != null)
				{
					// Process Packet
					PacketProcessor?.ProcessPacket(this, packet);
				}
				else
				{
					disconnectReason = "Client disconnected gracefully (null packet).";
					break;
				}
			}
		}
		catch (Exception ex)
		{
			disconnectReason = $"Server error in reading packet: {ex.Message}";
			Logger.Log($"[Server] [TCP Connection] Error while getting packet {ex.Message}");
		}
		finally
		{
			// Initiate close. But this will basically do nothing if we already closed from outside
			Logger.Log($"[Server] [TCP Connection] stopped - {_tcpClient.Client.RemoteEndPoint}");
			InitiateClose(disconnectReason);
			Cleanup();
		}
	}

	public override void SendPacket(BasePacket packet)
	{
		_ = _packetStream.SendPacketAsync(packet);
	}


	// Call this to INITIATE a close from server-side
	public override void InitiateClose(string disconnectReason)
	{
		base.InitiateClose(disconnectReason);
		_connectionCancellationTokenSource.Cancel();
	}

	// WILL run whenever the connection is closed either by server or not.
	private void Cleanup()
	{
		IsConnected = false;
		try
		{
			_tcpClient.Close();
		}
		catch (Exception ex)
		{
			Logger.Log($"[Server] [TCP Connection] Error closing TcpClient: {ex.Message}");
		}
		_connectionCancellationTokenSource.Dispose();
	}
}