using Invengo.NetAPI.Protocol.IRP1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment.Rfid
{
    public interface IRfidWrite:IWrite
    {
        /// <summary>
        /// 写Epc区
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool WriteEpcDataString(string message);
        /// <summary>
        /// 写User区
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool WriteUserDataString(string message);
        /// <summary>
        /// 写Reserve区
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool WriteReserveString(string message);
        bool WriteEpcDataBytes(byte[] bytes);
        bool WriteUserDataBytes(byte[] bytes);
        bool WriteReserveBytes(byte[] bytes);
    }
}
