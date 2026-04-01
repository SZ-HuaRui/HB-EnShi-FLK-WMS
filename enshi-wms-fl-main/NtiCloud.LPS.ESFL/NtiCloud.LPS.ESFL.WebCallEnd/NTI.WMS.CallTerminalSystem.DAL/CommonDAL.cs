using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WMSInterface;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NTI.WMS.CallTerminalSystem.DAL
{
    public class CommonDAL
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <param name="sessionKey"></param>
        /// <returns>0表示远程服务器失败；1表示密码尝试超过限制，该用户已锁定；-1表示用户名或密码错误;2表示登录成功</returns>
        public static int WorkShop_Loging(string url, string userName, string password, string ip, out string sessionKey)
        {
            url += "api/platform/login/getlogin";
            JObject jo = new JObject();
            jo.Add("username", userName);
            jo.Add("userpswd", Encryptcs.EncryptByAES(password));
            //jo.Add("userip", ip);
            //jo.Add("warehouse", "WMS");//FL
            //jo.Add("clientcode", 1);
            //jo.Add("logintype", "Web");
            //jo.Add("isadmin", "true");
            jo.Add("islogin", "true");

            string result = NitCloudService.PostGateway(url, jo.ToString());
            //if (string.IsNullOrEmpty(result)) throw new Exception("登录调用微服务失败！");

            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                sessionKey = "";
                return -1;
                //string errmsg= Convert.ToString(entity["errmsg"] ?? "");
                //if (errmsg!= "Error")
                //{
                //    sessionKey = "";
                //    return 1;
                //}
                //else
                //{
                //    sessionKey = "";
                //    return -1;
                //}

            }
            if (isSucceed == true)
            {
                sessionKey = Convert.ToString(entity["json"]["guid"] ?? "");
                return 2;
            }
            sessionKey = "";
            return 0;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static bool WorkShop_GetToken(string url, string sessionKey,string clientId, string warehouseID, string appCode, out string token)
        {
            url += "api/oauth2/token";
            JObject jo = new JObject();
            jo.Add("sessionKey", sessionKey);
            jo.Add("appCode", appCode);
            jo.Add("clientId", clientId);
            jo.Add("warehouseId", warehouseID);
            string result = NitCloudService.PostGateway(url, jo.ToString());
            //if (string.IsNullOrEmpty(result)) throw new Exception("登录调用微服务失败！");

            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                token = "";
                return false;
            }
            if (isSucceed == true)
            {
                token = Convert.ToString(entity["json"] ?? "");
                return true;
            }
            token = "";
            return false;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static bool WorkShop_GetToken2(string url, string sessionKey, out string token)
        {
            url += "api/platform/User/GetToken";
            JObject jo = new JObject();
            jo.Add("sessionKey", sessionKey);
            jo.Add("appCode", "IWMS");
            jo.Add("clientId", "");
            jo.Add("localLanguage", "zh-CN");
            string result = NitCloudService.PostGateway(url, jo.ToString());
            //if (string.IsNullOrEmpty(result)) throw new Exception("登录调用微服务失败！");

            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                token = "";
                return false;
            }
            if (isSucceed == true)
            {
                token = Convert.ToString(entity["json"] ?? "");
                return true;
            }
            token = "";
            return false;
        }

        //获取车间
        public static DataTable GetWorkShopZone(string url, string sessionKey, string warehouseId)
        {
            string sql = @"
SELECT T.T_ID, T.ZONE_NAME
  FROM SWMS_WI_ZONE T
 WHERE T.WAREHOUSE_ID = '" + warehouseId + @"'
   AND T.TYPE_CODE = 'Zone_Shop'
 ORDER BY T.ZONE_NAME";

            return NitCloudService.GetDataTable(url, sessionKey, sql);
        }

        //根据车间类型获取机台
        public static DataTable GetMachineDataByWorkShopType(string url,string userName, string sessionKey, string token, string warehouseId, string workShopType)
        {
            //string sql = $"select e.t_id as t_id,e.equipment_code as equipment_code,e.equipment_name as equipment_name from wms.wms_m_equipment e where e.warehouse_id='{warehouseId}' and e.equipment_type='EquipmentType_Machine' and e.act_status='1' order by e.equipment_code";
            //string sql = $"select e.t_id as t_id,e.equipment_code as equipment_code,e.equipment_name as equipment_name from wms.wms_m_equipment e inner join wms.wms_m_machineuser mu on mu.machine_id=e.t_id where e.warehouse_id='{warehouseId}' and and mu.username='{userName}'       e.equipment_type='EquipmentType_Machine' and e.act_status='1' order by e.equipment_code";
            string sql = $"select * from ( select e.t_id as t_id, e.equipment_code as equipment_code, e.equipment_name as equipment_name from wms.wms_m_equipment e inner join wms.wms_m_machineuser mu on mu.machine_id=e.t_id where e.equipment_type = 'EquipmentType_Machine' and mu.username='{userName}' and e.act_status = '1' and exists (select * from wms.wms_m_machineuser w where w.username='{userName}' )  union all  select e.t_id as t_id, e.equipment_code as equipment_code, e.equipment_name as equipment_name from wms.wms_m_equipment e where e.equipment_type = 'EquipmentType_Machine' and e.act_status = '1' and not exists (select * from wms.wms_m_machineuser w where w.username='{userName}' )) r order by r.equipment_code";
            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        /// <summary>
        /// 根据类型获取班次
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public static DataTable GetRunsCode(string url, string sessionKey, string token)
        {
            string sql = $"select ws.t_id,ws.shift_code,ws.shift_name from wms.wms_m_work_shift ws where ws.act_status='1' order by ws.create_date desc";
            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        /// <summary>
        /// 获取班组
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public static DataTable GetRunsGroup(string url, string sessionKey, string token)
        {
            string sql = $"select wt.t_id,wt.team_code,wt.team_name from wms.wms_m_work_team wt where wt.act_status='1' order by wt.create_date desc";

            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        /// <summary>
        /// 根据机台号获取机台当前生产计划
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="machineNumber"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetMachineCurPlanByMachineNumber(string url, string sessionKey, string machineId, string shift_id, string team_id, string token)
        {
            //string sql = $"select p.t_id, p.warehouse_id, p.equipment_id,e.equipment_name,e.equipment_code, p.job_no, p.item_id, p.item_code, p.item_name, p.plan_status, p.plan_start_time, p.plan_end_time, p.plan_qty, p.plan_stat_qty, p.actual_qty, p.actual_stat_qty, p.lot_no, p.shift_id, p.team_id, p.memo, p.create_date, p.created_by from wms.wms_d_prod_plan_out p,wms.wms_m_equipment e where p.equipment_id='{machineId}' and p.plan_status='PlanOutStatus_Executing' and p.shift_id='{shift_id}' and p.team_id='{team_id}' and e.t_id=p.equipment_id and p.act_status='1' and e.act_status='1'";
            string sql = $"select p.t_id, p.warehouse_id, p.equipment_id, e.equipment_name, e.equipment_code, p.job_no, p.item_id, p.item_code, p.item_name, p.plan_status, p.plan_start_time, p.plan_end_time, p.plan_qty, p.plan_stat_qty, p.actual_qty, p.actual_stat_qty, p.lot_no, (select sh.shift_name from wms.wms_m_work_shift sh where sh.t_id=p.shift_id) as shift_name, p.shift_id, (select te.team_name from wms.wms_m_work_team te where te.t_id=p.team_id) as team_name, p.team_id, p.memo, p.create_date, p.created_by, e.equipment_usage from wms.wms_d_prod_plan_out p, wms.wms_m_equipment e where p.equipment_id = '{machineId}' and p.plan_status = 'PlanOutStatus_Executing' and e.t_id = p.equipment_id and p.act_status = '1' and e.act_status = '1' ";

            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }
        /// <summary>
        /// 旧版根据机台号获取机台当前生产计划代码
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="machineNumber"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetMachineCurPlanByMachineNumber1(string url, string sessionKey, string machineNumber, string token)
        {
            string sql = @"
SELECT P.ITEM_ID,
       E.DESCRIPTION EQUIPMENT_NAME,
       I.ITEM_CODE,
       I.ITEM_NAME,
       P.SCHEME_ID,
       P.STATE_CODE,
       Z.T_ID        ZONE_ID,
       Z.ZONE_NAME
  FROM SWMS_B_MACHINE_PLAN P
  LEFT JOIN SWMS_WI_EQUIPMENT E
    ON P.EQUIPMENT_ID = E.T_ID
  LEFT JOIN SWMS_WI_ITEM I
    ON P.ITEM_ID = I.T_ID
  LEFT JOIN SWMS_WI_ZONE Z
    ON E.ZONE_ID = Z.T_ID
 WHERE E.EQUIPMENT_NUMBER = '" + machineNumber + "'";

            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        /// <summary>
        /// 根据机台获取站台位信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetMachineDockInfo(string url, string sessionKey, string warehouseId, string machineId, string token)
        {
            //string sql = $" select m.machine_id as machineId,e.equipment_code as machineCode,m.dock_id as dockId,m.dock_name as dockname,(select eq.equipment_code from wms.wms_m_equipment eq where eq.t_id=m.dock_id and eq.act_status='1') as dockcode,s.t_id as sectionId ,s.section_code as sectionCode,e.equipment_status as dockstatus,m.create_date from wms.wms_m_equipment e,wms.wms_m_machine_dock m,wms.wms_m_section s where e.t_id=m.dock_id and s.t_id=m.prod_section and m.machine_id='{machineId}' and m.is_from_rack='1' and m.act_status='1' and e.act_status='1' and s.act_status='1' order by e.position,e.equipment_code";
            string sql = $" select m.machine_id as machineId,e.equipment_code as machineCode,m.dock_id as dockId,m.dock_name as dockname,(select eq.equipment_code from wms.wms_m_equipment eq where eq.t_id=m.dock_id and eq.act_status='1') as dockcode,s.t_id as sectionId ,s.section_code as sectionCode,e.equipment_status as dockstatus,m.create_date from wms.wms_m_equipment e,wms.wms_m_machine_dock m,wms.wms_m_section s where e.t_id=m.dock_id and s.t_id=m.prod_section and m.machine_id='{machineId}' and m.is_from_rack='1' and m.act_status='1' and e.act_status='1' and s.act_status='1' order by e.equipment_code desc";
            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }



        //根据机台获取站台位信息
        public static DataTable GetMachineDockInfo(string url, string sessionKey, string warehouseId, string machineNumber)
        {
            string sql = @"SELECT E.EQUIPMENT_NUMBER, E.TYPE_CODE, D.T_ID DOCK_ID, D.DOCK_NUMBER, D.DOCK_NAME, D.DOCK_STATE, D.CUR_PALLET, D.CUR_TASK_NUMBER, D.SOURCE_FLAG, M.MODEL_CODE FROM SWMS_WI_EQUIPMENT E LEFT JOIN SWMS_B_MACHINE_DOCK D ON E.T_ID = D.EQUIPMENT_ID LEFT JOIN SWMS_B_MODEL M ON D.MODE_ID = M.T_ID WHERE E.WAREHOUSE_ID = '" + warehouseId + @"' AND E.EQUIPMENT_NUMBER = '" + machineNumber + @"' ORDER BY D.DOCK_NUMBER";

            return NitCloudService.GetDataTable(url, sessionKey, sql);
        }

        /// <summary>
        /// 获取机台当前任务(除人工退料和人工领料任务)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="schemeId">生产工单id</param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="taskTypes">单据类型</param>
        /// <param name="token"></param>
        /// <param name="isContainsReturnWaste">是否包含退废料（1：包含；0：不包含）</param>
        /// <returns></returns>
        public static DataTable GetMachineTaskByMachineNumber(string url, string sessionKey, string schemeId, string warehouseId, string machineId, string taskTypes, string token,string isContainsReturnWaste)
        {
            string sql = string.Empty;
            string[] taskTypeArr = taskTypes.Split(',');
            List<string> sqlList = new List<string>();

            for (int i = 0; i < taskTypeArr.Length; i++)
            {
                if (taskTypeArr[i] == "'DocType_CallTerminal'")//机台呼叫出库：出库
                {
                    sqlList.Add($" select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' else '未知状态' end) as ref_doc_type, t.dest_addr_text, s.section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, (select h.bom_code from wms.wms_m_bom_h h where h.t_id = (select h1.bom_id from wms.wms_b_container_h h1 where h1.t_id=t.container_id ) and h.act_status = '1' and rownum = 1) as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成' else '未知状态' end) as task_status from wms.wms_b_task t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status not in ('TaskStatus_Finished', 'TaskStatus_Cancel') and t.ref_doc_type = {taskTypeArr[i]}");
                }
                else if (taskTypeArr[i] == "'DocType_ClientReturnNullPallet'") //退空托盘：入库
                {
                    sqlList.Add($" select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' else '未知状态' end) as ref_doc_type, t.dest_addr_text, s.section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, '' as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成' else '未知状态' end) as task_status from wms.wms_b_task t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status not in ('TaskStatus_Finished', 'TaskStatus_Cancel') and t.ref_doc_type = {taskTypeArr[i]}");
                }
                else if (taskTypeArr[i] == "'DocType_ClientReturnWaste'" && isContainsReturnWaste=="1") //退废料
                {
                    sqlList.Add($" select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' else '未知状态' end) as ref_doc_type, t.dest_addr_text, '' as section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, '' as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成' else '未知状态' end) as task_status from wms.wms_b_task t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status not in ('TaskStatus_Finished', 'TaskStatus_Cancel') and t.ref_doc_type = {taskTypeArr[i]}");
                }
                else if (taskTypeArr[i] == "'DocType_ProductTransfer'") //成品转移
                {
                    sqlList.Add($" select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' when t.ref_doc_type = 'DocType_ProductTransfer' then '成品转移' else '未知状态' end) as ref_doc_type, t.dest_addr_text, '' as section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, '' as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成' else '未知状态' end) as task_status from wms.wms_b_task t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status not in ('TaskStatus_Finished', 'TaskStatus_Cancel') and t.ref_doc_type = {taskTypeArr[i]}");
                }
                else if (taskTypeArr[i] == "'DocType_ReturnBadPallet'" || taskTypeArr[i] == "'DocType_ClientReturnPallet'")//机台退坏料、退余料：入库
                {
                    sqlList.Add($"select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' else '未知状态' end) as ref_doc_type, t.dest_addr_text, s.section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, (select h.bom_code from wms.wms_m_bom_h h where h.prod_section = s.t_id and h.act_status = '1' and rownum = 1) as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成' else '未知状态' end) as task_status from wms.wms_b_task t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status not in ('TaskStatus_Finished', 'TaskStatus_Cancel') and t.ref_doc_type = {taskTypeArr[i]}");
                }
            }
            sql = string.Join(" union all ", sqlList.ToArray());
            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        /// <summary>
        /// 获取机台当前任务(除人工退料和人工领料任务)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="schemeId">生产工单id</param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="taskTypes">单据类型</param>
        /// <param name="token"></param>
        /// <param name="isContainsReturnWaste">是否包含退废料（1：包含；0：不包含）</param>
        /// <returns></returns>
        public static DataTable GetMachineFinishTaskByMachineNumber(string url, string sessionKey, string schemeId, string warehouseId, string machineId, string taskTypes, string token, string startDatetime, string endDatetime)
        {
            //string sql = $" select * from  ( select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' else '未知状态' end) as ref_doc_type, t.dest_addr_text, s.section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, (select h.bom_code from wms.wms_m_bom_h h where h.prod_section = s.t_id and h.act_status = '1' and rownum = 1) as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成'  when t.task_status = 'TaskStatus_Finished' then '已完成'  else '未知状态' end) as task_status from wms.wms_b_task_his t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status in ('TaskStatus_Finished') and t.ref_doc_type = 'DocType_CallTerminal' union all select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' else '未知状态' end) as ref_doc_type, t.dest_addr_text, s.section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, (select h.bom_code from wms.wms_m_bom_h h where h.prod_section = s.t_id and h.act_status = '1' and rownum = 1) as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成' when t.task_status = 'TaskStatus_Finished' then '已完成' else '未知状态' end) as task_status from wms.wms_b_task_his t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status in ('TaskStatus_Finished') and t.ref_doc_type = 'DocType_ReturnBadPallet' union all select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' else '未知状态' end) as ref_doc_type, t.dest_addr_text, s.section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, (select h.bom_code from wms.wms_m_bom_h h where h.prod_section = s.t_id and h.act_status = '1' and rownum = 1) as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成'  when t.task_status = 'TaskStatus_Finished' then '已完成'  else '未知状态' end) as task_status from wms.wms_b_task_his t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status in ('TaskStatus_Finished') and t.ref_doc_type = 'DocType_ClientReturnPallet' ) ggg where ggg.create_date>=to_date(to_char(sysdate,'yyyy-mm-dd')||' 07:00:00','yyyy-mm-dd hh24:mi:ss') and ggg.create_date<to_date(to_char(trunc(sysdate)+1,'yyyy-mm-dd')||' 07:00:00','yyyy-mm-dd hh24:mi:ss') order by ggg.create_date desc";
            //string sql = $" select * from  ( select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' else '未知状态' end) as ref_doc_type, t.dest_addr_text, s.section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, (select h.bom_code from wms.wms_m_bom_h h where h.prod_section = s.t_id and h.act_status = '1' and rownum = 1) as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成'  when t.task_status = 'TaskStatus_Finished' then '已完成'  else '未知状态' end) as task_status from wms.wms_b_task_his t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status in ('TaskStatus_Finished') and t.ref_doc_type = 'DocType_CallTerminal' union all select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' else '未知状态' end) as ref_doc_type, t.dest_addr_text, s.section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, (select h.bom_code from wms.wms_m_bom_h h where h.prod_section = s.t_id and h.act_status = '1' and rownum = 1) as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成' when t.task_status = 'TaskStatus_Finished' then '已完成' else '未知状态' end) as task_status from wms.wms_b_task_his t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status in ('TaskStatus_Finished') and t.ref_doc_type = 'DocType_ReturnBadPallet' union all select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' else '未知状态' end) as ref_doc_type, t.dest_addr_text, s.section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, (select h.bom_code from wms.wms_m_bom_h h where h.prod_section = s.t_id and h.act_status = '1' and rownum = 1) as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成'  when t.task_status = 'TaskStatus_Finished' then '已完成'  else '未知状态' end) as task_status from wms.wms_b_task_his t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status in ('TaskStatus_Finished') and t.ref_doc_type = 'DocType_ClientReturnPallet' ) ggg where ggg.create_date>=to_date('{startDatetime}','yyyy-mm-dd hh24:mi:ss') and ggg.create_date<to_date('{endDatetime}','yyyy-mm-dd hh24:mi:ss') order by ggg.create_date desc";
            string sql = $" select * from  ( select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' else '未知状态' end) as ref_doc_type, t.dest_addr_text, s.section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, (select h.bom_code from wms.wms_m_bom_h h where h.t_id = (select h1.bom_id from wms.wms_b_container_h h1 where h1.t_id=t.container_id ) and h.act_status = '1' and rownum = 1) as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成'  when t.task_status = 'TaskStatus_Finished' then '已完成'  else '未知状态' end) as task_status from wms.wms_b_task_his t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status in ('TaskStatus_Finished') and t.ref_doc_type = 'DocType_CallTerminal' union all select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' else '未知状态' end) as ref_doc_type, t.dest_addr_text, s.section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, (select h.bom_code from wms.wms_m_bom_h h where h.t_id = (select h1.bom_id from wms.wms_b_container_h h1 where h1.t_id=t.container_id ) and h.act_status = '1' and rownum = 1) as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成' when t.task_status = 'TaskStatus_Finished' then '已完成' else '未知状态' end) as task_status from wms.wms_b_task_his t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status in ('TaskStatus_Finished') and t.ref_doc_type = 'DocType_ReturnBadPallet' union all select distinct t.t_id, t.task_no, t.task_type, (case when t.ref_doc_type = 'DocType_CallTerminal' then '要料' when t.ref_doc_type = 'DocType_ReturnBadPallet' then '机台退坏料' when t.ref_doc_type = 'DocType_ClientReturnPallet' then '退余料' when t.ref_doc_type = 'DocType_ClientReturnNullPallet' then '退空盘' when t.ref_doc_type = 'DocType_ClientReturnWaste' then '退废料' else '未知状态' end) as ref_doc_type, t.dest_addr_text, s.section_code, (select n.equipment_code from wms.wms_m_equipment n where n.t_id=m.dock_id) as dock_name, (select h.bom_code from wms.wms_m_bom_h h where h.t_id = (select h1.bom_id from wms.wms_b_container_h h1 where h1.t_id=t.container_id ) and h.act_status = '1' and rownum = 1) as bom_code, t.create_date, (case when t.task_status = 'TaskStatus_Create' then '新建' when t.task_status = 'TaskStatus_NotExecute' then '未执行' when t.task_status = 'TaskStatus_WaitingExecute' then '待执行' when t.task_status = 'TaskStatus_Executing' then '执行中' when t.task_status = 'TaskStatus_PartialFinished' then '部分完成'  when t.task_status = 'TaskStatus_Finished' then '已完成'  else '未知状态' end) as task_status from wms.wms_b_task_his t inner join wms.wms_m_machine_dock m on (m.dock_id = t.src_addr_id or m.dock_id = t.dest_addr_id) and m.act_status = '1' inner join wms.wms_m_section s on s.t_id = m.prod_section and s.section_code not like '%交接班%' and s.section_code not like '%人工领料%' where m.machine_id = '{machineId}' and t.act_status = '1' and t.task_status in ('TaskStatus_Finished') and t.ref_doc_type = 'DocType_ClientReturnPallet'  union all SELECT DISTINCT t.t_id, t.task_no, t.task_type , CASE  WHEN t.ref_doc_type = 'DocType_CallTerminal' THEN '要料' WHEN t.ref_doc_type = 'DocType_ReturnBadPallet' THEN '机台退坏料' WHEN t.ref_doc_type = 'DocType_ClientReturnPallet' THEN '退余料' WHEN t.ref_doc_type = 'DocType_ClientReturnNullPallet' THEN '退空盘' WHEN t.ref_doc_type = 'DocType_ClientReturnWaste' THEN '退废料' WHEN t.ref_doc_type = 'DocType_ProductTransfer' THEN '成品转移' ELSE '未知状态' END AS ref_doc_type, t.dest_addr_text, '' AS section_code , ( SELECT n.equipment_code FROM wms.wms_m_equipment n WHERE n.t_id = m.dock_id ) AS dock_name, '' AS bom_code, t.create_date , CASE  WHEN t.task_status = 'TaskStatus_Create' THEN '新建' WHEN t.task_status = 'TaskStatus_NotExecute' THEN '未执行' WHEN t.task_status = 'TaskStatus_WaitingExecute' THEN '待执行' WHEN t.task_status = 'TaskStatus_Executing' THEN '执行中' WHEN t.task_status = 'TaskStatus_PartialFinished' THEN '部分完成' WHEN t.task_status = 'TaskStatus_Finished' THEN '已完成' ELSE '未知状态' END AS task_status FROM wms.wms_b_task_his t INNER JOIN wms.wms_m_machine_dock m ON (m.dock_id = t.src_addr_id OR m.dock_id = t.dest_addr_id) AND m.act_status = '1' INNER JOIN wms.wms_m_section s ON s.t_id = m.prod_section AND s.section_code NOT LIKE '%交接班%' AND s.section_code NOT LIKE '%人工领料%' WHERE m.machine_id = '{machineId}' AND t.act_status = '1' AND t.task_status IN ('TaskStatus_Finished') AND t.ref_doc_type = 'DocType_ProductTransfer') ggg where ggg.create_date>=to_date('{startDatetime}','yyyy-mm-dd hh24:mi:ss') and ggg.create_date<to_date('{endDatetime}','yyyy-mm-dd hh24:mi:ss') order by ggg.create_date desc";
            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        /// <summary>
        /// 获取机台当前任务(除人工退料和人工领料任务)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="schemeId">生产工单id</param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="taskTypes">单据类型</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetMachinePersonTaskByMachineNumber(string url, string sessionKey, string schemeId, string warehouseId, string machineId, string taskTypes, string token)
        {
            string sql = $"select h.pick_return_no,h.job_no,h.equipment_id,p.plan_status,t.* from wms.wms_b_task t left join wms.wms_d_pick_return_h h on t.ref_doc_id=h.t_id and h.act_status='1' left join wms.wms_d_prod_plan_out p on p.job_no=h.job_no and h.pick_return_status='PickReturnStatus_Confirmed' where t.act_status='1' and t.ref_doc_type in('DocType_PersonReceiveMateriel','DocType_PersonReturnMateriel') and p.t_id='{schemeId}'";
            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        //获取机台当前任务
        public static DataTable GetMachineTaskByMachineNumber(string url, string sessionKey, string warehouseId,
            string machineNumber, string taskTypes)
        {
            string sql = @"
SELECT P.DICT_NAME AS TASKTYPENAME,
       S.DICT_NAME AS TASKSTATENAME,
       M.MODEL_CODE MODELCODE,
       T.TASK_NUMBER,
       T.BOM_CODE,
       T.CREATE_DATE,
       CASE
         WHEN T.TASK_TYPE_CODE IN ({0}) THEN
          T.TARGET_TEXT
         ELSE
          T.SOURCE_TEXT
       END AS DOCKNUMBER,
       D.DOCK_NAME DOCKNAME,
       D.SOURCE_FLAG ITEMSOURCE
  FROM SWMS_M_TASK T
 INNER JOIN SWMS_B_MACHINE_DOCK D
    ON (D.DOCK_NUMBER = T.TARGET_TEXT OR D.DOCK_NUMBER = T.SOURCE_TEXT)
  LEFT JOIN SWMS_WI_EQUIPMENT E
    ON D.EQUIPMENT_ID = E.T_ID
  LEFT JOIN SWMS_B_MODEL M
    ON D.MODE_ID = M.T_ID
  LEFT JOIN PROD_SYSTEM_DICTIONARY P
    ON T.TASK_TYPE_CODE = P.DICT_CODE
  LEFT JOIN PROD_SYSTEM_DICTIONARY S
    ON T.STATE_CODE = S.DICT_CODE
   AND S.T_ID = '1122396542976790528'
 WHERE T.WAREHOUSE_ID = '" + warehouseId + @"'
   AND E.EQUIPMENT_NUMBER = '" + machineNumber + @"'
 ORDER BY T.CREATE_DATE DESC";

            if (string.IsNullOrEmpty(taskTypes))
                taskTypes = "'Task_CallTerminal_Call','Task_OutBound','Task_ManPower_Call'";

            sql = string.Format(sql, taskTypes);

            return NitCloudService.GetDataTable(url, sessionKey, sql);
        }

        /// <summary>
        /// 获取机台当前物料信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetMachineCurTaskByMachineNumber(string url, string sessionKey, string warehouseId, string machineId, string token)
        {
            string sql = $"select distinct m.dock_id, m.dock_name, s.section_code, i.item_name, p.short_name, d.qty, (select uom_name from wms.wms_m_item_uom where t_id=d.UOM_QTY_ID) as uom_name from wms.wms_m_machine_dock m inner join wms.wms_m_section s on m.prod_section = s.t_id inner join wms.wms_b_container_h h on m.container_id = h.t_id inner join wms.wms_b_container_d d on d.container_id = h.t_id inner join wms.wms_m_item i on d.item_id = i.t_id left join wms.wms_m_bpartner p on d.supplier_id = p.t_id and p.act_status = '1' where m.machine_id = '{machineId}' and s.section_code not like '%交接班%'  and s.section_code  not like '%人工领料%' and m.act_status = '1' and s.act_status = '1' and h.act_status = '1' and d.act_status = '1' and i.act_status = '1' order by m.dock_id";

            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        //获取机台当前所有站台存在的任务
        public static DataTable GetMachineCurTaskByMachineNumber(string url, string sessionKey, string warehouseId, string machineNumber)
        {
            string sql = @"
SELECT D.DOCK_NUMBER,
       M.MODEL_CODE,
       I.ITEM_NAME,
       S.SHIPPER_NAME,
       CD.BATCH_NO,
       CD.QTY,
       IU.CNAME AS UNITNAME,
       D.SOURCE_FLAG ITEMSOURCE
  FROM SWMS_B_MACHINE_DOCK D
  LEFT JOIN SWMS_WI_EQUIPMENT E
    ON D.EQUIPMENT_ID = E.T_ID
  LEFT JOIN SWMS_B_MODEL M
    ON D.MODE_ID = M.T_ID
  LEFT JOIN SWMS_M_CONTAINER_D CD
    ON D.CUR_CONTAINER_ID = CD.CONTAINER_ID
  LEFT JOIN SWMS_BI_SHIPPER S
    ON CD.SHIPPER_ID = S.T_ID
  LEFT JOIN SWMS_WI_ITEM I
    ON CD.ITEM_ID = I.T_ID
  LEFT JOIN SWMS_BI_ITEMUNIT IU
    ON I.UNIT_ID = IU.T_ID
 WHERE D.WAREHOUSE_ID = '" + warehouseId + @"'
   AND E.EQUIPMENT_NUMBER = '" + machineNumber + @"'
   AND D.CUR_CONTAINER_ID IS NOT NULL
 ORDER BY D.DOCK_NUMBER";

            return NitCloudService.GetDataTable(url, sessionKey, sql);
        }

        //根据机台号、站台位号获取可供呼叫出库的配盘单明细
        public static DataTable GetBomDetailByMachineNumber(string url, string sessionKey, string warehouseId, string machineNumber, string dockNumber)
        {
            string sql = @"
SELECT BOM.BOM_NUMBER  BOMNUMBER,
       M.MODEL_CODE    MODELCODE,
       BOM.PRIORITY,
       I.ITEM_NAME     ITEMNAME_FL,
       D.QTY,
       S.DICT_NAME     STATENAME,
       BOM.BOM2_STATUS,
       I.T_ID          ITEMID_FL,
       I.ITEM_CODE     ITEMCODE_FL,
       A.BOM_ID        BOMID
  FROM SWMS_B_BOM_SET A
 INNER JOIN (SELECT MP.ITEM_ID, MD.MODE_ID
               FROM SWMS_B_MACHINE_PLAN MP
              INNER JOIN SWMS_B_MACHINE_DOCK MD
                 ON MP.EQUIPMENT_ID = MD.EQUIPMENT_ID
              WHERE MD.DOCK_NUMBER = '" + dockNumber + @"') MPI
    ON MPI.ITEM_ID = A.ITEM_ID
 INNER JOIN SWMS_BI_BOM_H BOM
    ON BOM.T_ID = A.BOM_ID
 INNER JOIN SWMS_BI_BOM_ITEM D
    ON BOM.T_ID = D.BOM_H_ID
  LEFT JOIN SWMS_B_MODEL M
    ON BOM.MODEL_CODE = M.T_ID
 INNER JOIN SWMS_WI_ITEM I
    ON I.T_ID = D.ITEM_ID
 INNER JOIN SWMS_BI_ITEMUNIT IU
    ON IU.T_ID = I.UNIT_ID
 INNER JOIN PROD_SYSTEM_DICTIONARY S
    ON S.DICT_CODE = BOM.BOM2_STATUS
 WHERE A.ITEM_ID = MPI.ITEM_ID
   AND BOM.BOM2_STATUS IN
       ('BOMCode_CallingOut',
        'BOMCode_Using',
        'BOMCode_CallOutButNotAssembly')
   AND BOM.MODEL_CODE = MPI.MODE_ID
   AND BOM.WAREHOUSE_ID = '" + warehouseId + @"'
 ORDER BY BOM.PRIORITY, BOM.BOM_NUMBER DESC";

            url += "api/platform/database/selectsql";
            string dbJson = "{\"sessionKey\":\"" + sessionKey + "\",\"sql\":\"" + sql + "\",\"Parameters\":[[]]}";
            string sqlDataTable = NitCloudService.PostGateway(url, dbJson);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(sqlDataTable ?? "[]");
            return dt;
        }

        /// <summary>
        /// 根据机台id、站台编号获取可供呼叫出库的配盘单明细
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineNumber"></param>
        /// <param name="dockcode">站台编号</param>
        /// <param name="schemeId">生产工单id</param>
        /// <param name="machineId">机台id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetBomDetailByMachineNumber(string url, string sessionKey, string warehouseId, string machineNumber, string dockcode, string schemeId, string machineId, string token)
        {
            //string sql = $"select d.item_name,d.qty, (case when bh.bom_status = 'BOMStatus_NoUsing' then '未使用' when bh.bom_status = 'BOMStatus_Stop' then '停用' when bh.bom_status = 'BOMStatus_Using' then '在用' when bh.bom_status = 'BOMStatus_WillStart' then '即将使用' else '未知状态' end) as bom_status,s.section_code,p.t_id,e.equipment_code,bh.bom_code from wms.wms_m_machine_dock md inner join wms.wms_d_prod_plan_out p on p.equipment_id=md.machine_id inner join wms.wms_m_section s on s.t_id=md.prod_section inner join wms.wms_m_equipment e on md.dock_id=e.t_id inner join wms.wms_b_container_h h on md.container_id=h.t_id inner join wms.wms_b_container_d d on h.t_id=d.container_id inner join wms.wms_m_bom_h bh on p.item_id=bh.item_id and s.t_id=bh.prod_section  and bh.act_status='1' where e.equipment_code='{dockcode}' and p.t_id='{schemeId}' and md.act_status='1' and p.act_status='1' and s.act_status='1' and e.act_status='1' and h.act_status='1' and d.act_status='1'";
            //string sql = $"SELECT (select i.item_name from wms.wms_m_item i where i.t_id=bd.item_id ) as item_name, bd.qty ,(select n.uom_name from wms.wms_m_item_uom n where n.t_id=bd.uom_qty_id) as uom_name, CASE  WHEN bh.bom_status = 'BOMStatus_NoUsing' THEN '未使用' WHEN bh.bom_status = 'BOMStatus_Stop' THEN '停用' WHEN bh.bom_status = 'BOMStatus_Using' THEN '在用' WHEN bh.bom_status = 'BOMStatus_WillStart' THEN '即将使用' ELSE '未知状态' END AS bom_status, s.section_code, p.t_id, e.equipment_code, bh.bom_code FROM wms.wms_m_machine_dock md INNER JOIN wms.wms_d_prod_plan_out p ON p.equipment_id = md.machine_id INNER JOIN wms.wms_m_section s ON s.t_id = md.prod_section INNER JOIN wms.wms_m_equipment e ON md.dock_id = e.t_id INNER JOIN wms.wms_m_bom_h bh ON (p.item_id = bh.item_id AND s.t_id = bh.prod_section AND bh.bom_status='BOMStatus_Using' and bh.act_status = '1') INNER JOIN wms.wms_m_bom_d bd ON bd.bom_h_id = bh.t_id WHERE (e.equipment_code = '{dockcode}' AND p.t_id = '{schemeId}' AND md.act_status = '1' AND p.act_status = '1' AND s.act_status = '1' AND e.act_status = '1')";
            string sql = $"SELECT (select i.item_name from wms.wms_m_item i where i.t_id=bd.item_id ) as item_name, bd.qty ,(select n.uom_name from wms.wms_m_item_uom n where n.t_id=bd.uom_qty_id) as uom_name, CASE  WHEN bh.bom_status = 'BOMStatus_NoUsing' THEN '未使用' WHEN bh.bom_status = 'BOMStatus_Stop' THEN '停用' WHEN bh.bom_status = 'BOMStatus_Using' THEN '在用' WHEN bh.bom_status = 'BOMStatus_WillStart' THEN '即将使用' ELSE '未知状态' END AS bom_status, s.section_code, p.t_id, e.equipment_code, bh.bom_code FROM wms.wms_m_machine_dock md INNER JOIN wms.wms_d_prod_plan_out p ON p.equipment_id = md.machine_id INNER JOIN wms.wms_m_section s ON s.t_id = md.prod_section INNER JOIN wms.wms_m_equipment e ON md.dock_id = e.t_id   INNER JOIN (select * from (SELECT t_bh.* FROM wms.wms_m_machine_dock t_md INNER JOIN wms.wms_d_prod_plan_out t_p ON t_p.equipment_id = t_md.machine_id INNER JOIN wms.wms_m_section t_s ON t_s.t_id = t_md.prod_section INNER JOIN wms.wms_m_equipment t_e ON t_md.dock_id = t_e.t_id INNER JOIN wms.wms_m_bom_h t_bh ON (t_p.item_id = t_bh.item_id AND t_s.t_id = t_bh.prod_section AND t_bh.bom_status = 'BOMStatus_Using' and t_bh.act_status = '1') WHERE (t_e.equipment_code = '{dockcode}' AND t_p.t_id = '{schemeId}' AND t_md.act_status = '1' AND t_p.act_status = '1' AND t_s.act_status = '1' AND t_e.act_status = '1') order by t_bh.create_date desc) bh2 where rownum = 1) bh ON (p.item_id = bh.item_id AND s.t_id = bh.prod_section AND bh.bom_status = 'BOMStatus_Using' and bh.act_status = '1') INNER JOIN wms.wms_m_bom_d bd ON bd.bom_h_id = bh.t_id WHERE (e.equipment_code = '{dockcode}' AND p.t_id = '{schemeId}' AND md.act_status = '1' AND p.act_status = '1' AND s.act_status = '1' AND e.act_status = '1')";
            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        /// <summary>
        /// 根据机台id、站台编号获取可供呼叫出库的配盘单明细
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineNumber"></param>
        /// <param name="dockcode">站台编号</param>
        /// <param name="schemeId">生产工单id</param>
        /// <param name="machineId">机台id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetBomDetailByMachineNumber1(string url, string sessionKey,string bomId, string warehouseId, string machineNumber, string dockcode, string schemeId, string machineId, string token)
        {
            string sql = $"SELECT ( SELECT i.item_name FROM wms.wms_m_item i WHERE i.t_id = bd.item_id ) AS item_name, bd.qty , ( SELECT n.uom_name FROM wms.wms_m_item_uom n WHERE n.t_id = bd.uom_qty_id ) AS uom_name , CASE  WHEN bh.bom_status = 'BOMStatus_NoUsing' THEN '未使用' WHEN bh.bom_status = 'BOMStatus_Stop' THEN '停用' WHEN bh.bom_status = 'BOMStatus_Using' THEN '在用' WHEN bh.bom_status = 'BOMStatus_WillStart' THEN '即将使用' ELSE '未知状态' END AS bom_status, s.section_code, p.t_id, e.equipment_code, bh.bom_code FROM wms.wms_m_machine_dock md INNER JOIN wms.wms_d_prod_plan_out p ON p.equipment_id = md.machine_id INNER JOIN wms.wms_m_section s ON s.t_id = md.prod_section INNER JOIN wms.wms_m_equipment e ON md.dock_id = e.t_id INNER JOIN wms.wms_m_bom_h bh ON (p.item_id = bh.item_id AND s.t_id = bh.prod_section AND bh.bom_status = 'BOMStatus_Using' AND bh.act_status = '1' AND bh.t_id = '{bomId}') INNER JOIN wms.wms_m_bom_d bd ON bd.bom_h_id = bh.t_id WHERE (e.equipment_code = '{dockcode}' AND p.t_id = '{schemeId}' AND md.act_status = '1' AND p.act_status = '1' AND s.act_status = '1' AND e.act_status = '1')";
            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        /// <summary>
        /// 根据机台id、站台编号获取Bom
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineNumber"></param>
        /// <param name="dockcode">站台编号</param>
        /// <param name="schemeId">生产工单id</param>
        /// <param name="machineId">机台id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetBomByMachineNumber(string url, string sessionKey, string warehouseId, string machineNumber, string dockcode, string schemeId, string machineId, string token)
        {
            string sql = $"SELECT t_bh.* , ( SELECT COUNT(w.t_id) AS cnt FROM wms.wms_b_container_h w WHERE (w.bom_id = t_bh.t_id AND w.act_status = '1' AND w.location_id IS NOT NULL AND w.container_barcode <> '00000000') ) AS cnt FROM wms.wms_m_machine_dock t_md INNER JOIN wms.wms_d_prod_plan_out t_p ON t_p.equipment_id = t_md.machine_id INNER JOIN wms.wms_m_section t_s ON t_s.t_id = t_md.prod_section INNER JOIN wms.wms_m_equipment t_e ON t_md.dock_id = t_e.t_id INNER JOIN wms.wms_m_bom_h t_bh ON (t_p.item_id = t_bh.item_id AND t_s.t_id = t_bh.prod_section AND t_bh.bom_status = 'BOMStatus_Using' AND t_bh.act_status = '1') WHERE (t_e.equipment_code = '{dockcode}' AND t_p.t_id = '{schemeId}' AND t_md.act_status = '1' AND t_p.act_status = '1' AND t_s.act_status = '1' AND t_e.act_status = '1') ORDER BY t_bh.create_date";
            return NitCloudService.GetDataTable(url, sessionKey, sql, token);
        }

        //获取成品牌号
        public static DataTable GetItemCP(string url, string sessionKey, string warehouseId)
        {
            string sql = @"
SELECT T_ID ITEM_ID, ITEM_CODE, ITEM_NAME
  FROM SWMS_WI_ITEM
 WHERE WAREHOUSE_ID = '" + warehouseId + @"'
   AND TYPE_CODE = 'ItemTypeCode_CP' ORDER BY ITEM_NAME";

            return NitCloudService.GetDataTable(url, sessionKey, sql);
        }

        //修改机台成品牌号
        public static string UpdateMachineProductPlan(string url, string sessionKey, string machineNumber, string itemId)
        {
            url += "api/wms/fl/flcallend/changemachineproduct";

            JObject jo = new JObject();
            jo.Add("sessionKey", sessionKey);
            jo.Add("machineNumber", machineNumber);
            jo.Add("itemId", itemId);

            string result = NitCloudService.PostGateway(url, jo.ToString());
            if (string.IsNullOrEmpty(result)) throw new Exception("修改机台成品牌号调用微服务失败！");
            JObject entity = JObject.Parse(result);
            string state = Convert.ToString(entity["ErrorCode"] ?? "");
            if (state == "0")
            {
                return string.Empty;  //成功
            }
            return Convert.ToString(entity["ErrMsg"] ?? "未知错误.");
        }

        #region 5.0
        //根据生产牌号获取生产区间
        public static DataSet GetZoneByMachineNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber)
        {
            //return WCFService.CallEndWS.GetZoneByMachineNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                DataSet ds = srv.GetZoneByMachineNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber);
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

        public static DataSet GetItemid(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber)
        {
            //return WCFService.CallEndWS.GetZoneByMachineNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                DataSet ds = srv.GetItemid(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber);
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

        public static DataSet GetEquimentid(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber)
        {
            //return WCFService.CallEndWS.GetZoneByMachineNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                DataSet ds = srv.GetEquimentid(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber);
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





        //自动登录验证用户
        public static DataTable CheckAutoLoginUser(string warehouseId, string code)
        {
            //return WCFService.CallEndWS.CheckAutoLoginUser(warehouseId, code);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                DataTable dt = srv.CheckAutoLoginUser(warehouseId, code);
                return dt;
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

        //获取权限
        public static List<string> GetLogingUserPermission(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID)
        {
            //return WCFService.CallEndWS.GetLogingUserPermission(EquipmentNumber, UserName, UserPassword, WareHouseID);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                return srv.GetLogingUserPermission(EquipmentNumber, UserName, UserPassword, WareHouseID);
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
        /// 检查站台位状态 
        /// </summary>
        /// <param name="EquipmentNumber"></param>
        /// <param name="UserName"></param>
        /// <param name="UserPassword"></param>
        /// <param name="WareHouseID"></param>
        /// <param name="MachineNumber"></param>
        /// <param name="DockNumber"></param>
        /// <returns>FALSE 正在使用；TRUE 未使用</returns>       
        public static bool CheckDockState(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber)
        {
            //return WCFService.CallEndWS.CheckDockState(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                bool res = srv.CheckDockState(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
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

        /// <summary>
        /// 检查成型机台的父站台对应的子站台是否已经退料完毕 PEQ 20110211
        /// </summary>
        /// <param name="WarehouseID">仓库id</param>
        /// <param name="MachineNumber">机台</param>
        /// <param name="DockNumber">站台</param>
        /// <returns></returns>
        public static bool CheckCXMachineStatus(string WarehouseID, string MachineNumber, string DockNumber)
        {
            return true;//WCFService.CallEndWS.CheckCXMachineStatus(WarehouseID,MachineNumber,DockNumber);
        }

        /// <summary>
        /// 检查成型机台是否存在父站台
        /// </summary>
        /// <param name="Warehouseid">仓库id</param>
        /// <param name="MachineNumber">机台</param>
        /// <param name="DockNumber">站台</param>
        /// <returns></returns>
        public static object CheckExistsFatherDockNumber(string Warehouseid, string MachineNumber, string DockNumber)
        {
            return true;//WCFService.CallEndWS.CheckExistsFatherDockNumber(Warehouseid,MachineNumber,DockNumber);
        }

        /// <summary>
        /// 检查站台位退料单数据（仅用于退料）
        /// </summary>
        /// <param name="EquipmentNumber"></param>
        /// <param name="UserName"></param>
        /// <param name="UserPassword"></param>
        /// <param name="WareHouseID"></param>
        /// <param name="DockNumber"></param>
        /// <param name="MachineNumber"></param>
        /// <returns>FALSE 存在未执行的退料单；TRUE 没有未执行的退料单</returns>
        public static bool CheckDockReturnBill_Exists(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string DockNumber, string MachineNumber)
        {
            //return WCFService.CallEndWS.CheckDockReturnBill_Exists(EquipmentNumber, UserName, UserPassword, WareHouseID, DockNumber, MachineNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                bool ds = srv.CheckDockReturnBill_Exists(EquipmentNumber, UserName, UserPassword, WareHouseID, DockNumber, MachineNumber);
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

        /// <summary>
        /// 检查站台位任务数据（仅用于要料）
        /// </summary>
        /// <param name="EquipmentNumber"></param>
        /// <param name="UserName"></param>
        /// <param name="UserPassword"></param>
        /// <param name="WareHouseID"></param>
        /// <param name="DockNumber"></param>
        /// <returns>FALSE 存在任务；TRUE 没有任务</returns>
        public static bool CheckDockCurTaskByDockNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string DockNumber)
        {
            //return WCFService.CallEndWS.CheckDockCurTaskByDockNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, DockNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                bool res = srv.CheckDockCurTaskByDockNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, DockNumber);
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

        //检查站台位任务数据（仅用于退料）
        public static bool CheckDockCurTaskByDockNumber_Return(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string DockNumber)
        {
            //return WCFService.CallEndWS.CheckDockCurTaskByDockNumber_Return(EquipmentNumber, UserName, UserPassword, WareHouseID, DockNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                bool res = srv.CheckDockCurTaskByDockNumber_Return(EquipmentNumber, UserName, UserPassword, WareHouseID, DockNumber);
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

        //根据类型获取设备数据
        public static DataSet SelectEquipmentWithTypeCode(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string TypeCode)
        {
            //return WCFService.CallEndWS.SelectEquipmentWithTypeCode(EquipmentNumber, UserName, UserPassword, WareHouseID, TypeCode);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                DataSet res = srv.SelectEquipmentWithTypeCode(EquipmentNumber, UserName, UserPassword, WareHouseID, TypeCode);
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


        #region 贵阳新增接口
        //保存是否设置了产生AGV
        public static bool SetAgv(string warehouseId, string workshopId, string machineNumber, string hasAGV)
        {
            return true;//WCFService.CallEndWS.SetAgv(warehouseId, workshopId, machineNumber, hasAGV);
        }

        //显示用户是否设置了产生AGV
        public static string ShowAgv(string warehouseId, string machineNumber)
        {
            return "";//WCFService.CallEndWS.ShowAgv(warehouseId, machineNumber);
        }

        //插入交接班信息
        public static bool InsertClassInfo(string WarehouseID, string RunID, string WorkShop, string TeamDate, string ClassState, string strMachineId, string strRemark, string autoRunID, string productDate, string preClass)
        {
            return true;//WCFService.CallEndWS.InsertClassInfo(WarehouseID,RunID,WorkShop,TeamDate,
            //ClassState,strMachineId,strRemark,autoRunID,productDate,preClass);
        }

        //获取上一班次的信息
        public static DataSet GetPreClassInfo(string strWarehouseId, string strMachineNumber, string strWorkShop, string strClassState)
        {
            return null;//WCFService.CallEndWS.GetPreClassInfo(strWarehouseId,strMachineNumber,strWorkShop,strClassState);
        }

        //修改班次的状态
        public static int UpdateStatus(string WarehouseID, string WorkShop, string MachineNumber, string TeamGroup, string ClassState, string nextClass)
        {
            return 0;//WCFService.CallEndWS.UpdateStatus(WarehouseID,WorkShop,MachineNumber,TeamGroup,ClassState,nextClass);
        }

        #endregion
        //跟踪脚本

        private static void WriteTxtFile(string text)
        {
            try
            {
                FileStream fs = new FileStream("C:\\A.txt", FileMode.Append);
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                sw.Write(text);
                sw.Close();
                fs.Close();

            }
            catch { }
        }

        public static void TrackCallEndErrors(string EquipmentNumber, string UserName, string WareHouseID, int result, string Message, string RightMessages, string Errormessage)
        {
            //try
            //{
            //    WCFService.CallEndWS.TrackCallEndErrors(EquipmentNumber, UserName, WareHouseID, result, Message, RightMessages, Errormessage);
            //}
            //catch { }

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                srv.TrackCallEndErrors(EquipmentNumber, UserName, WareHouseID, result, Message, RightMessages, Errormessage);
            }
            catch (Exception)
            {
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        public static DataTable GetMachineShipperSet(string warehouseId, string machineNumber, string modelCode)
        {
            WMSInterface.InitializeWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.InitializeWS>();
                return srv.GetMachineShipperSet(warehouseId, machineNumber, modelCode);
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

        public static bool UpdateMachineProductPlan(string warehouseId, string machineNumber, string planNo, string state, out string msg)
        {
            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                return srv.UpdateMachineProductPlan(warehouseId, machineNumber, planNo, state, out msg);
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


        public static bool updatePlan(string warehouseId, string equitmentidwd, string equitmentnumberwd, string schemeidwd, string itemidwd, string itemcodewd, string statecodewd, string memowd, string useridwd, out string msg)
        {
            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                return srv.updatePlan(warehouseId, equitmentidwd, equitmentnumberwd, schemeidwd, itemidwd, itemcodewd, statecodewd, memowd, useridwd, out msg);
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



        //add by wd  updateSendTest
        public static bool updateSendTest(string warehouseId, string equitmentidwd, string equitmentnumberwd, string schemeidwd, string itemidwd, string itemcodewd, string statecodewd, string memowd, string useridwd, out string msg)
        {
            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                return srv.updateSendTest(warehouseId, equitmentidwd, equitmentnumberwd, schemeidwd, itemidwd, itemcodewd, statecodewd, memowd, useridwd, out msg);
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






        public static int UpdateMachineCurPlanBySchemeid(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string schemeid)
        {
            //return WCFService.CallEndWS.GetMachineCurPlanByMachineNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                int flag = srv.UpdateMachineCurPlanBySchemeid(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, schemeid);
                return flag;
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
        public static int UpdateMachineCurPlanBySchemeid_accept(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string schemeid)
        {
            //return WCFService.CallEndWS.GetMachineCurPlanByMachineNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                int flag = srv.UpdateMachineCurPlanBySchemeid_accept(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, schemeid);
                return flag;
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
        public static DataSet GetMachineCurPlanByMachineNumber1(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string schemeid)
        {
            //return WCFService.CallEndWS.GetMachineCurPlanByMachineNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                DataSet ds = srv.GetMachineCurPlanByMachineNumber1(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, schemeid);
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
        #endregion
    }
}
