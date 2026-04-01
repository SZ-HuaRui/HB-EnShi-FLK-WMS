using CWCS.Core.Library.PLCManager;
using CWCS.Entity;
using CWCS.Entity.Convery;
using CWCS.Entity.SRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Linq;
using CWCS.Equipment.Led;
using CWCS.Equipment.Led.Yb;
using CWCS.Equipment.Rfid;
using CWCS.Equipment.Rfid.Invengo;
using System.Text.RegularExpressions;
using CWCS.Interface.WMSMicroService;
using CWCS.Interface.Request.WMS;
using Newtonsoft.Json;
using CWCS.Interface.Response.WMS;
using CWCS.Common.Log;

namespace CWCS.BLL
{
    public class ConveryBLL : BLLBase
    {
        public static List<ConveryEntity> lst;
        private int DbLength => 30;

        public static string InBoundLEDMessage = string.Empty;
        public LedEquipment LedSend { get; set; }
        public RfidEquipment RfidEquipment { get; set; }

        public Action<List<ConveryEntity>> RefreshStation;

        public Action<string> WriteToLeftLog;

        public Action<string> WriteToDowntLog;

        public Action<string, string, bool> ShowStationPLCStatus;

        public Timer InBoundLEDTimer;


        public ConveryBLL()
        {

        }

        public void EventInit()
        {
            plc.ShowPLCStatus += ShowConveryPLCStatus;//连接状态显示
            plc.DoWork += DoWork;//站台请求处理
            plc.WriteToDowntLog += WriteToDowntLog;//交互日志显示
            plc.RefreshStation += RefreshStation;//刷新界面站台数据
        }


        /// <summary>
        /// 输送机设备初始化
        /// </summary>
        /// <returns></returns>
        public List<ConveryEntity> ConveyorInit()
        {
            try
            {
                TestRequestJson();
                DataTable dt = _repository.GetOPCItem();
                if (dt != null && dt.Rows.Count > 0)
                {
                    ConvertToList(dt);
                    //加载Led
                    LoadLed();
                    //加载RFID
                    //LoadRfid();
                    string error = string.Empty;
                    //连接plc
                    plc.InitData(lst, ref error);
                    LogUtil.Write(LogFileName.ConveyorLog, LogFileName.ConveyorLog, $"恩施卷烟厂辅料库,输送机初始化完毕。");
                    plc.Run(1);
                    LogUtil.Write(LogFileName.ConveyorLog, LogFileName.ConveyorLog, $"恩施卷烟厂辅料库,输送机服务启动成功。");
                }
                else
                {
                    LogUtil.Write(LogFileName.ConveyorExceptionLog, LogFileName.ConveyorExceptionLog, $"恩施卷烟厂辅料库,输送机初始化失败,找不到站台配置数据。");
                }
            }
            catch (Exception ex)
            {
                LogUtil.Write(LogFileName.ConveyorExceptionLog, LogFileName.ConveyorExceptionLog, $"函数ConveryInit异常。{ex.Message}{ex.StackTrace}");
            }
            return lst;
        }

        public void TestRequestJson()
        {
            WMSInBoundRequest request = new WMSInBoundRequest();
            request.Msg_Id = "sendTask";
            request.Msg_Mark = "发送任务";
            request.Msg_Params[0].ContainerBarCode = "11";
            request.Msg_Params[0].StationNo = "1001";
            string json = JsonConvert.SerializeObject(request);
            //json = "{\"State_Code\":\"000\",\"State_Desc\":\"成功\",\"Data\":[{\"WarehouseId\":\"188811\",\"ZoneId\":null,\"TaskNumber\":\"T-20200720-00001\",\"TaskType\":\"1\",\"ContainerBarcode\":\"80009991\",\"Wave_ID\":null,\"GroupNum\":null,\"SeqNum\":\"1\",\"SourceAddress\":\"5601\",\"TargetAddress\":\"A0010010011\",\"EquipmentNum\":\"SC01\",\"LedText\":\"任务申请成功\"}]}";
            //json = "{\"State_Code\":\"000\",\"State_Desc\":\"成功\",\"Data\":[{\"WarehouseId\":\"188811\",\"ZoneId\":null,\"TaskNumber\":\"T-20200720-00001\",\"TaskType\":\"1\",\"ContainerBarcode\":\"80009991\",\"Wave_ID\":null,\"GroupNum\":null,\"SeqNum\":\"1\",\"SourceAddress\":\"5601\",\"TargetAddress\":\"A0010010011\",\"EquipmentNum\":\"SC01\",\"LedText\":\"任务申请成功\"},{\"WarehouseId\":\"188811\",\"ZoneId\":null,\"TaskNumber\":\"T-20200720-00001\",\"TaskType\":\"1\",\"ContainerBarcode\":\"80009991\",\"Wave_ID\":null,\"GroupNum\":null,\"SeqNum\":\"1\",\"SourceAddress\":\"5601\",\"TargetAddress\":\"A0010010011\",\"EquipmentNum\":\"SC02\",\"LedText\":\"任务申请成功\"}]}";
            WMSInBoundResponse response = new WMSInBoundResponse();
            response.json = new InBoundResponseData();
            response.errcode = 0;
            response.errmsg = "";
            response.isSucceed = true;
            response.json.WarehouseId = "43411";
            response.json.ZoneId = "243141";
            json = JsonConvert.SerializeObject(response);

            WMSInBoundResponse item = JsonConvert.DeserializeObject<WMSInBoundResponse>(json);
            //List<WCS_RECEIVE_TASK> list = new List<WCS_RECEIVE_TASK>();

        }

