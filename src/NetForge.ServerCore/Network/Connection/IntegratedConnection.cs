using System;
using NetForge.Shared.Debugging;
using NetForge.Shared.Network.Packet;

namespace NetForge.ServerCore.Network.Connection;

public class IntegratedConnection : BaseConnection
{
	public event Action<BasePacket> PacketSentEvent = delegate {};

	public override void SendPacket(BasePacket packet)
	{
		PacketSentEvent.Invoke(packet);
	}
}