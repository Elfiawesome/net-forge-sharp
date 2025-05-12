namespace NetForge
{
	public interface IPacketHandler
	{

	}

	public interface IPacketHandlerServer : IPacketHandler
	{
		// Add server's handle packets here
		
	}

	public interface IPackerHandlerClient : IPacketHandler
	{
		// Add client's handle packets here
		
	}
}