using NtiCloud.Kernel;
using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Inventory;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.QC;
using NtiCloud.Service.WMS.Tasking;
using NtiCloud.Services.WMS.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NtiCloud.Service.WMS.Outbound
{
    public class OutboundOrder : WmsObject, IOutboundOrder
    {
        public void AddDetailProcessedQty(
            string detailID, decimal qty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + GetDetail(detailID)["processed_qty"]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by ", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", detailID },
                { "old_qty", oldQty.ToString() },
                { "qty",  qty.ToString() }
            };
            string sql =
                "update &DataSource&wms_d_outbound_d set modify_date = now(), modify_ip  = @modify_ip," +
                "       last_modified_by = @last_modified_by, processed_qty = ifnull(processed_qty, 0) + @qty " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "     and t_id = @t_id and ifnull(processed_qty, 0) = @old_qty";
            DBHelper.RunSql(sql, param, "更新订单明细已处理量");
        }

        public void AddDetailFinishedQty(
            string detailID, decimal qty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + GetDetail(detailID)["finished_qty"]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", detailID },
                { "old_qty", oldQty.ToString() },
                { "qty",  qty.ToString() }
            };
            string sql =
                "update &DataSource&wms_d_outbound_d set modify_date = now(), modify_ip  = @modify_ip," +
                "       last_modified_by = @last_modified_by, finished_qty = ifnull(finished_qty, 0) + @qty " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "     and t_id = @t_id and ifnull(finished_qty, 0) = @old_qty";
            DBHelper.RunSql(sql, param, "更新订单明细已完成量");
        }

        public void AddDetailFinishedStatQty(
            string detailID, decimal qty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + GetDetail(detailID)["finished_stat_qty"]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", detailID },
                { "old_qty", oldQty.ToString() },
                { "stat_qty",  qty.ToString() }
            };
            string sql =
                "update &DataSource&wms_d_outbound_d set modify_date = now(), modify_ip  = @modify_ip," +
                "       last_modified_by = @last_modified_by, finished_stat_qty = ifnull(finished_stat_qty, 0) + @stat_qty " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "     and t_id = @t_id and ifnull(finished_stat_qty, 0) = @old_qty";
            DBHelper.RunSql(sql, param, "更新订单明细已完成量");
        }

        public void AddPickingQty(string headID, string itemID, decimal qty)
        {
            DataRow[] orderDetails = GetDetails(headID, itemID);

            decimal pickQty = qty;
            foreach (DataRow row in orderDetails)
            {
                decimal planQty = decimal.Parse("0" + row["plan_qty"]);
                decimal processedQty = decimal.Parse("0" + row["processed_qty"]);
                decimal finishedQty = decimal.Parse("0" + row["finished_qty"]);
                decimal leftQty = planQty - finishedQty;

                if (leftQty < pickQty)
                {
                    AddDetailProcessedQty(row["t_id"].ToString(), leftQty, processedQty);
                    AddDetailFinishedQty(row["t_id"].ToString(), leftQty, finishedQty);
                    pickQty -= leftQty;
                }
                else
                {
                    AddDetailProcessedQty(row["t_id"].ToString(), pickQty, processedQty);
                    AddDetailFinishedQty(row["t_id"].ToString(), pickQty, finishedQty);
                    pickQty = 0;
                    break;
                }
            }
            if (pickQty > 0)
            {
                //throw new WmsException(100, "数量超出收货单明细，无法收货!");
                string detailID = orderDetails[orderDetails.Length - 1]["t_id"].ToString();
                AddDetailProcessedQty(detailID, pickQty);
                AddDetailFinishedQty(detailID, pickQty);
            }
        }

        public DataRow GetDetail(string detailID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", detailID },
            };
            string sql;
            sql = "select * from &DataSource&wms_d_outbound_d  " +
                          " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                          "    and warehouse_id = @warehouse " +
                          "    and act_status = '1' " +
                          "    and t_id = @t_id ";

            DataTable dt = DBHelper.Select(sql, param, "查询订单明细");

            return dt.Rows[0];
        }

        public DataRow GetHead(string headNoOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", headNoOrID },
            };
            string sql;
            sql = "select * from &DataSource&wms_d_outbound_h " +
                  " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                  "   and warehouse_id = @warehouse " +
                  "   and act_status = '1' " +
                  "   and (t_id = @t_id or doc_no = @t_id) ";

            DataTable dt = DBHelper.Select(sql, param, "查询订单");

            return dt.Rows[0];
        }

        public DataRow[] GetDetails(string orderIds)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "order_id", orderIds },
            };
            string sql;
            sql = "select * from &DataSource&wms_d_outbound_d " +
                          " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                          "    and act_status = '1' " +
                          "    and instr(@order_id, outbound_h_id) > 0 ";

            DataTable dt = DBHelper.Select(sql, param, "查询出库单明细");

            return dt.AsEnumerable()?.ToArray();
        }

        public DataRow[] GetDetails(string orderID, string itemID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "order_id", orderID },
                { "item_id", itemID }
            };
            string sql;
            sql = "select * from &DataSource&dwms_order_d  " +
                          " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                          "    and whse_id = @warehouse " +
                          "    and order_id = @order_id " +
                          "    and act_status = '1' " +
                          "    and item_id = @item_id ";

            DataTable dt = DBHelper.Select(sql, param, "查询出库单明细");

            return dt.AsEnumerable()?.ToArray();
        }

        public OutboundOrder Insert(OutboundDetailModel model)
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
                { "outbound_h_id", model.OutboundHeadID },
                { "item_id", model.ItemID },
                { "item_name", model.ItemName },
                { "spec", model.Spec },
                { "given_year", model.GivenYear.ToString() },
                { "lot_no", model.LotNo },
                { "owner_lot_no", model.OwnerLotNo },
                { "batches", model.Batches.ToString() },
                { "exist_barcode", model.ExistBarcode },
                { "is_full", model.IsFull },
                { "seq_no", model.SeqNo.ToString() },
                { "owner_id", model.OwnerID },
                { "plan_qty", model.PlanQty.ToString() },
                { "processed_qty", model.ProcessedQty.ToString() },
                { "finished_qty", model.FinishedQty.ToString() },
                { "uom_qty_id", model.UomQtyID },
                { "plan_stat_qty", model.PlanStatQty.ToString() },
                { "processed_stat_qty", model.ProcessedStatQty.ToString() },
                { "finished_stat_qty", model.FinishedStatQty.ToString() },
                { "uom_stat_id", model.UomStatID },
                { "src_factory", model.SrcFactory },
                { "src_inventory", model.SrcInventory },
                { "src_location", model.SrcLocation },
                { "dest_factory", model.DestFactory },
                { "dest_inventory", model.DestInventory },
                { "dest_location", model.DestLocation },
                { "memo", model.Memo },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into &DataSource&WMS_D_OUTBOUND_D ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       outbound_h_id, " +
                "       item_id, " +
                "       item_name, " +
                "       spec, " +
                "       given_year, " +
                "       lot_no, " +
                "       owner_lot_no, " +
                "       batches, " +
                "       exist_barcode, " +
                "       is_full, " +
                "       seq_no, " +
                "       owner_id, " +
                "       plan_qty, " +
                "       processed_qty, " +
                "       finished_qty, " +
                "       uom_qty_id, " +
                "       plan_stat_qty, " +
                "       processed_stat_qty, " +
                "       finished_stat_qty, " +
                "       uom_stat_id, " +
                "       src_factory, " +
                "       src_inventory, " +
                "       src_location, " +
                "       dest_factory, " +
                "       dest_inventory, " +
                "       dest_location, " +
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
                "       @outbound_h_id, " +
                "       @item_id, " +
                "       @item_name, " +
                "       @spec, " +
                "       @given_year, " +
                "       @lot_no, " +
                "       @owner_lot_no, " +
                "       @batches, " +
                "       @exist_barcode, " +
                "       @is_full, " +
                "       @seq_no, " +
                "       @owner_id, " +
                "       @plan_qty, " +
                "       @processed_qty, " +
                "       @finished_qty, " +
                "       @uom_qty_id, " +
                "       @plan_stat_qty, " +
                "       @processed_stat_qty, " +
                "       @finished_stat_qty, " +
                "       @uom_stat_id, " +
                "       @src_factory, " +
                "       @src_inventory, " +
                "       @src_location, " +
                "       @dest_factory, " +
                "       @dest_inventory, " +
                "       @dest_location, " +
                "       @memo, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "出库单的细项");

            return this;
        }
        public void UpdateHeadActStatus(string id, string actStatus, string oldActStatus = null)
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
update &DataSource&wms_d_outbound_h
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       act_status = @state
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id
   and (act_status = @old_state or @old_state = ' ')
";
            DBHelper.RunSql(sql, param, "更新出库单头状态");
        }

        public void UpdateDetailActStatus(string id, string actStatus, string oldActStatus = null)
        {
            oldActStatus ??= " ";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "state", actStatus},
                { "old_state", oldActStatus},
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_d_outbound_d
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       act_status = @state
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id
   and (act_status = @old_state or @old_state = ' ')
";
            DBHelper.RunSql(sql, param, "更新出库单明细状态");
        }

        public void UpdateDetailActStatusByHead(string headID, string actStatus, string oldActStatus = null)
        {
            oldActStatus ??= " ";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "state", actStatus},
                { "old_state", oldActStatus},
                { "h_id", headID }
            };
            string sql = @"
update &DataSource&wms_d_outbound_d
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       act_status = @state
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and outbound_h_id = @h_id
   and (act_status = @old_state or @old_state = ' ')
