using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace NetForgeSharp.ServerCore;

public class Server
{
	private readonly CancellationTokenSource _cancellationTokenSource;
	private readonly CancellationToken _cancellationToken;
	private readonly PacketHandlerServer _packetHandlerServer;

	public Server()
	{
		// This cancellation token will be the only one used throughout the entire program
		_cancellationTokenSource = new();
		_cancellationToken = _cancellationTokenSource.Token;
		// Bootstrap any registries we need
		BasePacket.Register();

		_packetHandlerServer = new(this);
	}

	public async Task StartListeningAsync()
	{
		TcpListener listener = new TcpListener(IPAddress.Any, 3115);
		listener.Start();
		while (!_cancellationToken.IsCancellationRequested)
		{
			var tcpClient = await listener.AcceptTcpClientAsync(_cancellationToken);
			var tcpServerConnection = new TCPServerConnection(tcpClient, _cancellationToken);
			tcpServerConnection.SendPacket(new ServerInitRequest());
			tcpServerConnection.PacketReceived += OnConnectionPacketReceived;
			GD.Print("New Client!");
		}
	}

	public void OnConnectionPacketReceived(BaseServerConnection connection, BasePacket packet)
	{
		GD.Print($"Server received packet: [{packet}]");
		packet.Handle(connection, _packetHandlerServer);
	}
}

public class PacketHandlerServer
{
	private readonly Server _server;

	public PacketHandlerServer(Server server)
	{
		_server = server;
	}

	public void HandleClientInitResponse(BaseServerConnection connection, BasePacket packet)
	{

	}
}
