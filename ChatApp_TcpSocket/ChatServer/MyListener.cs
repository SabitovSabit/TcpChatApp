using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    class MyListener
    {
        TcpListener server = null;
        public MyListener(string ip, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            server.Start();
            StartListener();
        }
        public void StartListener()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }
        public void HandleDeivce(Object obj)
        {
            TcpClient client = (TcpClient)obj;
            byte[] bytes = new byte[256];
            string data = null;
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
    }
}
