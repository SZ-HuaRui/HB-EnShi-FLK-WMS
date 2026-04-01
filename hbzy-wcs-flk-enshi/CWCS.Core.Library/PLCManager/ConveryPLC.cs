using CWCS.Common;
using CWCS.Entity;
using CWCS.Entity.Convery;
using CWCS.Entity.SRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using CWCS.Common.Uitl;

namespace CWCS.Core.Library.PLCManager
{
    /// <summary>
    /// 输送机PLC
    /// </summary>
    public class ConveryPLC : PLCHelper
    {
        public static List<ConveryEntity> lsRFID = new List<ConveryEntity>();
        public static List<ConveryEntity> StationsInfo = new List<ConveryEntity>();
        public static List<ConveryEntity> StaticOneCodeList = new List<ConveryEntity>();
        public static List<ConveryEntity> StaticBoxTypeList = new List<ConveryEntity>();
        public static List<ConveryEntity> StaticHeartBeatList = new List<ConveryEntity>();
        public List<ConveryGroupEntity> lstRead1 = new List<ConveryGroupEntity>();
        public List<ConveryGroupEntity> lstRead2 = new List<ConveryGroupEntity>();
        public static List<ConveryItemCode> lstItemCode = new List<ConveryItemCode>();
        public static bool IsInit = false;
        private string local_ip;
        private readonly object objLoc = new object();
        public Dictionary<string,bool> PLCConnectStatus = new Dictionary<string,bool>();

        private delegate void ReadOPCInfos(object info);
        private event ReadOPCInfos ReadEquipOPCInfo;
        private delegate void ReConnetPLC(string ip, int retryCount);
        private event ReConnetPLC STAReConnectPLC;


        private string _GuidID;
        private static readonly object obj = new object();

        public string GuidID
        {
            get
            {
                if (string.IsNullOrEmpty(_GuidID))
                {
                    _GuidID = Guid.NewGuid().ToString();
                }
                return _GuidID;
            }
            set { _GuidID = value; }
        }

