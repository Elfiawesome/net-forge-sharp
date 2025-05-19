using Godot;
using Server;

namespace Client.NetForgeBridge;

public partial class ServerNode : Node
{
	private readonly Server.Server server;

	public ServerNode()
	{
		DebugLogger.Logged += (string message) => { GD.Print(message); };
		server = new Server.Server(); //Lol why can I just call Server.server without any 'using Server' here EDIT: lol its cuz the namespace and the object is the same name, so we need to specify what exactly is the object
		_ = server.StartListeningAsync();
	}

	public override void _Ready()
	{
		base._Ready();
	}
}
