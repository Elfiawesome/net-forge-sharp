using Godot;

namespace Client.NetForgeBridge;

public partial class ServerNode : Node
{
	private readonly Server.Server server;

	public ServerNode()
	{
		DebugLogger.Logged += (string message) => { GD.Print(message); };
		server = new Server.Server(); //Lol why can I just call Server.server without any 'using Server' here
		_ = server.StartListeningAsync();
	}

	public override void _Ready()
	{
		base._Ready();
	}
}