";
            DBHelper.RunSql(sql, param, "更新出库单明细状态", DBHelper.OperatedNumType.ZeroOrMore);
        }


        public void PickOut(string orderID, string itemID,
            string locationNo, string containerBarcode, decimal qty)
        {

            // 取订单明细
            DataRow[] orderDetails = GetDetails(orderID, itemID);
            // 取托盘
            Container container = new Container();
            DataRow ctnHead = container.GetHead(containerBarcode);
            DataRow ctnDetail = container.GetDetail(ctnHead["t_id"].ToString(), itemID);

            // 取货位
            Location location = new Location();
            //DataRow locRow = location.Get(ctnHead["location_id"].ToString());
            DataRow locRow = location.Get(locationNo);

            // 核对货位
            if (ctnHead["location_id"].ToString() != locRow["t_id"].ToString())
            {
                throw new WmsException(120, "托盘不在货位上");
            }

            // 核对数量
            decimal inventoryQty = decimal.Parse("0" + ctnDetail["qty"]);
            decimal inQty = decimal.Parse("0" + ctnDetail["in_qty"]);
            if (qty > inventoryQty)
            {
                throw new WmsException(121, "捡出数量超过托盘上的数量");
            }

            // 托盘处理
            string ctnHeadID = ctnHead["t_id"].ToString();
            string ctnDetailID = ctnDetail["t_id"].ToString();
            if (qty == inventoryQty && inQty == 0)
            {
                container.DeleteDetail(ctnDetailID);
                inventoryQty = decimal.Parse("0" + ctnHead["qty"]);
                inQty = decimal.Parse("0" + ctnHead["in_qty"]);
                if (qty == inventoryQty && inQty == 0)
                {
                    container.DeleteHead(ctnHeadID);
                }
                else
                {
                    // 扣减托盘数量
                    container.AddHeadQty(ctnHeadID, -qty, decimal.Parse("0" + ctnHead["qty"]));
                }
            }
            else
            {
                // 扣减托盘数量
                container.AddDetailQty(ctnDetailID, -qty, decimal.Parse("0" + ctnDetail["qty"]));
                container.AddHeadQty(ctnHeadID, -qty, decimal.Parse("0" + ctnHead["qty"]));
            }

            // 货位处理
            if (location.IsLocationEmpty(locRow["t_id"].ToString()))
            {
                location.UpdateStatus(locRow["t_id"].ToString(),
                    "LocationState_Empty", locRow["location_state"].ToString());
            }

            // 添加任务
            DataRow orderHead = GetHead(orderID);
            new WorkTask().Insert(new WorkTaskModel()
            {
                TaskType = "Task_Picking",
                TaskNo = WmsHelper.GetSequence("TaskNo"),
                Priority = 100,
                //ContainerDetailID = ctnDetailID,
                RefDocType = "Inbound",
                SrcAddrID = locRow["t_id"].ToString(),
                SrcAddrType = "Address_Rack",
                SrcAddrText = locRow["location_no"].ToString(),
                //SourceBillHeadID = orderID,
                RefDocNo = orderHead["order_no"].ToString(),
                //OperationQty = decimal.Parse("0" + qty),
                TaskStatus = "TaskStatus_Finished",
                //ItemID = itemID
            });
            // TODO 处理task_item表

            // 订单数量处理
            AddPickingQty(orderID, itemID, qty);
        }

        /*
        private void LockInventory(DataRow row, DataRow[] details, decimal planQty)
        {
            decimal actualQty = details.Sum(r => decimal.Parse("0" + r["actual_qty"]));
            decimal inQty = details.Sum(r => decimal.Parse("0" + r["in_qty"]));
            decimal outQty = details.Sum(r => decimal.Parse("0" + r["out_qty"]));
            decimal lockQty = details.Sum(r => decimal.Parse("0" + r["lock_qty"]));
            decimal availQty = actualQty - outQty - lockQty;
            decimal qty = 0;
            decimal leftQty = planQty;
            DataRow locRow = Location.Get(locationID);
            DataRow containerRow = Container.GetHead(locRow["unit_no"].ToString());

            if (leftQty > availQty)
            {
                leftQty -= availQty;
                qty = availQty;
            }
            else
            {
                qty = leftQty;
                leftQty = 0;
            }

            DataRow[] ctnDtlRow = details.Where(r
                => r["item_id"].ToString() == row["item_id"].ToString()
                && r["isa_b_inbound_hid"].ToString() == r["isa_b_inbound_h_id"].ToString()
                ).ToArray();

            if (actualQty - qty == 0 && inQty == 0)
            {
                // 插入容器明细
                Container.Insert(new ContainerDetailModel()
                {
                    ID = ctnDetailID,
                    ZoneID = locRow["zone_id"].ToString(),
                    ContainerID = containerRow["isa_bi_container_id"].ToString(),
                    InboundHeadID = row["isa_b_inbound_h_id"].ToString(),
                    ItemID = row["item_id"].ToString(),
                    ItemName = row["item_name"].ToString(),
                    ItemInStatus = "Peng-In",
                    QCStatus = "Pend-Inspec",
                    ActualQty = 0,
                    FinanceQty = 0,
                    InQty = qty,
                    UomQtyID = row["isa_wi_uom_qtyid"].ToString(),
                    UomFinanceID = row["isa_wi_uom_financeid"].ToString(),
                    Ratio = DBHelper.GetDecimal(row["ratio"]),
                    FirstInboundDate = DateTime.Now,
                });
            }
            else
            {
                Container.AddDetailInQty(ctnDtlRow[0]["t_id"].ToString(),
                    qty, decimal.Parse("0" + ctnDtlRow[0]["in_qty"]));
            }
            WorkTask.Insert(new WorkTaskModel()
            {
                ZoneID = row["zone_id"].ToString(),
                TaskType = "Task_Inbound",
                TaskNo = WmsHelper.GetSequence("TaskNo"),
                OperationQty = qty,
                SeqNo = 0,
                ItemID = row["item_id"].ToString(),
                ContainerDetailID = ctnDetailID,
                ContainerBarcode = containerRow["container_barcode"].ToString(),
                RefDocType = "Doc_InBound",
                RefDocNo = orderRow["doc_no"].ToString(),
                RefDocID = orderRow["t_id"].ToString(),
                DestAddrID = locRow["t_id"].ToString(),
                DestAddrType = "Location",
                DestAddrText = locRow["unit_code"].ToString(),
                // todo
                TaskStatus = "TaskStatus_NotExecute",
                IsExecutable = "0"
            });
        }
        */

        public void PreallocateInventoryWithWave(string orderNoOrID)
        {
            if (string.IsNullOrWhiteSpace(orderNoOrID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }

            DataRow orderRow = GetHead(orderNoOrID);
            if (orderRow[oh.OutboundStatus].ToString() != OutDocStatus.OutDocStatus_New.ToString())
            {
                throw new WmsException(500, "出库单只有在新建状态下才能预分配");
            }

            //UpdateOrderHeadState(orderRow[oh.ID].ToString(), 
            //    OutDocStatus.OutDocStatus_InWave.ToString(), 
            //    orderRow[oh.OutboundStatus].ToString());

            string waveNo = WmsHelper.GetSequence("WaveNo");
            string orderID = orderRow[oh.ID].ToString(); 

            //string waveNo = "T-20200611-00020";

            Wave.CreateWave(waveNo, orderID);
            Wave.UpdateWaveType(waveNo, WaveType.WaveType_AutoFromOrder.ToString());

            //Wave.ComputeWave(waveNo, "1");
            Wave.ComputeWave(waveNo, "1");

            UpdateOrderHeadState(orderID, 
                OutDocStatus.OutDocStatus_Preallocation.ToString(),
                OutDocStatus.OutDocStatus_InWave.ToString());
        }

        public void PreallocateInventory(string orderNoOrID)
        {
            //var configJson = WmsConfig.GetJsonConfig();
            //LogHelper.LogDebug(configJson.GetSection("Receving").Value);

            DataRow orderRow = GetHead(orderNoOrID);

            if (orderRow["outbound_status"].ToString() != "OutDocStatus_New")
            {
                throw new WmsException(-200, "出库单状态不正确！");
            }

            UpdateOrderHeadState(orderRow["t_id"].ToString(),
                OutDocStatus.OutDocStatus_Preallocation.ToString(), orderRow["outbound_status"].ToString());

            DataRow[] detailRows = GetDetails(orderRow["t_id"].ToString());

            foreach (DataRow row in detailRows)
            {
                DataRow itemRow = Item.Get(row["item_id"].ToString());
                decimal leftQty = decimal.Parse("0" + row["plan_qty"]);
                string zoneID = null;

                while (leftQty > 0)
                {
                    //string locationID = LocationAllocator.AllocateLocation(
                    //    Context.WarehouseID, row["item_id"].ToString(),
                    //    "Location_TaskInBound", "UserDefined_WaveLocationAssign_Mode1",
                    //    "CheckState_CheckAvoid;CheckState_CheckPassed"
                    //);
                    /*
                    string locationID = LocationAllocator.AllocateInventory(
                        Context.WarehouseID, row["item_id"].ToString(),
                        row["exist_barcode"].ToString());
                        */
                    string locationID = LocationAllocator.AllocateInventory(
                        new Dictionary<string, object>()
                        {
                            { "I_WAREHOUSEID", row["warehouse_id"].ToString() },
                            { "I_ITEMID", row["item_id"].ToString() },
                            { "I_BARCODE", row["exist_barcode"].ToString() },
                            { "I_BATCH_NO", row["lot_no"].ToString() },
                            { "I_LOT_NO", row["lot_no"].ToString() },
                            { "I_SPECI", row["speci"].ToString() },
                            { "I_GIVENYEAR", row["givenyear"].ToString() },
                            { "I_IS_FULL", row["is_full"].ToString() },
                            { "I_FINANCE_QTY", row["plan_finance_qty"].ToString() },
                            { "I_LASTLOCATION_NO", "" },
                            { "I_FRACTION", "" },
                            { "I_STORAGE_LOCGP1", "" },
                            { "I_STORAGE_LOCGP2", "" },
                            { "I_EXPIRY_DATE", "" },
                            { "I_ZONE", "" },
                            { "I_HEADID", "" },
                            { "I_DETAILID", "" },
                            { "I_PACKING_TYPE", "" },
                        }, WmsConfig.GetOrDefault("Outbound:AllocationRule", "work_task_4"));


                    DataRow locRow = Location.Get(locationID);

                    if (zoneID == null)
                    {
                        zoneID = locRow["zone_id"].ToString();
                    }

                    leftQty = AddOutboundTask(orderRow, row, locRow, leftQty);
                }
            }
        }


        private void CreateQCDoc(DataRow orderRow)
        {
            string qcID = SnowflakeID.Guid().ToString();
            OutboundQC.Insert(new OutboundQCHeadModel()
            {
                ID = qcID,
                //ZoneID = zoneID,
                QcoutDocNo = WmsHelper.GetSequence("QCOutNo"),
                OutboundHid = orderRow["t_id"].ToString(),
                ServiceType = orderRow["outservice_type"].ToString(),
                QcoutStatus = "QCOutStatus_New",
                Status = "0",
                IsAutoCreated = "0",
                ActStatus = "1",
            });

            DataRow[] taskRows = WorkTask.GetByRefDocNoOrID("Order", orderRow["doc_no"].ToString());
            foreach (DataRow row in taskRows)
            {
                DataRow orderDetailRow = GetDetail(row["source_doc_id"].ToString());
                decimal spec = decimal.Parse("0" + orderDetailRow["speci"]);
                decimal opQty = decimal.Parse("0" + row["operation_qty"]);

                OutboundQC.Insert(new OutboundQCDetailModel()
                {
                    ZoneID = row["zone_id"].ToString(),
                    QCOutHeadID = qcID,
                    ItemID = row["item_id"].ToString(),
                    ItemName = orderDetailRow["item_name"].ToString(),
                    LocationID = row["source_address_id"].ToString(),
                    //FinanceQty = opQty * spec,
                    FinanceQty = decimal.Parse("0" + row["finance_qty"]),
                    UomFinanceID = orderDetailRow["isa_wi_uom_financeid"].ToString(),
                    PlanQty = opQty,
                    UomQtyID = orderDetailRow["isa_wi_uom_qtyid"].ToString(),
                    Spec = spec.ToString(),
                    ActStatus = "1",
                    OutboundDetailID = orderDetailRow["t_id"].ToString(),
                });
            }
        }

        private readonly string checkQtyWhenRelease = WmsConfig.GetOrDefault("Outbound:CheckQtyWhenRelease", "1");
        public void ConfirmPreallocation(string orderNoOrID, out WebJsonInfo info)
        {
            if (string.IsNullOrWhiteSpace(orderNoOrID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow orderRow = GetHead(orderNoOrID);
            if (orderRow["outbound_status"].ToString() != OutDocStatus.OutDocStatus_Preallocation.ToString())
            {
                throw new WmsException(200, "出库单状态不正确！");
            }
            DataRow[] waveDetailRows = Wave.GetDetailsByOrderID(orderRow[oh.ID].ToString());
            if (waveDetailRows.Select(r => r[wd.OutboundHeadID].ToString()).Distinct().Count() > 1)
            {
                throw new WmsException(500, "所选订单所属波次具有多个订单，无法在此确认");
            }
            var waveHeadRow = Wave.GetHeadByOrderID(orderRow["t_id"].ToString());
            if (waveHeadRow[wh.WaveType].ToString() != WaveType.WaveType_AutoFromOrder.ToString())
            {
                throw new WmsException(500, "无法在出库单维护中进行确认，请使用波次释放");
            }

            if (checkQtyWhenRelease == "1")
            {
                CheckQtyWithTask(orderRow);
            }

            Wave.ReleaseWave(waveDetailRows[0][wd.WaveHeadID].ToString(), "", out info);

            string outboundFromDock = WmsConfig.GetOrDefault("Outbound:OutboundFromDock", "0");
            string zoneID = string.Empty;
            DataRow dockRow = null;
            if (outboundFromDock == "1")
            {
                // 分配月台
                DataRow idRow = WorkTask.GetMainDockByTasks(orderRow["doc_no"].ToString());
                dockRow = Dock.GetDock(idRow["t_id"].ToString());
                zoneID = dockRow["zone_id"].ToString();
                //orderRow["m_dock_id"] = dockRow["t_id"];
                //UpdateHeadDock(orderRow["t_id"].ToString(), dockRow["t_id"].ToString());
            }

            string needCreate = WmsConfig.GetOrDefault("Outbound:GenerateOutboundQCBill", "0");
            if (needCreate == "1")
            {
                CreateQCDoc(orderRow);
            }

            string carLogin = WmsConfig.GetOrDefault("Outbound:CarLogin", "0");
            if (outboundFromDock == "1" && carLogin == "1")
            {
                Login4Car(orderRow, dockRow);
            }
        }

        private void CheckQtyWithTask(DataRow orderRow)
        {
            // 检查订单数量和任务中的数量是否一致
            DataRow[] taskItemRows = WorkTaskItem.GetByOrderNoOrID(orderRow["t_id"].ToString());
            DataRow[] orderDtlRows = GetDetails(orderRow["t_id"].ToString());
            decimal detailTotal;
            decimal itemTotal;
            foreach (DataRow orderDtlRow in orderDtlRows)
            {
                //var waveDetailRow = Wave.GetDetail
                detailTotal = decimal.Parse("0" + orderDtlRow["plan_qty"]);
                UomModel model = new UomModel()
                {
                    Qty = decimal.Parse("0" + orderDtlRow["plan_qty"]),
                    UomID = orderDtlRow["uom_qty_id"].ToString()
                };
                decimal spec = orderDtlRow["spec"] == null ? 1m : decimal.Parse("0" + orderDtlRow["spec"]);
                model = ItemUom.ConvertToDefaultPackUom(model, spec);
                itemTotal = taskItemRows
                    .Where(r => r["outbound_d_id"].ToString() == orderDtlRow["t_id"].ToString()
                                && string.IsNullOrWhiteSpace(r[t.DependentTaskNo].ToString()))
                    .Sum(r => decimal.Parse("0" + r["plan_qty"]));
                if (model.Qty != itemTotal)
                {
                    LogHelper.LogInformation($"order[{model.Qty}], TaskItem[{itemTotal}]");
                    throw new WmsException(230, orderDtlRow["item_name"] + "预分配的数量与出库明细数量不符");
                }
            }
        }

        public void ConfirmPreallocationV1(string orderNoOrID)
        {
            DataRow orderRow = GetHead(orderNoOrID);

            if (orderRow["outbound_status"].ToString() != OutDocStatus.OutDocStatus_Preallocation.ToString())
            {
                throw new WmsException(200, "出库单状态不正确！");
            }

            UpdateOrderHeadState(orderRow["t_id"].ToString(),
                        OutDocStatus.OutDocStatus_Allocated.ToString(), orderRow["outbound_status"].ToString());

            var waveHeadRow = Wave.GetHeadByOrderID(orderRow["t_id"].ToString());
            DataRow[] taskItemRows = WorkTaskItem.GetByOrderNoOrID(orderRow["t_id"].ToString());
            DataRow[] orderDtlRows = GetDetails(orderRow["t_id"].ToString());
            decimal detailTotal;
            decimal itemTotal;
            // 这样比较现在没有意义了，因为计量单位不一致
            //decimal detailTotal = orderDtlRows.Sum(r => decimal.Parse("0" + r["plan_qty"]));
            //decimal itemTotal = taskItemRows.Sum(r => decimal.Parse("0" + r["plan_qty"]));
            //if (detailTotal != itemTotal)
            //{
            //    throw new WmsException(230, "预分配的数量与入库计划数量不符");
            //}

            foreach (DataRow orderDtlRow in orderDtlRows)
            {
                //var waveDetailRow = Wave.GetDetail
                detailTotal = decimal.Parse("0" + orderDtlRow["plan_qty"]);
                UomModel model = new UomModel()
                {
                    Qty = decimal.Parse("0" + orderDtlRow["plan_qty"]),
                    UomID = orderDtlRow["uom_qty_id"].ToString()
                };
                decimal spec = orderDtlRow["spec"] == null ? 1m : decimal.Parse("0" + orderDtlRow["spec"]);
                model = ItemUom.ConvertToDefaultPackUom(model, spec);
                itemTotal = taskItemRows
                    .Where(r => r["outbound_d_id"].ToString() == orderDtlRow["t_id"].ToString()
                                && string.IsNullOrWhiteSpace(r[t.DependentTaskNo].ToString()))
                    .Sum(r => decimal.Parse("0" + r["plan_qty"]));
                if (model.Qty != itemTotal)
                {
                    LogHelper.LogInformation($"order[{model.Qty}], TaskItem[{itemTotal}]");
                    throw new WmsException(230, orderDtlRow["item_name"] + "预分配的数量与入库明细数量不符");
                }
            }

            DataRow[] taskRows = WorkTask.GetByOrderNoOrID(orderRow["t_id"].ToString());
            foreach (DataRow taskRow in taskRows)
            {
                if (taskRow["task_status"].ToString() != TaskStatus.TaskStatus_Create.ToString())
                {
                    throw new WmsException(200, $"任务{taskRow["task_no"]}状态不正确");
                }
                WorkTask.UpdateTaskStatus(taskRow["t_id"].ToString(),
                    TaskStatus.TaskStatus_NotExecute.ToString(),
                    taskRow["task_status"].ToString());
                if (taskRow["dependent_task_no"].ToString().Length <= 0)
                {
                    WorkTask.UpdateIsExecutable(taskRow["t_id"].ToString(), "1");
                }
            }

            string outboundFromDock = WmsConfig.GetOrDefault("Outbound:OutboundFromDock", "0");
            string zoneID = string.Empty;
            DataRow dockRow = null;
            if (outboundFromDock == "1")
            {
                // 分配月台
                DataRow idRow = WorkTask.GetMainDockByTasks(orderRow["doc_no"].ToString());
                dockRow = Dock.GetDock(idRow["t_id"].ToString());
                zoneID = dockRow["zone_id"].ToString();
                //orderRow["m_dock_id"] = dockRow["t_id"];
                //UpdateHeadDock(orderRow["t_id"].ToString(), dockRow["t_id"].ToString());
            }

            /*
            string serviceType = receiptRow["service_type"].ToString();
            if (serviceType == "InServiceType_PurchasingIn"
                || serviceType == "InServiceType_OutsourcingIn")
            {
                CreateQCDoc(receiptRow, dockRow);
            }
            */
            string needCreate = WmsConfig.GetOrDefault("Outbound:GenerateOutboundQCBill", "0");
            if (needCreate == "1")
            {
                CreateQCDoc(orderRow);
            }

            string carLogin = WmsConfig.GetOrDefault("Outbound:CarLogin", "0");
            if (outboundFromDock == "1" && carLogin == "1")
            {
                Login4Car(orderRow, dockRow);
            }
        }

        public void CancelConfirmPreallocation(string orderNoOrID)
        {
            if (string.IsNullOrWhiteSpace(orderNoOrID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow orderRow = GetHead(orderNoOrID);

            if (orderRow[oh.OutboundStatus].ToString() != OutDocStatus.OutDocStatus_Allocated.ToString())
            {
                throw new WmsException(200, "出库单状态不正确！");
            }

            DataRow waveRow = Wave.GetHeadByOrderID(orderRow["t_id"].ToString());
            Wave.CancelRelease(waveRow["t_id"].ToString());
        }

        private void Login4Car(DataRow headRow, DataRow dockRow)
        {
            throw new NotImplementedException();
            /*
            string seqName = "ISA_CarLogin" + dockRow["dock_code"].ToString().Substring(0, 1);
            CarLoginModel model = new CarLoginModel()
            {
                LicensePlate = headRow["LICENSE_PLATE"].ToString(),
                InboundDocnoID = headRow["t_id"].ToString(),
                DocNo = headRow["doc_no"].ToString(),
                SignState = "SignState_Allocation",
                AllocationTime = DateTime.Now,
                AllocationNo = WmsHelper.GetSequence(seqName),
                DockID = dockRow["t_id"].ToString(),
                ZoneID = dockRow["zone_id"].ToString(),
                DriverName = headRow["deliver"].ToString(),
                DriverMobile = headRow["deliver_tel"].ToString(),
                Type = "2",
            };

            new CarLogin().Insert(model);
            */
        }

        public void CancelInventoryPreallocation(string orderNoOrID)
        {
            if (string.IsNullOrWhiteSpace(orderNoOrID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow orderRow = GetHead(orderNoOrID);

            if (orderRow["outbound_status"].ToString() != OutDocStatus.OutDocStatus_Preallocation.ToString())
            {
                throw new WmsException(100, "出库单状态不正确！");
            }

            UpdateOrderHeadState(orderRow["t_id"].ToString(),
                OutDocStatus.OutDocStatus_New.ToString(),
                orderRow["outbound_status"].ToString());

            DataRow waveHeadRow = Wave.GetHeadByOrderID(orderRow["t_id"].ToString());

            /*
            DataRow[] taskRows = WorkTask.GetByRefDocID(waveHeadRow["t_id"].ToString());

            foreach (DataRow taskRow in taskRows)
            {
                WorkTask.RecycleTask(taskRow);
                //RemoveAllocatedTask(taskRow);
            }
            */

            Wave.RemoveWave(waveHeadRow["t_id"].ToString());
        }

        private void RemoveAllocatedTask2(DataRow taskRow)
        {
            if (taskRow["task_status"].ToString() != TaskStatus.TaskStatus_Create.ToString())
            {
                throw new WmsException(100, "预分配任务状态不正确！无法删除!");
            }

            Container.CancelStockingOut(
                taskRow["isa_bi_container_detail_id"].ToString(),
                decimal.Parse("0" + taskRow["operation_qty"]));

            WorkTask.DeleteWorkTask(taskRow["t_id"].ToString());

        }

        /*
        private void RemoveTaskItem(DataRow taskItemRow)
        {
            DataRow[] rowsInSameTask = WorkTask.GetTaskItemsBy(taskItemRow[ti.TaskID].ToString());
            if (rowsInSameTask.Length == 1)
            {
                WorkTask.RecycleTask(WorkTask.Get(taskItemRow[ti.TaskID].ToString()));
                return;
            }

            DataRow taskRow = WorkTask.Get(taskItemRow[ti.TaskID].ToString());
            WorkTask.RemoveTaskItem(taskRow, taskItemRow);

            DataRow[] dependentRows = WorkTask.GetDependentTasks(taskRow["task_no"].ToString());
            foreach (DataRow row in dependentRows)
            {
                taskItemRow = WorkTask.GetTaskItemBy();
                RecycleWaveTypeTask(row);
            }

            // TODO

            Container.CancelStockingOut(
                taskItemRow["isa_bi_container_detail_id"].ToString(),
                decimal.Parse("0" + taskItemRow["operation_qty"]));

            WorkTask.DeleteWorkTask(taskItemRow["t_id"].ToString());

        }
        */

        private void RemoveAllocatedTask(DataRow taskRow)
        {
            if (taskRow["task_status"].ToString() != TaskStatus.TaskStatus_Create.ToString())
            {
                throw new WmsException(100, "预分配任务状态不正确！无法删除!");
            }

            Container.CancelStockingOut(
                taskRow["isa_bi_container_detail_id"].ToString(),
                decimal.Parse("0" + taskRow["operation_qty"]));

            WorkTask.DeleteWorkTask(taskRow["t_id"].ToString());
        }

        public IOutboundOrder RemoveOutboundTaskItems(string taskItemIds)
        {
            if (string.IsNullOrWhiteSpace(taskItemIds))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow[] taskItemRows = WorkTask.GetTaskItems(taskItemIds);

            foreach (DataRow taskItem in taskItemRows)
            {
                DataRow[] waveDetailRows = Wave.GetWaveDetails(taskItem[ti.RefDocDetailID].ToString());
                DataRow orderRow = GetHead(waveDetailRows[0][wd.OutboundHeadID].ToString());
                if (orderRow[oh.OutboundStatus].ToString() != OutDocStatus.OutDocStatus_Preallocation.ToString())
                {
                    throw new WmsException(500, "单据状态不允许删除!");
                }

                #region new
                DataRow[] tasks = WorkTask.GetDependentTasks(WorkTask.Get(taskItem[ti.TaskID].ToString()));
                tasks = tasks.OrderByDescending(r => r["t_id"].ToString()).ToArray();

                DataRow[] rowsInSameTask = WorkTask.GetTaskItemsBy(taskItem[ti.TaskID].ToString());
                if (rowsInSameTask.Length == 1)
                {
                    //WorkTask.RecycleTask(WorkTask.Get(taskItem[ti.TaskID].ToString()));
                    //WorkTask.RecycleTasks(tasks);
                    WorkTask.RemoveWaveTasks(tasks);
                    continue;
                }

                if (ExistsPickingTask(tasks))
                {
                    WorkTask.RemoveTaskItem(taskItem);
                    continue;
                }

                //WorkTask.RecycleTask(WorkTask.Get(taskItem[ti.TaskID].ToString()));
                //WorkTask.RecycleTasks(tasks);
                WorkTask.RemoveWaveTasks(tasks);

                rowsInSameTask = rowsInSameTask.Where(
                    r => r[ti.ID].ToString() != taskItem[ti.ID].ToString()).ToArray();

                Route route = new Route();
                route.Init(Item.Get(
                    taskItem[ti.ItemID].ToString())["catalog_id"].ToString(),
                    RoutePurpose.RoutePurpose_Picking.ToString());

                Address src = new Address()
                {
                    ID = tasks.FirstOrDefault()[t.SrcAddrID].ToString(),
                    Type = tasks.FirstOrDefault()[t.SrcAddrType].ToString(),
                    Text = tasks.FirstOrDefault()[t.SrcAddrText].ToString(),
                };
                Address dest = new Address()
                {
                    ID = tasks.LastOrDefault()[t.DestAddrID].ToString(),
                    Type = tasks.LastOrDefault()[t.DestAddrType].ToString(),
                    Text = tasks.LastOrDefault()[t.DestAddrText].ToString(),
                };

                if (!route.MatchRoute(src, dest, out DataRow[] sequences))
                {
                    throw new WmsException(500, "无法找到合适的配置来生成任务");
                }


                decimal taskNum = WorkTask.SaveTask(sequences,
                    new WorkTaskModel()
                    {
                        SeqNo = DBHelper.GetDecimal(tasks.FirstOrDefault()[t.SeqNo]),
                        Priority = DBHelper.GetDecimal(tasks.FirstOrDefault()[t.Priority]),
                        ContainerID = tasks.FirstOrDefault()[t.ContainerID].ToString(),
                        ContainerBarcode = tasks.FirstOrDefault()[t.ContainerBarcode].ToString(),
                        RefDocID = tasks.FirstOrDefault()[t.RefDocID].ToString(),
                        RefDocNo = tasks.FirstOrDefault()[t.RefDocNo].ToString(),
                        RefDocType = tasks.FirstOrDefault()[t.RefDocType].ToString(),
                        SrcAddrID = tasks.FirstOrDefault()[t.SrcAddrID].ToString(),
                        SrcAddrType = tasks.FirstOrDefault()[t.SrcAddrType].ToString(),
                        SrcAddrText = tasks.FirstOrDefault()[t.SrcAddrText].ToString(),
                        DestAddrID = tasks.LastOrDefault()[t.DestAddrID].ToString(),
                        DestAddrType = tasks.LastOrDefault()[t.DestAddrType].ToString(),
                        DestAddrText = tasks.LastOrDefault()[t.DestAddrText].ToString(),
                    }, GetWorkItemList(rowsInSameTask));

                rowsInSameTask.ToList().ForEach(r =>
                    Wave.AddDetailProcessedQty(r[ti.RefDocDetailID].ToString(), 
                        decimal.Parse("0" + r[ti.PlanQty]))
                );

                Wave.AddHeadLeftTaskQty(tasks.FirstOrDefault()[t.RefDocID].ToString(), taskNum);

                #endregion

                //WorkTask.RemoveTaskItem(taskItem);
            }

            // 如果波次中没有任务了，将波次也删除
            List<string> refDocIds = taskItemRows
                .Select(r => r[ti.RefDocID].ToString()).Distinct().ToList();
            foreach (string waveID in refDocIds)
            {
                DataRow[] rows = WorkTask.GetByRefDocID(waveID);
                if (rows.Length == 0)
                {
                    Wave.UpdateHeadStatus(waveID, WaveStatus.WaveStatus_New.ToString(),
                        WaveStatus.WaveStatus_Editable.ToString());
                    Wave.DeleteWave(waveID);
                }
            }

            return this;
        }

        private List<WorkTaskItemModel> GetWorkItemList(DataRow[] rowsInSameTask)
        {
            List<WorkTaskItemModel> list = new List<WorkTaskItemModel>();
            foreach (DataRow row in rowsInSameTask)
            {
                list.Add(
                    new WorkTaskItemModel()
                    {
                        ItemID = row[ti.ItemID].ToString(),
                        ItemName = row[ti.ItemName].ToString(),
                        PlanQty = DBHelper.GetDecimal(row[ti.PlanQty]),
                        ProcessedQty = DBHelper.GetDecimal(row[ti.ProcessedQty]),
                        FinishedQty = DBHelper.GetDecimal(row[ti.FinishedQty]),
                        FinishedStatQty = DBHelper.GetDecimal(row[ti.FinishedStatQty]),
                        UomQtyID = row[ti.UomQtyID].ToString(),
                        UomStatID = row[ti.UomStatID].ToString(),
                        ContainerDetailID = row[ti.ContainerDetailID].ToString(),
                        RefDocDetailID = row[ti.RefDocDetailID].ToString(),
                        RefDocType = row[ti.RefDocType].ToString(),
                        RefDocID = row[ti.RefDocID].ToString(),
                        RefDocNo = row[ti.RefDocNo].ToString(),
                        ProcessStatus = row[ti.ProcessStatus].ToString(),
                    });
            }
            return list;
        }

        private bool ExistsPickingTask(DataRow[] tasks)
        {
            return tasks.Where(r => r[t.TaskType].ToString()
                        == TaskType.TaskType_Picking.ToString()).Count() > 0;
        }

        public IOutboundOrder RemoveOutboundTasks(string taskIds)
        {
            if (string.IsNullOrWhiteSpace(taskIds))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow[] taskRows = WorkTask.GetWorkTasks(taskIds);

            foreach (DataRow taskRow in taskRows)
            {
                RemoveAllocatedTask(taskRow);
            }
            LogHelper.LogInformation("删除波次任务!");

            return this;
        }

        public void AllocateInventory(string orderNoOrID)
        {
            if (string.IsNullOrWhiteSpace(orderNoOrID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            //var configJson = WmsConfig.GetJsonConfig();
            //LogHelper.LogDebug(configJson.GetSection("Receving").Value);

            DataRow orderRow = GetHead(orderNoOrID);

            if (orderRow["outbound_status"].ToString() != "OutDocStatus_New")
            {
                throw new WmsException(-200, "出库单状态不正确！");
            }

            UpdateOrderHeadState(orderRow["t_id"].ToString(),
                OutDocStatus.OutDocStatus_Allocated.ToString(), orderRow["outbound_status"].ToString());

            DataRow[] detailRows = GetDetails(orderRow["t_id"].ToString());

            string qcID = SnowflakeID.Guid().ToString();
            OutboundQC.Insert(new OutboundQCHeadModel()
            {
                ID = qcID,
                //ZoneID = zoneID,
                QcoutDocNo = WmsHelper.GetSequence("QCOutNo"),
                OutboundHid = orderRow["t_id"].ToString(),
                ServiceType = orderRow["outservice_type"].ToString(),
                QcoutStatus = "QCOutStatus_New",
                Status = "0",
                IsAutoCreated = "0",
                ActStatus = "1",
            });

            foreach (DataRow row in detailRows)
            {
                DataRow itemRow = Item.Get(row["item_id"].ToString());
                decimal leftQty = decimal.Parse("0" + row["plan_qty"]);
                string zoneID = null;

                while (leftQty > 0)
                {
                    //string locationID = LocationAllocator.AllocateLocation(
                    //    Context.WarehouseID, row["item_id"].ToString(),
                    //    "Location_TaskInBound", "UserDefined_WaveLocationAssign_Mode1",
                    //    "CheckState_CheckAvoid;CheckState_CheckPassed"
                    //);
                    string locationID = LocationAllocator.AllocateInventory(
                        Context.WarehouseID, row["item_id"].ToString(),
                        row["exist_barcode"].ToString());

                    DataRow locRow = Location.Get(locationID);

                    if (zoneID == null)
                    {
                        zoneID = locRow["zone_id"].ToString();
                    }

                    leftQty = AddOutboundTaskV1(orderRow, row, locRow, leftQty, qcID);
                }

                // 生成出库质检明细
                /* 移到添加出库任务AddOutboundTask里面去
                OutboundQC.Insert(new OutboundQCDetailModel()
                {
                    ZoneID = zoneID,
                    QCOutHeadID = qcID,
                    ItemID = row["item_id"].ToString(),
                    ItemName = row["item_name"].ToString(),
                    LocationID = "",
                    FinanceQty = decimal.Parse("0" + row["plan_finance_qty"]),
                    UomFinanceID = row["isa_wi_uom_financeid"].ToString(),
                    PlanQty = decimal.Parse("0" + row["plan_qty"]),
                    UomQtyID = row["isa_wi_uom_qtyid"].ToString(),
                    Spec = row["speci"].ToString(),
                    ActStatus = "1",
                });
                */
            }
        }

        public IOutboundOrder AddOutboundTaskV1(string orderDetailID,
            string locationNoOrID, decimal qty)
        {
            DataRow orderDetailRow = GetDetail(orderDetailID);
            DataRow orderRow = GetHead(orderDetailRow["ISA_B_OUTBOUND_HID"].ToString());
            DataRow locRow = Location.Get(locationNoOrID);

            decimal leftQty = AddOutboundTask(orderRow, orderDetailRow, locRow, qty);

            if (leftQty > 0)
            {
                throw new WmsException(240, "库存数量不足，程序无法处理!");
            }
            return this;
        }

        public IOutboundOrder AddOutboundTask(string orderDetailID,
            string locationNoOrID, decimal qty)
        {
            if (string.IsNullOrWhiteSpace(locationNoOrID)
                || string.IsNullOrWhiteSpace(orderDetailID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }

            DataRow orderDetailRow = GetDetail(orderDetailID);
            DataRow orderRow = GetHead(orderDetailRow[od.OutboundHeadID].ToString());
            string status = orderRow[oh.OutboundStatus].ToString();
            if (status == OutDocStatus.OutDocStatus_New.ToString())
            {
                UpdateOrderHeadState(orderRow[oh.ID].ToString(),
                    OutDocStatus.OutDocStatus_Preallocation.ToString(),
                    OutDocStatus.OutDocStatus_New.ToString());
                string waveNo = WmsHelper.GetSequence("WaveNo");
                Wave.CreateWave(waveNo, orderRow[oh.ID].ToString());
                Wave.UpdateWaveType(waveNo, WaveType.WaveType_AutoFromOrder.ToString());
                Wave.UpdateHeadStatus(waveNo, WaveStatus.WaveStatus_Editable.ToString(),
                    WaveStatus.WaveStatus_New.ToString());
            }
            else if (status != OutDocStatus.OutDocStatus_Preallocation.ToString())
            {
                throw new WmsException(500, "出库单状态不允许预分配！");
            }
            DataRow locRow = Location.Get(locationNoOrID);
            if (locRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString()
                && locRow[l.LocationStatus].ToString() != LocationStatus.LocationStatus_Stored.ToString())
            {
                throw new WmsException(500, "立体库货位状态不允许预分配！");
            }

            decimal leftQty = AddOutboundTask(orderRow, orderDetailRow, locRow, qty);
            UpdateOrderHeadState(orderRow[oh.ID].ToString(), OutDocStatus.OutDocStatus_Preallocation.ToString());

            if (leftQty > 0)
            {
                throw new WmsException(240, "库存数量不足，程序无法处理!");
            }
            return this;
        }

        private decimal AddOutboundTask(DataRow orderRow,
            DataRow orderDetailRow, DataRow locRow, decimal leftQty)
        {
            DataRow[] waveDetailRows = Wave.GetDetailsByOrderDetailID(orderDetailRow[od.ID].ToString());
            if (waveDetailRows.Length != 1)
            {
                throw new WmsException(500, "手工分配货位出错");
            }

            UomModel qtyUomModel = new UomModel()
            {
                Qty = leftQty,
                UomID = orderDetailRow["uom_qty_id"].ToString()
            };
            qtyUomModel = ItemUom.ConvertToDefaultPackUom(qtyUomModel);
            leftQty = qtyUomModel.Qty;

            return Wave.AddWaveTask(new WorkTaskModel()
            {
                RefDocDetailID = waveDetailRows[0][wd.ID].ToString(),
                SrcAddrID = locRow[l.ID].ToString(),
                DestAddrID = orderRow[oh.DestAddrID].ToString(),
                DestAddrType = orderRow[oh.DestAddrType].ToString(),
                DestAddrText = orderRow[oh.DestAddrText].ToString(),
            }, leftQty);
        }

        private decimal AddOutboundTaskV4(DataRow orderRow,
            DataRow orderDetailRow, DataRow locRow, decimal leftQty)
        {
            DataRow[] waveDetailRows = Wave.GetDetailsByOrderDetailID(orderDetailRow[od.ID].ToString());
            if (waveDetailRows.Length != 1)
            {
                throw new WmsException(500, "手工分配货位出错");
            }
            //decimal planQty = decimal.Parse("0" + orderDetailRow["plan_qty"]);
            //decimal processedQty = decimal.Parse("0" + orderDetailRow[od.ProcessedQty]);
            //if (leftQty + processedQty > planQty)
            //{
            //    throw new WmsException(500, "数量错误");
            //}

            //DataRow waveHeadRow = Wave.GetWaveHead(waveDetailRows[0][wd.WaveHeadID].ToString());)
            //string waveHeadID = waveHeadRow[wh.ID].ToString();

            UomModel qtyUomModel = new UomModel()
            {
                Qty = leftQty,
                UomID = orderDetailRow["uom_qty_id"].ToString()
            };
            qtyUomModel = ItemUom.ConvertToDefaultPackUom(qtyUomModel);
            leftQty = qtyUomModel.Qty;

            //DataRow[] taskRows = WorkTask.GetBySrcAddr(
            //     AddressType.AddressType_Location.ToString(), locRow[l.ID].ToString());
            //DataRow[] taskRows = WorkTask.GetByRefDocID(orderRow[oh.ID].ToString());
            //taskRows = taskRows.Where(r =>
            //    r[t.SrcAddrType].ToString() == AddressType.AddressType_Location.ToString() &&
            //    r[t.SrcAddrID].ToString() == locRow[l.ID].ToString()).ToArray();
            DataRow[] taskRows = WorkTask.GetBy(orderRow[oh.ID].ToString(),
                AddressType.AddressType_Location.ToString(), locRow[l.ID].ToString());
            if (taskRows.Length <= 0)
            {
                return Wave.AddWaveTask(new WorkTaskModel()
                {
                    RefDocDetailID = waveDetailRows[0][wd.ID].ToString(),
                    SrcAddrID = locRow[l.ID].ToString(),
                    DestAddrID = orderRow[oh.DestAddrID].ToString(),
                    DestAddrType = orderRow[oh.DestAddrType].ToString(),
                    DestAddrText = orderRow[oh.DestAddrText].ToString(),
                }, leftQty);
            }

            // 加入任务明细
            DataRow[] tasks = WorkTask.GetDependentTasks(taskRows[0]);
            waveDetailRows[0][wd.PlanQty] = leftQty +
                decimal.Parse("0" + waveDetailRows[0][wd.ProcessedQty]);
            decimal qty = AddTaskItemOnly(locRow, waveDetailRows, tasks);

            if (!ExistsPickingTask(tasks))
            {
                // 无需拣选，仅需要加入任务明细即可
                return qty;
            }

            // 判断是否还需要拣选
            DataRow[] inventoryRows = Container.GetInventoryDetails(locRow);
            bool needPicking = false;
            foreach (DataRow row in inventoryRows)
            {
                if (decimal.Parse("0" + row["avail_qty"]) > 0)
                {
                    needPicking = true;
                    break;
                }
            }
            if (needPicking)
            {
                return qty;
            }

            // 原来用的是拣选为目的的任务，现在不需要拣选了，重新生成任务
            List<WorkTaskItemModel> taskItemList = new List<WorkTaskItemModel>();
            DataRow[] taskItems = WorkTaskItem.GetByTaskID(taskRows[0][t.ID].ToString());
            foreach (DataRow taskItem in taskItems)
            {
                taskItemList.Add(new WorkTaskItemModel()
                {
                    ItemID = taskItem[ti.ItemID].ToString(),
                    PlanQty = DBHelper.GetDecimal(taskItem[ti.PlanQty]),
                    ContainerDetailID = taskItem[ti.ContainerDetailID].ToString(),
                    RefDocDetailID = taskItem[ti.RefDocDetailID].ToString(),
                });
            }

            RemoveOutboundTasks(taskRows[0][t.ID].ToString());

            //Address src = new Address(locRow[l.ID].ToString());
            Address src = new Address(AddressType.AddressType_Location, locRow[l.ID].ToString());
            Address dest = new Address()
            {
                Type = orderRow["dest_addr_type"].ToString(),
                ID = orderRow["dest_addr_id"].ToString(),
                Text = orderRow["dest_addr_text"].ToString(),
            };
            // 匹配任务路径
            //Address src = new Address(workTask.SrcAddrID, workTask.SrcAddrType, workTask.SrcAddrText);
            //Address dest = new Address(workTask.DestAddrID, workTask.DestAddrType, workTask.DestAddrText);
            Route route = new Route();
            DataRow itemCatalog = Item.GetItemCatalog(orderDetailRow["item_id"].ToString());
            route.Init(itemCatalog["t_id"].ToString(), RoutePurpose.RoutePurpose_Outbound.ToString());
            if (!route.MatchRoute(src, dest, out DataRow[] sequences))
            {
                throw new WmsException(500, "无法找到合适的配置来生成任务");
            }
            string priority = taskRows[0][t.Priority].ToString();
            DataRow waveHeadRow = Wave.GetWaveHead(waveDetailRows[0]["wave_h_id"].ToString());

            WorkTask.SaveTask(sequences, new WorkTaskModel()
            {
                SeqNo = Decimal.Parse(orderDetailRow["detail_seq"].ToString()),
                Priority = decimal.Parse(priority),
                ContainerID = inventoryRows[0]["t_id"].ToString(),
                ContainerBarcode = inventoryRows[0]["container_barcode"].ToString(),
                SrcAddrID = src.ID,
                SrcAddrType = src.Type,
                SrcAddrText = src.Text,
                DestAddrID = dest.ID,
                DestAddrType = dest.Type,
                DestAddrText = dest.Text,
                RefDocType = DocType.DocType_Wave.ToString(),
                RefDocID = waveHeadRow[wh.ID].ToString(),
                RefDocNo = waveHeadRow[wh.WaveNo].ToString(),
            }, taskItemList);

            return 0m;
        }

        private decimal AddTaskItemOnly(DataRow locRow, DataRow[] waveDetailRows, DataRow[] tasks)
        {
            DataRow[] inventoryRows = Container.GetInventoryDetails(locRow);
            inventoryRows = inventoryRows.Where(r =>
                r[cd.ItemID].ToString() == waveDetailRows[0][od.ItemID].ToString()).ToArray();
            //if (inventoryRows.Length <= 0)
            //{
            //    throw new WmsException(500, "没有可用库存");
            //}

            // 匹配库存与波次明细
            List<WorkTaskItemModel> taskItemList = Wave.AllocateItems(waveDetailRows, inventoryRows);
            if (taskItemList.Count() <= 0)
            {
                throw new WmsException(416, $"货位{locRow["location_no"]}分配物料错误！");
            }
            foreach (WorkTaskItemModel taskItem in taskItemList)
            {
                WorkTask.AddTaskItem(tasks, taskItem);
            }
            return decimal.Parse("0" + waveDetailRows[0][wd.PlanQty]) -
                    decimal.Parse("0" + waveDetailRows[0][wd.ProcessedQty]);
        }

        private decimal AddOutboundTaskV3(DataRow orderRow,
            DataRow orderDetailRow, DataRow locRow, decimal leftQty)
        {
            DataRow containerRow = Container.GetHead(locRow["location_no"].ToString());
            DataRow[] ctnDetails = Container.Get4StockingOut(new ContainerDetailModel()
            {
                ContainerID = containerRow["m_container_id"].ToString(),
                ItemID = orderDetailRow["item_id"].ToString(),
                IsFull = orderDetailRow["is_full"].ToString(),
                StatQty = decimal.Parse("0" + orderDetailRow["plan_finance_qty"].ToString()),
                InboundHeadID = orderDetailRow["wms_d_outbound_hid"].ToString(),
                LotNo = orderDetailRow["lot_no"].ToString(),
                GivenYear = DBHelper.GetDecimal(orderDetailRow["givenyear"]),
            });
            ctnDetails = ctnDetails.OrderBy(r => r["first_inbound_dt"].ToString()).ToArray();

            if (ctnDetails.Length <= 0)
            {
                throw new WmsException(212, "无法在货位上找到适用库存，无法继续处理");
            }

            decimal sumQty = 0;
            foreach (DataRow ctnDtlRow in ctnDetails)
            {
                if (leftQty == 0) break;

                decimal actualQty = decimal.Parse("0" + ctnDtlRow["actual_qty"]);
                decimal outQty = decimal.Parse("0" + ctnDtlRow["out_qty"]);
                decimal lockQty = decimal.Parse("0" + ctnDtlRow["lock_qty"]);
                decimal availQty = actualQty - outQty - lockQty;
                decimal qty = 0;

                if (availQty == 0)
                {
                    continue;
                }
                if (leftQty > availQty)
                {
                    leftQty -= availQty;
                    qty = availQty;
                }
                else
                {
                    qty = leftQty;
                    leftQty = 0;
                }
                if (qty <= 0)
                {
                    throw new WmsException(211, "货位上库存不足，程序停止处理!");
                }

                sumQty += qty;

                Container.AddDetailOutQty(ctnDtlRow["t_id"].ToString(),
                    qty, decimal.Parse("0" + ctnDtlRow["out_qty"]));
                string isFull = ctnDtlRow["is_full"].ToString();
                decimal financeQty = qty * decimal.Parse("0" + ctnDtlRow["speci"].ToString());
                isFull = string.IsNullOrWhiteSpace(isFull) ? "1" : isFull;
                if (isFull == "0")
                {
                    financeQty = decimal.Parse("0" + ctnDtlRow["finance_qty"].ToString());
                }

                DataRow dockRow = Dock.GetByZone(locRow["zone_id"].ToString());
                WorkTask.Insert(new WorkTaskModel()
                {
                    ZoneID = ctnDtlRow["zone_id"].ToString(),
                    TaskType = "TaskType_OutBound",
                    TaskNo = WmsHelper.GetSequence("TaskNo"),
                    //OperationQty = qty,
                    SeqNo = 0,
                    //ItemID = orderDetailRow["item_id"].ToString(),
                    //ContainerDetailID = ctnDtlRow["t_id"].ToString(),
                    ContainerBarcode = containerRow["container_barcode"].ToString(),
                    RefDocType = "RefDocType_OutBound",
                    RefDocNo = orderRow["doc_no"].ToString(),
                    RefDocID = orderDetailRow["t_id"].ToString(),
                    SrcAddrID = locRow["t_id"].ToString(),
                    SrcAddrType = "AddressType_Location",
                    SrcAddrText = locRow["location_no"].ToString(),
                    DestAddrID = dockRow["t_id"].ToString(),
                    DestAddrType = "AddressType_Dock",
                    DestAddrText = dockRow["dock_name"].ToString(),
                    TaskStatus = TaskStatus.TaskStatus_Create.ToString(),
                    IsExecutable = "1",
                    //IsFull = isFull,
                    //FinanceQty = financeQty,
                });
                // TODO to process task_item table
            }

            return leftQty;
        }

        private decimal AddOutboundTaskV2(DataRow orderRow,
            DataRow orderDetailRow, DataRow locRow, decimal leftQty)
        {
            DataRow containerRow = Container.GetHead(locRow["location_no"].ToString());
            DataRow[] ctnDetails = Container
                .GetDetails(containerRow["m_container_id"].ToString(),
                            orderDetailRow["item_id"].ToString())
                .OrderBy(r => r["first_inbound_dt"].ToString())
                .ToArray();

            /*
            DataRow[] ctnDtlRow = ctnDetails.Where(r
                => r["item_id"].ToString() == row["item_id"].ToString()
                && r["isa_b_inbound_hid"].ToString() == r["isa_b_inbound_h_id"].ToString()
                ).ToArray();
                */

            decimal sumQty = 0;
            foreach (DataRow ctnDtlRow in ctnDetails)
            {
                if (leftQty == 0) break;

                decimal actualQty = decimal.Parse("0" + ctnDtlRow["actual_qty"]);
                decimal outQty = decimal.Parse("0" + ctnDtlRow["out_qty"]);
                decimal lockQty = decimal.Parse("0" + ctnDtlRow["lock_qty"]);
                decimal availQty = actualQty - outQty - lockQty;
                decimal qty = 0;

                if (availQty == 0)
                {
                    continue;
                }
                if (leftQty > availQty)
                {
                    leftQty -= availQty;
                    qty = availQty;
                }
                else
                {
                    qty = leftQty;
                    leftQty = 0;
                }
                if (qty <= 0)
                {
                    throw new WmsException(111, "货位上物料数量不足，无法继续处理");
                }

                sumQty += qty;

                Container.AddDetailOutQty(ctnDtlRow["t_id"].ToString(),
                    qty, decimal.Parse("0" + ctnDtlRow["out_qty"]));

                DataRow dockRow = Dock.GetByZone(locRow["zone_id"].ToString());
                WorkTask.Insert(new WorkTaskModel()
                {
                    ZoneID = ctnDtlRow["zone_id"].ToString(),
                    TaskType = "TaskType_OutBound",
                    TaskNo = WmsHelper.GetSequence("TaskNo"),
                    //OperationQty = qty,
                    SeqNo = 0,
                    //ItemID = orderDetailRow["item_id"].ToString(),
                    //ContainerDetailID = ctnDtlRow["t_id"].ToString(),
                    ContainerBarcode = containerRow["container_barcode"].ToString(),
                    RefDocType = "RefDocType_OutBound",
                    RefDocNo = orderRow["doc_no"].ToString(),
                    RefDocID = orderDetailRow["t_id"].ToString(),
                    SrcAddrID = locRow["t_id"].ToString(),
                    SrcAddrType = "AddressType_Location",
                    SrcAddrText = locRow["location_no"].ToString(),
                    DestAddrID = dockRow["t_id"].ToString(),
                    DestAddrType = "AddressType_Dock",
                    DestAddrText = dockRow["dock_name"].ToString(),
                    TaskStatus = TaskStatus.TaskStatus_Create.ToString(),
                    IsExecutable = "0"
                });
                // TODO to process task_item table
            }

            return leftQty;
        }

        private decimal AddOutboundTaskV1(DataRow orderRow,
            DataRow orderDetailRow, DataRow locRow, decimal leftQty, string qcID)
        {
            DataRow containerRow = Container.GetHead(locRow["location_no"].ToString());
            DataRow[] ctnDetails = Container
                .GetDetails(containerRow["m_container_id"].ToString(),
                            orderDetailRow["item_id"].ToString())
                .OrderBy(r => r["first_inbound_dt"].ToString())
                .ToArray();

            /*
            DataRow[] ctnDtlRow = ctnDetails.Where(r
                => r["item_id"].ToString() == row["item_id"].ToString()
                && r["isa_b_inbound_hid"].ToString() == r["isa_b_inbound_h_id"].ToString()
                ).ToArray();
                */

            decimal sumQty = 0;
            foreach (DataRow ctnDtlRow in ctnDetails)
            {
                if (leftQty == 0) break;

                decimal actualQty = decimal.Parse("0" + ctnDtlRow["actual_qty"]);
                decimal outQty = decimal.Parse("0" + ctnDtlRow["out_qty"]);
                decimal lockQty = decimal.Parse("0" + ctnDtlRow["lock_qty"]);
                decimal availQty = actualQty - outQty - lockQty;
                decimal qty = 0;

                if (availQty == 0)
                {
                    continue;
                }
                if (leftQty > availQty)
                {
                    leftQty -= availQty;
                    qty = availQty;
                }
                else
                {
                    qty = leftQty;
                    leftQty = 0;
                }
                if (qty <= 0)
                {
                    throw new WmsException(111, "货位上物料数量不足，无法继续处理");
                }

                sumQty += qty;

                Container.AddDetailOutQty(ctnDtlRow["t_id"].ToString(),
                    qty, decimal.Parse("0" + ctnDtlRow["out_qty"]));

                DataRow dockRow = Dock.GetByZone(locRow["zone_id"].ToString());
                WorkTask.Insert(new WorkTaskModel()
                {
                    ZoneID = ctnDtlRow["zone_id"].ToString(),
                    TaskType = "TaskType_OutBound",
                    TaskNo = WmsHelper.GetSequence("TaskNo"),
                    //OperationQty = qty,
                    SeqNo = 0,
                    //ItemID = orderDetailRow["item_id"].ToString(),
                    //ContainerDetailID = ctnDtlRow["t_id"].ToString(),
                    ContainerBarcode = containerRow["container_barcode"].ToString(),
                    RefDocType = "RefDocType_OutBound",
                    RefDocNo = orderRow["doc_no"].ToString(),
                    RefDocID = orderDetailRow["t_id"].ToString(),
                    SrcAddrID = locRow["t_id"].ToString(),
                    SrcAddrType = "AddressType_Location",
                    SrcAddrText = locRow["location_no"].ToString(),
                    DestAddrID = dockRow["t_id"].ToString(),
                    DestAddrType = "AddressType_Dock",
                    DestAddrText = dockRow["dock_name"].ToString(),
                    TaskStatus = "TaskStatus_WaitingExecute",
                    IsExecutable = "0"
                });
                // TODO to process task_item table
            }

            return leftQty;
        }

        public void AddDetailByBom(string orderNoOrID, string bomID, decimal times)
        {
            if (string.IsNullOrWhiteSpace(orderNoOrID)
                || string.IsNullOrWhiteSpace(bomID)
                || times <= 0
                )
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }

            DataRow orderRow = GetHead(orderNoOrID);
            if (orderRow[oh.OutboundStatus].ToString() != OutDocStatus.OutDocStatus_New.ToString())
            {
                throw new WmsException(500, "只有新建订单才允许添加");
            }
            if (WmsConfig.GetOrDefault("Outbound:AddBomMode", "Normal") == "Hefei")
            {
                AddDetailByBomForHefei(orderRow, bomID, times);
                return;
            }
            UpdateDetailActStatusByHead(orderRow[oh.ID].ToString(), "0");
            UpdateHeadWaveCreationMode(orderRow[oh.ID].ToString(),
                WaveCreationMode.WaveCreationMode_ByBOM.ToString());

            DataRow bomHeadRow = Bom.GetHead(bomID);
            UpdateHeadBomCode(orderRow[oh.ID].ToString(), bomHeadRow[bh.BomCode].ToString());

            DataRow[] bomRows = Bom.GetFirstLevelDetail(bomID);
            bomRows = bomRows.OrderBy(r => decimal.Parse("0" + r[bd.SeqNo])).ToArray();
            foreach (DataRow bomRow in bomRows)
            {
                DataRow itemRow = Item.Get(bomRow[bd.ItemID].ToString());
                decimal qty = times * decimal.Parse("0" + bomRow[bd.Qty]);
                UomModel statQty = ItemUom.ConvertToDefaultStatUom(new UomModel()
                {
                    Qty = qty,
                    UomID = bomRow[bd.UomQtyID].ToString(),
                }, DBHelper.GetDecimal(itemRow[i.Spec].ToString()));
                Insert(new OutboundDetailModel()
                {
                    OutboundHeadID = orderRow[oh.ID].ToString(),
                    SeqNo = DBHelper.GetDecimal(bomRow[bd.SeqNo]),
                    ItemID = itemRow[i.ID].ToString(),
                    ItemName = itemRow[i.ItemName].ToString(),
                    Spec = itemRow[i.Spec].ToString(),
                    PlanQty = qty,
                    ProcessedQty = 0,
                    FinishedQty = 0,
                    UomQtyID = bomRow[bd.UomQtyID].ToString(),
                    PlanStatQty = statQty.Qty,
                    UomStatID = statQty.UomID,
                });
            }
        }
        private void AddDetailByBomForHefei(DataRow orderRow, string bomID, decimal times)
        {
            //UpdateDetailActStatusByHead(orderRow[oh.ID].ToString(), "0");
            //UpdateHeadWaveCreationMode(orderRow[oh.ID].ToString(),
            //    WaveCreationMode.WaveCreationMode_ByBOM.ToString());
            UpdateHeadPrepBindLotNo(orderRow[oh.ID].ToString(), times);

            DataRow bomHeadRow = Bom.GetHead(bomID);
            UpdateHeadBomCode(orderRow[oh.ID].ToString(), bomHeadRow[bh.BomCode].ToString());

            char lastChar = bomHeadRow[bh.BomCode].ToString().Last();
            if (lastChar == 'A') lastChar = '1';
            else if (lastChar == 'B') lastChar = '2';
            else lastChar = '0';

            DataRow[] bomRows = Bom.GetFirstLevelDetail(bomID);
            bomRows = bomRows.OrderBy(r => decimal.Parse("0" + r[bd.SeqNo])).ToArray();
            foreach (DataRow bomRow in bomRows)
            {
                DataRow itemRow = Item.Get(bomRow[bd.ItemID].ToString());
                decimal spec = decimal.Parse("0" + itemRow[i.Spec]);
                decimal qtyInBom = decimal.Parse("0" + bomRow[bd.Qty]);
                decimal weight = 200 * qtyInBom;
                int intQty = (int)(weight / spec);
                decimal decQty = weight % spec;
                if (Math.Abs(decQty - spec) < 10)
                {
                    intQty++;
                    decQty = 0;
                }
                decQty = decQty >= 2 ? decQty : 0;
                decimal qty = times * intQty;
                UomModel statQty = ItemUom.ConvertToDefaultStatUom(new UomModel()
                {
                    Qty = qty,
                    UomID = itemRow[i.DefaultPackUomID].ToString(),
                }, DBHelper.GetDecimal(itemRow[i.Spec].ToString()));

                string lotNo = orderRow[oh.PrepProdOrder].ToString();
                OutboundDetailModel orderDetailModel = new OutboundDetailModel()
                {
                    ID = SnowflakeID.Guid().ToString(),
                    OutboundHeadID = orderRow[oh.ID].ToString(),
                    SeqNo = DBHelper.GetDecimal(bomRow[bd.SeqNo]),
                    ItemID = itemRow[i.ID].ToString(),
                    ItemName = itemRow[i.ItemName].ToString(),
                    Spec = itemRow[i.Spec].ToString(),
                    PlanQty = qty,
                    ProcessedQty = 0,
                    FinishedQty = 0,
                    UomQtyID = itemRow[i.DefaultPackUomID].ToString(),
                    PlanStatQty = statQty.Qty,
                    UomStatID = statQty.UomID,
                    LotNo = lotNo + lastChar,
                    OwnerLotNo = lotNo,
                    Batches = times,
                };
                Insert(orderDetailModel);

                if (decQty == 0m) continue;
                orderDetailModel.ID = SnowflakeID.Guid().ToString();
                orderDetailModel.PlanQty = times;
                orderDetailModel.Spec = decQty.ToString("0");
                orderDetailModel.PlanStatQty = decQty;
                Insert(orderDetailModel);
            }
        }

        public DataRow GetOrder(string orderNoOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "id_no", orderNoOrID }
            };
            string sql;
            sql = @"
select oh.dest_addr_type target_type, oh.dest_addr_id target_id, oh.dest_addr_text target_text, oh.*  
  from &DataSource&wms_d_outbound_h oh 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and (t_id = @id_no or doc_no = @id_no)
   and act_status = '1'
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询出库单头");

            return dt.Rows[0];
        }

        public DataRow[] GetOrderHeads(string orderIds)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ids", orderIds }
            };
            string sql;
            sql = @"
select '' target_type, '' target_id, '' target_text, oh.* 
  from &DataSource&wms_d_outbound_h oh 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and instr(@ids, t_id) > 0
   and act_status = '1'
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询出库单头");

            int cnt = orderIds.Length - orderIds.Replace(",", "").Length + 1;
            if (cnt != dt.Rows.Count)
            {
                throw new WmsException(-220, "没有查询到足够的出库单头表数量");
            }

            DataRow[] rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rows, 0);

            return rows;
        }

        public DataRow[] GetBomDetail(string bomId)
        {
            // 获取出库单明细
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "bom_id", bomId }
            };
            string selSql = @"
select d.t_id,d.client_id,d.org_id,d.warehouse_id,d.bom_h_id,i.item_id,
           i.item_name,i.qty,d.order_seq,h.bom_code 
 from &DataSource&swms_bi_bom_d d 
  join &DataSource&swms_bi_bom_h h on d.bom_h_id = h.t_id 
  join (select * from ( 
               select a.t_id, a.warehouse_id ,a.bom_d_id,  
                          a.bom_h_id,  a.item_id,a.item_name, a.qty, 
                          a.level_number, row_number() 
                          over(PARTITION by a.bom_d_id order by a.level_number) ranknum 
                 from &DataSource&swms_bi_bom_item a ) where ranknum = 1) i on d.t_id = i.bom_d_id 
 where ifnull(d.client_id, ' ')= @S0X and ifnull(d.org_id, ' ')= @S1X 
   and h.bom_code = @S3X and d.warehouse_id = @S2X order by d.order_seq
   and d.act_status = '1'
";
            DataTable dt = DBHelper.Select(selSql, param, "BOM明细表");

            DataRow[] rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rows, 0);

            return rows;
        }

        public DataRow[] GetDetailNotFinished(string orderID)
        {
            throw new NotImplementedException();
        }

        public IOutboundOrder UpdateOrderHeadState(string orderIds, string state, string oldState = null)
        {
            oldState ??= " ";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "state", state},
                { "old_state", oldState},
                { "t_id", orderIds }
            };
            string sql = @"
