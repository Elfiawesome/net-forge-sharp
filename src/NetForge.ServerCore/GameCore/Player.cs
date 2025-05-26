namespace NetForge.ServerCore.GameCore;

// The game representation of a player in the game. It will hold all global game player data and not much networking
public class Player
{
	public readonly string Id;
	public string Username { get; }
	public string DisplayName => Username;

	public Player(string id, string username)
	{
		Id = id;
		Username = username;
	}
}