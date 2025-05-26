using NetForge.Shared.Network.Packet;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;

namespace NetForge.ServerCore.Network.Connection;

public abstract class BaseConnection
{
	protected bool isClosedSignaled = false;
	// private bool hasForcefullyClosed = false;

	public virtual void SendData(BasePacket packet)
	{
		// To be implemented by concrete connection classes
	}

	// Call this to INITIATE a close from server-side
	public virtual void InitiateClose(string disconnectReason)
	{
		if (isClosedSignaled) return;
		SendData(new S2CDisconnectPacket(disconnectReason));
		isClosedSignaled = true;
	}
}