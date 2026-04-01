using Invengo.NetAPI.Protocol.IRP1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment.Rfid.Invengo
{
    public interface IInvengoRfid:IRfid
    {
        Reader ReaderCore { get; set; }
    }
}
