
using System;
using System.Collections.Generic;

namespace NetForge.ServerCore.Game;

public class GameLogic
{
	private readonly Dictionary<Guid, Spaces> _spaces;

	public GameLogic()
	{
		_spaces = [];
	}
}