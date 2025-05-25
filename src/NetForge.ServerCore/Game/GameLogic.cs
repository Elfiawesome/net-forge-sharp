
using System;
using System.Collections.Generic;
using NetForge.ServerCore.Game.Instance;

namespace NetForge.ServerCore.Game;

public class GameLogic
{
	private readonly Dictionary<Guid, GameInstance> _gameInstances;

	public GameLogic()
	{
		_gameInstances = [];
	}
}