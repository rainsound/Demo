using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestSockest.Client
{
    public class ClientManager : ServiceManager
    {
        private Socket clientSocket;
        private byte[] readBuff = new byte[1024];

        public override void Start()
        {
            Console.WriteLine("开启客机");
            clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect("10.0.10.76", 1234);

            string sendMessage = Console.ReadLine();
            byte[] sendBytes = Encoding.UTF8.GetBytes(sendMessage);
            clientSocket.Send(sendBytes);
        }

        public override void ThreadUpdate()
        {
            if (clientSocket == null)
            {
                return;
            }
            try
            {
                int count = clientSocket.Receive(readBuff);
                Console.WriteLine("服务器发送的消息是：" + Encoding.UTF8.GetString(readBuff, 0, count));
            }
            catch (Exception ex)
            {
                clientSocket.Close();
                clientSocket.Dispose();
                clientSocket = null;
                Console.WriteLine("连接已中断: " + ex.Message);
            }
        }
    }
}
