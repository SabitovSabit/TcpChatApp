using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    class MultiClientListener
    {
        TcpListener server = null;
        public static readonly ConcurrentDictionary<int, TcpClient> clientList = new ConcurrentDictionary<int, TcpClient>();
        public MultiClientListener(string ip, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            server.Start();
            StartListener();
        }
      
        public static TcpClient GetCl(int id)
        {
            return clientList[id];
        }
        public void StartListener()
        {
            int count = 1;
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = server.AcceptTcpClient();
                    clientList.TryAdd(count, client);
                    Console.WriteLine("Connected!");
                    Thread handle = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    handle.Start(count);
                   
                    count++;
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
            int id = (int)obj;

            TcpClient client = GetCl(id);
            while (true)
            {
                string datas = default;
                byte[] bytes = new byte[256];
                NetworkStream stream = client.GetStream();
                int i;
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    datas = Encoding.ASCII.GetString(bytes, 0, i);

                    Console.WriteLine($"Received: {id},{datas}");

                    if (!datas.Contains(","))
                    {
                        foreach (var item in clientList.Values)
                        {
                            item.Client.Send(Encoding.ASCII.GetBytes(datas));
                        }
                    }
                    else
                    {
                        string[] texts = datas.Split(",");
                        int key = Convert.ToInt32(texts[0]);


                        string text = texts[1];



                        var cl = GetCl(key);
                        cl.Client.Send(Encoding.ASCII.GetBytes(text));

                    }

                }
            }
        }
    
    }
}
