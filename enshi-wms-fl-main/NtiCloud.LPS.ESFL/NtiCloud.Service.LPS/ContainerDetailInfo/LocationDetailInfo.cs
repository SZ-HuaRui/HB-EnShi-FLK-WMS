using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Common.Interface;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Tasking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NtiCloud.Service.WMS.Inventory;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using static NtiCloud.Model.WMS.CountContainerInput;
using System.Threading.Tasks;
using TaskStatus = NtiCloud.Service.WMS.Base.TaskStatus;
using NtiCloud.Service.LPS.Inbound;

namespace NtiCloud.Service.WMS.LocationDetailInfo
{
    public class LocationDetailInfo : WmsObject, ILocationDetailInfo
    {
        private readonly IWorkTask _workTask;
        private readonly IWorkTaskItem _workTaskItem;
        private readonly IContainer _container;

        public LocationDetailInfo(IWorkTask workTask, IWorkTaskItem workTaskItem, IContainer container)
        {
            _workTask = workTask;
            _workTaskItem = workTaskItem;
            _container = container;
        }

        public LocationDetailInfo()
        {
            _workTask = new WorkTask();
            _workTaskItem = new WorkTaskItem();
            _container = new Container();
        }

        #region 根据层查询库存
        /// <summary>
        /// 根据层查询库存
        /// </summary>
        /// <param name="location_row">行</param>
        /// <returns></returns>
        public WebJsonInfo GetLocationDetailInfo(string location_row, string zoneid,string itemname,string itemcode,string lot)
        {
            try
            {
                string sql = "";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Clear();
                sql = @"select l.location_code,l.location_row,l.location_column,l.location_layer,l.location_status,l.location_flag,l.LOCATION_DEPTH,
                        LISTAGG(d.item_name, ',') WITHIN GROUP (ORDER BY d.item_name) as ITEM_NAME, LISTAGG(i.ITEM_CODE, ',') WITHIN GROUP (ORDER BY i.ITEM_CODE) as ITEM_CODE,LISTAGG(d.LOT_NO, ',') WITHIN GROUP (ORDER BY d.LOT_NO) as LOT_NO, sum(d.qty) remain_qty 
                        from &DataSource&wms_m_location l left join &DataSource&wms_b_container_h h on l.t_id = h.location_id 
                        left join &DataSource&wms_b_container_d d on h.t_id = d.container_id
                        left join &DataSource&wms_m_item i on i.t_id = d.ITEM_ID
                        where l.Zone_ID=@ZONE_ID and l.act_status = '1' and h.act_status = '1' and d.act_status = '1' and l.location_row = @location_row and l.warehouse_id = @warehouse_id ";
                if(!string.IsNullOrEmpty(itemcode))
                {
                    sql += " and  i.ITEM_CODE=@ITEM_CODE ";
                    param.Add("ITEM_CODE", itemcode);
                }

                if (!string.IsNullOrEmpty(itemname))
                {
                    sql += " and  i.ITEM_NAME=@ITEM_NAME ";
                    param.Add("ITEM_NAME", itemname);
                }

                if (!string.IsNullOrEmpty(lot))
                {
                    sql += " and  d.LOT_NO=@LOT_NO ";
                    param.Add("LOT_NO", lot);
                }

                sql +=@"   group by l.location_code,l.location_row,l.location_column,l.location_layer,l.location_status,l.location_flag,l.LOCATION_DEPTH
                        order by l.location_row,l.location_column,l.location_layer,l.LOCATION_DEPTH ";
                
                param.Add("location_row", location_row);
                param.Add("ZONE_ID", zoneid);
                param.Add("warehouse_id", Context.WarehouseID);
                DataTable dt = DBHelper.Select(sql, param, "查询货位是否存在", false);
                if (dt.Rows.Count<1)
                {
                    throw new WmsException(500,"数据不存在");
                }

                List<LocationCell> cellList = new List<LocationCell>();
                foreach(DataRow r in dt.Rows)
                {
                    LocationCell cell = new LocationCell();
                    cell.location = (r["LOCATION_CODE"] == null) ? ("") : (r["LOCATION_CODE"].ToString());
                    cell.location_column = (r["LOCATION_COLUMN"] == null) ? ("") : (r["LOCATION_COLUMN"].ToString());
                    cell.location_row = (r["LOCATION_ROW"] == null) ? ("") : (r["LOCATION_ROW"].ToString());
                    cell.location_layer = (r["LOCATION_LAYER"] == null) ? ("") : (r["LOCATION_LAYER"].ToString());
                    cell.index_no = Convert.ToInt32(cell.location_layer).ToString();
                    cell.index_col = Convert.ToInt32(cell.location_column).ToString();
                    cell.deep = (r["LOCATION_DEPTH"] == null) ? ("") : (r["LOCATION_DEPTH"].ToString());
                    cell.itemcode = (r["ITEM_CODE"] == null) ? ("") : (r["ITEM_CODE"].ToString());
                    cell.itemname = (r["ITEM_NAME"] == null) ? ("") : (r["ITEM_NAME"].ToString());
                    cell.lot = (r["LOT_NO"] == null) ? ("") : (r["LOT_NO"].ToString());

                    string locationState = (r["LOCATION_STATUS"] == null) ? ("") : (r["LOCATION_STATUS"].ToString());
                    try
                    {
                        LocationStatus locState = (LocationStatus)Enum.Parse(typeof(LocationStatus), locationState);
                        switch (locState)
                        {
                            case LocationStatus.LocationStatus_Plan: //预分配
                                { 
                                    cell.stock_sts = "Plan"; 
                                }
                                break;
                            case LocationStatus.LocationStatus_Empty: //空
                                {
                                    cell.stock_sts = "Empty";
                                }
                                break;
                            case LocationStatus.LocationStatus_In: //待入库
                                {
                                    cell.stock_sts = "In";
                                }
                                break;
                            case LocationStatus.LocationStatus_Stored: //已存储
                                {
                                    cell.stock_sts = "Stored";
                                }
                                break;
                            case LocationStatus.LocationStatus_Out: //待出库
                                {
                                    cell.stock_sts = "Out";
                                }
                                break;
                        }
                    }
                    catch(Exception ex)
                    {

                    }

                    string locationFlag = (r["LOCATION_FLAG"] == null) ? ("") : (r["LOCATION_FLAG"].ToString());

                    try
                    {
                        LocationFlag locFlag = (LocationFlag)Enum.Parse(typeof(LocationFlag), locationFlag);
                        switch (locFlag)
                        {
                            case LocationFlag.LocationFlag_InOnly: //可入不可出
                                {
                                    cell.useable_sts = "OnlyIn";
                                }
                                break;
                            case LocationFlag.LocationFlag_OutOnly: //可出不可入
                                {
                                    cell.useable_sts = "OnlyOut";
                                }
                                break;
                            case LocationFlag.LocationFlag_Usable: //可用
                                {
                                    cell.useable_sts = "Enable";
                                }
                                break;

                            case LocationFlag.LocationFlag_RowLock: //行出库锁定
                                {
                                    cell.useable_sts = "RowLock";
                                }
                                break;

                            case LocationFlag.LocationFlag_Hold: //保留货位
                                {
                                    cell.useable_sts = "Hold";
                                }
                                break;

                            case LocationFlag.LocationFlag_Forbidden: //禁用
                                {
                                    cell.useable_sts = "Disable";
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    

                    cell.remain_qty = (r["remain_qty"] == null|| r["remain_qty"].ToString()=="") ? ("0") : (r["remain_qty"].ToString());
                    cellList.Add(cell);
                }

                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = null, json = cellList };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 根据层查询库存
        /// <summary>
        /// 根据层查询库存
        /// </summary>
        /// <param name="Layer">层</param>
        /// <returns></returns>
        public WebJsonInfo GetZoneInfo()
        {
            try
            {
                string sql = "";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Clear();
                sql = @"select * from &DataSource&wms_m_zone z where z.is_enabled = 1 and z.zone_type in ('ZoneType_Rack','ZoneType_Mix') and z.warehouse_id = @warehouse_id ";
                param.Add("warehouse_id", Context.WarehouseID);
                DataTable dt = DBHelper.Select(sql, param, "查询区域信息", false);
                if (dt.Rows.Count < 1)
                {
                    throw new WmsException(500, "区域数据不存在");
                }

                List<ZoneInfo> cellList = new List<ZoneInfo>();
                foreach (DataRow r in dt.Rows)
                {
                    ZoneInfo cell = new ZoneInfo();
                    cell.ZoneID = (r["T_ID"] == null) ? ("") : (r["T_ID"].ToString());
                    cell.ZoneCode = (r["ZONE_CODE"] == null) ? ("") : (r["ZONE_CODE"].ToString());
                    cell.ZoneName = (r["ZONE_NAME"] == null) ? ("") : (r["ZONE_NAME"].ToString());
                    cell.ParentID = (r["P_ID"] == null) ? ("") : (r["P_ID"].ToString());

                    param.Clear();
                    sql = @"select distinct LOCATION_LAYER from &DataSource&wms_m_location l where l.ZONE_ID=@ZONE_ID and l.warehouse_id = @warehouse_id order by LOCATION_LAYER";
                    param.Add("warehouse_id", Context.WarehouseID);
                    param.Add("ZONE_ID", cell.ZoneID);
                    DataTable layerDT = DBHelper.Select(sql, param, "查询区域所有层", false);

                    IList<string> layerList = new List<string>();

                    foreach(DataRow layerRow in layerDT.Rows)
                    {
                        layerList.Add(layerRow["LOCATION_LAYER"] == null ? "" : layerRow["LOCATION_LAYER"].ToString());
                    }

                    cell.LayerList = layerList;

                    //排
                    param.Clear();
                    sql = @"select distinct LOCATION_ROW from &DataSource&wms_m_location l where l.ZONE_ID=@ZONE_ID and l.warehouse_id = @warehouse_id order by LOCATION_ROW";
                    param.Add("warehouse_id", Context.WarehouseID);
                    param.Add("ZONE_ID", cell.ZoneID);
                    DataTable rowDT = DBHelper.Select(sql, param, "查询区域所有行", false);

                    IList<string> rowList = new List<string>();

                    foreach (DataRow rowRow in rowDT.Rows)
                    {
                        rowList.Add(rowRow["LOCATION_LAYER"] == null ? "" : rowRow["LOCATION_ROW"].ToString());
                    }

                    cell.RowList = rowList;

                    cellList.Add(cell);
                }

                List<List<ZoneInfo>> zoneTree = new List<List<ZoneInfo>>();
                List<ZoneInfo> parentZoneList = cellList.Where(o => o.ParentID == "" || o.ParentID == null).ToList();

                foreach (ZoneInfo parent in parentZoneList)
                {
                    List<ZoneInfo> lastZone = BulidTreeByRecursive(cellList, parent.ZoneID);
                    if (lastZone.Count == 0)
                    {
                        parent.ChildZone = null;
                    }
                    else
                    {
                        parent.ChildZone = lastZone;
                    }
                    List<ZoneInfo> zoneTemp = new List<ZoneInfo>();
                    zoneTemp.Add(parent);
                    zoneTree.Add(zoneTemp);
                }
                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = null, json = zoneTree };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 使用递归方法建树
        /// </summary>
        public List<ZoneInfo> BulidTreeByRecursive(List<ZoneInfo> treeNodes, string zoneID)
        {
            List<ZoneInfo> tempInfoList = new List<ZoneInfo>();
            List<ZoneInfo> tempList = treeNodes.Where(c => c.ParentID == zoneID).ToList();

            for (int i = 0; i < tempList.Count; i++)
            {
                ZoneInfo node = new ZoneInfo();
                node.ZoneID = tempList[i].ZoneID;
                node.ParentID = tempList[i].ParentID;
                node.ZoneName = tempList[i].ZoneName;
                node.ZoneCode = tempList[i].ZoneCode;
                node.LayerList = tempList[i].LayerList;
                node.RowList = tempList[i].RowList;
                node.ChildZone=BulidTreeByRecursive(treeNodes,  node.ZoneID);
                tempInfoList.Add(node);
            }

            return tempInfoList;
        }

        #endregion

    }
}