update &DataSource&wms_d_outbound_h  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       outbound_status = @state
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and instr(@t_id, t_id) > 0
   and (@old_state = ' ' or outbound_status = @old_state) 
";
            DBHelper.RunSql(sql, param, "更新出库单状态", DBHelper.OperatedNumType.OneOrMore);

            return this;
        }

        public IOutboundOrder UpdateHeadBomCode(string headID, string bomCode, string oldBomCode = null)
        {
            oldBomCode ??= " ";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "bom_code", bomCode},
                { "old_bom_code", oldBomCode},
                { "t_id", headID }
            };
            string sql = @"
update &DataSource&wms_d_outbound_h  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       bom_code = @bom_code
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and @t_id = t_id
   and (@old_bom_code = ' ' or bom_code = @old_bom_code) 
";
            DBHelper.RunSql(sql, param, "更新出库单BOM");

            return this;
        }

        public IOutboundOrder UpdateHeadPrepBindLotNo(string headID, decimal batches)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "batches", batches},
                { "t_id", headID }
            };
            string sql = @"
update &DataSource&wms_d_outbound_h  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       prep_bind_lot_no = @batches
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and @t_id = t_id
";
            DBHelper.RunSql(sql, param, "更新出库单备料绑定批次数");

            return this;
        }

        public IOutboundOrder UpdateHeadWaveCreationMode(string headID, string mode, string oldMode = null)
        {
            oldMode ??= " ";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "creation_mode", mode},
                { "old_mode", oldMode},
                { "t_id", headID }
            };
            string sql = @"
