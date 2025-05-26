using System;
using NetForge.ClientCore;
using NetForge.ServerCore;
using NetForge.ServerCore.Network.Listener;
using NetForge.Shared.Network.Packet;

namespace NetForge.Standalone;

public static class Program
{
	public static void Main(string[] args)
	{
		PacketFactory.Initialize();
		
		Server? server = new();
		server.NetworkService.AddListener(new TCPListener("127.0.0.1", 3115));
		server.Start();
		
		// Testing with a client connection
		Client client = new();
		client.Connect("127.0.0.1", 3115);

		while (true)
		{
			string? input = Console.ReadLine();
			if (input == null) { continue; }
			if (input == "c")
			{
				Console.WriteLine("Stopping Server...");
				server.Stop();
			}
		}
	}
}