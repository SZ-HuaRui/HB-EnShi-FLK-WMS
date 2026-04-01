using CWCS.Equipment.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment.Rfid
{
    public interface IRfid: IRfidRead, IRfidWrite, IInitialize,IConnectable,IRfidMessageEvent
    {
        string ReaderName { get; set; }
        string EpcData { get; set; }
        string UserData { get; set; }
        string TID { get; set; }
        string ReserveData { get; set; }
        void ResetData();
        void ResolveTID(byte[] bytes);
        void ResolveEpcData(byte[] bytes);
        void ResolveUserData(byte[] bytes);
        void ResolveReserveData(byte[] bytes);
    }
}
