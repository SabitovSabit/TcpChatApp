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

namespace ConsoleApp1
{
    class MultiClientListener
    {

        TcpListener server = null;
        public static readonly ConcurrentDictionary<int, TcpClient> clientList = new ConcurrentDictionary<int, TcpClient>();
        public static readonly List<string> allmessage = new List<string>();
        public List<Task> tasks = new List<Task>();
        string datas = default;

        public MultiClientListener(string ip, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            server.Start();
        }
        public static TcpClient GetCl(int id)
        {        
            return clientList[id];
        }
        public async Task StartListener()
        {
            int count = 1;
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = await server.AcceptTcpClientAsync();
                    clientList.TryAdd(count, client);
                    Console.WriteLine("Connected!");
                    tasks.Add(HandleDeivce(count));
                    Task.WaitAll();
                    count++;
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }
        public async Task HandleDeivce(Object obj)
        {
            int id = (int)obj;
            TcpClient client = GetCl(id);
            while (true)
            {
                byte[] bytes = new byte[256];
                NetworkStream stream = client.GetStream();
                int i;
                while ((i = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                {
                    datas = Encoding.ASCII.GetString(bytes, 0, i);
                    allmessage.Add(datas);
                    Console.WriteLine($"Received: {id},{datas}");
                    await WriteToClient(datas);
                    Console.WriteLine("Data sent to clients");
                }
            }
        }
        public async Task CallBack(int id, string text)
        {
            TcpClient client = GetCl(id);
            NetworkStream stream = client.GetStream();
            byte[] msg = Encoding.ASCII.GetBytes(text);
            await stream.WriteAsync(msg, 0, msg.Length);
            stream.Flush();
        }
        public async Task WriteToClient(string data)
        {
            if (!data.Contains(","))
            {
                foreach (var client in clientList.Values)
                {
                    ArraySegment<byte> array = new ArraySegment<byte>(Encoding.ASCII.GetBytes(data));
                    await client.Client.SendAsync(array, SocketFlags.None);
                }
            }
            else
            {
                string[] texts = data.Split(",");
                int key = Convert.ToInt32(texts[0]);
                string text = texts[1];
                var cl = GetCl(key);
                ArraySegment<byte> array = new ArraySegment<byte>(Encoding.ASCII.GetBytes(text));
                await cl.Client.SendAsync(array, SocketFlags.None);
            }
        }
    }
}
