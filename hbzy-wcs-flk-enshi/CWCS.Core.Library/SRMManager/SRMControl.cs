using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWCS.Core.Library.SocketManager;
using CWCS.Entity;
using System.Timers;
using CWCS.Common.Log;
using System.ComponentModel;

namespace CWCS.Core.Library.SRMManager
{
    public class SRMControl
    {
        //write报文头
        public static byte[] MessageHeader_2000 = new byte[]
        {
            0x53,
            0x35,
            0x10,//报文头长度
            0x01,
            0x03,
            0x03,
            0x03,
            0x08,
            0x01,//区域类型：DB.M.I.Q 
            0x3C,//DB块编号,DB60 
            0x00,//写入起始地址(2个字节)
            0x00,//写入起始地址(2个字节) 
            0x00,//写入长度(2个字节)
            0x1A,//写入长度(2个字节) 26
            0xFF,
            0x02
        };

        //fetch报文头                                                                                          
        public static byte[] MessageHeader_4000 = new byte[]
        { 0x53,
          0x35,
          0x10,//报文头长度 
          0x01,
          0x03,
          0x05,
          0x03,
          0x08,
          0x01,//区域类型：DB.M.I.Q           
          0x3D,//DB块编号,DB61
          0x00,//读取起始地址(2个字节)
          0x00,//读取起始地址(2个字节)
          0x00,//读取长度(2个字节)
          0x22,//读取长度(2个字节)
          0xFF,
          0x02
        };
        public static byte[] MessageHeader_6000 = new byte[]
        {   0x53,
            0x35,
            0x10,//报文头长度 
            0x01,
            0x03,
            0x05,
            0x03,
            0x08,
            0x01,//区域类型：DB.M.I.Q       
            0x46,//DB块编号,DB70
            0x00,//读取起始地址(2个字节) 
            0x00,//读取起始地址(2个字节) 
            0x00,//读取长度(2个字节) 
            0x47,//读取长度(2个字节)
            0xFF,
            0x02
        };

        public delegate void AnalyseMessageEventHandler(NotifyCommand CommandType, string srmno, int port, object msg);//B1声明Anysisdata 解析收到数据时事件的代理，传给BLL层
        public event AnalyseMessageEventHandler AnalyseMessageEvent;

        private SocketClient OSrmSocket = new SocketClient();
        private string m_serverIP, m_strSRMNo, m_MessageRecv;//堆垛机IP，字段名
        private int m_serverPort;
        private Timer CheckTimer = null;
        private Timer GetStatusTimer = null;

        public SRMControl(string strsrmno, string ipaddress, int port)//构造函数 
        {
            m_serverIP = ipaddress;
            m_serverPort = port;
            m_strSRMNo = strsrmno;
            try
            {
                if (port != 2000)
                {
                    OSrmSocket.Notify += new NotifyCommandHander(OSrmSocket_Notify);
                    CheckConnectTimer.Interval = 5000;//Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CheckConnectTimer"]);
                    CheckConnectTimer.Elapsed += new ElapsedEventHandler(CheckConnectTimer_Elapsed);//若连接失败则检测网络直到连接上了
                    GetSCStatusTimer.Interval = 800;//Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GetStatusTime"]);
                    GetSCStatusTimer.Elapsed += new ElapsedEventHandler(GetSCStatusTimer_Elapsed);//获取卫星小车信息Timer事件
                    GetSCStatusTimer.Start();
                }
            }
            catch (Exception ex)
            {
                LogUtil.Write(LogFileName.ScExceptionLog,LogFileName.ScExceptionLog,$"{ex.Message}{ex.StackTrace}");
            }
        }

        public Timer CheckConnectTimer
        {
            get
            {
                if (CheckTimer == null)
                {
                    CheckTimer = new Timer();
                }
                return CheckTimer;
            }
        }

        public Timer GetSCStatusTimer
        {
            get
            {
                if (GetStatusTimer == null)
                { GetStatusTimer = new Timer(); }
                return GetStatusTimer;
            }
        }

        private void GetSCStatusTimer_Elapsed(object sender, ElapsedEventArgs e) //获取卫星小车信息Timer事件
        {
            if (m_serverPort == 4000)
            {
                if (!SendMsg(MessageHeader_4000))
                {
                    CheckConnectTimer.Start();
                    GetSCStatusTimer.Stop();
                }
            }

            if (m_serverPort == 6000)
            {
                if (!SendMsg(MessageHeader_6000))
                {
                    CheckConnectTimer.Start();
                    GetSCStatusTimer.Stop();
                }
            }
        }

