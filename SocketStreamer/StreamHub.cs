using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

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