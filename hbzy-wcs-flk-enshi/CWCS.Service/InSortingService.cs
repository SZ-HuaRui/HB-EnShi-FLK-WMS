using CWCS.Common.Uitl;
using CWCS.Core.Library.PLCManager;
using CWCS.Entity;
using CWCS.Entity.Convery;
using CWCS.NetWork;
using LOG;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CWCS.Repository;

namespace CWCS.Service
{
    /// <summary>
    /// Ahthor:zpw
    /// Description:Logic of InSorting
    /// </summary>
    public class InSortingService
    {
        public static string InSortingLineIP = System.Configuration.ConfigurationManager.AppSettings["InSortingLineIP"].ToString();
        public static string InSortingLinePort = System.Configuration.ConfigurationManager.AppSettings["InSortingLinePort"].ToString();
        public static string[] SecondInSortingDock = System.Configuration.ConfigurationManager.AppSettings["SecondInSortingDock"].ToString().Split(';');
        public static string[] ThirdInSortingDock = System.Configuration.ConfigurationManager.AppSettings["ThirdInSortingDock"].ToString().Split(';');
        public delegate void DelegateShowText(string message);
        public delegate void DelegateShowSetDockTable(DataTable dt);
        public delegate void DelegateShowDockStauts(Dictionary<string,string> dic);
        public event DelegateShowText ShowWCSClientEvent;
        public event DelegateShowText ShowWCSServerEvent;
        public event DelegateShowText ShowConnectEvent;
        public event DelegateShowText ShowRunStatus;
        public event DelegateShowDockStauts ShowDockStatusEvent;
        public event DelegateShowSetDockTable ShowSetDockTable;
        public SocketClientBoth socketClient = null;
        public SocketServerForReceive socketServer = null;
        public Timer SetDockTimer = null;
        public object setDockLocker = new object();

        public OracleRepository _repository = new OracleRepository();
        public char[] trimArray = new char[] { ' ', '\0' };
        
        public Log DestinationRequestLog = new Log("目的地请求日志", ".\\入库分拣\\目的地请求日志\\");
        public Log InSortingReportLog = new Log("分拣报告", ".\\入库分拣\\分拣报告\\");
        public Log SystemStatusLog = new Log("系统状态日志", ".\\入库分拣\\系统状态日志\\");
        public Log DockStatusLog = new Log("道口状态日志", ".\\入库分拣\\道口状态日志\\");
        public Log InSortingExceptionLog = new Log("程序异常日志", ".\\入库分拣\\程序异常日志\\");
        public int DockFlag = 9;
        private Thread InSortingThread = null;
        public InSortingService()
        {
            socketClient = new SocketClientBoth(InSortingLineIP, InSortingLinePort);
            socketClient.NotifyByteEvent += ByteHandleWCS;
            socketClient.NotifyStringEvent += StringHandleWCSClient;
            InSortingThread = new Thread(ConnectInSorting);
            InSortingThread.IsBackground = true;
        }


        public bool StartServer()
        {
            bool successFlag = true;
            string logMessage = string.Empty;

            if (socketClient.Connect())
            {
                logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:WCS客户端连接分拣线({socketClient?.IP})成功！";
            }
            else
            {
                logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:WCS客户端连接分拣线({socketClient?.IP})失败！";
                successFlag = false;
            }
            WriteWCSClientLog(LogType.SystemStatus,logMessage);
            LoadDockStauts();
            return successFlag;
        }

        private void LoadDockStauts()
        {
            if (StaticCommon.DockStatusDic.Count == 0)
            {
                for (int i=1;i<=11;i++)
                {
                    StaticCommon.DockStatusDic.Add($"{i.ToString()}",$"{i.ToString()}号道正常");
                }
            }
            ShowDockStatusEvent?.Invoke(StaticCommon.DockStatusDic);
        }


        private void StringHandleWCSClient(SocketEvent socketEvent, Socket socket, string message)
        {
            if (socketEvent == SocketEvent.Break)
            {
                ShowConnectEvent?.Invoke("通信断开");
            }
            if (socketEvent == SocketEvent.Connected)
            {
                ShowConnectEvent?.Invoke("正常运行");
            }
            WriteWCSClientLog(LogType.SystemStatus, message);
        }
       