        private void CheckConnectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            QueueSpeech.AddQueue(string.Format("与{0}号堆垛机连接断开,系统开始尝试重连", m_strSRMNo.Substring(m_strSRMNo.Length - 1, 1)));
            if (Startup())
            {
                CheckConnectTimer.Stop();
                GetSCStatusTimer.Start();

                AnalyseMessageEvent(NotifyCommand.Connect, m_strSRMNo, m_serverPort, string.Format("{0},Ip:{1},Port:{2} 重连成功！", m_strSRMNo, m_serverIP, m_serverPort));
                QueueSpeech.AddQueue(string.Format("与{0}号堆垛机重连成功", m_strSRMNo.Substring(m_strSRMNo.Length - 1, 1)));
            }
            else
            {
                if (OSrmSocket != null) OSrmSocket.Close();
                AnalyseMessageEvent(NotifyCommand.Break, m_strSRMNo, m_serverPort, string.Format("{0},Ip:{1},Port:{2} 重连失败！", m_strSRMNo, m_serverIP, m_serverPort));
            }
        }

        public SRMControl()
        {
            // TODO: Complete member initialization
        }

        //建立Socket连接
        public bool Startup()
        {
            try
            {
                if (OSrmSocket != null) OSrmSocket.Close();
                return OSrmSocket.Connect(m_serverIP, m_serverPort);
            }
            catch (Exception ex)
            {
                LogUtil.Write(LogFileName.ScExceptionLog, LogFileName.ScExceptionLog, $"堆垛机:{m_strSRMNo},IP:{m_serverIP},Port:{m_serverPort},连接异常。{ex.Message}{ex.StackTrace}");
                return false;
            }
        }

        //发送报文
        public bool SendMsg(byte[] msg)
        {
            return OSrmSocket.Send(msg);
        }

        private void OSrmSocket_Notify(NotifyCommand Command, object state)//信息处理
        {
            if (AnalyseMessageEvent != null)
            {
                string strmsg = "";
                if (Command == NotifyCommand.RecvData)
                {
                    byte[] byt = (byte[])state;
                    if (m_serverPort == 6000)
                    {
                        for (int j = 18; j < 142; j++)
                        {
                            if (byt[j].ToString() != "0")
                                strmsg += (j - 16).ToString() + ":" + byt[j].ToString() + ";";
                        }
                    }
                    else
                    {

                        for (int i = 0; i < 200; i++)
                        {
                            strmsg += byt[i].ToString() + ";";
                        }
                        if (m_serverPort == 4000 && Command == NotifyCommand.RecvData && byt[22].ToString() != "0")
                        {
                            if (m_MessageRecv != "任务号:" + byt[20].ToString() + "," + byt[21].ToString() + "; 低位指令代码:" + byt[22].ToString() + "; 高位指令代码:" + byt[38].ToString()
                                + "; 函数模式:" + byt[23].ToString() + "; 堆垛机模式:" + byt[38].ToString() + "; 动作点:" + byt[26].ToString() + "; 巷道:" + byt[27].ToString() + "; 行:" + byt[28].ToString()
                                + "; 列:" + byt[29].ToString() + "; 列前后:" + byt[30].ToString() + "; 高度:" + byt[31].ToString() + "; 深度:" + byt[32].ToString())
                            {
                                m_MessageRecv = "任务号:" + byt[20].ToString() + "," + byt[21].ToString() + "; 低位指令代码:" + byt[22].ToString() + "; 高位指令代码:" + byt[38].ToString()
                                + "; 函数模式:" + byt[23].ToString() + "; 堆垛机模式:" + byt[38].ToString() + "; 动作点:" + byt[26].ToString() + "; 巷道:" + byt[27].ToString() + "; 行:" + byt[28].ToString()
                                + "; 列:" + byt[29].ToString() + "; 列前后:" + byt[30].ToString() + "; 高度:" + byt[31].ToString() + "; 深度:" + byt[32].ToString() + ";监测值" + byt[7].ToString();
                            }
                        }
                    }
                }
                else if (Command == NotifyCommand.Break || Command == NotifyCommand.Connect)
                {
                    LogUtil.Write(LogFileName.ScLog, LogFileName.ScLog, $"{Command}{state}");
                    strmsg = state.ToString();
                }
                AnalyseMessageEvent(Command, m_strSRMNo, m_serverPort, strmsg);
            }
        }

