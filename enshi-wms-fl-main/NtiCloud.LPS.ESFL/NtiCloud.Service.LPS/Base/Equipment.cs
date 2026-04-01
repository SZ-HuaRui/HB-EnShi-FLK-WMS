using NtiCloud.Service.LPS.Model;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    public class Equipment : IEquipment
    {
        public DataRow GetEquipment(string equipNoOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", equipNoOrID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_m_equipment  
 where /*client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and*/ warehouse_id = @warehouse 
   and t_id = @t_id or equipment_code =@t_id
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询设备表");

            return dt.Rows[0];
        }

        public DataRow[] GetEquipmentByStatus(string equipType, string equipStatus)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "equip_type", equipType },
                { "status", equipStatus },
            };
            string sql;
            sql = @"
select * from &DataSource&wms_m_equipment e
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and e.equipment_type = @equip_type 
   and e.equipment_status = @status 
";
            DataTable dt = DBHelper.Select(sql, param, $"按类型{equipType}状态{equipStatus}查询设备表", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow GetStackByLocation(string locationCodeOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", locationCodeOrID }
            };
            string sql;
            sql = @"
select e.* from &DataSource&wms_m_equipment e
  left join &DataSource&wms_m_zone z on e.zone_id = z.t_id
  left join &DataSource&wms_m_location l on l.zone_id = z.t_id
 where e.client_id='&CLIENT_ID&' and e.org_id='&ORG_ID&' 
   and e.warehouse_id = @warehouse 
   and l.location_code = @t_id or l.t_id = @t_id
   and e.equipment_type = 'EquipmentType_AS' and e.act_status='1' 
";
            return DBHelper.Select(sql, param, "根据货位查询堆垛机", false).AsEnumerable().FirstOrDefault();
            /*
            DataTable dt = DBHelper.Select(sql, param, "查询设备表", false);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dt.AsEnumerable().FirstOrDefault();
            }
            */
        }

        public DataRow[] GetPickingStation()
        {
            return null;
        }

        /// <summary>
        /// 根据设备编号获取设备信息
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="equipmentCode"></param>
        /// <returns></returns>
        public EquipmentModel GetEquipment(string warehouseId, string equipmentCode) {
            string sql = @" select t_id, client_id, org_id, warehouse_id, zone_id, equipment_code, equipment_name, equipment_status, equipment_type, equipment_usage, position, is_enabled, memo, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                             from &DataSource&wms_m_equipment
                             where warehouse_id=@warehouse_id and equipment_code=@equipment_code";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("warehouse_id", warehouseId);
            lstParam.Add("equipment_code", equipmentCode);
            DataTable dt = DBHelper.Select(sql, lstParam, "根据设备编号获取设备信息", false);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else {
                return DBHelper.ConvertTableToObject<EquipmentModel>(dt)[0];
            }
        }

        public bool IsOutboundStation(Address dest)
        {
            if (dest.Type != AddressType.AddressType_Station.ToString())
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(dest.ID))
            {
                return true;
            }
            DataRow station = GetEquipment(dest.ID);
            if (station["equipment_type"].ToString() == "EquipmentType_Station"
                && station["equipment_usage"].ToString()
                    .Contains(EquipmentUsage.EquipmentUsage_Outbound.ToString()))
            {
                return true;
            }
            return false;
        }
        public bool SynEquipmentStatus(string equipmentcode,string status){ 
            string sql = @"update &DataSource&wms_m_equipment e set e.equipment_status=@equipment_status,
                                                     e.modify_date=sysdate,
                                                     e.last_modified_by=@last_modified_by
                            where e.warehouse_id = @warehouse_id and e.equipment_code = @equipment_code";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("equipment_status", status);
            lstParam.Add("last_modified_by", Context.UserInfo==null?"":Context.UserInfo.UserName);
            lstParam.Add("warehouse_id", Context.WarehouseID);
            lstParam.Add("equipment_code", equipmentcode);
            DBHelper.RunSql(sql, lstParam, "更新设备状态");
            return true;

        }
    }
}