        /// <summary>
        /// 有效大数据报文
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        private bool IsValidBigDataPacket(byte[] byteArray)
        {
            try
            {
                if (byteArray == null) return false;
                if (byteArray.Length < 116) return false;
                if (!(byteArray[0] == 0xFE && byteArray[1] == 0xFE && byteArray[2] == 0xFE && byteArray[3] == 0xFE)) return false;
                return true;
            }
            catch
            {
                return false;
            }
           
        }

        /// <summary>
        /// 有效小数据报文
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        private bool IsVaildSmallDataPacket(byte[] byteArray)
        {
            try
            {
                if (byteArray == null) return false;
                if (byteArray.Length != 80) return false;
                if (!(byteArray[0] == 0xFC && byteArray[1] == 0xFC)) return false;
                return true;
            }
            catch
            {
                return false;
            }
           
        }


        public void ByteHandleWCS(SocketEvent socketEvent, Socket socket, byte[] byteArray)
        {
            try
            {
               
                string packetType = string.Empty;

                if (IsValidBigDataPacket(byteArray))
                {
                   
                    int i = 0;
                    while (byteArray[36 + i] == 0xFC && byteArray[36 + i + 1] == 0xFC)
                    {
                        byte[] smallPacket = new byte[80];
                        Array.Copy(byteArray, 36 + i, smallPacket, 0, 80);
                        packetType = Encoding.ASCII.GetString(smallPacket, 2, 4);
                        if (packetType.Equals("P1HB"))//心跳
                        {
                            socket?.Send(PackSmallPacket(NetFixedCommand.InSortingHeartBeat));
                        }
                        if (packetType.Equals("P3DR"))//目的地请求
                        {
                            WriteReportLog("分拣线发送给WCS目的请求报文:",smallPacket);
                            smallPacket = DestinationPacket(smallPacket);
                            byte[] bigPacket = null;
                            if (smallPacket != null) bigPacket = PackSmallPacket(smallPacket);
                            if (bigPacket != null)
                            {
                                socket.Send(bigPacket);
                                WriteReportLog("WCS反馈给分拣线目的地报文:",bigPacket);
                            }
                        }
                        if (packetType.Equals("P3SR"))//分拣报告
                        {
                            InSortingResutlDeal(smallPacket);
                        }
                        if (packetType.Equals("P5SS"))//系统状态
                        {
                            PLCSystemStatusDeal(smallPacket);
                        }
                        if (packetType.Equals("P6IF"))//设备状态
                        {
                            DockStatusDeal(smallPacket);
                        }
                        if (packetType.Equals("P3QR"))//清道状态返回
                        {
                            CleanDockResponseDeal(smallPacket);
                        }
                        i = i + 80;
                    }
                }

                if (IsVaildSmallDataPacket(byteArray))
                {
                    byte[] smallPacket = new byte[80];
                    packetType = Encoding.ASCII.GetString(byteArray, 2, 4);
                    if (packetType.Equals("P1HB"))//心跳
                    {
                        socket?.Send(PackSmallPacket(NetFixedCommand.InSortingHeartBeat));
                    }
                    if (packetType.Equals("P3DR"))//目的地请求
                    {
                        smallPacket = DestinationPacket(byteArray);
                        if (smallPacket != null) socket.Send(PackSmallPacket(byteArray));
                    }
                    if (packetType.Equals("P3SR"))//分拣报告
                    {
                        InSortingResutlDeal(byteArray);
                    }
                    if (packetType.Equals("P5SS"))//系统状态
                    {
                        PLCSystemStatusDeal(byteArray);
                    }
                    if (packetType.Equals("P6IF"))//设备状态
                    {
                        DockStatusDeal(byteArray);
                    }
                    if (packetType.Equals("P3QR"))//清道状态返回
                    {
                        CleanDockResponseDeal(byteArray);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteWCSClientLog(LogType.Exception,$"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:ByteHandleWCS处理分拣线指令失败！{ex.Message},{ex.StackTrace}");
                StringBuilder writeStr = new StringBuilder();
                if (byteArray.Length > 0)
                {
                    foreach (byte b in byteArray)
                    {
                        writeStr.Append(b.ToString("X2"));
                    }
                    InSortingExceptionLog.WriteLog($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:{writeStr.ToString()}");
                }
            }
          
        }

       


        public void WriteWCSClientLog(LogType logType, string message)
        {
            if (logType == LogType.DestinationRequest)
            {
                ShowWCSClientEvent.Invoke(message);
                DestinationRequestLog.WriteLog(message);
            }
            if (logType == LogType.DockStatus)
            {
                DockStatusLog.WriteLog(message);
            }
            if (logType == LogType.SortingReptort)
            {
                ShowWCSServerEvent.Invoke(message);
                InSortingReportLog.WriteLog(message);
            }
            if (logType == LogType.SystemStatus)
            {
                if (!message.Contains("运行模式"))
                {
                    ShowWCSClientEvent.Invoke(message);
                }
                SystemStatusLog.WriteLog(message);
            }
            if (logType == LogType.Exception)
            {
                ShowWCSClientEvent.Invoke(message);
                InSortingExceptionLog.WriteLog(message);
            }
        }


        public byte[] DestinationPacket(byte[] recieveArray)
        {
            try
            {
                string brandFloor = string.Empty;
                string scannerNO = string.Empty;
                string scanCode = string.Empty;
                string itemCode = string.Empty;
                string allSetDockFloor = string.Empty;
                string FXNO = string.Empty;
                Random random = new Random();
                int dockIndex = 0;
                int exceptionDock = 0;
                string logMessage = string.Empty;
                if (recieveArray == null) return null;
                if (recieveArray.Length != 80) return null;
                scannerNO = DataTransformUtil.TwoByteToInt(recieveArray[8], recieveArray[9]).ToString();
                scanCode = Encoding.ASCII.GetString(recieveArray, 14, 32).Trim(trimArray);
                //scanCode = "04501103N1Y0319102616270790";
                logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:扫码器：{scannerNO},条码：{scanCode}，分拣线向WCS请求目标道口...";
                ShowWCSServerEvent(logMessage);
                DestinationRequestLog.WriteLog(logMessage);
                Array.Copy(Encoding.ASCII.GetBytes("W3DS"), 0, recieveArray, 2, 4);
                if (scannerNO == null) return null;
                if (scannerNO.Equals("2"))
                {
                    if (!string.IsNullOrEmpty(scanCode) && scanCode.Length == 27) FXNO = scanCode.Substring(11, 2);
                    SaveOneCode(scanCode, FXNO);
                }
                DataTable dockFloorTable = GetAllSetDockFloor();
                if (dockFloorTable != null && dockFloorTable.Rows.Count >0)
                {
                    foreach (DataRow dr in dockFloorTable.Rows)
                    {
                        allSetDockFloor += dr["floor"].ToString();
                    }
                }
                if (string.IsNullOrEmpty(scanCode) || scanCode.Contains("?") || (scanCode.Length != 32 && scanCode.Length != 27))
                {
                    byte[] exceptionResult = GetExceptionDockArray(scannerNO, scanCode, allSetDockFloor, brandFloor, recieveArray, ref exceptionDock);
                    logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:扫码器：{scannerNO},条码：{scanCode},条码格式或长度不正确，剔除到异常口{exceptionDock.ToString()}";
                    WriteWCSClientLog(LogType.DestinationRequest, logMessage);
                    return exceptionResult;
                }
                if (scanCode.Length == 32)
                {
                    itemCode = scanCode.Substring(2, 6);
                }
                if (scanCode.Length == 27)
                {
                    itemCode = scanCode.Substring(0, 11).TrimEnd(new char[]{'0' ,'Y'});
                    FXNO = scanCode.Substring(11, 2);
                }

                if (scannerNO.Equals("1"))
                {
                    return null;
                }

                DataTable dt = new DataTable();
                logMessage = GetDockInfo(scanCode, itemCode, FXNO, ref dt);

                if (dt == null || dt.Rows.Count == 0)
                {
                    byte[] exceptionResult = GetExceptionDockArray(scannerNO, scanCode, allSetDockFloor, brandFloor, recieveArray, ref exceptionDock);
                    logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:扫码器：{scannerNO},条码：{scanCode},没有设道或者设道错误,剔除到异常口{exceptionDock.ToString()}。详细信息：{logMessage}";
                    WriteWCSClientLog(LogType.DestinationRequest, logMessage);
                    return exceptionResult;
                }

                List<DockInfo> dockList = new List<DockInfo>();
                foreach (DataRow dr in dt.Rows)
                {
                    DockInfo info = new DockInfo();
                    info.DockNO = dr["equipment_number"].ToString();
                    info.StationNO = dr["station_no"].ToString();
                    info.ItemCode = dr["item_code"].ToString();
                    dockList.Add(info);
                }

                for (int i = 0;i < dockList.Count;i++)
                {
                    if (SecondInSortingDock.Contains(dockList[i].DockNO))
                    {
                        brandFloor += "2;";
                    }
                    else
                    {
                        brandFloor += "3;";
                    }
                    if (StaticCommon.DockStatusDic.Keys.Contains(dockList[i].DockNO) && !StaticCommon.DockStatusDic[dockList[i].DockNO].Contains("正常"))
                    {
                        dockList.Remove(dockList[i]);
                    }
                }


                if (dockList == null || dockList.Count == 0)
                {
                    byte[] exceptionResult = GetExceptionDockArray(scannerNO, scanCode, allSetDockFloor, brandFloor, recieveArray, ref exceptionDock);
                    logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:扫码器：{scannerNO},条码：{scanCode},道口堵塞，无可用道口，剔除到异常口{exceptionDock}";
                    WriteWCSClientLog(LogType.DestinationRequest, logMessage);
                    return exceptionResult;
                }
               

                if (scannerNO.Equals("1") || scannerNO.Equals("2"))
                {
                    dockIndex = random.Next(0, dockList.Count);
                    Array.Copy(DataTransformUtil.IntToTwoByte(Convert.ToInt32(dockList[dockIndex].DockNO)), 0, recieveArray, 12, 2);
                }
                if (scannerNO.Equals("3"))
                {
                    dockList = dockList.Where(x => ThirdInSortingDock.Contains(x.DockNO)).ToList();
                    if (dockList == null || dockList.Count == 0)
                    {
                        exceptionDock = 13;
                        Array.Copy(DataTransformUtil.IntToTwoByte(exceptionDock), 0, recieveArray, 12, 2);
                        logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:扫码器：{scannerNO},条码：{scanCode},找不到目标道口,剔除到道口{exceptionDock}";
                        WriteWCSClientLog(LogType.DestinationRequest, logMessage);
                        return recieveArray;
                    }
                    dockIndex = random.Next(0, dockList.Count);
                    Array.Copy(DataTransformUtil.IntToTwoByte(Convert.ToInt32(dockList[dockIndex].DockNO)), 0, recieveArray, 12, 2);
                }
               
                logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:扫码器：{scannerNO},条码：{scanCode},WCS反馈给分拣线目标道口：{dockList[dockIndex].DockNO}";
                WriteWCSClientLog(LogType.DestinationRequest, logMessage);
                return recieveArray;
            }
            catch (Exception ex)
            {
                WriteWCSClientLog(LogType.Exception, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:异常位置：DestinationPacket。{ex.Message},{ex.StackTrace}");
                return new byte[1];
            }
        }

        public void WriteReportLog(string message,byte[] bytes)
        {
            StringBuilder writeStr = new StringBuilder();
            if (bytes.Length > 0)
            {
                foreach (byte b in bytes)
                {
                    writeStr.Append(b.ToString("X2"));
                }
                DestinationRequestLog.WriteLog($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:{message}。{writeStr.ToString()}");
            }
        }
        /// <summary>
        /// 保存一号工程码、质量追溯码
        /// </summary>
        /// <param name="oneCode"></param>
        public void SaveOneCode(string oneCode,string FXNO)
        {
            _repository.SaveOneCode(oneCode,FXNO);
        }

        private byte[] GetExceptionDockArray(string scannerNO,string scanCode,string allSetDockFloor,string brandFloor,byte[] array,ref int exceptionDock)
        {
            //14 12 13
            switch (scannerNO)
            {
                case "1": exceptionDock = 14; Array.Copy(DataTransformUtil.IntToTwoByte(exceptionDock), 0, array, 12, 2); break;
                case "2":
                    {
                        if (string.IsNullOrEmpty(brandFloor))
                        {
                            if (string.IsNullOrEmpty(allSetDockFloor))
                            {
                                exceptionDock = 12;
                            }
                            else if(allSetDockFloor.Contains("2") && allSetDockFloor.Contains("3"))
                            {
                                exceptionDock = 12;
                            }
                            else if (allSetDockFloor.Contains("3"))
                            {
                                exceptionDock = 13;
                            }
                            else
                            {
                                exceptionDock = 12;
                            }

                        }
                        else if(brandFloor.Contains("2") && brandFloor.Contains("3"))
                        {
                            exceptionDock = 12;
                        }
                        else if(brandFloor.Contains("3"))
                        {
                            exceptionDock = 13;
                        }
                        else
                        {
                            exceptionDock = 12;
                        }
                        Array.Copy(DataTransformUtil.IntToTwoByte(exceptionDock), 0, array, 12, 2);
                        break;
                    } 
                case "3": exceptionDock = 13; Array.Copy(DataTransformUtil.IntToTwoByte(exceptionDock), 0, array, 12, 2); break;
            }
            return array;
        }

        /// <summary>
        /// 封装小报文
        /// </summary>
        /// <param name="recieveArray"></param>
        /// <returns></returns>
        private byte[] PackSmallPacket(byte[] recieveArray)
        {
            byte[] bigPacket = new byte[1];
            try
            {
                byte[] start = new byte[36];
                start[0] = 0xFE;
                start[1] = 0xFE;
                start[2] = 0xFE;
                start[3] = 0xFE;
                Random random = new Random();
                int packetSequnceNO = random.Next(1, 60000);
                //数据包序号
                Array.Copy(DataTransformUtil.IntToFourByte(packetSequnceNO), 0, start, 4, 4);
                //和校验值
                start[10] = 0;
                start[11] = 0;
                //ACK确认数据包序号
                start[12] = 0;
                start[13] = 0;
                start[14] = 0;
                start[15] = 0;
                //ACK状态
                start[16] = 0;
                start[17] = 0;
                //发送设备编号
                start[18] = 0;
                start[19] = 0;
                //接受设备编号
                start[20] = 0;
                start[21] = 0;
                //备用
                for (int i = 22; i <= 35; i++)
                {
                    start[i] = 0;
                }
                int totalPacketLength = 840;
                Array.Copy(DataTransformUtil.IntToTwoByte(totalPacketLength), 0, start, 8, 2);
                bigPacket = new byte[totalPacketLength];
                Array.Copy(start, 0, bigPacket, 0, start.Length);
                Array.Copy(recieveArray, 0, bigPacket, start.Length, recieveArray.Length);
                for (int i = 1; i <= 4; i++)
                {
                    bigPacket[bigPacket.Length - i] = 0xFF;
                }
            }
            catch (Exception ex)
            {
                WriteWCSClientLog(LogType.Exception, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:异常位置：PackSmallPacket。{ex.Message},{ex.StackTrace}");
            }
            return bigPacket;
        }


        private void InSortingResutlDeal(byte[] recieveArray)
        {
            string logMessage = string.Empty;
            try
            {
                int result = DataTransformUtil.TwoByteToInt(recieveArray[8], recieveArray[9]);
                int WCSDestination = DataTransformUtil.TwoByteToInt(recieveArray[10], recieveArray[11]);
                int PLCDestination = DataTransformUtil.TwoByteToInt(recieveArray[12], recieveArray[13]);
                int InSortingStatusID = DataTransformUtil.TwoByteToInt(recieveArray[14], recieveArray[15]);
                string errorInfo = string.Empty;
                string scanCode = Encoding.ASCII.GetString(recieveArray,16,34).Trim(trimArray);
                int weightFlag = DataTransformUtil.TwoByteToInt(recieveArray[48], recieveArray[49]);
                switch (InSortingStatusID)
                {
                    case 0: errorInfo = "分拣正常"; break;
                    case 1: errorInfo = "异常:没有读到条码"; break;
                    case 2: errorInfo = "异常:没有读码器的任何信息"; break;
                    case 3: errorInfo = "异常:多条码故障"; break;
                    case 4: errorInfo = "异常:WCS未反馈目的地"; break;
                    case 5: errorInfo = "异常:出口故障（或满格）"; break;
                    case 6: errorInfo = "异常:人为干预"; break;
                    case 7: errorInfo = "异常:目的地地址无效（WCS有反馈，但是不在设定的范围之内）"; break;
                    case 8: errorInfo = "异常:跟踪丢失"; break;
                    case 9: errorInfo = "异常:超过最大循环数"; break;
                    case 10: errorInfo = "异常:间距错误"; break;
                    case 99: errorInfo = "异常:分拣状态ID：99，错误原因未知，请联系分拣线！"; break;
                    default: errorInfo = $"异常:分拣状态ID：{InSortingStatusID}，不在设定的范围之内，请联系分拣线！"; break;
                }
                logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:条码：{scanCode},{(InSortingStatusID == 0 ? "分拣成功！" : "分拣失败！")},WCS分配目的地：{WCSDestination.ToString()},分拣线执行目的地：{PLCDestination.ToString()}。详细信息：{errorInfo}。";
                WriteWCSClientLog(LogType.SortingReptort, logMessage);
                if (weightFlag.Equals(1))
                {
                    errorInfo = "重量异常";
                }
                SaveInSortingReport(scanCode,WCSDestination.ToString(),PLCDestination.ToString(), errorInfo);
            }
            catch (Exception ex)
            {
                WriteWCSClientLog(LogType.Exception, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:异常位置：InSortingResutlDeal。{ex.Message},{ex.StackTrace}");
            }
        }

        private void PLCSystemStatusDeal(byte[] recieveArray)
        {
            string logMessage = string.Empty;
            try
            {
                int systemStatus = DataTransformUtil.TwoByteToInt(recieveArray[8], recieveArray[9]);
                int runStatus = DataTransformUtil.TwoByteToInt(recieveArray[10], recieveArray[11]);
                string systemDes = systemStatus == 1 ? "正常运行" : "停止运行";
                string runDes = runStatus == 1 ? "手动" : "自动";
                ShowConnectEvent?.Invoke(systemDes);
                ShowRunStatus?.Invoke(runDes);
                logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:系统状态标记：{systemStatus.ToString()},{systemDes}。系统运行模式标记：{runStatus.ToString()},{runDes}";
                WriteWCSClientLog(LogType.SystemStatus, logMessage);
            }
            catch(Exception ex)
            {
                WriteWCSClientLog(LogType.Exception, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:异常位置：PLCSystemStatusDeal。{ex.Message},{ex.StackTrace}");
            }
            
        }

        private void DockStatusDeal(byte[] recieveArray)
        {
            string logMessage = string.Empty;
            try
            {
                int dockNO = 0;
                int dockStatusID = 0;
                string dockStautusDes = string.Empty;
                logMessage = string.Empty;
                dockNO = DataTransformUtil.TwoByteToInt(recieveArray[8], recieveArray[8 + 1]);
                dockStatusID = DataTransformUtil.TwoByteToInt(recieveArray[8 + 2], recieveArray[8 + 3]);
                switch (dockStatusID)
                {
                    case 0: dockStautusDes = $"{dockNO}号道正常"; break;
                    case 1: dockStautusDes = $"{dockNO}号道故障"; break;
                    case 2: dockStautusDes = $"{dockNO}号道堵塞"; break;
                    default: dockStautusDes = $"{dockNO}号道正常"; break;
                }
                logMessage += dockStautusDes + ";";
                if (StaticCommon.DockStatusDic.Keys.Contains(dockNO.ToString()))
                {
                    StaticCommon.DockStatusDic[dockNO.ToString()] = dockStautusDes;
                }
             
                WriteWCSClientLog(LogType.DockStatus,logMessage);
                ShowDockStatusEvent?.Invoke(StaticCommon.DockStatusDic);
            }
            catch(Exception ex)
            {
                WriteWCSClientLog(LogType.Exception, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:异常位置：DockStatusDeal。{ex.Message},{ex.StackTrace}");
            }
            
        }

        private void CleanDockResponseDeal(byte[] recieveArray)
        {
            string logMessage = string.Empty;
            try
            {
                int dockNO = 0;
                int cleanStatusID;
                int boxQty;
                string stationNO = string.Empty;
                logMessage = string.Empty;
                string cleanFlag = string.Empty;
                dockNO = DataTransformUtil.TwoByteToInt(recieveArray[8], recieveArray[9]);
                cleanStatusID = DataTransformUtil.TwoByteToInt(recieveArray[10], recieveArray[11]);//1 success,2 fail
                boxQty = DataTransformUtil.TwoByteToInt(recieveArray[12], recieveArray[13]);
                if (!_repository.SetPLCFlag(dockNO.ToString(), cleanStatusID))
                {
                    Thread.Sleep(1000);
                    _repository.SetPLCFlag(dockNO.ToString(), cleanStatusID);
                }
                if (cleanStatusID == 1)
                {
                    DataTable dt = _repository.GetBrandInfoByDock(dockNO.ToString());
                    if (dt != null && dt.Rows.Count == 1)
                    {
                        cleanFlag = dt.Rows[0]["canedit"].ToString();
                        stationNO = dt.Rows[0]["stationno"].ToString();
                        if (!cleanFlag.Equals("1"))
                        {
                            if (!_repository.SetCleanFlag(dockNO.ToString(), 2))
                            {
                                Thread.Sleep(1000);
                                _repository.SetCleanFlag(dockNO.ToString(), 2);
                            }

                            if (!string.IsNullOrEmpty(stationNO))
                            {
                                CleanDockToConveyer(dockNO.ToString(), stationNO);
                            }
                        }
                    }
                }
                ShowSetDockTable(GetAllDockInfo());
                logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:道口{dockNO},分拣线返回清道{(cleanStatusID == 1 ? "成功!" : "失败!")}道口烟箱数量：{boxQty.ToString()}";
                ShowWCSServerEvent(logMessage);
                WriteWCSClientLog(LogType.DockStatus,logMessage);
            }
            catch(Exception ex)
            {
                WriteWCSClientLog(LogType.Exception, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:异常位置：CleanDockResponseDeal。{ex.Message},{ex.StackTrace}");
            }
        }

        public void RequestSystemStatus()
        {
            try
            {
                string logMessage = string.Empty;
                byte[] samllPacket = new byte[80];
                samllPacket[0] = 0xFC;
                samllPacket[1] = 0xFC;
                samllPacket[2] = Convert.ToByte('W');
                samllPacket[3] = Convert.ToByte('5');
                samllPacket[4] = Convert.ToByte('S');
                samllPacket[5] = Convert.ToByte('R');
                Array.Copy(DataTransformUtil.IntToTwoByte(80), 0, samllPacket, 6, 2);
                Array.Copy(DataTransformUtil.IntToTwoByte(1), 0, samllPacket, 8, 2);
                for (int i = 10; i <= 77; i++)
                {
                    samllPacket[i] = 0;
                }
                samllPacket[78] = 0xFD;
                samllPacket[79] = 0xFD;
                byte[] bigPacket = PackSmallPacket(samllPacket);
                socketClient.CoreSocket?.Send(bigPacket);
            }
            catch(Exception ex)
            {
                WriteWCSClientLog(LogType.Exception, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:异常位置：CleanDockResponseDeal。{ex.Message},{ex.StackTrace}");
            }
          
        }

        private string GetDockInfo(string scanCode,string itemCode,string fxNO,ref DataTable result)
        {
            string message = _repository.GetDestinationDock(scanCode,itemCode,fxNO,ref result);
            return message;
        }
        public void CleanAllDock()
        {
            string logMessage = string.Empty;
            bool IsSuccess = true;
            try
            {
                DataTable dt = GetAllDockInfo();
                foreach (DataRow dr in dt.Rows)
                {
                    if (!CleanDockToSorting(dr["dockno"].ToString()))
                    {
                        IsSuccess = false;
                    }
                    if (IsSuccess)
                    {
                        SetCleanFlag(dr["dockno"].ToString(), 0);
                        SetPLCFlag(dr["dockno"].ToString(), 0);
                    }
                    IsSuccess = true;
                    Thread.Sleep(200);
                }
                WriteWCSClientLog(LogType.DockStatus,logMessage);
                ShowWCSClientEvent.Invoke(logMessage);
            }
            catch (Exception ex)
            {
                logMessage = $"\n{DateTime.Now:yyyy-MM-dd HH:mm:ss}:清道指令下发出现异常。异常位置：CleanAllDock。{ex.Message},{ex.StackTrace}";
                WriteWCSClientLog(LogType.Exception,logMessage);
            }
        }
      

      

        public bool CleanDockToConveyer(string dockNO,string stationNO)
        {
            if (!string.IsNullOrEmpty(stationNO) || ConveryPLC.StaticBoxTypeList.Count > 0 && ConveryPLC.StaticBoxTypeList.Exists(x => x.StationNo.Equals(stationNO)))
            {
                ConveryEntity entity = ConveryPLC.StaticBoxTypeList.Where(x => x.StationNo.Equals(stationNO)).ToList()[0];
                if (entity != null)
                {
                    byte[] array = new byte[28];
                    if(!ConveryPLC.PLCList[entity.Key_ip].Write(entity.DBAddress, entity.value, entity.length, array))
                    {
                        ShowWCSClientEvent?.Invoke($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:道口：{dockNO}，站台：{stationNO},下发清道指令给输送机失败！");
                        return false;
                    }
                    else
                    {
                        ShowWCSClientEvent?.Invoke($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:道口：{dockNO}，站台：{stationNO},下发清道指令给输送机成功！");
                       
                    }
                }
            }
            return true;
        }

        private void ConnectInSorting()
        {
            while (true)
            {
                try
                {
                    PLCHelper.PLCList[InSortingLineIP].DisConnect();
                    ShowWCSClientEvent?.Invoke($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:WCS与分拣线PLC{InSortingLineIP}断开连接,正在尝试重连...");
                    if (ConveryPLC.PLCList[InSortingLineIP].Connect())
                    {
                        ShowWCSClientEvent?.Invoke($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:WCS与分拣线PLC{InSortingLineIP}重连成功！");
                        break;
                    }
                    else
                    {
                        ShowWCSClientEvent?.Invoke($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:WCS与分拣线PLC{InSortingLineIP}重连失败！");
                       
                    }
                }
                catch(Exception ex)
                {
                    ShowWCSClientEvent?.Invoke($"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:WCS与分拣线PLC{InSortingLineIP}重连出现异常！{ex.Message}");
                }
                finally
                {
                    Thread.Sleep(1000);
                }
                
            }
            
        }

        public bool SetCleanFlag(string dockNO,int flag)
        {
            return _repository.SetCleanFlag(dockNO, flag);
        }

        public bool SetPLCFlag(string dockNO, int flag)
        {
            return _repository.SetPLCFlag(dockNO, flag);
        }
        public void SaveInSortingReport(string scanCode, string wcsDestination, string plcDestination, string exceptionDescription)
        {
            _repository.Wcs_SaveInSortingReport(scanCode, wcsDestination, plcDestination, exceptionDescription);
        }
        public bool SetInSortingFloor(string flag)
        {
            return _repository.SetInSortingFloor(flag);
        }
        public bool SwitchFloorToSorting(string flag)
        {
            try
            {
                if (!ConveryPLC.PLCList[InSortingLineIP].Write(54, 262, 2, DataTransformUtil.IntToTwoByte(Convert.ToInt32(flag))))
                {
                    if(InSortingThread.ThreadState != ThreadState.Running) InSortingThread.Start();
                    return false;
                }
            }
            catch
            {
                if (InSortingThread.ThreadState != ThreadState.Running) InSortingThread.Start();
                return false;
            }
            return true;
            
        }
        public bool SwitchFloor(string flag)
        {
            if (!SwitchFloorToSorting(flag)) return false;
            if (!SetInSortingFloor(flag)) return false;
            return true;
        }

        public bool CleanDockToSorting(string dockNO)
        {
            string logMessage = string.Empty;
            try
            {
                bool IsSuccess = false;
                byte[] smallPacket = new byte[80];
                smallPacket[0] = 0xFC;
                smallPacket[1] = 0xFC;
                Array.Copy(Encoding.ASCII.GetBytes("W3QS"),0,smallPacket,2,4);
                Array.Copy(DataTransformUtil.IntToTwoByte(80), 0, smallPacket, 6, 2);
                Array.Copy(DataTransformUtil.IntToTwoByte(Convert.ToInt32(dockNO)), 0, smallPacket, 8, 2);
                Array.Copy(DataTransformUtil.IntToTwoByte(1), 0, smallPacket, 10, 2);
                smallPacket[78] = 0xFD;
                smallPacket[79] = 0xFD;
                logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:道口：{dockNO},下发清道指令给分拣线失败！。";
                if (socketClient.Send(PackSmallPacket(smallPacket)))
                {
                    logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:道口：{dockNO},下发清道指令给分拣线成功！";
                    IsSuccess = true;
                }
                ShowWCSClientEvent.Invoke(logMessage);
                DockStatusLog.WriteLog(logMessage);
                return IsSuccess;
            }
            catch (Exception ex)
            {
                logMessage = $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:{dockNO}号道口,清道指令下发失败。{ex.Message},{ex.StackTrace}";
                WriteWCSClientLog(LogType.Exception,logMessage);
                return false;
            }
        }

        public DataTable GetAllDockInfo()
        {
            return _repository.GetAllDockInfo();
        }


        public enum LogType : int
        {
            DestinationRequest = 0,
            SystemStatus = 1,
            DockStatus = 2,
            SortingReptort = 3,
            Exception = 4,
        }

        public bool InsertWcsLog(string message)
        {
            return _repository.InsertEventlog(message);
        }

        public DataTable GetAllSetDockFloor()
        {
            return _repository.GetAllSetDockFloor();
        }

        public string GetSwitchFloorValue()
        {
            return _repository.GetSwitchFloorValue();
        }
    }
}
