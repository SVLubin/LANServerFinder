using System;
using System.Threading;
using LANServerFinder;

namespace ServerExample
{
	class Program
	{
		static void Main()
		{
			Console.WriteLine("Server example started");

			const int recivePort = 6000;

			var responder = new Responder();
			responder.StartResponding(recivePort);

			responder.StopResponding();

			//Thread.Sleep(100);
			responder.StartResponding(recivePort);

			Console.WriteLine("Start waiting for request at port {0}...", recivePort);

			Console.ReadKey();
		}
	}
}
