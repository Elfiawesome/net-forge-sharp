using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetForge.Shared.Debugging;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;
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

		Logger.Log("[Client] Connecting to server");
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
				Logger.Log($"[Client] Received packet [{packet}]");
				if (packet is S2CDisconnectPacket disconnectPacket)
				{
					Logger.Log("  ->Disconnect reason: " + disconnectPacket.Reason);
					break; // Dont need to care if we received a null packet later or not
				}

				if (packet == null)
				{
					break;
				}
			}
			catch (Exception)
			{

			}

		}
		_tcpClient.Close();
		Logger.Log("[Client] Ended Listening");
	}

	public void Leave()
	{
		_clientCancellationTokenSource.Cancel();
	}
}
