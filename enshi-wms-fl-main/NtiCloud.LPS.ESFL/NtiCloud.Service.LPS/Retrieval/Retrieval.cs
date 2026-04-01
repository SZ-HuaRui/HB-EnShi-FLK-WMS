using Microsoft.AspNetCore.Identity;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.LPS.Common.Interface;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Inventory;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Tasking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security;
using System.Text;
using System.Linq;
using static NtiCloud.Service.WMS.Common.DBHelper;

namespace NtiCloud.Service.WMS.Retrieval
{
    public class Retrieval : WmsObject, IRetrieval
    {
        private readonly IWorkTask _workTask;
        private readonly IWorkTaskItem _workTaskItem;
        private readonly IContainer _container;

        public Retrieval(IWorkTask workTask, IWorkTaskItem workTaskItem, IContainer container)
        {
            _workTask = workTask;
            _workTaskItem = workTaskItem;
            _container = container;
        }

        public Retrieval()
        {
            _workTask = new WorkTask();
            _workTaskItem = new WorkTaskItem();
            _container = new Container();
        }

        /// <summary>
        /// 判断密集库前面的货位是否为空
        /// </summary>
        /// <param name="locationID"></param>
        /// <returns></returns>
        public bool IsPevStoredCompact(List<string> locationIDList, string row,string location_no, string column)
        {
            int count = 0;
            string locationStr = "";
            foreach (string s in locationIDList)
            {
                locationStr = locationStr == "" ? ("'" + s + "'") : (locationStr + ",'" + s + "'");
            }
            //string sql = @"select count(*) c from &DataSource&wms_m_location l where l.T_ID not in (" + locationStr + ") and l.location_type='LocationType_Compact' and l.LOCATION_ROW =@rowsNum and l.LOCATION_COLUMN>=@colsNum and l.location_status<>'LocationStatus_Empty'";
            string sql = @"select count(*) c from &DataSource&wms_m_location l where l.T_ID not in (" + locationStr + ") and l.location_type='LocationType_Compact' and l.LOCATION_ROW =@rowsNum and l.LOCATION_COLUMN>=@colsNum and l.location_no>'"+ location_no + "' and l.location_status<>'LocationStatus_Empty'";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Clear();
            param.Add("rowsNum", row);
            param.Add("colsNum", column);
            string one = DBHelper.SelectOne(sql, param, "判断密集库前面的货位是否为空", false);
            if (Convert.ToInt32(one) > 0)
            {
                throw new WmsException(500, "需要下架的货位前面存在托盘或有非空货位");
            }
            return false;
        }

        #region 下架单提交下架
        /// <summary>
        /// 下架单提交下架
        /// </summary>
        /// <param name="RetrievalID">下架单ID</param>
        /// <returns></returns>
        public WebJsonInfo RetrievalDownCommit(string RetrievalID)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            string sql = @"select RETRIEVAL_CODE,RETRIEVAL_STATUS,DEST_ADDRID,zone_id,equipment_id from &DataSource&WMS_D_RETRIEVAL_H t where t.t_id=@RetrievalID";
            param.Clear();
            param.Add("RetrievalID", RetrievalID);
            DataTable dt = Select(sql, param, "查询指定ID的下架单", false);
            if (dt.Rows.Count < 1)
            {
                throw new WmsException(500, "不存在此下架单");
            }
            string exZone = dt.Rows[0]["zone_id"].ToString();
            string equipment_id = dt.Rows[0]["equipment_id"].ToString();
            if (string.IsNullOrEmpty(exZone))
            {
                throw new WmsException(500, "请在单据中，选择区域！");
            }
            if (string.IsNullOrEmpty(equipment_id))
            {
                throw new WmsException(500, "请在单据中，选择执行设备！");
            }
            //string DEST_ADDRID = dt.Rows[0]["DEST_ADDRID"] == null ? "" : dt.Rows[0]["DEST_ADDRID"].ToString();//下架单目的地址
            //sql = @" select t.equipment_name,t.equipment_code,t.zone_id  from &DataSource&wms_m_equipment  t where t.ACT_STATUS = '1' and t.t_ID=@DEST_ADDRID";
            //param.Clear();
            //param.Add("DEST_ADDRID", DEST_ADDRID);
            //DataTable srctemp = DBHelper.Select(sql, param, "查询下架单目标地址", false);
            //string zoneId;
            //if (srctemp == null || srctemp.Rows.Count < 1)
            //{
            //    throw new WmsException(500, "目标地址不存在:");
            //}
            //else
            //{
            //    zoneId = srctemp.Rows[0]["zone_id"].ToString();
            //}

            sql = $"select * from (select distinct t.location_id, t.location_code, t.CONTAINER_BARCODE, t.CONTAINER_ID, t.SEQ, l.zone_id,l.location_row from &DataSource&WMS_D_RETRIEVAL_D t left join &DataSource&wms_m_location l on t.location_id = l.t_id where t.ACT_STATUS = '1' and t.RETRIEVAL_H_ID = @RetrievalID) ";

            param.Clear();
            param.Add("RetrievalID", RetrievalID);
            DataTable dtRD = DBHelper.Select(sql, param, "查询指定ID的下架单明细", false);

            if (dtRD.Rows.Count < 1)
            {
                throw new WmsException(500, "下架单明细不存在");
            }
            DataView dataview = dtRD.DefaultView;
            List<string> locIdList = (dataview.ToTable(true, "location_id")).AsEnumerable().Select(o => o.Field<string>("location_id").ToString()).ToList();
            if (locIdList.Count!= dtRD.Rows.Count)
            {
                throw new WmsException(500, "下架单明细中存在相同下架货位，这些货位关联多个托盘信息！");
            }
            List<string> locZoneList = (dataview.ToTable(true, "zone_id")).AsEnumerable().Select(o => o.Field<string>("zone_id").ToString()).ToList();
            List<string> locLineList = (dataview.ToTable(true, "location_row")).AsEnumerable().Select(o => o.Field<decimal>("location_row").ToString()).ToList();
            if (locZoneList.Count != 1)
            {
                throw new WmsException(500, "下架单明细中不支持两个楼层的货位一起下架！");
            }

            string zoneId = locZoneList[0];
            if (!string.Equals(exZone, zoneId))
            {
                throw new WmsException(500, "下架单明细中下架的货位与执行设备所在区域不一致！");
            }

            string oneFloorZoneId = WmsConfig.GetOrDefault("Outbound:FirstFloorZone", "1291999756888969216");//一楼区域
            if (string.Equals(oneFloorZoneId, zoneId))//一楼
            {
                return CreateRetrievalDownTask(RetrievalID, equipment_id);
            }
            return twoFloorRDC(RetrievalID);
        }

