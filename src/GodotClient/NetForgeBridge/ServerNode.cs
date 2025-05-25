using Godot;
using NetForge.ServerCore;
using NetForge.ServerCore.Network.Listener;
using NetForge.Shared.Debugging;
using System;

public partial class ServerNode : Node
{
	private readonly Server server;

	public ServerNode()
	{
		Logger.MessageLoggedEvent += (string message) => GD.Print(message);
		server = new();
		server.AddListener(new TCPListener("127.0.0.1", 3115));
	}

	public void Start()
	{
		server.Start();
	}

	public void Shutdown()
	{
		server.Stop();
	}

}
