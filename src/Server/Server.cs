using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Shared.Network;
using Shared.Network.Packets.Clientbound;
using Server.Connection;

namespace Server;

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

		_packetHandlerServer = new(this);		
		// Bootstrap any registries we need
		BasePacket.Register();
	}

	public async Task StartListeningAsync()
	{
		var listener = new TcpListener(IPAddress.Any, 3115);
		listener.Start();
		while (!_cancellationToken.IsCancellationRequested)
		{
			var tcpClient = await listener.AcceptTcpClientAsync(_cancellationToken);
			var tcpServerConnection = new TCPServerConnection(tcpClient, _cancellationToken);

			tcpServerConnection.SendPacket(new S2CTestPacket());

			tcpServerConnection.PacketReceived += OnConnectionPacketReceived;
		}
	}

	public void OnConnectionPacketReceived(BaseServerConnection connection, BasePacket packet)
	{
		_packetHandlerServer.HandlePacket(packet, connection);
	}
}