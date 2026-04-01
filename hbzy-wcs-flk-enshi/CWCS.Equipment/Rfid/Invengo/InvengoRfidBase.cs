using Invengo.NetAPI.Core;
using Invengo.NetAPI.Protocol.IRP1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment.Rfid.Invengo
{
    public abstract class InvengoRfidBase:RfidBase
    {
        //读写器
        public Reader ReaderCore { get; set; }
      
        public InvengoRfidBase(string ip,string port = "7086"):base(ip,port)
        {

        }
       
    }
}
