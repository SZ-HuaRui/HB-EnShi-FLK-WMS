using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.LPS.Model;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Services.WMS.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace NtiCloud.Service.WMS.Inventory
{
    public class Container : WmsObject, IContainer
    {
        public void ChangeLocation(string headID, string newLocationNoOrID)
        {
            Location location = new Location();
            DataRow head = GetHead(headID);
            DataRow newLocRow = location.Get(newLocationNoOrID);
            string newLocID = newLocRow["t_id"].ToString();
            string oldLocID = head["location_id"].ToString();
            //if (head["location_id"].ToString() == newLocRow["t_id"].ToString())
            if (newLocID == oldLocID)
            {
                return;
            }
            DataRow oldLocRow = location.Get(oldLocID);
            UpdateHeadLocation(headID, newLocID, oldLocID);
            if (location.IsLocationEmpty(oldLocID))
            {
                location.UpdateStatus(oldLocID,
                    "LocationState_Empty", oldLocRow["location_state"].ToString());
            }
            //if (newLocRow["location_state"].ToString() == "LocationState_Empty")
            //{
            //    location.UpdateLocationStockState(oldLocID, 
            //        "LocationState_Stored", newLocRow["location_state"].ToString());
            //}
        }

        public bool existsPallet(string headIDOrBarcode)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container", headIDOrBarcode },
            };
            string sql;
            sql = "select * from &DataSource&dwms_container_h  " +
                          " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                          "    and whse_id = @warehouse " +
                          "    and (t_id = @container or container_barcode = @container) ";

            DataTable dt = DBHelper.Select(sql, param, "查询容器头表", false);

            if (dt.Rows.Count > 0) return true;
            return false;
        }

        public DataRow GetHead(string headIDOrBarcode)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container", headIDOrBarcode },
            };
            string sql = @"
select c.*, 
       l.t_id location_id, l.location_status
  from &DataSource&wms_b_container_h c
  left join &DataSource&wms_m_location l on l.t_id = c.location_id
 where c.client_id='&CLIENT_ID&' and c.org_id='&ORG_ID&'
   and c.warehouse_id = @warehouse 
   and c.act_status = '1'
   and (c.t_id = @container or c.container_barcode = @container) ";

            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询容器头表");

            return dt.Rows[0];
        }
        /// <summary>
        /// 不管是否已经出库
        /// </summary>
        /// <param name="headIDOrBarcode"></param>
        /// <returns></returns>
        public DataRow GetHeadAll(string headIDOrBarcode)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container", headIDOrBarcode },
            };
            string sql = @"
select c.*, 
       l.t_id location_id, l.location_status
  from &DataSource&wms_b_container_h c
  left join &DataSource&wms_m_location l on l.t_id = c.location_id
 where c.client_id='&CLIENT_ID&' and c.org_id='&ORG_ID&'
   and c.warehouse_id = @warehouse 
   and (c.t_id = @container or c.container_barcode = @container) ";

            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询容器头表");

            return dt.Rows[0];
        }

        public DataRow[] GetHeadsByLocation(string locationID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "location_id", locationID },
            };
            string sql = @"
select c.*, l.location_type, l.location_status, l.location_flag, l.process_status
  from &DataSource&wms_b_container_h c
  left join &DataSource&wms_m_location l on l.t_id = c.location_id
 where l.t_id = @location_id
   and l.act_status = '1'
   and c.act_status = '1'
";

            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询容器头表", false);

            return dt.AsEnumerable().ToArray();
        }

        /*
        public DataRow[] GetDetails(string containerID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container_id", containerID },
            };
            string sql = @"
select * from wms_b_container_d cd
 where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&'
   and cd.warehouse_id = @warehouse 
   and cd.container_id = @container_id ";

            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询容器明细", false);

            return dt.AsEnumerable().ToArray();
        }
        */

        public DataRow[] GetDetails(string containerID, string itemID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container_id", containerID },
                { "item_id", itemID },
            };
            string sql = @"
                            select * from &DataSource&wms_b_container_d cd
                             where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&'
                               and cd.warehouse_id = @warehouse 
                               and cd.container_id = @container_id 
                               and cd.item_id = @item_id 
                        ";

            DataTable dt;
            //dt = DBHelper.Select(sql, param, "查询容器明细", false);
            dt = DBHelper.Select(sql, param, "查询容器明细");

            return dt.AsEnumerable().ToArray();
        }

        public DataRow GetDetail(string detailID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "detail_id", detailID },
            };
            string sql = @"
select cd.*,(select i.item_code from &DataSource&wms_m_item i where i.t_id=cd.item_id) as item_code from &DataSource&wms_b_container_d cd
 where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&'
   and cd.warehouse_id = @warehouse 
    and t_id = @detail_id 
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询容器明细");

            return dt.Rows[0];
        }


        public DataRow GetDetail(string headID, string itemID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container_id", headID },
                { "item_id", itemID },
            };
            string sql = @"
select * from &DataSource&wms_b_container_d cd
 where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&'
   and cd.warehouse_id = @warehouse 
   and container_id = @container_id 
   and item_id = @item_id 
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询容器明细");

            return dt.Rows[0];
        }
        public DataRow[] GetDetailByHeadID(string headID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                //{ "warehouse", Context.WarehouseID },
                { "container_id", headID }
            };
            //string sql = @" select * from &DataSource&wms_b_container_d cd where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&'  and cd.warehouse_id = @warehouse  and container_id = @container_id ";
            string sql = $" select * from &DataSource&wms_b_container_d cd where container_id = @container_id and act_status='1'";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询容器明细");

            return dt.AsEnumerable().ToArray();
        }

        public DataRow GetDetail(string headID, string itemID, string batchNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container_id", headID },
                { "item_id", itemID },
                { "batch_no", batchNo },
            };
            string sql = @"
select * from &DataSource&wms_b_container_d cd
 where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&'
   and cd.warehouse_id = @warehouse 
   and container_id = @container_id 
   and item_id = @item_id 
   and lot_no = @batch_no 
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询容器明细");

            return dt.Rows[0];
        }      

        public DataRow GetDetail(string headID, string itemID,
            string batchNo = null, string inboundID = null)
        {
            //batchNo ??= " ";
            //inboundID ??= " ";
            batchNo = string.IsNullOrWhiteSpace(batchNo) ? " " : batchNo;
            inboundID = string.IsNullOrWhiteSpace(inboundID) ? " " : batchNo;

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container_id", headID },
                { "item_id", itemID },
                { "batch_no", batchNo },
                { "inbound_id", inboundID },
            };
            string sql = @"
select * from &DataSource&wms_b_container_d cd
 where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&'
   and cd.warehouse_id = @warehouse 
   and container_id = @container_id 
   and item_id = @item_id 
   and (lot_no = @batch_no or @batch_no = ' ')
   and (inbound_h_id = @inbound_id or @inbound_id = ' ')
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询容器明细");

            return dt.Rows[0];
        }
        public DataRow[] GetDetails(string headID, string itemID = null,
            string batchNo = null, string inboundID = null)
        {
            //batchNo ??= " ";
            //inboundID ??= " ";
            itemID = string.IsNullOrWhiteSpace(itemID) ? " " : itemID;
            batchNo = string.IsNullOrWhiteSpace(batchNo) ? " " : batchNo;
            inboundID = string.IsNullOrWhiteSpace(inboundID) ? " " : inboundID;

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container_id", headID },
                { "item_id", itemID },
                { "batch_no", batchNo },
                { "inbound_id", inboundID },
            };
            string sql = @"
select * from &DataSource&wms_b_container_d cd
 where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&'
   and cd.warehouse_id = @warehouse 
   and container_id = @container_id 
   and act_status = '1' 
   and (item_id = @item_id or @item_id = ' ')
   and (lot_no = @batch_no or @batch_no = ' ')
   and (inbound_h_id = @inbound_id or @inbound_id = ' ')
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询容器明细", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow GetDetailByLocation(string locNoOrID, string itemID,
            string batchNo = null, string inboundID = null)
        {
            DataRow locRow = Location.Get(locNoOrID);
            //DataRow containerRow = Container.GetHead(locRow[l.ID].ToString());
            DataRow[] containerRows = Container.GetHeadsByLocation(locRow[l.ID].ToString());
            DataRow containerRow = containerRows.Where(
                r => r[ch.ContainerType].ToString() == ContainerType.ContainerType_Virtual.ToString()
            ).FirstOrDefault();
            DataRow ctnDtlRow = GetDetail(
                        containerRow[ch.ID].ToString(),
                        itemID, batchNo, inboundID);
            return ctnDtlRow;
        }

        public IContainer Insert(ContainerHeadModel model)
        {
            model.ID = model.ID ?? SnowflakeID.Guid().ToString();
            model.WarehouseID = model.WarehouseID ?? Context.WarehouseID;
            model.CreatedBy = model.CreatedBy ?? Context.UserInfo.UserName;
            model.ModifyIP = model.ModifyIP ?? Context.UserInfo.LoginIP;
            model.LastModifiedBy = model.LastModifiedBy ?? Context.UserInfo.UserName;
            model.ActStatus = "1";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "t_id", model.ID },
                { "warehouse_id", model.WarehouseID },
                { "location_id", model.LocationID },
                { "container_barcode", model.ContainerBarcode },
                { "is_full", model.IsFull },
                { "container_type", model.ContainerType },
                { "container_catalog_id", model.ContainerCatalogID },
                { "bom_id", model.BomID },
                { "equipment_id", model.EquipmentID },
                { "allow_called", model.AllowCalled },
                { "first_in_date", model.FirstInDate.ToString() },
                { "label_code", model.LabelCode },
                { "owner_id", model.OwnerID },
                { "is_in", model.IsIn },
                { "take_in_by", model.TakeInBy },
                { "is_out", model.IsOut },
                { "take_out_by", model.TakeOutBy },
                { "is_mixed_item", model.IsMixedItem },
                { "is_mixed_lot", model.IsMixedLot },
                { "memo", model.Memo },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into &DataSource&WMS_B_CONTAINER_H ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       location_id, " +
                "       container_barcode, " +
                "       is_full, " +
                "       container_type, " +
                "       container_catalog_id, " +
                "       bom_id, " +
                "       equipment_id, " +
                "       allow_called, " +
                "       first_in_date, " +
                "       label_code, " +
                "       owner_id, " +
                "       is_in, " +
                "       take_in_by, " +
                "       is_out, " +
                "       take_out_by, " +
                "       is_mixed_item, " +
                "       is_mixed_lot, " +
                "       memo, " +
                "       create_date, " +
                "       created_by, " +
                "       modify_ip, " +
                "       modify_date, " +
                "       last_modified_by, " +
                "       act_status) " +
                "values ( " +
                "       @t_id, " +
                "       '&CLIENT_ID&', " +
                "       '&ORG_ID&', " +
                "       @warehouse_id, " +
                "       @location_id, " +
                "       @container_barcode, " +
                "       @is_full, " +
                "       @container_type, " +
                "       @container_catalog_id, " +
                "       @bom_id, " +
                "       @equipment_id, " +
                "       @allow_called, " +
                "       date_format(@first_in_date, '%Y-%m-%d %H:%i:%s'), " +
                "       @label_code, " +
                "       @owner_id, " +
                "       @is_in, " +
                "       @take_in_by, " +
                "       @is_out, " +
                "       @take_out_by, " +
                "       @is_mixed_item, " +
                "       @is_mixed_lot, " +
                "       @memo, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "容器的使用信息");

            return this;
        }

        public IContainer InsertHead(DataRow row)
        {
            Insert(new ContainerHeadModel()
            {
                ID = row["t_id"].ToString(),
                ClientID = row["client_id"].ToString(),
                OrgID = row["org_id"].ToString(),
                WarehouseID = row["warehouse_id"].ToString(),
                LocationID = row["location_id"].ToString(),
                ContainerBarcode = row["container_barcode"].ToString(),
                IsFull = row["is_full"].ToString(),
                ContainerType = row["container_type"].ToString(),
                ContainerCatalogID = row["container_catalog_id"].ToString(),
                BomID = row["bom_id"].ToString(),
                EquipmentID = row["equipment_id"].ToString(),
                AllowCalled = row["allow_called"].ToString(),
                FirstInDate = DBHelper.GetDateTime(row["first_in_date"]),
                LabelCode = row["label_code"].ToString(),
                OwnerID = row["owner_id"].ToString(),
                IsIn = row["is_in"].ToString(),
                TakeInBy = row["take_in_by"].ToString(),
                IsOut = row["is_out"].ToString(),
                TakeOutBy = row["take_out_by"].ToString(),
                IsMixedItem = row["is_mixed_item"].ToString(),
                IsMixedLot = row["is_mixed_lot"].ToString(),
                Memo = row["memo"].ToString(),
                CreateDate = DBHelper.GetDateTime(row["create_date"]),
                CreatedBy = row["created_by"].ToString(),
                ModifyIP = row["modify_ip"].ToString(),
                ModifyDate = DBHelper.GetDateTime(row["modify_date"]),
                LastModifiedBy = row["last_modified_by"].ToString(),
                ActStatus = row["act_status"].ToString()
            });
            return this;
        }

        public IContainer Insert(ContainerDetailModel model)
        {
            model.ID = model.ID ?? SnowflakeID.Guid().ToString();
            model.WarehouseID = model.WarehouseID ?? Context.WarehouseID;
            model.CreatedBy = model.CreatedBy ?? Context.UserInfo.UserName;
            model.ModifyIP = model.ModifyIP ?? Context.UserInfo.LoginIP;
            model.LastModifiedBy = model.LastModifiedBy ?? Context.UserInfo.UserName;
            model.ActStatus = "1";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "t_id", model.ID },
                { "warehouse_id", model.WarehouseID },
                { "container_id", model.ContainerID },
                { "inbound_h_id", model.InboundHeadID },
                { "outbound_h_id", model.OutboundHeadID },
                { "sub_container_no", model.SubContainerNo },
                { "supplier_id", model.SupplierID },
                { "bom_id", model.BomID },
                { "owner_id", model.OwnerID },
                { "item_id", model.ItemID },
                { "item_name", model.ItemName },
                { "lot_no", model.LotNo },
                { "owner_lot_no", model.OwnerLotNo },
                { "wms_lot_no", model.WmsLotNo },
                { "exist_barcode", model.ExistBarcode },
                { "is_full", model.IsFull },
                { "given_year", model.GivenYear.ToString() },
                { "sub_factory", model.SubFactory },
                { "sub_inventory", model.SubInventory },
                { "sub_location", model.SubLocation },
                { "qc_status", model.QCStatus },
                { "qty", model.Qty.ToString() },
                { "stat_qty", model.StatQty.ToString() },
                { "in_qty", model.InQty.ToString() },
                { "out_qty", model.OutQty.ToString() },
                { "lock_qty", model.LockQty.ToString() },
                { "spec", model.Spec },
                { "uom_qty_id", model.UomQtyID },
                { "uom_stat_id", model.UomStatID },
                { "ratio", model.Ratio.ToString() },
                { "first_in_date", model.FirstInDate.ToString() },
                { "start_out_date", model.StartOutDate.ToString() },
                { "production_date", model.ProductionDate.ToString() },
                { "exp_date", model.ExpDate.ToString() },
                { "warning_date", model.WarningDate.ToString() },
                { "first_out_date", model.FirstOutDate.ToString() },
                { "out_end_date", model.OutEndDate.ToString() },
                { "use_by_date", model.UseByDate.ToString() },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into &DataSource&WMS_B_CONTAINER_D ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       container_id, " +
                "       inbound_h_id, " +
                "       outbound_h_id, " +
                "       sub_container_no, " +
                "       supplier_id, " +
                "       bom_id, " +
                "       owner_id, " +
                "       item_id, " +
                "       item_name, " +
                "       lot_no, " +
                "       owner_lot_no, " +
                "       wms_lot_no, " +
                "       exist_barcode, " +
                "       is_full, " +
                "       given_year, " +
                "       sub_factory, " +
                "       sub_inventory, " +
                "       sub_location, " +
                "       qc_status, " +
                "       qty, " +
                "       stat_qty, " +
                "       in_qty, " +
                "       out_qty, " +
                "       lock_qty, " +
                "       spec, " +
                "       uom_qty_id, " +
                "       uom_stat_id, " +
                "       ratio, " +
                "       first_in_date, " +
                "       start_out_date, " +
                "       production_date, " +
                "       exp_date, " +
                "       warning_date, " +
                "       first_out_date, " +
                "       out_end_date, " +
                "       use_by_date, " +
                "       create_date, " +
                "       created_by, " +
                "       modify_ip, " +
                "       modify_date, " +
                "       last_modified_by, " +
                "       act_status) " +
                "values ( " +
                "       @t_id, " +
                "       '&CLIENT_ID&', " +
                "       '&ORG_ID&', " +
                "       @warehouse_id, " +
                "       @container_id, " +
                "       @inbound_h_id, " +
                "       @outbound_h_id, " +
                "       @sub_container_no, " +
                "       @supplier_id, " +
                "       @bom_id, " +
                "       @owner_id, " +
                "       @item_id, " +
                "       @item_name, " +
                "       @lot_no, " +
                "       @owner_lot_no, " +
                "       @wms_lot_no, " +
                "       @exist_barcode, " +
                "       @is_full, " +
                "       @given_year, " +
                "       @sub_factory, " +
                "       @sub_inventory, " +
                "       @sub_location, " +
                "       @qc_status, " +
                "       @qty, " +
                "       @stat_qty, " +
                "       @in_qty, " +
                "       @out_qty, " +
                "       @lock_qty, " +
                "       @spec, " +
                "       @uom_qty_id, " +
                "       @uom_stat_id, " +
                "       @ratio, " +
                "       date_format(@first_in_date, '%Y-%m-%d %H:%i:%s'), " +
                "       date_format(@start_out_date, '%Y-%m-%d %H:%i:%s'), " +
                "       date_format(@production_date, '%Y-%m-%d %H:%i:%s'), " +
                "       date_format(@exp_date, '%Y-%m-%d %H:%i:%s'), " +
                "       date_format(@warning_date, '%Y-%m-%d %H:%i:%s'), " +
                "       date_format(@first_out_date, '%Y-%m-%d %H:%i:%s'), " +
                "       date_format(@out_end_date, '%Y-%m-%d %H:%i:%s'), " +
                "       date_format(@use_by_date, '%Y-%m-%d %H:%i:%s'), " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "容器库存表");

            return this;
        }

        public IContainer InsertDetail(DataRow row)
        {
            Insert(new ContainerDetailModel()
            {
                ID = row["t_id"].ToString(),
                ClientID = row["client_id"].ToString(),
                OrgID = row["org_id"].ToString(),
                WarehouseID = row["warehouse_id"].ToString(),
                ContainerID = row["container_id"].ToString(),
                InboundHeadID = row["inbound_h_id"].ToString(),
                OutboundHeadID = row["outbound_h_id"].ToString(),
                SubContainerNo = row["sub_container_no"].ToString(),
                SupplierID = row["supplier_id"].ToString(),
                BomID = row["bom_id"].ToString(),
                OwnerID = row["owner_id"].ToString(),
                ItemID = row["item_id"].ToString(),
                ItemName = row["item_name"].ToString(),
                LotNo = row["lot_no"].ToString(),
                OwnerLotNo = row["owner_lot_no"].ToString(),
                WmsLotNo = row["wms_lot_no"].ToString(),
                ExistBarcode = row["exist_barcode"].ToString(),
                IsFull = row["is_full"].ToString(),
                GivenYear = DBHelper.GetDecimal(row["given_year"]),
                SubFactory = row["sub_factory"].ToString(),
                SubInventory = row["sub_inventory"].ToString(),
                SubLocation = row["sub_location"].ToString(),
                QCStatus = row["qc_status"].ToString(),
                Qty = DBHelper.GetDecimal(row["qty"]),
                StatQty = DBHelper.GetDecimal(row["stat_qty"]),
                InQty = DBHelper.GetDecimal(row["in_qty"]),
                OutQty = DBHelper.GetDecimal(row["out_qty"]),
                LockQty = DBHelper.GetDecimal(row["lock_qty"]),
                Spec = row["spec"].ToString(),
                UomQtyID = row["uom_qty_id"].ToString(),
                UomStatID = row["uom_stat_id"].ToString(),
                Ratio = DBHelper.GetDecimal(row["ratio"]),
                FirstInDate = DBHelper.GetDateTime(row["first_in_date"]),
                StartOutDate = DBHelper.GetDateTime(row["start_out_date"]),
                ProductionDate = DBHelper.GetDateTime(row["production_date"]),
                ExpDate = DBHelper.GetDateTime(row["exp_date"]),
                WarningDate = DBHelper.GetDateTime(row["warning_date"]),
                FirstOutDate = DBHelper.GetDateTime(row["first_out_date"]),
                OutEndDate = DBHelper.GetDateTime(row["out_end_date"]),
                UseByDate = DBHelper.GetDateTime(row["use_by_date"]),
                CreateDate = DBHelper.GetDateTime(row["create_date"]),
                CreatedBy = row["created_by"].ToString(),
                ModifyIP = row["modify_ip"].ToString(),
                ModifyDate = DBHelper.GetDateTime(row["modify_date"]),
                LastModifiedBy = row["last_modified_by"].ToString(),
                ActStatus = row["act_status"].ToString()
            });

            return this;
        }
        public IContainer UpdateHeadPackingType(string headID,
            string packingType, string oldPackingType = null)
        {
            if (oldPackingType == null)
            {
                oldPackingType = GetHead(headID)["packing_type"].ToString();
                oldPackingType = string.IsNullOrWhiteSpace(oldPackingType) ? " " : oldPackingType;
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", headID },
                { "new_type", packingType },
                { "old_type", oldPackingType },
            };
            string sql =
                "update &DataSource&wms_b_container_h set packing_type = @new_type " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "    and warehouse_id = @warehouse " +
                "    and t_id = @t_id " +
                "    and (packing_type = @old_type or @old_type = ' ') ";
            DBHelper.RunSql(sql, param, "修改容器垛型");

            return this;
        }

        public void UpdateHeadLocation(string headID, string locationID, string oldLocationID = null)
        {
            if (oldLocationID == null)
            {
                oldLocationID = GetHead(headID)["location_id"].ToString();
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", headID },
                { "new_loc", locationID },
                { "old_loc", oldLocationID },
            };
            string sql =
                "update &DataSource&dwms_container_h set location_id = @new_loc " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "    and whse_id = @warehouse " +
                "    and t_id = @t_id " +
                "    and location_id = @old_loc ";
            DBHelper.RunSql(sql, param, "修改容器所属货位");
        }

        public void AddHeadQty(string headID, decimal qty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + GetHead(headID)["qty"]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", headID },
                { "qty", qty.ToString() },
                { "old_qty", oldQty.ToString() },
            };
            string sql =
                "update &DataSource&dwms_container_h set qty = ifnull(qty, 0) + @qty " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "    and whse_id = @warehouse " +
                "    and t_id = @t_id " +
                "    and ifnull(qty, 0) = @old_qty ";
            DBHelper.RunSql(sql, param, "修改容器头数量");
        }

        public void AddHeadInQty(string headID, decimal qty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + GetHead(headID)["in_qty"]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", headID },
                { "qty", qty.ToString() },
                { "old_qty", oldQty.ToString() },
            };
            string sql =
                "update &DataSource&dwms_container_h set in_qty = ifnull(in_qty, 0) + @qty " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "    and whse_id = @warehouse " +
                "    and t_id = @t_id " +
                "    and ifnull(in_qty, 0) = @old_qty ";
            DBHelper.RunSql(sql, param, "修改容器头数量");
        }

        public IContainer AddDetailFinanceQty(string detailID, decimal qty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + GetHead(detailID)[cd.StatQty]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", detailID },
                { "qty", qty.ToString() },
                { "old_qty", oldQty.ToString() },
            };
            string sql =
                "update &DataSource&wms_b_container_d " +
                "   set stat_qty = ifnull(stat_qty, 0) + @qty " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "   and warehouse_id = @warehouse " +
                "   and t_id = @t_id " +
                "   and ifnull(stat_qty, 0) = @old_qty ";
            DBHelper.RunSql(sql, param, "修改容器明细统计数量");

            return this;
        }

        public IContainer AddDetailQty(string detailID, decimal qty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + GetDetail(detailID)["qty"]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", detailID },
                { "qty", qty.ToString() },
                { "old_qty", oldQty.ToString() },
            };
            string sql =
                "update &DataSource&wms_b_container_d " +
                "   set qty = ifnull(qty, 0) + @qty " +
                //"   set qty = ifnull(qty, 0) + @qty, " +
                //"       stat_qty = (ifnull(qty, 0) + @qty) * ifnull(speci, 1) " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "   and warehouse_id = @warehouse " +
                "   and t_id = @t_id " +
                "   and ifnull(qty, 0) = @old_qty ";
            DBHelper.RunSql(sql, param, "修改容器明细数量");

            return this;
        }


        public IContainer AddDetailInQty(string detailID, decimal qty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + GetDetail(detailID)["in_qty"]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", detailID },
                { "qty", qty.ToString() },
                { "old_qty", oldQty.ToString() },
            };
            string sql =
                "update &DataSource&wms_b_container_d set in_qty = ifnull(in_qty, 0) + @qty " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "    and warehouse_id = @warehouse " +
                "    and t_id = @t_id " +
                "    and ifnull(in_qty, 0) = @old_qty ";
            DBHelper.RunSql(sql, param, "修改容器明细数量");

            return this;
        }

        public IContainer AddDetailOutQty(string detailID, decimal qty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + GetDetail(detailID)["out_qty"]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", detailID },
                { "qty", qty.ToString() },
                { "old_qty", oldQty.ToString() },
            };
            string sql =
                "update &DataSource&wms_b_container_d set out_qty = ifnull(out_qty, 0) + @qty " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "    and warehouse_id = @warehouse " +
                "    and t_id = @t_id " +
                "    and ifnull(out_qty, 0) = @old_qty ";
            DBHelper.RunSql(sql, param, "修改容器明细待出数量");

            return this;
        }

        public IContainer DeleteDetail(string detailID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                //{ "warehouse", Context.WarehouseID },
                { "t_id", detailID },
            };
            //string sql = @" delete from &DataSource&wms_b_container_d cd where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&' and cd.warehouse_id = @warehouse  and cd.t_id = @t_id  ";
            string sql = @" delete from &DataSource&wms_b_container_d cd where cd.t_id = @t_id  ";
            DBHelper.RunSql(sql, param, "删除容器明细");

            return this;
        }
        public IContainer DeleteDetails(string containerId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                //{ "warehouse", Context.WarehouseID },
                { "container_id", containerId },
            };
            //string sql = @" delete from &DataSource&wms_b_container_d cd where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&' and cd.warehouse_id = @warehouse  and cd.t_id = @t_id  ";
            string sql = @" delete from &DataSource&wms_b_container_d cd where cd.container_id = @container_id  ";
            DBHelper.RunSql(sql, param, "删除容器明细");

            return this;
        }

        public IContainer ClearDetail(string detailID)
        {
            DataRow detailRow = GetDetail(detailID);
            if (decimal.Parse("0" + detailRow[cd.Qty]) <= 0m
                && decimal.Parse("0" + detailRow[cd.OutQty]) <= 0m
                && decimal.Parse("0" + detailRow[cd.InQty]) <= 0m
                && decimal.Parse("0" + detailRow[cd.LockQty]) <= 0m)
            {
                UpdateDetailActStatus(detailID, "0");
            }
            return this;
        }

        public IContainer ClearDetailV1(string detailID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", detailID },
            };
            string sql = @"
