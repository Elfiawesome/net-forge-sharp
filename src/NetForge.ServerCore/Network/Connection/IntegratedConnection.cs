using NetForge.Shared.Network;

namespace NetForge.ServerCore.Network.Connection;

public class IntegratedConnection : BaseConnection
{
	public IConnection ?ClientConnection;


	public override void SendPacket<TPacket>(TPacket packet)
	{
		ClientConnection?.HandlePacket(packet);
	}
}