using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using NetForge.Network;

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
			Task.Run(() => ReceiveDataTask(_cts.Token));
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

	public async Task ReceiveDataTask(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			if (tcpClient == null) { continue; }
			GD.Print($"Waiting for packets....");
			var packetStream = new PacketStream(tcpClient.GetStream());		
			var packet = await packetStream.GetPacketAsync(token);
			GD.Print($"Received packet from Server! {packet}");
		}		
	}

	public void SendData(string data)
	{
		if (tcpClient == null) { return; }
		var packetStream = new PacketStream(tcpClient.GetStream());// TODO: Ineffecient new stream everytime...
		_ = packetStream.SendPacketAsync(new HelloWorldPacket("This is a very cool message 👍"), _cts.Token);
	}
}