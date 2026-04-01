using Newtonsoft.Json.Linq;
using NtiCloud.Kernel.Core;
using NtiCloud.Service.LPS.Base;
using NtiCloud.Service.LPS.Inbound;
using NtiCloud.Service.LPS.Model;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Interface.WCS;
using NtiCloud.Service.WMS.Inventory;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Tasking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace NtiCloud.Service.WMS.QC
{
    public class QcBound
    {
        private readonly IWorkTask _workTask;
        private readonly IWorkTaskItem _workTaskItem;
        private readonly IContainer _container;
        private readonly IWorkTaskInterface _workTaskInterface;
        public QcBound(IWorkTask workTask, IWorkTaskItem workTaskItem, IContainer container)
        {
            _workTask = workTask;
            _workTaskItem = workTaskItem;
            _container = container;
            _workTaskInterface = new WorkTaskInterface();
        }

        public QcBound()
        {
            _workTask = new WorkTask();
            _workTaskItem = new WorkTaskItem();
            _container = new Container();
            _workTaskInterface = new WorkTaskInterface();
        }

        #region 质检单

        /// <summary>
        /// 确认质检
        /// </summary>
        /// <returns></returns>
        public WebJsonInfo ConfirmQcRunByType(dynamic param, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                info.errmsg = "确认质检";

                if (param.type.ToString() == QCServiceType.ServiceType_OutSide.ToString())
                {
                    info = AddOutQcTask(param, info);
                }
                else if (param.type.ToString() == QCServiceType.ServiceType_InSide.ToString())
                {
                    UpdateQcHStatus(param.qc_h_id.ToString(), QCDocStatus.QCDocStatus_Confirmed.ToString());

                }
                UpdateConQcStatusByQcHid(param.qc_h_id.ToString(), QcStatus.QcStatus_Checking.ToString());

                return info;
            },log);
        }

        /// <summary>
        /// 删除质检明细
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public WebJsonInfo DelQcDetail(dynamic param,UserLogModel log)
        {         
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                info = DelQcDetail(param, info);
                return info;
            },log);
        }

        /// <summary>
        /// 更新质检明细质检结果
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public WebJsonInfo UpdateQcResult(dynamic param, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                string strSql = string.Empty;

                if (string.IsNullOrEmpty(param.t_id.ToString())
                || string.IsNullOrEmpty(param.check_result.ToString()))
                {
                    info.errcode = 500;
                    info.errmsg = "存在参数为空";

                    return info;
                }
                string checkResult = string.Empty;
                checkResult = ToCheckResult(param.check_result.ToString());

                Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "t_id",  param.t_id.ToString() },
                    { "check_result", checkResult },
                    { "warehouse_id", Context.WarehouseID }
                };

                DataRow[] dr = _workTask.GetTableByRefDocID(param.t_id.ToString()).Select($"task_status='{TaskStatus.TaskStatus_Create.ToString()}'");

                if (dr.Length > 0)
                {
                    info.errcode = 500;
                    info.errmsg = "质检任务未执行完成";

                    return info;
                }

                DBHelper.RunSql(@"update &DataSource&wms_d_qc_d d set d.check_result=@check_result where d.qc_h_id=@t_id
                                   and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&' and d.warehouse_id = @warehouse_id", dicParam, "批量更新质检明细结果", DBHelper.OperatedNumType.OneOrMore);

                return info;
            },log);
        }

        /// <summary>
        /// 质检结果确认
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public WebJsonInfo ConfirmQcResult(dynamic param, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                string strSql = string.Empty;

                if (string.IsNullOrEmpty(param.qc_h_id.ToString()))
                {
                    info.errcode = 500;
                    info.errmsg = "存在参数为空";

                    return info;
                }
                Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "qc_h_id",  param.qc_h_id.ToString() },
                    { "warehouse_id", Context.WarehouseID }
                };
                DataRow dr = DBHelper.Select(@"select  count(*) cnt from &DataSource&wms_d_qc_d  d where d.qc_h_id=@qc_h_id and d.act_status=1 and d.check_result is null 
                                    and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&' and d.warehouse_id = @warehouse_id", dicParam).Rows[0];

                if (decimal.Parse("0" + dr["cnt"]) > 0)
                {
                    info.errcode = 500;
                    info.errmsg = "质检明细质检结果未全部录入";

                    return info;
                }

                dr = DBHelper.Select(@"select count(*) cnt from &DataSource&wms_d_qc_d d  where d.check_result!=nvl(d.first_result,'null') and d.qc_h_id=@qc_h_id and d.act_status='1'
                        and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&' and d.warehouse_id = @warehouse_id", dicParam).Rows[0];

                UpdateQcHStatus(param.qc_h_id.ToString(), decimal.Parse("0" + dr["cnt"]) > 0 ? QCDocStatus.QCDocStatus_UnAudit.ToString() : QCDocStatus.QCDocStatus_Executed.ToString());

                //更新库存质检状态
                if (decimal.Parse("0" + dr["cnt"]) == 0)
                {
                    DataRow[] drQcDetail = QueryQcDetailByHId(param.qc_h_id.ToString());

                    foreach (var item in drQcDetail)
                    {
                        this.UpdateContainerQcStatus(item["container_d_id"].ToString(), item["check_result"].ToString());
                    }
                }
                return info;
            },log);
        }

        /// <summary>
        /// 质检结果审核
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public WebJsonInfo CheckQcResult(dynamic param, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((Func<WebJsonInfo, WebJsonInfo>)((info) =>
            {
                string strSql = string.Empty;

                if (string.IsNullOrEmpty(param.t_id.ToString()) || string.IsNullOrEmpty(param.qc_doc_status.ToString()))
                {
                    info.errcode = 500;
                    info.errmsg = "存在参数为空";

                    return info;
                }
                Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "t_id",  param.t_id.ToString() },
                    { "qc_doc_status",  param.qc_doc_status.ToString() }
                };

                UpdateQcHStatus(param.t_id.ToString(), param.qc_doc_status.ToString());

                DataRow[] drQcDetail = QueryQcDetailByHId(param.t_id.ToString());
                //更新库存
                if (param.qc_doc_status.ToString() == QCDocStatus.QCDocStatus_Executed.ToString())
                {
                    foreach (var item in drQcDetail)
                    {
                        this.UpdateContainerQcStatus(item["container_d_id"].ToString(), item["check_result"].ToString());
                    }
                }

                return info;
            }),log);
        }

        #endregion

        #region 质检出库

        /// <summary>
        /// 执行出库任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskStatus"></param>
        public void RunOutQcTask(string taskId, TaskStatus taskStatus)
        {
            DataRow drTask = _workTask.GetWorkTask(taskId.ToString());
            if (drTask == null)
            {
                throw new WmsException(500, "查询不到任务");
            }
            string cId = drTask["container_id"].ToString();

            DataRow containerH = _container.GetContainerH(cId);
            if (containerH == null)
            {
                throw new WmsException(500, "查询不到容器头");
            }
            _workTask.UpdateTaskStatus(taskId, taskStatus.ToString());

            if (taskStatus == TaskStatus.TaskStatus_PartialFinished || taskStatus == TaskStatus.TaskStatus_Finished)
            {
                //WCS单独的部分完成接口已经把货位状态置为空
                //UpdateLocStatus(containerH["location_id"].ToString(), LocationStatus.LocationStatus_Empty.ToString());//更新货位状态     
                //更新为暂存区库存
                UpdateContainerLoc(containerH["t_id"].ToString(), "1268783997165805569");
            }

            if (taskStatus == TaskStatus.TaskStatus_Finished)
            {
                DataRow[] drTaskItem = _workTaskItem.GetByTaskID(taskId.ToString());
                foreach (DataRow item in drTaskItem)
                {
                    UpdateTaskItem(item["t_id"].ToString());
                }
            }

            _workTask.MoveToHistory(taskId);
        }

        #endregion

        #region 质检入库

        public WebJsonInfo GetQcDetail(string barcode, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                Tuple<DataTable, DataTable> qcInfo = GetQcInfo(barcode);
                DataTable dtContainer = qcInfo.Item1;
                DataTable dtQcDetail = qcInfo.Item2;

                List<object> qcItemDetail = new List<object>();

                foreach (DataRow item in dtQcDetail.Rows)
                {
                    qcItemDetail.Add(new
                    {
                        id = item["t_id"].ToString(),
                        itemName = item["item_name"].ToString(),
                        qty = item["plan_qty"].ToString(),
                        statQty = item["plan_stat_qty"].ToString()
                    });
                }

                info.json = new
                {
                    qcNo = dtContainer.Rows[0]["qc_no"].ToString(),
                    qcDetail = qcItemDetail
                };

                return info;
            },log);
        }

        /// <summary>
        /// RF调用入库任务
        /// </summary>
        /// <param name="barcode"></param>
        public WebJsonInfo StartInQcTask(string barcode, UserLogModel log, dynamic items = null)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                Tuple<DataTable, DataTable> qcInfo = GetQcInfo(barcode);
                DataTable dtContainer = qcInfo.Item1;
                DataTable dtQcDetail = qcInfo.Item2;

                DataRow dtCnt = DBHelper.Select(@" select count(*) cnt from &DataSource&wms_b_task t where t.ref_doc_id=@ref_doc_id
                        and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id 
                        and  t.container_barcode=@container_barcode
                         ", new Dictionary<string, object>() {
                    { "ref_doc_id", dtContainer.Rows[0]["qc_h_id"].ToString() },
                    { "warehouse_id", Context.WarehouseID },
                    { "container_barcode", barcode }
                }, "", false).Rows[0];

                if (decimal.Parse("0" + dtCnt["cnt"].ToString()) > 0)
                {
                    info.errcode = 500;
                    info.errmsg = "任务已产生";

                    return info;
                }

                string p = JsonSerializer.Serialize(items);
                items = JArray.Parse(p);
                if (items != null)
                {
                    for (int i = 0; i < dtQcDetail.Rows.Count; i++)
                    {
                        foreach (var item in items)
                        {
                            if (dtQcDetail.Rows[i]["t_id"].ToString() == item.id.ToString())//质检明细id
                            {
                                dtQcDetail.Rows[i]["plan_qty"] = item.qty.ToString();
                                dtQcDetail.Rows[i]["plan_stat_qty"] = item.statQty.ToString();
                            }
                        }
                    }
                }
                AddInQcTask(dtContainer, dtQcDetail);
                info.json = new
                {
                    barCode = barcode,
                    qcNo = dtContainer.Rows[0]["qc_no"].ToString()
                };

                return info;
            },log);
        }

        /// <summary>
        /// wcs调用质检入库
        /// </summary>
        /// <param name="barcode"></param>
        public WebJsonInfo RunInQcTaskByWcs(NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam applyInfo, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                info.json = GetQcInTask(applyInfo);

                return info;
            }, log);
        }

        /// <summary>
        /// 获取质检入库任务
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        public TaskHeaderModel GetQcInTask(NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam applyInfo)
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
            if (em == null && !string.IsNullOrEmpty(applyInfo.ApplyStationCode))
            {
                throw new Exception("没有找到申请站台信息");
            }

            Container container = new Container();
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

            string strSql = @"select t.warehouse_id,t.t_id, t.task_no, t.container_id,i.item_id,i.item_name from &DataSource&wms_b_task t left join &DataSource&wms_b_task_item i on t.t_id =i.task_id where t.task_status='TaskStatus_Create' and 
                      t.container_barcode=@container_barcode and t.act_status=1 and i.act_status=1
                      and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id";
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "container_barcode", applyInfo.ContainerNo },
                    { "warehouse_id", Context.WarehouseID }
                };
            DataTable dtTasks = DBHelper.Select(strSql, dicParam, "对应托盘任务明细", true);

            TaskHeaderModel currentTask = _workTask.GetTaskHeader(dtTasks.Rows[0]["t_id"].ToString());

            if (currentTask.src_addr_id != em.t_id && !string.IsNullOrEmpty(applyInfo.ApplyStationCode))
            {   //更新任务的原地址为当前站台
                currentTask.src_addr_id = em.t_id;
                currentTask.src_addr_text = applyInfo.ApplyStationCode;
                currentTask.src_addr_type = AddressType.AddressType_Station.ToString();

                _workTask.UpdateSource(currentTask.t_id, AddressType.AddressType_Station.ToString(), em.t_id, applyInfo.ApplyStationCode);
            }
            if (string.IsNullOrEmpty(currentTask.dest_addr_id))
            {
                Location loc = new Location();
                string allocateLocationId = new Putaway().AllocateOneLocationForPallet(dtTasks.Rows[0]["warehouse_id"].ToString(), dtTasks.Rows[0]["item_id"].ToString());
                LocationModel locInfo = loc.GetLocationById(allocateLocationId);

                currentTask.dest_addr_id = locInfo.t_id;
                currentTask.dest_addr_text = locInfo.location_no;
                currentTask.dest_addr_type = AddressType.AddressType_Location.ToString();

                DataRow equipRow = equipment.GetStackByLocation(locInfo.t_id);
                currentTask.equipment_id = equipRow?["t_id"].ToString();
                currentTask.task_status = TaskStatus.TaskStatus_Executing.ToString();

                DBHelper.RunSql(@"update  &DataSource&wms_b_task t set t.equipment_id=@equipment_id where
                  t.t_id =@t_id  and t.client_id='&CLIENT_ID&'  and t.warehouse_id = @warehouse_id",
                 new Dictionary<string, object>() {
                    { "t_id", currentTask.t_id },
                    { "equipment_id",  equipRow?["t_id"].ToString() },
                    { "warehouse_id", Context.WarehouseID }
                });

                loc.UpdateLocationStatus(locInfo.t_id, LocationStatus.LocationStatus_In);
                UpdateContainerLoc(dtTasks.Rows[0]["container_id"].ToString(), locInfo.t_id);//更新库存对应的货位id

                _workTask.UpdateTarget(currentTask.t_id, AddressType.AddressType_Location.ToString(), locInfo.t_id, locInfo.location_no);
            }

            _workTask.UpdateTaskStatus(currentTask.t_id, TaskStatus.TaskStatus_Executing.ToString());

            return currentTask;
        }

        /// <summary>
        /// 完成入库任务
        /// </summary>
        /// <param name="barcode"></param>
        public void FouceInQcTask(string taskId)
        {
            _workTask.UpdateTaskStatus(taskId, TaskStatus.TaskStatus_Finished.ToString());

            DataRow drTask = _workTask.GetWorkTask(taskId);
            DataRow[] drTaskItem = _workTaskItem.GetByTaskID(taskId);

            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "ref_doc_id",  drTask["ref_doc_id"].ToString() },
                    { "t_id",  drTask["t_id"].ToString() },
                    { "warehouse_id",  Context.WarehouseID }
                  };


            foreach (var item in drTaskItem)
            {
                //更新任务明细
                UpdateTaskItem(item["t_id"].ToString());
                //更新库存明细
                DataRow drQcDetail = QueryQcDetailById(item["ref_doc_d_id"].ToString());
                decimal taskQty = decimal.Parse("0" + item["plan_qty"].ToString());
                decimal taskStatQty = decimal.Parse("0" + item["finished_stat_qty"].ToString());
                decimal qcQty = decimal.Parse("0" + drQcDetail["plan_qty"].ToString());
                decimal qcStatQty = decimal.Parse("0" + drQcDetail["plan_stat_qty"].ToString());

                //更新质检明细
                UpdateQcDetail(item["ref_doc_d_id"].ToString(), drTask["dest_addr_id"].ToString(), drTask["dest_addr_text"].ToString(),
                    taskQty, taskStatQty);
                //抽样时处理
                if (taskQty != qcQty || taskStatQty != qcStatQty)
                {
                    UpdatQtyAndStatQty(item["container_d_id"].ToString(), -qcQty + taskQty, -qcStatQty + taskStatQty);
                }
            }

            UpdateLocStatus(drTask["dest_addr_id"].ToString(), LocationStatus.LocationStatus_Stored.ToString());//更新货位状态

            DataRow dr = DBHelper.Select(@"select  count(*) cnt from  &DataSource&wms_d_qc_d t where 
                 (t.to_location_id is null  or t.to_location_id ='')
                 and t.qc_h_id=@ref_doc_id
                 and t.t_id not in 
                 (
                      select i.ref_doc_d_id from  &DataSource&wms_b_task_item  i where  i.task_id =@t_id
                 )
                 and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id", dicParam).Rows[0];
            if (decimal.Parse("0" + dr["cnt"].ToString()) == 0)
            {
                UpdateQcHStatus(drTask["ref_doc_id"].ToString(), QCDocStatus.QCDocStatus_Confirmed.ToString());//更新质检单状态
            }

            _workTask.MoveToHistory(taskId);
        }

        #endregion

        #region 回收任务

        /// <summary>
        /// 回收质检任务
        /// </summary>
        /// <param name="taskId"></param>
        public void RecycleQcTask(DataRow taskRow)
        {
            if (taskRow == null)
            {
                throw new WmsException(500, "查询不到任务");
            }
            if (taskRow["task_type"].ToString() != TaskStatus.TaskStatus_NotExecute.ToString())
            {
                throw new WmsException(500, "任务状态不正确");
            }

            if (taskRow["task_type"].ToString() == TaskType.TaskType_InBound.ToString())
            {
                UpdateLocStatus(taskRow["dest_addr_id"].ToString(), LocationStatus.LocationStatus_Stored.ToString());//更新货位状态
            }
            else if (taskRow["task_type"].ToString() == TaskType.TaskType_OutBound.ToString())
            {
                UpdateLocStatus(taskRow["src_addr_id"].ToString(), LocationStatus.LocationStatus_Stored.ToString());//更新货位状态   
                UpdateConQcStatusByTaskId(taskRow["t_id"].ToString());

                DataRow dr = DBHelper.Select(@"select  count(t.t_id)  cnt from &DataSource&wms_b_task t where t.t_id!=@t_id and t.ref_doc_id=@ref_doc_id and 
                                           t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id",
                  new Dictionary<string, object>() {
                        { "t_id", taskRow["t_id"].ToString() },
                        { "ref_doc_id", taskRow["ref_doc_id"].ToString() },
                        { "warehouse_id", Context.WarehouseID}
                  }).Rows[0];

                if (decimal.Parse("0" + dr["cnt"].ToString()) == 0)
                {
                    UpdateQcHStatus(taskRow["ref_doc_id"].ToString(), QCDocStatus.QCDocStatus_NotExecute.ToString());//更新质检单状态
                }
            }

            _workTask.MoveToHistory(taskRow["t_id"].ToString());
        }

        #endregion

        #region 私有成员

        /// <summary>
        /// 更新质检主档状态
        /// </summary>
        /// <param name="tId"></param>
        /// <param name="status"></param>
        private void UpdateQcHStatus(string tId, string status)
        {
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam.Add("qc_doc_status", status);
            dicParam.Add("t_id", tId);
            dicParam.Add("warehouse_id", Context.WarehouseID);
            DBHelper.RunSql(@"update  &DataSource&wms_d_qc_h h 
                set h.qc_doc_status=@qc_doc_status
                where h.t_id=@t_id and h.act_status=1 
                and h.client_id='&CLIENT_ID&' and h.org_id='&ORG_ID&' and h.warehouse_id = @warehouse_id
                ", dicParam, "更新质检主档", DBHelper.OperatedNumType.OnlyOne);
        }

        /// <summary>
        /// 更新容器质检状态
        /// </summary>
        /// <param name="tId"></param>
        /// <param name="status"></param>
        private void UpdateConQcStatusByQcHid(string qcHId, string status)
        {
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam.Add("qc_status", status);
            dicParam.Add("qc_h_id", qcHId);
            dicParam.Add("warehouse_id", Context.WarehouseID);
            DBHelper.RunSql(@"update &DataSource&wms_b_container_d  a 
                   set a.qc_status=@qc_status
                   where a.t_id in
                   (
                     select d.container_d_id from &DataSource&wms_d_qc_d d where d.qc_h_id=@qc_h_id 
                     and d.act_status=1 
                     and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&'
                     and d.warehouse_id=@warehouse_id  
                   )    
                ", dicParam, "更新库存物料质检状态", DBHelper.OperatedNumType.OneOrMore);
        }

        /// <summary>
        /// 更新容器质检状态
        /// </summary>
        /// <param name="taskId"></param>
        private void UpdateConQcStatusByTaskId(string taskId)
        {
            Dictionary<string, object> dicParam = new Dictionary<string, object>();


            DataRow[] dtTaskItem = _workTask.GetTaskItemsBy(taskId);

            for (int i = 0; i < dtTaskItem.Length; i++)
            {
                dicParam.Clear();
                dicParam.Add("container_d_id", dtTaskItem[i]["container_d_id"].ToString());
                dicParam.Add("ref_doc_d_id", dtTaskItem[i]["ref_doc_d_id"].ToString());
                dicParam.Add("warehouse_id", Context.WarehouseID);

                DBHelper.RunSql(@"update &DataSource&wms_b_container_d  a 
                   set a.qc_status=(
                                    select d.first_result from &DataSource&wms_d_qc_d d where d.t_id=@ref_doc_d_id
                                    and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&'
                                    and d.warehouse_id=@warehouse_id
                                   )
                   where a.t_id =@container_d_id", dicParam, "更新库存物料质检状态", DBHelper.OperatedNumType.OnlyOne);
            }
        }

        /// <summary>
        /// 添加出库质检任务
        /// </summary>
        /// <param name="param"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private WebJsonInfo AddOutQcTask(dynamic param, WebJsonInfo info)
        {
            string strSql;
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam.Add("qc_h_id", param.qc_h_id.ToString());
            dicParam.Add("warehouse_id", Context.WarehouseID);

            DataRow drQcH = DBHelper.Select(@"select *from &DataSource&wms_d_qc_h h where h.t_id=@qc_h_id and  h.act_status=1
            and h.client_id='&CLIENT_ID&' and h.org_id='&ORG_ID&' and h.warehouse_id = @warehouse_id", dicParam).Rows[0];

            DataTable dtContainer = DBHelper.Select(@"select  distinct （select l.zone_id from  &DataSource&wms_m_location  l where  l.t_id=d1.from_location_id) zone_id,  d2.container_id,d3.container_barcode,d1.from_location_id,d1.from_location_name,d1.from_location_no
                        from &DataSource&wms_d_qc_d d1
                        left join &DataSource&wms_b_container_d d2 on d1.container_d_id=d2.t_id
                        left join &DataSource&wms_b_container_h d3 on d3.t_id=d2.container_id where d1.qc_h_id=@qc_h_id and  d1.act_status=1
                        and d1.client_id='&CLIENT_ID&' and d1.org_id='&ORG_ID&' and d1.warehouse_id = @warehouse_id
                         ", dicParam, "查询质检主档", true);

            DataTable dtQcDetail = DBHelper.Select(@"select 
                        d1.t_id,d2.container_id,d2.t_id container_d_id,d1.plan_qty,d1.plan_stat_qty,d1.item_id,d1.item_name,d1.uom_qty_id,d1.uom_stat_id
                        from &DataSource&wms_d_qc_d d1
                        left join &DataSource&wms_b_container_d d2 on d1.container_d_id=d2.t_id
                        left join &DataSource&wms_b_container_h d3 on d3.t_id=d2.container_id where d1.qc_h_id=@qc_h_id and  d1.act_status=1
                        and d1.client_id='&CLIENT_ID&' and d1.org_id='&ORG_ID&' and d1.warehouse_id = @warehouse_id
                        ", dicParam, "查询质检明细", true);

            if (drQcH["qc_doc_status"].ToString() == QCDocStatus.QCDocStatus_Executing.ToString())
            {
                info.errcode = 500;
                info.errmsg = "质检单已执行";

                return info;
            }

            List<string> idlist = new List<string>();
            foreach (DataRow cItem in dtContainer.Rows)//添加任务头
            {
                string taskId = InsTask(param.qc_h_id.ToString(), drQcH, cItem, true);

                foreach (DataRow item in dtQcDetail.Rows)//添加任务明细
                {
                    if (cItem["container_id"].ToString() == item["container_id"].ToString())
                    {
                        InsTaskItem(taskId, param.qc_h_id.ToString(), drQcH, item, true);
                        //更新质检明细源货位
                        //UpdateQcDetailSrcloc(item["t_id"].ToString(), cItem["from_location_id"].ToString(), cItem["from_location_name"].ToString());
                    }
                }

                UpdateLocStatus(cItem["from_location_id"].ToString(), LocationStatus.LocationStatus_Out.ToString());//更新货位状态   

                idlist.Add(taskId);
            }

            UpdateQcHStatus(param.qc_h_id.ToString(), QCDocStatus.QCDocStatus_Executing.ToString());

            _workTaskInterface.Send(idlist); //调用wcs接口执行任务

            return info;
        }

        /// <summary>
        /// 添加入库质检任务
        /// </summary>
        /// <param name="container_id"></param>
        /// <returns></returns>
        private void AddInQcTask(DataTable dtContainer, DataTable dtQcDetail)
        {
            if (dtContainer.Rows.Count == 0)
            {
                throw new WmsException(500, "查询不到容器信息");
            }
            DataRow drContainer = dtContainer.Rows[0];

            string qcHid = drContainer["qc_h_id"].ToString();
            DataRow drQcH = DBHelper.Select(@"select *from &DataSource&wms_d_qc_h h where h.t_id=@qc_h_id and  h.act_status=1
                    and h.client_id='&CLIENT_ID&' and h.org_id='&ORG_ID&' and h.warehouse_id = @warehouse_id", new Dictionary<string, object>() {
                    { "qc_h_id", qcHid },
                    {"warehouse_id", Context.WarehouseID }
                }).Rows[0];

            string taskId = InsTask(qcHid, drQcH, drContainer, false);
            foreach (DataRow item in dtQcDetail.Rows)//添加任务明细
            {
                if (drContainer["container_id"].ToString() == item["container_id"].ToString())
                {
                    InsTaskItem(taskId, qcHid, drQcH, item, false);
                }
            }
        }

        /// <summary>
        /// 插入任务头
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="qcHId"></param>
        /// <param name="drQcH"></param>
        /// <param name="item"></param>
        private string InsTask(string qcHId, DataRow drQcH, DataRow item, bool isOut, LocationModel loc = null)
        {
            //出库任务
            string taskId = SnowflakeID.Guid().ToString();
            Equipment equipment = new Equipment();
            if (isOut)
            {
                DataRow equipRow = equipment.GetStackByLocation(item["from_location_id"].ToString());
                DataRow dataRowEq = equipment.GetEquipment(drQcH["dest_addr_id"].ToString());

                Dictionary<string, object> dicParam = new Dictionary<string, object>() {                
                    { "t_id", item["from_location_id"].ToString() },
                    { "warehouse_id",  Context.WarehouseID}
                  };
                DataRow dataRowLoc = DBHelper.Select(@"select *from  &DataSource&wms_m_location l  where l.t_id=@t_id
                     and l.client_id='&CLIENT_ID&' and l.org_id='&ORG_ID&' and l.warehouse_id = @warehouse_id", dicParam).Rows[0];

                _workTask.Insert(new Model.WorkTaskModel

                {
                    ID = taskId,//主键ID
                    ZoneID = item["zone_id"].ToString(),//区域ID
                    TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号                                               
                    TaskType = TaskType.TaskType_OutBound.ToString(),//任务类型
                    TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                    SeqNo = 1,//顺序号
                    Priority = 1,//优先级
                    ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者
                    EquipmentID = equipRow?["t_id"].ToString(),
                    ContainerID = item["container_id"].ToString(),//容器ID
                    ContainerBarcode = item["container_barcode"].ToString(),//条码
                    RefDocType = DocType.DocType_Qc.ToString(),//来源单据类型
                    RefDocID = qcHId,//来源单ID
                    RefDocDetailID = "",//来源单明细ID
                    RefDocNo = drQcH["qc_no"].ToString(),//来源单号
                    SrcAddrID = item["from_location_id"].ToString(),//来源地址ID
                    SrcAddrType = AddressType.AddressType_Location.ToString(),//来源地址类型
                    SrcAddrText = item["from_location_name"].ToString(),//来源地址
                    DestAddrID = drQcH["dest_addr_id"].ToString(),//目标地址ID
                    DestAddrType = dataRowEq["equipment_type"].ToString(),//目标地址类型
                    DestAddrText = dataRowEq["equipment_code"].ToString(),//目标地址
                    ActStatus = "1"//状态
                });
            }
            else
            {
                //回库任务        
                _workTask.Insert(new Model.WorkTaskModel
                {
                    ID = taskId,//主键ID
                    ZoneID = item["zone_id"].ToString(),//区域ID
                    TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号                                               
                    TaskType = TaskType.TaskType_InBound.ToString(),//任务类型
                    TaskStatus = TaskStatus.TaskStatus_Create.ToString(),//任务状态
                    ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者
                    SeqNo = 1,//顺序号
                    Priority = 1,//优先级
                    ContainerID = item["container_id"].ToString(),//容器ID
                    ContainerBarcode = item["container_barcode"].ToString(),//条码
                    RefDocType = DocType.DocType_Qc.ToString(),//来源单据类型
                    RefDocID = qcHId,//来源单ID
                    RefDocDetailID = "",//来源单明细ID
                    RefDocNo = drQcH["qc_no"].ToString(),//来源单号
                    SrcAddrID = drQcH["dest_addr_id"].ToString(),//来源地址ID
                    SrcAddrType = AddressType.AddressType_Station.ToString(),//来源地址类型
                    SrcAddrText = drQcH["dest_addr_text"].ToString(),//来源地址
                    DestAddrID = "",//目标地址ID
                    DestAddrType = "",//目标地址类型
                    DestAddrText = "",//目标地址
                    ActStatus = "1"//状态
                });
            }

            return taskId;
        }

        /// <summary>
        /// 插入任务物料
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="qcHId"></param>
        /// <param name="drQcH"></param>
        /// <param name="item"></param>
        private void InsTaskItem(string taskId, string qcHId, DataRow drQcH, DataRow item, bool isOut)
        {
            decimal planQty = decimal.Parse("0" + item["plan_qty"].ToString());
            if (isOut)
            {
                _workTaskItem.Insert(new Model.WorkTaskItemModel
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    TaskID = taskId,//主任务ID
                    ItemID = item["item_id"].ToString(),//物料ID
                    ItemName = item["item_name"].ToString(),//物料名称
                    PlanQty = planQty,//计划数量
                    UomQtyID = item["uom_qty_id"].ToString(),//数量单位
                    UomStatID = item["uom_stat_id"].ToString(),//统计数量单位
                    ContainerDetailID = item["container_d_id"].ToString(),//容器明细ID
                    RefDocType = DocType.DocType_Qc.ToString(),//来源单据类型
                    RefDocID = qcHId,//来源单ID
                    RefDocDetailID = item["t_id"].ToString(),//来源单明细ID
                    RefDocNo = drQcH["qc_no"].ToString(),//来源单号
                    ActStatus = "1"//状态
                });
            }
            else
            {
                _workTaskItem.Insert(new Model.WorkTaskItemModel
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    TaskID = taskId,//主任务ID
                    ItemID = item["item_id"].ToString(),//物料ID
                    ItemName = item["item_name"].ToString(),//物料名称
                    PlanQty = planQty,//计划数量
                    FinishedStatQty = decimal.Parse(item["plan_stat_qty"].ToString()),
                    UomQtyID = item["uom_qty_id"].ToString(),//数量单位
                    UomStatID = item["uom_stat_id"].ToString(),//统计数量单位
                    ContainerDetailID = item["container_d_id"].ToString(),//容器明细ID
                    RefDocType = DocType.DocType_Qc.ToString(),//来源单据类型
                    RefDocID = qcHId,//来源单ID
                    RefDocDetailID = item["t_id"].ToString(),//来源单明细ID
                    RefDocNo = drQcH["qc_no"].ToString(),//来源单号
                    ActStatus = "1"//状态
                });
            }
        }

        /// <summary>
        /// 更新货位状态
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="status"></param>
        private void UpdateLocStatus(string locationId, string status)
        {
            string strSql = @"update &DataSource&wms_m_location l set l.location_status=@status where l.t_id=@t_id
                     and l.client_id='&CLIENT_ID&' and l.org_id='&ORG_ID&' and l.warehouse_id = @warehouse_id
                    ";
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "status", status},
                    { "t_id",  locationId },
                    { "warehouse_id",  Context.WarehouseID}
                  };

            DBHelper.RunSql(strSql, dicParam, "更新货位状态", DBHelper.OperatedNumType.OnlyOne);
        }

        /// <summary>
        ///  更新容器货位id
        /// </summary>
        /// <param name="cId"></param>
        /// <param name="locationId"></param>
        private void UpdateContainerLoc(string cId, string locationId)
        {
            string strSql = @"update &DataSource&wms_b_container_h h set h.location_id=@location_id where h.t_id=@t_id
                             and h.client_id='&CLIENT_ID&' and h.warehouse_id = @warehouse_id ";
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "location_id", locationId},
                    { "t_id",  cId},
                    { "warehouse_id",  Context.WarehouseID}
                  };

            DBHelper.RunSql(strSql, dicParam, "更新库存主档货位", DBHelper.OperatedNumType.OnlyOne);
        }

        /// <summary>
        /// 删除质检明细
        /// </summary>
        /// <param name="param"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private WebJsonInfo DelQcDetail(dynamic param, WebJsonInfo info)
        {
            string strSql = string.Empty;
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "qc_h_id",  param.qc_h_id.ToString() },
                    { "t_id",  param.t_id.ToString() },
                    { "warehouse_id",  Context.WarehouseID}
                  };
            DataRow drQcD = DBHelper.Select(@"select * from &DataSource&WMS_D_QC_D  d where  d.t_id=@t_id and d.act_status=1
                        and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&' and d.warehouse_id = @warehouse_id", dicParam).Rows[0];
            UpdateLocStatus(drQcD["from_location_id"].ToString(), LocationStatus.LocationStatus_Stored.ToString());

            strSql = @"update &DataSource&WMS_D_QC_D  a set a.ACT_STATUS = 0  where a.t_id=@t_id
                     and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&' and a.warehouse_id = @warehouse_id ";
            DBHelper.RunSql(strSql, dicParam, "删除质检明细", DBHelper.OperatedNumType.OnlyOne);


            DataRow drQcH = DBHelper.Select(@"select * from &DataSource&WMS_D_QC_H  t where  t.t_id=@qc_h_id and t.act_status=1
                        and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id", dicParam).Rows[0];

            if (drQcH["qc_doc_status"].ToString() == QCDocStatus.QCDocStatus_Executing.ToString())
            {
                DataRow dr;
                DataRow drTaskItem = DBHelper.Select(@"select * from &DataSource&wms_b_task_item  t where  t.ref_doc_d_id=@t_id and t.act_status=1
                        and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id", dicParam).Rows[0];
                DataRow drTask = _workTask.GetWorkTask(drTaskItem["task_id"].ToString());

                if (drTask["task_status"].ToString() != TaskStatus.TaskStatus_NotExecute.ToString())
                {
                    info.errcode = 500;
                    info.errmsg = "该质检任务已执行,不允许删除";

                    return info;
                }
                strSql = @"delete from &DataSource&wms_b_task_item t where  t.ref_doc_d_id=@t_id 
                           and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id";
                DBHelper.RunSql(strSql, dicParam, "删除任务明细", DBHelper.OperatedNumType.ZeroOrMore);

                dicParam.Add("task_id", drTask["t_id"].ToString());

                dr = DBHelper.Select(@"select count(*) cnt from &DataSource&wms_b_task_item  t where t.ref_doc_id =@qc_h_id and t.task_id=@task_id and t.ref_doc_d_id!=@t_id  and t.act_status=1
                       and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id", dicParam).Rows[0];
                if (decimal.Parse("0" + dr["cnt"].ToString()) == 0)
                {
                    strSql = @"delete from &DataSource&wms_b_task t where  t.ref_doc_id=@qc_h_id
                         and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id";
                    DBHelper.RunSql(strSql, dicParam, "删除任务", DBHelper.OperatedNumType.ZeroOrMore);
                }

                dr = DBHelper.Select(@"select count(*) cnt from  &DataSource&wms_d_qc_d  d where d.qc_h_id=@qc_h_id and d.t_id!=@t_id and d.act_status=1
                                 and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&' and d.warehouse_id = @warehouse_id", dicParam).Rows[0];
                if (decimal.Parse("0" + dr["cnt"].ToString()) == 0)
                {
                    UpdateQcHStatus(param.qc_h_id.ToString(), QCDocStatus.QCDocStatus_NotExecute.ToString());
                }
            }

            return info;
        }

        /// <summary>
        /// 更新库存锁定量
        /// </summary>
        /// <param name="container_d_id">容器明细id</param>
        /// <param name="qty">数量</param>
        private void UpdateContainerLockQty(string container_d_id, decimal qty)
        {
            string strSql = @"update &DataSource&wms_b_container_d d set d.lock_qty = nvl(d.lock_qty,0)+@lock_qty where d.t_id=@t_id
                       and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&' and d.warehouse_id = @warehouse_id";

            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "lock_qty", qty.ToString()},
                    { "t_id",  container_d_id },
                    { "warehouse_id",  Context.WarehouseID}
                  };

            DBHelper.RunSql(strSql, dicParam, "更新库存明细", DBHelper.OperatedNumType.OnlyOne);
        }

        /// <summary>
        /// 更新库存物料质检状态
        /// </summary>
        /// <param name="container_d_id"></param>
        /// <param name="qcStatus"></param>
        private void UpdateContainerQcStatus(string container_d_id, string qcStatus)
        {
            string strSql = @"update &DataSource&wms_b_container_d d set d.qc_status = @qc_status  where d.t_id=@t_id
                       and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&' and d.warehouse_id = @warehouse_id";

            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "qc_status", qcStatus},
                    { "t_id",  container_d_id },
                    { "warehouse_id",  Context.WarehouseID}
                  };

            DBHelper.RunSql(strSql, dicParam, "更新库存明细", DBHelper.OperatedNumType.OnlyOne);
        }

        /// <summary>
        /// 更新库存锁定量、实际量
        /// </summary>
        /// <param name="container_d_id">容器明细id</param>
        /// <param name="qty">数量</param>
        private void UpdateLockQtyAndQty(string container_d_id, decimal? qty)
        {
            string strSql = @"update &DataSource&wms_b_container_d d set 
                              d.qty=nvl(d.qty,0)+@lock_qty,
                              d.lock_qty =nvl(d.lock_qty,0)+@lock_qty where d.t_id=@t_id
                              and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&' and d.warehouse_id = @warehouse_id";
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "lock_qty", qty.ToString()},
                    { "t_id",  container_d_id },
                    { "warehouse_id",  Context.WarehouseID}
                  };

            DBHelper.RunSql(strSql, dicParam, "更新库存明细", DBHelper.OperatedNumType.OnlyOne);
        }

        /// <summary>
        /// 更新库存统计数量和库存数量
        /// </summary>
        /// <param name="container_d_id">容器明细id</param>
        /// <param name="qty">数量</param>
        private void UpdatQtyAndStatQty(string container_d_id, decimal qty, decimal statQty)
        {
            string strSql = @"update &DataSource&wms_b_container_d d set 
                              d.qty=nvl(d.qty,0)+@qty,
                              d.stat_qty =nvl(d.stat_qty,0)+@statQty where d.t_id=@t_id
                              and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&' and d.warehouse_id = @warehouse_id";
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "t_id", container_d_id},
                    { "qty", qty.ToString()},
                    { "statQty",  statQty.ToString() },
                    { "warehouse_id",  Context.WarehouseID}
                  };

            DBHelper.RunSql(strSql, dicParam, "更新库存明细", DBHelper.OperatedNumType.OnlyOne);
        }

        /// <summary>
        /// 查询库存明细
        /// </summary>
        /// <param name="container_d_id">容器明细id</param>
        private DataRow[] GetContainerDetailByCdId(string container_d_id)
        {
            string strSql = @"select *from &DataSource&wms_b_container_d d where d.t_id=@t_id
                     and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&' and d.warehouse_id = @warehouse_id";
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "t_id",  container_d_id },
                    { "warehouse_id",  Context.WarehouseID}
                  };

            return DBHelper.Select(strSql, dicParam).AsEnumerable().ToArray();
        }

        /// <summary>
        /// 更新任务物料
        /// </summary>
        /// <param name="tItemId">id</param>
        /// <param name="qty">操作量</param>
        /// <param name="diffQty">任务剩余量</param>
        private void UpdateTaskItem(string tItemId, decimal? qty, decimal diffQty)
        {
            string strSql = string.Empty;
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "t_id",  tItemId },
                    { "qty", qty.ToString()},
                    { "warehouse_id",  Context.WarehouseID}
                  };

            if (diffQty == qty)
            {
                strSql = @"update &DataSource&wms_b_task_item t
                    set t.processed_qty=nvl(t.processed_qty,0)+@qty,
                    t.finished_qty=t.plan_qty,t.start_date=nvl(t.start_date ,sysdate),t.end_date=sysdate  
                    where t.t_id=@t_id and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id";
            }
            else
            {
                strSql = @"update &DataSource&wms_b_task_item t 
                    set t.processed_qty=nvl(t.processed_qty,0)+@qty,t.start_date=nvl(t.start_date ,sysdate) 
                    where t.t_id=@t_id and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id";
            }

            DBHelper.RunSql(strSql, dicParam, "更新任务明细", DBHelper.OperatedNumType.OnlyOne);
        }


        /// <summary>
        /// 更新任务物料
        /// </summary>
        /// <param name="tItemId">id</param>
        private void UpdateTaskItem(string tItemId)
        {
            string strSql = string.Empty;
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "t_id",  tItemId },
                    { "warehouse_id",  Context.WarehouseID}
                  };
            strSql = @"update &DataSource&wms_b_task_item t
                    set t.processed_qty=t.plan_qty,
                    t.finished_qty=t.plan_qty,t.start_date=nvl(t.start_date ,sysdate),t.end_date=sysdate ,
                    t.PROCESS_STATUS='ProcessStatus_Done'
                    where t.t_id=@t_id and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id";

            DBHelper.RunSql(strSql, dicParam, "更新任务明细", DBHelper.OperatedNumType.OnlyOne);
        }

        /// <summary>
        /// 更新质检单源货位
        /// </summary>
        /// <param name="qcDId"></param>
        /// <param name="locId"></param>
        /// <param name="locName"></param>
        private void UpdateQcDetailSrcloc(string qcDId, string locId, string locName)
        {
            string strSql = string.Empty;
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "t_id",  qcDId },
                    { "from_location_id", locId},
                    { "from_location_name", locName},
                    { "warehouse_id",  Context.WarehouseID}
                  };
            strSql = @"update &DataSource&wms_d_qc_d  t
                    set t.from_location_id=@from_location_id,t.from_location_name=@from_location_name
                    where t.t_id=@t_id and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id";

            DBHelper.RunSql(strSql, dicParam, "更新质检明细源地址", DBHelper.OperatedNumType.OnlyOne);
        }

        /// <summary>
        /// 更新质检单明细
        /// </summary>
        /// <param name="qcDId"></param>
        /// <param name="locId"></param>
        /// <param name="locName"></param>
        private void UpdateQcDetail(string qcDId, string locId, string locName, decimal qty, decimal statQty)
        {
            string strSql = string.Empty;
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "t_id",  qcDId },
                    { "to_location_id", locId},
                    { "to_location_name", locName},
                    { "qty", qty.ToString()},
                    { "stat_qty", statQty.ToString()},
                    { "warehouse_id",  Context.WarehouseID}
                  };
            strSql = @"update  &DataSource&wms_d_qc_d  t
                    set t.to_location_id=@to_location_id,t.to_location_name=@to_location_name,
                        t.processed_qty=@qty,t.finished_qty=@qty,t.finished_stat_qty=@stat_qty
                    where t.t_id=@t_id and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id";

            DBHelper.RunSql(strSql, dicParam, "更新质检明细目的地址", DBHelper.OperatedNumType.OnlyOne);
        }

        /// <summary>
        /// 获取质检信息
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        private Tuple<DataTable, DataTable> GetQcInfo(string barCode)
        {
            DataTable dtContainerH = DBHelper.Select(@"select h.warehouse_id, h.t_id from  &DataSource&wms_b_container_h h where h.container_barcode =@barcode
                                  and h.client_id='&CLIENT_ID&' and h.org_id='&ORG_ID&' and h.warehouse_id = @warehouse_id ", new Dictionary<string, object>() {
                    { "barcode", barCode },
                    { "warehouse_id", Context.WarehouseID }
                }, "对应托盘", true);

            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam.Add("container_id", dtContainerH.Rows[0]["t_id"].ToString());
            dicParam.Add("warehouse_id", Context.WarehouseID);

            DataTable dtContainer = DBHelper.Select(@"select  distinct （select l.zone_id from  &DataSource&wms_m_location  l where  l.t_id=d1.from_location_id) zone_id,d1.qc_h_id,d4.qc_no,d2.container_id,d3.container_barcode
                        from &DataSource&wms_d_qc_d d1
                        left join  &DataSource&wms_d_qc_h d4 on d1.qc_h_id=d4.t_id
                        left join &DataSource&wms_b_container_d d2 on d1.container_d_id=d2.t_id
                        left join &DataSource&wms_b_container_h d3 on d3.t_id=d2.container_id where d2.container_id=@container_id and  d1.act_status=1 and  d4.qc_doc_status='QCDocStatus_Executing'
                        and d1.client_id='&CLIENT_ID&' and d1.org_id='&ORG_ID&' and d1.warehouse_id = @warehouse_id ", dicParam, "质检单", true);

            DataTable dtQcDetail = DBHelper.Select(@"select 
                        d1.t_id,d2.container_id,d2.t_id container_d_id,d1.plan_qty,d1.plan_stat_qty,d1.item_id,d1.item_name,d1.uom_qty_id,d1.uom_stat_id
                        from &DataSource&wms_d_qc_d d1
                        left join  &DataSource&wms_d_qc_h d4 on d1.qc_h_id=d4.t_id
                        left join &DataSource&wms_b_container_d d2 on d1.container_d_id=d2.t_id
                        left join &DataSource&wms_b_container_h d3 on d3.t_id=d2.container_id where d2.container_id=@container_id and  d1.act_status=1 and  d4.qc_doc_status='QCDocStatus_Executing'
                        and d1.client_id='&CLIENT_ID&' and d1.org_id='&ORG_ID&' and d1.warehouse_id = @warehouse_id ", dicParam, "质检单明细", true);

            return new Tuple<DataTable, DataTable>(dtContainer, dtQcDetail);
        }

        /// <summary>
        /// 获取质检明细
        /// </summary>
        /// <param name="tId"></param>
        /// <returns></returns>
        private DataRow QueryQcDetailById(string tId)
        {
            DataTable dt = DBHelper.Select(@"select * from  &DataSource&wms_d_qc_d h where h.t_id =@t_id
                                  and h.client_id='&CLIENT_ID&' and h.org_id='&ORG_ID&' and h.warehouse_id = @warehouse_id ", new Dictionary<string, object>() {
                    { "t_id", tId },
                    { "warehouse_id", Context.WarehouseID }
                }, "获取质检明细", true);

            return dt.Rows[0];
        }


        /// <summary>
        /// 获取质检明细
        /// </summary>
        /// <param name="tHId"></param>
        /// <returns></returns>
        private DataRow[] QueryQcDetailByHId(string tHId)
        {
            DataTable dt = DBHelper.Select(@"select * from  &DataSource&wms_d_qc_d h where h.qc_h_id =@qc_h_id
                                  and h.client_id='&CLIENT_ID&' and h.org_id='&ORG_ID&' and h.warehouse_id = @warehouse_id ", new Dictionary<string, object>() {
                    { "qc_h_id", tHId },
                    { "warehouse_id", Context.WarehouseID }
                }, "获取质检明细", true);

            return dt.AsEnumerable().ToArray();
        }

        private string ToCheckResult(string checkResult)
        {
            string qcStatus = string.Empty;

            if (checkResult == QCCheckResult.CheckResult_Avoid.ToString())
            {
                qcStatus = QcStatus.QcStatus_CheckAvoid.ToString();
            }
            else if (checkResult == QCCheckResult.CheckResult_Failed.ToString())
            {
                qcStatus = QcStatus.QcStatus_CheckFailed.ToString();
            }
            else if (checkResult == QCCheckResult.CheckResult_Passed.ToString())
            {
                qcStatus = QcStatus.QcStatus_CheckPassed.ToString();
            }

            return qcStatus;
        }

        #endregion
    }
}