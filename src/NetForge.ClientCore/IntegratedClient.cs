using System;
using NetForge.Shared.Network.Packet;

namespace NetForge.ClientCore;

public class IntegratedClient : BaseClient
{
	public event Action<BasePacket> PacketSentEvent = delegate { };

	public override void SendPacket(BasePacket packet)
	{
		PacketSentEvent.Invoke(packet);
	}
}