        #region 二楼下架
        /// <summary>
        /// 二楼下架
        /// </summary>
        /// <param name="RetrievalID"></param>
        /// <returns></returns>
        private WebJsonInfo twoFloorRDC(string RetrievalID)
        {
            try
            {
                int rescount = 0;
                string sql = @"";
                string selectOne = "";
                string RETRIEVAL_CODE = "";  //下架单号
                string RETRIEVAL_STATUS = ""; //下架单状态
                string DEST_ADDRID = "";   //下架单地址
                WorkTask workTask = new WorkTask();
                Dictionary<string, object> param = new Dictionary<string, object>();
                DataTable dt = null;

                sql = @"select RETRIEVAL_CODE,RETRIEVAL_STATUS,DEST_ADDRID from &DataSource&WMS_D_RETRIEVAL_H t where t.t_id=@RetrievalID";
                param.Clear();
                param.Add("RetrievalID", RetrievalID);
                dt = DBHelper.Select(sql, param, "查询指定ID的下架单", false);
                if (dt.Rows.Count < 1)
                {
                    throw new WmsException(500, "不存在此下架单");
                }
                RETRIEVAL_CODE = dt.Rows[0]["RETRIEVAL_CODE"] == null ? "" : dt.Rows[0]["RETRIEVAL_CODE"].ToString();
                RETRIEVAL_STATUS = dt.Rows[0]["RETRIEVAL_STATUS"] == null ? "" : dt.Rows[0]["RETRIEVAL_STATUS"].ToString();
                DEST_ADDRID = dt.Rows[0]["DEST_ADDRID"] == null ? "" : dt.Rows[0]["DEST_ADDRID"].ToString();

                if (RETRIEVAL_STATUS != RetrievalState.RetOutboundStatus_Create.ToString())
                {
                    throw new WmsException(500, "下架单不是新建状态");
                }

                if (string.IsNullOrEmpty(DEST_ADDRID))
                {
                    throw new WmsException(500, "下架单未指定目标地址");
                }

                sql = @" select * from ( select distinct t.location_id,t.LOCATION_CODE,t.CONTAINER_BARCODE,t.CONTAINER_ID,t.SEQ,l.location_column from &DataSource&WMS_D_RETRIEVAL_D t left join 
                         &DataSource&wms_m_location l on t.location_id = l.t_id  
                         where t.ACT_STATUS = '1' and t.RETRIEVAL_H_ID=@RetrievalID) order by location_column desc";
                param.Clear();
                param.Add("RetrievalID", RetrievalID);
                dt = DBHelper.Select(sql, param, "查询指定ID的下架单明细", false);

                if (dt.Rows.Count < 1)
                {
                    throw new WmsException(500, "下架单明细不存在");
                }

                List<string> locationIDList = dt.AsEnumerable().Select(o => o.Field<string>("location_id")).ToList();

                foreach (DataRow r in dt.Rows)
                {
                    string location_id = r["location_id"] == null ? "" : r["location_id"].ToString();
                    if (string.IsNullOrEmpty(location_id))
                    {
                        throw new WmsException(500, "存在货位为空的库存");
                    }

                    string locationType = "";
                    sql = @" select * from &DataSource&wms_m_location l where l.t_id =@location_id ";
                    param.Clear();
                    param.Add("location_id", location_id);
                    DataTable curLocation = DBHelper.Select(sql, param, "查询指定货位的类型", false);
                    if (curLocation == null || curLocation.Rows.Count < 1)
                    {
                        throw new WmsException(500, "货位不存在");
                    }
                    locationType = curLocation.Rows[0]["location_type"].ToString();
                    if (locationType == LocationType.LocationType_Compact.ToString())
                    {
                        bool isPevStored = IsPevStoredCompact(locationIDList, curLocation.Rows[0]["LOCATION_ROW"].ToString(), curLocation.Rows[0]["LOCATION_no"].ToString(), curLocation.Rows[0]["LOCATION_COLUMN"].ToString());
                        if (isPevStored)
                        {
                            throw new WmsException(500, "前面货位存在托盘或者前面货位为非空货位");
                        }
                    }
                }
                List<string> tasklist = new List<string>();
                List<string> rowlist = new List<string>();
                foreach (DataRow r in dt.Rows)
                {
                    string location_id = r["location_id"] == null ? "" : r["location_id"].ToString();
                    string LOCATION_CODE = r["LOCATION_CODE"] == null ? "" : r["LOCATION_CODE"].ToString();
                    string CONTAINER_BARCODE = r["CONTAINER_BARCODE"] == null ? "" : r["CONTAINER_BARCODE"].ToString();
                    string CONTAINER_ID = r["CONTAINER_ID"] == null ? "" : r["CONTAINER_ID"].ToString();
                    decimal? SEQ = r["SEQ"] == null ? null : ((decimal?)Convert.ToDecimal(r["SEQ"].ToString()));
                    string taskID = SnowflakeID.Guid().ToString();
                    sql = @" select location_status,LOCATION_NAME,LOCATION_TYPE,LOCATION_CODE,location_row,ZONE_ID  from &DataSource&wms_m_location t where t.ACT_STATUS = '1' and t.t_ID=@location_id";
                    param.Clear();
                    param.Add("location_id", location_id);
                    DataTable temp = DBHelper.Select(sql, param, "查询下架货位状态是否为已存储", false);
                    if (temp == null || temp.Rows.Count < 1)
                    {
                        throw new WmsException(500, "货位不存在:" + LOCATION_CODE);
                    }
                    else
                    {
                        string Loc_Type = temp.Rows[0]["LOCATION_TYPE"] == null ? "" : temp.Rows[0]["LOCATION_TYPE"].ToString();
                        if (Loc_Type == LocationType.LocationType_Compact.ToString() || Loc_Type == LocationType.LocationType_3DRack.ToString())
                        {
                            if (temp.Rows[0]["location_status"].ToString() != "LocationStatus_Stored" && temp.Rows[0]["location_status"].ToString() != "LocationStatus_Full")
                            {
                                throw new WmsException(500, "货位状态不为已存储:" + LOCATION_CODE);
                            }
                            else
                            {
                                sql = @" update &DataSource&wms_m_location t set t.location_status = '" + LocationStatus.LocationStatus_Out.ToString() + "' where t.t_ID=@location_id";
                                param.Clear();
                                param.Add("location_id", location_id);
                                DBHelper.RunSql(sql, param, "更新货位状态为待出库");
                            }
                        }

                    }


                    string _locRow = temp.Rows[0]["location_row"] == null ? "" : temp.Rows[0]["location_row"].ToString();
                    if (!rowlist.Contains("'" + _locRow.Trim() + "'"))
                    {
                        rowlist.Add("'"+_locRow.Trim()+"'");
                    }
                    string srcAddType = "AddressType_Zone";// temp.Rows[0]["LOCATION_TYPE"].ToString();
                    string srcAddName = temp.Rows[0]["LOCATION_CODE"].ToString();
                    string zoneId = temp.Rows[0]["ZONE_ID"].ToString();
                    string DestAddType = "";
                    string DestAddName = "";
                    sql = @" select EQUIPMENT_NAME,EQUIPMENT_CODE  from &DataSource&wms_m_equipment  t where t.ACT_STATUS = '1' and t.t_ID=@DEST_ADDRID";
                    param.Clear();
                    param.Add("DEST_ADDRID", DEST_ADDRID);
                    DataTable srctemp = DBHelper.Select(sql, param, "查询下架单目标地址", false);
                    if (srctemp == null || srctemp.Rows.Count < 1)
                    {
                        throw new WmsException(500, "目标地址不存在:" + LOCATION_CODE);
                    }
                    else
                    {
                        //DestAddType = srctemp.Rows[0][].ToString();
                        DestAddName = srctemp.Rows[0]["EQUIPMENT_CODE"].ToString();
                    }


                    workTask.Insert(new Model.WorkTaskModel()
                    {
                        ID = taskID,
                        ZoneID = zoneId,                //区域ID
                        TaskNo = WmsHelper.GetSequence("TaskNo"),              //任务编号
                        //DependentTaskNo = "",     //依赖任务号
                        TaskType = TaskType.TaskType_OutBound.ToString(),//任务类型编码
                        ExecutedBy = Context.UserInfo.UserName,          //任务执行人
                        EquipmentID = Equipment.GetStackByLocation(location_id)?["t_id"].ToString(),         //执行设备ID
                        ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),       //执行设备类型
                        ActualEquipmentID = "",   //实际执行设备ID
                        SeqNo = SEQ,               //顺序号
                        StartDate = DateTime.Now,   //开始时间
                        //Priority = "",            //优先级
                        ContainerID = CONTAINER_ID,         //容器ID
                        ContainerBarcode = CONTAINER_BARCODE,    //容器条码
                        RefDocType = DocType.DocType_Retrieval.ToString(),          //来源单据类型
                        RefDocID = RetrievalID,            //来源单ID
                        RefDocDetailID = "",      //来源单明细ID
                        RefDocNo = RETRIEVAL_CODE,            //来源单号
                        SrcAddrID = location_id,           //来源地址ID
                        SrcAddrType = srcAddType,         //来源地址类型
                        SrcAddrText = srcAddName,         //来源地址
                        DestAddrID = DEST_ADDRID,          //目标地址ID
                        DestAddrType = DestAddType,        //目标地址类型
                        DestAddrText = DestAddName,        //目标地址
                        ActualDestAddrID = "",    //实际目标地址ID
                        ActualDestAddrText = "",  //实际目标地址
                        TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),// "TaskStatus_NotExecute",          //任务状态
                        //StartDate = "",           //开始时间
                        //EndDate = "",             //结束时间
                        IsExecutable = "1",        //任务是否可执行
                        //PriorityChangeTime = "",  //优先级修改时间
                        LedText = "",             //LED显示内容
                        ExistLgv = "",            //是否存在LGV
                        CreateDate = DateTime.Now          //创建时间
                        //ModifyDate = ""           //编辑时间
                    });


                    sql = @"select * from &DataSource&wms_b_container_d t where t.container_id=@CONTAINER_ID";
                    param.Clear();
                    param.Add("CONTAINER_ID", CONTAINER_ID);
                    DataTable itemDt = DBHelper.Select(sql, param, "查询指定托盘ID的库存信息", false);
                    if (itemDt == null || itemDt.Rows.Count < 1)
                    {
                        continue;
                    }

                    foreach (DataRow itemR in itemDt.Rows)
                    {
                        workTask.AddTaskItem(new TaskItemModel()
                        {
                            t_id = SnowflakeID.Guid().ToString(),
                            client_id = itemR["client_id"] == null ? "" : itemR["client_id"].ToString(),
                            org_id = itemR["org_id"] == null ? "" : itemR["org_id"].ToString(),
                            warehouse_id = itemR["warehouse_id"] == null ? "" : itemR["warehouse_id"].ToString(),
                            task_id = taskID,
                            item_id = itemR["item_id"] == null ? "" : itemR["item_id"].ToString(),
                            item_name = itemR["item_name"] == null ? "" : itemR["item_name"].ToString(),
                            plan_qty = (itemR["qty"] == null || itemR["qty"].ToString() == "") ? 0 : Convert.ToDecimal(itemR["qty"]),
                            uom_qty_id = itemR["uom_qty_id"] == null ? "" : itemR["uom_qty_id"].ToString(),
                            uom_stat_id = itemR["uom_stat_id"] == null ? "" : itemR["uom_stat_id"].ToString(),
                            container_d_id = CONTAINER_ID,
                            ref_doc_type = DocType.DocType_Retrieval.ToString(),
                            ref_doc_id = RetrievalID,
                            ref_doc_no = RETRIEVAL_CODE,
                            ref_doc_d_id = "",
                            process_status = ProcessStatus.ProcessStatus_Doing.ToString(),
                            created_by = Context.UserInfo.UserName,
                            create_date = DateTime.Now,
                            modify_date = DateTime.Now,
                            act_status = "1",
                        });
                    }

