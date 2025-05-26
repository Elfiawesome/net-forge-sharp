using Godot;

namespace NetForge;

public partial class Bootstrapper : Node
{
	public static void Initialize()
	{
		// Initialize the packet factory
		NetForge.Shared.Network.Packet.PacketFactory.Initialize();

		// Register global logger
		NetForge.Shared.Debugging.Logger.MessageLoggedEvent += (string message) => GD.Print(message);
	}
}