using System;
using Godot;
using NetForge.ClientCore;
using NetForge.game_session;
using NetForge.ServerCore;
using NetForge.ServerCore.Network.Listener;
using NetForge.Shared.Debugging;

public partial class GameSession : Node
{
	public Client client;
	private Server IntegratedServer;

	public override void _Ready()
	{
		Logger.Log("[GameSession] GameSession Readied");
		// Initialize the integrated server and connect to it
		IntegratedServer = new();
		IntegratedServer.NetworkService.AddListener(new TCPListener("127.0.0.1", 3115));
		IntegratedServer.Start();

		// Set our own GD version PacketProcessor
		client = new Client();
		client.PacketProcessor = new PacketProcessorClient(client);
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