                    sql = @" update &DataSource&WMS_D_RETRIEVAL_H t set t.RETRIEVAL_STATUS='" + RetrievalState.RetOutboundStatus_Execting.ToString() + "' where t.ACT_STATUS = '1' and t.t_ID=@RetrievalID";
                    param.Clear();
                    param.Add("RetrievalID", RetrievalID);
                    rescount = DBHelper.RunSql(sql, param, "更新下架单状态为下架中", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount > 0)
                    {
                        //bool isLocChangeStateOK = OutChangeState(location_id);
                        //if (isLocChangeStateOK) //修改货位状态成功
                        //{

                        //}
                        sql = @"update &DataSource&WMS_D_RETRIEVAL_D t set t.RETRIEVAL_STATUS = '" + RetrievalState.RetOutboundStatus_Execting.ToString() + "' where  t.RETRIEVAL_H_ID=@RetrievalID and t.LOCATION_ID = @LOCATION_ID";
                        param.Clear();
                        param.Add("RetrievalID", RetrievalID);
                        param.Add("LOCATION_ID", location_id);
                        DBHelper.RunSql(sql, param, "更新下架单明细状态为下架中", DBHelper.OperatedNumType.ZeroOrMore);
                    }


                    tasklist.Add(taskID);

                }
                if (tasklist.Count > 0)
                {
                    if (rowlist.Count > 0)
                    {
                        sql = @"update &DataSource&wms_m_location t set t.location_flag = @location_flag where  t.location_row in(" + string.Join(",", rowlist.ToArray()) + ")";
                        param.Clear();
                        param.Add("location_flag", LocationFlag.LocationFlag_OutOnly.ToString());
                        DBHelper.RunSql(sql, param, "更新下架货位所在行的货位标识为可出不可入", DBHelper.OperatedNumType.ZeroOrMore);
                    }
                    WorkTaskInterface.Send(tasklist);
                }

                //更改任务执行状态
                //foreach(string id in idList)
                //{
                //    workTask.ForceTaskExecuting(id);
                //}
                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = "" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #endregion
        /*
        #region 修改货位状态为待出库
        /// <summary>
        /// 修改货位状态为待出库
        /// </summary>
        /// <param name="locationID">库位ID</param>
        /// <returns></returns>
        private bool OutChangeState(string locationID)
        {
            try
            {
                int rescount = 0;
                string sql = @"";
                Dictionary<string, object> param = new Dictionary<string, object>();
                sql = @"update &DataSource&wms_m_location l set l.location_status = 'LocationStatus_Out' where l.t_id=@LocationID ";
                param.Clear();
                param.Add("LocationID", locationID);
                rescount = DBHelper.RunSql(sql, param, "更新货位状态为空", DBHelper.OperatedNumType.ZeroOrMore);
                if (rescount > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        */

        #region 下架任务完成
        /// <summary>
        /// 下架任务完成
        /// </summary>
        /// <param name="taskHID">下架任务ID</param>
        /// <returns></returns>
        public WebJsonInfo RetrievalDownTaskOver(string taskHID)
        {
            try
            {
                int rescount = 0;
                string sql = @"";
                string selectOne = "";
                string RETRIEVAL_CODE = "";  //下架单号
                string RETRIEVAL_ID = "";  //下架单ID
                string ContainerID = "";   //容器ID
                string LocationID = "";  //下架货位ID
                string zoneIdInTask = "";  //区域ID
                WorkTask workTask = new WorkTask();
                Dictionary<string, object> param = new Dictionary<string, object>();
                DataTable dt = null;

                sql = @"select count(1) from &DataSource&wms_b_task t where t.T_ID=@t_id ";
                param.Clear();
                param.Add("t_id", taskHID);
                selectOne = DBHelper.SelectOne(sql, param, "查询下架任务是否存在", false);
                if (Convert.ToInt32(selectOne) < 1)
                {
                    throw new WmsException(500, "任务不存在");
                }


                sql = @"select zone_id,Container_ID,SRC_ADDR_ID,task_status from &DataSource&wms_b_task t where t.t_id=@t_id";
                param.Clear();
                param.Add("t_id", taskHID);
                DataTable taskDT = DBHelper.Select(sql, param, "查询下架任务主档信息", false);
                if (taskDT == null || taskDT.Rows.Count < 1)
                {
                    throw new WmsException(500, "任务主档不存在");
                }
                ContainerID = taskDT.Rows[0]["Container_ID"] == null ? "" : taskDT.Rows[0]["Container_ID"].ToString();
                LocationID = taskDT.Rows[0]["SRC_ADDR_ID"] == null ? "" : taskDT.Rows[0]["SRC_ADDR_ID"].ToString();
                zoneIdInTask = taskDT.Rows[0]["zone_id"] == null ? "" : taskDT.Rows[0]["zone_id"].ToString();
                string task_status = taskDT.Rows[0]["task_status"].ToString();
                sql = "select REF_DOC_NO from &DataSource&wms_b_task t where t.T_ID=@t_id";
                param.Clear();
                param.Add("t_id", taskHID);
                RETRIEVAL_CODE = DBHelper.SelectOne(sql, param, "查询下架任务对应的下架单号", false);
                if (string.IsNullOrEmpty(RETRIEVAL_CODE))
                {
                    throw new WmsException(500, "查询下架任务对应的下架单号不存在");
                }

                if (task_status!=TaskStatus.TaskStatus_PartialFinished.ToString())
                {

                    #region 这部分内容与部分完成功能一样(WorkTask.cs文件中的方法PartionCompleteTask部分内容一样)
                    //修改货位状态为空（此功能在部分完成任务中也做了，但是为了防止部分完成没有被执行，所以在此处也做一次）
                    sql = $"update  &DataSource&wms_m_location l set l.location_status='{LocationStatus.LocationStatus_Empty}' where l.t_ID=@LocationID";
                    param.Clear();
                    param.Add("LocationID", LocationID);
                    rescount = DBHelper.RunSql(sql, param, "修改货位状态为空", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount < 1)
                    {
                        throw new WmsException(500, "下架任务完成时修改货位状态为空，失败");
                    }
                    //清空库存
                    sql = "update  &DataSource&wms_b_container_h h set h.location_id = '',h.ACT_STATUS=0 where h.t_ID=@ContainerID";
                    param.Clear();
                    param.Add("ContainerID", ContainerID);
                    rescount = DBHelper.RunSql(sql, param, "更新容器主档数据为删除", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount < 1)
                    {
                        throw new WmsException(500, "更新容器主档数据为删除时失败");
                    }

                    sql = "update  &DataSource&wms_b_container_d d set d.ACT_STATUS=0 where d.CONTAINER_ID=@ContainerID";
                    param.Clear();
                    param.Add("ContainerID", ContainerID);
                    rescount = DBHelper.RunSql(sql, param, "更新容器明细数据为删除", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount < 1)
                    {
                        throw new WmsException(500, "更新容器明细数据为删除时失败");
                    }
                    if (string.Equals(zoneIdInTask, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))//恩施辅料库，二楼下架任务
                    {
                        Location lo = new Location();
                        LocationModel _locInfo = lo.GetLocationById(LocationID);
                        bool isLineEmpty = IsLineEmpty(_locInfo.location_row.ToString());//判断行，是否已空
                        Model.UserLogModel _logModel = new Model.UserLogModel();
                        _logModel.ModifyIP = Context.UserInfo.LoginIP;
                        _logModel.WarehouseID = Context.WarehouseID;
                        _logModel.TypeCode = "二楼下架任务部分完成";
                        _logModel.LogTextLong = $"货位编码{_locInfo.location_no},行号：" + _locInfo.location_row.ToString() + "  结果为" + isLineEmpty;
                        _logModel.CommonName = $"查询指定行是否已空,任务id：{taskHID}，任务的状态为：" + task_status;
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
                    }
                    #endregion
                }


                sql = @"update &DataSource&wms_b_task t set t.TASK_STATUS='TaskStatus_Finished',end_date=to_date(@enddate,'yyyy-MM-dd hh24:mi:ss') where t.T_ID=@t_id";
                param.Clear();
                param.Add("t_id", taskHID);
                param.Add("enddate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                rescount = DBHelper.RunSql(sql, param, "更新下架主档任务为完成", DBHelper.OperatedNumType.ZeroOrMore);
                if (rescount < 1)
                {
                    throw new WmsException(500, "更新下架主档任务为完成失败");
                }

                sql = @"update &DataSource&wms_b_task_item t set t.PROCESS_STATUS='ProcessStatus_Done' where t.TASK_ID=@t_id";
                param.Clear();
                param.Add("t_id", taskHID);
                rescount = DBHelper.RunSql(sql, param, "更新下架明细任务为完成", DBHelper.OperatedNumType.ZeroOrMore);
                if (rescount < 1)
                {
                    throw new WmsException(500, "更新下架明细任务为完成时失败");
                }



                sql = "select count(*) from &DataSource&WMS_D_RETRIEVAL_H t where t.RETRIEVAL_CODE=@RETRIEVAL_CODE";
                param.Clear();
                param.Add("RETRIEVAL_CODE", RETRIEVAL_CODE);
                selectOne = DBHelper.SelectOne(sql, param, "查询下架单", false);
                if (Convert.ToInt32(selectOne) != 1)
                {
                    throw new WmsException(500, "查询下架单不存在");
                }

                sql = "select t_id from &DataSource&WMS_D_RETRIEVAL_H t where t.RETRIEVAL_CODE=@RETRIEVAL_CODE";
                param.Clear();
                param.Add("RETRIEVAL_CODE", RETRIEVAL_CODE);
                RETRIEVAL_ID = DBHelper.SelectOne(sql, param, "查询下架单", false);


                
                sql = "update &DataSource&WMS_D_RETRIEVAL_D d set d.RETRIEVAL_STATUS='" + RetrievalState.RetOutboundStatus_Over.ToString() + "' where d.CONTAINER_ID=@CONTAINER_ID and d.RETRIEVAL_H_ID=@RETRIEVAL_ID";
                param.Clear();
                param.Add("CONTAINER_ID", ContainerID);
                param.Add("RETRIEVAL_ID", RETRIEVAL_ID);
                rescount = DBHelper.RunSql(sql, param, "更新下架单明细数据为已完成", DBHelper.OperatedNumType.ZeroOrMore);
                if (rescount < 1)
                {
                    throw new WmsException(500, "更新下架单明细数据为已完成时失败");
                }

                //判断后续单据状态

                sql = "select count(*) from &DataSource&WMS_D_RETRIEVAL_D d where d.RETRIEVAL_STATUS<>'" + RetrievalState.RetOutboundStatus_Over.ToString() + "' and d.RETRIEVAL_H_ID=@RETRIEVAL_ID";
                param.Clear();
                param.Add("RETRIEVAL_ID", RETRIEVAL_ID);
                selectOne = DBHelper.SelectOne(sql, param, "查询下架任务对应的下架单号", false);
                if (Convert.ToInt32(selectOne) == 0) //所有下架明细都已完成
                {
                    sql = "update &DataSource&WMS_D_RETRIEVAL_H h set h.RETRIEVAL_STATUS='" + RetrievalState.RetOutboundStatus_Over.ToString() + "' where h.t_id=@RETRIEVAL_ID";
                    param.Clear();
                    param.Add("RETRIEVAL_ID", RETRIEVAL_ID);
                    rescount = DBHelper.RunSql(sql, param, "更新下架单主档数据为已完成");
                    if (rescount < 1)
                    {
                        throw new WmsException(500, "更新下架单主档数据为已完成时失败");
                    }
                }

                _workTask.MoveToHistory(taskHID);

                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = "" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 删除下架单(明细或主档)
        /// <summary>
        /// 删除下架单(明细或主档)
        /// </summary>
        /// <param name="model">传入model的下架单ID-RetrievalID或下架单明细ID-DetailID</param>
        /// <returns></returns>
        public WebJsonInfo DelRetrievalDetail(string retrievalID, string detailID)
        {
            try
            {
                int rescount = 0;
                string sql = @"";
                string RETRIEVAL_CODE = "";  //下架单号
                string RETRIEVAL_ID = "";  //下架单ID
                string ContainerID = "";   //容器ID
                string LocationID = "";  //下架货位ID
                WorkTask workTask = new WorkTask();
                Dictionary<string, object> param = new Dictionary<string, object>();
                DataTable dt = null;





                if (string.IsNullOrEmpty(retrievalID)) //删除明细
                {
                    sql = @"select t.retrieval_status from &DataSource&wms_d_retrieval_h t inner join &DataSource&wms_d_retrieval_d d on t.t_id = d.RETRIEVAL_H_ID where d.T_ID=@t_id ";
                    param.Clear();
                    param.Add("t_id", detailID);
                    DataTable dateT = DBHelper.Select(sql, param, "查询下架明细否存在", false);
                    if (dateT.Rows.Count < 1)
                    {
                        throw new WmsException(500, "下架单明细不存在");
                    }

                    if (dateT.Rows[0]["retrieval_status"].ToString() != "RetOutboundStatus_Create")
                    {
                        throw new WmsException(500, "下架单不是新建状态");
                    }

                    //UpdateLocationForDel(retrievalID, detailID);

                    sql = @"delete &DataSource&wms_d_retrieval_d t where t.T_ID=@t_id ";
                    param.Clear();
                    param.Add("t_id", detailID);
                    rescount = DBHelper.RunSql(sql, param, "删除下架明细", DBHelper.OperatedNumType.ZeroOrMore);
                }
                else //删除主档
                {
                    sql = @"select * from &DataSource&wms_d_retrieval_h t where t.T_ID=@t_id ";
                    param.Clear();
                    param.Add("t_id", retrievalID);
                    DataTable dateT = DBHelper.Select(sql, param, "查询下架单是否存在", false);
                    if (dateT.Rows.Count < 1)
                    {
                        throw new WmsException(500, "下架单不存在");
                    }

                    if (dateT.Rows[0]["retrieval_status"].ToString() != "RetOutboundStatus_Create")
                    {
                        throw new WmsException(500, "下架单不是新建状态");
                    }

                    //UpdateLocationForDel(retrievalID, detailID);

                    sql = @"delete &DataSource&wms_d_retrieval_d d where d.RETRIEVAL_H_ID=@RETRIEVAL_H_ID ";
                    param.Clear();
                    param.Add("RETRIEVAL_H_ID", retrievalID);
                    rescount = DBHelper.RunSql(sql, param, "删除下架明细", DBHelper.OperatedNumType.ZeroOrMore);

                    sql = @"delete &DataSource&wms_d_retrieval_h t where t.T_ID=@t_id ";
                    param.Clear();
                    param.Add("t_id", retrievalID);
                    rescount = DBHelper.RunSql(sql, param, "删除下架主档", DBHelper.OperatedNumType.OnlyOne);
                }


                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = "" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
        /// <summary>
        /// 删除前，先更新下架明细对应的库位状态为已存储
        /// </summary>
        /// <param name="retrievalID"></param>
        /// <param name="detailID"></param>
        private void UpdateLocationForDel(string retrievalID, string detailID)
        {
            string sql;
            Dictionary<string, object> param = new Dictionary<string, object>();
            int rescount = 0;
            DataTable detail = null;
            if (string.IsNullOrEmpty(retrievalID))
            {
                sql = @"select * from &DataSource&wms_d_retrieval_d t where t.T_ID=@t_id ";
                param.Clear();
                param.Add("t_id", detailID);
                detail = DBHelper.Select(sql, param, "删除前通过明细查询下架明细", false);
            }
            else
            {
                sql = @"select * from &DataSource&wms_d_retrieval_d t where t.RETRIEVAL_H_ID=@RETRIEVAL_H_ID ";
                param.Clear();
                param.Add("RETRIEVAL_H_ID", retrievalID);
                detail = DBHelper.Select(sql, param, "删除前通过主档查询下架明细", false);
            }

            foreach (DataRow r in detail.Rows)
            {
                sql = @"update &DataSource&wms_m_location l set l.location_status=@OutState where l.t_id=@t_id";
                param.Clear();
                param.Add("t_id", r["location_id"].ToString());
                param.Add("OutState", LocationStatus.LocationStatus_Stored.ToString());
                rescount = DBHelper.RunSql(sql, param,"更新货位状态",DBHelper.OperatedNumType.ZeroOrMore);
                if (rescount < 1)
                {
                    throw new WmsException(500, "更新货位状态失败");
                }
            }
        }
        
        */
        #endregion

        #region 添加下架明细
        /// <summary>
        /// 添加下架明细
        /// </summary>
        /// <param name="retID">下架单ID</param>
        /// <param name="containerIDList">选择的主容器ID列表，别怀疑，就是主容器ID</param>
        /// <returns></returns>
        public WebJsonInfo RetrievalSaveDetail(string retID, IList<string> containerIDList)
        {
            try
            {
                int rescount = 0;
                string sql = @"";
                string RETRIEVAL_CODE = "";  //下架单号
                string RETRIEVAL_ID = "";  //下架单ID
                string ContainerID = "";   //容器ID
                string LocationID = "";  //下架货位ID
                WorkTask workTask = new WorkTask();
                Dictionary<string, object> param = new Dictionary<string, object>();
                DataTable dt = null;


                sql = @"select * from &DataSource&wms_d_retrieval_h t where t.T_ID=@t_id ";
                param.Clear();
                param.Add("t_id", retID);
                DataTable dateT = DBHelper.Select(sql, param, "查询下架单是否存在", false);
                if (dateT.Rows.Count < 1)
                {
                    throw new WmsException(500, "下架单不存在");
                }

                if (dateT.Rows[0]["retrieval_status"].ToString() != "RetOutboundStatus_Create")
                {
                    throw new WmsException(500, "下架单不是新建状态");
                }

                sql = @"select * from &DataSource&wms_d_retrieval_h h where h.t_id=@retID ";
                param.Clear();
                param.Add("retID", retID);
                DataTable dtHead = DBHelper.Select(sql, param, "查询下架单");

                //恩施辅料库因是密集库，涉及到任务的回收，但前面的货位任务回收会影响后续货位的下架，所以暂时只允许添加一个托盘

                //if (containerIDList.Count > 1)
                //{
                //    string locationStr = "";
                //    foreach(string loc in containerIDList)
                //    {
                //        locationStr = (locationStr == "") ? ("'"+loc+"'") : ("'"+loc + "'," + locationStr);
                //    }

                //    sql = @"select l.location_type from &DataSource&wms_m_location l left join  &DataSource&wms_b_container_h h on l.t_id = h.location_id where l.location_Type='"+LocationType.LocationType_Compact.ToString()+"' and h.t_id in ("+ locationStr + ")";
                //    param.Clear();
                //    DataTable conTb = DBHelper.Select(sql, param, "查询货位类型",false);
                //    if(conTb.Rows.Count>1)
                //    {
                //        throw new WmsException(500, "密集库只允许添加一个下架托盘");
                //    }
                //}
                //恩施end
                foreach (string containerID in containerIDList)
                {

                    sql = @"select * from &DataSource&containerdetail_ret_v v where v.t_id=@t_id";
                    param.Clear();
                    param.Add("t_id", containerID);
                    DataTable dtContainer = DBHelper.Select(sql, param, "查询下架库存");

                    sql = @"select h.* from &DataSource&wms_d_retrieval_d d inner join &DataSource&wms_d_retrieval_h h on h.t_id = d.RETRIEVAL_H_ID  where d.CONTAINER_ID=@t_id and d.RETRIEVAL_STATUS<>@Status1 and d.RETRIEVAL_STATUS<>@Status2 and h.ACT_STATUS='1'";
                    param.Clear();
                    param.Add("t_id", containerID);
                    param.Add("Status1", RetrievalState.RetOutboundStatus_Over.ToString());
                    param.Add("Status2", RetrievalState.RetOutboundStatus_Cancel.ToString());
                    DataTable dtTemp = DBHelper.Select(sql, param, "查询添加的下架明细是否已存在于其它单据", false);
                    if (dtTemp.Rows.Count > 0)
                    {
                        string retCode = dtTemp.Rows[0]["RETRIEVAL_CODE"].ToString();
                        throw new WmsException(500, "容器" + dtContainer.Rows[0]["container_barcode"].ToString() + "已存在于下架单:" + retCode);
                    }

                    sql = @"select item_code from &DataSource&wms_b_container_d d inner join &DataSource&wms_m_item i on d.item_id = i.t_id where d.container_id = @containerID";
                    param.Clear();
                    param.Add("containerID", containerID);
                    DataTable itemTB = DBHelper.Select(sql, param, "查询添加的下架明细的物料编码", false);

                    string itemcodestr = "";
                    foreach (DataRow r in itemTB.Rows)
                    {
                        string cell = r["item_code"].ToString();
                        if (string.IsNullOrEmpty(cell))
                        {
                            continue;
                        }
                        itemcodestr = itemcodestr == "" ? ("'" + cell + "'") : (itemcodestr + ",'" + cell + "'");
                    }

                    int seq = GetSEQ(retID);
                    int groupNum = GetGroupNum(retID, itemcodestr);

                    string newid = SnowflakeID.Guid().ToString();
                    sql = @"insert into &DataSource&wms_d_retrieval_d (t_id, 
                                 client_id, 
                                 org_id, 
                                 warehouse_id, 
                                 retrieval_h_id, 
                                 retrieval_code, 
                                 location_id, 
                                 location_code, 
                                 container_id, 
                                 container_barcode, 
                                 retrieval_status, 
                                 create_date, 
                                 created_by, 
                                 modify_ip, 
                                 modify_date, 
                                 last_modified_by, 
                                 SEQ,
                                 GROUP_NUM,
                                 act_status)
                                 values
                                 (
                                 @t_id,
                                 '&CLIENT_ID&',
                                 '&ORG_ID&',
                                 '&WAREHOUSE_ID&',
                                 @retrieval_h_id,
                                 @retrieval_code,
                                 @location_id,
                                 @location_code,
                                 @container_id,
                                 @container_barcode,
                                 @retrieval_status,
                                 sysdate,
                                 @created_by,
                                 '',
                                 sysdate,
                                 @last_modified_by,
                                 @seq,
                                 @groupNum,
                                 @act_status
                                 )  ";

                    param.Clear();
                    param.Add("t_id", newid);
                    param.Add("retrieval_h_id", retID);
                    param.Add("retrieval_code", dtHead.Rows[0]["RETRIEVAL_CODE"].ToString());
                    param.Add("location_id", dtContainer.Rows[0]["location_id"].ToString());
                    param.Add("location_code", dtContainer.Rows[0]["LOCATION_NUMBER"].ToString());
                    param.Add("container_id", dtContainer.Rows[0]["t_id"].ToString());
                    param.Add("container_barcode", dtContainer.Rows[0]["container_barcode"].ToString());
                    param.Add("retrieval_status", RetrievalState.RetOutboundStatus_Create.ToString());
                    param.Add("created_by", Context.GetUserInfo().UserName);
                    param.Add("last_modified_by", Context.GetUserInfo().UserName);
                    param.Add("seq", seq);
                    param.Add("groupNum", groupNum);
                    param.Add("act_status", "1");
                    rescount = DBHelper.RunSql(sql, param, "插入下架明细", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount < 1)
                    {
                        throw new WmsException(500, "添加容器时失败:" + dtContainer.Rows[0]["container_barcode"].ToString());
                    }

                }

                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = "" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 合肥烟厂因波次经常分配不到货位，经常用下架作为出库方式，该方法只为产生下架任务的序号做准备，以便托盘按顺序出库
        /// </summary>
        /// <param name="retrievalID"></param>
        /// <returns></returns>
        private int GetSEQ(string retrievalID)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();

            string sql = @"select max(SEQ) from &DataSource&wms_d_retrieval_d d where d.RETRIEVAL_H_ID=@t_id ";
            param.Clear();
            param.Add("t_id", retrievalID);
            string max = DBHelper.SelectOne(sql, param, "查询下架单序号", false);
            if (string.IsNullOrEmpty(max))
            {
                return 1;
            }
            int maxNum = Convert.ToInt32(max) + 1;
            return maxNum;
        }

        /// <summary>
        /// 合肥烟厂因波次经常分配不到货位，经常用下架作为出库方式，该方法只为产生下架任务的序号做准备，以便托盘按顺序出库
        /// </summary>
        /// <param name="retrievalID"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        private int GetGroupNum(string retrievalID, string itemCode)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();

            string sql = @"select d.group_num from &DataSource&wms_d_retrieval_d d inner join  
                           &DataSource&wms_b_container_d c on d.container_id  = c.container_id 
                           inner join &DataSource&wms_m_item i on i.t_id = c.item_id
                           where i.item_code in (" + itemCode + @")and c.act_status=1 and d.retrieval_h_id=@retrievalID
                           group by d.group_num ";
            param.Clear();
            param.Add("retrievalID", retrievalID);

            string groupNum = DBHelper.SelectOne(sql, param, "查询下架单组编号", false);
            if (!string.IsNullOrEmpty(groupNum))
            {
                return Convert.ToInt32(groupNum);
            }
            else
            {
                sql = @"select max(d.group_num) group_num  from &DataSource&wms_d_retrieval_d d inner join  
                           &DataSource&wms_b_container_d c on d.container_id  = c.container_id 
                           inner join &DataSource&wms_m_item i on i.t_id = c.item_id
                           where c.act_status=1 and d.retrieval_h_id=@retrievalID ";
                param.Clear();
                param.Add("retrievalID", retrievalID);
                groupNum = DBHelper.SelectOne(sql, param, "查询下架单组编号", false);
                if (!string.IsNullOrEmpty(groupNum))
                {
                    return Convert.ToInt32(groupNum) + 1;
                }
                else
                {
                    return 1;
                }
            }
        }
        #endregion

        #region 回收任务

        /// <summary>
        /// 回收下架任务
        /// </summary>
        /// <param name="taskId"></param>
        public void RecyclePetrivalTask(DataRow taskRow)
        {
            if (taskRow == null)
            {
                throw new WmsException(500, "查询不到任务");
            }

            if (taskRow["task_type"].ToString() == TaskType.TaskType_InBound.ToString())
            {
                UpdateLocStatus(taskRow["dest_addr_id"].ToString(), LocationStatus.LocationStatus_Empty.ToString());//更新货位状态
                _workTask.MoveToHistory(taskRow["t_id"].ToString());
            }
            else if (taskRow["task_type"].ToString() == TaskType.TaskType_OutBound.ToString())
            {
                UpdateLocStatus(taskRow["src_addr_id"].ToString(), LocationStatus.LocationStatus_Stored.ToString());//更新货位状态   
                UpdateRetrievalStateByTaskId(taskRow);
                UpdateTaskStatus(taskRow["t_id"].ToString());
                _workTask.MoveToHistory(taskRow["t_id"].ToString());

                //对于密集库，取消当前任务，则需把当前任务起始地址之前列的出库任务全部取消,约定同时只能有一个出库单(恩施辅料库)
                string srcaddid = taskRow["src_addr_id"].ToString();
                string equipmentId = taskRow["equipment_id"].ToString();
                string e1 = WmsConfig.GetOrDefault("Task:SC01", "1288091969666551808");  //1号堆垛机id
                string e2 = WmsConfig.GetOrDefault("Task:SC02", "1288092061295316992");  //2号堆垛机id
                bool isDesc;
                if (string.Equals(equipmentId, e1))
                {
                    isDesc = false;
                }
                else
                {
                    isDesc = true;
                }
                string sql = "select * from &DataSource&wms_m_location l where l.t_id = @t_id";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("t_id", srcaddid);
                DataTable locDT = DBHelper.Select(sql, param, "查询货位是否为密集库", false);
                if (locDT.Rows.Count > 0)
                {
                    string location_type = locDT.Rows[0]["location_type"].ToString();
                    if (location_type == LocationType.LocationType_Compact.ToString()) //是密集库
                    {
                        string row = locDT.Rows[0]["location_row"].ToString();
                        string col = locDT.Rows[0]["location_column"].ToString();
                        string tempLocationNo = locDT.Rows[0]["location_no"].ToString();
                        string ref_doc_id = taskRow["ref_doc_id"].ToString();

                        //sql = @"select distinct t.* from &DataSource&wms_m_location l inner join &DataSource&wms_b_task t
                        //        on l.t_id = t.src_addr_id where l.location_type = @location_type and l.location_row = @myrow 
                        //        and t.task_type=@task_type and t.task_status in ('TaskStatus_NotExecute','TaskStatus_Executing') and l.location_column"+(isDesc==true?"<":">")+@"@col and l.act_status='1' and t.act_status='1' 
                        //        and l.warehouse_id=@warehouse_id and l.client_id='&CLIENT_ID&' 
                        //        and l.org_id='&ORG_ID&'";
                        sql = $"SELECT DISTINCT t.* FROM &DataSource&wms_m_location l INNER JOIN &DataSource&wms_b_task t ON l.t_id = t.src_addr_id INNER JOIN &DataSource&wms_d_retrieval_d d ON d.location_id = l.t_id AND d.act_status = '1' INNER JOIN &DataSource&wms_d_retrieval_h h ON d.retrieval_h_id = h.t_id AND h.act_status = '1' WHERE l.location_type = @location_type AND l.location_row = @myrow AND t.ref_doc_type = @ref_doc_type AND t.task_status IN ('TaskStatus_NotExecute', 'TaskStatus_Executing') AND l.location_no {(isDesc == true ? "<" : ">")} @tempLocationNo AND l.act_status = '1' AND t.act_status = '1' AND t.ref_doc_id = @ref_doc_id";
                        param.Clear();
                        param.Add("location_type", LocationType.LocationType_Compact.ToString());
                        param.Add("myrow", row);
                        param.Add("tempLocationNo", tempLocationNo);
                        param.Add("ref_doc_id", ref_doc_id);
                        param.Add("ref_doc_type", DocType.DocType_Retrieval.ToString());
                        param.Add("warehouse_id", Context.WarehouseID);

                        DataTable taskDt = DBHelper.Select(sql, param, "查询密集库前面的出库任务", false);
                        if (taskDt.Rows.Count > 0)
                        {
                            foreach (DataRow r in taskDt.Rows)
                            {
                                UpdateLocStatus(r["src_addr_id"].ToString(), LocationStatus.LocationStatus_Stored.ToString());//更新货位状态
                                                                                                                              //
                                //sql = @"update &DataSource&wms_m_location t set t.location_flag = @location_flag where  t.location_row =@myrow";
                                //param.Clear();
                                //param.Add("location_flag", LocationFlag.LocationFlag_Usable.ToString());
                                //param.Add("myrow", row);
                                //DBHelper.RunSql(sql, param, "更新下架货位所在行的货位标识为可出不可入", DBHelper.OperatedNumType.ZeroOrMore);

                                UpdateRetrievalStateByTaskId(r);
                                UpdateTaskStatus(r["t_id"].ToString());
                                _workTask.MoveToHistory(r["t_id"].ToString());
                            }
                        }
                        bool isLineHaveOutFlag = IsLineHaveStoredLoc(row.ToString(), tempLocationNo);//判断行，是否存在可出不可入的货位
                        if (isLineHaveOutFlag == false)
                        {
                            UpdateLocationFlag(row, LocationFlag.LocationFlag_Usable.ToString());//将行上所有库位标记，全部修改成“可用”
                        }
                    }
                }
            }


        }


        #region 查询指定行是否存在非已存储状态的货位
        /// <summary>
        /// 查询指定行是否存在非已存储状态的货位
        /// </summary>
        /// <param name="line">行号</param>
        /// <returns>true:存在；false：不存在</returns>
        public bool IsLineHaveStoredLoc(string line,string locNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "line", line },
                { "locNo", locNo }
            };
            string sql = $"select count(l.t_id) as cnt from &DataSource&wms_m_location l where l.location_status not in('{LocationStatus.LocationStatus_Stored}','{LocationStatus.LocationStatus_Empty}') and l.location_no <>@locNo  and l.location_row=@line and l.act_status = '1'";
            DataTable dt = DBHelper.Select(sql, param, "查询指定行是否存在非已存储状态的货位");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) > 0;
        }
        #endregion

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

            DBHelper.RunSql(strSql, dicParam, "更新货位状态", DBHelper.OperatedNumType.ZeroOrMore);
        }

        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="status"></param>
        private void UpdateTaskStatus(string taskID)
        {
            //更新任务明细
            string strSql = @"update &DataSource&wms_b_task_item l set l.act_status=@status where l.task_id=@t_id
                     and l.client_id='&CLIENT_ID&' and l.org_id='&ORG_ID&' and l.warehouse_id = @warehouse_id
                    ";
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "status", "0"},
                    { "t_id",  taskID },
                    { "warehouse_id",  Context.WarehouseID}
                  };

            DBHelper.RunSql(strSql, dicParam, "更任务状态", DBHelper.OperatedNumType.ZeroOrMore);

            //更新任务主档
            strSql = @"update &DataSource&wms_b_task l set l.act_status=@status where l.t_id=@t_id
                     and l.client_id='&CLIENT_ID&' and l.org_id='&ORG_ID&' and l.warehouse_id = @warehouse_id
                    ";

            dicParam = new Dictionary<string, object>() {
                    { "status", "0"},
                    { "t_id",  taskID },
                    { "warehouse_id",  Context.WarehouseID}
                  };

            DBHelper.RunSql(strSql, dicParam, "更任务状态", DBHelper.OperatedNumType.ZeroOrMore);
        }

        /// <summary>
        /// 回收下架任务时回滚下架单状态
        /// </summary>
        /// <param name="tId"></param>
        /// <param name="status"></param>
        private void UpdateRetrievalStateByTaskId(DataRow taskRow)
        {
            //先更新下架单明细状态
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam.Clear();
            dicParam.Add("RetOutboundStatus", RetrievalState.RetOutboundStatus_Cancel.ToString());
            dicParam.Add("RetrievalCode", taskRow["REF_DOC_NO"].ToString());
            dicParam.Add("BarCode", taskRow["container_barcode"].ToString());
            DBHelper.RunSql(@"update &DataSource&wms_d_retrieval_d d set d.retrieval_status=@RetOutboundStatus
                              where d.retrieval_code =@RetrievalCode 
                              and d.container_barcode=@BarCode and d.act_status=1 and d.client_id='&CLIENT_ID&' 
                              and d.org_id='&ORG_ID&' ", dicParam, "更新下架明细",
                              DBHelper.OperatedNumType.ZeroOrMore);

            //更新主单数据

            //判断下架明细是否都已完成
            dicParam.Clear();
            dicParam.Add("RetrievalCode", taskRow["REF_DOC_NO"].ToString());
            dicParam.Add("BarCode", taskRow["container_barcode"].ToString());
            string one = DBHelper.SelectOne(@" select count(*) from &DataSource&wms_d_retrieval_d d 
                              where d.retrieval_code =@RetrievalCode and  d.retrieval_status in ('RetOutboundStatus_Execting') 
                              and d.container_barcode=@BarCode and d.act_status=1 and d.client_id='&CLIENT_ID&' 
                              and d.org_id='&ORG_ID&' ", dicParam, "查询下架明细是否都已完成", false);

            if (Convert.ToInt32(one) > 0) //尚未完成,什么也不用做
            {

            }
            else
            {
                dicParam.Clear();
                dicParam.Add("RetrievalCode", taskRow["REF_DOC_NO"].ToString());
                dicParam.Add("BarCode", taskRow["container_barcode"].ToString());
                string notOverNum = DBHelper.SelectOne(@" select count(*) from &DataSource&wms_d_retrieval_d d 
                              where d.retrieval_code =@RetrievalCode and  d.retrieval_status not in ('RetOutboundStatus_Cancel','RetOutboundStatus_Over') 
                              and d.container_barcode=@BarCode and d.act_status=1 and d.client_id='&CLIENT_ID&' 
                              and d.org_id='&ORG_ID&' ", dicParam, "查询下架明细是否都已完成", false);

                if (Convert.ToInt32(notOverNum) == 0) //都已 完成或取消
                {
                    dicParam.Clear();
                    dicParam.Add("RetOutboundStatus", RetrievalState.RetOutboundStatus_Over.ToString());
                    dicParam.Add("RetrievalCode", taskRow["REF_DOC_NO"].ToString());
                    dicParam.Add("StateNew", RetrievalState.RetOutboundStatus_Create.ToString());
                    DBHelper.RunSql(@"update &DataSource&wms_d_retrieval_h h set h.retrieval_status=@RetOutboundStatus
                              where h.retrieval_code =@RetrievalCode 
                              and h.act_status=1 and h.client_id='&CLIENT_ID&' 
                              and h.org_id='&ORG_ID&' ", dicParam, "更新下架主档",
                                      DBHelper.OperatedNumType.ZeroOrMore);
                }

            }
        }
        #endregion

        #region 创建一楼下架任务
        /// <summary>
        /// 创建一楼下架任务
        /// </summary>
        /// <param name="RetrievalID"></param>
        public WebJsonInfo CreateRetrievalDownTask(string RetrievalID, string equipmentId)
        {
            int rescount = 0;
            string sql = @"";
            string selectOne = "";
            string RETRIEVAL_CODE = "";  //下架单号
            string RETRIEVAL_STATUS = ""; //下架单状态
            string DEST_ADDRID = "";   //下架单地址
            WorkTask workTask = new WorkTask();
            Dictionary<string, object> param = new Dictionary<string, object>();
            DataTable dt = null;
            bool isCanExecuted = false;//堆垛机是否无法执行下架，默认不能执行
            bool isSC01Errored = false;//1号堆垛机是否故障
            bool isSC02Errored = false;//2号堆垛机是否故障
            bool isDesc = false;
            string state_OK = EquipmentStatus.EquipmentStatus_OK.ToString();
            sql = @"select RETRIEVAL_CODE,RETRIEVAL_STATUS,DEST_ADDRID from &DataSource&WMS_D_RETRIEVAL_H t where t.t_id=@RetrievalID";
            param.Clear();
            param.Add("RetrievalID", RetrievalID);
            dt = DBHelper.Select(sql, param, "查询指定ID的下架单", false);
            if (dt.Rows.Count < 1)
            {
                throw new WmsException(500, "不存在此下架单");
            }
            RETRIEVAL_CODE = dt.Rows[0]["RETRIEVAL_CODE"] == null ? "" : dt.Rows[0]["RETRIEVAL_CODE"].ToString();
            RETRIEVAL_STATUS = dt.Rows[0]["RETRIEVAL_STATUS"] == null ? "" : dt.Rows[0]["RETRIEVAL_STATUS"].ToString();
            DEST_ADDRID = dt.Rows[0]["DEST_ADDRID"] == null ? "" : dt.Rows[0]["DEST_ADDRID"].ToString();

            if (RETRIEVAL_STATUS != RetrievalState.RetOutboundStatus_Create.ToString())
            {
                throw new WmsException(500, "下架单不是新建状态");
            }

            if (string.IsNullOrEmpty(DEST_ADDRID))
            {
                throw new WmsException(500, "下架单未指定目标地址");
            }
            string e1 = WmsConfig.GetOrDefault("Task:SC01", "1288091969666551808");  //1号堆垛机id
            string e2 = WmsConfig.GetOrDefault("Task:SC02", "1288092061295316992");  //2号堆垛机id
            //string _tempSql = $"select * from &DataSource&wms_m_equipment t where t_id = '" + e2 + "' ";
            //DataTable _dt = DBHelper.Select(_tempSql, null, $"根据ID{e1}获取堆垛机");
            //if (_dt.Rows[0]["equipment_status"].ToString() == state_OK)
            //{
            //    isSC01Errored = false;
            //    isDesc = false;
            //    isCanExecuted = rdTask(RetrievalID, isDesc, DEST_ADDRID, RETRIEVAL_CODE, e1);
            //}
            //else
            //{
            //    isSC01Errored = true;
            //}
            //if (isSC01Errored == true || (isCanExecuted == false && isSC01Errored == false))//（1号堆垛机故障）或者（1号堆垛机正常且不能执行下架），则需要再次判断2号堆垛机是否可以执行任务
            //{
            //    _tempSql = $"select * from &DataSource&wms_m_equipment t where t_id = '" + e2 + "' ";
            //    _dt = DBHelper.Select(_tempSql, null, $"根据ID{e2}获取堆垛机");
            //    if (_dt.Rows[0]["equipment_status"].ToString() == state_OK)
            //    {
            //        isSC02Errored = false;
            //        isDesc = true;
            //        isCanExecuted = rdTask(RetrievalID, isDesc, DEST_ADDRID, RETRIEVAL_CODE, e2);
            //    }
            //    else
            //    {
            //        isSC02Errored = true;
            //    }
            //}
            //if (isSC01Errored == true && isSC02Errored == true)
            //{
            //    throw new WmsException(500, $"两个堆垛机都处于故障状态，请修复堆垛机后再执行下架任务！");
            //}
            //if (isCanExecuted == false)
            //{
            //    throw new WmsException(500, $"下架单号{RETRIEVAL_CODE}的明细，需要两个堆垛机执行任务或者堆垛机不可达！");
            //}


            if (string.Equals(equipmentId, e1))
            {
                string _tempSql = $"select * from &DataSource&wms_m_equipment t where t_id = '" + e2 + "' ";
                DataTable _dt = DBHelper.Select(_tempSql, null, $"根据ID{e1}获取堆垛机");
                if (_dt.Rows[0]["equipment_status"].ToString() == state_OK)
                {
                    isSC01Errored = false;
                    isDesc = false;
                    isCanExecuted = rdTask(RetrievalID, isDesc, DEST_ADDRID, RETRIEVAL_CODE, e1);
                }
                else
                {
                    isSC01Errored = true;
                }
            }
            else
            {
                string _tempSql = $"select * from &DataSource&wms_m_equipment t where t_id = '" + e2 + "' ";
                DataTable _dt = DBHelper.Select(_tempSql, null, $"根据ID{e2}获取堆垛机");
                if (_dt.Rows[0]["equipment_status"].ToString() == state_OK)
                {
                    isSC02Errored = false;
                    isDesc = true;
                    isCanExecuted = rdTask(RetrievalID, isDesc, DEST_ADDRID, RETRIEVAL_CODE, e2);
                }
                else
                {
                    isSC02Errored = true;
                }
            }

            if (isSC01Errored == true || isSC02Errored == true)
            {
                throw new WmsException(500, $"堆垛机处于故障状态，请修复堆垛机后再执行下架任务！");
            }

            return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = "" };
        }
        #endregion

        #region 根据堆垛机产生任务
        /// <summary>
        /// 根据堆垛机产生任务
        /// </summary>
        /// <param name="RetrievalID"></param>
        /// <param name="isDesc">排序(true:降序(2号堆垛机执行任务);false:升序(1号堆垛机执行任务))</param>
        /// <param name="DEST_ADDRID">目的地址id</param>
        /// <param name="RETRIEVAL_CODE">下架单编号</param>
        /// <param name="scId">执行设备id</param>
        public bool rdTask(string RetrievalID, bool isDesc, string DEST_ADDRID, string RETRIEVAL_CODE, string scId)
        {
            WorkTask workTask = new WorkTask();
            Dictionary<string, object> param = new Dictionary<string, object>();
            string sql = $"select * from (select distinct t.location_id, t.location_code, t.CONTAINER_BARCODE, t.CONTAINER_ID, t.SEQ, l.zone_id,l.location_row from &DataSource&WMS_D_RETRIEVAL_D t left join &DataSource&wms_m_location l on t.location_id = l.t_id where t.ACT_STATUS = '1' and t.RETRIEVAL_H_ID = @RetrievalID) ";
            if (isDesc == false)
            {
                sql += " order by location_row,location_code ";
            }
            else
            {
                sql += " order by location_row,location_code desc";
            }
            param.Clear();
            param.Add("RetrievalID", RetrievalID);
            DataTable dt = DBHelper.Select(sql, param, "查询指定ID的下架单明细", false);

            if (dt.Rows.Count < 1)
            {
                throw new WmsException(500, "下架单明细不存在");
            }
            DataView dataview = dt.DefaultView;
            List<string> locationIDList = dt.AsEnumerable().Select(o => o.Field<string>("location_id")).ToList();
            List<string> locLineList = (dataview.ToTable(true, "location_row")).AsEnumerable().Select(o => o.Field<decimal>("location_row").ToString().Substring(1, 2)).ToList();
            if (isDesc == true)
            {
                if (locLineList.Contains("28") || locLineList.Contains("29") || locLineList.Contains("30"))//这几行只能由1号堆垛机执行
                {
                    throw new WmsException(500, "2号堆垛机无法执行第28、29、30行的下架任务！");
                    //return false;
                }
            }
            else
            {
                if (locLineList.Contains("04") || locLineList.Contains("09") || locLineList.Contains("16") || locLineList.Contains("18") || locLineList.Contains("19") || locLineList.Contains("20") || locLineList.Contains("21") || locLineList.Contains("22") || locLineList.Contains("23") || locLineList.Contains("24") || locLineList.Contains("25") || locLineList.Contains("26") || locLineList.Contains("27"))//这几行只能由2号堆垛机执行
                {
                    throw new WmsException(500, "1号堆垛机无法执行下架明细中部分或全部货位的下架任务！");
                    //return false;
                }
            }
            foreach (DataRow r in dt.Rows)
            {
                string location_id = r["location_id"] == null ? "" : r["location_id"].ToString();
                if (string.IsNullOrEmpty(location_id))
                {
                    throw new WmsException(500, "下架单据中存在货位为空的明细");
                }

                string locationType = "";
                sql = @" select * from &DataSource&wms_m_location l where l.t_id =@location_id ";
                param.Clear();
                param.Add("location_id", location_id);
                DataTable curLocation = DBHelper.Select(sql, param, "查询指定货位的类型", false);
                if (curLocation == null || curLocation.Rows.Count < 1)
                {
                    throw new WmsException(500, "货位不存在");
                }
            }

            IEnumerable<IGrouping<string, DataRow>> result = dt.Rows.Cast<DataRow>().GroupBy<DataRow, string>(dr => dr["location_row"].ToString());//按行分组
            foreach (IGrouping<string, DataRow> ig in result)
            {
                string _tempLine = ig.Key;
                string locNo = ig.FirstOrDefault()["location_code"].ToString();
                string _sql = "select * from &DataSource&wms_m_location where location_row = " + _tempLine +
                        $" and location_status <> '{LocationStatus.LocationStatus_Empty}' and location_no " + (isDesc == true ? ">" : "<") + " '" + locNo + "'";
                DataTable _dt = DBHelper.Select(_sql, null, $"根据行{_tempLine}获取最{(isDesc == true ? "大" : "小")}列待入库货位", false);
                if (_dt.Rows.Count > 0)
                {
                    throw new Exception($"行{_tempLine}存在入库任务或者货位前面存在托盘，无法进行下架！");
                    //return false;
                }
            }
            List<string> tasklist = new List<string>();
            foreach (DataRow r in dt.Rows)
            {
                string location_id = r["location_id"] == null ? "" : r["location_id"].ToString();
                string LOCATION_CODE = r["LOCATION_CODE"] == null ? "" : r["LOCATION_CODE"].ToString();
                string CONTAINER_BARCODE = r["CONTAINER_BARCODE"] == null ? "" : r["CONTAINER_BARCODE"].ToString();
                string CONTAINER_ID = r["CONTAINER_ID"] == null ? "" : r["CONTAINER_ID"].ToString();
                decimal? SEQ = (decimal)1.0;//r["SEQ"] == null ? null : ((decimal?)Convert.ToDecimal(r["SEQ"].ToString()));
                string taskID = SnowflakeID.Guid().ToString();
                sql = @" select location_status,LOCATION_NAME,LOCATION_TYPE,LOCATION_CODE,ZONE_ID  from &DataSource&wms_m_location t where t.ACT_STATUS = '1' and t.t_ID=@location_id";
                param.Clear();
                param.Add("location_id", location_id);
                DataTable temp = DBHelper.Select(sql, param, "查询下架货位状态是否为已存储", false);
                if (temp == null || temp.Rows.Count < 1)
                {
                    throw new WmsException(500, "货位不存在:" + LOCATION_CODE);
                }
                else
                {
                    string Loc_Type = temp.Rows[0]["LOCATION_TYPE"] == null ? "" : temp.Rows[0]["LOCATION_TYPE"].ToString();
                    if (Loc_Type == LocationType.LocationType_Compact.ToString() || Loc_Type == LocationType.LocationType_3DRack.ToString())
                    {
                        if (temp.Rows[0]["location_status"].ToString() != "LocationStatus_Stored" && temp.Rows[0]["location_status"].ToString() != "LocationStatus_Full")
                        {
                            throw new WmsException(500, "货位状态不为已存储:" + LOCATION_CODE);
                        }
                        else
                        {
                            sql = @" update &DataSource&wms_m_location t set t.location_status = '" + LocationStatus.LocationStatus_Out.ToString() + "' where t.t_ID=@location_id";
                            param.Clear();
                            param.Add("location_id", location_id);
                            DBHelper.RunSql(sql, param, "更新货位状态为待出库");
                        }
                    }

                }
                string srcAddType = "AddressType_Zone";// temp.Rows[0]["LOCATION_TYPE"].ToString();
                string srcAddName = temp.Rows[0]["LOCATION_CODE"].ToString();
                string zoneId = temp.Rows[0]["ZONE_ID"].ToString();
                string DestAddType = "";
                string DestAddName = "";
                sql = @" select EQUIPMENT_NAME,EQUIPMENT_CODE  from &DataSource&wms_m_equipment  t where t.ACT_STATUS = '1' and t.t_ID=@DEST_ADDRID";
                param.Clear();
                param.Add("DEST_ADDRID", DEST_ADDRID);
                DataTable srctemp = DBHelper.Select(sql, param, "查询下架单目标地址", false);
                if (srctemp == null || srctemp.Rows.Count < 1)
                {
                    throw new WmsException(500, "目标地址不存在:" + LOCATION_CODE);
                }
                else
                {
                    //DestAddType = srctemp.Rows[0][].ToString();
                    DestAddName = srctemp.Rows[0]["EQUIPMENT_CODE"].ToString();
                }


                workTask.Insert(new Model.WorkTaskModel()
                {
                    ID = taskID,
                    ZoneID = zoneId,                //区域ID
                    TaskNo = WmsHelper.GetSequence("TaskNo"),              //任务编号
                    //DependentTaskNo = "",     //依赖任务号
                    TaskType = TaskType.TaskType_OutBound.ToString(),//任务类型编码
                    ExecutedBy = Context.UserInfo.UserName,          //任务执行人
                    EquipmentID = scId,         //执行设备ID
                    ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),       //执行设备类型
                    ActualEquipmentID = "",   //实际执行设备ID
                    SeqNo = SEQ,               //顺序号
                    StartDate = DateTime.Now,   //开始时间
                    //Priority = "",            //优先级
                    ContainerID = CONTAINER_ID,         //容器ID
                    ContainerBarcode = CONTAINER_BARCODE,    //容器条码
                    RefDocType = DocType.DocType_Retrieval.ToString(),          //来源单据类型
                    RefDocID = RetrievalID,            //来源单ID
                    RefDocDetailID = "",      //来源单明细ID
                    RefDocNo = RETRIEVAL_CODE,            //来源单号
                    SrcAddrID = location_id,           //来源地址ID
                    SrcAddrType = srcAddType,         //来源地址类型
                    SrcAddrText = srcAddName,         //来源地址
                    DestAddrID = DEST_ADDRID,          //目标地址ID
                    DestAddrType = DestAddType,        //目标地址类型
                    DestAddrText = DestAddName,        //目标地址
                    ActualDestAddrID = "",    //实际目标地址ID
                    ActualDestAddrText = "",  //实际目标地址
                    TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),// "TaskStatus_NotExecute",          //任务状态
                    //StartDate = "",           //开始时间
                    //EndDate = "",             //结束时间
                    IsExecutable = "1",        //任务是否可执行
                    //PriorityChangeTime = "",  //优先级修改时间
                    LedText = "",             //LED显示内容
                    ExistLgv = "",            //是否存在LGV
                    CreateDate = DateTime.Now          //创建时间
                    //ModifyDate = ""           //编辑时间
                });


                sql = @"select * from &DataSource&wms_b_container_d t where t.container_id=@CONTAINER_ID";
                param.Clear();
                param.Add("CONTAINER_ID", CONTAINER_ID);
                DataTable itemDt = DBHelper.Select(sql, param, "查询指定托盘ID的库存信息", false);
                if (itemDt == null || itemDt.Rows.Count < 1)
                {
                    continue;
                }

                foreach (DataRow itemR in itemDt.Rows)
                {
                    workTask.AddTaskItem(new TaskItemModel()
                    {
                        t_id = SnowflakeID.Guid().ToString(),
                        client_id = itemR["client_id"] == null ? "" : itemR["client_id"].ToString(),
                        org_id = itemR["org_id"] == null ? "" : itemR["org_id"].ToString(),
                        warehouse_id = itemR["warehouse_id"] == null ? "" : itemR["warehouse_id"].ToString(),
                        task_id = taskID,
                        item_id = itemR["item_id"] == null ? "" : itemR["item_id"].ToString(),
                        item_name = itemR["item_name"] == null ? "" : itemR["item_name"].ToString(),
                        plan_qty = (itemR["qty"] == null || itemR["qty"].ToString() == "") ? 0 : Convert.ToDecimal(itemR["qty"]),
                        uom_qty_id = itemR["uom_qty_id"] == null ? "" : itemR["uom_qty_id"].ToString(),
                        uom_stat_id = itemR["uom_stat_id"] == null ? "" : itemR["uom_stat_id"].ToString(),
                        container_d_id = CONTAINER_ID,
                        ref_doc_type = DocType.DocType_Retrieval.ToString(),
                        ref_doc_id = RetrievalID,
                        ref_doc_no = RETRIEVAL_CODE,
                        ref_doc_d_id = "",
                        process_status = ProcessStatus.ProcessStatus_Doing.ToString(),
                        created_by = Context.UserInfo.UserName,
                        create_date = DateTime.Now,
                        modify_date = DateTime.Now,
                        act_status = "1",
                    });
                }

                sql = @" update &DataSource&WMS_D_RETRIEVAL_H t set t.RETRIEVAL_STATUS='" + RetrievalState.RetOutboundStatus_Execting.ToString() + "' where t.ACT_STATUS = '1' and t.t_ID=@RetrievalID";
                param.Clear();
                param.Add("RetrievalID", RetrievalID);
                int rescount = DBHelper.RunSql(sql, param, "更新下架单状态为下架中", DBHelper.OperatedNumType.ZeroOrMore);
                if (rescount > 0)
                {
                    sql = @"update &DataSource&WMS_D_RETRIEVAL_D t set t.RETRIEVAL_STATUS = '" + RetrievalState.RetOutboundStatus_Execting.ToString() + "' where  t.RETRIEVAL_H_ID=@RetrievalID and t.LOCATION_ID = @LOCATION_ID";
                    param.Clear();
                    param.Add("RetrievalID", RetrievalID);
                    param.Add("LOCATION_ID", location_id);
                    DBHelper.RunSql(sql, param, "更新下架单明细状态为下架中", DBHelper.OperatedNumType.ZeroOrMore);
                }
                tasklist.Add(taskID);
            }
            if (tasklist.Count > 0)
            {
                WorkTaskInterface.Send(tasklist);
            }

            return true;
        }

        #endregion

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

    }
}
