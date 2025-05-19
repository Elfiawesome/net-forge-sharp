using System;
using Shared.Network;

namespace Server.Connection;

public class BaseServerConnection
{
	public event Action<BaseServerConnection, BasePacket> PacketReceived = delegate { };
	public event Action<BaseServerConnection> Disconnected = delegate { };
	public BaseServerConnection()
	{

	}

	public virtual void SendPacket(BasePacket packet) { }

	public void OnPacketReceived(BasePacket packet)
	{
		PacketReceived.Invoke(this, packet);
	}

	public void OnDisconnected()
	{
		Disconnected.Invoke(this);
	}
}