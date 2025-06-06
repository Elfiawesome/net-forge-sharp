using System.Data.Common;
using Godot;
using MessagePack;
using NetForge.ClientCore;
using NetForge.ServerCore;
using NetForge.ServerCore.Network.Connection;
using NetForge.ServerCore.Network.Listener;
using NetForge.Shared.Debugging;
using NetForge.Shared.Network.Packet;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;

public partial class GameSession : Node
{
	public BaseClient? client;
	private Server? IntegratedServer;

	public override void _Ready()
	{
		var buttonNode = GetNode<Button>("CanvasLayer/Control/Button");
		buttonNode.Pressed += OnServerShutdownButtonPressed;
		var globalNode = GetTree().Root.GetNode<Global>("/root/Global");

		if (globalNode.InstanceNumber == 0)
		{
			// Connect 2 integrated systems together
			var integratedConnection = new IntegratedConnection();
			var integratedClient = new IntegratedClient();
			integratedConnection.ClientConnection = integratedClient;
			integratedClient.ServerConnection = integratedConnection;

			// Start server
			IntegratedServer = new Server();
			IntegratedServer.NetworkService.AddListener(new TCPListener("127.0.0.1", 3115));
			IntegratedServer.Start();

			// Manually add the integrated connection to the server
			integratedClient.Connect("", 0, globalNode.Username); // To set the username basically
			IntegratedServer.NetworkService.ManualAddNewConnection(integratedConnection);

			client = integratedClient;
		}
		else
		{
			client = new TCPClient();
			client.Connect("127.0.0.1", 3115, globalNode.Username);
		}



		client.PacketReceivedEvent += (packet) =>
		{
			GD.Print($"[Client] Received packet: {packet.GetType().Name}");
		};
	}

	public override void _Process(double delta)
	{
		if (IntegratedServer != null)
		{
			var label = GetNode<Label>("CanvasLayer/Control/Label");
			label.Text = "Hello from Integrated Server!\n";
			foreach (var player in IntegratedServer.GameService.Players.Values)
			{
				label.Text += $"Player: {player.Id}\n";
			}
		}
	}

	private void OnServerShutdownButtonPressed()
	{
		GD.Print("[GameSession] Server shutdown button pressed.");
		IntegratedServer?.Stop();
	}
	
}
