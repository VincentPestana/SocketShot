using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketShot
{
    class Program
    {
        static void Main(string[] args)
        {
			// Setup SignalR
			//TODO : Ask user for url
			var hubConnection = new HubConnection("http://localhost:51628/");
			var hubProxy = hubConnection.CreateHubProxy("StreamHub");
			hubConnection.Start().Wait();

			var manager = new StreamManager(hubProxy);
        }
    }
}
