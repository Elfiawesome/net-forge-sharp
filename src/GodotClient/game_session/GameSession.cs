using Godot;
using NetForge.ClientCore;
using NetForge.ServerCore;
using NetForge.ServerCore.Network.Listener;
using NetForge.Shared.Debugging;
using NetForge.Shared.Network.Packet;

public partial class GameSession : Node
{
	public Client? client;
	private Server? IntegratedServer;

	public override void _Ready()
	{
		Logger.MessageLoggedEvent += (message) => { GD.Print(message); };
		PacketFactory.Initialize();

		IntegratedServer = new Server();
		IntegratedServer.NetworkService.AddListener(new TCPListener("127.0.0.1", 3115));
		IntegratedServer.Start();

		client = new();
		client.Connect("127.0.0.1", 3115, "TestPlayer");
		client.PacketReceivedEvent += (packet) =>
		{
			GD.Print($"[Client] Received packet: {packet.GetType().Name}");
		};
	}
	
}
