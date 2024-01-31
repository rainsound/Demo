using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestSockest.Server
{
    public class ServerManager : ServiceManager
    {
        private const int MAX_LENGTH = 10;
        private Socket serverSocket;
        private List<Socket> connections;
        private byte[] readBuff;
        private Queue<string> messages;

        public override void Start()
        {
            Console.WriteLine("开启主机");
            connections = new List<Socket>();
            readBuff = new byte[1024];
            messages = new Queue<string>();
            serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            EndPoint ipep = new IPEndPoint(IPAddress.Parse("10.0.10.76"), 1234);
            serverSocket.Bind(ipep);
            serverSocket.Listen(MAX_LENGTH);

            WaitNewConnection();
        }

        private async void WaitNewConnection()
        {
            while (serverSocket != null)
            {
                var connection = await serverSocket.AcceptAsync();
                Console.WriteLine("客户的本地端口是：" + connection.LocalEndPoint.ToString());
                Console.WriteLine("打电话进来的客户端是：" + connection.RemoteEndPoint.ToString());
                connections.Add(connection);
            }
        }

        private async void WaitConnectionMessage()
        {
            while (serverSocket != null)
            {
                var connection = await serverSocket.AcceptAsync();
                Console.WriteLine("客户的本地端口是：" + connection.LocalEndPoint.ToString());
                Console.WriteLine("打电话进来的客户端是：" + connection.RemoteEndPoint.ToString());
                connections.Add(connection);
            }
        }

        public override void ThreadUpdate()
        {
            if (connections == null || connections.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < connections.Count; i++)
            {
                var connection = connections[i];
                try
                {
                    int receiveCount = connection.Receive(readBuff);
                    string receiveMessage = Encoding.UTF8.GetString(readBuff, 0, receiveCount);
                    Console.WriteLine("客户端发过来的消息：" + receiveMessage);

                    connection.Send(Encoding.UTF8.GetBytes("服务器已经接到你发来的消息：" + receiveMessage));
                }
                catch (Exception ex)
                {
                    connection.Close();
                    connection.Dispose();
                    connections.Remove(connection);
                    connection = null;
                    i--;
                    Console.WriteLine("连接已中断: " + ex.Message);
                }
            }
        }
    }
}
