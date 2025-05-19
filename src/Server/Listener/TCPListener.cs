using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Server.Connection;

namespace Server.Listener;

public class TCPListener : BaseListener
{
	private readonly TcpListener _listener;

	public TCPListener(string addressString, int portNumber)
	{
		var ipAddress = IPAddress.Parse(addressString);
		_listener = new(ipAddress, portNumber);
	}

	public override async Task StartListening(CancellationToken cancellationToken)
	{
		_listener.Start();
		while (!cancellationToken.IsCancellationRequested)
		{
			var tcpClient = await _listener.AcceptTcpClientAsync(cancellationToken);
			var tcpServerConnection = new TCPServerConnection(tcpClient, cancellationToken);
			OnConnectionConnected(tcpServerConnection);
		}
	}

	public override void StopListening()
	{
		_listener.Stop();
		_listener.Dispose();
	}
}