delete from &DataSource&wms_b_container_d cd
 where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&'
   and cd.warehouse_id = @warehouse 
   and cd.t_id = @t_id 
   and ifnull(cd.qty, 0) = 0
   and ifnull(cd.in_qty, 0) = 0
   and ifnull(cd.out_qty, 0) = 0
   and ifnull(cd.lock_qty, 0) = 0
";
            DBHelper.RunSql(sql, param, "清理容器明细", DBHelper.OperatedNumType.ZeroOrMore);

            return this;
        }

        public bool DeleteHead(string headID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", headID },
            };
            string sql =
                "delete from &DataSource&wms_b_container_h " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "    and warehouse_id = @warehouse " +
                "    and t_id = @t_id ";
            return DBHelper.RunSql(sql, param, "删除容器") == 1;
        }

        public decimal PreparePutaway(ContainerDetailModel detailModel,
            decimal leftQty, decimal locationCapacity)
        {
            DataRow[] ctnDetails = Container.GetDetails(detailModel.ContainerID);

            if (locationCapacity == 0)
            {
                locationCapacity = decimal.MaxValue;
            }
            decimal actualQty = ctnDetails.Sum(r => decimal.Parse("0" + r["qty"]));
            decimal inQty = ctnDetails.Sum(r => decimal.Parse("0" + r["in_qty"]));
            decimal availQty = locationCapacity - actualQty - inQty;
            decimal qty = 0;

            // todo 4 test
            //availQty = leftQty;
            if (leftQty > availQty)
            {
                qty = availQty;
            }
            else
            {
                qty = leftQty;
            }
            if (qty <= 0)
            {
                throw new WmsException(151, "库容不足，入库数量错误，无法继续处理");
            }

            /*
            DataRow[] ctnDtlRow = null;
            string oneRow4OneReceipt = WmsConfig.GetOrDefault("Inbound:OneRow4OneReceipt", "1");
            if (oneRow4OneReceipt == "1")
            {
                ctnDtlRow = ctnDetails.Where(r
                    => r["item_id"].ToString() == detailModel.ItemID
                    && r["inbound_h_id"].ToString() == detailModel.InboundHeadID
                    ).ToArray();
            }
            else
            {
                ctnDtlRow = ctnDetails.Where(r
                    => r["item_id"].ToString() == detailModel.ItemID
                    ).ToArray();
            }
            */

            DataRow[] ctnDtlRows = null;
            ctnDtlRows = ctnDetails.Where(r
                    => r["item_id"].ToString() == detailModel.ItemID
            ).ToArray();

            string OneRow4OnePartialPacket = WmsConfig.GetOrDefault("Inbound:OneRow4OnePartialPacket", "0");
            detailModel.IsFull ??= "";
            if (OneRow4OnePartialPacket == "1")
            {
                if (detailModel.IsFull == "0")
                {
                    //Array.Clear(ctnDtlRows, 0, ctnDtlRows.Length);
                    ctnDtlRows = ctnDtlRows.Where(r => 1 == 0).ToArray();
                }
                else
                {
                    ctnDtlRows = ctnDtlRows.Where(r => r["is_full"].ToString() != "0").ToArray();
                }
            }

            string oneRow4Barcode = WmsConfig.GetOrDefault("Inbound:OneRow4BarcodeFlag", "1");
            detailModel.ExistBarcode ??= "";
            if (oneRow4Barcode == "1")
            {
                ctnDtlRows = ctnDtlRows.Where(r
                    => r["exist_barcode"].ToString() == detailModel.ExistBarcode
                    ).ToArray();
            }
            string oneRow4OneReceipt = WmsConfig.GetOrDefault("Inbound:OneRow4OneReceipt", "0");
            detailModel.InboundHeadID ??= "";
            if (oneRow4OneReceipt == "1")
            {
                ctnDtlRows = ctnDtlRows.Where(r
                    => r["inbound_h_id"].ToString() == detailModel.InboundHeadID
                    ).ToArray();
            }
            string oneRow4OneBatch = WmsConfig.GetOrDefault("Inbound:OneRow4OneBatch", "0");
            detailModel.LotNo ??= "";
            if (oneRow4OneBatch == "1")
            {
                ctnDtlRows = ctnDtlRows.Where(r
                    => r["lot_no"].ToString() == detailModel.LotNo
                    ).ToArray();
            }
            string oneRow4OneYear = WmsConfig.GetOrDefault("Inbound:OneRow4OneYear", "0");
            detailModel.GivenYear ??= 0;
            if (oneRow4OneYear == "1")
            {
                ctnDtlRows = ctnDtlRows.Where(r
                    => decimal.Parse("0" + r["givenyear"]) == detailModel.GivenYear
                    ).ToArray();
            }
            /* TODO comment 4 compile error
            string oneRow4OneMonth = WmsConfig.GetOrDefault("Inbound:OneRow4OneMonth", "0");
            detailModel.FirstInboundDate = Receipt.NormalizeDate(detailModel.FirstInboundDate);
            if (oneRow4OneMonth == "1")
            {
                ctnDtlRows = ctnDtlRows.Where(r
                    => r["first_in_date"].ToString() == detailModel.FirstInboundDate.ToString()
                    ).ToArray();
            }
            */


            if (ctnDtlRows != null && ctnDtlRows.Length < 1)
            {
                // 插入容器明细
                //detailModel.ItemInStatus = "ItemInOutStatus_InWaiting";
                //detailModel.QCStatus = "CheckState_CheckWaiting";
                detailModel.Qty = 0;
                detailModel.StatQty = 0;
                detailModel.InQty = qty;
                //detailModel.Ratio = DBHelper.GetDecimal(row["ratio"]);
                //detailModel.FirstInboundDate = DateTime.Now;
                detailModel.FirstInDate ??= DateTime.Now;
                Container.Insert(detailModel);
            }
            else
            {
                detailModel.ID = ctnDtlRows[0][cd.ID].ToString();
                detailModel.ContainerID = ctnDtlRows[0][cd.ContainerID].ToString();

                Container.AddDetailInQty(detailModel.ID,
                    qty, decimal.Parse("0" + ctnDtlRows[0][cd.InQty]));
            }

            return qty;
        }

        public void CancelPutawayPreparation(string containerDetailID, decimal qty)
        {
            DataRow ctnDtlRow = Container.GetDetail(containerDetailID);

            //decimal opQty = decimal.Parse("0" + taskRow["operation_qty"]);
            decimal opQty = qty;
            decimal inQty = decimal.Parse("0" + ctnDtlRow["in_qty"]);
            if (opQty > inQty)
            {
                throw new WmsException(152, "程序发生错误，无法继续处理");
            }

            decimal actualQty = decimal.Parse("0" + ctnDtlRow["qty"]);
            decimal outQty = decimal.Parse("0" + ctnDtlRow["out_qty"]);
            decimal lockQty = decimal.Parse("0" + ctnDtlRow["lock_qty"]);

            if (actualQty == 0 && inQty == opQty && outQty == 0 && lockQty == 0)
            {
                DeleteDetail(containerDetailID);
            }
            else
            {
                AddDetailInQty(containerDetailID, -opQty, inQty);
            }
        }

        public IContainer StockIn(string containerDetailID, decimal qty, decimal financeQty)
        {
            DataRow ctnDtlRow = Container.GetDetail(containerDetailID);

            StockIn(ctnDtlRow, qty, financeQty);

            return this;
        }

        //public IContainer StockIn(DataRow ctnDtlRow, decimal qty)
        public IContainer StockIn(DataRow ctnDtlRow, decimal qty, decimal financeQty)
        {
            string id = ctnDtlRow["t_id"].ToString();
            decimal actualQty = decimal.Parse("0" + ctnDtlRow["qty"]);
            decimal inQty = decimal.Parse("0" + ctnDtlRow["in_qty"]);

            Container
                .AddDetailQty(id, qty, actualQty)
                .AddDetailFinanceQty(id, financeQty, decimal.Parse("0" + ctnDtlRow["stat_qty"]))
                .AddDetailInQty(id, -qty, inQty);
            ctnDtlRow[cd.Qty] = actualQty + qty;

            DataRow ctnRow = GetHead(ctnDtlRow[cd.ContainerID].ToString());
            if (ctnRow[ch.ContainerType].ToString() == ContainerType.ContainerType_Actual.ToString()
                && string.IsNullOrWhiteSpace(ctnRow[ch.LocationID].ToString()))
            {
                return this;
            }
            DataRow locRow = Location.GetByContainer(ctnDtlRow["container_id"].ToString());
            decimal qtyLimit = decimal.Parse("0" + locRow["qty_limit"]);
            if (actualQty + qty >= qtyLimit)
            {
                Location.UpdateStatus(locRow["t_id"].ToString(),
                    LocationStatus.LocationStatus_Full.ToString(),
                    locRow[l.LocationStatus].ToString());
            }
            else
            {
                Location.UpdateStatus(locRow["t_id"].ToString(),
                    LocationStatus.LocationStatus_Partial.ToString(),
                    locRow["stock_status"].ToString());
            }

            return this;
        }

        public IContainer StockOut(DataRow ctnDtlRow, decimal qty, decimal statQty)
        {
            string id = ctnDtlRow["t_id"].ToString();
            decimal actualQty = decimal.Parse("0" + ctnDtlRow["qty"]);
            decimal outQty = decimal.Parse("0" + ctnDtlRow["out_qty"]);

            Container
                .AddDetailQty(id, -qty, actualQty)
                .AddDetailOutQty(id, -qty, outQty)
                .AddDetailFinanceQty(id, -statQty, decimal.Parse("0" + ctnDtlRow["stat_qty"]))
                .ClearDetail(id);

            DataRow locRow = Location.GetByContainer(ctnDtlRow[cd.ContainerID].ToString());
            DataRow[] ctnDetails = GetDetails(ctnDtlRow[cd.ContainerID].ToString());
            if (ctnDetails.Length <= 0)
            {
                Location.UpdateStatus(locRow[l.ID].ToString(),
                    LocationStatus.LocationStatus_Empty.ToString(), locRow[l.LocationStatus].ToString());
            }
            else
            {
                if (locRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                {
                    decimal sumOutQty = ctnDetails.Sum(r => decimal.Parse("0" + r["out_qty"]));
                    decimal sumQty = ctnDetails.Sum(r => decimal.Parse("0" + r["qty"]));
                    if (sumOutQty > 0
                        && locRow[l.LocationStatus].ToString() != LocationStatus.LocationStatus_Out.ToString())
                    {
                        Location.UpdateStatus(locRow[l.ID].ToString(),
                            LocationStatus.LocationStatus_Out.ToString(), locRow[l.LocationStatus].ToString());
                    }
                    else if (sumQty > 0
                        && locRow[l.LocationStatus].ToString() != LocationStatus.LocationStatus_Stored.ToString())
                    {
                        Location.UpdateStatus(locRow[l.ID].ToString(),
                            LocationStatus.LocationStatus_Stored.ToString(), locRow[l.LocationStatus].ToString());
                    }
                    else
                    {
                        Location.UpdateStatus(locRow[l.ID].ToString(),
                            LocationStatus.LocationStatus_Empty.ToString(), locRow[l.LocationStatus].ToString());
                    }
                }
                else
                {
                    decimal sumQty = ctnDetails.Sum(r => decimal.Parse("0" + r["qty"]));
                    decimal qtyLimit = decimal.Parse("0" + locRow[l.QtyLimit]);
                    if (locRow[l.QtyLimit].ToString() == LocationStatus.LocationStatus_Empty.ToString())
                    {
                        if (qtyLimit > 0 && sumQty > qtyLimit)
                        {
                            Location.UpdateStatus(locRow["t_id"].ToString(),
                                LocationStatus.LocationStatus_Full.ToString(),
                                locRow[l.LocationStatus].ToString());
                        }
                        else if (qtyLimit == 0 || sumQty < qtyLimit)
                        {
                            Location.UpdateStatus(locRow["t_id"].ToString(),
                                LocationStatus.LocationStatus_Partial.ToString(),
                                locRow[l.LocationStatus].ToString());
                        }
                    }
                    else if (locRow[l.QtyLimit].ToString() == LocationStatus.LocationStatus_Partial.ToString())
                    {
                        if (qtyLimit > 0 && sumQty > qtyLimit)
                        {
                            Location.UpdateStatus(locRow["t_id"].ToString(),
                                LocationStatus.LocationStatus_Full.ToString(),
                                locRow[l.LocationStatus].ToString());
                        }
                    }
                    else if (locRow[l.QtyLimit].ToString() == LocationStatus.LocationStatus_Full.ToString())
                    {
                        if (qtyLimit == 0 || sumQty < qtyLimit)
                        {
                            Location.UpdateStatus(locRow["t_id"].ToString(),
                                LocationStatus.LocationStatus_Partial.ToString(),
                                locRow[l.LocationStatus].ToString());
                        }
                    }
                }
            }

            return this;
        }

        public DataRow[] GetDetailsByLocation(string locationID, string itemID = null)
        {
            itemID ??= " ";
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "location_id", locationID },
                { "item_id", itemID },
            };
            string sql = @"
