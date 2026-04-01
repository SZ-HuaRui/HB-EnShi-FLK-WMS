using NtiCloud.Interface.Core;
using NtiCloud.Kernel;
using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Inventory;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Services.WMS.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static NtiCloud.Service.WMS.Common.DBHelper;
//using kernelDataBase = NtiCloud.Kernel.KernelDataBase;

namespace NtiCloud.Service.WMS.Base
{
    public class Location : WmsObject, ILocation
    {
        public DataRow Get(string locationNoOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "location", locationNoOrID },
            };
            string sql = @"
select * from &DataSource&wms_m_location 
where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
and warehouse_id = @warehouse 
and (t_id = @location or location_no = @location) 
";

            DataTable dt = DBHelper.Select(sql, param, "查询货位");

            return dt.Rows[0];
        }

        public DataRow GetByContainer(string containerBarcodeOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container", containerBarcodeOrID },
            };
            string sql = @"
select * from &DataSource&wms_m_location l 
  left join &DataSource&wms_b_container_h c on l.t_id = c.location_id 
 where l.client_id='&CLIENT_ID&' and l.org_id='&ORG_ID&' 
   and l.warehouse_id = @warehouse 
   and (container_barcode = @container or c.t_id = @container) 
";
            DataTable dt = DBHelper.Select(sql, param, "通过容器查询货位");

            return dt.Rows[0];
        }


        public DataRow GetEmptyLocation()
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
            };
            string selSql = @"
select * from &DataSource&wms_m_location l 
  left join &DataSource&wms_b_container_h c on l.t_id = c.location_id
  left join &DataSource&wms_b_container_d cd on c.t_id = cd.container_id
 where 1=1 
   and l.client_id='&CLIENT_ID&' and l.org_id='&ORG_ID&'
   and l.warehouse_id = @warehouse 
   and l.usage = 'LocationUse_Stock'
   and l.location_status = 'LocationStatus_Empty' 
   and cd.t_id is null
   and rownum < 2
";
            DataTable dtLocation = DBHelper.Select(selSql, param, "查找空货位");

            return dtLocation.Rows[0];
        }

        public DataRow[] GetLocationGroup()
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
            };
            string selSql = @"
select * from &DataSource&wms_m_locationgp l 
 where l.client_id='&CLIENT_ID&' and l.org_id='&ORG_ID&'
   and l.warehouse_id = @warehouse 
   and act_status = '1'
";
            DataTable dtLocation = DBHelper.Select(selSql, param, "查找货位组");

            return dtLocation.AsEnumerable().ToArray();
        }

        public bool IsCompactLocation(DataRow row)
        {
            if (row[l.LocationType].ToString() == LocationType.LocationType_Compact.ToString())
            {
                return true;
            }
            return false;
        }

        public bool IsRackLocation(DataRow row)
        {
            if (row[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
            {
                return true;
            }
            return false;
        }

        public bool IsLocationEmpty(string locationID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "location_id", locationID },
            };
            string sql;
            sql = "select t_id from &DataSource&dwms_container_h  " +
                          " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                          "    and whse_id = @warehouse " +
                          "    and location_id = @location_id limit 0, 1 ";

            DataTable dt = DBHelper.Select(sql, param, "查询容器头表", false);

            if (dt.Rows.Count > 0)
            {
                return false;
            }
            return true;
        }

        public void UpdateStatus(string locationID, string status, string oldStatus = null)
        {
            if (string.IsNullOrWhiteSpace(oldStatus))
            {
                oldStatus = " ";
            }

            // 修改货位状态
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "new_status", status },
                { "old_status", oldStatus },
                { "location_id", locationID }
            };

            string updSql = @"
update &DataSource&wms_m_location  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       location_status = @new_status
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @location_id
   and (location_status = @old_status
            or @old_status = ' ' or @old_status = '')
";

            DBHelper.RunSql(updSql, param, "修改货位状态");

        }

        public DataRow GetConveyorLocation()
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
            };
            string sql = @"
select * from &DataSource&wms_m_location 
where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
and warehouse_id = @warehouse 
and location_type = 'LocationType_Conveyor'
";
            DataTable dt = DBHelper.Select(sql, param, "查询货位", false);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return dt.Rows[0];
        }
        /// <summary>
        /// 获取入库暂存区货位
        /// </summary>
        /// <returns></returns>
        public DataRow GetInboundBufferLocation()
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID},
                { "usage", LocationUse.LocationUse_InboundBuffer.ToString()},
            };
            string sql = @"select * from &DataSource&wms_m_location 
                            where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
                                and warehouse_id = @warehouse 
                                and usage = @usage";
            
            DataTable dt = DBHelper.Select(sql, param, "查询货位", false);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return dt.Rows[0];
        }

        public DataRow GetPickingLocation()
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
            };
            string sql = @"
