using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetForgeSharp.ServerCore;

public class BaseServerConnection
{
	public event Action<BaseServerConnection, BasePacket> PacketReceived = delegate { };
	public BaseServerConnection()
	{

	}

	public virtual void SendPacket(BasePacket packet) { }

	public void OnPacketReceived(BasePacket packet)
	{
		PacketReceived.Invoke(this, packet);
	}
}

public class TCPServerConnection : BaseServerConnection
{
	private readonly PacketStream _packetStream;
	private readonly Task _getPacketLoop;
	private readonly CancellationToken _cancellationToken;

	public TCPServerConnection(TcpClient tcpClient, CancellationToken token)
	{
		_cancellationToken = token;
		_packetStream = new PacketStream(tcpClient.GetStream());
		_getPacketLoop = GetPacketLoop();
	}

	public async Task GetPacketLoop()
	{
		while (!_cancellationToken.IsCancellationRequested)
		{
			var packet = await _packetStream.GetPacketAsync(_cancellationToken);
			if (packet == null){ continue; }
			OnPacketReceived(packet);
		}
	}

	public override void SendPacket(BasePacket packet)
	{
		_ = _packetStream.SendPacketAsync(packet, _cancellationToken);
	}
}