        /// <summary>
        /// 获取货位侧
        /// </summary>
        /// <param name="scNo"></param>
        /// <param name="locationNumber"></param>
        /// <returns></returns>
        public int GetRackSide(string scNo, string locationNumber)
        {
            if (scNo.Equals("SC01"))
            {
                return 2;
            }

          
            if (scNo.Equals("SC02"))
            {
                #region 0010010011型货位
                if (locationNumber.Length == 10)
                {
                    if (Convert.ToInt32(locationNumber.Substring(3, 3)) <= 17)
                    {
                        return 1;
                    }
                }
                #endregion

                #region 10104011型货位
                //31801011
                if (locationNumber.Length == 8)
                {
                    if (Convert.ToInt32(locationNumber.Substring(1,2)) < 18)
                    {
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
                #endregion
            }
            return 2;
        }

        /// <summary>
        /// 获取货位深度
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public int GetRackDepth(string scNo, string locationNumber)
        {
            #region 0010010011型货位
            if (locationNumber.Length == 10)
            {
                if (scNo.Equals("SC01"))
                {
                    return Convert.ToInt32(locationNumber.Substring(3, 3));
                }

                if (Convert.ToInt32(locationNumber.Substring(3, 3)) <= 17)
                {
                    return 18 - Convert.ToInt32(locationNumber.Substring(3, 3));
                }
                return Convert.ToInt32(locationNumber.Substring(3, 3)) - 17;
            }
            #endregion

            #region 10104011型货位
            if (locationNumber.Length == 8)
            {
              
                if (scNo.Equals("SC01"))
                {
                    return Convert.ToInt32(locationNumber.Substring(3,2));
                }
                if (scNo.Equals("SC02"))
                {
                    if (Convert.ToInt32(locationNumber.Substring(1, 2)) >= 18)
                    {
                        return 7 - Convert.ToInt32(locationNumber.Substring(3, 2));
                    }
                    else
                    {
                        return 18 - Convert.ToInt32(locationNumber.Substring(3, 2));
                    }
                }
            }
            #endregion
            return Convert.ToInt32(locationNumber.Substring(3, 2)) - 17;
        }

        //18 2   
        //19 3
        //27 11
        public int GetRackLine(string locationNumber)
        {
            //31101011
            int line = 0;
            if (locationNumber.Length == 8)
            {
                line = Convert.ToInt32(locationNumber.Substring(1, 2));
                if (line < 18)
                {
                    return line;
                }
                else if( line >= 18 && line <= 27)
                {
                    line = line - 18 + 2;
                }
                else 
                {
                    if (line == 28)
                    {
                        line = 4;
                    }
                    if (line == 29)
                    {
                        line = 9;
                    }
                    if (line == 30)
                    {
                        line = 16;
                    }
                }
                return line;
            }
            line = Convert.ToInt32(locationNumber.Substring(0, 3));
            return line;
        }

        public int GetRackLayer(string locationNumber)
        {
            //31101011
            if (locationNumber.Length == 8)
            {
                return Convert.ToInt32(locationNumber.Substring(0, 1));
            }
            return Convert.ToInt32(locationNumber.Substring(6, 3));
        }

        public bool SendCode(string num, string type, string taskno, string station, bool isMan)
        {
            #region 厦门配方协议
            //    try
            //    {
            //        station = station.Trim('A').Trim('D');
            //        if (station.Length != 4 && station.Length != 10) return false;
            //        byte[] msg = new byte[40];
            //        Array.Copy(MessageHeader_2000, 0, msg, 0, 16);
            //        int actionpoint = 1;

            //        //Buffer state
            //        msg[16] = 0x01;
            //        //Message count
            //        msg[17] = 0x01;

            //        //dint;任务号
            //        msg[18] = 0x00;
            //        msg[19] = 0x00;
            //        msg[20] = Convert.ToByte(Convert.ToInt32(taskno) / 256);
            //        msg[21] = Convert.ToByte(Convert.ToInt32(taskno) % 256);

            //        //byte;function code 低位:GETC: 112的16进制是70   PUTC: 128 的16进制是80  NOFUNC: 0
            //        msg[22] = Convert.ToByte(Convert.ToInt32(type) / 256);

            //        //byte;UL type for LH1
            //        msg[23] = 0x01;

            //        //byte;UL type for LH2
            //        msg[24] = 0x00;

            //        //byte;Setpoint LH;货叉编号，单叉为1
            //        msg[25] = 0x01; 

            //       //IC入库站台和，OC出库动作端的定义  输送机地址编号
            //        if (station != null && station.Length == 4)
            //        {
            //            actionpoint = 0;
            //            switch (int.Parse(station))
            //            {
            //                //1为crane自身定位点
            //                case 5637: actionpoint = 2; break;//ic01-1
            //                case 5647: actionpoint = 3; break;//ico2-1
            //                case 5622: actionpoint = 4; break;//ico2-2
            //                case 5638: actionpoint = 6; break;//oc01-1
            //                case 5628: actionpoint = 7; break;//oco2-1
            //                case 5627: actionpoint = 8; break;//oco2-2
            //                default: actionpoint = 0; break;
            //            }
            //            for (int i = 27; i < 40; i++)
            //                msg[i] = 0x00;
            //        }
            //        else
            //        {
            //            int aisle1 = int.Parse(num.Substring(2, num.Length - 2));
            //            msg[27] = (byte)aisle1;//巷道号
            //            msg[28] = (byte)GetRackSide(num,station);  //RACK SIDE 1,left;2,right   
            //            msg[29] = Convert.ToByte(station.Substring(0, 3));//RACK FIELD 第几排
            //            msg[30] = 0x00;//Y:层  rack place  货格里的第几个位置
            //            msg[31] = Convert.ToByte(station.Substring(6,3));//Y:层  rack level 货位层
            //            msg[32] = (byte)GetRackDepth(num,station);// rack depth
            //            for (int i = 33; i < 40; i++)
            //                msg[i] = 0x00;
            //        }
            //        msg[26] = (byte)actionpoint;     //设置堆垛机的动作点
            //        msg[37] = 0x01;//1,2,3。高，中，低
            //        msg[38] = Convert.ToByte(Convert.ToInt32(type) % 256);//功能类型高位
            //        string commandType = string.Empty;
            //        switch (type)
            //        {
            //            case "112": commandType = "取货指令";break;
            //            case "128": commandType = "放货指令"; break;
            //            case "0": commandType = "清除指令"; break;
            //            case "96": commandType = "盘点指令"; break;
            //            case "1": commandType = "取放指令"; break;
            //            case "16": commandType = "移动指令"; break;
            //            default:break;
            //        }

            //        bool sendflag = SendMsg(msg);
            //        if (sendflag)
            //        {
            //            AnalyseMessageEvent(NotifyCommand.SendData, m_strSRMNo, m_serverPort, string.Format("{0}下发{1}任务:{2},指令:{3},地址:{4}", isMan ? "手动" : "WCS自动", num, taskno, commandType, station));
            //        }
            //        else
            //        {
            //            AnalyseMessageEvent(NotifyCommand.Break, m_strSRMNo, m_serverPort, string.Format("{0},Ip:{1},Port:{2} 指令下发失败！", m_strSRMNo, m_serverIP, m_serverPort));
            //            srmlog.WriteLog(num + "SendCode发送指令失败:" + type.ToString() + "/" + taskno.ToString() + "/" + station);
            //        }
            //        srmlog.WriteLog("WCS向" + num + "发送指令成功。" + commandType + "；任务号：" + taskno.ToString() + "；目标地址：" + station + "。" + (isMan ? "手动" : "自动"));
            //        return sendflag;
            //    }
            //    catch (Exception ee)
            //    {
            //        srmlog.WriteLog(num + "SendCode发送指令异常:" + type.ToString() + "/" + taskno.ToString() + "/" + station + "  " + ee.Message);
            //        return false;
            //    }
            //}
            #endregion

            try
            {

                station = station.Trim('A').Trim('D');
                if (station.Length != 4 && station.Length != 10 && station.Length != 8) return false;
                byte[] msg = new byte[68];
                Array.Copy(MessageHeader_2000, 0, msg, 0, 16);
                int actionpoint = 1;
                msg[16] = 0x01;
                msg[17] = 0x01;//messagecount
                msg[18] = 0x00;
                msg[19] = 0x00;
                //任务号
                msg[20] = Convert.ToByte(Convert.ToInt32(taskno) / 256);
                msg[21] = Convert.ToByte(Convert.ToInt32(taskno) % 256);
                //功能代码
                msg[22] = 0x00;
                msg[23] = Convert.ToByte(Convert.ToInt32(type));  //function code:GETC: 112的16进制是70   PUTC: 128 的16进制是80  NOFUNC: 0
                //UL type for LH1
                msg[24] = 0x00;
                msg[25] = 0x01;
                //UL type for LH2
                msg[26] = 0x00;
                msg[27] = 0x00;
                //Setpoint LH;货叉编号，单叉为1
                msg[28] = 0x00; 
                msg[29] = 0x01;  
                #region  IC入库站台和，OC出库动作端的定义  输送机地址编号
                if (station != null && station.Length == 4)
                {
                    actionpoint = 0;
                    switch (int.Parse(station))
                    {
                        case 5637: actionpoint = 2; break;//ic01-1
                        case 5647: actionpoint = 3; break;//ico2-1
                        case 5622: actionpoint = 4; break;//ico2-2
                        case 5638: actionpoint = 5; break;//oc01-1
                        case 5628: actionpoint = 6; break;//oco2-1
                        case 5627: actionpoint = 7; break;//oco2-2
                       default: actionpoint = 0; break;
                    }

                    for (int i = 32; i < 48; i++)
                        msg[i] = 0x00;
                }
                else
                {
                    //巷道号
                    int aisle1 = int.Parse(num.Substring(2, num.Length - 2));
                    msg[33] = (byte)aisle1;
                    //RACK SIDE 1,left;2,right
                    msg[34] = 0x00;
                    msg[35] = (byte)GetRackSide(num, station);
                    //RACK FIELD 行
                    msg[36] = 0x00;
                    msg[37] = (byte)GetRackLine(station);
                    //rack place  货格里的第几个位置
                    msg[38] = 0x00; 
                    msg[39] = 0x01;
                    // rack level 货位层
                    msg[40] = 0x00;
                    msg[41] = (byte)GetRackLayer(station);
                    //rack depth 深度。深度,1:单深度；2：双深度  rack depth
                    msg[42] = 0x00; 
                    msg[43] = (byte)GetRackDepth(num, station);

                    for (int i = 44; i < 68; i++)
                        msg[i] = 0x00;
                }
                #endregion
                msg[30] = 0x00;                  //设置堆垛机的动作点
                msg[31] = (byte)actionpoint;     //设置堆垛机的动作点
                string commandType = string.Empty;
                switch (type.ToString())
                {
                    case "112": commandType = "取货指令"; break;
                    case "128": commandType = "放货指令"; break;
                    case "0": commandType = "清除指令"; break;
                    case "96": commandType = "盘点指令"; break;
                    case "1": commandType = "取放指令"; break;
                    case "16": commandType = "移动指令"; break;
                    default: break;
                }

                bool sendflag = SendMsg(msg);
                if (sendflag)
                {
                    AnalyseMessageEvent(NotifyCommand.SendData, m_strSRMNo, m_serverPort, string.Format("{0}下发{1}任务:{2},指令:{3},地址:{4}", isMan ? "手动" : "WCS自动", num, taskno, commandType, station));
                }
                else
                {
                    AnalyseMessageEvent(NotifyCommand.Break, m_strSRMNo, m_serverPort, string.Format("{0},Ip:{1},Port:{2} 指令下发失败！", m_strSRMNo, m_serverIP, m_serverPort));
                    LogUtil.Write(LogFileName.ScLog, LogFileName.ScLog, $"{num}SendCode发送指令失败:{type},{taskno},{station}");
                }
                LogUtil.Write(LogFileName.ScLog, LogFileName.ScLog, $"WCS向{num}发送指令成功:{type};任务号:{taskno};目标地址:{station};{(isMan?"手动":"自动")}");
                return sendflag;
            }
            catch (Exception ex)
            {
                LogUtil.Write(LogFileName.ScExceptionLog, LogFileName.ScExceptionLog, $"{num}SendCode发送指令异常:{type},{taskno},{station},{ex.Message}{ex.StackTrace}");
                return false;
            }
        }
    }
}



