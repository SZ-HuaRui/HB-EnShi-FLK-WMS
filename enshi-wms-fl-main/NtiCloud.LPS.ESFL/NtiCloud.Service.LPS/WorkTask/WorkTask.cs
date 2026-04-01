using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Interface.WCS;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Runtime.CompilerServices;
using NtiCloud.Service.WMS.Inventory;
using Microsoft.AspNetCore.Server.IIS.Core;
using NtiCloud.Services.WMS.Base;
using NtiCloud.Service.LPS.Inbound;
using NtiCloud.Service.LPS.Model;
using NtiCloud.Service.LPS.Base;
using static NtiCloud.Model.WMS.MachineMaterialInput;
using static NtiCloud.Model.WMS.LocationAdjustModel;
using System.Diagnostics.Eventing.Reader;
using NtiCloud.Service.WMS.QC;
using Newtonsoft.Json.Linq;
using NtiCloud.Service.WMS.CallEnd;
using System.IO;
using static NtiCloud.Service.WMS.Common.DBHelper;

namespace NtiCloud.Service.WMS.Tasking
{
    public class WorkTask : WmsObject, IWorkTask
    {
        #region Data Access
        /*
        public IWorkTask DeleteDependentTask(string dependentTaskNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "task_no", dependentTaskNo }
            };
            string sql = @"
delete from isa_m_task
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and dependent_task_no = @task_no
";
            DBHelper.RunSql(sql, param, "删除依赖任务");
            return this;
        }
        */
        private IDBContextProvider dbW;
        public WorkTask(IDBContextProvider _dbW)
        {
            dbW = _dbW;
        }
        public WorkTask()
        {
            dbW = null;
        }

        public IWorkTask DeleteWorkTask(string taskNoOrID)
        {
            //InsertHistory(taskNoOrID);
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", taskNoOrID }
            };
            string sql = @"
delete from &DataSource&wms_b_task
 where (t_id = @t_id or task_no = @t_id)
";
            DBHelper.RunSql(sql, param, "删除任务表");
            return this;
        }

        /// <summary>
        /// 删除任务明细
        /// </summary>
        /// <param name="taskitemid">任务行ID</param>
        /// <returns></returns>
        public IWorkTask DeleteWorkTaskItem(string taskitemid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", taskitemid }
            };
            string sql = @"delete from &DataSource&wms_b_task_item
                            where task_id = @t_id ";
            DBHelper.RunSql(sql, param, "删除任务表", DBHelper.OperatedNumType.OneOrMore);
            return this;
        }
        public IWorkTask DeleteWorkTaskItemByItemId(string taskitemid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", taskitemid }
            };
            string sql = @"delete from &DataSource&wms_b_task_item
                            where warehouse_id = @warehouse  and t_id = @t_id ";
            DBHelper.RunSql(sql, param, "删除任务表", DBHelper.OperatedNumType.OneOrMore);
            return this;
        }
        public IWorkTask Insert(WorkTaskModel model)
        {
            model.ID = model.ID ?? SnowflakeID.Guid().ToString();
            model.WarehouseID = model.WarehouseID ?? Context.WarehouseID;
            model.CreatedBy = model.CreatedBy ?? Context.UserInfo.UserName;
            model.ModifyIP = model.ModifyIP ?? Context.UserInfo.LoginIP;
            model.LastModifiedBy = model.LastModifiedBy ?? Context.UserInfo.UserName;
            model.ActStatus = "1";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "t_id", model.ID },
                { "warehouse_id", model.WarehouseID },
                { "zone_id", model.ZoneID },
                { "task_no", model.TaskNo },
                { "dependent_task_no", model.DependentTaskNo },
                { "task_type", model.TaskType },
                { "executed_by", model.ExecutedBy },
                { "equipment_id", model.EquipmentID },
                { "executor_type", model.ExecutorType },
                { "actual_equipment_id", model.ActualEquipmentID },
                { "seq_no", model.SeqNo.ToString() },
                { "priority", model.Priority.ToString() },
                { "container_id", model.ContainerID },
                { "container_barcode", model.ContainerBarcode },
                { "ref_doc_type", model.RefDocType },
                { "ref_doc_id", model.RefDocID },
                { "ref_doc_d_id", model.RefDocDetailID },
                { "ref_doc_no", model.RefDocNo },
                { "src_addr_id", model.SrcAddrID },
                { "src_addr_type", model.SrcAddrType },
                { "src_addr_text", model.SrcAddrText },
                { "dest_addr_id", model.DestAddrID },
                { "dest_addr_type", model.DestAddrType },
                { "dest_addr_text", model.DestAddrText },
                { "actual_dest_addr_id", model.ActualDestAddrID },
                { "actual_dest_addr_text", model.ActualDestAddrText },
                { "task_status", model.TaskStatus },
                { "start_date", model.StartDate.ToString() },
                { "end_date", model.EndDate.ToString() },
                { "is_executable", model.IsExecutable },
                { "priority_change_time", model.PriorityChangeTime.ToString() },
                { "led_text", model.LedText },
                { "exist_lgv", model.ExistLgv },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into &DataSource&WMS_B_TASK ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       zone_id, " +
                "       task_no, " +
                "       dependent_task_no, " +
                "       task_type, " +
                "       executed_by, " +
                "       equipment_id, " +
                "       executor_type, " +
                "       actual_equipment_id, " +
                "       seq_no, " +
                "       priority, " +
                "       container_id, " +
                "       container_barcode, " +
                "       ref_doc_type, " +
                "       ref_doc_id, " +
                "       ref_doc_d_id, " +
                "       ref_doc_no, " +
                "       src_addr_id, " +
                "       src_addr_type, " +
                "       src_addr_text, " +
                "       dest_addr_id, " +
                "       dest_addr_type, " +
                "       dest_addr_text, " +
                "       actual_dest_addr_id, " +
                "       actual_dest_addr_text, " +
                "       task_status, " +
                "       start_date, " +
                "       end_date, " +
                "       is_executable, " +
                "       priority_change_time, " +
                "       led_text, " +
                "       exist_lgv, " +
                "       create_date, " +
                "       created_by, " +
                "       modify_ip, " +
                "       modify_date, " +
                "       last_modified_by, " +
                "       act_status) " +
                "values ( " +
                "       @t_id, " +
                "       '&CLIENT_ID&', " +
                "       '&ORG_ID&', " +
                "       @warehouse_id, " +
                "       @zone_id, " +
                "       @task_no, " +
                "       @dependent_task_no, " +
                "       @task_type, " +
                "       @executed_by, " +
                "       @equipment_id, " +
                "       @executor_type, " +
                "       @actual_equipment_id, " +
                "       @seq_no, " +
                "       @priority, " +
                "       @container_id, " +
                "       @container_barcode, " +
                "       @ref_doc_type, " +
                "       @ref_doc_id, " +
                "       @ref_doc_d_id, " +
                "       @ref_doc_no, " +
                "       @src_addr_id, " +
                "       @src_addr_type, " +
                "       @src_addr_text, " +
                "       @dest_addr_id, " +
                "       @dest_addr_type, " +
                "       @dest_addr_text, " +
                "       @actual_dest_addr_id, " +
                "       @actual_dest_addr_text, " +
                "       @task_status, " +
                "       date_format(@start_date, '%Y-%m-%d %H:%i:%s'), " +
                "       date_format(@end_date, '%Y-%m-%d %H:%i:%s'), " +
                "       @is_executable, " +
                "       date_format(@priority_change_time, '%Y-%m-%d %H:%i:%s'), " +
                "       @led_text, " +
                "       @exist_lgv, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "任务表");

            return this;
        }

        /// <summary>
        /// 根据任务实体类，插入任务历史表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IWorkTask InsertFinishedTask(WorkTaskModel model)

        {
            model.ID = model.ID ?? SnowflakeID.Guid().ToString();
            model.WarehouseID = model.WarehouseID ?? Context.WarehouseID;
            model.CreatedBy = model.CreatedBy ?? Context.UserInfo.UserName;
            model.ModifyIP = model.ModifyIP ?? Context.UserInfo.LoginIP;
            model.LastModifiedBy = model.LastModifiedBy ?? Context.UserInfo.UserName;
            model.ActStatus = "1";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "t_id", model.ID },
                { "warehouse_id", model.WarehouseID },
                { "zone_id", model.ZoneID },
                { "task_no", model.TaskNo },
                { "dependent_task_no", model.DependentTaskNo },
                { "task_type", model.TaskType },
                { "executed_by", model.ExecutedBy },
                { "equipment_id", model.EquipmentID },
                { "executor_type", model.ExecutorType },
                { "actual_equipment_id", model.ActualEquipmentID },
                { "seq_no", model.SeqNo.ToString() },
                { "priority", model.Priority.ToString() },
                { "container_id", model.ContainerID },
                { "container_barcode", model.ContainerBarcode },
                { "ref_doc_type", model.RefDocType },
                { "ref_doc_id", model.RefDocID },
                { "ref_doc_d_id", model.RefDocDetailID },
                { "ref_doc_no", model.RefDocNo },
                { "src_addr_id", model.SrcAddrID },
                { "src_addr_type", model.SrcAddrType },
                { "src_addr_text", model.SrcAddrText },
                { "dest_addr_id", model.DestAddrID },
                { "dest_addr_type", model.DestAddrType },
                { "dest_addr_text", model.DestAddrText },
                { "actual_dest_addr_id", model.ActualDestAddrID },
                { "actual_dest_addr_text", model.ActualDestAddrText },
                { "task_status", model.TaskStatus },
                { "start_date", model.StartDate.ToString() },
                { "end_date", model.EndDate.ToString() },
                { "is_executable", model.IsExecutable },
                { "priority_change_time", model.PriorityChangeTime.ToString() },
                { "led_text", model.LedText },
                { "exist_lgv", model.ExistLgv },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into &DataSource&WMS_B_TASK_HIS ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       zone_id, " +
                "       task_no, " +
                "       dependent_task_no, " +
                "       task_type, " +
                "       executed_by, " +
                "       equipment_id, " +
                "       executor_type, " +
                "       actual_equipment_id, " +
                "       seq_no, " +
                "       priority, " +
                "       container_id, " +
                "       container_barcode, " +
                "       ref_doc_type, " +
                "       ref_doc_id, " +
                "       ref_doc_d_id, " +
                "       ref_doc_no, " +
                "       src_addr_id, " +
                "       src_addr_type, " +
                "       src_addr_text, " +
                "       dest_addr_id, " +
                "       dest_addr_type, " +
                "       dest_addr_text, " +
                "       actual_dest_addr_id, " +
                "       actual_dest_addr_text, " +
                "       task_status, " +
                "       start_date, " +
                "       end_date, " +
                "       is_executable, " +
                "       priority_change_time, " +
                "       led_text, " +
                "       exist_lgv, " +
                "       create_date, " +
                "       created_by, " +
                "       modify_ip, " +
                "       modify_date, " +
                "       last_modified_by, " +
                "       act_status) " +
                "values ( " +
                "       @t_id, " +
                "       '&CLIENT_ID&', " +
                "       '&ORG_ID&', " +
                "       @warehouse_id, " +
                "       @zone_id, " +
                "       @task_no, " +
                "       @dependent_task_no, " +
                "       @task_type, " +
                "       @executed_by, " +
                "       @equipment_id, " +
                "       @executor_type, " +
                "       @actual_equipment_id, " +
                "       @seq_no, " +
                "       @priority, " +
                "       @container_id, " +
                "       @container_barcode, " +
                "       @ref_doc_type, " +
                "       @ref_doc_id, " +
                "       @ref_doc_d_id, " +
                "       @ref_doc_no, " +
                "       @src_addr_id, " +
                "       @src_addr_type, " +
                "       @src_addr_text, " +
                "       @dest_addr_id, " +
                "       @dest_addr_type, " +
                "       @dest_addr_text, " +
                "       @actual_dest_addr_id, " +
                "       @actual_dest_addr_text, " +
                "       @task_status, " +
                "       date_format(@start_date, '%Y-%m-%d %H:%i:%s'), " +
                "       date_format(@end_date, '%Y-%m-%d %H:%i:%s'), " +
                "       @is_executable, " +
                "       date_format(@priority_change_time, '%Y-%m-%d %H:%i:%s'), " +
                "       @led_text, " +
                "       @exist_lgv, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "任务历史表");

            return this;
        }

        /// <summary>
        /// 部分完成任务
        /// </summary>
        /// <param name="taskno"></param>
        /// <returns></returns>
        internal bool PartionCompleteTask(string taskno)
        {
            TaskHeaderModel task = GetTaskHeaderByNo(taskno);
            if (task == null)
            {
                throw new Exception("未找到任务信息:" + taskno);
            }

            if (task.ref_doc_type == DocType.DocType_CallTerminal.ToString())//要料
            {
                new MachineMaterialBusinessProce().PartionFinishMachineMaterial(new PartionFinishMachineMaterialModel { TaskTid = task.t_id });
                return true;
            }
            else if (task.ref_doc_type == DocType.DocType_ClientReturnNullPallet.ToString())//退空托盘
            {
                new MachineMaterialBusinessProce().PartionFinishReturnNullPallet(new PartionFinishReturnNullPalletModel { TaskTid = task.t_id });
                return true;
            }
            else if (task.ref_doc_type == DocType.DocType_ClientReturnPallet.ToString())//退余料
            {
                new MachineMaterialBusinessProce().PartionFinishClientReturnPallet(new PartionFinishClientReturnPalletModel { TaskTid = task.t_id });
                return true;
            }
            else if (task.ref_doc_type == DocType.DocType_ReturnBadPallet.ToString())//退坏料
            {
                new MachineMaterialBusinessProce().PartionFinishReturnBadPallet(new PartionFinishReturnBadPalletModel { TaskTid = task.t_id });
                return true;
            }
            else if (task.ref_doc_type == DocType.DocType_ClientReturnWaste.ToString())//退废料
            {
                new MachineMaterialBusinessProce().PartionFinishReturnWasteMaterials(new PartionFinishReturnWasteMaterialsModel { TaskTid = task.t_id });
                return true;
            }
            else if (task.ref_doc_type == DocType.DocType_ProductTransfer.ToString())//成品转移
            {
                new MachineMaterialBusinessProce().PartionFinishProductTransferTask(new PartionFinishProductTransferTaskModel { TaskTid = task.t_id });
                return true;
            }
            else if (task.ref_doc_type == DocType.DocType_Adjustment.ToString())//人工库位调整
            {
                new LocationAdjust.LocationAdj().PartionFinishLocationadJustTask(new PartionFinishLocationadJustTaskModel { TaskTid = task.t_id });
                return true;
            }
            else if (task.ref_doc_type == DocType.DocType_AutoAdjustment.ToString())//自动库位调整
            {
                new LocationAdjust.LocationAdj().PartionFinishAutoLocationadJustTask(new PartionFinishAutoLocationadJustTaskModel { TaskTid = task.t_id });
                return true;
            }
            if (task.task_type == TaskType.TaskType_OutBound.ToString())
            {
                if (task.task_status == TaskStatus.TaskStatus_Executing.ToString() ||
                    task.task_status == TaskStatus.TaskStatus_WaitingExecute.ToString())
                {
                    UpdateTaskStatus(task.t_id, TaskStatus.TaskStatus_PartialFinished.ToString(), task.task_status);
                    Container container = new Container();
                    ContainerModel ctnh = container.getContainerById(task.container_id);
                    string convayorlocationid = new Location().getTransferLineLocationId(task.warehouse_id);
                    container.UpdateContainerLocation(ctnh.t_id, "");
                    container.UpdateHeadActStatus(ctnh.t_id, "0");
                    Location location = new Location();
                    location.UpdateLocationStatus(ctnh.location_id, LocationStatus.LocationStatus_Empty);
                    #region 增加SN日志
                    LocationModel conveyoerLoc = location.GetLocationById(convayorlocationid);
                    container.UpdateContainerSnInLocation(task.container_id, conveyoerLoc, task.task_no, "出库部分完成", task.ref_doc_type);
                    #endregion

                    string zoneIdInTask = task.zone_id;
                    string LocationID = task.src_addr_id;
                    string refDocTypeInTask = task.ref_doc_type;
                    if (string.Equals(refDocTypeInTask, DocType.DocType_Retrieval.ToString()) && string.Equals(zoneIdInTask, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))//恩施辅料库，二楼下架任务
                    {
                        Location lo = new Location();
                        LocationModel _locInfo = lo.GetLocationById(LocationID);
                        bool isLineEmpty = IsLineEmpty(_locInfo.location_row.ToString());//判断行，是否已空
                        Model.UserLogModel _logModel = new Model.UserLogModel();
                        _logModel.ModifyIP = Context.UserInfo.LoginIP;
                        _logModel.WarehouseID = Context.WarehouseID;
                        _logModel.TypeCode = "二楼下架任务部分完成";
                        _logModel.LogTextLong = $"货位编码{_locInfo.location_no},行号：" + _locInfo.location_row.ToString() + "  结果为" + isLineEmpty;
                        _logModel.CommonName = "查询指定行是否已空";
                        _logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
                        _logModel.CreatedBy = Context.UserInfo.UserId;
                        _logModel.LogText = "";
                        UserLog.Add(_logModel);
                        if (isLineEmpty == true)
                        {
                            Model.UserLogModel _logModel2 = new Model.UserLogModel();
                            _logModel2.ModifyIP = Context.UserInfo.LoginIP;
                            _logModel2.WarehouseID = Context.WarehouseID;
                            _logModel2.TypeCode = "二楼下架任务部分完成";
                            _logModel2.LogTextLong = $"货位编码{_locInfo.location_no},行号：" + _locInfo.location_row.ToString();
                            _logModel2.CommonName = "查询指定行已全部为空，且全部为可出不可入";
                            _logModel2.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
                            _logModel2.CreatedBy = Context.UserInfo.UserId;
                            _logModel2.LogText = "";
                            UserLog.Add(_logModel2);
                            //lo.UpdateLocationStatus(_locInfo.t_id, LocationStatus.LocationStatus_Stored);
                            UpdateLocationFlag(_locInfo.location_row.ToString(), LocationFlag.LocationFlag_Usable.ToString());//将行上所有库位标记，全部修改成“可用”
                        }
                        else
                        {
                            bool isLineHaveOutFlag = IsLineHaveOutFlag(_locInfo.location_row.ToString(), _locInfo.location_no);//判断行，是否存在可出不可入的货位
                            if (isLineHaveOutFlag==false)
                            {
                                UpdateLocationFlag(_locInfo.location_row.ToString(), LocationFlag.LocationFlag_Usable.ToString());//将行上所有库位标记，全部修改成“可用”
                            }
                        }
                    }


                    return true;
                }
                else
                {
                    throw new Exception("任务当前状态为:" + task.task_status + "，不允许部分完成");
                }
            }
            else if (task.task_type == TaskType.TaskType_InBound.ToString())
            {
                if (task.task_status == TaskStatus.TaskStatus_Executing.ToString() ||
                   task.task_status == TaskStatus.TaskStatus_WaitingExecute.ToString())
                {
                    UpdateTaskPartionComplete(task.t_id);
                    return true;
                }
                else
                {
                    throw new Exception("任务当前状态为:" + task.task_status + "，不允许部分完成");
                }
            }
            else
            {
                return true;
            }
        }

        public IWorkTask Insert(DataRow row)
        {
            Insert(new WorkTaskModel()
            {
                ID = row["t_id"].ToString(),
                ClientID = row["client_id"].ToString(),
                OrgID = row["org_id"].ToString(),
                WarehouseID = row["warehouse_id"].ToString(),
                ZoneID = row["zone_id"].ToString(),
                TaskNo = row["task_no"].ToString(),
                DependentTaskNo = row["dependent_task_no"].ToString(),
                TaskType = row["task_type"].ToString(),
                ExecutedBy = row["executed_by"].ToString(),
                EquipmentID = row["equipment_id"].ToString(),
                ExecutorType = row["executor_type"].ToString(),
                ActualEquipmentID = row["actual_equipment_id"].ToString(),
                SeqNo = DBHelper.GetDecimal(row["seq_no"]),
                Priority = DBHelper.GetDecimal(row["priority"]),
                ContainerID = row["container_id"].ToString(),
                ContainerBarcode = row["container_barcode"].ToString(),
                RefDocType = row["ref_doc_type"].ToString(),
                RefDocID = row["ref_doc_id"].ToString(),
                RefDocDetailID = row["ref_doc_d_id"].ToString(),
                RefDocNo = row["ref_doc_no"].ToString(),
                SrcAddrID = row["src_addr_id"].ToString(),
                SrcAddrType = row["src_addr_type"].ToString(),
                SrcAddrText = row["src_addr_text"].ToString(),
                DestAddrID = row["dest_addr_id"].ToString(),
                DestAddrType = row["dest_addr_type"].ToString(),
                DestAddrText = row["dest_addr_text"].ToString(),
                ActualDestAddrID = row["actual_dest_addr_id"].ToString(),
                ActualDestAddrText = row["actual_dest_addr_text"].ToString(),
                TaskStatus = row["task_status"].ToString(),
                StartDate = DBHelper.GetDateTime(row["start_date"]),
                EndDate = DBHelper.GetDateTime(row["end_date"]),
                IsExecutable = row["is_executable"].ToString(),
                PriorityChangeTime = DBHelper.GetDateTime(row["priority_change_time"]),
                LedText = row["led_text"].ToString(),
                ExistLgv = row["exist_lgv"].ToString(),
                CreateDate = DBHelper.GetDateTime(row["create_date"]),
                CreatedBy = row["created_by"].ToString(),
                ModifyIP = row["modify_ip"].ToString(),
                ModifyDate = DBHelper.GetDateTime(row["modify_date"]),
                LastModifiedBy = row["last_modified_by"].ToString(),
                ActStatus = row["act_status"].ToString()
            });

            return this;
        }

        public void UpdateLedText(string idOrNo, string ledText)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "ledText", ledText },
                { "t_id", idOrNo }
            };
            string sql = @"
update &DataSource&wms_b_task
   set modify_date = now(),
       last_modified_by = @last_modified_by,
       modify_ip = @modify_ip,
       LED_TEXT = @ledText
 where (t_id = @t_id or task_no = @t_id)
";
            DBHelper.RunSql(sql, param, "更新LED显示内容");
        }

        public DataRow Get(string taskNoOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", taskNoOrID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_task_v
 where (t_id = @t_id or task_no = @t_id)
   and act_status = '1'
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询任务表");

            return dt.Rows[0];
        }

        /// <summary>
        /// 更新货位标识
        /// </summary>
        /// <param name="locationid">货位ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public bool UpdateLocationFlag(string line, string newStatus)
        {
            string sql = @"update  &DataSource&wms_m_location l set l.location_flag = @newStatus,
                                                         l.last_modified_by = @last_modified_by,
                                                         l.modify_ip = @modify_ip,
                                                         l.modify_date = sysdate
                            where l.location_row = @line and l.act_status = '1' ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("newStatus", newStatus);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("line", line);

            return DBHelper.RunSql(sql, lstParam, "更新货位标识", OperatedNumType.OneOrMore) > 0;
        }

        #region 查询指定行是否已空
        /// <summary>
        /// 查询指定行是否已空
        /// </summary>
        /// <param name="line">行号</param>
        /// <returns>true:已存满；false：未满</returns>
        public bool IsLineEmpty(string line)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "line", line }
            };
            string sql = $"select count(l.t_id) as cnt from &DataSource&wms_m_location l where l.location_status in('{LocationStatus.LocationStatus_In}','{LocationStatus.LocationStatus_Stored}','{LocationStatus.LocationStatus_Out}','{LocationStatus.LocationStatus_Plan}') and l.location_row=@line and l.act_status = '1'";
            DataTable dt = DBHelper.Select(sql, param, "查询指定行是否已空");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) == 0;
        }
        #endregion

        #region 查询指定行是否存在非已存储或为空的货位
        /// <summary>
        /// 查询指定行是否存在非已存储或为空的货位
        /// </summary>
        /// <param name="line">行号</param>
        /// <returns>true:存在；false：不存在</returns>
        public bool IsLineHaveOutFlag(string line,string locNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "line", line },
                { "locNo", locNo }
            };
            string sql = $"select count(l.t_id) as cnt from &DataSource&wms_m_location l where l.location_status not in('{LocationStatus.LocationStatus_Stored}','{LocationStatus.LocationStatus_Empty}') and l.location_no<>@locNo and l.location_row=@line and l.act_status = '1'";
            DataTable dt = DBHelper.Select(sql, param, "查询指定行是否存在非已存储或空的货位");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) > 0;
        }
        #endregion

        public TaskHeaderModel GetTask(string taskId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "t_id", taskId }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_task
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and t_id = @t_id
   and act_status = '1'
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询任务表");

            return DBHelper.ConvertTableToObject<TaskHeaderModel>(dt)[0];
        }

        public DataRow[] GetByOrderNoOrID(string orderNoOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ref_doc_type", DocType.DocType_Wave.ToString() },
                { "ref_doc_no", orderNoOrID }
            };
            string sql;
            sql = @"
select t.*, wh.t_id wave_h_id, wd.t_id wave_d_id, oh.t_id outbound_h_id, od.t_id outbound_d_id
  from &DataSource&wms_b_task t
  left join &DataSource&wms_b_wave_h wh on t.ref_doc_id = wh.t_id
  left join &DataSource&wms_b_wave_d wd on t.ref_doc_d_id = wd.t_id
  left join &DataSource&wms_d_outbound_h oh on wd.outbound_h_id = oh.t_id
  left join &DataSource&wms_d_outbound_d od on wd.outbound_d_id = od.t_id
 where t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&'
   and t.warehouse_id = @warehouse
   and t.ref_doc_type = @ref_doc_type
   and (oh.doc_no = @ref_doc_no or oh.t_id = @ref_doc_no)
   and t.act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询任务表", false);

            return dt.AsEnumerable().ToArray();
        }


        public DataRow[] GetByRefDocNoOrID(string refDocType, string refDocNoOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ref_doc_type", refDocType },
                { "ref_doc_no", refDocNoOrID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_task
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and ref_doc_type = @ref_doc_type
   and (ref_doc_no = @ref_doc_no or ref_doc_id = @ref_doc_no)
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询任务表", false);

            return dt.AsEnumerable().ToArray();
        }
        public DataRow[] GetByEquipmentID(string equipIds)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ids", equipIds },
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_task
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and (instr(@ids, t_id) > 0 or instr(@ids, equipment_code) > 0) 
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询任务表", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetByRefDocID(string refDocID)
        {
            DataTable dt = GetTableByRefDocID(refDocID);

            return dt.AsEnumerable().ToArray();
        }

        public DataTable GetTableByRefDocID(string refDocID)
        {

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ref_doc_id", refDocID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_task_v
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and ref_doc_id = @ref_doc_id
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询任务表", false);

            return dt;
        }

        public DataRow[] GetByRefDocDetailID(string refDocDetailID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ref_doc_d_id", refDocDetailID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_task
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and ref_doc_d_id = @ref_doc_d_id
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询任务表", false);

            return dt.AsEnumerable().ToArray();
        }
        public DataRow[] GetBySrcAddr(string srcAddrType, string srcAddrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "src_addr_type", srcAddrType },
                { "src_addr_id", srcAddrID },
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_task
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and src_addr_type = @src_addr_type
   and src_addr_id = @src_addr_id
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询任务表", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetByDestAddr(string destAddrType, string destAddrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "dest_addr_type", destAddrType },
                { "dest_addr_id", destAddrID },
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_task
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and dest_addr_type = @dest_addr_type
   and dest_addr_id = @dest_addr_id
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, $"通过目标地址{destAddrID}查询任务表", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetByContainer(string containerBarcode, string taskType = "N/A")
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container_barcode", containerBarcode },
                { "task_type", taskType },
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_task_v
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and container_barcode = @container_barcode
   and (task_type = @task_type or @task_type = 'N/A')
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询任务表", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetBy(string refDocID, string srcAddrType, string srcAddrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ref_doc_id", refDocID },
                { "src_addr_type", srcAddrType },
                { "src_addr_id", srcAddrID },
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_task
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and ref_doc_id = @ref_doc_id
   and src_addr_type = @src_addr_type
   and src_addr_id = @src_addr_id
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询任务表", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetFinishedTaskByRefDocNo(string docNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ref_doc_no", docNo },
            };
            string sql;
            sql = "select * from isa_m_task_his " +
                  " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                  "    and warehouse_id = @warehouse " +
                  "    and act_status = '1' " +
                  "    and ref_doc_no = @ref_doc_no ";

            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询工作任务", false);

            return dt.AsEnumerable().ToArray();
        }


        public DataRow GetMainDockByTasks(string docNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "source_doc_no", docNo },
            };
            string sql = @"
select d.t_id, sum(t.operation_qty) qty from isa_m_task t
  left join isa_bi_dock d on t.isa_bi_storage_zone_id = d.isa_bi_storage_zone_id
 where t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&'
   and t.warehouse_id = @warehouse
   and t.source_doc_no = @source_doc_no
   and act_status = '1'
 group by d.t_id
 order by qty
";
            DataTable dt = DBHelper.Select(sql, param, "根据工作任务查找月台");

            return dt.Rows[0];
        }

        public void UpdateExecutedBy(string id, string executedBy)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "executed_by", executedBy },
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_b_task
   set modify_date = now(),
       last_modified_by = @last_modified_by,
       modify_ip = @modify_ip,
       executed_by = @executed_by
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id
";
            DBHelper.RunSql(sql, param, "更新任务执行人");
        }

        public void UpdateStartDate(string id, DateTime startDate)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", id }
            };
            string sql = @"update &DataSource&wms_b_task
                            set modify_date = sysdate,
                               last_modified_by = @last_modified_by,
                               modify_ip = @modify_ip,
                               start_date = nvl(start_date,sysdate)
                            where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
                              and warehouse_id = @warehouse
                              and t_id = @t_id
                        ";
            DBHelper.RunSql(sql, param, "更新任务开始时间");
        }

        public void UpdateFinishDate(string id, DateTime finishDate)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", id }
            };
            string sql = @"update &DataSource&wms_b_task
                            set modify_date = sysdate,
                               last_modified_by = @last_modified_by,
                               modify_ip = @modify_ip,
                               end_date = sysdate
                            where t_id = @t_id
                        ";
            DBHelper.RunSql(sql, param, "更新任务开始时间");
        }

        public void UpdateTaskItemStartDate(string id, DateTime startDate)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "start_date", startDate.ToString() },
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_b_task_item
   set modify_date = now(),
       last_modified_by = @last_modified_by,
       modify_ip = @modify_ip,
       start_date = date_format(@start_date, '%Y-%m-%d %H:%i:%s')
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id
";
            DBHelper.RunSql(sql, param, "更新任务开始时间");
        }

        public void UpdateTaskItemEndDate(string id, DateTime finishDate)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "end_date", finishDate.ToString() },
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_b_task_item
   set modify_date = now(),
       last_modified_by = @last_modified_by,
       modify_ip = @modify_ip,
       end_date = date_format(@end_date, '%Y-%m-%d %H:%i:%s')
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id
";
            DBHelper.RunSql(sql, param, "更新任务结束时间");
        }

        /// <summary>
        /// 根据任务ID删除任务
        /// </summary>
        /// <param name="taskid">任务ID</param>
        /// <returns></returns>
        public bool DeleteTaskById(string taskid)
        {
            string sql = "delete from  &DataSource&wms_b_task where t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", taskid);
            return DBHelper.RunSql(sql, lstParam) > 0;
        }
        /// <summary>
        /// 根据任务行ID删除任务
        /// </summary>
        /// <param name="taskitemid"></param>
        /// <returns></returns>
        public bool DeleteTaskitemById(string taskitemid)
        {
            string sql = "delete from  &DataSource&wms_b_task_item where t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", taskitemid);
            return DBHelper.RunSql(sql, lstParam) > 0;
        }

        /// <summary>
        /// 根据任务ID删除任务物料表数据
        /// </summary>
        /// <param name="taskitemid">任务id</param>
        /// <returns></returns>
        public bool DeleteTaskItemByTaskId(string taskId)
        {
            string sql = "delete from  &DataSource&wms_b_task_item where task_id = @taskId";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("taskId", taskId);
            return DBHelper.RunSql(sql, lstParam) > 0;
        }
        /// <summary>
        /// 更新任务有效状态
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="actStatus">更新状态</param>
        /// <param name="oldActStatus">原状态</param>
        /// <returns></returns>
        public IWorkTask UpdateActStatus(string id, string actStatus, string oldActStatus = null)
        {
            oldActStatus ??= " ";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "state", actStatus},
                { "old_state", oldActStatus},
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_b_task
   set modify_date = now(),
       last_modified_by = @last_modified_by,
       act_status = @state
 where t_id = @t_id
   and (act_status = @old_state or @old_state = ' ')
