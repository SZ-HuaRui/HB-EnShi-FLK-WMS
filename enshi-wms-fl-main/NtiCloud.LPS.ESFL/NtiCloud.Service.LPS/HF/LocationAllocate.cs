using Microsoft.AspNetCore.SignalR.Protocol;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.HF.RF;
using NtiCloud.Service.WMS.Inventory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace NtiCloud.Service.WMS.HF
{
    public class LocationAllocate
    {
        /// <summary>
        /// 保存出库单信息
        /// </summary>
        /// <param name="outboundInfo"></param>
        /// <returns></returns>
        public bool UpdateOutOrder(NtiCloud.Model.WMS.EditOutboundIn outboundInfo)
        {
            string msg = string.Empty;

            //存储过程
            string spname = "ahzy_outbound_pg.upd_outboundorder_info@hfchk_link";//修改货位信息

            foreach (NtiCloud.Model.WMS.EditOutboundDetailIn detailinfo in outboundInfo.lstDetail)
            {
                List<Tuple<string, string, ParameterDirection>> spParams = new List<Tuple<string, string, ParameterDirection>>();
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_id", detailinfo.outbounddetail_id, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_orgid", outboundInfo.orgid, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_orderno", outboundInfo.order_no, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_orderid", outboundInfo.id, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_dccode", outboundInfo.warehouseid, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_itemid", detailinfo.itemid, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_itemname", detailinfo.itemname, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_specificationskg", detailinfo.specifications_kg, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_actualqty", detailinfo.actualqty, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_actualweight", detailinfo.actualweight, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_operator", Context.UserInfo.UserId, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_operatdate", DateTime.Now.ToString("yyyy-MM-dd"), ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_flowtype", outboundInfo.flow_type, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_flow", outboundInfo.transfers, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_flowcode", outboundInfo.transferscode, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_settlementtype", outboundInfo.settlement_type, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_chargeunit", outboundInfo.chargeunit, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_chargeunitcode", outboundInfo.chargeunitcode, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_remark", outboundInfo.remark, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_locationid", detailinfo.loactionid, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_returnreason", outboundInfo.returnreason, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_servicetype", outboundInfo.service_type, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_IsBarCode", outboundInfo.isbarcode, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_ProductName", outboundInfo.productname, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_ProductCode", outboundInfo.productcode, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_BatchNum", outboundInfo.batchnum, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_RcvDcCode", outboundInfo.arr_warehouseid, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_RcvOrgid", outboundInfo.rcvorgid, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_userid", Context.UserInfo.UserId, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_lossweight", detailinfo.lossweight, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_lossqty", detailinfo.lossqty, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_vcontractno", outboundInfo.vcontractno, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_msg", DBNull.Value.ToString(), ParameterDirection.Output));

                var outputs = DBHelper.RunProc(spname, spParams, "更新出库单", DBHelper.OperatedNumType.ZeroOrMore);

                msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                     .FirstOrDefault()?.Value.ToString();

                if (msg != "Y")
                {
                    throw new Exception("更新出库单出错:" + msg);
                }
            }

            //重量大于0才校验
            decimal totalWeight = GetOutboundActualweight(outboundInfo.order_no);
            if (totalWeight > 0)
            {
                //检查库存是否足够
                string checkmsg = CheckInventorySuffice(outboundInfo.order_no);
                if (checkmsg != "")
                {
                    throw new Exception("更新出库单出错:" + checkmsg);
                }
            }
            return true;
        }

        /// <summary>
        /// 保存并确认出库单
        /// </summary>
        /// <param name="outboundInfo"></param>
        /// <returns></returns>
        public bool SaveAndConfirmOutbound(NtiCloud.Model.WMS.EditOutboundIn outboundInfo)
        {
            bool up = UpdateOutOrder(outboundInfo);
            if (!up)
            {
                throw new Exception("保存出库单时出错");
            }
            up = ConfirmOutourderInfo(outboundInfo.order_no, outboundInfo.isbarcode);
            if (!up)
            {
                throw new Exception("保存出库单时出错");
            }
            return true;
        }
        /// <summary>
        /// 调整订单
        /// </summary>
        /// <param name="serviceName">存过名</param>
        /// <param name="orderNo">出库单号</param>
        /// <returns></returns>
        public bool AdjustOrder(string serviceName, string orderNo)
        {
            #region 变量
            serviceName = serviceName.ToUpper();
            string serviceType = "";
            string refOrder = "";
            string sourceOrder = "";
            string inboundno = "";
            string outboundno = "";
            string orderstate = "";
            string SQLString = "";
            string planNO = "";
            DataRow row = null;
            DataTable dt = null;
            #endregion
            string sql = "";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            #region 删除出库单
            if (serviceName == "P_DELETEOUTBOUNDORDER@HFCHK_LINK")//删除出库单
            {
                sql = @"select t.service_type,t.REF_ORDERNO,t.STATUS
                                          from root.hf_t_outboundorder_header t
                                         where t.order_no = @order_no";
                lstParam.Clear();
                lstParam.Add("order_no", orderNo);
                dt = DBHelper.Select(sql, lstParam);

                row = dt.Rows[0];

                serviceType = row["service_type"].ToString();
                planNO = row["ref_orderno"].ToString();

                if (orderNo == planNO || planNO == "" || orderNo.Contains(planNO))//来源单号等于出库单号，系统自增单据
                {
                    #region 内转出库单
                    if (serviceType == "Inturn_Service" || serviceType == "MESStock_Service" || serviceType == "Return_Service" || serviceType == "Chasing_Service")
                    {
                        sql = @"select t.inbound_number,t.status
                                          from HF_T_inbound t
                                         where t.ref_orderno = @orderno";
                        lstParam.Clear();
                        lstParam.Add("orderno", orderNo);
                        dt = DBHelper.Select(sql, lstParam, "", false);
                        if (dt.Rows.Count > 0)//判断是否生成入库单
                        {
                            row = dt.Rows[0];
                            inboundno = row["inbound_number"].ToString();
                            orderstate = row["status"].ToString();
                            if (orderstate != "Inbound_Allocation" && orderstate != "Inbound_New")
                            {
                                throw new Exception("备料/内转入库单的状态不是未确认状态，无法直接删除！");
                            }
                            if (serviceType == "MESStock_Service")//判断备料结构单下的生产领用是否能否删除
                            {
                                sql = @"SELECT   t.order_no,t.status, t.service_type, t.ref_orderno 
                                        FROM   root.hf_t_outboundorder_header t 
                                        WHERE       service_type = 'Productive_Service' 
                                                AND t.ref_orderno =@orderno";
                                lstParam.Clear();
                                lstParam.Add("orderno", orderNo);
                                DataTable dt1 = DBHelper.Select(sql, lstParam, "", false);
                                if (dt1.Rows.Count > 0)//判断状态为未处理的生产领用出库单是否存在
                                {
                                    row = dt1.Rows[0];
                                    outboundno = row["order_no"].ToString();
                                    orderstate = row["status"].ToString();

                                    if (orderstate != "Order_New" && orderstate != "Order_Allocation")
                                    {
                                        throw new Exception("生产领用单的状态不是未确认状态，无法直接删除！");
                                    }
                                    //删除备料单时，同步删除对应生产领用
                                    doAdjustOrder("P_DeleteProductiveOrder@hfchk_link", outboundno);

                                    //Log 日志
                                    sql = string.Format("insert into root.HF_T_FORM_LOG(ID,ORDERNO,REFORDERNO,STATUSCODE,STATUSNAME,REMARK,CREATEBY,CREATEDATE) values(sys_guid(),'{0}','{1}','{2}','{3}','{4}','{5}',to_date('{6}','yyyy-MM-dd HH24:mi:ss'))",
                                     outboundno, planNO, orderstate, "已删除", "删除备料单时，同步删除对应生产领用，出库单号：" + orderNo, Context.UserInfo.UserName, DateTime.Now);
                                    lstParam.Clear();
                                    DBHelper.RunSql(sql, lstParam);
                                }
                            }
                            //删除备料单时，同步删除对应备料入库单
                            doAdjustOrder("P_DeleteInboundOrder@hfchk_link", inboundno);

                            //Log 日志
                            sql = string.Format("insert into root.HF_T_FORM_LOG(ID,ORDERNO,REFORDERNO,STATUSCODE,STATUSNAME,REMARK,CREATEBY,CREATEDATE) values(sys_guid(),'{0}','{1}','{2}','{3}','{4}','{5}',to_date('{6}','yyyy-MM-dd HH24:mi:ss'))",
                             inboundno, planNO, orderstate, "已删除", "删除备料单时，同步删除对应备料入库单，对应出库单号：" + orderNo, Context.UserInfo.UserName, DateTime.Now);
                            lstParam.Clear();
                            DBHelper.RunSql(sql, lstParam);

                        }
                    }
                    #endregion

                    #region 厂际出库
                    if (serviceType == "Transfer_Service")
                    {
                        sql = @"select t.receiving_no,t.status
                                          from root.HF_T_inboundplan_header t
                                         where t.ref_orderno = @orderno";
                        lstParam.Clear();
                        lstParam.Add("orderno", orderNo);
                        dt = DBHelper.Select(sql, lstParam, "", false);
                        if (dt.Rows.Count > 0)//判断是否生成入库计划单
                        {
                            row = dt.Rows[0];
                            inboundno = row["receiving_no"].ToString();
                            orderstate = row["status"].ToString();
                            if (orderstate != "ReceivingState_New")
                            {
                                throw new Exception("厂际调剂入库计划单的状态不是未处理状态，无法直接删除！");
                            }
                            //删除厂际调剂出库单时，同步删除厂际调剂入库计划单
                            doAdjustOrder("P_DeleteInboundPlan@hfchk_link", inboundno);

                            //Log 日志
                            sql = string.Format("insert into root.hf_t_form_log(ID,ORDERNO,REFORDERNO,STATUSCODE,STATUSNAME,REMARK,CREATEBY,CREATEDATE) values(sys_guid(),'{0}','{1}','{2}','{3}','{4}','{5}',to_date('{6}','yyyy-MM-dd HH24:mi:ss'))",
                             inboundno, planNO, orderstate, "已删除", "删除厂际调剂出库单时，同步删除厂际调剂入库计划单，对应出库单号：" + orderNo, Context.UserInfo.UserName, DateTime.Now);
                            lstParam.Clear();
                            DBHelper.RunSql(sql, lstParam);
                        }
                    }
                    #endregion
                }
                else
                {
                    if ((serviceType == "MESStock_Service" || serviceType == "Productive_Service") && (row["status"].ToString() == "Order_New" || row["status"].ToString() == "Order_Allocation"))
                    {
                        //非系统自增的备料结构单未处理状态下可删除
                        doAdjustOrder(serviceName, orderNo);

                        //Log 日志
                        sql = string.Format(@"insert into root.hf_t_form_log(ID,ORDERNO,REFORDERNO,STATUSCODE,STATUSNAME,REMARK,CREATEBY,CREATEDATE) 
                                values(sys_guid(),'{0}','{1}','{2}','{3}','{4}','{5}',to_date('{6}','yyyy-MM-dd HH24:mi:ss'))",
                        orderNo, planNO, row["status"].ToString(), "已删除", "删除未处理状态下的备料结构出库单", Context.UserInfo.UserName, DateTime.Now);
                        lstParam.Clear();
                        DBHelper.RunSql(sql, lstParam);
                        return true;
                    }
                    throw new Exception("不是系统自增的出库单无法直接删除");
                }
            }
            #endregion

            #region 取消出库
            if (serviceName == "P_UNDOOUTBOUND@HFCHK_LINK")//取消备料出库时，删除备料入库单，生产领用出库单
            {
                sql = @"select t.service_type,t.ref_orderno,t.createdate,t.warehouseid
                                          from root.hf_t_outboundorder_header t
                                         where t.order_no = @order_no";
                lstParam.Clear();
                lstParam.Add("order_no", orderNo);
                dt = DBHelper.Select(sql, lstParam);
                row = dt.Rows[0];
                if (!IsOverMonth(row["warehouseid"].ToString(), DateTime.Parse(row["createdate"].ToString())))
                {
                    throw new Exception("出库日期只能为当前月份，不能跨月！");
                }
                serviceType = row["service_type"].ToString();
                planNO = row["ref_orderno"].ToString();

                #region 内转出库
                if (serviceType == "Inturn_Service" || serviceType == "MESStock_Service" || serviceType == "Return_Service" || serviceType == "Chasing_Service")
                {
                    sql = @"select t.inbound_number,t.status
                                          from root.hf_t_inbound t
                                         where t.ref_orderno = @ref_orderno ";
                    lstParam.Clear();
                    lstParam.Add("ref_orderno", orderNo);
                    dt = DBHelper.Select(sql, lstParam, "", false);
                    if (dt.Rows.Count > 0)//判断是否生成入库单
                    {
                        row = dt.Rows[0];
                        inboundno = row["inbound_number"].ToString();
                        orderstate = row["status"].ToString();
                        if (orderstate != "Inbound_Allocation" && orderstate != "Inbound_New")
                        {
                            throw new Exception("备料/内转入库单的状态不是未确认状态，无法直接取消出库！");
                        }

                        if (serviceType == "MESStock_Service")//判断备料结构单下的生产领用是否能否删除
                        {
                            sql = @"SELECT   t.order_no,t.status, t.service_type, t.ref_orderno 
                                    FROM   root.hf_t_outboundorder_header t 
                                    WHERE    service_type = 'Productive_Service' 
                                            AND t.ref_orderno = @ref_orderno";
                            lstParam.Clear();
                            lstParam.Add("ref_orderno", orderNo);
                            DataTable dt1 = DBHelper.Select(sql, lstParam, "", false);
                            if (dt1.Rows.Count > 0)//判断状态为未处理的生产领用出库单是否存在
                            {
                                row = dt1.Rows[0];
                                outboundno = row["order_no"].ToString();
                                orderstate = row["status"].ToString();

                                if (orderstate != "Order_New" && orderstate != "Order_Allocation")
                                {
                                    throw new Exception("生产领用单的状态不是未确认状态，无法直接取消出库！");
                                }
                                else
                                {
                                    sql = @"SELECT   id, order_no,status FROM  root.hf_t_outboundorder_detail  WHERE   refordernoid = @refordernoid";
                                    lstParam.Clear();
                                    lstParam.Add("refordernoid", orderNo);
                                    DataTable dt2 = DBHelper.Select(sql, lstParam, "", false);
                                    for (int i = 0; i < dt2.Rows.Count; i++) //判断对应备料出库单的生产领用出库单明细是否存在
                                    {
                                        row = dt2.Rows[i];
                                        outboundno = row["order_no"].ToString();
                                        orderstate = row["status"].ToString();
                                        string id = row["id"].ToString();
                                        if (orderstate != "Order_New" && orderstate != "Order_Allocation")
                                        {
                                            throw new Exception("生产领用单的明细状态不是未确认状态，无法直接取消出库！");
                                        }
                                        //取消备料出库时，删除对应生产领用出库单
                                        doAdjustOrder("P_DELETEPRODUCTIVEORDERID@hfchk_link", id);

                                        //Log 日志
                                        SQLString = string.Format("insert into root.hf_t_form_log(ID,ORDERNO,REFORDERNO,STATUSCODE,STATUSNAME,REMARK,CREATEBY,CREATEDATE) values(sys_guid(),'{0}','{1}','{2}','{3}','{4}','{5}',to_date('{6}','yyyy-MM-dd HH24:mi:ss'))",
                                        outboundno, planNO, orderstate, "已删除", "取消备料出库时，删除对应生产领用出库单明细，对应明细ID：" + id + " 已删除", Context.UserInfo.UserName, DateTime.Now);
                                        lstParam.Clear();
                                        DBHelper.RunSql(SQLString, lstParam);
                                    }
                                }

                            }
                        }

                        //取消备料出库时，删除对应备料入库单下相关信息
                        doAdjustOrder("P_DeleteInboundOrderByOutNo@hfchk_link", inboundno, orderNo);//如果是合单单据，通过出库单删除对应信息

                        SQLString = string.Format("insert into root.hf_t_form_log(ID,ORDERNO,REFORDERNO,STATUSCODE,STATUSNAME,REMARK,CREATEBY,CREATEDATE) values(sys_guid(),'{0}','{1}','{2}','{3}','{4}','{5}',to_date('{6}','yyyy-MM-dd HH24:mi:ss'))",
                        inboundno, planNO, orderstate, "已删除", "取消备料出库时，删除对应备料入库单下相关信息，对应出库单号：" + orderNo + " 被删除", Context.UserInfo.UserName, DateTime.Now);
                        lstParam.Clear();
                        DBHelper.RunSql(sql, lstParam);
                    }
                }
                #endregion

                #region 厂际出库
                if (serviceType == "Transfer_Service")
                {
                    sql = @"select t.receiving_no,t.status
                                          from root.HF_T_inboundplan_header t
                                         where t.ref_orderno = @ref_orderno";
                    lstParam.Clear();
                    lstParam.Add("ref_orderno", orderNo);
                    dt = DBHelper.Select(sql, lstParam, "", false);
                    if (dt.Rows.Count > 0)//判断是否生成入库计划单
                    {
                        row = dt.Rows[0];
                        inboundno = row["receiving_no"].ToString();
                        orderstate = row["status"].ToString();
                        if (orderstate != "ReceivingState_New")
                        {
                            throw new Exception("厂际调剂入库计划单的状态不是未处理状态，无法直接取消出库！");
                        }
                        //取消厂际调剂出库时，删除对应厂际调剂入库计划单
                        doAdjustOrder("P_DeleteInboundPlan@hfchk_link", inboundno);

                        SQLString = string.Format("insert into root.hf_t_form_log(ID,ORDERNO,REFORDERNO,STATUSCODE,STATUSNAME,REMARK,CREATEBY,CREATEDATE) values(sys_guid(),'{0}','{1}','{2}','{3}','{4}','{5}',to_date('{6}','yyyy-MM-dd HH24:mi:ss'))",
                        inboundno, planNO, orderstate, "已删除", "取消厂际调剂出库时，删除对应厂际调剂入库计划单，对应出库单号：" + orderNo + " 被删除", Context.UserInfo.UserName, DateTime.Now);
                        lstParam.Clear();
                        DBHelper.RunSql(SQLString, lstParam);
                    }
                }
                #endregion

                #region 生产领用
                if (serviceType == "Productive_Service")
                {
                    sql = @"select t.code
                                                  from root.HF_T_lossadjustment t
                                                 inner join root.HF_T_syncorder s
                                                    on t.code = s.orderno
                                                   and s.servicetype = 'LossAdjustment'
                                                 where t.outboundcode = @outboundcode
                                                   and s.status = 1";
                    lstParam.Clear();
                    lstParam.Add("outboundcode", orderNo);
                    string lossCode = DBHelper.SelectOne(sql, lstParam, "", false);
                    if (!string.IsNullOrEmpty(lossCode))//滁州厂生根据生产领用单生成仓耗
                    {
                        throw new Exception(orderNo + "生成的损益调整单:" + lossCode + "已经同步ERP,需要先取消调整单的同步,才能取消生产领用出库!");
                    }
                }
                #endregion
            }
            #endregion


            #region 执行存过
            //执行逆向操作
            doAdjustOrder(serviceName, orderNo);

            //写操作历史
            SQLString = string.Format("insert into hf_t_form_log(ID,ORDERNO,REFORDERNO,STATUSCODE,STATUSNAME,REMARK,CREATEBY,CREATEDATE) values(sys_guid(),'{0}','{1}','{2}','{3}','{4}','{5}',to_date('{6}','yyyy-MM-dd HH24:mi:ss'))",
            orderNo, orderNo, orderstate, "被执行", "相关存过：" + serviceName + " 被执行", Context.UserInfo.UserName, DateTime.Now);
            lstParam.Clear();
            DBHelper.RunSql(SQLString, lstParam);

            return true;
            #endregion
        }
        public DataTable GetDatatableFromSp(string spname, List<Tuple<string, string, ParameterDirection>> lstParams)
        {

            var outputs = DBHelper.RunProc(spname, lstParams, "", DBHelper.OperatedNumType.ZeroOrMore);
            DataTable dt = (DataTable)outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_curtype");
            return dt;
        }

        /// <summary>
        /// 获取出库的的实际总总量
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public decimal GetOutboundActualweight(string order_no)
        {
            string sql = @"select sum(v.actualweight) totalgw from hf_outboundorder_detail_view v where v.order_no = @order_no ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("order_no", order_no);

            return decimal.Parse(DBHelper.SelectOne(sql, lstParam));
        }
        /// <summary>
        /// 检查库存是否满足
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public string CheckInventorySuffice(string orderno)
        {
            string sql = @"select t.itemname,
                                         c.locationname,
                                 sum(t.actualweight) as orderweight,
                                 nvl(sum(c.weight), 0) / count(1) as inventoryweight,
                                 nvl(sum(i.weight), 0) / count(1) as outboundorderweight
                            from root.hf_t_outboundorder_detail t
                            left join (select a.itemid,
                                              l.locationname,
                                              l.locationid,
                                              sum(a.weight - a.frozenweight) as weight
                                         from root.hf_t_containerdetail a
                                        inner join root.hf_t_location l
                                           on a.containerid = l.containerid
                                        group by a.itemid, l.locationname, l.locationid) c
                              on t.itemid = c.itemid
                             and t.loactionid = c.locationid
                            left join (SELECT SUM(t1.actualweight - t1.rf_weight) AS weight,
                                              t1.itemid,
                                              t2.locationid
                                         FROM root.hf_t_outboundorder_detail t1
                                        inner join root.hf_t_outboundorder_header t3
                                           on t1.order_no = t3.order_no
                                        inner JOIN root.hf_t_location t2
                                           ON t1.loactionid = t2.locationid
                                        WHERE t3.createdate >=
                                              to_date(to_char(sysdate, 'yyyy-MM') || '-01','yyyy-MM-dd')
                                          AND t1.actualweight > t1.rf_weight
                                        GROUP BY t1.itemid, t2.locationid) i
                              on t.itemid = i.itemid
                             and t.loactionid = i.locationid
                            where t.order_no =  @order_no
                            group by c.locationname, t.itemname
                            having nvl(sum(c.weight), 0) / count(1) - nvl(sum(i.weight), 0) / count(1) < 0 ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("order_no", orderno);

            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            if (dt.Rows.Count == 0)
            {
                return "";
            }
            StringBuilder re = new StringBuilder();
            foreach (DataRow r in dt.Rows)
            {
                re.Append(r["locationname"]).Append(" ").Append(r["itemname"]).Append("<br/>");
            }
            re.Append(" 库存不足！");
            return re.ToString();
        }

        /// <summary>
        /// 检查日期，确认出库单
        /// </summary>
        /// <param name="orderno"></param>
        /// <param name="isbarcode"></param>
        /// <returns></returns>
        public bool ConfirmOutourderInfo(string orderno, string isbarcode)
        {
            CheckOutboundDetail(orderno);
            string sql = @"select CREATEDATE,warehouseid from root.hf_t_outboundorder_header where order_no = @order_no  and rownum=1";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("order_no", orderno);
            DataTable dt = DBHelper.Select(sql, lstParam);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("出库单确认失败:没有找到此出库单信息");
            }
            DataRow row = dt.Rows[0];
            if (!IsOverMonth(row["warehouseid"].ToString(), DateTime.Parse(row["CREATEDATE"].ToString())))
            {
                throw new Exception("出库单确认失败:出库日期只能为当前月份，不能跨月！");
            }
            return ConfirmLocationInfo(orderno, isbarcode);

        }
        /// <summary>
        /// 出库单拆分：将有码与无码拆分成两个出库单
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        public bool SplitOutbound(string order_no) {
           return AdjustOrder("P_SplitOutbound@hfchk_link", order_no);
        }
        /// <summary>
        /// 确认出库单
        /// </summary>
        /// <param name="orderno"></param>
        /// <param name="isbarcode">是否有码:0=有码，1=无码</param>
        /// <returns></returns>
        private bool ConfirmLocationInfo(string orderno, string isbarcode)
        {
            string msg = string.Empty;
            //存储过程
            string sql = "ahzy_outbound_pg.confirm_locationinfonew@hfchk_link";//修改货位信息
            string inboundsql = "ahzy_outbound_pg.insert_inboundinfo@hfchk_link";//生产管控平台入库信息

            string insql = "ahzy_outbound_pg.insert_barcodedetail@hfchk_link";
            string upsql = "ahzy_outbound_pg.up_outbound_task2@hfchk_link";

            List<Tuple<string, string, ParameterDirection>> spParams = new List<Tuple<string, string, ParameterDirection>>();
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_orgid", "", ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_orderno", orderno, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_isbarcode", isbarcode, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_userid", Context.UserInfo.UserId, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_msg", DBNull.Value.ToString(), ParameterDirection.Output));
            var outputs = DBHelper.RunProc(sql, spParams, "", DBHelper.OperatedNumType.OneOrMore);
            msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                      .FirstOrDefault()?.Value.ToString();

            if (!"Y".Equals(msg))
            {
                throw new Exception("确认出库单时异常:" + msg);
            }
            //如果是有码，先判断是不是整个货位的该物料一起出库，一起出库的，可以不用扫码
            if ("0".Equals(isbarcode) //有码
                &&
                WmsConfig.GetOrDefault("CHKSetting:CHKSetting_ForceScan", "Y")== "N" //配置的是否需要扫描条码开关
                )
            {
                string sql_outdetail = @"select * from root.hf_t_outboundorder_detail where order_no = @order_no";
                Dictionary<string, object> lstParams = new Dictionary<string, object>();
                lstParams.Add("order_no", orderno);
                DataTable dt = DBHelper.Select(sql_outdetail, lstParams);
                foreach (DataRow row in dt.Rows)
                {
                    List<string> list = GetLocationBarcode(row["loactionid"].ToString(), row["itemid"].ToString(), decimal.Parse(row["specifications_kg"].ToString()), decimal.Parse(row["actualweight"].ToString()));
                    if (list.Count == 0)
                    {
                        continue;
                    }
                    spParams.Clear();
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_orgid", "", ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_id", row["id"].ToString(), ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_wgt", row["ACTUALWEIGHT"].ToString(), ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_userid", Context.UserInfo.UserId, ParameterDirection.Input));
                    spParams.Add(new Tuple<string, string, ParameterDirection>("p_msg", DBNull.Value.ToString(), ParameterDirection.Output));
                    outputs = DBHelper.RunProc(sql, spParams, "", DBHelper.OperatedNumType.ZeroOrMore);
                    msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                                        .FirstOrDefault()?.Value.ToString();

                    if (!"Y".Equals(msg))
                    {
                        throw new Exception("确认出库单时异常;" + msg);
                    }
                    foreach (string bar in list)
                    {
                        spParams.Clear();
                        spParams.Add(new Tuple<string, string, ParameterDirection>("p_orgid", "", ParameterDirection.Input));
                        spParams.Add(new Tuple<string, string, ParameterDirection>("p_id", row["id"].ToString(), ParameterDirection.Input));
                        spParams.Add(new Tuple<string, string, ParameterDirection>("p_barcode", bar, ParameterDirection.Input));
                        spParams.Add(new Tuple<string, string, ParameterDirection>("p_speci", "0", ParameterDirection.Input));
                        spParams.Add(new Tuple<string, string, ParameterDirection>("p_userid", Context.UserInfo.UserId, ParameterDirection.Input));
                        spParams.Add(new Tuple<string, string, ParameterDirection>("p_msg", DBNull.Value.ToString(), ParameterDirection.Output));
                        outputs = DBHelper.RunProc(insql, spParams, "", DBHelper.OperatedNumType.ZeroOrMore);
                        msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                                      .FirstOrDefault()?.Value.ToString();
                        if (!"Y".Equals(msg))
                        {
                            throw new Exception("确认出库单时异常:" + msg);
                        }
                    }
                }
            }

            //确认货位时，生成备料库入库单，备料库出库单，高架库入库单，高架库出库单
            #region  
            spParams.Clear();
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_orderno", orderno, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_msg", DBNull.Value.ToString(), ParameterDirection.Output)); 
            outputs = DBHelper.RunProc(inboundsql, spParams, "", DBHelper.OperatedNumType.ZeroOrMore);
            string msg2 = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                                      .FirstOrDefault()?.Value.ToString();

            if (!"Y".Equals(msg2))
            {
                throw new Exception("确认出库单时异常:" + msg2);
            }
            #endregion
            return true;

        }

        /// <summary>
        /// 检查是否跨越
        /// </summary>
        /// <param name="warehouseid"></param>
        /// <param name="createdate"></param>
        /// <returns></returns>
        public bool IsOverMonth(string warehouseid, DateTime createdate)
        {
            string sql = "select root.GetBalanceInfo(to_date(@v_createdate,'yyyy-MM-dd HH24:mi:ss'),@v_warehouseid) from dual";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("v_createdate", DBHelper.GetDateStr(createdate));
            lstParams.Add("v_warehouseid", warehouseid);
           
            return DBHelper.SelectOne(sql,lstParams) == "0";
        }

        /// <summary>
        /// 获取货位对应的烟包条码信息
        /// </summary>
        /// <param name="locationid"></param>
        /// <param name="itemid"></param>
        /// <param name="speci">规格</param>
        /// <param name="weight">整个货位库存的总重量</param>
        /// <returns></returns>
        public List<string> GetLocationBarcode(string locationid, string itemid, decimal speci, decimal weight)
        {
            List<string> list = new List<string>();
            if (weight <= 0)
                return list;
            string sql = @"select * from root.hf_t_location where locationid = @locationid";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("locationid", locationid);
            DataTable dt = DBHelper.Select(sql, lstParams, "", false);

            if (dt.Rows.Count == 0)
            {
                return list;
            }
            string containerid = dt.Rows[0]["containerid"].ToString();
            sql = @"select weight from root.hf_t_containerdetail 
                           where isbarcode='0'  and itemid= @itemid and containerid= @containerid ";
            lstParams.Clear();
            lstParams.Add("itemid", itemid);
            lstParams.Add("containerid", containerid);

            if (speci > 0)
            {
                sql += " and speci= @speci ";
                lstParams.Add("speci", speci.ToString());
            }
            string strWeight = DBHelper.SelectOne(sql, lstParams, "", false);
            decimal tempWeight = decimal.Parse(string.IsNullOrEmpty(strWeight)?"0":strWeight);
            if (tempWeight == weight)
            {
                sql = @"select itembarcode,weight from root.HF_T_barcode
                         where state='1' and itemid= @itemid  and containerid= @containerid ";
                lstParams.Clear();
                lstParams.Add("itemid", itemid);
                lstParams.Add("containerid", containerid);
                if (speci > 0)
                {
                    sql += " and weight= @weight ";
                    lstParams.Add("weight", speci.ToString());
                }
                dt = DBHelper.Select(sql, lstParams, "", false);
                decimal totalWeight = 0;
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        list.Add(item[0].ToString());
                        totalWeight += decimal.Parse(item[1].ToString());
                    }
                    if (totalWeight != weight)
                    {
                        return new List<string>();
                        // throw new Exception("库存错误，条码重量与库存重量不相等！货位ID：" + locationid + " 物料ID：" + itemid);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 执行存过
        /// </summary>
        /// <param name="spName">存过名称</param>
        /// <param name="orderNO">单号</param>
        public static void doAdjustOrder(string spName, string orderNO)
        {
            List<Tuple<string, string, ParameterDirection>> spParams = new List<Tuple<string, string, ParameterDirection>>();
            spParams.Add(new Tuple<string, string, ParameterDirection>("I_ORDERNO", orderNO, ParameterDirection.Input));
            DBHelper.RunProc(spName, spParams, "", DBHelper.OperatedNumType.ZeroOrMore);

        }
        /// <summary>
        /// 执行单据业务
        /// </summary>
        /// <param name="spName">存过名称</param>
        /// <param name="orderNO">单据号</param>
        /// <returns></returns>
        public static void doAdjustOrder(string spName, string orderno, string outOrderno)
        {

            List<Tuple<string, string, ParameterDirection>> spParams = new List<Tuple<string, string, ParameterDirection>>();
            spParams.Add(new Tuple<string, string, ParameterDirection>("I_ORDERNO", orderno, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("I_OUTORDERNO", outOrderno, ParameterDirection.Input));
            DBHelper.RunProc(spName, spParams, "", DBHelper.OperatedNumType.ZeroOrMore);
        }


        /// <summary>
        /// 确认货位分配(分批次确认)
        /// </summary>
        /// <param name="orderno"></param>
        /// <param name="loginUser">用户信息</param>
        /// <returns></returns>
        public bool ConfirmLocationInfo2(string orderno, string isbarcode, decimal batchnum)
        {
            CheckOutboundDetail(orderno);
            string sql = @"select CREATEDATE,warehouseid from root.hf_t_outboundorder_header where order_no= @order_no and rownum=1 ";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("order_no", orderno);
            DataTable dt = DBHelper.Select(sql, lstParams);
            DataRow row = dt.Rows[0];

            if (!IsOverMonth(row["warehouseid"].ToString(), DateTime.Parse(row["createdate"].ToString())))
            {
                throw new Exception( "出库日期只能为当前月份，不能跨月！");
                
            }
           bool re = confirm_locationinfonew2( orderno, isbarcode, batchnum);
            return re;
        }
        /// <summary>
        /// 检查出库明细是否全部已经分配货位
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        private bool CheckOutboundDetail(string orderno) {
            string sql = @"select count(1)
                        from hf_outboundorder_detail_view v where v.order_no= @order_no  and v.loactionid is null ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("order_no", orderno);
            int erroRows = int.Parse(DBHelper.SelectOne(sql, lstParam));
            if (erroRows > 0) {
                throw new Exception("货位不能为空");
            }
            return true;
        }
        /// <summary>
        /// 确认货位分配(生产领用，无码烟分批次确认货位并入库)
        /// </summary>
        /// <param name="orderno">出库单号</param>
        /// <param name="isbarcode">有无条码：0=有码，1=无码</param>
        /// <param name="batchnum">批次数</param>
        /// <returns></returns>
        private bool confirm_locationinfonew2(string orderno, string isbarcode, decimal batchnum)
        {
            string msg = string.Empty;
            //存储过程
            string spname = "ahzy_outbound_pg.confirm_locationinfonew2@hfchk_link";//修改货位信息

            List<Tuple<string, string, ParameterDirection>> spParams = new List<Tuple<string, string, ParameterDirection>>();
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_orgid", "", ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_orderno", orderno, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_isbarcode", isbarcode, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_batchnum", batchnum.ToString(), ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_userid", Context.UserInfo.UserId, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_msg", DBNull.Value.ToString(), ParameterDirection.Output));
            var outputs = DBHelper.RunProc(spname, spParams, "", DBHelper.OperatedNumType.OneOrMore);

            msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                    .FirstOrDefault()?.Value.ToString();

            if (!"Y".Equals(msg))
            {
                throw new Exception(msg);
            }

            return true;
        }
        /// <summary>
        /// 获取目前最大的烟包号
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public int GetMaxBoxNumber(string itemId, string code)
        {
            string sql = "SELECT MAX(BOXNUMBER) FROM root.hf_t_barcode t WHERE t.itemid = '" + itemId + "'";
            if (!string.IsNullOrEmpty(code))
                sql += " AND t.ITEMBARCODE LIKE '" + code + "%' ";

            string obj = DBHelper.SelectOne(sql, new Dictionary<string, object>(), "", false);
            if (!string.IsNullOrEmpty(obj))
            {
                return Convert.ToInt32(obj);
            }
            return 0;
        }
        /// <summary>
        /// 按出库单生成烟包条码
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns>打印批次号</returns>
        public string MakeNewBoxNumberByOrder(string orderno)
        {
            /*string sql = @"select v.order_no,v.order_id,v.itemid,v.itemcode,v.itemspeci,v.qty,1 as  printall
                            from root.HF_OUTBOUNDORDER_DETAIL_VIEW v 
                            where v.ORDER_NO=@order_no";*/
            // lyl 修改打印条码时从出库明细中的包规作为打印标签的包规
            string sql = @"select   v.order_no,v.order_id,v.itemid,v.itemcode,v.specifications_kg as itemspeci,
                            v.qty,1 as  printall,v.ID as orderdetailid
                            from root.hf_outboundorder_detail_view v  
                            where v.ORDER_NO=@order_no";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("order_no", orderno);
            DataTable dt = DBHelper.Select(sql, lstParams, "", false);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("无此出库单信息");
            }
            List<ApplyNewbarcodein> lstDetails = DBHelper.ConvertTableToObject<ApplyNewbarcodein>(dt);
            return MakeNewBoxNumber(lstDetails);
        }


        /// <summary>
        /// 按订单+项次打印烟包条码
        /// <br/>
        /// 存在BUG：拆单会造成orderno_orderid多笔记录。
        /// 所以修改应该调用MakeNewBoxNumberByOrder按出库明细打印烟包条码
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns>打印批次号</returns>
        public string MakeNewBoxNumberByOrder(MakeNewbarckdeIn applyInfo)
        {
            /*string sql = @"select v.order_no,v.order_id,v.itemid,v.itemcode,v.itemspeci,v.qty,1 as  printall
                            from root.HF_OUTBOUNDORDER_DETAIL_VIEW v 
                            where v.ORDER_NO=@order_no";*/
            // lyl 修改打印条码时从出库明细中的包规作为打印标签的包规
            string sql = @" select   v.order_no,v.order_id,v.itemid,v.itemcode,"+ applyInfo.speci + @" as itemspeci,"+ applyInfo.copies + @" as qty,1 as  printall
                            from root.hf_outboundorder_detail_view v  
                            where v.ORDER_NO=@order_no and v.order_id=@order_id ";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("order_no", applyInfo.order_no);
            lstParams.Add("order_id", applyInfo.order_id);
            DataTable dt = DBHelper.Select(sql, lstParams, "", false);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("无此出库单信息");
            }
            List<ApplyNewbarcodein> lstDetails = DBHelper.ConvertTableToObject<ApplyNewbarcodein>(dt);
            return MakeNewBoxNumber(lstDetails);
        }
        /// <summary>
        /// 按出库单明细打印烟包条码
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns>打印批次号</returns>
        public string MakeNewBoxNumberByOrder(MakeNewbarckde2In applyInfo)
        {
            /*string sql = @"select v.order_no,v.order_id,v.itemid,v.itemcode,v.itemspeci,v.qty,1 as  printall
                            from root.HF_OUTBOUNDORDER_DETAIL_VIEW v 
                            where v.ORDER_NO=@order_no";*/
            // lyl 修改打印条码时从出库明细中的包规作为打印标签的包规
            string sql = @" select   v.id as orderdetailid,v.order_no,v.order_id,v.itemid,v.itemcode," + applyInfo.speci + @" as itemspeci," + applyInfo.copies + @" as qty,1 as  printall
                            from root.hf_outboundorder_detail_view v  
                            where v.id=@orderdetailid ";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("orderdetailid", applyInfo.orderdetailid); 
            DataTable dt = DBHelper.Select(sql, lstParams, "", false);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("无此出库单信息");
            }
            List<ApplyNewbarcodein> lstDetails = DBHelper.ConvertTableToObject<ApplyNewbarcodein>(dt);
            return MakeNewBoxNumber(lstDetails);
        }
        /// <summary>
        /// 按出库单明细打印烟包条码
        /// </summary>
        /// <param name="lstApplyInfo">多笔出库单明细</param>
        /// <returns>打印批次号</returns>
        public string MakeNewBoxNumberByOrder(List<MakeNewbarckde2In> lstApplyInfo)
        {
            /*string sql = @"select v.order_no,v.order_id,v.itemid,v.itemcode,v.itemspeci,v.qty,1 as  printall
                            from root.HF_OUTBOUNDORDER_DETAIL_VIEW v 
                            where v.ORDER_NO=@order_no";*/
            // lyl 修改打印条码时从出库明细中的包规作为打印标签的包规
            string printBatchId = SnowflakeID.Guid().ToString();
            foreach (MakeNewbarckde2In applyInfo in lstApplyInfo)
            {
                string sql = @" select   v.id as orderdetailid,v.order_no,v.order_id,v.itemid,v.itemcode,v.SPECIFICATIONS_KG as itemspeci,v.QTY as qty,1 as  printall
                            from root.hf_outboundorder_detail_view v  
                            where v.id=@orderdetailid ";
                Dictionary<string, object> lstParams = new Dictionary<string, object>();
                lstParams.Add("orderdetailid", applyInfo.orderdetailid);
                DataTable dt = DBHelper.Select(sql, lstParams, "", false);
               
                List<ApplyNewbarcodein> lstDetails = DBHelper.ConvertTableToObject<ApplyNewbarcodein>(dt);
                MakeNewBoxNumber(printBatchId,lstDetails);
            }
            return printBatchId;
        }

        /// <summary>
        /// 按物料打印指定份数的标签
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="weight"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public string MakeNewBoxNumber(string itemid,decimal weight,int qty)
        {
            NtiCloud.Service.WMS.Base.Item item = new Base.Item();
            NtiCloud.Service.LPS.Model.ItemModel im= item.GetItem(itemid);
            if (im == null) {
                throw new Exception("此物料不存在");
            }

            //return MakeNewBoxNumber(lstDetails);
            return "";
        }

        /// <summary>
        /// 批量生成条码
        /// </summary>
        /// <param name="applyinfo"></param>
        /// <returns></returns>
        public string MakeNewBoxNumber(List<ApplyNewbarcodein> applyinfos)
        {
            string printbatchid = SnowflakeID.Guid().ToString();
            foreach (ApplyNewbarcodein applyinfo in applyinfos)
            {
                GetNewBoxNumber(applyinfo, printbatchid);
            }

            return printbatchid;
        }
        /// <summary>
        /// 批量生成条码
        /// </summary>
        /// <param name="printBatchId"></param>
        /// <param name="applyinfos"></param>
        public void MakeNewBoxNumber(string printBatchId,List<ApplyNewbarcodein> applyinfos)
        {            
            foreach (ApplyNewbarcodein applyinfo in applyinfos)
            {
                GetNewBoxNumber(applyinfo, printBatchId);
            }
        }
        /// <summary>
        /// 单个出库单行生成新的烟包条码
        /// </summary>
        /// <returns></returns>
        private string GetNewBoxNumber(ApplyNewbarcodein applyinfo, string printbatchid)
        {


            bool printAll = applyinfo.printall > 0;

            string enterpriseCode = "19000001";
            string beltlineCode = "1";
            string classesCode = "1";
            string designatorCode = "0";
            string productionDate = DateTime.Now.ToString("yyMMdd");

            List<ORDERBARCODE> list = new List<ORDERBARCODE>();
            string snPart1 = "90" + enterpriseCode + beltlineCode + classesCode + designatorCode + DateTime.Now.ToString("yyMMdd") + applyinfo.itemcode;

            int boxNumber = GetMaxBoxNumber(applyinfo.itemid, snPart1) + 1;

            if (!printAll)
            {
                /*string itemBarcode = snPart1 + (boxNumber + applyinfo.qty - 1).ToString().PadLeft(5, '0');
                //只插入最大的箱号，减少数据量
                WMSInterface.Entity.Barcode entity = new WMSInterface.Entity.Barcode()
                {
                    Id = Guid.NewGuid().ToString("N").ToUpper(),
                    UserId = Utils.CurUserInfo.Account,
                    ItemId = itemID,
                    State = -1,
                    CreateTime = DateTime.Now,
                    ItemBarcode = itemBarcode,
                    BoxNumber = boxNumber + qty - 1,
                    GrossWeight = wgt + 18,
                    Weight = wgt,
                    WarterRate = 12,
                    PrinterLeaf = "安徽中烟加工一组",
                    Speci = wgt
                };

                Result tmp = Utils.GetFactory<IBasicDatas>().AddBarcode(entity);*/
            }
            else
            {
                bool re = true;
                for (var i = 0; i < applyinfo.qty; i++)
                {
                    string itemBarcode = snPart1 + (boxNumber + i).ToString().PadLeft(5, '0');
                    re = AddNewBarcode(itemBarcode, (boxNumber + i).ToString(), applyinfo.itemspeci, applyinfo.itemid);
                    if (!re)
                    {
                        throw new Exception("保存新生成条码信息出错");
                    }
                    re = AddNewBarcodeToLog(applyinfo.order_no, applyinfo.order_id,applyinfo.orderdetailid, itemBarcode, printbatchid);
                    if (!re)
                    {
                        throw new Exception("记录出库单与条码关系失败");
                    }
                }
            }
            return boxNumber.ToString();
        }

        /// <summary>
        /// 新增单个条码
        /// </summary>
        /// <param name="itembarcode"></param>
        /// <param name="boxno"></param>
        /// <param name="gw"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        private bool AddNewBarcode(string itembarcode, string boxno, decimal gw, string itemid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into root.hf_t_barcode(");
            strSql.Append("ID,CONTAINERID,OUTBOUNDID,WEIGHT,GROSSWEIGHT,WARTERRATE,PRINTERLEAF,ITEMID,SPECI,ITEMBARCODE,BOXNUMBER,USERID,CREATETIME,STATE");
            strSql.Append(") values (");
            strSql.Append("@ID,@CONTAINERID,@OUTBOUNDID,@WEIGHT,@GROSSWEIGHT,@WARTERRATE,@PRINTERLEAF,@ITEMID,@SPECI,@ITEMBARCODE,@BOXNUMBER,@USERID,sysdate,@STATE");
            strSql.Append(") ");

            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("ID", SnowflakeID.Guid().ToString());
            lstParams.Add("CONTAINERID", "");
            lstParams.Add("OUTBOUNDID", "");
            lstParams.Add("WEIGHT", gw.ToString());
            lstParams.Add("GROSSWEIGHT", (gw + 18).ToString());
            lstParams.Add("WARTERRATE", "12");
            lstParams.Add("PRINTERLEAF", "安徽中烟加工一组");
            lstParams.Add("ITEMID", itemid);
            lstParams.Add("SPECI", gw.ToString());
            lstParams.Add("ITEMBARCODE", itembarcode);
            lstParams.Add("BOXNUMBER", boxno);
            lstParams.Add("USERID", Context.UserInfo.UserId);
            lstParams.Add("STATE", "-1");
            return DBHelper.RunSql(strSql.ToString(), lstParams) > 0;

        }
        /// <summary>
        /// 件货位分配打印的条码增加包打印历史表
        /// </summary>
        /// <param name="truckorder">出库单</param>
        /// <param name="truckorderid">出库单项次</param>
        /// <param name="orderdetailid">出库单明细ID</param>
        /// <param name="barcode">条码</param>
        /// <param name="printbatchid">打印批次号</param>
        /// <returns></returns>
        private bool AddNewBarcodeToLog(string truckorder, string truckorderid,string orderdetailid, string barcode, string printbatchid)
        {
            string sql = @"insert into root.hf_t_barcodeprintbyorder(id, orderno, orderid,orderdetailid, barcode,printbatchid, createdate, state)
                            values
                            (@id, @orderno, @orderid,@orderdetailid, @barcode,@printbatchid, sysdate, -1 )";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("id", SnowflakeID.Guid().ToString());
            lstParams.Add("orderno", truckorder);
            lstParams.Add("orderid", truckorderid);
            lstParams.Add("orderdetailid", orderdetailid);
            lstParams.Add("barcode", barcode);
            lstParams.Add("printbatchid", printbatchid);
            return DBHelper.RunSql(sql, lstParams) > 0;
        }

        /// <summary>
        /// 更新出库单打印状态
        /// </summary>
        /// <param name="ordernos">多个单号用,号隔开</param>
        /// <returns></returns>
        public bool UpdatePrintStatus(string order_nos)
        {  string sql = @"update root.hf_t_outboundorder_header 
                            set printstatus='已打印',
                            printdate=sysdate,printuser='" + Context.UserInfo.UserId + @"'  
                          where order_no in('"+ order_nos.Replace(",", "','") + "')";
            Dictionary<string, object> lstParams = new Dictionary<string, object>(); 
            return DBHelper.RunSql(sql, lstParams, "更新打印状态") > 0;
        }

        /// <summary>
        /// 生成分组打印数据并插入到临时抱
        /// </summary>
        /// <param name="parms"></param>
        /// <returns>打印批次号</returns>
        public string GenerateDetailGroupData(OutboundDetailGroupIn parms) {
            DeletePriorityTempData();
            DataTable dt = GernerateDetailGroupData(parms);
            string batchid = SnowflakeID.Guid().ToString();
            InsertDataToDetailGroupRptTmp(dt, batchid);
            return batchid;
        }
        /// <summary>
         /// 删除该用户之间的临时报表数据
         /// </summary>
        private void DeletePriorityTempData() {
            string sql = "delete from root.hf_t_temp_detailgrouprpt where createby=@createby";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("createby", Context.UserInfo.UserId);
            DBHelper.RunSql(sql, lstParam, "删除该用户之间的临时报表数据", DBHelper.OperatedNumType.ZeroOrMore);
        }
        /// <summary>
        /// 获取分组报表数据
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        private DataTable GernerateDetailGroupData(OutboundDetailGroupIn parms)
        {
            string spname = "root.HF_P_OutboundReportByGroup";
            List<Tuple<string, string, ParameterDirection>> spParams = new List<Tuple<string, string, ParameterDirection>>();
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_orderno", parms.order_no, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_busstype", parms.service_type, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_dccode", parms.warehouseid, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_dateto", parms.dateto, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_datefrom", parms.datefrom, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_reforderno", parms.ref_orderno, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_state", parms.status, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_inorgid", parms.rcvorgid, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_itemname", parms.itemname, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_productname", parms.productname, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_username", parms.username, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_reccode", parms.arr_warehouseid, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("v_cur", "", ParameterDirection.Output));
            DataSet ds = DBHelper.RunProcDataSet(spname, spParams);
            return ds.Tables[0];
        }
        /// <summary>
        /// 将数据插入到报表临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="batchid"></param>
        /// <returns></returns>
        private void InsertDataToDetailGroupRptTmp(DataTable dt, string batchid) {
            string sql = @"insert into root.hf_t_temp_detailgrouprpt(
                            id,itemname, itemid, provincename, originname, kindname, gradename, givenyear, actualqty, 
                            actualweight, rf_qty, rf_weight, actualdan, batchid,createby)
                        values(@id,@itemname, @itemid, @provincename, @originname,@kindname, @gradename,@givenyear, @actualqty, 
                            @actualweight, @rf_qty, @rf_weight, @actualdan, @batchid,@createby)";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            foreach (DataRow dr in dt.Rows) {
                lstParam.Clear();
                lstParam.Add("id", SnowflakeID.Guid().ToString());
                lstParam.Add("itemname", dr["itemname"].ToString());
                lstParam.Add("itemid", dr["itemid"].ToString());
                lstParam.Add("provincename", dr["provincename"].ToString());
                lstParam.Add("originname", dr["originname"].ToString());
                lstParam.Add("kindname", dr["kindname"].ToString());
                lstParam.Add("gradename", dr["gradename"].ToString());
                lstParam.Add("givenyear", dr["givenyear"].ToString());
                lstParam.Add("actualqty", dr["actualqty"].ToString());
                lstParam.Add("actualweight", dr["actualweight"].ToString());
                lstParam.Add("rf_qty", dr["rf_qty"].ToString());
                lstParam.Add("rf_weight", dr["rf_weight"].ToString());
                lstParam.Add("actualdan", dr["actualdan"].ToString());
                lstParam.Add("batchid", batchid);
                lstParam.Add("createby", Context.UserInfo.UserId);
                DBHelper.RunSql(sql, lstParam);
            }             
        }
        /// <summary>
        /// 撤销已扫描的条码
        /// </summary>
        /// <param name="orderno">出库单号</param> 
        /// <param name="orderdetailid">出库明细ID</param>
        /// 
        /// <returns></returns>
        public bool RestoreScanedBarcode(string orderno,string orderdetailid) {
            #region 还原从系统打印条码的扫描状态
            string sql = @"update root.hf_t_barcodeprintbyorder b set b.STATE=-1 
                            where b.orderno=@orderno
                                and b.orderdetailid = @orderdetailid  ";
            Dictionary<string,object> lstParams=new Dictionary<string, object>();
            lstParams.Add("orderno", orderno);
            lstParams.Add("orderdetailid", orderdetailid); 
            DBHelper.RunSql(sql,lstParams,"删除已扫描条码", DBHelper.OperatedNumType.ZeroOrMore);
            #endregion

            #region 删除已同步到高架库还未生成入库信息的条码
            sql = @"delete from &DataSource&wms_d_chkshipperbarcode c where exists(
                           select 1 from root.hf_t_barcodeshipedrelation bs where c.truckorder=bs.OUTORDERNO 
                           and c.item_barcode=bs.BARCODE and bs.OUTORDERNO=@orderno and bs.OUTDETAILID=@orderdetailid) 
                    and c.state=-1";
            lstParams.Clear();
            lstParams.Add("orderno", orderno);
            lstParams.Add("orderdetailid", orderdetailid);
            DBHelper.RunSql(sql, lstParams, "删除已同步到高架库还未生成入库信息的条码", DBHelper.OperatedNumType.ZeroOrMore);
            #endregion

            #region 删除出库明细&条码的绑定关系
            sql = @"delete from root.hf_t_barcodeshipedrelation bs 
                    where bs.OUTORDERNO= @orderno and bs.OUTDETAILID= @orderdetailid";
            lstParams.Clear();
            lstParams.Add("orderno", orderno);
            lstParams.Add("orderdetailid", orderdetailid);
            DBHelper.RunSql(sql, lstParams, "删除出库明细&条码的绑定关系", DBHelper.OperatedNumType.ZeroOrMore);
            #endregion

            #region 还原出库行扫描状态
            sql = @"update root.hf_t_outboundorder_detail d set d.ISSCANEDBARCODE='否',d.SCANEDQTY=0
                    where d.ORDER_NO=@order_no and d.ID=@orderdetailid ";
            lstParams.Clear();
            lstParams.Add("order_no", orderno);
            lstParams.Add("orderdetailid", orderdetailid);
            DBHelper.RunSql(sql,lstParams);
            #endregion

            #region 还原出库单扫描状态
            sql = @"update root.hf_t_outboundorder_header h set h.ISSCANEDBARCODE='否'
                    where h.ORDER_NO=@order_no";
            lstParams.Clear();
            lstParams.Add("order_no", orderno);
            DBHelper.RunSql(sql, lstParams);
            #endregion

            return true;
        }

        public string MakeCtnCompanent(List<CombineCtnIn> lst) {
            if (lst == null && lst.Count == 0) {
                throw new Exception("请传入正确的合包参数");
            }
            string combineno = SnowflakeID.Guid().ToString();
            foreach (CombineCtnIn combine in lst) {
                MakeCtnCompanent(combineno, combine.PlanNo, combine.OutOrderNo, combine.OutDetailId);
            }
            return combineno;
        }
        /// <summary>
        /// 合箱
        /// <br/>
        /// 多个出库明细组合成一个条码
        /// </summary>
        /// <param name="combinebatch">合包批次号</param>
        /// <param name="orderno">备料单号</param>
        /// <param name="planorderno">出库单号</param>
        /// <param name="orderdetailid">出库单明细</param>
        /// <returns>返回合包批次</returns>
        private bool MakeCtnCompanent(string combinebatch, string planorderno, string orderno, string orderdetailid) {
            if (!CheckCanCompanent(planorderno, orderno, orderdetailid)) {
                throw new Exception(orderdetailid + "已经存在合箱，不能再合箱");
            }
            string sql = @"insert into barcodecombinebyorder
                            (id, planorderno, outorderno, outorderdetailid, combinebatchno,
                             state, remarks, createby, createdate, editby, editdate)
                            values
                            (@id, @planorderno, @outorderno, @outorderdetailid, @combinebatchno,
                             1, '', @createby, sysdate, '', sysdate)
                            ";

            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("id",SnowflakeID.Guid().ToString());
            lstParams.Add("planorderno", planorderno);
            lstParams.Add("outorderno", orderno);
            lstParams.Add("outorderdetailid", orderdetailid);
            lstParams.Add("combinebatchno", combinebatch);
            lstParams.Add("createby", Context.UserInfo.UserId);
            DBHelper.RunSql(sql, lstParams);
            return true; 
        }
        /// <summary>
        /// 检查出库明细行是否允许合箱
        /// <br/>
        /// 已存在合箱的明细不允许再合箱
        /// </summary>
        /// <param name="planorderno"></param>
        /// <param name="orderno"></param>
        /// <param name="orderdetailid"></param>
        /// <returns></returns>
        private bool CheckCanCompanent(string planorderno, string orderno, string orderdetailid) {
            string sql = @"select count(1) from barcodecombinebyorder c 
                         where c.planorderno = @planorderno and c.outorderno = @outorderno
                               and c.outorderdetailid = @outorderdetailid and c.state = 1";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("planorderno", planorderno);
            lstParams.Add("outorderno", orderno);
            lstParams.Add("outorderdetailid", orderdetailid);
            return int.Parse(DBHelper.SelectOne(sql, lstParams)) == 0;
        }


    }
}

