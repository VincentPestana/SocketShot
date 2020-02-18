using Microsoft.AspNet.SignalR.Client;
using System;

namespace SocketShot
{
	class Program
    {
        static void Main(string[] args)
        {
			// Setup SignalR
			Console.WriteLine("Enter SR url? Hit enter for default: http://localhost:51628/");
			var enteredUrl = Console.ReadLine();
			// Check entered url
			if (string.IsNullOrEmpty(enteredUrl))
				enteredUrl = "http://localhost:51628/";

			if (!enteredUrl.StartsWith("http://"))
				enteredUrl = "http://" + enteredUrl;

			var hubConnection = new HubConnection(enteredUrl);
			var hubProxy = hubConnection.CreateHubProxy("StreamHub");
			hubConnection.Start().Wait();

			Console.WriteLine("Connected to server - Starting stream");

			var manager = new StreamManager(hubProxy);
        }
    }
}
