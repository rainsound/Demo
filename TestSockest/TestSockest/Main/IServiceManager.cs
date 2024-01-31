using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSockest
{
    public interface IServiceManager
    {
        void Start();

        void ThreadUpdate();
    }
}
