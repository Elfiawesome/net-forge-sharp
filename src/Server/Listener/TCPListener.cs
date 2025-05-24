using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Server.Connection;

namespace Server.Listener;

public class TCPListener : BaseListener
{
	private readonly TcpListener _tcpListener;

	public TCPListener(string addressString, int portNumber)
	{
		var ipAddress = IPAddress.Parse(addressString);
		_tcpListener = new(ipAddress, portNumber);
	}

	public async override Task StartListening(CancellationToken serverCancellationToken)
	{
		_tcpListener.Start();
		while (!serverCancellationToken.IsCancellationRequested)
		{
			var tcpClient = await _tcpListener.AcceptTcpClientAsync(serverCancellationToken);
			var tcpServerConnection = new TCPConnection(tcpClient, serverCancellationToken);
			OnConnectionConnected(tcpServerConnection);
		}
	}

	public override void StopListening()
	{
		// Can't do anything because the cancelation request is made thru the server cancelation token
	}
}