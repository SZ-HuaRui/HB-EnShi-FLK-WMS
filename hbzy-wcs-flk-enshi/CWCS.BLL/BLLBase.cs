using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Collections;
using CWCS.Entity.SRM;
using CWCS.Core.Library.PLCManager;
using CWCS.Entity;
using System.Linq;
using CWCS.Repository;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Remoting.Messaging;
using AutoMapper;
using CWCS.Common.Automapper;

namespace CWCS.BLL
{
    public abstract class BLLBase
    {
        public string WareHouseID = "";
        public string ZoneId01 = string.Empty;
        public string ZoneId02 = string.Empty;
        #region SRM
        public static string EQUIPMENT_ERROR = "EquipmentStatus_Error";
        public static string EQUIPMENT_OK = "EquipmentStatus_OK";
        /// <summary>
        /// 取货完成0x79
        /// </summary>
        public static string CR_GETC_EXECUTED = "121";//
        /// <summary>
        /// 放货完成0x89
        /// </summary>
        public static string CR_PUTC_EXECUTED = "137";//
        /// <summary>
        /// 取货指令0x70
        /// </summary>
        public static string CR_GETC = "112";
        /// <summary>
        /// 放货指令0x80
        /// </summary>
        public static string CR_PUTC = "128";
        public static string CR_NO_FUNC = "0";
        public static string CR_POS_GET = "16";//取货前行走

        public static string CR_POS_GET_EXECUTED = "25";//取货前行走完成
        public static string CR_POS_COUNT_EXECUTED = "105";//盘库行走完成

        public static string CR_DROPSH_GETC = "368";//0x0170
        public static string CR_PICKSH_GETC = "624";//0x0270

        public static string CR_DROPSH_PUTC = "384";//0x0180
        public static string CR_PICKSH_PUTC = "640";//0x0280

        //充电站
        public static string CR_DROPSH_PARK = "400";//0x0190
        public static string CR_PICKSH_PARK = "656";//0x0290

        public static string CR_DROPSH_GETC_EXECUTED = "377";//0x0179
        public static string CR_PICKSH_GETC_EXECUTED = "633";//0x0279

        public static string CR_DROPSH_PUTC_EXECUTED = "393";//0x0189
        public static string CR_PICKSH_PUTC_EXECUTED = "649";//0x0289

        public static string CR_DROPSH_PARK_EXECUTED = "409";//0x0199
        public static string CR_PICKSH_PARK_EXECUTED = "665";//0x0299

        //今天国际堆垛机报头
        public static byte[] MessageHeader_NTI_4000 = new byte[] { 0xFF };
        public static byte[] MessageHeader_NTI_6000 = new byte[] { 0xFF };

        //旧协议报头
        public static byte[] MessageHeader_2000 = new byte[] { 0x53, 0x35, 0x10, 0x01, 0x03, 0x03, 0x03, 0x08, 0x01, 0x3C, 0x00, 0x00, 0x00, 0x0C, 0xFF, 0x02 };
        public static byte[] MessageHeader_4000 = new byte[] { 0x53, 0x35, 0x10, 0x01, 0x03, 0x05, 0x03, 0x08, 0x01, 0x3D, 0x00, 0x00, 0x00, 0x0C, 0xFF, 0x02 };
        public static byte[] MessageHeader_6000 = new byte[] { 0x53, 0x35, 0x10, 0x01, 0x03, 0x05, 0x03, 0x08, 0x01, 0x46, 0x00, 0x00, 0x00, 0x47, 0xFF, 0x02 };
        public static byte[] MessageHeaderHC_6000 = new byte[] { 0x53, 0x35, 0x10, 0x01, 0x03, 0x05, 0x03, 0x08, 0x01, 0x46, 0x00, 0x00, 0x00, 0x3D, 0xFF, 0x02 };

        //新协议的报头
        public static byte[] MessageHeader_20000 = new byte[] { 0x53, 0x35, 0x10, 0x01, 0x03, 0x03, 0x03, 0x08, 0x01, 0x3C, 0x00, 0x00, 0x00, 0x1A, 0xFF, 0x02 };
        public static byte[] MessageHeader_40000 = new byte[] { 0x53, 0x35, 0x10, 0x01, 0x03, 0x05, 0x03, 0x08, 0x01, 0x3D, 0x00, 0x00, 0x00, 0x16, 0xFF, 0x02 };
        public static byte[] MessageHeader_60000 = new byte[] { 0x53, 0x35, 0x10, 0x01, 0x03, 0x05, 0x03, 0x08, 0x01, 0x46, 0x00, 0x00, 0x00, 0x47, 0xFF, 0x02 };
        public static byte[] MessageHeaderHC_60000 = new byte[] { 0x53, 0x35, 0x10, 0x01, 0x03, 0x05, 0x03, 0x08, 0x01, 0x46, 0x00, 0x00, 0x00, 0x3D, 0xFF, 0x02 };

        #endregion

        public ConveryPLC plc;
        public static List<SRMObject> lstSRM = new List<SRMObject>();//堆垛机实体类

        public static Hashtable AlarmHash = Hashtable.Synchronized(new Hashtable());//堆垛机详细报警信息存储
        public static List<SRM_Status_Entity> L_SRMStatus = new List<SRM_Status_Entity>();//堆垛机状态类
        public OracleRepository _repository = new OracleRepository();

        public static IMapper _mapper = AutoMapperConfiguration.Initializer();


        protected BLLBase()
        {
             WareHouseID = StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("WarehouseId")).ToList()[0].PARAM_VALUE;
             ZoneId01 = StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("ZoneId01")).ToList()[0].PARAM_VALUE;
             ZoneId02 = StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("ZoneId02")).ToList()[0].PARAM_VALUE;
             plc = new ConveryPLC();
        }
    }
}
