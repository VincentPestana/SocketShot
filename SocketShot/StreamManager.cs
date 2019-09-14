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
            
            bitmap = new Bitmap(1000, 1000, PixelFormat.Format24bppRgb);

            graphics = Graphics.FromImage(bitmap as Image);
            string b64Bitmap = "";

			// Setup encoder
			ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
			EncoderParameters encoderParameters = new EncoderParameters(1);
			// Quality options
			var qualityEncoder = Encoder.Quality;
			EncoderParameter qualityEncoderParameter = new EncoderParameter(qualityEncoder, 10L);
			encoderParameters.Param[0] = qualityEncoderParameter;
			while (capture)
			{
				timer.Restart();

				graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

				using (var ms = new MemoryStream())
				{
					//bitmap.Save(ms, ImageFormat.Png); // Too large
					bitmap.Save(ms, jpgEncoder, encoderParameters);


					byte[] byteImage = ms.ToArray();
					b64Bitmap = Convert.ToBase64String(byteImage);
				}

				bool sendFailed;
				try
				{
					hubProxy.Invoke("sendScreen", "data:image/png;base64, " + b64Bitmap).Wait();
					sendFailed = false;
				}
				catch (Exception e)
				{
					// Image could not be sent
					sendFailed = true;
				}

				timer.Stop();
				averageTime = (averageTime + timer.ElapsedMilliseconds) / 2;

				Console.WriteLine(averageTime + "ms" + " Size:" + b64Bitmap.Length / 1024 + "Kb - " + ((sendFailed) ? "failed" : ""));
			}
		}

		private ImageCodecInfo GetEncoder(ImageFormat format)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

			foreach (ImageCodecInfo codec in codecs)
			{
				if (codec.FormatID == format.Guid)
				{
					return codec;
				}
			}

			return null;
		}
	}
}