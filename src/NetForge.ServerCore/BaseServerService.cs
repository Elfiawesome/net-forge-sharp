namespace NetForge.ServerCore;

public abstract class BaseServerService
{
	protected readonly Server server;
	public BaseServerService(Server server)
	{
		this.server = server;
	}
}