        public void InitData(List<ConveryEntity> lst, ref string error)
        {
            try
            {
                foreach (var item in lst as List<ConveryEntity>)
                {
                    local_ip = item.Key_ip;
                    if (!PLCList.ContainsKey(local_ip))
                    {
                        PLCList.Add(local_ip, new OPCServer(local_ip, 1));
                        if (PLCList[local_ip].Connect())
                        {
                            if (!PLCConnectStatus.ContainsKey(item.Key_ip))
                            {
                                PLCConnectStatus.Add(item.Key_ip, true);
                            }
                        }
                        else
                        {
                            if (!PLCConnectStatus.ContainsKey(item.Key_ip))
                            {
                                PLCConnectStatus.Add(item.Key_ip, false);
                            }
                        }
                    }
                    ConveryInit(item, local_ip);
                }
                WriteToDowntLog?.Invoke("输送机初始化完成");
                IsInit = true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

    
        /// <summary>
        /// 加载烟箱类型db块配置数据
        /// </summary>
        /// <param name="dt"></param>
        public void LoadBoxTypeStation(DataTable dt)
        {
            DataRow[] array = dt.AsEnumerable().Where(x => x["groupname"].Equals("BoxType")).ToArray();
            if (array != null && array.Length > 0)
            {
                foreach (DataRow dr in array)
                {
                    ConveryEntity entity = new ConveryEntity();
                    entity.StationNo = dr["stationno"].ToString();
                    entity.Key_ip = dr["userid"].ToString();
                    entity.DBAddress = int.Parse(dr["objects"].ToString());
                    entity.length = int.Parse(dr["signaltype"].ToString());
                    entity.value = int.Parse(dr["value"].ToString());
                    StaticBoxTypeList.Add(entity);
                }
            }
        }


        /// <summary>
        /// 加载心跳db块配置数据
        /// </summary>
        /// <param name="dt"></param>
        public void LoadHeartBeatStation(DataTable dt)
        {
            DataRow[] array = dt.AsEnumerable().Where(x => x["groupname"].Equals("HeartBeat")).ToArray();
            if (array != null && array.Length > 0)
            {
                foreach (DataRow dr in array)
                {
                    ConveryEntity entity = new ConveryEntity();
                    entity.StationNo = dr["stationno"].ToString();
                    entity.Key_ip = dr["userid"].ToString();
                    entity.DBAddress = int.Parse(dr["objects"].ToString());
                    entity.length = int.Parse(dr["signaltype"].ToString());
                    entity.value = int.Parse(dr["value"].ToString());
                    StaticHeartBeatList.Add(entity);
                }
            }
        }






        public ConveryPLC()
        {
            ReadEquipOPCInfo += new ReadOPCInfos(BatchReadConveryInfo);
            this.STAReConnectPLC += new ReConnetPLC(ReConnect);
            QueueSpeech.Start();
        }
        /// <summary>
        /// 初始化输送机
        /// </summary>
        /// <param name="si"></param>
        private void ConveryInit(ConveryEntity si, string strlocal_ip)
        {
                if (si.config == "Convery")
                {
                    if (int.Parse(si.Belong) < 16)
                    {
                        if (lstRead1.Exists(it => it.key_ip.ToString().Trim() == si.Key_ip && it.Belong == si.Belong))
                        {
                            ConveryGroupEntity cr = lstRead1.Find(it => it.key_ip.ToString().Trim() == strlocal_ip && it.Belong == si.Belong.Trim());
                            if (cr != null)
                            {
                                int index = ((int.Parse(si.StationNo) - cr.currentStatioNo));
                                if (index > 1)
                                {
                                    index = index - 1;
                                }

                                cr.length += si.length * index;

                                cr.currentStatioNo = int.Parse(si.StationNo);
                                cr.lststation.Add(si);
                            }
                        }
                        else
                        {
                            ConveryGroupEntity cr = new ConveryGroupEntity();
                            cr.value = si.value;
                            cr.length = si.length;
                            cr.key_ip = si.Key_ip;
                            cr.DBAddress = si.DBAddress;
                            cr.lststation.Add(si);
                            cr.currentStatioNo = int.Parse(si.StationNo);
                            cr.Belong = si.Belong;
                            lstRead1.Add(cr);
                        }
                    }
                    else
                    {
                        if (lstRead2.Exists(it => it.key_ip.ToString().Trim() == si.Key_ip && it.Belong == si.Belong))
                        {
                            ConveryGroupEntity cr = lstRead2.Find(it => it.key_ip.ToString().Trim() == strlocal_ip && it.Belong == si.Belong.Trim());
                            if (cr != null)
                            {
                                int index = ((int.Parse(si.StationNo) - cr.currentStatioNo));
                                if (index > 1)
                                    index = index - 1;
                                cr.length += si.length * index;

                                cr.currentStatioNo = int.Parse(si.StationNo);
                                cr.lststation.Add(si);
                            }
                        }
                        else
                        {
                            ConveryGroupEntity cr = new ConveryGroupEntity();
                            cr.value = si.value;
                            cr.length = si.length;
                            cr.key_ip = si.Key_ip;
                            cr.DBAddress = si.DBAddress;
                            cr.lststation.Add(si);
                            cr.currentStatioNo = int.Parse(si.StationNo);
                            cr.Belong = si.Belong;
                            lstRead2.Add(cr);
                        }
                    }
                    if (!StationsInfo.Exists(it => it.StationNo == si.StationNo && it.Key_ip.ToString().Trim() == si.Key_ip && it.DBAddress == it.DBAddress))
                        StationsInfo.Add(si);
                }
        }
     

        public void Run(object i_Index)
        {
            Thread threadCom = new Thread(TimeScan1);
            threadCom.IsBackground = true;
            threadCom.Start(1);

            Thread bindStationThread = new Thread(TimeScan2);
            bindStationThread.IsBackground = true;
            bindStationThread.Start(1);
        }

        /// <summary>
        /// 写入plc心跳
        /// </summary>
        public void WriteHearBeat()
        {
            int flag = 0;
            byte[] wirteBytes = new byte[2];
            while (true)
            {
                try
                {
                    wirteBytes = DataTransformUtil.IntToTwoByte(flag);
                    if (StaticHeartBeatList != null && StaticHeartBeatList.Count == 1)
                    {
                        ConveryEntity entity = StaticHeartBeatList[0];
                        PLCList[entity.Key_ip].Write(entity.DBAddress, entity.value, entity.length, wirteBytes);
                        flag = flag == 1 ? 0 : 1;
                        Thread.Sleep(2000);
                    }
                }
                catch (Exception)
                {

                }
            }
           
        }



        private void TimeScan1(object belong)
        {
            while (true)
            {
                try
                {
                    lock (obj)
                    {
                        pft.Start();
                        foreach (ConveryGroupEntity csi in lstRead1)
                        {
                            BatchReadConveryInfo(csi);
                        }
                        pft.Stop();

                        foreach (var key in PLCConnectStatus.Keys)
                        {
                            ShowPLCStatus?.Invoke(new PLCStatus() { _connectStatus = PLCConnectStatus[key], _elapsedMilliseconds = pft.Duration, _IP = key });
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteToDowntLog?.Invoke($"轮询PLC数据异常。{ex.Message}{ex.StackTrace}");
                }
                Thread.Sleep(600);
            }
        }

        private void TimeScan2(object belong)
        {
            while (true)
            {
                try
                {
                    lock (obj)
                    {
                        pft.Start();
                        foreach (ConveryGroupEntity csi in lstRead2)
                        {
                         
                            BatchReadConveryInfo(csi);
                        }
                        pft.Stop();

                        foreach (var key in PLCConnectStatus.Keys)
                        {
                            ShowPLCStatus?.Invoke(new PLCStatus() { _connectStatus = PLCConnectStatus[key], _elapsedMilliseconds = pft.Duration, _IP = key });
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteToDowntLog?.Invoke($"轮询PLC数据异常。{ex.Message}{ex.StackTrace}");
                }
                Thread.Sleep(600);
            }
        }

        /// <summary>
        /// 读取所属线程所有站台信息
        /// </summary>
        /// <param name="stationinfo"></param>
        private void BatchReadConveryInfo(object obj)
        {
            if (ReadEquipOPCInfo != null)
            {
                ConveryGroupEntity stat = (ConveryGroupEntity)obj;
                int length = stat.length;
                Byte[] ConveryBuffer = new byte[length];
                if (PLCList[stat.key_ip].Read(stat.DBAddress, stat.value, length, ref ConveryBuffer))//读多个站台数据
                {
                    stat.cFlag = true;
                    BindListStation(ConveryBuffer, stat);
                }
                else
                {
                    stat.cFlag = false;
                    ReConnect(stat.key_ip, 3);
                }
                if (!PLCConnectStatus.ContainsKey(stat.key_ip))
                    PLCConnectStatus.Add(stat.key_ip, stat.cFlag);
                else
                    PLCConnectStatus[stat.key_ip] = stat.cFlag;
                if (StationsInfo != null)
                    RefreshStation?.Invoke(StationsInfo);
            }
        }

        #region 数据解析
        /// <summary>
        /// 绑定任务信息
        /// </summary>
        /// <param name="stationno"></param>
        /// <param name="byt"></param>
        private void BindStation(string stationno, byte[] byt)
        {
            ConveryEntity si = StationsInfo.Find(
              delegate(ConveryEntity ss)
              {
                  return ss.StationNo == stationno;
              });
            if (si != null)
            {

                si.C_TaskInfo.TaskNumber = byt[0] * 256 + byt[1];//任务号
                si.C_TaskInfo.Barcode = ((byt[2] * 256 + byt[3]) * 65536 + byt[4] * 256 + byt[5]).ToString().PadLeft(5, '0');//托盘条码23
                si.C_TaskInfo.GoodsType = byt[6] * 256 + byt[7];//托盘类型
                si.C_TaskInfo.FromStation = byt[8] * 256 + byt[9];//起始地址
                si.C_TaskInfo.ToStation = byt[10] * 256 + byt[11];//目标地址
                si.C_TaskInfo.Qty = byt[12]; ;//数量
                si.C_TaskInfo.BoxType = byt[13];//没有意义，但是电控开了这个字段
                si.C_TaskInfo.RfidFlag = byt[14] * 256 + byt[15];//RFID标记
                si.C_TaskInfo.CheckInfo = byt[16] * 256 + byt[17];//外形检测
                si.C_TaskInfo.SystemError = byt[18] * 256 + byt[19];//WCS内部错误
            }
      
            if ((si.C_TaskInfo.TaskNumber > 0 && si.C_TaskInfo.StationNo == si.C_TaskInfo.ToStation.ToString()&&si.StationType > 0))
            {
                DoWork?.Invoke(si);//业务处理
            }
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="byt"></param>
        /// <param name="cr"></param>
        private void BindListStation(byte[] byt, ConveryGroupEntity cr)
        {
            try
            {
                lock (obj)
                {
                    int i_Length = cr.length;
                    int currentStation = 0;
                    foreach (var item in cr.lststation)
                    {
                        ConveryEntity convery = item as ConveryEntity;
                        ConveryEntity si = StationsInfo.Find(
                              delegate (ConveryEntity ss)
                              {
                                  return ss != null && ss.StationNo == convery.StationNo;
                              }
                         );
                        if (si != null)
                        {
                            byte[] bytetemp = new byte[si.length];

                            Array.Copy(byt, cr.length - i_Length, bytetemp, 0, si.length);
                            if (bytetemp != null)
                            {
                                //可采用异步方式
                                BindStation(si.StationNo, bytetemp);//绑定信息
                            }
                        }
                        i_Length = i_Length - si.length;
                        currentStation = int.Parse(si.StationNo);
                    }
                    Thread.Sleep(50);
                }
            }
            catch(Exception ex)
            {
                WriteToDowntLog.Invoke($"绑定数据异常。{ex.Message}{ex.StackTrace}");
            }
        }

        #endregion

        /// <summary>
        /// 重新连接
        /// </summary>
        /// <param name="strIP"></param>
        /// <param name="retryCount"></param>
        private void ReConnect(string strIP, int retryCount = 3)
        {
            if (STAReConnectPLC != null)
            {
                lock (objLoc)
                {
                    WriteToDowntLog?.Invoke($"PLC:{strIP},连接断开,系统开始自动重连...");
                    QueueSpeech.AddQueue("与PLC连接断开,系统开始尝试重连");
                    if (PLCList.ContainsKey(strIP))
                    {
                        bool flag = true;
                        int i_Count = 1;
                        while (flag)
                        {
                            if (i_Count > retryCount)
                                break;
                            PLCList[strIP].DisConnect();
                            WriteToDowntLog?.Invoke($"PLC:{strIP},尝试连接次数:{i_Count}");
                            if (PLCList[strIP].Connect())
                            {
                                WriteToDowntLog?.Invoke($"PLC:{strIP},连接成功");
                                QueueSpeech.AddQueue("与PLC重连成功");
                                flag = false;
                                break;
                            }
                            else
                            {
                                i_Count++;
                            }
                        }
                    }
                }
            }
        }

        public override string[] ReadPLCInfo(string EquipNum, int index = 0)
        {

            ConveryEntity si = StationsInfo.Find(x => x.StationNo.Equals(EquipNum));

            if (si != null)
            {
                string[] info = new string[10];
                Byte[] ConveryBuffer = new byte[si.length];
                if (PLCList[si.Key_ip].Read(si.DBAddress, si.value, si.length, ref ConveryBuffer))
                {
                    si.CFlag = true;

                    si.C_TaskInfo.TaskNumber = ConveryBuffer[0] * 256 + ConveryBuffer[1];//任务信息
                    si.C_TaskInfo.Barcode = ((ConveryBuffer[2] * 256 + ConveryBuffer[3]) * 65536 + ConveryBuffer[4] * 256 + ConveryBuffer[5]).ToString().PadLeft(5, '0');//托盘条码
                    si.C_TaskInfo.GoodsType = ConveryBuffer[6] * 256 + ConveryBuffer[7];//托盘类型
                    si.C_TaskInfo.FromStation = ConveryBuffer[8] * 256 + ConveryBuffer[9];//起始地址
                    si.C_TaskInfo.ToStation = ConveryBuffer[10] * 256 + ConveryBuffer[11];//目标地址
                    si.C_TaskInfo.Qty = ConveryBuffer[12];//数量
                    si.C_TaskInfo.BoxType = ConveryBuffer[13];//烟箱类型
                    si.C_TaskInfo.RfidFlag = ConveryBuffer[14] * 256 + ConveryBuffer[15];
                    si.C_TaskInfo.CheckInfo = ConveryBuffer[16] * 256 + ConveryBuffer[17];
                    si.C_TaskInfo.SystemError = ConveryBuffer[18] * 256 + ConveryBuffer[19];

                    info[0] = si.C_TaskInfo.TaskNumber.ToString();
                    info[1] = si.C_TaskInfo.Barcode;
                    info[2] = si.C_TaskInfo.GoodsType.ToString();
                    info[3] = si.C_TaskInfo.FromStation.ToString();
                    info[4] = si.C_TaskInfo.ToStation.ToString();
                    info[5] = si.C_TaskInfo.Qty.ToString();
                    info[6] = si.C_TaskInfo.BoxType.ToString();
                    info[7] = si.C_TaskInfo.RfidFlag.ToString();
                    info[8] = si.C_TaskInfo.CheckInfo.ToString();
                    info[9] = si.C_TaskInfo.SystemError.ToString();
                }
                else
                {
                    si.CFlag = false;
                }
                return info;
            }
            return null;
        }
     

       

        private bool Write(string[] str, string equipmentno, int index = 0, int length = 0)
        {
            byte[] tempBytes;
            ConveryEntity si = StationsInfo.Find(a=>a.StationNo.Equals(equipmentno));
            if (si != null)
            {
                byte[] byt;
                if (length == 0)
                    byt = new byte[si.length];
                else
                    byt = new byte[length];

                for (int i = 0;i < length;i++)
                {
                    byt[i] = 0;
                }
                for (int i = 0; i < str.Length; i++)
                {
                    //任务号
                    if (i == 0)
                    {
                        Array.Copy(DataTransformUtil.IntToTwoByte(Convert.ToInt32(str[i])), 0, byt, 0, 2);
                    }
                    //托盘号
                    if (i == 1)
                    {
                        tempBytes = DataTransformUtil.IntToFourByte(Convert.ToInt32(str[i]));
                        Array.Copy(tempBytes, 0, byt, 2, 4);
                    }
                    //货物类型
                    if (i == 2)
                    {
                        tempBytes = DataTransformUtil.IntToTwoByte(Convert.ToInt32(str[i]));
                        Array.Copy(tempBytes, 0, byt, 6, 2);
                    }
                    //起始地址
                    if (i == 3)
                    {
                        tempBytes = DataTransformUtil.IntToTwoByte(Convert.ToInt32(str[i]));
                        Array.Copy(tempBytes, 0, byt, 8, 2);
                    }
                    //目标地址
                    if (i == 4)
                    {
                        tempBytes = DataTransformUtil.IntToTwoByte(Convert.ToInt32(str[i]));
                        Array.Copy(tempBytes, 0, byt, 10, 2);
                    }
                    //烟箱数量
                    if (i == 5)
                    {
                        byt[12] = Convert.ToByte(str[5]);
                    }
                    //烟箱类型
                    if (i == 6)
                    {
                        byt[13] = Convert.ToByte(str[6]);
                    }
                    //RFID标记
                    if (i == 7)
                    {
                        tempBytes = DataTransformUtil.IntToTwoByte(Convert.ToInt32(str[i]));
                        Array.Copy(tempBytes, 0, byt, 14, 2);
                    }
                    //故障位
                    if (i == 8)
                    {
                        tempBytes = DataTransformUtil.IntToTwoByte(Convert.ToInt32(str[i]));
                        Array.Copy(tempBytes, 0, byt, 16, 2);
                    }
                    //系统故障
                    if (i == 9)
                    {
                        tempBytes = DataTransformUtil.IntToTwoByte(Convert.ToInt32(str[i]));
                        Array.Copy(tempBytes, 0, byt, 18, 2);
                    }
                }
                if (length == 0)
                {
                    if (PLCList[si.Key_ip].Write(si.DBAddress, si.value + index, si.length, byt))
                    {
                        return true;
                    }
                }
                else
                {
                    if (PLCList[si.Key_ip].Write(si.DBAddress, si.value + index, length, byt))
                    {
                        return true;
                    }
                }

            }
            return false;
        }


        public bool Write(STACommand cmd, string[] str, string equipmentno, int index, int length)
        {
            bool flag = false;
            switch (cmd)
            {
                case STACommand.ClearToStation:
                    flag = Write(str, equipmentno, index, 2);
                    break;
                case STACommand.ReWrite:
                    flag = Write(str, equipmentno);
                    break;
            }
            return flag;
        }

        /// <summary>
        /// 读取一号工程条码
        /// </summary>
        /// <param name="stationNO"></param>
        /// <returns></returns>
        public List<string> ReadOneCodePLC(string stationNO)
        {
            List<string> list = new List<string>();
            string oneCode = string.Empty;
            if (StaticOneCodeList != null && StaticOneCodeList.Count > 0)
            {
                byte[] readByte = new byte[32];
                ConveryEntity entity = StaticOneCodeList.Where(x => x.StationNo.Equals(stationNO)).ToList()[0];
                for (int i = 0;i < entity.length/32;i++)
                {
                    if(PLCList[entity.Key_ip].Read(entity.DBAddress,entity.value+32*i,32, ref readByte))
                    {
                        oneCode = Encoding.ASCII.GetString(readByte).Trim('\0').Trim(' ');
                        if (!string.IsNullOrEmpty(oneCode) && !oneCode.Contains("?") && !oneCode.Contains("NoRead"))
                        {
                            list.Add(oneCode);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 读取站台信息
        /// </summary>
        /// <param name="stationNo"></param>
        /// <returns></returns>
        public static string[] ReadStationInfo(string stationNo)
        {
            string[] info = new string[15];
            ConveryEntity si = StationsInfo.Find(x=>x.StationNo == stationNo.ToString() && IsInit);
            if (si != null )
            {
                byte[] byt = new byte[si.length];
                if (PLCList[si.Key_ip].Read(si.DBAddress, si.value, si.length, ref byt))//读多个站台数据
                {
                    
                    si.CFlag = true;
                    si.C_TaskInfo.TaskNumber = byt[0] * 256 + byt[1];//任务信息
                    si.C_TaskInfo.Barcode = ((byt[2] * 256 + byt[3]) * 65536 + byt[4] * 256 + byt[5]).ToString().PadLeft(5, '0');//托盘条码
                    si.C_TaskInfo.GoodsType = byt[6] * 256 + byt[7];//托盘类型
                    si.C_TaskInfo.FromStation = byt[8] * 256 + byt[9];//起始地址
                    si.C_TaskInfo.ToStation = byt[10] * 256 + byt[11];//目标地址
                    si.C_TaskInfo.Qty = byt[12];//数量
                    si.C_TaskInfo.BoxType = byt[13];//烟箱类型
                    si.C_TaskInfo.RfidFlag = byt[14] * 256 + byt[15];//RFID标记
                    si.C_TaskInfo.CheckInfo = byt[16] * 256 + byt[17];
                    si.C_TaskInfo.SystemError = byt[18] * 256 + byt[19];
                }
                else
                {
                    si.CFlag = false;
                }
                info[0] = si.C_TaskInfo.TaskNumber.ToString();
                info[1] = si.C_TaskInfo.Barcode.ToString();
                info[2] = si.C_TaskInfo.GoodsType.ToString();
                info[3] = si.C_TaskInfo.FromStation.ToString();
                info[4] = si.C_TaskInfo.ToStation.ToString();
                info[5] = si.C_TaskInfo.Qty.ToString();
                info[6] = si.C_TaskInfo.BoxType.ToString();
                info[7] = si.C_TaskInfo.RfidFlag.ToString();
                info[8] = si.C_TaskInfo.CheckInfo.ToString();
                info[9] = si.C_TaskInfo.SystemError.ToString();
            }
            return info;
        }
      
       
    }
}
