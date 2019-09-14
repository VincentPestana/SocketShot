using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Newtonsoft.Json;

namespace SocketShot
{
    internal class StreamManager
    {
        public StreamManager()
        {
            bool capture = true;

            Stopwatch timer = new Stopwatch();
            var averageTime = 0l;

            var hubConnection = new HubConnection("http://localhost:51628/");
            var hubProxy = hubConnection.CreateHubProxy("StreamHub");
            hubConnection.Start().Wait();

            // Initialize
            Bitmap bitmap;
            Graphics graphics;
            var groupName = DateTime.Now.Ticks.ToString();
            

            bitmap = new Bitmap(100, 100);

            graphics = Graphics.FromImage(bitmap as Image);
            string b64Bitmap = "";

            while (capture)
            {
                timer.Restart();


                graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    b64Bitmap = Convert.ToBase64String(byteImage);
                }

                hubProxy.Invoke("sendScreen", "data:image/png;base64, " + b64Bitmap).Wait();

                timer.Stop();
                averageTime = (averageTime + timer.ElapsedMilliseconds) / 2;
                Console.WriteLine(averageTime + "ms");
            }
        }
    }
}