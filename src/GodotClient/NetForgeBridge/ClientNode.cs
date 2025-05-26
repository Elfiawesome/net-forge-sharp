using Godot;
using NetForge.ClientCore;
using System;

public partial class ClientNode : Node
{
	private readonly Client _client;

	public ClientNode()
	{
		_client = new();
	}

	public void ConnectToServer()
	{
		_client.Connect("127.0.0.1", 3115);
	}

	public void Leave()
	{
		_client.Leave();
	}
	
}
