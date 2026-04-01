using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Inventory;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Tasking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace NtiCloud.Service.LPS.Inbound
{
    class Putaway
    {
        private IDBContextProvider dbW;
        public Putaway(IDBContextProvider _dbW)
        {
            dbW = _dbW;
        }

        public Putaway() { }
        /// <summary>
        /// 货位预分配入口
        /// </summary>
        /// <param name="putawayinfo"></param>
        /// <returns></returns>
        public WebJsonInfo AllocateStorageBin(PutawayModelInput putawayinfo)
        {
            WebJsonInfo re = new WebJsonInfo();
            try
            {

                PutawayHeaderModel putawayheader = GetPutawayHeader(putawayinfo.PutawayId);
                if (putawayheader == null)
                {
                    throw new Exception("未找到当前上架单信息");
                }
                if (putawayheader.act_status != "1")
                {
                    throw new Exception("当前上架单已不再有效");
                }
                if (putawayheader.doc_status != PutawayStatus.Putaway_Create.ToString())
                {
                    DataRow[] dr = WmsHelper.GetDictionary("DictCodeAll:WMS:Putaway_Status:" + putawayheader.doc_status);
                    if (dr.Length > 0)
                    {
                        throw new Exception("上架单当前状态为:" + dr[0]["dict_name"].ToString() + "，不允许再分配货位。");
                    }
                    else {
                        throw new Exception("上架单当前状态为:" + putawayheader.doc_status + "，不允许再分配货位。");
                    }
                    
                }
                List<PutawayDetailModel> lstDetail = GetPutawayDetailList(putawayheader.t_id);
                foreach (PutawayDetailModel dr in lstDetail)
                {
                    putawayinfo.PutawayDetailId = dr.t_id;
                    AllocateLocation(putawayinfo.owner_id, putawayinfo.PutawayId, putawayinfo.PutawayNo, putawayinfo.PutawayDetailId);
                }
                UpdatePutawayStatus(dbW, Context.UserInfo, putawayinfo.PutawayId, PutawayStatus.Putaway_Allocate);
                re.errcode = 0;
                re.errmsg = "OK";
                re.isSucceed = true;
                re.json = "";
            }
            catch (Exception ex)
            {
                re.errcode = -1;
                re.errmsg = "异常:" + ex.Message;
                re.isSucceed = false;
                re.json = "";
            }
            return re;
        }

        /// <summary>
        /// 货位预分配:分配单个上架单行的货位
        /// </summary>
        /// <param name="ownerid">货主</param>
        ///<param name="putawayid">上架单ID</param>
        ///<param name="putawayno">上架单号</param>
        ///<param name="putawaydetailid">上架单明细ID</param>
        private void AllocateLocation(string ownerid, string putawayid, string putawayno, string putawaydetailid)
        {
            PutawayDetailModel putawayDetail = getPutawayDetail(putawaydetailid);
            AllocateLocationIn parm = new AllocateLocationIn();
            parm.warehouserid = putawayDetail.warehouse_id;
            parm.itemcode = putawayDetail.item_code;
            parm.itemName= putawayDetail.item_name;
            parm.lotno = putawayDetail.lot_no;
            parm.owner_id = ownerid;
            parm.proucedate = putawayDetail.production_date == null ? "" : ((DateTime)putawayDetail.production_date).ToString("yyyy-MM-dd");
            parm.qty = putawayDetail.plan_qty ?? 0;
            parm.supplier_id = putawayDetail.supplier_id;
            parm.itemid = putawayDetail.item_id;
            parm.ref_doc_type = DocType.DocType_Inbound.ToString();
            parm.ref_doc_id = putawayid;
            parm.ref_doc_no = putawayno;
            parm.ref_doc_d_id = putawaydetailid;
            ContainerDModel detail = new ContainerDModel();
            detail.item_id = parm.itemid;
            if (!string.IsNullOrEmpty(parm.proucedate))
            {
                detail.production_date = DateTime.Parse(parm.proucedate);
            }
            detail.lot_no = parm.lotno;
            detail.sub_inventory = putawayDetail.dest_warehouse;
            detail.sub_location = putawayDetail.dest_location;

            parm.wmslotno = new Container().GenerateWmsLot(detail);
            while (parm.qty > 0)
            {
                AllocateOneLocation(ref parm, putawayDetail);
            }

        }
        /// <summary>
        /// 分配单个货位
        /// </summary>
        /// <param name="dbR">只读DB</param>
        /// <param name="dbW">只写DB</param>
        /// <param name="user">用户信息</param>
        /// <param name="param">分配货位需要传入的参数信息</param>
        /// <param name="drdetail">待分配货位的行数据</param>
        /// <returns></returns>
        private AllocateLocationOut AllocateOneLocation(ref AllocateLocationIn param, DataRow drdetail)
        {
            string sql = @"select warehouse_id, t_id, location_code, container_limit, qty_limit, weight_limit, plts, totalqty, uom 
                            from &DataSource&view_location_storageinfo where warehouse_id = @warehouserid and rownum = 1";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("warehouserid", param.warehouserid);
            DataTable dt = DBHelper.Select(sql, lstParam);
            AllocateLocationOut re = new AllocateLocationOut();
            if (dt.Rows.Count == 0)
            {
                re.isSuccess = false;
                return re;
            }
            DataRow dr = dt.Rows[0];
            re.isSuccess = true;
            re.locationId = dr["t_id"].ToString();
            re.locationCode = dr["location_code"].ToString();
            re.containerNo = dr["location_code"].ToString();

            decimal qty_limit = decimal.Parse(dr["qty_limit"].ToString());
            if (qty_limit > param.qty)
            { //按数量控制               
                if (qty_limit >= param.qty)
                { //一个货位能够存下
                    re.allocateQty = param.qty;
                    param.qty = 0;
                }
                else
                {
                    re.allocateQty = qty_limit;
                    param.qty = param.qty - qty_limit;
                }
            }
            else
            { //托盘、容器，一个货位只能放一个容器 
                re.allocateQty = param.qty;
                param.qty = 0;
            }
            Container iContainerService = new Container();
            string containerId = "", ctnDetailId = "";
            dbW.BeginTransaction(Context.UserInfo);

            #region 生产/更新容器信息

            bool isExists = iContainerService.CheckLocationExis(re.locationCode);
            ContainerDModel detailm = null; ;
            if (isExists)
            {
                containerId = iContainerService.getContainerIdByLocation(re.locationCode);
                iContainerService.UpdateContainerModifyInfo(dbW, Context.UserInfo, containerId);
                isExists = iContainerService.CheckContainerDetailExists(containerId, param.itemid, param.wmslotno);
                if (isExists)
                { //存在，则将本次待入库的熟练累加到现有容器行
                    ctnDetailId = iContainerService.getContainerDetailId(containerId, param.itemid, param.wmslotno);
                    detailm = iContainerService.getContainerDetailById(ctnDetailId);
                    iContainerService.AddInQty(dbW, Context.UserInfo, ctnDetailId, re.allocateQty);
                }
                else
                { //新增容器明细
                    detailm = new ContainerDModel();
                    detailm.container_id = containerId;
                    detailm.act_status = "1";
                    detailm.warehouse_id = drdetail["warehouse_id"].ToString();
                    detailm.client_id = drdetail["client_id"].ToString();
                    detailm.t_id = SnowflakeID.Guid().ToString();
                    detailm.item_id = drdetail["item_id"].ToString();
                    detailm.item_name = drdetail["item_name"].ToString();
                    detailm.in_qty = 0;
                    detailm.in_qty = re.allocateQty;
                    detailm.is_full = drdetail["is_full"].ToString();
                    detailm.given_year = int.Parse(string.IsNullOrEmpty(drdetail["given_year"].ToString()) ? "0" : drdetail["given_year"].ToString());
                    detailm.lot_no = drdetail["lot_no"].ToString();
                    detailm.wms_lot_no = param.wmslotno;
                    detailm.uom_qty_id = drdetail["uom_qty_id"].ToString();
                    detailm.uom_stat_id = drdetail["uom_stat_id"].ToString();
                    detailm.created_by = Context.UserInfo.UserName;
                    detailm.create_date = DateTime.Now;
                    detailm.modify_date = DateTime.Now;
                    iContainerService.AddContainerD(dbW, Context.UserInfo, detailm);
                }
            }//容器已存在结束
            else
            { //容器不存在
                containerId = SnowflakeID.Guid().ToString();
                ContainerModel ctnH = new ContainerModel();
                ctnH.t_id = containerId;
                ctnH.client_id = drdetail["client_id"].ToString();
                ctnH.org_id = drdetail["org_id"].ToString();
                ctnH.warehouse_id = drdetail["warehouse_id"].ToString();
                ctnH.act_status = "1";
                ctnH.location_id = re.locationId;
                ctnH.container_barcode = re.locationCode;
                ctnH.container_type = "ContainerType_Virtual";
                ctnH.first_in_date = DateTime.Now;
                ctnH.owner_id = param.owner_id;
                ctnH.created_by = Context.UserInfo.UserName;
                ctnH.create_date = DateTime.Now;
                ctnH.modify_date = DateTime.Now;
                iContainerService.AddContainerH(dbW, Context.UserInfo, ctnH);

                detailm = new ContainerDModel();
                detailm.container_id = containerId;
                detailm.act_status = "1";
                detailm.client_id = drdetail["client_id"].ToString();
                detailm.t_id = SnowflakeID.Guid().ToString();
                detailm.item_id = drdetail["item_id"].ToString();
                detailm.item_name = drdetail["item_name"].ToString();
                detailm.in_qty = 0;
                detailm.in_qty = re.allocateQty;
                detailm.is_full = drdetail["is_full"].ToString();
                detailm.given_year = int.Parse(string.IsNullOrEmpty(drdetail["given_year"].ToString()) ? "0" : drdetail["given_year"].ToString());
                detailm.lot_no = drdetail["lot_no"].ToString();
                detailm.uom_qty_id =
                detailm.wms_lot_no = param.wmslotno;
                detailm.uom_qty_id = drdetail["uom_qty_id"].ToString();
                detailm.uom_stat_id = drdetail["uom_stat_id"].ToString();
                detailm.created_by = Context.UserInfo.UserName;
                detailm.create_date = DateTime.Now;
                detailm.modify_date = DateTime.Now;
                iContainerService.AddContainerD(dbW, Context.UserInfo, detailm);

            }//容器不存在结束
            #endregion

            #region 生产任务信息
            TaskService task = new TaskService();
            TaskHeaderModel tm = new TaskHeaderModel();
            tm.t_id = SnowflakeID.Guid().ToString();
            tm.task_no = WmsHelper.GetSequence("TaskNo");
            tm.client_id = detailm.client_id;
            tm.org_id = detailm.org_id;
            tm.warehouse_id = detailm.warehouse_id;
            tm.task_type = TaskType.TaskType_InBound.ToString();
            tm.seq_no = decimal.ToInt32(decimal.Parse(drdetail["LINE_NO"].ToString()));
            tm.priority = 2;
            tm.container_id = containerId;
            tm.container_barcode = re.containerNo;
            tm.ref_doc_type = param.ref_doc_type;
            tm.ref_doc_id = param.ref_doc_id;
            tm.ref_doc_no = param.ref_doc_no;
            tm.ref_doc_d_id = param.ref_doc_d_id;
            tm.dest_addr_id = re.locationId;
            tm.dest_addr_text = re.locationCode;
            tm.dest_addr_type = "LocationCode";
            tm.task_status = WMS.Base.TaskStatus.TaskStatus_Create.ToString();
            tm.is_executable = "0";
            tm.created_by = Context.UserInfo.UserName;
            tm.create_date = DateTime.Now;
            tm.modify_date = DateTime.Now;
            tm.act_status = "1";
            task.AddTask(dbW, Context.UserInfo, tm);

            TaskItemModel tim = new TaskItemModel();
            tim.t_id = SnowflakeID.Guid().ToString();
            tim.client_id = detailm.client_id;
            tim.org_id = detailm.org_id;
            tim.warehouse_id = detailm.warehouse_id;
            tim.task_id = tm.t_id;
            tim.item_id = param.itemid;
            tim.item_name = param.itemName;
            tim.plan_qty = re.allocateQty;
            tim.uom_qty_id = detailm.uom_qty_id;
            tim.uom_stat_id = detailm.uom_stat_id;
            tim.container_d_id = detailm.t_id;
            tim.ref_doc_type = param.ref_doc_type;
            tim.ref_doc_id = param.ref_doc_id;
            tim.ref_doc_no = param.ref_doc_no;
            tim.ref_doc_d_id = param.ref_doc_d_id;
            tim.process_status = ProcessStatus.ProcessStatus_New.ToString();
            tim.created_by = Context.UserInfo.UserName;
            tim.create_date = DateTime.Now;
            tim.modify_date = DateTime.Now;
            tim.act_status = "1";
            task.AddTaskItem(dbW, Context.UserInfo, tim);

            #endregion

            dbW.EndTransaction(Context.UserInfo, true, false);
            return re;
        }

        /// <summary>
        /// 分配单个货位
        /// </summary>
        /// <param name="param">分配货位需要的相关参数</param>
        /// <param name="putawayDetail">对应的上架单明细信息</param>
        /// <returns></returns>
        private AllocateLocationOut AllocateOneLocation(ref AllocateLocationIn param, PutawayDetailModel putawayDetail)
        {
            /*string sql = @"select warehouse_id, t_id, location_code, container_limit, qty_limit, weight_limit, plts, totalqty, uom 
                            from &DataSource&view_location_storageinfo where warehouse_id = @warehouserid and location_status=@location_status and rownum = 1";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("warehouserid", param.warehouserid);
            lstParam.Add("location_status", LocationStatus.LocationStatus_Empty.ToString());
            DataTable dt = DBHelper.Select(sql, lstParam);
            AllocateLocationOut re = new AllocateLocationOut();
            if (dt.Rows.Count == 0)
            {
                re.isSuccess = false;
                return re;
            }
            DataRow dr = dt.Rows[0];
            re.isSuccess = true;
            re.locationId = dr["t_id"].ToString();
            re.locationCode = dr["location_code"].ToString();
            re.containerNo = dr["location_code"].ToString();
            */
            AllocateLocationOut re = new AllocateLocationOut();
            string locationid = AllocateOneLocationForPallet2(param.warehouserid, putawayDetail.item_id);
            if (string.IsNullOrEmpty(locationid))
            {
                re.isSuccess = false;
                return re;
            }
            LocationModel loc = new Location().GetLocationById(locationid);
            re.isSuccess = true;
            re.locationId = loc.t_id;
            re.locationCode = loc.location_code;
            re.containerNo = loc.location_code;

            decimal qty_limit = loc.qty_limit == null ? 0 : (decimal)loc.qty_limit;
            if (qty_limit > param.qty)
            { //按数量控制               
                if (qty_limit >= param.qty)
                { //一个货位能够存下
                    re.allocateQty = param.qty;
                    param.qty = 0;
                }
                else
                {
                    re.allocateQty = qty_limit;
                    param.qty = param.qty - qty_limit;
                }
            }
            else
            { //托盘、容器，一个货位只能放一个容器 
                re.allocateQty = param.qty;
                param.qty = 0;
            }
            Container iContainerService = new Container();
            string containerId = "", ctnDetailId = "";
            dbW.BeginTransaction(Context.UserInfo);

            #region 生产/更新容器信息

            bool isExists = iContainerService.CheckLocationExis(re.locationCode);
            ContainerDModel detailm = null; ;
            if (isExists)
            {
                containerId = iContainerService.getContainerIdByLocation(re.locationCode);
                iContainerService.UpdateContainerModifyInfo(dbW, Context.UserInfo, containerId);
                isExists = iContainerService.CheckContainerDetailExists(containerId, param.itemid, param.wmslotno);
                if (isExists)
                { //存在，则将本次待入库的熟练累加到现有容器行
                    ctnDetailId = iContainerService.getContainerDetailId(containerId, param.itemid, param.wmslotno);
                    detailm = iContainerService.getContainerDetailById(ctnDetailId);
                    iContainerService.AddInQty(dbW, Context.UserInfo, ctnDetailId, re.allocateQty);
                }
                else
                { //新增容器明细
                    detailm = new ContainerDModel();
                    detailm.container_id = containerId;
                    detailm.act_status = "1";
                    detailm.warehouse_id = putawayDetail.warehouse_id;
                    detailm.client_id = putawayDetail.client_id;
                    detailm.t_id = SnowflakeID.Guid().ToString();
                    detailm.item_id = putawayDetail.item_id;
                    detailm.item_name = putawayDetail.item_name;
                    detailm.in_qty = 0;
                    detailm.in_qty = re.allocateQty;
                    detailm.is_full = putawayDetail.is_full;
                    detailm.given_year = putawayDetail.given_year;
                    detailm.lot_no = putawayDetail.lot_no;
                    detailm.wms_lot_no = param.wmslotno;
                    detailm.uom_qty_id = putawayDetail.uom_qty_id;
                    detailm.uom_stat_id = putawayDetail.uom_stat_id;
                    detailm.ratio = (putawayDetail.plan_stat_qty ?? 0) / (putawayDetail.plan_qty ?? 0);
                    detailm.created_by = Context.UserInfo.UserName;
                    detailm.create_date = DateTime.Now;
                    detailm.modify_date = DateTime.Now;
                    iContainerService.AddContainerD(dbW, Context.UserInfo, detailm);
                }
            }//容器已存在结束
            else
            { //容器不存在
                containerId = SnowflakeID.Guid().ToString();
                ContainerModel ctnH = new ContainerModel();
                ctnH.t_id = containerId;
                ctnH.client_id = putawayDetail.client_id;// drdetail["client_id"].ToString();
                ctnH.org_id = putawayDetail.org_id;//drdetail["org_id"].ToString();
                ctnH.warehouse_id = putawayDetail.warehouse_id;// drdetail["warehouse_id"].ToString();
                ctnH.act_status = "1";
                ctnH.location_id = re.locationId;
                ctnH.container_barcode = re.locationCode;
                ctnH.container_type = ContainerType.ContainerType_Virtual.ToString();
                ctnH.first_in_date = DateTime.Now;
                ctnH.owner_id = param.owner_id;
                ctnH.created_by = Context.UserInfo.UserName;
                ctnH.create_date = DateTime.Now;
                ctnH.modify_date = DateTime.Now;
                iContainerService.AddContainerH(dbW, Context.UserInfo, ctnH);

                detailm = new ContainerDModel();
                detailm.container_id = containerId;
                detailm.act_status = "1";
                detailm.warehouse_id = putawayDetail.warehouse_id;
                detailm.client_id = putawayDetail.client_id;
                detailm.t_id = SnowflakeID.Guid().ToString();
                detailm.item_id = putawayDetail.item_id;
                detailm.item_name = putawayDetail.item_name;
                detailm.in_qty = 0;
                detailm.in_qty = re.allocateQty;
                detailm.is_full = putawayDetail.is_full;
                detailm.given_year = putawayDetail.given_year;
                detailm.lot_no = putawayDetail.lot_no;
                detailm.wms_lot_no = param.wmslotno;
                detailm.uom_qty_id = putawayDetail.uom_qty_id;
                detailm.uom_stat_id = putawayDetail.uom_stat_id;
                detailm.ratio = (putawayDetail.plan_stat_qty ?? 0) / (putawayDetail.plan_qty ?? 0);
                detailm.created_by = Context.UserInfo.UserName;
                detailm.create_date = DateTime.Now;
                detailm.modify_date = DateTime.Now;
                iContainerService.AddContainerD(dbW, Context.UserInfo, detailm);

            }//容器不存在结束
            #endregion

            #region 生产任务信息
            TaskService task = new TaskService();
            TaskHeaderModel tm = new TaskHeaderModel();
            tm.t_id = SnowflakeID.Guid().ToString();
            tm.task_no = WmsHelper.GetSequence("TaskNo");
            tm.client_id = detailm.client_id;
            tm.org_id = detailm.org_id;
            tm.warehouse_id = detailm.warehouse_id;
            tm.task_type = TaskType.TaskType_InBound.ToString();
            if (!string.IsNullOrEmpty(putawayDetail.line_no)) {
                tm.seq_no = decimal.ToInt32(decimal.Parse(putawayDetail.line_no));
            }
            tm.priority = 100;
            tm.container_id = containerId;
            tm.container_barcode = re.containerNo;
            tm.ref_doc_type = param.ref_doc_type;
            tm.ref_doc_id = param.ref_doc_id;
            tm.ref_doc_no = param.ref_doc_no;
            tm.ref_doc_d_id = param.ref_doc_d_id;
            tm.dest_addr_id = re.locationId;
            tm.dest_addr_text = re.locationCode;
            tm.dest_addr_type = AddressType.AddressType_Location.ToString();
            tm.task_status = WMS.Base.TaskStatus.TaskStatus_Create.ToString();
            tm.is_executable = "0";
            tm.created_by = Context.UserInfo.UserName;
            tm.create_date = DateTime.Now;
            tm.modify_date = DateTime.Now;
            tm.act_status = "1";
            task.AddTask(dbW, Context.UserInfo, tm);

            TaskItemModel tim = new TaskItemModel();
            tim.t_id = SnowflakeID.Guid().ToString();
            tim.client_id = detailm.client_id;
            tim.org_id = detailm.org_id;
            tim.warehouse_id = detailm.warehouse_id;
            tim.task_id = tm.t_id;
            tim.item_id = param.itemid;
            tim.item_name = param.itemName;
            tim.plan_qty = re.allocateQty;
            tim.uom_qty_id = detailm.uom_qty_id;
            tim.uom_stat_id = detailm.uom_stat_id;
            tim.container_d_id = detailm.t_id;
            tim.ref_doc_type = param.ref_doc_type;
            tim.ref_doc_id = param.ref_doc_id;
            tim.ref_doc_no = param.ref_doc_no;
            tim.ref_doc_d_id = param.ref_doc_d_id;
            tim.process_status = ProcessStatus.ProcessStatus_New.ToString();
            tim.created_by = Context.UserInfo.UserName;
            tim.create_date = DateTime.Now;
            tim.modify_date = DateTime.Now;
            tim.act_status = "1";
            task.AddTaskItem(dbW, Context.UserInfo, tim);

            #endregion

            dbW.EndTransaction(Context.UserInfo, true, false);
            return re;
        }

        /// <summary>
        /// 模拟货位分配，随机返回一个货位
        /// </summary>
        /// <param name="warehouserid"></param>
        /// <param name="itemcode"></param>
        /// <returns></returns>
        public string AllocateOneLocationForPallet(string warehouserid, string itemcode, LocationType locationType)
        {
            string sql = @"select location_code from &DataSource&VIEW_EMPTYLOCATION sample ( 10 ) where location_type=@location_type and rownum＜2";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("warehouserid", warehouserid);
            lstParam.Add("location_type", locationType.ToString());
            return DBHelper.SelectOne(sql, lstParam);
        }
        /// <summary>
        /// 调用高架算法分配
        /// </summary>
        /// <param name="warehouserid"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        public string AllocateOneLocationForPallet(string warehouserid, string itemid, string bomId = "", string zoneId = "")
        {
            /*string sql = @"select location_code from &DataSource&VIEW_EMPTYLOCATION sample ( 10 ) where location_type=@location_type and rownum＜2";
             Dictionary<string, object> lstParam = new Dictionary<string, object>();
             lstParam.Add("warehouserid", warehouserid);
             lstParam.Add("location_type", locationType.ToString());
             return DBHelper.SelectOne(sql, lstParam);*/

            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "I_WAREHOUSEID",warehouserid },
                { "ITEMID",itemid },
                { "BOMID",bomId },
                { "I_ZONE", "" },
                { "ZONE_ID", zoneId },                
                { "I_HEADID", "" },
                { "I_DETAILID", "" },
                { "I_PACKING_TYPE", "" },
                { "I_ROOM_NO", "" },
            };
            string rule = WmsConfig.GetOrDefault("Inbound:AllocationRule", "work_task_3");
            
            if (string.Equals(zoneId, WmsConfig.GetOrDefault("Outbound:FirstFloorZone", "1291999756888969216")) || string.Equals(zoneId, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))
            {//恩施专用
                if (!string.Equals(zoneId, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))
                {//一楼密集库
                    rule = WmsConfig.GetOrDefault("Inbound:Allocation_Dense", "work_task_8888");
                }
                else
                {//二楼AGV
                    rule = WmsConfig.GetOrDefault("Inbound:Allocation_AGV", "work_task_21");
                    param.Add("LINE", "-1" );//第一次货位分配
                }
            }
            return new LocationAllocator().AllocateLocation(param, rule);

        }
        /// <summary>
        /// 调用平库算法分配
        /// </summary>
        /// <param name="warehouserid"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        public string AllocateOneLocationForPallet2(string warehouserid, string itemid)
        {
            /*string sql = @"select location_code from &DataSource&VIEW_EMPTYLOCATION sample ( 10 ) where location_type=@location_type and rownum＜2";
             Dictionary<string, object> lstParam = new Dictionary<string, object>();
             lstParam.Add("warehouserid", warehouserid);
             lstParam.Add("location_type", locationType.ToString());
             return DBHelper.SelectOne(sql, lstParam);*/

            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "I_WAREHOUSEID",warehouserid },
                { "ITEMID",itemid },
                { "I_ZONE", "" },
                { "I_HEADID", "" },
                { "I_DETAILID", "" },
                { "I_PACKING_TYPE", "" },
                { "I_ROOM_NO", "" },
            };
            return new LocationAllocator().AllocateLocation(param,
                        WmsConfig.GetOrDefault("Inbound:AllocationRule_FLAT", "worktask10"));

        }

        public string GenerateWmsLot(PutawayDetailModel putawaydetail)
        {
            return putawaydetail.lot_no + "," + putawaydetail.dest_warehouse + "," + putawaydetail.dest_location;
        }
        /// <summary>
        /// 新增容器信息
        /// </summary>
        /// <param name="containercode"></param>
        /// <param name="putawayinfo"></param>
        /// <param name="putawaydetail"></param>
        /// <param name="putawayheader"></param>
        /// <returns></returns>
        public ContainerDModel UpdateContainerInfo(string containercode, PutawayDirectDoneIn putawayinfo, PutawayDetailModel putawaydetail, PutawayHeaderModel putawayheader)
        {
            #region 生产/更新容器信息
            Container iContainerService = new Container();
            bool isExists = iContainerService.CheckLocationExis(containercode);
            ContainerDModel detailm = null;
            string containerId = "", ctnDetailId = "";
            string wmslotno = GenerateWmsLot(putawaydetail);
            if (isExists)
            {
                containerId = iContainerService.getContainerIdByLocation(putawayinfo.ToLocation);
                iContainerService.UpdateContainerModifyInfo(containerId);
                isExists = iContainerService.CheckContainerDetailExists(containerId, putawaydetail.item_id, wmslotno);
                if (isExists)
                { //存在，则将本次待入库的熟练累加到现有容器行
                    ctnDetailId = iContainerService.getContainerDetailId(containerId, putawaydetail.item_id, wmslotno);
                    detailm = iContainerService.getContainerDetailById(ctnDetailId);
                    iContainerService.AddInQty(ctnDetailId, putawayinfo.Qty);
                }
                else
                { //新增容器明细
                    detailm = new ContainerDModel();
                    detailm.container_id = containerId;
                    detailm.act_status = "1";
                    detailm.warehouse_id = putawaydetail.warehouse_id;
                    detailm.client_id = putawaydetail.client_id;
                    detailm.t_id = SnowflakeID.Guid().ToString();
                    detailm.item_id = putawaydetail.item_id;
                    detailm.item_name = putawaydetail.item_name;
                    detailm.in_qty = putawayinfo.Qty;
                    detailm.is_full = putawaydetail.is_full;
                    detailm.given_year = putawaydetail.given_year;
                    detailm.lot_no = putawaydetail.lot_no;
                    detailm.wms_lot_no = wmslotno;
                    detailm.created_by = Context.UserInfo.UserName;
                    detailm.create_date = DateTime.Now;
                    detailm.modify_date = DateTime.Now;

                    detailm.sub_factory = putawaydetail.dest_factory;
                    detailm.sub_inventory = putawaydetail.dest_warehouse;
                    detailm.sub_location = putawaydetail.dest_location;
                    detailm.production_date = putawayinfo.ProduceDate;
                    detailm.owner_id = putawayheader.owner_id;
                    detailm.exist_barcode = putawaydetail.exist_barcode;
                    detailm.qc_status = putawaydetail.qc_status;
                    detailm.uom_qty_id = putawaydetail.uom_qty_id;
                    detailm.uom_stat_id = putawaydetail.uom_stat_id;
                    detailm.first_in_date = DateTime.Now;
                    iContainerService.SetContainerAlarmDate(detailm);

                    iContainerService.AddContainerD(detailm);
                }
            }//容器已存在结束
            else
            { //容器不存在
                containerId = SnowflakeID.Guid().ToString();
                ContainerModel ctnH = new ContainerModel();
                ctnH.t_id = containerId;
                ctnH.client_id = putawaydetail.client_id;
                ctnH.org_id = putawaydetail.org_id;
                ctnH.warehouse_id = putawaydetail.warehouse_id;
                ctnH.act_status = "1";
                LocationModel location = new Location().GetLocationByCode(putawayheader.warehouse_id, putawayinfo.ToLocation);
                ctnH.location_id = location.t_id;
                ctnH.container_barcode = putawayinfo.ToLocation;
                ctnH.container_type = "ContainerType_Virtual";
                ctnH.first_in_date = DateTime.Now;
                ctnH.owner_id = putawayheader.owner_id;
                ctnH.created_by = Context.UserInfo.UserName;
                ctnH.create_date = DateTime.Now;
                ctnH.modify_date = DateTime.Now;
                iContainerService.AddContainerH(ctnH);

                detailm = new ContainerDModel();
                detailm.container_id = containerId;
                detailm.act_status = "1";
                detailm.client_id = putawaydetail.client_id;
                detailm.t_id = SnowflakeID.Guid().ToString();
                detailm.item_id = putawaydetail.item_id;
                detailm.item_name = putawaydetail.item_name;
                detailm.in_qty = putawayinfo.Qty;
                detailm.is_full = putawaydetail.is_full;
                detailm.given_year = putawaydetail.given_year;
                detailm.lot_no = putawayinfo.LotNo;
                detailm.wms_lot_no = wmslotno;
                detailm.created_by = Context.UserInfo.UserName;
                detailm.create_date = DateTime.Now;
                detailm.modify_date = DateTime.Now;

                detailm.warehouse_id = ctnH.warehouse_id;
                detailm.sub_factory = putawaydetail.dest_factory;
                detailm.sub_inventory = putawaydetail.dest_warehouse;
                detailm.sub_location = putawaydetail.dest_location;
                detailm.production_date = putawayinfo.ProduceDate;
                detailm.owner_id = putawayheader.owner_id;
                detailm.exist_barcode = putawaydetail.exist_barcode;
                detailm.qc_status = putawaydetail.qc_status;
                detailm.uom_qty_id = putawaydetail.uom_qty_id;
                detailm.uom_stat_id = putawaydetail.uom_stat_id;
                detailm.first_in_date = DateTime.Now;
                iContainerService.SetContainerAlarmDate(detailm);

                iContainerService.AddContainerD(detailm);

            }//容器不存在结束
            #endregion
            return detailm;
        }


        public TaskItemModel MakeNewTask(LocationModel location, ContainerModel container, ContainerDModel containerd,
            PutawayHeaderModel putaway, PutawayDetailModel putawaydetail, PutawayDirectDoneIn doneinfo)
        {
            #region 生产任务信息
            TaskService task = new TaskService();
            TaskHeaderModel tm = new TaskHeaderModel();
            tm.t_id = SnowflakeID.Guid().ToString();
            tm.task_no = WmsHelper.GetSequence("TaskNo");
            tm.client_id = containerd.client_id;
            tm.org_id = containerd.org_id;
            tm.warehouse_id = containerd.warehouse_id;
            tm.task_type = TaskType.TaskType_InBound.ToString();
            tm.seq_no = decimal.ToInt32(decimal.Parse(putawaydetail.line_no));
            tm.priority = 2;
            tm.container_id = containerd.container_id;
            tm.container_barcode = location.location_code;
            tm.ref_doc_type = putaway.doc_type;
            tm.ref_doc_id = putaway.t_id;
            tm.ref_doc_no = putaway.doc_no;
            tm.ref_doc_d_id = putawaydetail.t_id;
            tm.dest_addr_id = container.location_id;
            tm.dest_addr_text = location.location_code;
            tm.dest_addr_type =AddressType.AddressType_Location.ToString();
            tm.task_status = WMS.Base.TaskStatus.TaskStatus_Create.ToString();
            tm.is_executable = "0";
            tm.created_by = Context.UserInfo.UserName;
            tm.create_date = DateTime.Now;
            tm.modify_date = DateTime.Now;
            tm.act_status = "1";
            task.AddTask(tm);

            TaskItemModel tim = new TaskItemModel();
            tim.t_id = SnowflakeID.Guid().ToString();
            tim.client_id = containerd.client_id;
            tim.org_id = containerd.org_id;
            tim.warehouse_id = containerd.warehouse_id;
            tim.task_id = tm.t_id;
            tim.item_id = containerd.item_id;
            tim.item_name = containerd.item_name;
            tim.plan_qty = doneinfo.Qty;
            tim.uom_qty_id = putawaydetail.uom_qty_id;
            tim.uom_stat_id = putawaydetail.uom_stat_id;
            tim.container_d_id = containerd.t_id;
            tim.ref_doc_type = putaway.doc_type;
            tim.ref_doc_id = putaway.t_id;
            tim.ref_doc_no = putaway.doc_no;
            tim.ref_doc_d_id = putawaydetail.t_id;
            tim.process_status = ProcessStatus.ProcessStatus_New.ToString();
            tim.created_by = Context.UserInfo.UserName;
            tim.create_date = DateTime.Now;
            tim.modify_date = DateTime.Now;
            tim.act_status = "1";
            task.AddTaskItem(tim);

            #endregion
            return tim;

        }

        /// <summary>
        /// 修改上架单状态
        /// </summary>
        /// <param name="dbW"></param>
        /// <param name="user"></param>
        /// <param name="putawayid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdatePutawayStatus(IDBContextProvider dbW, UserLoginInfo user, string putawayid, PutawayStatus status)
        {

            string sql = @" update  &DataSource&wms_d_inbound_h set doc_status = '" + status.ToString() + "',MODIFY_IP = '" + user.LoginIP + "',LAST_MODIFIED_BY = '" + user.UserId + "',MODIFY_DATE=sysdate where t_id = '" + putawayid + "' ";
            int rows;
            string msg = "";
            List<string> lstSql = new List<string>();
            lstSql.Add(DBHelper.ConvertSql(sql));            
            dbW.DBRunSql(user, lstSql, out rows, out msg);
            return rows > 0;
        }
        /// <summary>
        /// 更新上架单状态
        /// </summary>
        /// <param name="putawayid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdatePutawayStatus(string putawayid, PutawayStatus status)
        {

            string sql = @" update  &DataSource&wms_d_inbound_h set doc_status = @doc_status,
                                        modify_ip = @modify_ip,last_modified_by = @last_modified_by ,modify_date=sysdate 
                            where t_id = @t_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("doc_status", status.ToString());
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("t_id", putawayid);
            return DBHelper.RunSql(sql, lstParam, "更新上架单状态") > 0;
        }

        /// <summary>
        /// 保存手工调整后的货位分配信息
        /// </summary>
        /// <param name="modifiedList"></param>
        /// <returns></returns>
        public WebJsonInfo ModifyAllocateStorageBin(List<ModifyAllocLocationIn> modifiedList)
        {
            WebJsonInfo re = new WebJsonInfo();
            string[] allowModifiStorageStatus = new string[] { PutawayStatus.Putaway_Allocate.ToString(), PutawayStatus.Putaway_Create.ToString() };
            foreach (ModifyAllocLocationIn md in modifiedList)
            {
                PutawayHeaderModel putaway = GetPutawayHeader(md.inbound_id);
                if (!allowModifiStorageStatus.Contains(putaway.doc_status))
                {
                    throw new Exception("此上架单状态不允许再调整货位");
                }
                if ("update".Equals(md.actionType))
                {
                    ContainerComponent ctn = SaveContainerHeader(md);
                    WorkTask wt = new WorkTask();
                    if (!string.IsNullOrEmpty(md.task_id))
                    {
                        wt.DeleteTaskById(md.task_id);
                    }
                    if (!string.IsNullOrEmpty(md.task_item_id))
                    {
                        wt.DeleteTaskitemById(md.task_item_id);
                    }
                    TaskHeaderModel task = CreateTaskHeader(md, ctn.container);
                    TaskItemModel taskitem = CreateTaskItem(md, ctn.containerdetails[0], task);
                }
                else if (string.Equals("insert", md.actionType))
                {
                    ReduceContainerDetailInQty(md.container_d_id, md.task_item_id, md.task_plan_qty ?? 0);
                    ContainerComponent ctn = SaveContainerHeader(md);
                    TaskHeaderModel task = CreateTaskHeader(md, ctn.container);
                    TaskItemModel taskitem = CreateTaskItem(md, ctn.containerdetails[0], task);
                }
                else if ("delete".Equals(md.actionType))
                {
                    ReduceContainerDetailInQty(md.container_d_id, md.task_item_id, md.task_plan_qty ?? 0);
                    WorkTask wt = new WorkTask();
                    if (string.IsNullOrEmpty(md.task_id))
                    {
                        wt.DeleteWorkTask(md.task_id);
                    }
                    if (string.IsNullOrEmpty(md.task_item_id))
                    {
                        wt.DeleteWorkTaskItem(md.task_item_id);
                    }
                }
                UpdatePutawayStatus(putaway.t_id, PutawayStatus.Putaway_Allocate);
            }
            re.isSucceed = true;
            re.errmsg = null;
            re.errcode = 200;
            return re;
        }

        /// <summary>
        /// 获取上架单明细
        /// </summary>
        /// <param name="detailid"></param>
        /// <returns></returns>
        public PutawayDetailModel getPutawayDetail(string detailid)
        {
            string sql = @"select t_id, client_id, org_id, warehouse_id, inbound_id, line_no, item_id, item_code, item_name, lot_no, given_year, spec, production_date, exist_barcode, is_full, need_qc, qc_status, plan_qty, processed_qty, finished_qty, uom_qty_id, plan_stat_qty, processed_stat_qty, finished_stat_qty, uom_stat_id, packing_type, supplier_id, src_factory, src_warehouse, src_location, dest_factory, dest_warehouse, dest_location, memo, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                from &DataSource&wms_d_inbound_d
                where t_id=@t_id";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("t_id", detailid);
            DataTable dt = DBHelper.Select(sql, lstParams);
            PutawayDetailModel re = new PutawayDetailModel();
            List<Object> lstRe = WmsHelper.ConvertTableToObject(dt, "PutawayDetailModel", re);
            if (lstRe.Count > 0)
            {
                return lstRe[0] as PutawayDetailModel;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 获取上架单头信息
        /// </summary>
        /// <param name="putawayid">上架单ID</param>
        /// <returns>上架单头</returns>
        public PutawayHeaderModel GetPutawayHeader(string putawayid)
        {
            string sql = @"select t_id, client_id, org_id, warehouse_id, doc_no, doc_type, service_type, ref_doc_no, plan_no, bom_code, doc_status, source, start_date, finish_date, supplier_id, manufacturer_id, owner_id, src_factory, src_inventory, src_location, dest_factory, dest_inventory, dest_location, shipper, zone_id, dock_id, license_plate, deliver, deliver_tel, trans_permit, contract, net_weight, gross_weight, truck_weight, uom_weight_id, src_addr, dest_addr, left_task_count, is_auto_created, memo, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                            from &DataSource&wms_d_inbound_h
                            where t_id = @t_id";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("t_id", putawayid);
            DataTable dt = DBHelper.Select(sql, lstParams);
            PutawayHeaderModel re = new PutawayHeaderModel();
            List<PutawayHeaderModel> lstRe = DBHelper.ConvertTableToObject<PutawayHeaderModel>(dt);
            if (lstRe.Count > 0)
            {
                return lstRe[0] as PutawayHeaderModel;
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// 按上架单号查询上架单头信息
        /// </summary>
        /// <param name="putawayno"></param>
        /// <returns></returns>
        public PutawayHeaderModel GetPutawayHeaderByNo(string putawayno)
        {
            string sql = @"select t_id, client_id, org_id, warehouse_id, doc_no, doc_type, service_type, ref_doc_no, plan_no, bom_code, doc_status, source, start_date, finish_date, supplier_id, manufacturer_id, owner_id, src_factory, src_inventory, src_location, dest_factory, dest_inventory, dest_location, shipper, zone_id, dock_id, license_plate, deliver, deliver_tel, trans_permit, contract, net_weight, gross_weight, truck_weight, uom_weight_id, src_addr, dest_addr, left_task_count, is_auto_created, memo, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                            from &DataSource&wms_d_inbound_h
                            where doc_no = @doc_no";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("doc_no", putawayno);
            DataTable dt = DBHelper.Select(sql, lstParams);
            PutawayHeaderModel re = new PutawayHeaderModel();
            List<PutawayHeaderModel> lstRe = DBHelper.ConvertTableToObject<PutawayHeaderModel>(dt);
            if (lstRe.Count > 0)
            {
                return lstRe[0] as PutawayHeaderModel;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 调整货位后，修改相关批容器的带入量
        /// </summary>
        /// <param name="ma"></param>
        /// <returns></returns>
        public ContainerComponent SaveContainerHeader(ModifyAllocLocationIn ma)
        {

            Container ctn = new Container();

            bool isExists = ctn.CheckContainerExisByLocation(ma.dest_addr_text);
            string containerid = "", containerdetailid = "";
            ContainerDModel ctnd = new ContainerDModel();
            ContainerModel ctnHeader = new ContainerModel();
            List<ContainerDModel> lstCtnD = new List<ContainerDModel>();
            ContainerComponent re = new ContainerComponent();
            if (isExists)
            {
                #region 目标货位在容器中存在
                containerid = ctn.getContainerIdByLocation(ma.dest_addr_text);
                ctnHeader = ctn.getContainerById(containerid);
                ctn.UpdateContainerModifyInfo(containerid);
                if (string.Equals(containerid, ma.container_id))
                { //货位没变，只是修改了计划数量
                    /*containerdetailid = ma.container_d_id;
                    ContainerDModel containerD = ctn.getContainerDetailById(containerdetailid);
                    lstCtnD.Add(containerD);*/
                    //根据原来的任务计划数量-新的计划数量的差值来更新容器行的待入库数量
                    containerdetailid = ma.container_d_id;
                    WorkTask task = new WorkTask();
                    TaskItemModel taskitem = task.getTaskItem(ma.task_item_id);
                    ContainerDModel containerD = ctn.getContainerDetailById(containerdetailid);
                    containerD.in_qty += ma.task_plan_qty - taskitem.plan_qty;
                    ctn.AddInQty(containerdetailid, ma.task_plan_qty - taskitem.plan_qty);
                    lstCtnD.Add(containerD);
                }
                else
                {//修改了目标货位                    
                    #region 扣减原货位的待入量
                    // ReduceContainerDetailInQty(ma.container_d_id, ma.task_item_id,ma.task_plan_qty??0);
                    #endregion
                    #region 处理新货位的待入量
                    PutawayDetailModel pd = getPutawayDetail(ma.inbound_detail_id);
                    isExists = ctn.CheckContainerDetailExists(containerid, pd.item_id, ctn.GenerateWmsLot(ctnd));
                    if (isExists)
                    { //存在，则将本次待入库的熟练累加到现有容器行 
                        containerdetailid = ctn.getContainerDetailId(containerid, pd.item_id, ctnd.wms_lot_no);
                        ctnd = ctn.getContainerDetailById(containerdetailid);
                        ctn.AddInQty(containerdetailid, ma.task_plan_qty);
                        lstCtnD.Add(ctnd);
                    }
                    else
                    { //新增容器明细
                        ContainerDetailModel ctnDetail = new ContainerDetailModel();
                        ctnDetail.ContainerID = containerid;
                        ctnDetail.ActStatus = "1";
                        ctnDetail.ClientID = pd.client_id;
                        ctnDetail.ID = SnowflakeID.Guid().ToString();
                        ctnDetail.ItemID = pd.item_id;
                        ctnDetail.ItemName = pd.item_name;
                        ctnDetail.Qty = 0;
                        ctnDetail.InQty = ma.task_plan_qty;
                        ctnDetail.IsFull = pd.is_full;
                        ctnDetail.GivenYear = pd.given_year;
                        ctnDetail.LotNo = pd.lot_no;
                        ctnDetail.WmsLotNo = ctn.GenerateWmsLot(pd);
                        ctnDetail.CreatedBy = Context.UserInfo.UserName;
                        ctnDetail.CreateDate = DateTime.Now;
                        ctnDetail.ModifyDate = DateTime.Now;
                        ctn.Insert(ctnDetail);
                        ContainerDModel ctnD = new ContainerDModel();
                        ctnD.t_id = ctnDetail.ID;
                        ctnD.item_id = ctnDetail.ItemID;
                        ctnD.in_qty = ctnDetail.InQty;
                        lstCtnD.Add(ctnD);
                    }
                    #endregion
                }
                #endregion
            }//新货位在容器中已存在结束
            else
            { //新货位在容器中已存在结束  

                #region 容器不存在
                containerid = SnowflakeID.Guid().ToString();
                ContainerHeadModel ctnH = new ContainerHeadModel();
                PutawayDetailModel pd = getPutawayDetail(ma.inbound_detail_id);
                PutawayHeaderModel putawayh = GetPutawayHeader(ma.inbound_id);
                ctnH.ID = containerid;
                ctnH.ClientID = pd.client_id;
                ctnH.OrgID = pd.org_id;
                ctnH.WarehouseID = pd.warehouse_id;
                ctnH.ActStatus = "1";
                ctnH.LocationID = ctn.getLocationId(ma.dest_addr_text);
                ctnH.ContainerBarcode = ma.dest_addr_text;
                ctnH.ContainerType = "ContainerType_Virtual";
                ctnH.FirstInDate = DateTime.Now;
                ctnH.OwnerID = putawayh.owner_id;
                ctnH.CreatedBy = Context.UserInfo.UserName;
                ctnH.CreateDate = DateTime.Now;
                ctnH.ModifyDate = DateTime.Now;
                ctn.Insert(ctnH);
                ctnHeader.t_id = containerid;
                ctnHeader.location_id = ctnH.LocationID;
                ctnHeader.container_barcode = ctnH.ContainerBarcode;

                ContainerDetailModel ctnDetail = new ContainerDetailModel();
                ctnDetail.ContainerID = containerid;
                ctnDetail.ActStatus = "1";
                ctnDetail.ClientID = pd.client_id;
                ctnDetail.ID = SnowflakeID.Guid().ToString();
                ctnDetail.ItemID = pd.item_id;
                ctnDetail.ItemName = pd.item_name;
                ctnDetail.Qty = 0;
                ctnDetail.InQty = ma.task_plan_qty;
                ctnDetail.IsFull = pd.is_full;
                ctnDetail.GivenYear = pd.given_year;
                ctnDetail.LotNo = pd.lot_no;
                ctnDetail.WmsLotNo = ctn.GenerateWmsLot(pd);
                ctnDetail.CreatedBy = Context.UserInfo.UserName;
                ctnDetail.CreateDate = DateTime.Now;
                ctnDetail.ModifyDate = DateTime.Now;
                ctn.Insert(ctnDetail);

                ContainerDModel ctnD = new ContainerDModel();
                ctnD.t_id = ctnDetail.ID;
                ctnD.item_id = ctnDetail.ItemID;
                ctnD.in_qty = ctnDetail.InQty;
                lstCtnD.Add(ctnD);
                #endregion
            }//容器不存在结束

            re.container = ctnHeader;
            re.containerdetails = lstCtnD;
            return re;

        }
        /// <summary>
        /// 扣减库存行待入量
        /// <br/>当数量为0时删除库存行
        /// </summary>
        /// <param name="containerdetailid">库存行ID</param>
        /// <param name="taskitemid">对应的任务行ID</param>
        /// <returns></returns>
        private TaskItemModel ReduceContainerDetailInQty(string containerdetailid, string taskitemid, decimal reduceqty)
        {
            WorkTask task = new WorkTask();
            TaskItemModel taskitem = task.getTaskItem(taskitemid);
            if (taskitem != null)
            {
                Container ctn = new Container();
                ContainerDModel ctndetail = ctn.getContainerDetailById(containerdetailid);
                if ((ctndetail.qty == null ? 0 : ctndetail.qty) == 0 && ctndetail.in_qty == reduceqty)
                {
                    ctn.DeleteContainerD(containerdetailid);
                }
                else
                {
                    ctn.ReduceContainerQty(containerdetailid, reduceqty);
                }
            }
            return taskitem;

        }

        public ContainerDetailModel SaveContainerDetail(ModifyAllocLocationIn ma, ContainerModel cthH)
        {
            Container ctn = new Container();

            bool isExists = ctn.CheckContainerExisByID(cthH.t_id);
            string ctnDetailId = "";
            PutawayDetailModel putdetail = getPutawayDetail(ma.inbound_detail_id);
            ContainerDetailModel ctnDetail = new ContainerDetailModel();
            if (isExists)
            {

                isExists = ctn.CheckContainerDetailExists(cthH.t_id, putdetail.item_id, ctn.GenerateWmsLot(putdetail));
                if (isExists)
                { //存在，则将本次待入库的熟练累加到现有容器行
                    ctnDetailId = ctn.getContainerDetailId(cthH.t_id, putdetail.item_id, ctn.GenerateWmsLot(putdetail));
                    ContainerDModel ctndetail = ctn.getContainerDetailById(ctnDetailId);
                    ctnDetail.ID = ctndetail.t_id;
                    ctn.AddInQty(ctnDetailId, ma.task_plan_qty);
                }
                else
                { //新增容器明细 
                    ctnDetail.ContainerID = cthH.t_id;
                    ctnDetail.ActStatus = "1";
                    ctnDetail.ClientID = putdetail.client_id;
                    ctnDetail.ID = SnowflakeID.Guid().ToString();
                    ctnDetail.ItemID = putdetail.item_id;
                    ctnDetail.ItemName = putdetail.item_name;
                    ctnDetail.Qty = 0;
                    ctnDetail.InQty = ma.task_plan_qty;
                    ctnDetail.IsFull = putdetail.is_full;
                    ctnDetail.GivenYear = putdetail.given_year;
                    ctnDetail.LotNo = putdetail.lot_no;
                    ctnDetail.WmsLotNo = ctn.GenerateWmsLot(putdetail);
                    ctnDetail.CreatedBy = Context.UserInfo.UserName;
                    ctnDetail.CreateDate = DateTime.Now;
                    ctnDetail.ModifyDate = DateTime.Now;
                    ctn.Insert(ctnDetail);
                }
            }//容器已存在结束 
            return ctnDetail;
        }

        /// <summary>
        /// 创建新的任务头
        /// </summary>
        /// <param name="ma"></param>
        /// <param name="ctnH"></param>
        /// <returns></returns>
        public TaskHeaderModel CreateTaskHeader(ModifyAllocLocationIn ma, ContainerModel ctnH)
        {
            PutawayHeaderModel putheader = GetPutawayHeader(ma.inbound_id);
            PutawayDetailModel putdetail = getPutawayDetail(ma.inbound_detail_id);
            TaskHeaderModel task = new TaskHeaderModel();
            task.t_id = SnowflakeID.Guid().ToString();
            task.task_no = WmsHelper.GetSequence("TaskNo");
            task.client_id = putdetail.client_id;
            task.org_id = putdetail.org_id;
            task.warehouse_id = putdetail.warehouse_id;
            task.task_type = TaskType.TaskType_InBound.ToString();
            decimal line_no = 0;
            decimal.TryParse(putdetail.line_no, out line_no);
            task.seq_no = decimal.ToInt32(line_no);
            task.priority = 2;
            task.container_id = ctnH.t_id;
            task.container_barcode = ctnH.container_barcode;
            task.ref_doc_type = DocType.DocType_Inbound.ToString();
            task.ref_doc_id = putheader.t_id;
            task.ref_doc_no = putheader.doc_no;
            task.ref_doc_d_id = putdetail.t_id;
            task.dest_addr_id = ctnH.location_id;
            task.dest_addr_text = ma.dest_addr_text;
            task.dest_addr_type = AddressType.AddressType_Location.ToString();
            task.task_status = TaskStatus.TaskStatus_Create.ToString();
            task.is_executable = "0";
            task.created_by = Context.UserInfo.UserName;
            task.create_date = DateTime.Now;
            task.modify_date = DateTime.Now;
            task.act_status = "1";
            new WorkTask().AddTask(task);

            return task;
        }

        /// <summary>
        /// 创建新的任务明细
        /// </summary>
        /// <param name="ma"></param>
        /// <param name="ctnDetail"></param>
        /// <param name="taskH"></param>
        /// <returns></returns>
        public TaskItemModel CreateTaskItem(ModifyAllocLocationIn ma, ContainerDModel ctnDetail, TaskHeaderModel taskH)
        {
            PutawayHeaderModel putheader = GetPutawayHeader(ma.inbound_id);
            PutawayDetailModel putdetail = getPutawayDetail(ma.inbound_detail_id);
            WorkTask task = new WorkTask();

            TaskItemModel tskitem = new TaskItemModel();
            tskitem.t_id = SnowflakeID.Guid().ToString();
            tskitem.client_id = putdetail.client_id;
            tskitem.org_id = putdetail.org_id;
            tskitem.warehouse_id = putdetail.warehouse_id;
            tskitem.task_id = taskH.t_id;
            tskitem.item_id = putdetail.item_id;
            tskitem.item_name = putdetail.item_name;
            tskitem.plan_qty = ma.task_plan_qty;
            tskitem.uom_qty_id = putdetail.uom_qty_id;
            tskitem.uom_stat_id = putdetail.uom_stat_id;
            tskitem.container_d_id = ctnDetail.t_id;
            tskitem.ref_doc_type = DocType.DocType_Inbound.ToString();
            tskitem.ref_doc_id = putheader.t_id;
            tskitem.ref_doc_no = putheader.doc_no;
            tskitem.ref_doc_d_id = putdetail.t_id;
            tskitem.process_status = ProcessStatus.ProcessStatus_New.ToString();
            tskitem.created_by = Context.UserInfo.UserName;
            tskitem.create_date = DateTime.Now;
            tskitem.modify_date = DateTime.Now;
            tskitem.act_status = "1";
            task.AddTaskItem(tskitem);
            return tskitem;

        }

        /// <summary>
        /// 将上架单状态更新为已确认
        /// </summary>
        /// <param name="putawayid"></param>
        /// <returns></returns>
        public bool ConfirmAllocation(string putawayid)
        {
            string sql = @" update &DataSource&wms_d_inbound_h h set h.doc_status=@doc_status,
                                                    h.modify_ip=@modify_ip,
                                                    h.modify_date=sysdate,
                                                    h.last_modified_by=@last_modified_by
                             where h.t_id=@t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("doc_status", PutawayStatus.Putaway_Confirm.ToString());
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("t_id", putawayid);
            return DBHelper.RunSql(sql, lstParam) > 0;
        }
        /// <summary>
        /// 确认货位分配
        /// </summary>
        /// <param name="putawayid"></param>
        /// <returns></returns>
        public WebJsonInfo ConfirmAllocationLocation(string putawayid)
        {
            WebJsonInfo re = new WebJsonInfo();
            PutawayHeaderModel putaway = GetPutawayHeader(putawayid);
            if (putaway.doc_status != PutawayStatus.Putaway_Allocate.ToString())
            {
                throw new Exception("上架单当前状态不允许货位确认");
            }
            if (!CheckAllocateQtyEqual(putawayid)) {
                throw new Exception("已分配上架货位数量与上架单不一致，请修改");
            }
            ConfirmAllocation(putawayid);
            WorkTask task = new WorkTask();
            task.UpdatetaskAvailable(putawayid);
            re.isSucceed = true;
            re.errcode = 0;
            return re;
        }
        /// <summary>
        /// 检查上架单是否全部分配货位
        /// </summary>
        /// <param name="putawayid"></param>
        /// <returns></returns>
        private bool CheckAllocateQtyEqual(string putawayid)
        {
            string sql = @"select count(*) from &DataSource&wms_d_inbound_d ind left join  
                    (select i.ref_doc_d_id,sum(i.plan_qty) as plan_qty
                     from &DataSource&wms_b_task_item i where i.ref_doc_id=@inbound_id
                     group by i.ref_doc_d_id) ti on ind.t_id=ti.ref_doc_d_id
                 where ind.plan_qty!=nvl(ti.plan_qty,0) 
                       and ind.inbound_id=@inbound_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("inbound_id", putawayid);
            return int.Parse(DBHelper.SelectOne(sql, lstParam)) == 0;
        }

        /// <summary>
        /// 将上架单状态更新为已完成
        /// </summary>
        /// <param name="putawayid">上架单ID</param>
        /// <returns></returns>
        public bool UpdatePutawayCompleteStatus(string putawayid)
        {
            string sql = @" update  &DataSource&wms_d_inbound_h   set  start_date=nvl( start_date,sysdate),
                                                   finish_date=sysdate,
                                                   doc_status=@doc_status,
                                                   last_modified_by=@last_modified_by,
                                                   modify_ip=@modify_ip,
                                                   modify_date=sysdate
                         where  t_id = @t_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", putawayid);
            lstParam.Add("doc_status", PutawayStatus.Putaway_Done.ToString());
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            return DBHelper.RunSql(sql, lstParam, "完成上架单") > 0;
        }
        /// <summary>
        /// 将上架单状态更新为执行中
        /// </summary>
        /// <param name="putawayid"></param>
        /// <returns></returns>
        public bool UpdatePutawayStartStatus(string putawayid)
        {
            string sql = @" update  &DataSource&wms_d_inbound_h   set  start_date=nvl( start_date,sysdate), 
                                                   doc_status=@doc_status,
                                                   last_modified_by=@last_modified_by,
                                                   modify_ip=@modify_ip,
                                                   modify_date=sysdate
                         where  t_id = @t_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", putawayid);
            lstParam.Add("doc_status", PutawayStatus.Putaway_Doing.ToString());
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            return DBHelper.RunSql(sql, lstParam, "完成上架单") > 0;
        }
        /// <summary>
        /// 按上架单批量完成所有上架任务
        /// </summary>
        /// <param name="putawayid"></param>
        /// <returns></returns>
        public bool CompletePutaway(string putawayid)
        {

            PutawayHeaderModel pth = GetPutawayHeader(putawayid);
            string[] allowedConpleteStatus = new string[] { PutawayStatus.Putaway_Confirm.ToString(), PutawayStatus.Putaway_Doing.ToString() };
            if (!allowedConpleteStatus.Contains(pth.doc_status))
            {
                throw new Exception("此上架单状态不允许手工完成");
            }
            UpdatePutawayCompleteStatus(putawayid);
            WorkTask task = new WorkTask(this.dbW);
            List<TaskHeaderModel> lstTask = task.getUnCompleteTask(putawayid);
            WorkTask wtsk = new WorkTask();
            foreach (TaskHeaderModel tsk in lstTask)
            {
                wtsk.ConpleteTask_Inbound_Auto(tsk);
            }
            UpdatePutawayStatus(putawayid, PutawayStatus.Putaway_Done);
            return true;
        }
        /// <summary>
        /// 关闭上架单
        /// </summary>
        /// <param name="putawayid"></param>
        /// <returns></returns>
        public bool ClosePutaway(string putawayid)
        {

            PutawayHeaderModel pth = GetPutawayHeader(putawayid);
            string[] allowedConpleteStatus = new string[] { PutawayStatus.Putaway_Doing.ToString(), PutawayStatus.Putaway_Confirm.ToString() };
            if (!allowedConpleteStatus.Contains(pth.doc_status))
            {
                throw new Exception("此上架单状态不允许手工完成");
            }
            UpdatePutawayCompleteStatus(putawayid);
            WorkTask task = new WorkTask();
            List<TaskHeaderModel> lstTask = task.getUnCompleteTask(putawayid);
            WorkTask wtsk = new WorkTask();
            foreach (TaskHeaderModel tsk in lstTask)
            {
                wtsk.CloseTask_Inbound_Auto(tsk);
            }
            UpdatePutawayStatus(putawayid, PutawayStatus.Putaway_Done);
            return true;
        }
        /// <summary>
        /// 更新上架任行的已上架数量
        /// </summary>
        /// <param name="putawayDetailid">上架单明细ID</param>
        /// <param name="processQty">已处理数量</param>
        /// <returns></returns>
        public bool ProcessPutawaydetailQty(string putawayDetailid, decimal processQty)
        {

            string sql = @"update &DataSource&wms_d_inbound_d d set d.processed_qty=nvl(d.processed_qty,0)+@thisQty,
                                               d.finished_qty = nvl(d.finished_qty, 0) + @thisQty,
                                               d.processed_stat_qty = nvl(d.processed_stat_qty, 0) + @thisQty * (case when d.plan_qty = 0 then null else d.plan_stat_qty / d.plan_qty end),
                                               d.finished_stat_qty = nvl(d.finished_stat_qty, 0) + @thisQty * (case when d.plan_qty = 0 then null else d.plan_stat_qty / d.plan_qty end),
                                               d.modify_ip = @modify_ip,
                                               d.last_modified_by = @last_modified_by,
                                               d.modify_date = sysdate
                        where d.t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("thisQty", processQty.ToString());
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("t_id", putawayDetailid);
            return DBHelper.RunSql(sql, lstParam, "更新上架任务行的已上架数量") > 0;

        }
        /// <summary>
        /// 判断上架单是否已经完成的委托：有任务与无任务时判断逻辑不一致
        /// <br/>
        /// 有任务时，判断上架单对应的任务是否完成
        /// <br/>
        /// 无任务时，根据上库存
        /// </summary>
        /// <param name="putawayid"></param>
        /// <returns></returns>
        internal delegate bool CheckPutawayComplete(string putawayid);
        /// <summary>
        /// 上架任务行执行上架动作
        /// </summary>
        /// <param name="putawayToLocation"></param>
        /// <returns></returns>
        public WebJsonInfo DoPutawayToLocation(PutawayToLocation putawayToLocation, CheckPutawayComplete checkFeedback)
        {
            WebJsonInfo re = new WebJsonInfo();
            WorkTask task = new WorkTask();
            TaskItemModel taskitem = task.getTaskItem(putawayToLocation.TaskItemId);
            task.CompleteTaskItem_Inbound(taskitem, putawayToLocation.LstSN, putawayToLocation.InQty);
            ProcessPutawaydetailQty(taskitem.ref_doc_d_id, putawayToLocation.InQty);
            if (checkFeedback(taskitem.ref_doc_id))
            {
                UpdatePutawayCompleteStatus(taskitem.ref_doc_id);
                re.errmsg = "整单完成";
            }
            else
            {
                UpdatePutawayStartStatus(taskitem.ref_doc_id);
                re.errmsg = "整单未完成";
            }
            
            if (taskitem.process_status == ProcessStatus.ProcessStatus_Done.ToString())
            {
                task.MoveTaskItemToHistory(taskitem.task_id, taskitem.t_id);
            }
            PutawayDetailModel putD = getPutawayDetail(taskitem.ref_doc_d_id);
            
            if (putD.finished_qty >= putD.plan_qty) {
                re.json = new { putdetailIsComplete = true };
            }
            else
            {
                re.json = new { putdetailIsComplete = false };
            }
            re.isSucceed = true;
            return re;
        }
       
        /// <summary>
        /// 检查上架单是否全部完成：通过检查上架单对应任务是否全部完成来判断
        /// </summary>
        /// <param name="putawayid">上架单ID</param>
        /// <returns></returns>
        public bool CheckPutawayDone(string putawayid)
        {
            string sql = @"select count(1) num from &DataSource&wms_b_task t 
                            where t.ref_doc_id =  @ref_doc_id  
                            and t.task_status <> @task_status ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("ref_doc_id", putawayid);
            lstParam.Add("task_status", TaskStatus.TaskStatus_Finished.ToString());
            string re = DBHelper.SelectOne(sql, lstParam, "", false);
            return re == "0";
        }

        /// <summary>
        /// 检查上架单是否全部完成：同时已完成量来判断
        /// </summary>
        /// <param name="putawayid"></param>
        /// <returns></returns>
        public bool CheckPutawayDoneWithNoTask(string putawayid)
        {
            string sql = @"select count(1) as num from  &DataSource&wms_d_inbound_d d where d.inbound_id=@inbound_id and d.plan_qty>d.finished_qty";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("inbound_id", putawayid);
            return DBHelper.Select(sql, lstParam).Rows[0]["num"].ToString() == "0";
        }

        /// <summary>
        /// 获取上架单未完成的上架明细信息
        /// </summary>
        /// <param name="putawayno"></param>
        /// <returns></returns>
        public List<PutawayDetailModel> GetPutawayDetailForDoing(string putawayno)
        {
            PutawayHeaderModel putaway = GetPutawayHeaderByNo(putawayno);
            if (putaway == null)
            {
                throw new Exception("此上架单不存在");
            }
            string[] NotValidStatus = new string[] { PutawayStatus.Putaway_Allocate.ToString(), PutawayStatus.Putaway_Done.ToString() };
            if (NotValidStatus.Contains(putaway.act_status))
            {
                throw new Exception("此上架单状态不可直接上架");
            }
            string sql = @"select t_id, client_id, org_id, warehouse_id, inbound_id, line_no, item_id, item_code, item_name, lot_no, given_year, spec, production_date, exist_barcode, is_full, need_qc, qc_status, plan_qty, processed_qty, finished_qty, uom_qty_id, plan_stat_qty, processed_stat_qty, finished_stat_qty, uom_stat_id, packing_type, supplier_id, src_factory, src_warehouse, src_location, dest_factory, dest_warehouse, dest_location, memo, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                    from &DataSource&wms_d_inbound_d d 
                    where d.inbound_id = @inbound_id and d.plan_qty>nvl(d.finished_qty,0) and act_status='1'";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("inbound_id", putaway.t_id);
            DataTable dt = DBHelper.Select(sql, lstParam, "查询未完成的上架任务", false);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("此上架单已经完成上架");
            }
            return DBHelper.ConvertTableToObject<PutawayDetailModel>(dt);
        }

        /// <summary>
        /// 查询上架单明细
        /// </summary>
        /// <param name="putawayid"></param>
        /// <returns></returns>
        public List<PutawayDetailModel> GetPutawayDetailList(string putawayid)
        {
            string sql = @"select t_id, client_id, org_id, warehouse_id, inbound_id, line_no, item_id, item_code, item_name, lot_no, given_year, spec, production_date, exist_barcode, is_full, need_qc, qc_status, plan_qty, processed_qty, finished_qty, uom_qty_id, plan_stat_qty, processed_stat_qty, finished_stat_qty, uom_stat_id, packing_type, supplier_id, src_factory, src_warehouse, src_location, dest_factory, dest_warehouse, dest_location, memo, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                    from &DataSource&wms_d_inbound_d d 
                    where d.inbound_id = @inbound_id and act_status='1'";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("inbound_id", putawayid);
            DataTable dt = DBHelper.Select(sql, lstParam, "查询上架单明细", false);
            return DBHelper.ConvertTableToObject<PutawayDetailModel>(dt);
        }
        /// <summary>
        /// 根据上架单+物料查询待执行的上架单明细
        /// </summary>
        /// <param name="putawayno"></param>
        /// <returns></returns>
        public List<PutawayDetailModel> GetPutawayDetailForDoing(string putawayno, string itemcode)
        {
            PutawayHeaderModel putaway = GetPutawayHeaderByNo(putawayno);
            if (putaway == null)
            {
                throw new Exception("此上架单不存在");
            }
            string sql = @"select t_id, client_id, org_id, warehouse_id, inbound_id, line_no, item_id, item_code, item_name, lot_no, given_year, spec, production_date, exist_barcode, is_full, need_qc, qc_status, plan_qty, processed_qty, finished_qty, uom_qty_id, plan_stat_qty, processed_stat_qty, finished_stat_qty, uom_stat_id, packing_type, supplier_id, src_factory, src_warehouse, src_location, dest_factory, dest_warehouse, dest_location, memo, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                    from &DataSource&wms_d_inbound_d d where d.inbound_id = @inbound_id and item_code = @item_code  and d.plan_qty>nvl(d.finished_qty)";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("inbound_id", putaway.doc_no);
            lstParam.Add("item_code", itemcode);
            DataTable dt = DBHelper.Select(sql, lstParam, "查询未完成的上架任务", false);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("此上架单+物料已经完成上架");
            }
            return DBHelper.ConvertTableToObject<PutawayDetailModel>(dt);
        }
        /// <summary>
        /// 上架单直接上架提交
        /// </summary>
        /// <param name="putawaydoneinfo">上架执行信息</param>
        /// <returns></returns>
        public WebJsonInfo SubmitPutawayWithoutTask(PutawayDirectDoneIn putawaydoneinfo)
        {
            WebJsonInfo re = new WebJsonInfo();
            if (putawaydoneinfo.Qty <= 0)
            {
                throw new Exception("上架量必须大于0");
            }
            PutawayHeaderModel putaway = GetPutawayHeader(putawaydoneinfo.PutawayId);
            if (putaway == null)
            {
                throw new Exception("没找到上架单信息");
            }
            PutawayDetailModel putawayDetail = getPutawayDetail(putawaydoneinfo.PutawayDetailId);
            if (putawayDetail == null)
            {
                throw new Exception("没找到上架明细信息");
            }
            LocationModel location = new Location().GetLocationByCode(putaway.warehouse_id, putawaydoneinfo.ToLocation);
            if (location == null)
            {
                throw new Exception("没找到货位信息");
            }
            string[] allowInStatus = new string[] { LocationFlag.LocationFlag_InOnly.ToString(),
            LocationFlag.LocationFlag_Usable.ToString()};
            if (!allowInStatus.Contains(location.location_flag)) {
                throw new Exception("此货位不允许存放");
            }
            #region 更新库存容器信息

            ContainerDModel containerD = UpdateContainerInfo(putawaydoneinfo.ToLocation, putawaydoneinfo, putawayDetail, putaway);
            if (location.location_type == LocationType.LocationType_FlatStock.ToString())
            {
                //平库：更新货位状态为部分或已满
                int ctnInThisLocation = GetLocationContainerQty(location.t_id);
                if (ctnInThisLocation >= location.container_limit)
                {
                    new Location().UpdateLocationStatus(location.t_id, LocationStatus.LocationStatus_Full);
                }
                else
                {
                    new Location().UpdateLocationStatus(location.t_id, LocationStatus.LocationStatus_Partial);
                }
            }
            else
            {
                new Location().UpdateLocationStatus(location.t_id, LocationStatus.LocationStatus_Stored);
            }
            #endregion


            #region 产生上架任务 
            ContainerModel containerH = new Container().getContainerById(containerD.container_id);
            TaskItemModel taskitem = MakeNewTask(location, containerH, containerD, putaway, putawayDetail, putawaydoneinfo);
            #endregion

            #region 完成任务
            PutawayToLocation puttoinfo = new PutawayToLocation();
            puttoinfo.TaskId = taskitem.task_id;
            puttoinfo.TaskItemId = taskitem.t_id;
            puttoinfo.ToLocationCode = putawaydoneinfo.ToLocation;
            puttoinfo.InQty = putawaydoneinfo.Qty;
            puttoinfo.LstSN = new List<SNModel>();
            re = DoPutawayToLocation(puttoinfo, new CheckPutawayComplete(CheckPutawayDoneWithNoTask));

            #endregion
            
            return re;
        }

        public WebJsonInfo GetPalletInfoForIn(string containerno)
        {
            WebJsonInfo re = new WebJsonInfo();
            Container container = new Container();
            string containerid = container.GetContainerIdByContainerNo(containerno);
            if (string.IsNullOrEmpty(containerid))
            {
                throw new Exception("没有查询到此容器的待上架信息");
            }
            List<ContainerDModel> lstDetails = container.getContainerDetails(containerid);
            if (lstDetails.Count == 0)
            {
                throw new Exception("此托盘没有物料明细信息");
            }
            //string locationid=AllocateOneLocationForPallet(lstDetails[0].warehouse_id, lstDetails[0].item_id);
            //string locationcode = new Location().GetLocationById(locationid).location_code;
            string locationId = "";
            string locationcode = "";
            try
            {
                locationId = AllocateOneLocationForPallet2(lstDetails[0].warehouse_id, lstDetails[0].item_id);
                locationcode = new Location().GetLocationById(locationId).location_code;
            }
            catch (Exception ex)
            {
                locationcode = ex.Message;
            }
            re.isSucceed = true;
            re.json = new { ContainerNo = containerno, ContainerId = containerid, SuggestLocatio = locationcode, Details = lstDetails };

            return re;
        }
        /// <summary>
        /// 查询货位上的托盘数
        /// </summary>
        /// <param name="locatoinId"></param>
        /// <returns></returns>
        private int GetLocationContainerQty(string locatoinId) {
            string sql = @"select count(1) 
                    from &DataSource&wms_b_container_h h where h.location_id=@location_id and h.act_status=1";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("location_id", locatoinId);
            return int.Parse(DBHelper.SelectOne(sql, lstParam));
        }
        /// <summary>
        /// RF上架:容器,托盘RF上架到货位
        /// </summary>
        /// <param name="containerId">容器ID</param>
        /// <param name="toLocationCode">目标货位</param>
        /// <returns></returns>
        public WebJsonInfo PutawayPalletToLocation(string containerId, string toLocationCode)
        {
            WebJsonInfo re = new WebJsonInfo();


            if (string.IsNullOrEmpty(containerId))
            {
                throw new Exception("请扫描上架容器");
            }
            if (string.IsNullOrEmpty(toLocationCode))
            {
                throw new Exception("请扫描上架货位");
            }
            Container container = new Container();
            ContainerModel ctn = container.getContainerById(containerId);
            if (ctn == null)
            {
                throw new Exception("获取容器信息失败");
            }
            Location location = new Location();
            LocationModel LM = location.GetLocationByCode(ctn.warehouse_id, toLocationCode);
            if (LM == null)
            {
                throw new Exception("请扫描正确的上架货位");
            }
            string[] allowInStatus = new string[] { LocationFlag.LocationFlag_InOnly.ToString(),
            LocationFlag.LocationFlag_Usable.ToString()};
            if (!allowInStatus.Contains(LM.location_flag)) {
                throw new Exception("此货位不允许存放");
            }
            bool up = container.UpdateContainerLocation(containerId, LM.t_id);
            if (!up)
            {
                throw new Exception("更新容器货位失败");
            }
            if (LM.location_type == LocationType.LocationType_FlatStock.ToString()) {
                //平库：更新货位状态为部分或已满
                int ctnInThisLocation = GetLocationContainerQty(LM.t_id);
                if (ctnInThisLocation >= LM.container_limit)
                {
                    up = location.UpdateLocationStatus(LM.t_id, LocationStatus.LocationStatus_Full);
                }
                else {
                    up = location.UpdateLocationStatus(LM.t_id, LocationStatus.LocationStatus_Partial);
                }
            }
            else
            {
                up = location.UpdateLocationStatus(LM.t_id, LocationStatus.LocationStatus_Stored);
            }
            if (!up)
            {
                throw new Exception("更新货位状态失败");
            }
             
            #region 任务任务记录
            WorkTask task = new WorkTask();
            TaskHeaderModel taskHeader = new TaskHeaderModel();
            taskHeader.t_id = SnowflakeID.Guid().ToString();
            taskHeader.task_no = WmsHelper.GetSequence("TaskNo");
            taskHeader.task_type = TaskType.TaskType_InBound.ToString();
            taskHeader.warehouse_id = ctn.warehouse_id;
            taskHeader.start_date = DateTime.Now;
            taskHeader.actual_dest_addr_id = LM.t_id;
            taskHeader.actual_dest_addr_text = toLocationCode;
            taskHeader.act_status = "1";
            taskHeader.client_id = ctn.client_id;
            taskHeader.container_barcode = ctn.container_barcode;
            taskHeader.container_id = containerId;
            taskHeader.created_by = Context.UserInfo.UserName;
            taskHeader.end_date = DateTime.Now;
            taskHeader.executed_by = Context.UserInfo.UserName;
            taskHeader.is_executable = "0";
            taskHeader.last_modified_by = Context.UserInfo.UserName;
            taskHeader.modify_date = DateTime.Now;
            taskHeader.modify_ip = Context.UserInfo.LoginIP;
            taskHeader.org_id = ctn.org_id;
            taskHeader.zone_id = LM.zone_id;
            taskHeader.task_status = TaskStatus.TaskStatus_Finished.ToString();
            task.AddTask(taskHeader);
            List<ContainerDModel> lstDetails = container.getContainerDetails(containerId);

            string needPlusDocQty = WmsConfig.GetOrDefault("Inbound:NeedPlusFinishQty", "1");
            foreach (ContainerDModel ctnd in lstDetails)
            {
                TaskItemModel taskitem = new TaskItemModel();
                taskitem.act_status = "1";
                taskitem.warehouse_id = taskHeader.warehouse_id;
                taskitem.task_id = taskHeader.t_id;
                taskitem.client_id = ctnd.client_id;
                taskitem.container_d_id = ctnd.t_id;
                taskitem.created_by = Context.UserInfo.UserName;
                taskitem.create_date = DateTime.Now;
                taskitem.start_date = DateTime.Now;
                taskitem.end_date = DateTime.Now;
                taskitem.finished_qty = ctnd.qty;
                taskitem.finished_stat_qty = (ctnd.qty ?? 0) * (ctnd.ratio ?? 0);
                taskitem.item_id = ctnd.item_id;
                taskitem.item_name = ctnd.item_name;
                taskitem.last_modified_by = Context.UserInfo.UserName;
                taskitem.modify_date = DateTime.Now;
                taskitem.modify_ip = Context.UserInfo.LoginIP;
                taskitem.org_id = ctnd.org_id;
                taskitem.plan_qty = ctnd.qty;
                taskitem.processed_qty = ctnd.qty;
                taskitem.process_status = ProcessStatus.ProcessStatus_Done.ToString();
                taskitem.uom_qty_id = ctnd.uom_qty_id;
                taskitem.uom_stat_id = ctnd.uom_stat_id;
                task.AddTaskItem(taskitem);
                //是否需要更新收获单明细已完成量
                if ("1".Equals(needPlusDocQty))
                {
                    if (!string.IsNullOrEmpty(ctnd.inbound_h_id) && !string.IsNullOrEmpty(ctnd.inbound_d_id))
                    {
                        PlusInboudOrderCompleteQtyWhenIn(ctnd.inbound_h_id, ctnd.inbound_d_id);
                    }
                }

            }
            #endregion 
            task.MoveToHistory(taskHeader.t_id);


            re.isSucceed = true;

            return re;
        }


        /// <summary>
        /// 容器上架任务执行
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="toLocationCode">上架货位</param>
        /// <returns></returns>
        public WebJsonInfo PutawayPalletTaskToLocation(string taskId, string toLocationCode)
        {
            WebJsonInfo re = new WebJsonInfo();

            #region 必须参数检验
            if (string.IsNullOrEmpty(taskId))
            {
                throw new Exception("任务ID不能为空");
            }
            if (string.IsNullOrEmpty(toLocationCode))
            {
                throw new Exception("请扫描上架货位");
            }
            WorkTask task = new WorkTask();
            TaskHeaderModel taskHeader = task.GetTask(taskId);
            if (taskHeader == null)
            {
                throw new Exception("获取任务信息失败");
            }
            Container container = new Container();
            ContainerModel ctn = container.getContainerById(taskHeader.container_id);
            if (ctn == null)
            {
                throw new Exception("获取容器信息失败");
            }
            Location location = new Location();
            LocationModel LM = location.GetLocationByCode(ctn.warehouse_id, toLocationCode);
            if (LM == null)
            {
                throw new Exception("上架货位不存在");
            }
            #endregion

            bool up = container.UpdateContainerLocation(taskHeader.container_id, LM.t_id);
            if (!up)
            {
                throw new Exception("更新容器货位失败");
            }
            container.UpdateContainerSnInLocation(taskHeader.container_id, LM, taskHeader.task_no, "上架", "PutawayPalletTaskToLocation");
            container.UpdateContainerStaQty(taskHeader.container_id);            
            up = location.UpdateLocationStatus(LM.t_id, LocationStatus.LocationStatus_Stored);
            if (!up)
            {
                throw new Exception("更新货位状态失败");
            }

            #region 任务任务记录
            DateTime _dt = DateTime.Now;
            task.UpdateStartDate(taskHeader.t_id, _dt);
            task.UpdateFinishDate(taskHeader.t_id, _dt);
            container.UpdateDetailFirstInDate(taskHeader.container_id, _dt);//明细中的首次入库时间
            container.UpdateContainerHFirstInDate(taskHeader.container_id, _dt);//主档中的首次入库时间
            task.UpdateTaskStatus(taskHeader.t_id, TaskStatus.TaskStatus_Finished.ToString(), taskHeader.task_status);
            List<TaskItemModel> lstDetails = task.getTaskItemsForDoing(taskHeader.t_id);
            foreach (TaskItemModel taskItem in lstDetails)
            {
                task.UpdateTaskItemCompleteAndQty(taskItem.t_id);
            }
            #endregion

            task.MoveToHistory(taskId);


            re.isSucceed = true;

            return re;
        }

        /// <summary>
        /// 累加更新收货单已完成量和单据状态
        /// </summary>
        /// <param name="inboundid">收货单ID</param>
        /// <param name="inboundddetailid">收货明细ID</param>
        /// <returns></returns>        
        public bool PlusInboudOrderCompleteQtyWhenIn(string inboundid, string inboundddetailid) {
            string sql = @"update &DataSource&wms_d_inbound_d d set d.finished_qty=nvl(d.finished_qty,0)+1, 
                                                     d.finished_stat_qty=nvl(d.finished_stat_qty,0) + d.spec,
                                                     d.processed_qty=nvl(d.processed_qty,0) + 1,
                                                     d.processed_stat_qty=nvl(d.processed_stat_qty,0) + d.spec,
                                                     d.modify_date=sysdate
                        where d.inbound_id=@inbound_id and d.t_id=@t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("inbound_id", inboundid) ;
            lstParam.Add("t_id", inboundddetailid);
            bool up = DBHelper.RunSql(sql, lstParam) > 0;
            if (!up) {
                throw new Exception("更新收货单状态失败");
            }
            if (!UpdateInboundStatus(inboundid)) {
                throw new Exception("更新收货单状态失败");
            }
            return true;
        }
        /// <summary>
        /// 更新收货单状态为已完成或收货中
        /// </summary>
        /// <param name="inboundid"></param>
        /// <returns></returns>
        private bool UpdateInboundStatus(string inboundid) {
            string sql = @"select count(1) from &DataSource&wms_d_inbound_d d 
                    where nvl(d.finished_qty,0)<d.plan_qty 
                          and d.inbound_id=@inbound_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Clear();
            lstParam.Add("inbound_id", inboundid);
            int leftrows = int.Parse(DBHelper.SelectOne(sql, lstParam));
            bool re;
            if (leftrows > 0)
            { //还未完成收货
                re = UpdateInboundStarting(inboundid);
            }
            else
            { //已完成收货
                re = UpdateInboundFinish(inboundid);
            }
            if (!re)
            {
                throw new Exception("更新收货单状态失败");
            }
            return true;
        }
        /// <summary>
        /// 更新收货单状态为已完成
        /// </summary>
        /// <param name="inboundid"></param>
        /// <returns></returns>
        private bool UpdateInboundFinish(string inboundid) {
            string sql = @"update &DataSource&wms_d_inbound_h h set h.finish_date=sysdate,
                                                         h.doc_status=@doc_status
                            where h.t_id=@t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("doc_status", InDocStatus.InDocStatus_Finished.ToString());
            lstParam.Add("t_id", inboundid);
            return DBHelper.RunSql(sql, lstParam) > 0;
        }
        /// <summary>
        /// 更新收货单状态为收货中
        /// </summary>
        /// <param name="inboundid"></param>
        /// <returns></returns>
        private bool UpdateInboundStarting(string inboundid)
        {
            string sql = @"update &DataSource&wms_d_inbound_h h set h.start_date=nvl(h.start_date,sysdate),
                             h.doc_status=@doc_status
                          where h.t_id=@t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("doc_status", InDocStatus.InDocStatus_Executing.ToString());
            lstParam.Add("t_id", inboundid);
            return DBHelper.RunSql(sql, lstParam) > 0;
        }
        public ContainerModel ScanBarcodeIntoInv(string containerbarcode, string batcnno)
        {
            Container ctn = new Container();
            ContainerModel ctnh = ctn.GetContainer(Context.WarehouseID, containerbarcode);
            if (ctnh != null)
            {
                throw new Exception("此托盘已存在");
            }
            string itemcode = containerbarcode.Substring(19, 23);
            DataRow iteminfo = new NtiCloud.Service.WMS.Base.Item().Get(Context.WarehouseID, itemcode);
            if (iteminfo == null)
            {
                throw new Exception("没有此物料信息");
            }
            ctnh = new ContainerModel();
            ctnh.t_id = SnowflakeID.Guid().ToString();
            ctnh.client_id = Context.ClientID;
            ctnh.warehouse_id = Context.WarehouseID;
            ctnh.location_id = "1268783997165805569";
            ctnh.container_barcode = containerbarcode;
            ctnh.is_full = "1";
            ctnh.container_type = ContainerType.ContainerType_Actual.ToString();
            ctnh.allow_called = "0";
            ctnh.act_status = "1";
            ctnh.first_in_date = DateTime.Now;
            ctn.AddContainerH(ctnh);
            ContainerDModel ctnd = new ContainerDModel();
            ctnd.t_id = SnowflakeID.Guid().ToString();
            ctnd.client_id = ctnh.client_id;
            ctnd.warehouse_id = ctnh.warehouse_id;
            ctnd.container_id = ctnh.t_id;
            ctnd.item_id = iteminfo["t_id"].ToString();
            ctnd.item_name = iteminfo["item_name"].ToString();
            ctnd.lot_no = batcnno;
            ctnd.is_full = "1";
            ctnd.qc_status = QcStatus.QcStatus_CheckPassed.ToString();
            ctnd.qty = 1; ;
            ctnd.stat_qty = decimal.Parse(iteminfo["spec"].ToString());
            ctnd.spec = iteminfo["spec"].ToString();
            ctnd.uom_qty_id = iteminfo["default_pack_uom_id"].ToString();
            ctnd.uom_stat_id = iteminfo["default_stat_uom_id"].ToString();
            ctnd.first_in_date = DateTime.Now;
            ctnd.ratio = 1;
            ctnd.act_status = "1";
            ctn.AddContainerD(ctnd);
            return ctnh;
        }
    }
}