select * from &DataSource&wms_b_container_d cd
  left join &DataSource&wms_b_container_h ch on cd.container_id = ch.t_id
  left join &DataSource&wms_m_location l on ch.location_id = l.t_id
 where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&'
   and cd.warehouse_id = @warehouse 
   and l.t_id = @location_id
   and (cd.item_id = @item_id or @item_id = ' ') 
   and act_status = '1'
";

            DataTable dt = DBHelper.Select(sql, param, "查询容器明细", false);
            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetDetailsByLocation(string locNoOrID, string itemID,
                                string batchNo = null, string inboundID = null)
        {
            DataRow locRow = Location.Get(locNoOrID);
            DataRow containerRow = Container.GetHead(locRow["location_no"].ToString());
            DataRow[] ctnDtlRows = GetDetails(
                        containerRow["m_container_id"].ToString(),
                        itemID, batchNo, inboundID);
            return ctnDtlRows;
        }

        public DataRow[] GetInventoryDetailsByContainer(string containerID)
        {
            // 获取货位库存
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container_id", containerID },
            };

            string sql = @"
select * from (
select ch.t_id, ch.warehouse_id, ch.location_id, iv.item_id, iv.first_in_date, 
       iv.lot_no, container_barcode, container_d_id, '' task_no, lot_seq, ch.is_out,
       ifnull(iv.qty, 0) qty, ifnull(iv.in_qty, 0) in_qty, 
       ifnull(iv.out_qty, 0) out_qty, ifnull(iv.lock_qty, 0) lock_qty, 
       ifnull(iv.qty, 0) - ifnull(out_qty, 0) - ifnull(lock_qty, 0) avail_qty
  from &DataSource&wms_b_container_h ch
  left join (select container_id container_id, t_id container_d_id, 
                    item_id item_id, lot_no, lot_seq,
                    sum(qty) qty, sum(in_qty) in_qty, sum(out_qty) out_qty, 
                    sum(lock_qty) lock_qty, first_in_date, 
                    sum(ifnull(qty,0)) - sum(ifnull(out_qty,0)) - sum(ifnull(lock_qty,0)) avail_qty 
               from &DataSource&wms_b_container_d iv 
              group by container_id, t_id, item_id, 
                       lot_no, lot_seq, first_in_date) iv
         on ch.t_id = iv.container_id
 where ch.client_id='&CLIENT_ID&' and ch.org_id='&ORG_ID&' 
   and ch.warehouse_id = @warehouse 
   and ch.t_id = @container_id 
) t
   where t.avail_qty > 0 
";
            return DBHelper.Select(sql, param, "查询容器库存", false)?.AsEnumerable().ToArray();
        }

        public DataRow[] GetInventoryDetailsByLocation(string locationID)
        {
            // 获取货位库存
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "loc_id", locationID },
            };

            string sql = @"
select * from (
select ch.t_id, ch.warehouse_id, ch.location_id, iv.item_id, iv.first_in_date, 
       iv.spec, iv.owner_lot_no, iv.lot_seq, ch.is_out,
       iv.lot_no, container_barcode, container_d_id, '' task_no, 
       ifnull(iv.qty, 0) qty, ifnull(iv.in_qty, 0) in_qty, 
       ifnull(iv.out_qty, 0) out_qty, ifnull(iv.lock_qty, 0) lock_qty, 
       ifnull(iv.qty, 0) - ifnull(out_qty, 0) - ifnull(lock_qty, 0) avail_qty
  from &DataSource&wms_b_container_h ch
  left join (select container_id container_id, t_id container_d_id, 
                    item_id item_id, spec, owner_lot_no, lot_no, lot_seq,
                    sum(qty) qty, sum(in_qty) in_qty, sum(out_qty) out_qty, 
                    sum(lock_qty) lock_qty, first_in_date, 
                    sum(ifnull(qty,0)) - sum(ifnull(out_qty,0)) - sum(ifnull(lock_qty,0)) avail_qty 
               from &DataSource&wms_b_container_d iv 
              where iv.act_status = '1'
              group by container_id, t_id, item_id, spec, owner_lot_no,
                       lot_no, lot_seq, first_in_date) iv
         on ch.t_id = iv.container_id
 where ch.client_id='&CLIENT_ID&' and ch.org_id='&ORG_ID&' 
   and ch.warehouse_id = @warehouse 
   and ch.location_id = @loc_id 
   and ch.act_status = '1'
) t
   where t.avail_qty > 0 
";
            return DBHelper.Select(sql, param, "查询容器库存", false)?.AsEnumerable().ToArray();
        }

        public DataRow[] GetInventoryDetails(DataRow locRow)
        {
            DataRow[] inventoryRows;
            DataRow[] ctnRows = Container.GetHeadsByLocation(locRow[l.ID].ToString());
            if (ctnRows.Length > 1)
            {
                decimal actualNum = ctnRows.Count(r =>
                    r[ch.ContainerType].ToString() == ContainerType.ContainerType_Actual.ToString());
                decimal virtualNum = ctnRows.Count(r =>
                    r[ch.ContainerType].ToString() == ContainerType.ContainerType_Virtual.ToString());
                if (actualNum >= 1 && virtualNum >= 1)
                {
                    throw new WmsException(416, $"货位{locRow["location_no"]}错误！");
                }
                if (virtualNum >= 1)
                {
                    throw new WmsException(416, $"货位{locRow["location_no"]}错误！多于1个虚拟容器");
                }
                actualNum = ctnRows.Count(r =>
                    r[ch.ContainerType].ToString() == ContainerType.ContainerType_Actual.ToString()
                    && r[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString());
                if (actualNum >= 1)
                {
                    throw new WmsException(416, $"货位{locRow["location_no"]}错误！立库货位多于1个托盘");
                }
            }
            else if (ctnRows.Length <= 0)
            {
                throw new WmsException(416, $"货位{locRow["location_no"]}找不到容器！");
            }

            if (ctnRows[0][l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
            {
                inventoryRows = Container.GetInventoryDetailsByContainer(ctnRows[0][ch.ID].ToString());
            }
            else
            {
                inventoryRows = Container.GetInventoryDetailsByLocation(locRow[l.ID].ToString());
            }

            return inventoryRows;
        }

        public DataRow[] GetInventoryDetails(string locationID,
            string itemID = null, string batchNo = null)
        {
            itemID ??= " ";
            batchNo ??= " ";

            // 获取货位库存
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "loc_id", locationID },
                { "item_id", itemID },
                { "lot_no", batchNo },
            };

            string sql = @"
select * from (
select ch.t_id, ch.warehouse_id, ch.location_id, iv.item_id, iv.first_in_date, 
       iv.lot_no, container_barcode, container_d_id, '' task_no, 
       ifnull(iv.qty, 0) qty, ifnull(iv.in_qty, 0) in_qty, 
       ifnull(iv.out_qty, 0) out_qty, ifnull(iv.lock_qty, 0) lock_qty, 
       ifnull(iv.qty, 0) - ifnull(out_qty, 0) - ifnull(lock_qty, 0) avail_qty
  from &DataSource&wms_b_container_h ch
  left join (select container_id container_id, t_id container_d_id, 
                    item_id item_id, lot_no,
                    sum(qty) qty, sum(in_qty) in_qty, sum(out_qty) out_qty, 
                    sum(lock_qty) lock_qty, first_in_date, 
                    sum(ifnull(qty,0)) - sum(ifnull(out_qty,0)) - sum(ifnull(lock_qty,0)) avail_qty 
               from &DataSource&wms_b_container_d iv 
              where iv.act_status = '1'
              group by container_id, t_id, item_id, 
                       lot_no, first_in_date) iv
         on ch.t_id = iv.container_id
 where 1=1
   and ch.warehouse_id = @warehouse 
   and ch.location_id = @loc_id 
   and ch.act_status = '1'
   and (@item_id = ' ' or iv.item_id = @item_id)
) t
   where t.avail_qty > 0 
";
            //where ch.client_id='&CLIENT_ID&' and ch.org_id='&ORG_ID&' 
            //and (@lot_no = ' ' or iv.lot_no = @lot_no)
            return DBHelper.Select(sql, param, "查询容器库存", false)?.AsEnumerable().ToArray();
        }

        public DataRow[] GetInventoryDetails_v2(string locationID, string itemID = null)
        {
            itemID = itemID ?? " ";
            // 获取货位库存
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "loc_id", locationID } ,
                { "item_id", itemID } ,
            };

            string sql = @"
select * from (
select ch.t_id, ch.warehouse_id, ch.location_id, iv.item_id, iv.first_in_date, 
       '' batch_no, container_barcode, container_d_id, '' task_no, 
       ifnull(iv.qty, 0) qty, ifnull(iv.in_qty, 0) in_qty, 
       ifnull(iv.out_qty, 0) out_qty, ifnull(iv.lock_qty, 0) lock_qty, 
       ifnull(iv.qty, 0) - ifnull(out_qty, 0) - ifnull(lock_qty, 0) avail_qty
  from &DataSource&wms_b_container_h ch
  left join (select container_id container_id, t_id container_d_id, item_id item_id, 
                    sum(qty) qty, sum(in_qty) in_qty, sum(out_qty) out_qty, 
                    sum(lock_qty) lock_qty, first_in_date, 
                    sum(ifnull(qty,0)) - sum(ifnull(out_qty,0)) - sum(ifnull(lock_qty,0)) avail_qty 
               from &DataSource&wms_b_container_d iv 
              group by container_id, t_id, item_id, first_in_date) iv
         on ch.t_id = iv.container_id
 where ch.client_id='&CLIENT_ID&' and ch.org_id='&ORG_ID&' 
   and ch.warehouse_id = @warehouse 
   and ch.location_id = @loc_id 
   and (@item_id = ' ' or iv.item_id = @item_id)
) t
   where t.avail_qty > 0 
";
            return DBHelper.Select(sql, param, "查询容器库存", false)?.AsEnumerable().ToArray();
        }

        public DataRow[] GetInventoryDetails_V1(string locationID, string itemID = null)
        {
            itemID = itemID ?? " ";
            // 获取货位库存
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "loc_id", locationID } ,
                { "item_id", itemID } ,
            };

            string sql = @"
select * from (
select ch.t_id, ch.warehouse_id, ch.location_id, iv.item_id, cd.first_in_date, 
       '' batch_no, container_barcode, container_d_id, '' task_no, 
       ifnull(iv.qty, 0) qty, ifnull(iv.in_qty, 0) in_qty, 
       ifnull(iv.out_qty, 0) out_qty, ifnull(iv.lock_qty, 0) lock_qty, 
       ifnull(iv.qty, 0) - ifnull(out_qty, 0) - ifnull(lock_qty, 0) avail_qty
  from &DataSource&wms_b_container_h ch
  left join isa_inv_container iv on ch.t_id = iv.container_id
  left join (select container_id, min(cd.first_in_date) first_in_date, 
                    min(t_id) container_d_id
               from &DataSource&wms_b_container_d cd group by container_id) cd
         on cd.container_id = iv.container_id
 where ch.client_id='&CLIENT_ID&' and ch.org_id='&ORG_ID&' 
   and ch.warehouse_id = @warehouse 
   and ch.location_id = @loc_id 
   and (@item_id = ' ' or iv.item_id = @item_id)
) t
   where t.avail_qty > 0 
