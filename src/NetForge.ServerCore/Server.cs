using System.Collections.Generic;
using System.Threading;
using NetForge.ServerCore.GameCore;
using NetForge.ServerCore.Network;
using NetForge.ServerCore.Network.Connection;
using NetForge.ServerCore.Network.Listener;
using NetForge.Shared.Debugging;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;

namespace NetForge.ServerCore;

public class Server
{
	public readonly int ProtocolNumber = 1;
	private readonly CancellationTokenSource _serverCancellationTokenSource;
	public readonly CancellationToken _serverCancellationToken;
	private readonly PacketProcessorServer _packetProcessorServer;
	public readonly GameService GameService;
	public readonly NetworkService NetworkService;

	public Server()
	{
		// I've come to the conclusion that if an object is just and extension of it's parent, we can just have circular reference.
		_packetProcessorServer = new(this); // note circular reference here
		_serverCancellationTokenSource = new();
		_serverCancellationToken = _serverCancellationTokenSource.Token;

		// Services are meant to be parts of the server that I need to seperate out for better organization.
		NetworkService = new(this);
		GameService = new(this);
	}

	public void Start()
	{
		Logger.Log("[Server] Starting server...");
		// Start all listeners
		NetworkService.StartListeners();
	}

	public void Stop()
	{
		Logger.Log("[Server] Stopping server...");
		NetworkService.StopListeners();
		_serverCancellationTokenSource.Cancel();
	}

	public void OnNewConnection(BaseConnection connection)
	{
		connection.PacketProcessor = _packetProcessorServer;
		// Do Handshake
		connection.SendPacket(new S2CRequestLoginPacket());
		// The rest of the handshake will be handled by the PacketProcessorServer
	}
}
