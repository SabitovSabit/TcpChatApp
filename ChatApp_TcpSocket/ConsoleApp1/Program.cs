using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Net;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //new Thread(() =>
            //{
            //    myServer();
            //}).Start();

            MultiClientListener myserver = new MultiClientListener("172.18.180.7", 1005);

            await myserver.StartListener();
            //await Task.Run(async () =>
            //{
            //    MultiClientListener myserver = new MultiClientListener("172.18.180.7", 1005);

            //    await myserver.StartListener();
            //});
            //Task task = new Task(async () =>
            //{
            //    MultiClientListener myserver = new MultiClientListener("172.18.180.7", 1005);

            //    await myserver.StartListener();
            //});
            //task.Start();



        }

    }
}

