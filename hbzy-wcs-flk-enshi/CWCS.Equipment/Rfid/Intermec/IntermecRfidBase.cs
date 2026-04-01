using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment.Rfid.Intermec
{
    public abstract class IntermecRfidBase:RfidBase
    {
        public IntermecRfidBase(string ip, string port) : base(ip, port)
        {

        }
    }
}
