using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using NetForge;

public partial class ClientNode : RefCounted
{
	[Signal]
	public delegate void ReceivedDataEventHandler();
	
	TcpClient ?tcpClient;
	CancellationTokenSource _cts;
	public ClientNode()
	{
		_cts = new CancellationTokenSource();
	}

	public void ConnectToServer()
	{
		try
		{
			tcpClient = new TcpClient();
			IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
			IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 3115);
			Task.Run(() => {
				tcpClient.ConnectAsync(ipEndPoint, _cts.Token);
			});
		}
		catch (Exception ex)
		{
			GD.Print(ex.Message);
		}
	}

	public void LeaveServer()
	{
		if (tcpClient == null) { return; }
		tcpClient.Close();
		_cts.Cancel();
	}

	public void SendData(string data)
	{
		if (tcpClient == null) { return; }
		var packetStream = new PacketStream(tcpClient.GetStream());
		_ = packetStream.SendPacketAsync(new ExamplePacket(data), _cts.Token);
	}
}