
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetForge.Shared;
using NetForge.Shared.Debugging;
using NetForge.Shared.Network.Packet;

namespace NetForge.ServerCore.Game;

public class GameService
{
	private readonly CancellationTokenSource _CancellationTokenSource;
	private readonly CancellationToken _CancellationToken;
	private Task ?_processTask;
	private readonly Dictionary<PlayerId, Player> _players = [];

	public GameService(CancellationToken _parentCancellationToken)
	{
		_CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentCancellationToken);
		_CancellationToken = _CancellationTokenSource.Token;
	}


	public void Start()
	{
		Logger.Log("[GameService] Starting game service");
		_processTask = Process();
	}

	public void Stop()
	{
		Logger.Log("[GameService] Stopping game service");
		_CancellationTokenSource.Cancel();
	}

	private async Task Process()
	{
		Logger.Log("[GameService] Starting game service process...");
		while (!_CancellationToken.IsCancellationRequested)
		{
			try
			{
				await Task.Delay(1000);
				Logger.Log("[GameService] Ticked!");
				Logger.Log(_players.Count.ToString());
			}
			catch (Exception)
			{

			}
		}
		Logger.Log("[GameService] Ended game service process.");
	}

	public void OnPlayerJoined(PlayerId playerId)
	{
		if (_players.ContainsKey(playerId)) { return; }
		var player = new Player(playerId);
		_players.Add(playerId, player);
	}

	public void OnPlayerLeft(PlayerId playerId)
	{
		if (!_players.ContainsKey(playerId)) { return; }
		_players.Remove(playerId);
	}

	public void OnPlayerPacketReceived(PlayerId playerId, BasePacket packet)
	{
	}
}