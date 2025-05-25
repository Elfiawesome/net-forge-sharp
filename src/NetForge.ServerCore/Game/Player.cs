using NetForge.ServerCore.Network.Connection;

namespace NetForge.ServerCore.Game;

public class Player
{
	public readonly BaseConnection Connection;
	public readonly string Id;

	public Player(string playerId, BaseConnection connection)
	{
		Id = playerId;
		Connection = connection;
	}
}