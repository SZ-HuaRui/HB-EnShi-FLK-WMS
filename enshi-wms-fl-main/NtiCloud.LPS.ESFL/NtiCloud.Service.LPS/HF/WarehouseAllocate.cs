using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NtiCloud.Service.WMS.HF
{
   public  class WarehouseAllocate
    {
        /// <summary>
        /// 保存手工调整的仓库分配信息
        /// </summary>
        /// <param name="outBoundInfo"></param>
        /// <returns></returns>
        public bool EditOutboundPlan(NtiCloud.Model.WMS.EditWarhouseIn outBoundInfo)
        {
            string msg = string.Empty;
            string order_id = SnowflakeID.Guid().ToString();
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(outBoundInfo.order_no))
            {
                lstParam.Clear();
                lstParam.Add("order_no", outBoundInfo.order_no);
                order_id = DBHelper.SelectOne("select ORDER_ID from root.hf_outboundplan_header_view where rownum<=1 and order_no=@order_no", lstParam);
            }
            //存储过程
            string sql = "ahzy_outbound_pg.edit_outboundplan@hfchk_link";

            try
            {
                if (string.IsNullOrEmpty(outBoundInfo.order_no))
                {
                    throw new Exception("暂时不支持新增单据");
                }
                foreach (NtiCloud.Model.WMS.EditWarhouseDetailIn detailinfo in outBoundInfo.lstDetail)
                {
                    List<Tuple<string, string, System.Data.ParameterDirection>> param = new List<Tuple<string, string, System.Data.ParameterDirection>>();
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_userid", Context.UserInfo.UserId, System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_orderid", outBoundInfo.order_id, System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_orderno", outBoundInfo.order_no, System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_createdate", DateTime.Now.ToString("yyyy-MM-dd"), System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_hremark", outBoundInfo.remark, System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_ordernoid", detailinfo.operation_type == NtiCloud.Model.WMS.OperationType.add ? "0" : detailinfo.orderdetail_id, System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_itemid", detailinfo.itemid, System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_itemname", detailinfo.itemname, System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_qty", detailinfo.qty.ToString(), System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_weight", detailinfo.weight.ToString(), System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_specificationskg", (detailinfo.qty == 0 ? 0 : (detailinfo.weight / detailinfo.qty)).ToString(), System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_dccode", detailinfo.warehouseid, System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_rcvdcCode", detailinfo.rcvwarehouseid, System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_remark", detailinfo.remark, System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_hbatchnum", outBoundInfo.batchnum, System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_productcode", outBoundInfo.productcode, System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_productname", outBoundInfo.productname, System.Data.ParameterDirection.Input));
                    param.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_msg", DBNull.Value.ToString(), System.Data.ParameterDirection.Output));

                    var outputs = DBHelper.RunProc(sql, param, "更新仓库分配信息");

                    msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
            .FirstOrDefault()?.Value.ToString(); ;
                    if (msg != "Y")
                    {
                        throw new Exception("保存异常出错:" + msg);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;

        }

        /// <summary>
        /// 确认仓库分配
        /// </summary>
        /// <param name="outBoundInfo"></param>
        /// <returns></returns>
        public bool MakesureWarehouseAllocate(NtiCloud.Model.WMS.ConfirmWarehouseIn confirmInfo)
        {
            string checkInfo = CheckWarehouseAllocate(confirmInfo.order_no);
            if (!string.IsNullOrEmpty(checkInfo))
            {
                throw new Exception("校验失败:" + checkInfo);
            }
            List<string> lstNewOrder = AddOutboundInfo(confirmInfo);
            AllocateLocation(confirmInfo.orgid, lstNewOrder);


            return true;
        }
        public bool MakesureWarehouseAllocate2(string order_no)
        {
            string checkInfo = CheckWarehouseAllocate(order_no);
            if (!string.IsNullOrEmpty(checkInfo))
            {
                throw new Exception("校验失败:" + checkInfo);
            }
            #region 构建请求参数
            NtiCloud.Model.WMS.ConfirmWarehouseIn confirmInfo = new NtiCloud.Model.WMS.ConfirmWarehouseIn();
            DataTable dt = getOutboundPlanHeader(order_no);
            confirmInfo.order_no = order_no;
            confirmInfo.order_type = dt.Rows[0]["order_type"].ToString();
            confirmInfo.orgid = dt.Rows[0]["orgid"].ToString();
            confirmInfo.service_type = dt.Rows[0]["service_type"].ToString();
            dt = getOutboundPlanDetails(order_no);
            List<NtiCloud.Model.WMS.ConfirmWarehouseDetailIn> lstDetail = new List<NtiCloud.Model.WMS.ConfirmWarehouseDetailIn>();
            foreach (DataRow dr in dt.Rows) {
                NtiCloud.Model.WMS.ConfirmWarehouseDetailIn d = new NtiCloud.Model.WMS.ConfirmWarehouseDetailIn();
                d.itemid = dr["itemid"].ToString();
                d.itemname = dr["itemname"].ToString();
                d.orderdetail_id = dr["orderdetail_id"].ToString();
                d.qty = decimal.Parse(dr["qty"].ToString());
                d.rcvwarehouseid = dr["rcvwarehouseid"].ToString();
                d.specifications_kg = dr["specifications_kg"].ToString();
                d.warehouseid = dr["warehouseid"].ToString();
                d.weight = decimal.Parse(dr["weight"].ToString());
                lstDetail.Add(d);
            }
            confirmInfo.lstDetail = lstDetail;
            #endregion
            List<string> lstNewOrder = AddOutboundInfo(confirmInfo);
            AllocateLocation(confirmInfo.orgid, lstNewOrder);
            return true;
        }
        /// <summary>
        /// 获取出库通知头信息
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        public DataTable getOutboundPlanHeader(string order_no) {
            string sql = @" select * from hf_outboundplan_header_view where order_no = @order_no";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("order_no", order_no);
            return DBHelper.Select(sql, lstParams);
        }
        /// <summary>
        /// 获取出库通知单明细信息
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        public DataTable getOutboundPlanDetails(string order_no)
        {
            string sql = @" select * from hf_outboundplan_detail_view where order_no = @order_no";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("order_no", order_no);
            return DBHelper.Select(sql, lstParams);
        }
        /// <summary>
        /// 检查库存是否满足
        /// </summary>
        /// <param name="orderno">单据号</param>
        /// <returns>满足时返回"";不满足时提示具体不满足信息</returns>
        private string CheckWarehouseAllocate(string orderno) {
            string sql = @"select t.warehouseid,
                                           w.name as warehousename,
                                           t.itemid,
                                           t.itemname,
                                           nvl(sum(p.weight), 0) / count(1) as orderweight,
                                           nvl(sum(c.weight), 0) / count(1) as inventoryweight,
                                           nvl(sum(i.weight), 0) / count(1) as outboundorderweight
                                      from hf_t_outboundplan_detail t
                                     inner join hf_t_warehouse w
                                        on t.warehouseid = w.warehouseid
    
                                      left join (select a.itemid,
                                                        l.warehouseid,
                                                        sum(a.weight - a.frozenweight) as weight
                                                   from hf_t_containerdetail a
                                                  inner join hf_t_location l
                                                     on a.containerid = l.containerid
                                                  group by a.itemid, l.warehouseid) c
                                        on t.itemid = c.itemid
                                       and t.warehouseid = c.warehouseid
   
                                      left join (SELECT SUM(t1.actualweight - t1.rf_weight) AS weight,
                                                        t1.itemid,
                                                        t2.warehouseid
                                                   FROM hf_t_outboundorder_detail t1
                                                  inner join hf_t_outboundorder_header t3
                                                     on t1.order_no = t3.order_no
                                                  inner JOIN hf_t_location t2
                                                     ON t1.loactionid = t2.locationid
                                                  WHERE t1.createdate >= to_date(to_char(sysdate, 'yyyy-MM') || '-01', 'yyyy-MM-dd') 
                                                    AND t1.actualweight > t1.rf_weight
                                                  GROUP BY t1.itemid, t2.warehouseid) i
                                        on t.itemid = i.itemid
                                       and t.warehouseid = i.warehouseid
   
                                        left join (select sum(t.weight - t.actualweight) as weight,
                                                                   t.warehouseid,t.itemid
                                                              from hf_t_outboundplan_detail t
                                                              inner join hf_t_outboundplan_header h
                                                              on t.order_no=h.order_no
                                                             where t.weight > t.ACTUALWEIGHT
                                                               and t.createdate>= to_date(to_char(sysdate, 'yyyy-MM') || '-01', 'yyyy-MM-dd')  
                                                             group by t.warehouseid,t.itemid) p
                                         on t.warehouseid = p.warehouseid
                                         and t.itemid=p.itemid 
                                     where t.order_no = @order_no
                                     group by t.warehouseid, w.name, t.itemname, t.itemid
                                    having nvl(sum(c.weight), 0) / count(1) - nvl(sum(i.weight),0)/ count(1) - nvl(sum(p.weight), 0) / count(1) < 0";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("order_no", orderno);
            System.Data.DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            if (dt.Rows.Count == 0) {
                return "";
            }
            StringBuilder erroInfo = new StringBuilder();
            foreach (System.Data.DataRow dr in dt.Rows) {
                erroInfo.Append(dr["warehousename"].ToString()).Append(" ").Append(dr["itemname"].ToString()).Append("<br/>");
            }
            erroInfo.Append("库存不足！");
            return erroInfo.ToString();
        }
        /// <summary>
        /// 确认仓库分配
        /// </summary>
        /// <param name="confirmInfo"></param>
        /// <returns></returns>
        private List<string> AddOutboundInfo(NtiCloud.Model.WMS.ConfirmWarehouseIn confirmInfo) {
            string spname = "ahzy_outbound_pg.add_outboundorder_info@hfchk_link";
            List<string> list = new List<string>();
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            foreach (NtiCloud.Model.WMS.ConfirmWarehouseDetailIn detailinfo in confirmInfo.lstDetail)
            {
                lstParam.Clear();
                lstParam.Add("ref_orderno", confirmInfo.order_no);
                lstParam.Add("warehouseid", detailinfo.warehouseid);
                lstParam.Add("arr_warehouseid", detailinfo.rcvwarehouseid);
                string orderno = DBHelper.SelectOne(@" SELECT order_no  FROM hf_t_outboundorder_header 
                                WHERE rownum<=1 and ref_orderno = @ref_orderno and warehouseid= @warehouseid and arr_warehouseid= @arr_warehouseid ",
                                lstParam,"",false);
                if (string.IsNullOrEmpty(orderno))
                {
                    orderno = GetNewOutOrderno("OutBoundOrder");
                }
                if (!list.Contains(orderno))
                    list.Add(orderno);
                List<Tuple<string, string, System.Data.ParameterDirection>> spParams = new List<Tuple<string, string, System.Data.ParameterDirection>>();
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_orgid", confirmInfo.orgid, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_orderno", orderno, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_orderid", detailinfo.orderdetail_id, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_ordertype", confirmInfo.order_type, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_servicetype", confirmInfo.service_type, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_reforderno", confirmInfo.order_no, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_itemid", detailinfo.itemid, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_itemname", detailinfo.itemname, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_qty", detailinfo.qty.ToString(), System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_weight", detailinfo.weight.ToString(), System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_specificationskg", detailinfo.specifications_kg.ToString(), System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_dccode", detailinfo.warehouseid, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_userid", Context.UserInfo.UserId, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_rcvdcCode", detailinfo.rcvwarehouseid, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_msg", DBNull.Value.ToString(), System.Data.ParameterDirection.Output));
                var outputs=DBHelper.RunProc(spname, spParams, "", DBHelper.OperatedNumType.OneOrMore);
                string msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                                .FirstOrDefault()?.Value.ToString(); 
                 
                if (!"Y".Equals(msg))
                {
                    throw new Exception("仓库分配失败:"+ msg); 
                }
            }
            return list;
        }

        /// <summary>
        /// 分配货位
        /// </summary>
        /// <param name="confirmInfo"></param>
        /// <returns></returns>
        private bool AllocateLocation(string orgid,List<string> lstNeworder)
        {
            string spname = "ahzy_outbound_pg.upd_locationinfo@hfchk_link";
            List<string> list = new List<string>();
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            foreach (string orderno in lstNeworder)
            { ;
                List<Tuple<string, string, System.Data.ParameterDirection>> spParams = new List<Tuple<string, string, System.Data.ParameterDirection>>();
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_orgid", orgid, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_orderno", orderno, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_userid", Context.UserInfo.UserId, System.Data.ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>("p_msg", DBNull.Value.ToString(), System.Data.ParameterDirection.Output));
                var outputs = DBHelper.RunProc(spname, spParams, "", DBHelper.OperatedNumType.OneOrMore);
                string msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                                .FirstOrDefault()?.Value.ToString();

                if (!"Y".Equals(msg))
                {
                    throw new Exception("分配货位失败:" + msg);
                }
            }
            return true;
        }
        /// <summary>
        /// 同步配方
        /// </summary> 
        /// <param name="ordernos">多个单号时用,号分割</param>
        /// <returns></returns>
        public string syncPrescription( string ordernos)
        {
            try
            {
                //存储过程
                string spname = "P_syncPrescription@hfchk_link";
                List<Tuple<string, string, ParameterDirection>> spParams = new List<Tuple<string, string, ParameterDirection>>();
                spParams.Add(new Tuple<string, string, ParameterDirection>("i_ordernos", ordernos, ParameterDirection.Input));
                DBHelper.RunProc(spname, spParams);
                return "true";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        /// <summary>
        /// 获取新的订单号
        /// </summary>
        /// <param name="prefix">流水号类型</param>
        /// <returns></returns>
        private string GetNewOutOrderno(string prefix) {
            string sp = "M_GETBILLNUMBER@hfchk_link";
            List<Tuple<string, string, System.Data.ParameterDirection>> lstParam = new List<Tuple<string, string, System.Data.ParameterDirection>>();
            lstParam.Add(new Tuple<string, string, System.Data.ParameterDirection>("I_BILLNAME", prefix, System.Data.ParameterDirection.Input));
            lstParam.Add(new Tuple<string, string, System.Data.ParameterDirection>("O_BILLNUMBER", DBNull.Value.ToString(), System.Data.ParameterDirection.Output));
            var outputs = DBHelper.RunProc(sp, lstParam);
            string newNumber=outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "O_BILLNUMBER")
            .FirstOrDefault()?.Value.ToString();
            if (string.IsNullOrEmpty(newNumber)) {
                throw new Exception("获取新流水号失败");
            }
            return newNumber;
        }
        /// <summary>
        /// 删除出库计划
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        public bool DeleteOutboundPlan(string order_no) {
            AdjustOrder("P_DeleteOutboundPlan@Hfchk_Link", order_no,"");
            return true;
        }
        /// <summary>
        /// 删除出库明细
        /// </summary>
        /// <param name="order_no"></param>
        /// <param name="orderdetail_id"></param>
        /// <returns></returns>
        public bool DeleteOutboundPlanDetail(string order_no, string orderdetail_id) {
           string sql= @"delete root.hf_t_outboundplan_detail where order_no= @order_no  and orderdetail_id= @orderdetail_id";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("order_no", order_no);
            lstParams.Add("orderdetail_id", orderdetail_id);
            return DBHelper.RunSql(sql, lstParams) > 0;
        }
        /// <summary>
        /// 单据调整
        /// </summary>
        /// <param name="spName">存过名</param>
        /// <param name="orderNO">单据号</param>
        public static void AdjustOrder(string spName, string orderNO,string orderstate)
        {
            List<Tuple<string, string, System.Data.ParameterDirection>> spParams = new List<Tuple<string, string, System.Data.ParameterDirection>>();
            spParams.Add(new Tuple<string, string, System.Data.ParameterDirection>( "I_ORDERNO", orderNO, System.Data.ParameterDirection.Input));
            DBHelper.RunProc(spName, spParams, "", DBHelper.OperatedNumType.ZeroOrMore);

            string sql = string.Format(@"insert into FORM_LOG@Hfchk_Link(ID,ORDERNO,REFORDERNO,STATUSCODE,STATUSNAME,REMARK,CREATEBY,CREATEDATE) 
                values(sys_guid(),'{0}','{1}','{2}','{3}','{4}','{5}',to_date('{6}','yyyy-MM-dd HH24:mi:ss'))",
                    orderNO, orderNO, orderstate, "被执行", "相关存过：" + spName + " 被执行", Context.UserInfo.UserName, DateTime.Now);
            DBHelper.RunSql(sql,new Dictionary<string, object>());
            return;
        }

    }
}
