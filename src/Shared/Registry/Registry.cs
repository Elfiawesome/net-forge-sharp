using System.Collections.Generic;

namespace Shared.Registry;

public class SimpleRegistry<TKey, TValue> where TKey : notnull
{
	private readonly Dictionary<TKey, TValue> _map = [];

	public SimpleRegistry() { }

	public void Register(TKey key, TValue value)
	{
		// NOTE: This silently ignores registration if the key already exists.
		// It does NOT overwrite or throw an error.
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

	public bool ContainsKey(TKey key)
	{
		return _map.ContainsKey(key);
	}

	public int Count => _map.Count;
}
