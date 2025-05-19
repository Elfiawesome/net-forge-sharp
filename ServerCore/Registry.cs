using System.Collections.Generic;

namespace NetForgeSharp.ServerCore;

public class SimpleRegistry<TKey, TValue> where TKey : notnull
{
	private readonly Dictionary<TKey, TValue> _map = [];

	public SimpleRegistry()
	{

	}

	public void Register(TKey key, TValue value)
	{
		if (_map.ContainsKey(key))
		{
			return;
		}
		_map.Add(key, value);
	}

	public TValue? Get(TKey key)
	{
		if (_map.TryGetValue(key, out TValue? value))
		{
			return value;
		}
		return default; // This will throw a error btw cuz it means the key isnt in the registry
	}
}