update &DataSource&wms_d_outbound_h  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       wave_creation_mode = @creation_mode
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and @t_id = t_id
   and (@old_mode = ' ' or wave_creation_mode = @old_mode) 
";
            DBHelper.RunSql(sql, param, "更新出库单波次创建模式");

            return this;
        }

        public IOutboundOrder FinishDetail(string headID, string detailID, decimal qty)
        {
            DataRow detailRow = GetDetail(detailID);
            UomModel qtyModel = ItemUom.ConvertFromDefaultPackUom(qty, detailRow[od.UomQtyID].ToString());
            AddDetailFinishedQty(detailID, qtyModel.Qty);
            UomModel statQtyModel = ItemUom.ConvertToDefaultStatUom(qtyModel, decimal.Parse("0" + detailRow[od.Spec]));
            AddDetailFinishedStatQty(detailID, statQtyModel.Qty);

            FinishOrderWhenTasksFinishedAndQtyFinished(headID);

            return this;
        }

        private bool IsQtyFinished(string orderID)
        {
            DataRow[] detailRows = GetDetails(orderID);
            foreach (DataRow detailRow in detailRows)
            {
                if (decimal.Parse("0" + detailRow[od.FinishedQty]) < 
                                    decimal.Parse("" + detailRow[od.PlanQty]))
                {
                    return false;
                }
            }
            return true;
        }

        public IOutboundOrder FinishOrderWhenTasksFinishedAndQtyFinished(string orderID)
        {
            if (!WorkTask.IsFinished(WorkTask.GetByOrderNoOrID(orderID)))
            {
                return this;
            }

            if (!IsQtyFinished(orderID))
            {
                return this;
            }

            FinishOrder(orderID);

            return this;
        }

        public void CloseOrder(string orderNoOrID)
        {
            if (string.IsNullOrWhiteSpace(orderNoOrID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            string processBarcodeNumber = WmsConfig.GetOrDefault("Barcode:ProcessBarcodeNumber", "0");
            DataRow headRow = GetHead(orderNoOrID);
            if (processBarcodeNumber == "1")
            {
                if (headRow["doc_type"].ToString() == "ComplementCode")
                {
                    if (headRow["doc_status"].ToString() != "InDocStatus_New")
                    {
                        throw new WmsException(100, "出库单状态不正确");
                    }
                    throw new WmsException(99, "功能尚未实现！");
                    //FinishBarcodeOrder(headRow);
                    //return;
                }
            }

            if (headRow[oh.OutboundStatus].ToString() == OutDocStatus.OutDocStatus_Finished.ToString())
            {
                return;
            }
            if (headRow[oh.OutboundStatus].ToString() != OutDocStatus.OutDocStatus_Allocated.ToString())
            {
                throw new WmsException(100, "出库单状态不正确");
            }

            UpdateOrderHeadState(headRow["t_id"].ToString(),
                OutDocStatus.OutDocStatus_Finished.ToString(), headRow[oh.OutboundStatus].ToString());

            //DataRow waveHeadRow = Wave.GetHeadByOrderID(headRow["t_id"].ToString());
            //if (waveHeadRow[wh.WaveType].ToString() == WaveType.WaveType_Normal.ToString()
            //    && waveHeadRow[wh.WaveStatus].ToString() != WaveStatus.WaveStatus_Finished.ToString())
            //{
            //    throw new WmsException(500, "波次未完成，无法关闭!");
            //}

            //DataRow[] waveDetailRows = Wave.GetDetailsByOrderID(headRow["t_id"].ToString());
            //foreach (DataRow row in waveDetailRows)
            //{
            //    DataRow[] taskRows = WorkTask.GetByRefDocID(row["t_id"].ToString());
            //    foreach (DataRow task in taskRows)
            //    {
            //        if (task["task_status"].ToString() == "TaskStatus_Finished")
            //        {
            //            continue;
            //        }
            //        throw new WmsException(500, "还有未完成的任务，无法关闭!");
            //    }
            //}

            // 删除已回收任务
            DataRow[] taskRows = WorkTask.GetByOrderNoOrID(headRow[oh.ID].ToString());
            taskRows = taskRows.Where(r => r[t.ActStatus].ToString() == "1"
                        && r[t.TaskStatus].ToString() == TaskStatus.TaskStatus_Create.ToString()).ToArray();
            WorkTask.RemoveWaveTasks(taskRows);

            if (!WorkTask.IsFinished(WorkTask.GetByOrderNoOrID(headRow[oh.ID].ToString())))
            {
                throw new WmsException(500, "还有未完成的任务，无法关闭!");
            }

            // 把单据移入历史表
            //MoveToHistory(headRow["t_id"].ToString());
        }

        public void FinishOrder(string orderNoOrID)
        {
            string processBarcodeNumber = WmsConfig.GetOrDefault("Barcode:ProcessBarcodeNumber", "0");
            DataRow headRow = GetHead(orderNoOrID);
            if (processBarcodeNumber == "1")
            {
                if (headRow["doc_type"].ToString() == "ComplementCode")
                {
                    if (headRow["doc_status"].ToString() != "InDocStatus_New")
                    {
                        throw new WmsException(100, "出库单状态不正确");
                    }
                    throw new WmsException(99, "功能尚未实现！");
                    //FinishBarcodeOrder(headRow);
                    //return;
                }
            }

            if (headRow["outbound_status"].ToString() == "OutDocStatus_Finished")
            {
                return;
            }
            if (headRow["outbound_status"].ToString() != OutDocStatus.OutDocStatus_Allocated.ToString()
                && headRow["outbound_status"].ToString() != OutDocStatus.OutDocStatus_Prepared.ToString())
            {
                throw new WmsException(100, "出库单状态不正确");
            }

            UpdateOrderHeadState(headRow["t_id"].ToString(),
                OutDocStatus.OutDocStatus_Finished.ToString(),
                headRow["outbound_status"].ToString());

            //DataRow[] detailRows = GetDetails(headRow["t_id"].ToString());
            //foreach (DataRow row in detailRows)
            //{
            //    DataRow[] taskRows = WorkTask.GetByRefDocID(row["t_id"].ToString());
            //    foreach (DataRow task in taskRows)
            //    {
            //        if (task["task_status"].ToString() == "TaskStatus_Finished")
            //        {
            //            continue;
            //        }
            //        WorkTask.ForceTaskFinished(row["warehouse_id"].ToString(), task["t_id"].ToString());
            //    }
            //}

            // TODO 把单据移入历史表
            // MoveToHistory(headRow["t_id"].ToString());
        }

        private void FinishBarcodeOrder(DataRow headRow)
        {
            UpdateOrderHeadState(headRow["t_id"].ToString(),
                "OutDocStatus_Finished", headRow["outbound_status"].ToString());

            string barcodeLoc = WmsConfig.GetOrDefault("Barcode:BarcodeLocationCode", "");

            DataRow locRow = Location.Get(barcodeLoc);

            DataRow[] detailRows = GetDetails(headRow["t_id"].ToString());
            foreach (DataRow row in detailRows)
            {
                DataRow containerRow = Container.GetHead(locRow["location_no"].ToString());

                /*
                string ctnDetailID = SnowflakeID.Guid().ToString();
                ContainerDetailModel ctnDetailModel = new ContainerDetailModel()
                {
                    ID = ctnDetailID,
                    ZoneID = locRow["zone_id"].ToString(),
                    ContainerID = containerRow["m_container_id"].ToString(),
                    InboundHeadID = row["isa_b_inbound_h_id"].ToString(),
                    ItemID = row["item_id"].ToString(),
                    ItemName = row["item_name"].ToString(),
                    UomQtyID = row["isa_wi_uom_qtyid"].ToString(),
                    UomFinanceID = row["isa_wi_uom_financeid"].ToString(),
                    ExternalLotNo = row["lot_no"].ToString(),
                    Givenyear = decimal.Parse("0" + row["givenyear"]),
                    Speci = row["speci"].ToString(),
                    ExistBarcode = row["exist_barcode"].ToString(),
                };

                decimal planQty = decimal.Parse("0" + row["plan_qty"]);
                // TODO
                decimal allocatedQty = Container.PreparePutaway(ctnDetailModel,
                    planQty, decimal.Parse("0" + locRow["qty_limit"]));
                if (allocatedQty <= 0)
                {
                    throw new WmsException(102, "出库数量错误，无法继续处理");
                }

                DataRow ctnDtlRow = Container.GetDetail(ctnDetailID);
                */
                decimal planQty = decimal.Parse("0" + row["plan_qty"]);
                DataRow ctnDtlRow = Container.GetDetail(
                    containerRow["m_container_id"].ToString(), row["item_id"].ToString());

                Container.AddDetailOutQty(ctnDtlRow["m_container_id"].ToString(), planQty,
                    decimal.Parse("0" + ctnDtlRow["out_qty"]));

                // 修改容器明细表的数量
                Container.StockOut(ctnDtlRow, planQty, decimal.Parse("0" + row["plan_finance_qty"]));

                // 入库单据明细，需要修改相应单据的数量信息
                AddDetailFinishedQty(row["t_id"].ToString(), planQty);
            }

            MoveToHistory(headRow["t_id"].ToString());

            return;
        }

        public IOutboundOrder MoveToHistory(string headID)
        {
            InsertDetailHistory(headID);

            InsertHeadHistory(headID);

            DeleteDetailByHeadID(headID);

            DeleteHead(headID);

            return this;
        }

        private void InsertDetailHistory(string headID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "head_id", headID }
            };
            /*
            string sql = @"
insert into wms_d_outbound_d_his
  (t_id, client_id, org_id, warehouse_id, wms_d_outbound_hid, item_id, 
   item_name, speci, givenyear, lot_no, exist_barcode, plan_qty, 
   isa_wi_uom_qtyid, plan_finance_qty, isa_wi_uom_financeid, out_qty, 
   out_finance_qty, create_date, created_by, modify_ip, modify_date, 
   last_modified_by, act_status)
select t_id, client_id, org_id, warehouse_id, wms_d_outbound_hid, item_id, 
       item_name, speci, givenyear, lot_no, exist_barcode, plan_qty, 
       isa_wi_uom_qtyid, plan_finance_qty, isa_wi_uom_financeid, out_qty, 
       out_finance_qty, create_date, created_by, modify_ip, modify_date, 
       last_modified_by, act_status 
    from wms_d_outbound_d
   where warehouse_id = @warehouse
     and wms_d_outbound_hid = @head_id
";
*/
            string sql = @"
insert into &DataSource&wms_d_outbound_d_his
  select *
    from &DataSource&wms_d_outbound_d
   where warehouse_id = @warehouse
     and wms_d_outbound_hid = @head_id
";
            DBHelper.RunSql(sql, param, "插入出库单明细历史表", DBHelper.OperatedNumType.OneOrMore);
        }

        private void InsertHeadHistory(string headID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "head_id", headID }
            };
            /*
            string sql = @"
insert into wms_d_outbound_h_his
  ( t_id, client_id, org_id, warehouse_id, doc_no, outservice_type, 
    ref_order_no, isa_wi_bpartner_ownerid, outbound_status, is_auto_created, 
    total_qty, total_finance_qty, plan_delivery_date, rec_unit, 
    delivery_unit, settlement_type, chargeunit, checkresult, 
    license_plate, tracking_no, isa_wi_bpartner_shipperid, isa_bi_dock_id, 
    remarks, create_date, created_by, modify_ip, modify_date, 
    last_modified_by, act_status, contracts, loading_seqno)
select t_id, client_id, org_id, warehouse_id, doc_no, outservice_type, 
       ref_order_no, isa_wi_bpartner_ownerid, outbound_status, is_auto_created, 
       total_qty, total_finance_qty, plan_delivery_date, rec_unit, 
       delivery_unit, settlement_type, chargeunit, checkresult, 
       license_plate, tracking_no, isa_wi_bpartner_shipperid, isa_bi_dock_id, 
       remarks, create_date, created_by, modify_ip, modify_date, 
       last_modified_by, act_status, contracts, loading_seqno from &DataSource&wms_d_outbound_h
 where warehouse_id = @warehouse
   and t_id = @head_id
";
*/
            string sql = @"
