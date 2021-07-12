using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Thread(() =>
            //{
            //    myServer();
            //}).Start();
            Thread t = new Thread(delegate ()
            {
                MultiClientListener myserver = new MultiClientListener("192.168.100.114", 1005);
                //myServer();
            });
            t.Start();
            
            Console.WriteLine("Server Started...!");
        }
        static void myServer()
        {
            TcpListener server = null;
            try
            {
                IPAddress localAddr = IPAddress.Parse("172.18.210.87");
                server = new TcpListener(localAddr, 1005);
                server.Start();
                byte[] bytes = new byte[256];
                string data = null;
                Console.Write("Waiting for a connection... ");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                new Thread(() => {
                    while (true)
                    {
                        data = null;
                        NetworkStream stream = client.GetStream();
                        int i;
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            data = Encoding.ASCII.GetString(bytes, 0, i);
                            Console.WriteLine("Received: {0}", data);
                        }
                    }
                }).Start();
                new Thread(() => {
                    while (true)
                    {
                        data = null;
                        NetworkStream stream = client.GetStream();
                        Console.WriteLine("Write your text!");
                        string text = Console.ReadLine();
                        byte[] msg = Encoding.ASCII.GetBytes(text);
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", text);
                    }
                }).Start();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }
            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}
