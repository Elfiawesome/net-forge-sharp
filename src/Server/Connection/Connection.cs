using System;
using Shared.Network;

namespace Server.Connection;

public class BaseServerConnection
{
	public event Action<BaseServerConnection, BasePacket> PacketReceived = delegate { };
	public BaseServerConnection()
	{

	}

	public virtual void SendPacket(BasePacket packet) { }

	public void OnPacketReceived(BasePacket packet)
	{
		PacketReceived.Invoke(this, packet);
	}
}