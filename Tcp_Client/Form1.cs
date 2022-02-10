using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tcp_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string GetIpAddress()
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
        UdpClient udpclient;
        IPEndPoint ep;
        private void button1_Click(object sender, EventArgs e)
        {
            var client = new TcpClient();
            client.Connect(IPAddress.Parse(GetIpAddress()), 777);

            udpclient = new UdpClient(888);
            ep = new IPEndPoint(IPAddress.Any, 0);
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var bytearray = udpclient.Receive(ref ep);
            byte[] bt = Decompress(bytearray);
            pictureBox1.Image = byteArrayToImage(bt);
        }
    }
}
