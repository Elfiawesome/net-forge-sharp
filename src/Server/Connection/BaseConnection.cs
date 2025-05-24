using System;
using Shared.Network;
using Shared.Network.Packets.Clientbound.Root;

namespace Server.Connection;

public abstract class BaseConnection
{
	public event Action<BasePacket> PacketReceived = delegate { };
	public event Action Disconnected = delegate { };
	
	public virtual void SendPacket(BasePacket packet) { }

	public virtual void Close(string disconnectMessage = "Server closed this connection.")
	{
		SendPacket(new S2CDisconnectPacket(disconnectMessage));
		OnDisconnected();
	}

	public void OnPacketReceived(BasePacket packet)
	{
		PacketReceived(packet);
	}

	public void OnDisconnected()
	{
		Disconnected();
	}
}