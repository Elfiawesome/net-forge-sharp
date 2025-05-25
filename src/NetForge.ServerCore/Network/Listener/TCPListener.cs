
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetForge.ServerCore.Network.Connection;
using NetForge.Shared.Debugging;

namespace NetForge.ServerCore.Network.Listener;

public class TCPListener : BaseListener
{
	private readonly TcpListener _tcpListener;

	public TCPListener(string addressString, int portNumber)
	{
		var iPAddress = IPAddress.Parse(addressString);
		_tcpListener = new(iPAddress, portNumber);
		_tcpListener.Start();
	}

	public override async Task Listen(CancellationToken serverCancellationToken)
	{
		Logger.Log("[Server] TCP Listener started listening");
		while (!serverCancellationToken.IsCancellationRequested)
		{
			try
			{
				TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync(serverCancellationToken);
				var tcpConnection = new TCPConnection(tcpClient, serverCancellationToken);
				OnNewConnection(tcpConnection);
			}
			catch (Exception)
			{

			}
		}
		Logger.Log("[Server] TCP Listener ended listening");
	}
}