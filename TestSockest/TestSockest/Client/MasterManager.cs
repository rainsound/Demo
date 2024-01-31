using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace TestSockest.Client
{
    public class MasterManager : ServiceManager
    {
        private Socket masterSocket;
        private byte[] readBuff = new byte[1024];

        public override void Start()
        {
            Console.WriteLine("开启主机");
            masterSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            masterSocket.Connect("10.0.10.76", 1234);

            string sendMessage = Console.ReadLine();
            byte[] sendBytes = Encoding.UTF8.GetBytes(sendMessage);
            masterSocket.Send(sendBytes);
        }

        public override void ThreadUpdate()
        {
            if (masterSocket == null)
            {
                return;
            }
            try
            {
                int count = masterSocket.Receive(readBuff);
                Console.WriteLine("服务器发送的消息是：" + Encoding.UTF8.GetString(readBuff, 0, count));
            }
            catch (Exception ex)
            {
                masterSocket.Close();
                masterSocket.Dispose();
                masterSocket = null;
                Console.WriteLine("连接已中断: " + ex.Message);
            }
        }
    }
}
