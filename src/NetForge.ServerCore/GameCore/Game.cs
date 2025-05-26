using System;
using System.Collections.Generic;
using NetForge.Shared.Network.Packet;

namespace NetForge.ServerCore.GameCore;

public class GameService : BaseServerService
{
	private readonly Dictionary<string, Player> _players = new();

	public GameService(Server server) : base(server)
	{
	}

	public void OnPlayerJoinedGame(string playerId)
	{
		if (_players.ContainsKey(playerId)) { return; } // Shouldnt happen since OnPlayerJoiendGame only happens if we dont have an existing player
		_players[playerId] = new Player(playerId, playerId);
	}

	public void OnPlayerLeftGame(string playerId)
	{
		if (_players.ContainsKey(playerId))
		{
			_players.Remove(playerId);
		}
	}

	public bool IsPlayerInGame(string playerId)
	{
		return _players.ContainsKey(playerId);
	}
}