using CWCS.Common.Uitl;
using CWCS.Entity.SRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CWCS.Core.Library.PLCManager
{
    /// <summary>
    /// Robo 堆垛机
    /// </summary>
    public class SRMPLC:PLCHelper
    {
        public List<SRM_Robot_Entity> SRMSInfo = new List<SRM_Robot_Entity>();
        private List<string> PLCReLink = new List<string>();
        private string local_ip = "";

        #region Event

        private delegate void ReadOPCInfos(object info);
        private event ReadOPCInfos ReadEquipOPCInfo;

        private delegate void ReConnetPLC(string ip,int retryCount);
        private event ReConnetPLC SRMReConnectPLC;

        #endregion

        private string _GuidID;
        private static readonly object objLoc = new object();

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

        public SRMPLC()
        {
            this.ReadEquipOPCInfo += new ReadOPCInfos(ReadSRMInfo);
            this.SRMReConnectPLC += new ReConnetPLC(ReConnect);
        }

        public bool InitData(List<SRM_Robot_Entity> lst, ref string error)
        {
            try
            {
                if (lst is List<SRM_Robot_Entity>)
                {
                    foreach (var item in lst as List<SRM_Robot_Entity>)
                    {
                        local_ip = item.key_ip;
                        if (!PLCList.ContainsKey(local_ip))
                        { 
                            //1500系列 使用0 
                            PLCList.Add(local_ip, new OPCServer(local_ip,0));

                            if (PLCList[local_ip].Connect())
                                WriteToDowntLog?.Invoke($"堆垛机:{item.SCNO},IP:{item.key_ip},连接成功！");
                            else
                                WriteToDowntLog?.Invoke($"堆垛机:{item.SCNO},IP:{item.key_ip},连接失败！");
                        }

                        SRMSInfo.Add(item);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        public void Run(object i_Index)
        {
            Thread threadCom = new Thread(new ParameterizedThreadStart(TimeScan));
            threadCom.IsBackground = true;
            threadCom.Start(i_Index);
        }

        private void TimeScan(object belong)
        {
            while (true)
            {
                if (SRMSInfo != null)
                {
                    foreach (var sc in SRMSInfo)
                        ReadSRMInfo(sc);
                }
                Thread.Sleep(200);
            }
        }

        public override string[] ReadPLCInfo(string EquipNum, int index = 0)
        {
            throw new NotImplementedException();
        }

           /// <summary>
        /// 读取所属线程所有站台信息
        /// </summary>
        /// <param name="stationinfo"></param>
        private void ReadSRMInfo(object obj)
        {
            if (obj != null)
            {
                SRM_Robot_Entity sc = (SRM_Robot_Entity)obj;
                Byte[] scBuffer = new byte[sc.lenght_R];

                if (PLCList[sc.key_ip].Read(sc.DBAddress_R, 0, sc.lenght_R, ref scBuffer))//读多个站台数据
                {
                    sc.cFlag = true;
                    BindSCInfo(sc.SCNO, scBuffer);
                }
                else
                {
                    ReConnect(sc.key_ip);
                    sc.cFlag = false;
                }
                //通知业务处理
                DoWork?.Invoke(sc);
            }
        }

        private void BindSCInfo(string SCNO, byte[] byt)
        {
            SRM_Robot_Entity sc = SRMSInfo.Find(
                delegate(SRM_Robot_Entity ss)
                {
                    return ss.SCNO == SCNO;
                }
            );
            int[] scstatus = new int[16];
            if (sc != null)
            {
                sc.operationMethod = byt[0] * 256 + byt[1];//总状态
                scstatus = SCSTARUS(sc.operationMethod);
                sc.b_I_Get_Finish1 = scstatus[11];//堆垛机完成取货信号（工位1）
                sc.b_I_Put_Finish1 = scstatus[12];//堆垛机完成放货信号（工位1）
                sc.b_I_Get_Finish2 = scstatus[13];//堆垛机完成取货信号（工位2）
                sc.b_I_Put_Finish2 = scstatus[14];//堆垛机完成放货信号（工位2）
                sc.b_I_Auto = scstatus[15];//堆垛机当前运行模式指示1：自动模式 //T 0：非自动模式//F

                sc.b_I_Alarm = scstatus[0];//堆垛机报警状态指示
                sc.b_I_Task_Finish1 = scstatus[1];//堆垛机工位1任务完成
                sc.b_I_Task_Finish2 = scstatus[2];//堆垛机工位2任务完成
                sc.b_I_Loaded1 = scstatus[3];//货叉1有货物
                sc.b_I_Loaded2 = scstatus[4];//货叉2有货物
                sc.b_I_Fork_Zero1 = scstatus[5];//货叉1处于原点位置
                sc.b_I_Fork_Zero2 = scstatus[6];//货叉1处于原点位置

                sc.b_I_SRM_Num = SCNO.Contains("SC") ? Convert.ToInt32(SCNO.Substring(2, 2)) : Convert.ToInt32(SCNO);
                sc.b_I_Alarm_Code = byt[4] * 256 * 256 * 256 + byt[5] * 256 * 256 + byt[6] * 256 + byt[7]; //堆垛机报警代码
                sc.B_I_State = byt[8] * 256 + byt[9];//0：空闲，无任务1：等待，有任务，因安全原因暂停，条件许可即可恢复执行2：定位3：工位1取货4：工位1放货5：工位2取货6：工位2放货98：维修
                sc.B_I_Row = byt[10] * 256 * 256 * 256 + byt[11] * 256 * 256 + byt[12] * 256 + byt[13];//堆垛机当前列（以货叉1为准）
                sc.B_I_Layer = byt[14] * 256 * 256 * 256 + byt[15] * 256 * 256 + byt[16] * 256 + byt[17];//堆垛机当前层
                sc.nTravelPos = byt[18] * 256 + byt[19];//堆垛机当前行走位置，单位mm
                sc.nLiftPos = byt[22] * 256 + byt[26];//堆垛机当前升降位置，单位mm
                sc.nForkPos_Single1 = byt[26] * 256 + byt[27];//堆垛机货叉1位置 货叉向左位置为正值，货叉向右位置为负值
                sc.nForkPos_Single2 = byt[28] * 256 + byt[29];//堆垛机货叉2位置，单位mm货叉向左位置为正值，货叉向右位置为负值
                sc.B_I_TaskId1 = byt[30] * 256 + byt[31];//工位1任务号
                sc.B_I_TaskId2 = byt[32] * 256 + byt[33];//工位2任务号
                sc.B_I_Is_TankOpened = byt[34] * 256 + byt[35];//=1干冰储罐阀门已打开 =0默认值

            }

        }

        public int[] SCSTARUS(int checkinfo)
        {
            string errorcode = Convert.ToString(checkinfo, 2).PadLeft(16, '0');
            Int32 length = errorcode.Length;
            int[] ErrorCode = new int[length];
            for (int i = length - 1; i >= 0; i--)
            {
                ErrorCode[length - i - 1] = int.Parse(errorcode.Substring(i, 1).ToString());
            }
            return ErrorCode;
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        /// <param name="strIP"></param>
        /// <param name="retryCount"></param>
        private void ReConnect(string strIP, int retryCount = 3)
        {
            if (SRMReConnectPLC != null)
            {
                lock (objLoc)
                {
                    WriteToDowntLog?.Invoke($"PLC:{strIP},连接断开,系统开始自动重连...");
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

        #region Write

       

        /// <summary>
        /// 写入值
        /// </summary>
        /// <param name="scno"></param>
        /// <param name="str">写入数组值</param>
        /// <param name="index">地址偏移值</param>
        /// /// <param name="index">长度</param>
        /// <returns></returns>
        private bool Write(string scno, string[] str, int index, int length)
        {
            try
            {
                SRM_Robot_Entity si = SRMSInfo.Find(
                   delegate(SRM_Robot_Entity sc)
                   {
                       return sc.SCNO == scno;
                   }
               );
                if (si != null)
                {
                    byte[] byt = new byte[length];
                    for (int i = 1; i < length; i++)
                    {
                        Array.Copy(DataTransformUtil.StringToTwoByte(str[i - 1]), 0, byt, (i - 1) * 2, 2);
                    }
                    if (si.cFlag)
                    {
                        if (index == 6)
                            si.wcstaskno = 0;
                        if (PLCList[si.key_ip].Write(si.DBAddress_W, si.value_W + index, length, byt))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }


        public bool Write(string[] str, string scno, string barcode, string fromstation, string tostation)
        {
            SRM_Robot_Entity si = SRMSInfo.Find(
               delegate(SRM_Robot_Entity sc)
               {
                   return sc.SCNO == scno;
               }
           );
            if (si != null)
            {
                byte[] byt = new byte[si.lenght_W];
                for (int i = 1; i < str.Length; i++)
                {
                    Array.Copy(DataTransformUtil.StringToTwoByte(str[i]), 0, byt, (i - 1) * 2, 2);
                }
                si.fork1_taskno = int.Parse(str[19]);
                si.WCSBarCode = barcode;
                si.WCSFromstation = fromstation;
                si.WCSToStation = tostation;
                if (si.cFlag)
                {
                    if (PLCList[si.key_ip].Write(si.DBAddress_W, si.value_W, si.lenght_W, byt))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;

                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        public bool UpdateSCWorkModeInfo(string scno, int i_wcsworkmode)
        {
            SRM_Robot_Entity si = SRMSInfo.Find(
                  delegate(SRM_Robot_Entity sc)
                  {
                      return sc.SCNO == scno;
                  }
              );

            if (si != null)
            {
                si.WCSControlMode = i_wcsworkmode;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取堆垛机取放货地址
        /// </summary>
        /// <param name="scno"></param>
        /// <param name="stationnum"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public string[] ConvertAddr(string scno, string stationnum, ref string errorMsg)
        {
            errorMsg = "";
            SRM_Robot_Entity si = SRMSInfo.Find(
                delegate(SRM_Robot_Entity sc)
                {
                    return sc.SCNO == scno;
                }
            );
            if (si != null)
            {

                SCStationInfo scsi = si.lstStation.Find(
                   delegate(SCStationInfo ss)
                   {
                       return ss.StationNum == stationnum;
                   }
               );
                if (scsi != null)
                {
                    string[] str = new string[4];
                    str[0] = scsi.Line;
                    str[1] = scsi.Column;
                    str[2] = scsi.Layer;
                    str[3] = scsi.Deep;
                    return str;
                }
                else
                {
                    errorMsg = "堆垛机：" + "取放货站台:" + stationnum + "不存在！";
                    return null;
                }
            }
            errorMsg = "不存在堆垛机：" + scno;
            return null;
        }
        /// <summary>
        /// 更新堆垛机任务模式
        /// </summary>
        /// <param name="scno"></param>
        /// <param name="taskmode"></param>
        /// <returns></returns>
        public bool UpdateTaskModeInfo(string scno, SCTaskMode taskmode)
        {
            SRM_Robot_Entity si = SRMSInfo.Find(
                  delegate(SRM_Robot_Entity sc)
                  {
                      return sc.SCNO == scno;
                  }
              );

            if (si != null)
            {
                si.WCSTaskMode = taskmode;
                if (taskmode == SCTaskMode.Pause)
                {
                    si.WCSPauseCause = "手动任务暂停";
                }
                return true;
            }
            return false;
        }
    }
}
