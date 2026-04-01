using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WMSInterface;
using System.Collections;
using WMSInterface.Entity50;
using WMSInterface.EntityDefination;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace NTI.WMS.CallTerminalSystem.DAL
{
    public class ReturnPalletDAL
    {
        /// <summary>
        /// 退空托盘
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="dockNumber"></param>
        /// <param name="hasLGV"></param>
        /// <param name="runsType"></param>
        /// <returns></returns>
        public static string ClientReturnNullPallet(string url, string sessionKey, string warehouseId, string dockId,
            string hasLGV, string shiftId, string teamId, string productPlanOutId, string machine, string token)
        {
            url += "api/wms/MachineMaterialService/ClientReturnNullPallet";
            JObject jo = new JObject();
            jo.Add("machine", machine);
            jo.Add("dock", dockId);
            jo.Add("productPlanOutId", productPlanOutId);
            jo.Add("shiftId", shiftId);
            jo.Add("hasAGV", hasLGV);
            jo.Add("teamId", teamId);
            string result = NitCloudService.PostGateway(url, jo.ToString(), token);
            if (string.IsNullOrEmpty(result)) throw new Exception("退空托盘调用微服务失败！");
            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                return entity["errmsg"].ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 退余料前检查站台当前是否存在未执行结束的任务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="dockNumber">站台编码</param>
        /// <param name="token">token</param>
        /// <returns>true：存在；false：不存在</returns>
        public static bool CheckDockCurTask(string url, string sessionKey, string warehouseId, string dockNumber, string token)
        {
            string sql = $"select count(t.t_id) as cnt from wms.wms_b_task t where (t.dest_addr_text='{dockNumber}' or t.src_addr_text='{dockNumber}')  and t.task_status<>'TaskStatus_Finished' and t.ref_doc_type in ('DocType_CallTerminal','DocType_ReturnBadPallet','DocType_ClientReturnPallet','DocType_ClientReturnNullPallet') and t.act_status = '1'";
            DataTable dt = NitCloudService.GetDataTable(url, sessionKey, sql, token);
            if (dt == null || dt.Rows.Count <= 0)
            {
                return true;
            }
            int taskNum = Convert.ToInt32(dt.Rows[0]["cnt"].ToString());
            return taskNum > 0;
        }

        /// <summary>
        /// 退坏料前检查站台当前是否存在未执行结束的任务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="dockNumber">站台编码</param>
        /// <param name="token">token</param>
        /// <returns>true：存在；false：不存在</returns>
        public static bool CheckAllowed_ReturnPallet(string url, string sessionKey, string warehouseId, string dockNumber, string token)
        {
            string sql = $"select count(t.t_id) as cnt from wms.wms_b_task t where (t.dest_addr_text='{dockNumber}' or t.src_addr_text='{dockNumber}')    and t.ref_doc_type in ('DocType_CallTerminal','DocType_ReturnBadPallet','DocType_ClientReturnPallet','DocType_ClientReturnNullPallet') and t.task_status<>'TaskStatus_Finished'  and t.act_status='1'";
            DataTable dt = NitCloudService.GetDataTable(url, sessionKey, sql, token);
            if (dt == null || dt.Rows.Count <= 0)
            {
                return true;
            }
            int taskNum = Convert.ToInt32(dt.Rows[0]["cnt"].ToString());
            return taskNum > 0;
        }

        /// <summary>
        /// 退废料
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="dockNumber"></param>
        /// <param name="hasLGV"></param>
        /// <param name="runsType"></param>
        /// <returns></returns>
        public static string ReturnWasteMaterials(string url, string sessionKey, string warehouseId, string dockId,
            string hasLGV, string shiftId, string teamId, string productPlanOutId, string machine, string token)
        {
            url += "api/wms/MachineMaterialService/ReturnWasteMaterials";
            JObject jo = new JObject();
            jo.Add("machine", machine);
            jo.Add("dock", dockId);
            jo.Add("productPlanOutId", productPlanOutId);
            jo.Add("shiftId", shiftId);
            jo.Add("hasAGV", hasLGV);
            jo.Add("teamId", teamId);
            string result = NitCloudService.PostGateway(url, jo.ToString(), token);
            if (string.IsNullOrEmpty(result)) throw new Exception("退废料调用微服务失败！");
            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                return entity["errmsg"].ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 成品转移
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="dockNumber"></param>
        /// <param name="hasLGV"></param>
        /// <param name="runsType"></param>
        /// <returns></returns>
        public static string ProductTransfer(string url, string sessionKey, string warehouseId, string dockId,
            string hasLGV, string shiftId, string teamId, string productPlanOutId, string machine, string token)
        {
            url += "api/wms/MachineMaterialService/ProductTransfer";
            JObject jo = new JObject();
            jo.Add("machine", machine);
            jo.Add("dock", dockId);
            jo.Add("productPlanOutId", productPlanOutId);
            jo.Add("shiftId", shiftId);
            jo.Add("hasAGV", hasLGV);
            jo.Add("teamId", teamId);
            string result = NitCloudService.PostGateway(url, jo.ToString(), token);
            if (string.IsNullOrEmpty(result)) throw new Exception("成品转移调用微服务失败！");
            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                return entity["errmsg"].ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 退空盘+要料
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="dockNumber"></param>
        /// <param name="hasLGV"></param>
        /// <param name="runsType"></param>
        /// <returns></returns>
        public static string ReturnNullAndCallPallet(string url, out string errcode, string sessionKey, string warehouseId, string dockId,
            string hasLGV, string shiftId, string teamId, string productPlanOutId, string machine, string token)
        {
            url += "api/wms/MachineMaterialService/ReturnNullAndCallPallet";
            JObject jo = new JObject();
            jo.Add("machine", machine);
            jo.Add("dock", dockId);
            jo.Add("productPlanOutId", productPlanOutId);
            jo.Add("shiftId", shiftId);
            jo.Add("hasAGV", hasLGV);
            jo.Add("teamId", teamId);
            string result = NitCloudService.PostGateway(url, jo.ToString(), token);
            if (string.IsNullOrEmpty(result)) throw new Exception("退空托盘+要料调用微服务失败！");
            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            errcode = entity["errcode"].ToString();
            return entity["errmsg"].ToString();
        }

        /// <summary>
        /// 退余料界面，展示的托盘数据。根据站台位获取当前托盘数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="dockNumber">站台编码</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public static DataTable GetContainerDetailByDockNumber(string url, string sessionKey, string warehouseId, string machineId, string dockNumber, string token)
        {
            string sql = $"select distinct d.t_id as containerDetailId,md.dock_id as dockId, (select h.container_barcode from wms.wms_b_container_h h where h.t_id=md.container_id and h.act_status='1') as containerBarcode, (select s.section_code from wms.wms_m_section s where s.t_id=md.prod_section and s.act_status='1') as sectionCode, md.dock_name as dockName,d.item_id as itemId, d.item_name as itemName, d.spec, '0' as planQty, '0' as qty, (select iu.uom_name from wms.wms_m_item_uom iu where iu.t_id = d.uom_qty_id  and iu.act_status='1') as uomQty, d.uom_qty_id, d.lot_no as lotNo, (select b.short_name from wms.wms_m_bpartner b where b.t_id=d.supplier_id  and b.act_status='1') as owner,d.supplier_id as ownerId from wms.wms_m_machine_dock md inner join wms.wms_b_container_d d on d.container_id = md.container_id  and d.act_status='1' where md.dock_id in (select e.t_id from wms.wms_m_equipment e where e.equipment_code='{dockNumber}' ) and md.machine_id = '{machineId}' and md.act_status='1' and d.act_status='1'";

            //url += "api/platform/database/selectsql";

            DataTable dt = NitCloudService.GetDataTable(url, sessionKey, sql, token);
            return dt;
        }

        /// <summary>
        /// 根据生产工单获取最新的机台要料/接班任务信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="planOutId">生产工单id</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public static DataTable GetTheNewItemNumByPlanOutId(string url, string isChangePersonReceive, string sessionKey, string warehouseId, string machineId, string planOutId, string token)
        {
            //string sql = $"select his.item_id,his.plan_qty,his.container_d_id from wms.wms_b_task_item_his his where his.task_id=(select u.t_id from (select h.t_id from wms.wms_b_task_his h where h.ref_doc_id='{planOutId}' and h.ref_doc_type in( 'DocType_CallTerminal','DocType_ReturnMaterielReturnBill')  and h.act_status='1' order by h.create_date desc ) u where rownum=1) and his.act_status = '1'";
            //string sql = $"select his.item_id, his.plan_qty, his.container_d_id,(select q.container_id from wms.wms_b_task_his q where q.t_id=his.task_id) as container_id,his.task_id from wms.wms_b_task_item_his his where his.task_id =(select u.t_id from (select h.t_id from wms.wms_b_task_his h where h.ref_doc_id = '{planOutId}' and h.ref_doc_type in ('DocType_CallTerminal', 'DocType_ReturnMaterielReturnBill') and h.act_status = '1' order by h.create_date desc) u where rownum = 1) and his.act_status = '1' union select his.item_id, his.plan_qty, his.container_d_id,(select q.container_id from wms.wms_b_task_his q where q.t_id=his.task_id) as container_id,his.task_id from wms.wms_b_task_item_his his where his.task_id in (select h.t_id from wms.wms_b_task_his h where h.ref_doc_type = 'DocType_PersonReceiveMateriel' and h.ref_doc_id in (select x.t_id from wms.wms_d_pick_return_h x where x.act_status = '1' and x.pick_return_status = 'PickReturnStatus_Finish' and x.job_no = (select z.job_no from wms.wms_d_prod_plan_out z where z.t_id = '{planOutId}' and z.act_status = '1')) and h.act_status = '1') and his.act_status = '1'";

            string sql = $"select distinct his.item_id, his.plan_qty, his.container_d_id, (select q.container_id from wms.wms_b_task_his q where q.t_id = his.task_id) as container_id, his.task_id from wms.wms_b_task_item_his his where his.task_id in (select h.t_id from wms.wms_b_task_his h where exists (select * from (select s.ref_doc_id, max(s.create_date) DT,s.dest_addr_id from wms.wms_b_task_his s group by s.ref_doc_id,s.dest_addr_id) T where T.ref_doc_id = h.ref_doc_id and T.DT = h.create_date and T.dest_addr_id=h.dest_addr_id and h.ref_doc_id in (select distinct md.doc_id from wms.wms_m_machine_dock md, wms.wms_m_section x where md.machine_id = '{machineId}' and md.prod_section = x.t_id and x.section_code not like ('%人工领料%') and x.section_code not like ('%交接班%') and md.act_status = '1' and x.act_status = '1') and h.ref_doc_type in ('DocType_CallTerminal', 'DocType_ReturnMaterielReturnBill'))) and his.act_status = '1'  ";
            //拼接上查询人工领料的任务信息的sql
            if (string.Equals(isChangePersonReceive, "1"))
            {
                sql += $" union select his.item_id, his.plan_qty, his.container_d_id, (select q.container_id from wms.wms_b_task_his q where q.t_id = his.task_id) as container_id, his.task_id from wms.wms_b_task_item_his his where his.task_id in (select h.t_id from wms.wms_b_task_his h where h.ref_doc_type = 'DocType_PersonReceiveMateriel' and h.ref_doc_id in (select k.t_id from wms.wms_d_pick_return_h k where k.pick_return_type = 'PickReturnType_RGLL' and k.act_status = '1' and k.pick_return_status = 'PickReturnStatus_Finish' and k.job_no in (select z.job_no from wms.wms_d_prod_plan_out z where z.t_id = (select distinct md.doc_id from wms.wms_m_machine_dock md, wms.wms_m_section s where md.machine_id = '{machineId}' and md.prod_section = s.t_id and s.section_code like ('%人工领料%') and md.act_status = '1' and s.act_status = '1') and z.act_status = '1') and k.pick_return_no not in (select q.ref_doc_no from wms.wms_d_pick_return_h q where q.pick_return_type = 'PickReturnType_RGTL' and q.pick_return_status = 'PickReturnStatus_Finish' and q.act_status = '1' and q.job_no = (select z.job_no from wms.wms_d_prod_plan_out z where z.t_id = (select distinct md.doc_id from wms.wms_m_machine_dock md, wms.wms_m_section s where md.machine_id = '{machineId}' and md.prod_section = s.t_id and s.section_code like ('%人工领料%') and md.act_status = '1' and s.act_status = '1') and z.act_status = '1'))) and h.act_status = '1') and his.act_status = '1' ";
            }
            DataTable dt = NitCloudService.GetDataTable(url, sessionKey, sql, token);
            return dt;
        }

        /// <summary>
        /// 根据站台获取最新的要料/接班任务信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="dockNumber">站台编号</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public static DataTable GetTaskByDock(string url, string isChangePersonReceive, string sessionKey, string warehouseId, string machineId, string dockNumber, string token)
        {
            //string sql = $"select his.item_id, his.plan_qty, his.container_d_id, ( select q.container_id from wms.wms_b_task_his q where q.t_id = his.task_id ) as container_id, his.task_id from wms.wms_b_task_item_his his where his.task_id = ( select u.t_id from ( select h.t_id from wms.wms_b_task_his h where h.ref_doc_id in ( select distinct md.doc_id from wms.wms_m_machine_dock md, wms.wms_m_section s where md.machine_id = '{machineId}' and md.dock_id in ( select e.t_id from wms.wms_m_equipment e where e.equipment_code = '{dockNumber}' ) and md.prod_section = s.t_id and s.section_code not like ('%人工领料%') and s.section_code not like ('%交接班%') and md.act_status = '1' and s.act_status = '1' ) and h.ref_doc_type in ( 'DocType_CallTerminal', 'DocType_ReturnMaterielReturnBill' ) and h.dest_addr_id=(select e.t_id from wms.wms_m_equipment e where e.equipment_code = '{dockNumber}') and h.act_status = '1' order by h.create_date desc ) u where rownum = 1 ) and his.act_status = '1'  ";
            string sql = $"select distinct his.item_id, his.plan_qty, his.container_d_id, (select q.container_id from wms.wms_b_task_his q where q.t_id = his.task_id) as container_id, his.task_id from wms.wms_b_task_item_his his where his.task_id in (select h.t_id from wms.wms_b_task_his h where exists (select * from (select s.ref_doc_id, max(s.create_date) DT, s.dest_addr_id from wms.wms_b_task_his s group by s.ref_doc_id, s.dest_addr_id) T where T.ref_doc_id = h.ref_doc_id and T.DT = h.create_date and T.dest_addr_id = h.dest_addr_id and h.ref_doc_id in (select a.doc_id from wms.wms_m_machine_dock a where a.dock_id= (select z.t_id from wms.wms_m_equipment z where z.equipment_code='{dockNumber}') and a.act_status='1') and h.ref_doc_type in ('DocType_CallTerminal', 'DocType_ReturnMaterielReturnBill'))) and his.act_status = '1'";
            DataTable dt = NitCloudService.GetDataTable(url, sessionKey, sql, token);
            return dt;
        }

        /// <summary>
        /// 退坏料界面，展示的托盘数据。根据站台位获取当前托盘数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="dockNumber">站台编码</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public static DataTable BadMaterialGetContainerDetailByDockNumber(string url, string sessionKey, string warehouseId, string machineId, string dockNumber, string token)
        {
            string sql = $"select distinct d.t_id as containerDetailId,md.dock_id as dockId, (select h.container_barcode from wms.wms_b_container_h h where h.t_id=md.container_id and h.act_status='1') as containerBarcode, (select s.section_code from wms.wms_m_section s where s.t_id=md.prod_section  and s.act_status='1') as sectionCode, md.dock_name as dockName,d.item_id as itemId, d.item_name as itemName, d.spec, '0' as planQty, '0' as qty, (select iu.uom_name from wms.wms_m_item_uom iu where iu.t_id = d.uom_qty_id  and iu.act_status='1') as uomQty, d.uom_qty_id, d.lot_no as lotNo, (select b.short_name from wms.wms_m_bpartner b where b.t_id=d.supplier_id  and b.act_status='1') as owner,d.supplier_id as ownerId from wms.wms_m_machine_dock md inner join wms.wms_b_container_d d on d.container_id = md.container_id  and d.act_status='1' where md.dock_id in (select e.t_id from wms.wms_m_equipment e where e.equipment_code='{dockNumber}' ) and md.machine_id = '{machineId}' and md.act_status='1' and d.act_status='1'";

            //url += "api/platform/database/selectsql";

            DataTable dt = NitCloudService.GetDataTable(url, sessionKey, sql, token);
            return dt;
        }

        /// <summary>
        /// 退坏料界面，展示的目的区域数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="dockNumber">站台编码</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public static DataTable GetTargetAreaOnBadMaterial(string url, string sessionKey, string warehouseId, string machineId, string dockNumber, string token)
        {
            string sql = $"select * from wms.wms_m_equipment e where e.zone_id =(select zone_id from wms.wms_m_equipment where t_id='{machineId}'  and act_status='1') and e.equipment_usage='EquipmentUsage_ReturnBadMaterials'  and e.equipment_status='EquipmentStatus_OK' and e.act_status='1'";
            DataTable dt = NitCloudService.GetDataTable(url, sessionKey, sql, token);
            return dt;
        }

        /// <summary>
        /// 调用退余料微服务，进行退余料
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isNeedCallPallet">是否需要再次要料（1:需要；0:不需要）</param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="dockId">站台id</param>
        /// <param name="productPlanOutId">生产工单id</param>
        /// <param name="shiftId">班次id</param>
        /// <param name="teamId">班组id</param>
        /// <param name="details">余料明细（json格式）</param>
        /// <param name="hasLGV">是否产生AGV任务</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public static string ClientReturnPallet(string url, out string errcode, bool isNeedCallPallet, bool isCanBeCallOut, string sessionKey, string warehouseId, string machineId, string dockId, string productPlanOutId, string shiftId, string teamId, string details, string hasAGV, string token)
        {
            //url += "api/wms/fl/flcallend/clientreturnpallet";
            url += "api/wms/MachineMaterialService/ClientReturnPallet";
            JObject jo = new JObject();
            jo.Add("machine", machineId);
            jo.Add("dock", dockId);
            jo.Add("productPlanOutId", productPlanOutId);
            jo.Add("shiftId", shiftId);
            jo.Add("teamId", teamId);
            jo.Add("details", details);
            jo.Add("IsNeedCallPallet", (isNeedCallPallet == true ? "1" : "0"));
            jo.Add("IsCanBeCallOut", (isCanBeCallOut == true ? "1" : "0"));
            jo.Add("hasAGV", hasAGV);

            string result = NitCloudService.PostGateway(url, jo.ToString(), token);
            if (string.IsNullOrEmpty(result)) 
                throw new Exception("退余料调用微服务失败！");
            JObject entity = JObject.Parse(result);
            //bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            errcode = entity["errcode"].ToString();
            return entity["errmsg"].ToString();
        }

        /// <summary>
        /// 调用退坏料微服务，进行退余料
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="dockId">站台id</param>
        /// <param name="productPlanOutId">生产工单id</param>
        /// <param name="shiftId">班次id</param>
        /// <param name="teamId">班组id</param>
        /// <param name="details">坏料明细（json格式）</param>
        /// <param name="hasLGV">是否产生AGV任务</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public static string ClientReturnBadPallet(string url, string sessionKey, string warehouseId, string machineId, string dockId, string productPlanOutId, string shiftId, string teamId, string details, string hasAGV, string token)
        {
            //url += "api/wms/fl/flcallend/clientreturnpallet";
            url += "api/wms/MachineMaterialService/ClientReturnBadPallet";
            JObject jo = new JObject();
            jo.Add("machine", machineId);
            jo.Add("dock", dockId);
            jo.Add("productPlanOutId", productPlanOutId);
            jo.Add("shiftId", shiftId);
            jo.Add("teamId", teamId);
            jo.Add("details", details);
            jo.Add("hasAGV", hasAGV);

            string result = NitCloudService.PostGateway(url, jo.ToString(), token);
            if (string.IsNullOrEmpty(result)) throw new Exception("退坏料调用微服务失败！");
            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                return entity["errmsg"].ToString();
            }
            return string.Empty;
        }


        /// <summary>
        /// 调用换牌微服务，进行退余料
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="productPlanOutId">生产工单id</param>
        /// <param name="shiftId">班次id</param>
        /// <param name="teamId">班组id</param>
        /// <param name="details">余料明细（json格式）</param>
        /// <param name="hasLGV">是否产生AGV任务</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public static string ChangeCard(string url, string sessionKey, bool isCanBeCallOut, string warehouseId, string machineId, string productPlanOutId, string shiftId, string teamId, string details, string hasAGV, string token)
        {
            url += "api/wms/MachineMaterialService/ChangeCard";
            JObject jo = new JObject();
            jo.Add("machine", machineId);
            jo.Add("productPlanOutId", productPlanOutId);
            jo.Add("shiftId", shiftId);
            jo.Add("teamId", teamId);
            jo.Add("details", details);
            jo.Add("IsNeedCallPallet", "0");
            jo.Add("IsCanBeCallOut", (isCanBeCallOut == true ? "1" : "0"));
            jo.Add("hasAGV", hasAGV);

            string result = NitCloudService.PostGateway(url, jo.ToString(), token);
            if (string.IsNullOrEmpty(result)) throw new Exception("换牌调用微服务失败！");
            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                return entity["errmsg"].ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取交接班时挂在站台上的物料信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="schemeId"></param>
        /// <param name="token"></param>
        /// <param name="isChangePersonReceive">是否交接人工领料数据（true:交；false:不交）</param>
        /// <returns></returns>
        public static DataTable GetMachineCurItemsInfo(string url, string sessionKey, string warehouseId, string machineId, string schemeId, string token, bool isChangePersonReceive)
        {
            //string sql = $"select distinct d.t_id as containerDetailId, md.dock_id as dockId, (select h.container_barcode from wms.wms_b_container_h h where h.t_id = md.container_id  and h.act_status='1') as containerBarcode, s.section_code as sectionCode, md.dock_name as dockName, d.item_id as itemId, d.item_name as itemName, d.spec, 0 as planQty, '0' as qty, (select iu.uom_name from wms.wms_m_item_uom iu where iu.t_id = d.uom_qty_id  and iu.act_status='1') as uomName, d.uom_qty_id, d.lot_no as lotNo, (select b.short_name from wms.wms_m_bpartner b where b.t_id = d.supplier_id  and b.act_status='1') as owner, d.supplier_id as ownerId, d.spec as spec, d.ratio, d.act_status from wms.wms_m_machine_dock md inner join wms.wms_b_container_d d on d.container_id = md.container_id inner join wms.wms_m_section s on md.prod_section=s.t_id where md.machine_id = '{machineId}' and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' and md.act_status = '1' and d.act_status = '1' and s.act_status = '1' ";
            string sql = $"select distinct d.t_id as containerDetailId,d.container_id, md.dock_id as dockId,(select h.container_barcode from wms.wms_b_container_h h where h.t_id = md.container_id and h.act_status = '1') as containerBarcode, s.section_code as sectionCode, md.dock_name as dockName, d.item_id as itemId, d.item_name as itemName, d.spec, 0 as planQty, '0' as qty, (select iu.uom_name from wms.wms_m_item_uom iu where iu.t_id = d.uom_qty_id and iu.act_status = '1') as uomName, d.uom_qty_id, d.lot_no as lotNo, (select b.short_name from wms.wms_m_bpartner b where b.t_id = d.supplier_id and b.act_status = '1') as owner, d.supplier_id as ownerId, d.spec as spec, d.ratio, d.act_status from wms.wms_m_machine_dock md inner join wms.wms_b_container_d d on instr(md.container_id,d.container_id) > 0 inner join wms.wms_m_section s on md.prod_section = s.t_id where md.machine_id = '{machineId}' and s.section_code not like '%交接班%' ";
            if (isChangePersonReceive == false)
            {
                sql += $"  and s.section_code not like '%人工领料%' ";
            }

            sql += $"  and md.act_status = '1' and d.act_status = '1' and s.act_status = '1' order by md.dock_id desc  ";

            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        /// <summary>
        /// 获取机台交接班站台
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="schemeId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetClassDockNumberByMachineNumber(string url, string sessionKey, string warehouseId, string machineId, string schemeId, string token)
        {
            //            string sql = @"
            //SELECT M.DOCK_NUMBER DOCKNUMBER
            //  FROM SWMS_B_MACHINE_DOCK M
            // INNER JOIN SWMS_WI_EQUIPMENT E
            //    ON M.EQUIPMENT_ID = E.T_ID
            //  LEFT JOIN SWMS_B_MODEL T
            //    ON M.MODE_ID = T.T_ID
            // WHERE M.WAREHOUSE_ID = '" + warehouseId + @"'
            //   AND E.EQUIPMENT_NUMBER = '" + machineNumber + @"'
            //   AND T.MODEL_CODE = '交接班站台'";

            string sql = $"select md.*  from wms.wms_m_machine_dock md,wms.wms_m_section s where md.machine_id = '{machineId}' and md.prod_section=s.t_id and s.section_code like ('%交接班%') and md.act_status='1' and s.act_status='1'";

            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        /// <summary>
        /// 接班时获取机台当前交班托盘信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="schemeId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetMachineCurItemsInfoWhenChangeClass(string url, string isChangePersonReceive, string sessionKey, string warehouseId, string machineId, string schemeId, string token)
        {
            //string sql = $"select distinct d.t_id as containerDetailId, md.dock_id as dockId, (select h.container_barcode from wms.wms_b_container_h h where h.t_id = md.container_id and h.act_status = '1') as containerBarcode, s.section_code as sectionCode, md.dock_name as dockName, d.item_id as itemId, d.item_name as itemName, d.spec, '0' as planQty, d.qty as qty, (select iu.uom_name from wms.wms_m_item_uom iu where iu.t_id = d.uom_qty_id and iu.act_status = '1') as uomName, d.uom_qty_id, d.lot_no as lotNo, (select b.short_name from wms.wms_m_bpartner b where b.t_id = d.owner_id and b.act_status = '1') as owner, d.owner_id as ownerId, d.spec as spec, d.ratio, d.act_status from wms.wms_m_machine_dock md inner join wms.wms_m_section s on md.prod_section = s.t_id inner join wms.wms_b_container_d d on d.container_id = md.container_id and d.act_status = '1' where md.machine_id = '{machineId}' and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' and md.act_status = '1' and s.act_status = '1' order by d.item_id desc ";
            string sql = $"select distinct d.t_id as containerDetailId,d.container_id, md.dock_id as dockId, (select h.container_barcode from wms.wms_b_container_h h where h.t_id = md.container_id and h.act_status = '1') as containerBarcode, s.section_code as sectionCode, md.dock_name as dockName, d.item_id as itemId, d.item_name as itemName, d.spec, '0' as planQty, d.qty as qty, (select iu.uom_name from wms.wms_m_item_uom iu where iu.t_id = d.uom_qty_id and iu.act_status = '1') as uomName, d.uom_qty_id, d.lot_no as lotNo, (select b.short_name from wms.wms_m_bpartner b where b.t_id = d.supplier_id and b.act_status = '1') as owner, d.supplier_id as ownerId, d.spec as spec, d.ratio, d.act_status from wms.wms_m_machine_dock md inner join wms.wms_m_section s on md.prod_section = s.t_id inner join wms.wms_b_container_d d on instr(md.container_id,d.container_id) > 0 and d.act_status = '1' where md.machine_id = '{machineId}' and s.section_code not like '%交接班%' ";
            if (string.Equals(isChangePersonReceive, "0"))
            {
                sql += $" and s.section_code not like '%人工领料%' ";
            }

            sql += $"  and md.act_status = '1' and s.act_status = '1' order by md.dock_id desc ";

            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        /// <summary>
        /// 获取机台未完成的交接单信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetNoFinishMachineReturnMaterialBill(string url, string sessionKey, string warehouseId, string machineId, string token)
        {
            //string sql = $"select md.*,po.job_no,po.shift_id,po.team_id  from wms.wms_m_machine_dock md,wms.wms_m_section s,wms.wms_d_prod_plan_out po where md.machine_id = '{machineId}' and po.plan_status='PlanOutStatus_Executing' and md.prod_section=s.t_id and md.machine_id=po.equipment_id and md.container_id is not null and s.section_code like ('%交接班%') and md.act_status='1' and s.act_status='1' and po.act_status='1'";
            string sql = $"select md.*,po.job_no,po.shift_id,po.team_id  from wms.wms_m_machine_dock md,wms.wms_m_section s,wms.wms_d_prod_plan_out po where md.machine_id = '{machineId}' and md.prod_section=s.t_id and md.doc_id = po.t_id and md.container_id is not null and s.section_code like ('%交接班%') and md.act_status='1' and s.act_status='1' and po.act_status='1'";
            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        //获取机台未完成的交接单信息
        public static DataTable GetMachineReturnMaterialBill(string url, string sessionKey, string warehouseId, string machineNumber)
        {
            #region SQL
            string sql = @"
SELECT E.DESCRIPTION MACHINENAME,
       MD.DOCK_NAME DOCKNAME,
       MD.DOCK_STATE DOCKSTATUS,
       S.SHIPPER_NAME SHIPPERNAME,
       H.*,
       H.T_ID BILLID,
       D.T_ID CONTAINERDETAILID,
       D.QTY AS CD_QTY,
       D.BATCH_NO LOTNUMBER,
       I.ITEM_NAME,
       IU.CNAME,
       BH.BOM_NUMBER BOMCODE,
       IU.RATE,
       I.T_ID ITEM_ID,
       I.ITEM_CODE CODE,
       I.ITEM_DESCRIPTION DESCS,
       DECODE(D.RATE, 0, 1, NULL, 1, D.RATE) ITEM_RATE,
       D.QTY * DECODE(D.RATE, 0, 1, NULL, 1, D.RATE) QTY_COUNT,
       D.SHIPPER_ID SHIPPER_ID,
       D.STATISTIC_UNIT UNIT_ID,
       U2.CNAME UNIT_NAME
  FROM SWMS_B_MATERIAL_BILL H
  LEFT JOIN SWMS_M_CONTAINER_D D
    ON H.CONTAINER_ID = D.CONTAINER_ID
  LEFT JOIN SWMS_BI_SHIPPER S
    ON D.SHIPPER_ID = S.T_ID
  LEFT JOIN SWMS_WI_ITEM I
    ON I.T_ID = D.ITEM_ID
  LEFT JOIN SWMS_BI_ITEMUNIT IU
    ON IU.T_ID = I.UNIT_ID
  LEFT JOIN SWMS_BI_BOM_H BH
    ON BH.T_ID = H.BOM_ID
  LEFT JOIN SWMS_WI_EQUIPMENT E
    ON H.MACHINE_NUMBER = E.EQUIPMENT_NUMBER
  LEFT JOIN SWMS_B_MACHINE_DOCK MD
    ON E.T_ID = MD.EQUIPMENT_ID
   AND H.DOCK_NUMBER = MD.DOCK_NUMBER
  LEFT JOIN SWMS_BI_ITEMUNIT U2
    ON D.STATISTIC_UNIT = U2.T_ID
 WHERE H.WAREHOUSE_ID = '" + warehouseId + @"'
   AND H.TYPE_CODE = 'Client_ReturnMaterial_False_This'
   AND H.STATE_CODE <> 'Finished'
   AND H.MACHINE_NUMBER = '" + machineNumber + @"'
 ORDER BY H.DOCK_NUMBER";
            #endregion

            return NitCloudService.GetDataTable(url, sessionKey, sql);
        }

        /// <summary>
        /// 获取机台未完成的交接单信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="dockId">交接班站台id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetMachineReturnMaterialBill(string url, string sessionKey, string warehouseId, string schemeId, string dockId, string token)
        {
            string sql = $"select t.*,p.shift_id,p.team_id,(select s.shift_name from  wms.wms_m_work_shift s where s.t_id=p.shift_id and s.act_status='1') as shiftName,(select wt.team_name from  wms.wms_m_work_team wt where wt.t_id= p.team_id and wt.act_status='1') as teamName from wms.wms_b_task t inner join wms.wms_m_machine_dock md on t.ref_doc_id = md.dock_id inner join wms.wms_d_prod_plan_out p on p.equipment_id = md.machine_id where md.act_status = '1' and t.act_status = '1' and p.act_status = '1' and p.t_id = '{schemeId}' and md.dock_id = '{dockId}' and t.task_status <> 'TaskStatus_Finished' and t.ref_doc_type = 'DocType_ReturnMaterielToNext'";
            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        /// <summary>
        /// 查看接班信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="schemeId">交班的生产工单id</param>
        /// <param name="dockId">交接班站台id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetMachineReturnMaterialBillWhenChangeClass(string url, string sessionKey, string warehouseId, string schemeId, string dockId, string token)
        {
            //string sql = $"select distinct p.shift_id,md.modify_date, p.team_id, (select s.shift_name from wms.wms_m_work_shift s where s.t_id = p.shift_id and s.act_status='1') as shiftName, (select wt.team_name from wms.wms_m_work_team wt where wt.t_id = p.team_id and wt.act_status='1') as teamName from wms.wms_m_machine_dock md inner join wms.wms_d_prod_plan_out p on p.equipment_id = md.machine_id  and p.plan_status='PlanOutStatus_Executing' where md.act_status = '1' and p.act_status = '1' and p.t_id = '{schemeId}' and md.dock_id = '{dockId}' ";
            string sql = $"select distinct p.shift_id,md.modify_date, p.team_id, (select s.shift_name from wms.wms_m_work_shift s where s.t_id = p.shift_id and s.act_status='1') as shiftName, (select wt.team_name from wms.wms_m_work_team wt where wt.t_id = p.team_id and wt.act_status='1') as teamName from wms.wms_m_machine_dock md inner join wms.wms_d_prod_plan_out p on p.t_id = md.doc_id where md.act_status = '1' and p.act_status = '1' and md.container_id is not null  and md.dock_id = '{dockId}' ";
            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        /// <summary>
        /// 接班时获取机台未完成的交接单信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="schemeId">交班的生产工单id</param>
        /// <param name="dockId">交接班站台id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetChangeClassAcceptInfo(string url, string sessionKey, string warehouseId, string schemeId, string dockId, string token)
        {
            //string sql = $"select distinct p.shift_id,md.modify_date, p.team_id, (select s.shift_name from wms.wms_m_work_shift s where s.t_id = p.shift_id and s.act_status='1') as shiftName, (select wt.team_name from wms.wms_m_work_team wt where wt.t_id = p.team_id and wt.act_status='1') as teamName from wms.wms_m_machine_dock md inner join wms.wms_d_prod_plan_out p on p.equipment_id = md.machine_id  and p.plan_status='PlanOutStatus_Executing' where md.act_status = '1' and p.act_status = '1' and p.t_id = '{schemeId}' and md.dock_id = '{dockId}' ";
            string sql = $"select distinct p.shift_id,md.modify_date, p.team_id, (select s.shift_name from wms.wms_m_work_shift s where s.t_id = p.shift_id and s.act_status='1') as shiftName, (select wt.team_name from wms.wms_m_work_team wt where wt.t_id = p.team_id and wt.act_status='1') as teamName from wms.wms_m_machine_dock md inner join wms.wms_d_prod_plan_out p on p.t_id = md.doc_id where md.act_status = '1' and p.act_status = '1' and md.container_id is not null  and md.dock_id = '{dockId}' ";
            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }


        //客户端交接
        public static string ClientReturnPalletToNext(string url, bool isChangePersonReceive, string sessionKey, string warehouseId, string machineId, string productPlanOutId, string shiftId, string teamId, string details, string hasAGV, string token)
        {
            //url += "api/wms/fl/flcallend/clientreturnpallettonext";

            url += "api/wms/MachineMaterialService/ReturnMaterielToNext";
            JObject jo = new JObject();
            jo.Add("machine", machineId);
            jo.Add("productPlanOutId", productPlanOutId);
            jo.Add("shiftId", shiftId);
            jo.Add("teamId", teamId);
            jo.Add("details", details);
            jo.Add("isChangePersonReceive", isChangePersonReceive == true ? "1" : "0");// 是否交接人工领料数据（1:交；0:不交）
            jo.Add("hasAGV", hasAGV);

            string result = NitCloudService.PostGateway(url, jo.ToString(), token);
            if (string.IsNullOrEmpty(result)) throw new Exception("交班调用微服务失败！");
            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                return entity["errmsg"].ToString();
            }
            return string.Empty;
        }

        //客户确认接班
        public static string ClientConfirmReturnBill(string url, string sessionKey, string warehouseId, string machineNumber,
             string dockNumber, string schemeId, string billId, string runsType)
        {
            url += "api/wms/fl/flcallend/clientconfirmreturnbill";

            JObject jo = new JObject();
            jo.Add("sessionKey", sessionKey);
            jo.Add("warehouseId", warehouseId);
            jo.Add("machineNumber", machineNumber);
            jo.Add("dockNumber", dockNumber);
            jo.Add("schemeId", schemeId);
            jo.Add("billId", billId);
            jo.Add("runsType", runsType);

            string result = NitCloudService.PostGateway(url, jo.ToString());
            if (string.IsNullOrEmpty(result)) throw new Exception("接班调用微服务失败！");
            JObject entity = JObject.Parse(result);
            string state = Convert.ToString(entity["ErrorCode"] ?? "");
            if (state == "0")
            {
                return string.Empty;  //成功
            }
            return Convert.ToString(entity["ErrMsg"] ?? "未知错误.");
        }

        /// <summary>
        /// 确认接班
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="productPlanOutId"></param>
        /// <param name="shiftId"></param>
        /// <param name="teamId"></param>
        /// <param name="details"></param>
        /// <param name="hasAGV"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string ClientModifyReturnBill(string url, string isChangePersonReceive, string sessionKey, string warehouseId, string machineId, string productPlanOutId, string shiftId, string teamId, string details, string hasAGV, string token)
        {
            //url += "api/wms/fl/flcallend/clientmodifyreturnbill";
            url += "api/wms/MachineMaterialService/ReturnMaterielReturnBill";
            JObject jo = new JObject();
            jo.Add("machine", machineId);
            jo.Add("productPlanOutId", productPlanOutId);
            jo.Add("shiftId", shiftId);
            jo.Add("teamId", teamId);
            jo.Add("details", details);
            jo.Add("isChangePersonReceive", isChangePersonReceive);
            jo.Add("hasAGV", hasAGV);

            string result = NitCloudService.PostGateway(url, jo.ToString(), token);
            if (string.IsNullOrEmpty(result)) throw new Exception("交班调用微服务失败！");
            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                return entity["errmsg"].ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 修改交班时提交的物料数量
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="productPlanOutId"></param>
        /// <param name="shiftId"></param>
        /// <param name="teamId"></param>
        /// <param name="details"></param>
        /// <param name="hasAGV"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string EditReturnBill(string url, string sessionKey, string warehouseId, string machineId, string productPlanOutId, string shiftId, string teamId, string details, string hasAGV, string token)
        {
            //url += "api/wms/fl/flcallend/clientmodifyreturnbill";
            url += "api/wms/MachineMaterialService/EditMaterielBillModel";
            JObject jo = new JObject();
            jo.Add("machine", machineId);
            jo.Add("productPlanOutId", productPlanOutId);
            jo.Add("shiftId", shiftId);
            jo.Add("teamId", teamId);
            jo.Add("details", details);
            jo.Add("hasAGV", hasAGV);

            string result = NitCloudService.PostGateway(url, jo.ToString(), token);
            if (string.IsNullOrEmpty(result)) throw new Exception("修改交班时提交的物料数量调用微服务失败！");
            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                return entity["errmsg"].ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据机台id获取机台所有站台位的容器信息(换牌退料)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetMachineCurContainInfo(string url, string sessionKey, string warehouseId, string machineId, string token)
        {
            string sql = $@"select d.t_id as containerDetailId, md.dock_id, (select e.equipment_code from wms.wms_m_equipment e where e.t_id = md.dock_id  and e.act_status='1') as dockCode, md.prod_section, (select s1.section_code from wms.wms_m_section s1 where s1.t_id = md.prod_section  and s1.act_status='1') as sectionCode, (select s2.setion_descripton from wms.wms_m_section s2 where s2.t_id = md.prod_section and s2.act_status='1') as sectionDes, d.item_id, (select i.item_name from wms.wms_m_item i where i.t_id = d.item_id and i.act_status='1') as itemName, '0' as planQty, '0' as qty, d.uom_qty_id, (select u.uom_name from wms.wms_m_item_uom u where u.t_id = d.uom_qty_id  and u.act_status='1') as uomName, d.lot_no as lot_no, d.supplier_id, (select bp.short_name from wms.wms_m_bpartner bp where bp.t_id = d.supplier_id  and bp.act_status='1') as ownerName from wms.wms_m_machine_dock md  inner join wms.wms_m_section s on md.prod_section=s.t_id inner join wms.wms_b_container_d d on md.container_id = d.container_id  and d.act_status = '1' where md.machine_id = '{machineId}' and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' and md.act_status = '1' and s.act_status='1' order by d.item_id desc";

            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }


        #region old
        //仓库人员退回某退料单 PC 2009/07/31
        public static int Return_MaterialBillByBillID(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string BillID, string PalletNumber, string ContainerID, string MachineNumber, string DockNumber)
        {
            //return WCFService.CallEndWS.Return_MaterialBillByBillID(EquipmentNumber, UserName, UserPassword, WareHouseID, BillID, PalletNumber, ContainerID, MachineNumber, DockNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                int res = srv.Return_MaterialBillByBillID(EquipmentNumber, UserName, UserPassword, WareHouseID, BillID, PalletNumber, ContainerID, MachineNumber, DockNumber);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        public static bool CheckDockReturnBill_Exists(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string DockNumber, string MachineNumber)
        {
            //return WCFService.CallEndWS.CheckDockReturnBill_Exists(EquipmentNumber, UserName, UserPassword, WareHouseID, DockNumber, MachineNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                bool res = srv.CheckDockReturnBill_Exists(EquipmentNumber, UserName, UserPassword, WareHouseID, DockNumber, MachineNumber);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        //修改机台为换牌后的首次呼叫状态 QWH 2009/07/10        
        public static int UpdateMachineToFirstCallState(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber)
        {
            //return WCFService.CallEndWS.UpdateMachineToFirstCallState(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                int res = srv.UpdateMachineToFirstCallState(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        //根据BOMID获取配盘明细数据（含ITEM允差率） QWH 2009/06/08
        public static DataSet GetBOMDetailByBomID(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string BomID)
        {
            //return WCFService.CallEndWS.GetBOMDetailByBomID(EquipmentNumber, UserName, UserPassword, WareHouseID, BomID);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                DataSet ds = srv.GetBOMDetailByBomID(EquipmentNumber, UserName, UserPassword, WareHouseID, BomID);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        public static bool ChangeReturnQty(string WarehouseID, DataTable change_DataTable)
        {
            //return WCFService.CallEndWS.ChangeReturnQty(WarehouseID, change_DataTable);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                bool res = srv.ChangeReturnQty(WarehouseID, change_DataTable);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        public static DataSet CheckReturnRemnantTask(string MachineNumber, string WareHouseID, string StartTime, string EndTime)
        {
            //return WCFService.CallEndWS.CheckReturnRemnantTask(MachineNumber, WareHouseID, StartTime, EndTime);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                DataSet ds = srv.CheckReturnRemnantTask(MachineNumber, WareHouseID, StartTime, EndTime);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }


        //修改退料单某物料的退料量
        public static int UpdateReturnItemQty(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string ContainerDetailID, decimal Qty, decimal odd1, decimal odd2, decimal odd3)
        {
            //return WCFService.CallEndWS.UpdateReturnItemQty(EquipmentNumber, UserName, UserPassword, WareHouseID, ContainerDetailID, Qty, odd1, odd2, odd3);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                int res = srv.UpdateReturnItemQty(EquipmentNumber, UserName, UserPassword, WareHouseID, ContainerDetailID, Qty, odd1, odd2, odd3);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        //重置工单归集标识
        public static int UpdateSendHistory(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string schemeID)
        {
            //return WCFService.CallEndWS.UpdateSendHistory(EquipmentNumber, UserName, UserPassword, WareHouseID, schemeID);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                int res = srv.UpdateSendHistory(EquipmentNumber, UserName, UserPassword, WareHouseID, schemeID);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        //创建人工退料信息
        public static string CreateManPowerCallMaterTask(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, TaskEntity taskEntity, Container_View containerinfo, List<ContainerDetailN> details)
        {
            //return WCFService.CallEndWS.CreateManPowerCallMaterTask(EquipmentNumber, UserName, UserPassword, WareHouseID, taskEntity, containerinfo, details);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                return srv.CreateManPowerCallMaterTask(EquipmentNumber, UserName, UserPassword, WareHouseID, taskEntity, containerinfo, details);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        public static String CreateManPowerReturnNullPallet(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber)
        {
            //return WCFService.CallEndWS.CreateManPowerReturnNullPallet(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                return srv.CreateManPowerReturnNullPallet(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        public static DataSet SelectTypeByCondition(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string SqlCondition)
        {
            //return WCFService.InitializeWS.SelectTypeByCondition(WarehouseID, EquipmentNumber, UserName, UserPassword, SqlCondition);

            WMSInterface.InitializeWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.InitializeWS>();
                return srv.SelectTypeByCondition(WarehouseID, EquipmentNumber, UserName, UserPassword, SqlCondition);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }
        /// <summary>
        /// 20140401 H
        /// </summary>
        /// <param name="WarehouseID"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static DataTable CheckBomItemInventory(string WarehouseID, string itemId)
        {
            //return WCFService.InitializeWS.CheckBomItemInventory(WarehouseID, itemId);
            return null;
        }


        //退废料
        public static int ClientReturnWaste(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber)
        {
            WorkFlowResult rs = null;
            WMSInterface.CallEndWS srv = null;

            try
            {
                //rs = WCFService.CallEndWS.ClientReturnWaste(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                rs = srv.ClientReturnWaste(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
            }
            catch
            { }
            finally { }

            if (rs == null || rs.Result <= 0)
            {
                WriteLog.Write(rs, "ClientReturnWaste");
            }

            return rs.Result;
        }

        //退残烟
        public static int ClientReturnReject(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber)
        {
            WorkFlowResult rs = null;
            WMSInterface.CallEndWS srv = null;

            try
            {
                //rs = WCFService.CallEndWS.ClientReturnReject(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                rs = srv.ClientReturnReject(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
            }
            catch
            { }
            finally { }

            if (rs == null || rs.Result <= 0)
            {
                WriteLog.Write(rs, "ClientReturnReject");
            }

            return rs.Result;
        }





        //客户端发出退坏料呼叫操作 QWH 2009/06/16        
        public static int ClientReturnBadPallet(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber, string BomID, ContainerEntity ContainerEntity, string ReturnBillType, string hasLGV, string Destination, string Runs)
        {
            WorkFlowResult rs = null;
            WMSInterface.CallEndWS srv = null;
            try
            {
                //rs = WCFService.CallEndWS.ClientReturnBadPallet(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber, BomID, ContainerEntity, ReturnBillType, hasLGV, Destination, Runs);
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                rs = srv.ClientReturnBadPallet(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber, BomID, ContainerEntity, ReturnBillType, hasLGV, Destination, Runs);
            }
            catch
            { }
            finally { }

            if (rs == null || rs.Result <= 0)
            {
                WriteLog.Write(rs, "ClientReturnBadPallet");
            }

            return rs.Result;
        }

        //仓库人员确认接收退料单操作 QWH 2009/06/17  WorkShop_ReceiveReturnBill      
        public static int ClientConfirmReturnBill_Warehouse(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, ContainerEntity containerEntity, string returnBillID)
        {
            WorkFlowResult rs = null;
            WMSInterface.CallEndWS srv = null;
            try
            {
                //rs = WCFService.CallEndWS.ClientConfirmReturnBill_Warehouse(EquipmentNumber, UserName, UserPassword, WareHouseID, containerEntity, returnBillID);
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                rs = srv.ClientConfirmReturnBill_Warehouse(EquipmentNumber, UserName, UserPassword, WareHouseID, containerEntity, returnBillID);
            }
            catch
            { }
            finally { }

            if (rs == null || rs.Result <= 0)
            {
                WriteLog.Write(rs, "ClientConfirmReturnBill_Warehouse");
            }

            return rs.Result;
        }

        public static DataSet GetMaterialBillDetail(string WarehouseID, string MachineNumber, string StartTime, string EndTime, string Field4)
        {
            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                return srv.GetMaterialBillDetail(WarehouseID, MachineNumber, StartTime, EndTime, Field4);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
            //return WCFService.CallEndWS.GetMaterialBillDetail(WarehouseID, MachineNumber, StartTime, EndTime, Field4);
        }

        //获取所有未完成的退料单信息 QWH 2009/05/14        
        public static DataSet GetAllReturnMaterialBill(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID)
        {
            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                return srv.GetAllReturnMaterialBill(EquipmentNumber, UserName, UserPassword, WareHouseID);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
            //return WCFService.CallEndWS.GetAllReturnMaterialBill(EquipmentNumber, UserName, UserPassword, WareHouseID);
        }




        //根据机台号、班次获取物料消耗 HJ 2012/12/20
        public static DataSet GetUsedMaterialsByRuns(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string StartTime, string EndTime)
        {
            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                return srv.GetUsedMaterialsByRuns(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, StartTime, EndTime);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
            //return WCFService.CallEndWS.GetUsedMaterialsByRuns(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, StartTime, EndTime);
        }
        //HJ 2012/12/24
        public static String InsertUsedMaterials(WMSInterface.UsedMaterialsHead head, List<WMSInterface.UsedMaterialsDetail> useddetails)
        {
            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                return srv.InsertUsedMaterials(head, useddetails);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
            //return WCFService.CallEndWS.InsertUsedMaterials(head, useddetails);
        }
        //HJ 2012/12/27
        public static bool CheckHavedEnsure(string WarehouseID, string MachineNumber, string RunsTypeCode, string StartTime, string EndTime)
        {
            //return WCFService.CallEndWS.CheckHavedEnsure(WarehouseID, MachineNumber, RunsTypeCode, StartTime, EndTime);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                bool res = srv.CheckHavedEnsure(WarehouseID, MachineNumber, RunsTypeCode, StartTime, EndTime);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }
        //HJ 2013/4/28
        public static DataSet GetManPowerReturnDetailByDockNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber)
        {
            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                return srv.GetManPowerReturnDetailByDockNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
            //return WCFService.CallEndWS.GetManPowerReturnDetailByDockNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
        }
        //HJ 2013/9/18
        public static DataSet GetManPowerCallDetailByDockNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber)
        {
            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                return srv.GetManPowerCallDetailByDockNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
            //return WCFService.CallEndWS.GetManPowerCallDetailByDockNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
        }
        //加载机台和机台生产牌号  pengenqi 2012/7/11
        public static DataSet LoadMachineProduct(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID)
        {
            WMSInterface.InitializeWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.InitializeWS>();
                return srv.GetMachineProductPlanAndEquipmentInfo(EquipmentNumber, UserName, UserPassword, WarehouseID);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
            //return WCFService.InitializeWS.GetMachineProductPlanAndEquipmentInfo(EquipmentNumber, UserName, UserPassword, WarehouseID);
        }

        public static DataSet LoadProduct(string EquipmentNumber, string UserID, string UserPassword, string WarehouseID, WMSInterface.EntityDefination.ITEM.ITEMQueryConditionBase Conditions)
        {
            WMSInterface.InitializeWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.InitializeWS>();
                return srv.GetItemInfoByConditions(EquipmentNumber, UserID, UserPassword, WarehouseID, Conditions);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
            //return WCFService.InitializeWS.GetItemInfoByConditions(EquipmentNumber, UserID, UserPassword, WarehouseID, Conditions);
        }

        public static int UpdateMachineProductPlan(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string MachineEquipmentID, string SchemeID, string ItemID, string MachineNumber, int Qty, string LotNumber, string Version)
        {
            WMSInterface.InitializeWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.InitializeWS>();
                return srv.UpdateMachineProductPlan(EquipmentNumber, UserName, UserPassword, WarehouseID, MachineEquipmentID, "",
                    ItemID, MachineNumber, 0, "", "");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
            //return WCFService.InitializeWS.UpdateMachineProductPlan(EquipmentNumber, UserName, UserPassword, WarehouseID, MachineEquipmentID, "",
            //        ItemID, MachineNumber, 0, "", "");
        }

        public static DataTable LoadMachinePalletType(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<String, String> Params)
        {
            //return WCFService.InitializeWS.SelectMachineDockN(loginInfo, Params);

            WMSInterface.InitializeWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.InitializeWS>();
                return srv.SelectMachineDockN(loginInfo, Params);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        public static WMSInterface.Entity50.Result UpdateMachineDockN(WMSInterface.Entity50.LoginInfo loginInfo, WMSInterface.Entity50.MachineDockN machineDock)
        {
            //return WCFService.InitializeWS.UpdateMachineDockN(loginInfo, machineDock);

            WMSInterface.InitializeWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.InitializeWS>();
                return srv.UpdateMachineDockN(loginInfo, machineDock);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        public static DataTable GetModelN(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<String, String> Params)
        {
            //return WCFService.InitializeWS.GetModelN(loginInfo, Params);

            WMSInterface.InitializeWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.InitializeWS>();
                return srv.GetModelN(loginInfo, Params);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        public static DataSet GetTwoProductName(string warehouseid, string machinenumber)
        {
            //return WCFService.CallEndWS.GetTwoProductName(warehouseid, machinenumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                return srv.GetTwoProductName(warehouseid, machinenumber);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        public static bool UpdateChangeClassItemQty(string warehouseId, string billId, DataTable dtChangeItmes, out string msg)
        {
            //return WCFService.CallEndWS.UpdateChangeClassItemQty(warehouseId, billId, dtChangeItmes, out msg);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                bool res = srv.UpdateChangeClassItemQty(warehouseId, billId, dtChangeItmes, out msg);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }
        #endregion
    }
}