";
            DBHelper.RunSql(sql, param, "更新任务状态");
            return this;
        }

        /// <summary>
        /// 修改任务物料表中的软删除标记
        /// </summary>
        /// <param name="id">任务明细ID:任务物料表t_id</param>
        /// <param name="actStatus">软删除标记</param>
        /// <returns></returns>
        public IWorkTask UpdateTaskItemActStatus(string id, string actStatus)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "state", actStatus},
                { "t_id", id }
            };
            string sql = $"update &DataSource&wms_b_task_item  set modify_date = sysdate, last_modified_by = @last_modified_by, act_status = @state where t_id = @t_id";
            DBHelper.RunSql(sql, param, "更新任务物料状态");
            return this;
        }
        /// <summary>
        /// 更新任务明细是否有效状态
        /// </summary>
        /// <param name="taskid">任务ID</param>
        /// <param name="actStatus">有效标识:1有效，0无效</param>
        /// <returns></returns>
        public IWorkTask UpdateTaskItemActStatusByTask(string taskId, string actStatus)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "state", actStatus},
                { "task_id", taskId }
            };
            string sql = @"update &DataSource&wms_b_task_item  
                            set modify_date = sysdate, 
                            last_modified_by = @last_modified_by, 
                            act_status = @state 
                        where task_id = @task_id";
            DBHelper.RunSql(sql, param, "更新任务物料状态");
            return this;
        }

        public IWorkTask UpdateIsExecutable(string id, string isExecutable, string oldIsExecutable = null)
        {
            if (oldIsExecutable == null)
            {
                oldIsExecutable = " ";
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "is_executable", isExecutable },
                { "old_is_executable", oldIsExecutable },
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_b_task
   set modify_date = now(),
       last_modified_by = @last_modified_by,
       modify_ip = @modify_ip,
       is_executable = @is_executable
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id
   and (@old_is_executable = ' '
        or is_executable = @old_is_executable)
";
            DBHelper.RunSql(sql, param, "更新任务是否可执行");

            return this;
        }

        public IWorkTask UpdateTaskItemStatus(string id, string status, string oldStatus = null)
        {
            if (oldStatus == null)
            {
                oldStatus = " ";
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "status", status },
                { "old_status", oldStatus },
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_b_task_item
   set modify_date = now(),
       last_modified_by = @last_modified_by,
       modify_ip = @modify_ip,
       process_status = @status
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id
   and (@old_status = ' '
        or process_status = @old_status)
";
            DBHelper.RunSql(sql, param, "更新任务明细状态");

            return this;
        }

        public IWorkTask UpdateTaskStatus(string id, string taskStatus, string oldTaskStatus = null)
        {
            if (oldTaskStatus == null)
            {
                oldTaskStatus = " ";
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "task_status", taskStatus },
                { "old_task_status", oldTaskStatus },
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_b_task
   set modify_date = now(),
       last_modified_by = @last_modified_by,
       modify_ip = @modify_ip,
       task_status = @task_status
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id
   and (@old_task_status = ' '
        or task_status = @old_task_status)
";
            DBHelper.RunSql(sql, param, "更新任务状态");

            return this;
        }

        public IWorkTask UpdateOperationQty(string id, decimal operationQty, decimal? oldOperationQty = null)
        {
            if (oldOperationQty == null)
            {
                oldOperationQty = decimal.Parse("0" + Get(id)["operation_qty"]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_edited_by", Context.UserInfo.UserName },
                { "operation_qty", operationQty.ToString()},
                { "old_operation_qty", oldOperationQty.ToString()},
                { "t_id", id }
            };
            string sql = @"
update isa_m_task
   set last_edited_dt = now(),
       last_edited_by = @last_edited_by,
       operation_qty = @operation_qty
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id
   and operation_qty = @old_operation_qty
";
            DBHelper.RunSql(sql, param, "更新任务表数量");
            return this;
        }

        public IWorkTask UpdateFinanceQty(string id, decimal financeQty, decimal? oldFinanceQty = null)
        {
            if (oldFinanceQty == null)
            {
                oldFinanceQty = decimal.Parse("0" + Get(id)["finance_qty"]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_edited_by", Context.UserInfo.UserName },
                { "finance_qty", financeQty.ToString()},
                { "old_finance_qty", oldFinanceQty.ToString()},
                { "t_id", id }
            };
            string sql = @"
update isa_m_task
   set last_edited_dt = now(),
       last_edited_by = @last_edited_by,
       finance_qty = @finance_qty
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id
   and finance_qty = @old_finance_qty
";
            DBHelper.RunSql(sql, param, "更新任务表重量");
            return this;
        }

        public IWorkTask AddFinishedQty(string id, decimal qty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + Get(id)[ti.FinishedQty]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "finished_qty", qty.ToString()},
                { "old_qty", oldQty.ToString()},
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_b_task_item
   set modify_date = now(),
       last_modified_by = @last_modified_by,
       finished_qty = ifnull(finished_qty, 0) + @finished_qty
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id
   and ifnull(finished_qty, 0) = @old_qty
";
            DBHelper.RunSql(sql, param, "计算完成数量");
            return this;
        }

        public IWorkTask UpdatePickQty(string id, decimal pickQty, decimal? oldPickQty = null)
        {
            if (oldPickQty == null)
            {
                oldPickQty = decimal.Parse("0" + Get(id)["pick_qty"]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_edited_by", Context.UserInfo.UserName },
                { "pick_qty", pickQty.ToString()},
                { "old_pick_qty", oldPickQty.ToString()},
                { "t_id", id }
            };
            string sql = @"
update isa_m_task
   set last_edited_dt = now(),
       last_edited_by = @last_edited_by,
       pick_qty = @pick_qty
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id
   and ifnull(pick_qty, 0) = @old_pick_qty
";
            DBHelper.RunSql(sql, param, "更新数量");
            return this;
        }

        public IWorkTask UpdateSource(string taskID, string sourceType, string sourceID, string sourceText)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", taskID },
                { "src_addr_type", sourceType },
                { "src_addr_id", sourceID },
                { "src_addr_text", sourceText },
            };
            string sql = @"update &DataSource&wms_b_task set
                              src_addr_id = @src_addr_id,
                              src_addr_type = @src_addr_type,
                              src_addr_text = @src_addr_text
                          where client_id = '&CLIENT_ID&'  and warehouse_id = @warehouse  and t_id = @t_id";
            DBHelper.RunSql(sql, param, "修改任务表源位置");

            return this;
        }

        public IWorkTask UpdateTarget(string taskID, string targetType, string targetID, string targetText)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", taskID },
                { "target_type", targetType },
                { "target_id", targetID },
                { "target_text", targetText },
            };
            string sql = @"update &DataSource&wms_b_task set DEST_ADDR_ID = @target_id,
                          DEST_ADDR_TYPE = @target_type,
                          DEST_ADDR_TEXT = @target_text
                          where client_id='&CLIENT_ID&'  and warehouse_id = @warehouse  and t_id = @t_id ";
            DBHelper.RunSql(sql, param, "修改任务表目标位置");
            return this;
        }

        public IWorkTask UpdateStorageZoneID(string id, string storageZoneID, string oldStorageZoneID = null)
        {
            if (oldStorageZoneID == null)
            {
                oldStorageZoneID = Get(id)["isa_bi_storage_zone_id"].ToString();
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_edited_by", Context.UserInfo.UserName },
                { "isa_bi_storage_zone_id", storageZoneID},
                { "old_isa_bi_storage_zone_id", oldStorageZoneID},
                { "t_id", id }
            };
            string sql = @"
update isa_m_task
   set last_edited_dt = now(),
       last_edited_by = @last_edited_by,
       isa_bi_storage_zone_id = @isa_bi_storage_zone_id
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id
   and isa_bi_storage_zone_id = @old_isa_bi_storage_zone_id
";
            DBHelper.RunSql(sql, param, "更新库区ID");
            return this;
        }

        public IWorkTask UpdateContainerDetailID(string id,
            string containerDetailID, string oldContainerDetailID = null)
        {
            if (oldContainerDetailID == null)
            {
                oldContainerDetailID = Get(id)["isa_bi_container_detail_id"].ToString();
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_edited_by", Context.UserInfo.UserName },
                { "detail_id", containerDetailID },
                { "old_detail_id", oldContainerDetailID },
                { "t_id", id }
            };
            string sql = @"
update isa_m_task
   set last_edited_dt = now(),
       last_edited_by = @last_edited_by,
       isa_bi_container_detail_id= @detail_id
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id
   and isa_bi_container_detail_id = @old_detail_id
";
            DBHelper.RunSql(sql, param, "更新容器明细标识");
            return this;
        }

        public IWorkTask UpdateContainerBarcode(string id,
            string containerBarcode, string oldContainerBarcode = null)
        {
            if (oldContainerBarcode == null)
            {
                oldContainerBarcode = Get(id)["container_barcode"].ToString();
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_edited_by", Context.UserInfo.UserName },
                { "barcode", containerBarcode },
                { "old_barcode", oldContainerBarcode },
                { "t_id", id }
            };
            string sql = @"
update isa_m_task
   set last_edited_dt = now(),
       last_edited_by = @last_edited_by,
       container_barcode = @barcode
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id
   and container_barcode = @old_barcode
";
            DBHelper.RunSql(sql, param, "更新容器条码");
            return this;
        }

        public DataRow GetWorkTask(string taskNoOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", taskNoOrID },
            };
            string sql;
            sql = "select * from &DataSource&wms_b_task " +
                  " where act_status = '1' " +
                  "    and (@t_id = t_id or @t_id = task_no) ";

            DataTable dt = DBHelper.Select(sql, param, "查询工作任务");

            return dt.Rows[0];
        }

        public DataRow[] GetDependentTasks(string taskNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "task_no", taskNo },
            };
            string sql;
            sql = "select * from &DataSource&wms_b_task " +
                  " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                  "    and warehouse_id = @warehouse " +
                  "    and task_status != 'TaskStatus_Create' " +
                  "    and act_status = '1' " +
                  "    and @task_no = dependent_task_no ";

            DataTable dt = DBHelper.Select(sql, param, "查询依赖工作任务", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetWorkTasks(string taskNoOrIds)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", taskNoOrIds },
            };
            string sql;
            sql = "select * from &DataSource&wms_b_task " +
                  " where act_status = '1' " +
                  "    and (instr(@t_id, t_id) > 0 or instr(@t_id, task_no) > 0) ";

            DataTable dt = DBHelper.Select(sql, param, "查询工作任务");

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetWorkTasksWithProcess(string taskNoOrIds)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", taskNoOrIds },
            };
            string sql;
            sql = @" select t.*,
                        (select sum(ti.processed_qty) from &DataSource&wms_b_task_item ti where ti.task_id=t.t_id) as processed_qty,
                        (select sum(ti.finished_qty) from &DataSource&wms_b_task_item ti where ti.task_id=t.t_id) as finished_qty
                     from &DataSource&wms_b_task  t
                     where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
                        and warehouse_id = @warehouse
                        and act_status = '1'
                        and (instr(@t_id, t_id) > 0 or instr(@t_id, task_no) > 0)";

            DataTable dt = DBHelper.Select(sql, param, "查询工作任务");

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetTaskItems(string taskItemIds)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", taskItemIds },
            };
            string sql;
            sql = "select * from &DataSource&wms_b_task_item " +
                  " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                  "    and warehouse_id = @warehouse " +
                  "    and act_status = '1' " +
                  "    and instr(@t_id, t_id) > 0 ";

            DataTable dt = DBHelper.Select(sql, param, "查询任务明细");

            return dt.AsEnumerable().ToArray();
        }
        public DataRow[] GetTaskItemsBy(string taskID, string actStatus = "1", bool errorWhenNotFound = true)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "task_id", taskID },
                { "act_status", actStatus },
            };
            string sql;
            sql = "select * from &DataSource&wms_b_task_item_v " +
                  " where act_status = @act_status " +
                  "    and task_id = @task_id ";

            DataTable dt = DBHelper.Select(sql, param, "查询任务明细", errorWhenNotFound);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow GetTaskItemBy(string taskID, string itemID, string refDocDetailID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "task_id", taskID },
                { "item_id", itemID },
                { "ref_doc_d_id", refDocDetailID },
            };
            string sql;
            sql = "select * from &DataSource&wms_b_task_item " +
                  " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                  "    and warehouse_id = @warehouse " +
                  "    and act_status = '1' " +
                  "    and task_id = @task_id " +
                  "    and item_id = @item_id " +
                  "    and ref_doc_d_id = @ref_doc_d_id ";

            DataTable dt = DBHelper.Select(sql, param, "查询任务明细");

            return dt.Rows[0];
        }


        /// <summary>
        /// 查询待执行的任务清单
        /// </summary>
        /// <param name="searchinfo"></param>
        /// <returns></returns>
        public DataRow[] GetWorkTasks(NtiCloud.Model.WMS.TaskdoingIn searchinfo)
        {
            string sql = @"select t_id, client_id, org_id, warehouse_id, zone_id, task_no, dependent_task_no, task_type, executed_by, equipment_id, executor_type, actual_equipment_id, seq_no, priority, container_id, container_barcode, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, src_addr_id, src_addr_type, src_addr_text, dest_addr_id, dest_addr_type, dest_addr_text, actual_dest_addr_id, actual_dest_addr_text, task_status, start_date, end_date, is_executable, priority_change_time, led_text, exist_lgv, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status, totalplanqty, totalprocessedqty, totalfinishedqty
            from &DataSource&v_taskfordoing where 1 = 1 ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(searchinfo.RefDocno))
            {
                sql += " and ref_doc_no = @ref_doc_no";
                lstParam.Add("ref_doc_no", searchinfo.RefDocno);
            }
            if (!string.IsNullOrEmpty(searchinfo.LocationCode))
            {
                sql += " and ref_doc_no = @ref_doc_no";
                lstParam.Add("dest_addr_text", searchinfo.LocationCode);
            }

            sql += " order by ref_doc_no,v_taskfordoingdest_addr_text ";
            return DBHelper.Select(sql, lstParam, "查询待执行任务清单", false).AsEnumerable().ToArray();

        }
        public IWorkTask InsertHistory(string taskId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", taskId }
            };

            string sql = @"
                        insert into &DataSource&wms_b_task_his
                        select *
                          from  &DataSource&wms_b_task
                         where t_id = @t_id";

            DBHelper.RunSql(sql, param, "插入任务历史表");
            return this;
        }

        public IWorkTask TaskItemInsertHistory(string taskId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", taskId }
            };

            string sql = @"
                        insert into &DataSource&wms_b_task_item_his
                        select *
                          from  &DataSource&wms_b_task_item
                         where task_id = @t_id";

            DBHelper.RunSql(sql, param, "插入任务物料历史表", DBHelper.OperatedNumType.OneOrMore);
            return this;
        }

        /// <summary>
        /// 将任务明细插入历史表
        /// </summary>
        /// <param name="taskItemId"></param>
        /// <returns></returns>
        public IWorkTask TaskItemInsertHistoryByItemId(string taskItemId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", taskItemId }
            };

            string sql = @"
                        insert into &DataSource&wms_b_task_item_his
                        select *
                          from  &DataSource&wms_b_task_item
                         where warehouse_id = @warehouse
                           and t_id = @t_id";

            DBHelper.RunSql(sql, param, "插入任务物料历史表", DBHelper.OperatedNumType.OnlyOne);
            return this;
        }

        public IWorkTask InsertTask(DataRow waveTaskRow, string id = null)
        {
            throw new NotImplementedException();
#pragma warning disable CS0162 // 检测到无法访问的代码
            DataRow waveHeadRow = Wave.GetWaveHead(waveTaskRow["wave_h_id"].ToString());
#pragma warning restore CS0162 // 检测到无法访问的代码

            DataRow[] orderHeadRow = OutboundOrder.
                GetOrderHeads(waveTaskRow["outbound_h_id"].ToString());

            if (id == null)
            {
                id = SnowflakeID.Guid().ToString();
            }

            Insert(new WorkTaskModel()
            {
                ID = id,
                TaskType = waveTaskRow["task_type_code"].ToString(),
                //TaskExeTypeCode = waveTaskRow["task_exec_type_code"].ToString(),
                TaskNo = waveTaskRow["task_no"].ToString(),
                ActualEquipmentID = waveTaskRow["equipment_id"].ToString(),
                //GroupNo = waveTaskRow["group_no"].ToString(),
                //SequenceNo = waveTaskRow["seq_no"].ToString(),
                SeqNo = decimal.Parse("0" + (waveTaskRow["seq_no"])),
                Priority = DBHelper.GetDecimal(waveTaskRow["priority"]),
                //ContainerID = waveTaskRow["container_id"].ToString(),
                //ContainerDetailID = waveTaskRow["container_id"].ToString(),
                ContainerBarcode = waveTaskRow["container_barcode"].ToString(),
                //TODO ItemID = waveTaskRow["item_id"].ToString(),
                //TODO OperationQty = DBHelper.GetDecimal(waveTaskRow["pallet_qty"]),
                //TODO PickQty = DBHelper.GetDecimal(waveTaskRow["operation_qty"]),
                //BatchNo = waveTaskRow["batch_no"].ToString(),
                //BomCode = waveTaskRow["bom_code"].ToString(),
                SrcAddrID = waveTaskRow["source_id"].ToString(),
                RefDocType = "Wave",
                SrcAddrType = waveTaskRow["source_type"].ToString(),
                SrcAddrText = waveTaskRow["source_text"].ToString(),
                DestAddrID = waveTaskRow["target_id"].ToString(),
                DestAddrType = waveTaskRow["target_type"].ToString(),
                DestAddrText = waveTaskRow["target_text"].ToString(),
                RefDocID = waveTaskRow["wave_d_id"].ToString(),
                RefDocNo = waveHeadRow["wave_no"].ToString(),
                //SourceBillDetailID = waveTaskRow["order_d_id"].ToString(),
                //SourceBillDetailNo = orderHeadRow[0]["order_no"].ToString(),
                TaskStatus = "TaskStatus_NotExecute",
                IsExecutable = waveTaskRow["executable"].ToString(),
                //DependenceTaskNo = waveTaskRow["dependence_task_no"].ToString(),
                //LedText = waveTaskRow["led_text"].ToString(),
            });

            return this;
        }

        public IWorkTask InsertFinishedTask(DataRow row)
        {
            throw new NotImplementedException();
        }
        private string FormateDateStr(DateTime? dt)
        {
            if (dt == null)
            {
                return "";
            }
            else
            {
                return ((DateTime)dt).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public WebJsonInfo AddTask(TaskHeaderModel task)
        {
            WebJsonInfo re = new WebJsonInfo();

            if (string.IsNullOrEmpty(task.t_id))
            {
                task.t_id = Guid.NewGuid().ToString();
            }
            string sql = @"insert into &DataSource&wms_b_task
                    (t_id, client_id, org_id, warehouse_id, zone_id, task_no, dependent_task_no, task_type, executed_by, equipment_id, executor_type, actual_equipment_id, seq_no, priority, container_id, container_barcode, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, src_addr_id, src_addr_type, src_addr_text, dest_addr_id, dest_addr_type, dest_addr_text, actual_dest_addr_id, actual_dest_addr_text, task_status, start_date, end_date, is_executable, priority_change_time, led_text, exist_lgv, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status)
                    values
                    (@t_id, @client_id, @org_id, @warehouse_id, @zone_id" +
                    ", @task_no , @dependent_task_no, @task_type, @executed_by" +
                    ", @equipment_id, @executor_type, @actual_equipment_id, @seq_no" +
                    ", @priority, @container_id, @container_barcode, @ref_doc_type " +
                    ", @ref_doc_id, @ref_doc_d_id , @ref_doc_no , @src_addr_id " +
                    ", @src_addr_type, @src_addr_text , @dest_addr_id, @dest_addr_type " +
                    ", @dest_addr_text, @actual_dest_addr_id , @actual_dest_addr_text, @task_status" +
                    ", to_date( @start_date ,'yyyy-MM-dd HH24:mi:ss')" +
                    ", to_date( @end_date,'yyyy-MM-dd HH24:mi:ss')" +
                    " , @is_executable," +
                    " to_date( @priority_change_time ,'yyyy-MM-dd HH24:mi:ss')" +
                    ",@led_text , @exist_lgv, sysdate" +
                    ", @created_by, @modify_ip, sysdate, @last_modified_by, '1')";
            Dictionary<string, object> lstParm = new Dictionary<string, object>();
            lstParm.Add("t_id", task.t_id);
            lstParm.Add("client_id", task.client_id);
            lstParm.Add("org_id", task.org_id);
            lstParm.Add("warehouse_id", task.warehouse_id);
            lstParm.Add("zone_id", task.zone_id);
            lstParm.Add("task_no", task.task_no);
            lstParm.Add("dependent_task_no", task.dependent_task_no);
            lstParm.Add("task_type", task.task_type);
            lstParm.Add("executed_by", task.executed_by);
            lstParm.Add("equipment_id", task.equipment_id);
            lstParm.Add("executor_type", task.executor_type);
            lstParm.Add("actual_equipment_id", task.actual_equipment_id);
            lstParm.Add("seq_no", task.seq_no.ToString());
            lstParm.Add("priority", task.priority.ToString());
            lstParm.Add("container_id", task.container_id);
            lstParm.Add("container_barcode", task.container_barcode);
            lstParm.Add("ref_doc_type", task.ref_doc_type);
            lstParm.Add("ref_doc_id", task.ref_doc_id);
            lstParm.Add("ref_doc_d_id", task.ref_doc_d_id);
            lstParm.Add("ref_doc_no", task.ref_doc_no);
            lstParm.Add("src_addr_id", task.src_addr_id);
            lstParm.Add("src_addr_type", task.src_addr_type);
            lstParm.Add("src_addr_text", task.src_addr_text);
            lstParm.Add("dest_addr_id", task.dest_addr_id);
            lstParm.Add("dest_addr_type", task.dest_addr_type);
            lstParm.Add("dest_addr_text", task.dest_addr_text);
            lstParm.Add("actual_dest_addr_id", task.actual_dest_addr_id);
            lstParm.Add("actual_dest_addr_text", task.actual_dest_addr_text);
            lstParm.Add("task_status", task.task_status);
            lstParm.Add("start_date", FormateDateStr(task.start_date));
            lstParm.Add("end_date", FormateDateStr(task.end_date));
            lstParm.Add("is_executable", task.is_executable);
            lstParm.Add("priority_change_time", FormateDateStr(task.priority_change_time));
            lstParm.Add("led_text", task.led_text);
            lstParm.Add("exist_lgv", task.exist_lgv);
            lstParm.Add("created_by", task.created_by);
            lstParm.Add("modify_ip", task.modify_ip);
            lstParm.Add("last_modified_by", task.last_modified_by);

            int rows = DBHelper.RunSql(sql, lstParm, "新增任务头");
            re.isSucceed = rows > 0;
            re.json = task.t_id;
            return re;

        }




        public WebJsonInfo AddTaskItem(TaskItemModel taskitem)
        {

            WebJsonInfo re = new WebJsonInfo();

            if (string.IsNullOrEmpty(taskitem.t_id))
            {
                taskitem.t_id = Guid.NewGuid().ToString();
            }
            string sql = @"insert into &DataSource&wms_b_task_item
  (t_id, client_id, org_id, warehouse_id, task_id, item_id, item_name, plan_qty, processed_qty, finished_qty, uom_qty_id, finished_stat_qty, uom_stat_id, container_d_id, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, start_date, end_date, process_status, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status)
values
  ( @t_id, @client_id, @org_id, @warehouse_id, @task_id" +
  ", @item_id, @item_name, @plan_qty, @processed_qty" +
  ", @finished_qty, @uom_qty_id, @finished_stat_qty, @uom_stat_id" +
  ", @container_d_id, @ref_doc_type, @ref_doc_id, @ref_doc_d_id" +
  ", @ref_doc_no" +
  ", to_date(@start_date,'yyyy-MM-dd HH24:mi:ss')" +
  ", to_date( @end_date ,'yyyy-MM-dd HH24:mi:ss')" +
  ", @process_status, sysdate,  @created_by,  @modify_ip, sysdate,  @last_modified_by, '1')";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", taskitem.t_id);
            lstParam.Add("client_id", taskitem.client_id);
            lstParam.Add("org_id", taskitem.org_id);
            lstParam.Add("warehouse_id", taskitem.warehouse_id);
            lstParam.Add("task_id", taskitem.task_id);
            lstParam.Add("item_id", taskitem.item_id);
            lstParam.Add("item_name", taskitem.item_name);
            lstParam.Add("plan_qty", taskitem.plan_qty.ToString());
            lstParam.Add("processed_qty", taskitem.processed_qty.ToString());
            lstParam.Add("finished_qty", taskitem.finished_qty.ToString());
            lstParam.Add("uom_qty_id", taskitem.uom_qty_id);
            lstParam.Add("finished_stat_qty", taskitem.finished_stat_qty.ToString());
            lstParam.Add("uom_stat_id", taskitem.uom_stat_id);
            lstParam.Add("container_d_id", taskitem.container_d_id);
            lstParam.Add("ref_doc_type", taskitem.ref_doc_type);
            lstParam.Add("ref_doc_id", taskitem.ref_doc_id);
            lstParam.Add("ref_doc_d_id", taskitem.ref_doc_d_id);
            lstParam.Add("ref_doc_no", taskitem.ref_doc_no);
            lstParam.Add("start_date", FormateDateStr(taskitem.start_date));
            lstParam.Add("end_date", FormateDateStr(taskitem.end_date));
            lstParam.Add("process_status", taskitem.process_status);
            lstParam.Add("created_by", taskitem.created_by);
            lstParam.Add("modify_ip", taskitem.modify_ip);
            lstParam.Add("last_modified_by", taskitem.last_modified_by);

            int rows = DBHelper.RunSql(sql, lstParam, "新增任务明细");

            re.isSucceed = rows > 0;
            re.json = taskitem.t_id;
            return re;
        }

        #endregion

        #region Business Logic
        /// <summary>
        /// 任务强制执行中
        /// </summary>
        /// <param name="taskID">任务ID列表</param>
        /// <returns></returns>
        public IWorkTask ForceTaskExecuting(string warehouseID, string taskID)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 任务强制子还行中
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public bool ForceTaskExecuting(string taskID)
        {
            string sql = @"update &DataSource&wms_b_task t set t.task_status=@task_status,
                                      t.modify_ip=@modify_ip,
                                      t.modify_date=sysdate,
                                      t.last_modified_by=@last_modified_by
                    where t.t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("task_status", TaskStatus.TaskStatus_Executing.ToString());
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("t_id", taskID);
            return DBHelper.RunSql(sql, lstParam, "任务强制执行中") > 0;
        }
        /// <summary>
        /// 强制未执行
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public bool ForceTaskNotExecute(string taskID)
        {
            string recycleinfo = RecycleWcsTask(taskID);
            if (!string.IsNullOrEmpty(recycleinfo))
            {
                throw new Exception("回收WCS任务失败:" + recycleinfo);
            }
            string sql = @"update &DataSource&wms_b_task t set t.task_status=@task_status,
                                      t.modify_ip=@modify_ip,
                                      t.modify_date=sysdate,
                                      t.last_modified_by=@last_modified_by
                    where t.t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("task_status", TaskStatus.TaskStatus_NotExecute.ToString());
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("t_id", taskID);
            return DBHelper.RunSql(sql, lstParam, "任务强制执行中") > 0;
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="taskNoOrIds"></param>
        /// <returns></returns>
        public IWorkTask FinishWorkTasks(string taskNoOrIds)
        {
            DataRow[] taskRows = GetWorkTasks(taskNoOrIds);
            taskRows = taskRows.OrderBy(r => r[t.ID].ToString()).ToArray();

            foreach (DataRow task in taskRows)
            {
                ForceTaskFinished(Context.WarehouseID, task["t_id"].ToString());
            }
            return this;
        }

        public IWorkTask FinishOneTask(string taskNoOrID, decimal qty)
        {
            DataRow taskRow = GetWorkTask(taskNoOrID);

            ForceTaskFinished(Context.WarehouseID, taskRow["t_id"].ToString(), qty);

            return this;
        }
        /// <summary>
        /// 将完成的任务插入历史表（包括任务物料明细）
        /// </summary>
        /// <param name="taskId">任务id</param>
        /// <returns></returns>
        public IWorkTask MoveToHistory(string taskId)
        {
            InsertHistory(taskId);
            TaskItemInsertHistory(taskId);
            DeleteWorkTask(taskId);
            DeleteWorkTaskItem(taskId);
            return this;
        }

        /// <summary>
        /// 任务完成后，将任务明细转入历史表
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="taskitemid"></param>
        /// <returns></returns>
        public IWorkTask MoveTaskItemToHistory(string taskid, string taskitemid)
        {
            TaskItemInsertHistoryByItemId(taskitemid);
            DeleteWorkTaskItemByItemId(taskitemid);
            if (getTaskItemsForDoing(taskid).Count == 0)
            {
                InsertHistory(taskid);
                DeleteWorkTask(taskid);
            }
            return this;
        }
        private void CheckDependentTask(DataRow taskRow)
        {
            DataRow row = null;
            string dependentTask = taskRow["dependent_task_no"].ToString();
            string task_no = taskRow["task_no"].ToString();//任务号
            string RefDocType = taskRow["REF_DOC_TYPE"].ToString();//来源单据类型
            if (string.IsNullOrWhiteSpace(dependentTask))
            {
                return;
            }
            try
            {
                row = Get(dependentTask);
            }
            catch (WmsException ex)
            {
                if (ex.ErrorCode != DBHelper.NotFound)
                {
                    throw;
                }
                return;
            }
            if (row != null && row["task_status"].ToString() != "TaskStatus_Finished")
            {
                //恩施使用下面逻辑
                if (string.Equals(RefDocType, DocType.DocType_CallTerminal.ToString()))
                {
                    //退空盘+要料,要料依赖于退空盘；退余料+要料,要料依赖于退余料
                    //此处设置因为，现场退空盘+要料，出现要料的agv先到达目的地，而退空盘的agv，并未到达目的地，未完成任务
                    //此时会出现，要料任务，需要在退空盘任务完成前完成。而之前设定的业务是不允许要料在退空盘任务完成前完成的。
                    //为了解决这个问题，设置了如下方法
                    //要料任务需要完成时，如果退空盘任务没有完成，则使用一个字段保存特定数据。在退空盘任务完成时，读取要料任务的这个字段，如果该字段保存的是这个特定的数据，则需要调用完成要料的任务。
                    UpdateLedText(task_no, "55555555");
                }

                throw new WmsException(308, $"前序任务{dependentTask}还未完成！");
            }
        }

        public IWorkTask ForceTaskFinished(string warehouseID, string taskID, decimal? qty = null)
        {
            DataRow taskRow = Get(taskID);
            string[] allowTaskStatus = new string[] { TaskStatus.TaskStatus_NotExecute.ToString(), TaskStatus.TaskStatus_Executing.ToString(), TaskStatus.TaskStatus_PartialFinished.ToString() };
            if (!allowTaskStatus.Contains(taskRow[t.TaskStatus].ToString()))
            {
                throw new WmsException(301, "任务状态不正确");
            }

            CheckDependentTask(taskRow);
            TaskType type = (TaskType)Enum.Parse(typeof(TaskType), taskRow["task_type"].ToString());
            WorkTask ww = new WorkTask();
            ww.UpdateFinishDate(taskID, DateTime.Now);
            switch (type)
            {
                case TaskType.TaskType_InBound:
                    FinishInboundTask(taskRow, qty);
                    /*
                    MoveToHistory(taskID);
                    Receipt.FinishReceiptWhenTasksFinished(taskRow["source_doc_no"].ToString());
                    */
                    break;
                case TaskType.TaskType_OutBound:
                    FinishOutboundTask(taskRow, qty);
                    Container container = new Container();
                    container.UpdateContainerSnInLocation(taskRow["container_id"].ToString(),
                        taskRow["dest_addr_text"].ToString(), taskRow["task_no"].ToString(),
                        "出库完成", taskRow["ref_doc_type"].ToString());
                    break;
                case TaskType.TaskType_Replenishment:
                    FinishOutboundTask(taskRow, qty);
                    break;
                case TaskType.TaskType_Picking:
                    //FinishOutboundTask(taskRow, qty);
                    throw new NotImplementedException();
                    break;
                //case "TaskType_Out4StockUp":
                //    FinishStockUpTask(taskRow, qty);
                //    MoveToHistory(taskID);
                //    break;

                case TaskType.TaskType_Move:
                    FinishMoveTask(taskRow, qty);
                    break;

                default:
                    throw new NotImplementedException();
#pragma warning disable CS0162 // 检测到无法访问的代码
                    return this;
#pragma warning restore CS0162 // 检测到无法访问的代码
            }

            return this;
        }

        public bool ExistsFinishedTaskInRefDoc(string srcDocNo)
        {
            DataRow[] rows = GetFinishedTaskByRefDocNo(srcDocNo);

            if (rows.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ExistsUnfinishedTaskInRefDoc(string docType, string docNoOrID)
        {
            DataRow[] taskRows = GetByRefDocNoOrID(docType, docNoOrID);

            List<string> completeStatus = new List<string>()
            {
                TaskStatus.TaskStatus_Finished.ToString(),
            };
            foreach (DataRow task in taskRows)
            {
                if (!completeStatus.Contains(task[t.TaskStatus].ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否存在已经释放但是还没有完成的任务在目标地址
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public bool ExistsUnfinishedTaskAtDestAddress(Address addr)
        {
            DataRow[] taskRows = GetByDestAddr(addr.Type, addr.ID);

            List<string> states = new List<string>()
            {
                TaskStatus.TaskStatus_Create.ToString(),
                TaskStatus.TaskStatus_Finished.ToString(),
            };
            foreach (DataRow task in taskRows)
            {
                if (!states.Contains(task[t.TaskStatus].ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        public void FinishOutboundTask(DataRow taskRow, decimal? qty)
        {
            DocType type = (DocType)Enum.Parse(typeof(DocType), taskRow[t.RefDocType].ToString(), true);
            switch (type)
            {
                case DocType.DocType_Wave:
                    FinishOutboundDocTask(taskRow);
                    break;
                case DocType.DocType_CallTerminal://机台要料
                    new CallEnd.MachineMaterialBusinessProce().FinishMachineMaterialModel(
                        new FinishMachineMaterialModel()
                        {
                            TaskTid = taskRow[t.ID].ToString()
                        });
                    break;
                case DocType.DocType_PersonReceiveMateriel://人工领料
                    new CallEnd.MachineMaterialBusinessProce().FinishPersonReceiveMaterielTask(new FinishPersonReceiveMaterielTaskModel()
                    {
                        TaskTid = taskRow[t.ID].ToString()
                    });
                    break;
                case DocType.DocType_Qc:
                    new QC.QcBound().RunOutQcTask(taskRow[t.ID].ToString(), TaskStatus.TaskStatus_Finished);
                    break;
                case DocType.DocType_Retrieval:
                    Retrieval.Retrieval ret = new Retrieval.Retrieval();
                    ret.RetrievalDownTaskOver(taskRow[t.ID].ToString());
                    break;
                case DocType.DocType_Count:
                    CountContainer.CountContainer countContainer = new CountContainer.CountContainer();
                    countContainer.FinishOutCount(taskRow[t.ID].ToString(), TaskStatus.TaskStatus_Finished);
                    break;
                case DocType.DocType_EmptyContainerOut:
                    CountContainer.CountContainer cou = new CountContainer.CountContainer();
                    cou.FinishOutEmptyContaiener(taskRow[t.ID].ToString(), TaskStatus.TaskStatus_Finished);
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        /// <summary>
        /// 强制完成移库任务
        /// </summary>
        /// <param name="taskRow"></param>
        /// <param name="qty"></param>
        public void FinishMoveTask(DataRow taskRow, decimal? qty)
        {
            DocType type = (DocType)Enum.Parse(typeof(DocType), taskRow[t.RefDocType].ToString(), true);
            switch (type)
            {
                case DocType.DocType_ClientReturnWaste://退废料
                    new CallEnd.MachineMaterialBusinessProce().FinishReturnWasteMaterials(
                        new FinishReturnWasteMaterialsModel()
                        {
                            TaskTid = taskRow[t.ID].ToString()
                        });
                    break;
                case DocType.DocType_ProductTransfer://成品移库
                    new CallEnd.MachineMaterialBusinessProce().FinishProductTransferTask(
                        new FinishProductTransferTaskModel()
                        {
                            TaskTid = taskRow[t.ID].ToString()
                        });
                    break;
                case DocType.DocType_Adjustment://人工库位调整
                    new LocationAdjust.LocationAdj().ForceFinishLocationadJustTask(
                        new ForceFinishLocationadJustTaskModel()
                        {
                            TaskTid = taskRow[t.ID].ToString()
                        });
                    break;
                case DocType.DocType_AutoAdjustment://自动库位调整
                    new LocationAdjust.LocationAdj().ForceFinishAutoLocationadJustTask(
                        new ForceFinishAutoLocationadJustTaskModel()
                        {
                            TaskTid = taskRow[t.ID].ToString()
                        });
                    break;
                default:
                    throw new NotImplementedException();
#pragma warning disable CS0162 // 检测到无法访问的代码
                    break;
#pragma warning restore CS0162 // 检测到无法访问的代码
            }
        }

        private void FinishOutboundDocTaskV1(DataRow taskRow, decimal? qty)
        {
            decimal pickQty = decimal.Parse("0" + taskRow["pick_qty"]);
            decimal diffQty = (decimal)qty - pickQty;

            FinishOutboundDocTaskMoreV1(taskRow, qty);

            if (qty != null)
            {
                ProcessBarcodeInventory(taskRow, diffQty);
            }

            MoveToHistory(taskRow["t_id"].ToString());

            if (string.IsNullOrWhiteSpace(taskRow["order_no"].ToString()))
            {
                OutboundOrder.FinishOrderWhenTasksFinishedAndQtyFinished(taskRow["source_doc_no"].ToString());
            }
            else
            {
                OutboundOrder.FinishOrderWhenTasksFinishedAndQtyFinished(taskRow["order_no"].ToString());
#pragma warning disable CS0612 // '“WorkTask.FinishWaveWhenTasksFinished(string)”已过时
                FinishWaveWhenTasksFinished(taskRow["source_doc_no"].ToString());
#pragma warning restore CS0612 // '“WorkTask.FinishWaveWhenTasksFinished(string)”已过时
            }
        }

        private void FinishOutboundDocTask(DataRow taskRow)
        {
            string taskID = taskRow[t.ID].ToString();
            DataRow[] taskItems = GetTaskItemsBy(taskID);

            // 任务必须有明细，所以此处不需要了
            //if (taskItems.Length <= 0)
            //{
            //    FinishTask(taskRow);
            //}
            foreach (DataRow row in taskItems)
            {
                decimal qty = decimal.Parse("0" + row[wd.PlanQty])
                    - decimal.Parse("0" + row[wd.FinishedQty]);
                FinishOutboundDocTaskItemPartialQty(row, qty);
            }
            MoveToHistory(taskID);
        }

        [Obsolete]
        public IWorkTask FinishWaveWhenTasksFinished(string waveNoOrID)
        {
            DataRow[] taskRows = WorkTask.GetByRefDocNoOrID("Wave", waveNoOrID);
            DataRow[] barcodeTaskRows = taskRows
                .Where(r => r["task_type"].ToString() == "TaskType_Barcode").ToArray();
            if (taskRows.Length > barcodeTaskRows.Length)
            {
                return this;
            }

            ProcessBarcodeTask(barcodeTaskRows);

            Wave.FinishWave(waveNoOrID);

            return this;
        }

        //public void AddTaskItem(DataRow taskRow, WorkTaskItemModel taskItem)
        public void AddTaskItem(WorkTaskModel task, WorkTaskItemModel taskItem, bool isRelay)
        {
            DataRow itemRow = Item.Get(taskItem.ItemID);

            //taskItem.TaskID = taskRow[t.ID].ToString();
            //taskItem.RefDocType = taskRow[t.RefDocType].ToString();
            //taskItem.RefDocID = taskRow[t.RefDocID].ToString();
            //taskItem.RefDocNo = taskRow[t.RefDocNo].ToString();
            taskItem.TaskID = task.ID;
            taskItem.RefDocType = task.RefDocType;
            taskItem.RefDocID = task.RefDocID;
            taskItem.RefDocNo = task.RefDocNo;
            taskItem.ID = null;
            taskItem.ItemName = itemRow["item_name"].ToString();
            taskItem.UomQtyID = itemRow["default_pack_uom_id"].ToString();
            taskItem.UomStatID = itemRow["default_stat_uom_id"].ToString();
            taskItem.ProcessedQty = 0;
            taskItem.FinishedQty = 0;
            taskItem.FinishedStatQty = 0;
            taskItem.ProcessStatus = ProcessStatus.ProcessStatus_New.ToString();

            WorkTaskItem.Insert(taskItem);

            ProcessContainerDetail(
                //new Address(task[t.SrcAddrID].ToString(),
                //            task[t.SrcAddrType].ToString(), task[t.SrcAddrText].ToString()),
                //new Address(task[t.DestAddrID].ToString(),
                //            task[t.DestAddrType].ToString(), task[t.DestAddrText].ToString()),
                //new Address(task.SrcAddrID, task.SrcAddrType, task.SrcAddrText),
                //new Address(task.DestAddrID, task.DestAddrType, task.DestAddrText),
                //taskItem.ContainerDetailID, (decimal)taskItem.PlanQty, isRelay);
                task, taskItem, isRelay);

        }

        public void AddTaskItem(DataRow[] taskRows, WorkTaskItemModel taskItem)
        {
            foreach (DataRow taskRow in taskRows)
            {
                AddTaskItem(new WorkTaskModel()
                {
                    ID = taskRow[t.ID].ToString(),
                    RefDocType = taskRow[t.RefDocType].ToString(),
                    RefDocID = taskRow[t.RefDocID].ToString(),
                    RefDocNo = taskRow[t.RefDocNo].ToString(),
                }, taskItem, false);
            }
        }

        public WorkTaskModel AddOutboundTask(WorkTaskModel task,
            ContainerDetailModel detailModel, ref decimal leftQty)
        {
            DataRow containerRow = Container.GetHeadsByLocation(task.SrcAddrID)[0];
            DataRow[] ctnDtlRows = null;

            if (string.IsNullOrWhiteSpace(detailModel.ID))
            {
                ctnDtlRows = Container
                .GetDetails(containerRow["m_container_id"].ToString(), "") // TODO task.ItemID)
                                                                           //.OrderBy(r => r["first_inbound_dt"].ToString())
                .ToArray();

                string byBatch = WmsConfig.GetOrDefault("Outbound:OutboundByBatch", "0");
                detailModel.LotNo ??= "";
                if (byBatch == "1")
                {
                    ctnDtlRows = ctnDtlRows.Where(r
                        => r["external_lotno"].ToString() == detailModel.LotNo
                        ).ToArray();
                }
                string byBarcode = WmsConfig.GetOrDefault("Outbound:OutboundByBarcode", "0");
                detailModel.ExistBarcode ??= "";
                if (byBarcode == "1")
                {
                    ctnDtlRows = ctnDtlRows.Where(r
                        => r["exist_barcode"].ToString() == detailModel.ExistBarcode
                        ).ToArray();
                }

                string orderFields = WmsConfig.GetOrDefault("Outbound:OrderFields", "first_inbound_dt");
                DataView dv = ctnDtlRows.CopyToDataTable().DefaultView;
                dv.Sort = orderFields;
                ctnDtlRows = dv.ToTable().AsEnumerable().ToArray();
            }
            else
            {
                DataRow row = Container.GetDetail(detailModel.ID);
                ctnDtlRows = new DataRow[] { row };
            }

            decimal sumQty = 0;
            foreach (DataRow ctnDtlRow in ctnDtlRows)
            {
                if (leftQty == 0) break;

                decimal actualQty = decimal.Parse("0" + ctnDtlRow["actual_qty"]);
                decimal outQty = decimal.Parse("0" + ctnDtlRow["out_qty"]);
                decimal lockQty = decimal.Parse("0" + ctnDtlRow["lock_qty"]);
                decimal availQty = actualQty - outQty - lockQty;
                decimal qty = 0;

                if (availQty == 0)
                {
                    continue;
                }
                if (leftQty > availQty)
                {
                    leftQty -= availQty;
                    qty = availQty;
                }
                else
                {
                    qty = leftQty;
                    leftQty = 0;
                }
                if (qty <= 0)
                {
                    throw new WmsException(111, "货位上物料数量不足，无法继续处理");
                }

                sumQty += qty;

                Container.AddDetailOutQty(ctnDtlRow["t_id"].ToString(),
                    qty, decimal.Parse("0" + ctnDtlRow["out_qty"]));

                task.ID = null;
                task.ZoneID = ctnDtlRow["isa_bi_storage_zone_id"].ToString();
                task.TaskType ??= "TaskType_OutBound";
                task.TaskNo = WmsHelper.GetSequence("TaskNo");
                // TODO task.OperationQty = qty;
                // TODO task.ContainerDetailID = ctnDtlRow["t_id"].ToString();
                task.ContainerBarcode = containerRow["container_barcode"].ToString();
                task.RefDocType ??= "RefDocType_OutBound";
                task.TaskStatus = TaskStatus.TaskStatus_Create.ToString();
                //task.IsExecutable = "1";
                WorkTask.Insert(task);
                /*
                {
                    SequenceNo = 0,
                    ItemID = detailModel.ItemID,
                    RefDocNo = task.RefDocNo,
                    RefDocID = task.RefDocID,
                    SrcAddrID = task.SrcAddrID,
                    SrcAddrType = task.SrcAddrType,
                    SrcAddrText = task.DestAddrText,
                    DestAddrID = task.DestAddrID,
                    DestAddrType = task.DestAddrType,
                    DestAddrText = task.DestAddrText,
                });
                */
            }

            return task;
        }

        private void FinishOutboundDocTaskMoreV1(DataRow taskRow, decimal? qty)
        {
            DataRow ctnDtlRow = Container.GetDetail(
                taskRow["isa_bi_container_detail_id"].ToString());
            decimal opQty = decimal.Parse("0" + taskRow["operation_qty"]);
            decimal pickQty = decimal.Parse("0" + taskRow["pick_qty"]);

            if (qty != null)
            {
                if (qty < pickQty)
                {
                    throw new WmsException(304, "当前输入的完成数量小于实际已完成数量");
                }
                else if (qty > opQty)
                {
                    throw new WmsException(306, "当前输入的完成数量大于计划数量");
                }
                else if (qty > pickQty)
                {
                    string codeNumOnly = WmsConfig.GetOrDefault("Task:BarcodeNumberOnly", "0");
                    if (codeNumOnly == "0" && ctnDtlRow["exist_barcode"].ToString() != "ExistBarcode_None")
                    {
                        throw new WmsException(305, "有码烟无法通过输入数量直接出库");
                    }

                    // 修改任务的处理数量
                    UpdatePickQty(taskRow["t_id"].ToString(), (decimal)qty, pickQty);
                    taskRow["pick_qty"] = qty;

                    // 修改容器明细表的数量
                    decimal diffQty = (decimal)qty - pickQty;
                    decimal financeQty = decimal.Parse("0" + taskRow["finance_qty"]);
                    if (taskRow["is_full"].ToString() != "0")
                    {
                        financeQty = financeQty / opQty * diffQty;
                    }

                    Container.StockOut(ctnDtlRow, diffQty, financeQty);
                    pickQty += diffQty;

                    // 出库单据明细，需要修改相应单据的数量信息
                    if (string.IsNullOrWhiteSpace(taskRow["order_no"].ToString()))
                    {
                        OutboundOrder.FinishDetail(taskRow["source_doc_no"].ToString(),
                            taskRow["source_doc_id"].ToString(), diffQty);
                    }
                    else
                    {
                        OutboundOrder.FinishDetail(taskRow["order_no"].ToString(),
                            taskRow["order_id"].ToString(), diffQty);
                        Wave.AddDetailFinishedQty(taskRow["source_doc_id"].ToString(), diffQty);
                    }
                }
            }

            /*
            UpdateState(taskRow["t_id"].ToString(),
                "TaskStatus_Finished", taskRow["task_status"].ToString());
            UpdateStartDate(taskRow["t_id"].ToString(), DateTime.Now);
            UpdateFinishDate(taskRow["t_id"].ToString(), DateTime.Now);
            string executorIsDocCreator = WmsConfig.GetOrDefault("Task:ExecutorIsDocCreator", "0");
            string executor = Context.UserInfo.UserName;
            if (executorIsDocCreator == "1")
            {
                DataRow headRow = OutboundOrder.GetOrder(taskRow["order_no"].ToString());
                executor = headRow["created_by"].ToString();
            }
            UpdateExecutedBy(taskRow["t_id"].ToString(), executor);
            */
            UpdateOutboundTask(taskRow);

            if (pickQty == 0)
            {
                throw new WmsException(302, $"任务{taskRow["task_no"]}完成数量为0，无法强制完成!");
            }
            if (pickQty > 0)
            {
                // 修改货位状态
                //Location.UpdateStatus(taskRow["source_address_id"].ToString(), "StockStatus_Partial");
            }

            decimal leftQty = opQty - pickQty;
            if (leftQty > 0)
            {
                decimal outQty = decimal.Parse("0" + ctnDtlRow["out_qty"]);
                if (leftQty > outQty)
                {
                    throw new WmsException(303, "任务剩余数量大于待入数量，无法继续!");
                }
                Container.AddDetailOutQty(ctnDtlRow["t_id"].ToString(), -leftQty);
            }

        }

        private bool ExistsFollowUpTask(string taskNo)
        {
            DataRow[] dependentRows = GetDependentTasks(taskNo);
            if (dependentRows.Length <= 0)
            {
                return false;
            }
            return true;
        }
        private void FinishOutboundDocTaskItemPartialQty(DataRow taskItemRow, decimal qty)
        {
            if (qty <= 0)
            {
                throw new WmsException(304, "数量不能小于等于0");
            }

            decimal planQty = decimal.Parse("0" + taskItemRow[ti.PlanQty]);
            decimal finishedQty = decimal.Parse("0" + taskItemRow[ti.FinishedQty]);

            if (finishedQty + qty > planQty)
            {
                throw new WmsException(306, "完成数量大于计划数量");
            }

            // 修改任务明细的处理数量
            AddFinishedQty(taskItemRow["t_id"].ToString(), qty, finishedQty);
            finishedQty += qty;
            taskItemRow[ti.FinishedQty] = finishedQty;

            DataRow taskRow = Get(taskItemRow[ti.TaskID].ToString());
            if (qty == finishedQty)
            {
                StartTaskItem(taskItemRow, taskRow);
            }

            ProcessSrcWhenFinishingTask(taskRow, taskItemRow, qty);

            ProcessDestWhenFinishingTask(taskRow, taskItemRow, qty);

            FinishTaskItem(taskItemRow);

            // 出库单据明细，需要修改相应单据的数量信息
            //DataRow[] dependentRows = GetDependentTasks(taskRow[t.TaskNo].ToString());
            //if (dependentRows.Length <= 0)
            if (!ExistsFollowUpTask(taskRow[t.TaskNo].ToString()))
            {
                Wave.FinishDetail(taskItemRow[ti.RefDocDetailID].ToString(), qty);
            }
        }

        private void ProcessDestWhenFinishingTask(DataRow taskRow, DataRow taskItemRow, decimal qty)
        {
            DataRow ctnDtlRow = Container.GetDetail(
                taskItemRow[ti.ContainerDetailID].ToString());
            DataRow ctnRow = Container.GetHead(ctnDtlRow[cd.ContainerID].ToString());
            bool existsFollowUpTask = ExistsFollowUpTask(taskRow[t.TaskNo].ToString());

            DataRow destLocRow = GetLocation(
                taskRow[t.DestAddrType].ToString(), taskRow[t.DestAddrID].ToString());
            if (destLocRow == null)
            {
                if (existsFollowUpTask)
                {
                    return;
                }
                if (taskRow[t.TaskType].ToString() != TaskType.TaskType_OutBound.ToString())
                {
                    return;
                }
                if (ctnRow[ch.ContainerType].ToString() == ContainerType.ContainerType_Virtual.ToString())
                {
                    return;
                }
                Address dest = new Address(taskRow[t.DestAddrType].ToString(), taskRow[t.DestAddrID].ToString());
                if (!string.IsNullOrWhiteSpace(dest.Type)
                    && dest.Type != AddressType.AddressType_Dock.ToString()
                    && !Equipment.IsOutboundStation(dest))
                {
                    // 如果不是这三种情况，就不处理了
                    return;
                }
                // 这三种情况，算成出库，把托盘明细清除
                Container.UpdateDetailActStatus(ctnDtlRow[ch.ID].ToString(), "0");
                return;
            }
            if (destLocRow?[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
            {
                Location.UpdateStatus(destLocRow[l.ID].ToString(),
                    LocationStatus.LocationStatus_Full.ToString(), destLocRow[l.LocationStatus].ToString());
                Container.UpdateContainerLocation(ctnDtlRow[cd.ContainerID].ToString(), destLocRow[l.ID].ToString());
                return;
            }
            if (ctnRow[ch.ContainerType].ToString() == ContainerType.ContainerType_Actual.ToString() &&
                    existsFollowUpTask)
            {
                return;
            }

            // 修改容器明细表的数量
            DataRow destCtnDtlRow = Container.GetDetailByLocation(
                    destLocRow[l.ID].ToString(), taskItemRow[ti.ItemID].ToString());
            UomModel statQty = ItemUom.ConvertToDefaultStatUom(
                taskItemRow[ti.UomQtyID].ToString(), qty);
            Container.StockIn(destCtnDtlRow, qty, statQty.Qty);

            if (GetDependentTasks(taskRow).Length > 1)
            {
                Container.AddDetailQty(
                    destCtnDtlRow[cd.ID].ToString(), -qty);
            }
        }

        private void ProcessSrcWhenFinishingTask(DataRow taskRow, DataRow taskItemRow, decimal qty)
        {
            DataRow ctnDtlRow = Container.GetDetail(
                taskItemRow[ti.ContainerDetailID].ToString());

            string codeNumOnly = WmsConfig.GetOrDefault("Task:BarcodeNumberOnly", "0");
            if (codeNumOnly == "0" && ctnDtlRow[cd.ExistBarcode].ToString() == "ExistBarcode_Exist")
            {
                throw new WmsException(305, "有码烟无法通过输入数量直接出库");
            }

            DataRow locRow = GetLocation(
                taskRow[t.SrcAddrType].ToString(), taskRow[t.SrcAddrID].ToString());
            if (locRow == null)
            {
                return; // 找不到对应的货位
            }
            DataRow ctnRow = Container.GetHead(ctnDtlRow[cd.ContainerID].ToString());
            //if (string.IsNullOrWhiteSpace(ctnRow[ch.LocationID].ToString())
            //    || ctnRow[ch.LocationID].ToString() != locRow[l.ID].ToString())
            if (taskRow[t.TaskStatus].ToString() == TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                return; // 容器已改变位置，不能接着处理了
            }
            //if (locRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
            //{
            //    Location.UpdateStatus(locRow[l.ID].ToString(),
            //        LocationStatus.LoationStatus_Empty.ToString(), locRow[l.LocationStatus].ToString());
            //    Container.UpdateContainerLocation(ctnDtlRow[cd.ContainerID].ToString(), string.Empty);
            //}
            if (ctnRow[ch.ContainerType].ToString() == ContainerType.ContainerType_Actual.ToString())
            {

                Container.UpdateContainerLocation(ctnDtlRow[cd.ContainerID].ToString(), string.Empty);
                if (!CheckLocationHasMoreCtn(ctnDtlRow[cd.ContainerID].ToString()))
                {
                    Location.UpdateStatus(locRow[l.ID].ToString(),
                        LocationStatus.LocationStatus_Empty.ToString(), locRow[l.LocationStatus].ToString());
                }
                // 实托盘不处理容器明细数量
                return;
            }
            // 修改容器明细表的数量
            UomModel statQty = ItemUom.ConvertToDefaultStatUom(
                taskItemRow[ti.UomQtyID].ToString(), qty);
            Container.StockOut(ctnDtlRow, qty, statQty.Qty);

            if (ctnRow[ch.ContainerType].ToString() == ContainerType.ContainerType_Actual.ToString())
            {
                Container.UpdateContainerLocation(ctnDtlRow[cd.ContainerID].ToString(), string.Empty);
            }
            //UpdateSrcLocationStatus(locRow, ctnRow);
        }
        /// <summary>
        ///检查此容器货位上是否还有其它容器
        /// </summary>
        /// <param name="containerid"></param>
        /// <returns></returns>
        private bool CheckLocationHasMoreCtn(string containerid)
        {
            string sql = @"select count(1) from &DataSource&wms_b_container_h h 
                            where h.act_status=1 
                                and exists(select 1 from &DataSource&wms_b_container_h x 
                                        where x.t_id=@containerid and x.location_id=h.location_id
                                    )
                            and h.t_id!=@containerid";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("containerid", containerid);
            return int.Parse(DBHelper.SelectOne(sql, lstParam)) > 0;
        }
        private DataRow GetLocation(string addrType, string addrID)
        {
            if (addrType == AddressType.AddressType_Location.ToString())
            {
                return Location.Get(addrID);
            }
            if (addrType == AddressType.AddressType_Picking.ToString())
            {
                //if (addrID == null)
                //{
                //    return null;
                //}
                return Location.GetPickingLocation();
            }
            if (addrType == AddressType.AddressType_Station.ToString())
            {
                if (string.IsNullOrWhiteSpace(addrID))
                {
                    return null;
                }
                DataRow equip = Equipment.GetEquipment(addrID);
                if (equip != null && equip["equipment_usage"].ToString().Contains(EquipmentUsage.EquipmentUsage_Picking.ToString()))
                {
                    return Location.GetConveyorLocation();
                }
            }
            return null;
        }

        private void FinishTaskItem(DataRow taskItemRow)
        {
            decimal planQty = decimal.Parse("0" + taskItemRow[ti.PlanQty].ToString());
            decimal finishedQty = decimal.Parse("0" + taskItemRow[ti.FinishedQty].ToString());
            if (planQty == finishedQty)
            {
                UpdateTaskItemStatus(taskItemRow[ti.ID].ToString(),
                    ProcessStatus.ProcessStatus_Done.ToString(), taskItemRow[ti.ProcessStatus].ToString());
                UpdateTaskItemEndDate(taskItemRow[ti.ID].ToString(), DateTime.Now);
            }

            FinishTaskWhenTaskItemFinished(taskItemRow[ti.TaskID].ToString());
        }

        private void FinishTaskWhenTaskItemFinished(string taskID)
        {
            DataRow[] taskItems = GetTaskItemsBy(taskID);
            foreach (DataRow row in taskItems)
            {
                if (decimal.Parse("0" + row[wd.FinishedQty])
                    < decimal.Parse("0" + row[wd.PlanQty]))
                {
                    return;
                }
            }

            FinishTask(taskID);
        }

        private void FinishTask(string taskID)
        {
            FinishTask(Get(taskID));
        }

        private void FinishTask(DataRow taskRow)
        {
            string taskStatus = taskRow[t.TaskStatus].ToString();
            if (taskStatus != TaskStatus.TaskStatus_Executing.ToString()
                && taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "任务状态错");
            }
            UpdateTaskStatus(taskRow[t.ID].ToString(),
                TaskStatus.TaskStatus_Finished.ToString(),
                taskRow[t.TaskStatus].ToString());

            DataRow ctnRow = Container.GetHead(taskRow[t.ContainerID].ToString());
            if (ctnRow[ch.ContainerType].ToString() == ContainerType.ContainerType_Actual.ToString())
            {
                DataRow[] ctnDtlRows = Container.GetDetails(ctnRow[ch.ID].ToString());
                bool empty = true;
                foreach (DataRow row in ctnDtlRows)
                {
                    if (decimal.Parse("0" + row[cd.Qty]) > 0m)
                    {
                        empty = false;
                        break;
                    }
                }
                if (empty)
                {
                    foreach (DataRow row in ctnDtlRows)
                    {
                        if (decimal.Parse("0" + row[cd.Qty]) <= 0m)
                        {
                            Container.UpdateDetailActStatus(row[cd.ID].ToString(), "0");
                        }
                    }
                    Container.UpdateHeadActStatus(ctnRow[ch.ID].ToString(), "0")
                        .UpdateHeadIsOut(ctnRow[ch.ID].ToString(), "1");
                }
            }
            // TODO ProcessBarcodeTask(barcodeTaskRows);
            Wave.AddHeadLeftTaskQty(taskRow[t.RefDocID].ToString(), -1);
            Wave.FinishWaveWhenTasksFinished(taskRow[t.RefDocID].ToString());
        }

        // 修改货位状态
        private void UpdateSrcLocationStatus(DataRow locationRow, DataRow containerRow)
        {
            if (locationRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
            {
                Location.UpdateStatus(locationRow[l.ID].ToString(),
                    LocationStatus.LocationStatus_Empty.ToString(),
                    locationRow[l.LocationStatus].ToString());
            }
            else
            {
                DataRow[] ctnDtlRows = Container.GetDetailsByLocation(locationRow[l.ID].ToString());
                if (ctnDtlRows.Length > 0)
                {
                    Location.UpdateStatus(locationRow[l.ID].ToString(),
                        LocationStatus.LocationStatus_Partial.ToString());
                }
                else
                {
                    Location.UpdateStatus(locationRow[l.ID].ToString(),
                        LocationStatus.LocationStatus_Empty.ToString());
                }
            }
        }

        private void StartTaskItem(DataRow taskItemRow, DataRow taskRow = null)
        {
            string status = taskItemRow[ti.ProcessStatus].ToString();
            if (status == ProcessStatus.ProcessStatus_Doing.ToString())
            {
                return;
            }
            if (status != ProcessStatus.ProcessStatus_New.ToString())
            {
                throw new WmsException(500, "任务状态错");
            }

            UpdateTaskItemStatus(taskItemRow[ti.ID].ToString(),
                ProcessStatus.ProcessStatus_Doing.ToString(),
                taskItemRow[ti.ProcessStatus].ToString());
            UpdateTaskItemStartDate(taskItemRow[ti.ID].ToString(), DateTime.Now);
            taskItemRow[ti.ProcessStatus] = ProcessStatus.ProcessStatus_Doing.ToString();

            taskRow = taskRow ?? Get(taskItemRow[ti.TaskID].ToString());
            StartTask(taskRow);
        }

        public IWorkTask StartTask(string taskID)
        {
            return StartTask(Get(taskID));
        }

        public IWorkTask StartTask(DataRow taskRow)
        {
            string taskStatus = taskRow[t.TaskStatus].ToString();
            if (taskStatus == TaskStatus.TaskStatus_Executing.ToString()
                || taskStatus == TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                return this;
            }
            if (taskStatus != TaskStatus.TaskStatus_NotExecute.ToString()
                && taskStatus != TaskStatus.TaskStatus_Create.ToString())
            {
                throw new WmsException(500, "任务状态错");
            }
            string executorType = taskRow[t.ExecutorType].ToString();
            if (executorType == TaskExecutor.TaskExecutor_WCS.ToString())
            {
                UpdateTaskStatus(taskRow[t.ID].ToString(),
                    TaskStatus.TaskStatus_Executing.ToString(),
                    taskRow[t.TaskStatus].ToString());
            }
            else
            {
                UpdateTaskStatus(taskRow[t.ID].ToString(),
                    TaskStatus.TaskStatus_WaitingExecute.ToString(),
                    taskRow[t.TaskStatus].ToString());
            }
            //if (taskRow["dependent_task_no"].ToString().Length <= 0)
            //{
            WorkTask.UpdateIsExecutable(taskRow[t.ID].ToString(), "1");
            //}

            string executorIsDocCreator = WmsConfig.GetOrDefault("Task:ExecutorIsDocCreator", "0");
            string executor = Context.UserInfo.UserName;
            if (executorIsDocCreator == "1" &&
                !string.IsNullOrWhiteSpace(taskRow["order_no"].ToString()))
            {
                DataRow headRow = OutboundOrder.GetOrder(taskRow["order_no"].ToString());
                executor = headRow["created_by"].ToString();
            }
            UpdateExecutedBy(taskRow["t_id"].ToString(), executor);
            return this;
        }

        private void FinishOutboundDocTaskItem(DataRow taskItemRow, decimal? qty)
        {
            DataRow ctnDtlRow = Container.GetDetail(
                taskItemRow[ti.ContainerDetailID].ToString());
            decimal planQty = decimal.Parse("0" + taskItemRow[ti.PlanQty]);
            decimal finishedQty = decimal.Parse("0" + taskItemRow[ti.FinishedQty]);
            decimal diffQty = qty != null ? (decimal)qty : planQty - finishedQty;

            //if (qty < finishedQty)
            //{
            //    throw new WmsException(304, "当前输入的完成数量小于实际已完成数量");
            //}
            //else
            if (diffQty < 0m)
            {
                throw new WmsException(304, "数量不能为负数");
            }
            else if (finishedQty + diffQty > planQty)
            {
                throw new WmsException(306, "完成数量大于计划数量");
            }
            string codeNumOnly = WmsConfig.GetOrDefault("Task:BarcodeNumberOnly", "0");
            if (codeNumOnly == "0" && ctnDtlRow["exist_barcode"].ToString() != "ExistBarcode_None")
            {
                throw new WmsException(305, "有码烟无法通过输入数量直接出库");
            }

            // 修改任务的处理数量
            AddFinishedQty(taskItemRow["t_id"].ToString(), diffQty, finishedQty);
            //UpdatePickQty(taskItemRow["t_id"].ToString(), currentQty, finishedQty);
            finishedQty += diffQty;
            taskItemRow[ti.FinishedQty] = finishedQty;
            if (finishedQty == 0)
            {
                throw new WmsException(302, $"任务{taskItemRow["task_no"]}完成数量为0，无法强制完成!");
            }

            // 修改容器明细表的数量
            //decimal diffQty = currentQty - finishedQty;
            UomModel statQty = ItemUom.ConvertToDefaultStatUom(
                taskItemRow[ti.UomQtyID].ToString(), diffQty);
            //decimal financeQty = decimal.Parse("0" + taskItemRow["finance_qty"]);
            //if (taskItemRow["is_full"].ToString() != "0")
            //{
            //    financeQty = financeQty / planQty * diffQty;
            //}

            Container.StockOut(ctnDtlRow, diffQty, statQty.Qty);

            // 出库单据明细，需要修改相应单据的数量信息
            Wave.FinishDetail(taskItemRow[ti.RefDocID].ToString(), diffQty);

            // TODO
            UpdateOutboundTask(taskItemRow);

            UpdateTaskItemStatus(taskItemRow[ti.ID].ToString(),
                ProcessStatus.ProcessStatus_Done.ToString(), taskItemRow[ti.ProcessStatus].ToString());

            if (finishedQty > 0)
            {
                // 修改货位状态
                Location.UpdateStatus(taskItemRow["source_address_id"].ToString(), "StockStatus_Partial");
            }

            decimal leftQty = planQty - finishedQty;
            if (leftQty > 0)
            {
                decimal outQty = decimal.Parse("0" + ctnDtlRow["out_qty"]);
                if (leftQty > outQty)
                {
                    throw new WmsException(303, "任务剩余数量大于待出数量，无法继续!");
                }
                Container.AddDetailOutQty(ctnDtlRow["t_id"].ToString(), -leftQty);
            }

        }

        private void ProcessBarcodeInventory(DataRow taskRow, decimal qty)
        {
            //DataRow headRow = OutboundOrder.GetOrder(taskRow["order_no"].ToString());

            //if (headRow["doc_type"].ToString() == "ComplementCode")
            //{
            //    throw new WmsException(307, "码段不可以有任务");
            //}

            string codeNumOnly = WmsConfig.GetOrDefault("Task:BarcodeNumberOnly", "0");
            if (codeNumOnly == "0")
            {
                return;
            }
            string detailID = "";
            if (string.IsNullOrWhiteSpace(taskRow["order_id"].ToString()))
            {
                detailID = taskRow["source_doc_id"].ToString();
            }
            else
            {
                detailID = taskRow["order_id"].ToString();
            }
            DataRow detailRow = OutboundOrder.GetDetail(detailID);
            if (detailRow["inventory_class"].ToString() == "ComplementCode")
            {
                throw new WmsException(307, "码段不可以有任务");
            }

            //if (headRow["doc_type"].ToString() == "WorkInProcess")
            if (detailRow["inventory_class"].ToString() == "WorkInProcess")
            {
                string barcodeLoc = WmsConfig.GetOrDefault("Barcode:BarcodeLocationCode", "");
                DataRow locRow = Location.Get(barcodeLoc);
                DataRow containerRow = Container.GetHead(locRow["location_no"].ToString());
                DataRow docDtlRow = OutboundOrder.GetDetail(detailID);

                decimal planQty = decimal.Parse("0" + taskRow["operation_qty"]);
                decimal diffQty = planQty - qty;
                /*
                DataRow barcodeDetailRow = Container.GetDetail(
                    containerRow["m_container_id"].ToString(),
                    taskRow["isa_wi_item_id"].ToString(),
                    docDtlRow["lot_no"].ToString()
                );

                // 修改容器明细表的数量
                Container.StockOut(barcodeDetailRow, qty);
                if (diffQty > 0)
                {
                    Container.AddDetailOutQty(
                        barcodeDetailRow["t_id"].ToString(), -diffQty);
                }
                */
                DataRow[] barcodeDetailRows = Container.GetDetails(
                    containerRow["m_container_id"].ToString(),
                    taskRow["isa_wi_item_id"].ToString(),
                    docDtlRow["lot_no"].ToString()
                );
                barcodeDetailRows = barcodeDetailRows.OrderBy(
                    r => r["first_inbound_dt"].ToString()).ToArray();

                foreach (DataRow row in barcodeDetailRows)
                {
                    decimal rowQty = decimal.Parse("0" + row["out_qty"]);
                    decimal leftQty = rowQty < qty ? rowQty : qty;
                    if (qty > 0)
                    {
                        decimal financeQty = leftQty * decimal.Parse("0" + row["speci"]);
                        Container.StockOut(row, leftQty, financeQty);
                        qty -= leftQty;
                    }
                    else if (diffQty > 0)
                    {
                        Container.AddDetailOutQty(
                            row["container_d_id"].ToString(), -leftQty);
                        diffQty -= leftQty;
                    }
                }
                if (qty < 0 || diffQty < 0)
                {
                    throw new WmsException(308, "码段数量不足");
                }
            }
        }

        private void ProcessBarcodeTask(DataRow[] barcodeTaskRows)
        {
            foreach (DataRow row in barcodeTaskRows)
            {
                UpdatePickQty(row["t_id"].ToString(),
                    decimal.Parse("0" + row["operation_qty"]),
                    decimal.Parse("0" + row["pick_qty"]));

                UpdateOutboundTask(row);
                MoveToHistory(row["t_id"].ToString());
            }
        }

        private void UpdateOutboundTask(DataRow row)
        {
            UpdateTaskStatus(row["t_id"].ToString(),
                TaskStatus.TaskStatus_Finished.ToString(),
                row["task_status"].ToString());
            UpdateStartDate(row["t_id"].ToString(), DateTime.Now);
            UpdateFinishDate(row["t_id"].ToString(), DateTime.Now);
            string executorIsDocCreator = WmsConfig.GetOrDefault("Task:ExecutorIsDocCreator", "0");
            string executor = Context.UserInfo.UserName;
            if (executorIsDocCreator == "1" &&
                !string.IsNullOrWhiteSpace(row["order_no"].ToString()))
            {
                DataRow headRow = OutboundOrder.GetOrder(row["order_no"].ToString());
                executor = headRow["created_by"].ToString();
            }
            UpdateExecutedBy(row["t_id"].ToString(), executor);
        }

        private void FinishStockUpTask(DataRow taskRow, decimal? qty)
        {
            FinishOutboundDocTaskMoreV1(taskRow, qty);

            //DataRow locRow = Location.Get(taskRow["target_address_id"].ToString());
            //DataRow containerRow = Container.GetHead(locRow["location_no"].ToString());
            //DataRow ctnDtlRow = Container.GetDetail(
            //            containerRow["m_container_id"].ToString(),
            //            taskRow["isa_wi_item_id"].ToString());
            DataRow ctnDtlRow = Container.GetDetailByLocation(
                        taskRow["target_address_id"].ToString(),
                        taskRow["isa_wi_item_id"].ToString());

            decimal opQty = decimal.Parse("0" + taskRow["operation_qty"]);
            decimal inQty = qty == null ? opQty : (decimal)qty;
            decimal financeQty = decimal.Parse("0" + taskRow["finance_qty"]);

            Container.StockIn(ctnDtlRow, inQty, financeQty);
            if (inQty != opQty)
            {
                Container.AddDetailInQty(ctnDtlRow["t_id"].ToString(), inQty - opQty);
            }
        }

        /// <summary>
        /// 自动完成入库任务
        /// </summary>
        /// <param name="taskRow"></param>
        /// <param name="qty"></param>
        private void FinishInboundTask(DataRow taskRow, decimal? qty)
        {
            DocType type = (DocType)Enum.Parse(typeof(DocType), taskRow[t.RefDocType].ToString(), true);
            switch (type)
            {
                case DocType.DocType_ClientReturnNullPallet://退空盘
                    new CallEnd.MachineMaterialBusinessProce().FinishClientReturnNullPallet(
                        new FinishClientReturnNullPalletModel()
                        {
                            TaskTid = taskRow[t.ID].ToString()
                        });
                    break;
                case DocType.DocType_ClientReturnPallet://退余料
                    new CallEnd.MachineMaterialBusinessProce().FinishClientReturnPallet(
                        new FinishClientReturnPalletModel()
                        {
                            TaskTid = taskRow[t.ID].ToString()
                        });
                    break;
                case DocType.DocType_ReturnBadPallet://退坏料
                    new CallEnd.MachineMaterialBusinessProce().FinishClientReturnBadPallet(
                        new FinishClientReturnBadPalletModel() { TaskTid = taskRow[t.ID].ToString() });
                    break;
                case DocType.DocType_PersonReturnMateriel://人工退料
                    new CallEnd.MachineMaterialBusinessProce().FinishPersonReturnMaterielTask(new FinishPersonReturnMaterielTaskModel()
                    {
                        TaskTid = taskRow[t.ID].ToString()
                    });
                    break;
                case DocType.DocType_Inbound:
                    TaskHeaderModel taskHeader = GetTaskHeader(taskRow["t_id"].ToString());
                    Container container = new Container();
                    ContainerModel ctnHeader = container.getContainerById(taskHeader.container_id);
                    if (string.IsNullOrEmpty(taskHeader.dest_addr_id))
                    {
                        throw new Exception("此任务没有目标货位，不允许完成");
                    }
                    Putaway putaway = new Putaway();
                    if (ctnHeader.container_type == ContainerType.ContainerType_Actual.ToString())
                    {
                        putaway.PutawayPalletTaskToLocation(taskHeader.t_id, taskHeader.dest_addr_text);
                        LocationModel locm = new Location().GetLocationById(taskHeader.t_id);
                        container.UpdateContainerSnInLocation(taskHeader.container_id, locm, taskHeader.task_no, "上架", "入库完成");

                        List<ContainerDModel> lstCtnDetail = container.getContainerDetails(ctnHeader.t_id);
                        foreach (ContainerDModel ctnD in lstCtnDetail)
                        {
                            if (lstCtnDetail.Count > 0 && !string.IsNullOrEmpty(lstCtnDetail[0].inbound_h_id))
                            {
                                putaway.PlusInboudOrderCompleteQtyWhenIn(ctnD.inbound_h_id, ctnD.inbound_d_id);
                            }
                        }
                    }
                    else if (ctnHeader.container_type == ContainerType.ContainerType_Virtual.ToString())
                    {
                        List<TaskItemModel> lstTaskItem = getTaskItemsForDoing(taskHeader.t_id);
                        if (lstTaskItem.Count > 0)
                        {
                            foreach (TaskItemModel ti in lstTaskItem)
                            {
                                NtiCloud.Model.WMS.PutawayToLocation putto = new NtiCloud.Model.WMS.PutawayToLocation();
                                putto.TaskItemId = ti.t_id;
                                putto.TaskId = taskHeader.t_id;
                                putto.ToLocationCode = taskHeader.dest_addr_text;
                                putto.InQty = ti.plan_qty ?? 0;
                                putaway.DoPutawayToLocation(putto, new Putaway.CheckPutawayComplete(putaway.CheckPutawayDone));
                            }
                        }
                        else
                        {
                            UpdateTaskStatus(taskHeader.t_id, TaskStatus.TaskStatus_Finished.ToString());
                            MoveToHistory(taskHeader.t_id);
                        }
                    }
                    else
                    {
                        throw new Exception("暂不支持");
                    }

                    break;
                case DocType.DocType_EmptyContainerIn:
                case DocType.DocType_ContainerIn:
                case DocType.DocType_ProductPlanIn:
                case DocType.DocType_ReceiptGroup:
                    TaskHeaderModel taskHeaderEmp = GetTaskHeader(taskRow["t_id"].ToString());
                    Container containertmp = new Container();
                    ContainerModel ctnHeaderEmp = containertmp.getContainerById(taskHeaderEmp.container_id);
                    if (string.IsNullOrEmpty(taskHeaderEmp.dest_addr_id))
                    {
                        throw new Exception("此任务没有目标货位，不允许完成");
                    }
                    Putaway putawayEmp = new Putaway();
                    putawayEmp.PutawayPalletTaskToLocation(taskHeaderEmp.t_id, taskHeaderEmp.dest_addr_text);
                    if (type == DocType.DocType_ContainerIn || type == DocType.DocType_ReceiptGroup)
                    {
                        List<ContainerDModel> lstCtnDetail = containertmp.getContainerDetails(ctnHeaderEmp.t_id);
                        //是否需要更新收获单明细已完成量
                        string needPlusDocQty = WmsConfig.GetOrDefault("Inbound:NeedPlusFinishQty", "1");
                        if ("1".Equals(needPlusDocQty))
                        {
                            foreach (ContainerDModel ctnD in lstCtnDetail)
                            {
                                if (lstCtnDetail.Count > 0 && !string.IsNullOrEmpty(lstCtnDetail[0].inbound_h_id) && !string.IsNullOrEmpty(lstCtnDetail[0].inbound_d_id))
                                {
                                    putawayEmp.PlusInboudOrderCompleteQtyWhenIn(ctnD.inbound_h_id, ctnD.inbound_d_id);
                                }
                            }
                        }

                        if (string.Equals(taskHeaderEmp.zone_id, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))//恩施，实托盘入库任务完成后，查询该货位所在行，是否已放满，如果放满，则更改货位标识为可用
                        {

                            Location lo = new Location();
                            LocationModel _locInfo = lo.GetLocationById(taskHeaderEmp.dest_addr_id);
                            NtiCloud.Service.WMS.Base.CallEnd ce = new NtiCloud.Service.WMS.Base.CallEnd();
                            bool isLineFull = ce.IsLineFull(_locInfo.location_row.ToString());

                            Model.UserLogModel _logModel1 = new Model.UserLogModel();
                            _logModel1.ModifyIP = Context.UserInfo.LoginIP;
                            _logModel1.WarehouseID = Context.WarehouseID;
                            _logModel1.TypeCode = "完成任务二楼入库任务";
                            _logModel1.LogTextLong = "行号：" + _locInfo.location_row.ToString();
                            _logModel1.CommonName = $"入库货位编码{_locInfo.location_no},检查行是否已存储满，结果：" + isLineFull;
                            _logModel1.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
                            _logModel1.CreatedBy = Context.UserInfo.UserId;
                            _logModel1.LogText = "";
                            UserLog.Add(_logModel1);
                            if (isLineFull == true)
                            {
                                Model.UserLogModel _logModel = new Model.UserLogModel();
                                _logModel.ModifyIP = Context.UserInfo.LoginIP;
                                _logModel.WarehouseID = Context.WarehouseID;
                                _logModel.TypeCode = "完成任务二楼入库任务";
                                _logModel.LogTextLong = $"入库货位编码{_locInfo.location_no},行号：" + _locInfo.location_row.ToString();
                                _logModel.CommonName = "行已满";
                                _logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
                                _logModel.CreatedBy = Context.UserInfo.UserId;
                                _logModel.LogText = "";
                                UserLog.Add(_logModel);

                                lo.UpdateLocationStatus(_locInfo.t_id, LocationStatus.LocationStatus_Stored);
                                lo.UpdateLocationFlag(_locInfo.location_row.ToString(), LocationFlag.LocationFlag_Usable.ToString(), LocationFlag.LocationFlag_InOnly.ToString());
                            }
                        }
                    }


                    break;
                case DocType.DocType_Qc:
                    new QC.QcBound().FouceInQcTask(taskRow["t_id"].ToString());
                    break;
                case DocType.DocType_PickSurpluBack: //余料回库
                    new RFPick.RFPick().RFPickSurpluBackOver(taskRow["t_id"].ToString());
                    break;
                case DocType.DocType_PickOrderBack: //订单箱回库
                    new RFPick.RFPick().RFPickOrderBackOver(taskRow["t_id"].ToString());
                    break;
                case DocType.DocType_Count:  //盘点回库任务
                    new CountContainer.CountContainer().FinishInCount(taskRow["t_id"].ToString(), TaskStatus.TaskStatus_Finished);
                    break;
                default:
                    throw new NotImplementedException("此任务类型暂不支持此操作");
#pragma warning disable CS0162 // 检测到无法访问的代码
                    break;
#pragma warning restore CS0162 // 检测到无法访问的代码
            }
            /*
            DataRow ctnDtlRow = Container.GetDetail(
                taskRow["isa_bi_container_detail_id"].ToString());
            DataRow headRow = null;
            decimal opQty = decimal.Parse("0" + taskRow["operation_qty"]);
            decimal pickQty = decimal.Parse("0" + taskRow["pick_qty"]);

            if (qty != null)
            {
                if (qty < pickQty)
                {
                    throw new WmsException(304, "当前输入的完成数量小于实际已完成数量");
                }
                else if (qty > opQty)
                {
                    throw new WmsException(306, "当前输入的完成数量大于计划数量");
                }
                else if (qty > pickQty)
                {
                    string codeNumOnly = WmsConfig.GetOrDefault("Task:BarcodeNumberOnly", "0");
                    if (codeNumOnly == "0" && ctnDtlRow["exist_barcode"].ToString() != "ExistBarcode_None")
                    {
                        throw new WmsException(305, "有码烟无法通过输入数量直接入库");
                    }

                    // 修改任务的处理数量
                    UpdatePickQty(taskRow["t_id"].ToString(), (decimal)qty, pickQty);
                    taskRow["pick_qty"] = qty;

                    // 修改容器明细表的数量
                    decimal diffQty = (decimal)qty - pickQty;
                    decimal financeQty = decimal.Parse("0" + taskRow["finance_qty"]);
                    if (taskRow["is_full"].ToString() != "0")
                    {
                        financeQty = financeQty / opQty * diffQty;
                    }
                    Container.StockIn(ctnDtlRow, diffQty, financeQty);
                    pickQty += diffQty;

                    headRow = Receipt.GetHead(taskRow["source_doc_no"].ToString());

                    if (headRow["doc_type"].ToString() == "ComplementCode")
                    {
                        throw new WmsException(307, "码段不可以有任务");
                    }
                    if (headRow["doc_type"].ToString() == "WorkInProcess")
                    {
                        string barcodeLoc = WmsConfig.GetOrDefault("Barcode:BarcodeLocationCode", "");
                        DataRow locRow = Location.Get(barcodeLoc);
                        DataRow containerRow = Container.GetHead(locRow["location_no"].ToString());

                        DataRow docDetailRow = Receipt.GetDetail(taskRow["source_doc_id"].ToString());
                        //DateTime? produceDate = docDetailRow["produce_date"] is System.DBNull
                        //    ? DateTime.Now : (DateTime)docDetailRow["produce_date"];
                        DateTime? produceDate = Receipt.NormalizeDate(docDetailRow["produce_date"]);

                        string ctnDetailID = SnowflakeID.Guid().ToString();
                        ContainerDetailModel ctnDetailModel = new ContainerDetailModel()
                        {
                            ID = ctnDetailID,
                            ZoneID = locRow["isa_bi_storage_zone_id"].ToString(),
                            ContainerID = containerRow["m_container_id"].ToString(),
                            InboundHeadID = headRow["t_id"].ToString(),
                            ItemID = docDetailRow["isa_wi_item_id"].ToString(),
                            ItemName = docDetailRow["item_name"].ToString(),
                            UomQtyID = docDetailRow["isa_wi_uom_qtyid"].ToString(),
                            UomFinanceID = docDetailRow["isa_wi_uom_financeid"].ToString(),
                            ExternalLotNo = docDetailRow["lot_no"].ToString(),
                            GivenYear = decimal.Parse("0" + docDetailRow["givenyear"]),
                            Speci = docDetailRow["speci"].ToString(),
                            //ExistBarcode = row["exist_barcode"].ToString(),
                            ExistBarcode = "ExistBarcode_Exist",
                            //FirstInboundDate = (DateTime)docDetailRow["produce_date"],
                            FirstInboundDate = produceDate,
                        };

                        //decimal planQty = decimal.Parse("0" + row["plan_qty"]);
                        decimal planQty = diffQty;
                        decimal allocatedQty = Container.PreparePutaway(ctnDetailModel,
                            planQty, decimal.Parse("0" + locRow["qty_limit"]));
                        if (allocatedQty <= 0)
                        {
                            throw new WmsException(102, "入库数量错误，无法继续处理");
                        }

                        DataRow barcodeDetailRow = Container.GetDetail(
                            containerRow["m_container_id"].ToString(),
                            taskRow["isa_wi_item_id"].ToString(),
                            ctnDetailModel.ExternalLotNo);

                        // 修改容器明细表的数量
                        Container.StockIn(barcodeDetailRow, diffQty, financeQty);
                    }

                    // 入库单据明细，需要修改相应单据的数量信息
                    //Receipt.AddDetailFinishedQty(taskRow["source_doc_id"].ToString(), diffQty);
                    Receipt.FinishDetail(taskRow["source_doc_no"].ToString(),
                        taskRow["source_doc_id"].ToString(), diffQty);
                }
            }

            UpdateState(taskRow["t_id"].ToString(),
                "TaskStatus_Finished", taskRow["task_status"].ToString());
            UpdateStartDate(taskRow["t_id"].ToString(), DateTime.Now);
            UpdateFinishDate(taskRow["t_id"].ToString(), DateTime.Now);

            string executorIsDocCreator = WmsConfig.GetOrDefault("Task:ExecutorIsDocCreator", "0");
            string executor = Context.UserInfo.UserName;
            if (executorIsDocCreator == "1")
            {
                executor = headRow?["created_by"].ToString();
            }
            UpdateExecutedBy(taskRow["t_id"].ToString(), executor);

            if (pickQty == 0)
            {
                throw new WmsException(302, $"任务{taskRow["task_no"]}完成数量为0，无法强制完成!");
            }
            if (pickQty > 0)
            {
                // 修改货位状态
                //Location.UpdateLocationStockState(taskRow["target_address_id"].ToString(), "StockStatus_Partial");
            }

            decimal leftQty = opQty - pickQty;
            if (leftQty > 0)
            {
                decimal inQty = decimal.Parse("0" + ctnDtlRow["in_qty"]);
                if (leftQty > inQty)
                {
                    throw new WmsException(303, "任务剩余数量大于待入数量，无法继续!");
                }
                Container.AddDetailInQty(ctnDtlRow["t_id"].ToString(), -leftQty);
            }
            */
        }


        public IWorkTask RecycleInboundTask(DataRow taskRow)
        {
            decimal processQty = decimal.Parse("0" + taskRow["processed_qty"]);
            decimal finishQty = decimal.Parse("0" + taskRow["finished_qty"]);

            if (finishQty > 0)
            {
                throw new WmsException(314, $"任务{taskRow["task_no"]}已有完成数量，无法回收!");
            }
            if (processQty > 0)
            {
                throw new WmsException(314, $"任务{taskRow["task_no"]}已有处理数量，无法回收!");
            }

            /* string recycleInfo = RecycleWcsTask(taskRow["t_id"].ToString());
             if (!string.IsNullOrEmpty(recycleInfo)) {
                 throw new WmsException(314, $"回收WCS任务{taskRow["task_no"]}失败:"+recycleInfo);
             }*/

            List<TaskItemModel> lstTaskItem = getTaskItems(taskRow["t_id"].ToString());
            UpdateActStatus(taskRow["t_id"].ToString(), "0");
            UpdateTaskItemActStatusByTask(taskRow["t_id"].ToString(), "0");
            MoveToHistory(taskRow["t_id"].ToString());

            foreach (TaskItemModel ti in lstTaskItem)
            {
                DataRow ctnDtlRow = Container.GetDetail(ti.container_d_id);
                decimal qty = decimal.Parse("0" + ctnDtlRow["qty"]);
                decimal inQty = decimal.Parse("0" + ctnDtlRow["in_qty"]);
                if (inQty < ti.plan_qty)
                {
                    throw new WmsException(315, $"任务计划数量大于待入数量，任务{taskRow["task_no"]}无法回收!");
                }
                else if (inQty > ti.plan_qty || qty > 0)
                {
                    Container.AddDetailInQty(ctnDtlRow["t_id"].ToString(), -(ti.plan_qty ?? 0));
                }
                else
                {
                    Container.DeleteDetail(ctnDtlRow["t_id"].ToString());
                }
            }
            DataRow[] ctnDtlRows = Container.GetDetails(taskRow["container_id"].ToString());
            if (ctnDtlRows.Length > 0)
            {
                return this;
            }
            //DataRow ctnRow = Container.GetHead(ctnDtlRow["isa_m_container_id"].ToString());
            //DataRow locRow = Location.Get(ctnRow["location_id"].ToString());
            DataRow locRow = Location.Get(taskRow["dest_addr_id"].ToString());
            Location.UpdateStatus(locRow["t_id"].ToString(),
                LocationStatus.LocationStatus_Empty.ToString(),
                locRow["location_status"].ToString());

            DataRow drInboundBuffer = new Location().GetInboundBufferLocation();
            Container.UpdateContainerLocation(taskRow["container_id"].ToString(), drInboundBuffer["t_id"].ToString());

            return this;
        }
        /// <summary>
        /// 回收空托盘入库任务
        /// <br/>
        /// 会删除任务对应的容器信息
        /// </summary>
        /// <param name="taskRow"></param>
        /// <returns></returns>
        public IWorkTask RecycleEmptyContainerInTask(DataRow taskRow)
        {

            string row = taskRow["dest_addr_text"].ToString();
            string zoneId = taskRow["zone_id"].ToString();
            string refDocType = taskRow["ref_doc_type"].ToString();
            string twoFloorZoneId = WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336");//二楼区域
            if (string.Equals(zoneId, twoFloorZoneId))
            {//恩施专用,用于一次和二次货位分配，空托盘入库的任务回收
                //修改任务软删除标记为“0”
                WorkTask.UpdateActStatus(taskRow["t_id"].ToString(), "0");
                if (row.Length <= 3)//一次货位分配
                {
                    RecycleInBoundFirstDistributionLocationTask(taskRow);
                }
                else//二次货位分配
                {
                    RecycleInBoundSecoundDistributionLocationTask(taskRow);
                }
            }
            else
            {
                List<TaskItemModel> lstTaskItem = getTaskItems(taskRow["t_id"].ToString());

                MoveToHistory(taskRow["t_id"].ToString());

                foreach (TaskItemModel ti in lstTaskItem)
                {
                    Container.DeleteDetail(ti.container_d_id);
                }
                Container.DeleteHead(taskRow["container_id"].ToString());

                LocationModel locRow = Location.GetLocationById(taskRow["dest_addr_id"].ToString());
                Location.UpdateStatus(locRow.t_id,
                    LocationStatus.LocationStatus_Empty.ToString(),
                    locRow.location_status);
            }


            return this;
        }

        /// <summary>
        /// 回收空托盘出库任务
        /// </summary>
        /// <param name="taskRow"></param>
        /// <returns></returns>
        public IWorkTask RecycleEmptyContainerOutTask(DataRow taskRow)
        {
            MoveToHistory(taskRow["t_id"].ToString());
            LocationModel locRow = Location.GetLocationById(taskRow["src_addr_id"].ToString());
            Location.UpdateStatus(locRow.t_id,
                LocationStatus.LocationStatus_Stored.ToString(),
                locRow.location_status);

            return this;
        }

        /// <summary>
        /// 回收实托盘入库货位一次分配后的任务
        /// </summary>
        /// <param name="taskRow">任务主档信息</param>
        /// <returns></returns>
        public void RecycleInBoundFirstDistributionLocationTask(DataRow taskRow)
        {
            string tid = taskRow["t_id"].ToString();
            string row = taskRow["dest_addr_text"].ToString();
            string containerId = taskRow["container_id"].ToString();
            string zoneId = taskRow["zone_id"].ToString();
            NtiCloud.Service.WMS.Base.CallEnd ce = new Base.CallEnd();
            DataTable dt = ce.GetLocByLine(row, true);
            string locId = dt.Rows[0]["t_id"].ToString();
            ce.UpdateLocStatusByLocationId(locId, LocationStatus.LocationStatus_Empty.ToString(), LocationStatus.LocationStatus_Plan.ToString(), "");
            string isAllowedInbound = WmsConfig.GetOrDefault("Inbound:IsAllowedInbound", "0");
            if (string.Equals(isAllowedInbound, "0"))
            {//清空容器主档货位字段信息，并软删除容器主档
                Container.UpdateHeadActStatus(containerId, "0").UpdateContainerLocation(containerId, string.Empty);
            }
            else
            {//将容器主档中货位字段的数据改成暂存区对应的货位
                DataRow drInboundBuffer = new Location().GetInboundBufferLocation();
                Container.UpdateContainerLocation(containerId, drInboundBuffer["t_id"].ToString());
            }
            bool isEmptyLoc = ce.IsLineEmpty(row);//IsLineEmpty    GetPlanLocNumByLine
            //if (isHavePlanLoc == false)
            //{
            //    ce.UpdatelocationflagByRow(row, LocationFlag.LocationFlag_Usable.ToString());
            //}
            if (isEmptyLoc == true)
            {
                ce.UpdatelocationflagByRow(row, LocationFlag.LocationFlag_Usable.ToString());
            }
        }

        /// <summary>
        /// 回收实托盘入库货位二次分配后的任务
        /// </summary>
        /// <param name="taskRow">任务主档信息</param>
        /// <returns></returns>
        public void RecycleInBoundSecoundDistributionLocationTask(DataRow taskRow)
        {
            string tid = taskRow["t_id"].ToString();

            LocationModel locRow = Location.GetLocationById(taskRow["dest_addr_id"].ToString());
            string row = locRow.location_row.ToString();
            string containerId = taskRow["container_id"].ToString();
            string zoneId = taskRow["zone_id"].ToString();
            NtiCloud.Service.WMS.Base.CallEnd ce = new Base.CallEnd();
            ce.UpdateLocStatusByLocationId(locRow.t_id, LocationStatus.LocationStatus_Plan.ToString(), LocationStatus.LocationStatus_In.ToString(), "");
            bool isEmptyLoc = ce.IsLineEmpty(row);//IsLineEmpty    GetPlanLocNumByLine
            //if (isHavePlanLoc == false)
            //{
            //    ce.UpdatelocationflagByRow(row, LocationFlag.LocationFlag_Usable.ToString());
            //}
            if (isEmptyLoc == true)
            {
                ce.UpdatelocationflagByRow(row, LocationFlag.LocationFlag_Usable.ToString());
            }
            else
            {
                //获取预分配最大的货位，并修改成空状态
                DataTable _dt = ce.GetLocByLine(row, true);
                string _locId = _dt.Rows[0]["t_id"].ToString();
                ce.UpdateLocStatusByLocationId(_locId, LocationStatus.LocationStatus_Empty.ToString(), LocationStatus.LocationStatus_Plan.ToString(), "");
                //清空容器主档货位字段信息，并软删除容器主档
                Container.UpdateHeadActStatus(containerId, "0").UpdateContainerLocation(containerId, string.Empty);
            }
        }

        /// <summary>
        /// 回收托盘入库任务
        /// <br>会删除任务对应的容器信息</br>
        /// </summary>
        /// <param name="taskRow"></param>
        /// <returns></returns>
        public IWorkTask RecycleContainerInTask(DataRow taskRow)
        {
            string row = taskRow["dest_addr_text"].ToString();
            string zoneId = taskRow["zone_id"].ToString();
            string refDocType = taskRow["ref_doc_type"].ToString();
            string twoFloorZoneId = WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336");//二楼区域
            if (string.Equals(zoneId, twoFloorZoneId))
            {//恩施专用,用于一次和二次货位分配，实托盘入库的任务回收
                //修改任务软删除标记为“0”
                WorkTask.UpdateActStatus(taskRow["t_id"].ToString(), "0");
                if (row.Length <= 3)//一次货位分配
                {
                    RecycleInBoundFirstDistributionLocationTask(taskRow);
                }
                else//二次货位分配
                {
                    RecycleInBoundSecoundDistributionLocationTask(taskRow);
                }
            }
            else
            {
                MoveToHistory(taskRow["t_id"].ToString());
                LocationModel locRow = Location.GetLocationById(taskRow["dest_addr_id"].ToString());
                Location.UpdateStatus(locRow.t_id,
                    LocationStatus.LocationStatus_Empty.ToString(),
                    locRow.location_status);

                DataRow drInboundBuffer = new Location().GetInboundBufferLocation();
                //Container.UpdateContainerLocation(taskRow["container_id"].ToString(), drInboundBuffer["t_id"].ToString());
                Container.UpdateContainerLocation(taskRow["container_id"].ToString(), "");
                Container.UpdateHeadActStatus(taskRow["container_id"].ToString(), "0");
            }
            return this;
        }

        /// <summary>
        /// 回收生产入库任务
        /// <br>扣减生产计划对应的以及公量</br>
        /// </summary>
        /// <param name="taskRow"></param>
        /// <returns></returns>
        public IWorkTask RecycleProductContainerInTask(DataRow taskRow)
        {

            List<TaskItemModel> lstTaskItem = getTaskItems(taskRow["t_id"].ToString());

            MoveToHistory(taskRow["t_id"].ToString());
            ProdPlanIn planin = new ProdPlanIn();
            Container container = new Container();
            foreach (TaskItemModel ti in lstTaskItem)
            {//只删除生产入库明细，保留容器投标
                Container.DeleteDetail(ti.container_d_id);
                //扣减生产计划已完成量
                planin.AddFinishQty(ti.ref_doc_id, -(ti.plan_qty ?? 0));
                List<Container_SN> lstSn = container.GetContainerSnListByCtndetailid(ti.container_d_id);
                foreach (Container_SN sn in lstSn)
                {
                    container.MoveSnToLog(sn, null, "任务回收", "删除", taskRow["ref_doc_no"].ToString(), taskRow["task_no"].ToString());
                }
            }
            //还原货位状态为空
            LocationModel locRow = Location.GetLocationById(taskRow["dest_addr_id"].ToString());
            Location.UpdateStatus(locRow.t_id,
                LocationStatus.LocationStatus_Empty.ToString(),
                locRow.location_status);

            DataRow drInboundBuffer = new Location().GetInboundBufferLocation();
            Container.UpdateContainerLocation(taskRow["container_id"].ToString(), drInboundBuffer["t_id"].ToString());


            return this;
        }

        public IWorkTask RecycleTask(DataRow taskRow)
        {
            string taskState = taskRow["task_status"].ToString();
            if (taskState != TaskStatus.TaskStatus_NotExecute.ToString()
                && taskState != TaskStatus.TaskStatus_WaitingExecute.ToString()
                && taskState != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(311, $"任务{taskRow["task_no"]}不可回收!");
            }
            if (taskState == TaskStatus.TaskStatus_Executing.ToString()
                && taskRow[t.ExecutorType].ToString() == TaskExecutor.TaskExecutor_WCS.ToString())
            {
                string recycleInfo = RecycleWcsTask(taskRow["t_id"].ToString());
                if (!string.IsNullOrEmpty(recycleInfo))
                {
                    throw new Exception("回收WCS任务失败:" + recycleInfo);
                }
            }

            switch (taskRow["ref_doc_type"].ToString())
            {
                case "DocType_InBound"://上架任务
                    RecycleInboundTask(taskRow);
                    break;
                case "DocType_EmptyContainerIn"://空托盘入库任务
                    RecycleEmptyContainerInTask(taskRow);
                    break;
                case "DocType_ContainerIn"://托盘入库任务
                case "DocType_ReceiptGroup":
                    RecycleContainerInTask(taskRow);
                    break;
                case "DocType_ProductPlanIn"://生产入库任务
                    RecycleProductContainerInTask(taskRow);
                    break;
                case "DocType_Wave":
                    RecycleWaveTypeTask(taskRow);
                    break;
                case "DocType_Retrieval": //下架任务
                    new Retrieval.Retrieval().RecyclePetrivalTask(taskRow);
                    break;
                case "DocType_Count": //盘点任务
                    new CountContainer.CountContainer().RecycleCountTask(taskRow);
                    break;
                case "DocType_PickOrderBack": //备料回库
                    new RFPick.RFPick().RecyclePickOrderBack(taskRow);
                    break;
                case "DocType_PickSurpluBack": //余料回库
                    new RFPick.RFPick().RecyclePickSurpluBack(taskRow);
                    break;
                case "DocType_CallTerminal"://机台要料
                    new CallEnd.MachineMaterialBusinessProce().RecycleMachineMaterial(new RecycleMachineMaterialModel()
                    {
                        TaskTid = taskRow["t_id"].ToString()
                    });
                    break;
                case "DocType_ClientReturnPallet"://退余料
                    new CallEnd.MachineMaterialBusinessProce().RecycleClientReturnPallet(new RecycleClientReturnPalletModel()
                    {
                        TaskTid = taskRow["t_id"].ToString()
                    });
                    break;
                case "DocType_PersonReceiveMateriel"://人工领料
                    new CallEnd.MachineMaterialBusinessProce().RecyclePersonReceiveMaterielTask(new RecyclePersonReceiveMaterielTaskModel()
                    {
                        TaskTid = taskRow["t_id"].ToString()
                    });
                    break;
                case "DocType_PersonReturnMateriel"://人工退料
                    new CallEnd.MachineMaterialBusinessProce().RecyclePersonReturnMaterielTask(new RecyclePersonReturnMaterielTaskModel()
                    {
                        TaskTid = taskRow["t_id"].ToString()
                    });
                    break;
                case "DocType_ClientReturnNullPallet"://退空托
                    new CallEnd.MachineMaterialBusinessProce().RecycleClientReturnNullPallet(new RecycleClientReturnNullPalletModel()
                    {
                        TaskTid = taskRow["t_id"].ToString()
                    });
                    break;
                case "DocType_ReturnBadPallet"://退坏料
                    new CallEnd.MachineMaterialBusinessProce().RecycleClientReturnBadPallet(new RecycleClientReturnBadPalletModel()
                    {
                        TaskTid = taskRow["t_id"].ToString()
                    });
                    break;
                case "DocType_ClientReturnWaste"://退废料
                    new CallEnd.MachineMaterialBusinessProce().RecycleReturnWasteMaterials(new RecycleReturnWasteMaterialsModel()
                    {
                        TaskTid = taskRow["t_id"].ToString()
                    });
                    break;
                case "DocType_ProductTransfer"://成品转移
                    new CallEnd.MachineMaterialBusinessProce().RecycleProductTransferTask(new RecycleProductTransferTaskModel()
                    {
                        TaskTid = taskRow["t_id"].ToString()
                    });
                    break;
                case "DocType_Adjustment"://手工库位调整
                    new LocationAdjust.LocationAdj().RecycleLocationadJustTask(new RecycleLocationadJustTaskModel()
                    {
                        TaskTid = taskRow["t_id"].ToString()
                    });
                    break;
                case "DocType_AutoAdjustment"://自动库位调整
                    new LocationAdjust.LocationAdj().RecycleAutoLocationadJustTask(new RecycleAutoLocationadJustTaskModel()
                    {
                        TaskTid = taskRow["t_id"].ToString()
                    });
                    break;
                case "DocType_Qc"://质检
                    new QcBound().RecycleQcTask(taskRow);
                    break;
                case "DocType_EmptyContainerOut"://空托盘出库
                    RecycleEmptyContainerOutTask(taskRow);
                    break;
                default:
                    //RecycleOtherTask(taskRow);
                    throw new WmsException(312, "任务类型错误:暂不支持任务回收");
                    break;
            }

            return this;
        }

        public IWorkTask RevertTask(DataRow taskRow)
        {
            string task_state = taskRow["task_status"].ToString();
            if (task_state != "TaskStatus_WaitingExecute" && task_state != TaskStatus.TaskStatus_Create.ToString())
            {
                throw new WmsException(311, $"任务{taskRow["task_no"]}不可回收!");
            }
            string recycleInfo = WorkTaskInterface.Recycle(taskRow["t_id"].ToString());
            if (string.IsNullOrEmpty(recycleInfo))
            {
                throw new WmsException(311, $"任务{taskRow["task_no"]}回收WCS失败:" + recycleInfo);
            }
            switch (taskRow["ref_doc_type"].ToString())
            {
                //case "DocType_InBound":
                //    RecycleInboundTask(taskRow);
                //    break;
                case "DocType_Wave":
                    RecycleWaveTypeTask(taskRow);
                    break;
                default:
                    //RecycleOtherTask(taskRow);
                    throw new WmsException(312, "任务类型错误");
            }

            return this;
        }

        public void RecycleTask(string taskNoOrID)
        {
            DataRow taskRow = GetWorkTask(taskNoOrID);

            RecycleTask(taskRow);
        }

        public IWorkTask RecycleTasks(string taskNoOrIds)
        {
            //List<string> taskList = new List<string>(taskNoOrIds.Split(","));

            DataRow[] taskRows = WorkTask.GetWorkTasksWithProcess(taskNoOrIds);

            return RecycleTasks(taskRows);
        }

        public IWorkTask RecycleTasks(DataRow[] taskRows)
        {
            int successNum = 0;
            int errNum = 0;
            StringBuilder errMsg = new StringBuilder();

            taskRows = taskRows.OrderByDescending(r => r["t_id"].ToString()).ToArray();

            //CurrentUser.RollbackWork();
            foreach (DataRow row in taskRows)
            {
                try
                {
                    //CurrentUser.BeginWork();

                    RecycleTask(row);

                    successNum++;

                    //CurrentUser.CommitWork();
                }
                catch (Exception ex)
                {
                    if (ex is WmsException)
                    {
                        errMsg.AppendLine(ex.Message);
                    }
                    else
                    {
                        errMsg.AppendLine("系统错误:" + ex.Message);
                        LogHelper.LogError(ex.Message);
                    }
                    errNum++;

                    //CurrentUser.RollbackWork();
                }
            }
            //CurrentUser.BeginWork();

            if (errNum == 0)
            {
                return this;
            }
            /*
            else if (successNum > 0)
            {
                throw new WmsException(-777,
                    $"部分回收成功！成功{successNum}笔，失败{errNum}笔\n" + errMsg.ToString());
            }
            */
            else
            {
                throw new WmsException(313, $"回收失败！\n" + errMsg.ToString());
            }
        }

        public IWorkTask RemoveWaveTasks(string taskNoOrIds)
        {
            DataRow[] taskRows = WorkTask.GetWorkTasks(taskNoOrIds);
            return RemoveWaveTasks(taskRows);
        }

        public IWorkTask RemoveWaveTasks(DataRow[] taskRows)
        {
            taskRows
                .OrderByDescending(r => r["t_id"].ToString())
                .ToList()
                .ForEach(r => RemoveWaveTask(r));
            return this;
        }

        readonly string updateStatusForCompactLocation = WmsConfig.GetOrDefault("Outbound:UpdateStatusForCompactLocation", "0");
        private void RemoveWaveTask(DataRow taskRow)
        {
            if (taskRow[t.RefDocType].ToString() != DocType.DocType_Wave.ToString())
            {
                throw new WmsException(-657, "仅能删除波次任务");
            }
            string status = taskRow[t.TaskStatus].ToString();
            if (status != TaskStatus.TaskStatus_Create.ToString())
            {
                throw new WmsException(-652, $"只有新建状态任务才可以进行此操作！任务号{taskRow["task_no"]}");
            }
            DataRow[] dependentRows = GetDependentTasks(taskRow[t.TaskNo].ToString());
            if (dependentRows.Length > 0)
            {
                throw new WmsException(500, $"任务{taskRow[t.TaskNo]}存在依赖任务，无法删除");
            }

            // 修改波次任务
            //Wave.AddHeadLeftTaskQty(taskRow["ref_doc_id"].ToString(), -1);

            DataRow[] taskItemRows = WorkTaskItem.GetByTaskID(taskRow["t_id"].ToString());
            foreach (DataRow row in taskItemRows)
            {
                RemoveTaskItem(taskRow, row);
                decimal qty = decimal.Parse("0" + row[ti.PlanQty].ToString());
                if (string.IsNullOrWhiteSpace(taskRow[t.DependentTaskNo].ToString()))
                {
                    Wave.AddDetailProcessedQty(row[ti.RefDocDetailID].ToString(), -qty);
                }
            }

            if (taskRow[t.SrcAddrType].ToString() == AddressType.AddressType_Location.ToString())
            {
                DataRow locRow = Location.Get(taskRow[t.SrcAddrID].ToString());
                if (Location.IsRackLocation(locRow) ||
                    (Location.IsCompactLocation(locRow) && updateStatusForCompactLocation == "1"))
                {
                    Location.UpdateStatus(locRow[l.ID].ToString(),
                        LocationStatus.LocationStatus_Stored.ToString(),
                        LocationStatus.LocationStatus_Out.ToString());
                }
            }
            // 删除工作任务
            UpdateActStatus(taskRow["t_id"].ToString(), "0");

            // 根据情况重置波次
            //CheckAndResetWave(taskRow);
        }

        public IWorkTask RevertWaveTypeTask(DataRow taskRow)
        {
            string taskType = taskRow[t.TaskType].ToString();
            if (taskRow[t.RefDocType].ToString() != DocType.DocType_Wave.ToString())
            {
                throw new WmsException(-657, "仅能回收波次任务");
            }

            string status = taskRow[t.TaskStatus].ToString();
            if (status != TaskStatus.TaskStatus_NotExecute.ToString()
                && status != TaskStatus.TaskStatus_WaitingExecute.ToString())
            {
                throw new WmsException(-652, $"只有未执行/待执行任务才可以回收！任务号{taskRow["task_no"]}");
            }

            DataRow[] dependentRows = GetDependentTasks(taskRow[t.TaskNo].ToString());
            if (dependentRows.Length > 0)
            {
                throw new WmsException(500, $"任务{taskRow[t.TaskNo]}存在依赖任务，无法删除");
            }

            // 修改波次任务
            //Wave.AddHeadLeftTaskQty(taskRow["ref_doc_id"].ToString(), -1);

            //DataRow[] taskItemRows = WorkTaskItem.GetByTaskID(taskRow["t_id"].ToString());
            //foreach (DataRow row in taskItemRows)
            //{
            //    RemoveTaskItem(taskRow, row);
            //}

            //DataRow[] dependentRows = GetDependentTasks(taskRow["task_no"].ToString());
            //foreach (DataRow row in dependentRows)
            //{
            //    RecycleWaveTypeTask(row);
            //}
            // 删除工作任务
            //DeleteWorkTask(taskRow["t_id"].ToString());
            //UpdateActStatus(taskRow["t_id"].ToString(), "0");
            UpdateTaskStatus(taskRow[t.ID].ToString(), TaskStatus.TaskStatus_Create.ToString(),
                taskRow[t.TaskStatus].ToString());

            // 根据情况修改波次状态
            //CheckAndRestoreWaveEditableStatus(taskRow);
            // 根据情况删除波次
            //DataRow[] leftRows = GetByRefDocID(taskRow["ref_doc_id"].ToString());
            //if (leftRows.Length <= 0)
            //{
            //    Wave.UpdateHeadState(taskRow["ref_doc_id"].ToString(), WaveStatus.WaveStatus_New.ToString());
            //    DataRow waveRow = Wave.GetWaveHead(taskRow["ref_doc_id"].ToString());
            //    if (waveRow[wh.WaveType].ToString() == WaveType.WaveType_AutoFromOrder.ToString())
            //    {
            //        Wave.DeleteWave(waveRow[wh.ID].ToString());
            //    }
            //}
            return this;
        }

        public void RecycleWaveTypeTask(DataRow taskRow)
        {
            RevertWaveTypeTask(taskRow);

            // 修改波次任务
            Wave.AddHeadLeftTaskQty(taskRow["ref_doc_id"].ToString(), -1);

            //UpdateTaskStatus(taskRow[t.ID].ToString(), TaskStatus.TaskStatus_Create.ToString(),
            //    taskRow[t.TaskStatus].ToString());

            // 根据情况修改波次状态
            CheckAndRestoreWaveEditableStatus(taskRow);
        }

        private void CheckAndRestoreWaveEditableStatus(DataRow taskRow)
        {
            DataRow[] leftRows = GetByRefDocID(taskRow[t.RefDocID].ToString());
            leftRows = leftRows.Where(
                r => r[t.TaskStatus].ToString() != TaskStatus.TaskStatus_Create.ToString()
            ).ToArray();
            if (leftRows.Length <= 0)
            {
                Wave.RestoreEditableStatus(taskRow[t.RefDocID].ToString());
            }
        }

        private void CheckAndResetWave(DataRow taskRow)
        {
            DataRow[] leftRows = GetByRefDocID(taskRow[t.RefDocID].ToString());
            if (leftRows.Length <= 0)
            {
                Wave.ResetWave(taskRow[t.RefDocID].ToString());
            }
        }

        public void RecycleWaveTypeTaskV1(DataRow taskRow)
        {
            string taskType = taskRow["task_type"].ToString();
            //if (taskType != TaskType.TaskType_OutBoundToString()) // TODO TaskType.Outbound &&
            //    // TODO taskType != TaskType.Filling &&
            //    // TODO taskType != TaskType.Picking &&
            //    // TODO taskType != TaskType.Barcode &&
            //    // TODO taskType != TaskType.StockUp)
            //{
            //    throw new WmsException(-657, "仅能回收波次任务");
            //}
            if (taskRow[t.RefDocType].ToString() != DocType.DocType_Wave.ToString())
            {
                throw new WmsException(-657, "仅能回收波次任务");
            }

            string state = taskRow["task_status"].ToString();
            if (state != TaskStatus.TaskStatus_NotExecute.ToString()
                && state != TaskStatus.TaskStatus_Create.ToString())
            {
                throw new WmsException(-652, $"只有未执行/待执行任务才可以回收！任务号{taskRow["task_no"]}");
            }

            DataRow[] dependentRows = GetDependentTasks(taskRow[t.TaskNo].ToString());
            if (dependentRows.Length > 0)
            {
                throw new WmsException(500, $"任务{taskRow[t.TaskNo]}存在依赖任务，无法删除");
            }

            //IWave wave = WmsFactory.MakeWave();

            //DataRow waveTaskRow;
            //try
            //{
            //    waveTaskRow = Wave.GetWaveTask(taskRow["task_no"].ToString());
            //}
            //catch (WmsException ex)
            //{
            //    if (ex.ErrorCode != DBHelper.NotFound)
            //    {
            //        throw;
            //    }
            //    waveTaskRow = null;
            //}

            // TODO get from task_item table
            //string ctnDtlID = taskRow["container_d_id"].ToString();
            //decimal opQty = decimal.Parse("0" + taskRow["plan_qty"]);

            // 修改波次任务
            Wave.AddHeadLeftTaskQty(taskRow["ref_doc_id"].ToString(), -1);

            /*
            if (waveTaskRow != null)
            {
                // 修改波次任务
                Wave.UpdateWaveTaskState(taskRow["task_no"].ToString(), "Wave_Editable")
                    .AddHeadLeftTaskQty(taskRow["ref_doc_no"].ToString(), -1);
                //-decimal.Parse("0" + taskRow["operation_qty"]))
                //.UpdateHeadState(taskRow["source_doc_no"].ToString(), "Wave_Editable");

                //DataRow[] waveTaskRows = Wave.GetWaveTasksByHeadID(waveTaskRow["wave_h_id"].ToString());
                //decimal cnt = waveTaskRows.Where(r => r["state_code"].ToString() == "Wave_Released").Count();
                //if (cnt == 0)
                //{
                //    Wave.UpdateHeadState(taskRow["source_doc_no"].ToString(), "Wave_Editable");
                //}

                string locID = waveTaskRow["target_id"].ToString();
                if (!string.IsNullOrWhiteSpace(locID))
                {
                    DataRow locRow = Location.Get(locID);
                    if (locRow["usage"].ToString() == "LocationUse_Backup")
                    {
                        DataRow containerRow = Container.GetHead(locRow["location_no"].ToString());
                        DataRow ctnDtlRow = Container.GetDetail(
                            containerRow["container_id"].ToString(),
                            taskRow["item_id"].ToString());
                        Container.CancelPutawayPreparation(ctnDtlRow["t_id"].ToString(), opQty);
                    }
                }
            }
            else
            */
            //if (taskType == TaskType.Outbound)
            if (taskType == TaskType.TaskType_OutBound.ToString())
            {
                // TODO 这个应该是备货位的处理
                string destAddr = taskRow["dest_addr_id"].ToString();
                if (!string.IsNullOrWhiteSpace(destAddr)
                    && taskRow[t.DestAddrType].ToString() == AddressType.AddressType_Location.ToString())
                {
                    DataRow locRow = Location.Get(destAddr);
                    if (locRow["usage"].ToString() == "LocationUse_Backup")
                    {
                        // TODO get from task_item table
                        //Container.AddDetailOutQty(ctnDtlID, -opQty)
                        //.ClearDetail(ctnDtlID);
                    }
                }
            }
            else if (taskType == "") // TODO TaskType.Barcode)
            {
                // TODO get from task_item table
                //Container.AddDetailOutQty(ctnDtlID, -opQty);
            }

            // 修改波次明细已完成量
            if (state == "TaskStatus_WaitingExecute")
            {

            }
            DataRow[] taskItemRows = WorkTaskItem.GetByTaskID(taskRow["t_id"].ToString());
            foreach (DataRow row in taskItemRows)
            {
                RemoveTaskItem(taskRow, row);
            }

            //DataRow[] dependentRows = GetDependentTasks(taskRow["task_no"].ToString());
            //foreach (DataRow row in dependentRows)
            //{
            //    RecycleWaveTypeTask(row);
            //}
            // 删除工作任务
            //DeleteWorkTask(taskRow["t_id"].ToString());
            UpdateActStatus(taskRow["t_id"].ToString(), "0");

            // 根据情况删除波次
            DataRow[] leftRows = GetByRefDocID(taskRow["ref_doc_id"].ToString());
            if (leftRows.Length <= 0)
            {
                Wave.UpdateHeadStatus(taskRow["ref_doc_id"].ToString(), WaveStatus.WaveStatus_New.ToString());
                DataRow waveRow = Wave.GetWaveHead(taskRow["ref_doc_id"].ToString());
                if (waveRow[wh.WaveType].ToString() == WaveType.WaveType_AutoFromOrder.ToString())
                {
                    Wave.DeleteWave(waveRow[wh.ID].ToString());
                }
            }
        }

        public bool RefDocIsStarted(string srcDocType, string srcDocNo)
        {
            DataRow[] rows = GetByRefDocNoOrID(srcDocType, srcDocNo);

            //rows = rows.Where(r => r[t.TaskStatus].ToString() != TaskStatus.TaskStatus_Create.ToString()
            //        && r[t.TaskStatus].ToString() != TaskStatus.TaskStatus_NotExecute.ToString()
            //        && r[t.TaskStatus].ToString() != TaskStatus.TaskStatus_WaitingExecute.ToString()
            //        ).ToArray();
            rows = rows.Where(r => r[t.TaskStatus].ToString() == TaskStatus.TaskStatus_Executing.ToString()
                    || r[t.TaskStatus].ToString() == TaskStatus.TaskStatus_PartialFinished.ToString()
                    || r[t.TaskStatus].ToString() == TaskStatus.TaskStatus_Finished.ToString()
                    ).ToArray();

            if (rows.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ProcessSrcWhenRemoveTaskItem(DataRow taskItemRow, DataRow taskRow, decimal qty)
        {
            DataRow srcLocRow = GetLocation(taskRow[t.SrcAddrType].ToString(), taskRow[t.SrcAddrID].ToString());
            if (srcLocRow == null)
            {
                return;
            }
            DataRow containerRow = Container.GetHead(taskRow[t.ContainerID].ToString());
            if (containerRow[ch.ContainerType].ToString() == ContainerType.ContainerType_Actual.ToString())
            {
                if (srcLocRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                {
                    Location.UpdateStatus(srcLocRow[l.ID].ToString(),
                        LocationStatus.LocationStatus_Stored.ToString(),
                        LocationStatus.LocationStatus_Out.ToString());
                }
                Container.UpdateHeadIsOut(containerRow[ch.ID].ToString(), "0");
            }
            else
            {
                if (srcLocRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                {
                    throw new WmsException(500, "虚拟容器不应该放在立体库货位上");
                }

                Container.CancelStockingOut(taskItemRow[ti.ContainerDetailID].ToString(), qty);
            }

        }
        private void ProcessDestWhenRemoveTaskItem(DataRow taskItemRow, DataRow taskRow, decimal qty)
        {
            DataRow srcLocRow = GetLocation(taskRow[t.SrcAddrType].ToString(), taskRow[t.SrcAddrID].ToString());
            DataRow destLocRow = GetLocation(taskRow[t.DestAddrType].ToString(), taskRow[t.DestAddrID].ToString());
            if (destLocRow == null)
            {
                return;
            }
            DataRow containerRow = Container.GetHead(taskRow[t.ContainerID].ToString());
            if (containerRow[ch.ContainerType].ToString() == ContainerType.ContainerType_Actual.ToString())
            {
                if (destLocRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                {
                    Location.UpdateStatus(destLocRow[l.ID].ToString(),
                        LocationStatus.LocationStatus_In.ToString(),
                        LocationStatus.LocationStatus_Empty.ToString());
                }
            }
            else
            {
                if (destLocRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                {
                    throw new WmsException(500, "虚拟容器不应该放在立体库货位上");
                }

                DataRow ctnDtlRow = Container.GetDetailByLocation(
                    destLocRow[l.ID].ToString(), taskItemRow[ti.ItemID].ToString());

                if (srcLocRow != null)
                {
                    Container.AddDetailQty(ctnDtlRow[cd.ID].ToString(),
                        -decimal.Parse(taskItemRow["plan_qty"].ToString()));
                }

                Container.CancelPutawayPreparation(ctnDtlRow[cd.ID].ToString(),
                    decimal.Parse(taskItemRow["plan_qty"].ToString()));
            }

        }
        private void RemoveTaskItem(DataRow taskRow, DataRow taskItemRow)
        {
            if (taskItemRow[ti.ActStatus].ToString() == "0")
            {
                throw new WmsException(500, "任务明细无法被再次删除");
            }
            if (taskItemRow[ti.ProcessStatus].ToString() != ProcessStatus.ProcessStatus_New.ToString())
            {
                throw new WmsException(500, "任务明细正在处理，无法删除");
            }

            decimal qty = decimal.Parse("0" + taskItemRow["plan_qty"].ToString());
            //if (string.IsNullOrWhiteSpace(taskRow[t.DependentTaskNo].ToString()))
            //{
            //    Wave.AddDetailProcessedQty(taskItemRow["ref_doc_d_id"].ToString(), -qty);
            //}

            ProcessSrcWhenRemoveTaskItem(taskItemRow, taskRow, qty);
            ProcessDestWhenRemoveTaskItem(taskItemRow, taskRow, qty);
            WorkTaskItem.UpdateActStatus(taskItemRow["t_id"].ToString(), "0");
        }

        private void RemoveTaskItemV1(DataRow taskRow, DataRow row)
        {
            if (row[ti.ActStatus].ToString() == "0")
            {
                throw new WmsException(500, "任务明细无法被再次删除");
            }
            if (row[ti.ProcessStatus].ToString() != ProcessStatus.ProcessStatus_New.ToString())
            {
                throw new WmsException(500, "任务明细正在处理，无法删除");
            }
            WorkTaskItem.UpdateActStatus(row["t_id"].ToString(), "0");
            if (string.IsNullOrWhiteSpace(taskRow[t.DependentTaskNo].ToString()))
            {
                Wave.AddDetailProcessedQty(row["ref_doc_d_id"].ToString(),
                    -decimal.Parse(row["plan_qty"].ToString()));
            }
            if (taskRow[t.SrcAddrType].ToString() == AddressType.AddressType_Location.ToString())
            {
                Container.AddDetailOutQty(row[ti.ContainerDetailID].ToString(),
                    -decimal.Parse(row["plan_qty"].ToString()));

                DataRow locRow = Location.Get(taskRow[t.SrcAddrID].ToString());
                if (locRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                {
                    Location.UpdateStatus(taskRow[t.SrcAddrID].ToString(), LocationStatus.LocationStatus_Stored.ToString());
                }
            }
            if (taskRow[t.DestAddrType].ToString() == AddressType.AddressType_Location.ToString())
            {
                DataRow locRow = Location.Get(taskRow[t.DestAddrID].ToString());
                DataRow ctnDtlRow = Container.GetDetailByLocation(
                    locRow[l.ID].ToString(), row[ti.ItemID].ToString());
                Container.AddDetailInQty(row[ti.ContainerDetailID].ToString(),
                    -decimal.Parse(row["plan_qty"].ToString()));

                if (taskRow[t.SrcAddrType].ToString() == AddressType.AddressType_Location.ToString())
                {
                    Container.AddDetailQty(ctnDtlRow[cd.ID].ToString(),
                        -decimal.Parse(row["plan_qty"].ToString()));
                }

                Container.CancelPutawayPreparation(row[ti.ContainerDetailID].ToString(),
                    -decimal.Parse(row["plan_qty"].ToString()));
            }
        }

        public IWorkTask RemoveTaskItem(DataRow taskItemRow)
        {
            DataRow[] rowsInSameTask = WorkTask.GetTaskItemsBy(taskItemRow[ti.TaskID].ToString());
            if (rowsInSameTask.Length == 1)
            {
                //WorkTask.RecycleTask(WorkTask.Get(taskItemRow[ti.TaskID].ToString()));
                RemoveWaveTask(WorkTask.Get(taskItemRow[ti.TaskID].ToString()));
                return this;
            }

            DataRow taskRow = WorkTask.Get(taskItemRow[ti.TaskID].ToString());
            RemoveTaskItem(taskRow, taskItemRow);

            DataRow[] dependentRows = GetDependentTasks(taskRow["task_no"].ToString());
            foreach (DataRow row in dependentRows)
            {
                taskItemRow = GetTaskItemBy(row[t.ID].ToString(),
                                taskItemRow[ti.ItemID].ToString(),
                                taskItemRow[ti.RefDocDetailID].ToString());
                RemoveTaskItem(taskItemRow);
            }
            return this;
        }

        public TaskItemModel getTaskItem(string taskitemid)
        {
            string sql = @" select t_id, client_id, org_id, warehouse_id, task_id, item_id, item_name, plan_qty, processed_qty, finished_qty, uom_qty_id, finished_stat_qty, uom_stat_id, container_d_id, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, start_date, end_date, process_status, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status
                from &DataSource&wms_b_task_item
                where t_id= @t_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", taskitemid);
            DataTable dt = DBHelper.Select(sql, lstParam, "查询任务明细");
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<TaskItemModel>(dt)[0];
            }
            else
            {
                return null;
            }
        }

        public List<TaskItemModel> getTaskItems(string taskid, bool errorWhenNotFound = true)
        {
            string sql = @" select t_id, client_id, org_id, warehouse_id, task_id, item_id, item_name, plan_qty, processed_qty, finished_qty, uom_qty_id, finished_stat_qty, uom_stat_id, container_d_id, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, start_date, end_date, process_status, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status
                from &DataSource&wms_b_task_item
                where task_id= @task_id and act_status='1'";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("task_id", taskid);
            DataTable dt = DBHelper.Select(sql, lstParam, "查询任务明细", errorWhenNotFound);
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<TaskItemModel>(dt);
            }
            else
            {
                return new List<TaskItemModel>(); ;
            }
        }
        /// <summary>
        /// 获取待执行任务行
        /// </summary>
        /// <param name="taskid">任务ID</param>
        /// <returns></returns>
        public List<TaskItemModel> getTaskItemsForDoing(string taskid)
        {
            string sql = @" select t_id, client_id, org_id, warehouse_id, task_id, item_id, item_name, plan_qty, processed_qty, finished_qty, uom_qty_id, finished_stat_qty, uom_stat_id, container_d_id, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, start_date, end_date, process_status, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status
                from &DataSource&wms_b_task_item
                where task_id= @task_id and act_status='1' 
                        and process_status in ('ProcessStatus_Doing','ProcessStatus_New') ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("task_id", taskid);
            DataTable dt = DBHelper.Select(sql, lstParam, "查询任务明细", false);
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<TaskItemModel>(dt);
            }
            else
            {
                return new List<TaskItemModel>(); ;
            }
        }

        /// <summary>
        /// 查询待上架任务
        /// </summary>
        /// <param name="item_id">物料编码</param>
        /// <returns></returns>
        public List<TaskItemModel> getTaskItemsForDoingByItem(string warehouseid, string item_code)
        {
            Item item = new Item();
            DataRow dr = item.Get(warehouseid, item_code);

            string sql = @" select t_id, client_id, org_id, warehouse_id, task_id, item_id, item_name, plan_qty, processed_qty, finished_qty, uom_qty_id, finished_stat_qty, uom_stat_id, container_d_id, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, start_date, end_date, process_status, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status
                from &DataSource&wms_b_task_item
                where item_id= @item_id  and process_status in ('ProcessStatus_Doing','ProcessStatus_New') ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("item_id", dr["t_id"].ToString());
            DataTable dt = DBHelper.Select(sql, lstParam, "查询任务明细");
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<TaskItemModel>(dt);
            }
            else
            {
                return new List<TaskItemModel>(); ;
            }
        }
        public List<TaskItemForRfModel> getTaskItemsForDoingRF(string warehouseid, string item_code)
        {
            Item item = new Item();
            DataRow dr = item.Get(warehouseid, item_code);

            string sql = @" select i.t_id,i.client_id,i. org_id,i. warehouse_id,i. task_id,i. item_id,i.item_name,i.plan_qty,i. processed_qty,i. finished_qty,i. uom_qty_id,i. finished_stat_qty,i. uom_stat_id,i. container_d_id,i. ref_doc_type,i. ref_doc_id,i. ref_doc_d_id,i. ref_doc_no,i. start_date,i. end_date,i. process_status,i. create_date,i. created_by,i. modify_ip,i. modify_date,i. last_modified_by,i. act_status,
                                  i.plan_qty-nvl(i.finished_qty,0) as waiting_qty,
                                  t.dest_addr_text to_location
                            from  &DataSource&wms_b_task_item i inner join &DataSource&wms_b_task t on i.task_id=t.t_id
                            where i.item_id= @item_id  and i.process_status in ('ProcessStatus_Doing','ProcessStatus_New')
                                and t.ref_doc_type='DocType_Inbound' ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("item_id", dr["t_id"].ToString());
            DataTable dt = DBHelper.Select(sql, lstParam, "查询任务明细");
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<TaskItemForRfModel>(dt);
            }
            else
            {
                return new List<TaskItemForRfModel>(); ;
            }
        }
        /// <summary>
        /// 将任务状态更新为可执行
        /// </summary>
        /// <returns></returns>
        public bool UpdatetaskAvailable(string putawayid)
        {
            string sql = @"update &DataSource&wms_b_task t set t.task_status=@task_status,
                                        t.is_executable='1',
                                        t.modify_ip=@modify_ip,
                                        t.modify_date=sysdate,
                                        t.last_modified_by=@last_modified_by
                        where t.ref_doc_id=@ref_doc_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("task_status", TaskStatus.TaskStatus_NotExecute.ToString());
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserId);
            lstParam.Add("ref_doc_id", putawayid);
            return DBHelper.RunSql(sql, lstParam, "上架单货位确认时释放任务", DBHelper.OperatedNumType.OneOrMore) > 0;
        }

        public bool CompleteTask(string putawayid)
        {
            string sql = @"select t_id, client_id, org_id, warehouse_id, task_id, item_id, item_name, plan_qty, processed_qty, finished_qty, uom_qty_id, finished_stat_qty, uom_stat_id, container_d_id, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, start_date, end_date, process_status, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status
                    from &DataSource&wms_b_task_item
                    where ref_doc_id=@ref_doc_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("ref_doc_id", putawayid);
            DataTable dt = DBHelper.Select(sql, lstParam, "手工完成任务");

            List<TaskItemModel> lstTaskitem = DBHelper.ConvertTableToObject<TaskItemModel>(dt);

            return true;
        }

        /// <summary>
        /// 根据来源单号ID获取人、为完成的任务
        /// </summary>
        /// <param name="taskrefid"></param>
        /// <returns></returns>
        public List<TaskHeaderModel> getUnCompleteTask(string taskrefid)
        {
            string sql = @"select t_id, client_id, org_id, warehouse_id, zone_id, task_no, dependent_task_no, task_type, executed_by, equipment_id, executor_type, actual_equipment_id, seq_no, priority, container_id, container_barcode, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, src_addr_id, src_addr_type, src_addr_text, dest_addr_id, dest_addr_type, dest_addr_text, actual_dest_addr_id, actual_dest_addr_text, task_status, start_date, end_date, is_executable, priority_change_time, led_text, exist_lgv, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status
                        from &DataSource&wms_b_task
                        where act_status='1' and task_status in('" + TaskStatus.TaskStatus_NotExecute.ToString() + "','" + TaskStatus.TaskStatus_Executing.ToString() + "') and ref_doc_id=@ref_doc_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("ref_doc_id", taskrefid);
            DataTable dt = DBHelper.Select(sql, lstParam);
            return DBHelper.ConvertTableToObject<TaskHeaderModel>(dt);
        }
        /// <summary>
        /// 自动完成入库任务
        /// <br></br>
        /// 更新任务状态与库存数量
        /// </summary>
        /// <param name="task">入库任务</param>
        /// <returns></returns>
        public bool ConpleteTask_Inbound_Auto(TaskHeaderModel task)
        {
            string[] allowStatus = new string[] { TaskStatus.TaskStatus_NotExecute.ToString(), TaskStatus.TaskStatus_Executing.ToString() };
            if (!allowStatus.Contains(task.task_status))
            {
                throw new Exception("只能自动完成可执行、执行中的任务");
            }
            List<TaskItemModel> lstItems = getTaskItemsForDoing(task.t_id);
            Putaway putaway = new Putaway(this.dbW);
            foreach (TaskItemModel ti in lstItems)
            {
                if (string.Equals(ti.process_status, ProcessStatus.ProcessStatus_Done.ToString()))
                {
                    continue;
                }
                PutawayDetailModel putdetail = putaway.getPutawayDetail(ti.ref_doc_d_id);
                if (ti.plan_qty > (ti.finished_qty ?? 0))
                {
                    decimal thisqty = (ti.plan_qty ?? 0) - (ti.finished_qty ?? 0);
                    putaway.ProcessPutawaydetailQty(ti.ref_doc_d_id, thisqty);
                    CompleteTaskItem_Inbound(ti, thisqty, (putdetail.plan_stat_qty ?? 0) == 0 ? 0 : ((putdetail.plan_qty ?? 0) / (putdetail.plan_stat_qty ?? 0)));
                }
            }
            #region 更新货位存储状态
            Location location = new Location();
            LocationModel loc = location.GetLocationById(task.dest_addr_id);
            if (string.Equals(loc.location_type, LocationType.LocationType_FlatStock.ToString()))
            {
                Container container = new Container();
                int locationHoldQty = location.GetLocationHoldCtns(loc.location_code);
                if (locationHoldQty >= loc.qty_limit)
                {
                    location.UpdateLocationStatus(loc.t_id, LocationStatus.LocationStatus_Full);
                }
                else
                {
                    location.UpdateLocationStatus(loc.t_id, LocationStatus.LocationStatus_Partial);
                }
            }
            else
            {
                location.UpdateLocationStatus(loc.t_id, LocationStatus.LocationStatus_Stored);
            }
            #endregion
            if (getTaskItemsForDoing(task.t_id).Count == 0)
            {
                UpdateTaskStatus(task.t_id, TaskStatus.TaskStatus_Finished.ToString());
                MoveToHistory(task.t_id);
            }
            return true;
        }

        /// <summary>
        /// 关闭单个入库任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool CloseTask_Inbound_Auto(TaskHeaderModel task)
        {
            string[] allowStatus = new string[] { TaskStatus.TaskStatus_NotExecute.ToString(), TaskStatus.TaskStatus_Executing.ToString() };
            if (!allowStatus.Contains(task.task_status))
            {
                throw new Exception("只能自动完成可执行、执行中的任务");
            }
            List<TaskItemModel> lstItems = getTaskItems(task.t_id);
            foreach (TaskItemModel ti in lstItems)
            {
                if (string.Equals(ti.process_status, ProcessStatus.ProcessStatus_Done.ToString()))
                {
                    continue;
                }
                if (ti.plan_qty > ti.finished_qty)
                {
                    CloseTaskItem_Inbound(ti);
                }
            }

            return true;
        }


        /// <summary>
        /// 完成单个任务明细，并更新库存
        /// </summary>
        /// <param name="ti">任务行</param>
        /// <param name="processQty">本次上架数量</param>
        /// <param name="ratio">统计数量转化因子</param>
        /// <returns></returns>
        public bool CompleteTaskItem_Inbound(TaskItemModel ti, decimal processQty, decimal ratio)
        {
            if (processQty <= 0)
            {
                throw new Exception("待处理入库数量必须大于0");
            }
            if ((ti.finished_qty ?? 0) + processQty >= ti.plan_qty)
            { //任务完成
                ti.processed_qty = (ti.processed_qty ?? 0) + processQty;
                ti.finished_qty = (ti.finished_qty ?? 0) + processQty;
                ti.finished_stat_qty = ti.finished_stat_qty + processQty * ratio;
                ti.process_status = ProcessStatus.ProcessStatus_Done.ToString();
                ti.end_date = DateTime.Now;
                ti.start_date = ti.start_date ?? DateTime.Now;
            }
            else
            {
                ti.processed_qty = (ti.processed_qty ?? 0) + processQty;
                ti.finished_qty = (ti.finished_qty ?? 0) + processQty;
                ti.finished_stat_qty = ti.finished_stat_qty + processQty * ratio;
                ti.process_status = ProcessStatus.ProcessStatus_Doing.ToString();
                ti.start_date = ti.start_date ?? DateTime.Now;
            }
            Container ctn = new Container();
            ContainerDModel ctnd = ctn.getContainerDetailById(ti.container_d_id);
            PutawayDetailModel putawayDetail = new Putaway().getPutawayDetail(ti.ref_doc_d_id);
            ctn.ContainerItemInbound(ctnd, processQty, ratio);
            return UpdateTaskitemStatus(ti);
        }

        /// <summary>
        /// 车载上架
        /// </summary>
        /// <param name="ti">任务明细行</param>
        /// <param name="lstSN">扫描的箱码</param>
        /// <param name="processQty">上架数量</param>
        /// <returns></returns>
        public bool CompleteTaskItem_Inbound(TaskItemModel ti, List<NtiCloud.Model.WMS.SNModel> lstSN, decimal processQty)
        {
            if (lstSN != null && lstSN.Count > 0 && lstSN.Count != processQty)
            {
                throw new Exception("扫描的箱码与入库数量不一致");
            }
            Container ctn = new Container();
            ContainerDModel ctndetail = ctn.getContainerDetailById(ti.container_d_id);
            ContainerModel ctnm = ctn.getContainerById(ctndetail.container_id);
            TaskHeaderModel task = GetTaskHeader(ti.task_id);
            Putaway putaway = new Putaway();
            PutawayDetailModel putdetail = putaway.getPutawayDetail(ti.ref_doc_d_id);
            ctn.AddSNIntoContainer(ref task, ctnm, ctndetail, lstSN);
            CompleteTaskItem_Inbound(ti, processQty, (putdetail.plan_stat_qty ?? 0) / (putdetail.plan_qty ?? 1));

            if (CheckTaskComplete(ti.task_id))
            {
                UpdateTaskComplete(ti.task_id);
            }
            else
            {
                UpdateTaskDoing(ti.task_id);
            }
            return true;
        }
        /// <summary>
        /// 获取任务信息
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public TaskHeaderModel GetTaskHeader(string taskid)
        {
            string sql = @" select t_id, client_id, org_id, warehouse_id, zone_id, task_no, dependent_task_no, task_type, executed_by, equipment_id, executor_type, actual_equipment_id, seq_no, priority, container_id, container_barcode, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, src_addr_id, src_addr_type, src_addr_text, dest_addr_id, dest_addr_type, dest_addr_text, actual_dest_addr_id, actual_dest_addr_text, task_status, start_date, end_date, is_executable, priority_change_time, led_text, exist_lgv, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status
                        from &DataSource&wms_b_task where t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", taskid);
            DataTable dt = DBHelper.Select(sql, lstParam);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("获取任务失败");
            }
            return DBHelper.ConvertTableToObject<TaskHeaderModel>(dt)[0];
        }

        public TaskHeaderModel GetTaskHeaderByNo(string taskno)
        {
            string sql = @" select t_id, client_id, org_id, warehouse_id, zone_id, task_no, dependent_task_no, task_type, executed_by, equipment_id, executor_type, actual_equipment_id, seq_no, priority, container_id, container_barcode, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, src_addr_id, src_addr_type, src_addr_text, dest_addr_id, dest_addr_type, dest_addr_text, actual_dest_addr_id, actual_dest_addr_text, task_status, start_date, end_date, is_executable, priority_change_time, led_text, exist_lgv, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status
                        from &DataSource&wms_b_task where task_no = @task_no";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("task_no", taskno);
            DataTable dt = DBHelper.Select(sql, lstParam);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("获取任务失败");
            }
            return DBHelper.ConvertTableToObject<TaskHeaderModel>(dt)[0];
        }

        /// <summary>
        /// 关闭单个任务行，并回滚容器待入库数量
        /// </summary>
        /// <param name="ti"></param>
        /// <param name="processQty"></param>
        /// <returns></returns>
        public bool CloseTaskItem_Inbound(TaskItemModel ti)
        {
            Container ctn = new Container();
            ContainerDModel ctnd = ctn.getContainerDetailById(ti.container_d_id);
            ctn.RollbackContainerInqty(ctnd, (ti.plan_qty ?? 0) - (ti.finished_qty ?? 0));
            return CLoseTaskitemStatus(ti);
        }

        /// <summary>
        /// 更新任务状态为已完成
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public bool UpdateTaskComplete(string taskid)
        {
            string sql = @"  update &DataSource&wms_b_task t set t.task_status = @task_status,
                                                  t.start_date = nvl(start_date,sysdate),
                                                  t.end_date = sysdate,
                                                  t.modify_ip = @modify_ip,
                                                  t.last_modified_by= @last_modified_by,
                                                  t.modify_date = sysdate
                              where t.t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", taskid);
            lstParam.Add("task_status", TaskStatus.TaskStatus_Finished.ToString());
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            return DBHelper.RunSql(sql, lstParam, "更新任务为已完成") > 0;
        }

        /// <summary>
        /// 根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
        /// </summary>
        /// <param name="taskid">任务物料id</param>
        /// <returns></returns>
        public bool UpdateTaskItemCompleteAndQty(string taskItemid)
        {
            string sql = @"  update &DataSource&wms_b_task_item t
                                   set t.process_status   = @process_status,
                                   t.processed_qty    = t.plan_qty,
                                   t.finished_qty     = t.plan_qty,
                                   t.start_date       = sysdate,
                                   t.end_date         = sysdate,
                                   t.modify_ip        = @modify_ip,
                                   t.last_modified_by = @last_modified_by,
                                   t.modify_date      = sysdate
                             where t.t_id = @taskItemid";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("taskItemid", taskItemid);
            lstParam.Add("process_status", ProcessStatus.ProcessStatus_Done.ToString());
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            return DBHelper.RunSql(sql, lstParam, "更新任务物料为已完成") > 0;
        }
        /// <summary>
        /// 更新任务状态为执行中
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public bool UpdateTaskDoing(string taskid)
        {
            string sql = @"  update &DataSource&wms_b_task t set t.task_status = @task_status,
                                                  t.start_date = nvl(start_date,sysdate),
                                                  t.modify_ip = @modify_ip,
                                                  t.last_modified_by= @last_modified_by,
                                                  t.modify_date = sysdate
                              where t.t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", taskid);
            lstParam.Add("task_status", TaskStatus.TaskStatus_Executing.ToString());
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            return DBHelper.RunSql(sql, lstParam, "更新任务为已完成") > 0;
        }
        /// <summary>
        /// 更新任务状态为部分完成
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public bool UpdateTaskPartionComplete(string taskid)
        {
            string sql = @"  update &DataSource&wms_b_task t set t.task_status = @task_status, 
                                                  t.modify_ip = @modify_ip,
                                                  t.last_modified_by= @last_modified_by,
                                                  t.modify_date = sysdate
                              where t.t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", taskid);
            lstParam.Add("task_status", TaskStatus.TaskStatus_PartialFinished.ToString());
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            return DBHelper.RunSql(sql, lstParam, "更新任务为部分完成") > 0;
        }
        public bool UpdateTaskitemStatus(TaskItemModel ti)
        {
            string sql = @"update &DataSource&wms_b_task_item   set start_date=to_date(@start_date,'yyyy-MM-dd HH24:mi:ss'),
                                                   end_date=to_date(@end_date,'yyyy-MM-dd HH24:mi:ss'),
                                                   processed_qty=@processed_qty,
                                                   finished_qty=@finished_qty,
                                                   process_status=@process_status,
                                                   last_modified_by=@last_modified_by,
                                                   modify_ip=@modify_ip,
                                                   modify_date=sysdate
                            where t_id=@t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("start_date", FormateDateStr(ti.start_date));
            lstParam.Add("end_date", FormateDateStr(ti.end_date));
            lstParam.Add("processed_qty", (ti.processed_qty ?? 0).ToString());
            lstParam.Add("finished_qty", (ti.finished_qty ?? 0).ToString());
            lstParam.Add("process_status", ti.process_status);
            lstParam.Add("last_modified_by", Context.UserInfo.UserId);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("t_id", ti.t_id);

            return DBHelper.RunSql(sql, lstParam, "更新任务行状态") > 0;

        }
        /// <summary>
        /// 关闭任务明细：只将任务状态更新为已完成
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        private bool CLoseTaskitemStatus(TaskItemModel ti)
        {
            string sql = @"update &DataSource&wms_b_task_item ti  set    ti.process_status=@process_status,
                                                   ti.last_modified_by=@last_modified_by,
                                                   ti.modify_ip=@modify_ip,
                                                   ti.modify_date=sysdate
                            where ti.t_id=@t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("process_status", ProcessStatus.ProcessStatus_Done.ToString());
            lstParam.Add("last_modified_by", Context.UserInfo.UserId);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("t_id", ti.t_id);

            return DBHelper.RunSql(sql, lstParam, "更新任务行状态") > 0;

        }
        #endregion
        /// <summary>
        /// 检查任务下的明细行是否全部执行完成
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public bool CheckTaskComplete(string taskid)
        {
            string sql = @" select count(1) num from &DataSource&wms_b_task_item t where t.task_id= @task_id and process_status != @process_status";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("task_id", taskid);
            lstParam.Add("process_status", ProcessStatus.ProcessStatus_Done.ToString());
            return DBHelper.Select(sql, lstParam).Rows[0]["num"].ToString() == "0";
        }

        /// <summary>
        /// 检查站台是否有待执行的任务
        /// </summary>
        /// <param name="equimentId"></param>
        /// <returns></returns>
        public bool CheckStationHasWaitingTask(string equimentId)
        {
            string sql = @"select count(1) nums from &DataSource&wms_b_task t
                            where t.src_addr_id=@src_addr_id
                                    and t.task_status=@task_status";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("src_addr_id", equimentId);
            lstParam.Add("task_status", TaskStatus.TaskStatus_WaitingExecute.ToString());
            return int.Parse(DBHelper.SelectOne(sql, lstParam, "", false)) > 0;
        }
        /// <summary>
        /// 获取站台执行中的任务(时间限定:5分钟之内)
        /// </summary>
        /// <param name="equimentId">站台/设备ID</param>
        /// <returns></returns>
        public TaskHeaderModel GetStationExctingTask(string equimentId)
        {
            string sql = @"select * from &DataSource&wms_b_task t
                            where t.src_addr_id=@src_addr_id
                                    and t.task_status=@task_status and act_status='1'
                                    and t.create_date > sysdate-5/(24*60) ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("src_addr_id", equimentId);
            lstParam.Add("task_status", TaskStatus.TaskStatus_WaitingExecute.ToString());
            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<TaskHeaderModel>(dt)[0];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取待执行（执行中的）的入库任务
        /// </summary>
        /// <param name="containercode">容器号</param>
        /// <returns></returns>
        public TaskHeaderModel GetStationWaitingInboundTask(string containercode)
        {
            string sql = @"select * from &DataSource&wms_b_task t
                            where  t.task_type=@task_type
                                  and t.container_barcode=@container_barcode
                                  and t.task_status=@task_status";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_barcode", containercode);
            lstParam.Add("task_type", TaskType.TaskType_InBound.ToString());
            lstParam.Add("task_status", TaskStatus.TaskStatus_Executing.ToString());
            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<TaskHeaderModel>(dt)[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取待执行（执行中的）的入库任务
        /// </summary>
        /// <param name="containercode">容器号</param>
        /// <returns></returns>
        public TaskHeaderModel GetStationWaitingInboundTask2(string containercode)
        {
            string sql = @"select * from &DataSource&wms_b_task t
                            where  t.task_type=@task_type
                                  and t.container_barcode=@container_barcode
                                  and t.task_status=@task_status and t.act_status = '1'";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_barcode", containercode);
            lstParam.Add("task_type", TaskType.TaskType_InBound.ToString());
            lstParam.Add("task_status", TaskStatus.TaskStatus_Executing.ToString());
            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<TaskHeaderModel>(dt)[0];
            }
            else
            {
                return null;
            }
        }


        //private void SaveTask(Route route, Address dest, DataRow waveDetailRow,
        //    DataRow[] inventoryRows, List<WorkTaskItemModel> taskItemList)
        public decimal SaveTask(DataRow[] route, WorkTaskModel workTask,
            List<WorkTaskItemModel> taskItemList)
        {
            // 按路径段生成任务及任务明细
            Address src = new Address(workTask.SrcAddrID, workTask.SrcAddrType, workTask.SrcAddrText);
            Address dest = new Address(workTask.DestAddrID, workTask.DestAddrType, workTask.DestAddrText);
            string lastTask = string.Empty;
            ContainerHeadModel container = new ContainerHeadModel();
            container.ID = workTask.ContainerID;
            container.ContainerBarcode = workTask.ContainerBarcode;
            string id = workTask.ID;

            foreach (DataRow row in route)
            {
                bool isRelay = false;
                Address relay;
                if (row == route.Last())
                {
                    relay = dest;
                }
                else
                {
                    relay = new Address(row[r.DestAddrType].ToString(), row[r.DestAddrID].ToString());
                    isRelay = true;
                }
                string taskType = row[r.TaskType].ToString();
                string taskNo = WmsHelper.GetSequence("TaskNo");
                DataRow equipRow = Equipment.GetStackByLocation(src.ID);

                WorkTaskModel task = new WorkTaskModel()
                {
                    ID = id,
                    RefDocType = workTask.RefDocType,
                    RefDocID = workTask.RefDocID,
                    RefDocNo = workTask.RefDocNo,
                    RefDocDetailID = taskItemList.FirstOrDefault().RefDocDetailID,
                    TaskType = taskType,
                    TaskNo = taskNo, // WmsHelper.GetSequence("TaskNo"),
                    EquipmentID = equipRow?["t_id"].ToString(),
                    //ExecutedBy = row[r.ExecutorType].ToString(),
                    //ExecutorType = TaskExecutor.TaskExecutor_RF.ToString(),
                    ExecutedBy = "",
                    ExecutorType = row[r.ExecutorType].ToString(),
                    //EquipmentType = row[r.ExecutorType].ToString(),
                    //SeqNo = Decimal.Parse(waveDetailRow["detail_seq"].ToString()),
                    //Priority = decimal.Parse(priority),
                    SeqNo = workTask.SeqNo,
                    Priority = workTask.Priority,
                    ContainerID = container.ID,
                    ContainerBarcode = container.ContainerBarcode,
                    TaskStatus = workTask.TaskStatus ?? TaskStatus.TaskStatus_Create.ToString(),
                    SrcAddrID = src.ID,
                    SrcAddrType = src.Type,
                    SrcAddrText = src.Text,
                    DestAddrID = relay.ID,
                    DestAddrType = relay.Type,
                    DestAddrText = relay.Text,
                    IsExecutable = "0",
                    DependentTaskNo = lastTask,
                    LedText = workTask.LedText,
                    ZoneID = workTask.ZoneID,
                };
                DataRow locRow;
                /*
                if (taskType == TaskType.TaskType_OutBound.ToString()
                    || taskType == TaskType.TaskType_Replenishment.ToString())
                {
                    if (src.Type == AddressType.AddressType_Location.ToString())
                    {
                        locRow = Location.Get(src.ID);
                        if (locRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                        {
                            task.ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString();
                        }
                    }
                }
                */

                WorkTask.Insert(task);
                lastTask = task.TaskNo;

                locRow = Location.GetByAddress(relay);

                // 此处暂定补货任务不生成任务明细，出库/拣选需要生成
                // 如果此处不生成，将来取消的时候将无法找到该任务对应的物料

                foreach (WorkTaskItemModel taskItem in taskItemList)
                {
                    AddTaskItem(task, taskItem, isRelay);
                }
                id = null;
                container.ID = task.ContainerID;
                container.ContainerBarcode = task.ContainerBarcode;
                src = relay;
            }
            //Wave.AddHeadLeftTaskQty(headRow[wh.ID].ToString(), sequences.Length);
            return route.Length;
        }


        private void ProcessContainerDetail
                (WorkTaskModel task, WorkTaskItemModel taskItem, bool isRelay)
        //(Address src, Address dest, string containerDetailID, decimal qty, bool isRelay)
        {
            // TODO 针对立库可能需要更多的处理，针对入库也可能还要处理
            Address src = new Address(task.SrcAddrID, task.SrcAddrType, task.SrcAddrText);
            Address dest = new Address(task.DestAddrID, task.DestAddrType, task.DestAddrText);
            string containerDetailID = taskItem.ContainerDetailID;
            decimal qty = (decimal)taskItem.PlanQty;

            DataRow ctnDtlRow = Container.GetDetail(taskItem.ContainerDetailID);
            DataRow ctnRow = Container.GetHead(ctnDtlRow[cd.ContainerID].ToString());
            DataRow srcLoc = GetLocation(src.Type, src.ID);
            DataRow destLoc = GetLocation(dest.Type, dest.ID);
            if (ctnRow[ch.ContainerType].ToString() == ContainerType.ContainerType_Actual.ToString())
            {
                if (srcLoc?[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                {
                    Location.UpdateStatus(srcLoc[l.ID].ToString(),
                        LocationStatus.LocationStatus_Out.ToString(),
                        srcLoc[l.LocationStatus].ToString());
                }
                if (destLoc?[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                {
                    Location.UpdateStatus(destLoc[l.ID].ToString(),
                        LocationStatus.LocationStatus_In.ToString(),
                        destLoc[l.LocationStatus].ToString());
                }
                return;
            }

            if (srcLoc != null)
            {
                Container.AddDetailOutQty(containerDetailID, qty);
            }
            if (destLoc != null)
            {
                if (destLoc[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                {
                    throw new WmsException(500, "虚拟容器无法入到立体库货架！");
                }
                /*
                DataRow[] destContainers = Container.GetHeadsByLocation(destLoc[l.ID].ToString());
                destContainers = destContainers.Where(
                    r => r[ch.ContainerType].ToString() == ContainerType.ContainerType_Virtual.ToString()).ToArray();
                if (destContainers.Length <= 0)
                {
                    Container.Insert(new ContainerHeadModel()
                    {
                        LocationID = destLoc[l.ID].ToString(),
                        ContainerBarcode = destLoc[l.LocationCode].ToString(),
                        ContainerType = ContainerType.ContainerType_Virtual.ToString(),
                        FirstInDate = DateTime.Now,
                        OwnerID = destLoc[l.OwnerID].ToString(),
                    });
                }
                */
                ctnRow = Container.GetOrNewVirtualHead(destLoc);
                string ctnDetailID = SnowflakeID.Guid().ToString();
                ContainerDetailModel ctnDetailModel = new ContainerDetailModel()
                {
                    ID = ctnDetailID,
                    //ZoneID = locRow["isa_bi_storage_zone_id"].ToString(),
                    ContainerID = ctnRow[ch.ID].ToString(),
                    InboundHeadID = "",
                    ItemID = ctnDtlRow[cd.ItemID].ToString(),
                    ItemName = ctnDtlRow[cd.ItemName].ToString(),
                    UomQtyID = ctnDtlRow[cd.UomQtyID].ToString(),
                    UomStatID = ctnDtlRow[cd.UomStatID].ToString(),
                    LotNo = ctnDtlRow[cd.LotNo].ToString(),
                    GivenYear = decimal.Parse("0" + ctnDtlRow[cd.GivenYear]),
                    Spec = ctnDtlRow[cd.Spec].ToString(),
                    ExistBarcode = ctnDtlRow[cd.ExistBarcode].ToString(),
                    FirstInDate = (ctnDtlRow[cd.FirstInDate].ToString() == "") ? null : ((DateTime?)ctnDtlRow[cd.FirstInDate]),
                    QCStatus = ctnDtlRow[cd.QCStatus].ToString(),
                };
                Container.PreparePutaway(ctnDetailModel, qty,
                    decimal.Parse("0" + destLoc[l.QtyLimit]));
                taskItem.ContainerDetailID = ctnDetailModel.ID;
                task.ContainerID = ctnDetailModel.ContainerID;
                task.ContainerBarcode = Container.GetHead(task.ContainerID)[ch.ContainerBarcode].ToString();

                if (isRelay)
                {
                    Container.AddDetailQty(ctnDetailModel.ID, qty);
                }
            }
        }

        private void ProcessContainerDetailV2(Address src, Address dest,
                    string containerDetailID, decimal qty)
        {
            // TODO 针对立库可能需要更多的处理，针对入库也可能还要处理
            if (src.Type == AddressType.AddressType_Location.ToString())
            {
                Container.AddDetailOutQty(containerDetailID, qty);
            }
            if (dest.Type == AddressType.AddressType_Location.ToString())
            //if (destLocation != null)
            {
                DataRow destLocation = Location.Get(dest.ID);
                DataRow ctnDtlRow = Container.GetDetail(containerDetailID);
                string ctnDetailID = SnowflakeID.Guid().ToString();
                ContainerDetailModel ctnDetailModel = new ContainerDetailModel()
                {
                    ID = ctnDetailID,
                    //ZoneID = locRow["isa_bi_storage_zone_id"].ToString(),
                    ContainerID = ctnDtlRow[cd.ContainerID].ToString(),
                    InboundHeadID = "",
                    ItemID = ctnDtlRow[cd.ItemID].ToString(),
                    ItemName = ctnDtlRow[cd.ItemName].ToString(),
                    UomQtyID = ctnDtlRow[cd.UomQtyID].ToString(),
                    UomStatID = ctnDtlRow[cd.UomStatID].ToString(),
                    LotNo = ctnDtlRow[cd.LotNo].ToString(),
                    GivenYear = decimal.Parse("0" + ctnDtlRow[cd.GivenYear]),
                    Spec = ctnDtlRow[cd.Spec].ToString(),
                    ExistBarcode = ctnDtlRow[cd.ExistBarcode].ToString(),
                    FirstInDate = (DateTime)ctnDtlRow[cd.FirstInDate],
                    QCStatus = ctnDtlRow[cd.QCStatus].ToString(),
                };
                Container.PreparePutaway(ctnDetailModel, qty,
                    decimal.Parse("0" + destLocation[l.QtyLimit]));

                if (Location.GetByAddress(src) != null)
                {
                    Container.AddDetailQty(ctnDetailModel.ID, qty);
                }
            }
        }

        private void ProcessContainerDetailV1(Address src, DataRow destLocation,
                    string containerDetailID, decimal qty)
        {
            if (src.Type == AddressType.AddressType_Location.ToString())
            {
                Container.AddDetailOutQty(containerDetailID, qty);
            }
            if (destLocation != null)
            {
                DataRow ctnDtlRow = Container.GetDetail(containerDetailID);
                string ctnDetailID = SnowflakeID.Guid().ToString();
                ContainerDetailModel ctnDetailModel = new ContainerDetailModel()
                {
                    ID = ctnDetailID,
                    //ZoneID = locRow["isa_bi_storage_zone_id"].ToString(),
                    ContainerID = ctnDtlRow[cd.ContainerID].ToString(),
                    InboundHeadID = "",
                    ItemID = ctnDtlRow[cd.ItemID].ToString(),
                    ItemName = ctnDtlRow[cd.ItemName].ToString(),
                    UomQtyID = ctnDtlRow[cd.UomQtyID].ToString(),
                    UomStatID = ctnDtlRow[cd.UomStatID].ToString(),
                    LotNo = ctnDtlRow[cd.LotNo].ToString(),
                    GivenYear = decimal.Parse("0" + ctnDtlRow[cd.GivenYear]),
                    Spec = ctnDtlRow[cd.Spec].ToString(),
                    ExistBarcode = ctnDtlRow[cd.ExistBarcode].ToString(),
                    FirstInDate = (DateTime)ctnDtlRow[cd.FirstInDate],
                };
                Container.PreparePutaway(ctnDetailModel, qty,
                    decimal.Parse("0" + destLocation[l.QtyLimit]));

                if (Location.GetByAddress(src) != null)
                {
                    Container.AddDetailQty(ctnDetailModel.ID, qty);
                }
            }
        }

        /// <summary>
        /// 返回连task在内的所有依赖任务
        /// </summary>
        /// <param name="task">起始任务</param>
        /// <returns></returns>
        public DataRow[] GetDependentTasks(DataRow task)
        {
            DataRow[] tasks = new DataRow[] { task };
            do
            {
                DataRow[] dependents = GetDependentTasks(tasks.Last()[t.TaskNo].ToString());
                if (dependents.Length == 0)
                {
                    break;
                }
                tasks = tasks.Append(dependents[0]).ToArray();
            } while (true);
            return tasks;
        }

        public IWorkTask UpdateTaskAndItemActStatus(string taskID, string status)
        {
            DataRow[] itemRows = GetTaskItemsBy(taskID, status == "0" ? "1" : "0");
            foreach (DataRow row in itemRows)
            {
                UpdateTaskItemActStatus(row[ti.ID].ToString(), status);
            }
            UpdateActStatus(taskID, status);
            return this;
        }

        public List<WorkTaskItemModel> AllocateItems(DataRow[] taskItemRows, DataRow[] inventoryRows)
        {
            List<WorkTaskItemModel> taskItemList = new List<WorkTaskItemModel>();

            foreach (DataRow taskItem in taskItemRows)
            {
                decimal planQty = decimal.Parse("0" + taskItem[wd.PlanQty].ToString())
                - decimal.Parse("0" + taskItem[wd.ProcessedQty].ToString());
                if (planQty <= 0)
                {
                    continue;
                }
                decimal leftQty = planQty;

                DataRow[] selectedRows = inventoryRows.Where(
                    r => r[cd.ItemID].ToString() == taskItem[ti.ItemID].ToString()).ToArray();

                foreach (DataRow invRow in selectedRows)
                {
                    decimal availQty = decimal.Parse(invRow["avail_qty"].ToString());
                    if (availQty <= 0)
                    {
                        continue;
                    }

                    decimal outQty = availQty < planQty ? availQty : planQty;
                    leftQty -= outQty;
                    invRow["avail_qty"] = availQty - outQty;

                    // 更新明细表中的已处理量
                    taskItem[wd.ProcessedQty] = decimal.Parse("0" + taskItem[ti.ProcessedQty]) + outQty;

                    taskItemList.Add(new WorkTaskItemModel()
                    {
                        ItemID = taskItem[ti.ItemID].ToString(),
                        PlanQty = DBHelper.GetDecimal(taskItem[ti.PlanQty]),
                        ContainerDetailID = invRow[ti.ContainerDetailID].ToString(),
                        RefDocDetailID = taskItem[ti.RefDocDetailID].ToString(),
                    });

                    if (leftQty <= 0)
                    {
                        break;
                    }
                }
                if (leftQty > 0)
                {
                    throw new WmsException(500, $"物料{taskItem[ti.ItemName]}不足");
                }
            }

            return taskItemList;
        }

        private TaskType DetermineTaskType(DataRow[] taskRows)
        {
            //DataRow taskRow = WorkTask.Get(taskID);
            //DataRow[] taskRows = WorkTask.GetDependentTasks(taskRow);

            int cnt = taskRows.Count(r => r[t.TaskType].ToString() == TaskType.TaskType_Picking.ToString());
            if (cnt > 0)
            {
                return TaskType.TaskType_Picking;
            }
            return TaskType.TaskType_OutBound;
        }

        private bool IsRunningOut(DataRow[] inventoryRows)
        {
            foreach (DataRow row in inventoryRows)
            {
                if (decimal.Parse("0" + row["avail_qty"]) > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsFinished(DataRow taskRow)
        {
            if (taskRow[t.TaskStatus].ToString() != TaskStatus.TaskStatus_Finished.ToString())
            {
                return false;
            }
            return true;
        }

        public bool IsFinished(DataRow[] taskRows)
        {
            taskRows = taskRows.Where(r => !IsFinished(r)).ToArray();
            if (taskRows.Length > 0)
            {
                return false;
            }
            return true;
        }

        public RoutePurpose DetermineRoutePurpose(DataRow locRow, DataRow[] inventoryRows)
        {
            if (locRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
            {
                if (!IsRunningOut(inventoryRows))
                {
                    return RoutePurpose.RoutePurpose_Picking;
                }
            }
            return RoutePurpose.RoutePurpose_Outbound;
        }
        private RoutePurpose DetermineRoutePurposeV1(string taskType)
        {
            TaskType type = (TaskType)Enum.Parse(typeof(TaskType), taskType.ToString());
            RoutePurpose purpose;
            switch (type)
            {
                case TaskType.TaskType_OutBound:
                    purpose = RoutePurpose.RoutePurpose_Outbound;
                    break;
                case TaskType.TaskType_Picking:
                    purpose = RoutePurpose.RoutePurpose_Picking;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return purpose;
        }

        public decimal SaveTask(RoutePurpose purpose, WorkTaskModel task,
            List<WorkTaskItemModel> taskItemList)
        {
            DataRow itemCatalog = Item.GetItemCatalog(taskItemList[0].ItemID);
            Route route = new Route(itemCatalog["t_id"].ToString(), purpose.ToString());

            // 匹配任务路径
            Address src = new Address(task.SrcAddrID, task.SrcAddrType, task.SrcAddrText);
            Address dest = new Address(task.DestAddrID, task.DestAddrType, task.DestAddrText);
            if (!route.MatchRoute(src, dest, out DataRow[] sequences))
            {
                throw new WmsException(500, "无法找到合适的配置来生成任务");
            }

            // 保存任务
            return WorkTask.SaveTask(sequences, task, taskItemList);
        }


        /// <summary>
        /// WCS申请任务接口
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        public WebJsonInfo ApplyTask(NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam applyInfo)
        {
            WebJsonInfo re = new WebJsonInfo();
            WorkTask task = new WorkTask();
            Model.TaskHeaderModel newTask = null;
            TaskHeaderModel t = GetStationWaitingInboundTask(applyInfo.ContainerNo);

            DataTable dtTask = DBHelper.Select(@"select * from &DataSource&wms_b_task t
                            where t.container_barcode=@container_barcode  and client_id='&CLIENT_ID&' and org_id='&ORG_ID&' and warehouse_id = @warehouse_id", new Dictionary<string, object>() {
                    { "container_barcode", applyInfo.ContainerNo },
                    { "warehouse_id", Context.WarehouseID }
            }, "", false);

            if (applyInfo.ApplyType == NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam.ApplyInboundType.EmptyContainer)
            {
                newTask = task.EmptyContainerIn(applyInfo);
            }
            else if (applyInfo.ApplyType == NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam.ApplyInboundType.TallyedContainer)
            {
                string decodeSwitch = WmsConfig.Get("Inbound:DecodeBarcodeWhenPalletApplyIn");
                if ("1".Equals(decodeSwitch))
                {
                    //string
                }
                else
                {
                    if (dtTask.Rows.Count > 0 && dtTask.Rows[0]["ref_doc_type"].ToString() == DocType.DocType_Qc.ToString())
                    {
                        newTask = new QcBound().GetQcInTask(applyInfo);
                    }
                    else
                    {
                        newTask = task.TallyedContainerIn(applyInfo);
                    }
                }
            }
            else if (applyInfo.ApplyType == NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam.ApplyInboundType.ProductContainer)
            {
                newTask = task.ProductContainerIn(applyInfo);
            }
            else if (applyInfo.ApplyType == NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam.ApplyInboundType.ContainerPickBack)
            {

            }
            else
            {
                throw new Exception("目前不支持：" + applyInfo.ApplyType + "入库");
            }
            newTask.lstTaskItems = getTaskItems(newTask.t_id, false);
            re.isSucceed = true;
            re.json = newTask;
            return re;
        }

        /// <summary>
        /// 更新任务执行设备信息
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="warehouseid"></param>
        /// <param name="equipmentcode"></param>
        /// <returns></returns>
        public bool UpdateTaskEquipment(TaskHeaderModel task, string equipmentcode)
        {
            EquipmentModel em = new Equipment().GetEquipment(task.warehouse_id, equipmentcode);
            task.executed_by = em.equipment_code;
            task.executor_type = "WCS";
            task.equipment_id = em.t_id;

            string sql = @"update &DataSource&wms_b_task t set t.equipment_id=@equipment_id,
                                executed_by=@executed_by,
                                executor_type='WCS'
                              where t.t_id = @t_id";
            Dictionary<string, object> lstPrams = new Dictionary<string, object>();
            lstPrams.Add("equipment_id", task.equipment_id);
            lstPrams.Add("executed_by", task.executed_by);
            lstPrams.Add("t_id", task.t_id);
            return DBHelper.RunSql(sql, lstPrams) > 0;

        }
        /// <summary>
        /// 更新任务的目的地址
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="locm"></param>
        /// <returns></returns>
        public bool UpdateTaskDestLocation(TaskHeaderModel task, LocationModel locm)
        {
            if (!string.IsNullOrEmpty(locm.zone_id4))
            {
                UpdateTaskEquipment(task, locm.zone_id4);
            }
            string sql = @"update &DataSource&wms_b_task t set t.dest_addr_id=@dest_addr_id,
                                              t.dest_addr_type=@dest_addr_type,
                                              t.dest_addr_text=@dest_addr_text,
                                              t.modify_ip=@modify_ip,
                                              t.last_modified_by=@last_modified_by,
                                              t.modify_date=sysdate
                            where t.t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("dest_addr_id", locm.t_id);
            lstParam.Add("dest_addr_type", LocationType.LocationType_3DRack.ToString());
            lstParam.Add("dest_addr_text", locm.location_code);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("t_id", task.t_id);
            return DBHelper.RunSql(sql, lstParam) > 0;
        }

        /// <summary>
        /// 更新任务的目的地址
        /// </summary>
        /// <param name="task"></param>
        /// <param name="locm"></param>
        /// <returns></returns>
        public bool UpdateTaskDestLocation(string taskId, LocationModel locm)
        {
            string sql = @"update &DataSource&wms_b_task t set t.dest_addr_id=@dest_addr_id,
                                              t.dest_addr_type=@dest_addr_type,
                                              t.dest_addr_text=@dest_addr_text,
                                              t.modify_ip=@modify_ip,
                                              t.last_modified_by=@last_modified_by,
                                              t.modify_date=sysdate
                            where t.t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("dest_addr_id", locm.t_id);
            //lstParam.Add("dest_addr_type", LocationType.LocationType_Compact.ToString());
            //lstParam.Add("dest_addr_type", LocationType.LocationType_3DRack.ToString());
            lstParam.Add("dest_addr_type", AddressType.AddressType_Location.ToString());
            lstParam.Add("dest_addr_text", locm.location_code);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("t_id", taskId);
            return DBHelper.RunSql(sql, lstParam) > 0;
        }

        /// <summary>
        /// 更新任务的源地址地址
        /// </summary>
        /// <param name="task"></param>
        /// <param name="locm"></param>
        /// <returns></returns>
        public bool UpdateTaskSrcAddrLocation(string taskId, LocationModel locm)
        {
            string sql = @"update &DataSource&wms_b_task t set t.src_addr_id=@src_addr_id,
                                              t.src_addr_type=@src_addr_type,
                                              t.src_addr_text=@src_addr_text,
                                              t.modify_ip=@modify_ip,
                                              t.last_modified_by=@last_modified_by,
                                              t.modify_date=sysdate
                            where t.t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("src_addr_id", locm.t_id);
            lstParam.Add("src_addr_type", LocationType.LocationType_Compact.ToString());
            lstParam.Add("src_addr_text", locm.location_code);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("t_id", taskId);
            return DBHelper.RunSql(sql, lstParam) > 0;
        }
        /// <summary>
        /// 空托盘申请入库
        /// <br>如果此站台已经有待执行(执行中)的任务，则直接返回此任务给WCS</br>
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        public TaskHeaderModel EmptyContainerIn(NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam applyInfo)
        {
            if (string.IsNullOrEmpty(applyInfo.WarehouseCode))
            {
                throw new Exception("申请仓库代码不能为空");
            }
            LogHelper.LogInformation("EmptyContainerIn开始获取仓库ID");
            WarehouseModel wm = new Warehouse().GetWarehouseId(applyInfo.WarehouseCode);
            if (wm == null)
            {
                throw new Exception("根据仓库代码没有找到对应的仓库信息");
            }
            LogHelper.LogInformation("EmptyContainerIn开始获取空托盘组物料信息");
            ItemModel item = new Item().GetEmptyContainerItem(wm.t_id, applyInfo.Width);
            if (item == null)
            {
                throw new Exception("请现在物料基表中配置空托盘物料信息");
            }
            if (string.IsNullOrEmpty(applyInfo.ApplyStationCode))
            {
                throw new Exception("申请站台不能为空");
            }
            LogHelper.LogInformation("EmptyContainerIn开始获取设备信息");
            Equipment equipment = new Equipment();
            EquipmentModel em = equipment.GetEquipment(wm.t_id, applyInfo.ApplyStationCode);
            if (em == null)
            {
                throw new Exception("申请站台不存在");
            }
            Putaway putaway = new Putaway();
            Location loc = new Location();
            LocationModel locm = null;
            string allocateLocationId = "";
            LogHelper.LogInformation("EmptyContainerIn开始检查是否存在待执行的任务");
            TaskHeaderModel currentTask = GetStationExctingTask(em.t_id);
            if (currentTask != null)
            {
                LogHelper.LogInformation("EmptyContainerIn存在待执行的任务");
                if (string.IsNullOrEmpty(currentTask.dest_addr_id))
                {
                    LogHelper.LogInformation("EmptyContainerIn开始分配货位");
                    allocateLocationId = putaway.AllocateOneLocationForPallet(wm.t_id, item.t_id);
                    if (string.IsNullOrEmpty(allocateLocationId))
                    {
                        throw new Exception("分配货位失败");
                    }
                    loc = new Location();
                    LogHelper.LogInformation("EmptyContainerIn获取货位信息");
                    locm = loc.GetLocationById(allocateLocationId);
                    LogHelper.LogInformation("EmptyContainerIn更新货位状态为待入库");
                    loc.UpdateLocationStatus(locm.t_id, LocationStatus.LocationStatus_In);
                    LogHelper.LogInformation("EmptyContainerIn更新任务的目的货位");
                    UpdateTaskDestLocation(currentTask, locm);
                }
                LogHelper.LogInformation("EmptyContainerIn存在待执行的任务 结束");
                return currentTask;
            }

            //loc = new Location();
            //locm = loc.GetLocationById(allocateLocationId);
            //loc.UpdateLocationStatus(locm.t_id, LocationStatus.LocationStatus_In);


            LogHelper.LogInformation("EmptyContainerIn不存在待执行的任务");
            Container container = new Container();
            #region 生成空托盘容器信息
            ContainerModel ctn = new ContainerModel();
            ctn.act_status = "1";
            ctn.allow_called = "0";
            ctn.client_id = Context.ClientID;
            ctn.container_barcode = string.IsNullOrEmpty(applyInfo.ContainerNo) ? "99999999" : applyInfo.ContainerNo;
            ctn.container_catalog_id = item.catalog_id;
            ctn.container_type = ContainerType.ContainerType_Actual.ToString();
            ctn.created_by = Context.UserInfo.UserName;
            ctn.create_date = DateTime.Now;
            ctn.is_in = "0";
            //ctn.location_id = locm.t_id;
            ctn.modify_date = DateTime.Now;
            ctn.modify_ip = Context.UserInfo.LoginIP;
            ctn.warehouse_id = wm.t_id;
            ctn.org_id = wm.org_id;
            ctn.client_id = Context.ClientID;
            ctn.t_id = SnowflakeID.Guid().ToString();
            container.AddContainerH(ctn);
            LogHelper.LogInformation("EmptyContainerIn新增容器信息");
            ContainerDModel ctnd = new ContainerDModel();
            ctnd.t_id = SnowflakeID.Guid().ToString();
            ctnd.act_status = "1";
            ctnd.client_id = Context.ClientID;
            ctnd.container_id = ctn.t_id;
            ctnd.created_by = Context.UserInfo.UserName;
            ctnd.create_date = DateTime.Now;
            ctnd.first_in_date = DateTime.Now;
            ctnd.item_id = item.t_id;
            ctnd.item_name = item.item_name;
            ctnd.owner_id = item.owner_id;
            ctnd.qty = applyInfo.Qty ?? 1;
            ctnd.warehouse_id = wm.t_id;
            ctnd.modify_ip = Context.UserInfo.LoginIP;
            ctnd.modify_date = DateTime.Now;
            ctnd.last_modified_by = Context.UserInfo.UserName;
            container.AddContainerD(ctnd);
            LogHelper.LogInformation("EmptyContainerIn新增容器明细信息");
            #endregion

            //allocateLocationId = putaway.AllocateOneLocationForPallet(wm.t_id, item.t_id);
            //if (string.IsNullOrEmpty(allocateLocationId))
            //{
            //    throw new Exception("分配货位失败");
            //}

            //loc = new Location();
            //locm = loc.GetLocationById(allocateLocationId);
            //loc.UpdateLocationStatus(locm.t_id, LocationStatus.LocationStatus_In);
            //ctn.location_id = locm.t_id;

            LogHelper.LogInformation("EmptyContainerIn分配货位");
            allocateLocationId = putaway.AllocateOneLocationForPallet(wm.t_id, item.t_id, item.t_id, applyInfo.ZoneID);
            if (allocateLocationId.Length > 3)//小于等于3，表示获取的是行，不是货位id
            {
                if (string.IsNullOrEmpty(allocateLocationId))
                {
                    throw new Exception("分配货位失败");
                }
                loc = new Location();
                locm = loc.GetLocationById(allocateLocationId);
                loc.UpdateLocationStatus(locm.t_id, LocationStatus.LocationStatus_In);
                ctn.location_id = locm.t_id;
                container.UpdateContainerBom(ctn.t_id, item.t_id);
            }
            else
            {
                loc = new Location();
                //根据行号获取预分配货位信息
                LocationModel planLocationInfo = loc.GetPlanLocationByLine(allocateLocationId);
                string planLocId = planLocationInfo.t_id;
                container.UpdateContainerLocation(ctn.t_id, planLocId);
                container.UpdateContainerBom(ctn.t_id, item.t_id);
            }

            #region 产生空托盘入库任务

            TaskHeaderModel newTask = new TaskHeaderModel();
            newTask.t_id = SnowflakeID.Guid().ToString();
            newTask.act_status = "1";
            newTask.client_id = Context.ClientID;
            newTask.container_barcode = ctn.container_barcode;
            newTask.container_id = ctn.t_id;
            newTask.create_date = DateTime.Now;
            newTask.created_by = Context.UserInfo.UserName;
            newTask.executor_type = TaskExecutor.TaskExecutor_WCS.ToString();
            newTask.ref_doc_type = DocType.DocType_EmptyContainerIn.ToString();
            newTask.dest_addr_id = locm == null ? allocateLocationId : locm.t_id;
            newTask.dest_addr_text = locm == null ? allocateLocationId : locm.location_code;
            newTask.dest_addr_type = AddressType.AddressType_Location.ToString();
            newTask.is_executable = "1";
            newTask.modify_date = DateTime.Now;
            newTask.modify_ip = Context.UserInfo.LoginIP;
            newTask.last_modified_by = Context.UserInfo.UserName;
            newTask.org_id = wm.org_id;
            newTask.priority = 100;
            newTask.src_addr_id = em.t_id;
            newTask.src_addr_text = em.equipment_code;
            newTask.src_addr_type = AddressType.AddressType_Station.ToString();
            newTask.task_no = WmsHelper.GetSequence("TaskNo");
            newTask.task_status = TaskStatus.TaskStatus_Executing.ToString();
            newTask.task_type = TaskType.TaskType_InBound.ToString();
            newTask.warehouse_id = wm.t_id;
            newTask.zone_id = locm == null ? applyInfo.ZoneID : locm.zone_id;
            AddTask(newTask);
            LogHelper.LogInformation("EmptyContainerIn新增任务信息");

            TaskItemModel taskitem = new TaskItemModel();
            taskitem.act_status = "1";
            taskitem.client_id = Context.ClientID;
            taskitem.container_d_id = ctnd.t_id;
            taskitem.created_by = Context.UserInfo.UserName;
            taskitem.create_date = DateTime.Now;
            taskitem.item_id = ctnd.item_id;
            taskitem.item_name = ctnd.item_name;
            taskitem.modify_date = DateTime.Now;
            taskitem.modify_ip = Context.UserInfo.LoginIP;
            taskitem.org_id = ctn.org_id;
            taskitem.plan_qty = ctnd.qty;
            taskitem.process_status = ProcessStatus.ProcessStatus_New.ToString();
            taskitem.task_id = newTask.t_id;
            taskitem.t_id = SnowflakeID.Guid().ToString();
            taskitem.warehouse_id = wm.t_id;
            AddTaskItem(taskitem);
            LogHelper.LogInformation("EmptyContainerIn新增任务明细信息");
            #endregion

            if (locm != null && !string.IsNullOrEmpty(locm.zone_id4))
            {
                UpdateTaskEquipment(newTask, locm.zone_id4);
            }
            LogHelper.LogInformation("EmptyContainerIn返回新生成的任务");
            return newTask;
        }
        /// <summary>
        /// 更新任务的起始站台信息
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public bool UpdateTaskSrcLocation(string taskid, EquipmentModel equipment)
        {
            string sql = @"update &DataSource&wms_b_task t set t.src_addr_id=@src_addr_id,
                                            t.src_addr_type = @src_addr_type,
                                            t.src_addr_text = @src_addr_text,
                                            t.modify_ip = @modify_ip,
                                            t.last_modified_by = @last_modified_by,
                                            t.modify_date = sysdate
                         where t.t_id = @t_id and t.client_id = '&CLIENT_ID&'   ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("src_addr_id", equipment.t_id);
            lstParam.Add("src_addr_type", AddressType.AddressType_Station.ToString());
            lstParam.Add("src_addr_text", equipment.equipment_code);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("t_id", taskid);

            return DBHelper.RunSql(sql, lstParam) > 0;
        }
        /// <summary>
        /// 已组盘容器入库
        /// <br>如果此容器在当前站台有未执行的入库任务，则直接返回现有任务给WCS</br>
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        public TaskHeaderModel TallyedContainerIn(NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam applyInfo)
        {
            if (string.IsNullOrEmpty(applyInfo.WarehouseCode))
            {
                throw new Exception("申请仓库代码不能为空");
            }
            WarehouseModel wm = new Warehouse().GetWarehouseId(applyInfo.WarehouseCode);
            if (wm == null)
            {
                throw new Exception("根据仓库代码没有找到对应的仓库信息");
            }
            if (string.IsNullOrEmpty(applyInfo.ContainerNo))
            {
                throw new Exception("申请入库容器不能为空");
            }

            Equipment equipment = new Equipment();
            EquipmentModel em = equipment.GetEquipment(wm.t_id, applyInfo.ApplyStationCode);
            if (em == null)
            {
                throw new Exception("没有找到申请站台信息");
            }

            Container container = new Container();
            Putaway putaway = new Putaway();
            TaskHeaderModel currentTask = GetStationWaitingInboundTask2(applyInfo.ContainerNo);
            if (currentTask != null)
            {
                if (currentTask.src_addr_id != em.t_id)
                { //更新任务的原地址为当前站台
                    currentTask.src_addr_id = em.t_id;
                    currentTask.src_addr_text = applyInfo.ApplyStationCode;
                    currentTask.src_addr_type = AddressType.AddressType_Station.ToString();
                    UpdateTaskSrcLocation(currentTask.t_id, em);

                }
                if (string.IsNullOrEmpty(currentTask.dest_addr_id))
                {
                    Location loc2 = new Location();
                    List<ContainerDModel> lstCtnDetail2 = container.getContainerDetails(currentTask.container_id);
                    ItemModel item2 = new Item().GetItem(lstCtnDetail2[0].item_id);
                    string allocateLocationId2 = putaway.AllocateOneLocationForPallet(wm.t_id, item2.item_code);
                    LocationModel locm2 = loc2.GetLocationById(allocateLocationId2);
                    loc2.UpdateLocationStatus(locm2.t_id, LocationStatus.LocationStatus_In);
                    UpdateTaskDestLocation(currentTask, locm2);
                }
                AddCtnApplyinSNLog(currentTask.container_id, currentTask.task_no);
                return currentTask;
            }
            string ctnid = container.GetContainerIdByContainerNo(applyInfo.ContainerNo);
            if (string.IsNullOrEmpty(ctnid))
            {
                throw new Exception("没有找到此容器的待入库信息");
            }
            ContainerModel ctn = container.getContainerById(ctnid);
            if (ctn == null)
            {
                throw new Exception("没有找到此容器的待入库信息");
            }
            string memo = ctn.memo;

            List<ContainerDModel> lstCtnDetail = container.getContainerDetails(ctn.t_id);
            ItemModel item = new Item().GetItem(lstCtnDetail[0].item_id);



            string allocateLocationId = putaway.AllocateOneLocationForPallet(wm.t_id, item.item_code, ctn.bom_id, memo);
            LocationModel locm = null;
            if (allocateLocationId.Length > 3)//小于等于3，表示获取的是行，不是货位id
            {
                if (string.IsNullOrEmpty(allocateLocationId))
                {
                    throw new Exception("分配货位失败");
                }
                Location loc = new Location();
                locm = loc.GetLocationById(allocateLocationId);
                loc.UpdateLocationStatus(locm.t_id, LocationStatus.LocationStatus_In);
                ctn.location_id = locm.t_id;
                container.UpdateContainerLocation(ctn.t_id, locm.t_id);
                container.UpdateContainerBom(ctn.t_id, ctn.bom_id);
            }
            else
            {
                Location loc = new Location();
                //根据行号获取预分配货位信息
                LocationModel planLocationInfo = loc.GetPlanLocationByLine(allocateLocationId);
                string planLocId = planLocationInfo.t_id;
                container.UpdateContainerLocation(ctn.t_id, planLocId);
                container.UpdateContainerBom(ctn.t_id, ctn.bom_id);
            }

            #region 产生托盘入库任务

            TaskHeaderModel task = new TaskHeaderModel();
            task.t_id = SnowflakeID.Guid().ToString();
            task.act_status = "1";
            task.client_id = Context.ClientID;
            task.container_barcode = ctn.container_barcode;
            task.container_id = ctn.t_id;
            task.create_date = DateTime.Now;
            task.created_by = Context.UserInfo.UserName;
            task.executor_type = TaskExecutor.TaskExecutor_WCS.ToString();
            task.ref_doc_type = DocType.DocType_ContainerIn.ToString();
            task.dest_addr_id = locm != null ? locm.t_id : "";
            task.dest_addr_text = locm != null ? locm.location_code : allocateLocationId;
            task.dest_addr_type = AddressType.AddressType_Location.ToString();
            task.is_executable = "1";
            task.modify_date = DateTime.Now;
            task.modify_ip = Context.UserInfo.LoginIP;
            task.last_modified_by = Context.UserInfo.UserName;
            task.start_date = DateTime.Now;
            task.org_id = wm.org_id;
            task.priority = 100;
            task.src_addr_id = em.t_id;
            task.src_addr_text = em.equipment_code;
            task.src_addr_type = AddressType.AddressType_Station.ToString();
            task.task_no = WmsHelper.GetSequence("TaskNo");
            task.task_status = TaskStatus.TaskStatus_Executing.ToString();
            task.task_type = TaskType.TaskType_InBound.ToString();
            task.warehouse_id = wm.t_id;
            task.zone_id = (locm != null ? locm.zone_id : (WmsConfig.GetOrDefault("Inbound:InboundZone", "1293844199577358336")));
            AddTask(task);

            foreach (ContainerDModel ctnd in lstCtnDetail)
            {
                TaskItemModel taskitem = new TaskItemModel();
                taskitem.act_status = "1";
                taskitem.client_id = Context.ClientID;
                taskitem.container_d_id = ctnd.t_id;
                taskitem.created_by = Context.UserInfo.UserName;
                taskitem.create_date = DateTime.Now;
                taskitem.item_id = ctnd.item_id;
                taskitem.item_name = ctnd.item_name;
                taskitem.modify_date = DateTime.Now;
                taskitem.modify_ip = Context.UserInfo.LoginIP;
                taskitem.org_id = ctn.org_id;
                taskitem.uom_qty_id = ctnd.uom_qty_id;
                taskitem.uom_stat_id = ctnd.uom_stat_id;
                taskitem.plan_qty = (ctnd.qty ?? 0) == 0 ? ctnd.in_qty : ctnd.qty;
                taskitem.process_status = ProcessStatus.ProcessStatus_New.ToString();
                taskitem.task_id = task.t_id;
                taskitem.t_id = SnowflakeID.Guid().ToString();
                taskitem.warehouse_id = ctn.warehouse_id;
                AddTaskItem(taskitem);
            }
            #endregion
            if (locm != null)
            {
                UpdateTaskDestLocation(task, locm);
            }

            AddCtnApplyinSNLog(task.container_id, task.task_no);
            return task;

        }

        /// <summary>
        /// 新增SN申请入库日志
        /// </summary>
        /// <param name="containerid"></param>
        private void AddCtnApplyinSNLog(string containerid, string taskno)
        {
            Container container = new Container();
            List<Container_SN> lstSn = container.GetContainerSnListByCtnId(containerid);
            foreach (Container_SN sn in lstSn)
            {
                container.AddSNDLog(sn, null, "申请入库", "申请入库", "", taskno);
            }
        }
        /// <summary>
        /// 新增Sn日志
        /// </summary>
        /// <param name="containercode"></param>
        /// <param name="snLogIn"></param>
        /// <returns></returns>
        public bool AddSNLog(string containercode,
            NtiCloud.Model.WMS.WorkTaskModel.SNLogIn snLogIn)
        {
            Container container = new Container();
            ContainerModel ctnHeader = container.GetContainer(Context.WarehouseID, containercode);
            if (ctnHeader == null)
            {
                throw new Exception("此托盘信息不存在");
            }
            Container_SN sn = container.GetContainerSn(ctnHeader.t_id, snLogIn.sn);
            if (sn == null)
            {
                List<ContainerDModel> lstDetails = container.getContainerDetails(ctnHeader.t_id);
                LocationModel locm = new Location().GetLocationById(ctnHeader.location_id);
                foreach (ContainerDModel ctnd in lstDetails)
                {
                    sn = new Container_SN();
                    sn.t_id = SnowflakeID.Guid().ToString();
                    sn.client_id = ctnHeader.client_id;
                    sn.warehouse_id = ctnHeader.warehouse_id;
                    sn.item_sn = snLogIn.sn;
                    sn.item_id = ctnd.item_id;
                    sn.item_name = ctnd.item_name;
                    sn.container_id = ctnHeader.t_id;
                    sn.container_no = ctnHeader.container_barcode;
                    sn.container_d_id = ctnd.t_id;
                    sn.location_id = ctnHeader.location_id;
                    sn.location_name = locm == null ? "" : locm.location_name;
                    sn.created_by = Context.UserInfo == null ? "" : Context.UserInfo.UserId;
                    sn.create_date = DateTime.Now;
                    sn.modify_date = DateTime.Now;
                    sn.act_status = "1";
                    container.Insert(sn);
                }
            }
            Container_SN oldSn = new Container_SN();
            oldSn.location_name = snLogIn.fromLocationName;
            oldSn.location_id = snLogIn.fromLocationId;
            sn.location_id = snLogIn.toLocationId;
            sn.location_name = snLogIn.toLocationName;
            container.AddSNDLog(sn, oldSn, snLogIn.action, snLogIn.reason, snLogIn.docno, snLogIn.taskno);
            return true;
        }

        /// <summary>
        /// 生产入库
        /// <br>如果此容器在此站台有待执行任务，直接返回此任务</br>
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        public TaskHeaderModel ProductContainerIn(NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam applyInfo)
        {
            if (string.IsNullOrEmpty(applyInfo.WarehouseCode))
            {
                throw new Exception("申请仓库代码不能为空");
            }
            WarehouseModel wm = new Warehouse().GetWarehouseId(applyInfo.WarehouseCode);
            if (wm == null)
            {
                throw new Exception("根据仓库代码没有找到对应的仓库信息");
            }

            if (string.IsNullOrEmpty(applyInfo.ContainerNo))
            {
                throw new Exception("申请入库容器号不能为空");
            }

            if (string.IsNullOrEmpty(applyInfo.ApplyStationCode))
            {
                throw new Exception("申请站台不能为空");
            }
            Equipment equipment = new Equipment();
            EquipmentModel em = equipment.GetEquipment(wm.t_id, applyInfo.ApplyStationCode);
            if (em == null)
            {
                throw new Exception("没找到此站台信息");
            }

            #region  根据设备获取当前的生产计划
            ProdPlanIn planin = new ProdPlanIn();
            ProdPlanInModel planinModel = planin.GettProdPlanInDoing(em.t_id);
            if (planinModel == null)
            {
                throw new Exception("没有找到设备:" + applyInfo.ApplyStationCode + "当前执行中的生产计划");
            }
            if (planinModel.is_qty_controlled == "1" && planinModel.plan_qty < (planinModel.finished_qty ?? 0) + (applyInfo.Qty ?? 0))
            {
                throw new Exception("生产计划开启了数量控制，不允许超过计划数量");
            }
            bool up = planin.AddFinishQty(planinModel.t_id, applyInfo.Qty ?? 0);
            if (!up)
            {
                throw new Exception("更新生产计划已完成量失败");
            }
            #endregion
            ItemModel item = new Item().GetItem(planinModel.item_id);
            if (applyInfo.lstSN.Count > 0)
            {
                foreach (NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam.ApplyTaskInDetail sn in applyInfo.lstSN)
                {
                    if ((sn.SNQty ?? 0) == 0)
                    {
                        sn.SNQty = 1;
                    }
                }
            }
            if (applyInfo.lstSN.Count > 0 && applyInfo.Qty > 0)
            {
                if (applyInfo.lstSN.Sum(s => s.SNQty) != applyInfo.Qty)
                {
                    throw new Exception("SN数量汇总与申请入库数量不一致");
                }
            }
            if (applyInfo.lstSN.Count > 0 && (applyInfo.Qty ?? 0) == 0)
            {
                applyInfo.Qty = applyInfo.lstSN.Sum(s => s.SNQty);
            }

            Putaway putaway = new Putaway();
            TaskHeaderModel currentTask = GetStationWaitingInboundTask(applyInfo.ContainerNo);
            if (currentTask != null)//容器存在此托盘的待入库任务时，更新当前任务的起始地址并返回当前任务
            {
                if (currentTask.src_addr_id != em.t_id)
                { //更新任务的原地址为当前站台
                    currentTask.src_addr_id = em.t_id;
                    currentTask.src_addr_text = applyInfo.ApplyStationCode;
                    currentTask.src_addr_type = AddressType.AddressType_Station.ToString();
                    UpdateTaskSrcLocation(currentTask.t_id, em);

                }
                if (string.IsNullOrEmpty(currentTask.dest_addr_id))
                {
                    Location loc2 = new Location();
                    string allocateLocationId2 = putaway.AllocateOneLocationForPallet(wm.t_id, item.item_code);
                    LocationModel locm2 = loc2.GetLocationById(allocateLocationId2);
                    loc2.UpdateLocationStatus(locm2.t_id, LocationStatus.LocationStatus_In);
                    UpdateTaskDestLocation(currentTask, locm2);

                }
                return currentTask;
            }

            Container container = new Container();
            //判断当前容器是否可以申请入库
            if (!container.CheckContainerCanAppyIn(wm.t_id, applyInfo.ContainerNo))
            {
                throw new Exception("当前容器不允许申请入库");
            }

            string allocateLocationId = putaway.AllocateOneLocationForPallet(wm.t_id, item.t_id);
            if (string.IsNullOrEmpty(allocateLocationId))
            {
                throw new Exception("分配货位失败");
            }
            Location loc = new Location();
            LocationModel locm = loc.GetLocationById(allocateLocationId);
            loc.UpdateLocationStatus(locm.t_id, LocationStatus.LocationStatus_In);

            #region 生成托盘容器信息
            //判断容器号在库存表是否存在
            string ctnid = container.GetContainerIdByContainerNo(wm.t_id, applyInfo.ContainerNo);
            ContainerModel ctn = null;
            bool isNewContainer = false;
            if (string.IsNullOrEmpty(ctnid))//无此容器信息，则新增
            {
                ctn = new ContainerModel();
                ctn.container_barcode = applyInfo.ContainerNo;
                ctn.created_by = Context.UserInfo.UserName;
                ctn.create_date = DateTime.Now;
                ctn.t_id = SnowflakeID.Guid().ToString();
                isNewContainer = true;
            }
            else
            {
                //已有此容器信息，则更新
                ctn = container.getContainerById(ctnid);
                isNewContainer = false;
            }
            ctn.org_id = wm.org_id;
            ctn.warehouse_id = wm.t_id;
            ctn.client_id = Context.ClientID;
            ctn.location_id = "";
            ctn.is_full = "Y";
            ctn.allow_called = "0";
            ctn.is_in = "0";
            ctn.is_out = "0";
            ctn.take_out_by = "";
            ctn.take_in_by = "";
            ctn.equipment_id = "";
            ctn.act_status = "1";
            ctn.container_type = ContainerType.ContainerType_Actual.ToString();
            ctn.first_in_date = DateTime.Now;
            ctn.container_catalog_id = item.catalog_id;
            ctn.modify_date = DateTime.Now;
            ctn.modify_ip = Context.UserInfo.LoginIP;
            ctn.last_modified_by = Context.UserInfo.UserName;
            ctn.location_id = locm.t_id;

            if (isNewContainer)
            {
                container.AddContainerH(ctn);
            }
            else
            {
                container.UpdateContainerHeaderInfo(ctn);
            }

            ContainerDModel ctnd = new ContainerDModel();
            ctnd.t_id = SnowflakeID.Guid().ToString();
            ctnd.act_status = "1";
            ctnd.client_id = Context.ClientID;
            ctnd.org_id = ctn.org_id;
            ctnd.container_id = ctn.t_id;
            ctnd.created_by = Context.UserInfo.UserName;
            ctnd.create_date = DateTime.Now;
            ctnd.first_in_date = DateTime.Now;
            ctnd.lot_no = planinModel.lot_no;
            ctnd.wms_lot_no = planinModel.wms_lot_no;
            ctnd.item_id = item.t_id;
            ctnd.item_name = item.item_name;
            ctnd.owner_id = item.owner_id;
            ctnd.qty = applyInfo.Qty ?? 0;
            ctnd.warehouse_id = ctn.warehouse_id;
            ctnd.modify_ip = Context.UserInfo.LoginIP;
            ctnd.modify_date = DateTime.Now;
            ctnd.last_modified_by = Context.UserInfo.UserName;
            ctnd.inbound_h_id = planinModel.t_id;
            ctnd.owner_id = planinModel.owner_id;
            ctnd.qc_status = QcStatus.QcStatus_CheckPassed.ToString();
            ctnd.uom_qty_id = planinModel.uom_qty_id;
            ctnd.uom_stat_id = planinModel.uom_stat_id;
            ctnd.production_date = DateTime.Now;


            container.AddContainerD(ctnd);
            string newTaskNo = WmsHelper.GetSequence("TaskNo");
            foreach (NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam.ApplyTaskInDetail sn in applyInfo.lstSN)
            {
                container.AddSNIntoContainer(ctn, ctnd, sn.SNCode, sn.SNQty ?? 1, sn.Position, "下线申请入库", newTaskNo, planinModel.plan_no);
            }

            #endregion



            #region 产生托盘入库任务

            TaskHeaderModel task = new TaskHeaderModel();
            task.t_id = SnowflakeID.Guid().ToString();
            task.act_status = "1";
            task.client_id = Context.ClientID;
            task.container_barcode = ctn.container_barcode;
            task.container_id = ctn.t_id;
            task.create_date = DateTime.Now;
            task.created_by = Context.UserInfo.UserName;
            task.dest_addr_id = locm.t_id;
            task.dest_addr_text = locm.location_code;
            task.dest_addr_type = AddressType.AddressType_Location.ToString(); ;
            task.is_executable = "1";
            task.modify_date = DateTime.Now;
            task.modify_ip = Context.UserInfo.LoginIP;
            task.last_modified_by = Context.UserInfo.UserName;
            task.org_id = wm.org_id;
            task.priority = 100;
            task.src_addr_id = em.t_id;
            task.src_addr_text = em.equipment_code;
            task.src_addr_type = AddressType.AddressType_Robot.ToString(); ;
            task.ref_doc_type = DocType.DocType_ProductPlanIn.ToString();
            task.ref_doc_no = planinModel.plan_no;
            task.ref_doc_id = planinModel.t_id;
            task.task_no = newTaskNo;
            task.task_status = TaskStatus.TaskStatus_Executing.ToString();
            task.task_type = TaskType.TaskType_InBound.ToString();
            task.warehouse_id = wm.t_id;
            task.zone_id = locm.zone_id;
            AddTask(task);

            TaskItemModel taskitem = new TaskItemModel();
            taskitem.act_status = "1";
            taskitem.client_id = Context.ClientID;
            taskitem.container_d_id = ctnd.t_id;
            taskitem.created_by = Context.UserInfo.UserName;
            taskitem.create_date = DateTime.Now;
            taskitem.item_id = ctnd.item_id;
            taskitem.item_name = ctnd.item_name;
            taskitem.modify_date = DateTime.Now;
            taskitem.modify_ip = Context.UserInfo.LoginIP;
            taskitem.org_id = ctn.org_id;
            taskitem.plan_qty = ctnd.qty;
            taskitem.process_status = ProcessStatus.ProcessStatus_New.ToString();
            taskitem.task_id = task.t_id;
            taskitem.t_id = SnowflakeID.Guid().ToString();
            taskitem.ref_doc_type = DocType.DocType_ProductPlanIn.ToString();
            taskitem.ref_doc_no = planinModel.plan_no;
            taskitem.ref_doc_id = planinModel.t_id;
            taskitem.warehouse_id = ctn.warehouse_id;
            taskitem.uom_qty_id = planinModel.uom_qty_id;
            taskitem.uom_stat_id = planinModel.uom_stat_id;
            AddTaskItem(taskitem);
            #endregion
            return task;


        }

        public TaskHeaderModel CallEmptyContainer(NtiCloud.Model.WMS.WorkTaskModel.CallEmptyContainerParam applyInfo)
        {
            if (string.IsNullOrEmpty(applyInfo.WarehouseCode))
            {
                throw new Exception("申请仓库代码不能为空");
            }
            WarehouseModel wm = new Warehouse().GetWarehouseId(applyInfo.WarehouseCode);
            if (wm == null)
            {
                throw new Exception("根据仓库代码没有找到对应的仓库信息");
            }

            if (string.IsNullOrEmpty(applyInfo.ApplyStationCode))
            {
                throw new Exception("申请站台不能为空");
            }
            Equipment equipment = new Equipment();
            EquipmentModel em = equipment.GetEquipment(wm.t_id, applyInfo.ApplyStationCode);
            if (em == null)
            {
                throw new Exception("没找到此站台信息");
            }
            Container container = new Container();
            ContainerModel ctn = container.CalloutEmptyContainer(wm.t_id, applyInfo.Width);
            if (ctn == null)
            {
                throw new Exception("分配货位失败");
            }
            #region 产生空托盘出库任务
            TaskHeaderModel task = new TaskHeaderModel();
            task.t_id = SnowflakeID.Guid().ToString();
            task.act_status = "1";
            task.client_id = Context.ClientID;
            task.container_barcode = ctn.container_barcode;
            task.container_id = ctn.t_id;
            task.create_date = DateTime.Now;
            task.created_by = Context.UserInfo.UserName;
            task.ref_doc_type = DocType.DocType_EmptyContainerOut.ToString();
            task.dest_addr_id = em.t_id;
            task.dest_addr_text = em.equipment_code;
            task.dest_addr_type = AddressType.AddressType_Station.ToString();
            task.is_executable = "1";
            task.modify_date = DateTime.Now;
            task.modify_ip = Context.UserInfo.LoginIP;
            task.last_modified_by = Context.UserInfo.UserName;
            task.org_id = wm.org_id;
            task.priority = 2;
            task.src_addr_id = ctn.location_id;
            task.src_addr_text = new Location().GetLocationById(ctn.location_id).location_no;
            task.src_addr_type = AddressType.AddressType_Location.ToString();
            task.task_no = WmsHelper.GetSequence("TaskNo");
            task.task_status = TaskStatus.TaskStatus_Executing.ToString();
            task.task_type = TaskType.TaskType_OutBound.ToString();
            task.warehouse_id = wm.t_id;
            AddTask(task);

            List<ContainerDModel> lstCtnDetail = container.getContainerDetails(ctn.t_id);
            ContainerDModel ctnd = lstCtnDetail[0];
            TaskItemModel taskitem = new TaskItemModel();
            taskitem.act_status = "1";
            taskitem.task_id = task.t_id;
            taskitem.client_id = ctnd.client_id;
            taskitem.container_d_id = ctnd.t_id;
            taskitem.created_by = Context.UserInfo.UserName;
            taskitem.create_date = DateTime.Now;
            taskitem.item_id = ctnd.item_id;
            taskitem.item_name = ctnd.item_name;
            taskitem.last_modified_by = Context.UserInfo.UserName;
            taskitem.modify_date = DateTime.Now;
            taskitem.modify_ip = Context.UserInfo.LoginIP;
            taskitem.org_id = ctnd.org_id;
            taskitem.plan_qty = ctnd.qty;
            taskitem.processed_qty = ctnd.qty;
            taskitem.process_status = ProcessStatus.ProcessStatus_New.ToString();
            taskitem.uom_qty_id = ctnd.uom_qty_id;
            taskitem.uom_stat_id = ctnd.uom_stat_id;
            taskitem.warehouse_id = wm.t_id;
            AddTaskItem(taskitem);
            #endregion
            //将原货位更新为待出库
            Location loc = new Location();
            loc.UpdateLocationStatus(ctn.location_id, LocationStatus.LocationStatus_Out);

            List<TaskItemModel> lstDetails = new List<TaskItemModel>();
            lstDetails.Add(taskitem);
            task.lstTaskItems = lstDetails;
            LocationModel originLocation = loc.GetLocationById(ctn.location_id);
            UpdateTaskEquipment(task, originLocation.zone_id4);
            return task;
        }

        /// <summary>
        /// 入库调用存过获取货位二次分配的值，并拼接成响应体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public NtiCloud.Model.WMS.WorkTaskModel.SecondDistribuLocResponse InBoundSecondDistributionLocation(NtiCloud.Model.WMS.WorkTaskModel.SecondDistribuLocRequest model)
        {
            if (model.Msg_Params == null || model.Msg_Params.Count <= 0)
            {
                throw new Exception("请求体Msg_Params不能为空");
            }
            if (string.IsNullOrEmpty(model.Msg_Params[0].WarehouseID))
            {
                throw new Exception("仓库id不能为空");
            }
            if (string.IsNullOrEmpty(model.Msg_Params[0].ZoneID))
            {
                throw new Exception("区域id不能为空");
            }
            if (string.IsNullOrEmpty(model.Msg_Params[0].TaskNum))
            {
                throw new Exception("任务号不能为空");
            }
            //任务号
            string taskNum = model.Msg_Params[0].TaskNum;
            //获取任务信息
            DataRow drTask = WorkTask.GetWorkTask(taskNum);
            //行号
            string line = drTask["DEST_ADDR_TEXT"].ToString();
            //容器主档id
            string containerId = drTask["container_id"].ToString();
            //获取容器主档信息
            DataRow drContainer = Container.GetContainerH(containerId);
            //获取bomId
            string bomId = drContainer["bom_id"].ToString();
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "I_WAREHOUSEID",model.Msg_Params[0].WarehouseID },
                { "BOMID",bomId },
                { "LINE", line }
            };
            string rule = WmsConfig.GetOrDefault("Inbound:Allocation_AGV", "work_task_8888");
            //调用存过，获取货位id
            string locId = new LocationAllocator().AllocateLocation(param, rule);
            Location loc = new Location();
            LocationModel locm = loc.GetLocationById(locId);
            NtiCloud.Model.WMS.WorkTaskModel.SecondDistribuLocResponse sr = new NtiCloud.Model.WMS.WorkTaskModel.SecondDistribuLocResponse();
            NtiCloud.Model.WMS.WorkTaskModel.SecondDistribuLocResponse.Msg_Paramslist mp = new NtiCloud.Model.WMS.WorkTaskModel.SecondDistribuLocResponse.Msg_Paramslist();
            mp.WarehouseId = model.Msg_Params[0].WarehouseID;
            mp.ZoneId = model.Msg_Params[0].ZoneID;
            mp.TaskNum = model.Msg_Params[0].TaskNum;
            mp.TargetAddress = locm.location_code;
            sr.Msg_Params = mp;
            UpdateTaskDestLocation(drTask["t_id"].ToString(), locm);//将二次分配后的结果保存到原有任务中
            return sr;
        }

        /// <summary>
        /// 出库调用存过获取货位二次分配的值，并拼接成响应体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public NtiCloud.Model.WMS.WorkTaskModel.SecondDistribuLocResponse OutBoundSecondDistributionLocation(NtiCloud.Model.WMS.WorkTaskModel.SecondDistribuLocRequest model)
        {
            if (model.Msg_Params == null || model.Msg_Params.Count <= 0)
            {
                throw new Exception("请求体Msg_Params不能为空");
            }
            if (string.IsNullOrEmpty(model.Msg_Params[0].WarehouseID))
            {
                throw new Exception("仓库id不能为空");
            }
            if (string.IsNullOrEmpty(model.Msg_Params[0].ZoneID))
            {
                throw new Exception("区域id不能为空");
            }
            if (string.IsNullOrEmpty(model.Msg_Params[0].TaskNum))
            {
                throw new Exception("任务号不能为空");
            }
            //任务号
            string taskNum = model.Msg_Params[0].TaskNum;
            //获取任务信息
            DataRow drTask = WorkTask.GetWorkTask(taskNum);
            //行号
            string line = drTask["SRC_ADDR_ID"].ToString();
            //站台id
            string dockId = drTask["DEST_ADDR_ID"].ToString();
            //生产工单id
            string productPlanOutId = drTask["REF_DOC_ID"].ToString();
            //工单号
            string job_no = drTask["REF_DOC_NO"].ToString();
            //区域
            string zone_id = drTask["ZONE_ID"].ToString();
            Base.CallEnd ce = new Base.CallEnd();
            //根据站台信息获取bom
            //DataTable dtBomInfo = ce.GetBOMInfo(dockId);
            string bomId = string.Empty;
            //string secondFloorZone = WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336");  //2楼区域
            //for (int i = 0; i < dtBomInfo.Rows.Count; i++)
            //{
            //    if (ce.isHaveStockByBomId(dtBomInfo.Rows[i]["t_id"].ToString(), secondFloorZone))
            //    {
            //        bomId = dtBomInfo.Rows[i]["t_id"].ToString();
            //        break;
            //    }
            //}

            //if (string.IsNullOrEmpty(bomId))
            //{
            //    throw new WmsException(500, "未查询到可用配盘信息！");
            //}
            DataTable _dtBomInfo = ce.GetTwoFloorBOMInfo(line, zone_id);
            if (_dtBomInfo == null || _dtBomInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "获取获取预分配货位上的托盘BOM信息！");
            }
            bomId = _dtBomInfo.Rows[0]["bom_id"].ToString();
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "I_WAREHOUSEID",model.Msg_Params[0].WarehouseID },
                { "BOMID",bomId },
                { "ZONE_ID",zone_id },
                { "LINE", line }
            };
            string rule = WmsConfig.GetOrDefault("Outbound:GetLocationByBomId2", "work_task_22");
            //调用存过，获取货位id
            string loc_id = new LocationAllocator().AllocateLocation(param, rule);
            Location loc = new Location();
            LocationModel locm = loc.GetLocationById(loc_id);
            NtiCloud.Model.WMS.WorkTaskModel.SecondDistribuLocResponse sr = new NtiCloud.Model.WMS.WorkTaskModel.SecondDistribuLocResponse();
            NtiCloud.Model.WMS.WorkTaskModel.SecondDistribuLocResponse.Msg_Paramslist mp = new NtiCloud.Model.WMS.WorkTaskModel.SecondDistribuLocResponse.Msg_Paramslist();
            mp.WarehouseId = model.Msg_Params[0].WarehouseID;
            mp.ZoneId = model.Msg_Params[0].ZoneID;
            mp.TaskNum = model.Msg_Params[0].TaskNum;
            mp.TargetAddress = locm.location_code;
            sr.Msg_Params = mp;
            UpdateTaskSrcAddrLocation(drTask["t_id"].ToString(), locm);//将二次分配后的结果保存到原有任务中

            #region 6.获取货位信息

            ////查询货位对应区域信息
            //DataTable dtLocationInfo = ce.GetLocationInfo(loc_id);
            //货位状态
            string location_status = locm.location_status;
            ////区域
            //string zone_id = WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336");//dtLocationInfo.Rows[0]["zone_id"].ToString();
            ////货位对应的编码
            //string loc_no = dtLocationInfo.Rows[0]["location_code"].ToString();

            //if (location_status != LocationStatus.LocationStatus_Stored.ToString())
            //{
            //    throw new WmsException(500, "货位分配策略分配的货位，状态不为已存储状态，无法进行出库");
            //}
            #endregion


            DataTable _BomInLoc = ce.GetBOMByLocationId(loc_id);
            if (_BomInLoc == null || _BomInLoc.Rows.Count == 0)
            {
                throw new WmsException(500, $"货位二次分配策略分配的货位为{locm.location_code}，实际上这个货位没有站台需要的盘型！");
            }
            if (_BomInLoc.Rows.Count != 1)
            {
                throw new WmsException(500, $"货位二次分配策略分配的货位为{locm.location_code}，实际上这个货位上有多个盘型，请联系库管人员处理！");
            }
            string _bomInLoc = _BomInLoc.Rows[0]["bom_id"].ToString();
            if (!string.Equals(bomId, _bomInLoc))
            {
                throw new WmsException(500, $"货位二次分配策略分配的货位为{locm.location_code}，该货位上的盘型与站台上需要的盘型不一致，请联系库管人员处理！");
            }

            #region 7.获取货位对应容器信息
            //查询库位对应的容器头表信息
            DataRow[] _dtHeadsInfo = Container.GetHeadsByLocation(loc_id);
            if (_dtHeadsInfo == null || _dtHeadsInfo.Length <= 0)
            {
                throw new WmsException(500, "货位分配返回空货位");
            }
            DataRow dtcontainerhInfo = _dtHeadsInfo[0];
            //容器头表主键
            string container_htid = dtcontainerhInfo["t_id"].ToString();
            //容器头表编码
            string container_barcode = dtcontainerhInfo["container_barcode"].ToString();
            //查询容器明细信息
            DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_htid);
            #endregion
            //将容器信息写入任务中
            ce.UpdateTaskContainerInfo(taskNum, container_htid, container_barcode);
            //产生任务明细
            for (int i = 0; i < dtrContainerDInfo.Length; i++)
            {
                decimal qty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString() == "" ? "0" : dtrContainerDInfo[i]["qty"].ToString());
                //修改容器明细中物料待出数量
                Container.UpdateDetailOutQty(dtrContainerDInfo[i]["t_id"].ToString(), qty);
                #region 插入任务物料信息
                WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    //ClientID = "",//集团ID
                    //OrgID = "",//组织ID
                    //WarehouseID = "",//仓库ID
                    TaskID = drTask["t_id"].ToString(),//主任务ID
                    ItemID = dtrContainerDInfo[i]["item_id"].ToString(),//物料ID
                    ItemName = dtrContainerDInfo[i]["item_name"].ToString(),//物料名称
                    PlanQty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString()),//计划数量
                    //ProcessedQty = "",//已处理数量
                    //FinishedQty = "",//完成数量
                    UomQtyID = dtrContainerDInfo[i]["uom_qty_id"].ToString(),//数量单位
                    //FinishedStatQty = "",//完成统计数量
                    UomStatID = dtrContainerDInfo[i]["uom_stat_id"].ToString(),//统计数量单位
                    ContainerDetailID = dtrContainerDInfo[i]["t_id"].ToString(),//容器明细ID
                    RefDocType = DocType.DocType_CallTerminal.ToString(),//来源单据类型  "机台要料"
                    RefDocID = productPlanOutId,//来源单ID
                    //RefDocDetailID = "",//来源单明细ID
                    RefDocNo = job_no,//来源单号
                    //StartDate = "",//开始时间
                    //EndDate = "",//结束时间
                    ProcessStatus = ProcessStatus.ProcessStatus_New.ToString()//工作状态
                    //CreateDate = "",//创建时间
                    //CreatedBy = "",//创建人帐号
                    //ModifyIP = "",//操作人IP
                    //ModifyDate = "",//编辑时间
                    //LastModifiedBy = "",//最后修改人帐号
                    //ActStatus = ""//状态
                };
                WorkTaskItem.Insert(taskItemModel);
                #endregion
            }

            //#region 10.获取交接班和人工领料站台
            ////获取交接班站台
            //DataRow drShiftDockInfo = ce.GetShiftDockByMachine(model.Machine);
            //string jiaojiebanDockTid = drShiftDockInfo["t_id"].ToString();//交接班站台在机台站台表中的t_id
            //#endregion

            //#region 11.清空挂在交接班站台上的容器
            ////清空挂在交接班站台上的容器
            //ce.UpdateMachineDockDeatilId(jiaojiebanDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            //#endregion

            #region 12.修改货位状态为待出库
            //修改货位状态为待出库
            ce.UpdateLocStatusByLocationId(loc_id, LocationStatus.LocationStatus_Out.ToString(), location_status, "机台要料出库,修改货位状态为待出库");
            #endregion

            return sr;
        }

        /// <summary>
        /// 回收WCS任务
        /// </summary>
        /// <param name="taskids"></param>
        /// <returns></returns>
        public string RecycleWcsTask(string taskids)
        {
            StringBuilder reInfo = new StringBuilder();
            string[] ids = taskids.Split(",");
            foreach (string id in ids)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                string re = WorkTaskInterface.Recycle(id);
                if (!string.IsNullOrEmpty(re))
                {
                    reInfo.Append("任务ID:").Append(id).Append("回收失败:").Append(re).Append(";");
                }
            }
            return reInfo.ToString();
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public string BuildRecycleWcsTaskBody(string taskid)
        {
            TaskHeaderModel task = GetTaskHeader(taskid);
            JObject header = new JObject();
            header.Add("Msg_Id", SnowflakeID.Guid().ToString());
            header.Add("Msg_Mark", "WMS->WCS");
            header.Add("Msg_Time", DateTime.Now.ToString("yyyy-mm-dd hh24:mm:ss"));
            header.Add("Msg_User", "");
            header.Add("Msg_UserPwd", "");
            header.Add("Msg_Method", "回收任务");
            JArray body = new JArray();
            JObject obj = new JObject();
            obj.Add("WarehouseID", task.warehouse_id);
            obj.Add("ZoneID", task.zone_id);
            obj.Add("TaskNum", task.task_no);
            obj.Add("Wave_ID", "");
            obj.Add("Mark", "");
            body.Add(obj);
            header.Add("Msg_Params", body);
            return header.ToString(Newtonsoft.Json.Formatting.None, null);

        }

        /// <summary>
        /// 入库更改巷道
        /// </summary>
        /// <param name="task_no">任务号</param>
        /// <returns>更改成功后返回更新后的任务信息，更改失败后抛出异常信息</returns>
        public TaskHeaderModel ChangeInboundChannel(string task_no)
        {
            TaskHeaderModel task = GetTaskHeaderByNo(task_no);
            if (task == null)
            {
                throw new Exception("此任务不存在");
            }
            if (task.task_status == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new Exception("此任务已完成，不可以再变更巷道");
            }
            if (task.task_type != TaskType.TaskType_InBound.ToString())
            {
                throw new Exception("只有上架任务才允许变更巷道");
            }
            string spname = "&DataSource&m_move_outside";
            List<Tuple<string, string, ParameterDirection>> lstParams = new List<Tuple<string, string, ParameterDirection>>();
            lstParams.Add(new Tuple<string, string, ParameterDirection>("i_warehouseid", task.warehouse_id, ParameterDirection.Input));
            lstParams.Add(new Tuple<string, string, ParameterDirection>("i_tasknumber", task.task_no, ParameterDirection.Input));
            lstParams.Add(new Tuple<string, string, ParameterDirection>("o_ifsucess", "", ParameterDirection.Output));
            lstParams.Add(new Tuple<string, string, ParameterDirection>("o_msg", "", ParameterDirection.Output));

            var outputs = DBHelper.RunProc(spname, lstParams, "入库巷道变更", DBHelper.OperatedNumType.OneOrMore);
            string resultcode = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "o_ifsucess")
                                               .FirstOrDefault()?.Value.ToString();
            string msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "o_msg")
                                               .FirstOrDefault()?.Value.ToString();
            if (resultcode == "1")//1=变更成功
            {
                //获取更新后的任务
                task = GetTaskHeaderByNo(task_no);
                task.lstTaskItems = getTaskItems(task.t_id, true);
                return task;
            }
            throw new Exception("更改巷道失败:" + msg);
        }

    }
}
