using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetForge.Shared.Debugging;
using NetForge.Shared.Network.Packet;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;
using NetForge.Shared.Network.Packet.Serverbound.Authentication;
using NetForge.Shared.Network.Stream;

namespace NetForge.ClientCore;


// Wrapper to simplify networking
public class Client
{
	public event Action<BasePacket> PacketReceivedEvent = delegate { };

	public readonly int ProtocolNumber = 1;
	private readonly TcpClient _tcpClient;
	private PacketStream? _packetStream;
	private readonly CancellationTokenSource _clientCancellationTokenSource;
	private readonly CancellationToken _clientCancellationToken;
	private Task? _listeningTask;
	private bool _isAuthenticated = false;


	public Client()
	{
		_clientCancellationTokenSource = new();
		_clientCancellationToken = _clientCancellationTokenSource.Token;

		_tcpClient = new();
	}

	public void Connect(string ipAddressString, int port, string loginUsername = "DefaultUsername")
	{
		try
		{
			IPAddress ipAddress = IPAddress.Parse(ipAddressString);
			IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
			_tcpClient.Connect(ipEndPoint);
			_packetStream = new PacketStream(_tcpClient.GetStream());

			Logger.Log("[Client] Connecting to server");
			_listeningTask = Listen(loginUsername);
		}
		catch (Exception ex)
		{
			Logger.Log($"[Client] Error connecting to server: {ex.Message}");
			_clientCancellationTokenSource.Cancel();
			_tcpClient.Close();
			return;
		}
	}

	private async Task Listen(string loginUsername)
	{
		while (!_clientCancellationToken.IsCancellationRequested)
		{
			try
			{
				if (_packetStream == null) { continue; }
				var packet = await _packetStream.GetPacketAsync(_clientCancellationToken);
				if (packet == null)
				{
					break;
				}

				if (packet is S2CDisconnectPacket disconnectPacket)
				{
					Logger.Log($"[Client] Disconnected from server: {disconnectPacket.Reason}");
					break;
				}

				// Simple Authentication Process
				if (_isAuthenticated == false)
				{
					if (packet is S2CRequestLoginPacket)
					{
						SendPacket(new C2SLoginResponsePacket(ProtocolNumber, loginUsername));
					}
					if (packet is S2CLoginSuccessPacket)
					{
						_isAuthenticated = true;
						Logger.Log("[Client] Authentication successful");
					}
				}
				else
				{
					PacketReceivedEvent.Invoke(packet);
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

	public void SendPacket(BasePacket packet)
	{
		if (_packetStream == null) { return; }
		_ = _packetStream.SendPacketAsync(packet);
	}
}
