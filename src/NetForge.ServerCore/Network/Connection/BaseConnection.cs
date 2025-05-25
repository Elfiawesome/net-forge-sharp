using System;

namespace NetForge.ServerCore.Network.Connection;

public abstract class BaseConnection
{
	public event Action DisconnectedEvent = delegate { };
	private bool hasForcefullyClosed = false;
	public void OnDisconnected()
	{
		DisconnectedEvent();
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