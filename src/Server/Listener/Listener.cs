using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Connection;

namespace Server.Listener;

public abstract class BaseListener
{
	public event Action<BaseServerConnection> ConnectionAccepted = delegate { };

	public abstract Task StartListening(CancellationToken cancellationToken);
	public abstract void StopListening();
	public void OnConnectionConnected(BaseServerConnection connection)
	{
		ConnectionAccepted.Invoke(connection);
	}
}