using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace NetForge
{
	public class BaseServerConnection
	{
		public event Action<BaseServerConnection, BasePacket> PacketReceived = delegate {};
		public event Action<BaseServerConnection> Disconnected = delegate {};
		public readonly Guid ClientId = Guid.NewGuid();
		public virtual void Disconnect() {}

		protected virtual void OnPacketReceived(BasePacket packet)
		{
			PacketReceived.Invoke(this, packet);
		}

		protected virtual void OnDisconnected()
		{
			Disconnected.Invoke(this);
		}
	}

	public class TCPServerConnection : BaseServerConnection
	{
		private readonly TcpClient _tcpClient;
		private readonly CancellationTokenSource _cts;
		public TCPServerConnection(TcpClient tcpClient)
		{
			_cts = new CancellationTokenSource();
			_tcpClient = tcpClient;
			Task.Run(() => receiveDataTask(_cts.Token));
		}

		public override void Disconnect()
		{
			if (_cts.IsCancellationRequested) return;
			// Need to get manager to delete me
			OnDisconnected();
			// Stop all other processes
			_cts.Cancel();
			_tcpClient.Close();
		}

		private async Task receiveDataTask(CancellationToken token)
		{
			var packetStream = new PacketStream(_tcpClient.GetStream());
			while (!token.IsCancellationRequested && _tcpClient.Connected)
			{
				var packet = await packetStream.GetPacketAsync(token);
				if (packet != null)
				{
					OnPacketReceived(packet);
				}
				else
				{
					Disconnect();
					break;
				}
			}
			GD.Print($"Client connection closed");
		}
	}
}