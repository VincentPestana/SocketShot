using Microsoft.AspNet.SignalR;

namespace SocketStreamer
{
	public class StreamHub : Hub
    {
        public string SendScreen(string jsonBitmap)
        {
            Clients.AllExcept(Context.ConnectionId).sendScreen(jsonBitmap);
            return "";
        }
    }
}