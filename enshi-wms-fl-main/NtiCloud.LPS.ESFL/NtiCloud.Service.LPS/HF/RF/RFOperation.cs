using Microsoft.AspNetCore.Components.Forms;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Model.HF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NtiCloud.Service.WMS.HF.RF
{
    /// <summary>
    /// 发生此类异常时，继续继续提交事务
    /// </summary>
    public class CommitWhenException : Exception {
        public CommitWhenException(string msg):base(msg) {
             
        }
    }
    public class RFOperation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public  UserInfo Login(string account, string password)
        {
            NtiCloud.Model.WMS.UserInfo loginUser = null;
            UserInfo userInfo = doLogin(account, password); 
            if (userInfo != null)
            {
                Organization org =   GetOrginization(userInfo.OrgId);
                if (org != null)
                {
                    userInfo.OrgName = org.Name;
                }
            }

            return userInfo;
        }
        /// <summary>
        /// 获取待执行任务信息
        /// </summary>
        /// <param name="userinfo">当前用户</param>
        /// <param name="warehouseid">仓库ID</param>
        /// <param name="floor">楼层</param>
        /// <param name="room">仓间</param>
        /// <param name="ref_order_no">来源单号</param>
        /// <param name="needBarcode">是否下载条码</param>
        /// <returns></returns>
        public List<OutboundTaskList> getOutboundTaskList(UserInfo userinfo,string warehouseid,
                string floor,string room,string ref_order_no,bool needBarcode) {
            string sql = @"select * from root.hf_outbounddetail_rf_view v 
                        where 1=1";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(ref_order_no))
            {
                sql += " and v.ref_order_no=@ref_order_no";
                lstParams.Add("ref_order_no", ref_order_no);
            }
            if (!string.IsNullOrEmpty(userinfo.OrgId)) {
                sql += " and ORGID=@ORGID";
                lstParams.Add("ORGID", userinfo.OrgId);
            }
            if (!string.IsNullOrEmpty(userinfo.WarehouseCode))
            {
                sql += " and warehouseid in('"+userinfo.WarehouseCode.Replace(",","','")+"')"; 
            }
            if (!string.IsNullOrEmpty(warehouseid))
            {
                sql += " and warehouseid =@warehouseid";
                lstParams.Add("warehouseid", warehouseid);
            }
            if (!string.IsNullOrEmpty(floor))
            {
                sql += " and floor =@floor";
                lstParams.Add("floor", floor);
            }
            if (!string.IsNullOrEmpty(room))
            {
                sql += " and room =@room";
                lstParams.Add("room", room);
            }
            List< OutboundTaskList > lstRe= DBHelper.ConvertTableToObject<OutboundTaskList>(DBHelper.Select(sql, lstParams, "", false));
            if (needBarcode)
            {
                foreach (OutboundTaskList task in lstRe)
                {
                    List<OutbounddetailBarcodeOut> lstBarcode = GetoutboundTaskDetail(task.id);
                    task.lstBarcodes = lstBarcode;
                }
            }
            else {
                foreach (OutboundTaskList task in lstRe)
                { 
                    task.lstBarcodes = new List<OutbounddetailBarcodeOut>();
                }
            }
            return lstRe;
        }

        /// <summary>
        /// 获取待扫码任务的汇总信息
        /// </summary>
        /// <param name="userinfo"></param> 
        /// <param name="isLoosenTask">获取零散料任务还是整箱任务</param>
        /// <returns></returns>
        public List<OutboundTaskPlanList> GetOutboundTaskPlanList(UserInfo userinfo,bool isLoosenTask)
        {
            string sql = @" select h.order_no as plan_order_no,h.productname,h.batchnum,h.createdate   
                            from root.hf_t_outboundplan_header h 
                            where exists(select 1 from root.hf_t_outboundorder_header x where x.REF_ORDERNO=h.ORDER_NO
                                  and x.STATUS in('Order_Confirm','Order_Excuting','Order_Excuted','Order_Checked') 
                                         and nvl(x.ISSCANEDBARCODE,'否')='否' 
                                 ";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(userinfo.OrgId))
            {
                sql += " and x.ORGID= @ORGID";
                lstParams.Add("ORGID", userinfo.OrgId);
            }
            if (!string.IsNullOrEmpty(userinfo.WarehouseCode))
            {
                sql+= " and exists ( select 1 from root.hf_t_warehouse w where x.WAREHOUSEID=w.WAREHOUSEID " +
                    "               and w.WAREHOUSECODE in('" + userinfo.WarehouseCode.Replace(",", "','") + "'))";
               // sql += " and x.warehouseid in('" + userinfo.WarehouseCode.Replace(",", "','") + "')";
            }
            sql += @"  )  and h.CREATEDATE>sysdate-30  ";


            
            List<OutboundTaskPlanList> lstRe = DBHelper.ConvertTableToObject<OutboundTaskPlanList>(DBHelper.Select(sql, lstParams, "", false));
            #region 查询任务行对应的待扫描条码
            if (!isLoosenTask)
            {
                foreach (OutboundTaskPlanList taskplan in lstRe)
                {
                    List<OutboundTaskList> lstTask = getOutboundTaskList2(userinfo,
                        taskplan.plan_order_no, false);
                    taskplan.lstTaskList = lstTask;
                }
            }
            else {
                foreach (OutboundTaskPlanList taskplan in lstRe)
                {
                    List<OutboundTaskList> lstTask = getOutboundTaskList3(userinfo,
                        taskplan.plan_order_no,true);
                    taskplan.lstTaskList = lstTask;
                }                
            }
            #endregion

            #region 出库计划(备料单)对应的已扫描条码
            foreach (OutboundTaskPlanList taskplan in lstRe)
            { 
                taskplan.lstShippedBarcodeList = getShippedBarcodeList(taskplan.plan_order_no);
            }
            #endregion
            return lstRe;
        }

        /// <summary>
        /// 获取待合包任务明细
        /// </summary>
        /// <param name="userinfo"></param> 
        /// <param name="isLoosenTask">获取待合包任务明细</param>
        /// <returns></returns>
        public List<OutboundTaskPlanList> GetTasklistForCombine(UserInfo userinfo)
        {
            string sql = @" select h.order_no as plan_order_no,h.productname,h.batchnum,h.createdate   
                            from root.hf_t_outboundplan_header h 
                            where exists(select 1 from root.hf_t_outboundorder_header x where x.REF_ORDERNO=h.ORDER_NO
                                  and x.STATUS in('Order_Confirm','Order_Excuting','Order_Excuted','Order_Checked') 
                                         and nvl(x.ISSCANEDBARCODE,'否')='否' 
                                 ";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(userinfo.OrgId))
            {
                sql += " and x.ORGID= @ORGID";
                lstParams.Add("ORGID", userinfo.OrgId);
            }
            if (!string.IsNullOrEmpty(userinfo.WarehouseCode))
            {
                sql += " and exists(select 1 from root.hf_t_warehouse w where x.WAREHOUSEID=w.WAREHOUSEID and w.WAREHOUSECODE in('" + userinfo.WarehouseCode.Replace(",", "','") + "'))";
                //sql += " and x.warehouseid in('" + userinfo.WarehouseCode.Replace(",", "','") + "')";
            }
            sql += @"  )  and h.CREATEDATE>sysdate-30  ";



            List<OutboundTaskPlanList> lstRe = DBHelper.ConvertTableToObject<OutboundTaskPlanList>(DBHelper.Select(sql, lstParams, "", false));
            #region 查询任务行对应的待扫描条码
            foreach (OutboundTaskPlanList taskplan in lstRe)
            {
                List<OutboundTaskList> lstTask = getOutboundTaskList3(userinfo,
                    taskplan.plan_order_no, true);
                taskplan.lstTaskList = lstTask;
            }
            #endregion

            #region 出库计划(备料单)对应的已合箱的烟包(暂时不返回备料单对应的已合包清单)
            foreach (OutboundTaskPlanList taskplan in lstRe)
            {
                taskplan.lstShippedBarcodeList = new List<OutbounddetailBarcodeOut>(); //getShippedBarcodeList(taskplan.plan_order_no);
            }
            #endregion
            return lstRe;
        }

        /// <summary>
        /// 获取指定配料计划单号下面的待扫描任务
        /// <br/>
        /// 整包任务
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="plan_order_no">备料计划单号</param>
        /// <param name="needbarcode">是否需要同步条码信息</param>
        /// <returns></returns>
        private List<OutboundTaskList> getOutboundTaskList(UserInfo userinfo,
            string plan_order_no,bool needbarcode)
        {
            string sql = @"select * from root.HF_OUTBOUNDDETAIL_RF_2_VIEW v 
                        where 1=1";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(userinfo.OrgId))
            {
                sql += " and ORGID=@ORGID";
                lstParams.Add("ORGID", userinfo.OrgId);
            }
            if (!string.IsNullOrEmpty(userinfo.WarehouseCode))
            {
                sql += " and warehouseid in('" + userinfo.WarehouseCode.Replace(",", "','") + "')";
            }
            sql += " and ref_orderno=@ref_orderno";
            lstParams.Add("ref_orderno", plan_order_no );

            List<OutboundTaskList> lstRe = DBHelper.ConvertTableToObject<OutboundTaskList>(DBHelper.Select(sql, lstParams, "", false));
            foreach (OutboundTaskList task in lstRe)
            {
                if (needbarcode)
                {
                    List<OutbounddetailBarcodeOut> lstBarcode = GetoutboundTaskDetail(task.id);
                    task.lstBarcodes = lstBarcode;
                }
                else {
                    task.lstBarcodes = new List<OutbounddetailBarcodeOut>();
                }
            }
            return lstRe;
        }
        /// <summary>
        /// 获取整箱任务列表
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="plan_order_no"></param>
        /// <param name="needbarcode"></param>
        /// <returns></returns>
        private List<OutboundTaskList> getOutboundTaskList2(UserInfo userinfo,
          string plan_order_no, bool needbarcode)
        {
            string sql = @"select * from root.HF_OUTBOUNDDETAIL_RF_2_VIEW v 
                        where 1=1";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(userinfo.OrgId))
            {
                sql += " and ORGID=@ORGID";
                lstParams.Add("ORGID", userinfo.OrgId);
            }
            if (!string.IsNullOrEmpty(userinfo.WarehouseCode))
            {
                sql += " and warehousecode in('" + userinfo.WarehouseCode.Replace(",", "','") + "')";
            }
            sql += " and ref_orderno=@ref_orderno";
            lstParams.Add("ref_orderno", plan_order_no);

            List<OutboundTaskList> lstRe = DBHelper.ConvertTableToObject<OutboundTaskList>(DBHelper.Select(sql, lstParams, "", false));
            #region 处理出库任务对应的待扫描条码
            if (needbarcode)
            {
                foreach (OutboundTaskList task in lstRe)
                {
                    List<OutbounddetailBarcodeOut> lstBarcode = GetoutboundTaskDetail(task.id);
                    task.lstBarcodes = lstBarcode;
                }
            }
            else
            {
                foreach (OutboundTaskList task in lstRe)
                {
                    task.lstBarcodes = new List<OutbounddetailBarcodeOut>();
                }
            }
            #endregion
             
            return lstRe;
        }
        /// <summary>
        /// 获取备料单已出库扫描的条码列表
        /// </summary>
        /// <param name="ref_order_no"></param>
        /// <returns></returns>
        private List<OutbounddetailBarcodeOut> getShippedBarcodeList(string ref_order_no) {
            string sql = @"select sb.BARCODE as itembarcode
                        from root.hf_t_barcodeshipedrelation sb 
                        inner join root.hf_t_outboundorder_header h on sb.outorderno=h.order_no
                        where h.ref_orderno=@ref_orderno";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("ref_orderno", ref_order_no);
            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            return DBHelper.ConvertTableToObject<OutbounddetailBarcodeOut>(dt);
        }
        /// <summary>
        /// 获取散箱任务列表
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="plan_order_no"></param>
        /// <param name="needbarcode"></param>
        /// <returns></returns>
        private List<OutboundTaskList> getOutboundTaskList3(UserInfo userinfo,
          string plan_order_no, bool needbarcode)
        {
            string sql = @"select * from root.HF_OUTBOUNDDETAIL_RF_3_VIEW v 
                        where 1=1";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(userinfo.OrgId))
            {
                sql += " and ORGID=@ORGID";
                lstParams.Add("ORGID", userinfo.OrgId);
            }
            if (!string.IsNullOrEmpty(userinfo.WarehouseCode))
            {
                sql += " and warehousecode in('" + userinfo.WarehouseCode.Replace(",", "','") + "')";
            }
            sql += " and ref_orderno=@ref_orderno ";
            lstParams.Add("ref_orderno", plan_order_no);

            List<OutboundTaskList> lstRe = DBHelper.ConvertTableToObject<OutboundTaskList>(DBHelper.Select(sql, lstParams, "", false));
            if (needbarcode)
            {
                foreach (OutboundTaskList task in lstRe)
                {
                    List<OutbounddetailBarcodeOut> lstBarcode = GetoutboundTaskDetail(task.id);
                    task.lstBarcodes = lstBarcode;
                }
            }
            else
            {
                foreach (OutboundTaskList task in lstRe)
                {
                    task.lstBarcodes = new List<OutbounddetailBarcodeOut>();
                }
            }

            return lstRe;
        } 

        /// <summary>
        /// 获取用户权限的仓库列表
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<Warehouse> GetWarehouseList(UserInfo user) {
            string sql = @"SELECT   a.*,b.NAME AS orgidname
                            FROM      root.hf_t_warehouse a JOIN  root.hf_t_ORGANIZATION b   ON A.ORGID = B.ORGID
                            WHERE a.WARESTATE in('2', '3') 
                          "; 
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(user.OrgId)) {
                sql = sql + " and a.orgid= @orgid";
                lstParams.Add("orgid", user.OrgId); 
            }
            if (!string.IsNullOrEmpty(user.WarehouseCode))
            {
                sql += " and a.warehousecode in ('@warehousecode')";
                lstParams.Add("warehousecode", user.WarehouseCode);
            }
            sql += " order by a.warehousecode";
            return DBHelper.ConvertTableToObject<Warehouse>(DBHelper.Select(sql, lstParams, "", false));
        }
        /// <summary>
        /// 获取用户权限的货位信息
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public List<Location> getLocationList(UserInfo userinfo) {
            string sql = @"select a.LocationId,a.LocationName,a.LocationNumber,a.WarehouseID,a.House,a.Floor,a.Room,a.Local,a.OrgId,
                        b.name    as warehousename,
                        c.name    as orgname,
                        d.account as modifyusername
                    from hf_t_location a
                    inner join hf_t_warehouse b
                    on a.warehouseid = b.warehouseid and b.WARESTATE=3
                    inner join hf_t_organization c
                    on b.orgid = c.orgid
                    left join hf_t_userinfo d
                    on a.modifyuser = d.account
                    where 1=1 ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(userinfo.OrgId)) {
                lstParam.Add("orgid", userinfo.OrgId);
                sql += " and a.orgid=@orgid";
            }
            if (!string.IsNullOrEmpty(userinfo.WarehouseCode)) {
                sql += "  and a.house in('"+ userinfo .WarehouseCode.Replace(",","','")+ "')";
            }
            sql += " order by c.name,b.name,a.locationname";
            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            return DBHelper.ConvertTableToObject<Location>(dt);
        }

        /// <summary>
        /// 获取楼层信息
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public List<Floor> GetFloorByWarehouse(string warehouseId)
        {
            string sql = @"select  t.warehouseid, w.name as warehousename, t.floor  from 
                            (select distinct loc.warehouseid as warehouseid, loc.floor from hf_t_location loc
                             where warehouseid = @warehouseid )    t
                             left join hf_t_warehouse w on  t.warehouseid = w.warehouseid  ORDER BY   floor  ";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("warehouseid", warehouseId);
            return DBHelper.ConvertTableToObject<Floor>(DBHelper.Select(sql, lstParams, "", false));
        }
        /// <summary>
        /// 获取仓间信息
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="floor"></param>
        /// <returns></returns>
        public List<FloorRoom> GetRoomByFloor(string warehouseId, string floor)
        {
            string sql = "";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(floor))
            {
                  sql = @"  select  t.warehouseid, w.name as warehousename, t.floor ,t.room 
                        from  
                          ( select distinct loc.warehouseid as  warehouseid, loc.floor, loc.room  from hf_t_location loc  
                            where loc.status=0 and loc.warehouseid = @warehouseid  and loc.floor=@floor 
                            )  t  
                        left join  hf_t_warehouse w on  t.warehouseid= w.warehouseid     ORDER BY   room ";
                lstParams.Add("warehouseid", warehouseId);
                lstParams.Add("floor", floor);
            }
            else {
                  sql = @"  select  t.warehouseid, w.name as warehousename, t.floor ,t.room 
                        from  
                          ( select distinct loc.warehouseid as  warehouseid, loc.floor, loc.room  from hf_t_location loc  
                            where loc.status=0 and loc.warehouseid = @warehouseid 
                            )  t  
                        left join  hf_t_warehouse w on  t.warehouseid= w.warehouseid     ORDER BY   room ";
                lstParams.Add("warehouseid", warehouseId);
            }

            return DBHelper.ConvertTableToObject<FloorRoom>(DBHelper.Select(sql, lstParams, "", false));
        }

        /// <summary>
        /// 更新出库任务信息
        /// </summary> 
        /// <param name="outorderid">出库明细ID</param>
        /// <param name="wgt"></param>
        /// <param name="itemBarcodes"></param>
        /// <param name="specis"></param>
        /// <returns></returns>
        public bool UpOutTaskInfoNew(SubmitOutboudDetailIn pickedInfo)
        {
            string sql = @"select t.CREATEDATE,t.warehouseid 
                            from  root.hf_t_outboundorder_header t 
                            inner join root.hf_t_outboundorder_detail d on t.order_no=d.order_no
                            where d.id=@outorderid";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("outorderid", pickedInfo.OutboundOrderDetailId);
            DataTable dt = DBHelper.Select(sql, lstParams, "", false);
            
            if (!new LocationAllocate().IsOverMonth(dt.Rows[0]["warehouseid"].ToString(), DateTime.Parse(dt.Rows[0]["CREATEDATE"].ToString())))
            {
                throw new Exception("出库日期只能为当前月份，不能跨月！"); 
            }
            return UpdateOutTaskInfo(pickedInfo);
        }

        public System.Collections.ArrayList UpOutTaskInfoNew2(SubmitOutboudDetailIn pickedInfo)
        {
            System.Collections.ArrayList lstRe = new System.Collections.ArrayList();
            string sql = @"select t.CREATEDATE,t.warehouseid 
                            from  root.hf_t_outboundorder_header t 
                            inner join root.hf_t_outboundorder_detail d on t.order_no=d.order_no
                            where d.id=@outorderid";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("outorderid", pickedInfo.OutboundOrderDetailId);
            DataTable dt = DBHelper.Select(sql, lstParams, "", false);

            if (!new LocationAllocate().IsOverMonth(dt.Rows[0]["warehouseid"].ToString(), DateTime.Parse(dt.Rows[0]["CREATEDATE"].ToString())))
            {
                throw new Exception("出库日期只能为当前月份，不能跨月！");
            }
            SubmitOutboudDetailIn onebc = null;
            List<PickedBarcode> lstOnebarcode = new List<PickedBarcode>();
            foreach (var bar in pickedInfo.lstDetails) {
                onebc = new SubmitOutboudDetailIn();
                lstOnebarcode.Clear();
                onebc.OutboundOrderDetailId = pickedInfo.OutboundOrderDetailId;
                onebc.TotalGW = bar.Specis;
                lstOnebarcode.Add(new PickedBarcode() { Barcode = bar.Barcode, Specis = bar.Specis });
                onebc.lstDetails = lstOnebarcode;
                try
                {
                    bool re = UpdateOutTaskInfo(onebc);
                    lstRe.Add(new { Barcode=bar.Barcode,msg="" });
                }
                catch (Exception ex) {
                    lstRe.Add(new { Barcode = bar.Barcode, msg = ex.Message });
                }
            }
            return lstRe;
        }

        /// <summary>
        /// 根据出库明细ID查找对应的来源单号
        /// </summary>
        /// <param name="outbounddetailid"></param>
        /// <returns></returns>
        private string GetRefOrderNo(string outbounddetailid) {
            string sql = @"select h.REF_ORDERNO 
                        from hf_t_outboundorder_detail d inner join hf_t_outboundorder_header h on d.ORDER_NO=h.ORDER_NO
                        where d.id = @id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("id", outbounddetailid);
            return DBHelper.SelectOne(sql, lstParam);
        }
        /// <summary>
        /// 重新分配出库明细ID
        /// </summary>
        /// <param name="reforderno"></param>
        /// <param name="itemcode"></param>
        /// <returns></returns>
        private DataTable AllocatDetailRow(string reforderno, string itemcode) {
            string sql = @"select v.id,v.spec from HF_OUTBOUNDDETAIL_RF_2_VIEW v 
                            where v.REF_ORDERNO=@REF_ORDERNO
                                  and v.itemcode=@itemcode
                                  and v.actualqty>nvl(v.SCANEDQTY,0)";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("REF_ORDERNO", reforderno);
            lstParams.Add("itemcode", itemcode);
            return DBHelper.Select(sql, lstParams, "", false);
        }
        public SubmitOutboudDetailIn AllocateBarcodeToOutboundDetailId(string orgdetailid, string barcode) {
            string itemcode = barcode.Substring(19, 22);
            string reforderno = GetRefOrderNo(orgdetailid);
            DataTable dt = AllocatDetailRow(reforderno, itemcode);
            if (dt != null && dt.Rows.Count > 0)
            {
                SubmitOutboudDetailIn newSubmitInfo = new SubmitOutboudDetailIn();
                newSubmitInfo.OutboundOrderDetailId = dt.Rows[0]["id"].ToString();
                newSubmitInfo.TotalGW = decimal.Parse(dt.Rows[0]["spec"].ToString());
                List<PickedBarcode> lstbarcode = new List<PickedBarcode>();
                lstbarcode.Add(new PickedBarcode { Barcode = barcode, Specis = decimal.Parse(dt.Rows[0]["spec"].ToString()) });
                newSubmitInfo.lstDetails = lstbarcode;
                return newSubmitInfo;
            }
            else {
                return null;
            }
            

        }
        /// <summary>
        /// 获取任务明细行对应的可用条码列表
        /// </summary>
        /// <param name="outorderdetailId"></param>
        /// <returns></returns>
        public List<OutbounddetailBarcodeOut> GetoutboundTaskDetail(string outboundOrderdetailId) {
            string sql = @"SELECT DISTINCT a.id,d.itembarcode, d.weight
                            FROM hf_t_outboundorder_detail a
                            JOIN hf_t_location b ON a.loactionid = b.locationid
                            JOIN hf_t_containerinfo c ON b.containerid = c.containerid
                            JOIN hf_t_barcode d
                            ON     c.containerid = d.containerid  /*  AND a.specifications_kg = d.speci*/
                            AND a.itemid = d.itemid
                            AND d.state = 1
                            WHERE a.id = @id";
            //增加在此出库单生成的条码信息
            sql = @"SELECT DISTINCT a.id,d.itembarcode, d.weight
                            FROM hf_t_outboundorder_detail a
                            JOIN hf_t_location b ON a.loactionid = b.locationid
                            JOIN hf_t_containerinfo c ON b.containerid = c.containerid
                            JOIN hf_t_barcode d
                            ON     c.containerid = d.containerid  /*  AND a.specifications_kg = d.speci*/
                            AND a.itemid = d.itemid
                            AND d.state = 1
                            WHERE a.id = @id
                        union
                        SELECT DISTINCT a.id,d.itembarcode, d.weight
                                                    FROM hf_t_outboundorder_detail a
                                                    JOIN hf_t_barcodeprintbyorder b ON a.ORDER_NO = b.ORDERNO and a.ID=b.ORDERDETAILID 
                                                    JOIN hf_t_barcode d
                                                    ON     b.BARCODE = d.ITEMBARCODE  /*  AND a.specifications_kg = d.speci*/
                                                    AND a.itemid = d.itemid
                                                    AND d.state = -1
                                                    WHERE a.id = @id";

            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("id", outboundOrderdetailId);

            return DBHelper.ConvertTableToObject<OutbounddetailBarcodeOut>(DBHelper.Select(sql,lstParams,"",false));
        }
        /// <summary>
        /// 获取任务行对应的可用条码信息
        /// </summary>
        /// <param name="outboundOrderdetailId"></param>
        /// <returns></returns>
        public List<OutbounddetailBarcodeOut> GetoutboundTaskDetail2(string outboundOrderdetailId)
        { 
            string sql = @" SELECT DISTINCT a.id,d.itembarcode, d.weight
                            FROM hf_t_outboundorder_detail a
                            JOIN hf_t_barcodeprintbyorder b ON a.ORDER_NO = b.ORDERNO and a.ID=b.ORDERDETAILID 
                            JOIN hf_t_barcode d
                            ON  b.BARCODE = d.ITEMBARCODE 
                                AND a.itemid = d.itemid
                                AND d.state = -1
                            WHERE    a.id = @id";

            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("id", outboundOrderdetailId);

            return DBHelper.ConvertTableToObject<OutbounddetailBarcodeOut>(DBHelper.Select(sql, lstParams, "", false));
        }

        /// <summary>
        /// 获取出库单有无条码标识
        /// </summary>
        /// <param name="truckorderdetailid"></param>
        /// <returns></returns>
        private string GetTruckOrderIsbarcode(string truckorderdetailid) {
            string sql = @"select h.ISBARCODE 
                                    from root.hf_t_outboundorder_header h 
                                    inner join root.hf_t_outboundorder_detail d on h.ORDER_NO=d.ORDER_NO
                                    where d.ID=@id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("id", truckorderdetailid);
            return DBHelper.SelectOne(sql, lstParam);
        }

        /// <summary>
        /// 醇化库扫码提交
        /// <br/>
        /// 更新条码状态为出库&更新单据状态
        /// </summary>
        /// <param name="pickedInfo"></param>
        /// <returns></returns>
        private bool UpdateOutTaskInfo(SubmitOutboudDetailIn pickedInfo)
        {

            string isbarcode=GetTruckOrderIsbarcode(pickedInfo.OutboundOrderDetailId);
            //存储过程
            string msg = string.Empty;
            string insql = "ahzy_outbound_pg.insert_barcodedetail_hf@HFCHK_LINK";
            string upsql_ym = "ahzy_outbound_pg.up_outbound_task2_hfym@HFCHK_LINK";//有码出库
            string upsql_wm = "ahzy_outbound_pg.up_outbound_task2_hfwm@HFCHK_LINK";//无码出库
            string exeid = Guid.NewGuid().ToString("N").ToUpper();

            if (isbarcode == "0")//有码:先更新单据在处理条码
            {
                #region 扣减库存，更新单据状态
                List<Tuple<string, string, ParameterDirection>> spParams = new List<Tuple<string, string, ParameterDirection>>();
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_orgid", "", ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_id", pickedInfo.OutboundOrderDetailId, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_wgt", pickedInfo.TotalGW.ToString(), ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_userid", Context.UserInfo.UserId, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_msg", "", ParameterDirection.Output));

                var outputs = DBHelper.RunProc(upsql_ym, spParams, "提交出库任务", DBHelper.OperatedNumType.ZeroOrMore);
                msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                                 .FirstOrDefault()?.Value.ToString();
                if (msg != "Y")
                {
                    throw new Exception(msg);
                }
                #endregion

                #region 更新条码状态
                int i = 0;
                foreach (PickedBarcode bar in pickedInfo.lstDetails)
                {
                    spParams.Clear();
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_orgid", "", ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_id", pickedInfo.OutboundOrderDetailId, ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_barcode", bar.Barcode, ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_speci", bar.Specis.ToString(), ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_userid", Context.UserInfo.UserId, ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_msg", "", ParameterDirection.Output));
                    outputs = DBHelper.RunProc(insql, spParams, "提交出库任务", DBHelper.OperatedNumType.ZeroOrMore);
                    msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                                     .FirstOrDefault()?.Value.ToString();
                    if (msg != "Y")
                    {
                        throw new Exception(msg);
                    }
                }
                #endregion
            }
            else { //无码：先处理条码状态再更新是否扫描标识

                List<Tuple<string, string, ParameterDirection>> spParams = new List<Tuple<string, string, ParameterDirection>>();
                #region 更新条码状态
                int i = 0;
                foreach (PickedBarcode bar in pickedInfo.lstDetails)
                {
                    spParams.Clear();
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_orgid", "", ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_id", pickedInfo.OutboundOrderDetailId, ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_barcode", bar.Barcode, ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_speci", bar.Specis.ToString(), ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_userid", Context.UserInfo.UserId, ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_msg", "", ParameterDirection.Output));
                    var outputs2 = DBHelper.RunProc(insql, spParams, "提交出库任务", DBHelper.OperatedNumType.ZeroOrMore);
                    msg = outputs2.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                                     .FirstOrDefault()?.Value.ToString();
                    if (msg != "Y")
                    {
                        throw new Exception(msg);
                    }
                }
                #endregion


                #region 更新单据状态
                spParams.Clear();
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_orgid", "", ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_id", pickedInfo.OutboundOrderDetailId, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_wgt", pickedInfo.TotalGW.ToString(), ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_userid", Context.UserInfo.UserId, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_msg", "", ParameterDirection.Output));

                var outputs = DBHelper.RunProc(upsql_wm, spParams, "提交出库任务", DBHelper.OperatedNumType.ZeroOrMore);
                msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                                 .FirstOrDefault()?.Value.ToString();
                if (msg != "Y")
                {
                    throw new Exception(msg);
                }
                #endregion

            }

            return true;
        }
        /// <summary>
        /// 获取基础数据
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public List<SysType> getSysType(string flag) {
            string sql = @"select  Code,Description,Flag,SequenceNumber,DefaultDescription  
                            FROM root.hf_systype  
                            where FLAG= @FLAG
                            order by TO_NUMBER (SEQUENCENUMBER) ";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("FLAG", flag);
            return DBHelper.ConvertTableToObject<SysType>(DBHelper.Select(sql, lstParams, "", false));
        }

        /// <summary>
        /// 获取组织信息
        /// </summary>
        /// <param name="orgid"></param>
        /// <returns></returns>
        private Organization GetOrginization(string orgid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ORGID, CODE, NAME, CREATETIME, REMARK  ");
            strSql.Append("  from root.hf_t_organization ");
            strSql.Append(" where ORGID= @ORGID");
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("ORGID", orgid);

            DataTable dt = DBHelper.Select(strSql.ToString(), lstParams,"",false);

            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<Organization>(dt)[0];
            }
            else
            {
                return null;
            }
        }
        //登录
        private UserInfo doLogin(string account, string password)
        {
            account = account.Trim();
            UserInfo model = getUserInfo(account); 
            if (model == null)
            {
                throw new Exception("用户“" + account + "”不存在！" );
            }
            if (model.Status == 0)
            {
                throw new Exception("用户“" + account + "”已经被删除了！"); 
            }
            if (!model.Password.Equals(Class_MD5.StringToMD5(password)))
            {
                model.ErrorTimes = model.ErrorTimes.ToString() == "" ? 1 : model.ErrorTimes + 1;
                UpdateUserInfo(model);
               throw new CommitWhenException( "密码输入错误,请核对密码！" );
            }
            model.ErrorTimes = 0;
            UpdateUserInfo(model);
            return model;
        }
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool UpdateUserInfo(UserInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update root.hf_t_userinfo set ");  
            strSql.Append(" EMAIL = @EMAIL , ");
            strSql.Append(" REMARK = @REMARK , ");
            strSql.Append(" CREATEDATE = sysdate , ");
            strSql.Append(" STATUS = @STATUS , ");
            strSql.Append(" ORGID = @ORGID , ");
            strSql.Append(" WAREHOUSECODE = @WAREHOUSECODE , ");
            strSql.Append(" ACCOUNT = @ACCOUNT , ");
            strSql.Append(" PASSWORD = @PASSWORD , ");
            strSql.Append(" CHINESENAME = @CHINESENAME , ");
            strSql.Append(" ENGLISHNAME = @ENGLISHNAME , ");
            strSql.Append(" SEX = @SEX , ");
            strSql.Append(" BIRTHDAY = to_date(@BIRTHDAY,'yyyy-MM-dd') , ");
            strSql.Append(" MOBILE = @MOBILE , ");
            strSql.Append(" TEL = @TEL,  ");
            strSql.Append(" EnterpriseID = @EnterpriseID,  ");
            strSql.Append(" ERRORTIMES = @ERRORTIMES  ");
            strSql.Append(" where ID=@ID  ");

            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("ID", model.ID);
            lstParams.Add("Email", model.Email);
            lstParams.Add("REMARK", model.Remark);
            lstParams.Add("STATUS", model.Status.ToString());
            lstParams.Add("ORGID", model.OrgId);
            lstParams.Add("WAREHOUSECODE", model.WarehouseCode);
            lstParams.Add("ACCOUNT", model.Account);
            lstParams.Add("PASSWORD", model.Password);
            lstParams.Add("CHINESENAME", model.ChineseName);
            lstParams.Add("ENGLISHNAME", model.EnglishName);
            lstParams.Add("SEX", model.Sex);
            lstParams.Add("BIRTHDAY", model.Birthday.ToString("yyyy-MM-dd"));
            lstParams.Add("MOBILE", model.Mobile);
            lstParams.Add("TEL", model.Tel);
            lstParams.Add("EnterpriseID", model.EnterpriseID);
            lstParams.Add("ERRORTIMES", model.ErrorTimes.ToString());  
            
            if (DBHelper.RunSql(strSql.ToString(),lstParams) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private UserInfo getUserInfo(string account) {
          
                string sql = "select id from root.hf_t_userinfo where account = @account  and rownum<=1 ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("account", account);

            string userid = DBHelper.SelectOne(sql, lstParam, "", false) ;
            if (!string.IsNullOrEmpty(userid))
            {
                return GetModel(userid);
            }
            else
            {
                return null;
            }
            
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public UserInfo GetModel(string ID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID, EMAIL, REMARK, CREATEDATE, STATUS, ORGID, WAREHOUSECODE, ACCOUNT, PASSWORD, CHINESENAME, ENGLISHNAME, SEX, BIRTHDAY, MOBILE, TEL,EnterpriseID,ERRORTIMES  ");
            strSql.Append("  from root.hf_t_userinfo ");
            strSql.Append(" where ID= @id");
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("id", ID); 
            UserInfo model = new UserInfo();
            DataTable dt = DBHelper.Select(strSql.ToString(), lstParam);
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<UserInfo>(dt)[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取单个已出库的条码信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns>有可能返回空值</returns>
        public BarcodeInfo GetShippedBarcode(string barcode) {
            string sql = @"select bd.itembarcode,od.itemname,bd.weight,oh.order_no,od.ID orderdetailid,
                            oh.ref_orderno
                            from root.hf_t_outboundorder_detail od 
                            inner join root.hf_t_outboundorder_header oh on od.order_no=oh.order_no
                            inner join root.hf_t_barcodedetail bd on od.id=bd.sourcebillid 
                            where   bd.itembarcode= @itembarcode ";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("itembarcode", barcode);
            DataTable dt = DBHelper.Select(sql, lstParams,"",false);
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<BarcodeInfo>(dt)[0];
            }
            else { 
                return null;
            }
        }

        public bool CombineShippedBarcode(MergebarcodeIn mergeBarcode) {
            #region 检查是否可以合包
            if (string.IsNullOrEmpty(mergeBarcode.NewBarcode)) {
                throw new Exception("请扫描新条码");
            }
            if (mergeBarcode.OldBarcodes == null || mergeBarcode.OldBarcodes.Count == 0) {
                throw new Exception("请扫描被合并的条码");
            }
            if (!checkNewBarcodeValid(mergeBarcode.NewBarcode)) {
                throw new Exception("此新条码已经使用，不可以再用");
            }
            string orderno = string.Empty;
            BarcodeInfo barcodeinfo;
            foreach (string bar in mergeBarcode.OldBarcodes) {
                barcodeinfo= GetShippedBarcode(bar);
                if (barcodeinfo == null)
                {
                    throw new Exception("条码：" + bar + "没有出库扫描记录，不可以合包");
                }
                else {
                    if (string.IsNullOrEmpty(orderno))
                    {
                        orderno = barcodeinfo.order_no;
                    }
                    else if (!string.Equals(orderno, barcodeinfo.order_no)) {
                        throw new Exception("只有同一个出库单的条码不可以合包");
                    }
                }
            }
            #endregion

            string sql = @"insert into root.hf_t_barcodecombined
                              (id, itembarcode, weight, sourcebillid, orderno, remark, newitembarcode, createby, createdate,combinbatchno)
                            values
                              (@id, @itembarcode, @weight, @sourcebillid, @orderno, @remark, @newitembarcode, @createby, sysdate,@combinbatchno)";
            Dictionary<string,object>lstParams=new Dictionary<string, object>();
            bool re = true;
            foreach (string bar in mergeBarcode.OldBarcodes) {
                lstParams.Clear();
                BarcodeInfo barinfo = GetShippedBarcode(bar);
                lstParams.Add("id", SnowflakeID.Guid().ToString());
                lstParams.Add("itembarcode", bar);
                lstParams.Add("weight", barinfo.weight);
                lstParams.Add("sourcebillid", barinfo.orderdetailid);
                lstParams.Add("orderno", barinfo.order_no);
                lstParams.Add("remark", "");
                lstParams.Add("newitembarcode", mergeBarcode.NewBarcode);
                lstParams.Add("createby", Context.UserInfo.UserName);
                lstParams.Add("combinbatchno", SnowflakeID.Guid().ToString());
                re = DBHelper.RunSql(sql, lstParams, "保存合记录")==1;
            }
            return re;
        }
        /// <summary>
        /// 出库烟包条码合并
        /// <br/>
        /// 不需要扫描新条码，只提交合包记录
        /// </summary>
        /// <param name="mergeBarcode"></param>
        /// <returns></returns>
        public bool CombineShippedBarcodeNew(MergebarcodeIn mergeBarcode)
        {
            #region 检查是否可以合包
             
            if (mergeBarcode.OldBarcodes == null 
                || mergeBarcode.OldBarcodes.Count ==0)
            {
                throw new Exception("请扫描被合并的条码");
            }
            
            string orderno = string.Empty;
            BarcodeInfo barcodeinfo;
            foreach (string bar in mergeBarcode.OldBarcodes)
            {
                barcodeinfo = GetShippedBarcode(bar);
                if (barcodeinfo == null)
                {
                    throw new Exception("条码：" + bar + "没有出库扫描记录，不可以合包");
                }
                else
                {
                    if (string.IsNullOrEmpty(orderno))
                    {
                        orderno = barcodeinfo.order_no;
                    }
                    else if (!string.Equals(orderno, barcodeinfo.order_no))
                    {
                        throw new Exception("只有同一个出库单的条码不可以合包");
                    }
                }
            }
            #endregion

            string sql = @"insert into root.hf_t_barcodecombined
                              (id, itembarcode, weight, sourcebillid, orderno, remark, newitembarcode, createby, createdate,combinbatchno)
                            values
                              (@id, @itembarcode, @weight, @sourcebillid, @orderno, @remark, @newitembarcode, @createby, sysdate,@combinbatchno)";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            bool re = true;
            foreach (string bar in mergeBarcode.OldBarcodes)
            {
                lstParams.Clear();
                BarcodeInfo barinfo = GetShippedBarcode(bar);
                lstParams.Add("id", SnowflakeID.Guid().ToString());
                lstParams.Add("itembarcode", bar);
                lstParams.Add("weight", barinfo.weight);
                lstParams.Add("sourcebillid", barinfo.orderdetailid);
                lstParams.Add("orderno", barinfo.order_no);
                lstParams.Add("remark", "");
                lstParams.Add("newitembarcode", mergeBarcode.NewBarcode);
                lstParams.Add("createby", Context.UserInfo.UserName);
                lstParams.Add("combinbatchno", SnowflakeID.Guid().ToString());
                re = DBHelper.RunSql(sql, lstParams, "保存合记录") == 1;
            }
            return re;
        }

        /// <summary>
        /// 检查条码是否已经扫描出库
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="spec"></param>
        /// <param name="outdetailid"></param>
        /// <returns></returns>
        private bool CheckBarcodeIsScaned(string barcode) {
            string sql = @"select count(*) from hf_t_barcodeshipedrelation sb where sb.BARCODE=@barcode";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("barcode", barcode);
            return int.Parse(DBHelper.SelectOne(sql, lstParams, "检查条码是否已出库扫描", false)) > 0;
        }
        /// <summary>
        /// 检查出库条码是否在指定的出库明细ID下
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="outdetailid"></param>
        /// <returns></returns>
        private bool CheckBarcodeShippedDetailId(string barcode, string outdetailid) {
            string sql = @"select count(*) from hf_t_barcodeshipedrelation sb 
                        where sb.BARCODE=@barcode and sb.OUTDETAILID=@outdetailid";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("barcode", barcode);
            lstParam.Add("outdetailid", outdetailid);
            return int.Parse(DBHelper.SelectOne(sql,lstParam,"",false))>0;
        }
        /// <summary>
        /// 出库烟包条码合并        
        /// </summary>
        /// <param name="mergeBarcode"></param>
        /// <returns>返回合包批次流水号</returns>
        public string CombineShippedBarcodeNew(MergebarcodeNewIn mergedbars)
        {
            if (mergedbars == null
                || mergedbars.OldBarcodes == null
                || mergedbars.OldBarcodes.Count == 0)
            {
                throw new Exception("请扫描被合并的条码");
            }

            #region 先检查合包的条码是否绑定到其它出库任务明细上；如果没有出库扫描则还需要先提交扫描
            foreach (var obar in mergedbars.OldBarcodes)
            {
                if (CheckBarcodeHasCombined(obar.itembarcode)) {
                    throw new Exception(obar.itembarcode + "已经存在合箱记录");
                }
                if (CheckBarcodeIsScaned(obar.itembarcode))
                {
                    if (!CheckBarcodeShippedDetailId(obar.itembarcode, obar.orderdetailid))
                    {
                        throw new Exception(obar.itembarcode + "已经绑定到其它任务明细");
                    }
                }
                else
                {
                    //先出库提交
                    SubmitOutboudDetailIn onepick = new SubmitOutboudDetailIn();
                    onepick.OutboundOrderDetailId = obar.orderdetailid;
                    onepick.TotalGW = obar.speci;
                    List<PickedBarcode> lstOnepickBarcode = new List<PickedBarcode>();
                    lstOnepickBarcode.Add(new PickedBarcode() { Barcode = obar.itembarcode, Specis = obar.speci });
                    onepick.lstDetails = lstOnepickBarcode;
                    UpOutTaskInfoNew(onepick);
                }
            }
            #endregion

           
            #region 插入到合包记录表
            string sql = @"insert into root.hf_t_barcodecombined
                              (id, itembarcode, weight, sourcebillid, orderno, remark, newitembarcode, createby, createdate,combinbatchno)
                            values
                              (@id, @itembarcode, @weight, @sourcebillid, @orderno, @remark, @newitembarcode, @createby, sysdate,@combinbatchno)";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            bool re = true;
            string mergeBatchId = SnowflakeID.Guid().ToString();
            foreach (BarcodeMergeInfo bar in mergedbars.OldBarcodes)
            {
                lstParams.Clear();
                //BarcodeInfo barinfo = GetShippedBarcode(bar);
                lstParams.Add("id", SnowflakeID.Guid().ToString());
                lstParams.Add("itembarcode", bar.itembarcode);
                lstParams.Add("weight", bar.speci);
                lstParams.Add("sourcebillid", bar.orderdetailid);
                lstParams.Add("orderno", bar.order_no);
                lstParams.Add("remark", "");
                lstParams.Add("newitembarcode", "");
                lstParams.Add("createby", Context.UserInfo.UserName);
                lstParams.Add("combinbatchno", mergeBatchId);
                re = DBHelper.RunSql(sql, lstParams, "保存合记录") == 1;
            }
            #endregion
            return mergeBatchId;
        }
            /// <summary>
            /// 检查条码是否已经合并
            /// </summary>
            /// <param name="barcode"></param>
            /// <returns></returns>
        private bool CheckBarcodeHasCombined(string barcode) {
            string sql = @"select count(1) from root.hf_t_barcodecombined c 
                                where c.ITEMBARCODE=@itembarcode";
            Dictionary<string, object> lstCheckParams = new Dictionary<string, object>();
            lstCheckParams.Add("itembarcode", barcode);
            return int.Parse(DBHelper.SelectOne(sql, lstCheckParams)) > 0;
        }
        /// <summary>
        /// 检查新的条码是否可用
        /// </summary>
        /// <param name="newbarcode"></param>
        /// <returns></returns>
        private bool checkNewBarcodeValid(string newbarcode) {
            string sql = "select count(1) from  root.hf_t_barcodecombined x where x.NEWITEMBARCODE = @NEWITEMBARCODE";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("NEWITEMBARCODE", newbarcode);
            string counts = DBHelper.SelectOne(sql, lstParams);
            return int.Parse(counts) == 0;
        }

        /// <summary>
        /// 获取在库条码信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public BarcodeOut GetInvBarcodeInfo(string barcode) {
            string sql = @"select b.id,b.itembarcode,b.weight,d.itemname ,i.code as itemcode,b.containerid,b.itemid,b.warterrate,b.state
                           from root.hf_t_barcode b inner join root.hf_t_containerdetail d on b.containerid=d.containerid and b.itemid=d.itemid and b.speci=d.speci
                                inner join root.hf_t_item i on b.itemid=i.id
                            where b.itembarcode=@itembarcode    and b.state in(0,1)";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("itembarcode", barcode);
            DataTable dt = DBHelper.Select(sql,lstParams,"",false);
            if (dt.Rows.Count == 0) {
                throw new Exception("库存中无此烟包条码");
            }
            return DBHelper.ConvertTableToObject<BarcodeOut>(dt)[0]; 
        }
        /// <summary>
        /// 提交拆分信息，并生成对应的拆分条码信息
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool SubmitSplitInfo(BarcodeSplitIn splitInfo){
            ///生成一个拆分批次
            string splitbatchid = SnowflakeID.Guid().ToString();

            string sql = @"insert into root.hf_t_barcodesplit
              (id, itembarcodeid, itembarcode, splitgw, splitqty, leftgw, splitbatchno, remark, 
              printstatus, creator, createdate)
            values
              (@id, @itembarcodeid, @itembarcode, @splitgw, @splitqty, @leftgw, @splitbatchno, @remark,@printstatus, 
              @creator, sysdate)";
            BarcodeOut barInfo = GetInvBarcodeInfo(splitInfo.itembarcode);
            decimal? totalSpltGw = splitInfo.lstSplitDetail.Sum(s => s.SplitGw*s.SplitQty);
            if ((totalSpltGw??0) == 0) {
                throw new Exception("拆分重量不能为0");
            }
            decimal leftgw = (decimal)barInfo.weight - (totalSpltGw ?? 0);
            if (leftgw < 0) {
                throw new Exception("总计拆分重量不能大于此条码库存数量{"+ barInfo.weight.ToString() + "}");
            }

            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            foreach (SplitDetail split in splitInfo.lstSplitDetail) {
                lstParams.Clear();
                #region 新增拆分记录表
                if ((split.SplitGw ?? 0) <= 0) {
                    throw new Exception("拆分重量必须大于0");
                }
                if ((split.SplitQty ?? 0) <= 0)
                {
                    throw new Exception("拆分件数必须大于0");
                }
                lstParams.Add("id", SnowflakeID.Guid().ToString());
                lstParams.Add("itembarcodeid", splitInfo.id);
                lstParams.Add("itembarcode", splitInfo.itembarcode);
                lstParams.Add("splitgw", split.SplitGw.ToString());
                lstParams.Add("splitqty", split.SplitQty.ToString());
                lstParams.Add("leftgw", leftgw.ToString());
                lstParams.Add("splitbatchno", splitbatchid);
                lstParams.Add("remark", "");
                lstParams.Add("printstatus", "未打印");
                lstParams.Add("creator", Context.UserInfo.UserId);
                int re = DBHelper.RunSql(sql, lstParams);
                if (re == 0) {
                    throw new Exception("保存条码拆分记录出错");
                }
                #endregion
                #region 生成拆分出的条码信息

                //产生拆分出的新条码
                MakeSplitNewBarcode(splitInfo, (decimal)split.SplitGw, (decimal)split.SplitQty, splitbatchid,"新条码"); 
                #region 判断此货位是否有编码+规格的库存行，如果没有则新增

                if (CheckThisLocationHasSameSpeciRecord(splitInfo.containerid, splitInfo.itemid, split.SplitGw.ToString()))
                {
                    UpdatContainerDetailQty(splitInfo.containerid, splitInfo.itemid, split.SplitGw.ToString(), split.SplitQty.ToString());
                }
                else {
                    AddNewContainerDetail(splitInfo.containerid, splitInfo.itemid,splitInfo.itemname, split.SplitGw.ToString(),split.SplitQty.ToString());
                }                
                #endregion
                #endregion
            }

            //扣减原规格库存
            ReduceInv(splitInfo.containerid, splitInfo.itemid, splitInfo.weight.ToString());

            //产生剩余物料的新条码
            if (leftgw > 0)
            {
                MakeSplitNewBarcode(splitInfo, leftgw, 1, splitbatchid, "原条码剩余");
                if (CheckThisLocationHasSameSpeciRecord(splitInfo.containerid, splitInfo.itemid, leftgw.ToString()))
                {
                    UpdatContainerDetailQty(splitInfo.containerid, splitInfo.itemid, leftgw.ToString(), "1");
                }
                else
                {
                    AddNewContainerDetail(splitInfo.containerid, splitInfo.itemid, splitInfo.itemname, leftgw.ToString(), "1");
                }
            }

            //将原烟包状态更新为2已出库
            updateBarcodeState(splitInfo.id, 2);

            return true;
        }
        /// <summary>
        /// 扣减货位库存
        /// </summary>
        /// <param name="containerid"></param>
        /// <param name="itemid"></param>
        /// <param name="speci"></param>
        /// <returns></returns>
        private bool ReduceInv(string containerid, string itemid, string speci) {
            string sql = @"update root.hf_t_containerdetail d set d.qty=d.qty-1,d.weight=d.weight- to_number(@speci)
                            where d.containerid=@containerid and d.itemid=@itemid 
                                  and d.speci=to_number(@speci) and d.isbarcode='0'";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("containerid", containerid);
            lstParam.Add("itemid", itemid);
            lstParam.Add("speci", speci);
          return  DBHelper.RunSql(sql, lstParam)>0;
        }
    
        /// <summary>
        /// 检查容器中是否有此货位规格的记录
        /// </summary>
        /// <param name="containerid"></param>
        /// <param name="itemid"></param>
        /// <param name="speci"></param>
        /// <returns></returns>
        private bool CheckThisLocationHasSameSpeciRecord(string containerid, string itemid, string speci) {
            string sql = @"select count(1) from root.hf_t_containerdetail d 
                            where d.containerid=@containerid and d.itemid=@itemid 
                                  and d.speci=to_number(@speci) and d.isbarcode=0";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("containerid", containerid);
            lstParam.Add("itemid", itemid);
            lstParam.Add("speci", speci);
            return int.Parse(DBHelper.SelectOne(sql, lstParam)) > 0;
        }
        /// <summary>
        /// 在货位增加规格库存
        /// </summary>
        /// <param name="containerid"></param>
        /// <param name="itemid"></param>
        /// <param name="speci"></param>
        /// <returns></returns>
        private bool UpdatContainerDetailQty(string containerid, string itemid, string speci,string qty) {
            string sql = @"update root.hf_t_containerdetail d set d.qty=nvl(d.qty,0)+to_number(@qty),d.weight=nvl(d.weight,0)+to_number(@speci)*to_number(@qty)
                          where d.containerid=@containerid and d.speci=to_number(@speci)
                                and d.itemid=@itemid and d.isbarcode='0'";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("containerid", containerid);
            lstParam.Add("speci", speci);
            lstParam.Add("qty", qty);
            lstParam.Add("itemid", itemid);
            return DBHelper.RunSql(sql, lstParam) > 0;
        }
        /// <summary>
        /// 在货位新增规格库存
        /// </summary>
        /// <param name="containerid"></param>
        /// <param name="itemid"></param>
        /// <param name="itemname"></param>
        /// <param name="speci"></param>
        /// <returns></returns>
        private bool AddNewContainerDetail(string containerid, string itemid,string itemname ,string speci,string qty) {
            string sql = @"insert into root.hf_t_containerdetail
             (id, containerid, itemid, weight, userid, createtime, frozenreason, frozenqty, frozenweight, remark, lockqty, lockweight, speci, uncheckedqty, uncheckedweight, itemname, isbarcode, qty, itemspeci)
           values
             (@id, @containerid, @itemid, to_number(@speci)*to_number(@qty), @userid, sysdate, '', 0, 0, '', 0, 0, @speci, 0, 0, @itemname, 0, @qty, '')
              ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("id", SnowflakeID.Guid().ToString());
            lstParam.Add("containerid", containerid);
            lstParam.Add("itemid", itemid);
            lstParam.Add("speci", speci);
            lstParam.Add("qty", qty);
            lstParam.Add("userid", Context.UserInfo.UserId);
            lstParam.Add("itemname", itemname);
            return DBHelper.RunSql(sql, lstParam) > 0;
        }

        /// <summary>
        /// 批量提交条码拆分
        /// </summary>
        /// <param name="splitInfos"></param>
        /// <returns></returns>
        public bool SubmitSplitInfos(List<BarcodeSplitIn> splitInfos)
        {
            foreach (BarcodeSplitIn splitInfo in splitInfos) {
                SubmitSplitInfo(splitInfo);
            }
            return true;
        }
        /// <summary>
        /// 产生新的条码
        /// </summary>
        /// <param name="oldBarid"></param>
        /// <param name="splitgw"></param>
        /// <param name="splitqty"></param>
        /// <returns></returns>
        private bool MakeSplitNewBarcode(BarcodeOut oldBarcode, decimal splitgw, decimal splitqty, string splitBatchId, string isNewbarcodeOrleft)
        {

            string enterpriseCode = "19000001";
            string beltlineCode = "1";
            string classesCode = "1";
            string designatorCode = "0"; 

            string snPart1 = "90" + enterpriseCode + beltlineCode + classesCode + designatorCode + DateTime.Now.ToString("yyMMdd") + oldBarcode.itemcode;

            int boxNumber = new LocationAllocate().GetMaxBoxNumber(oldBarcode.itemid, snPart1) + 1;

            bool re = true;
            for (var i = 0; i < splitqty; i++)
            {
                string itemBarcode = snPart1 + (boxNumber + i).ToString().PadLeft(5, '0');
                string newbarcodeId = SnowflakeID.Guid().ToString();
                re = AddNewBarcode(newbarcodeId, oldBarcode.containerid, itemBarcode, (boxNumber + i).ToString(),
                        splitgw, oldBarcode.itemid, oldBarcode.state, oldBarcode.warterrate);
                if (!re)
                {
                    throw new Exception("保存新生成条码信息出错");
                }
                re = AddBarcodeSplitdetail(splitBatchId, newbarcodeId, isNewbarcodeOrleft);
                if (!re)
                {
                    throw new Exception("记录拆分单与新条码关系失败");
                }
            }

            return true;

        }
        /// <summary>
        /// 保存新增的烟包条码
        /// </summary>
        /// <param name="containerid"></param>
        /// <param name="itembarcode"></param>
        /// <param name="boxno"></param>
        /// <param name="gw"></param>
        /// <param name="itemid"></param>
        /// <param name="state"></param>
        /// <param name="warterrate"></param>
        /// <returns></returns>
        private bool AddNewBarcode(string id,string containerid,string itembarcode, string boxno, decimal gw, string itemid,int? state,decimal? warterrate)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into root.hf_t_barcode(");
            strSql.Append("ID,CONTAINERID,OUTBOUNDID,WEIGHT,GROSSWEIGHT,WARTERRATE,PRINTERLEAF,ITEMID,SPECI,ITEMBARCODE,BOXNUMBER,USERID,CREATETIME,STATE");
            strSql.Append(") values (");
            strSql.Append("@ID,@CONTAINERID,@OUTBOUNDID,@WEIGHT,@GROSSWEIGHT,@WARTERRATE,@PRINTERLEAF,@ITEMID,@SPECI,@ITEMBARCODE,@BOXNUMBER,@USERID,sysdate,@STATE");
            strSql.Append(") ");

            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("ID", id);
            lstParams.Add("CONTAINERID", containerid);
            lstParams.Add("OUTBOUNDID", "");
            lstParams.Add("WEIGHT", gw.ToString());
            lstParams.Add("GROSSWEIGHT", (gw + 18).ToString());
            lstParams.Add("WARTERRATE", (warterrate??12).ToString());
            lstParams.Add("PRINTERLEAF", "安徽中烟加工一组");
            lstParams.Add("ITEMID", itemid);
            lstParams.Add("SPECI", gw.ToString());
            lstParams.Add("ITEMBARCODE", itembarcode);
            lstParams.Add("BOXNUMBER", boxno);
            lstParams.Add("USERID", Context.UserInfo.UserId);
            lstParams.Add("STATE", (state??1).ToString());
            return DBHelper.RunSql(strSql.ToString(), lstParams) > 0;

        }
        /// <summary>
        /// 保存拆分单&新拆分条码对应关系
        /// </summary>
        /// <param name="truckorder"></param>
        /// <param name="truckorderid"></param>
        /// <param name="barcode"></param>
        /// <param name="printbatchid"></param>
        /// <returns></returns>
        private bool AddBarcodeSplitdetail(string splitbatchno, string newbarcodeid,string remark)
        {
            string sql = @"insert into root.hf_t_barcodesplitdetail
                               (id, splitbatchno, newbarcodeid,remark)
                             values
                               (@id, @splitbatchno, @newbarcodeid,@remark)";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("id", SnowflakeID.Guid().ToString());
            lstParams.Add("splitbatchno", splitbatchno);
            lstParams.Add("newbarcodeid", newbarcodeid);
            lstParams.Add("remark", remark);
            return DBHelper.RunSql(sql, lstParams) > 0;
        }
        /// <summary>
        /// 更新烟包条码状态:( 新增=-1， 待检=0，可用=1，已出库=2)
        /// </summary>
        /// <param name="barcodeid"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private bool updateBarcodeState(string barcodeid, int state)
        {
            string sql = @"update root.hf_t_barcode b set b.state = @state
                        where b.id = @id";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("id", barcodeid);
            lstParams.Add("state", state.ToString());
            return DBHelper.RunSql(sql, lstParams) > 0;
        }
    }



    public class ORDERBARCODE
    { 
        //
        // Summary:
        //     OrderBarcode#条码
        public string BARCODE { get; set; }
        //
        // Summary:
        //     OrderBarcode#创建时间
        public DateTime? CREATEDATE { get; set; }
        //
        // Summary:
        //     ID#主键
        public string ID { get; set; }
        //
        // Summary:
        //     OrderBarcode#单据号
        public string ORDERNO { get; set; }
        //
        // Summary:
        //     组织
        public string ORGID { get; set; }
        //
        // Summary:
        //     状态 0:新增,1:正在操作,2:已发送
        public int State { get; set; }
    }
    public class Class_MD5
    {
        public static string StringToMD5(string Password)
        {
            byte[] result = Encoding.Default.GetBytes(Password);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");
        }
    }
}
