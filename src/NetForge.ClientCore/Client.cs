using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetForge.Shared.Debugging;
using NetForge.Shared.Network.Stream;

namespace NetForge.ClientCore;

public class Client
{
	private readonly TcpClient _tcpClient;
	private PacketStream ?_packetStream;
	private readonly CancellationTokenSource _clientCancellationTokenSource;
	private readonly CancellationToken _clientCancellationToken;
	private Task ?_listeningTask;

	public Client()
	{
		_clientCancellationTokenSource = new();
		_clientCancellationToken = _clientCancellationTokenSource.Token;

		_tcpClient = new();
	}

	public void Connect()
	{
		IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
		IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 3115);
		_tcpClient.Connect(ipEndPoint);
		_packetStream = new PacketStream(_tcpClient.GetStream());

		Logger.Log("Client connecting to server");
		_listeningTask = Listen();
	}

	private async Task Listen()
	{
		while (!_clientCancellationToken.IsCancellationRequested)
		{
			try
			{
				if (_packetStream == null) { continue; }
				var packet = await _packetStream.GetPacketAsync(_clientCancellationToken);
				Logger.Log($"Client received packet [packet]");
			}
			catch (Exception)
			{

			}

		}
		Logger.Log("Client ended listening");
	}

	public void Leave()
	{
		_clientCancellationTokenSource.Cancel();
	}
}
