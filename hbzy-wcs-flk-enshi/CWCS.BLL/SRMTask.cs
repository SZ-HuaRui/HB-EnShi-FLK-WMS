using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWCS.Entity.SRM;
using CWCS.Core.Library.PLCManager;
using System.Collections;
using System.Data;
using System.Threading;
using CWCS.Entity;
using CWCS.Repository;
using CWCS.Common.Log;

namespace CWCS.BLL
{
    public class SRMTask : BLLBase
    {
        public string _barcode = string.Empty;
        public string _squen = string.Empty;
        public string _weight = string.Empty;
        public string _lane = string.Empty;
        public string _executingsc = string.Empty;
        public string _tostation = string.Empty;
        public string _groupno = string.Empty;
        private static readonly object obj = new object();

        public void UpdateCRCStatus(SRM_Status_Entity sse, string num, string str)
        {
            lock (obj)
            {
                string[] msg = str.Split(new char[] { ';' });
                string fromStationTemp = string.Empty;
                string toStationTemp = string.Empty;
                string shuttleNo = string.Empty;

                try
                {
                    fromStationTemp = sse.Fromstation;
                    toStationTemp = sse.Tostation;
                    if (sse != null)
                    {
                        if (str == "Break")
                        {
                            sse.CrcStatus = EQUIPMENT_ERROR;
                            sse.CraneMode = "0";
                            sse.LabelInfo = "位置:" + sse.Position + ".模式:关闭.状态:无.起止:" + sse.Fromstation + "-->" + sse.Tostation + ".";
                        }
                        else if (msg.Length > 38)
                        {
                            string mode = EQUIPMENT_ERROR;
                            int[] functionmode = FunctionMode(int.Parse(msg[24]));  //自动，半自动   functionmode：bool
                            int[] cranemode = FunctionMode(int.Parse(msg[52]));//远程，本地
                            if (functionmode[0] == 1 && functionmode[4] == 1 && functionmode[7] == 0)
                            { 
                                mode = EQUIPMENT_OK; 
                            }
                            int t = int.Parse(msg[20].ToString()) * 256 + int.Parse(msg[21].ToString());  
                            sse.TaskNo = t.ToString();
                            sse.CraneMode = msg[52];      
                            sse.FunctionMode = msg[24];     
                            sse.FunctionReportLow = msg[23];//功能编码低位
                            sse.CrcStatus = mode;
                            sse.ZAlarm = functionmode[7];   //总报警
                            sse.ActionPoint = TransStrToInt(msg[30], msg[31]).ToString();
                            sse.AisleNumber = TransStrToInt(msg[32], msg[33]).ToString();//巷道号 09--10
                            sse.RackSide = TransStrToInt(msg[34], msg[35]).ToString();  //巷道左右边 01左 02右    rack side
                            sse.RackPosition = TransStrToInt(msg[36], msg[37]).ToString(); //列值 
                            sse.RackPlace = TransStrToInt(msg[38], msg[39]).ToString();//货位内左右
                            sse.RackLevel = TransStrToInt(msg[40], msg[41]).ToString();
                            sse.RackDepth = TransStrToInt(msg[42], msg[43]).ToString();//深度 
                            sse.FunctionReportHigh = msg[22];//功能编码高位
                            sse.FunctionReport = (Convert.ToInt32(sse.FunctionReportHigh) * 256 + Convert.ToInt32(sse.FunctionReportLow)).ToString();
                            sse.Position = GetCurrentPosition(sse.AisleNumber,sse.RackSide,sse.RackPosition,sse.RackDepth,sse.RackLevel);
                            sse.LabelInfo = string.Format("任务:{0}.位置:{1}.模式:{2}.工作:{3}.出库:{4}.状态:{5}.起止:{6}.", sse.TaskNo, sse.Position, GetFunctionMode(functionmode, cranemode), GetTaskMode(sse.SCTaskMode), sse.CheckOut ? "正常出库" : "快速出库", GetActionStatusBase(sse.FunctionReport), sse.Fromstation + "-->" + sse.Tostation);
                            //显示信息
                            if (sse.LabelInfo == sse.OldLabelInfo && !string.IsNullOrEmpty(sse.LabelInfo))
                            {
                                sse.Change = true;
                            }
                            else
                            {
                                sse.OldLabelInfo = sse.LabelInfo;
                                sse.Change = true;
                            }

                            if (sse.CrcStatus == EQUIPMENT_ERROR || sse.SCTaskMode == SCTaskMode.Pause)
                            {
                                sse.CrcTaskFlag = "0";
                            }
                            else
                            {
                                sse.CrcTaskFlag = "1";
                            }
                            //更新设备状态
                            _repository.Interface_UpdateEquipmentStatus(new UpdateEquipmentStatusModel()
                            {
                                WarehouseId = WareHouseID,
                                ZoneId = ZoneId01,
                                EquipmentNo = sse.CRCNum,
                                EquipmentStatus = mode
                            });

                            if (t != 0 && t.ToString() != sse.TaskNo)
                            {
                                if (sse != null && sse.CRCNum != null)
                                {
                                    LogUtil.Write(LogFileName.ScLog, num + LogFileName.ScLog, $"接收{num}堆垛机状态报文。任务号：{sse.TaskNo},FunctionCode：{sse.FunctionReport}");
                                }
                            }
                            //发送端口
                            SRMObject sob = lstSRM.Find(x => x.SrmNo.Equals(num) && x.Port.Equals(2000));

                            if (sob != null && t > 0)
                            {
                                if (string.IsNullOrEmpty(sse.Fromstation) || string.IsNullOrEmpty(sse.Tostation))
                                {
                                    //查询此时堆垛机任务号对应的起始和目标地址
                                    DataTable dt = _repository.QueryTableData(new WCS_SUBTASKINFO()
                                    {
                                        EQUIPMENTNUMBER = sse.CRCNum,
                                        TASKSTATUS = "2"
                                    });
                                    if (dt != null && dt.Rows.Count > 0)
                                    {
                                        DataRow dr = dt.Rows[0];
                                        sse.Fromstation = dr["FROMSTATION"].ToString().Trim().Replace("A", "");
                                        sse.Tostation = dr["TOSTATION"].ToString().Trim().Replace("A", "");
                                    }
                                    else if (!sse.IsMan)
                                        sse.Tostation = "";
                                }


                                #region 取货完成
                                if (sse.FunctionReport == CR_GETC_EXECUTED)
                                {
                                    #region 屏蔽穿梭板
                                    //if (!string.IsNullOrEmpty(sse.Fromstation) && sse.Fromstation.Length > 4)
                                    //{
                                    //    DataTable shuttleInfo = repository.GetAllShuttleInfo(num);
                                    //    if (shuttleInfo != null && shuttleInfo.Rows.Count > 0)
                                    //    {
                                    //        if (!repository.UpdateShuttleBoardInfo_RackToSc(num, sse.Fromstation.Substring(0, 3), sse.Fromstation.Substring(6, 3), sse.RackSide.Equals("1") ? "Left" : "Right"))
                                    //        {
                                    //            LogUtil.Write(LogFileName.ScLog, num + LogFileName.ScLog, $"接收{num}堆垛机状态报文。任务号：{sse.TaskNo},FunctionCode：{sse.FunctionReport},更新穿梭板信息失败！");
                                    //        }
                                    //    }
                                    //}
                                    #endregion 
                                    if (sse.Tostation != null && sse.TaskNo.Length > 4)
                                    {
                                        sob.CSrm.SendCode(num, CR_PUTC, sse.TaskNo, sse.Tostation, false);
                                    }
                                    else
                                    {
                                        sob.CSrm.SendCode(num, CR_NO_FUNC, sse.TaskNo, "0000000000", false);
                                        ResetScTaskInfo(sse);
                                    }
                                   
                                }
                                #endregion

                                #region 放货完成
                                if (sse.FunctionReport == CR_PUTC_EXECUTED)
                                {
                                    #region 屏蔽穿梭板
                                    //if (!string.IsNullOrEmpty(sse.Tostation) && sse.Tostation.Length > 4)
                                    //{
                                    //    //0010020011
                                    //    DataTable shuttleInfo = repository.GetAllShuttleInfo(num);
                                    //    if (shuttleInfo != null && shuttleInfo.Rows.Count > 0)
                                    //    {
                                    //        int shuttleInScCount = shuttleInfo.AsEnumerable().Count(x => x["current_status"].Equals("In_Sc"));
                                    //        //穿梭板在堆垛机上，则将位置更新到货架。否则不用更新
                                    //        if (shuttleInScCount > 0)
                                    //        {
                                    //            if (!repository.UpdateShuttleBoardInfo_ScToRack(num, sse.Tostation.Substring(0, 3), sse.Tostation.Substring(6, 3), sse.RackSide.Equals("1") ? "Left" : "Right"))
                                    //            {
                                    //                LogUtil.Write(LogFileName.ScLog, num + LogFileName.ScLog, $"接收{num}堆垛机状态报文。任务号：{sse.TaskNo},FunctionCode：{sse.FunctionReport},更新穿梭板信息失败！");
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    #endregion
                                    string goodstype = "1";
                                    if (sse.TaskNo != null && sse.TaskNo.Length > 4)
                                    {
                                        bool finish = true;
                                        if (sse.Tostation != null && sse.Fromstation.Length > 6 && sse.Tostation.Length == 4)
                                        {
                                            DataTable dt = _repository.QueryTableData(new WCS_SUBTASKINFO() 
                                            {
                                                TASKNO = Convert.ToInt32(sse.TaskNo),
                                                FROMSTATION = sse.Tostation,
                                                EQUIPMENTTYPE = "STA"
                                            });
                                            if (dt != null && dt.Rows.Count > 0)
                                            {
                                                DataRow dr = dt.Rows[0];
                                                _barcode = string.IsNullOrEmpty(dr["BARCODE"].ToString()) ? "99999999" : dr["BARCODE"].ToString();
                                                if (int.Parse(_barcode) == 99999999) goodstype = "10";

                                                string[] info = new string[9];
                                                info[0] = sse.TaskNo;
                                                info[1] = _barcode;
                                                info[2] = goodstype;
                                                info[3] = dr["FROMSTATION"].ToString();
                                                info[4] = dr["TOSTATION"].ToString();
                                                info[5] = "1";
                                                info[6] = "0";
                                                info[7] = "0";
                                                info[8] = "0";
                                                finish = plc.Write(STACommand.ReWrite, info, sse.Tostation, 0, 30);
                                            }
                                        }
                                        if (finish)
                                        {
                                            if (!string.IsNullOrEmpty(sse.Fromstation) && !string.IsNullOrEmpty(sse.Tostation) && sse.TaskNo.Length == 5)
                                            {
                                                //修改堆垛机任务状态
                                                if (!_repository.DealTaskStatus(
                                                    new WCS_SUBTASKINFO() 
                                                    {
                                                        TASKNO = Convert.ToInt32(sse.TaskNo),
                                                        EQUIPMENTTYPE = "SC",
                                                    },
                                                    new WCS_SUBTASKINFO()
                                                    {
                                                        TASKSTATUS = "1"
                                                    })
                                                )
                                                {
                                                    Thread.Sleep(100);
                                                    _repository.DealTaskStatus(
                                                    new WCS_SUBTASKINFO()
                                                    {
                                                        TASKNO = Convert.ToInt32(sse.TaskNo),
                                                        EQUIPMENTTYPE = "SC",
                                                    },
                                                    new WCS_SUBTASKINFO()
                                                    {
                                                        TASKSTATUS = "1"
                                                    });
                                                };
                                                //修改输送机任务状态
                                                if (sse.Tostation != null && sse.Tostation.Length == 4)
                                                {
                                                    Thread.Sleep(100);
                                                    _repository.DealTaskStatus(
                                                    new WCS_SUBTASKINFO()
                                                    {
                                                        TASKNO = Convert.ToInt32(sse.TaskNo),
                                                        EQUIPMENTTYPE = "STA",
                                                    },
                                                    new WCS_SUBTASKINFO()
                                                    {
                                                        RUNSTATUS = "1",
                                                        TASKSTATUS = "2"
                                                    });
                                                }
                                            }
                                            sob.CSrm.SendCode(num, CR_NO_FUNC, sse.TaskNo, "0000000000", false);
                                            ResetScTaskInfo(sse);
                                        }
                                    }
                                    else
                                    {
                                        sob.CSrm.SendCode(num, CR_NO_FUNC, sse.TaskNo, "0000000000", false);
                                        ResetScTaskInfo(sse);
                                    }
                                }
                                #endregion

                                #region 取货前，卸下穿梭板完成
                                //if (sse.FunctionReport == CR_DROPSH_GETC_EXECUTED)
                                //{
                                //    if (!string.IsNullOrEmpty(sse.Fromstation) && sse.Fromstation.Length > 4)
                                //    {
                                //        DataTable shuttleInfo = repository.GetAllShuttleInfo(num);
                                //        if (shuttleInfo != null && shuttleInfo.Rows.Count > 0)
                                //        {
                                //            int shuttleInScCount = shuttleInfo.AsEnumerable().Count(x => x["current_status"].Equals("In_Sc"));
                                //            if (shuttleInScCount > 0)
                                //            {
                                //                if (!repository.UpdateShuttleBoardInfo_ScToRack(num, sse.Fromstation.Substring(0, 3), sse.Fromstation.Substring(6, 3), sse.RackSide.Equals("1") ? "Left" : "Right"))
                                //                {
                                //                    LogUtil.Write(LogFileName.ScLog, num + LogFileName.ScLog, $"接收{num}堆垛机状态报文。任务号：{sse.TaskNo},FunctionCode：{sse.FunctionReport},更新穿梭板信息失败！");
                                //                }
                                //            }
                                //        }
                                //    }
                                //    sob.CSrm.SendCode(num, CR_NO_FUNC, sse.TaskNo, "0000000000", false);
                                //    ResetScTaskInfo(sse);
                                //}
                                #endregion

                                #region 取货前，穿梭板收回完成
                                //if (sse.FunctionReport == CR_PICKSH_GETC_EXECUTED)
                                //{
                                //    if (!string.IsNullOrEmpty(sse.Fromstation) && sse.Fromstation.Length > 4)
                                //    {
                                //        DataTable shuttleInfo = repository.GetAllShuttleInfo(num);
                                //        if (shuttleInfo != null && shuttleInfo.Rows.Count > 0)
                                //        {
                                //            if (!repository.UpdateShuttleBoardInfo_RackToSc(num, sse.Fromstation.Substring(0, 3), sse.Fromstation.Substring(6, 3), sse.RackSide.Equals("1") ? "Left" : "Right"))
                                //            {
                                //                LogUtil.Write(LogFileName.ScLog, num + LogFileName.ScLog, $"接收{num}堆垛机状态报文。任务号：{sse.TaskNo},FunctionCode：{sse.FunctionReport},更新穿梭板信息失败！");
                                //            }
                                //        }
                                //    }
                                //    sob.CSrm.SendCode(num, CR_NO_FUNC, sse.TaskNo, "0000000000", false);
                                //    ResetScTaskInfo(sse);
                                //}
                                #endregion

                                #region 放货前，卸下穿梭板完成
                                //if (sse.FunctionReport == CR_DROPSH_PUTC_EXECUTED)
                                //{
                                //    if (!string.IsNullOrEmpty(sse.Tostation) && sse.Tostation.Length > 4)
                                //    {
                                //        DataTable shuttleInfo = repository.GetAllShuttleInfo(num);
                                //        if (shuttleInfo != null && shuttleInfo.Rows.Count > 0)
                                //        {
                                //                if (!repository.UpdateShuttleBoardInfo_ScToRack(num, sse.Tostation.Substring(0, 3), sse.Tostation.Substring(6, 3), sse.RackSide.Equals("1") ? "Left" : "Right"))
                                //                {
                                //                    LogUtil.Write(LogFileName.ScLog, num + LogFileName.ScLog, $"接收{num}堆垛机状态报文。任务号：{sse.TaskNo},FunctionCode：{sse.FunctionReport},更新穿梭板信息失败！");
                                //                }
                                //        }
                                //    }
                                //    sob.CSrm.SendCode(num, CR_NO_FUNC, sse.TaskNo, "0000000000", false);
                                //    ResetScTaskInfo(sse);
                                //}
                                #endregion

                                #region 放货前，收回穿梭板完成
                                //if (sse.FunctionReport == CR_PICKSH_PUTC_EXECUTED)
                                //{
                                //    if (!string.IsNullOrEmpty(sse.Tostation) && sse.Tostation.Length > 4)
                                //    {
                                //        DataTable shuttleInfo = repository.GetAllShuttleInfo(num);
                                //        if (shuttleInfo != null && shuttleInfo.Rows.Count > 0)
                                //        {
                                //                if (!repository.UpdateShuttleBoardInfo_RackToSc(num, sse.Tostation.Substring(0, 3), sse.Tostation.Substring(6, 3), sse.RackSide.Equals("1") ? "Left" : "Right"))
                                //                {
                                //                    LogUtil.Write(LogFileName.ScLog, num + LogFileName.ScLog, $"接收{num}堆垛机状态报文。任务号：{sse.TaskNo},FunctionCode：{sse.FunctionReport},更新穿梭板信息失败！");
                                //                }
                                //        }
                                //    }
                                //    sob.CSrm.SendCode(num, CR_NO_FUNC, sse.TaskNo, "0000000000", false);
                                //    ResetScTaskInfo(sse);
                                //}
                                #endregion

                                #region 卸下穿梭板到充电位完成
                                //if (sse.FunctionReport == CR_DROPSH_PARK_EXECUTED)
                                //{
                                //    if (!string.IsNullOrEmpty(sse.Tostation) && sse.Tostation.Length > 4)
                                //    {
                                //            if (!repository.UpdateShuttleBoardInfo_ScToCs(string.Empty,num))
                                //            {
                                //                LogUtil.Write(LogFileName.ScLog, num + LogFileName.ScLog, $"接收{num}堆垛机状态报文。任务号：{sse.TaskNo},FunctionCode：{sse.FunctionReport},更新穿梭板信息失败！");
                                //            }
                                //    }
                                //    sob.CSrm.SendCode(num, CR_NO_FUNC, sse.TaskNo, "0000000000", false);
                                //    ResetScTaskInfo(sse);
                                //}
                                #endregion

                                #region 从充电位收回穿梭板完成
                                //if (sse.FunctionReport == CR_PICKSH_PARK_EXECUTED)
                                //{
                                //    if (!string.IsNullOrEmpty(sse.Fromstation) && sse.Fromstation.Length > 4)
                                //    {
                                //        if (!repository.UpdateShuttleBoardInfo_CsToSc(string.Empty,num))
                                //        {
                                //            LogUtil.Write(LogFileName.ScLog, num + LogFileName.ScLog, $"接收{num}堆垛机状态报文。任务号：{sse.TaskNo},FunctionCode：{sse.FunctionReport},更新穿梭板信息失败！");
                                //        }
                                //    }
                                //    sob.CSrm.SendCode(num, CR_NO_FUNC, sse.TaskNo, "0000000000", false);
                                //    ResetScTaskInfo(sse);
                                //}
                                #endregion

                                if (sse.FunctionReport == CR_POS_GET_EXECUTED)  //行走完成
                                {
                                    sob.CSrm.SendCode(num, CR_NO_FUNC, sse.TaskNo, "0000000000", false);
                                    ResetScTaskInfo(sse);
                                }
                            }
                            else if (sob == null)
                            {
                                SRMObject status = new SRMObject();
                                status.SrmNo = num;
                                status.aislenumber = int.Parse(num.Substring(num.Length - 1, 1));
                                lstSRM.Add(status);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //异常时不改变地址状态
                    if (sse != null && sse.FunctionReport != null && !sse.FunctionReport.Contains("CR_NO_FUNC"))
                    {
                        sse.Fromstation = fromStationTemp;
                        sse.Tostation = toStationTemp;
                    }
                    LogUtil.Write(LogFileName.ScExceptionLog, LogFileName.ScExceptionLog, $"接收堆垛机状态报文时出现异常。{ex.Message}{ex.StackTrace}");
                }
            }
        }

        private void ResetScTaskInfo(SRM_Status_Entity sse)
        {
            sse.Fromstation = string.Empty;
            sse.Tostation = string.Empty;
            sse.TaskType = SCTaskType.None;
        }

       


        public int TransStrToInt(string strH, string strL)
        {
            return int.Parse(strH) * 256 + int.Parse(strL);
        }

        //p1巷道号    P2是rackside  p3是列值 p4是1 P5是高度层值  p6是1深度
        private string GetLocationNew(string num, int p1, int p2, int p3, int p4, int p5, int p6)
        {
            string str = string.Empty;
            p6 = 1;
            int row = (p1 - 1) * 2 + (p2 == 0 ? 1 : p2);
            if (p3 == 255)
                p3 = 1;
            str += "S" + row.ToString().PadLeft(3, '0');
            str += ((p3 - 1) * 3 + p4).ToString().PadLeft(3, '0');
            str += p5.ToString().PadLeft(3, '0') + p6.ToString();//+ p6.ToString();
            return str;
        }

        //堆垛机当前位置
        private string GetCurrentPosition(string aisleNo,string rackSide,string line,string depth,string layer)
        {
            if (aisleNo.Equals("1"))
            {
                return $"{line.PadLeft(3,'0')}{depth.PadLeft(3,'0')}{layer.PadLeft(3,'0')}";
            }
            if (rackSide.Equals("1"))
            {
                return $"{line.PadLeft(3, '0')}{(18 - Convert.ToInt32(depth)).ToString().PadLeft(3,'0')}{layer.PadLeft(3, '0')}";
            }
            return $"{line.PadLeft(3, '0')}{(17 + Convert.ToInt32(depth)).ToString().PadLeft(3, '0')}{layer.PadLeft(3, '0')}";
        }

        public string GetAlarmInfo1(string num, string alarm, bool update)
        {
            SRM_Status_Entity css = L_SRMStatus.Find(
                delegate (SRM_Status_Entity info)
                {
                    return info.CRCNum == num;
                }
            );
            if (css == null)
                return "";
            if (update)
                css.AlarmInfo = alarm;
            else
            {
                string stralarm = "", changealarm = "";
                int[] crcmodestatus = new int[8];
                int[] crcplcinfo = new int[11];
                if (!System.String.IsNullOrEmpty(css.AlarmInfo))
                {
                    string[] msg = css.AlarmInfo.Split(new char[] { ';' });
                    for (int i = 0; i < msg.Length; i++)
                    {
                        if (msg[i] != "")
                        {
                            string index = msg[i].Substring(0, msg[i].IndexOf(':'));
                            string ala = msg[i].Substring(msg[i].IndexOf(':') + 1, msg[i].Length - msg[i].IndexOf(':') - 1);
                            int[] alarmbit = FunctionMode(int.Parse(ala));
                            for (int j = 0; j < alarmbit.Length; j++)
                            {
                                if (alarmbit[j] == 1)
                                {
                                    foreach (DictionaryEntry de in AlarmHash)
                                    {
                                        string w = "C" + index + "." + j.ToString();
                                        if (de.Key.ToString() == "C" + index + "." + j.ToString())
                                        {
                                            stralarm += de.Value.ToString() + ";";
                                            changealarm += index + "." + j.ToString() + ";";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    for (int k = 0; k < (stralarm.Length - 1) / 15; k++)
                    {
                        stralarm = stralarm.Insert((k + 1) * 15, ".");//stralarm.Substring(0, (k + 1) * 15 + k) + "." + stralarm.Substring((k + 1) * 15 + k + 1, stralarm.Length - 1);
                    }
                    return stralarm;
                }
            }
            return "";
        }


        public string GetAlarmInfo(string num, string alarm, bool update)
        {
            SRM_Status_Entity css = L_SRMStatus.Find(
                delegate (SRM_Status_Entity info)
                {
                    return info.CRCNum == num;
                }
            );
            if (css == null)
                return "";
            css.AlarmInfo = alarm;
            string stralarm = "", changealarm = "";
            int[] crcmodestatus = new int[8];
            int[] crcplcinfo = new int[11];
            if (!System.String.IsNullOrEmpty(css.AlarmInfo))
            {
                string[] msg = css.AlarmInfo.Split(new char[] { ';' });
                for (int i = 0; i < msg.Length; i++)
                {
                    if (msg[i] != "")
                    {
                        string index = msg[i].Substring(0, msg[i].IndexOf(':'));
                        string ala = msg[i].Substring(msg[i].IndexOf(':') + 1, msg[i].Length - msg[i].IndexOf(':') - 1);
                        int[] alarmbit = FunctionMode(int.Parse(ala));
                        for (int j = 0; j < alarmbit.Length; j++)
                        {
                            if (alarmbit[j] == 1)
                            {
                                foreach (DictionaryEntry de in AlarmHash)
                                {
                                    string w = "C" + index + "." + j.ToString();
                                    if (de.Key.ToString() == "C" + index + "." + j.ToString())
                                    {
                                        stralarm += de.Value.ToString() + ";";
                                        changealarm += index + "." + j.ToString() + ";";
                                    }
                                }
                            }
                        }
                    }
                }
                //for (int k = 0; k < (stralarm.Length - 1) / 15; k++)
                //{
                //    stralarm = stralarm.Insert((k + 1) * 15, ".");//stralarm.Substring(0, (k + 1) * 15 + k) + "." + stralarm.Substring((k + 1) * 15 + k + 1, stralarm.Length - 1);
                //}
                return stralarm;
            }
            return "";
        }

        public bool UpdateCRCTaskInfo(string num, string fromstation, string tostation, string tasktype, bool isMan)
        {
            SRM_Status_Entity sse = L_SRMStatus.Find(x => x.CRCNum.Equals(num));
            if (sse != null)
            {
                sse.Fromstation = !string.IsNullOrEmpty(fromstation) ? fromstation.Replace("A", "") : string.Empty;
                sse.Tostation = !string.IsNullOrEmpty(tostation) ? tostation.Replace("A", "") : string.Empty;
                sse.IsMan = isMan;
                sse.TaskType = ConvertTaskType(tasktype);
                return true;
            }
            return false;
        }
        //p1巷道号 P2是rackside p3是列值 p4是1（货格）P5是高度层值 p6是1深度
        private string GetLocation(string num, int p1, int p2, int p3, int p4, int p5, int p6)
        {
            if (p3 == 255 || p3 == 254) p3 = 1;
            string str = string.Format("{0}巷道{1}列{2}层", p1, p3 == 1 ? p4 : p3 * 2 - 3 + p4, p5);
            return str;
        }
        public int[] FunctionMode(int status)
        {
            string errorsobde = Convert.ToString(status, 2);//10001,(17,2)
            Int32 length = errorsobde.Length;
            int[] Errorsobde = new int[8];
            for (int i = length - 1; i >= 0; i--)
            {
                Errorsobde[length - i - 1] = int.Parse(errorsobde.Substring(i, 1).ToString());
            }
            return Errorsobde;
        }
        public string GetFunctionMode(int[] functionmode, int[] cranemode)
        {
            string mode = (functionmode[0] == 1 ? "自动;" : "");
            mode += (functionmode[1] == 1 ? "半自动;" : "");
            mode += (functionmode[2] == 1 ? "手动;" : "");
            mode += (functionmode[3] == 1 ? "软停;" : "");
            mode += (functionmode[4] == 1 ? "就绪;" : "");
            mode += (functionmode[7] == 1 ? "故障;" : "正常;");
            if (cranemode[0] == 1)
                mode += "远程;";
            if (cranemode[1] == 1)
                mode += "本地;";
            if (cranemode[7] == 1)
                mode += "报警;";
            return mode;
        }

        public string GetTaskMode(SCTaskMode taskmode)
        {
            string mode = "";
            switch (taskmode)
            {
                case SCTaskMode.All: mode = "可出可入"; break;
                case SCTaskMode.OnlyInFeed: mode = "只入不出"; break;
                case SCTaskMode.OnlyOutFeed: mode = "只出不入"; break;
                case SCTaskMode.Pause: mode = "暂停使用"; break;
            }
            return mode;
        }

        private int GetDistance(string fromstation, string tostation, string equipmentnumber)
        {

            SRM_Status_Entity sse = L_SRMStatus.Find
            (
                delegate (SRM_Status_Entity info)
                {
                    return info.CRCNum == equipmentnumber;
                }
            );
            if (sse != null && sse.RackPosition != null)
            {
                if (fromstation.Length == 4)
                    return int.Parse(sse.RackPosition);//XF列
                else
                    //return Math.Abs(int.Parse(sse.RackPosition) - (int.Parse(fromstation.Substring(3, 3)) - 100));  ////列值-0520YANG
                    return Math.Abs(int.Parse(sse.RackPosition) - (int.Parse(fromstation.Substring(2, 2)) - 100));  ////列值-
            }
            return -1;
        }

        public DataTable GetTask()
        {
            //更新堆垛机状态,获取任务
            DataTable taskTable = _repository.GetNoExecuteSCTask(); 
            DataRow[] taskRows = null;
            string[] stationInfo = new string[9];
            bool isExistSmallerTask = false;
            if (taskTable != null && taskTable.Rows.Count > 0)
            {
                taskRows = taskTable.AsEnumerable().ToArray();
                foreach (DataRow dr in taskRows)
                {
                    try
                    {
                        //入库
                        if (dr["fromstation"].ToString().Length == 4 &&  dr["tostation"].ToString().Length == 8)
                        {
                            stationInfo = plc.ReadPLCInfo(dr["fromstation"].ToString());
                            if (!stationInfo[0].Equals(dr["taskno"].ToString()))
                            {
                                taskRows = taskRows.Where(x => !x["taskno"].ToString().Equals(dr["taskno"].ToString())).ToArray();
                                _repository.AddLog($"入库失败！堆垛机入库任务,WCS任务号:{dr["taskno"]},目标货位:{dr["tostation"]}.输送机站台上的任务号为:{stationInfo[0]},任务号不匹配,请人工处理！");
                            }
                            //排除非最小任务
                            isExistSmallerTask = _repository.IsExistSmallerTask(dr["tostation"].ToString()); 
                            if (isExistSmallerTask)
                            {
                                taskRows = taskRows.Where(x => !x["taskno"].ToString().Equals(dr["taskno"].ToString())).ToArray();
                                _repository.AddLog($"入库失败！堆垛机入库任务,WCS任务号:{dr["taskno"]},目标货位:{dr["tostation"]},不是最小的货位,暂停入库.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtil.Write(LogFileName.ScExceptionLog, LogFileName.ScExceptionLog,$"{ex.Message}{ex.StackTrace}");
                    }
                }
            }
            //排除出库任务
            if (taskRows != null && taskRows.Length > 0)
            {
                DataTable toStationDt = _repository.GetOutBoundTargetAddressForSC();
                if (toStationDt != null && toStationDt.Rows.Count > 0)
                {
                    foreach (DataRow dr in toStationDt.Rows)
                    {
                            stationInfo = plc.ReadPLCInfo(dr["tostation"].ToString());
                            if (Convert.ToInt32(stationInfo[0]) > 0)
                            {
                                taskRows = taskRows.Where(x => !x["tostation"].ToString().Equals(dr["tostation"].ToString())).ToArray();
                            }
                    }
                }
            }
            return (taskRows != null && taskRows.Length > 0) ? taskRows.CopyToDataTable() : null;
        }

        /// <summary>
        /// 任务类型转换
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="SCStatus"></param>
        /// <returns></returns>
        private SCTaskType ConvertTaskType(string TaskType)
        {
            SCTaskType strStatus = SCTaskType.None;
            switch (TaskType)
            {
                case "Task_InBound": strStatus = SCTaskType.Task_InBound; break;
                case "Task_OutBound": strStatus = SCTaskType.Task_OutBound; break;
                case "Task_Transfer": strStatus = SCTaskType.Task_Transfer; break;
                case "Task_QCOutBound": strStatus = SCTaskType.Task_OutBound; break;
                case "Task_QCInBound": strStatus = SCTaskType.Task_InBound; break;
                case "Task_CountInBound": strStatus = SCTaskType.Task_InBound; break;
                case "Task_CountOutBound": strStatus = SCTaskType.Task_OutBound; break;
                case "Task_CountAuto": strStatus = SCTaskType.Task_CountAuto; break;
                case "Task_DropShuttle": strStatus = SCTaskType.Task_DropShuttle;break;
                case "Task_PickShuttle": strStatus = SCTaskType.Task_PickShuttle; break;
                default: strStatus = SCTaskType.None; break;
            }
            return strStatus;
        }

        public string GetActionStatusBase(string code)
        {
            switch (code)
            {
                case "0": return "CR_NO_FUNC_RUNNING";//空闲
                case "16": return "CR_POS_GET_WAIT_H";//等待取货
                case "17": return "CR_POS_GET_RUNNING";//取货运行中
                case "25": return "CR_POS_GET_EXECUTED";//取货完成
                case "26": return "CR_POS_GET_FAILED";//取货失败
                case "27": return "CR_POS_GET_ABORTED";//取货中断
                case "32": return "CR_GET_WAIT_H";
                case "33": return "CR_GET_RUNNING";
                case "41": return "CR_GET_EXECUTED";
                case "42": return "CR_GET_FAILED";
                case "43": return "CR_GET_ABORTED";
                case "44": return "CR_GET_MF_FAULT";
                case "45": return "CR_GET_LOC_EMPTY";
                case "46": return "CR_GET_PNOTREACH";
                case "48": return "CR_POS_PUT_WAIT_H";
                case "49": return "CR_POS_PUT_RUNNING";
                case "57": return "CR_POS_PUT_EXECUTED";
                case "58": return "CR_POS_PUT_FAILED";
                case "59": return "CR_POS_PUT_ABORTED";
                case "64": return "CR_PUT_WAIT_H";
                case "65": return "CR_PUT_RUNNING";
                case "73": return "CR_PUT_EXECUTED";
                case "74": return "CR_PUT_FAILED";
                case "75": return "CR_PUT_ABORTED";
                case "76": return "CR_PUT_MF_FAULT";
                case "77": return "CR_PUT_LOC_FULL";
                case "78": return "CR_PUT_PNOTREACH";
                case "80": return "CR_POS_PICK_WAIT_H";
                case "81": return "CR_POS_PICK_RUNNING";
                case "89": return "CR_POS_PICK_EXECUTED";
                case "90": return "CR_POS_PICK_FAILED";
                case "91": return "CR_POS_PICK_ABORTED";
                case "97": return "盘库行走中";//盘库中
                case "105": return "到达货位";//盘库中
                case "112": return "CR_GETC_WAIT_H";//等待取货
                case "113": return "CR_GETC_POS_RUNNING";//取货运行中
                case "114": return "CR_GETC_GET_RUNNING";//取货伸叉
                case "121": return "CR_GETC_EXECUTED";//取货完成
                case "122": return "CR_GETC_FAILED";//取货失败
                case "123": return "CR_GETC_ABORTED";//取货中止
                case "124": return "CR_GETC_MF_FAULT";//
                case "125": return "CR_GETC_LOC_EMPTY";
                case "126": return "CR_GETC_PNOTREACH";
                case "128": return "CR_PUTC_WAIT_H";
                case "129": return "CR_PUTC_POS_RUNNING";
                case "130": return "CR_PUTC_PUT_RUNNING";
                case "137": return "CR_PUTC_EXECUTED";
                case "138": return "CR_PUTC_FAILED";
                case "139": return "CR_PUTC_ABORTED";
                case "140": return "CR_PUTC_MF_FAULT";
                case "141": return "CR_PUTC_LOC_FULL";
                case "142": return "CR_PUTC_PNOTREACH";

                case "377": return "CR_DROPSH_GETC_EXECUTED";
                case "633": return "CR_PICKSH_GETC_EXECUTED";
                case "393": return "CR_DROPSH_PUTC_EXECUTED";
                case "649": return "CR_PICKSH_PUTC_EXECUTED";
                case "409": return "CR_DROPSH_PARK_EXECUTED";
                case "665": return "CR_PICKSH_PARK_EXECUTED";
                default: return "";
            }
        }
    }
}
