using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Shared;
using Shared.Network;

namespace Server.Connection;

public class TCPConnection : BaseConnection
{
	private readonly TcpClient _tcpClient;
	private readonly PacketStream _packetStream;
	// private readonly List<Task> sendPacketTasks = new();

	private readonly CancellationTokenSource _localCancellationTokenSource;
	private readonly CancellationToken _localCancellationToken;

	private readonly Task _receivingPacketTask;
	private bool hasClosed = false;

	public TCPConnection(TcpClient tcpClient, CancellationToken token)
	{
		_localCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
		_localCancellationToken = _localCancellationTokenSource.Token;

		_tcpClient = tcpClient;
		_packetStream = new(tcpClient.GetStream());

		_receivingPacketTask = ReceivePacket();
	}

	private async Task ReceivePacket()
	{
		while (!_localCancellationToken.IsCancellationRequested)
		{
			var packet = await _packetStream.GetPacketAsync(_localCancellationToken);
			if (packet == null) { break; }
			OnPacketReceived(packet);
		}
		// If connections terminates prematurely
		if (!hasClosed)
		{
			Close("Connection terminated prematurely.");
		}

		// Check that all sendPackets have been done
		_tcpClient.Close();
	}

	public override void SendPacket(BasePacket packet)
	{
		base.SendPacket(packet);
		var sendTask = _packetStream.SendPacketAsync(packet, _localCancellationToken);
	}

	public override void Close(string disconnectMessage = "Server closed this connection.")
	{
		base.Close(disconnectMessage);
		_localCancellationTokenSource.Cancel();
		hasClosed = true;
	}
}