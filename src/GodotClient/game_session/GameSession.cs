using Godot;
using NetForge.ClientCore;
using NetForge.ServerCore;
using NetForge.ServerCore.Network.Connection;
using NetForge.ServerCore.Network.Listener;
using NetForge.Shared.Network;
using NetForge.Shared.Network.Packet;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;

public partial class GameSession : Node
{
	public BaseClient? client;
	private Server? IntegratedServer;
	private PacketHandler<int> _packetHandler = new();


	public override void _Ready()
	{
		var shutdownButtonNode = GetNode<Button>("CanvasLayer/Control/MarginContainer/VBoxContainer/ShutdownButton");
		shutdownButtonNode.Pressed += OnServerShutdownButtonPressed;
		var disconnectbuttonNode = GetNode<Button>("CanvasLayer/Control/MarginContainer/VBoxContainer/DisconnectButton");
		disconnectbuttonNode.Pressed += () => client?.Leave();

		var globalNode = GetTree().Root.GetNode<Global>("/root/Global");

		// Register packet handlers
		_packetHandler.Register<S2CLoginSuccessPacket>(PacketId.S2CLoginSuccessPacket, OnS2CLoginSuccessPacket);


		if (globalNode.InstanceNumber == 0)
		{
			var islocalNetwork = true;

			// Start server
			IntegratedServer = new Server();
			IntegratedServer.NetworkService.AddListener(new TCPListener("127.0.0.1", 3115));
			IntegratedServer.Start();


			if (islocalNetwork)
			{
				var integratedConnection = new IntegratedConnection();
				var integratedClient = new IntegratedClient();
				integratedConnection.clientConnection = integratedClient;
				integratedClient.serverConnection = integratedConnection;


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
		}
		else
		{
			client = new TCPClient();
			client.Connect("127.0.0.1", 3115, globalNode.Username);
		}

		client.PacketReceivedEvent += _packetHandler.HandlePacket;
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


	private void OnS2CLoginSuccessPacket(S2CLoginSuccessPacket packet)
	{
		GD.Print($"WOW WE ARE CONNECTED LESGOOOO with username {packet.PlayerId}");
	}
}