";
            return DBHelper.Select(sql, param, "查询容器库存", false)?.AsEnumerable().Select(r => r).ToArray();
        }

        public decimal GetInventory(string containerID, string itemID)
        {
            throw new NotImplementedException();
        }

        public decimal GetInventoryByLocation(string locationID, string itemID)
        {
            DataRow[] invRows = GetInventoryDetails(locationID, itemID);

            decimal qty = invRows.Sum(r => decimal.Parse("0" + r["avail_qty"]));

            return qty;
        }

        public IContainer InsertHeadbak(ContainerHeadModel model)
        {
            throw new NotImplementedException();
        }

        public IContainer InsertDetailbak(ContainerDetailModel model)
        {
            throw new NotImplementedException();
        }

        public IContainer UpdateDetailCountingState(string containerDetailID, string countingState)
        {
            throw new NotImplementedException();
        }

        public IContainer UpdateDetailCountingStateByContainerID(string containerID, string countingState)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 修改容器明细物料数量
        /// </summary>
        /// <param name="containerDetailID">容器明细id</param>
        /// <param name="qty">物料数量</param>
        /// <returns></returns>
        public IContainer UpdateDetailQty(string containerDetailID, decimal qty)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "qty", qty.ToString() },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", containerDetailID }
            };
            //统计库存量STAT_QTY=物流库存量QTY*转换比率RATIO
            string sql = "update &DataSource&wms_b_container_d d set d.qty = @qty,d.stat_qty = @qty*d.ratio ,d.last_modified_by = @last_modified_by,d.modify_ip = @modify_ip,d.modify_date = sysdate  where d.t_id = @t_id";
            DBHelper.RunSql(sql, param, "修改容器明细中物料数量");

            return this;
        }

        /// <summary>
        /// 修改容器明细待出量
        /// </summary>
        /// <param name="containerDetailID">容器明细id</param>
        /// <param name="outQty">待出量</param>
        /// <returns></returns>
        public IContainer UpdateDetailOutQty(string containerDetailID, decimal outQty)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "outQty", outQty.ToString() },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", containerDetailID }
            };
            string sql = "update &DataSource&wms_b_container_d d set d.out_qty = @outQty ,d.last_modified_by = @last_modified_by,d.modify_ip = @modify_ip,d.modify_date = sysdate  where d.t_id = @t_id";
            DBHelper.RunSql(sql, param, "修改容器明细中待出物料数量");

            return this;
        }

        /// <summary>
        /// 修改容器明细待入量
        /// </summary>
        /// <param name="containerDetailID">容器明细id</param>
        /// <param name="inQty">待入量</param>
        /// <returns></returns>
        public IContainer UpdateDetailInQty(string containerDetailID, decimal inQty)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "inQty", inQty.ToString() },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", containerDetailID }
            };
            string sql = "update &DataSource&wms_b_container_d d set d.in_qty = @inQty ,d.last_modified_by = @last_modified_by,d.modify_ip = @modify_ip,d.modify_date = sysdate  where d.t_id = @t_id";
            DBHelper.RunSql(sql, param, "修改容器明细中待入物料数量");

            return this;
        }

        /// <summary>
        /// 修改容器明细质检状态
        /// </summary>
        /// <param name="containerDetailID">容器明细id</param>
        /// <param name="qcStatus">质检状态</param>
        /// <returns></returns>
        public IContainer UpdateDetailQCStatus(string containerDetailID, string qcStatus)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "qcStatus", qcStatus },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", containerDetailID }
            };
            string sql = "update &DataSource&wms_b_container_d d set d.qc_status = @qcStatus ,d.last_modified_by = @last_modified_by,d.modify_ip = @modify_ip,d.modify_date = sysdate  where d.t_id = @t_id";
            DBHelper.RunSql(sql, param, "修改容器明细质检状态");

            return this;
        }

        /// <summary>
        /// 修改容器主档是否允许呼叫
        /// </summary>
        /// <param name="containerHeadID">容器id</param>
        /// <param name="isAllowCalled">是否允许呼叫，0：不允许，1：允许</param>
        /// <returns></returns>
        public IContainer UpdateHeadIsAllowCalled(string containerHeadID, string isAllowCalled)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "isAllowCalled", isAllowCalled },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", containerHeadID }
            };
            string sql = "update &DataSource&wms_b_container_h h set h.allow_called = @isAllowCalled ,h.last_modified_by = @last_modified_by,h.modify_ip = @modify_ip,h.modify_date = sysdate  where h.t_id = @t_id";
            DBHelper.RunSql(sql, param, "修改容器主档是否允许呼叫");

            return this;
        }

        public IContainer UpdateDetail(DataRow row)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", row["t_id"].ToString() },
                { "container_id", row["container_id"].ToString() },
                { "inbound_h_id", row["inbound_h_id"].ToString() },
                { "outbound_h_id", row["outbound_h_id"].ToString() },
                { "sub_container_no", row["sub_container_no"].ToString() },
                { "supplier_id", row["supplier_id"].ToString() },
                { "bom_id", row["bom_id"].ToString() },
                { "owner_id", row["owner_id"].ToString() },
                { "item_id", row["item_id"].ToString() },
                { "item_name", row["item_name"].ToString() },
                { "lot_no", row["lot_no"].ToString() },
                { "owner_lot_no", row["owner_lot_no"].ToString() },
                { "wms_lot_no", row["wms_lot_no"].ToString() },
                { "exist_barcode", row["exist_barcode"].ToString() },
                { "is_full", row["is_full"].ToString() },
                { "given_year", row["given_year"].ToString() },
                { "sub_factory", row["sub_factory"].ToString() },
                { "sub_inventory", row["sub_inventory"].ToString() },
                { "sub_location", row["sub_location"].ToString() },
                { "qc_status", row["qc_status"].ToString() },
                { "qty", row["qty"].ToString() },
                { "stat_qty", row["stat_qty"].ToString() },
                { "in_qty", row["in_qty"].ToString() },
                { "out_qty", row["out_qty"].ToString() },
                { "lock_qty", row["lock_qty"].ToString() },
                { "spec", row["spec"].ToString() },
                { "uom_qty_id", row["uom_qty_id"].ToString() },
                { "uom_stat_id", row["uom_stat_id"].ToString() },
                { "ratio", row["ratio"].ToString() },
                { "first_in_date", row["first_in_date"].ToString() },
                { "start_out_date", row["start_out_date"].ToString() },
                { "production_date", row["production_date"].ToString() },
                { "exp_date", row["exp_date"].ToString() },
                { "warning_date", row["warning_date"].ToString() },
                { "first_out_date", row["first_out_date"].ToString() },
                { "out_end_date", row["out_end_date"].ToString() },
                { "use_by_date", row["use_by_date"].ToString() },
                { "act_status", row["act_status"].ToString() }
            };
            string sql = @"
update &DataSource&wms_b_container_d 
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       container_id= @container_id, 
       inbound_h_id= @inbound_h_id, 
       outbound_h_id= @outbound_h_id, 
       sub_container_no= @sub_container_no, 
       supplier_id= @supplier_id, 
       bom_id= @bom_id, 
       owner_id= @owner_id, 
       item_id= @item_id, 
       item_name= @item_name, 
       lot_no= @lot_no, 
       owner_lot_no= @owner_lot_no, 
       wms_lot_no= @wms_lot_no, 
       exist_barcode= @exist_barcode, 
       is_full= @is_full, 
       given_year= @given_year, 
       sub_factory= @sub_factory, 
       sub_inventory= @sub_inventory, 
       sub_location= @sub_location, 
       qc_status= @qc_status, 
       qty= @qty, 
       stat_qty= @stat_qty, 
       in_qty= @in_qty, 
       out_qty= @out_qty, 
       lock_qty= @lock_qty, 
       spec= @spec, 
       uom_qty_id= @uom_qty_id, 
       uom_stat_id= @uom_stat_id, 
       ratio= @ratio, 
       first_in_date= to_date(@first_in_date, 'yyyy-MM-dd HH24:mi:ss'), 
       start_out_date= to_date(@start_out_date, 'yyyy-MM-dd HH24:mi:ss'), 
       production_date= to_date(@production_date, 'yyyy-MM-dd HH24:mi:ss'), 
       exp_date= to_date(@exp_date, 'yyyy-MM-dd HH24:mi:ss'), 
       warning_date= to_date(@warning_date, 'yyyy-MM-dd HH24:mi:ss'), 
       first_out_date= to_date(@first_out_date, 'yyyy-MM-dd HH24:mi:ss'), 
       out_end_date= to_date(@out_end_date, 'yyyy-MM-dd HH24:mi:ss'), 
       use_by_date= to_date(@use_by_date, 'yyyy-MM-dd HH24:mi:ss'), 
       act_status= @act_status 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id
";
            DBHelper.RunSql(sql, param, "修改容器库存表");

            return this;
        }

        public IContainer UpdateHeadActStatus(string id, string actStatus, string oldActStatus = null)
        {
            oldActStatus ??= " ";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "state", actStatus},
                { "old_state", oldActStatus},
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_b_container_h  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       act_status = @state
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id
   and (act_status = @old_state or @old_state = ' ')
";
            DBHelper.RunSql(sql, param, "更新容器状态");
            return this;
        }

        /// <summary>
        /// 修改容器明细删除标记
        /// </summary>
        /// <param name="detailID">容器明细id</param>
        /// <param name="actStatus">删除标记（1表示不删除；0表示删除）</param>
        /// <returns></returns>
        public IContainer UpdateDetailActStatus(string detailID, string actStatus)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "actStatus", actStatus },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", detailID }
            };
            string sql = "update &DataSource&wms_b_container_d d set d.act_status = @actStatus ,d.last_modified_by = @last_modified_by,d.modify_ip = @modify_ip,d.modify_date = sysdate  where d.t_id = @t_id";
            DBHelper.RunSql(sql, param, "修改容器明细删除状态");

            return this;
        }

        /// <summary>
        /// 修改容器明细首次入库时间
        /// </summary>
        /// <param name="headID">容器主档id</param>
        /// <param name="dt">首次入库时间</param>
        /// <returns></returns>
        public IContainer UpdateDetailFirstInDate(string headID,DateTime dt)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "first_in_date", FormatDatetimeForSql(dt) },
                { "headID", headID }
            };
            string sql = "update &DataSource&wms_b_container_d d set d.first_in_date = to_date(@first_in_date,'yyyy-mm-dd hh24:mi:ss') ,d.last_modified_by = @last_modified_by,d.modify_ip = @modify_ip,d.modify_date = sysdate  where d.container_id = @headID";
            DBHelper.RunSql(sql, param, "修改容器明细首次入库时间",DBHelper.OperatedNumType.OneOrMore);

            return this;
        }

        /// <summary>
        /// 修改容器主档首次入库时间
        /// </summary>
        /// <param name="headID">容器主档id</param>
        /// <param name="dt">首次入库时间</param>
        /// <returns></returns>
        public IContainer UpdateContainerHFirstInDate(string headID, DateTime dt)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "first_in_date", FormatDatetimeForSql(dt) },
                { "t_id", headID }
            };
            string sql = "update &DataSource&wms_b_container_h d set d.first_in_date = to_date(@first_in_date,'yyyy-mm-dd hh24:mi:ss') ,d.last_modified_by = @last_modified_by,d.modify_ip = @modify_ip,d.modify_date = sysdate  where d.t_id = @t_id";
            DBHelper.RunSql(sql, param, "修改容器主档首次入库时间");

            return this;
        }
        public IContainer UpdateHeadIsOut(string id, string value, string oldValue = null)
        {
            oldValue ??= " ";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "is_out", value},
                { "old_is_out", oldValue},
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_b_container_h  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       is_out = @is_out
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id
   and (is_out = @old_is_out or @old_is_out = ' ')