insert into wms_d_outbound_h_his
select *
  from &DataSource&wms_d_outbound_h
 where warehouse_id = @warehouse
   and t_id = @head_id
   and d.act_status = '1'
";
            DBHelper.RunSql(sql, param, "插入出库单头历史表");
        }

        private void DeleteDetailByHeadID(string headID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", headID }
            };
            string sql = @"
delete from &DataSource&wms_d_outbound_d
 where warehouse_id = @warehouse
   and wms_d_outbound_hid = @t_id
";
            DBHelper.RunSql(sql, param, "删除出库单明细表", DBHelper.OperatedNumType.OneOrMore);
        }

        private void DeleteHead(string headID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", headID }
            };
            string sql = @"
delete from &DataSource&wms_d_outbound_h
 where warehouse_id = @warehouse
   and t_id = @t_id
";
            DBHelper.RunSql(sql, param, "删除出库单头表");
        }

        public IOutboundOrder ModifyOutboundTask(string taskNoOrID, decimal qty, string locationID)
        {
            if (string.IsNullOrWhiteSpace(locationID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow taskRow = WorkTask.Get(taskNoOrID);

            if (taskRow["task_status"].ToString() != TaskStatus.TaskStatus_Create.ToString())
            {
                throw new WmsException(440, "预分配信息状态不正确！无法继续!");
            }
            //if (taskRow["source_address_id"].ToString() == locationID)
            //{
            //    throw new WmsException(441, "请选择不同的货位!");
            //}

            RemoveOutboundTasks(taskRow["t_id"].ToString());

            AddOutboundTask(taskRow["source_doc_id"].ToString(), locationID, qty);

            /*
            DataRow locRow = Location.Get(locationID);
            ContainerDetailModel ctnDtl = new ContainerDetailModel()
            {
                ItemID = taskRow["item_id"].ToString()
            };
            Container.Prepare4StockingOut(locRow["t_id"].ToString(), ctnDtl);

            string taskID = taskRow["t_id"].ToString();
            WorkTask.UpdateOperationQty(taskID, qty, decimal.Parse("0" + taskRow["operation_qty"]))
                .UpdateContainerDetailID(taskID, ctnDtl.ID)
                .UpdateStorageZoneID(taskID, ctnDtl.ZoneID, taskRow["zone_id"].ToString())
                .UpdateSource(taskID, "AddressType_Location", locRow["t_id"].ToString(), locRow["location_no"].ToString());
                */

            return this;
        }

        public IOutboundOrder ModifyOutboundTaskItem(string taskItemID, decimal qty, string locationID)
        {
            if (string.IsNullOrWhiteSpace(locationID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow taskItemRow = WorkTaskItem.Get(taskItemID);
            DataRow taskRow = WorkTask.Get(taskItemRow[ti.TaskID].ToString());
            DataRow[] waveDetailRows = Wave.GetWaveDetails(taskItemRow[ti.RefDocDetailID].ToString());
            DataRow orderRow = GetHead(waveDetailRows[0][wd.OutboundHeadID].ToString());
            if (orderRow[oh.OutboundStatus].ToString() != OutDocStatus.OutDocStatus_Preallocation.ToString())
            {
                throw new WmsException(500, "单据状态不允许修改!");
            }
            if (taskRow["task_status"].ToString() != TaskStatus.TaskStatus_Create.ToString())
            {
                throw new WmsException(440, "预分配信息状态不正确！无法继续!");
            }

            RemoveOutboundTaskItems(taskItemID);
            //RemoveOutboundTasks(taskRow["t_id"].ToString());

            AddOutboundTask(waveDetailRows[0][wd.OutboundDetailID].ToString(), locationID, qty);

            return this;
        }

        IOutboundOrder IOutboundOrder.AddDetailProcessedQty(string detailID, decimal processedQty, decimal? oldQty)
        {
            return this;
        }

        public IOutboundOrder TransferOwnership(string orderNoOrID)
        {
            throw new NotImplementedException();
        }

        public IOutboundOrder CreateShippingOrder(string json)
        {
            throw new NotImplementedException();
        }

        public IOutboundOrder ReplaceTask4StackerFault()
        {
            DataRow[] faultStacker = Equipment.GetEquipmentByStatus(
                EquipmentType.EquipmentType_AS.ToString(),
                EquipmentStatus.EquipmentStatus_Error.ToString());
            if (faultStacker.Length <= 0)
            {
                throw new WmsException(500, "没有找到故障堆垛机，不需要进行替换");
            }

            List<string> statusList = new List<string>()
            {
                TaskStatus.TaskStatus_Executing.ToString(),
                TaskStatus.TaskStatus_WaitingExecute.ToString(),
            };
            DataRow[] taskRows = WorkTask.GetByEquipmentID(
                string.Join(',', faultStacker.Select(r => r["t_id"].ToString()).ToArray()));
            taskRows = taskRows.Where(r => statusList.Contains(r[t.TaskStatus].ToString()) &&
                    r[t.RefDocType].ToString() == DocType.DocType_Wave.ToString() ).ToArray();

            // 取得replaceID
            ItemReplacement ir = new ItemReplacement();
            List<string> idList = ir.ReplaceItems(string.Join(',', taskRows.Select(r => r["t_id"].ToString())));

            // 确认
            ir.ConfirmItemReplacement(string.Join(',', idList));

            return this;
        }

    }
}
