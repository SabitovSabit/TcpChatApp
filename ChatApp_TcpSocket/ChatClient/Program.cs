using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient
{
    class Program
    {
     
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();
            IPAddress localAddr = IPAddress.Parse("172.18.180.7");
            client.Connect(localAddr, 1005);
            
            List<Task> proceslist = new List<Task>();
            proceslist.Add(ClientRead(client));
            proceslist.Add(ClientWrite(client));
            Task.WaitAll(proceslist.ToArray());
            
            //Console.WriteLine("client exiting");

            //new Thread(() =>
            //{
            //    myClient();
            //}).Start();

        }
        public static async Task ClientRead(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            while (true)
            {

                byte[] data = new byte[256];
                string responseData = string.Empty;
                int bytes = await stream.ReadAsync(data, 0, data.Length);
                responseData = Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                Console.WriteLine("Write your text!");

            }
        }
        public static async Task ClientWrite(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            Console.WriteLine("Write your text!");

            while (true)
            {
                string text = Console.ReadLine();
                byte[] data = Encoding.ASCII.GetBytes(text);
                await stream.WriteAsync(data, 0, data.Length);
                Console.WriteLine("Sent: {0}", text);


            }
        }

        static void myClient()
        {
            TcpClient client = new TcpClient();
            IPAddress localAddr = IPAddress.Parse("172.18.180.7");
            client.Connect(localAddr, 1005);
            NetworkStream stream= client.GetStream();
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
                    Console.WriteLine("Write your text!");


                }
            }).Start();

        }
      
    }   
}
