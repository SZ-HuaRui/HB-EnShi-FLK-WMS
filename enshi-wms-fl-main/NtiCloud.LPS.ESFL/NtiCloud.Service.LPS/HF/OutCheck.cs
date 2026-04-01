using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NtiCloud.Service.WMS.HF
{
   public  class OutCheck
    {
        /// <summary>
        /// 新增出库质检单
        /// </summary>
        /// <param name="OutCheckHeader"></param>
        /// <param name="loginUser">用户信息</param>
        /// <returns></returns>
        public bool InsertOutCheck(NtiCloud.Model.WMS.AddOutCheckHeaderIn OutCheckInfo)
        {
            string msg = string.Empty;
            int i = 0;
            //存储过程
            string sql = "ahzy_outbound_pg.add_outboundcheck_info@hfchk_link";
            string upsql = "ahzy_outbound_pg.up_checkstateinfo@hfchk_link";

            List<Tuple<string, string, ParameterDirection>> spParams = new List<Tuple<string, string, ParameterDirection>>();
            List<IDbDataParameter> outputs = null;
            foreach (NtiCloud.Model.WMS.AddOutCheckDetailIn detailinfo in OutCheckInfo.lstDetail)
            {
                spParams.Clear();
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_orgid", OutCheckInfo.orgid, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_orderno", OutCheckInfo.checkno, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_orderid", detailinfo.order_id, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_reforderno", OutCheckInfo.order_no, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_busstype", OutCheckInfo.service_type, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_remark", OutCheckInfo.remark, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_itemid", detailinfo.itemid, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_itemname", detailinfo.itemname, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_qty", detailinfo.qty.ToString(), ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_weight", detailinfo.weight.ToString(), ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_specificationskg", detailinfo.specifications_kg.ToString(), ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_locationid", detailinfo.locationid, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_actualqty", detailinfo.checkqty.ToString(), ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_actualweight", detailinfo.checkweight.ToString(), ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_appearance", detailinfo.appearance, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_humidity", detailinfo.humidity, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_temperature", detailinfo.temperature, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_mildew", detailinfo.mildew, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_pest", detailinfo.pest, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_oilpressure", detailinfo.oilpressure, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_compaction", detailinfo.compaction, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_color", detailinfo.color, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_result", detailinfo.result, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_createdate", OutCheckInfo.create_date, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_userid", Context.UserInfo.UserId, ParameterDirection.Input));
                spParams.Add(new Tuple<string, string, ParameterDirection>("p_msg", DBNull.Value.ToString(), ParameterDirection.Output));

                outputs = DBHelper.RunProc(sql, spParams, "", DBHelper.OperatedNumType.OneOrMore);

                msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                                      .FirstOrDefault()?.Value.ToString();

                if (msg != "Y")
                {
                    throw new Exception(msg);
                } 
                if (detailinfo.result == "不合格")
                {
                    i++;
                } 
            }
            spParams.Clear();
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_checknumber", OutCheckInfo.checkno, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_orgid", OutCheckInfo.orgid, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_orderno", OutCheckInfo.order_no, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_result", i > 0 ? "不合格" : "合格", ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_userid", Context.UserInfo.UserId, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_msg", DBNull.Value.ToString(), ParameterDirection.Output));
            outputs = DBHelper.RunProc(upsql, spParams, "", DBHelper.OperatedNumType.OneOrMore);
            msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                                  .FirstOrDefault()?.Value.ToString();  
            if (msg != "Y")
            {
                throw new Exception(msg);
            }

            return true;
        }

        /// <summary>
        /// 确认质检信息
        /// </summary> 
        /// <param name="orderno">质检单号</param>
        /// <returns></returns>
        public bool UpdCheckConfirm(string orderno)
        {
            //存储过程
            string msg = string.Empty;
            string spname = "ahzy_outbound_pg.update_checkconfirm@hfchk_link";
            List<Tuple<string, string, ParameterDirection>> spParams = new List<Tuple<string, string, ParameterDirection>>();
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_orderno", orderno, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_userid", Context.UserInfo.UserId, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("p_msg", "", ParameterDirection.Output));

            var outputs = DBHelper.RunProc(spname, spParams, "确认质检信息", DBHelper.OperatedNumType.OneOrMore);
            msg = outputs.Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == "p_msg")
                              .FirstOrDefault()?.Value.ToString();
            if (msg != "Y")
            {
                throw new Exception(msg);
            }

            AddCheckHistory(orderno);

            return true;

        }
        /// <summary>
        /// 增加质检确认日志
        /// </summary>
        /// <param name="checkno">质检单号</param>
        /// <returns></returns>
        private string AddCheckHistory(string checkno)
        {
            string msg = string.Empty;
            string sql = @"select a.id,
                                   a.order_no,
                                   t.isbarcode,
                                   c.APPEARANCE,
                                   c.HUMIDITY,
                                   c.TEMPERATURE,
                                   c.MILDEW,
                                   c.PEST,
                                   c.OILPRESSURE,
                                   c.COMPACTION,
                                   c.color
                              from root.hf_t_outboundorder_header t
                             inner join root.hf_t_outboundorder_detail a
                                on t.order_no = a.order_no
                             inner join root.hf_t_outbound_check c
                                 on t.checknumber =c.order_no
                               and a.itemid = c.itemid
                           where t.checknumber= @checknumber ";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("checknumber", checkno);
            DataTable dt = DBHelper.Select(sql, lstParams, "", false);  
            foreach (DataRow row in dt.Rows)
            {
                sql = @"select t.id,t.containerid from root.hf_t_containerhistory t 
                        where rownum=1 and t.sourcebillid='" + row["id"].ToString() + "' ";
                lstParams.Clear();
                DataTable dt2 = DBHelper.Select(sql, lstParams,"",false);
                if (dt2.Rows.Count == 0)
                {
                    continue;
                }
                string hisID = dt2.Rows[0]["id"].ToString();
                string containerid = dt2.Rows[0]["containerid"].ToString();
                string checkresult = GetCheckResult(row); 
                AddContainerHistoryCheck(hisID, "7", row["order_no"].ToString(), containerid,"","","", checkresult);

                if (int.Parse(row["isbarcode"].ToString()) == 0)//有码
                {
                    sql = @"insert into root.hf_t_barcodedetail
                           (id, itembarcode, containerid, actiontype, inventoryreason, sourcebillid, orderno,createtime ,userid,CheckResult)
                    select sys_guid(), t.itembarcode,@containerid,7,0, @sourcebillid, @orderno, sysdate,@userid, @checkresult
                                    from barcodedetail t
                                    where t.sourcebillid = @id ";
                    lstParams.Clear();
                    lstParams.Add("containerid", containerid);
                    lstParams.Add("sourcebillid", row["order_no"].ToString());
                    lstParams.Add("orderno", row["order_no"].ToString());
                    lstParams.Add("userid", Context.UserInfo.UserId);
                    lstParams.Add("checkresult", checkresult);
                    lstParams.Add("id", row["id"].ToString());
                    DBHelper.RunSql(sql,lstParams);
                    lstParams.Clear();
                }
            }

            return msg;
        }
        /// <summary>
        /// 新增质检记录
        /// </summary>
        /// <param name="drCheckHistory"></param>
        /// <returns></returns>
        private bool AddContainerHistoryCheck(string hisid,string ordertype,string orderno,
                string containerid,string inventoryreason,string target,string orderid,string checkresult)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into root.hf_t_CONTAINERHISTORYCHECK(");
            strSql.Append("ID,HISTORYID,ORDERTYPE,ORDERNUMBER,CREATEDATE,CREATEUSER,ContainerID,InventoryReason,Target,OrderID,CheckResult");
            strSql.Append(") values (");
            strSql.Append("@ID,@HISTORYID,@ORDERTYPE,@ORDERNUMBER,sysdate,@CREATEUSER,@ContainerID,@InventoryReason,@Target,@OrderID,@CheckResult");
            strSql.Append(") ");

            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("ID", SnowflakeID.Guid().ToString());
            lstParams.Add("HISTORYID", hisid);
            lstParams.Add("ORDERTYPE", ordertype);
            lstParams.Add("ORDERNUMBER", orderno); 
            lstParams.Add("CREATEUSER", Context.UserInfo.UserId);
            lstParams.Add("ContainerID", containerid);
            lstParams.Add("InventoryReason", inventoryreason);
            lstParams.Add("Target", target);
            lstParams.Add("OrderID", orderid);
            lstParams.Add("CheckResult", checkresult); 
            return DBHelper.RunSql(strSql.ToString(), lstParams) > 0;

        }
        /// <summary>
        /// 检查质检信息是否全部合格
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string GetCheckResult(DataRow row)
        {
            string msg = "";
            if (row == null)
                return msg;

            if (row["APPEARANCE"].ToString() != "无" ||
                row["MILDEW"].ToString() != "无" ||
                row["PEST"].ToString() != "无" ||
                row["HUMIDITY"].ToString() == "手感偏高" ||
                row["COMPACTION"].ToString() != "正常" ||
                row["OILPRESSURE"].ToString() != "正常" ||
                row["TEMPERATURE"].ToString() == "手感偏高" ||
                row["color"].ToString() != "正常")

                return "1";

            return msg;
        }
    }
}
