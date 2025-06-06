using System;
using NetForge.Shared.Network;

namespace NetForge.ClientCore;

public class IntegratedClient : BaseClient
{
	public IConnection ?ServerConnection;

	public override void SendPacket<TPacket>(TPacket packet)
	{
		ServerConnection?.HandlePacket(packet);
	}
}