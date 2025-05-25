using System;
using NetForge.Shared.Network.Packet;

namespace NetForge.ServerCore.Network.Connection;

public abstract class BaseConnection
{
	public event Action<BasePacket> PacketReceivedEvent = delegate { };
	public event Action DisconnectedEvent = delegate { };
	private bool hasForcefullyClosed = false;

	public void OnDisconnected()
	{
		DisconnectedEvent();
	}

	public void OnPacketReceived(BasePacket packet)
	{
		PacketReceivedEvent(packet);
	}

	public virtual void SendData(BasePacket packet)
	{
		
	}

	public virtual void ForcefullyClose(string disconnectReason = "Disconnected from server for unkown reason.")
	{
		if (!hasForcefullyClosed)
		{
			// SendData reason of closure 'disconnectReason'
			OnDisconnected();
			hasForcefullyClosed = true;
		}
	}
}