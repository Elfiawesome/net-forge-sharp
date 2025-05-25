using NetForge.Shared.Network.Packet;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;

namespace NetForge.ServerCore.Network.Connection;

public abstract class BaseConnection
{
	private bool hasForcefullyClosed = false;

	public virtual void SendData(BasePacket packet)
	{
		// To be implemented by concrete connection classes
	}

	public virtual void ForcefullyClose(string disconnectReason = "Disconnected from server for unkown reason.")
	{
		if (!hasForcefullyClosed)
		{
			SendData(new S2CDisconnectPacket(disconnectReason));
			hasForcefullyClosed = true;
		}
	}
}