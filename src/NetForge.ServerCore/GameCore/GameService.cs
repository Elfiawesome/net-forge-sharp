
using System.Threading;

namespace NetForge.ServerCore.GameCore;

public class GameService
{
	private readonly CancellationTokenSource _CancellationTokenSource;
	private readonly CancellationToken _CancellationToken;

	public GameService(CancellationToken _parentCancellationToken)
	{
		_CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentCancellationToken);
		_CancellationToken = _CancellationTokenSource.Token;
	}
}