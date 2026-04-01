using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Text;
using static NtiCloud.Model.WMS.MachineMaterialInput;
using static NtiCloud.Service.WMS.Common.DBHelper;

namespace NtiCloud.Service.WMS.Base
{
    public class CallEnd : WmsObject
    {
        #region 调用方法

        #region 获取AGV
        /// <summary>
        /// 获取AGV
        /// </summary>
        /// <returns></returns>
        public DataRow GetAgv()
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "equipment_type", EquipmentType.EquipmentType_AGV.ToString() }
            };
            string sql = $"select * from &DataSource&wms_m_equipment t where t.equipment_type=@equipment_type and t.act_status='1' ";
            DataTable dt = DBHelper.Select(sql, param, "查询AGV");
            return dt.Rows[0];
        }
        #endregion

        #region 根据来源单id、目的地址id和单据类型，获取任务id
        /// <summary>
        /// 根据来源单id、目的地址id和单据类型，获取任务id
        /// </summary>
        /// <param name="refDocId">来源单id</param>
        /// <param name="destAddrId">目的地址id</param>
        /// <param name="refDocType">单据类型</param>
        /// <returns></returns>
        public DataTable GetOutboundTaskId(string refDocId, string destAddrId, string refDocType = null)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "refDocId", refDocId },
                { "destAddrId", destAddrId }
            };
            string sql = $"select * from &DataSource&wms_b_task t where t.ref_doc_id=@refDocId and t.dest_addr_id=@destAddrId and t.act_status='1'";
            if (refDocType != null)
            {
                param.Add("refDocType", refDocType);
                sql += $" and t.ref_doc_type=@refDocType";
            }

            sql += "  order by t.create_date desc";
            DataTable dt = DBHelper.Select(sql, param, "查询站台对应的任务");
            return dt;
        }
        #endregion

        #region 根据任务id，获取任务信息
        /// <summary>
        /// 根据任务id，获取任务信息
        /// </summary>
        /// <param name="taskId">任务id</param>
        /// <returns></returns>
        public DataTable GeTaskInfoByTaskId(string taskId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "taskId", taskId }
            };
            string sql = $"select * from &DataSource&wms_b_task t where t.t_id=@taskId and t.task_status not in( '{TaskStatus.TaskStatus_Finished}') and t.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "查询任务");
            return dt;
        }
        #endregion

        #region 检查站台当前任务
        /// <summary>
        /// 检查站台当前任务
        /// </summary>
        /// <param name="dock">站台id</param>
        /// <returns></returns>
        public DataRow SelectTaskNumByDock(string dock)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "dock", dock }
            };
            string sql = $"select count(t.t_id) as cnt from &DataSource&wms_b_task t where (t.dest_addr_id=@dock or t.src_addr_id=@dock)  and t.task_status not in ('{TaskStatus.TaskStatus_Finished}','{TaskStatus.TaskStatus_PartialFinished}') and t.ref_doc_type in ('{DocType.DocType_CallTerminal}','{DocType.DocType_ReturnBadPallet}','{DocType.DocType_ClientReturnPallet}','{DocType.DocType_ClientReturnNullPallet}','{DocType.DocType_PersonReceiveMateriel}') and t.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "查询站台对应的任务数量");

            return dt.Rows[0];
        }
        #endregion

        #region 检查机台当前是否存在未完成的任务
        /// <summary>
        /// 检查机台当前是否存在未完成的任务
        /// </summary>
        /// <param name="machine">机台id</param>
        /// <returns></returns>
        public bool SelectTaskNumByMachine(string machine)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "machine", machine }
            };
            //string sql = $"select count(t.t_id) as cnt from &DataSource&wms_b_task t where (t.dest_addr_id in (select md.dock_id from &DataSource&wms_m_machine_dock md where md.machine_id = @machine and md.act_status = '1') or t.src_addr_id in(select md.dock_id from &DataSource&wms_m_machine_dock md where md.machine_id = @machine and md.act_status = '1'))  and t.task_status not in ('{TaskStatus.TaskStatus_Finished}','{TaskStatus.TaskStatus_PartialFinished}') and t.ref_doc_type in ('{DocType.DocType_CallTerminal}','{DocType.DocType_ReturnBadPallet}','{DocType.DocType_ClientReturnPallet}','{DocType.DocType_ClientReturnNullPallet}','{DocType.DocType_PersonReceiveMateriel}') and t.act_status='1'";
            string sql = $"select sum(kk.cnt) as cnt from (select count(t.t_id) as cnt from &DataSource& wms_b_task t where(t.dest_addr_id in (select md.dock_id from &DataSource& wms_m_machine_dock md where md.machine_id = @machine and md.act_status = '1') or t.src_addr_id in(select md.dock_id from &DataSource& wms_m_machine_dock md where md.machine_id = @machine and md.act_status = '1'))  and t.task_status not in ('{TaskStatus.TaskStatus_Finished}') and t.ref_doc_type in ('{DocType.DocType_CallTerminal}') and t.act_status = '1' union all select count(t.t_id) as cnt from &DataSource& wms_b_task t where(t.dest_addr_id in (select md.dock_id from &DataSource& wms_m_machine_dock md where md.machine_id = @machine and md.act_status = '1') or t.src_addr_id in(select md.dock_id from &DataSource& wms_m_machine_dock md where md.machine_id = @machine and md.act_status = '1'))  and t.task_status not in ('{TaskStatus.TaskStatus_Finished}', '{TaskStatus.TaskStatus_PartialFinished}') and t.ref_doc_type in   ('{DocType.DocType_ReturnBadPallet}', '{DocType.DocType_ClientReturnPallet}', '{DocType.DocType_ClientReturnNullPallet}','{DocType.DocType_PersonReceiveMateriel}') and t.act_status = '1') kk";
            DataTable dt = DBHelper.Select(sql, param, "检查机台当前是否存在未完成的任务");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) > 0;
        }
        #endregion

        #region 获取当前生产工单对应的最新的要料或者接班任务
        /// <summary>
        /// 获取当前生产工单对应的最新的要料或者接班任务
        /// </summary>
        /// <param name="planOutId">当前生产工单id</param>
        /// <returns></returns>
        public DataTable GetTheNewTaskInfoByPlanOutId(string planOutId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "planOutId", planOutId }
            };
            string sql = $"select his.* from &DataSource&wms_b_task_item_his his where his.task_id = (select u.t_id from (select h.t_id from &DataSource&wms_b_task_his h where h.ref_doc_id = '{planOutId}' and h.ref_doc_type in ('{DocType.DocType_CallTerminal}', '{DocType.DocType_ReturnMaterielReturnBill}') and h.act_status = '1' order by h.create_date desc) u where rownum = 1) and his.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "获取当前生产工单对应的最新的要料或者接班任务");

            return dt;
        }
        #endregion

        #region 获取站台信息
        /// <summary>
        /// 获取站台信息
        /// </summary>
        /// <param name="dock">站台id</param>
        /// <returns></returns>
        public DataRow GetDockStatus(string dock)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "dock", dock }
            };
            string sql = $"select m.t_id,m.doc_id,e.zone_id,m.machine_id,e.equipment_status as dock_status, m.dock_name,m.prod_section,(select s.status from &DataSource&wms_m_section s where s.t_id=m.prod_section and s.act_status='1') as sectionStatus, e.equipment_code,m.container_id,h.bom_code,h.t_id as bomId,m.memo   from &DataSource&wms_m_machine_dock m left join &DataSource&wms_d_prod_plan_out p on p.equipment_id=m.machine_id  left join &DataSource&wms_m_equipment e on e.t_id = m.dock_id left join &DataSource&wms_m_bom_h h on h.item_id = p.item_id where m.dock_id = @dock  and h.prod_section=m.prod_section and m.act_status = '1' and p.act_status='1' and e.act_status='1' and h.act_status='1' and rownum=1";
            DataTable dt = DBHelper.Select(sql, param, "获取站台信息!");

            return dt.Rows[0];
        }
        #endregion

        #region 获取站台和Bom信息
        /// <summary>
        /// 获取站台和Bom信息
        /// </summary>
        /// <param name="dock">站台id</param>
        /// <returns></returns>
        public DataRow GetDockAndBomInfo(string dock)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "dock", dock }
            };
            //string sql = $"select m.t_id,m.doc_id,e.zone_id,m.machine_id,e.equipment_status as dock_status, m.dock_name,m.prod_section,(select s.status from &DataSource&wms_m_section s where s.t_id=m.prod_section and s.act_status='1') as sectionStatus, e.equipment_code,m.container_id,h.bom_code,h.t_id as bomId,m.memo   from &DataSource&wms_m_machine_dock m left join &DataSource&wms_d_prod_plan_out p on p.equipment_id=m.machine_id  left join &DataSource&wms_m_equipment e on e.t_id = m.dock_id left join &DataSource&wms_m_bom_h h on h.item_id = p.item_id  and h.t_id=(select co.bom_id from &DataSource&wms_b_container_h co where co.t_id =m.container_id) where m.dock_id = @dock  and h.prod_section=m.prod_section and m.act_status = '1' and p.plan_status='{PlanOutStatus.PlanOutStatus_Executing}' and p.act_status='1' and e.act_status='1' and h.act_status='1' and rownum=1";
            string sql = $"SELECT m.t_id, m.doc_id, e.zone_id, m.machine_id, e.equipment_status AS dock_status , m.dock_name, m.prod_section , ( SELECT s.status FROM &DataSource&wms_m_section s WHERE s.t_id = m.prod_section AND s.act_status = '1' ) AS sectionStatus, e.equipment_code, m.container_id, h.bom_code, h.t_id AS bomId , m.memo FROM &DataSource&wms_m_machine_dock m LEFT JOIN &DataSource&wms_d_prod_plan_out p ON p.equipment_id = m.machine_id LEFT JOIN &DataSource&wms_m_equipment e ON e.t_id = m.dock_id INNER JOIN &DataSource&wms_b_container_h co ON co.t_id = m.container_id LEFT JOIN &DataSource&wms_m_bom_h h ON h.item_id = p.item_id AND h.t_id = co.bom_id WHERE (m.dock_id = @dock AND h.prod_section = m.prod_section AND m.act_status = '1' AND p.plan_status = '{PlanOutStatus.PlanOutStatus_Executing}' AND p.act_status = '1' AND e.act_status = '1' AND h.act_status = '1' AND rownum = 1)";
            DataTable dt = DBHelper.Select(sql, param, "获取站台和Bom信息!");

            return dt.Rows[0];
        }
        #endregion

        #region 获取机台信息
        /// <summary>
        /// 获取机台信息
        /// </summary>
        /// <param name="machineId">机台id</param>
        /// <returns></returns>
        public DataRow GetMachineInfoByMachineId(string machineId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "machineId", machineId }
            };
            string sql = $"select e.* from &DataSource&wms_m_equipment e where e.t_id=@machineId and e.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "获取机台信息");

            return dt.Rows[0];
        }
        #endregion

        #region 获取设备信息
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="equipmentId">设备id</param>
        /// <returns></returns>
        public DataRow GetEquipmentInfo(string equipmentId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "equipmentId", equipmentId }
            };
            string sql = $"select e.* from &DataSource&wms_m_equipment e where e.t_id = @equipmentId and e.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "获取设备信息");

            return dt.Rows[0];
        }
        #endregion

        #region 根据区域和设备用途获取退空托盘/退坏料时目的站台信息
        /// <summary>
        /// 根据区域和设备用途获取退空托盘/退坏料时目的站台信息
        /// </summary>
        /// <param name="zoneId">区域id</param>
        /// <param name="equipmentUsage">设备用途</param>
        /// <returns></returns>
        public DataTable GetReturnNullPalletDestAddrInfo(string zoneId, string equipmentUsage)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "zoneId", zoneId },
                { "equipmentUsage", equipmentUsage }
            };
            string sql = $"select e.* from &DataSource&wms_m_equipment e where e.zone_id = @zoneId and e.equipment_usage=@equipmentUsage and e.act_status='1' and e.equipment_status='{EquipmentStatus.EquipmentStatus_OK}'";
            DataTable dt = DBHelper.Select(sql, param, "根据区域和设备用途获取目的站台信息");

            return dt;
        }
        #endregion

        #region 根据区域和设备用途获取成品转移时目的站台信息
        /// <summary>
        /// 根据区域和设备用途获取成品转移时目的站台信息
        /// </summary>
        /// <param name="zoneId">区域id</param>
        /// <param name="equipmentUsage">设备用途</param>
        /// <returns></returns>
        public DataTable GetProductTransferDestAddrInfo(string zoneId, string equipmentUsage)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "zoneId", zoneId },
                { "equipmentUsage", equipmentUsage }
            };
            string sql = $"select e.* from &DataSource&wms_m_equipment e where e.zone_id = @zoneId and e.equipment_usage=@equipmentUsage and e.act_status='1' and e.equipment_status='{EquipmentStatus.EquipmentStatus_OK}' and e.equipment_type='{EquipmentType.EquipmentType_Station}'";
            DataTable dt = DBHelper.Select(sql, param, "根据区域和设备用途获取目的站台信息");

            return dt;
        }
        #endregion

        #region 根据挂在站台上的容器主档id获取站台信息
        /// <summary>
        /// 根据挂在站台上的容器主档id获取站台信息
        /// </summary>
        /// <param name="containerID">容器主档id</param>
        /// <returns></returns>
        public DataRow GetDockInfoByContainerID(string containerID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "containerID", containerID }
            };
            string sql = $"select md.* from &DataSource&wms_m_machine_dock md where md.container_id = @containerID and md.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "根据挂在站台上的容器主档id获取站台信息");

            return dt.Rows[0];
        }
        #endregion

        #region 获取生产工单计划信息
        /// <summary>
        /// 获取生产工单计划信息
        /// </summary>
        /// <param name="productPlanOutId">工单id</param>
        /// <returns></returns>
        public DataRow[] GetProductPlanInfo(string productPlanOutId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "productPlanOutId", productPlanOutId }
            };
            string sql = $"select p.t_id,p.plan_status,p.job_no,p.item_id,p.item_code,p.item_name,p.equipment_id,p.shift_id, p.team_id from &DataSource&wms_d_prod_plan_out p where p.t_id=@productPlanOutId and p.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "获取生产工单计划信息");

            return dt.AsEnumerable()?.ToArray();
        }
        #endregion

        #region 根据生产工单单号获取生产工单计划信息
        /// <summary>
        /// 根据生产工单单号获取生产工单计划信息
        /// </summary>
        /// <param name="productPlanOutNo">生产工单单号</param>
        /// <returns></returns>
        public DataRow GetProductPlanInfoByNo(string productPlanOutNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "productPlanOutNo", productPlanOutNo }
            };
            string sql = $"select p.t_id,p.plan_status,p.job_no,p.item_id,p.item_code,p.item_name,p.equipment_id,p.shift_id, p.team_id from &DataSource&wms_d_prod_plan_out p where p.job_no=@productPlanOutNo and p.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "获取生产工单计划信息");

            return dt.Rows[0];
        }
        #endregion

        #region 根据机台获取正在生产中的生产工单计划信息
        /// <summary>
        /// 根据机台获取正在生产中的生产工单计划信息
        /// </summary>
        /// <param name="machineId">机台id</param>
        /// <returns></returns>
        public DataRow[] GetProductPlanInfoByMachineId(string machineId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "machineId", machineId }
            };
            string sql = $"select p.t_id,p.plan_status,p.job_no,p.item_id,p.item_code,p.item_name,p.equipment_id from &DataSource&wms_d_prod_plan_out p where p.equipment_id=@machineId and p.plan_status='{PlanOutStatus.PlanOutStatus_Executing}' and p.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "根据机台获取正在生产中的生产工单计划信息!");

            return dt.AsEnumerable()?.ToArray();
        }
        #endregion

        #region 根据机台和站台id获取机台站台表中的信息
        /// <summary>
        /// 根据机台和站台id获取机台站台表中的信息
        /// </summary>
        /// <param name="machineId">机台id</param>
        /// <param name="dockId">站台id</param>
        /// <returns></returns>
        public DataTable GetMachineDockInfoByMachineAndDock(string machineId, string dockId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "machineId", machineId },
                { "dockId", dockId }
            };
            string sql = $"select * from &DataSource&wms_m_machine_dock md where md.machine_id='{machineId}' and md.dock_id='{dockId}' and md.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "根据机台和站台获取机台站台表中的信息");

            return dt;
        }
        #endregion

        #region 根据生产工单号和容器主档获取人工领料主档信息
        /// <summary>
        /// 根据生产工单号和容器主档获取人工领料主档信息
        /// </summary>
        /// <param name="planOutNo">生产工单号</param>
        /// <param name="containerHTid">容器主档id</param>
        /// <returns></returns>
        public DataTable GetPickReturnHByPlanOutNoAndContailnerH(string planOutNo, string containerHTid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "planOutNo", planOutNo },
                { "containerHTid", containerHTid }
            };
            string sql = $"select h.* from &DataSource&wms_d_pick_return_h h where h.job_no='{planOutNo}' and h.container_id='{containerHTid}' and h.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "根据生产工单号和容器主档获取人工领料主档信息");

            return dt;
        }
        #endregion

        #region 获取站台BOM信息
        /// <summary>
        /// 获取站台BOM信息
        /// </summary>
        /// <param name="dockId">站台id</param>
        /// <returns></returns>
        public DataTable GetBOMInfo(string dockId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                //{ "productPlanOutId", productPlanOutId },
                { "dockId", dockId }
            };
            //string sql = $"select distinct h.t_id,p.item_id,h.create_date from &DataSource&wms_m_bom_h h inner join &DataSource&wms_d_prod_plan_out p on p.plan_status='{PlanOutStatus.PlanOutStatus_Executing}' and h.item_id=p.item_id and h.bom_status='{BOMStatus.BOMStatus_Using}' and h.can_be_called='1' inner join &DataSource&wms_m_machine_dock d on d.machine_id=p.equipment_id and h.prod_section=d.prod_section where d.dock_id=@dockId and d.act_status='1' and h.act_status='1' and p.act_status='1' order by h.create_date";
            string sql = $" select distinct * from( select distinct h.t_id, p.item_id, h.create_date,0 selected from &DataSource&wms_m_bom_h h inner join &DataSource&wms_d_prod_plan_out p on p.plan_status = '{PlanOutStatus.PlanOutStatus_Executing}' and h.item_id = p.item_id and h.bom_status = '{BOMStatus.BOMStatus_Using}' and h.can_be_called = '1' inner join &DataSource&wms_m_machine_dock d on d.machine_id = p.equipment_id and h.prod_section = d.prod_section where d.dock_id = @dockId and d.act_status = '1' and h.act_status = '1' and p.act_status = '1' union all select distinct h.t_id, p.item_id, h.create_date,1 selected from &DataSource&wms_m_bom_h h inner join &DataSource&wms_b_bom_set k on k.bom_id=h.t_id and k.act_status='1' inner join &DataSource&wms_d_prod_plan_out p on p.plan_status = '{PlanOutStatus.PlanOutStatus_Executing}' and k.item_id = p.item_id and h.bom_status = '{BOMStatus.BOMStatus_Using}' and h.can_be_called = '1' inner join &DataSource&wms_m_machine_dock d on d.machine_id = p.equipment_id and k.prod_section = d.prod_section where d.dock_id = @dockId and d.act_status = '1' and h.act_status = '1' and p.act_status = '1') x order by x.selected,x.create_date";
            DataTable dt = DBHelper.Select(sql, param, "获取站台在用BOM信息");
            return dt;
        }
        #endregion

        #region 指定bomId是否有库存
        /// <summary>
        /// 指定bomId是否有库存
        /// </summary>
        /// <param name="bomId">bomId</param>
        /// <returns>true:有库存；false：没有库存</returns>
        public bool isHaveStockByBomId(string bomId,string zoneId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                //{ "productPlanOutId", productPlanOutId },
                { "bomId", bomId },
                { "zoneId", zoneId }
            };
            string sql = $"select count(h.t_id) as cnt from &DataSource&wms_b_container_h h inner join &DataSource&wms_m_location l on h.location_id=l.t_id where h.bom_id=@bomId and h.location_id is not null and h.act_status='1' and h.container_type='ContainerType_Actual' and h.container_barcode not in ('00000000','99999999')  AND H.LOCATION_ID<>'1268783997165805569' and l.zone_id=@zoneId  and l.location_status='{LocationStatus.LocationStatus_Stored}' and l.location_flag in ('{LocationFlag.LocationFlag_OutOnly}','{LocationFlag.LocationFlag_Usable}')";
            DataTable dt = DBHelper.Select(sql, param, "根据bom查询库存");
            return Convert.ToInt32(dt.Rows[0]["cnt"]) > 0;
        }
        #endregion

        #region 指定bomId是否有库存
        /// <summary>
        /// 指定bomId是否有库存
        /// </summary>
        /// <param name="bomId">bomId</param>
        /// <returns>true:有库存；false：没有库存</returns>
        public bool isHaveStockByBomId2(string bomId, string zoneId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                //{ "productPlanOutId", productPlanOutId },
                { "bomId", bomId },
                { "zoneId", zoneId }
            };
            string sql = $"select count(h.t_id) as cnt from &DataSource&wms_b_container_h h inner join &DataSource&wms_m_location l on h.location_id=l.t_id where h.bom_id=@bomId and h.location_id is not null and h.act_status='1' and h.container_type='ContainerType_Actual' and h.container_barcode not in ('00000000','99999999')  AND H.LOCATION_ID<>'1268783997165805569' and l.zone_id=@zoneId  and l.location_status='{LocationStatus.LocationStatus_Stored}' and l.location_flag in ('{LocationFlag.LocationFlag_OutOnly}','{LocationFlag.LocationFlag_Usable}','{LocationFlag.LocationFlag_InOnly}')";
            DataTable dt = DBHelper.Select(sql, param, "根据bom查询库存");
            return Convert.ToInt32(dt.Rows[0]["cnt"]) > 0;
        }
        #endregion

        #region 查询货位对应区域信息
        /// <summary>
        /// 查询货位对应区域信息
        /// </summary>
        /// <param name="loc_id">货位id</param>
        /// <returns></returns>
        public DataTable GetLocationInfo(string loc_id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "loc_id", loc_id }
            };
            string sql = $"select l.zone_id,l.location_code,h.t_id,l.location_status,l.location_row from &DataSource&wms_m_location l left join &DataSource&wms_b_container_h h on h.location_id = l.t_id  where l.t_id = @loc_id and l.act_status = '1'";
            DataTable dt = DBHelper.Select(sql, param, "查询货位对应区域信息");

            return dt;
        }
        #endregion

        #region 查询货位盘型
        /// <summary>
        /// 查询货位盘型
        /// </summary>
        /// <param name="loc_id">货位id</param>
        /// <returns></returns>
        public DataTable GetBOMByLocationId(string loc_id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "loc_id", loc_id }
            };
            string sql = $"select distinct h.bom_id,l.location_no,h.t_id from &DataSource&wms_m_location l right join &DataSource&wms_b_container_h h on h.location_id = l.t_id  where l.t_id = @loc_id and l.act_status = '1' ";
            DataTable dt = DBHelper.Select(sql, param, "查询货位盘型", false);

            return dt;
        }
        #endregion

        #region 根据站台修改机台站台表中的容器id
        /// <summary>
        /// 根据站台修改机台站台表中的容器id
        /// </summary>
        /// <param name="machineDockTid">机台站台表的tid</param>
        /// <param name="container_htid">容器id</param>
        /// <param name="doc_id">交班时工单id</param>
        /// <param name="bom_id">BOMID</param>
        public void UpdateMachineDockDeatilId(string machineDockTid, string container_htid, string doc_id, string bom_id, string pallet_code)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "container_htid", container_htid },
                { "doc_id", doc_id},
                { "bom_id", bom_id},
                { "pallet_code", pallet_code},
                { "machineDockTid",  machineDockTid }
            };
            string sql = "update &DataSource&wms_m_machine_dock r set r.container_id=@container_htid,r.last_modified_by=@last_modified_by,r.modify_ip=@modify_ip,r.modify_date=sysdate,r.doc_id=@doc_id,r.bom_id=@bom_id,r.pallet_code=@pallet_code  ";
            sql += $" where r.t_id=@machineDockTid  and r.act_status='1'";
            DBHelper.RunSql(sql, param, "根据站台修改机台站台表中的容器id");
        }
        #endregion

        #region 修改站台状态
        /// <summary>
        /// 修改站台状态
        /// </summary>
        /// <param name="machineDockTid">机台站台t_id</param>
        /// <param name="equipmentTid">设备表t_id</param>
        /// <param name="status">站台状态</param>
        public void UpdateDockStatus(string machineDockTid, string equipmentTid, string status)
        {
            //UpdateMachineDockStatus(machineDockTid, status);
            UpdateDockStatusInEquipment(equipmentTid, status);
        }
        #endregion

        #region 根据站台修改机台站台表中的站台状态
        /// <summary>
        /// 根据站台修改机台站台表中的站台状态
        /// </summary>
        /// <param name="machineDockTid">站台id</param>
        /// <param name="status">站台状态</param>
        public void UpdateMachineDockStatus(string machineDockTid, string status)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "status", status },
                { "machineDockTid",  machineDockTid }
            };
            string sql = " update &DataSource&wms_m_machine_dock r set r.dock_status=@status,r.last_modified_by=@last_modified_by,r.modify_ip=@modify_ip,r.modify_date=sysdate where r.t_id=@machineDockTid  and r.act_status='1'";
            DBHelper.RunSql(sql, param, "根据站台修改机台站台表中的站台状态");
        }
        #endregion

        #region 根据站台修改设备表中的站台状态
        /// <summary>
        /// 根据站台修改设备表中的站台状态
        /// </summary>
        /// <param name="equipmentTid">站台id</param>
        /// <param name="status">站台状态</param>
        public void UpdateDockStatusInEquipment(string equipmentTid, string status)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "status", status },
                { "equipmentTid",  equipmentTid }
            };
            string sql = " update &DataSource&wms_m_equipment q set q.equipment_status=@status,q.last_modified_by=@last_modified_by,q.modify_ip=@modify_ip,q.modify_date=sysdate  where q.t_id=@equipmentTid and q.act_status='1'";
            DBHelper.RunSql(sql, param, "根据站台修改设备表中的站台状态");
        }
        #endregion

        #region 根据容器id，取消容器与货位的绑定关系
        /// <summary>
        /// 根据容器id，取消容器与货位的绑定关系
        /// </summary>
        /// <param name="container_htid">容器id</param>
        public void SetLocIsNullInContainerHead(string container_htid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "container_htid", container_htid }
            };
            string sql = "update &DataSource&wms_b_container_h h set h.location_id =  null ,h.last_modified_by=@last_modified_by,h.modify_ip=@modify_ip,h.modify_date=sysdate where h.t_id=@container_htid  and h.act_status='1'";
            DBHelper.RunSql(sql, param, "根据容器id，取消容器与货位的绑定关系");
        }
        #endregion

        #region 修改货位状态
        /// <summary>
        /// 修改货位状态
        /// </summary>
        /// <param name="locationId">货位id</param>
        /// <param name="newlocStatus">新货位状态</param>
        /// <param name="oldLocStatus">旧货位状态</param>
        /// <param name="modifyStateReason">修改货位状态原因</param>
        public void UpdateLocStatusByLocationId(string locationId, string newlocStatus, string oldLocStatus, string modifyStateReason)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "location_status", newlocStatus },
                { "status_reason", modifyStateReason },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", locationId },
                { "oldLocStatus", oldLocStatus }
            };
            string sql = "update &DataSource&wms_m_location l set l.location_status=@location_status,l.status_reason=@status_reason,l.last_modified_by=@last_modified_by,l.modify_ip=@modify_ip,l.modify_date=sysdate where l.t_id=@t_id  and l.act_status='1' and l.location_status=@oldLocStatus";
            DBHelper.RunSql(sql, param, "修改货位状态");
        }
        #endregion

        #region 根据机台和站台，获取挂在站台上面的容器主档id
        /// <summary>
        /// 根据机台和站台，获取挂在站台上面的容器主档id
        /// </summary>
        /// <param name="dock">站台id</param>
        /// <param name="machineId">机台id</param>
        /// <returns></returns>
        public DataTable GetContainerInfoByDockIdAndMachineId(string dock, string machineId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "dock", dock },
                { "machineId", machineId }
            };
            string sql = $"select md.container_id from &DataSource&wms_m_machine_dock md where md.machine_id=@machineId and md.dock_id = @dock and md.act_status = '1'";
            DataTable dt = DBHelper.Select(sql, param, "根据机台和站台，获取挂在站台上面的容器主档");

            return dt;
        }
        #endregion

        #region 根据机台站台t_id，获取挂在站台上面的容器主档id
        /// <summary>
        /// 根据机台站台t_id，获取挂在站台上面的容器主档id
        /// </summary>
        /// <param name="machineDockId">站台id</param>
        /// <returns></returns>
        public DataTable GetContainerInfoByMachineDockId(string machineDockId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "machineDockId", machineDockId }
            };
            string sql = $"select md.container_id from &DataSource&wms_m_machine_dock md where md.t_id = @machineDockId and md.act_status = '1'";
            DataTable dt = DBHelper.Select(sql, param, "获取挂在站台上面的容器主档");

            return dt;
        }
        #endregion

        #region datarow[] 转换成 datatable
        /// <summary>
        /// datarow[] 转换成 datatable
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public DataTable ToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone(); // 复制DataRow的表结构
            foreach (DataRow row in rows)
            {

                tmp.ImportRow(row); // 将DataRow添加到DataTable中
            }
            return tmp;
        }
        #endregion

        #region 根据id获取人工领/退料主档单据信息
        /// <summary>
        /// 根据id获取人工领/退料主档单据信息
        /// </summary>
        /// <param name="pickReturnhTid">主档id</param>
        /// <returns></returns>
        public DataRow SelectPickReturnHById(string pickReturnhTid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "pickReturnhTid", pickReturnhTid }
            };
            string sql = $"select h.pick_return_no,h.pick_return_type,h.pick_return_status,h.shift_id,h.team_id,h.job_no,h.ref_doc_no,h.equipment_id,h.dock_id,h.dock_name,(select e.equipment_code from &DataSource&wms_m_equipment e where e.t_id=h.dock_id and e.act_status='1') as dock_code from &DataSource&wms_d_pick_return_h h where h.t_id=@pickReturnhTid and h.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "获取人工领/退料主档单据信息");

            return dt.Rows[0];
        }
        #endregion

        #region 根据单号获取人工领/退料主档单据信息
        /// <summary>
        /// 根据单号获取人工领/退料主档单据信息
        /// </summary>
        /// <param name="pickReturnhNo">单号</param>
        /// <returns></returns>
        public DataRow SelectPickReturnHByNo(string pickReturnhNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "pickReturnhNo", pickReturnhNo }
            };
            string sql = $"select h.*,(select e.equipment_code from &DataSource&wms_m_equipment e where e.t_id=h.dock_id and e.act_status='1') as dock_code from &DataSource&wms_d_pick_return_h h where h.pick_return_no=@pickReturnhNo and h.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "获取人工领/退料主档单据信息");

            return dt.Rows[0];
        }
        #endregion

        #region 获取人工领/退料明细单据信息
        /// <summary>
        /// 获取人工领/退料明细单据信息
        /// </summary>
        /// <param name="pickReturnhTid">主档id</param>
        /// <returns></returns>
        public DataTable SelectPickReturnDById(string pickReturnhTid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "pickReturnhTid", pickReturnhTid }
            };
            string sql = $"select d.t_id, d.container_dtid, d.item_id, d.item_code, d.item_name, d.lot_no, d.suppliy, d.qty, d.uom_qty_id, d.memo from &DataSource&wms_d_pick_return_d d where d.pick_return_id=@pickReturnhTid and d.act_status='1' order by d.create_date desc ";
            DataTable dt = DBHelper.Select(sql, param, "获取人工领/退料明细单据信息");

            return dt;
        }
        #endregion

        #region 获取暂存区货位对应的容器主档id
        /// <summary>
        /// 获取暂存区货位对应的容器主档id
        /// </summary>
        /// <returns></returns>
        public DataTable GetContainerHTidOfInboundBufferLoc()
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "usage", LocationUse.LocationUse_InboundBuffer.ToString() },
                //{ "locationType", LocationType.LocationType_3DRack.ToString() },
                { "containerType", ContainerType.ContainerType_Virtual.ToString() }
            };
            string sql = $"select distinct h.*,l.zone_id,l.location_code,l.t_id as loc_id from &DataSource&wms_b_container_h h inner join &DataSource&wms_m_location l on h.location_id=l.t_id and l.usage=@usage and h.container_type=@containerType  where h.act_status='1' and l.act_status='1'";
            //string sql = $"select distinct h.*,l.zone_id,l.location_code,l.t_id as loc_id from &DataSource&wms_b_container_h h inner join &DataSource&wms_m_location l on h.location_id=l.t_id and l.usage=@usage and h.container_type=@containerType  where h.act_status='1' and l.act_status='1'";
            //string sql = $"select distinct h.t_id,l.zone_id,h.container_barcode,l.location_code,l.t_id as loc_id from &DataSource&wms_b_container_h h inner join &DataSource&wms_m_location l on h.location_id=l.t_id and l.usage=@usage and h.container_type=@containerType and l.location_type=@locationType where h.act_status='1' and l.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "获取暂存区货位对应的容器主档");

            return dt;
        }
        #endregion

        #region 将人工领料/退料单与容器主档id关联
        /// <summary>
        /// 将人工领料/退料单与容器主档id关联
        /// </summary>
        /// <param name="pickReturnhTid">主档id</param>
        /// <param name="containerId">容器id</param>
        /// <returns></returns>
        public void UpdatePickReturnContainerIdById(string pickReturnhTid, string containerId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "containerId", containerId },
                { "t_id", pickReturnhTid }
            };
            string sql = "update &DataSource&wms_d_pick_return_h h set h.container_id=@containerId,h.last_modified_by=@last_modified_by,h.modify_ip=@modify_ip,h.modify_date=sysdate where h.t_id=@t_id and h.act_status='1'";
            DBHelper.RunSql(sql, param, "单据信息关联容器");
        }
        #endregion

        #region 修改人工领/退料单据状态
        /// <summary>
        /// 修改人工领/退料单据状态
        /// </summary>
        /// <param name="pickReturnhTid">主档id</param>
        /// <param name="pickReturnStatus">单据状态</param>
        /// <returns></returns>
        public void UpdatePickReturnhStatusById(string pickReturnhTid, string pickReturnStatus)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "pick_return_status", pickReturnStatus },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", pickReturnhTid }
            };
            string sql = "update &DataSource&wms_d_pick_return_h h set h.pick_return_status=@pick_return_status,h.last_modified_by=@last_modified_by,h.modify_ip=@modify_ip,h.modify_date=sysdate where h.t_id=@t_id and h.act_status='1'";
            DBHelper.RunSql(sql, param, "修改人工领/退料单据状态");
        }
        #endregion

        #region 根据机台和站台类型，获取站台id
        /// <summary>
        /// 根据机台和站台类型，获取站台id
        /// </summary>
        /// <param name="machineId">机台</param>
        /// <param name="prodSection">区段类型</param>
        /// <returns></returns>
        public DataTable GetDockInfoByEquipmentIdShiftTeam(string machineId, string prodSection)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "machineId", machineId },
                { "prodSection", prodSection }
            };
            string sql = $"select md.dock_id,e.equipment_code from &DataSource&wms_m_machine_dock md,&DataSource&wms_m_equipment e where md.dock_id=e.t_id and md.machine_id=@machineId and md.prod_section=@prodSection and md.act_status='1' and e.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "根据机台和站台类型，获取站台id");

            return dt;
        }
        #endregion

        #region 修改人工退料明细中容器明细的id
        /// <summary>
        /// 修改人工退料明细中容器明细的id
        /// </summary>
        /// <param name="tId">人工退料明细id</param>
        /// <param name="containerDTid">容器明细id</param>
        public void UpdatePickReturnContainerDTid(string tId, string containerDTid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "container_dtid", containerDTid },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", tId }
            };
            string sql = "update &DataSource&wms_d_pick_return_d l set l.container_dtid   = @container_dtid,l.last_modified_by=@last_modified_by,l.modify_ip=@modify_ip,l.modify_date=sysdate where l.t_id=@t_id and l.act_status='1'";
            DBHelper.RunSql(sql, param, "修改人工退料明细中容器明细的id");
        }
        #endregion

        #region 根据是否交接人工领料，获取机台除交接班站台以外的所有站台：交接领料数据，则包含人工领料站台数据，否则不包含
        /// <summary>
        /// 根据是否交接人工领料，获取机台除交接班站台以外的所有站台：交接领料数据，则包含人工领料站台数据，否则不包含
        /// </summary>
        /// <param name="machineId">机台id</param>
        /// <param name="isChangePersonReceive">是否交接人工领料数据（1:交；0:不交）</param>
        /// <returns></returns>
        public DataTable GetAllDockByMachineAndIsChangePersonReceive(string machineId, string isChangePersonReceive)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "machineId", machineId }
            };
            //string sql = $"select distinct md.*  from &DataSource&wms_m_machine_dock md,&DataSource&wms_m_section s where md.machine_id = @machineId and md.prod_section=s.t_id and s.section_code not like '%交接班%'  and s.section_code  not like '%人工领料%' and md.act_status='1' and s.act_status='1'";
            string sql = $"select distinct md.*  from &DataSource&wms_m_machine_dock md,&DataSource&wms_m_section s where md.machine_id = @machineId and md.prod_section=s.t_id and s.section_code not like '%交接班%' ";

            if (string.Equals(isChangePersonReceive, "0"))//不包含人工领料站台
            {
                sql += $" and s.section_code not like '%人工领料%' ";
            }

            sql += $"  and md.act_status='1' and s.act_status='1'";

            DataTable dt = DBHelper.Select(sql, param, "获取机台除交接班站台以外的所有站台");

            return dt;
        }
        #endregion

        #region 获取机台交接班站台
        /// <summary>
        /// 获取机台交接班站台
        /// </summary>
        /// <param name="machineId">机台id</param>
        /// <returns></returns>
        public DataRow GetShiftDockByMachine(string machineId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "machineId", machineId }
            };
            string sql = $"select distinct md.*  from &DataSource&wms_m_machine_dock md,&DataSource&wms_m_section s where md.machine_id = @machineId and md.prod_section=s.t_id and s.section_code like ('%交接班%') and md.act_status='1' and s.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "获取机台交接班站台");

            return dt.Rows[0];
        }
        #endregion

        #region 获取机台人工领料站台
        /// <summary>
        /// 获取机台人工领料站台
        /// </summary>
        /// <param name="machineId">机台id</param>
        /// <returns></returns>
        public DataRow GetJieBDockByMachine(string machineId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "machineId", machineId }
            };
            string sql = $"select distinct md.*  from &DataSource&wms_m_machine_dock md,&DataSource&wms_m_section s where md.machine_id = @machineId and md.prod_section=s.t_id and s.section_code like ('%人工领料%') and md.act_status='1' and s.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "获取机台人工领料站台");

            return dt.Rows[0];
        }
        #endregion

        #region 获取机台上除了人工领料和交接班站台以外的所有站台信息
        /// <summary>
        /// 获取机台上除了人工领料和交接班站台以外的所有站台信息
        /// </summary>
        /// <param name="machineId">机台id</param>
        /// <returns></returns>
        public DataTable GetAllDockNotContainsRJDockByMachine(string machineId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "machineId", machineId }
            };
            string sql = $"select distinct md.*  from &DataSource&wms_m_machine_dock md,&DataSource&wms_m_section s where md.machine_id = @machineId and md.prod_section=s.t_id and s.section_code not like '%交接班%'  and s.section_code  not like '%人工领料%' and md.act_status='1' and s.act_status='1'";

            DataTable dt = DBHelper.Select(sql, param, "获取机台上除了人工领料和交接班站台以外的所有站台信息");

            return dt;
        }
        #endregion

        #region 获取机台上所有站台信息
        /// <summary>
        /// 获取机台上所有站台信息
        /// </summary>
        /// <param name="machineId">机台id</param>
        /// <returns></returns>
        public DataTable GetAllDockByMachine(string machineId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "machineId", machineId }
            };
            string sql = $"select distinct s.section_code,md.* from &DataSource&wms_m_machine_dock md, &DataSource&wms_m_section s where md.machine_id = @machineId and md.prod_section = s.t_id and md.act_status = '1' and s.act_status = '1'";
            DataTable dt = DBHelper.Select(sql, param, "获取机台上所有站台信息");

            return dt;
        }
        #endregion

        #region 根据物料、供应商、批次和物料数量获取暂存区中未组盘的物料
        /// <summary>
        /// 根据物料、供应商、批次和物料数量获取暂存区中未组盘的物料
        /// </summary>
        /// <param name="itemId">物料id</param>
        /// <param name="lotNo">批次id</param>
        /// <param name="supplierId">供应商id</param>
        /// <param name="qty">物料数量</param>
        /// <returns></returns>
        public DataTable GetItemInboundBuffer(string itemId, string lotNo, string supplierId, string qty)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "itemId", itemId },
                { "lotNo", lotNo },
                { "supplierId", supplierId },
                { "qty", qty }
            };
            string sql = $"select d.* from &DataSource&wms_b_container_h h inner join &DataSource&wms_m_location l on h.location_id = l.t_id and l.usage = '{LocationUse.LocationUse_InboundBuffer}' inner join &DataSource&wms_b_container_d d on h.t_id = d.container_id where h.bom_id is null and h.container_type='{ContainerType.ContainerType_Virtual}' and (d.qty-nvl(d.out_qty,0)-nvl(d.lock_qty,0))>={qty} and d.item_id='{itemId}' and d.lot_no='{lotNo}' and d.supplier_id='{supplierId}' and h.act_status='1' and l.act_status='1' and d.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "根据物料、供应商、批次和物料数量获取暂存区中未组盘的物料");

            return dt;
        }
        #endregion

        #region 根据物料、供应商和批次获取暂存区中未组盘的物料
        /// <summary>
        /// 根据物料、供应商和批次获取暂存区中未组盘的物料
        /// </summary>
        /// <param name="itemId">物料id</param>
        /// <param name="lotNo">批次id</param>
        /// <param name="supplierId">供应商id</param>
        /// <returns></returns>
        public DataTable GetItemInboundBuffer(string itemId, string lotNo, string supplierId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "itemId", itemId },
                { "lotNo", lotNo },
                { "supplierId", supplierId }
            };
            string sql = $"select d.* from &DataSource&wms_b_container_h h inner join &DataSource&wms_m_location l on h.location_id = l.t_id and l.usage = '{LocationUse.LocationUse_InboundBuffer}' inner join &DataSource&wms_b_container_d d on h.t_id = d.container_id where h.bom_id is null and h.container_type='{ContainerType.ContainerType_Virtual}' and d.item_id='{itemId}' and d.lot_no='{lotNo}' and d.supplier_id='{supplierId}' and h.act_status='1' and l.act_status='1' and d.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "获取暂存区中未组盘的物料", false);

            return dt;
        }
        #endregion

        #region 修改人工领料明细中关联的容器明细id
        /// <summary>
        /// 修改人工领料明细中关联的容器明细id
        /// </summary>
        /// <param name="equipmentTid">人工领料明细id</param>
        /// <param name="status">容器明细id</param>
        public void UpdateContainerDTidInPickReturnD(string pickReturnDTid, string containerDTid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "containerDTid", containerDTid },
                { "pickReturnDTid",  pickReturnDTid }
            };
            string sql = " update &DataSource&wms_d_pick_return_d q set q.container_dtid=@containerDTid,q.last_modified_by=@last_modified_by,q.modify_ip=@modify_ip,q.modify_date=sysdate  where q.t_id=@pickReturnDTid and q.act_status='1'";
            DBHelper.RunSql(sql, param, "修改人工领料明细中关联的容器明细id");
        }
        #endregion

        #region 根据BOM查询暂存区物料信息
        /// <summary>
        /// 根据BOM查询暂存区物料信息
        /// </summary>
        /// <param name="bomId">bomId</param>
        /// <returns></returns>
        public DataTable GetLocationByBomId(string bomId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "bomId", bomId }
            };
            string sql = $"select h.t_id,h.location_id,h.bom_id,count(d.t_id) as cnt from &DataSource&wms_b_container_h h left join &DataSource&wms_b_container_d d on h.t_id=d.container_id and d.act_status='1' and d.qty > 0 where h.bom_id=@bomId group by h.t_id,h.location_id,h.bom_id having count(d.t_id)>0";
            DataTable dt = DBHelper.Select(sql, param, "根据BOM查询暂存区物料信息");
            return dt;
        }
        #endregion

        #region 根据来源单号获取已完成的人工退料单据数量
        /// <summary>
        /// 根据来源单号获取已完成的人工退料单据数量
        /// </summary>
        /// <param name="refDocNo">来源单据号</param>
        /// <returns></returns>
        public DataTable GetFinishPersonReturnNum(string refDocNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "refDocNo", refDocNo }
            };
            string sql = $"select count(h.t_id) as cnt from &DataSource&wms_d_pick_return_h h where h.ref_doc_no=@refDocNo and h.pick_return_status='{PickReturnStatus.PickReturnStatus_Finish}' and h.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "根据来源单号获取已完成的人工退料单据数量!");
            return dt;
        }
        #endregion

        #region 根据人工领料单号和容器明细id获取任务明细历史数据
        /// <summary>
        /// 根据人工领料单号和容器明细id获取任务明细历史数据
        /// </summary>
        /// <param name="pickReturnHNo">人工领料单号</param>
        /// <param name="containerDTid">容器明细id</param>
        /// <returns></returns>
        public DataTable GetFinishTaskItemInfo(string pickReturnHNo, string containerDTid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "pickReturnHNo", pickReturnHNo },
                { "containerDTid", containerDTid }
            };
            string sql = $"select i.t_id, i.task_id, i.item_id, i.item_name, i.plan_qty, i.processed_qty, i.finished_qty, i.uom_qty_id, i.finished_stat_qty, i.uom_stat_id, i.container_d_id, i.ref_doc_type, i.ref_doc_id, i.ref_doc_d_id, i.ref_doc_no, i.start_date, i.end_date, i.process_status from &DataSource&wms_b_task_his h,&DataSource&wms_b_task_item_his i where h.t_id = i.task_id and h.act_status = '1' and h.ref_doc_no = @pickReturnHNo and i.container_d_id = @containerDTid";
            DataTable dt = DBHelper.Select(sql, param, "根据人工领料单号和容器明细获取任务明细数据!");
            return dt;
        }
        #endregion

        #region 根据BOMID调用存过获取出库货位
        /// <summary>
        /// 根据BOMID调用存过获取出库货位
        /// </summary>
        /// <param name="bomid">BOMID</param>
        /// <returns>货位id</returns>
        public string GetLocationInfoByBomId(string bomid, string zoneId)
        {
            Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    { "I_WAREHOUSEID", Context.WarehouseID },
                    { "BOMID", bomid },
                    { "ITEMID", string.Empty },
                    { "ZONE_ID",zoneId}
                };
            string twoFloorZoneId = WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336");
            string definedRule;
            if (!string.Equals(zoneId, twoFloorZoneId))
            {//一楼
                definedRule = WmsConfig.GetOrDefault("Outbound:GetLocationByBomId", "work_task_9999");
            }
            else
            {//二楼
                param.Add("LINE", "-1");//第一次货位分配
                definedRule = WmsConfig.GetOrDefault("Outbound:GetLocationByBomId2", "work_task_22");
            }
            LogHelper.LogInformation("根据BOMID调用存过获取出库货位参数：\r\n" + JsonConvert.SerializeObject(param) + "\r\n definedRule=" + definedRule);
            string location = LocationAllocator.AllocateInventory(param, definedRule);
            //location = LocationAllocator.AllocateInventory(param, "work_task_7");
            return location;
        }
        #endregion

        #region 获取高架库区空货位
        /// <summary>
        /// 获取高架库区空货位
        /// </summary>
        /// <returns>货位id</returns>
        public string GetEmptyLocation()
        {
            Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    { "I_WAREHOUSEID", Context.WarehouseID }
                };
            string definedRule = WmsConfig.GetOrDefault("Inbound:GetEmptyLocation", "work_task_6");
            LogHelper.LogInformation("获取空货位参数：\r\n" + JsonConvert.SerializeObject(param) + "\r\n definedRule=" + definedRule);
            string location = LocationAllocator.AllocateInventory(param, definedRule);
            //location = LocationAllocator.AllocateInventory(param, WmsConfig.GetOrDefault("Inbound:GetEmptyLocation", "work_task_6"));
            //location = LocationAllocator.AllocateInventory(param, "work_task_6");

            return location;
        }
        #endregion

        #region 插入生产领料退料主表数据
        /// <summary>
        /// 插入生产领料退料主表数据
        /// </summary>
        /// <param name="model"></param>
        public void InsertPickReturnH(PickReturnHModel model)
        {
            model.Tid = model.Tid ?? SnowflakeID.Guid().ToString();
            model.WarehouseID = model.WarehouseID ?? Context.WarehouseID;
            model.CreatedBy = model.CreatedBy ?? Context.UserInfo.UserName;
            model.ModifyIP = model.ModifyIP ?? Context.UserInfo.LoginIP;
            //model.LastModifiedBy = model.LastModifiedBy ?? Context.UserInfo.UserName;
            model.ActStatus = "1";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "t_id", model.Tid },
                { "pick_return_no", model.PickReturnNo },
                { "pick_return_type", model.PickReturnType },
                { "job_no", model.JobNo },
                { "equipment_id", model.EquipmentId },
                { "dock_id", model.DockId },
                { "dock_name", model.DockName },
                { "item_id", model.ItemId },
                { "item_name", model.ItemName },
                { "container_id", model.ContainerId },
                { "shift_id", model.ShiftId },
                { "team_id", model.TeamId },
                { "pick_return_date", model.PickReturnDate.ToString() },
                { "route", model.Route },
                { "ref_doc_no", model.RefDocNo},
                { "src_addr_id", model.SrcAddrId },
                { "src_addr_type", model.SrcAddrType },
                { "src_addr_text", model.SrcAddrText },
                { "dest_addr_id", model.DestAddrId },
                { "dest_addr_type", model.DestAddrType },
                { "dest_addr_text", model.DestAddrText },
                { "production_line", model.ProductionLine },
                { "is_rf", model.IsRf },
                { "reason", model.Reason },
                { "pick_return_status", model.PickReturnStatus },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into &DataSource&wms_d_pick_return_h ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       pick_return_no, " +
                "       pick_return_type, " +
                "       job_no, " +
                "       equipment_id, " +
                "       dock_id, " +
                "       dock_name, " +
                "       item_id, " +
                "       item_name, " +
                "       container_id, " +
                "       shift_id, " +
                "       team_id, " +
                "       pick_return_date, " +
                "       route, " +
                "       ref_doc_no, " +
                "       src_addr_id, " +
                "       src_addr_type, " +
                "       src_addr_text, " +
                "       dest_addr_id, " +
                "       dest_addr_type, " +
                "       dest_addr_text, " +
                "       production_line, " +
                "       is_rf, " +
                "       reason, " +
                "       pick_return_status, " +
                "       create_date, " +
                "       created_by, " +
                "       modify_ip, " +
                "       act_status) " +
                "values ( " +
                "       @t_id, " +
                "       '&CLIENT_ID&', " +
                "       '&ORG_ID&', " +
                "       '&WAREHOUSE_ID&'," +
                "       @pick_return_no, " +
                "       @pick_return_type, " +
                "       @job_no, " +
                "       @equipment_id, " +
                "       @dock_id, " +
                "       @dock_name, " +
                "       @item_id, " +
                "       @item_name, " +
                "       @container_id, " +
                "       @shift_id, " +
                "       @team_id, " +
                "       date_format(@pick_return_date, '%Y-%m-%d %H:%i:%s'), " +
                "       @route, " +
                "       @ref_doc_no, " +
                "       @src_addr_id, " +
                "       @src_addr_type, " +
                "       @src_addr_text, " +
                "       @dest_addr_id, " +
                "       @dest_addr_type, " +
                "       @dest_addr_text, " +
                "       @production_line, " +
                "       @is_rf, " +
                "       @reason, " +
                "       @pick_return_status, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "生产领料退料主表");
        }
        #endregion

        #region 插入生产领料退料明细表数据
        /// <summary>
        /// 插入生产领料退料明细表数据
        /// </summary>
        /// <param name="model"></param>
        public void InsertPickReturnD(PickReturnDModel model)
        {
            model.Tid = model.Tid ?? SnowflakeID.Guid().ToString();
            model.WarehouseID = model.WarehouseID ?? Context.WarehouseID;
            model.CreatedBy = model.CreatedBy ?? Context.UserInfo.UserName;
            model.ModifyIP = model.ModifyIP ?? Context.UserInfo.LoginIP;
            //model.LastModifiedBy = model.LastModifiedBy ?? Context.UserInfo.UserName;
            model.ActStatus = "1";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "t_id", model.Tid },
                { "pick_return_id", model.PickReturnId },
                { "container_dtid", model.ContainerDtid },
                { "item_id", model.ItemId },
                { "item_code", model.ItemCode },
                { "item_name", model.ItemName },
                { "lot_no", model.LotNo },
                { "suppliy", model.Suppliy },
                { "qty", model.Qty.ToString() },
                { "uom_qty_id", model.UomQtyId },
                { "memo", model.Memo },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into &DataSource&wms_d_pick_return_d ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       pick_return_id, " +
                "       container_dtid, " +
                "       item_id, " +
                "       item_code, " +
                "       item_name, " +
                "       lot_no, " +
                "       suppliy, " +
                "       qty, " +
                "       uom_qty_id, " +
                "       memo, " +
                "       create_date, " +
                "       created_by, " +
                "       modify_ip, " +
                "       act_status) " +
                "values ( " +
                "       @t_id, " +
                "       '&CLIENT_ID&', " +
                "       '&ORG_ID&', " +
                "       '&WAREHOUSE_ID&'," +
                "       @pick_return_id, " +
                "       @container_dtid, " +
                "       @item_id, " +
                "       @item_code, " +
                "       @item_name, " +
                "       @lot_no, " +
                "       @suppliy, " +
                "       @qty, " +
                "       @uom_qty_id, " +
                "       @memo, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "生产领料退料明细表");
        }
        #endregion

        #region 根据任务号获取依赖该任务的不可执行的任务
        /// <summary>
        /// 根据任务号获取依赖该任务的不可执行的任务
        /// </summary>
        /// <param name="taskNo">任务号</param>
        /// <returns></returns>
        public DataTable GetDependentTask(string taskNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "taskNo", taskNo }
            };
            string sql = $"select * from &DataSource&wms_b_task t where t.dependent_task_no=@taskNo and t.is_executable='0' and t.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "根据任务号获取依赖该任务的不可执行的任务", false);
            return dt;
        }
        #endregion

        #region 根据任务号获取未完成的依赖任务(完成包含部分完成和完成)
        /// <summary>
        /// 根据任务号获取未完成的依赖任务(完成包含部分完成和完成)
        /// </summary>
        /// <param name="taskNo">任务号</param>
        /// <returns></returns>
        public DataTable GetDependentTaskByTaskNo(string taskNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "taskNo", taskNo }
            };
            string sql = $"select * from &DataSource&wms_b_task t where t.task_no=@taskNo and t.task_status not in( '{TaskStatus.TaskStatus_Finished}','{TaskStatus.TaskStatus_PartialFinished}') and t.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "根据任务号获取未完成的依赖任务", false);
            return dt;
        }
        #endregion

        #region 修改任务可执行状态
        /// <summary>
        /// 修改任务可执行状态
        /// </summary>
        /// <param name="taskId">任务id</param>
        public void UpdateTaskIsExecutableStatus(string taskId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "taskId", taskId }
            };
            string sql = $"update &DataSource&wms_b_task l set l.is_executable='1',l.task_status='{TaskStatus.TaskStatus_Executing}',l.last_modified_by=@last_modified_by,l.modify_ip=@modify_ip,l.modify_date=sysdate where l.t_id=@taskId  and l.act_status='1'";
            DBHelper.RunSql(sql, param, "修改任务可执行状态");
        }
        #endregion

        #region 修改任务中容器主档信息
        /// <summary>
        /// 修改任务中容器主档信息
        /// </summary>
        /// <param name="task_no">任务号</param>
        public void UpdateTaskContainerInfo(string task_no, string container_id, string container_barcode)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "container_id", container_id },
                { "container_barcode", container_barcode },
                { "task_no", task_no }
            };
            string sql = $"update &DataSource&wms_b_task l set l.container_id=@container_id, l.container_barcode = @container_barcode,l.last_modified_by=@last_modified_by,l.modify_ip=@modify_ip,l.modify_date=sysdate where l.task_no=@task_no  and l.act_status='1'";
            DBHelper.RunSql(sql, param, "修改任务可执行状态");
        }
        #endregion

        #region 根据货位查询堆垛机
        /// <summary>
        /// 根据货位查询堆垛机
        /// </summary>
        /// <param name="locId">货位id</param>
        /// <returns></returns>
        public string GetDDJ(string locId)
        {
            DataRow equipRow = Equipment.GetStackByLocation(locId);
            //return "1288091969666551808";//堆垛机01,此处因为目前货位只有其中一部分，所以暂时写死，后续需要研究出更好的分配堆垛机的方法
            return "1288092061295316992";//堆垛机02
            //return equipRow == null ? "" : equipRow["t_id"].ToString();
        }
        #endregion

        #region 查询指定行是否已存满
        /// <summary>
        /// 查询指定行是否已存满
        /// </summary>
        /// <param name="line">行号</param>
        /// <returns>true:已存满；false：未满</returns>
        public bool IsLineFull(string line)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "line", line }
            };
            string sql = $"select count(l.t_id) as cnt from &DataSource&wms_m_location l where l.location_status not in('{LocationStatus.LocationStatus_Stored}') and l.location_row=@line and l.act_status = '1'";
            DataTable dt = DBHelper.Select(sql, param, "查询指定行是否已存满");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) == 0;
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

        #region 获取指定行中所有非空货位
        /// <summary>
        /// 获取指定行中所有非空货位
        /// </summary>
        /// <param name="line">行号</param>
        /// <param name="isDesc">排序规则。true:降序；false:升序</param>
        /// <returns></returns>
        public DataTable GetNotEmptyLocInfo(string line, bool isDesc)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "line", line }
            };
            string sql = $"select l.* from &DataSource&wms_m_location l where l.location_status in('{LocationStatus.LocationStatus_In}','{LocationStatus.LocationStatus_Stored}','{LocationStatus.LocationStatus_Out}','{LocationStatus.LocationStatus_Plan}') and l.location_row=@line and l.act_status = '1'";
            //string sql = $"select l.* from &DataSource&wms_m_location l where l.location_status in('{LocationStatus.LocationStatus_Stored}','{LocationStatus.LocationStatus_Out}','{LocationStatus.LocationStatus_Plan}') and l.location_row=@line and l.act_status = '1'";
            if (isDesc == true)
            {
                sql += "  order by l.location_no desc";
            }
            else
            {
                sql += "  order by l.location_no ";
            }
            DataTable dt = DBHelper.Select(sql, param, "获取指定行中所有非空货位", false);

            return dt;
        }
        #endregion

        #region 查询指定行是否全部为可出不可入
        /// <summary>
        /// 查询指定行是否全部为可出不可入
        /// </summary>
        /// <param name="line">行号</param>
        /// <returns>true:全是；false：不全是</returns>
        public bool IsLineContainsOut(string line)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "line", line }
            };
            string sql = $"select count(l.t_id) as cnt from &DataSource&wms_m_location l where l.location_flag <>'{LocationFlag.LocationFlag_OutOnly}' and l.location_row=@line and l.act_status = '1'";
            DataTable dt = DBHelper.Select(sql, param, "查询指定行是否全部为可出不可入");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) == 0;
        }
        #endregion

        /// <summary>
        /// 根据行号获取预分配货位
        /// </summary>
        /// <param name="line">行号</param>
        /// <param name="isDesc">是否为降序排序,true:降序；false:升序</param>
        /// <returns></returns>
        public DataTable GetLocByLine(string line, bool isDesc)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "line", line }
            };
            //string sql = $"select l.* from &DataSource&wms_m_location l inner join &DataSource&wms_b_container_h h on h.location_id=l.t_id where h.act_status='1' and l.act_status='1' and l.location_row=@line and l.location_status='{LocationStatus.LocationStatus_Plan}' order by l.location_no desc";
            string sql = $" select l.* from &DataSource&wms_m_location l where l.act_status = '1' and l.location_row = @line and l.location_status in( '{LocationStatus.LocationStatus_Plan}','{LocationStatus.LocationStatus_Out}','{LocationStatus.LocationStatus_In}') ";
            if (isDesc == true)
            {
                sql += " order by l.location_no desc";
            }
            else
            {
                sql += " order by l.location_no";
            }
            DataTable dt = DBHelper.Select(sql, param, "根据行号获取预分配货位");

            return dt;
        }

        /// <summary>
        /// 根据行号获取所有货位
        /// </summary>
        /// <param name="line">行号</param>
        /// <param name="isDesc">是否为降序排序,true:降序；false:升序</param>
        /// <returns></returns>
        public DataTable GetAllLocByLine(string line, bool isDesc)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "line", line }
            };
            //string sql = $"select l.* from &DataSource&wms_m_location l inner join &DataSource&wms_b_container_h h on h.location_id=l.t_id where h.act_status='1' and l.act_status='1' and l.location_row=@line and l.location_status='{LocationStatus.LocationStatus_Plan}' order by l.location_no desc";
            string sql = $" select l.* from &DataSource&wms_m_location l where l.act_status = '1' and l.location_row = @line  ";
            if (isDesc == true)
            {
                sql += " order by l.location_no desc";
            }
            else
            {
                sql += " order by l.location_no";
            }
            DataTable dt = DBHelper.Select(sql, param, "根据行号获取所有货位");

            return dt;
        }

        /// <summary>
        /// 根据行号获取已存储的货位
        /// </summary>
        /// <param name="line">行号</param>
        /// <param name="isDesc">是否为降序排序,true:降序；false:升序</param>
        /// <returns></returns>
        public DataTable GetStoredLocByLine(string line, bool isDesc)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "line", line }
            };
            //string sql = $"select l.* from &DataSource&wms_m_location l inner join &DataSource&wms_b_container_h h on h.location_id=l.t_id where h.act_status='1' and l.act_status='1' and l.location_row=@line and l.location_status='{LocationStatus.LocationStatus_Plan}' order by l.location_no desc";
            string sql = $" select l.* from &DataSource&wms_m_location l where l.act_status = '1' and l.location_row = @line and l.location_status in( '{LocationStatus.LocationStatus_Stored}') ";
            if (isDesc == true)
            {
                sql += " order by l.location_no desc";
            }
            else
            {
                sql += " order by l.location_no";
            }
            DataTable dt = DBHelper.Select(sql, param, "根据行号获取已存储的货位");

            return dt;
        }

        /// <summary>
        /// 根据行号获取是否存在预分配货位
        /// </summary>
        /// <param name="line">行号</param>
        /// <returns>true:存在；false:不存在</returns>
        public bool GetPlanLocNumByLine(string line)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "line", line }
            };
            string sql = $" select count(l.t_id) as cnt from &DataSource&wms_m_location l where l.act_status = '1' and l.location_row = @line and l.location_status in( '{LocationStatus.LocationStatus_Plan}','{LocationStatus.LocationStatus_Out}','{LocationStatus.LocationStatus_In}') ";
            DataTable dt = DBHelper.Select(sql, param, "根据行号获取是否存在预分配货位");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) > 0;
        }

        #region 修改货位标识
        /// <summary>
        /// 修改货位标识
        /// </summary>
        /// <param name="row">行号</param>
        /// <param name="locationflag">货位标识</param>
        public void UpdatelocationflagByRow(string row, string locationflag)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "location_flag", locationflag },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "location_row", row }
            };
            string sql = "update &DataSource&wms_m_location l set l.location_flag=@location_flag,l.last_modified_by=@last_modified_by,l.modify_ip=@modify_ip,l.modify_date=sysdate where l.location_row=@location_row  and l.act_status='1'";
            DBHelper.RunSql(sql, param, "修改货位状态", OperatedNumType.OneOrMore);
        }
        #endregion

        #region 机台上是否有物料
        /// <summary>
        /// 机台上是否有物料
        /// </summary>
        /// <param name="machine_id">机台id</param>
        /// <returns>true:有；false：无</returns>
        public bool IsHaveContainerInfo(string machine_id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "machine_id", machine_id }
            };
            string sql = $"select count(md.t_id) as cnt from &DataSource&wms_m_machine_dock md where md.machine_id=@machine_id and md.container_id is not null and md.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "机台上是否有物料");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) > 0;
        }
        #endregion

        #region 检查机台与站台是否有关联(关联指的是站台是否挂在机台下面)
        /// <summary>
        /// 机台上是否有物料
        /// </summary>
        /// <param name="machine_id">机台id</param>
        /// <param name="dock_id">站台id</param>
        /// <returns>true:有；false：无</returns>
        public bool MachineHaveDockInfo(string machine_id, string dock_id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "machine_id", machine_id },
                { "dock_id", dock_id }
            };
            string sql = $"select count(md.t_id) as cnt from &DataSource&wms_m_machine_dock md where md.machine_id=@machine_id and md.dock_id=@dock_id and md.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "检查机台与站台是否有关联");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) > 0;
        }
        #endregion

        /// <summary>
        /// 更新货位对应的容器是否允许呼叫
        /// </summary>
        /// <param name="containerid">容器ID</param>
        /// <param name="isAllowCalled">0：不允许，1：允许</param>
        /// <returns></returns>
        public bool UpdateContainerAllowCalled(string containerid, string isAllowCalled)
        {
            string sql = @"update &DataSource&wms_b_container_h h set h.ALLOW_CALLED = @isAllowCalled,
                                                 h.take_in_by = @take_in_by,
                                                 h.last_modified_by = @last_modified_by,
                                                 h.modify_ip = @modify_ip,
                                                 h.modify_date = sysdate  
                where h.t_id=@t_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", containerid);
            lstParam.Add("isAllowCalled", isAllowCalled);
            lstParam.Add("take_in_by", Context.UserInfo.UserName);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            return DBHelper.RunSql(sql, lstParam, "更新货位对应容器的是否允许呼叫") > 0;
        }

        public string GetEquipmentByLoc(string row, ref string loc_id)
        {
            bool isSc1Error = false;
            string state_OK = EquipmentStatus.EquipmentStatus_OK.ToString();

            if (string.Equals( row.Substring(1,2),"04")|| string.Equals(row.Substring(1, 2), "09")|| string.Equals(row.Substring(1, 2), "16"))
            {
                #region 密集2区且堆垛机状态正常，直接返回2号堆垛机
                string _e2 = WmsConfig.GetOrDefault("Task:SC02", "1288092061295316992");  //2号堆垛机id
                string _sql = $"select * from &DataSource&wms_m_equipment t where t_id = '" + _e2 + "' ";
                DataTable _dt = DBHelper.Select(_sql, null, $"根据ID{_e2}获取堆垛机");
                if (_dt.Rows[0]["equipment_status"].ToString() == state_OK)
                {
                    string _sql_Loc = "select max(t.location_no) from &DataSource&wms_m_location t where location_row = " + row +
                $" and location_status = '{LocationStatus.LocationStatus_Stored}'";

                    _sql = "select * from &DataSource&wms_m_location where location_row = " + row +
                        $" and location_status = '{LocationStatus.LocationStatus_In}' and location_no > " + $"({_sql_Loc})";
                    _dt = DBHelper.Select(_sql, null, $"根据行{row}获取最大列待入库货位", false);
                    if (_dt.Rows.Count > 0)
                    {
                        throw new Exception($"行{row}存在入库任务，请在此任务完成后再试！");
                    }

                    _sql = "select t_id from &DataSource&wms_m_location where location_no =" + $"({_sql_Loc})";
                    _dt = DBHelper.Select(_sql, null, $"根据行{row}获取最大列已存储货位");
                    loc_id = _dt.Rows[0]["t_id"].ToString();  //返回新货位

                    return _e2;  //设备正常直接返回设备id
                }
                else
                {
                    throw new Exception("需执行出库的2号堆垛机故障，请恢复后再试！");
                }
                #endregion
            }
            else if (string.Equals(row.Substring(1, 2), "28") || string.Equals(row.Substring(1, 2), "29") || string.Equals(row.Substring(1, 2), "30"))
            {
                #region 密集1区且堆垛机状态正常，直接返回1号堆垛机
                string e1 = WmsConfig.GetOrDefault("Task:SC01", "1288091969666551808");  //1号堆垛机id
                string _sql = $"select * from &DataSource&wms_m_equipment t where t_id = '" + e1 + "' ";
                DataTable _dt = DBHelper.Select(_sql, null, $"根据ID{e1}获取堆垛机");
                if (_dt.Rows[0]["equipment_status"].ToString() == state_OK)
                {
                    string _sql_Loc = $"select min(t.location_no) from &DataSource&wms_m_location t where location_row = " + row +
                $" and location_status = '{LocationStatus.LocationStatus_Stored}'";

                    _sql = "select * from &DataSource&wms_m_location where location_row = " + row +
                        $" and location_status = '{LocationStatus.LocationStatus_In}' and location_no < " + $"({_sql_Loc})";
                    _dt = DBHelper.Select(_sql, null, $"根据行{row}获取最小列待入库货位", false);
                    if (_dt.Rows.Count > 0)
                    {
                        throw new Exception($"行{row}存在入库任务，请在此任务完成后再试！");
                    }

                    _sql = "select t_id from &DataSource&wms_m_location where location_no =" + $"({_sql_Loc})";
                    _dt = DBHelper.Select(_sql, null, $"根据行{row}获取最小列已存储货位");
                    loc_id = _dt.Rows[0]["t_id"].ToString();  //返回新货位
                    return e1;  //设备正常直接返回设备id
                }
                else
                {
                    throw new Exception("需执行出库的1号堆垛机故障，请恢复后再试！");
                }
                #endregion
            }

            string sql = $"select * from &DataSource&wms_si_zonesql_h t where t.zonesql_code = @code and act_status = '1'";
            string code = WmsConfig.GetOrDefault("Task:MJZoneCode1", "MJZone1");  //策略管理 -> 区域策略定义
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "code", code }
            };
            DataTable dt = DBHelper.Select(sql, param, $"根据区域策略定义编码{code}获取值");
            string zoneSql = dt.Rows[0]["ZONE_SQL"].ToString();
            zoneSql = zoneSql.Replace("&lt;", "<").Replace("&gt;", ">");
            sql = $"select distinct t.location_row from &DataSource&wms_m_location t where act_status = '1' and " + zoneSql;
            dt = DBHelper.Select(sql, null, $"根据区域策略定义编码{code}获取货位");
            DataRow[] rows = dt.Select("location_row = '" + row + "'");
            if (rows.Length > 0)
            {
                #region 密集1区且堆垛机状态正常，直接返回1号堆垛机
                string e1 = WmsConfig.GetOrDefault("Task:SC01", "1288091969666551808");  //1号堆垛机id
                sql = $"select * from &DataSource&wms_m_equipment t where t_id = '" + e1 + "' ";
                dt = DBHelper.Select(sql, null, $"根据ID{e1}获取堆垛机");
                if (dt.Rows[0]["equipment_status"].ToString() == state_OK)
                {
                    string _tempEquipment = GetNotFullLocByRow(row, ref loc_id);
                    if (!string.IsNullOrEmpty(_tempEquipment))
                    {
                        return _tempEquipment;
                    }
                    return e1;  //设备正常直接返回设备id
                }
                isSc1Error = true;
                #endregion
            }

            #region 密集2区或者1号堆垛机故障，重新分配列并返回2号堆垛机
            string e2 = WmsConfig.GetOrDefault("Task:SC02", "1288092061295316992");  //2号堆垛机id
            sql = $"select * from &DataSource&wms_m_equipment t where t_id = '" + e2 + "' ";
            dt = DBHelper.Select(sql, null, $"根据ID{e2}获取堆垛机");
            if (dt.Rows[0]["equipment_status"].ToString() != state_OK)
            {
                if (isSc1Error == true)
                    throw new Exception("所有堆垛机均故障，请恢复后再试！");
                else
                    throw new Exception("需执行出库的2号堆垛机故障，请恢复后再试！");
            }

            string sql_Loc = "select max(t.location_no) from &DataSource&wms_m_location t where location_row = " + row +
                " and location_status = 'LocationStatus_Stored'";

            sql = "select * from &DataSource&wms_m_location where location_row = " + row +
                " and location_status = 'LocationStatus_In' and location_no > " + $"({sql_Loc})";
            dt = DBHelper.Select(sql, null, $"根据行{row}获取最大列待入库货位", false);
            if (dt.Rows.Count > 0)
            {
                throw new Exception($"行{row}存在入库任务，请在此任务完成或1号堆垛机故障修复后再试！");
            }

            sql = "select t_id from &DataSource&wms_m_location where location_no =" + $"({sql_Loc})";
            dt = DBHelper.Select(sql, null, $"根据行{row}获取最大列已存储货位");
            loc_id = dt.Rows[0]["t_id"].ToString();  //返回新货位
            return e2;  //2号堆垛机id
            #endregion
        }

        #region 根据行号获取为散盘（第一盘或者最后一盘），如果首尾均不是则按照货位分配出来的货位进行出库
        /// <summary>
        /// 根据行号获取为散盘（第一盘或者最后一盘），如果首尾均不是则按照货位分配出来的货位进行出库
        /// </summary>
        /// <param name="line">行号</param>
        /// <param name="loc_id">货位id</param>
        /// <returns>堆垛机Id</returns>
        public string GetNotFullLocByRow(string line, ref string loc_id)
        {
            string sql = $"select l.*,(select h.is_full from &DataSource&wms_b_container_h h where h.location_id=l.t_id and h.act_status='1' and h.container_barcode<>'99999999' and h.container_barcode<>'00000000'  and rownum=1) as is_full from &DataSource&wms_m_location l where l.t_id='{loc_id}'";
            DataTable _tempLocInfo = DBHelper.Select(sql, null, $"根据货位id{line}获取货位和容器信息");
            string _tempIsFull = _tempLocInfo.Rows[0]["is_full"].ToString();
            if (string.Equals(_tempIsFull, "0"))//散盘
            {
                return "";
            }
            string state_OK = EquipmentStatus.EquipmentStatus_OK.ToString();
            string e2 = WmsConfig.GetOrDefault("Task:SC02", "1288092061295316992");  //2号堆垛机id
            sql = $"select * from &DataSource&wms_m_equipment t where t_id = '" + e2 + "' ";
            DataTable dt = DBHelper.Select(sql, null, $"根据ID{e2}获取堆垛机");
            if (dt.Rows[0]["equipment_status"].ToString() != state_OK)//2号堆垛机没有处于正常状态
            {
                return "";
            }

            string sql_Loc = "select max(t.location_no) from &DataSource&wms_m_location t where location_row = " + line +
                $" and location_status = '{LocationStatus.LocationStatus_Stored.ToString()}'";

            sql = "select * from &DataSource&wms_m_location where location_row = " + line +
                $" and location_status = '{LocationStatus.LocationStatus_In.ToString()}' and location_no > " + $"({sql_Loc})";
            dt = DBHelper.Select(sql, null, $"根据行{line}获取最大列待入库货位", false);
            if (dt.Rows.Count > 0)//有待入任务
            {
                return "";
            }

            sql = $"select l.t_id,l.location_status,(select h.is_full from &DataSource&wms_b_container_h h where h.location_id=l.t_id and h.act_status='1' and h.container_barcode<>'99999999' and h.container_barcode<>'00000000'  and rownum=1) as is_full from &DataSource&wms_m_location l where location_no=({sql_Loc})";
            dt = DBHelper.Select(sql, null, $"根据行{line}获取最大列已存储货位");
            string isFull = dt.Rows[0]["is_full"].ToString();
            string location_status = dt.Rows[0]["location_status"].ToString();
            if (!string.Equals(isFull, "0") || !string.Equals(location_status, LocationStatus.LocationStatus_Stored.ToString()))
            {
                return "";
            }
            loc_id = dt.Rows[0]["t_id"].ToString();  //返回新货位
            return e2;  //2号堆垛机id
        }
        #endregion

        /// <summary>
        /// 判断待出货位前面是否有托盘
        /// </summary>
        /// <param name="locNo">待出货位</param>
        /// <param name="isDesc">待出货位前面的货位编码与待出货位编码之间的关系：true：大于;false:小于</param>
        /// <returns>true表示存在；false表示不存在</returns>
        public bool JustHaveContainerBeforeStoredOut(string locNo,string locLine,bool isDesc)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "locNo", locNo },
                { "locLine", locLine }
            };
            string sql = $"select count(l.t_id) as cnt from &DataSource&wms_m_location l where l.location_row = @locLine and l.location_status not in ('{LocationStatus.LocationStatus_Empty}','{LocationStatus.LocationStatus_Out}') and l.location_no{(isDesc==true?">":"<")}'{locNo}'";
            DataTable dt = DBHelper.Select(sql, param, "判断待出货位前面是否有托盘");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) > 0;
        }

        #endregion

        #region 获取站台BOM信息
        /// <summary>
        /// 获取站台BOM信息
        /// </summary>
        /// <param name="_line">站台id</param>
        /// <returns></returns>
        public DataTable GetTwoFloorBOMInfo(string _line, string zoneId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                //{ "productPlanOutId", productPlanOutId },
                { "location_row", _line },
                 { "zoneId", zoneId }
            };
            //string sql = $"select distinct h.t_id,p.item_id,h.create_date from &DataSource&wms_m_bom_h h inner join &DataSource&wms_d_prod_plan_out p on p.plan_status='{PlanOutStatus.PlanOutStatus_Executing}' and h.item_id=p.item_id and h.bom_status='{BOMStatus.BOMStatus_Using}' and h.can_be_called='1' inner join &DataSource&wms_m_machine_dock d on d.machine_id=p.equipment_id and h.prod_section=d.prod_section where d.dock_id=@dockId and d.act_status='1' and h.act_status='1' and p.act_status='1' order by h.create_date";
            string sql = $"select distinct j.bom_id from &DataSource&wms_m_location l  inner join &DataSource&wms_b_container_h j on l.t_id=j.location_id where l.act_status='1' and j.act_status='1' and l.zone_id=@zoneId and l.location_row=@location_row ";
            DataTable dt = DBHelper.Select(sql, param, "获取获取预分配货位上的托盘BOM信息", false);
            return dt;
        }
        #endregion
    }
}
