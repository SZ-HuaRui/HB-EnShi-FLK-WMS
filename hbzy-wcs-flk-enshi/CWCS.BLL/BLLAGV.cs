using System;
using System.Timers;
using System.Data;
using COMResolution;
using System.Threading;
using CWCS.Entity.SRM;
using CWCS.Entity;
using System.Linq;
using CWCS.BLL;
using CWCS.Common.Log;
using CWCS.Interface.Request.WMS;
using CWCS.Interface.WMSMicroService;
using CWCS.Interface.Response.WMS;
using Newtonsoft.Json;
using CWCS.Equipment.Led;
using CWCS.Common.Uitl;
using CWCS.Core.Library.AGVManager;
using System.Security.Cryptography;

namespace BLL
{
    public class BLLAGV : BLLBase
    {


        public CRLAgvControl agvclass;
        private System.Timers.Timer ScanDbTimer = new System.Timers.Timer();
        private object locker = new object();
        public delegate void Sys_conn(string command, object state);
        public event Sys_conn GetConnectStatus;
        public delegate void NotifyTask(DataTable dt);
        public event NotifyTask GetTask;
        public int itask = 0;
        public System.Threading.Timer AgvAlarmTimer;
        public string[] locationLine = new string[40] { "1", "2","3", "4", "5", "6", "7", "8", "9", "10"
        ,"11", "12","13", "14", "15", "16", "17", "18", "19", "20"
        ,"21", "22","23", "24", "25", "26", "27", "28", "29", "30"
        ,"31", "32","33", "34", "35", "36", "37", "38", "39", "40" };

        public string AgvNo = string.Empty;

        public void AgvRun()
        {
            agvclass.Notify += new COMResolution.UpdateAGVStatus(agvclass_Notify);
            ScanDbTimer.Interval = Convert.ToDouble(StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("SendAgvTaskInterval")).ToList()[0].PARAM_VALUE) * 1000;
            ScanDbTimer.Elapsed += new ElapsedEventHandler(ScanDbTimer_Elapsed);
            agvclass.Run();

        }

        public BLLAGV(string AgvNo, string Ip, string Port)
        {
            this.AgvNo = AgvNo;
            agvclass = new CRLAgvControl(Ip, Port);
        }


