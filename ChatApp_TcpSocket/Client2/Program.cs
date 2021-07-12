using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client2
{
    class Program
    {
        static void Main(string[] args)
        {

            new Thread(() =>
            {
                myClient();
            }).Start();

        }
        static void myClient()
        {
            TcpClient client = new TcpClient();
            IPAddress localAddr = IPAddress.Parse("172.18.210.87");
            client.Connect(localAddr, 1005);
            NetworkStream stream = client.GetStream();
            new Thread(() =>
            {

                while (true)
                {
                    Console.WriteLine("Write your text!");
                    string text = Console.ReadLine();
                    byte[] data = Encoding.ASCII.GetBytes(text);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("Sent: {0}", text);


                }
            }).Start();

            new Thread(() =>
            {

                while (true)
                {

                    byte[] data = new byte[256];
                    string responseData = string.Empty;
                    int bytes = stream.Read(data, 0, data.Length);
                    responseData = Encoding.ASCII.GetString(data, 0, bytes);
                    Console.WriteLine("Received: {0}", responseData);

                }
            }).Start();
        }
    }
}
