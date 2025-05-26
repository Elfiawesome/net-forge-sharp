using Godot;
using NetForge.ClientCore;
using NetForge.game_session;
using NetForge.ServerCore;
using NetForge.ServerCore.Network.Listener;

public partial class GameSession : Node
{
	public Client client;
	private Server IntegratedServer;
	public string Username;

	public override void _Ready()
	{
		// Initialize the integrated server and connect to it
		var GlobalNode = GetNode("/root/Global");

		int InstanceNum = (int)GlobalNode.Get("instance_num");
		if (InstanceNum == 0)
		{
			IntegratedServer = new();
			IntegratedServer.NetworkService.AddListener(new TCPListener("127.0.0.1", 3115));
			IntegratedServer.Start();
		}

		// Set our own GD version PacketProcessor
		Username = (string)GlobalNode.Get("username");
		client = new Client();
		client.PacketProcessor = new PacketProcessorClient(client, this);
		ConnectToServer("127.0.0.1", 3115);
	}

	public void ConnectToServer(string ipAddress, int port)
	{
		client.Connect(ipAddress, port);
	}

	public void LeaveServer()
	{
		client.Leave();
		IntegratedServer.Stop();
	}
}
