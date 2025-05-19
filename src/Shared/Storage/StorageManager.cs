namespace Shared.Storage;

public abstract class BaseStorageManager
{
	public abstract void OpenStorage(string saveName);
	public abstract void WriteFile(string category, string fileName);
	public abstract FileType LoadFile<FileType>(string category, string fileName);
}