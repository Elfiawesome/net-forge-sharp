using Godot;
using Server.Listener;
using Shared;

namespace Client.NetForgeBridge;

public partial class ServerNode : Node
{
	private readonly Server.Server server;

	public ServerNode()
	{
		// Can check if this does any GC thingy
		DebugLogger.Logged += (string message) => { GD.Print(message); };
		server = new Server.Server();
		server.AttachListener(
			new TCPListener("127.0.0.1", 3115)
		);
	}

	public override void _Ready()
	{
		base._Ready();
		server.Start();
	}

	public void Shutdown()
	{
		server.Stop();
	}
}
