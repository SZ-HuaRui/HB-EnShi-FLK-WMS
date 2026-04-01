using CWCS.Equipment.Led;
using CWCS.Equipment.Rfid;
using HslCommunication.Profinet.Siemens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
    public static class StaticCommon
    {
        public static Dictionary<string, string> DockStatusDic = new Dictionary<string, string>();

        public static List<WCS_PARAMETER_CONFIG> WcsParamConfigList = new List<WCS_PARAMETER_CONFIG>();

        public static List<RfidEquipment> RfidEquipmentList = new List<RfidEquipment>();

        public static List<LedEquipment> LedEquipmentList = new List<LedEquipment>();

        //输送机网络连接集合
        public static Dictionary<string, SiemensS7Net> ConveyorNetDictionary = new Dictionary<string, SiemensS7Net>();

        //输送机IP地址
        public static List<string> ConveyorIpList = new List<string>();
    }
}
