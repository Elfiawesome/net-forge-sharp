using NetForge.ServerCore.Network.PacketProcessor;
using NetForge.Shared.Network.Packet;
using NetForge.Shared.Network.Packet.Clientbound.Authentication;

namespace NetForge.ServerCore.Network.Connection;

public abstract class BaseConnection
{
	public IPacketProcessor? PacketProcessor;
	protected bool isClosedSignaled = false;

	public virtual void SendPacket(BasePacket packet)
	{
		// To be implemented by concrete connection classes
	}

	// Call this to INITIATE a close from server-side
	public virtual void InitiateClose(string disconnectReason)
	{
		if (isClosedSignaled) return;
		SendPacket(new S2CDisconnectPacket(disconnectReason));
		PacketProcessor?.OnConnectionLost(disconnectReason);
		isClosedSignaled = true;
	}
}