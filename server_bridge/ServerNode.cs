
using Godot;

public partial class ServerNode : RefCounted
{
	private NetForge.Server server;
	
	public ServerNode()
	{
		server = new NetForge.Server();
	}

	public void Start()
	{
		server.Start();
	}

	public void Stop()
	{
		server.Stop();
	}

}