using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSockest.Client;
using TestSockest.Server;

namespace TestSockest
{
    public class LogicManager
    {
        private IServiceManager service;

        public void Start()
        {
            Console.WriteLine("Test");
            Console.WriteLine("1.主机   2.客机   else.退出");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    service = new MasterManager();
                    break;
                case "2":
                    service = new ClientManager();
                    break;
                default:
                    break;
            }
            if (service != null)
            {
                WaitCallback callback = new WaitCallback(ThreadUpdate);
                ThreadPool.QueueUserWorkItem(callback);
                service.Start();
            }
            Console.ReadLine();
        }

        private void ThreadUpdate(object state)
        {
            while (service != null)
            {
                service.ThreadUpdate();
                Thread.Sleep(20);
            }
        }
    }
}