        /// <summary>
        /// 加载RFID
        /// </summary>
        public void LoadRfid()
        {
            try
            {
                List<WCS_PARAMETER_CONFIG> list = StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Contains("RfidAddress")).ToList();
                IRfid rfid = null;
                RfidEquipment rfidEquipment = null;
                string StationNo = string.Empty;
                string ip = string.Empty;
                foreach (WCS_PARAMETER_CONFIG item in list)
                {
                    if (item.PARAM_CODE.Contains("5658"))
                    {
                        StationNo = Regex.Match(item.PARAM_CODE, "(?<=_).+").Value;
                        rfid = new InvengoRfid(item.PARAM_VALUE);
                        rfidEquipment = new RfidEquipment(StationNo, EquipmentPosition.Down, rfid);
                        rfidEquipment.NotifyStringEvent += WriteToDowntLog;
                        rfid.Connect();
                        StaticCommon.RfidEquipmentList.Add(rfidEquipment);
                    }

                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 加载Led
        /// </summary>
        public void LoadLed()
        {
            try
            {
                List<WCS_PARAMETER_CONFIG> list = StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Contains("LedAddress")).ToList();
                ILed led = null;
                LedEquipment ledEquipment = null;
                string StationNo = string.Empty;
                string ip = string.Empty;
                foreach (WCS_PARAMETER_CONFIG item in list)
                {
                    StationNo = Regex.Match(item.PARAM_CODE, "(?<=_).+").Value;
                    led = new YbLed(item.PARAM_VALUE);
                    led.NotifyStringMessage += WriteToDowntLog;
                    if (led.Connect())
                    {
                        led.SendString("设备运行中,请注意安全！");
                    }
                    ledEquipment = new LedEquipment(StationNo, led);
                    StaticCommon.LedEquipmentList.Add(ledEquipment);
                }
            }
            catch
            {

            }

        }




        private void ConvertToList(DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count <= 0) throw new Exception();
                lst = new List<ConveryEntity>();
                DataRow[] d = dt.Select(" groupname ='Convery' ", "");
                foreach (DataRow dr in dt.Select(" groupname ='Convery' ", ""))
                {
                    ConveryEntity ci = new ConveryEntity();
                    ci.C_TaskInfo = new ConveryTaskInfo();
                    ci.StationNo = dr["StationNo"].ToString();
                    ci.Key_ip = dr["userid"].ToString();
                    ci.DBAddress = int.Parse(dr["objects"].ToString());
                    ci.length = int.Parse(dr["signaltype"].ToString());
                    ci.value = int.Parse(dr["value"].ToString());
                    ci.StationType = int.Parse(dr["StationType"].ToString());
                    ci.LocationX = int.Parse(dr["LocationX"].ToString());
                    ci.LocationY = int.Parse(dr["LocationY"].ToString());
                    ci.Width = int.Parse(dr["Width"].ToString());
                    ci.Height = int.Parse(dr["Height"].ToString());
                    ci.Belong = dr["belong"].ToString();
                    ci.config = dr["groupname"].ToString();
                    ci.field3 = dr["field3"].ToString();
                    ci.AreaCode = dr["areacode"].ToString();
                    ci.C_TaskInfo.StationNo = ci.StationNo;
                    ci.C_TaskInfo.StationType = ci.StationType;
                    lst.Add(ci);
                }
            }
            catch (Exception)
            {

            }
        }


        /// <summary>
        /// 发送Led信息
        /// </summary>
        /// <param name="StationNo"></param>
        /// <param name="message"></param>
        private void SendLedMessage(string StationNo, string message)
        {
            if (StaticCommon.LedEquipmentList.Exists(x => x.EquipmentNo.Equals(StationNo)))
            {
                LedSend = StaticCommon.LedEquipmentList.Where(x => x.EquipmentNo.Equals(StationNo)).ToList()[0];
                if (!string.IsNullOrEmpty(message))
                {
                    LedSend.Led.SendString(message);
                }
            }
        }

        public string GetLedStationNo(string stationNo)
        {
            string ledStationNo = string.Empty;
            switch (stationNo)
            {
                case "5673": ledStationNo = "5673"; break;
                case "5601": ledStationNo = "5673"; break;
                case "5674": ledStationNo = "5674"; break;
                case "5605": ledStationNo = "5674"; break;
                case "5658": ledStationNo = "5658"; break;
                case "5659": ledStationNo = "5659"; break;
                case "5722": ledStationNo = "5722"; break;
            }
            return ledStationNo;
        }

        /// <summary>
        /// 输送机业务逻辑处理
        /// </summary>
        /// <param name="si"></param>
        private void DoWork(object task)
        {
            try
            {
                //return;
                ConveryEntity si = task as ConveryEntity;
                string[] stationTaskInfo = ConveyorEntityToArray(si);
                ConveryTaskInfo sta = si.C_TaskInfo;
                char[] trimArray = new char[3] { '\0', '\r', '\n' };
                string logMessage = string.Empty;
                string procedureMessage = string.Empty;
                string wmsResponseStr = string.Empty;
                string wcsRequestStr = string.Empty;
                string _wcsTaskNo = string.Empty;
                string _ToStation = string.Empty;
                ResultObject wmsServiceResult = new ResultObject();
                string isRfidUsing = "0";//Rfid使用标记
                bool isRfidWriteSuccess = false;
                string _ledMessage = string.Empty;
                string _logMessage = string.Empty;
                string _logMessage1 = string.Empty;
                string _emergencyMode1 = string.Empty;
                string _emergencyMode2 = string.Empty;
                string _ifRequestTask5622 = string.Empty;
                string _ifRequestTask5722 = string.Empty;
                //WMS服务
                WMSServicRequest servicRequest = new WMSServicRequest() { DataUrl = "/api/tyjk/Inbound/RequestPallets_Put" };
                WMSInBoundRequest inboundRequest = new WMSInBoundRequest();
                inboundRequest.Msg_Mark = $"站台{si.StationNo}入库请求";
                inboundRequest.Msg_Params[0].WarehouseID = WareHouseID;
                inboundRequest.Msg_Params[0].ZoneID = string.Empty;
                inboundRequest.Msg_Params[0].ContainerBarCode = sta.Barcode;
                inboundRequest.Msg_Params[0].StationNo = sta.StationNo;
                servicRequest.PostData = inboundRequest;

                if (sta.StationNo.Equals("5601") || sta.StationNo.Equals("5605"))
                {
                    _emergencyMode1 = _repository.GetWcsParamConfigValueByCode($"EmergencyMode");
                }

                #region 配盘入库，分配入库楼层
                if (sta.StationType.Equals(1) && (sta.GoodsType.Equals(1) || sta.GoodsType.Equals(10)) && sta.TaskNumber < 10000 && !_emergencyMode1.Equals("1"))
                {
                    string exceptionStationNo = "0";
                    switch (si.StationNo)
                    {
                        case "5601": exceptionStationNo = "5673"; break;
                        case "5605": exceptionStationNo = "5674"; break;
                        case "5733": exceptionStationNo = "5734"; break;
                        case "5735": exceptionStationNo = "5736"; break;
                        default: break;
                    }
                    //实托盘,写入Rfid
                    if (sta.GoodsType.Equals(1))
                    {
                        if (sta.StationNo.Equals("5601") || sta.StationNo.Equals("5605"))
                        {
                            isRfidUsing = _repository.GetWcsParamConfigValueByCode($"Write_Rfid_{sta.StationNo}");
                            if (isRfidUsing.Equals("1"))
                            {
                                if (StaticCommon.RfidEquipmentList.Exists(x => x.EquipmentNo.Equals(si.StationNo)))
                                {
                                    RfidEquipment = StaticCommon.RfidEquipmentList.Where(x => x.EquipmentNo.Equals(si.StationNo)).ToList()[0];
                                    if (RfidEquipment != null)
                                    {
                                        isRfidWriteSuccess = RfidEquipment.Rfid.WriteUserDataString(sta.Barcode);
                                        if (!isRfidWriteSuccess)
                                        {
                                            stationTaskInfo[4] = exceptionStationNo;
                                            stationTaskInfo[7] = "2";
                                            stationTaskInfo[9] = "0";
                                            WriteLogBoth($"站台:{sta.StationNo},托盘号:{sta.Barcode},RFID写入失败！剔除到目标站台：{stationTaskInfo[4]}");
                                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                                            return;
                                        }
                                    }
                                }

                            }
                        }
                    }

                    if (sta.GoodsType.Equals(1) || sta.GoodsType.Equals(10))
                    {
                        //wms任务类型区分:1，空盘入库；2，盘点入库；3，实盘入库；4，质检入库；

                        if (sta.GoodsType.Equals(10))
                        {
                            string floorNo = StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("NullPalletInBoundFloor")).ToList()[0].PARAM_VALUE.Trim();
                            if (!floorNo.Equals("1") && !floorNo.Equals("2"))
                            {
                                stationTaskInfo[4] = exceptionStationNo;
                                stationTaskInfo[9] = "6";
                                _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},空托盘组入库楼层设置错误,请检查!";
                                WriteLogBoth(_logMessage);
                                SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                                WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                                return;
                            }
                            else
                            {
                                _ToStation = floorNo.Equals("1") ? "5622" : "5722";
                                stationTaskInfo[4] = _ToStation;
                                stationTaskInfo[9] = "0";
                                _logMessage = $"站台:{sta.StationNo},任务号:{sta.TaskNumber},托盘号:{sta.Barcode},分配楼层为:{(_ToStation.Equals("5622") ? "一楼" : "二楼")},前往目标站台:{_ToStation}.";
                                _logMessage1 = $"**************站台:{sta.StationNo},任务号:{sta.TaskNumber},托盘号:{sta.Barcode},分配楼层为:{(_ToStation.Equals("5622") ? "一楼" : "二楼")},前往目标站台:{_ToStation}.";
                                WriteLogBoth(_logMessage);
                                if (sta.StationNo == "5601")
                                {
                                    SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage1);
                                }
                                else
                                {
                                    SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                                }

                                WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                                return;
                            }
                        }
                        //result[0] = parameters[2].Value.ToString();//区域
                        //result[1] = parameters[3].Value.ToString();//描述
                        //result[2] = parameters[4].Value.ToString();//结果

                        //获取入库楼层失败
                        string[] zoneResult = _repository.GetInBoundZone(" ", sta.Barcode);
                        if (!zoneResult[2].Equals("1") || string.IsNullOrEmpty(zoneResult[0]))
                        {
                            stationTaskInfo[4] = exceptionStationNo;
                            stationTaskInfo[9] = "4";
                            _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},入库失败,剔除到站台:{exceptionStationNo},详细信息:{zoneResult[1]}";
                            WriteLogBoth(_logMessage);
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            return;
                        }

