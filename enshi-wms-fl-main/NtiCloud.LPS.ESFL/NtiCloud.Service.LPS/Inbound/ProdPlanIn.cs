using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.LPS.Inbound
{
    /// <summary>
    /// 生产入库计划
    /// </summary>
    public class ProdPlanIn
    {
        /// <summary>
        /// 根据来源设备ID查找当前执行中的生产计划
        /// </summary>
        /// <param name="refEquipId"></param>
        /// <returns></returns>
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        public  Model.ProdPlanInModel? GetProdPlanInByRefEquimentId(string refEquipId, string planStatus) {
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
            string sql = @" select t_id, client_id, org_id, warehouse_id, equipment_id, plan_no, ref_doc_no, item_id, item_code, item_name, lot_no, wms_lot_no, shift_id, team, exist_barcode, src_equipment_id, owner_id, plan_qty, finished_qty, uom_qty_id, plan_stat_qty, finished_stat_qty, uom_stat_id, allow_end,is_qty_controlled, is_auto_created, plan_status, start_time, end_time, memo, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                             from &DataSource&wms_d_prod_plan_in
                             where src_equipment_id  = @src_equipment_id and plan_status = @plan_status";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("src_equipment_id", refEquipId);
            lstParam.Add("plan_status", planStatus);
            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            if (dt.Rows.Count == 0) {
                return null;                
            }

            if (dt.Rows.Count > 1) {
                throw new Exception("根据设备ID找到多笔执行中的入库计划");
            }          
            return DBHelper.ConvertTableToObject<Model.ProdPlanInModel>(dt)[0];
        }
        /// <summary>
        /// 根据设备ID查找当前执行中的生产计划
        /// </summary>
        /// <param name="equipId"></param>
        /// <returns></returns>
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        public Model.ProdPlanInModel? GetProdPlanInByEquimentId(string equipId,string planStatus)
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        {
            string sql = @" select t_id, client_id, org_id, warehouse_id, equipment_id, plan_no, ref_doc_no, item_id, item_code, item_name, lot_no, wms_lot_no, shift_id, team, exist_barcode, src_equipment_id, owner_id, plan_qty, finished_qty, uom_qty_id, plan_stat_qty, finished_stat_qty, uom_stat_id, allow_end, is_auto_created,is_qty_controlled, plan_status, start_time, end_time, memo, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                             from &DataSource&wms_d_prod_plan_in
                             where equipment_id  = @equipment_id and plan_status = @plan_status";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("equipment_id", equipId);
            lstParam.Add("plan_status", planStatus);
            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            if (dt.Rows.Count == 0)
            {
                return null;
            }

            if (dt.Rows.Count > 1)
            {
                throw new Exception("根据设备ID找到多笔执行中的入库计划");
            }
            return DBHelper.ConvertTableToObject<Model.ProdPlanInModel>(dt)[0];
        }
        /// <summary>
        /// 获取当前设置正在执行的生产计划
        /// </summary>
        /// <returns></returns>
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        public Model.ProdPlanInModel? GettProdPlanInDoing(string equipmentid) { 
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
            Model.ProdPlanInModel? planin = GetProdPlanInByRefEquimentId(equipmentid, "PlanStatus_Executing");
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
            if (planin == null) {
                planin = GetProdPlanInByEquimentId(equipmentid, "PlanStatus_Executing");
            }
            return planin;
        }

        /// <summary>
        /// 更新生产计划已完成量
        /// </summary>
        /// <param name="planid">生产计划ID</param>
        /// <param name="thisQty">本次完成量</param>
        /// <returns></returns>
        public bool AddFinishQty(string planid, decimal thisQty) {
            string sql = @" update &DataSource&wms_d_prod_plan_in i 
                             set i.finished_qty=nvl(i.finished_qty,0) + @thiQty,
                                 i.last_modified_by=@last_modified_by,
                                 i.modify_ip=@modify_ip,
                                 i.modify_date=sysdate
                             where i.t_id=@planid";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("thiQty", thisQty.ToString());
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("planid", planid);
            return DBHelper.RunSql(sql, lstParam, "更新生产计划已完成量") > 0;
        }
    }
}
