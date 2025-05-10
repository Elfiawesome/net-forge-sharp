using System;
using System.Threading.Tasks;

namespace NetForgeServerSharp
{
	public class Program
	{
		static async Task Main(string[] args)
		{
			Server server = new Server();
			server.Start();

			Console.CancelKeyPress += new ConsoleCancelEventHandler(async (object ?sender, ConsoleCancelEventArgs args) => {
				await server.Stop();
				// This doesnt await btw, it just kills the entire program immediately
				Console.WriteLine("Stopping");
			});
			
			// To hold the program running
			while (true)
			{
				string ?res = Console.ReadLine();
				if (res == "q")
				{
					await server.Stop();
				}
			}

		}
	}
}