using NtiCloud.Service.LPS.Model;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.LPS.Base
{
   public class Warehouse
    {
        /// <summary>
        /// 根据仓库代码查询仓库信息
        /// </summary>
        /// <param name="warehousecode"></param>
        /// <returns></returns>
        public WarehouseModel GetWarehouseId(string warehousecode) {
            string sql = @"select t_id, client_id, org_id, warehouse_name, warehouse_code, warehouse_desc, warehouse_property, address, postcode, warehouse_type, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                        from &DataSource&wms_m_warehouse
                        where warehouse_code=@warehouse_code ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("warehouse_code", warehousecode);
            DataTable dt = DBHelper.Select(sql, lstParam, "根据仓库代码查询仓库信息",false);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else {
               return DBHelper.ConvertTableToObject<WarehouseModel>(dt)[0];
            } 
        }
    }
}
