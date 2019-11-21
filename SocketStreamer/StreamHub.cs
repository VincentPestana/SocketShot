using Microsoft.AspNet.SignalR;

namespace SocketStreamer
{
	public class StreamHub : Hub
    {
        public void SendB64Screen(string jsonBitmap)
        {
            Clients.AllExcept(Context.ConnectionId).sendScreen(jsonBitmap);
        }
    }
}