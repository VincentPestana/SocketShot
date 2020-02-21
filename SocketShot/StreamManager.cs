﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNet.SignalR.Client;

namespace SocketShot
{
	internal class StreamManager
    {
		// Initial starting quality setting
		private long _qualitySetting = 20L;

		private int _desiredSizePerShotKB = 80;

		// Image resize
		//	1920 × 1080 
		//	1280 × 720 
		//	960 × 540 
		//	720 × 480 
		//	640 × 360
		private int _streamImageWidth = 960;
		private int _streamImageHeight = 540;

		private IHubProxy _hubProxy;

		bool _capture = true;

		StreamMeta _streamDetailed;

		public StreamManager(IHubProxy hubProxy)
		{
			_hubProxy = hubProxy;
			StreamBase64();
		}

        public void StreamBase64()
        {
			// TODO: Have multiple timers, timing each piece of the capture+stream

            Stopwatch timer = new Stopwatch();
            var averageTime = 0L;

            // Initialize
            Bitmap bitmap;
            Graphics graphics;
            string b64Bitmap = "";

			_streamDetailed = new StreamMeta()
			{
				CaptureWidth = 1920,
				CaptureHeight = 1080,
				BitmapEncodeQuality = _qualitySetting.ToString()
			};

			// Capture 1920x1080 pixels
			bitmap = new Bitmap(1920, 1080, PixelFormat.Format24bppRgb);
            graphics = Graphics.FromImage(bitmap as Image);
			
			// Setup encoder
			ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
			EncoderParameters encoderParameters = new EncoderParameters(1);

			// Quality options
			var qualityEncoder = Encoder.Quality;
			EncoderParameter qualityEncoderParameter = new EncoderParameter(qualityEncoder, _qualitySetting);
			encoderParameters.Param[0] = qualityEncoderParameter;
			
			// Capture to infinity
			while (_capture)
			{
				timer.Restart();

				graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

				// Resize image
				var resizedBitmap = ResizeBitmap(bitmap, _streamImageWidth, _streamImageHeight);

				using (var ms = new MemoryStream())
				{
					resizedBitmap.Save(ms, jpgEncoder, encoderParameters);
					
					// Bitmap image to array
					byte[] byteImage = ms.ToArray();

					// Convert to Base64
					b64Bitmap = Convert.ToBase64String(byteImage);

					ms.Dispose();
				}
				
				// Send image over SignalR
				bool sendFailed;
				try
				{
					_hubProxy.Invoke("sendB64Screen", "data:image/png;base64, " + b64Bitmap).Wait();
					sendFailed = false;
				}
				catch (Exception e)
				{
					// Image could not be sent
					sendFailed = true;
				}

				if (sendFailed)
				{
					// Sending failed, decrease quality all the way
					_qualitySetting = 0L;
				}
				else
				{
					// Dynamic Quality adjustment
					var sizeSent = b64Bitmap.Length / 1024;
					if (sizeSent > (_desiredSizePerShotKB + 5))
						_qualitySetting -= 2L;
					else if (sizeSent < (_desiredSizePerShotKB - 5))
						_qualitySetting += 2L;
				}

				qualityEncoderParameter = new EncoderParameter(qualityEncoder, _qualitySetting);
				encoderParameters.Param[0] = qualityEncoderParameter;

				timer.Stop();
				averageTime = (averageTime + timer.ElapsedMilliseconds) / 2;

				Console.WriteLine("FPS:Qual:Size - " + (1000 / averageTime) + " : " + _qualitySetting + " : " + b64Bitmap.Length / 1024 + "Kb" + ((sendFailed) ? " - failed" : ""));
			}
		}

		private Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
		{
			Bitmap result = new Bitmap(width, height);
			using (Graphics g = Graphics.FromImage(result))
			{
				g.DrawImage(bmp, 0, 0, width, height);
			}

			return result;
		}

		private ImageCodecInfo GetEncoder(ImageFormat format)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

			foreach (ImageCodecInfo codec in codecs)
			{
				if (codec.FormatID == format.Guid)
					return codec;
			}

			return null;
		}
	}
}