select * from &DataSource&wms_m_location 
where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
and warehouse_id = @warehouse 
and usage = 'LocationUse_OutboundBuffer'
";
            DataTable dt = DBHelper.Select(sql, param, "查询货位", false);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return dt.Rows[0];
        }

        public ILocation ModifyLocationStockState(string locationID,
            string stockState, string previousState = null, IDbTransaction tran = null)
        {
            if (string.IsNullOrWhiteSpace(previousState))
            {
                previousState = " ";
            }
            if (tran == null)
            {
                tran = Context.UserInfo.UserDbConnW.Transaction;
            }

            // 修改货位状态
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", locationID },
                { "state", stockState },
                { "old_state", previousState },
            };

            string updSql = @"
update wms_m_location set stock_status = @state 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse and t_id = @t_id 
   and (@old_state = ' ' or stock_status = @old_state) ";
            DBHelper.RunSql(updSql, param, "修改货位表状态", tran: tran);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="failureList"></param>
        /// <param name="dict">替换字典，key为旧值</param>
        /// <returns></returns>
        public ILocation RestoreLocationStockState(List<string> idList, out List<string> failureList, 
            Dictionary<string, object> dict)
        {
            failureList = new List<string>();

            //IDbConnection conn = Context.GetConnection();
            //conn.Open();
            //IDbTransaction tran = Context.GetTransaction();
            UserLoginInfo user = Context.GetUserInfo();
            //using KernelDataBase kernelDataBase = new KernelDataBase();
            //kernelDataBase.BeginTransaction(Context.UserInfo);
            DBHelper.BeginTransaction();
            //IDbTransaction tran = user.UserDbConnW.Transaction;

            foreach (string locationID in idList)
            {
                try
                {
                    //ModifyLocationStockState(locationID,
                    //    LocationStatus.LocationStatus_Partial.ToString(),
                    //    previousState, tran);
                    DataRow locRow = Get(locationID);
                    string newValue = dict[locRow[l.LocationStatus].ToString()].ToString();
                    if (newValue == null)
                    {
                        throw new WmsException(500, $"找不到可恢复的状态{locRow[l.LocationStatus]}");
                    }
                    UpdateStatus(locationID, newValue, locRow[l.LocationStatus].ToString());
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex.Message);
                    LogHelper.LogError(ex.StackTrace);
                    failureList.Add(locationID);
                }
            }
            DBHelper.EndTransaction(true);
            return this;
        }

        public ILocation ResetLocationStatus(DataRow locationRow)
        {
            string locID = locationRow[l.ID].ToString();
            DataRow[] containerDetails = Container.GetDetailsByLocation(locID);
            if (locationRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
            {
                _ = containerDetails.Length > 0
                    ? UpdateLocationStatus(locID, LocationStatus.LocationStatus_Full)
                    : UpdateLocationStatus(locID, LocationStatus.LocationStatus_Empty);
            }
            else
            {
                decimal qty = containerDetails.Sum(r => decimal.Parse("0" + r[cd.Qty]));
                decimal capacity = decimal.Parse("0" + locationRow[l.QtyLimit]);
                _ = containerDetails.Length <= 0
                    ? UpdateLocationStatus(locID, LocationStatus.LocationStatus_Empty)
                    : qty >= capacity 
                        ? UpdateLocationStatus(locID, LocationStatus.LocationStatus_Full)
                        : UpdateLocationStatus(locID, LocationStatus.LocationStatus_Partial);
            }
            return this;
        }

        public ILocation ResetLocationStatus(string locationNoOrID)
        {
            DataRow locRow = Get(locationNoOrID);
            ResetLocationStatus(locRow);
            return this;
        }

        public ILocation SetContainer(string locationID, string containerID)
        {
            throw new NotImplementedException();
        }

        public ILocation SetItem(string locationID, string itemID)
        {
            throw new NotImplementedException();
        }

        public DataRow GetByAddress(Address relay)
        {
            if (relay.Type == AddressType.AddressType_Picking.ToString())
            {
                return GetPickingLocation();
            }
            return null;
        }

        /// <summary>
        /// 根据货位ID获取货位信息
        /// <br/>
        /// 如果没有找到，则返回null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LocationModel GetLocationById(string id)
        {
            string sql = @" select t_id, client_id, org_id, warehouse_id, location_no, location_code, location_name, zone_id, zone_id2, zone_id3, zone_id4, location_row, location_column, location_layer, location_depth, floor_no, room_no, unit_no, tags, location_type, location_status, location_flag, process_status, usage, owner_id, container_catalog_id, is_mix_item, is_mix_lot, is_mix_grade, length, width, height, uom_id, seq_no, volume_limit, weight_limit, qty_limit, container_limit, uom_qtyid, weightid, description, xcoord, ycoord, zcoord, xdistance, ydistance, inbound_date, packing_type, flag_reason, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                         from &DataSource&wms_m_location where t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", id);
            DataTable dt = DBHelper.Select(sql, lstParam);
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<LocationModel>(dt)[0];
            }
            else {
                return null;
            }

        }

        /// <summary>
        /// 根据行号获取预分配货位信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LocationModel GetPlanLocationByLine(string line)
        {
            string sql = $" select * from &DataSource&wms_m_location l where l.location_row=@line and l.location_status='{LocationStatus.LocationStatus_Plan}'  order by l.location_no desc";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("line", line);
            DataTable dt = DBHelper.Select(sql, lstParam);
            return DBHelper.ConvertTableToObject<LocationModel>(dt)[0];

        }

        public LocationModel GetLocationByCode(string warehouse_id, string locationcode)
        {
            string sql = @" select t_id, client_id, org_id, warehouse_id, location_no, location_code, location_name, zone_id, zone_id2, zone_id3, zone_id4, location_row, location_column, location_layer, location_depth, floor_no, room_no, unit_no, tags, location_type, location_status, location_flag, process_status, usage, owner_id, container_catalog_id, is_mix_item, is_mix_lot, is_mix_grade, length, width, height, uom_id, seq_no, volume_limit, weight_limit, qty_limit, container_limit, uom_qtyid, weightid, description, xcoord, ycoord, zcoord, xdistance, ydistance, inbound_date, packing_type, flag_reason, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                         from &DataSource&wms_m_location where location_no = @location_no and warehouse_id=@warehouse_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("location_no", locationcode);
            lstParam.Add("warehouse_id", warehouse_id);
            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return DBHelper.ConvertTableToObject<LocationModel>(dt)[0];
            }
        }

        /// <summary>
        /// 更新货位状态
        /// </summary>
        /// <param name="locationid">货位ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public bool UpdateLocationStatus(string locationid, LocationStatus status)
        {
            string sql = @"update  &DataSource&wms_m_location l set l.location_status = @location_status,
                                                         l.last_modified_by = @last_modified_by,
                                                         l.modify_ip = @modify_ip,
                                                         l.modify_date = sysdate
                            where l.t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("location_status", status.ToString());
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("t_id", locationid);

            return DBHelper.RunSql(sql, lstParam, "更新货位状态") > 0;
        }

        /// <summary>
        /// 更新货位标识
        /// </summary>
        /// <param name="locationid">货位ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public bool UpdateLocationFlag(string line, string newStatus,string oldStatus)
        {
            string sql = @"update  &DataSource&wms_m_location l set l.location_flag = @newStatus,
                                                         l.last_modified_by = @last_modified_by,
                                                         l.modify_ip = @modify_ip,
                                                         l.modify_date = sysdate
                            where l.location_row = @line and l.act_status = '1' and l.location_flag =@oldStatus";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("newStatus", newStatus);
            lstParam.Add("oldStatus", oldStatus);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("line", line);

            return DBHelper.RunSql(sql, lstParam, "更新货位标识", OperatedNumType.OneOrMore) > 0;
        }

        /// <summary>
        /// 获取输送线货位ID，配有配置时反恐空值
        /// </summary>
        /// <param name="wareouseid"></param>
        /// <returns></returns>
        public string getTransferLineLocationId(string wareouseid) {
            string sql = @"select   l.t_id from &DataSource&wms_m_location l      
                            where l.warehouse_id = @warehouse_id  and l.location_type= @location_type  and rownum<=1";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("warehouse_id", wareouseid); 
            lstParam.Add("location_type", LocationType.LocationType_Conveyor.ToString());

            return DBHelper.SelectOne(sql, lstParam,"",false) ;
        }
        /// <summary>
        /// 查询货位中有多少容器
        /// </summary>
        /// <param name="locationcode"></param>
        /// <returns></returns>
        public int GetLocationHoldCtns(string locationcode) {
            string sql = @"select sum(d.qty) 
                                from &DataSource&wms_b_container_h h 
                                    inner join &DataSource&wms_b_container_d d on h.t_id=d.container_id
                                where h.container_barcode=@locationcode";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("locationcode", locationcode);
            return int.Parse(DBHelper.SelectOne(sql,lstParam));
        }
    }
}
