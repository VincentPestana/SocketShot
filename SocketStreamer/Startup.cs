using Microsoft.AspNet.SignalR;
using Owin;

namespace SocketStreamer
{
	public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
			// Set max size to 128kb
			GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = 131072;
			// Set message queue size
			GlobalHost.Configuration.DefaultMessageBufferSize = 0;
            app.MapSignalR();

		}
    }
}