        private void ScanDbTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                lock (locker)
                {
                    int FromStation = 0, Tostation = 0;
                    string fromStationStr = string.Empty;
                    string toStationStr = string.Empty;
                    WMSServicRequest servicRequest = new WMSServicRequest();
                    ResultObject wmsServiceResult = new ResultObject();
                    DataTable dt = _repository.GetSubTaskInfo(AgvNo);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        GetTask?.Invoke(dt);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            try
                            {
                                if ((dt.Rows[i]["taskstatus"].ToString() == "未开始") && dt.Rows[i]["RUNSTATUS"].ToString() == "可执行")
                                {
                                    //DataTable taskTable;
                                    //if (dt.Rows[i]["tostation"].ToString().Equals("5722"))
                                    //{
                                    //    taskTable = _repository.QueryTableData(new WCS_SUBTASKINFO()
                                    //    {
                                    //        EQUIPMENTNUMBER = "AGV02",
                                    //        TASKSTATUS = "2",
                                    //        TOSTATION = "5722"
                                    //    });
                                    //    if (taskTable != null && taskTable.Rows.Count >= 4)
                                    //    {
                                    //        continue;
                                    //    }
                                    //}
                                    if (dt.Rows[i]["tostation"].ToString().Equals("R_201"))
                                    {
                                        bool isExist = _repository.IsExistSCPTask();
                                        if (isExist)
                                        {
                                            continue;
                                        }
                                        DataTable rotationDt = _repository.QueryTableData(
                                                                new WCS_STATION_STATUS()
                                                                {
                                                                    STATION_NO = dt.Rows[i]["tostation"].ToString()
                                                                }
                                                                );
                                        if (rotationDt != null && rotationDt.Rows.Count == 1)
                                        {
                                            if (rotationDt.Rows[0]["exist_pallet"].ToString().Equals("1"))
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    //1401011 AGV货架区
                                    if (DataCheckUtil.IsInteger(dt.Rows[i]["fromstation"].ToString()) && dt.Rows[i]["fromstation"].ToString().Length == 7)
                                    {
                                        fromStationStr = _repository.GetAgvAddressByWcsAddress(dt.Rows[i]["fromstation"].ToString().Substring(0, 4));
                                        if (string.IsNullOrEmpty(fromStationStr))
                                        {
                                            WriteLog($"任务:{dt.Rows[i]["taskno"]},WCS取货地址转换为AGV取货地址时,值为:{fromStationStr}.不合法");
                                            continue;
                                        }
                                        FromStation = Convert.ToInt32(fromStationStr);
                                    }
                                    else
                                    {
                                        if (DataCheckUtil.IsInteger(dt.Rows[i]["fromstation"].ToString()) && locationLine.Contains(dt.Rows[i]["fromstation"].ToString()))
                                        {
                                            fromStationStr = _repository.GetAgvAddressByWcsAddress($"{dt.Rows[i]["fromstation"].ToString().PadLeft(2, '0')}01");
                                        }
                                        else
                                        {
                                            fromStationStr = _repository.GetAgvAddressByWcsAddress(dt.Rows[i]["fromstation"].ToString());
                                        }
                                        if (string.IsNullOrEmpty(fromStationStr))
                                        {
                                            WriteLog($"任务:{dt.Rows[i]["taskno"]},WCS取货地址转换为AGV取货地址时,值为:{fromStationStr}.不合法");
                                            continue;
                                        }
                                        FromStation = Convert.ToInt32(fromStationStr);
                                    }

                                    //目标地址是给定的AGV区
                                    if (DataCheckUtil.IsInteger(dt.Rows[i]["tostation"].ToString()) && dt.Rows[i]["tostation"].ToString().Length == 7)
                                    {
                                        toStationStr = _repository.GetAgvAddressByWcsAddress(dt.Rows[i]["tostation"].ToString().Substring(0, 4));
                                        if (string.IsNullOrEmpty(toStationStr))
                                        {
                                            WriteLog($"任务:{dt.Rows[i]["taskno"]},WCS放货地址转换为AGV放货地址时,值为:{toStationStr}.不合法");
                                            continue;
                                        }
                                        Tostation = Convert.ToInt32(toStationStr);
                                    }
                                    else
                                    {
                                        if (DataCheckUtil.IsInteger(dt.Rows[i]["tostation"].ToString()) && locationLine.Contains(dt.Rows[i]["tostation"].ToString()))
                                        {
                                            toStationStr = _repository.GetAgvAddressByWcsAddress($"{dt.Rows[i]["tostation"].ToString().PadLeft(2, '0')}01");
                                        }
                                        else
                                        {
                                            toStationStr = _repository.GetAgvAddressByWcsAddress(dt.Rows[i]["tostation"].ToString());
                                        }
                                        if (string.IsNullOrEmpty(toStationStr))
                                        {
                                            WriteLog($"任务:{dt.Rows[i]["taskno"]},WCS放货地址转换为AGV放货地址时,值为:{toStationStr}.不合法");
                                            continue;
                                        }
                                        Tostation = Convert.ToInt32(toStationStr);
                                    }

                                    //if (FromStation.Equals(1) || FromStation.Equals(2) || FromStation.Equals(3))
                                    //{
                                    //    WriteLog($"WCS --> AGV (q-message):{dt.Rows[i]["taskno"]}:{FromStation}-->{Tostation},发送任务前延时2秒。");
                                    //    Thread.Sleep(2000);
                                    //}


                                    if (agvclass.SendTask(int.Parse(dt.Rows[i]["taskno"].ToString()), FromStation, Tostation, dt.Rows[i]["TASKTYPE"].ToString()))
                                    {
                                        WriteLog("WCS --> AGV (q-message) :" + dt.Rows[i]["taskno"].ToString() + "  " + FromStation + " --> " + Tostation);
                                        _repository.AddLog($"【发送任务】WCS --> AGV (q-message) :任务号：{dt.Rows[i]["taskno"]}，起始地址：{FromStation}，目标地址：{Tostation}");
                                        _repository.DealTaskStatus(
                                            new WCS_SUBTASKINFO()
                                            {
                                                TASKNO = Convert.ToInt32(dt.Rows[i]["taskno"]),
                                                RUNSTATUS = "1",
                                                EQUIPMENTTYPE = "AGV",
                                                FROMSTATION = dt.Rows[i]["fromstation"].ToString()
                                            },
                                            new WCS_SUBTASKINFO()
                                            {
                                                TASKSTATUS = "2"
                                            }
                                        );
                                        Thread.Sleep(200);
                                    }
                                    else
                                    {
                                        WriteLog("AGV任务发送失败，AGV断开连接");
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                WriteExceptionLog(ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex.Message);
            }
        }



        public void SendjMessage(int carno)
        {
            agvclass.SendjMessageByCarNo(carno);
        }
        public void StopTimer()
        {
            ScanDbTimer.Stop();
        }
        public void StartTimer()
        {
            ScanDbTimer.Start();
        }
        private void agvclass_Notify(string command, object data)
        {
            try
            {
                switch (command)
                {
                    case "Connect":
                        ScanDbTimer.Start();
                        NotifyConnect(command, data);
                        //WriteLog("AGV连接成功");
                        break;
                    case "Break":
                        ScanDbTimer.Stop();
                        NotifyConnect(command, data);
                        //WriteLog("AGV断开连接");
                        break;
                    case "b":
                        bMessageHandler((byte[])data);
                        break;
                    case "s":
                        sMessageHandler((byte[])data);
                        break;
                    case "E":
                        EMessageHandle((byte[])data);
                        break;
                    case "r":
                        rMessageHandler((byte[])data);
                        break;
                    default:
                        NotifyConnect(command, null);
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex.Message);
            }

        }

        private void EMessageHandle(byte[] msg)
        {
            int unitId = msg[4] * 256 + msg[5];
            int LGVNo = msg[6] * 256 + msg[7];
            int EventCode = msg[18] * 256 * 256 * 256 + msg[19] * 256 * 256 + msg[20] * 256 + msg[21];
            LogUtil.Write(LogFileName.AgvLog, LogFileName.AgvLog, $"AGV --> WCS (E-message):{LGVNo}{EventCode}");
        }
        public void WriteLog(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                LogUtil.Write(LogFileName.AgvLog, LogFileName.AgvLog, msg);
                NotifyConnect("Show", msg);
            }
        }

        private void SendLedMessage(string StationNo, string message)
        {
            if (StaticCommon.LedEquipmentList.Exists(x => x.EquipmentNo.Equals(StationNo)))
            {
                LedEquipment LedSend = StaticCommon.LedEquipmentList.Where(x => x.EquipmentNo.Equals(StationNo)).ToList()[0];
                LedSend.Led.SendString(message);
            }
        }


        public void WriteExceptionLog(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                LogUtil.Write(LogFileName.AgvExceptionLog, LogFileName.AgvExceptionLog, msg);
                NotifyConnect("Show", msg);
            }
        }




        /// <summary>
        /// 发送任务给AGV之后，用于接收AGV的反馈信息
        /// 信息包括：key => wcs发送的任务号
        /// inde => AGV自身索引号
        /// carno => AGV车号
        /// </summary>
        /// <param name="msg"></param>
        private void bMessageHandler(byte[] msg)
        {
            string index = msg[4].ToString("X2").Trim() + msg[5].ToString("X2").Trim();

            index = Convert.ToString(Convert.ToInt32(index.Substring(2, 2), 16));
            string status = msg[7].ToString("X2").Trim();
            string ACKNo = msg[8].ToString("X2").Trim();

            ACKNo = Convert.ToString(Convert.ToInt32(ACKNo, 16));
            int ikey = 0;
            if (msg.Length > 11)
                ikey = msg[10] * 256 + msg[11];
            WriteLog($"AGV --> WCS (b-Message),AGV索引号index:{index}；AGV返回的WCS任务号：{(ikey == 0 ? "空" : ikey.ToString())}AGV状态编码status：{status}；AGV的ACKNO：{ACKNo}");
            switch (status)
            {
                case "01":
                    if (_repository.UpdateAGVIndex(ikey.ToString(), index, "2", AgvNo))
                    {
                        WriteLog($"{AgvNo},成功！根据AGV的返回的任务号{ikey}，更新数据库AGV任务的索引号：{index}");
                        break;
                    }
                    else
                    {
                        WriteLog($"{AgvNo},第一次更新失败！根据AGV的返回的任务号{ikey}，更新AGV任务的索引号：{index}");
                        Thread.Sleep(1500);
                        if (_repository.UpdateAGVIndex(ikey.ToString(), index, "2", AgvNo))
                        {
                            WriteLog($"{AgvNo},第二次更新成功！AGV的返回的任务号{ikey}，AGV任务的索引号：{index}");
                            break;
                        }
                        else
                        {
                            WriteLog($"{AgvNo},第二次更新失败！根据AGV的返回的任务号{ikey}，更新数据库AGV任务的索引号：{index}");
                            Thread.Sleep(2000);
                            if (_repository.UpdateAGVIndex(ikey.ToString(), index, "2", AgvNo))
                            {
                                WriteLog($"{AgvNo},第三次更新成功！AGV的返回的任务号{ikey}，AGV任务的索引号：{index}");
                                break;
                            }
                            else
                            {
                                WriteLog($"{AgvNo},第三次更新失败！根据AGV的返回的任务号{ikey}，更新AGV任务的索引号：{index}");
                                break;
                            }
                        }
                    }
                case "02":
                    if (_repository.UpdateAGVCarNO(index, ACKNo.TrimStart('0'), "2", AgvNo))
                    {
                        WriteLog($"{AgvNo},成功！根据AGV的返回的索引号：{index}，更新数据库AGV任务的车号：{ACKNo}");
                    }
                    else
                    {
                        WriteLog($"{AgvNo},第一次更新失败！根据AGV的返回的索引号：{index}，更新数据库AGV任务的车号：{ACKNo}");
                        Thread.Sleep(1500);
                        if (_repository.UpdateAGVCarNO(index, ACKNo.TrimStart('0'), "2", AgvNo))
                        {
                            WriteLog($"{AgvNo},第二次更新成功！根据AGV的返回的索引号：{index}，更新数据库AGV任务的车号：{ACKNo}");
                        }
                        else
                        {
                            WriteLog($"{AgvNo},第二次更新失败！根据AGV的返回的索引号：{index}，更新数据库AGV任务的车号：{ACKNo}");
                        }
                    }
                    if (ikey > 0)
                    {
                        _repository.UpdateAGVIndex(ikey.ToString(), index, "2", AgvNo);
                    }

                    break;
                case "07":
                    if (ACKNo == "01" || ACKNo == "00")
                        agvclass.SendmMessage(msg);
                    break;
                case "13": agvclass.SendmMessage(msg); break;
                case "1B":
                    WriteLog($"{AgvNo},任务号：{ikey},被AGV拒绝接受");
                    Thread.Sleep(1000);
                    DataTable subInfo = _repository.QueryTableData(new WCS_SUBTASKINFO()
                    {
                        TASKNO = ikey,
                        EQUIPMENTNUMBER = "AGV02",
                        TASKSTATUS = "2",
                        EQUIPMENTTYPE = "AGV",
                        RUNSTATUS = "1"
                    });
                    if (subInfo != null && subInfo.Rows.Count == 1)
                    {
                        if (subInfo.Rows[0]["fromstation"].ToString().Contains("R"))
                        {
                            if (_repository.UpdateTableData(
                                 new WCS_SUBTASKINFO()
                                 {
                                     TASKNO = ikey,
                                     EQUIPMENTNUMBER = "AGV02",
                                     TASKSTATUS = "2",
                                     EQUIPMENTTYPE = "AGV",
                                     RUNSTATUS = "1",
                                     FROMSTATION = subInfo.Rows[0]["fromstation"].ToString()
                                 },
                                 new WCS_SUBTASKINFO()
                                 {
                                     TASKSTATUS = "0"
                                 }
                             ))
                            {
                                WriteLog($"{AgvNo},任务号：{ikey},被AGV拒绝接受,重新更新任务{ikey}为未开始，成功");
                            }
                            else
                            {
                                WriteLog($"{AgvNo},任务号：{ikey},被AGV拒绝接受,重新更新任务{ikey}为未开始，失败");
                            }
                        }
                    }
                    break;

            }

            NotifyConnect("刷新任务", null);
        }

        /// <summary>
        /// 用于接收AGV各种状态信息，关键是magic
        /// 
        /// </summary>
        /// <param name="msg"></param>
        private void sMessageHandler(byte[] msg)
        {
            string index = msg[4].ToString("X2").Trim() + msg[5].ToString("X2").Trim();
            index = Convert.ToString(Convert.ToInt32(index.Substring(2, 2), 16));
            string magic = msg[8].ToString("X2").Trim() + msg[9].ToString("X2").Trim();
            string Carno = msg[12].ToString().TrimStart('0');
            string strNewstationNo = string.Empty;
            string strTasktypeNo = string.Empty;
            string addressType = string.Empty;
            int agvPutAddress;
            int agvPickAddress;
            string HexAGVFetch = "";
            string HexDelivery = "";
            int layer = 0;
            int AGVFetch = 0;
            int AGVDelivery = 0;
            string fromStationNew = string.Empty;
            string toStationNew = string.Empty;
            string logMessage = string.Empty;
            string[] taskInfo = new string[16] { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };
            DataTable taskDataTable = new DataTable();
            WMSServicRequest servicRequest = new WMSServicRequest();

            ResultObject wmsServiceResult = new ResultObject();
            WriteLog($"AGV --> WCS (s-message),索引号:{index},小车号:{Carno},magic:{magic}");

            //准备取货
            if (magic.Equals("7002"))
            {
                HexAGVFetch = msg[10].ToString("X2").Trim() + msg[11].ToString("X2").Trim();
                HexDelivery = msg[18].ToString("X2").Trim() + msg[19].ToString("X2").Trim();
                AGVFetch = Convert.ToInt32(HexAGVFetch, 16);
                AGVDelivery = Convert.ToInt32(HexDelivery, 16);
                WriteLog($"AGV --> WCS (s-message) {Carno}小车向WCS发送取货请求，取货站台：{AGVFetch}");

                //zhf1 准备取货 ok
                var workStation7002 = BDControl.workStations.FirstOrDefault(c => c.DeviceCode == AGVFetch.ToString());
                if (workStation7002 is not null)
                {
                    var bdRes = BDControl.BdOpenControl(workStation7002, BDOPerationType.WMSOpenDoorBeforGet, BDDoorStatus.DoorOpenWhenGet);
                    if (!bdRes.IsSuccess)
                    {
                        WriteLog($"准备取货时:禁止取货,{bdRes.Description}{Environment.NewLine}");
                        return;
                    }
                }

                DataSet ds = new DataSet();
                agvPickAddress = AGVFetch;
                addressType = _repository.GetAddressTypeByAgvAddress(AGVFetch.ToString());
                //取货架区
                if (addressType != null && addressType.Equals("MachineType_Rack"))
                {
                    ////D0010120031
                    //获取agv最终地址
                    DataTable dt = _repository.GetTotalTask(new WCS_SUBTASKINFO()
                    {
                        AGVINDEX = index,
                        EQUIPMENTNUMBER = AgvNo,
                        TASKSTATUS = "2"

                    });

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        logMessage = $"异常.根据索引号:{index},找不到对应的WCS任务";
                        WriteLog(logMessage);
                        return;
                    }

                    //如果是指定货位下架,则不调用接口,直接分配1401011
                    if (dt.Rows[0]["mainfromstation"] != null && dt.Rows[0]["mainfromstation"].ToString().Length == 7)
                    {
                        agvPickAddress = Convert.ToInt32(_repository.GetAgvAddressByWcsAddress(dt.Rows[0]["mainfromstation"].ToString().Substring(0, 4)));
                        layer = Convert.ToInt32(dt.Rows[0]["mainfromstation"].ToString().Substring(4, 2));
                    }
                    else
                    {
                        //接口关闭,直接反馈
                        if (!_repository.GetWcsParamConfigValueByCode("OutBoundSecondQuestWMS").Equals("1"))
                        {
                            logMessage = $"WCS --> AGV (s-message) 出库货位二次分配接口已关闭，请检查！";
                            WriteLog(logMessage);
                            return;
                        }
                        #region 二次分配WMS不支持,暂时屏蔽
                        WMSUpdateTargetAddressRequest request = new WMSUpdateTargetAddressRequest();
                        request.Msg_Params[0].TaskNum = dt.Rows[0]["wmstaskno"].ToString();
                        request.Msg_Method = "二次分配";
                        request.Msg_Params[0].WarehouseID = WareHouseID;
                        request.Msg_Params[0].ZoneID = ZoneId02;
                        servicRequest.PostData = request;
                        servicRequest.DataUrl = "/api/wms/WorkTask/OutBoundSecondDistributionLocation";
                        wmsServiceResult = WMSMicroServiceUtil.RequestWMS(servicRequest);
                        if (!string.IsNullOrEmpty(wmsServiceResult.ResultJson))
                        {
                            _repository.AddLog($"货位二次分配接口反馈" + wmsServiceResult.ResultJson);
                        }
                        else
                        {
                            _repository.AddLog($"货位二次分配接口反馈" + "ResultJson为空");
                        }
                        if (!wmsServiceResult.IsSuccess || string.IsNullOrEmpty(wmsServiceResult.ResultJson))
                        {
                            logMessage = $"WCS --> AGV (s-message) 反馈给{Carno}小车,index:{index},取货地址：{agvPickAddress}。WMS接口反馈失败,详细信息:{wmsServiceResult.ErrorMessage}";
                            WriteLog(logMessage);
                            return;
                        }
                        WMSUpdateTargetAddressResponse response = JsonConvert.DeserializeObject<WMSUpdateTargetAddressResponse>(wmsServiceResult.ResultJson);
                        //货位分配失败
                        if (!response.isSucceed)
                        {
                            logMessage = $"WCS --> AGV (s-message) 反馈给{Carno}小车,index:{index}.WMS货位分配失败,详细信息:{response.errmsg}";
                            WriteLog(logMessage);
                            return;
                        }
                        if (string.IsNullOrEmpty(response.json.TargetAddress) || (response.json.TargetAddress.Length != 7 && response.json.TargetAddress.Length != 2))
                        {
                            logMessage = $"WCS --> AGV (s-message) 反馈给{Carno}小车，index:{index}。WMS货位分配失败,详细信息:{response.errmsg}";
                            WriteLog(logMessage);
                            return;
                        }
                        //分配最终地址
                        if (response.json.TargetAddress.Length == 7)
                        {
                            agvPickAddress = Convert.ToInt32(_repository.GetAgvAddressByWcsAddress(response.json.TargetAddress.Substring(0, 4)));
                            layer = Convert.ToInt32(response.json.TargetAddress.Substring(4, 2));
                        }
                        if (response.json.TargetAddress.Length != 7)
                        {
                            agvPickAddress = Convert.ToInt32(_repository.GetAgvAddressByWcsAddress(response.json.TargetAddress.PadLeft(2, '0') + "01"));
                            layer = 0;
                        }

                        //更新子任务起始地址
                        if (!_repository.UpdateSubTask(
                            new WCS_SUBTASKINFO()
                            {
                                AGVINDEX = index,
                                EQUIPMENTNUMBER = AgvNo,
                                TASKSTATUS = "2"
                            },
                            new WCS_SUBTASKINFO()
                            {
                                FROMSTATION = response.json.TargetAddress
                            }))
                        {
                            logMessage = $"WCS --> AGV (s-message) 反馈给{Carno}小车，index:{index}，取货地址：{agvPickAddress}。更新WCS子任务起始地址失败！";
                            WriteLog(logMessage);
                        }
                        DataTable subTable = _repository.QueryTableData(new WCS_SUBTASKINFO()
                        {
                            AGVINDEX = index,
                            EQUIPMENTNUMBER = AgvNo,
                            TASKSTATUS = "2"
                        });

                        if (subTable == null || subTable.Rows.Count != 1)
                        {
                            logMessage = $"WCS --> AGV (s-message) 反馈给{Carno}小车，index:{index}，取货地址：{agvPickAddress}。查询到多条执行中的AGV任务！";
                            WriteLog(logMessage);
                            return;
                        }

                        //更新主任务起始地址
                        if (!_repository.UpdateMainTask(
                            new WCS_TASKINFO()
                            {
                                TASKNO = Convert.ToInt32(subTable.Rows[0]["taskno"])
                            },
                            new WCS_TASKINFO()
                            {
                                FROMSTATION = response.json.TargetAddress
                            }))
                        {
                            logMessage = $"WCS --> AGV (s-message) 反馈给{Carno}小车，index:{index}，取货地址：{agvPickAddress}。更新WCS主任务起始地址失败！";
                            WriteLog(logMessage);
                        }
                        #endregion
                    }
                }
                else
                {
                    layer = 1;
                }
                logMessage = $"WCS --> AGV (s-message) 反馈给{Carno}小车，index:{index}，取货地址：{agvPickAddress}，层：{layer}。";
                WriteLog(logMessage);
                _repository.AddLog(logMessage);
                agvclass.SendmMessage_UpdateLayer(msg, agvPickAddress, AGVDelivery, layer);
                _repository.UpdateAGVMagic(index, Carno, magic);
                return;
            }

            //准备放货
            if (magic.Equals("7006"))
            {
                HexAGVFetch = msg[10].ToString("X2").Trim() + msg[11].ToString("X2").Trim();
                HexDelivery = msg[18].ToString("X2").Trim() + msg[19].ToString("X2").Trim();
                AGVFetch = Int32.Parse(HexAGVFetch, System.Globalization.NumberStyles.HexNumber);
                AGVDelivery = Int32.Parse(HexDelivery, System.Globalization.NumberStyles.HexNumber);
                agvPutAddress = AGVDelivery;
                addressType = _repository.GetAddressTypeByAgvAddress(AGVDelivery.ToString());
                WriteLog($"AGV --> WCS (s-message) {Carno}小车向WCS发送放货请求，agv请求放货地址：{AGVDelivery}");

                //zhf2 放货前处理 ing
                var workStation7006 = BDControl.workStations.FirstOrDefault(c => c.DeviceCode == AGVDelivery.ToString());
                if (workStation7006 is not null)
                {
                    var bdRes = BDControl.BdOpenControl(workStation7006, BDOPerationType.WMSOpenDoorBeforPut, BDDoorStatus.DoorOpenWhenPut);
                    if (!bdRes.IsSuccess)
                    {
                        WriteLog($"准备放时:禁止放货,{bdRes.Description}{Environment.NewLine}");
                        return;
                    }
                }

                //放货架
                if (addressType != null && addressType.Equals("MachineType_Rack"))
                {
                    ////D0010120031
                    //获取agv最终地址
                    DataTable dt = _repository.GetTotalTask(new WCS_SUBTASKINFO()
                    {
                        AGVINDEX = index,
                        TASKSTATUS = "2",
                        EQUIPMENTNUMBER = AgvNo
                    });
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        logMessage = $"异常.根据索引号index:{index},找不到对应的WCS任务";
                        WriteLog(logMessage);
                        return;
                    }
                    //如果是已经有了最终地址
                    if (dt.Rows[0]["maintostation"] != null && dt.Rows[0]["maintostation"].ToString().Length == 7)
                    {
                        agvPutAddress = Convert.ToInt32(_repository.GetAgvAddressByWcsAddress(dt.Rows[0]["maintostation"].ToString().Substring(0, 4)));
                        layer = Convert.ToInt32(dt.Rows[0]["maintostation"].ToString().Substring(4, 2));
                    }
                    else
                    {
                        //接口关闭,直接反馈
                        if (!_repository.GetWcsParamConfigValueByCode("InBoundSecondQuestWMS").Equals("1"))
                        {
                            logMessage = $"WCS --> AGV (s-message) 入库货位二次分配接口已关闭，请检查！";
                            WriteLog(logMessage);
                            return;
                        }
                        #region 二次分配WMS不支持,暂时屏蔽
                        WMSUpdateTargetAddressRequest request = new WMSUpdateTargetAddressRequest();
                        request.Msg_Params[0].TaskNum = dt.Rows[0]["wmstaskno"].ToString();
                        request.Msg_Method = "二次分配";
                        request.Msg_Params[0].WarehouseID = WareHouseID;
                        request.Msg_Params[0].ZoneID = ZoneId02;
                        servicRequest.DataUrl = "/api/wms/WorkTask/InBoundSecondDistributionLocation";
                        servicRequest.PostData = request;
                        wmsServiceResult = WMSMicroServiceUtil.RequestWMS(servicRequest);
                        if (!string.IsNullOrEmpty(wmsServiceResult.ResultJson))
                        {
                            _repository.AddLog($"货位二次分配接口反馈" + wmsServiceResult.ResultJson);
                        }
                        else
                        {
                            _repository.AddLog($"货位二次分配接口反馈" + "ResultJson为空");
                        }
                        if (!wmsServiceResult.IsSuccess || string.IsNullOrEmpty(wmsServiceResult.ResultJson))
                        {
                            logMessage = $"WCS --> AGV (s-message) 反馈给{Carno}小车，index:{index}。WMS接口调用失败,详细信息:{wmsServiceResult.ErrorMessage}";
                            WriteLog(logMessage);
                            return;
                        }
                        WMSUpdateTargetAddressResponse response = JsonConvert.DeserializeObject<WMSUpdateTargetAddressResponse>(wmsServiceResult.ResultJson);
                        if (!response.isSucceed)
                        {
                            logMessage = $"WCS --> AGV (s-message) 反馈给{Carno}小车，index:{index}，WMS货位分配失败,详细信息:{response.errmsg}";
                            WriteLog(logMessage);
                            return;
                        }
                        if (string.IsNullOrEmpty(response.json.TargetAddress) || (response.json.TargetAddress.Length != 7 && response.json.TargetAddress.Length != 2 && response.json.TargetAddress.Length != 1))
                        {
                            logMessage = $"WCS --> AGV (s-message) 反馈给{Carno}小车，index:{index}。WMS货位分配失败,详细信息:{response.errmsg}";
                            WriteLog(logMessage);
                            return;
                        }
                        //分配最终地址
                        if (response.json.TargetAddress.Length == 7)
                        {
                            agvPutAddress = Convert.ToInt32(_repository.GetAgvAddressByWcsAddress(response.json.TargetAddress.Substring(0, 4)));
                            layer = Convert.ToInt32(response.json.TargetAddress.Substring(4, 2));
                        }
                        if (response.json.TargetAddress.Length != 7)
                        {
                            agvPutAddress = Convert.ToInt32(_repository.GetAgvAddressByWcsAddress(response.json.TargetAddress.PadLeft(2, '0') + "01"));
                            layer = 0;
                        }
                        //更新子任务
                        if (!_repository.UpdateSubTask(
                            new WCS_SUBTASKINFO()
                            {
                                AGVINDEX = index,
                                EQUIPMENTNUMBER = AgvNo,
                                TASKSTATUS = "2"
                            },
                            new WCS_SUBTASKINFO()
                            {
                                TOSTATION = response.json.TargetAddress
                            }))
                        {
                            logMessage = $"WCS --> AGV (s-message) 反馈给{Carno}小车，index:{index}。更新WCS子任务目标地址失败！";
                            WriteLog(logMessage);
                        }
                        DataTable subTable = _repository.QueryTableData(new WCS_SUBTASKINFO()
                        {
                            AGVINDEX = index,
                            EQUIPMENTNUMBER = AgvNo,
                            TASKSTATUS = "2"
                        });
                        if (subTable == null || subTable.Rows.Count != 1)
                        {
                            logMessage = $"WCS --> AGV (s-message) 反馈给{Carno}小车，index:{index}。查询到多条执行中的AGV任务！";
                            WriteLog(logMessage);
                            return;
                        }
                        //更新主任务
                        if (!_repository.UpdateMainTask(
                            new WCS_TASKINFO()
                            {
                                TASKNO = Convert.ToInt32(subTable.Rows[0]["taskno"])
                            },
                            new WCS_TASKINFO()
                            {
                                TOSTATION = response.json.TargetAddress
                            }))
                        {
                            logMessage = $"WCS --> AGV (s-message) 反馈给{Carno}小车，index:{index}。更新WCS主任务目标地址失败！";
                            WriteLog(logMessage);
                        }
                        #endregion
                    }
                }
                else
                {
                    //卷包机台或者暂存区
                    if (addressType != null && (addressType.Equals("MachineType_Dock") || addressType.Equals("MachineType_Z")))
                    {

                    }
                    else if (addressType != null && addressType.Equals("MachineType_Conveyor"))//输送机
                    {
                        string[] stationInfo = plc.ReadPLCInfo(agvPutAddress.ToString());
                        if (stationInfo == null)
                        {
                            WriteLog($"wcs读取agv放货站台号：{agvPutAddress}信息失败，wcs暂时不响应agv放货请求！");
                            return;
                        }
                        if (Convert.ToInt32(stationInfo[0]) > 0)
                        {
                            WriteLog($"agv放货站台号：{agvPutAddress}任务号大于0，wcs暂时不响应agv放货请求！");
                            return;
                        }
                        if (AGVDelivery.ToString().Equals("5659"))
                        {
                            stationInfo = plc.ReadPLCInfo("5799");
                            if (stationInfo == null || Convert.ToInt32(stationInfo[0]) > 0)
                            {
                                WriteLog($"站台:5799.任务号大于0，wcs暂时不响应agv放货请求！");
                                return;
                            }
                        }
                    }
                    else if (addressType != null && addressType.Equals("MachineType_Rotation"))
                    {
                        DataTable agvAddressDt = _repository.QueryTableData(new WCS_AGV_ADDRESS()
                        {
                            AGV_ADDRESS = agvPutAddress.ToString()
                        });
                        if (agvAddressDt == null || agvAddressDt.Rows.Count != 1)
                        {
                            WriteLog($"agv放货地址{agvPutAddress},在wcs-agv配置表中不存在");
                            return;
                        }

                        DataTable dt = _repository.QueryTableData(
                           new WCS_STATION_STATUS()
                           {
                               STATION_NO = agvAddressDt.Rows[0]["wcs_address"].ToString()
                           }
                           );
                        if (dt != null && dt.Rows.Count == 1)
                        {
                            if (dt.Rows[0]["exist_pallet"].Equals("1"))
                            {
                                WriteLog($"agv放货的转向架：{agvPutAddress}有托盘，wcs暂时不响应agv放货请求！");
                                return;
                            }
                        }
                    }
                    layer = 1;


                }

                logMessage = $"WCS --> AGV (s-message) 允许{Carno}小车放货，index:{index}，放货地址：{agvPutAddress}，层：{layer}。";
                WriteLog(logMessage);
                _repository.AddLog(logMessage);
                _repository.UpdateAGVMagic(index, Carno, magic);
                agvclass.SendmMessage_UpdateLayer(msg, AGVFetch, agvPutAddress, layer);
                return;
            }

            switch (magic)
            {
                //AGV冷启动清除所有任务
                case "6002":
                    WriteLog("AGV --> WCS (s-message) 冷启动");
                    break;
                case "6001":
                    WriteLog("AGV --> WCS (s-message) 热启动");
                    break;
                case "1002":
                    WriteLog("AGV --> WCS (s-message) 任务在取货之前结束");
                    break;
                case "6003":
                    WriteLog("AGV --> WCS (s-message) 空车在CarWash状态");
                    //dal.UpdateAGVIndexByCarNo(Carno, index);
                    break;
                case "6004":
                    WriteLog("AGV --> WCS (s-message) 载货车在CarWash状态");
                    // dal.UpdateAGVIndexByCarNo(Carno, index);
                    break;
                case "6005":
                    WriteLog("AGV --> WCS (s-message) 小车在系统中成功移动");
                    break;
                case "6006":
                    WriteLog("AGV --> WCS (s-message) 小车在系统中不能移动报错");
                    break;
                case "6007":
                    WriteLog("AGV --> WCS (s-message) WCS发送任务的参数有问题，AGV不接受");
                    break;
                case "7001":
                    WriteLog("AGV --> WCS (s-message) 小车到达搬运目的地");
                    break;
                case "7003": //取货完成
                case "7004":
                    if (magic.Equals("7003"))
                    {
                        WriteLog("AGV --> WCS (s-message) " + Carno + "自动状态下小车取到货物");
                    }
                    else
                    {
                        WriteLog("AGV --> WCS (s-message) " + Carno + "人工状态下小车取到货物");
                    }
                    HexAGVFetch = msg[10].ToString("X2").Trim() + msg[11].ToString("X2").Trim();
                    HexDelivery = msg[18].ToString("X2").Trim() + msg[19].ToString("X2").Trim();
                    AGVFetch = int.Parse(HexAGVFetch, System.Globalization.NumberStyles.HexNumber);
                    AGVDelivery = int.Parse(HexDelivery, System.Globalization.NumberStyles.HexNumber);
                    WriteLog("AGV --> WCS (s-message) " + Carno + "小车完成步骤2 ");
                    agvPickAddress = AGVFetch;

                    //zhf3 取货完成 ok
                    var workStation7004 = BDControl.workStations.FirstOrDefault(c => c.DeviceCode == AGVFetch.ToString());
                    if (workStation7004 is not null)
                    {
                        var bdRes = BDControl.BdOpenControl(workStation7004, BDOPerationType.CloseDoorAfterGet);
                        if (!bdRes.IsSuccess)
                        {
                            WriteLog($"取货完成时:关门失败,{bdRes.Description}{Environment.NewLine}");
                        }
                        else
                        {
                            WriteLog($"取货完成时:关门成功,{bdRes.Description}{Environment.NewLine}");
                        }
                    }

                    addressType = _repository.GetAddressTypeByAgvAddress(AGVFetch.ToString());
                    //清空转向架信息
                    if (addressType != null && (addressType.Equals("MachineType_Rotation") || addressType.Equals("MachineType_Dock")))
                    {
                        DataTable dtAddress = _repository.QueryTableData(new WCS_AGV_ADDRESS()
                        {
                            AGV_ADDRESS = AGVFetch.ToString()
                        });

                        //卷包机台，调用WMS部分完成接口
                        if (addressType.Equals("MachineType_Dock")
                            && (dtAddress.Rows[0]["WCS_ADDRESS"].ToString().Contains("FS")
                                || dtAddress.Rows[0]["WCS_ADDRESS"].ToString().Contains("BZ")
                                || dtAddress.Rows[0]["WCS_ADDRESS"].ToString().Contains("JJ")
                                || dtAddress.Rows[0]["WCS_ADDRESS"].ToString().Contains("ZX"))
                                || dtAddress.Rows[0]["WCS_ADDRESS"].ToString().Contains("CP"))
                        {
                            _repository.DealWmsTaskStatus(new WCS_SUBTASKINFO()
                            {
                                AGVINDEX = index,
                                EQUIPMENTNUMBER = AgvNo,
                                EQUIPMENTTYPE = "AGV",
                                TASKSTATUS = "2"
                            },
                           new WCS_SUBTASKINFO()
                           {
                               TASKSTATUS = "3"
                           }
                           );
                        }
                        //转向站台
                        if (addressType.Equals("MachineType_Rotation"))
                        {
                            DataTable dtStationInfo = _repository.QueryTableData(new WCS_STATION_STATUS()
                            {
                                STATION_NO = dtAddress.Rows[0]["WCS_ADDRESS"].ToString()
                            });

                            _repository.UpdateTableData(
                                new WCS_STATION_STATUS()
                                {
                                    STATION_NO = dtAddress.Rows[0]["WCS_ADDRESS"].ToString()
                                },
                                new WCS_STATION_STATUS()
                                {
                                    EXIST_PALLET = "0",
                                    TASK_COUNT = Convert.ToInt32(dtStationInfo.Rows[0]["TASK_COUNT"]) - 1
                                }
                            );
                        }


                    }
                    //从货架取货完成，报部分完成
                    if (addressType != null && addressType.Equals("MachineType_Rack"))
                    {
                        _repository.DealWmsTaskStatus(new WCS_SUBTASKINFO()
                        {
                            AGVINDEX = index,
                            EQUIPMENTNUMBER = AgvNo,
                            EQUIPMENTTYPE = "AGV",
                            TASKSTATUS = "2"
                        },
                        new WCS_SUBTASKINFO()
                        {
                            TASKSTATUS = "3"
                        }
                        );
                    }
                    _repository.UpdateAGVMagic(index, Carno, magic);
                    break;
                case "7005":
                    WriteLog("AGV --> WCS (s-message) 小车空取货");
                    break;
                case "7007": //放货完成
                    HexDelivery = msg[18].ToString("X2").Trim() + msg[19].ToString("X2").Trim();
                    AGVDelivery = Int32.Parse(HexDelivery, System.Globalization.NumberStyles.HexNumber);
                    addressType = _repository.GetAddressTypeByAgvAddress(AGVDelivery.ToString());
                    WriteLog($"AGV --> WCS (s-message) 小车{Carno}放货完成，,AGV取放货位置：{AGVFetch}->{AGVDelivery}");

                    //zhf4 放货完成
                    var workStation7007 = BDControl.workStations.FirstOrDefault(c => c.DeviceCode == AGVDelivery.ToString());
                    if (workStation7007 is not null)
                    {
                        var bdRes = BDControl.BdOpenControl(workStation7007, BDOPerationType.CloseDoorAfterPut);
                        if (!bdRes.IsSuccess)
                        {
                            WriteLog($"放货完成时:关门失败,{bdRes.Description}{Environment.NewLine}");
                        }
                        else
                        {
                            WriteLog($"放货完成时:关门成功,{bdRes.Description}{Environment.NewLine}");
                        }
                    }
                    

                    //放输送机
                    if (addressType != null && addressType.Equals("MachineType_Conveyor"))
                    {
                        taskDataTable = _repository.GetTotalTask(new WCS_SUBTASKINFO()
                        {
                            EQUIPMENTNUMBER = AgvNo,
                            AGVINDEX = index,
                            TASKSTATUS = "2"
                        });

                        //一楼余料回库站台。退空盘，退余料，紧急入库
                        if (AGVDelivery.Equals(5658))
                        {
                            if (taskDataTable == null || taskDataTable.Rows.Count == 0)
                            {
                                WriteLog($"WCS --> AGV (s-message) 索引号:{index},AGV放货位置:{AGVDelivery},WCS找不到对应的任务信息,所以只能在{AGVDelivery}临时写入信息。");
                                taskInfo[0] = "1000";
                                taskInfo[1] = "0";
                                taskInfo[2] = "2";
                                taskInfo[3] = AGVDelivery.ToString();
                                taskInfo[4] = "5656";
                                if (!plc.Write(STACommand.ReWrite, taskInfo, AGVDelivery.ToString(), 0, 30))
                                {
                                    WriteLog($"AGV放货位置:{AGVDelivery}.写入输送机任务信息失败");
                                }
                                SendLedMessage("5658", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss},站台号:5658,单空托盘前往码盘机,目标地址:5656");
                            }
                            else
                            {
                                if (taskDataTable.Rows[0]["barcode"].ToString().Contains("9999999"))
                                {
                                    taskInfo[0] = "1000";
                                    taskInfo[1] = "0";
                                    taskInfo[2] = "2";
                                    taskInfo[3] = AGVDelivery.ToString();
                                    taskInfo[4] = "5656";
                                    taskInfo[5] = "1";
                                    taskInfo[6] = "0";
                                    taskInfo[7] = "0";
                                    taskInfo[8] = "0";
                                    taskInfo[9] = "0";
                                    if (!plc.Write(STACommand.ReWrite, taskInfo, AGVDelivery.ToString(), 0, 30))
                                    {
                                        WriteLog($"AGV放货位置:{AGVDelivery}.写入输送机任务信息失败");
                                    }
                                    SendLedMessage("5658", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss},站台号:5658,单空托盘来自机台:{taskDataTable.Rows[0]["mainfromstation"]},前往码盘机,目标地址:5656");
                                }
                                else
                                {
                                    DataTable staTable = _repository.GetSTATaskInfo(index, "2", "1", AgvNo);
                                    taskInfo[0] = "1000";
                                    if (staTable != null)
                                    {
                                        taskInfo[0] = taskDataTable.Rows[0]["taskno"].ToString();
                                    }
                                    taskInfo[1] = taskDataTable.Rows[0]["barcode"].ToString();
                                    taskInfo[2] = "1";
                                    taskInfo[3] = AGVDelivery.ToString();
                                    taskInfo[4] = "5647";
                                    taskInfo[5] = "1";
                                    taskInfo[6] = "0";
                                    taskInfo[7] = "0";
                                    taskInfo[8] = "0";
                                    
                                    taskInfo[9] = "0";
                                    if (!plc.Write(STACommand.ReWrite, taskInfo, AGVDelivery.ToString(), 0, 30))
                                    {
                                        WriteLog($"AGV放货位置:{AGVDelivery}.写入输送机任务信息失败");
                                    }
                                    if (taskInfo[0].Equals("1000"))
                                    {
                                        SendLedMessage("5658", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss},站台号:5658,余料托盘来自机台:{taskDataTable.Rows[0]["mainfromstation"]},前往目标站台:5647");
                                    }
                                }
                            }

                        }
                        //二楼退空盘口
                        if (AGVDelivery.Equals(5667))
                        {
                            taskInfo[0] = "1000";
                            taskInfo[1] = "0";
                            taskInfo[2] = "2";
                            taskInfo[3] = AGVDelivery.ToString();
                            taskInfo[4] = "5666";
                            taskInfo[5] = "1";
                            taskInfo[6] = "0";
                            taskInfo[7] = "0";
                            taskInfo[8] = "0";
                            taskInfo[9] = "0";
                            if (!plc.Write(STACommand.ReWrite, taskInfo, AGVDelivery.ToString(), 0, 30))
                            {
                                WriteLog($"AGV放货位置:{AGVDelivery}.写入输送机任务信息失败");
                            }
                        }
                        //二楼成品出库口
                        if (AGVDelivery.Equals(5722))
                        {
                            if (taskDataTable == null || taskDataTable.Rows.Count == 0)
                            {
                                #region 20260827 zhoujianglai 放货完成信号重复处理问题
                                //20260827 zhoujianglai 放货完成信号重复处理问题
                                //第二次信号过来，找不到执行中的agv任务，直接将站台信息覆盖掉，导致托盘任务停止
                                //查找当前agv任务是否已完成，如果已经完成，说明任务已经过去，则不需要写信息
                                try
                                {
                                    var  taskTableTemp2 = _repository.GetTotalTask(new WCS_SUBTASKINFO()
                                    {
                                        EQUIPMENTNUMBER = AgvNo,
                                        AGVINDEX = index,
                                        TASKSTATUS = "1"
                                    });

                                    if (taskTableTemp2 != null && taskTableTemp2.Rows.Count > 0)
                                    {
                                        WriteLog($"WCS --> AGV (s-message) 索引号:{index},AGV放货位置:{AGVDelivery},WCS中agv任务已完成，完成信号重复触发，无需处理.。");
                                        break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    WriteLog($"WCS --> AGV (s-message) 索引号:{index},AGV放货位置:{AGVDelivery},agv任务重复触发判断异常，异常原因:{e.Message}");
                                }
                                #endregion
                                
                                WriteLog($"WCS --> AGV (s-message) 索引号:{index},AGV放货位置:{AGVDelivery},WCS找不到对应的任务信息,所以只能在{AGVDelivery}临时写入信息。");
                                taskInfo[0] = "1000";
                                taskInfo[1] = "0";
                                taskInfo[2] = "1";
                                taskInfo[3] = AGVDelivery.ToString();
                                if (!plc.Write(STACommand.ReWrite, taskInfo, AGVDelivery.ToString(), 0, 30))
                                {
                                    WriteLog($"AGV放货位置:{AGVDelivery}.写入输送机任务信息失败");
                                }
                                break;
                            }
                            taskDataTable = _repository.GetSTATaskInfo(index, "2", "1", AgvNo);
                            if (taskDataTable == null)
                            {
                                WriteLog($"WCS --> AGV (s-message) 索引号:{index},AGV放货位置:{AGVDelivery},WCS找不到对应输送机任务,所以只能在{AGVDelivery}临时写入信息。");
                                taskInfo[0] = "1000";
                                taskInfo[1] = "0";
                                taskInfo[2] = "1";
                                taskInfo[3] = AGVDelivery.ToString();
                                if (!plc.Write(STACommand.ReWrite, taskInfo, AGVDelivery.ToString(), 0, 30))
                                {
                                    WriteLog($"AGV放货位置:{AGVDelivery}.写入输送机任务信息失败");
                                }
                                break;
                            }
                            string barcodeTemp = string.Empty;
                            barcodeTemp = taskDataTable.Rows[0]["barcode"].ToString();
                            if (!DataCheckUtil.IsInteger(barcodeTemp) || barcodeTemp.Length > 6)
                            {
                                barcodeTemp = "99999999";
                            }
                            taskInfo[0] = taskDataTable.Rows[0]["taskno"].ToString() ?? "1000";
                            taskInfo[1] = barcodeTemp;
                            taskInfo[2] = taskDataTable.Rows[0]["barcode"].ToString().Contains("9999999") ? "10" : "1";
                            taskInfo[3] = AGVDelivery.ToString();
                            taskInfo[4] = "5721";
                            taskInfo[5] = "1";
                            taskInfo[6] = "0";
                            taskInfo[7] = "0";
                            taskInfo[8] = "0";
                            taskInfo[9] = "0";
                            if (!plc.Write(STACommand.ReWrite, taskInfo, AGVDelivery.ToString(), 0, 30))
                            {
                                WriteLog($"AGV放货位置:{AGVDelivery}.写入输送机任务信息失败");
                            }
                            SendLedMessage(AGVDelivery.ToString(), $"站台:{AGVDelivery},托盘号:{taskInfo[1]},放货完成！");
                        }
                        //二楼紧急成品出库口 5716
                        if (AGVDelivery.Equals(5716))
                        {
                            if (taskDataTable == null || taskDataTable.Rows.Count == 0)
                            {
                                WriteLog($"WCS --> AGV (s-message) 索引号:{index},AGV放货位置:{AGVDelivery},WCS找不到对应的任务信息,所以只能在{AGVDelivery}临时写入信息。");
                                taskInfo[0] = "1000";
                                taskInfo[1] = "0";
                                taskInfo[2] = "1";
                                taskInfo[3] = AGVDelivery.ToString();
                                if (!plc.Write(STACommand.ReWrite, taskInfo, AGVDelivery.ToString(), 0, 30))
                                {
                                    WriteLog($"AGV放货位置:{AGVDelivery}.写入输送机任务信息失败");
                                }
                                break;
                            }
                            taskDataTable = _repository.GetSTATaskInfo(index, "2", "1", AgvNo);
                            if (taskDataTable == null || taskDataTable.Rows.Count == 0)
                            {
                                WriteLog($"WCS --> AGV (s-message) 索引号:{index},AGV放货位置:{AGVDelivery},WCS找不到对应输送机任务,所以只能在{AGVDelivery}临时写入信息。");
                                taskInfo[0] = "1000";
                                taskInfo[1] = "0";
                                taskInfo[2] = "1";
                                taskInfo[3] = AGVDelivery.ToString();
                                if (!plc.Write(STACommand.ReWrite, taskInfo, AGVDelivery.ToString(), 0, 30))
                                {
                                    WriteLog($"AGV放货位置:{AGVDelivery}.写入输送机任务信息失败");
                                }
                                break;
                            }
                            taskInfo[0] = taskDataTable.Rows[0]["taskno"].ToString() ?? "1000";
                            taskInfo[1] = taskDataTable.Rows[0]["barcode"].ToString() ?? "0";
                            taskInfo[2] = taskDataTable.Rows[0]["barcode"].ToString().Contains("9999999") ? "10" : "1";
                            taskInfo[3] = AGVDelivery.ToString();
                            taskInfo[4] = AGVDelivery.ToString();
                            taskInfo[5] = "1";
                            taskInfo[6] = "0";
                            taskInfo[7] = "0";
                            taskInfo[8] = "0";
                            taskInfo[9] = "0";
                            if (!plc.Write(STACommand.ReWrite, taskInfo, AGVDelivery.ToString(), 0, 30))
                            {
                                WriteLog($"AGV放货位置:{AGVDelivery}.写入输送机任务信息失败");
                            }
                        }
                    }

                    DataTable subTaskInfo = null;
                    //输送机
                    if (addressType != null && addressType.Equals("MachineType_Conveyor"))
                    {
                        subTaskInfo = _repository.QueryTableData(new WCS_SUBTASKINFO()
                        {
                            EQUIPMENTNUMBER = AgvNo,
                            AGVINDEX = index,
                            TASKSTATUS = "2"
                        });
                        if (subTaskInfo != null && subTaskInfo.Rows.Count == 1)
                        {
                            _repository.DealTaskStatus(
                                new WCS_SUBTASKINFO()
                                {
                                    TASKNO = Convert.ToInt32(subTaskInfo.Rows[0]["taskno"]),
                                    EQUIPMENTTYPE = "STA",
                                    FROMSTATION = AGVDelivery.ToString()
                                },
                                new WCS_SUBTASKINFO()
                                {
                                    RUNSTATUS = "1",
                                    TASKSTATUS = "2"
                                });
                        }
                    }
                    //旋转台
                    if (addressType != null && addressType.Equals("MachineType_Rotation"))
                    {
                        agvclass.SendmMessage(msg);
                        string taskDelayTime = _repository.GetWcsParamConfigValueByCode($"rotation_task_delay");
                        int taskDelayTimeInt = 2000;
                        if (!string.IsNullOrEmpty(taskDelayTime))
                        {
                            taskDelayTimeInt = Convert.ToInt32(taskDelayTime);
                        }
                        WriteLog($"WCS --> AGV (s-message) 索引号:{index},AGV放货位置:{AGVDelivery},转向站台,放货完成，给与AGV确认信号，延迟{taskDelayTimeInt}毫秒后，下发取货任务");
                        Thread.Sleep(taskDelayTimeInt);
                        WriteLog($"WCS --> AGV (s-message) 索引号:{index},AGV放货位置:{AGVDelivery},转向站台,放货完成，延迟结束。取货任务改为可执行");

                        subTaskInfo = _repository.QueryTableData(new WCS_SUBTASKINFO()
                        {
                            EQUIPMENTNUMBER = AgvNo,
                            AGVINDEX = index,
                            TASKSTATUS = "2"
                        });
                        if (subTaskInfo != null)
                        {
                            subTaskInfo = subTaskInfo.AsEnumerable().OrderByDescending(x => x["create_date"]).CopyToDataTable();

                            DataTable agvAddressDt = _repository.QueryTableData(new WCS_AGV_ADDRESS()
                            {
                                AGV_ADDRESS = Convert.ToInt32(AGVDelivery.ToString()).ToString()
                            });

                            _repository.DealTaskStatus(
                               new WCS_SUBTASKINFO()
                               {
                                   TASKNO = Convert.ToInt32(subTaskInfo.Rows[0]["taskno"]),
                                   EQUIPMENTTYPE = "AGV",
                                   FROMSTATION = agvAddressDt.Rows[0]["wcs_address"].ToString(),
                               },
                               new WCS_SUBTASKINFO()
                               {
                                   RUNSTATUS = "1",
                               });

                            //转向架标记为有货
                            _repository.UpdateTableData(
                               new WCS_STATION_STATUS()
                               {
                                   STATION_NO = agvAddressDt.Rows[0]["wcs_address"].ToString(),
                               },
                               new WCS_STATION_STATUS()
                               {
                                   EXIST_PALLET = "1"
                               }
                            );
                        }

                    }
                    //放货架
                    if (addressType != null && addressType.Equals("MachineType_Rack"))
                    {

                        subTaskInfo = _repository.QueryTableData(new WCS_SUBTASKINFO()
                        {
                            EQUIPMENTNUMBER = AgvNo,
                            AGVINDEX = index,
                            TASKSTATUS = "2"
                        });
                        //如果搬送任务还没有分配最终货位，则不完成任务。
                        if (subTaskInfo != null && subTaskInfo.Rows.Count == 1 && AgvNo.Equals("02") && locationLine.Contains(subTaskInfo.Rows[0]["TOSTATION"].ToString()))
                        {
                            WriteLog($"{AgvNo},自动放货完成.索引号:{index},未进行二次货位分配,不允许完成AGV任务。");
                            break;
                        }
                    }

                    DataTable subInfo = _repository.QueryTableData(new WCS_SUBTASKINFO()
                    {
                        AGVINDEX = index,
                        EQUIPMENTNUMBER = AgvNo,
                        TASKSTATUS = "2",
                        EQUIPMENTTYPE = "AGV"
                    });
                    if (subInfo != null && subInfo.Rows.Count > 0)
                    {
                        if (subInfo.Rows.Count > 1)
                        {
                            WriteLog($"{AgvNo},索引号:{index},任务有{subInfo.Rows.Count}条,开始批量完成...");
                        }

                        foreach (DataRow dr in subInfo.Rows)
                        {
                            if (!_repository.DealTaskStatus(
                                    new WCS_SUBTASKINFO()
                                    {
                                        AGVINDEX = index,
                                        EQUIPMENTNUMBER = AgvNo,
                                        TASKSTATUS = "2",
                                        EQUIPMENTTYPE = "AGV",
                                        TASKNO = Convert.ToInt32(dr["taskno"].ToString())
                                    },
                                    new WCS_SUBTASKINFO()
                                    {
                                        TASKSTATUS = "1"
                                    })
                                )
                            {
                                WriteLog($"{AgvNo},放货完成,完成AGV任务失败.任务号:{Convert.ToInt32(dr["taskno"].ToString())},索引号index:{index}");
                            }
                            else
                            {
                                WriteLog($"{AgvNo},放货完成,完成AGV任务成功.任务号:{Convert.ToInt32(dr["taskno"].ToString())},索引号index:{index}");
                            }
                        }
                    }
                    _repository.UpdateAGVMagic(index, Carno, magic);

                    break;
                case "7008":
                    WriteLog($"AGV --> WCS (s-message) 手动放货完成,索引号：{index},车号：{Carno},AGV取放货位置：{AGVFetch}->{AGVDelivery}");

                    DataTable dt = _repository.QueryTableData(new WCS_SUBTASKINFO()
                    {
                        AGVINDEX = index,
                        EQUIPMENTNUMBER = AgvNo,
                        TASKSTATUS = "2",
                        RUNSTATUS = "1",
                        EQUIPMENTTYPE = "AGV"
                    });
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        WriteLog($"{AgvNo},手动放货完成.索引号:{index},找不到对应的WCS任务,不予处理。");
                        break;
                    }
                    //如果搬送任务还没有分配最终货位，则不完成任务。
                    if (AgvNo.Equals("02") && locationLine.Contains(dt.Rows[0]["TOSTATION"].ToString()))
                    {
                        WriteLog($"{AgvNo},手动放货完成.索引号:{index},未进行二次货位分配,不允许完成AGV任务。");
                        break;
                    }
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 1)
                        {
                            WriteLog($"{AgvNo},索引号:{index},任务有{dt.Rows.Count}条,开始批量完成...");
                        }
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (!_repository.DealTaskStatus(
                                    new WCS_SUBTASKINFO()
                                    {
                                        AGVINDEX = index,
                                        EQUIPMENTNUMBER = AgvNo,
                                        TASKSTATUS = "2",
                                        EQUIPMENTTYPE = "AGV",
                                        TASKNO = Convert.ToInt32(dr["taskno"].ToString())
                                    },
                                    new WCS_SUBTASKINFO()
                                    {
                                        TASKSTATUS = "1"
                                    })
                                )
                            {
                                WriteLog($"{AgvNo},手动放货完成,完成AGV任务失败.任务号:{Convert.ToInt32(dr["taskno"].ToString())},索引号:{index}");
                            }
                            else
                            {
                                WriteLog($"{AgvNo},手动放货完成,完成AGV任务成功.任务号:{Convert.ToInt32(dr["taskno"].ToString())},索引号:{index}");
                            }
                        }
                    }
                    break;
                case "7009":
                    WriteLog($"AGV --> WCS (s-message) 目标站台位被占用，发生重复存放,AGV取放货位置：{AGVFetch}->{AGVDelivery}");
                    break;
                default: break;
            }
            if (addressType != null && addressType.Equals("MachineType_Rotation") && magic == "7007")
            {
                ;
            }
            else
            {
                agvclass.SendmMessage(msg);
            }
            GetTask(_repository.GetSubTaskInfo(AgvNo));
        }

        private void rMessageHandler(byte[] msg)
        {
            string index = msg[4].ToString("X2").Trim() + msg[5].ToString("X2").Trim();
        }
        public void SetTimer(bool flag)
        {
            ScanDbTimer.Enabled = flag;
        }
        private void NotifyConnect(string command, object state)
        {
            if (GetConnectStatus != null)
                GetConnectStatus(command, state);
        }
    }
}
