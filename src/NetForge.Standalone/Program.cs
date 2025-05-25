using System;
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
		server.AddListener(new TCPListener("127.0.0.1", 3115));
		server.Start();

		while (true)
		{
			string ?input = Console.ReadLine();
			if (input == null) { continue; }
			if (input == "c")
			{
				Console.WriteLine("Stopping Server...");
				server.Stop();
			}
		}
	}
}