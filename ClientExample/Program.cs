using System;
using LANServerFinder;

namespace ClientExample
{
	class Program
	{
		static void Main()
		{
			Console.WriteLine("Client example started");

			const int serverPort = 6000;
			const int recivePort = 7000;

			var finder = new Finder();
			finder.StartFindingServer(
				serverPort,
				recivePort,
				(ip, message) => Console.WriteLine("Server found at {0}, he says \"{1}\".\nWork done!", ip, message));

			Console.WriteLine("Trying to find server in LAN at port {0}...", serverPort);

			Console.ReadKey();
		}
	}
}
