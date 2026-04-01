using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Common.Interface;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Tasking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NtiCloud.Service.WMS.Inventory;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using static NtiCloud.Model.WMS.CountContainerInput;
using System.Threading.Tasks;
using TaskStatus = NtiCloud.Service.WMS.Base.TaskStatus;
using NtiCloud.Service.LPS.Inbound;

namespace NtiCloud.Service.WMS.CountContainer
{
    public class CountContainer : WmsObject, ICountContainer
    {
        private readonly IWorkTask _workTask;
        private readonly IWorkTaskItem _workTaskItem;
        private readonly IContainer _container;

        public CountContainer(IWorkTask workTask, IWorkTaskItem workTaskItem, IContainer container)
        {
            _workTask = workTask;
            _workTaskItem = workTaskItem;
            _container = container;
        }

        public CountContainer()
        {
            _workTask = new WorkTask();
            _workTaskItem = new WorkTaskItem();
            _container = new Container();
        }

        #region 删除盘点明细
        /// <summary>
        /// 删除盘点明细
        /// </summary>
        /// <param name="countDetailID">盘点明细ID</param>
        /// <returns></returns>
        public WebJsonInfo DelCountDetail(string countDetailID)
        {
            try
            {
                string o_msg = "";
                string sql = "";
                string selectOne = "";
                string locationID = "";
                int runCount = 0;
                Dictionary<string, object> param = new Dictionary<string, object>();

                sql = @"select from_location_id from &DataSource&WMS_D_COUNT_D d where d.T_ID = @T_ID";
                param.Clear();
                param.Add("T_ID", countDetailID);
                locationID = DBHelper.SelectOne(sql, param, "查询盘点明细是否存在", false);
                if (string.IsNullOrEmpty(locationID))
                {
                    throw new WmsException(500,"盘点明细不存在");
                }

                sql = @"delete &DataSource&WMS_D_COUNT_D d where d.T_ID=@T_ID";
                param.Clear();
                param.Add("T_ID", countDetailID);
                runCount = DBHelper.RunSql(sql, param, "删除盘点明细");
                if (runCount < 1)
                {
                    throw new WmsException(500,"没有删除任何数据");
                }

                sql = @"select count(1) from &DataSource&WMS_D_COUNT_D d where d.from_location_id=@from_location_id";
                param.Clear();
                param.Add("from_location_id", locationID);
                selectOne = DBHelper.SelectOne(sql, param, "查询同货位盘点明细是否存在", false);
                if (Convert.ToInt32(selectOne)==0) //不存在则更新货位为存储
                {
                    sql = "update &DataSource&wms_m_location l set l.location_status='LocationStatus_Stored' where l.t_id=@from_location_id";
                    param.Clear();
                    param.Add("from_location_id", locationID);
                    int resCount = DBHelper.RunSql(sql, param, "更新盘点明细货位为已存储");
                    if (resCount < 1)
                    {
                        throw new WmsException(500, "更新盘点明细货位为已存储有误");
                    }
                }

                
                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = null, json = null };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 添加盘点明细
        /// <summary>
        /// 添加盘点明细
        /// </summary>
        /// <param name="detailIDList">添加盘点明细IDList</param>
        /// <returns></returns>
        public WebJsonInfo AddCountDetail(string countID, IList<string> detailIDList)
        {
            try
            {
                string o_msg = "";
                string sql = "";
                string selectOne = "";
                int runCount = 0;
                Dictionary<string, object> param = new Dictionary<string, object>();

                string idStr = "";
                foreach (string id in detailIDList)
                {
                    idStr = (idStr == "") ? ("'" + id + "'") : (idStr + ",'" + id + "'");
                }
                sql = @"select count(1) from &DataSource&wms_b_container_d d where d.T_ID in ("+idStr+")";
                param.Clear();
                selectOne = DBHelper.SelectOne(sql, param, "查询盘点明细是否存在", false);
                if (Convert.ToInt32(selectOne) < 1)
                {
                    throw new WmsException(500,"选择的盘点明细不存在");
                }

                sql = @"select * from &DataSource&wms_b_container_d d where d.T_ID in (" + idStr + ")";
                param.Clear();
                DataTable dt = DBHelper.Select(sql, param, "查询盘点明细", false);

                var containerList = dt.AsEnumerable().Select(o => new { container_id = o.Field<string>("container_id") }).GroupBy(o => o.container_id).ToList();
                foreach (var containerid in containerList)
                {
                    sql = @"select c.container_barcode from &DataSource&wms_d_count_h h  inner join &DataSource&wms_d_count_d d on h.t_id = d.count_h_id
                            inner join &DataSource&wms_b_container_h c on c.t_id=d.container_id
                            where h.t_id<>@countid and d.container_id=@container_id and h.count_status<>@OverState";
                    param.Clear();
                    param.Add("countid", countID);
                    param.Add("container_id", containerid.Key);
                    param.Add("OverState", CountState.CountDocStatus_Executed.ToString());
                    DataTable temp = DBHelper.Select(sql, param, "查询盘点容器是否存已存在盘点任务", false);
                    if (temp != null && temp.Rows.Count > 0)
                    {
                        throw new WmsException(500,"选择的盘点明细已存在盘点任务,托盘：" + temp.Rows[0]["container_barcode"].ToString());
                    }
                }

                foreach (var containerid in containerList)
                {
                    sql = @"select d.*,l.LOCATION_CODE,l.t_id LOCATION_ID,l.LOCATION_NAME,d.t_id CONTAINER_D_ID  from &DataSource&wms_b_container_d d inner join &DataSource&wms_b_container_h h on d.container_id = h.t_id inner join &DataSource&wms_m_location l on l.t_id = h.location_id where d.container_id=@container_id and l.ACT_STATUS='1' and h.ACT_STATUS='1' and d.ACT_STATUS='1'";
                    param.Clear();
                    param.Add("container_id", containerid.Key);
                    DataTable temp = DBHelper.Select(sql, param, "查询盘点明细所在容器的所有物料", false);
                    if (temp == null && temp.Rows.Count < 0)
                    {
                        throw new WmsException(500,"选择的盘点明细不存在");
                    }

                    foreach (DataRow containerdetailRow in temp.Rows)
                    {
                        //sql = @"insert into &DataSource&swms_b_count_d(t_id,client_id, warehouse_id,count_id,container_id,CONTAINER_D_ID,location_number,create_date,created_by,act_status,ISAUTOADD)
                        //    values(getntiid(),'&CLIENT_ID&','&WAREHOUSE_ID&',@header_id,@container_id,@CONTAINER_D_ID,@location_number,sysdate,@created_by,'1',@ISAUTOADD)";
                        sql = @"INSERT INTO &DataSource&WMS_D_COUNT_D
                              (t_id,
                               client_id,
                               org_id,
                               warehouse_id,
                               count_h_id,
                               from_location_id,
                               from_location_no,
                               from_location_name,
                               to_location_id,
                               to_location_no,
                               to_location_name,
                               process_status,
                               diff_qty,
                               uom_qty_id,
                               diff_stat_qty,
                               uom_stat_id,
                               result,
                               comfirmor_id,
                               confirmation_date,
                               memo,
                               create_date,
                               created_by,
                               modify_ip,
                               modify_date,
                               last_modified_by,
                               CONTAINER_ID,
                               CONTAINER_D_ID,
                               ITEMCODE,
                               ITEMNAME,
                               ISAUTOADD,
                               OLD_QTY,
                               act_status)
                            VALUES
                              ( getntiid(),
                               '&CLIENT_ID&',
                               '&ORG_ID&',
                               '&WAREHOUSE_ID&',
                               @count_h_id,
                               @from_location_id,
                               @from_location_no,
                               @from_location_name,
                               '',
                               '',
                               '',
                               'CountDocStatus_NotExecute',
                               0,
                               null,
                               null,
                               null,
                               null,
                               null,
                               null,
                               null,
                               now(),
                               @created_by,
                               '',
                               now(),
                               '',
                               @container_id,
                               @CONTAINER_D_ID,
                               (select i.item_code from &DataSource&wms_m_item i where i.t_id = @itemid),
                               @itemname,
                               @ISAUTOADD,
                               (select qty from &DataSource&WMS_B_CONTAINER_D d where d.t_id = @CONTAINER_D_ID),
                               '1')";
                        string ISAUTOADD = "1";
                        if (detailIDList.Contains(containerdetailRow["t_id"].ToString()))
                        {
                            ISAUTOADD = "0";
                        }

                        param.Clear();
                        param.Add("count_h_id", countID);
                        param.Add("from_location_id", containerdetailRow["location_id"].ToString());
                        param.Add("from_location_no", containerdetailRow["LOCATION_CODE"].ToString());
                        param.Add("from_location_name", containerdetailRow["location_name"].ToString());
                        param.Add("container_id", containerdetailRow["container_id"].ToString());
                        param.Add("CONTAINER_D_ID", containerdetailRow["CONTAINER_D_ID"].ToString());
                        param.Add("location_number", containerdetailRow["LOCATION_CODE"].ToString());
                        param.Add("created_by", Context.UserInfo.UserName);
                        param.Add("itemname", containerdetailRow["ITEM_NAME"].ToString());
                        param.Add("itemid", containerdetailRow["ITEM_ID"].ToString());
                        param.Add("ISAUTOADD", ISAUTOADD);
                        runCount = DBHelper.RunSql(sql, param, "插入盘点明细");
                        if (runCount < 1)
                        {
                            throw new WmsException(500,"插入盘点明细有误");
                        }
                    }

                    sql = "update &DataSource&wms_m_location l set l.location_status='LocationStatus_Out' where l.t_id=@locationID";
                    param.Clear();
                    param.Add("locationID", temp.Rows[0]["LOCATION_ID"].ToString());
                    runCount = DBHelper.RunSql(sql, param, "更新盘点明细货位为待出库");
                    if (runCount < 1)
                    {
                        throw new WmsException(500, "更新盘点明细货位为待出库");
                    }
                }

                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "操作成功", json = null };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 执行盘点单，产生盘点任务
        /// <summary>
        /// 外部-盘点-立库，执行盘点单，生成盘点任务
        /// </summary>
        /// <param name="countHeadID">盘点单ID</param>
        /// <returns></returns>
        public WebJsonInfo GenCountTask(string countHeadID)
        {
            try
            {
                string o_msg = "";
                string sql = "";
                string selectOne = "";
                Dictionary<string, object> param = new Dictionary<string, object>();

                sql = @"select count(1) from &DataSource&WMS_D_COUNT_H h where h.T_ID=@CountHeadID";
                param.Clear();
                param.Add("CountHeadID", countHeadID);
                selectOne = DBHelper.SelectOne(sql, param, "查询盘点单据是否存在", false);
                if (Convert.ToInt32(selectOne) < 1)
                {
                    throw new WmsException(500, "盘点单不存在");
                }


                sql = @"select count(1) from &DataSource&WMS_D_COUNT_D d where d.COUNT_H_ID=@CountHeadID";
                param.Clear();
                param.Add("CountHeadID", countHeadID);
                selectOne = DBHelper.SelectOne(sql, param, "查询盘点单明细是否存在", false);
                if (Convert.ToInt32(selectOne) < 1)
                {
                    throw new WmsException(500, "盘点单明细不存在");
                }

                sql = @"select * from &DataSource&WMS_D_COUNT_H h where h.T_ID=@CountHeadID";
                param.Clear();
                param.Add("CountHeadID", countHeadID);
                DataTable dtH = DBHelper.Select(sql, param, "查询盘点单", false);
                if (dtH.Rows[0]["COUNT_STATUS"].ToString() != "CountDocStatus_NotExecute")
                {
                    throw new WmsException(500, "盘点单状态不可执行");
                }

                sql = @"select * from &DataSource&WMS_D_COUNT_D D where D.COUNT_H_ID=@CountHeadID";
                param.Clear();
                param.Add("CountHeadID", countHeadID);
                DataTable dtD = DBHelper.Select(sql, param, "查询盘点单明细", false);
                if (dtH.Rows[0]["COUNT_STATUS"].ToString() != "CountDocStatus_NotExecute")
                {
                    throw new WmsException(500, "盘点单状态不可执行");
                }

                if (dtH.Rows[0]["DEST_ADDR_ID"] == null)
                {
                    throw new WmsException(500, "未指定目标地址");
                }

                string countType = dtH.Rows[0]["COUNT_TYPE"].ToString(); //盘点类型（在库盘不产生任务，出库盘产生任务）

                if(countType == "CountType_OnLine") //线上盘既出库盘，需产生任务
                {
                    //生成盘点任务

                    var ContainerGroup = from p in dtD.AsEnumerable()
                                         group p by p.Field<string>("CONTAINER_ID") into g
                                         select new
                                         {
                                             CONTAINER_ID = g.Key,
                                             Data = g
                                         };


                    List<string> taskIDList = new List<string>();
                    foreach (var cell in ContainerGroup)
                    {
                        IList<DataRow> drow = cell.Data.ToList<DataRow>();
                        string taskID = InsertTask(dtH, drow);
                        taskIDList.Add(taskID);
                    }

                    WorkTaskInterface.Send(taskIDList);
                }
                


                //更改单据状态
                sql = @"update &DataSource&WMS_D_COUNT_H h set h.COUNT_STATUS='CountDocStatus_Executing',START_DATE=now() where h.T_ID=@CountHeadID";
                param.Clear();
                param.Add("CountHeadID", countHeadID);
                int res = DBHelper.RunSql(sql, param, "更新盘点单状态");
                if (res < 1)
                {
                    throw new WmsException(500, "更新盘点状态失败");
                }
                //更改单据明细状态
                sql = @"update &DataSource&WMS_D_COUNT_D h set h.PROCESS_STATUS='CountDocStatus_Executing' where h.COUNT_H_ID=@CountHeadID";
                param.Clear();
                param.Add("CountHeadID", countHeadID);
                res = DBHelper.RunSql(sql, param, "更新盘点单状态",DBHelper.OperatedNumType.OneOrMore);
                if (res < 1)
                {
                    throw new WmsException(500, "更新盘点明细状态失败");
                }

                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "", json = null };

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string InsertTask(DataTable dtH, IList<DataRow> dtD)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string containerBarCode = "";
                string sql = "";
                string selectOne = "";

                Route route = new Route();
                string anyItemCode = dtD[0]["ITEMCODE"].ToString();
                route.Init(Item.Get(anyItemCode)["catalog_id"].ToString(), RoutePurpose.RoutePurpose_CountOut.ToString());

                sql = "select t.container_barcode from &DataSource&wms_b_container_h t where t.t_id=@t_id ";
                param.Clear();
                param.Add("t_id", dtD[0]["CONTAINER_ID"].ToString());
                containerBarCode = DBHelper.SelectOne(sql, param, "查询盘点托盘编码", false);

                Address src = new Address()
                {
                    ID = dtD[0]["FROM_LOCATION_ID"].ToString(),
                    Type = "AddressType_Location",
                    Text = dtD[0]["FROM_LOCATION_NAME"].ToString(),
                };

                //sql = "select l.location_type from &DataSource&wms_m_location l where l.t_id=@t_id ";
                //param.Clear();
                //param.Add("t_id", dtD[0]["FROM_LOCATION_ID"].ToString());
                //selectOne = DBHelper.SelectOne(sql, param, "查询盘点货位类型", false);

                Address dest = new Address()
                {
                    ID = dtH.Rows[0]["DEST_ADDR_ID"].ToString(),
                    Type = "AddressType_Station",
                    Text = dtH.Rows[0]["DEST_ADDR_TEXT"].ToString(),
                };

                if (!route.MatchRoute(src, dest, out DataRow[] sequences))
                {
                    throw new WmsException(500, "无法找到合适的配置来生成任务");
                }

                string CONTAINER_ID = dtD[0]["CONTAINER_ID"].ToString();
                string RefDocNo = dtH.Rows[0]["COUNT_NO"] == null ? "" : dtH.Rows[0]["COUNT_NO"].ToString();

                string RefDocID = dtH.Rows[0]["T_ID"] == null ? "" : dtH.Rows[0]["T_ID"].ToString();
                string RefDocDetailID = dtD.FirstOrDefault(o => o.Field<string>("CONTAINER_ID") == CONTAINER_ID).Field<string>("T_ID");

                string taskID = SnowflakeID.Guid().ToString();
                Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
                {
                    ID = taskID,
                    SeqNo = 1,
                    Priority = 100,
                    ContainerID = CONTAINER_ID,
                    ContainerBarcode = containerBarCode,
                    SrcAddrID = src.ID,
                    SrcAddrType = src.Type,
                    SrcAddrText = src.Text,
                    DestAddrID = dest.ID,
                    DestAddrType = dest.Type,
                    DestAddrText = dest.Text,
                    RefDocType = DocType.DocType_Count.ToString(),
                    RefDocID = RefDocID,
                    RefDocNo = RefDocNo,
                    RefDocDetailID = RefDocDetailID,
                    IsExecutable = "1",
                    TaskStatus = TaskStatus.TaskStatus_Executing.ToString()
                };

                List<WorkTaskItemModel> taskItemList = GetWorkItemList(dtD);

                if (taskItemList == null)
                {
                    throw new WmsException(500,"产生任务明细出错");
                }

                WorkTask.SaveTask(sequences, taskModel, taskItemList);
                return taskID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 产生盘点任务明细
        /// </summary>
        /// <param name="rowsInSameTask"></param>
        /// <returns></returns>
        private List<WorkTaskItemModel> GetWorkItemList(IList<DataRow> rowsInSameTask)
        {
            List<WorkTaskItemModel> list = new List<WorkTaskItemModel>();
            foreach (DataRow row in rowsInSameTask)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string containerBarCode = "";
                string selectOne = "";
                string itemcode = row["ITEMCODE"] == null ? "" : row["ITEMCODE"].ToString();
                string sql = "select * from &DataSource&wms_m_item i where i.item_code=@itemcode";
                param.Clear();
                param.Add("itemcode", itemcode);
                DataTable dtItem = DBHelper.Select(sql, param, "盘点查询物料信息", false);
                if (dtItem == null || dtItem.Rows.Count != 1)
                {
                    return null;
                }

                sql = "select qty from &DataSource&wms_b_container_d d where d.t_id=@detailID";
                param.Clear();
                param.Add("detailID", row["CONTAINER_D_ID"].ToString());
                selectOne = DBHelper.SelectOne(sql, param, "查询盘点物料数量", false);
                decimal qty = 0;
                bool parseRes = decimal.TryParse(selectOne, out qty);
                if (!parseRes)
                {
                    return null;
                }

                if (dtItem == null || dtItem.Rows.Count != 1)
                {
                    return null;
                }

                list.Add(
                    new WorkTaskItemModel()
                    {
                        ItemID = dtItem.Rows[0]["T_ID"].ToString(),
                        ItemName = row["ITEMNAME"].ToString(),
                        PlanQty = qty,
                        ProcessedQty = 0,
                        FinishedQty = 0,
                        FinishedStatQty = 0,
                        ContainerDetailID = row["CONTAINER_D_ID"].ToString(),
                        ProcessStatus = ProcessStatus.ProcessStatus_Doing.ToString()
                    });
            }
            return list;
        }


        #endregion

        #region WCS完成盘点出库任务
        /// <summary>
        /// WCS完成盘点出库任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="taskStatus">任务状态</param>
        public void FinishOutCount(string taskId, TaskStatus taskStatus)
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
                UpdateContainerLoc(containerH["t_id"].ToString(), string.Empty);
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
        /// <summary>
        /// 空托盘任务完成
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskStatus"></param>
        public void FinishOutEmptyContaiener(string taskId, TaskStatus taskStatus)
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
                // WCS单独的部分完成接口已经把货位状态置为空
                //UpdateLocStatus(containerH["location_id"].ToString(), LocationStatus.LocationStatus_Empty.ToString());//更新货位状态  //已经在部分完成中释放货位   
                UpdateContainerLoc(containerH["t_id"].ToString(), string.Empty);
            }

            if (taskStatus == TaskStatus.TaskStatus_Finished)
            {
                DataRow[] drTaskItem = _workTaskItem.GetByTaskID(taskId.ToString());
                foreach (DataRow item in drTaskItem)
                {
                    UpdateTaskItem(item["t_id"].ToString());
                }
                _container.DeleteDetails(cId);
                _container.UpdateHeadActStatus(cId, "0");
            }

            _workTask.MoveToHistory(taskId);
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
                             and h.client_id='&CLIENT_ID&' and h.org_id='&ORG_ID&' and h.warehouse_id = @warehouse_id ";
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "location_id", locationId},
                    { "t_id",  cId},
                    { "warehouse_id",  Context.WarehouseID}
                  };

