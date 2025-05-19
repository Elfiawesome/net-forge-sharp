using Godot;
using NetForgeSharp.ServerCore;

// This is meant to be a wrapper for the server object so that gdscript can interact with c# server 
// without the c# server object itself being inherited by Node
public partial class ServerNode : Node
{
	private readonly Server _server = new Server();
	public override void _Ready()
	{
		base._Ready();
		_ = _server.StartListeningAsync();
	}
}
