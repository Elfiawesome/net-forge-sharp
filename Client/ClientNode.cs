using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using NetForgeSharp.ServerCore;

// Same with serverNode. Meant to act as a wrapper only
public partial class ClientNode : Node
{
	private readonly CancellationTokenSource _cancellationTokenSource;
	private readonly CancellationToken _cancellationToken;

	public ClientNode()
	{
		_cancellationTokenSource = new();
		_cancellationToken = _cancellationTokenSource.Token;
		_packetHandlerClient = new(this);
	}

	public override void _Ready()
	{
		base._Ready();
		_ = ConnectToServer();
	}

	private PacketStream? _packetStream;
	private readonly PacketHandlerClient _packetHandlerClient;
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
				GD.Print($"Client received packet: [{packet}]");
				packet.Handle(_packetHandlerClient);
			}
		}
	}

	public void SendData(BasePacket packet)
	{
		_packetStream?.SendPacketAsync(packet, _cancellationToken);
	}
}

public class PacketHandlerClient
{
	private readonly ClientNode _client;
	public PacketHandlerClient(ClientNode client)
	{
		_client = client;
	}

	public void HandleServerInitRequest(ServerInitRequest packet)
	{
		_client.SendData(new ServerInitRequest());
	}
}