                        if (zoneResult[0].Contains("M"))
                        {
                            if (sta.ToStation.Equals(5601) || sta.ToStation.Equals(5605))
                            {
                                _ToStation = "5622";
                            }
                            if (sta.ToStation.Equals(5733) || sta.ToStation.Equals(5735))
                            {
                                stationTaskInfo[4] = exceptionStationNo;
                                stationTaskInfo[9] = "4";
                                _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},入库区域有误，请检查。";
                                WriteLogBoth(_logMessage);
                                SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                                WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                                return;
                            }
                        }

                        if (zoneResult[0].Contains("A"))
                        {
                            _emergencyMode2 = _repository.GetWcsParamConfigValueByCode($"SecondFloorEmergencyMode");
                            _ToStation = "5722";
                            if (_emergencyMode2.Equals("1"))
                            {
                                _ToStation = "5716";
                            }
                        }
                        stationTaskInfo[4] = _ToStation;
                        stationTaskInfo[9] = "0";
                        _logMessage = $"站台:{sta.StationNo},任务号:{sta.TaskNumber},托盘号:{sta.Barcode},分配楼层为:{(_ToStation.Equals("5622") ? "一楼" : "二楼")},前往目标站台:{_ToStation}.";
                        _logMessage1 = $"**************站台:{sta.StationNo},任务号:{sta.TaskNumber},托盘号:{sta.Barcode},分配楼层为:{(_ToStation.Equals("5622") ? "一楼" : "二楼")},前往目标站台:{_ToStation}.";
                        WriteLogBoth(_logMessage);
                        if (sta.StationNo == "5601")
                        {
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage1);
                        }
                        else
                        {
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                        }

                      
                        WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                        return;
                    }
                    return;
                }
                #endregion
                //rfid标记:1,成功;2,失败;
                #region 申请入库任务
                if ((sta.StationNo.Equals("5622") || sta.StationNo.Equals("5722") || sta.StationNo.Equals("5647") || sta.StationNo.Equals("5716")) && (sta.GoodsType.Equals(1) || sta.GoodsType.Equals(10)))
                {
                    if ((sta.StationNo.Equals("5622") || sta.StationNo.Equals("5722") || sta.StationNo.Equals("5716") || sta.StationNo.Equals("5647")) && sta.TaskNumber < 10000)
                    {
                        if (sta.StationNo.Equals("5622"))
                        {
                            _ifRequestTask5622 = _repository.GetWcsParamConfigValueByCode($"IfRequestTask_5622");
                            if (_ifRequestTask5622.Trim().Equals("0"))
                            {
                                _logMessage = $"站台:{sta.StationNo},当前为【不申请任务】模式,请注意";
                                WriteLogBoth(_logMessage);
                                return;
                            }
                        }
                        if (sta.StationNo.Equals("5722"))
                        {
                            _ifRequestTask5722 = _repository.GetWcsParamConfigValueByCode($"IfRequestTask_5722");
                            if (_ifRequestTask5722.Trim().Equals("0"))
                            {
                                _logMessage = $"站台:{sta.StationNo},当前为【不申请任务】模式,请注意";
                                WriteLogBoth(_logMessage);
                                return;
                            }
                        }
                        //wms任务类型区分:1，空盘入库；2，盘点入库；3，实盘入库；4，质检入库；
                        inboundRequest.Msg_Params[0].TaskType = $"{(sta.GoodsType.Equals(1) ? 3 : 1)}";
                        inboundRequest.Msg_Params[0].ZoneID = $"{((sta.StationNo.Equals("5622") || sta.StationNo.Equals("5647")) ? ZoneId01 : ZoneId02)}";

                        //调用WMS接口
                        _repository.AddLog($"入库请求接口" + JsonConvert.SerializeObject(servicRequest.PostData));
                        wmsServiceResult = WMSMicroServiceUtil.RequestWMS(servicRequest);
                        _repository.AddLog($"入库反馈接口" + wmsServiceResult.ResultJson);

                        if (!wmsServiceResult.IsSuccess || string.IsNullOrEmpty(wmsServiceResult.ResultJson))
                        {
                            stationTaskInfo[4] = "0";
                            stationTaskInfo[9] = "1";
                            _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},WMS接口调用失败.详细信息:{wmsServiceResult.ErrorMessage}";
                            WriteLogBoth(_logMessage);
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            return;
                        }

                        WMSInBoundResponse response = JsonConvert.DeserializeObject<WMSInBoundResponse>(wmsServiceResult.ResultJson);
                        if (!response.isSucceed)
                        {
                            stationTaskInfo[4] = "0";
                            stationTaskInfo[9] = "2";
                            _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},WMS入库服务返回失败！反馈状态码：{response.errcode},反馈描述：{response.errmsg}";
                            WriteLogBoth(_logMessage);
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            return;
                        }

                        WCS_RECEIVE_TASK taskInfo = _mapper.Map<WCS_RECEIVE_TASK>(response.json);

                        //插入WMS任务
                        if (!_repository.InsertReceiveTask(taskInfo))
                        {
                            stationTaskInfo[4] = "0";
                            stationTaskInfo[9] = "3";
                            _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},WCS插入WMS任务失败";
                            WriteLogBoth(_logMessage);
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            return;
                        }
                        //拆分WMS任务
                        string[] splitResult = _repository.SplitReceiveTask(taskInfo.WAREHOUSEID, taskInfo.ZONEID, taskInfo.TASKNUM);
                        if (!splitResult[1].Equals("1"))
                        {
                            stationTaskInfo[4] = "0";
                            stationTaskInfo[9] = "4";
                            _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},WCS拆分WMS任务失败,详细信息:{splitResult[0]}！";
                            WriteLogBoth(_logMessage);
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            return;
                        }
                        //修改目标地址
                        _wcsTaskNo = _repository.GetWcsTaskNoByWmsTaskNo(taskInfo.TASKNUM);
                        if (string.IsNullOrEmpty(_wcsTaskNo))
                        {
                            stationTaskInfo[4] = "0";
                            stationTaskInfo[9] = "5";
                            _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},WCS根据WMS任务号:{taskInfo.TASKNUM},查询到0个或者多个WCS任务,请检查！目标地址改为:{stationTaskInfo[4]}";
                            WriteLogBoth(_logMessage);
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            return;
                        }
                        //更新任务状态
                        //_repository.UpdateTableData(
                        //    new WCS_TASKINFO()
                        //    {
                        //        TASKNO = Convert.ToInt32(_wcsTaskNo)
                        //    },
                        //    new WCS_TASKINFO()
                        //    {
                        //        TASKSTATUS = "2"
                        //    }
                        //    );
                        stationTaskInfo[0] = _wcsTaskNo;
                        stationTaskInfo[4] = "0";
                        stationTaskInfo[9] = "0";
                        _logMessage = $"站台:{sta.StationNo},任务号{_wcsTaskNo},托盘号:{sta.Barcode},申请入库成功";
                        WriteLogBoth(_logMessage);
                        SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                        WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                        return;
                    }
                    if (sta.StationNo.Equals("5647") && sta.TaskNumber >= 10000)
                    {
                        //完成输送机任务
                        _repository.DealTaskStatus(
                                new WCS_SUBTASKINFO()
                                {
                                    TASKNO = sta.TaskNumber,
                                    EQUIPMENTTYPE = "STA"
                                },
                                new WCS_SUBTASKINFO()
                                {
                                    TASKSTATUS = "1"
                                });
                        //更新子任务
                        if (!_repository.DealTaskStatus(
                                new WCS_SUBTASKINFO()
                                {
                                    TASKNO = sta.TaskNumber,
                                    EQUIPMENTTYPE = "SC"
                                },
                                new WCS_SUBTASKINFO()
                                {
                                    RUNSTATUS = "1"
                                })
                            )
                        {
                            WriteLogBoth($"站台:{sta.StationNo},托盘号:{sta.Barcode},更新堆垛机任务为【可执行】失败！");
                            return;
                        }
                        _ToStation = "0";
                        stationTaskInfo[4] = _ToStation;
                        WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                        return;
                    }
                    if (sta.StationNo.Equals("5722") && sta.TaskNumber >= 10000)
                    {
                        // 二楼agv取货口,异常处理流程
                        if (sta.FromStation.Equals(5721))
                        {
                            WriteLogBoth($"站台:{sta.StationNo},托盘号:{sta.Barcode},外检或Rfid异常,托盘退回到位！");
                            if (sta.CheckInfo > 0)
                            {
                                _logMessage += GetErrorInfo(sta.CheckInfo);
                            }
                            if (sta.RfidFlag.Equals("2"))
                            {
                                _logMessage += $"Rfid写入失败";
                            }
                            stationTaskInfo[4] = "0";
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            SendLedMessage(sta.StationNo, _logMessage);
                            return;
                        }
                    }
                    return;
                }
                #endregion

                #region 紧急模式配盘入库
                if (sta.StationType.Equals(1) && sta.TaskNumber < 10000 && _emergencyMode1.Equals("1"))
                {
                    string exceptionStationNo = "0";
                    switch (si.StationNo)
                    {
                        case "5601": exceptionStationNo = "5673"; break;
                        case "5605": exceptionStationNo = "5674"; break;
                        case "5733": exceptionStationNo = "5734"; break;
                        case "5735": exceptionStationNo = "5736"; break;
                        default: break;
                    }
                    //实托盘,写入Rfid
                    if (sta.GoodsType.Equals(1))
                    {
                        if (sta.StationNo.Equals("5601") || sta.StationNo.Equals("5605"))
                        {
                            isRfidUsing = _repository.GetWcsParamConfigValueByCode($"Write_Rfid_{sta.StationNo}");
                            if (isRfidUsing.Equals("1"))
                            {
                                if (StaticCommon.RfidEquipmentList.Exists(x => x.EquipmentNo.Equals(si.StationNo)))
                                {
                                    RfidEquipment = StaticCommon.RfidEquipmentList.Where(x => x.EquipmentNo.Equals(si.StationNo)).ToList()[0];
                                    if (RfidEquipment != null)
                                    {
                                        isRfidWriteSuccess = RfidEquipment.Rfid.WriteUserDataString(sta.Barcode);
                                        if (!isRfidWriteSuccess)
                                        {
                                            stationTaskInfo[4] = exceptionStationNo;
                                            stationTaskInfo[7] = "2";
                                            stationTaskInfo[9] = "0";
                                            WriteLogBoth($"站台:{sta.StationNo},托盘号:{sta.Barcode},RFID写入失败！剔除到目标站台：{stationTaskInfo[4]}");
                                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                                            return;
                                        }
                                    }
                                }

                            }
                        }
                    }

                    if (sta.GoodsType.Equals(1) || sta.GoodsType.Equals(10))
                    {
                        //wms任务类型区分:1，空盘入库；2，盘点入库；3，实盘入库；4，质检入库；
                        inboundRequest.Msg_Params[0].TaskType = $"{(sta.GoodsType.Equals(1) ? 3 : 1)}";

                        if (sta.GoodsType.Equals(10))
                        {
                            string floorNo = inboundRequest.Msg_Params[0].ZoneID = StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("NullPalletInBoundFloor")).ToList()[0].PARAM_VALUE.Trim();
                            if (!floorNo.Equals("1") && !floorNo.Equals("2"))
                            {
                                stationTaskInfo[4] = exceptionStationNo;
                                stationTaskInfo[9] = "6";
                                _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},空托盘入库楼层设置错误,请检查!";
                                WriteLogBoth(_logMessage);
                                SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                                WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                                return;
                            }
                            inboundRequest.Msg_Params[0].ZoneID = floorNo.Equals("1") ? ZoneId01 : ZoneId02;
                        }
                        //调用WMS接口
                        _repository.AddLog($"入库请求接口" + JsonConvert.SerializeObject(servicRequest.PostData));
                        wmsServiceResult = WMSMicroServiceUtil.RequestWMS(servicRequest);
                        _repository.AddLog($"入库反馈接口" + wmsServiceResult.ResultJson);
                        if (!wmsServiceResult.IsSuccess || string.IsNullOrEmpty(wmsServiceResult.ResultJson))
                        {
                            stationTaskInfo[4] = exceptionStationNo;
                            stationTaskInfo[9] = "1";
                            _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},WMS接口调用失败.剔除到站台:{exceptionStationNo},详细信息:{wmsServiceResult.ErrorMessage}";
                            WriteLogBoth(_logMessage);
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            return;
                        }
                        WMSInBoundResponse response = JsonConvert.DeserializeObject<WMSInBoundResponse>(wmsServiceResult.ResultJson);
                        if (!response.isSucceed)
                        {
                            stationTaskInfo[4] = exceptionStationNo;
                            stationTaskInfo[9] = "2";
                            _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},WMS入库服务返回失败！剔除到站台:{exceptionStationNo},反馈状态码：{response.errcode},反馈描述：{response.errmsg}";
                            WriteLogBoth(_logMessage);
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            return;
                        }
                        WCS_RECEIVE_TASK taskInfo = _mapper.Map<WCS_RECEIVE_TASK>(response.json);

                        //插入WMS任务
                        if (!_repository.InsertReceiveTask(taskInfo))
                        {
                            stationTaskInfo[4] = "0";
                            stationTaskInfo[9] = "3";
                            _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},WCS插入WMS任务失败";
                            WriteLogBoth(_logMessage);
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            return;
                        }
                        //拆分WMS任务
                        string[] splitResult = _repository.SplitReceiveTask(taskInfo.WAREHOUSEID, taskInfo.ZONEID, taskInfo.TASKNUM);
                        if (!splitResult[1].Equals("1"))
                        {
                            stationTaskInfo[4] = "0";
                            stationTaskInfo[9] = "4";
                            _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},WCS拆分WMS任务失败,剔除到站台:{exceptionStationNo},详细信息:{splitResult[0]}！";
                            WriteLogBoth(_logMessage);
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            return;
                        }
                        //修改目标地址
                        _wcsTaskNo = _repository.GetWcsTaskNoByWmsTaskNo(taskInfo.TASKNUM);
                        if (string.IsNullOrEmpty(_wcsTaskNo))
                        {
                            stationTaskInfo[4] = "0";
                            stationTaskInfo[9] = "5";
                            _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},WCS根据WMS任务号:{taskInfo.TASKNUM},查询到0个或者多个WCS任务,请检查！目标地址改为:{stationTaskInfo[4]}";
                            WriteLogBoth(_logMessage);
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            return;
                        }
                        _ToStation = _repository.GetToStation(_wcsTaskNo, "STA", sta.ToStation.ToString());
                        if (string.IsNullOrEmpty(_ToStation))
                        {
                            stationTaskInfo[4] = "0";
                            stationTaskInfo[9] = "0";
                            _logMessage = $"站台:{sta.StationNo},托盘号:{sta.Barcode},WCS根据查询条件:任务号{_wcsTaskNo},任务类型STA,起始地址{sta.ToStation},查询不到目标地址,请检查！目标地址改为:{stationTaskInfo[4]}";
                            WriteLogBoth(_logMessage);
                            SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            return;
                        }
                        //更新任务状态
                        _repository.UpdateTableData(
                            new WCS_TASKINFO()
                            {
                                TASKNO = Convert.ToInt32(_wcsTaskNo)
                            },
                            new WCS_TASKINFO()
                            {
                                TASKSTATUS = "2"
                            }
                            );
                        stationTaskInfo[0] = _wcsTaskNo;
                        stationTaskInfo[4] = _ToStation;
                        stationTaskInfo[9] = "0";
                        _logMessage = $"站台:{sta.StationNo},任务号{_wcsTaskNo},托盘号:{sta.Barcode},申请入库成功,前往目标站台:{_ToStation}.";
                        WriteLogBoth(_logMessage);
                        SendLedMessage(GetLedStationNo(sta.StationNo), _logMessage);
                        WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                        return;
                    }
                }
                #endregion

                #region 一楼agv取货口
                if (sta.StationType.Equals(3) && sta.TaskNumber >= 10000)
                {
                    WriteLogBoth($"站台:{sta.StationNo},托盘号:{sta.Barcode},agv取货口,托盘到位！");

                    if (!_repository.DealTaskStatus(
                        new WCS_SUBTASKINFO()
                        {
                            TASKNO = sta.TaskNumber,
                            EQUIPMENTTYPE = "STA",
                        },
                        new WCS_SUBTASKINFO()
                        {
                            TASKSTATUS = "1"
                        }
                    ))
                    {
                        WriteLogBoth($"站台:{sta.StationNo},托盘号:{sta.Barcode},任务号:{sta.TaskNumber},完成输送机任务失败！");
                    }
                    //AGV任务可执行
                    if (!_repository.DealTaskStatus(
                        new WCS_SUBTASKINFO()
                        {
                            TASKNO = sta.TaskNumber,
                            EQUIPMENTTYPE = "AGV",
                        },
                        new WCS_SUBTASKINFO()
                        {
                            RUNSTATUS = "1"
                        })
                    )
                    {
                        WriteLogBoth($"站台:{si.StationNo},托盘号:{sta.Barcode},任务号:{sta.TaskNumber},修改AGV任务状态为(可执行)失败！");
                    }
                    stationTaskInfo[4] = "0";
                    WriteStationInfo(STACommand.ReWrite, stationTaskInfo, sta.StationNo, 0, DbLength);
                    return;
                }
                #endregion

                #region 机台退料放货站台
                if (sta.StationType.Equals(4) && (sta.GoodsType.Equals(1) || sta.GoodsType.Equals(2) || sta.GoodsType.Equals(10)))
                {
                    //表示退回
                    if (sta.FromStation.Equals(5657))
                    {
                        _logMessage = $"站台:{sta.StationNo},托盘号：{sta.Barcode},到位。来源地址：{sta.FromStation}";
                        if (sta.CheckInfo > 0)
                        {
                            _logMessage += $"故障：{GetErrorInfo(sta.CheckInfo)}";
                        }
                        WriteLogBoth(_logMessage);
                        stationTaskInfo[4] = "0";
                        WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                        SendLedMessage(sta.StationNo, _logMessage);
                        return;
                    }

                    //表示申请写入RFID
                    if (sta.FromStation.Equals(sta.ToStation))
                    {
                        //单托盘
                        if (sta.GoodsType.Equals(2))
                        {
                            stationTaskInfo[4] = "5656";
                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            return;
                        }

                        //实托盘写入，托盘组不写入
                        if (sta.GoodsType.Equals(1) && sta.TaskNumber < 10000)
                        {
                            isRfidUsing = _repository.GetWcsParamConfigValueByCode($"Write_Rfid_{sta.StationNo}");
                            if (isRfidUsing.Equals("1"))
                            {
                                if (StaticCommon.RfidEquipmentList.Exists(x => x.EquipmentNo.Equals(sta.StationNo)))
                                {
                                    RfidEquipment = StaticCommon.RfidEquipmentList.Where(x => x.EquipmentNo.Equals(si.StationNo)).ToList()[0];
                                    if (RfidEquipment != null)
                                    {
                                        isRfidWriteSuccess = RfidEquipment.Rfid.WriteUserDataString(sta.Barcode);
                                        if (!isRfidWriteSuccess)
                                        {
                                            stationTaskInfo[4] = "0";
                                            _logMessage = $"站台：{si.StationNo},托盘号:{sta.Barcode},RFID写入失败！";
                                            WriteLogBoth(_logMessage);
                                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                                            SendLedMessage(sta.StationNo, _logMessage);
                                            return;
                                        }
                                    }
                                }

                            }
                        }

                        stationTaskInfo[4] = "5647";
                        WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                        if (Convert.ToInt32(stationTaskInfo[0]) > 10000)
                        {
                            _logMessage = $"站台号:{sta.StationNo},托盘号:{sta.Barcode},紧急入库托盘,前往目标站台:{stationTaskInfo[4]}";
                            SendLedMessage(sta.StationNo, _logMessage);
                        }
                        WriteLogBoth(_logMessage);

                    }
                    return;
                }
                #endregion

                #region 人工送料流程.一楼卷包车间到提升机入库口
                if (sta.StationType.Equals(5) && sta.TaskNumber < 10000)
                {
                    string sourceAddress = string.Empty;
                    string targetAddress = string.Empty;
                    if (sta.StationNo.Equals("5659") && sta.GoodsType != 0)
                    {
                        if (sta.Qty > 0)
                        {
                            for (int i = 0; i < sta.Qty; i++)
                            {
                                if (sta.GoodsType.Equals(1))
                                {
                                    sourceAddress = "5659";
                                    targetAddress = "8888";
                                }
                                else
                                {
                                    sourceAddress = "8888";
                                    targetAddress = "5659";
                                }
                                WCS_RECEIVE_TASK receiveTask = new WCS_RECEIVE_TASK();
                                string wmsTaskNo = wmsTaskNo = $"U-{DateTime.Now:yyyyMMddHHmmss}";
                                receiveTask.WAREHOUSEID = WareHouseID;
                                receiveTask.ZONEID = ZoneId01;
                                receiveTask.TASKNUM = wmsTaskNo;
                                receiveTask.SOURCEADDRESS = sourceAddress;
                                receiveTask.TARGETADDRESS = targetAddress;
                                receiveTask.CONTAINERBARCODE = "99999999";
                                receiveTask.TASKTYPE = "Task_CallTerminal_ReturnBadPallet";
                                bool insertTaskSuccess = _repository.InsertReceiveTask(receiveTask);
                                if (insertTaskSuccess)
                                {
                                    _repository.SplitReceiveTask(WareHouseID, ZoneId01, wmsTaskNo);
                                }
                            }
                            if (sta.GoodsType.Equals(1))
                            {
                                stationTaskInfo[4] = "0";
                            }
                            else
                            {
                                for (int i = 0; i <= 8; i++)
                                {
                                    stationTaskInfo[i] = "0";
                                }
                            }

                            WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                            logMessage = $"站台：{si.StationNo},响应输送线送料请求成功！";
                            WriteLogBoth(logMessage);
                        }
                    }
                    return;
                }
                #endregion

                #region 5721,二楼agv成品出库rfid写入站台
                if (sta.StationType.Equals(8) && sta.TaskNumber >= 10000)
                {
                    isRfidUsing = _repository.GetWcsParamConfigValueByCode($"Write_Rfid_{sta.StationNo}");
                    if (isRfidUsing.Equals("1"))
                    {
                        if (StaticCommon.RfidEquipmentList.Exists(x => x.EquipmentNo.Equals(si.StationNo)))
                        {
                            RfidEquipment = StaticCommon.RfidEquipmentList.Where(x => x.EquipmentNo.Equals(si.StationNo)).ToList()[0];
                            if (RfidEquipment != null)
                            {
                                isRfidWriteSuccess = RfidEquipment.Rfid.WriteUserDataString(sta.Barcode);
                                if (!isRfidWriteSuccess)
                                {
                                    stationTaskInfo[4] = "5722";
                                    stationTaskInfo[7] = "2";
                                    stationTaskInfo[9] = "0";
                                    WriteLogBoth($"站台:{sta.StationNo},托盘号:{sta.Barcode},RFID写入失败！剔除到目标站台：{stationTaskInfo[4]}");
                                    WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                                    return;
                                }
                            }
                        }

                    }
                    DataTable subTaskTable = _repository.QueryTableData(new WCS_SUBTASKINFO()
                    {
                        TASKNO = sta.TaskNumber,
                        EQUIPMENTTYPE = "STA"
                    });
                    if (subTaskTable == null || subTaskTable.Rows.Count != 1)
                    {
                        _logMessage = $"站台:{si.StationNo},托盘号:{sta.Barcode},任务号:{sta.TaskNumber},找不到输送机任务,所以无法给出目标地址,请检查";
                        WriteLogBoth(_logMessage);
                        return;
                    }
                    stationTaskInfo[4] = subTaskTable.Rows[0]["tostation"].ToString() ?? "0";
                    WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                    _logMessage = $"站台号:{sta.StationNo},托盘号:{sta.Barcode},Rfid写入成功,前往目标站台:{stationTaskInfo[4]}";
                    WriteLogBoth(_logMessage);
                    return;
                }
                #endregion

                #region 一楼配盘上托盘口,和二楼三楼出库口,Led显示
                if (sta.StationType.Equals(7))
                {
                    if (sta.TaskNumber >= 10000)
                    {
                        _logMessage = $"站台:{sta.StationNo},任务号：{sta.TaskNumber},托盘号：{sta.Barcode}。下架到位！";
                        //完成输送机任务
                        _repository.DealTaskStatus(
                               new WCS_SUBTASKINFO()
                               {
                                   TASKNO = sta.TaskNumber,
                                   EQUIPMENTTYPE = "STA",
                               },
                               new WCS_SUBTASKINFO()
                               {
                                   TASKSTATUS = "1"
                               });
                    }
                    else
                    {
                        //if (sta.SystemError > 0)
                        //{
                        //    _logMessage += _repository.GetSystemErrorDescriptionByCode(sta.SystemError.ToString());
                        //}
                        if (sta.CheckInfo > 0)
                        {
                            _logMessage += GetErrorInfo(sta.CheckInfo);
                        }
                        if (sta.RfidFlag.Equals(2))
                        {
                            _logMessage += $"Rfid写入失败";
                        }
                    }
                    SendLedMessage(sta.StationNo, _logMessage);
                    WriteLogBoth(_logMessage);
                    stationTaskInfo[4] = "0";
                    WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                    return;
                }
                #endregion

                #region 5659，一楼紧急供货口
                if (sta.StationType.Equals(5) && sta.TaskNumber >= 10000)
                {
                    _logMessage = $"站台:{sta.StationNo},任务号:{sta.TaskNumber},托盘号:{sta.Barcode},一楼紧急入库口,托盘到位！";
                    WriteLogBoth(_logMessage);
                    //完成输送机任务
                    _repository.DealTaskStatus(
                           new WCS_SUBTASKINFO()
                           {
                               TASKNO = sta.TaskNumber,
                               EQUIPMENTTYPE = "STA",
                               TOSTATION = sta.StationNo.ToString()
                           },
                           new WCS_SUBTASKINFO()
                           {
                               TASKSTATUS = "1"
                           });
                    //AGV任务可执行
                    if (!_repository.UpdateSubTask(
                        new WCS_SUBTASKINFO()
                        {
                            TASKNO = sta.TaskNumber,
                            EQUIPMENTTYPE = "AGV",
                            FROMSTATION = sta.StationNo.ToString()
                        },
                        new WCS_SUBTASKINFO()
                        {
                            RUNSTATUS = "1"
                        })
                    )
                    {
                        WriteLogBoth($"站台:{si.StationNo},托盘号:{sta.Barcode},一楼紧急入库口,更新AGV任务为【可执行】失败！");
                    }
                    stationTaskInfo[4] = "0";
                    WriteStationInfo(STACommand.ReWrite, stationTaskInfo, sta.StationNo, 0, DbLength);
                    SendLedMessage(sta.StationNo, _logMessage);
                    return;
                }

                #endregion

                #region 5716,二楼agv紧急取货口
                if (sta.StationNo.Equals("5716") && sta.TaskNumber >= 10000)
                {
                    WriteLogBoth($"站台:{sta.StationNo},托盘号:{sta.Barcode},二楼紧急出库口，托盘到位。");
                    //申请Rfid写入
                    if (sta.FromStation.Equals(sta.ToStation))
                    {
                        isRfidUsing = _repository.GetWcsParamConfigValueByCode($"Write_Rfid_{sta.StationNo}");
                        if (isRfidUsing.Equals("1"))
                        {
                            if (StaticCommon.RfidEquipmentList.Exists(x => x.EquipmentNo.Equals(si.StationNo)))
                            {
                                RfidEquipment = StaticCommon.RfidEquipmentList.Where(x => x.EquipmentNo.Equals(si.StationNo)).ToList()[0];
                                if (RfidEquipment != null)
                                {
                                    isRfidWriteSuccess = RfidEquipment.Rfid.WriteUserDataString(sta.Barcode);
                                    if (!isRfidWriteSuccess)
                                    {
                                        stationTaskInfo[4] = "0";
                                        stationTaskInfo[7] = "2";
                                        stationTaskInfo[9] = "4";
                                        WriteLogBoth($"站台:{sta.StationNo},任务号:{sta.TaskNumber},托盘号:{sta.Barcode},RFID写入失败！");
                                        WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                                        return;
                                    }
                                }
                            }
                        }
                        DataTable subTaskTable = _repository.QueryTableData(new WCS_SUBTASKINFO()
                        {
                            TASKNO = sta.TaskNumber,
                            EQUIPMENTTYPE = "STA"
                        });
                        if (subTaskTable == null || subTaskTable.Rows.Count != 1)
                        {
                            _logMessage = $"站台:{si.StationNo},托盘号:{sta.Barcode},任务号:{sta.TaskNumber},找不到输送机任务,所以无法给出目标地址,请检查!";
                            WriteLogBoth(_logMessage);
                            return;
                        }
                        stationTaskInfo[4] = subTaskTable.Rows[0]["tostation"].ToString() ?? "0";
                        WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                        _logMessage = $"站台号:{sta.StationNo},托盘号:{sta.Barcode},任务号:{sta.TaskNumber},Rfid写入成功,前往目标站台:{stationTaskInfo[4]}";
                        WriteLogBoth(_logMessage);
                    }
                    return;
                }
                #endregion

                #region 二楼空托盘码盘入库
                if (sta.StationType.Equals(10) && sta.TaskNumber < 10000 && sta.GoodsType.Equals(10))
                {
                    WriteLogBoth($"站台:{sta.StationNo},托盘号:{sta.Barcode},空托盘入库口,托盘到位！");
                    inboundRequest.Msg_Params[0].TaskType = "1";
                    inboundRequest.Msg_Params[0].ZoneID = ZoneId02;
                    wmsServiceResult = WMSMicroServiceUtil.RequestWMS(servicRequest);
                    if (!wmsServiceResult.IsSuccess || string.IsNullOrEmpty(wmsServiceResult.ResultJson))
                    {
                        WriteLogBoth($"站台:{si.StationNo},托盘号:{sta.Barcode},WMS接口调用失败,详细信息:{wmsServiceResult.ErrorMessage}！");
                        return;
                    }
                    WMSInBoundResponse response = JsonConvert.DeserializeObject<WMSInBoundResponse>(wmsServiceResult.ResultJson);
                    if (!response.isSucceed)
                    {
                        WriteLogBoth($"站台:{sta.StationNo},托盘号:{sta.Barcode},WMS接口反馈失败,详细信息:{response.errmsg}！");
                        return;
                    }
                    WCS_RECEIVE_TASK taskInfo = _mapper.Map<WCS_RECEIVE_TASK>(response.json);
                    //插入WMS任务
                    if (!_repository.InsertReceiveTask(taskInfo))
                    {
                        stationTaskInfo[4] = "0";
                        stationTaskInfo[9] = "3";
                        WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                        WriteLogBoth($"站台:{sta.StationNo},托盘号:{sta.Barcode},WCS插入WMS任务失败.");
                        return;
                    }
                    //拆分WMS任务
                    string[] splitResult = _repository.SplitReceiveTask(taskInfo.WAREHOUSEID, taskInfo.ZONEID, taskInfo.TASKNUM);
                    if (!splitResult[1].Equals("1"))
                    {
                        stationTaskInfo[4] = "0";
                        stationTaskInfo[9] = "4";
                        WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                        WriteLogBoth($"站台:{sta.StationNo},托盘号:{sta.Barcode},WCS拆分WMS任务失败,请人工检查！");
                        return;
                    }
                    //修改目标地址
                    _wcsTaskNo = _repository.GetWcsTaskNoByWmsTaskNo(taskInfo.TASKNUM);
                    if (string.IsNullOrEmpty(_wcsTaskNo))
                    {
                        stationTaskInfo[4] = "0";
                        stationTaskInfo[9] = "5";
                        WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                        WriteLogBoth($"站台:{sta.StationNo},托盘号:{sta.Barcode},WCS根据WMS任务号:{taskInfo.TASKNUM},查询到0个或者多个WCS任务,请检查！");
                        return;
                    }
                    _ToStation = "0";
                    stationTaskInfo[0] = _wcsTaskNo;
                    stationTaskInfo[4] = _ToStation;
                    WriteStationInfo(STACommand.ReWrite, stationTaskInfo, si.StationNo, 0, DbLength);
                    return;
                }
                #endregion

                return;
            }

            catch (Exception ex)
            {
                WriteLogBoth($"函数DoWork异常:{ex.Message}{ex.StackTrace}");
                //LogUtil.Write(LogFileName.ConveyorExceptionLog, LogFileName.ConveyorExceptionLog,$"函数DoWork异常:{ex.Message}{ex.StackTrace}");
            }
        }

        public void testInsertTask()
        {
            string sourceAddress = "JJR8888";
            string targetAddress = "5659";
            WCS_RECEIVE_TASK receiveTask = new WCS_RECEIVE_TASK();
            string wmsTaskNo = _repository.CreateWmsTaskNo("Task");
            receiveTask.WAREHOUSEID = WareHouseID;
            receiveTask.TASKNUM = wmsTaskNo;
            receiveTask.SOURCEADDRESS = sourceAddress;
            receiveTask.TARGETADDRESS = targetAddress;
            receiveTask.CONTAINERBARCODE = "99999999";
            receiveTask.TASKTYPE = "Task_CallTerminal_ReturnBadPallet";
            bool insertTaskSuccess = _repository.InsertReceiveTask(receiveTask);
            if (insertTaskSuccess)
            {
                _repository.SplitReceiveTask(WareHouseID, ZoneId01, wmsTaskNo);
            }
        }


        public void WriteLogBoth(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                WriteToDowntLog(message);
                _repository.AddLog(message);
            }
        }








        /// <summary>
        /// 写入命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="StationInfo"></param>
        /// <param name="stationum"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool WriteStationInfo(STACommand cmd, string[] StationInfo, string stationum, int index, int length)
        {
            return plc.Write(cmd, StationInfo, stationum, index, length);
        }


        public bool WriteStationInfo(string[] StationInfo, string stationum)
        {
            ConveryEntity si = lst.Find(
                delegate (ConveryEntity ss)
                {
                    return ss.StationNo == stationum;
                });
            if (si != null)
            {
                return WriteStationInfo(STACommand.ReWrite, StationInfo, stationum, si.value, si.length);
            }
            return false;
        }

        /// <summary>
        /// 读取站台信息
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public string[] ReadStationInfo(string StationNo)
        {
            return ConveryPLC.ReadStationInfo(StationNo);
        }


        /// <summary>
        /// 任务转换
        /// </summary>
        /// <param name="TaskInfo"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private string[] ConvertToTaskInfo(string AreaCode, string[] TaskInfo, string Msg, ref string Ledtext)
        {
            try
            {
                if (string.IsNullOrEmpty(Msg))
                    throw new Exception(string.Format("任务信息为空"));
                string[] strTemp = Msg.Split(';');//分解结果
                string Barcode = string.Empty;

                for (int i = 0; i < strTemp.Length; i++)
                {
                    if (strTemp[i] != null)
                    {
                        string str = strTemp[i].ToString().Substring(0, 2);

                        switch (str)
                        {
                            //任务号
                            case "TN": TaskInfo[0] = strTemp[i].ToString().Substring(strTemp[i].ToString().LastIndexOf(":") + 1, strTemp[i].ToString().Length - strTemp[i].ToString().LastIndexOf(":") - 1); break;
                            //托盘条码
                            case "BC":
                                {
                                    Barcode = strTemp[i].ToString().Substring(strTemp[i].ToString().LastIndexOf(":") + 1, strTemp[i].ToString().Length - strTemp[i].ToString().LastIndexOf(":") - 1);
                                    TaskInfo[1] = (int.Parse(Barcode) / 65536).ToString();
                                    TaskInfo[2] = (int.Parse(Barcode) % 65536).ToString();
                                }
                                break;
                            //货物类型
                            case "GT":
                                {
                                    TaskInfo[3] = strTemp[i].ToString().Substring(strTemp[i].ToString().LastIndexOf(":") + 1, strTemp[i].ToString().Length - strTemp[i].ToString().LastIndexOf(":") - 1);
                                }
                                break;
                            //起始地址
                            case "FS":
                                {
                                    TaskInfo[4] = strTemp[i].ToString().Substring(strTemp[i].ToString().LastIndexOf(":") + 1, strTemp[i].ToString().Length - strTemp[i].ToString().LastIndexOf(":") - 1);
                                }
                                break;
                            //目标站台
                            case "TS":
                                {
                                    TaskInfo[5] = strTemp[i].ToString().Substring(strTemp[i].ToString().LastIndexOf(":") + 1, strTemp[i].ToString().Length - strTemp[i].ToString().LastIndexOf(":") - 1);
                                }
                                break;
                            //LED显示
                            case "ED":
                                {
                                    Ledtext = strTemp[i].ToString().Substring(strTemp[i].ToString().LastIndexOf(":") + 1, strTemp[i].ToString().Length - strTemp[i].ToString().LastIndexOf(":") - 1);
                                }
                                break;
                        }
                    }
                }
                return TaskInfo;
            }
            catch (Exception ex)
            {
                throw new Exception("任务信息转换失败，具体信息如下:" + ex.Message.ToString());
            }

        }

        /// <summary>
        /// 输送机界面显示
        /// </summary>
        /// <param name="ci"></param>
        private void ShowConveryPLCStatus(PLCStatus plc)
        {
            ShowStationPLCStatus?.Invoke(plc._IP, plc._elapsedMilliseconds.ToString(), plc._connectStatus);
        }


        public string[] ConveyorEntityToArray(ConveryEntity si)
        {
            try
            {
                string[] info = new string[10];
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
                return info;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private string GetErrorInfo(int checkinfo)
        {
            string _ledtext = string.Empty;
            string errorcode = Convert.ToString(checkinfo, 2).PadLeft(16, '0');
            char[] ErrorCode = errorcode.ToCharArray();
            for (int j = 0; j < 16; j++)
            {
                if (ErrorCode[j] == '1')
                {
                    switch (j)
                    {
                        case 0:
                            _ledtext = _ledtext + "";
                            break;
                        case 1:
                            _ledtext = _ledtext + "";
                            break;
                        case 2:
                            _ledtext = _ledtext + "右超宽;";
                            break;
                        case 3:
                            _ledtext = _ledtext + "左超宽;";
                            break;
                        case 4:
                            _ledtext = _ledtext + "后超长;";
                            break;
                        case 5:
                            _ledtext = _ledtext + "前超长;";
                            break;
                        case 6:
                            _ledtext = _ledtext + "超高;";
                            break;
                        case 7:
                            _ledtext = _ledtext + "故障;";
                            break;
                        case 15:
                            _ledtext = _ledtext + "烟包条码读取错误;";
                            break;
                        case 14:
                            _ledtext = _ledtext + "托盘条码读取错误;";
                            break;
                        case 13:
                            _ledtext = _ledtext + "重量读取错误;";
                            break;
                        default:
                            _ledtext = _ledtext + "未知异常;";
                            break;
                    }
                }
            }
            return _ledtext;
        }
    }
}
