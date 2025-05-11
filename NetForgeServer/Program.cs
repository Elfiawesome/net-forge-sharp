using System;
using System.Threading.Tasks;

namespace NetForgeServerSharp
{
	public class Program
	{
		private static Server ?_server;

		static void Main(string[] args)
		{
			Console.WriteLine("Starting NetForgeServer...");
			_server = new Server();
			_server.Start();


			Console.CancelKeyPress += async (sender, e) =>
            {
                Console.WriteLine("Ctrl+C detected. Initiating server shutdown...");
                e.Cancel = true;
                await _server.Stop();
				_server = null;
			};

			while (true) {
				if (_server == null) {
					break;
				}
			}
		}
	}
}