﻿using Microsoft.AspNet.SignalR;

namespace SocketStreamer
{
	public class StreamHub : Hub
    {
        public void SendB64Screen(string jsonBitmap)
        {
            Clients.AllExcept(Context.ConnectionId).sendB64Screen(jsonBitmap);
        }

        public void SendScreenDetailed(object jsonImageObject)
        {
            Clients.AllExcept(Context.ConnectionId).sendScreenDetailed(jsonImageObject);
        }
    }
}