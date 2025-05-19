using System;
using Shared.Network;
using Shared.Network.Packets.Clientbound.Authentication;

namespace Server.Connection;

public class BaseServerConnection
{
	public event Action<BaseServerConnection, BasePacket> PacketReceived = delegate { };
	public event Action<BaseServerConnection> Disconnected = delegate { };
	public BaseServerConnection()
	{

	}

	public virtual void SendPacket(BasePacket packet) { }

	public virtual void Close(string disconnectMessage = "Server closed this connection.")
	{
		SendPacket(new S2CDisconnect(disconnectMessage));
		OnDisconnected();
	}

	public void OnPacketReceived(BasePacket packet)
	{
		PacketReceived.Invoke(this, packet);
	}

	public void OnDisconnected()
	{
		Disconnected.Invoke(this);
	}
}