using CWCS.Common.Log;
using CWCS.Core.Library.SRMManager;
using CWCS.Entity;
using CWCS.Entity.SRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.IO;
using static System.Collections.Specialized.BitVector32;
using System.Threading.Tasks;

namespace CWCS.BLL
{
    public class SRMBLL : BLLBase
    {
        private SRMTask sctask;
        public static Thread TaskThread;
        public delegate void ShowEquipmentMessage(string type, string msg);
        public event ShowEquipmentMessage ShowSRMMsg;
        public delegate void ShowEquipmentStatus(SRM_Status_Entity srminfo);
        public event ShowEquipmentStatus ShowSCStatus;
        public static Dictionary<string, string> SCErrorInfoDictionay = new Dictionary<string, string>();

        public SRMBLL()
        {
            try
            {
                sctask = new SRMTask();
            }
            catch (Exception ex)
            {
                NotityMesssage("SRMMsgInit", $"{ex.Message}{ex.StackTrace}");
            }
        }

        #region 堆垛机配置

        /// <summary>
        /// 更新设备模式
        /// </summary>
        /// <returns></returns>
        public bool Update_SCMode(string scno, SCTaskMode taskmode)
        {
            return _repository.Update_SCMode(scno, taskmode);
        }

        #endregion

        /// <summary>
        /// 设备运行
        /// </summary>
        /// <param name="SCType"></param>
        public void Start(string SCType)
        {
            try
            {
                if (InitSRM())
                {
                    TaskThread = new Thread(new ThreadStart(TimeScan));
                    TaskThread.IsBackground = true;
                    TaskThread.Start();
                }
                if (File.Exists(".\\SCAlarm.txt"))
                {
                    FileStream fs = new FileStream(".\\SCAlarm.txt", FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs);
                    string strline = sr.ReadToEnd();
                    string[] msg = strline.Split(new char[] { ';' });
                    string[] msg2;
                    for (int i = 0; i < msg.Length; i++)
                    {
                        msg2 = msg[i].Split(new char[] { ',' });
                        if (!AlarmHash.Contains(msg2[0]) && msg2.Length > 1)
                            AlarmHash.Add(msg2[0], msg2[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                NotityMesssage("SRMInit", "堆垛机启动异常：" + ex.Message);
            }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool InitSRM()
        {
            try
            {
                DataTable dt = _repository.GetSCInfo();
                if (dt != null)
                {
                    var rows = dt.AsEnumerable().ToArray();
                    if (rows != null)
                    {
                        List<WCS_PARAMETER_CONFIG> listParam = StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.StartsWith("SC")).ToList();
                        foreach (WCS_PARAMETER_CONFIG item in listParam)
                        {
                            if (item.PARAM_VALUE.Equals("0"))
                            {
                                rows = rows.Where(x => !x["EQUIPMENTNUM"].ToString().Equals(item.PARAM_CODE)).ToArray();
                            }
                        }
                        if (rows != null && rows.Length > 0)
                        {
                            foreach (DataRow dr in rows)
                            {
                                AddSRM(dr["EQUIPMENTNUM"].ToString(), dr["ripaddr"].ToString(), int.Parse(dr["rport"].ToString()), 1, dr["equipment_mode"].ToString());
                            }
                        }

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddSRM(string scno, string ipaddress, int port, int aisle, string taskmode)
        {
            SRMObject srm = new SRMObject();
            srm.SrmNo = scno;
            srm.IpAddress = ipaddress;
            srm.Port = port;
            srm.aislenumber = aisle;
            srm.ScMode = true;
            srm.checkout = true;
            srm.CSrm = new SRMControl(scno, ipaddress, port);
            srm.CSrm.AnalyseMessageEvent += new SRMControl.AnalyseMessageEventHandler(OSRM_AnalyseMessageEvent);
            bool connectFlag = srm.CSrm.Startup();
            NotityMesssage("Connect", string.Format("WCS与{0},IP:{1},Port:{2},连接{3}!", scno, ipaddress, port, connectFlag ? "成功" : "失败"));
            lstSRM.Add(srm);
            if (port == 4000)
            {
                SRM_Status_Entity status = new SRM_Status_Entity();
                status.TaskNo = "0";
                status.RackLevel = "0";
                status.CRCNum = scno;
                status.Connectstatus = connectFlag;
                status.AisleNumber = aisle.ToString();
                status.SCTaskMode = taskmode.Equals("All") ? SCTaskMode.All : taskmode.Equals("OnlyInFeed") ? SCTaskMode.OnlyInFeed : taskmode.Equals("OnlyOutFeed") ? SCTaskMode.OnlyOutFeed : SCTaskMode.Pause;
                L_SRMStatus.Add(status);
            }
        }

        private void OSRM_AnalyseMessageEvent(NotifyCommand CommandType, string scno, int port, object msg) //解析收到数据的代理的参数,CommandType是break，connect等
        {
            SRMObject srm = lstSRM.Find(info => info.SrmNo == scno && info.Port == port);
            SRM_Status_Entity sse = L_SRMStatus.Find(info => info.CRCNum == scno);
            if (srm != null && sse != null)
            {
                if (port == 4000 && CommandType == NotifyCommand.RecvData)//执行状态反馈
                {
                    if (srm != null)
                        srm.connectstatus = (srm.connectstatus == 1 ? 2 : 1);

                    sctask.UpdateCRCStatus(sse, scno, msg.ToString());
                }
                else if (port == 6000 && CommandType == NotifyCommand.RecvData)//报警信息反馈
                {
                    sse.AlarmMessage = sctask.GetAlarmInfo(scno, msg.ToString(), true);
                    #region 此处调用wms接口,更新堆垛机状态.改走DBLink形式更新数据库
                    //WMSEquipmentStatusRequest request = new WMSEquipmentStatusRequest();
                    //request.Msg_Mark = $"堆垛机{scno}更新设备状态";
                    //request.Msg_Params[0].WarehouseID = WareHouseID;
                    //request.Msg_Params[0].ZoneID = ZoneId01;
                    //request.Msg_Params[0].StaType = "SC";
                    //request.Msg_Params[0].StaStatus = "Equipment_Error";
                    //request.Msg_Params[0].STA_Errorcode = $"000";
                    //request.Msg_Params[0].STA_Errorinfo = sse.AlarmMessage;
                    //request.Msg_Params[0].EquipmentNo = scno;
                    //WMSServicRequest servicRequest = new WMSServicRequest() 
                    //{ 
                    //    PostData = request
                    //};
                    //ResultObject resultObject = WMSMicroServiceUtil.RequestWMS(servicRequest);
                    //if (!resultObject.IsSuccess)
                    //{
                    //    _repository.AddLog($"堆垛机：{scno},更新设备状态接口反馈失败.详细信息:{resultObject.ErrorMessage}");
                    //}
                    #endregion

                    if (!string.IsNullOrEmpty(sse.AlarmMessage))
                    {
                        _repository.Interface_UpdateEquipmentStatus(new UpdateEquipmentStatusModel()
                        {
                            WarehouseId = WareHouseID,
                            ZoneId = ZoneId01,
                            EquipmentNo = sse.CRCNum,
                            EquipmentStatus = "EquipmentStatus_Error",
                            ErrorDescription = sse.AlarmMessage
                        });
                    }

                }
                else if (CommandType == NotifyCommand.SendData)
                {
                    NotityMesssage("SRMMsg", msg.ToString());
                }
                else if (CommandType == NotifyCommand.Break)
                {
                    if (srm != null)
                        srm.connectstatus = 0;
                    sse.Connectstatus = false;
                    NotityMesssage("Break", msg.ToString());
                }
                else if (CommandType == NotifyCommand.Connect)
                {
                    if (srm != null)
                    {
                        srm.connectstatus = 1;
                    }
                    sse.Connectstatus = true;
                    SRMObject srm2000 = lstSRM.Find(info => info.SrmNo == scno && info.Port == 2000);
                    if (srm2000 != null && port == 4000)
                    {
                        srm2000.CSrm.Startup();
                    }
                    NotityMesssage("Connect", msg.ToString());
                }
                if (sse.Change)
                    ShowSCStatus(sse);
            }
        }

        /// <summary>
        /// 过滤同一行的任务，不允许两台堆垛机同时执行同一行的任务
        /// </summary>
        private DataTable CheckSameRowTask(DataTable dataTable)
        {
            try
            {
                DataTable newTable = dataTable;
                var executingTask = _repository.GetAllScExecuting();
                foreach(DataRow taskrow in executingTask.Rows)
                {
                    //执行中的为出库任务
                    var fromStation = taskrow["fromstation"].ToString();                   
                    if (fromStation.Length == 8)
                    {
                        //拿到正在执行任务的行
                        var sameRow = fromStation.Substring(0, 3);
                        //过滤相同行的任务,留下的任务都可以执行
                        newTable = newTable.AsEnumerable().Where(x => !x["fromstation"].ToString().StartsWith(sameRow) && !x["tostation"].ToString().StartsWith(sameRow)).CopyToDataTable();                                          
                    }

                    //执行中的为入库任务
                    var toStation = taskrow["tostation"].ToString();
                    if (toStation.Length == 8)
                    {
                        //拿到正在执行任务的行
                        var sameRow = toStation.Substring(0, 3);
                        //过滤相同行的任务,留下的任务都可以执行
                        newTable = newTable.AsEnumerable().Where(x => !x["fromstation"].ToString().StartsWith(sameRow) && !x["tostation"].ToString().StartsWith(sameRow)).CopyToDataTable();
                    }
                }

                int beforeCheckTask = dataTable.Rows.Count;
                int afterCheckTask = newTable.Rows.Count;

                LogUtil.Write(LogFileName.ScLog, LogFileName.ScLog, $"过滤同一行的任务成功，过滤掉的任务数：{beforeCheckTask - afterCheckTask}");

                return newTable;
            }
            catch (Exception e)
            {
                LogUtil.Write(LogFileName.ScLog, LogFileName.ScLog, $"过滤同一行的任务时异常，{e.Message}");
                return dataTable;
            }
        }

        /// <summary>
        /// 检查并处理有故障的堆垛机任务
        /// </summary>
        private void CheckErrorTask(List<SRM_Status_Entity> L_SRMStatus)
        {
            //遍历堆垛机
            foreach (var sse in L_SRMStatus)
            {
                //判断这台堆垛机是否存在任务号，过滤任务号等于0但是有起始地址或者目标地址的情况
                //如果是这种情况的话，把正在执行中的任务改成未开始，让WCS重新下发即可
                if (sse.TaskNo == "0")
                {
                    //堆垛机的任务号是0，但是起始地址和目标地址是不是空
                    if ((sse.Fromstation != null && sse.Fromstation.Length >= 4) || (sse.Tostation != null && sse.Tostation.Length >= 4))
                    {
                        LogUtil.Write(LogFileName.ScLog, LogFileName.ScLog, $"WCS发现堆垛机{sse.CRCNum}有任务号写入失败的情况;设备任务号:{sse.TaskNo};起始地址:{sse.Fromstation},目标地址:{sse.Tostation};");

                        //说明出现了任务写入任务号失败的情况
                        var executingTaskTable = _repository.GetScExecuting(sse.CRCNum);
                        if (executingTaskTable.Rows.Count > 0)
                        {
                            //把正在执行中的任务改成未开始，让WCS自动重新下发
                            var executingTask = executingTaskTable.Rows[0];
                            var taskNo = executingTask["taskno"];
                            var fromStation = executingTask["fromstation"];
                            var toStation = executingTask["tostation"];
                            //校验起始地址和目标地址和设备是否匹配
                            if (fromStation.ToString() == sse.Fromstation.ToString() && toStation.ToString() == sse.Tostation.ToString())
                            {
                                //任务状态更新
                                _repository.DealTaskStatus(
                                       new WCS_SUBTASKINFO()
                                       {
                                           TASKNO = Convert.ToInt32(taskNo),
                                           FROMSTATION = fromStation.ToString(),
                                           TOSTATION = toStation.ToString(),
                                           EQUIPMENTTYPE = "SC",
                                       },
                                       new WCS_SUBTASKINFO()
                                       {
                                           TASKSTATUS = "0"
                                       });

                                LogUtil.Write(LogFileName.ScLog, LogFileName.ScLog, $"WCS发现堆垛机{sse.CRCNum}有任务号写入失败的情况,自动修正状态成功;数据库任务号:{taskNo};起始地址:{fromStation},目标地址:{toStation};");
                            }
                            else
                            {
                                LogUtil.Write(LogFileName.ScLog, LogFileName.ScLog,
                                    $"WCS发现堆垛机{sse.CRCNum}有任务号写入失败的情况,找到的任务地址与堆垛机地址不一致;" +
                                    $"数据库任务号:{taskNo};起始地址:{fromStation},目标地址:{toStation};" +
                                    $"堆垛机任务号:{sse.TaskNo};起始地址:{sse.Fromstation},目标地址:{sse.Tostation}");
                            }

                        }
                        else
                        {
                            LogUtil.Write(LogFileName.ScLog, LogFileName.ScLog, $"WCS发现堆垛机{sse.CRCNum}有任务号写入失败的情况,但是无法找到对应执行中的任务;设备任务号:{sse.TaskNo};起始地址:{sse.Fromstation},目标地址:{sse.Tostation};");
                        }
                    }
                }
            }

        }

        private void TimeScan()
        {
            while (true)
            {
                try
                {
                    //检查并处理有问题的任务
                    CheckErrorTask(L_SRMStatus);
                    Thread.Sleep(100);

                    //获取可执行的任务
                    DataTable dt = sctask.GetTask();
                    DataRow rowTemp;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (var sse in L_SRMStatus)
                        {
                            //获取这台堆垛机是否有正在执行中的任务，有就跳过
                            if (_repository.IsScExecuting(sse.CRCNum))
                            {
                                continue;
                            }
                            //判断当前可执行任务有没有和执行中的任务的行（货位前三位）相同的任务，有则过滤掉这些任务。   禁止两台堆垛机同时执行同一行的任务。
                            dt = CheckSameRowTask(dt);
                            if(dt.Rows.Count == 0)
                            {
                                continue;
                            }

                            SRMObject sob = lstSRM.Find(x => x.SrmNo.Equals(sse.CRCNum) && x.Port.Equals(2000) && x.ScMode);
                            if (sob != null && sse.FunctionReport == CR_NO_FUNC && sse.CrcTaskFlag.Equals("1"))
                            {
                                DataRow[] rows = dt.AsEnumerable().Where(x => x["equipmentnumber"].ToString().Equals(sse.CRCNum)).ToArray();
                                DataRow taskRow = null;
                                if (rows != null && rows.Length > 0)
                                {
                                    taskRow = rows[0];
                                    if (taskRow != null && CanTaskExecute(taskRow["tasktype"].ToString(), sse.SCTaskMode))
                                    {
                                        if (sse.CRCNum.Equals("SC02"))
                                        {
                                            //入库
                                            if (taskRow["tasktype"].ToString().Equals("TaskType_InBound") && !taskRow["fromstation"].ToString().Equals("5647"))
                                            {
                                                rowTemp = rows.Where(x => x["tasktype"].ToString().Equals("TaskType_InBound") &&
                                                                          x["fromstation"].ToString().Equals("5647")).FirstOrDefault();
                                                if (rowTemp != null)
                                                {
                                                    taskRow = rowTemp;
                                                }
                                            }
                                        }
                                        //任务类型转换
                                        if (sctask.UpdateCRCTaskInfo(taskRow["equipmentnumber"].ToString(), taskRow["fromstation"].ToString(), taskRow["tostation"].ToString(), taskRow["tasktype"].ToString(), false))
                                        {
                                            //任务下发
                                            sob.CSrm.SendCode(taskRow["equipmentnumber"].ToString(), CR_GETC, taskRow["taskno"].ToString(), taskRow["fromstation"].ToString(), false);
                                            //任务状态更新
                                            _repository.DealTaskStatus(
                                                   new WCS_SUBTASKINFO()
                                                   {
                                                       TASKNO = Convert.ToInt32(taskRow["taskno"]),
                                                       EQUIPMENTTYPE = "SC",
                                                   },
                                                   new WCS_SUBTASKINFO()
                                                   {
                                                       TASKSTATUS = "2"
                                                   });
                                            continue;
                                        }
                                    }

                                }
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    LogUtil.Write(LogFileName.ScExceptionLog, LogFileName.ScExceptionLog, $"轮询可执行的堆垛机任务时出现异常。{ex.Message}{ex.StackTrace}");
                }
                Thread.Sleep(1500);
            }
        }


        public bool CanTaskExecute(string taskTypeCode, SCTaskMode taskMode)
        {
            if (taskTypeCode.Equals("TaskType_Inbound") || taskTypeCode.Equals("TaskType_InBound") || taskTypeCode.Equals("Task_CallTerminal_ReturnPallet"))
            {
                if (taskMode == SCTaskMode.OnlyInFeed || taskMode == SCTaskMode.All)
                {
                    return true;
                }
            }
            if (taskTypeCode.Equals("TaskType_Outbound") || taskTypeCode.Equals("TaskType_OutBound") || taskTypeCode.Equals("Task_CallTerminal_Call"))
            {
                if (taskMode == SCTaskMode.OnlyOutFeed || taskMode == SCTaskMode.All)
                {
                    return true;
                }
            }
            if (taskTypeCode.Equals("TaskType_Move"))
            {
                return true;
            }
            return false;
        }



        /// <summary>
        /// 通知界面显示
        /// </summary>
        /// <param name="scnum"></param>
        /// <param name="msg"></param>
        private void NotityMesssage(string type, string msg)
        {
            if (ShowSRMMsg != null)
            {
                ShowSRMMsg(type, msg);//通知界面显示
                LogUtil.Write(LogFileName.ScLog, LogFileName.ScLog, msg);
            }
        }
        #region Other  Function

        public void ChangeWCSWorkMode(string scnum, int WCSWorkMode)
        {
            SRMObject sob = lstSRM.Find(info => info.SrmNo == scnum && info.Port == 2000);
            if (sob != null) sob.ScMode = WCSWorkMode > 0;
        }

        public bool ChanageWCSTaskMode(string scno, SCTaskMode taskmode, bool checkout)
        {
            SRM_Status_Entity sse = L_SRMStatus.Find(
           delegate (SRM_Status_Entity info)
           {
               return info.CRCNum == scno;
           });
            if (sse != null)
            {
                sse.SCTaskMode = taskmode;
                sse.CheckOut = checkout;
                return Update_SCMode(scno, taskmode);
            }
            return false;
        }


        #endregion

        /// <summary>
        /// 手动发送命令
        /// </summary>
        /// <param name="taskno"></param>
        /// <param name="scno"></param>
        /// <param name="sourceAddress"></param>
        /// <param name="targetAddress"></param>
        /// <param name="cmdType"></param>
        public bool SendTaskByMan(string scno, string taskno, string sourceAddress, string targetAddress, SRMCommand cmdType, bool isOpen = false)
        {
            try
            {
                SRMObject sob = lstSRM.Find(x => x.SrmNo == scno && x.Port == 2000);
                if (sob != null)
                {
                    {
                        string sendAddress = string.Empty;
                        if (cmdType == SRMCommand.CR_NO_FUNC)
                        {
                            sctask.UpdateCRCTaskInfo(scno, "", "", "", true);
                            return sob.CSrm.SendCode(scno, Convert.ToString(Convert.ToInt32(cmdType)), taskno, "0000000000", true);
                        }
                        else if (cmdType == SRMCommand.CR_GETC)
                        {
                            sendAddress = sourceAddress;
                        }
                        else if (cmdType == SRMCommand.CR_PUTC)
                        {
                            sendAddress = targetAddress;
                        }
                        else if (cmdType == SRMCommand.CR_POS_GET)
                        {
                            sendAddress = targetAddress;
                        }
                        else if (cmdType == SRMCommand.CR_PICKSH_PUTC)
                        {
                            sendAddress = targetAddress;
                        }
                        else if (cmdType == SRMCommand.CR_DROPSH_PUTC)
                        {
                            sendAddress = targetAddress;
                        }
                        if (string.IsNullOrEmpty(sendAddress))
                        {
                            return false;
                        }
                        sctask.UpdateCRCTaskInfo(scno, sourceAddress, targetAddress, "", true);
                        return sob.CSrm.SendCode(scno, Convert.ToString(Convert.ToInt32(cmdType)), taskno, sendAddress, true);

                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetTaskInfo(string taskno)
        {
            return _repository.GetTaskInfo(taskno);
        }


    }
}
