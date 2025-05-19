using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Shared;
using Shared.Network;

namespace Server.Connection;

public class TCPServerConnection : BaseServerConnection
{
	private readonly PacketStream _packetStream;
	private readonly TcpClient _tcpClient;
	private readonly Task _getPacketLoopTask; // dunno what to do w this task
	private readonly CancellationToken _cancellationToken;

	public TCPServerConnection(TcpClient tcpClient, CancellationToken token)
	{
		_cancellationToken = token;
		_tcpClient = tcpClient;
		_packetStream = new PacketStream(_tcpClient.GetStream());
		_getPacketLoopTask = GetPacketLoop();
	}

	public async Task GetPacketLoop()
	{
		while (!_cancellationToken.IsCancellationRequested && _tcpClient.Connected)
		{
			var packet = await _packetStream.GetPacketAsync(_cancellationToken);
			if (packet == null) { continue; }
			OnPacketReceived(packet);
		}
	}

	public override void SendPacket(BasePacket packet)
	{
		// TODO store these async tasks and await them if this connections shuts down
		_ = _packetStream.SendPacketAsync(packet, _cancellationToken);
	}

	~TCPServerConnection()
	{
		DebugLogger.Log("Connection is being GC'd");
	}

}