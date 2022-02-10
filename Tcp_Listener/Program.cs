using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Tcp_Listener
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = new TcpListener(IPAddress.Parse(GetIpAddress()), 777);
            listener.Start(10);

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("Connected");

                var udpclient = new UdpClient();
                var ep = new IPEndPoint(IPAddress.Parse(GetIpAddress()), 888);

                do
                {

                    var msg = Console.ReadLine();

                    if (msg == "send")
                    {

                        Console.WriteLine("Image send!");
                        var bytearray = ImageToByteArray(ScreenConsole());
                        var comprossed_array = Compress(bytearray);

                       

                        var skipcount = 0;
                        var max = ushort.MaxValue - 28;


                        if (comprossed_array.Length > max)
                        {
                            while (skipcount + max <= comprossed_array.Length)
                            {
                                udpclient.Send(comprossed_array.Skip(skipcount).Take(max).ToArray(), max, ep);

                                skipcount += max;
                            }
                            udpclient.Send(comprossed_array.Skip(skipcount).ToArray(), comprossed_array.Length - skipcount, ep);
                        }
                        else
                        {
                            udpclient.Send(comprossed_array, comprossed_array.Length, ep);
                        }
                        byte[] LastSend = Encoding.ASCII.GetBytes("END");
                    }

                } while (true);

            }
        }

        public static Bitmap ScreenConsole()
        {
            Bitmap memoryImage;
            memoryImage = new Bitmap(1400, 780);
            Size s = new Size(memoryImage.Width, memoryImage.Height);

            Graphics memoryGraphics = Graphics.FromImage(memoryImage);

            memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);


            return memoryImage;
        }

        public static byte[] ImageToByteArray(Bitmap imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static byte[] Compress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                return memory.ToArray();
            }
        }

        static string GetIpAddress()
        {
            IPHostEntry host;
            string localhost = "?";
            host = Dns.GetHostEntry(Dns.GetHostName()); // return hostname

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localhost = ip.ToString();
                }
            }
            return localhost;
        }
    }
}
