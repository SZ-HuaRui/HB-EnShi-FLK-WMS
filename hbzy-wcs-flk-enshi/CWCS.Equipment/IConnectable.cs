using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment.Communication
{
    public interface IConnectable
    {
        bool Connect();
        bool Close();
        bool IsConnected { get; set; }
        bool IsReConnectLoopStarted { get; set;}
        void ReConnectLoop();
    }
}