";
            DBHelper.RunSql(sql, param, "更新容器状态");
            return this;
        }


        public IContainer UpdateDetailOwnership(string containerID, string shipperID, string orderNo)
        {
            throw new NotImplementedException();
        }

        public IContainer UpdateHeadCheckState(string headID, string checkState)
        {
            throw new NotImplementedException();
        }

        public IContainer UpdateHeadCountingState(string headID, string countingState)
        {
            throw new NotImplementedException();
        }

        IContainer IContainer.UpdateHeadLocation(string headID, string locationID, string oldID)
        {
            throw new NotImplementedException();
        }

        public IContainer TakeItemOut(string containerID, string itemBarcodes)
        {
            throw new NotImplementedException();
        }

        public IContainer Prepare4StockingOut(string containerDetailID, decimal qty)
        {
            DataRow detailRow = GetDetail(containerDetailID);

            decimal actualQty = decimal.Parse("0" + detailRow["qty"]);
            decimal outQty = decimal.Parse("0" + detailRow["out_qty"]);
            decimal availQty = actualQty - outQty;
            //decimal lockQty = decimal.Parse("0" + detailRow["lock_qty"]);

            if (availQty < qty)
            {
                throw new WmsException(240, "库存数量不足，程序无法处理!");
            }

            AddDetailOutQty(containerDetailID, qty, outQty);

            return this;
        }

        public IContainer Prepare4StockingOut(string locationID, ContainerDetailModel detail)
        {
            DataRow ctnDtlRow = GetDetailByLocation(locationID,
                detail.ItemID, detail.LotNo, detail.InboundHeadID);

            decimal qty = detail.OutQty ?? 0;
            if (qty <= 0)
            {
                throw new WmsException(241, $"不正确的出库数量{qty}");
            }
            if (detail.ItemID != ctnDtlRow["item_id"].ToString())
            {
                throw new WmsException(242, $"货位上的货品与指定的货品不一致");
            }
            detail.ID = ctnDtlRow["t_id"].ToString();
            detail.ContainerID = ctnDtlRow["container_id"].ToString();

            return Prepare4StockingOut(ctnDtlRow["t_id"].ToString(), qty);
        }

        /*
        public IContainer Prepare4StockingOut(string locationID, ContainerDetailModel detail)
        {
            DataRow[] ctnDtlRows = GetDetailsByLocation(locationID,
                detail.ItemID, detail.ExternalLotNo, detail.InboundHeadID);

            decimal qty = detail.OutQty ?? 0;
            if (qty <= 0)
            {
                throw new WmsException(241, $"不正确的出库数量{qty}");
            }
            if (detail.ItemID != ctnDtlRow["item_id"].ToString())
            {
                throw new WmsException(242, $"货位上的货品与指定的货品不一致");
            }
            detail.ID = ctnDtlRow["t_id"].ToString();

            return Prepare4StockingOut(ctnDtlRow["t_id"].ToString(), qty);
        }
        */

        public DataRow[] Get4StockingOut(ContainerDetailModel detailModel)
        {
            DataRow[] ctnDetails = Container.GetDetails(detailModel.ContainerID);

            DataRow[] ctnDtlRows = null;
            ctnDtlRows = ctnDetails.Where(r
                    => r["item_id"].ToString() == detailModel.ItemID
            ).ToArray();

            string OneRow4OnePartialPacket = WmsConfig.GetOrDefault("Outbound:OneRow4OnePartialPacket", "0");
            detailModel.IsFull ??= "";
            if (OneRow4OnePartialPacket == "1")
            {
                if (detailModel.IsFull == "0")
                {
                    ctnDtlRows = ctnDtlRows.Where(
                        r => r["is_full"].ToString() == "0"
                            && decimal.Parse("0" + r["stat_qty"]) == detailModel.StatQty).ToArray();
                }
                else
                {
                    ctnDtlRows = ctnDtlRows.Where(r => r["is_full"].ToString() != "0").ToArray();
                }
            }

            string oneRow4OneReceipt = WmsConfig.GetOrDefault("Outbound:OneRow4OneReceipt", "0");
            detailModel.InboundHeadID ??= "";
            if (oneRow4OneReceipt == "1")
            {
                ctnDtlRows = ctnDtlRows.Where(r
                    => r["inbound_h_id"].ToString() == detailModel.InboundHeadID
                    ).ToArray();
            }
            string oneRow4OneBatch = WmsConfig.GetOrDefault("Outbound:OneRow4OneBatch", "0");
            detailModel.LotNo ??= "";
            if (oneRow4OneBatch == "1")
            {
                ctnDtlRows = ctnDtlRows.Where(r
                    => r["lot_no"].ToString() == detailModel.LotNo
                    ).ToArray();
            }
            string oneRow4OneYear = WmsConfig.GetOrDefault("Outbound:OneRow4OneYear", "0");
            detailModel.GivenYear ??= 0;
            if (oneRow4OneYear == "1")
            {
                ctnDtlRows = ctnDtlRows.Where(r
                    => decimal.Parse("0" + r["givenyear"]) == detailModel.GivenYear
                    ).ToArray();
            }

            return ctnDtlRows;
        }

        public IContainer CancelStockingOut(string containerDetailID, decimal qty)
        {
            DataRow detailRow = GetDetail(containerDetailID);

            decimal actualQty = decimal.Parse("0" + detailRow["qty"]);
            decimal outQty = decimal.Parse("0" + detailRow["out_qty"]);
            decimal inQty = decimal.Parse("0" + detailRow["in_qty"]);
            decimal lockQty = decimal.Parse("0" + detailRow["lock_qty"]);

            if (outQty < qty)
            {
                throw new WmsException(261, "待出数量不足，程序无法处理!");
            }
            outQty -= qty;
            // 此处的逻辑有误，这个if的条件和后面if的条件似乎不匹配，但不会导致严重问题。
            if (actualQty < outQty)
            {
                LogHelper.LogError($"ContainerDetailID=[{containerDetailID}]");
                throw new WmsException(260, "库存数据错误，程序无法处理!");
            }

            if (actualQty == 0 && outQty == 0 && inQty == 0 && lockQty == 0)
            {
                DeleteDetail(containerDetailID);
                CheckAndSetLocation(detailRow["container_id"].ToString());
            }
            else
            {
                AddDetailOutQty(containerDetailID, -qty, decimal.Parse("0" + detailRow["out_qty"]));
            }

            return this;
        }

        public IContainer TakeOff(string containerID, string itemID, decimal qty)
        {
            bool clearLoc = true;
            DataRow[] detailRows = GetDetails(containerID, itemID);
            decimal actualQty = detailRows.Sum(r => decimal.Parse("0" + r["qty"]));
            decimal outQty = detailRows.Sum(r => decimal.Parse("0" + r["out_qty"]));
            decimal inQty = detailRows.Sum(r => decimal.Parse("0" + r["in_qty"]));
            decimal lockQty = detailRows.Sum(r => decimal.Parse("0" + r["lock_qty"]));

            if (outQty < qty)
            {
                throw new WmsException(261, "待出数量不足，程序无法处理!");
            }
            if (actualQty < outQty)
            {
                LogHelper.LogError($"ContainerID=[{containerID}] ItemID=[{itemID}]");
                throw new WmsException(260, "库存数据错误，程序无法处理!");
            }

            decimal leftQty = qty;
            foreach (DataRow row in detailRows)
            {
                if (row["item_id"].ToString() != itemID)
                {
                    clearLoc = false;
                    continue;
                }
                actualQty = decimal.Parse("0" + row["qty"]);
                outQty = decimal.Parse("0" + row["out_qty"]);
                inQty = decimal.Parse("0" + row["in_qty"]);
                lockQty = decimal.Parse("0" + row["lock_qty"]);

                if (actualQty < outQty)
                {
                    LogHelper.LogError($"ContainerID=[{containerID}] ItemID=[{itemID}]");
                    throw new WmsException(260, "库存数据错误，程序无法处理!");
                }

                decimal availQty = actualQty - lockQty;
                if (availQty > outQty)
                {
                    availQty = outQty;
                }
                if (availQty == 0)
                {
                    clearLoc = false;
                    continue;
                }
                leftQty -= availQty;

                if (actualQty == availQty && inQty == 0 && outQty == availQty && lockQty == 0)
                {
                    Container.DeleteDetail(row["t_id"].ToString());
                }
                else
                {
                    clearLoc = false;
                    Container.AddDetailInQty(row["t_id"].ToString(), -qty, outQty);
                }
            }
            if (leftQty > 0)
            {
                throw new WmsException(211, "货位上库存不足，程序停止处理!");
            }
            if (clearLoc)
            {
                CheckAndSetLocation(containerID);
            }

            return this;
        }

        private void CheckAndSetLocation(string containerID)
        {
            DataRow[] rows = GetDetails(containerID);
            if (rows.Length > 0)
            {
                return;
            }

            DataRow ctnRow = GetHead(containerID);

            Location.UpdateStatus(ctnRow["location_id"].ToString(),
                "StockStatus_Empty", ctnRow["stock_status"].ToString());
        }

        public IContainer AddContainerOutQty(string containerID, decimal outQty, decimal? oldQty = null)
        {
            throw new NotImplementedException();
        }

        public bool needTakeFullPalletOut(string locType,
            string defaultLoc, decimal qty, out string packingType)
        {
            //List<string> packingTypeList = getPackingType(locType, defaultLoc);
            packingType = "";
            return true;
        }

        private string FormatDatetimeForSql(DateTime? dt)
        {
            if (dt == null)
            {
                return "";
            }
            else
            {
                return ((DateTime)dt).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public WebJsonInfo AddContainerD(IDBContextProvider dbW, UserLoginInfo user, ContainerDModel ctndetail)
        {
            WebJsonInfo re = new WebJsonInfo();

            if (string.IsNullOrEmpty(ctndetail.t_id))
            {
                ctndetail.t_id = Guid.NewGuid().ToString();
            }
            string sql = @"insert into &DataSource&wms_b_container_d
                            (t_id, client_id, org_id, warehouse_id, container_id, inbound_h_id, outbound_h_id, 
                            sub_container_no, supplier_id, bom_id, owner_id, item_id, item_name, lot_no, lot_seq,
                            owner_lot_no, wms_lot_no, exist_barcode, is_full, given_year, sub_factory, sub_inventory, 
                            sub_location, qc_status, qty, stat_qty, in_qty, out_qty, lock_qty, spec, uom_qty_id, 
                            uom_stat_id, ratio, first_in_date, start_out_date, production_date, exp_date, 
                            warning_date, first_out_date, out_end_date, use_by_date, create_date, created_by, 
                            modify_ip, modify_date, last_modified_by, act_status)
                            values
                            ('" + ctndetail.t_id + "', '" + ctndetail.client_id + "', '" + ctndetail.org_id + "', '" + ctndetail.warehouse_id + "'," +
                            " '" + ctndetail.container_id + "', '" + ctndetail.inbound_h_id + "', '" + ctndetail.outbound_h_id + "', " +
                            "'" + ctndetail.sub_container_no + "', '" + ctndetail.supplier_id + "', '" + ctndetail.bom_id + "', '" + ctndetail.owner_id +
                            "', '" + ctndetail.item_id + "', '" + ctndetail.item_name + "', '" + ctndetail.lot_no + "', '" + ctndetail.lot_seq + "','" + ctndetail.owner_lot_no +
                            "','" + ctndetail.wms_lot_no + "', '" + ctndetail.exist_barcode + "', '" + ctndetail.is_full + "', '" + ctndetail.given_year +
                            "', '" + ctndetail.sub_factory + "', '" + ctndetail.sub_inventory + "', '" + ctndetail.sub_location +
                            "', '" + ctndetail.qc_status + "', '" + ctndetail.qty + "', '" + ctndetail.stat_qty + "', '" + ctndetail.in_qty +
                            "', '" + ctndetail.out_qty + "', '" + ctndetail.lock_qty + "', '" + ctndetail.spec + "', '" + ctndetail.uom_qty_id +
                            "', '" + ctndetail.uom_stat_id + "', '" + ctndetail.ratio +
                            "', to_date('" + FormatDatetimeForSql(ctndetail.first_in_date) + "','yyyy-mm-dd hh24:mi:ss')" +
                            ", to_date('" + FormatDatetimeForSql(ctndetail.start_out_date) + "','yyyy-mm-dd hh24:mi:ss')" +
                            ", to_date('" + FormatDatetimeForSql(ctndetail.production_date) + "','yyyy-mm-dd hh24:mi:ss')" +
                            ",to_date('" + FormatDatetimeForSql(ctndetail.exp_date) + "','yyyy-mm-dd hh24:mi:ss')" +
                            ", to_date('" + FormatDatetimeForSql(ctndetail.warning_date) + "','yyyy-mm-dd hh24:mi:ss')" +
                            ",to_date('" + FormatDatetimeForSql(ctndetail.first_out_date) + "','yyyy-mm-dd hh24:mi:ss')" +
                            ", to_date('" + FormatDatetimeForSql(ctndetail.out_end_date) + "','yyyy-mm-dd hh24:mi:ss')" +
                            ", to_date('" + FormatDatetimeForSql(ctndetail.use_by_date) + "','yyyy-mm-dd hh24:mi:ss')" +
                            ", sysdate, '" + ctndetail.created_by + "', '" + ctndetail.modify_ip +
                            "', sysdate, '" + ctndetail.last_modified_by + "', '1')";
            int rows;
            string msg = "";
            List<string> lstSql = new List<string>();

            lstSql.Add(DBHelper.ConvertSql(sql));
            dbW.DBRunSql(user, lstSql, out rows, out msg);
            re.isSucceed = rows > 0;
            re.errmsg = msg;
            re.json = ctndetail.t_id;
            return re;

        }


        public bool AddContainerD(ContainerDModel ctndetail)
        {
            string sql = @" insert into &DataSource&wms_b_container_d
               (t_id, client_id, org_id, warehouse_id, container_id, inbound_h_id, outbound_h_id, sub_container_no, supplier_id, bom_id, 
                owner_id, item_id, item_name, lot_no,lot_seq, owner_lot_no, wms_lot_no, exist_barcode, is_full, given_year, sub_factory, 
                sub_inventory, sub_location, qc_status, qty, stat_qty, in_qty, out_qty, lock_qty, spec, uom_qty_id, 
                uom_stat_id, ratio, first_in_date, start_out_date, production_date, exp_date, warning_date, first_out_date, out_end_date, use_by_date, 
                create_date, created_by, modify_ip, modify_date, last_modified_by, act_status)
             values
               (@t_id, @client_id, @org_id, @warehouse_id, @container_id, @inbound_h_id, @outbound_h_id, @sub_container_no, @supplier_id, @bom_id, 
                @owner_id, @item_id, @item_name, @lot_no, @lot_seq, @owner_lot_no, @wms_lot_no, @exist_barcode, @is_full, @given_year, @sub_factory, 
                @sub_inventory, @sub_location, @qc_status, @qty, @stat_qty, @in_qty, @out_qty, @lock_qty, @spec, @uom_qty_id, 
                @uom_stat_id, @ratio, 
                to_date(@first_in_date,'yyyy-mm-dd hh24:mi:ss'), to_date(@start_out_date,'yyyy-mm-dd hh24:mi:ss') ,
                to_date(@production_date,'yyyy-mm-dd hh24:mi:ss') ,to_date(@exp_date,'yyyy-mm-dd hh24:mi:ss'), 
                to_date(@warning_date,'yyyy-mm-dd hh24:mi:ss'), to_date(@first_out_date,'yyyy-mm-dd hh24:mi:ss'), 
                to_date(@out_end_date,'yyyy-mm-dd hh24:mi:ss'),to_date(@use_by_date,'yyyy-mm-dd hh24:mi:ss'), 
                sysdate, @created_by, @modify_ip, sysdate, @last_modified_by, '1')";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", ctndetail.t_id);
            lstParam.Add("client_id", ctndetail.client_id);
            lstParam.Add("org_id", ctndetail.org_id);
            lstParam.Add("warehouse_id", ctndetail.warehouse_id);
            lstParam.Add("container_id", ctndetail.container_id);
            lstParam.Add("outbound_h_id", ctndetail.outbound_h_id);
            lstParam.Add("inbound_h_id", ctndetail.inbound_h_id);
            lstParam.Add("sub_container_no", ctndetail.sub_container_no);
            lstParam.Add("supplier_id", ctndetail.supplier_id);
            lstParam.Add("bom_id", ctndetail.bom_id);

            lstParam.Add("owner_id", ctndetail.owner_id);
            lstParam.Add("item_id", ctndetail.item_id);
            lstParam.Add("item_name", ctndetail.item_name);
            lstParam.Add("lot_no", ctndetail.lot_no);
            lstParam.Add("lot_seq", ctndetail.lot_seq);
            lstParam.Add("owner_lot_no", ctndetail.owner_lot_no);
            lstParam.Add("wms_lot_no", ctndetail.wms_lot_no);
            lstParam.Add("exist_barcode", ctndetail.exist_barcode);
            lstParam.Add("is_full", ctndetail.is_full);
            lstParam.Add("given_year", ctndetail.given_year.ToString());
            lstParam.Add("sub_factory", ctndetail.sub_factory);


            lstParam.Add("sub_inventory", ctndetail.sub_inventory);
            lstParam.Add("sub_location", ctndetail.sub_location);
            lstParam.Add("qc_status", ctndetail.qc_status);
            lstParam.Add("qty", (ctndetail.qty ?? 0).ToString());
            lstParam.Add("stat_qty", (ctndetail.stat_qty ?? 0).ToString());
            lstParam.Add("in_qty", (ctndetail.in_qty ?? 0).ToString());
            lstParam.Add("out_qty", (ctndetail.out_qty ?? 0).ToString());
            lstParam.Add("lock_qty", (ctndetail.lock_qty ?? 0).ToString());
            lstParam.Add("spec", ctndetail.given_year.ToString());
            lstParam.Add("uom_qty_id", ctndetail.uom_qty_id);


            lstParam.Add("uom_stat_id", ctndetail.uom_stat_id);
            lstParam.Add("ratio", (ctndetail.ratio ?? 0).ToString());
            lstParam.Add("first_in_date", FormatDatetimeForSql(ctndetail.first_in_date));
            lstParam.Add("start_out_date", FormatDatetimeForSql(ctndetail.start_out_date));
            lstParam.Add("production_date", FormatDatetimeForSql(ctndetail.production_date));
            lstParam.Add("exp_date", FormatDatetimeForSql(ctndetail.exp_date));
            lstParam.Add("warning_date", FormatDatetimeForSql(ctndetail.warning_date));
            lstParam.Add("first_out_date", FormatDatetimeForSql(ctndetail.first_out_date));
            lstParam.Add("out_end_date", FormatDatetimeForSql(ctndetail.out_end_date));
            lstParam.Add("use_by_date", FormatDatetimeForSql(ctndetail.use_by_date));

            lstParam.Add("created_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);

            return DBHelper.RunSql(sql, lstParam) > 0;

        }


        public WebJsonInfo AddContainerH(IDBContextProvider dbW, UserLoginInfo user, ContainerModel ctn)
        {
            WebJsonInfo re = new WebJsonInfo();

            if (string.IsNullOrEmpty(ctn.t_id))
            {
                ctn.t_id = Guid.NewGuid().ToString();
            }
            string sql = @" insert into &DataSource&wms_b_container_h
                           (t_id, client_id, org_id, warehouse_id, location_id, container_barcode, is_full, container_type, container_catalog_id, bom_id, equipment_id, allow_called, first_in_date, label_code, owner_id, is_in, take_in_by, is_out, take_out_by, is_mixed_item, is_mixed_lot, memo, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status)
                         values
                           ('" + ctn.t_id + "', '" + ctn.client_id + "', '" + ctn.org_id + "', '" + ctn.warehouse_id +
                           "', '" + ctn.location_id + "', '" + ctn.container_barcode + "', '" + ctn.is_full +
                           "', '" + ctn.container_type + "', '" + ctn.container_catalog_id + "', '" + ctn.bom_id +
                           "', '" + ctn.equipment_id + "', '" + ctn.allow_called +
                           "', to_date('" + ctn.first_in_date.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-MM-dd HH24:mi:ss')" +
                           ", '" + ctn.label_code + "', '" + ctn.owner_id + "', '" + ctn.is_in +
                           "', '" + ctn.take_in_by + "', '" + ctn.is_out + "', '" + ctn.take_out_by +
                           "', '" + ctn.is_mixed_item + "', '" + ctn.is_mixed_lot + "', '" + ctn.memo +
                           "', sysdate, '" + ctn.created_by + "', '" + ctn.modify_ip + "', sysdate, '" + ctn.last_modified_by + "', '1') ";
            int rows;
            string msg = "";
            List<string> lstSql = new List<string>();
            lstSql.Add(DBHelper.ConvertSql(sql));       
            dbW.DBRunSql(user, lstSql, out rows, out msg);
            re.isSucceed = rows > 0;
            re.errmsg = msg;
            re.json = ctn.t_id;
            return re;
        }

        public WebJsonInfo AddContainerH(ContainerModel ctn)
        {
            WebJsonInfo re = new WebJsonInfo();

            if (string.IsNullOrEmpty(ctn.t_id))
            {
                ctn.t_id = Guid.NewGuid().ToString();
            }
            string sql = @"  insert into &DataSource&wms_b_container_h
                       (t_id, client_id, org_id, warehouse_id, location_id, container_barcode, is_full, container_type, container_catalog_id, bom_id, 
                        equipment_id, allow_called, first_in_date, label_code, owner_id, is_in, take_in_by, is_out, take_out_by, is_mixed_item, 
                        is_mixed_lot, memo, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status)
                     values
                       (@t_id, @client_id, @org_id, @warehouse_id, @location_id, @container_barcode, @is_full, @container_type, @container_catalog_id, @bom_id, 
                        @equipment_id, @allow_called, to_date(@first_in_date,'yyyy-MM-dd HH24:mi:ss'), @label_code, @owner_id, @is_in, @take_in_by, @is_out, @take_out_by, @is_mixed_item, 
                        @is_mixed_lot, @memo, sysdate, @created_by, @modify_ip, sysdate, @last_modified_by, '1') ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", ctn.t_id);
            lstParam.Add("client_id", ctn.client_id);
            lstParam.Add("org_id", ctn.org_id);
            lstParam.Add("warehouse_id", ctn.warehouse_id);
            lstParam.Add("location_id", ctn.location_id);
            lstParam.Add("container_barcode", ctn.container_barcode);
            lstParam.Add("is_full", ctn.is_full);
            lstParam.Add("container_type", ctn.container_type);
            lstParam.Add("container_catalog_id", ctn.container_catalog_id);
            lstParam.Add("bom_id", ctn.bom_id);

            lstParam.Add("equipment_id", ctn.equipment_id);
            lstParam.Add("allow_called", ctn.allow_called);
            lstParam.Add("first_in_date", FormatDatetimeForSql(ctn.first_in_date));
            lstParam.Add("label_code", ctn.label_code);
            lstParam.Add("owner_id", ctn.owner_id);
            lstParam.Add("is_in", ctn.is_in);
            lstParam.Add("take_in_by", ctn.take_in_by);
            lstParam.Add("is_out", ctn.is_out);
            lstParam.Add("take_out_by", ctn.take_out_by);
            lstParam.Add("is_mixed_item", ctn.is_mixed_item);

            lstParam.Add("is_mixed_lot", ctn.is_mixed_lot);
            lstParam.Add("memo", ctn.memo);
            lstParam.Add("created_by", ctn.created_by);
            lstParam.Add("modify_ip", ctn.modify_ip);
            lstParam.Add("last_modified_by", ctn.last_modified_by);

            re.isSucceed = DBHelper.RunSql(sql, lstParam) > 0;
            re.errmsg = "";
            re.json = ctn;
            return re;
        }
        /// <summary>
        /// 更新库粗容器信息
        /// </summary>
        /// <param name="ctn"></param>
        /// <returns></returns>
        public bool UpdateContainerHeaderInfo(ContainerModel ctn)
        {
            WebJsonInfo re = new WebJsonInfo();

            if (string.IsNullOrEmpty(ctn.t_id))
            {
                ctn.t_id = Guid.NewGuid().ToString();
            }
            string sql = @"  update &DataSource&wms_b_container_h
                               set client_id = @client_id,
                                   org_id = @org_id,
                                   warehouse_id = @warehouse_id,
                                   location_id = @location_id, 
                                   is_full = @is_full,
                                   container_type = @container_type,
                                   container_catalog_id = @container_catalog_id,
                                   bom_id = @bom_id,
                                   equipment_id = @equipment_id,
                                   allow_called = @allow_called,
                                   first_in_date = to_date(@first_in_date,'yyyy-MM-dd HH24:mi:ss'),
                                   label_code = @label_code,
                                   owner_id = @owner_id,
                                   is_in = @is_in,
                                   take_in_by = @take_in_by,
                                   is_out = @is_out,
                                   take_out_by = @take_out_by,
                                   is_mixed_item = @is_mixed_item,
                                   is_mixed_lot = @is_mixed_lot,
                                   memo = @memo, 
                                   modify_ip = @modify_ip,
                                   modify_date = sysdate,
                                   last_modified_by = @last_modified_by,
                                   act_status = @act_status
                             where t_id = @t_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", ctn.t_id);
            lstParam.Add("client_id", ctn.client_id);
            lstParam.Add("org_id", ctn.org_id);
            lstParam.Add("warehouse_id", ctn.warehouse_id);
            lstParam.Add("location_id", ctn.location_id);
            lstParam.Add("is_full", ctn.is_full);
            lstParam.Add("container_type", ctn.container_type);
            lstParam.Add("container_catalog_id", ctn.container_catalog_id);
            lstParam.Add("bom_id", ctn.bom_id);

            lstParam.Add("equipment_id", ctn.equipment_id);
            lstParam.Add("allow_called", ctn.allow_called);
            lstParam.Add("first_in_date", FormatDatetimeForSql(ctn.first_in_date));
            lstParam.Add("label_code", ctn.label_code);
            lstParam.Add("owner_id", ctn.owner_id);
            lstParam.Add("is_in", ctn.is_in);
            lstParam.Add("take_in_by", ctn.take_in_by);
            lstParam.Add("is_out", ctn.is_out);
            lstParam.Add("take_out_by", ctn.take_out_by);
            lstParam.Add("is_mixed_item", ctn.is_mixed_item);

            lstParam.Add("is_mixed_lot", ctn.is_mixed_lot);
            lstParam.Add("memo", ctn.memo);
            lstParam.Add("modify_ip", ctn.modify_ip);
            lstParam.Add("last_modified_by", ctn.last_modified_by);
            lstParam.Add("act_status", ctn.act_status);

            return DBHelper.RunSql(sql, lstParam) > 0;

        }

        public bool CheckContainerExisByLocation(string locationcode)
        {
            string sql = @" select count(1) num from &DataSource&wms_b_container_h  where container_barcode=@container_barcode";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("container_barcode", locationcode);
            return int.Parse(DBHelper.Select(sql, lstParams).Rows[0]["num"].ToString()) > 0;
        }
        public bool DeleteContainerSN(string snid)
        {
            string sql = @"delete from &DataSource&wms_b_sn_h where t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", snid);
            return DBHelper.RunSql(sql, lstParam, "删除SN") == 1;
        }
        /// <summary>
        /// 将SN移到历史表
        /// </summary>
        /// <param name="sn">SN信息</param>
        /// <param name="oldSnInfo">SN原始信息</param>
        /// <param name="action"></param>
        /// <param name="reason"></param>
        /// <param name="ref_doc_no">来源单号</param>
        /// <param name="ref_task_no">来源任务号</param>
        /// <returns></returns> 
        public bool MoveSnToLog(Container_SN sn, Container_SN? oldSnInfo, string action, string reason, string ref_doc_no = "", string ref_task_no = "")
        { 
            Container_SN_D sn_d = new Container_SN_D();
            sn_d.action = action;
            sn_d.act_status = "1";
            sn_d.client_id = sn.client_id;
            sn_d.created_by = Context.UserInfo.UserName;
            sn_d.create_date = DateTime.Now;
            sn_d.doc_no = ref_doc_no;
            sn_d.from_location_id = oldSnInfo == null ? "" : oldSnInfo.location_id;
            sn_d.from_location_name = oldSnInfo == null ? "" : oldSnInfo.location_name;
            sn_d.modify_date = DateTime.Now;
            sn_d.modify_ip = Context.UserInfo.LoginIP;
            sn_d.org_id = sn.org_id;
            sn_d.qty = sn.qty;
            sn_d.reason = reason;
            sn_d.sn_id = sn.t_id;
            sn_d.task_no = ref_task_no;
            sn_d.to_location_id = sn.location_id;
            sn_d.to_location_name = sn.location_name;
            sn_d.t_id = SnowflakeID.Guid().ToString();
            sn_d.warehouse_id = sn.warehouse_id;
            AddSND(sn_d);

            return DeleteContainerSN(sn.t_id);
        }
        /// <summary>
        /// 增加SN操作日志
        /// </summary>
        /// <param name="sn">SN当前信息</param>
        /// <param name="oldSnInfo">SN原始信息</param>
        /// <param name="action">动作</param>
        /// <param name="reason">原因</param>
        /// <param name="ref_doc_no">参考单号</param>
        /// <param name="ref_task_no">参考任务号</param>
        /// <returns></returns>
        public bool AddSNDLog(Container_SN sn, Container_SN? oldSnInfo, string action, string reason, string ref_doc_no = "", string ref_task_no = "")
        {
            Container_SN_D sn_d = new Container_SN_D();
            sn_d.action = action;
            sn_d.act_status = "1";
            sn_d.client_id = sn.client_id;
            sn_d.created_by = Context.UserInfo.UserName;
            sn_d.create_date = DateTime.Now;
            sn_d.doc_no = ref_doc_no;
            sn_d.from_location_id = oldSnInfo == null ? "" : oldSnInfo.location_id;
            sn_d.from_location_name = oldSnInfo == null ? "" : oldSnInfo.location_name;
            sn_d.modify_date = DateTime.Now;
            sn_d.modify_ip = Context.UserInfo.LoginIP;
            sn_d.org_id = sn.org_id;
            sn_d.qty = sn.qty;
            sn_d.reason = reason;
            sn_d.sn_id = sn.t_id;
            sn_d.task_no = ref_task_no;
            sn_d.to_location_id = sn.location_id;
            sn_d.to_location_name = sn.location_name;
            sn_d.t_id = SnowflakeID.Guid().ToString();
            sn_d.warehouse_id = sn.warehouse_id;
            return AddSND(sn_d); 
        }

        public List<Container_SN> GetContainerSnListByCtndetailid(string containerdetailid)
        {
            string sql = @" select t_id, client_id, org_id, warehouse_id, item_sn, item_id, item_name, container_id, container_no, container_d_id, location_id, location_name, zone_id, zone_name, qty, spec, lot_no, wms_lot_no, qc_status, store_status, position, stat_qty1, stat_qty2, lot_att, procedure_att, maintenance_att, first_in_date, net_weight, gross_weight, water_rate, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                             from &DataSource&wms_b_sn_h
                             where container_d_id = @container_d_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_d_id", containerdetailid);
            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            return DBHelper.ConvertTableToObject<Container_SN>(dt);
        }
        /// <summary>
        /// 获取容器中的SN信息
        /// </summary>
        /// <param name="containerid"></param>
        /// <returns></returns>
        public List<Container_SN> GetContainerSnListByCtnId(string containerid)
        {
            string sql = @" select t_id, client_id, org_id, warehouse_id, item_sn, item_id, item_name, container_id, container_no, container_d_id, location_id, location_name, zone_id, zone_name, qty, spec, lot_no, wms_lot_no, qc_status, store_status, position, stat_qty1, stat_qty2, lot_att, procedure_att, maintenance_att, first_in_date, net_weight, gross_weight, water_rate, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                             from &DataSource&wms_b_sn_h
                             where container_id = @container_id and act_status=1 ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_id", containerid);
            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            return DBHelper.ConvertTableToObject<Container_SN>(dt);
        }
        /// <summary>
        /// 获取SN信息
        /// </summary>
        /// <param name="containerid"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        public Container_SN GetContainerSn(string containerid,string sn)
        {
            string sql = @" select t_id, client_id, org_id, warehouse_id, item_sn, item_id, item_name, container_id, container_no, container_d_id, location_id, location_name, zone_id, zone_name, qty, spec, lot_no, wms_lot_no, qc_status, store_status, position, stat_qty1, stat_qty2, lot_att, procedure_att, maintenance_att, first_in_date, net_weight, gross_weight, water_rate, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                             from &DataSource&wms_b_sn_h
                             where container_id = @container_id 
                                and item_sn = @item_sn
                                and act_status=1 ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_id", containerid);
            lstParam.Add("item_sn", sn);
            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<Container_SN>(dt)[0];
            }
            else {
                return null;
            }
        }
        public bool CheckContainerExisByID(string containerid)
        {
            string sql = @" select count(1) num from &DataSource&wms_b_container_h  where t_id=@t_id";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("t_id", containerid);
            return int.Parse(DBHelper.Select(sql, lstParams).Rows[0]["num"].ToString()) > 0;
        }
        public bool CheckLocationExis(IDBContextProvider dbR, UserLoginInfo user, string locationcode)
        {
            string sql = @" select count(1) num from &DataSource&wms_b_container_h  where container_barcode='" + locationcode + "'";
            DataTable dt = dbR.DBSelectAll(user, sql).Tables[0];
            return int.Parse(dt.Rows[0]["num"].ToString()) > 0;
        }
        public bool CheckLocationExis(string locationcode)
        {
            string sql = @" select count(1) num from &DataSource&wms_b_container_h  where container_barcode = @container_barcode and act_status='1'";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_barcode", locationcode);
            DataTable dt = DBHelper.Select(sql, lstParam);
            return int.Parse(dt.Rows[0]["num"].ToString()) > 0;
        }

        public WebJsonInfo DeleteContainerD(IDBContextProvider dbW, UserLoginInfo user, string ctndetailid)
        {
            WebJsonInfo re = new WebJsonInfo();
            string sql = @" delete &DataSource&wms_b_container_d where t_id = '" + ctndetailid + "' ";
            int rows;
            string msg = "";
            List<string> lstSql = new List<string>();
            lstSql.Add(sql);
            dbW.DBRunSql(user, lstSql, out rows, out msg);
            re.isSucceed = rows > 0;
            re.errmsg = msg;
            return re;
        }
        /// <summary>
        /// 按库存行ID删除库存明细
        /// </summary>
        /// <param name="ctndetailid"></param>
        /// <returns></returns>
        public bool DeleteContainerD(string ctndetailid)
        {
            WebJsonInfo re = new WebJsonInfo();
            string sql = @" delete &DataSource&wms_b_container_d where t_id = @t_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", ctndetailid);
            return DBHelper.RunSql(sql, lstParam, "删除容器明细") > 0;
        }

        public WebJsonInfo DeleteContainerH(IDBContextProvider dbW, UserLoginInfo user, string ctnid)
        {
            WebJsonInfo re = new WebJsonInfo();
            string sql = @" delete &DataSource&wms_b_container_h where t_id = '" + ctnid + "' ";
            int rows;
            string msg = "";
            List<string> lstSql = new List<string>();
            lstSql.Add(sql);
            dbW.DBRunSql(user, lstSql, out rows, out msg);
            re.isSucceed = rows > 0;
            re.errmsg = msg;
            return re;
        }

        public string getContainerIdByLocation(string locationcode)
        {
            WebJsonInfo re = new WebJsonInfo();
            string sql = @" select t_id from &DataSource&wms_b_container_h where container_barcode =@container_barcode and act_status='1' and rownum=1 ";
            Dictionary<string, object> lspParma = new Dictionary<string, object>();
            lspParma.Add("container_barcode", locationcode);

            DataTable dt = DBHelper.Select(sql, lspParma,"",false);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["t_id"].ToString();
            }
            else
            {
                return "";
            }
        }


        /// <summary>
        /// 更新修改人信息
        /// </summary>
        /// <param name="ctnid"></param>
        /// <returns></returns>
        public bool UpdateContainerModifyInfo(string ctnid)
        {
            string sql = @" update &DataSource&wms_b_container_h
            set modify_ip = @modify_ip , modify_date = sysdate, last_modified_by = @last_modified_by  where t_id=@t_id ";
            Dictionary<string, object> lstParma = new Dictionary<string, object>();
            lstParma.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParma.Add("last_modified_by", Context.UserInfo.UserId);
            lstParma.Add("t_id", ctnid);
            return DBHelper.RunSql(sql, lstParma) > 0;

        }

        /// <summary>
        /// 扣减待入量
        /// </summary>
        /// <param name="ctndetailid"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public bool ReduceContainerQty(string ctndetailid, decimal qty)
        {
            string sql = @" update &DataSource&wms_b_container_d d set d.in_qty =d.in_qty-@reduce_qty,
                                d.last_modified_by = @last_modified_by,
                                d.modify_ip = @modify_ip,
                                d.modify_date = sysdate
                         where d.t_id = @t_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("last_modified_by", Context.UserInfo.UserId);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("reduce_qty", qty.ToString());
            lstParam.Add("t_id", ctndetailid);
            return DBHelper.RunSql(sql, lstParam, "扣减库存待入量") > 0;
        }

        public WebJsonInfo UpdateContainerModifyInfo(IDBContextProvider dbW, UserLoginInfo user, string ctnid)
        {
            WebJsonInfo re = new WebJsonInfo();
            string sql = @" update &DataSource&wms_b_container_h
            set modify_ip = '" + user.LoginIP + "', modify_date = sysdate, last_modified_by = '" + user.UserId +
            "' where t_id='" + ctnid + "' ";
            int rows;
            string msg = "";
            List<string> lstSql = new List<string>();
            lstSql.Add(DBHelper.ConvertSql(sql));
            dbW.DBRunSql(user, lstSql, out rows, out msg);
            re.isSucceed = rows > 0;
            re.errmsg = msg;
            return re;
        }
        /// <summary>
        /// 判断容器行是否需要合并
        /// </summary>
        /// <param name="ctnId"></param>
        /// <param name="itemid"></param>
        /// <param name="wmsLot"></param>
        /// <returns></returns>
        public bool CheckContainerDetailExists(string ctnId, string itemid, string wmsLot)
        {
            string sql = @" select count(1) num from &DataSource&wms_b_container_d  
                    where container_id = @container_id  and item_id = @item_id  and wms_lot_no = @wms_lot_no and act_status='1' ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_id", ctnId);
            lstParam.Add("item_id", itemid);
            lstParam.Add("wms_lot_no", wmsLot);
            DataTable dt = DBHelper.Select(sql, lstParam);
            return int.Parse(dt.Rows[0]["num"].ToString()) > 0;
        }

        public bool CheckContainerDetailExists(IDBContextProvider dbR, UserLoginInfo user, string ctnId, string itemid, string wmsLot)
        {
            string sql = @" select count(1) num from &DataSource&wms_b_container_d  where container_id = '" + ctnId + "' and item_id = '" + itemid + "' and wms_lot_no = '" + wmsLot + "'";
            DataTable dt = dbR.DBSelectAll(user, sql).Tables[0];
            return int.Parse(dt.Rows[0]["num"].ToString()) > 0;
        }

        public string getContainerDetailId(IDBContextProvider dbR, UserLoginInfo user, string ctnId, string itemid, string wmsLot)
        {

            string sql = @" select  t_id  from &DataSource&wms_b_container_d  where container_id='" + ctnId + "' and item_id='" + itemid + "' and wms_lot_no='" + wmsLot + "'";
            DataTable dt = dbR.DBSelectAll(user, sql).Tables[0];
            return dt.Rows[0]["t_id"].ToString();
        }

        public string getContainerDetailId(string ctnId, string itemid, string wmsLot)
        {
            string sql = @" select  t_id  from &DataSource&wms_b_container_d  where container_id= @container_id 
                    and item_id = @item_id  and wms_lot_no= @wms_lot_no and act_status='1' and rownum = 1 ";
            Dictionary<string, object> lstparam = new Dictionary<string, object>();
            lstparam.Add("container_id", ctnId);
            lstparam.Add("item_id", itemid);
            lstparam.Add("wms_lot_no", wmsLot);
            DataTable dt = DBHelper.Select(sql, lstparam,"",false);
            if (dt.Rows.Count == 0) {
                return "";
            }
            return dt.Rows[0]["t_id"].ToString();
        }
        public string GenerateWmsLot(ContainerDModel ctndetail)
        {
            return ctndetail.lot_no + "," + ctndetail.sub_inventory + "," + ctndetail.sub_location;
        }

        public string GenerateWmsLot(PutawayDetailModel putd)
        {
            return putd.lot_no + "," + putd.dest_warehouse + "," + putd.dest_location;
        }

        public WebJsonInfo AddInQty(IDBContextProvider dbW, UserLoginInfo user, string ctndetailid, decimal addQty)
        {

            WebJsonInfo re = new WebJsonInfo();
            string sql = @" update &DataSource&wms_b_container_d d
                           set d.in_qty           = nvl(d.in_qty, 0) + " + addQty + @",
                               d.last_modified_by = '" + user.UserId + @"',
                               d.modify_ip        = '" + user.LoginIP + @"',
                               d.modify_date      = sysdate
                         where d.t_id = '" + ctndetailid + "' ";
            int rows;
            string msg = "";
            List<string> lstSql = new List<string>();
            lstSql.Add(DBHelper.ConvertSql(sql));
            dbW.DBRunSql(user, lstSql, out rows, out msg);
            re.isSucceed = rows > 0;
            re.errmsg = msg;
            return re;
        }

        public WebJsonInfo AddInQty(string ctndetailid, decimal? addQty)
        {
            WebJsonInfo re = new WebJsonInfo();
            string sql = @" update &DataSource&wms_b_container_d d
                            set d.in_qty           = nvl(d.in_qty, 0) + @in_qty,
                               d.last_modified_by = @last_modified_by ,
                               d.modify_ip        =  @modify_ip ,
                               d.modify_date      = sysdate
                            where d.t_id = @t_id ";
            int rows;
            string msg = "";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("in_qty", addQty.ToString());
            lstParam.Add("last_modified_by", Context.UserInfo.UserId);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("t_id", ctndetailid);
            rows = DBHelper.RunSql(sql, lstParam);

            re.isSucceed = rows > 0;
            re.errmsg = msg;
            return re;
        }

        /// <summary>
        /// 更新库存明细数量：将待入数量更新到库存数量
        /// </summary>
        /// <param name="ctnd">容器明细</param>
        /// <param name="inQty">本次上架数量</param>
        /// <param name="ratio">物料单位转化因子</param>
        /// <returns></returns>
        public bool ContainerItemInbound(ContainerDModel ctnd, decimal thisQty, decimal ratio)
        {
            string sql = @"update &DataSource&wms_b_container_d d set d.qty=nvl(d.qty,0)+@thisQty,
                                                           d.in_qty=d.in_qty-@thisQty,
                                                           d.stat_qty=nvl(d.stat_qty,0) + @thisQty * @ratio, 
                                                           d.last_modified_by=@last_modified_by,
                                                           d.modify_ip=@modify_ip,
                                                           d.modify_date=sysdate
                            where d.t_id=@t_id";
            Dictionary<string, object> lstParm = new Dictionary<string, object>();
            lstParm.Add("thisQty", thisQty.ToString());
            lstParm.Add("ratio", ratio.ToString());
            lstParm.Add("last_modified_by", Context.UserInfo.UserId);
            lstParm.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParm.Add("t_id", ctnd.t_id);

            return DBHelper.RunSql(sql, lstParm, "更新容器明细入库量") > 0;

        }
        /// <summary>
        /// 撤销容器待入库数量
        /// </summary>
        /// <param name="ctnd"></param>
        /// <param name="inQty"></param>
        /// <returns></returns>
        public bool RollbackContainerInqty(ContainerDModel ctnd, decimal rollQty)
        {
            string sql = @"update &DataSource&wms_b_container_d d set  d.in_qty=d.in_qty-@rollQty,
                                                           d.last_modified_by=@last_modified_by,
                                                           d.modify_ip=@modify_ip,
                                                           d.modify_date=sysdate
                            where d.t_id=@t_id";
            Dictionary<string, object> lstParm = new Dictionary<string, object>();
            lstParm.Add("rollQty", rollQty.ToString());
            lstParm.Add("last_modified_by", Context.UserInfo.UserId);
            lstParm.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParm.Add("t_id", ctnd.t_id);

            return DBHelper.RunSql(sql, lstParm, "更新容器明细入库量") > 0;

        }
        //表转换为对象 yanglinyi 2018-7-19
        public static List<object> ConvertTableToObject<T>(DataTable dt, string className, T newt)
        {
            List<object> list = new List<object>();
            Type type = newt.GetType();

            //遍历DataTable中所有的数据行
            foreach (DataRow dr in dt.Rows)
            {
                var t = Activator.CreateInstance(type);

                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pro in propertys)
                {
                    //检查DataTable是否包含此列（列名==对象的属性名）  
                    if (dt.Columns.Contains(pro.Name) && dr[pro.Name] != null)
                    {
                        object value = dr[pro.Name];

                        Type tmpType = Nullable.GetUnderlyingType(pro.PropertyType) ?? pro.PropertyType;
                        try
                        {
                            object safeValue = (value == null) ? null : Convert.ChangeType(value, tmpType);

                            //如果非空，则赋给对象的属性  PropertyInfo
                            if (safeValue != DBNull.Value)
                            {
                                pro.SetValue(t, safeValue, null);
                            }
                        }
                        catch { }
                    }
                }
                //对象添加到泛型集合中
                list.Add(t);
            }
            return list;
        }

        public ContainerDModel getContainerDetailById(IDBContextProvider dbR, UserLoginInfo user, string ctndetailid)
        {
            string sql = @"select * from &DataSource&wms_b_container_d where t_id='" + ctndetailid + "'";
            DataTable dt = dbR.DBSelectAll(user, sql).Tables[0];
            List<ContainerDModel> lstContainerDetail = DBHelper.ConvertTableToObject<ContainerDModel>(dt);
            if (lstContainerDetail.Count > 0)
            {
                return (ContainerDModel)lstContainerDetail[0];
            }
            else
            {
                return null;
            }
        }
        public ContainerModel getContainerById(string ctnid)
        {
            string sql = @"select * from &DataSource&wms_b_container_h where t_id= @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", ctnid);
            DataTable dt = DBHelper.Select(sql, lstParam);
            List<ContainerModel> lstContainerDetail = DBHelper.ConvertTableToObject<ContainerModel>(dt);
            if (lstContainerDetail.Count > 0)
            {
                return (ContainerModel)lstContainerDetail[0];
            }
            else
            {
                return null;
            }
        }
        public ContainerDModel getContainerDetailById(string ctndetailid)
        {
            string sql = @"select * from &DataSource&wms_b_container_d where t_id= @t_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", ctndetailid);
            DataTable dt = DBHelper.Select(sql, lstParam, "查询容器明细", false);
            if (dt.Rows.Count > 0)
            {
                List<ContainerDModel> lstContainerDetail = DBHelper.ConvertTableToObject<ContainerDModel>(dt);
                return (ContainerDModel)lstContainerDetail[0];
            }
            else
            {
                return null;
            }
        }
        public List<ContainerDModel> getContainerDetails(string containerId)
        {
            string sql = @"select * from &DataSource&wms_b_container_d where container_id= @container_id and act_status = '1' ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_id", containerId);
            DataTable dt = DBHelper.Select(sql, lstParam, "查询容器明细", false);
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<ContainerDModel>(dt);

            }
            else
            {
                return new List<ContainerDModel>();
            }
        }
        public string getLocationId(string locationcode)
        {
            string sql = @"select t_id from &DataSource&wms_m_location l where l.location_no=@location_no";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("location_no", locationcode);
            DataTable dt = DBHelper.Select(sql, lstParam);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["t_id"].ToString();
            }
            else
            {
                return "";
            }
        }

        public DataRow GetContainerH(string containerHID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "containerHID", containerHID }
            };
            string sql = @" select * from &DataSource&wms_b_container_h h where h.t_id=@containerHID";

            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询容器头表");

            return dt.Rows[0];
        }

        public DataTable getContainerDetail(string ctnId, string itemid, string wmsLot = null)
        {
            wmsLot = string.IsNullOrWhiteSpace(wmsLot) ? " " : wmsLot;
            string sql = @" select  *  from &DataSource&wms_b_container_d  where container_id= @container_id 
                    and item_id = @item_id  and (wms_lot_no= @wms_lot_no or @wms_lot_no='') ";
            Dictionary<string, object> lstparam = new Dictionary<string, object>();
            lstparam.Add("container_id", ctnId);
            lstparam.Add("item_id", itemid);
            lstparam.Add("wms_lot_no", wmsLot);
            DataTable dt = DBHelper.Select(sql, lstparam,"",false);
            return dt;
        }
        /// <summary>
        /// 查询库存容器信息
        /// <br/>
        /// 容器不存在时返回null
        /// </summary>
        /// <param name="warhouseId"></param>
        /// <param name="containerCode"></param>
        /// <returns></returns>
        public ContainerModel GetContainer(string warhouseId, string containerCode)
        {
            string sql = @" select * from  &DataSource&wms_b_container_h h 
                    where h.warehouse_id = @warehouse_id and h.container_barcode=@container_barcode and act_status='1' ";
            Dictionary<string, object> lstparam = new Dictionary<string, object>();
            lstparam.Add("warehouse_id", warhouseId);
            lstparam.Add("container_barcode", containerCode);
            DataTable dt = DBHelper.Select(sql, lstparam, "查询库存容器信息", false);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            if (dt.Rows.Count > 1)
            {
                throw new Exception("根据容器查询到多笔库存信息");
            }
            return DBHelper.ConvertTableToObject<ContainerModel>(dt)[0];

        }

        public bool AddSNIntoContainer(ref TaskHeaderModel task, ContainerModel ctn, ContainerDModel ctndetail, List<SNModel> lstSn)
        {
            if (lstSn == null)
            {
                return true;
            }
            foreach (SNModel sn in lstSn)
            {
                if (!CheckSnCanAddTo(ctndetail, sn.SN))
                {
                    throw new Exception("序列号:" + sn.SN + "已经在库");
                }
            }
            LocationModel location = Location.GetLocationById(ctn.location_id);
            string userid = Context.UserInfo.UserId;
            foreach (SNModel sn in lstSn)
            {
                AddSNIntoContainer(ctn, ctndetail, sn.SN, sn.Qty ?? 1, "", location, ref task);
            };

            return true;

        }
        /// <summary>
        /// 检查SN在序列号表是否已存在并且状态为已存储
        /// </summary>
        /// <param name="ctndetail">库存容器行</param>
        /// <param name="sn">序列号</param>
        /// <returns></returns>
        public bool CheckSnCanAddTo(ContainerDModel ctndetail, string sn)
        {
            string sql = "select count(1) num from &DataSource&wms_b_sn_h h where h.item_sn = @sn and store_status = @store_status";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("sn", sn);
            lstParam.Add("store_status", "1");
            return int.Parse(DBHelper.Select(sql, lstParam).Rows[0]["num"].ToString()) == 0;
        }
        /// <summary>
        /// 将SN添加到库存容器行中
        /// </summary>
        /// <param name="ctn">库存容器</param>
        /// <param name="ctndetail">库存容器行</param>
        /// <param name="sn">序列号</param>
        /// <param name="snqty">SN数量</param>
        /// <param name="toLocation">货位</param>
        /// <returns></returns>
        public bool AddSNIntoContainer(ContainerModel ctn, ContainerDModel ctndetail, string sn, decimal snqty, string snposition, LocationModel toLocation, ref TaskHeaderModel task)
        {
            string sql = @"insert into &DataSource&wms_b_sn_h
                        (t_id, client_id, org_id, warehouse_id, item_sn, item_id, item_name, container_id, container_no, container_d_id, 
                        location_id, location_name, zone_id, zone_name, qty, spec, lot_no, wms_lot_no, qc_status, store_status, 
                        position, stat_qty1,  first_in_date, create_date, created_by, modify_date, act_status)
                        values
                        (@t_id, @client_id, @org_id, @warehouse_id, @item_sn, @item_id, @item_name, @container_id, @container_no, @container_d_id,
                        @location_id, @location_name, @zone_id, @zone_name , @qty, @spec, @lot_no, @wms_lot_no, @qc_status, @store_status,
                        @position, @stat_qty1, sysdate, sysdate, @created_by , sysdate, '1')
                           ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            string newSNid = SnowflakeID.Guid().ToString();
            lstParam.Add("t_id", newSNid);
            lstParam.Add("client_id", ctndetail.client_id);
            lstParam.Add("org_id", ctndetail.org_id);
            lstParam.Add("warehouse_id", ctndetail.warehouse_id);
            lstParam.Add("item_sn", sn);
            lstParam.Add("item_id", ctndetail.item_id);
            lstParam.Add("item_name", ctndetail.item_name);
            lstParam.Add("container_id", ctndetail.container_id);
            lstParam.Add("container_no", ctn.container_barcode);
            lstParam.Add("container_d_id", ctndetail.t_id);

            lstParam.Add("location_id", ctn.location_id);
            lstParam.Add("location_name", toLocation.location_name);
            lstParam.Add("zone_id", toLocation.zone_id);
            lstParam.Add("zone_name", "");
            lstParam.Add("qty", snqty.ToString());
            lstParam.Add("spec", ctndetail.spec);
            lstParam.Add("lot_no", ctndetail.lot_no);
            lstParam.Add("wms_lot_no", ctndetail.wms_lot_no);
            lstParam.Add("qc_status", ctndetail.qc_status);
            lstParam.Add("store_status", "1");

            lstParam.Add("position", snposition);
            lstParam.Add("stat_qty1", (ctndetail.ratio ?? 0 * snqty).ToString());
            lstParam.Add("created_by", Context.UserInfo.UserId);

            Container_SN_D snd = new Container_SN_D();
            snd.t_id = SnowflakeID.Guid().ToString();
            snd.client_id = ctn.client_id;
            snd.org_id = ctn.org_id;
            snd.warehouse_id = ctn.warehouse_id;
            snd.sn_id = newSNid;
            snd.to_location_id = toLocation.t_id;
            snd.to_location_name = toLocation.location_name;
            snd.to_location_no = toLocation.location_code;
            snd.action = "上架";
            snd.reason = "上架任务执行=上架";
            snd.task_no = task.task_no;
            snd.doc_no = task.ref_doc_no;
            snd.created_by = Context.UserInfo.UserId;
            snd.last_modified_by = Context.UserInfo.UserId;
            snd.modify_ip = Context.UserInfo.LoginIP;
            AddSND(snd);

            return DBHelper.RunSql(sql, lstParam) > 0;

        }

        /// <summary>
        /// 新增SN
        /// </summary>
        /// <param name="ctn">SN对应的容器信息</param>
        /// <param name="ctndetail">SN对应的容器行信息</param>
        /// <param name="sn">SN号</param>
        /// <param name="snqty">SN数量</param>
        /// <param name="snposition">sn位置</param>
        /// <param name="action">新增类型，如：产线申请入库等</param>
        /// <param name="taskno">任务号</param>
        /// <param name="ref_no">业务单号</param>
        /// <returns></returns>
        public bool AddSNIntoContainer(ContainerModel ctn, ContainerDModel ctndetail, string sn, decimal snqty, string snposition, string reason, string taskno, string ref_no)
        {
            string sql = @"insert into &DataSource&wms_b_sn_h
                        (t_id, client_id, org_id, warehouse_id, item_sn, item_id, item_name, container_id, container_no, container_d_id, 
                        location_id, location_name, zone_id, zone_name, qty, spec, lot_no, wms_lot_no, qc_status, store_status, 
                        position, stat_qty1,  first_in_date, create_date, created_by, modify_date, act_status)
                        values
                        (@t_id, @client_id, @org_id, @warehouse_id, @item_sn, @item_id, @item_name, @container_id, @container_no, @container_d_id,
                        @location_id, @location_name, @zone_id, @zone_name , @qty, @spec, @lot_no, @wms_lot_no, @qc_status, @store_status,
                        @position, @stat_qty1, sysdate, sysdate, @created_by , sysdate, '1')
                           ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            string newSNid = SnowflakeID.Guid().ToString();
            lstParam.Add("t_id", newSNid);
            lstParam.Add("client_id", ctndetail.client_id);
            lstParam.Add("org_id", ctndetail.org_id);
            lstParam.Add("warehouse_id", ctndetail.warehouse_id);
            lstParam.Add("item_sn", sn);
            lstParam.Add("item_id", ctndetail.item_id);
            lstParam.Add("item_name", ctndetail.item_name);
            lstParam.Add("container_id", ctndetail.container_id);
            lstParam.Add("container_no", ctn.container_barcode);
            lstParam.Add("container_d_id", ctndetail.t_id);

            lstParam.Add("location_id", ctn.location_id);
            lstParam.Add("location_name", "");
            lstParam.Add("zone_id", "");
            lstParam.Add("zone_name", "");
            lstParam.Add("qty", snqty.ToString());
            lstParam.Add("spec", ctndetail.spec);
            lstParam.Add("lot_no", ctndetail.lot_no);
            lstParam.Add("wms_lot_no", ctndetail.wms_lot_no);
            lstParam.Add("qc_status", ctndetail.qc_status);
            lstParam.Add("store_status", "1");

            lstParam.Add("position", snposition);
            lstParam.Add("stat_qty1", (ctndetail.ratio ?? 0 * snqty).ToString());
            lstParam.Add("created_by", Context.UserInfo.UserId);

            Container_SN_D snd = new Container_SN_D();
            snd.t_id = SnowflakeID.Guid().ToString();
            snd.client_id = ctn.client_id;
            snd.org_id = ctn.org_id;
            snd.warehouse_id = ctn.warehouse_id;
            snd.sn_id = newSNid;
            snd.action = "新增";
            snd.reason = reason;
            snd.task_no = taskno;
            snd.doc_no = ref_no;
            snd.created_by = Context.UserInfo.UserId;
            snd.last_modified_by = Context.UserInfo.UserId;
            snd.modify_ip = Context.UserInfo.LoginIP;
            AddSND(snd);

            return DBHelper.RunSql(sql, lstParam) > 0;

        }
        /// <summary>
        /// 新增SN记录
        /// </summary>
        /// <param name="sninfo">SN信息</param>
        /// <returns></returns>
        public bool Insert(Container_SN sninfo)
        {
            string sql = @"insert into &DataSource&wms_b_sn_h
                        (t_id, client_id, org_id, warehouse_id, item_sn, item_id, item_name, container_id, container_no, container_d_id, 
                        location_id, location_name, zone_id, zone_name, qty, spec, lot_no, wms_lot_no, qc_status, store_status, 
                        position, stat_qty1,  first_in_date, create_date, created_by, modify_date, act_status)
                        values
                        (@t_id, @client_id, @org_id, @warehouse_id, @item_sn, @item_id, @item_name, @container_id, @container_no, @container_d_id,
                        @location_id, @location_name, @zone_id, @zone_name , @qty, @spec, @lot_no, @wms_lot_no, @qc_status, @store_status,
                        @position, @stat_qty1, sysdate, sysdate, @created_by , sysdate, '1')
                           ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            if (string.IsNullOrEmpty(sninfo.t_id))
            {
                sninfo.t_id = SnowflakeID.Guid().ToString();
            }
            lstParam.Add("t_id", sninfo.t_id);
            lstParam.Add("client_id", sninfo.client_id);
            lstParam.Add("org_id", sninfo.org_id);
            lstParam.Add("warehouse_id", sninfo.warehouse_id);
            lstParam.Add("item_sn", sninfo.item_sn);
            lstParam.Add("item_id", sninfo.item_id);
            lstParam.Add("item_name", sninfo.item_name);
            lstParam.Add("container_id", sninfo.container_id);
            lstParam.Add("container_no", sninfo.container_no);
            lstParam.Add("container_d_id", sninfo.container_d_id);

            lstParam.Add("location_id", sninfo.location_id);
            lstParam.Add("location_name", sninfo.location_name);
            lstParam.Add("zone_id", sninfo.zone_id);
            lstParam.Add("zone_name", sninfo.zone_name);
            lstParam.Add("qty", sninfo.qty);
            lstParam.Add("spec", sninfo.spec);
            lstParam.Add("lot_no", sninfo.lot_no);
            lstParam.Add("wms_lot_no", sninfo.wms_lot_no);
            lstParam.Add("qc_status", sninfo.qc_status);
            lstParam.Add("store_status", "1");

            lstParam.Add("position", sninfo.position);
            lstParam.Add("stat_qty1", sninfo.stat_qty1);
            lstParam.Add("created_by", Context.UserInfo.UserId); 

            return DBHelper.RunSql(sql, lstParam) > 0;

        }

        /// <summary>
        /// 新增SN_D日志
        /// </summary>
        /// <returns></returns>
        private bool AddSND(Container_SN_D snd)
        {
            string sql = @"insert into &DataSource&wms_b_sn_d
                            (t_id, client_id, org_id, warehouse_id, sn_id, from_location_id, from_location_no, from_location_name, to_location_id, to_location_no, 
                                to_location_name, action, reason, doc_no, task_no, create_date, created_by, modify_ip, modify_date, last_modified_by, 
                                act_status, qty)
                            values
                            (@t_id, @client_id, @org_id, @warehouse_id, @sn_id, @from_location_id, @from_location_no, @from_location_name, @to_location_id, @to_location_no, 
                                @to_location_name, @action, @reason, @doc_no, @task_no, sysdate, @created_by, @modify_ip, sysdate, @last_modified_by, 
                                '1', @qty)";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", snd.t_id);
            lstParam.Add("client_id", snd.client_id);
            lstParam.Add("org_id", snd.org_id);
            lstParam.Add("warehouse_id", snd.warehouse_id);
            lstParam.Add("sn_id", snd.sn_id);
            lstParam.Add("from_location_id", snd.from_location_id);
            lstParam.Add("from_location_no", snd.from_location_no);
            lstParam.Add("from_location_name", snd.from_location_name);
            lstParam.Add("to_location_id", snd.to_location_id);
            lstParam.Add("to_location_no", snd.to_location_no);

            lstParam.Add("to_location_name", snd.to_location_name);
            lstParam.Add("action", snd.action);
            lstParam.Add("reason", snd.reason);
            lstParam.Add("doc_no", snd.doc_no);
            lstParam.Add("task_no", snd.task_no);
            lstParam.Add("created_by", snd.created_by);
            lstParam.Add("modify_ip", snd.modify_ip);
            lstParam.Add("last_modified_by", snd.last_modified_by);
            lstParam.Add("qty", (snd.qty ?? 0).ToString());

            return DBHelper.RunSql(sql, lstParam) > 0;
        }
        /// <summary>
        /// 根据物料的控制日期类型，计算相关控制时间点
        /// </summary>
        /// <param name="containerd"></param>
        public void SetContainerAlarmDate(ContainerDModel containerd)
        {
            ItemModel item = new Item().GetItem(containerd.item_id);
            if (item.is_control_expiry_date == "1")
            {
                if (item.expiry_date_control_type == ExpiryDateControlType.ExpiryDateControlType_ByProduce.ToString() && containerd.production_date != null)
                {//按生产日期控制
                    DateTime production_date = (DateTime)containerd.production_date;
                    //计算超期日期
                    if (item.expiry_days != null)
                    {
                        containerd.exp_date = production_date.AddDays(item.expiry_days ?? 0);
                    }
                    //计算预警日期
                    if (containerd.exp_date != null && item.alert_days != null)
                    {
                        containerd.warning_date = ((DateTime)containerd.exp_date).AddDays(-1 * (item.alert_days ?? 0));
                    }
                    ///计算优先出库日期
                    if (containerd.exp_date != null && item.out_first_days != null)
                    {
                        containerd.first_out_date = ((DateTime)containerd.exp_date).AddDays(-1 * (item.out_first_days ?? 0));
                    }
                    ///计算出库截止日期
                    if (containerd.exp_date != null && item.stock_out_expiry_days != null)
                    {
                        containerd.out_end_date = ((DateTime)containerd.exp_date).AddDays(-1 * (item.stock_out_expiry_days ?? 0));
                    }
                }
                else if (item.expiry_date_control_type == ExpiryDateControlType.ExpiryDateControlType_ByIn.ToString() && containerd.first_in_date != null)
                {//按入库日期控制
                    DateTime first_in_date = (DateTime)containerd.first_in_date;
                    //计算可以出库时间
                    if (item.stock_in_expiry_minutes != null)
                    {
                        containerd.use_by_date = first_in_date.AddMinutes(item.stock_in_expiry_minutes ?? 0);
                    }
                }
            }

        }

        /// <summary>
        /// 查询暂存区待上架托盘ID
        /// </summary>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="containerNo">容器号</param>
        /// <returns></returns>
        public string GetContainerIdByContainerNo(string containerNo)
        {
            string sql = @"select h.t_id
                        from &DataSource&wms_b_container_h h 
                        where h.container_barcode= @container_barcode  and h.container_type = @container_type
                            and exists(select 1 from &DataSource&wms_m_location x 
                            where h.location_id=x.t_id and x.location_name='暂存区虚拟货位' and x.act_status='1' )
                        and h.act_status='1'";

            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_barcode", containerNo);
            lstParam.Add("container_type", ContainerType.ContainerType_Actual.ToString());

            return DBHelper.SelectOne(sql, lstParam, "查询暂存区待上架托盘ID");

        }
        /// <summary>
        /// 查询容器ID
        /// </summary>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="containerNo">容器号</param>
        /// <returns></returns>
        public string GetContainerIdByContainerNo(string warehouseId, string containerNo)
        {
            string sql = @"select h.t_id
                        from &DataSource&wms_b_container_h h where h.container_barcode= @container_barcode  and  h.warehouse_id=@warehouse_id";

            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_barcode", containerNo);
            lstParam.Add("warehouse_id", warehouseId);

            DataTable dt = DBHelper.Select(sql, lstParam, "查询容器ID", false);
            if (dt.Rows.Count == 0)
            {
                return "";
            }
            else
            {
                return dt.Rows[0][0].ToString();
            }

        }

        /// <summary>
        /// 更新容器货位信息
        /// </summary>
        /// <param name="containerid">容器ID</param>
        /// <param name="locationId">货位ID</param>
        /// <returns></returns>
        public bool UpdateContainerLocation(string containerid, string locationId)
        {
            string sql = @"update &DataSource&wms_b_container_h h set h.location_id = @location_id,
                                                 h.take_in_by = @take_in_by,
                                                 h.is_in = 1,
                                                 h.last_modified_by = @last_modified_by,
                                                 h.modify_ip = @modify_ip,
                                                 h.modify_date = sysdate  
                where h.t_id=@t_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", containerid);
            lstParam.Add("location_id", locationId);
            lstParam.Add("take_in_by", Context.UserInfo.UserName);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            return DBHelper.RunSql(sql, lstParam, "更新容器货位信息") > 0;
        }

        /// <summary>
        /// 更新容器满载标记
        /// </summary>
        /// <param name="containerid">容器ID</param>
        /// <param name="isFull">满载标记</param>
        /// <returns></returns>
        public bool UpdateContainerIsFull(string containerid, string isFull)
        {
            string sql = @"update &DataSource&wms_b_container_h h set h.is_full = @isFull,
                                                 h.take_in_by = @take_in_by,
                                                 h.is_in = 1,
                                                 h.last_modified_by = @last_modified_by,
                                                 h.modify_ip = @modify_ip,
                                                 h.modify_date = sysdate  
                where h.t_id=@t_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", containerid);
            lstParam.Add("isFull", isFull);
            lstParam.Add("take_in_by", Context.UserInfo.UserName);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            return DBHelper.RunSql(sql, lstParam, "更新容器满载标记") > 0;
        }

        /// <summary>
        /// 更新货位对应的容器Bom信息
        /// </summary>
        /// <param name="containerid">容器ID</param>
        /// <param name="bomId">bomID</param>
        /// <returns></returns>
        public bool UpdateContainerBom(string containerid, string bomId)
        {
            string sql = @"update &DataSource&wms_b_container_h h set h.bom_id = @bomId,
                                                 h.take_in_by = @take_in_by,
                                                 h.is_in = 1,
                                                 h.last_modified_by = @last_modified_by,
                                                 h.modify_ip = @modify_ip,
                                                 h.modify_date = sysdate  
                where h.t_id=@t_id ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", containerid);
            lstParam.Add("bomId", bomId);
            lstParam.Add("take_in_by", Context.UserInfo.UserName);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            return DBHelper.RunSql(sql, lstParam, "更新货位对应容器的bom信息") > 0;
        }

        /// <summary>
        /// 更新SN表中的货位信息
        /// <br>并新增SN_D表日志</br>
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="toLocation"></param>
        /// <param name="taskno"></param>
        /// <param name="action"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool UpdateContainerSnInLocation(string containerId, LocationModel toLocation, string taskno, string action, string reason)
        {
            string sql = @" insert into &DataSource&wms_b_sn_d
                           (t_id, client_id, org_id, warehouse_id, sn_id, from_location_id, from_location_no, from_location_name, 
                            to_location_id, to_location_no, to_location_name, action, reason, doc_no, task_no, 
                            create_date, created_by, modify_ip, modify_date, last_modified_by, act_status, qty)
                         select sys_guid() ,h.client_id,h.org_id,h.warehouse_id,h.t_id,h.location_id,'',h.location_name,
                            @to_location_id,@to_location_no,@to_location_name,@action,@reason,'',@task_no,
                            sysdate,@created_by,@modify_ip,sysdate,'','1',h.qty  
                         from &DataSource&wms_b_sn_h h
                         where h.container_id=@container_id and act_status='1'";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_id", containerId);
            lstParam.Add("to_location_id", toLocation==null?"":toLocation.t_id);
            lstParam.Add("to_location_no", toLocation==null?"":toLocation.location_no);
            lstParam.Add("to_location_name", toLocation==null?"":toLocation.location_name);
            lstParam.Add("action", action);
            lstParam.Add("reason", reason);
            lstParam.Add("task_no", taskno);
            lstParam.Add("created_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            DBHelper.RunSql(sql, lstParam, "", DBHelper.OperatedNumType.ZeroOrMore);

            sql = @" update &DataSource&wms_b_sn_h h set  h.location_id=@location_id,
                                         h.location_name=@location_name,
                                         h.store_status='1',
                                         h.last_modified_by=@last_modified_by,
                                         h.modify_ip=@modify_ip,
                                         h.modify_date=sysdate
                     where h.container_id=@container_id and act_status='1'";
            lstParam.Clear();
            lstParam.Add("container_id", containerId);
            lstParam.Add("location_id", toLocation==null?"":toLocation.t_id);
            lstParam.Add("location_name", toLocation==null?"":toLocation.location_name);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            DBHelper.RunSql(sql, lstParam, "", DBHelper.OperatedNumType.ZeroOrMore);
            return true;
        }
        /// <summary>
        /// 更新SN表中的货位信息
        /// <br>并新增SN_D表日志</br>
        public bool UpdateContainerSnInLocation(string containerId,  string stationName, string taskno, string action, string reason)
        {
            string sql = @" insert into &DataSource&wms_b_sn_d
                           (t_id, client_id, org_id, warehouse_id, sn_id, from_location_id, from_location_no, from_location_name, 
                            to_location_id, to_location_no, to_location_name, action, reason, doc_no, task_no, 
                            create_date, created_by, modify_ip, modify_date, last_modified_by, act_status, qty)
                         select sys_guid() ,h.client_id,h.org_id,h.warehouse_id,h.t_id,h.location_id,'',h.location_name,
                            @to_location_id,@to_location_no,@to_location_name,@action,@reason,'',@task_no,
                            sysdate,@created_by,@modify_ip,sysdate,'','1',h.qty  
                         from &DataSource&wms_b_sn_h h
                         where h.container_id=@container_id and act_status='1'";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_id", containerId);
            lstParam.Add("to_location_id", "");
            lstParam.Add("to_location_no","");
            lstParam.Add("to_location_name", stationName);
            lstParam.Add("action", action);
            lstParam.Add("reason", reason);
            lstParam.Add("task_no", taskno);
            lstParam.Add("created_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            DBHelper.RunSql(sql, lstParam, "", DBHelper.OperatedNumType.ZeroOrMore);

            sql = @" update &DataSource&wms_b_sn_h h set  h.location_id=@location_id,
                                         h.location_name=@location_name,
                                         h.store_status='1',
                                         h.last_modified_by=@last_modified_by,
                                         h.modify_ip=@modify_ip,
                                         h.modify_date=sysdate
                     where h.container_id=@container_id and act_status='1'";
            lstParam.Clear();
            lstParam.Add("container_id", containerId);
            lstParam.Add("location_id", "");
            lstParam.Add("location_name", stationName);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            DBHelper.RunSql(sql, lstParam, "", DBHelper.OperatedNumType.ZeroOrMore);
            return true;
        }

        /// <summary>
        /// 托盘管理：更新容器的统计数量
        /// </summary>
        /// <param name="containerid"></param>
        /// <returns></returns>
        public bool UpdateContainerStaQty(string containerid)
        {
            string sql = @"update &DataSource&wms_b_container_d d 
                            set d.stat_qty= d.spec*d.ratio*d.qty ,
                                  d.last_modified_by=@last_modified_by,
                                  d.modify_ip=@modify_ip,
                                  d.modify_date=sysdate  
                            where d.container_id=@container_id 
                                    and d.ratio >0 and act_status='1'";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_id", containerid);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            return DBHelper.RunSql(sql, lstParam, "更新容器的统计数量", DBHelper.OperatedNumType.ZeroOrMore) > 0;
        }
        private bool UpdateContainerSnLocation(string containerid, string locationid, string locationCode)
        {
            string sql = @"update wms_b_sn_h h set h.location_id = @location_id,
                                                    h.location_name = @location_name,
                                                    h.last_modified_by = @last_modified_by,
                                                    h.modify_ip = @modify_ip,
                                                    h.modify_date=sysdate
                            where h.container_id=@container_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("container_id", containerid);
            lstParam.Add("location_id", locationid);
            lstParam.Add("location_name", locationCode);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            return DBHelper.RunSql(sql, lstParam, "更新序列号货位") > 0;
        }


        /// <summary>
        /// 判断当前容器是否允许申请入库
        /// </summary>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="containerCode">容器号</param>
        /// <returns></returns>
        public bool CheckContainerCanAppyIn(string warehouseId, string containerCode)
        {
            ContainerModel ctnHeader = GetContainer(warehouseId, containerCode);
            if (ctnHeader == null)
            {
                return true;
            }
            else
            {
                List<ContainerDModel> lstDetail = getContainerDetails(ctnHeader.t_id);
                foreach (ContainerDModel ctnd in lstDetail)
                {
                    if ((ctnd.qty ?? 0) > 0)
                    {
                        return false;
                    }
                    if ((ctnd.in_qty ?? 0) > 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public DataRow GetOrNewVirtualHead(DataRow locationRow)
        {
            DataRow[] containers = Container.GetHeadsByLocation(locationRow[l.ID].ToString());
            containers = containers.Where(
                r => r[ch.ContainerType].ToString() == ContainerType.ContainerType_Virtual.ToString()).ToArray();
            if (containers.Length <= 0)
            {
                Container.Insert(new ContainerHeadModel()
                {
                    LocationID = locationRow[l.ID].ToString(),
                    ContainerBarcode = locationRow[l.LocationCode].ToString(),
                    ContainerType = ContainerType.ContainerType_Virtual.ToString(),
                    FirstInDate = DateTime.Now,
                    OwnerID = locationRow[l.OwnerID].ToString(),
                });
                return GetOrNewVirtualHead(locationRow);
            }
            return containers.ToArray().First();
        }

        /// <summary>
        /// 模拟分配空托盘
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public ContainerModel CalloutEmptyContainer(string warehouseId, int? width)
        {
            ItemModel item = new Item().GetEmptyContainerItem(warehouseId, width);
            if (item == null)
            {                
                throw new Exception("无此空托盘物料基础信息");
            }
            string sql = @" select * from(
                        select * from &DataSource&wms_b_container_h  h  
                        where  nvl(h.is_in,'0')='1' and nvl(h.is_out,'0')='0' 
                             and exists(select 1 from &DataSource&wms_b_container_d d where h.t_id=d.container_id and d.item_id=@item_id)
                             and exists(select 1 from &DataSource&wms_m_location l inner join &DataSource&wms_m_equipment e on l.zone_id4=e.equipment_code 
                                           where h.location_id=l.t_id and e.equipment_status='EquipmentStatus_OK' 
                                                 and l.location_status='LocationStatus_Stored'
                                                 and l.location_flag in('LocationFlag_Usable','LocationFlag_OutOnly')
                                    )
                        order by h.first_in_date)
                        where  rownum=1 ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("item_id", item.t_id);
            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("分配空托盘失败");
            }
            else
            {
                return DBHelper.ConvertTableToObject<ContainerModel>(dt)[0];
            }
        }

        DataRow[] IContainer.GetDetailsAll(string containerID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container_id", containerID },
            };
            string sql = @"
select * from &DataSource&wms_b_container_d cd
 where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&'
   and cd.warehouse_id = @warehouse 
   and container_id = @container_id 
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询容器明细");

            return dt.AsEnumerable().ToArray();
        }

        public DataRow GetContainerByBarcode(string barcode)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "container", barcode },
            };
            string sql = @"select 
              d.* from &DataSource&wms_b_container_h h
              left join &DataSource&wms_b_container_d d on h.t_id = d.container_id
             where
               h.client_id='&CLIENT_ID&' and h.org_id='&ORG_ID&'
               and h.warehouse_id = @warehouse 
               and h.act_status = '1'
               and h.container_barcode=@container";

            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询容器明细");

            return dt.Rows[0];
        }
    }
}
