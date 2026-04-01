using Invengo.NetAPI.Protocol.IRP1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment.Rfid
{
    public interface IRfidRead:IRead
    {
        void ReadEpcData();
        void ReadUserData();
        void ReadReserveData();
        void ReadTID();
        void ReadAll();
    }
}