            DBHelper.RunSql(strSql, dicParam, "更新库存主档货位", DBHelper.OperatedNumType.OnlyOne);
        }

        /// <summary>
        /// 更新任务物料
        /// </summary>
        /// <param name="tItemId">id</param>
        /// <param name="qty">操作量</param>
        /// <param name="diffQty">任务剩余量</param>
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
        #endregion

        #region 盘点回库任务完成
        public void FinishInCount(string taskId, TaskStatus taskStatus)
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
                UpdateLocStatus(containerH["location_id"].ToString(), LocationStatus.LocationStatus_Stored.ToString());//更新货位状态     
                UpdateContainerLoc(containerH["t_id"].ToString(), string.Empty);
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

        #region WMS/RF盘点确认
        /// <summary>
        /// WMS/RF盘点确认
        /// </summary>
        /// <param name="countDetailID">盘点明细ID</param>
        /// <param name="isConErr">是否条码错误，0正确，1错误</param>
        /// <param name="remark">备注</param>
        /// <param name="qtyCount">盘点数量</param>
        /// <returns></returns>
        public WebJsonInfo ConfirmCountQty(string countDetailID, string isConErr, string remark, decimal qtyCount)
        {
            try
            {
                string o_msg = "";
                string sql = "";
                string selectOne = "";
                int ResCount = 0;
                Dictionary<string, object> param = new Dictionary<string, object>();
                sql = @"select count(1) from &DataSource&WMS_D_COUNT_D d where d.T_ID=@T_ID";
                param.Clear();
                param.Add("T_ID", countDetailID);
                selectOne = DBHelper.SelectOne(sql, param, "查询盘点明细是否存在", false);
                if (Convert.ToInt32(selectOne) < 1)
                {
                    throw new WmsException(500, "盘点明细不存在");
                }

                sql = "update  &DataSource&WMS_D_COUNT_D d set d.COUNT_QTY=@qtyCount ,DIFF_QTY=(@qtyCount-ifnull(OLD_QTY,0)),MEMO=@remark,ISCODEERR=@isConErr,PROCESS_STATUS='CountDocStatus_Confirmed' where d.T_ID=@T_ID";
                param.Clear();
                param.Add("T_ID", countDetailID);
                param.Add("qtyCount", qtyCount.ToString());
                param.Add("remark", remark);
                param.Add("isConErr", isConErr);
                ResCount = DBHelper.RunSql(sql, param, "更新盘点明细结果");
                if (ResCount > 0)
                {
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "", json = null };
                }
                else
                {
                    throw new WmsException(500, "更新盘点明细失败");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region RF或WCS调用，产生盘点回库任务
        /// <summary>
        /// RF调用入库任务
        /// </summary>
        /// <param name="CountHeadID">盘点主表ID</param>
        /// <param name="barcode">托盘条码</param>
        /// <returns></returns>
        public WebJsonInfo RFSingleGenInCountTask(string countHeadID, string barcode)
        {
            string o_msg = "";
            string sql = "";
            string selectOne = "";
            Dictionary<string, object> param = new Dictionary<string, object>();

            sql = @"select count(1) from &DataSource&WMS_D_COUNT_H h where h.T_ID=@CountHeadID";
            param.Clear();
            param.Add("CountHeadID", countHeadID);
            selectOne = DBHelper.SelectOne(sql, param, "查询盘点单据是否存在", true);
            if (Convert.ToInt32(selectOne) < 1)
            {
                throw new WmsException(500, "盘点单不存在");
            }


            sql = @"select count(1) from &DataSource&WMS_D_COUNT_D d where d.COUNT_H_ID=@CountHeadID";
            param.Clear();
            param.Add("CountHeadID", countHeadID);
            selectOne = DBHelper.SelectOne(sql, param, "查询盘点单明细是否存在", true);
            if (Convert.ToInt32(selectOne) < 1)
            {
                throw new WmsException(500, "盘点单明细不存在");
            }

            sql = @"select * from &DataSource&WMS_D_COUNT_H h where h.T_ID=@CountHeadID";
            param.Clear();
            param.Add("CountHeadID", countHeadID);
            DataTable dtH = DBHelper.Select(sql, param, "查询盘点单", false);

            sql = @"select * from &DataSource&wms_b_container_h h where h.CONTAINER_BARCODE=@barcode and h.ACT_STATUS='1' ";
            param.Clear();
            param.Add("barcode", barcode);
            DataTable containerDt = DBHelper.Select(sql, param, "查询盘点单", false);

            string cont_tID = containerDt.Rows[0]["T_ID"].ToString();

            sql = @"select * from &DataSource&WMS_D_COUNT_D D where D.COUNT_H_ID=@CountHeadID and D.CONTAINER_ID=@CONTAINER_ID";
            param.Clear();
            param.Add("CountHeadID", countHeadID);
            param.Add("CONTAINER_ID", cont_tID);
            DataTable dtD = DBHelper.Select(sql, param, "查询指定托盘的盘点单明细", false);


            //生成盘点任务
            var ContainerGroup = from p in dtD.AsEnumerable()
                                 group p by p.Field<string>("CONTAINER_ID") into g
                                 select new
                                 {
                                     CONTAINER_ID = g.Key,
                                     Data = g
                                 };

            foreach (var cell in ContainerGroup)
            {
                IList<DataRow> drow = cell.Data.ToList<DataRow>();
                bool isSuccess = InsertTask_Back(dtH, drow);
                if (!isSuccess)
                {
                    throw new WmsException(500, "生成盘点任务失败");
                }
            }

            //更改单据状态
            sql = @"update &DataSource&WMS_D_COUNT_H h set h.COUNT_STATUS='CountDocStatus_Confirmed',START_DATE=now() where h.T_ID=@CountHeadID";
            param.Clear();
            param.Add("CountHeadID", countHeadID);
            int res = DBHelper.RunSql(sql, param, "更新盘点单状态");
            if (res < 1)
            {
                throw new WmsException(500, "更新盘点状态失败");
            }
            //更改单据明细状态
            sql = @"update &DataSource&WMS_D_COUNT_D d set d.PROCESS_STATUS='CountDocStatus_Confirmed' where d.COUNT_H_ID=@CountHeadID  and d.CONTAINER_ID=@CONTAINER_ID";
            param.Clear();
            param.Add("CountHeadID", countHeadID);
            param.Add("CONTAINER_ID", cont_tID);
            res = DBHelper.RunSql(sql, param, "更新盘点单状态");
            if (res < 1)
            {
                throw new WmsException(500, "更新盘点明细状态失败");
            }

            return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "", json = null };
        }

        /// <summary>
        /// 生成盘点任务
        /// </summary>
        /// <param name="dtH"></param>
        /// <param name="dtD"></param>
        /// <returns></returns>
        private bool InsertTask_Back(DataTable dtH, IList<DataRow> dtD)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string containerBarCode = "";
                string sql = "";
                string selectOne = "";

                Route route = new Route();
                string anyItemCode = dtD[0]["ITEMCODE"].ToString();
                route.Init(Item.Get(anyItemCode)["catalog_id"].ToString(), RoutePurpose.RoutePurpose_CountIn.ToString());

                sql = "select t.container_barcode from &DataSource&wms_b_container_h t where t.t_id=@t_id ";
                param.Clear();
                param.Add("t_id", dtD[0]["CONTAINER_ID"].ToString());
                containerBarCode = DBHelper.SelectOne(sql, param, "查询盘点托盘编码", false);


                sql = "select t_id from &DataSource&wms_m_item t where t.ITEM_CODE=@ITEM_CODE ";
                param.Clear();
                param.Add("ITEM_CODE", anyItemCode);
                string itemid = DBHelper.SelectOne(sql, param, "查询物料ID", false);

                Putaway putaway = new Putaway();
                string location = putaway.AllocateOneLocationForPallet(dtH.Rows[0]["WAREHOUSE_ID"].ToString(), itemid);

                sql = "select t.location_code from &DataSource&wms_m_location t where t.t_id=@t_id ";
                param.Clear();
                param.Add("t_id", location);
                string locationName = DBHelper.SelectOne(sql, param, "查询货位编码");

                Address dest = new Address();
                //dest.ID = location;
                dest.Type = "AddressType_Zone";
                //dest.Text = locationName;

                Address src = new Address()
                {
                    ID = dtH.Rows[0]["DEST_ADDR_ID"].ToString(),
                    Type = "AddressType_Station",
                    Text = dtH.Rows[0]["DEST_ADDR_TEXT"].ToString(),
                };

                if (!route.MatchRoute(src, dest, out DataRow[] sequences))
                {
                    throw new WmsException(500, "无法找到合适的配置来生成任务");
                }

                string CONTAINER_ID = dtD[0]["CONTAINER_ID"].ToString();
                string RefDocNo = dtH.Rows[0]["COUNT_NO"] == null ? "" : dtH.Rows[0]["COUNT_NO"].ToString();

                string RefDocID = dtH.Rows[0]["T_ID"] == null ? "" : dtH.Rows[0]["T_ID"].ToString();
                string RefDocDetailID = dtD.FirstOrDefault(o => o.Field<string>("CONTAINER_ID") == CONTAINER_ID).Field<string>("T_ID");

                Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
                {
                    SeqNo = 1,
                    Priority = 100,
                    ContainerID = CONTAINER_ID,
                    ContainerBarcode = containerBarCode,
                    SrcAddrID = src.ID,
                    SrcAddrType = src.Type,
                    SrcAddrText = src.Text,
                    DestAddrID = dest.ID,
                    DestAddrType = dest.Type,
                    DestAddrText = dest.Text,
                    RefDocType = DocType.DocType_Count.ToString(),
                    TaskType = TaskType.TaskType_InBound.ToString(),
                    RefDocID = RefDocID,
                    RefDocNo = RefDocNo,
                    RefDocDetailID = RefDocDetailID,
                    IsExecutable = "1",
                    TaskStatus = "TaskStatus_NotExecute"
                };

                List<WorkTaskItemModel> taskItemList = GetWorkItemList_Back(dtD);

                if (taskItemList == null)
                {
                    return false;
                }

                decimal taskNum = WorkTask.SaveTask(sequences, taskModel, taskItemList);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<WorkTaskItemModel> GetWorkItemList_Back(IList<DataRow> rowsInSameTask)
        {
            List<WorkTaskItemModel> list = new List<WorkTaskItemModel>();
            foreach (DataRow row in rowsInSameTask)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string containerBarCode = "";
                string selectOne = "";
                string itemcode = row["ITEMCODE"] == null ? "" : row["ITEMCODE"].ToString();
                string sql = "select * from &DataSource&wms_m_item i where i.item_code=@itemcode";
                param.Clear();
                param.Add("itemcode", itemcode);
                DataTable dtItem = DBHelper.Select(sql, param, "盘点查询物料信息", false);
                if (dtItem == null || dtItem.Rows.Count != 1)
                {
                    return null;
                }

                sql = "select qty from &DataSource&wms_b_container_d d where d.t_id=@detailID";
                param.Clear();
                param.Add("detailID", row["CONTAINER_D_ID"].ToString());
                selectOne = DBHelper.SelectOne(sql, param, "查询盘点物料数量", false);
                decimal qty = 0;
                bool parseRes = decimal.TryParse(selectOne, out qty);
                if (!parseRes)
                {
                    return null;
                }

                if (dtItem == null || dtItem.Rows.Count != 1)
                {
                    return null;
                }

                list.Add(
                    new WorkTaskItemModel()
                    {
                        ItemID = dtItem.Rows[0]["T_ID"].ToString(),
                        ItemName = row["ITEMNAME"].ToString(),
                        PlanQty = qty,
                        ProcessedQty = 0,
                        FinishedQty = 0,
                        FinishedStatQty = 0,
                        ContainerDetailID = row["CONTAINER_D_ID"].ToString(),
                        ProcessStatus = "TaskStatus_Create"
                    });
            }
            return list;
        }


        #endregion

        #region 提交盘点到审核
        /// <summary>
        /// 提交盘点到审核
        /// </summary>
        /// <param name="countID">盘点主表ID</param>
        /// <returns></returns>
        public WebJsonInfo SubmitCountAudit(string countID)
        {
            try
            {
                string o_msg = "";
                string sql = "";
                string selectOne = "";
                int ResCount = 0;

                Dictionary<string, object> param = new Dictionary<string, object>();

                sql = "select count(*) from  &DataSource&WMS_D_COUNT_H where T_ID=@countID";
                param.Clear();
                param.Add("countID", countID);
                selectOne = DBHelper.SelectOne(sql, param, "查询盘点主数据是否存在");
                if (Convert.ToInt32(selectOne) < 1)
                {
                    throw new WmsException(500, "盘点主数据不存在");
                }

                sql = "update &DataSource&WMS_D_COUNT_H set COUNT_STATUS='CountDocStatus_UnAudit',END_DATE=now() where T_ID=@countID";
                param.Clear();
                param.Add("countID", countID);
                ResCount = DBHelper.RunSql(sql, param, "提交盘点结果");
                if (ResCount < 1)
                {
                    throw new WmsException(500, "提交盘点结果失败");
                }
                else
                {
                    sql = "select * from &DataSource&WMS_D_COUNT_D D where D.COUNT_H_ID=@countID ";
                    param.Clear();
                    param.Add("countID", countID);
                    DataTable DetailDT = DBHelper.Select(sql, param, "查询盘点明细");

                    if (DetailDT == null || DetailDT.Rows.Count < 1)
                    {
                        throw new WmsException(500, "盘点明细不存在");
                    }

                    foreach (DataRow row in DetailDT.Rows)
                    {

                        sql = "update &DataSource&WMS_D_COUNT_D set PROCESS_STATUS=@State,CONFIRMATION_DATE=now(),COMFIRMOR_ID=@UserID where T_ID=@T_ID";
                        param.Clear();
                        param.Add("State", "CountDocStatus_UnAudit");
                        param.Add("UserID", Context.UserInfo.UserName);
                        param.Add("T_ID", row["T_ID"].ToString());
                        ResCount = DBHelper.RunSql(sql, param, "审核明细处理");
                        if (ResCount < 1)
                        {
                            throw new WmsException(500, "审核明细处理失败");
                        }
                    }
                }
                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = null };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 通过或不通过审核
        /// <summary>
        /// 审核通过或不通过
        /// </summary>
        /// <param name="countHeadID">盘点明细ID</param>
        /// <param name="audit">审核是否通过，0不通过，1通过</param>
        /// <returns></returns>
        public WebJsonInfo PassCountAudit(string countHeadID, string audit)
        {
            try
            {
                string o_msg = "";
                string sql = "";
                string selectOne = "";
                int ResCount = 0;
                string countState = "";
                if (audit == "0")
                {
                    countState = "CountDocStatus_Confirmed";
                }
                else
                {
                    countState = "CountDocStatus_Executed";
                }

                Dictionary<string, object> param = new Dictionary<string, object>();

                sql = "select count(*) from  &DataSource&WMS_D_COUNT_H where T_ID=@countID";
                param.Clear();
                param.Add("countID", countHeadID);
                selectOne = DBHelper.SelectOne(sql, param, "查询盘点主数据是否存在");
                if (Convert.ToInt32(selectOne) < 1)
                {
                    throw new WmsException(500, "盘点单不存在");
                }

                sql = "select count(*) from  &DataSource&WMS_D_COUNT_H where T_ID=@countID and COUNT_STATUS='CountDocStatus_UnAudit'";
                param.Clear();
                param.Add("countID", countHeadID);
                selectOne = DBHelper.SelectOne(sql, param, "查询盘点主数据是否待审核");
                if (Convert.ToInt32(selectOne) < 1)
                {
                    throw new WmsException(500, "盘点单不是待审核");
                }

                sql = "update &DataSource&WMS_D_COUNT_H set COUNT_STATUS=@UState where T_ID=@countID";
                param.Clear();
                param.Add("UState", countState);
                param.Add("countID", countHeadID);
                ResCount = DBHelper.RunSql(sql, param, "审核处理成功");

                if (ResCount < 1)
                {
                    throw new WmsException(500, "审核处理失败");
                }


                sql = "select * from &DataSource&WMS_D_COUNT_D D where D.COUNT_H_ID=@countHeadID ";
                param.Clear();
                param.Add("countHeadID", countHeadID);
                DataTable DetailDT = DBHelper.Select(sql, param, "查询盘点明细");
                if (DetailDT == null || DetailDT.Rows.Count < 1)
                {
                    throw new WmsException(500, "盘点明细不存在");
                }

                foreach (DataRow row in DetailDT.Rows)
                {

                    sql = "update &DataSource&WMS_D_COUNT_D set PROCESS_STATUS=@State,CONFIRMATION_DATE=now(),COMFIRMOR_ID=@UserID where T_ID=@T_ID";
                    param.Clear();
                    param.Add("State", countState);
                    param.Add("UserID", Context.UserInfo.UserName);
                    param.Add("T_ID", row["T_ID"].ToString());
                    ResCount = DBHelper.RunSql(sql, param, "审核明细处理");
                    if (ResCount < 1)
                    {
                        throw new WmsException(500, "审核明细处理失败");
                    }

                    if (audit == "1") //成功调整库存
                    {
                        sql = "update &DataSource&WMS_B_CONTAINER_D set QTY=@QTY,MODIFY_DATE=now(),LAST_MODIFIED_BY=@UserID where T_ID=@CONTAINER_D_ID";
                        param.Clear();
                        param.Add("QTY", row["COUNT_QTY"].ToString());
                        param.Add("CONTAINER_D_ID", row["CONTAINER_D_ID"].ToString());
                        param.Add("UserID", Context.UserInfo.UserName);
                        ResCount = DBHelper.RunSql(sql, param, "审核处理成功");
                        if (ResCount < 1)
                        {
                            throw new Exception("库存调整失败");
                        }
                    }
                }

                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = null };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取盘点容器库存信息
        /// <summary>
        /// 获取盘点容器库存信息
        /// </summary>
        /// <param name="containerCode"></param>
        /// <returns></returns>
        public WebJsonInfo CountGetConInfo(string containerCode)
        {
            try
            {
                string o_msg = "";
                string sql = "";
                string selectOne = "";
                int ResCount = 0;

                Dictionary<string, object> param = new Dictionary<string, object>();
                sql = "select count(*) from  &DataSource&wms_b_task h where CONTAINER_BARCODE=@CONTAINER_BARCODE and TASK_TYPE='"+TaskType.TaskType_OutBound.ToString()+"' and REF_DOC_TYPE='DocType_Count'";
                param.Clear();
                param.Add("CONTAINER_BARCODE", containerCode);
                selectOne = DBHelper.SelectOne(sql, param, "查询盘点任务是否完成",false);
                if (Convert.ToInt32(selectOne) > 0)
                {
                    throw new WmsException(500, "盘点任务未完成");
                }


                sql = "select count(*) from  &DataSource&WMS_B_TASK_HIS h where CONTAINER_BARCODE=@CONTAINER_BARCODE ";
                param.Clear();
                param.Add("CONTAINER_BARCODE", containerCode);
                selectOne = DBHelper.SelectOne(sql, param, "查询盘点任务是否正常转移到任务历史");
                if (Convert.ToInt32(selectOne) < 1)
                {
                    throw new WmsException(500, "盘点任务未正常转移到任务历史");
                }

                sql = "select * from  &DataSource&WMS_B_TASK_HIS h where CONTAINER_BARCODE=@CONTAINER_BARCODE order by create_date desc";
                param.Clear();
                param.Add("CONTAINER_BARCODE", containerCode);
                DataTable countDT = DBHelper.Select(sql, param, "查询托盘最新的任务历史");
                if (countDT.Rows[0]["Task_Type"].ToString() == TaskType.TaskType_OutBound.ToString() && countDT.Rows[0]["REF_DOC_TYPE"].ToString() == "DocType_Count")
                {
                    string CountCode = countDT.Rows[0]["REF_DOC_NO"].ToString();
                    string CountHeadID = countDT.Rows[0]["REF_DOC_ID"].ToString();
                    sql = "select d.T_ID,d.ITEM_NAME,i.ITEM_CODE,d.QTY from &DataSource&wms_b_container_h h inner join &DataSource&wms_b_container_d d on h.t_id = d.container_id  inner join &DataSource&wms_m_item i on d.item_id=i.t_id where h.container_barcode=@CONTAINER_BARCODE and h.act_status='1'";
                    param.Clear();
                    param.Add("CONTAINER_BARCODE", containerCode);
                    DataTable dtDetail = DBHelper.Select(sql, param, "查询托盘明细");
                    IList<CountContainerDetail> detailList = new List<CountContainerDetail>();
                    foreach (DataRow r in dtDetail.Rows)
                    {
                        CountContainerDetail cell = new CountContainerDetail();
                        cell.DetailID = r["T_ID"].ToString();
                        cell.ItemCode = r["ITEM_CODE"].ToString();
                        cell.ItemName = r["ITEM_NAME"].ToString();
                        cell.Qty = r["QTY"].ToString();
                        cell.CountCode = CountCode;
                        cell.CountHeadID = CountHeadID;
                        detailList.Add(cell);
                    }
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "", json = detailList };
                }
                else
                {
                    throw new WmsException(500, "该托盘最后一次任务不是盘点任务");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #endregion



        #region 回收盘点任务
        /// <summary>
        /// 回收盘点任务
        /// </summary>
        /// <param name="taskRow">任务表</param>
        public void RecycleCountTask(DataRow taskRow)
        {
            if (taskRow == null)
            {
                throw new WmsException(500, "查询不到任务");
            }

            if (taskRow["task_type"].ToString() == TaskType.TaskType_InBound.ToString())
            {
                UpdateLocStatus(taskRow["dest_addr_id"].ToString(), LocationStatus.LocationStatus_Empty.ToString());//更新货位状态
            }
            else if (taskRow["task_type"].ToString() == TaskType.TaskType_OutBound.ToString())
            {
                UpdateLocStatus(taskRow["src_addr_id"].ToString(), LocationStatus.LocationStatus_Stored.ToString());//更新货位状态   
                UpdateCountStateByTaskId(taskRow);
            }

            _workTask.MoveToHistory(taskRow["t_id"].ToString());
        }

        /// <summary>
        /// 回收盘点任务时回滚下架单状态
        /// </summary>
        /// <param name="tId"></param>
        /// <param name="status"></param>
        private void UpdateCountStateByTaskId(DataRow taskRow)
        {
            //先更新盘点单明细状态
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam.Clear();
            dicParam.Add("process_status", CountState.CountDocStatus_NotExecute.ToString());
            dicParam.Add("COUNT_H_ID", taskRow["REF_DOC_ID"].ToString());
            dicParam.Add("BarCode", taskRow["container_barcode"].ToString());
            DBHelper.RunSql(@"update &DataSource&WMS_D_COUNT_D d set d.process_status=@process_status
                              where d.COUNT_H_ID =@COUNT_H_ID 
                              and d.act_status=1 and d.client_id='&CLIENT_ID&' 
                              and d.org_id='&ORG_ID&' ", dicParam, "更新盘点明细",
                              DBHelper.OperatedNumType.OnlyOne);

            //更新主单数据

            dicParam.Clear();
            dicParam.Add("COUNT_STATUS", CountState.CountDocStatus_NotExecute.ToString());
            dicParam.Add("T_ID", taskRow["REF_DOC_ID"].ToString());
            dicParam.Add("StateNew", CountState.CountDocStatus_NotExecute.ToString());
            DBHelper.RunSql(@"update &DataSource&WMS_D_COUNT_H h set h.COUNT_STATUS=@COUNT_STATUS,h.start_date = null 
                              where h.T_ID =@T_ID 
                              and not exists(select * from &DataSource&WMS_D_COUNT_D d where d.COUNT_H_ID =@T_ID and (d.process_status<>@StateNew or d.process_status is null)) and h.act_status=1 and h.client_id='&CLIENT_ID&' 
                              and h.org_id='&ORG_ID&' ", dicParam, "更新盘点主档",
                              DBHelper.OperatedNumType.ZeroOrMore);
        }
        #endregion
    }
}
