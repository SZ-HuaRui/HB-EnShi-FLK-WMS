using CWCS.Equipment.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment.Led
{
    public interface ILed:ISend,IInitialize,IConnectable,ILedMessageEvent
    {

    }
}
