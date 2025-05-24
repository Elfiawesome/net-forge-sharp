using Godot;
using Shared.Network;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client.NetForgeBridge;

public partial class ClientNode : Node
{
	private readonly CancellationTokenSource _cancellationTokenSource;
	private readonly CancellationToken _cancellationToken;
	private PacketStream? _packetStream;
	public ClientNode()
	{
		_cancellationTokenSource = new();
		_cancellationToken = _cancellationTokenSource.Token;
	}

	public override void _Ready()
	{
		base._Ready();
		_ = ConnectToServer();
	}

	public async Task ConnectToServer()
	{
		var tcpClient = new TcpClient();
		IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
		IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 3115);

		await tcpClient.ConnectAsync(ipEndPoint, _cancellationToken);

		_packetStream = new PacketStream(tcpClient.GetStream());

		while (true)
		{
			var packet = await _packetStream.GetPacketAsync(_cancellationToken);

			if (packet != null)
			{
			}
		}
	}

	public void SendData(BasePacket packet)
	{
		_packetStream?.SendPacketAsync(packet, _cancellationToken);
	}
}
