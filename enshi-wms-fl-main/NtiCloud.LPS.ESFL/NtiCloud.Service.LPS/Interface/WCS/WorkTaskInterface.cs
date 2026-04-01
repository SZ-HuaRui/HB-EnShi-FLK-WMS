using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.Core;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Tasking;
using NtiCloud.Services.WMS.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NtiCloud.Service.WMS.Interface.WCS
{
    public class WorkTaskInterface : WmsObject, IWorkTaskInterface
    {
        //public WebJsonInfo NotifyWaveFinished(string waveNo)
        //{
        //    throw new NotImplementedException();
        //}
        public WebJsonInfo NotifyWaveFinished(string waveNo)
        {
            return UpdateWaveStatus(waveNo, "2");
        }

        public WebJsonInfo NotifyWavePause(string waveNo)
        {
            return UpdateWaveStatus(waveNo, "3");
        }
        /// <summary>
        ///  更新波次状态
        /// </summary>
        /// <param name="waveNo"></param>
        /// <param name="status">波次状态 ：1 波次开始； 2波次结束； 3波次暂停</param>
        /// <returns></returns>
        public WebJsonInfo UpdateWaveStatus(string waveNo, string status)
        {
            WebJsonInfo info = new WebJsonInfo();

            string sendToWcs = WmsConfig.GetOrDefault("Task:SendToWCS", "0");
            if (sendToWcs != "1")
            {
                return info;
            }

            WaveInfo taskInfo = new WaveInfo()
            {
                Msg_Mark = "WMS->WCS",
                Msg_Time = DateTime.Now.ToString(),
                Msg_User = "WMS",
                Msg_UserPwd = "admin",
                Msg_Method = "波次出库",
            };
            taskInfo.Msg_Id = WmsHelper.GetSequence("SWMS_MessageID");
            taskInfo.Msg_Params = new List<WaveInfo.Msg_Paramslist>()
            {
                new WaveInfo.Msg_Paramslist()
                {
                    WarehouseID = Context.WarehouseID,
                    ZoneID = string.Empty,
                    Wave_ID = waveNo,
                    //波次状态 ：1 波次开始； 2波次结束； 3波次暂停
                    Wave_Status = status,
                }
            };

            LogHelper.LogDebug("发送任务到WCS(WaveStatus)");
            LogHelper.LogDebug(JsonConvert.SerializeObject(taskInfo));
            //var service = ServiceLocator.GetService<IWcsPassiveService>();
            //string result = service.WaveStatus(JsonConvert.SerializeObject(taskInfo)).Result;
            var service = Context.ServiceProvider.GetService<IPostGetDataService>();
            var url = WmsConfig.GetOrDefault("Interface:IPSAddress", "localhost");
            var model = new PostGetDataInput.PostResponseDataModel()
            {
                url = url + "/api/WmsWorkFlow/WaveStatus",
                postData = JsonConvert.SerializeObject(taskInfo),
                type = "json",
            };
            UserLogModel log = new UserLogModel()
            {
                LogText = $"更新波次{waveNo}状态为{status}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(taskInfo),
                TypeCode = "更新波次状态",
                CommonName = "/api/WmsWorkFlow/WaveStatus",
            };
            try
            {
                WebJsonInfo result = service.PostResponseData(model);
                info = ResolveJsonResultFromWcs(result.json.ToString(), out JObject regMsg);
                /*
                var je = JsonDocument.Parse(result.json.ToString()).RootElement;
                var errCode = je.GetProperty("errcode").GetInt32();
                if (errCode != 0)
                {
                    retInfo.errcode = errCode;
                    retInfo.errmsg = je.GetProperty("errmsg").ToString();
                    retInfo.isSucceed = false;
                }
                return retInfo;
                */
                if (info.errcode == 0)
                {
                    log.LogText += ", 执行成功";
                    log.LogTextLong += ", 返回信息：成功";
                    info.isSucceed = true;
                }
                else
                {
                    log.LogText += ", 执行失败";
                    log.LogTextLong += $", 返回信息：[{info.errcode}]{info.errmsg}";
                    info.isSucceed = false;
                }
            }
            catch (Exception ex)
            {
                log.LogText += ", 执行失败";
                log.LogTextLong += $", 返回信息：{ex.Message}";

                info.errcode = 500;
                info.errmsg = $"更新波次状态:{ex.Message}";
                info.isSucceed = false;
            }
            LogHelper.LogInformation(JsonConvert.SerializeObject(log));
            UserLog.Add(log);

            //info = Util.resolveJsonResultFromWcs(result, out JObject retMsg);
            //if (info.errcode != 0)
            //{
            //    UserLog.InsertInterfaceLog("通知WCS更新波次状态", info.errmsg,
            //        $"WaveStatus('{waveNo}')", "", "", "", "", "");

            //}

            return info;
        }
        public static WebJsonInfo ResolveJsonResultFromWcs(string jsonResult, out JObject retMsg)
        {
            WebJsonInfo retInfo = new WebJsonInfo();
            retMsg = null;

            LogHelper.LogDebug(jsonResult);

            retInfo = JsonConvert.DeserializeObject<WebJsonInfo>(jsonResult.ToString());
            JObject inputParameters = null;
            try
            {
                inputParameters = JObject.Parse(jsonResult);
            }
            catch (Exception ex)
            {
                string errMsg = "WCS返回的结果有错：" + jsonResult;
                LogHelper.LogError(errMsg);
                LogHelper.LogError(jsonResult);
                retInfo.errcode = -101;
                retInfo.errmsg = errMsg;
                return retInfo;
            }
            retMsg = inputParameters;
            /*
            string state = inputParameters["State_Code"].ToString();
            retInfo.json = retMsg;
            retInfo.errcode = int.Parse(state);
            retInfo.errmsg = "WCS返回错误：" + inputParameters["State_Desc"].ToString();
            */
            return retInfo;
        }

        /// <summary>
        /// 回收WCS任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns>回收成功返回"",失败返回错误信息</returns>
        public string Recycle(string taskId)
        {
            if (WmsConfig.GetOrDefault("Task:SendToWCS", "0") == "0")
            {
                return "";
            }

            string postData = new WorkTask().BuildRecycleWcsTaskBody(taskId);
            LogHelper.LogDebug("回收WCS任务");
            LogHelper.LogDebug(JsonConvert.SerializeObject(postData));
            var service = Context.ServiceProvider.GetService<IPostGetDataService>();
            var url = WmsConfig.GetOrDefault("Interface:IPSAddress", "localhost");
            var model = new PostGetDataInput.PostResponseDataModel()
            {
                url = url + "/api/tyjk/Task/RollbackTask",
                postData = postData,
                type = "json",
            };
            WebJsonInfo result = service.PostResponseData(model);
            try
            {
                var je = JsonDocument.Parse(result.json.ToString()).RootElement;
                //var errCode = je.GetProperty("errcode").GetInt32();
                var wcsjson = je.GetProperty("json");
                string wcsjsonStr = wcsjson.ToString();
                Newtonsoft.Json.Linq.JArray jArray = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(wcsjsonStr);
                if (jArray.Count == 0)
                {
                    throw new Exception("wcs回收任务失败:" + wcsjsonStr);
                }
                Newtonsoft.Json.Linq.JObject jdata = (Newtonsoft.Json.Linq.JObject)jArray[0];
                if (jdata["Code"].ToString() == "0" || jdata["Code"].ToString() == "1")
                {
                    return "";
                }
                else
                {
                    return jdata["ErrMsg"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public WebJsonInfo Recycle(DataRow[] taskRows)
        {
            WebJsonInfo info = new WebJsonInfo();

            var sendToWcs = WmsConfig.GetOrDefault("Task:SendToWCS", "0");
            if (sendToWcs != "1")
            {
                return info;
            }

            RollBackTaskInfo taskInfo = new RollBackTaskInfo()
            {
                Msg_Mark = "WMS->WCS",
                Msg_Time = DateTime.Now.ToString(),
                Msg_User = "WMS",
                Msg_UserPwd = "admin",
                Msg_Method = "回收任务",
            };
            taskInfo.Msg_Id = WmsHelper.GetSequence("SWMS_MessageID");
            taskInfo.Msg_Params = new List<RollBackTaskInfo.Msg_Paramslist>();

            //info = new PYWorkTask().GetWorkTask(string.Join(",", ids), out DataRow[] taskRows);
            //if (info.errcode != 0) return info;

            foreach (DataRow row in taskRows)
            {
                RollBackTaskInfo.Msg_Paramslist p = new RollBackTaskInfo.Msg_Paramslist()
                {
                    WarehouseID = row["warehouse_id"].ToString(),
                    ZoneID = row["zone_id"].ToString(),
                    TaskNum = row["task_number"].ToString(),
                    Wave_ID = row["source_bill_h_no"].ToString(),
                    Mark = "1",
                };
                taskInfo.Msg_Params.Add(p);
            }

            LogHelper.LogDebug("向WCS回收任务(RollBackTask)");
            LogHelper.LogDebug(JsonConvert.SerializeObject(taskInfo));

            var service = Context.ServiceProvider.GetService<IPostGetDataService>();
            var url = WmsConfig.GetOrDefault("Interface:IPSAddress", "localhost");
            var model = new PostGetDataInput.PostResponseDataModel()
            {
                url = url + "/api/tyjk/Task/RollbackTask",
                postData = JsonConvert.SerializeObject(taskInfo),
                type = "json",
            };
            UserLogModel log = new UserLogModel()
            {
                LogText = $"向WCS回收任务",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(taskInfo),
                TypeCode = "向WCS回收任务",
                CommonName = model.url,
            };
            JObject retMsg = null;
            try
            {
                WebJsonInfo result = service.PostResponseData(model);
                info = WmsHelper.ResolveJsonResultFromWcs(result.json.ToString());
                if (info.errcode == 0)
                {
                    log.LogText += ", 执行成功";
                    log.LogTextLong += ", 返回信息：成功";
                }
                else
                {
                    log.LogText += ", 执行失败";
                    log.LogTextLong += $", 返回信息：[{info.errcode}]{info.errmsg}";
                }
            }
            catch (Exception ex)
            {
                log.LogText += ", 执行失败";
                log.LogTextLong += $", 返回信息：{ex.Message}";

                info.errcode = 500;
                info.errmsg = $"从WCS回收任务发生错误:{ex.Message}";
                info.isSucceed = false;
            }
            LogHelper.LogInformation(JsonConvert.SerializeObject(log));
            //if (result.Contains("错误"))
            //{
            //    info.errcode = -701;
            //    info.errmsg = result;
            //    return info;
            //}

            //JObject inputParameters = null;
            //try
            //{
            //    inputParameters = JObject.Parse(result);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("错误的返回结果", ex);
            //}
            //string state = inputParameters["State_Code"].ToString();
            //if (state != "000" && state != "999")
            //{
            //    return new WebJsonInfo(-666, inputParameters["State_Desc"].ToString());
            //}
            UserLog.Add(log);
            if (info.errcode == 0)
            {
                List<string> taskNoList = new List<string>();
                //for (int i = 0; i < retMsg["Data"].)

                IEnumerable<JToken> taskNos = retMsg.SelectTokens("$.Data[?(1)].TaskNum");
                foreach (JToken item in taskNos)
                {
                    taskNoList.Add(item.ToString());
                }

                //info.Result = retMsg["Data"][0]["TaskNum"].ToString();
                info.json = string.Join(";", taskNoList.ToArray());
            }

            return info;
        }


        public WebJsonInfo Recycle(List<string> ids)
        {
            WebJsonInfo info = new WebJsonInfo();

            //if (WmsConfig.Get("Task", "SendToWCS") != "1")
            //{
            //    return info;
            //}

            //RollBackTaskInfo taskInfo = new RollBackTaskInfo()
            //{
            //    Msg_Mark = "WMS->WCS",
            //    Msg_Time = DateTime.Now.ToString(),
            //    Msg_User = "WMS",
            //    Msg_UserPwd = "admin",
            //    Msg_Method = "波次出库",
            //};
            //info = Util.GetSequence("SWMS_MessageID");
            //if (info.errcode != 0) return info;

            //taskInfo.Msg_Id = info.Result;
            //taskInfo.Msg_Params = new List<RollBackTaskInfo.Msg_Paramslist>();

            DataRow[] taskRows = new WorkTask().GetWorkTasks(string.Join(",", ids));

            info = Recycle(taskRows);
            if (info.errcode != 0) return info;

            // foreach (DataRow row in taskRows)
            // {
            //     RollBackTaskInfo.Msg_Paramslist p = new RollBackTaskInfo.Msg_Paramslist()
            //     {
            //         WarehouseID = row["warehouse_id"].ToString(),
            //         ZoneID = row["zone_id"].ToString(),
            //         TaskNum = row["task_number"].ToString(),
            //         Wave_ID = row["source_bill_h_no"].ToString(),
            //         Mark = "1",
            //     };
            //     taskInfo.Msg_Params.Add(p);
            // }

            //var service = ServiceLocator.GetService<IWcsPassiveService>();
            // string result = service.RollBackTask(JsonConvert.SerializeObject(taskInfo)).Result;
            // /*
            // if (result.Contains("错误"))
            // {
            //     info.errcode = -701;
            //     info.errmsg = result;
            //     return info;
            // }

            // JObject inputParameters = null;
            // try
            // {
            //     inputParameters = JObject.Parse(result);
            // }
            // catch (Exception ex)
            // {
            //     throw new Exception("错误的返回结果", ex);
            // }
            // string state = inputParameters["State_Code"].ToString();
            // if (state != "000" && state != "999")
            // {
            //     return new WebJsonInfo(-666, inputParameters["State_Desc"].ToString());
            // }
            // */
            // info = Util.resolveJsonResultFromWcs(result, out JObject retMsg);
            // if (info.errcode != 0)
            // {
            //     Util.InsertInterfaceLog("从WCS回收任务", info.errmsg, "RollBackTask",
            //         taskRows[0]["task_number"].ToString(),
            //         taskRows[0]["task_type_code"].ToString(),
            //         taskRows[0]["container_barcode"].ToString(),
            //         taskRows[0]["source_text"].ToString(),
            //         taskRows[0]["target_text"].ToString());
            // }
            // else
            // {
            //     List<string> taskNoList = new List<string>();
            //     //for (int i = 0; i < retMsg["Data"].)

            //     IEnumerable<JToken> taskNos = retMsg.SelectTokens("$.Data[?(1)].TaskNum");
            //     foreach (JToken item in taskNos)
            //     {
            //         taskNoList.Add(item.ToString());
            //     }

            //     //info.Result = retMsg["Data"][0]["TaskNum"].ToString();
            //     info.Result = string.Join(";", taskNoList.ToArray());
            // }

            return info;
        }

        
        //报文样例
        //{
        //     "Msg_Id": "M-20200807-00007",
        //     "Msg_Mark": "WMS-\u003EWCS",
        //     "Msg_Time": "2020-08-07 15:41:48",
        //     "Msg_User": "WCS",
        //     "Msg_UserPwd": "admin",
        //     "Msg_Method": "inbound",
        //     "Msg_Params": {
        //         "WarehouseID": "57274b9c55464753b8bd7a0df96421f2",
        //         "EquipmentNo": "1081"
        //     }
        // }
        /// <summary>
        /// 每次波次计算前调用获取批次的接口，获取开包线允许生产的批次号，
        /// 然后和出库订单头表的生产批次号进行比较，如果一致则允许波次计算，否则不允许波次计算
        /// 错误信息。
        /// </summary>
        /// <param name="ids">任务ID或任务号，如有多个，用英文逗号隔开</param>
        /// <returns>当报文发送失败或者返回的结果无法解析时，错误信息包含在返回结果中</returns>
        public WebJsonInfo GetBatchNo(string warehouseID, string destAddrNo, string batchNo)
        {
            WebJsonInfo retInfo = new WebJsonInfo();
            if (WmsConfig.GetOrDefault("Outbound:CheckBatchNoWhenComputingWave", "0") == "0")
            {
                retInfo.json = batchNo;
                return retInfo;
            }
            if (WmsConfig.GetOrDefault("Task:SendToWCS", "0") == "0")
            {
                retInfo.json = batchNo;
                return retInfo;
            }

            TaskInfo taskInfo = new TaskInfo()
            {
                Msg_Id = WmsHelper.GetSequence("WMS_MessageID"),
                Msg_Mark = "WMS->WCS",
                Msg_Time = DateTime.Now.ToString(),
                Msg_User = "WMS",
                Msg_UserPwd = "admin",
                Msg_Method = "GetBatchNo",
            };

            taskInfo.Msg_Params = new List<TaskInfo.Msg_Paramslist>();

            TaskInfo.Msg_Paramslist p = new TaskInfo.Msg_Paramslist()
            {
                WarehouseID = warehouseID,
                EquipmentNum = destAddrNo,
            };
            taskInfo.Msg_Params.Add(p);

            var newTaskInfo = new
            {
                Msg_Params = new
                {
                    WarehouseID = warehouseID,
                    EquipmentNo = destAddrNo,
                }
            };
            LogHelper.LogInformation("向WCS查询批次号");
            //LogHelper.LogInformation(JsonConvert.SerializeObject(taskInfo));
            LogHelper.LogInformation(JsonConvert.SerializeObject(newTaskInfo));
            var service = Context.ServiceProvider.GetService<IPostGetDataService>();
            var url = WmsConfig.GetOrDefault("Interface:IPSAddress", "localhost");
            var model = new PostGetDataInput.PostResponseDataModel()
            {
                url = url + "/api/tyjk/Task/GetBatchNo",
                //postData = JsonConvert.SerializeObject(taskInfo),
                postData = JsonConvert.SerializeObject(newTaskInfo),
                type = "json",
            };
            UserLogModel log = new UserLogModel()
            {
                LogText = $"向WCS查询批次号{destAddrNo}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(taskInfo),
                TypeCode = "向WCS查询批次号",
                CommonName = model.url,
            };
            try
            {
                WebJsonInfo result = service.PostResponseData(model);
                retInfo = WmsHelper.ResolveJsonResultFromWcs(result.json.ToString());
                if (retInfo.errcode == 0)
                {
                    log.LogText += ", 执行成功";
                    log.LogTextLong += ", 返回信息：成功";
                }
                else
                {
                    log.LogText += ", 执行失败";
                    log.LogTextLong += $", 返回信息：[{retInfo.errcode}]{retInfo.errmsg}";
                }
            }
            catch (Exception ex)
            {
                log.LogText += ", 执行失败";
                log.LogTextLong += $", 返回信息：{ex.Message}";

                retInfo.errcode = 500;
                retInfo.errmsg = $"向WCS查询批次号:{ex.Message}";
                retInfo.isSucceed = false;
            }
            LogHelper.LogInformation(JsonConvert.SerializeObject(log));
            UserLog.Add(log);

            return retInfo;
        }

        /// <summary>
        /// 下发任务到WCS, 若报文发送失败或者返回的结果无法解析，本程序不会抛出异常，只会返回
        /// 错误信息。
        /// </summary>
        /// <param name="ids">任务ID或任务号，如有多个，用英文逗号隔开</param>
        /// <returns>当报文发送失败或者返回的结果无法解析时，错误信息包含在返回结果中</returns>
        public WebJsonInfo Send(List<string> ids)
        {
            WebJsonInfo retInfo = new WebJsonInfo();
            if (WmsConfig.GetOrDefault("Task:SendToWCS", "0") == "0")
            {
                return retInfo;
            }

            TaskInfo taskInfo = new TaskInfo()
            {
                Msg_Id = WmsHelper.GetSequence("WMS_MessageID"),
                Msg_Mark = "WMS->WCS",
                Msg_Time = DateTime.Now.ToString(),
                Msg_User = "WMS",
                Msg_UserPwd = "admin",
                Msg_Method = "下发任务",
            };

            taskInfo.Msg_Params = new List<TaskInfo.Msg_Paramslist>();

            DataRow[] taskRows = WorkTask.GetWorkTasks(string.Join(",", ids));
            foreach (DataRow row in taskRows)
            {
                if (row[t.ExecutorType].ToString() != TaskExecutor.TaskExecutor_WCS.ToString())
                {
                    continue;
                }
                DataRow equipRow = null;
                if (!string.IsNullOrWhiteSpace(row[t.EquipmentID].ToString()))
                {
                    equipRow = Equipment.GetEquipment(row[t.EquipmentID].ToString());
                }
                DataRow[] taskItemRows = WorkTaskItem.GetByTaskID(row[t.ID].ToString());
                DataRow itemRow = null;
                if (taskItemRows.Count() > 0)
                {
                    itemRow = Item.Get(taskItemRows.First()[ti.ItemID].ToString());
                }

                TaskInfo.Msg_Paramslist p = new TaskInfo.Msg_Paramslist()
                {
                    WarehouseID = row[t.WarehouseID].ToString(),
                    ZoneID = row[t.ZoneID].ToString(),
                    TaskNum = row[t.TaskNo].ToString(),
                    TaskStatus = row[t.TaskStatus].ToString(),
                    TaskType = row[t.TaskType].ToString(),
                    Barcode = row[t.ContainerBarcode].ToString(),
                    Wave_ID = row[t.RefDocID].ToString(),
                    GroupNum = "1", //row["group_no"].ToString(),
                    SeqNum = Int32.Parse("0" + row[t.SeqNo].ToString()),
                    SourceBillType = row[t.RefDocType].ToString(),
                    SourceAddress = row[t.SrcAddrText].ToString(),
                    TargetAddress = row[t.DestAddrText].ToString(),
                    EquipmentNum = equipRow?["equipment_code"].ToString(),
                    LedText = row[t.LedText].ToString(),
                    ItemCode = itemRow?[i.ItemCode].ToString(),
                    //WmsOrderNo = row["source_bill_d_no"].ToString(),
                    //WmsOrderQty = taskRows.Length.ToString(),
                    //Priority = row["priority"].ToString(),
                    //RowSeq = row["seq_no"].ToString(),
                    //ItemName = row["item_name"].ToString(),
                    //ItemCount = row["operation_qty"].ToString(),
                };
                taskInfo.Msg_Params.Add(p);
            }

            LogHelper.LogDebug("发送任务到WCS(DownOutboundTask)");
            LogHelper.LogDebug(JsonConvert.SerializeObject(taskInfo));
            var service = Context.ServiceProvider.GetService<IPostGetDataService>();
            var url = WmsConfig.GetOrDefault("Interface:IPSAddress", "localhost");
            var model = new PostGetDataInput.PostResponseDataModel()
            {
                url = url + "/api/tyjk/Task/DownOutboundTask",
                postData = JsonConvert.SerializeObject(taskInfo),
                type = "json",//json
            };
            UserLogModel log = new UserLogModel()
            {
                LogText = $"下发WCS任务{taskRows[0][t.TaskNo]}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(taskInfo),
                TypeCode = "下发WCS任务",
                CommonName = model.url,
            };
            try
            {
                WebJsonInfo result = service.PostResponseData(model);
                retInfo = WmsHelper.ResolveJsonResultFromWcs(result.json.ToString());
                /*
                var je = JsonDocument.Parse(result.json.ToString()).RootElement;
                var errCode = je.GetProperty("errcode").GetInt32();
                if (errCode != 0)
                {
                    retInfo.errcode = errCode;
                    retInfo.errmsg = je.GetProperty("errmsg").ToString();
                    retInfo.isSucceed = false;
                }
                return retInfo;
                */
                if (retInfo.errcode == 0)
                {
                    log.LogText += ", 执行成功";
                    log.LogTextLong += ", 返回信息：成功";
                    retInfo.isSucceed = true;
                }
                else
                {
                    log.LogText += ", 执行失败";
                    retInfo.isSucceed = false;
                    log.LogTextLong += $", 返回信息：[{retInfo.errcode}]{retInfo.errmsg}，接口平台反馈信息：{result.json.ToString()}";
                }
            }
            catch (Exception ex)
            {
                log.LogText += ", 执行失败";
                log.LogTextLong += $", 返回信息：{ex.Message}";

                retInfo.errcode = 500;
                retInfo.errmsg = $"下发任务到WCS发生错误:{ex.Message}";
                retInfo.isSucceed = false;
            }
            LogHelper.LogInformation(JsonConvert.SerializeObject(log));
            UserLog.Add(log);
            /*
            var service = CurrentUser.Proxy.GetService<IWcsPassiveService>();
            string result = service.DownOutboundTask(JsonConvert.SerializeObject(taskInfo)).Result;
            info = Util.resolveJsonResultFromWcs(result, out JObject retMsg);
            if (info.ErrorCode != 0)
            {
                Util.InsertInterfaceLog("发送波次任务到WCS", info.ErrMsg, "DownOutboundTask",
                    taskRows[0]["task_number"].ToString(),
                    taskRows[0]["task_type_code"].ToString(),
                    taskRows[0]["container_barcode"].ToString(),
                    taskRows[0]["source_text"].ToString(),
                    taskRows[0]["target_text"].ToString());
            }
            */
            return retInfo;
        }

        /// <summary>
        /// 要料终端下发入库任务到WCS, 若报文发送失败或者返回的结果无法解析，本程序不会抛出异常，只会返回错误信息。
        /// </summary>
        /// <param name="ids">任务ID或任务号，如有多个，用英文逗号隔开</param>
        /// <returns>当报文发送失败或者返回的结果无法解析时，错误信息包含在返回结果中</returns>
        WebJsonInfo IWorkTaskInterface.SendInBonud(List<string> ids)
        {
            WebJsonInfo retInfo = new WebJsonInfo();
            if (WmsConfig.GetOrDefault("Task:SendToWCS", "0") == "0")
            {
                return retInfo;
            }

            TaskInfo taskInfo = new TaskInfo()
            {
                Msg_Id = WmsHelper.GetSequence("WMS_MessageID"),
                Msg_Mark = "WMS->WCS",
                Msg_Time = DateTime.Now.ToString(),
                Msg_User = "WMS",
                Msg_UserPwd = "admin",
                Msg_Method = "入库",
            };

            taskInfo.Msg_Params = new List<TaskInfo.Msg_Paramslist>();

            DataRow[] taskRows = WorkTask.GetWorkTasks(string.Join(",", ids));
            foreach (DataRow row in taskRows)
            {
                if (row[t.ExecutorType].ToString() != TaskExecutor.TaskExecutor_WCS.ToString())
                {
                    continue;
                }
                DataRow equipRow = Equipment.GetEquipment(row[t.EquipmentID].ToString());
                DataRow[] taskItemRows = WorkTaskItem.GetByTaskID(row[t.ID].ToString());
                DataRow itemRow = null;
                if (taskItemRows.Count() > 0)
                {
                    itemRow = Item.Get(taskItemRows.First()[ti.ItemID].ToString());
                }

                TaskInfo.Msg_Paramslist p = new TaskInfo.Msg_Paramslist()
                {
                    WarehouseID = row[t.WarehouseID].ToString(),
                    ZoneID = row[t.ZoneID].ToString(),
                    TaskNum = row[t.TaskNo].ToString(),
                    TaskStatus = row[t.TaskStatus].ToString(),
                    TaskType = row[t.TaskType].ToString(),
                    Barcode = row[t.ContainerBarcode].ToString(),
                    Wave_ID = row[t.RefDocID].ToString(),
                    GroupNum = "1", //row["group_no"].ToString(),
                    SeqNum = Int32.Parse("0" + row[t.SeqNo].ToString()),
                    SourceBillType = row[t.RefDocType].ToString(),
                    SourceAddress = row[t.SrcAddrText].ToString(),
                    TargetAddress = row[t.DestAddrText].ToString(),
                    EquipmentNum = equipRow["equipment_code"].ToString(),
                    LedText = row[t.LedText].ToString(),
                    ItemCode = itemRow?[i.ItemCode].ToString(),
                    //WmsOrderNo = row["source_bill_d_no"].ToString(),
                    //WmsOrderQty = taskRows.Length.ToString(),
                    //Priority = row["priority"].ToString(),
                    //RowSeq = row["seq_no"].ToString(),
                    //ItemName = row["item_name"].ToString(),
                    //ItemCount = row["operation_qty"].ToString(),
                };
                taskInfo.Msg_Params.Add(p);
            }

            LogHelper.LogDebug("发送任务到WCS(DownOutboundTask)");
            LogHelper.LogDebug(JsonConvert.SerializeObject(taskInfo));
            var service = Context.ServiceProvider.GetService<IPostGetDataService>();
            var url = WmsConfig.GetOrDefault("Interface:IPSAddress", "localhost");
            var model = new PostGetDataInput.PostResponseDataModel()
            {
                url = url + "/api/tyjk/Task/DownOutboundTask",
                postData = JsonConvert.SerializeObject(taskInfo),
                type = "json",
            };
            UserLogModel log = new UserLogModel()
            {
                LogText = $"下发WCS任务{taskRows[0][t.TaskNo]}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(taskInfo),
                TypeCode = "下发WCS任务",
                CommonName = "/api/tyjk/Task/DownOutboundTask",
            };
            try
            {
                WebJsonInfo result = service.PostResponseData(model);
                retInfo = WmsHelper.ResolveJsonResultFromWcs(result.json.ToString());
                /*
                var je = JsonDocument.Parse(result.json.ToString()).RootElement;
                var errCode = je.GetProperty("errcode").GetInt32();
                if (errCode != 0)
                {
                    retInfo.errcode = errCode;
                    retInfo.errmsg = je.GetProperty("errmsg").ToString();
                    retInfo.isSucceed = false;
                }
                return retInfo;
                */
                if (retInfo.errcode == 0)
                {
                    log.LogText += ", 执行成功";
                    log.LogTextLong += ", 返回信息：成功";
                }
                else
                {
                    log.LogText += ", 执行失败";
                    log.LogTextLong += $", 返回信息：[{retInfo.errcode}]{retInfo.errmsg}";
                }
            }
            catch (Exception ex)
            {
                log.LogText += ", 执行失败";
                log.LogTextLong += $", 返回信息：{ex.Message}";

                retInfo.errcode = 500;
                retInfo.errmsg = $"下发任务到WCS发生错误:{ex.Message}";
                retInfo.isSucceed = false;
            }
            LogHelper.LogInformation(JsonConvert.SerializeObject(log));
            UserLog.Add(log);
            /*
            var service = CurrentUser.Proxy.GetService<IWcsPassiveService>();
            string result = service.DownOutboundTask(JsonConvert.SerializeObject(taskInfo)).Result;
            info = Util.resolveJsonResultFromWcs(result, out JObject retMsg);
            if (info.ErrorCode != 0)
            {
                Util.InsertInterfaceLog("发送波次任务到WCS", info.ErrMsg, "DownOutboundTask",
                    taskRows[0]["task_number"].ToString(),
                    taskRows[0]["task_type_code"].ToString(),
                    taskRows[0]["container_barcode"].ToString(),
                    taskRows[0]["source_text"].ToString(),
                    taskRows[0]["target_text"].ToString());
            }
            */
            return retInfo;
        }
    }

    /// <summary>
    /// 任务信息
    /// </summary>
    public class TaskInfo
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public string Msg_Id { get; set; }
        /// <summary>
        /// 消息标记
        /// </summary>
        public string Msg_Mark { get; set; }
        /// <summary>
        /// 调用时间
        /// </summary>
        public string Msg_Time { get; set; }
        /// <summary>
        /// 调用用户
        /// </summary>
        public string Msg_User { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Msg_UserPwd { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        public string Msg_Method { get; set; }
        /// <summary>
        /// 数据组
        /// </summary>
        public List<Msg_Paramslist> Msg_Params { get; set; }

        /// <summary>
        /// 数据组
        /// </summary>
        public class Msg_Paramslist
        {

            /// <summary>
            /// 
            /// </summary>
            //public string SourceType { get; set; }
            /// <summary>
            /// 仓库ID
            /// </summary>
            public string WarehouseID { get; set; }
            /// <summary>
            /// 区域ID
            /// </summary>
            public string ZoneID { get; set; }
            /// <summary>
            /// 任务号
            /// </summary>
            public string TaskNum { get; set; }
            /// <summary>
            /// 任务状态
            /// </summary>
            public string TaskStatus { get; set; }
            /// <summary>
            /// 任务类型
            /// </summary>
            public string TaskType { get; set; }
            /// <summary>
            /// 托盘条码
            /// </summary>
            //public string ContainerBarCode { get; set; }
            /// <summary>
            /// 烟包条码
            /// </summary>
            public string Barcode { get; set; }
            /// <summary>
            /// 波次号
            /// </summary>
            public string Wave_ID { get; set; }
            /// <summary>
            /// 组编号
            /// </summary>
            public string GroupNum { get; set; }
            /// <summary>
            /// 组序号
            /// </summary>
            public int SeqNum { get; set; }
            /// <summary>
            /// 订单顺序号
            /// </summary>
            public string OrderSeqNum { get; set; }
            /// <summary>
            /// 来源单据类型
            /// </summary>
            public string SourceBillType { get; set; }
            /// <summary>
            /// 来源地址
            /// </summary>
            public string SourceAddress { get; set; }
            /// <summary>
            /// 目标地址
            /// </summary>
            public string TargetAddress { get; set; }
            /// <summary>
            /// 执行设备编号
            /// </summary>
            public string EquipmentNum { get; set; }
            /// <summary>
            /// LED显示信息
            /// </summary>
            public string LedText { get; set; }
            /// <summary>
            /// 物料编码
            /// </summary>
            public string ItemCode { get; set; }

            /// <summary>
            /// 件烟数量
            /// </summary>
            //public string ItemCount { get; set; }
            /// <summary>
            /// 物料名称
            /// </summary>
            //public string ItemName { get; set; }
            /// <summary>
            /// 行顺序
            /// </summary>
            //public string RowSeq { get; set; }
            /// <summary>
            /// 优先级
            /// </summary>
            //public string Priority { get; set; }
            /// <summary>
            /// 订单数量
            /// </summary>
            //public string WmsOrderQty { get; set; }
            /// <summary>
            /// 订单号
            /// </summary>
            //public string WmsOrderNo { get; set; }
            /// <summary>
            /// 件烟条码
            /// </summary>
            //public List<BarcodeList> ItemBarcode { get; set; }

            /// <summary>
            /// 条码
            /// </summary>
            public class BarcodeList
            {
                /// <summary>
                /// 条码
                /// </summary>
                public string Barcode { get; set; }
            }
        }
    }

    //
    // 摘要:
    //     回收任务 ，可按批次号回收或者按任务号回收
    public class RollBackTaskInfo
    {
        //
        // 摘要:
        //     消息ID
        public string Msg_Id { get; set; }
        //
        // 摘要:
        //     消息标记
        public string Msg_Mark { get; set; }
        //
        // 摘要:
        //     调用时间
        public string Msg_Time { get; set; }
        //
        // 摘要:
        //     调用用户
        public string Msg_User { get; set; }
        //
        // 摘要:
        //     密码
        public string Msg_UserPwd { get; set; }
        //
        // 摘要:
        //     方法名称
        public string Msg_Method { get; set; }
        //
        // 摘要:
        //     数据组
        public List<Msg_Paramslist> Msg_Params { get; set; }

        //
        // 摘要:
        //     数据组
        public class Msg_Paramslist
        {
            //
            // 摘要:
            //     仓库ID
            public string WarehouseID { get; set; }
            //
            // 摘要:
            //     区域ID
            public string ZoneID { get; set; }
            //
            // 摘要:
            //     任务号
            public string TaskNum { get; set; }
            //
            // 摘要:
            //     波次号
            public string Wave_ID { get; set; }
            //
            // 摘要:
            //     标记
            public string Mark { get; set; }
        }
    }

    public class WaveInfo
    {
        public string Msg_Id { get; set; }
        public string Msg_Mark { get; set; }
        public string Msg_Time { get; set; }
        public string Msg_User { get; set; }
        public string Msg_UserPwd { get; set; }
        public string Msg_Method { get; set; }
        public List<Msg_Paramslist> Msg_Params { get; set; }

        public class Msg_Paramslist
        {
            public string WarehouseID { get; set; }
            public string ZoneID { get; set; }
            public string Wave_ID { get; set; }
            public string Wave_Status { get; set; }
        }
    }
}
