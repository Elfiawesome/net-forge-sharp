using Godot;
using Server.Listener;
using Shared;

namespace Client.NetForgeBridge;

public partial class ServerNode : Node
{
	private readonly Server.Server server;

	public ServerNode()
	{
		DebugLogger.Logged += (string message) => { GD.Print(message); };
		server = new Server.Server();
		server.AttachListener(new TCPListener("127.0.0.1", 3115));	
	}

	public override void _Ready()
	{
		base._Ready();
	}
}
