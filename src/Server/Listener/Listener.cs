using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Connection;

namespace Server.Listener;

public abstract class BaseListener
{
	public event Action<BaseServerConnection> ConnectionAccepted = delegate { };

	public abstract Task StartListening(CancellationToken cancellationToken);
	// TODO Stop listening needs to be async function and waits for listening to be finished before completing
	public abstract void StopListening();
	public void OnConnectionConnected(BaseServerConnection connection)
	{
		ConnectionAccepted.Invoke(connection);
	}
}