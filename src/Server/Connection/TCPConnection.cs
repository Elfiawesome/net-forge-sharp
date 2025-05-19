using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Shared.Network;

namespace Server.Connection;

public class TCPServerConnection : BaseServerConnection
{
	private readonly PacketStream _packetStream;
	private readonly Task _getPacketLoopTask;
	private readonly CancellationToken _cancellationToken;

	public TCPServerConnection(TcpClient tcpClient, CancellationToken token)
	{
		_cancellationToken = token;
		_packetStream = new PacketStream(tcpClient.GetStream());
		_getPacketLoopTask = GetPacketLoop();
	}

	public async Task GetPacketLoop()
	{
		while (!_cancellationToken.IsCancellationRequested)
		{
			var packet = await _packetStream.GetPacketAsync(_cancellationToken);
			if (packet == null) { continue; }
			OnPacketReceived(packet);
		}
	}

	public override void SendPacket(BasePacket packet)
	{
		_ = _packetStream.SendPacketAsync(packet, _cancellationToken);
	}
}