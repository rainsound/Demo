using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSockest
{
    public abstract class ServiceManager : IServiceManager
    {
        protected SocketArgsPool socketArgsPool;

        public ServiceManager()
        {
            socketArgsPool = new SocketArgsPool();
        }

        public virtual void Start() { }

        public virtual void ThreadUpdate() { }
    }
}
