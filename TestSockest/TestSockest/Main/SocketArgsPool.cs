using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestSockest
{
    public class SocketArgsPool
    {
        private const int POOL_LENGTH = 10;

        private List<SocketArg> argList;

        public SocketArgsPool()
        {
            argList = new List<SocketArg>();
        }

        public SocketArg Get(Action<object, SocketAsyncEventArgs> action)
        {
            if (action == null)
            {
                Console.WriteLine("未设置回调事件, 获取失败");
                return null;
            }
            SocketArg arg = null;
            if (argList.Count > 0)
            {
                for (int i = 0; i < argList.Count; i++)
                {
                    if (!argList[i].isUse)
                    {
                        arg = argList[i];
                        break;
                    }
                }
            }
            if (arg == null)
            {
                arg = new SocketArg();
                if (argList.Count >= POOL_LENGTH)
                {
                    arg.needClear = true;
                }
                else
                {
                    arg.isUse = true;
                    argList.Add(arg);
                }
            }
            arg.SetCompletedEvent(action);
            return arg;
        }

        public void Recycle(SocketArg arg)
        {
            if (arg == null)
            {
                return;
            }
            if (arg.needClear)
            {
                arg.asyncArg.Dispose();
                arg.asyncArg = null;
                arg = null;
            }
            else
            {
                arg.asyncArg.AcceptSocket = null;
                arg.ClearEvent();
                arg.isUse = false;
            }
        }
    }

    public class SocketArg
    {
        public bool isUse;
        public bool needClear;
        public SocketAsyncEventArgs asyncArg;
        private EventHandler<SocketAsyncEventArgs> eventHandler;

        public SocketArg()
        {
            asyncArg = new SocketAsyncEventArgs();
        }

        public void SetCompletedEvent(Action<object, SocketAsyncEventArgs> action)
        {
            if (asyncArg != null)
            {
                if (eventHandler != null)
                {
                    Console.WriteLine("回收时未清理事件");
                    asyncArg.Completed -= eventHandler;
                    eventHandler = null;
                }
                eventHandler = new EventHandler<SocketAsyncEventArgs>(action);
                asyncArg.Completed += eventHandler;
            }
        }

        public void ClearEvent()
        {
            if (asyncArg != null && eventHandler != null)
            {
                asyncArg.Completed -= eventHandler;
                eventHandler = null;
            }
        }
    }
}
