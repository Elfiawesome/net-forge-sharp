
using System.Threading;
using NetForge.Shared;
using NetForge.Shared.Network.Packet;

namespace NetForge.ServerCore.Game;

public class GameService
{
	private readonly CancellationTokenSource _CancellationTokenSource;
	private readonly CancellationToken _CancellationToken;

	public GameService(CancellationToken _parentCancellationToken)
	{
		_CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentCancellationToken);
		_CancellationToken = _CancellationTokenSource.Token;
	}


	public void OnPlayerJoined(PlayerId playerId)
	{
	}

	public void OnPlayerLeft(PlayerId playerId)
	{
	}

	public void OnPlayerPacketReceived(PlayerId playerId, BasePacket packet)
	{
	}

}