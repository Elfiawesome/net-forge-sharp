using System;

namespace Server;

public static class DebugLogger
{
	public static event Action<string> Logged = delegate { };
	public static void Log(string message, object? obj = null)
	{
		if (obj != null)
		{
			Logged.Invoke($"[C# Logger][{obj}] - {message}");
		}
		else
		{
			Logged.Invoke($"[C# Logger] - {message}");
		}
	}
}