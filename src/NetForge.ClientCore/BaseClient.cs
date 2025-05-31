using System;
using NetForge.Shared.Network.Packet;

namespace NetForge.ClientCore;

public class BaseClient
{
	public event Action<BasePacket> PacketReceivedEvent = delegate { };
	public readonly int ProtocolNumber = 1;
	protected bool _isAuthenticated = false;

	public virtual void Connect(string ipAddressString, int port, string loginUsername = "DefaultUsername")
	{

	}

	public virtual void Leave()
	{

	}

	public virtual void SendPacket(BasePacket packet)
	{

	}

	public void OnPacketReceived(BasePacket packet)
	{
		PacketReceivedEvent?.Invoke(packet);
	}

}