using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Common.Interface;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Services.WMS.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    public class ItemUom : WmsObject, IItemUom
    {
        #region DAL
        public void Delete(string id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", id }
            };
            string sql = @"
delete from &DataSource&wms_m_item_uom  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id 
";
            DBHelper.RunSql(sql, param, "删除物料单位表");
        }


        public DataRow Get(string id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", id }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_m_item_uom  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id 
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询物料单位表");

            return dt.Rows[0];
        }


        public DataRow[] GetByItemID(string itemID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "item_id", itemID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_m_item_uom  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and item_id = @item_id 
";
            DataTable dt = DBHelper.Select(sql, param, "查询物料单位表", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetByCategory(string uomCategory)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "uom_category", uomCategory }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_m_item_uom  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and uom_category = @uom_category 
";
            DataTable dt = DBHelper.Select(sql, param, "查询物料单位表", false);

            return dt.AsEnumerable().ToArray();
        }
        public DataRow GetByCategoryAndCode(string category, string code)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "category", category },
                { "code", code }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_m_item_uom  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and uom_category = @category 
   and uom_desc = @code 
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询物料单位表");

            return dt.Rows[0];
        }


        public DataRow GetDefaultPackUomByItemCatalog(string itemCatalogID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "item_catalog_id", itemCatalogID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_m_item_uom  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and uom_type = 'UomType_Pack' 
   and is_default = '1' 
   and item_catalog_id = @item_catalog_id 
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询物料单位表");

            return dt.Rows[0];
        }

        public DataRow GetDefaultStatUomByItemCatalog(string itemCatalogID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "item_catalog_id", itemCatalogID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_m_item_uom  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and uom_type = 'UomType_Stat' 
   and is_default = '1' 
   and item_catalog_id = @item_catalog_id 
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询物料单位表");

            return dt.Rows[0];
        }

        public ItemUom Insert(ItemUomModel model)
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
                { "item_id", model.ItemID },
                { "item_catalog_id", model.ItemCatalogID },
                { "tag", model.Tag },
                { "supplier_id", model.SupplierID },
                { "uom_id", model.UomID },
                { "child_id", model.ChildID },
                { "uom_name", model.UomName },
                { "uom_desc", model.UomDesc },
                { "uom_category", model.UomCategory },
                { "pack_name", model.PackName },
                { "spec", model.Spec },
                { "is_visible", model.IsVisible },
                { "uom_type", model.UomType },
                { "is_default", model.IsDefault },
                { "is_min_io_pack_uom", model.IsMinIOPackUom },
                { "is_min_uom", model.IsMinUom },
                { "seq_no", model.SeqNo.ToString() },
                { "rate", model.Rate.ToString() },
                { "is_disabled", model.IsDisabled },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into &DataSource&WMS_M_ITEM_UOM ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       item_id, " +
                "       item_catalog_id, " +
                "       tag, " +
                "       supplier_id, " +
                "       uom_id, " +
                "       child_id, " +
                "       uom_name, " +
                "       uom_desc, " +
                "       uom_category, " +
                "       pack_name, " +
                "       spec, " +
                "       is_visible, " +
                "       uom_type, " +
                "       is_default, " +
                "       is_min_io_pack_uom, " +
                "       is_min_uom, " +
                "       seq_no, " +
                "       rate, " +
                "       is_disabled, " +
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
                "       @item_id, " +
                "       @item_catalog_id, " +
                "       @tag, " +
                "       @supplier_id, " +
                "       @uom_id, " +
                "       @child_id, " +
                "       @uom_name, " +
                "       @uom_desc, " +
                "       @uom_category, " +
                "       @pack_name, " +
                "       @spec, " +
                "       @is_visible, " +
                "       @uom_type, " +
                "       @is_default, " +
                "       @is_min_io_pack_uom, " +
                "       @is_min_uom, " +
                "       @seq_no, " +
                "       @rate, " +
                "       @is_disabled, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "物料单位表");

            return this;
        }

        public ItemUom Insert(DataRow row)
        {
            Insert(new ItemUomModel()
            {
                ID = row["t_id"].ToString(),
                ClientID = row["client_id"].ToString(),
                OrgID = row["org_id"].ToString(),
                WarehouseID = row["warehouse_id"].ToString(),
                ItemID = row["item_id"].ToString(),
                ItemCatalogID = row["item_catalog_id"].ToString(),
                Tag = row["tag"].ToString(),
                SupplierID = row["supplier_id"].ToString(),
                UomID = row["uom_id"].ToString(),
                ChildID = row["child_id"].ToString(),
                UomName = row["uom_name"].ToString(),
                UomDesc = row["uom_desc"].ToString(),
                UomCategory = row["uom_category"].ToString(),
                PackName = row["pack_name"].ToString(),
                Spec = row["spec"].ToString(),
                IsVisible = row["is_visible"].ToString(),
                UomType = row["uom_type"].ToString(),
                IsDefault = row["is_default"].ToString(),
                IsMinIOPackUom = row["is_min_io_pack_uom"].ToString(),
                IsMinUom = row["is_min_uom"].ToString(),
                SeqNo = DBHelper.GetDecimal(row["seq_no"]),
                Rate = DBHelper.GetDecimal(row["rate"]),
                IsDisabled = row["is_disabled"].ToString(),
                CreateDate = DBHelper.GetDateTime(row["create_date"]),
                CreatedBy = row["created_by"].ToString(),
                ModifyIP = row["modify_ip"].ToString(),
                ModifyDate = DBHelper.GetDateTime(row["modify_date"]),
                LastModifiedBy = row["last_modified_by"].ToString(),
                ActStatus = row["act_status"].ToString()
            });

            return this;
        }
        #endregion

        #region BLL

        public DataRow GetItemCatalogWithUom(string itemCatalogID)
        {
            DataRow itemCatalogRow = ItemCatalog.Get(itemCatalogID);
            while (true)
            {
                if (!string.IsNullOrWhiteSpace(itemCatalogRow["default_pack_uom_id"].ToString()))
                {
                    break;
                }
                if (string.IsNullOrWhiteSpace(itemCatalogRow["p_id"].ToString()))
                {
                    throw new WmsException(500, "找不到可用的物料种类单位");
                }
                itemCatalogRow = ItemCatalog.Get(itemCatalogRow["p_id"].ToString());
            }
            return itemCatalogRow;
        }

        public UomConverterModel GetItemSupplierUomConvertModel(string itemID, string supplierID)
        {
            UomConverterModel model = new UomConverterModel();
            DataRow itemSupplierRow = Item.GetSupplier(itemID, supplierID);
            model.DefaultPackUomID = itemSupplierRow["default_pack_uom_id"].ToString();
            model.DefaultStatUomID = itemSupplierRow["default_stat_uom_id"].ToString();
            DataRow defaultPackUomRow = Get(model.DefaultPackUomID);
            model.DefaultPackRate = decimal.Parse("0" + defaultPackUomRow["rate"]);
            DataRow defaultStatUomRow = Get(model.DefaultStatUomID);
            model.DefaultStatRate = decimal.Parse("0" + defaultStatUomRow["rate"]);
            model.Rate = decimal.Parse("0" + itemSupplierRow["rate"]);
            return model;
        }

        public UomConverterModel GetItemUomConvertModel(string itemID)
        {
            UomConverterModel model = new UomConverterModel();
            DataRow itemRow = Item.Get(itemID);
            model.DefaultPackUomID = itemRow["default_pack_uom_id"].ToString();
            model.DefaultStatUomID = itemRow["default_stat_uom_id"].ToString();
            DataRow defaultPackUomRow = Get(model.DefaultPackUomID);
            model.DefaultPackRate = decimal.Parse("0" + defaultPackUomRow["rate"]);
            DataRow defaultStatUomRow = Get(model.DefaultStatUomID);
            model.DefaultStatRate = decimal.Parse("0" + defaultStatUomRow["rate"]);
            model.Rate = decimal.Parse("0" + itemRow["rate"]);
            return model;
        }

        public UomConverterModel GetItemCatalogUomConvertModel(string itemCatalogID)
        {
            UomConverterModel model = new UomConverterModel();
            DataRow itemCatalogRow = GetItemCatalogWithUom(itemCatalogID);
            model.DefaultPackUomID = itemCatalogRow["default_pack_uom_id"].ToString();
            model.DefaultStatUomID = itemCatalogRow["default_stat_uom_id"].ToString();
            DataRow defaultPackUomRow = Get(model.DefaultPackUomID);
            model.DefaultPackRate = decimal.Parse("0" + defaultPackUomRow["rate"]);
            DataRow defaultStatUomRow = Get(model.DefaultStatUomID);
            model.DefaultStatRate = decimal.Parse("0" + defaultStatUomRow["rate"]);
            model.Rate = decimal.Parse("0" + itemCatalogRow["rate"]);
            if (model.Rate <= 0m)
            {
                throw new WmsException(500, "物料种类表中的默认转换率设置有误");
            }
            if (model.DefaultPackRate <= 0m)
            {
                throw new WmsException(500, $"物料种类表中{defaultPackUomRow["uom_name"]}的转换率设置有误");
            }
            if (model.DefaultStatRate <= 0m)
            {
                throw new WmsException(500, $"物料种类表中{defaultStatUomRow["uom_name"]}的转换率设置有误");
            }
            return model;
        }

        public UomModel ConvertFromDefaultPackUom(decimal qty, string destUomID, decimal spec = 1m)
        {
            DataRow destItemUom = Get(destUomID);

            if (destItemUom["is_default"].ToString() == "1"
                //&& destItemUom["uom_type"].ToString() == UomType.Stat)
                && destItemUom["uom_type"].ToString() == UomType.UomType_Stat.ToString())
            {
                return new UomModel()
                {
                    Qty = qty,
                    UomID = destUomID
                };
            }

            string itemID = destItemUom["item_id"].ToString().Trim();
            string itemCatalogID = destItemUom["item_catalog_id"].ToString().Trim();
            string supplierID = destItemUom["supplier_id"].ToString().Trim();

            decimal destRate = 1m;
            UomConverterModel model = new UomConverterModel();

            if (!string.IsNullOrWhiteSpace(itemID))
            {
                if (!string.IsNullOrWhiteSpace(supplierID))
                {
                    model = GetItemSupplierUomConvertModel(itemID, supplierID);
                }
                else
                {
                    model = GetItemUomConvertModel(itemID);
                }
            }
            else if (!string.IsNullOrWhiteSpace(itemCatalogID))
            {
                model = GetItemCatalogUomConvertModel(itemCatalogID);
            }
            else
            {
                throw new WmsException(500, "无法进行计量单位转换!");
            }

            if (destItemUom["uom_type"].ToString() == "UomType_Pack")
            {
                return new UomModel()
                {
                    Qty = decimal.Parse(destItemUom["rate"].ToString()) / model.DefaultPackRate * qty,
                    UomID = destUomID
                };
            }

            if (destItemUom["is_default"].ToString() != "1")
            {
                destRate = model.DefaultStatRate / decimal.Parse(destItemUom["rate"].ToString());
            }

            return new UomModel()
            {
                Qty = model.Rate * destRate * qty * spec,
                UomID = destUomID
            };
        }

        public UomModel ConvertFromDefaultStatUom(decimal qty, string destUomID, decimal spec = 1m)
        {
            DataRow destItemUom = Get(destUomID);

            if (destItemUom["is_default"].ToString() == "1"
                //&& destItemUom["uom_type"].ToString() == UomType.Stat)
                && destItemUom["uom_type"].ToString() == UomType.UomType_Stat.ToString())
            {
                return new UomModel()
                {
                    Qty = qty,
                    UomID = destUomID
                };
            }

            string itemID = destItemUom["item_id"].ToString().Trim();
            string itemCatalogID = destItemUom["item_catalog_id"].ToString().Trim();
            string supplierID = destItemUom["supplier_id"].ToString().Trim();

            decimal destRate = 1m;
            UomConverterModel model = new UomConverterModel();


            if (!string.IsNullOrWhiteSpace(itemID))
            {
                if (!string.IsNullOrWhiteSpace(supplierID))
                {
                    model = GetItemSupplierUomConvertModel(itemID, supplierID);
                }
                else
                {
                    model = GetItemUomConvertModel(itemID);
                }
            }
            else if (!string.IsNullOrWhiteSpace(itemCatalogID))
            {
                model = GetItemCatalogUomConvertModel(itemCatalogID);
            }
            else
            {
                throw new WmsException(500, "无法进行计量单位转换!");
            }

            if (destItemUom["uom_type"].ToString() == "UomType_Stat")
            {
                return new UomModel()
                {
                    Qty = decimal.Parse(destItemUom["rate"].ToString()) / model.DefaultStatRate * qty,
                    UomID = destUomID
                };
            }

            if (destItemUom["is_default"].ToString() != "1")
            {
                destRate = decimal.Parse(destItemUom["rate"].ToString()) / model.DefaultPackRate;
            }

            return new UomModel()
            {
                Qty = qty / model.Rate / destRate / spec,
                UomID = destUomID
            };
        }

        public UomModel ConvertToDefaultStatUom(string uomID, decimal qty, decimal spec = 1m)
        {
            UomModel model = new UomModel()
            {
                UomID = uomID,
                Qty = qty,
            };
            return ConvertToDefaultStatUom(model, spec);
        }


        public UomModel ConvertToDefaultStatUom(UomModel srcQty, decimal? spec = null)
        {
            DataRow srcItemUom = Get(srcQty.UomID);

            if (srcItemUom["is_default"].ToString() == "1"
                //&& srcItemUom["uom_type"].ToString() == UomType.Stat)
                && srcItemUom["uom_type"].ToString() == UomType.UomType_Stat.ToString())
            {
                return srcQty;
            }

            string itemID = srcItemUom["item_id"].ToString().Trim();
            string itemCatalogID = srcItemUom["item_catalog_id"].ToString().Trim();
            string supplierID = srcItemUom["supplier_id"].ToString().Trim();

            decimal srcRate = 1m;
            UomConverterModel model = new UomConverterModel();


            if (!string.IsNullOrWhiteSpace(itemID))
            {
                if (!string.IsNullOrWhiteSpace(supplierID))
                {
                    model = GetItemSupplierUomConvertModel(itemID, supplierID);
                }
                else
                {
                    model = GetItemUomConvertModel(itemID);
                }
            }
            else if (!string.IsNullOrWhiteSpace(itemCatalogID))
            {
                model = GetItemCatalogUomConvertModel(itemCatalogID);
            }
            else
            {
                return srcQty;
            }

            if (srcItemUom["uom_type"].ToString() == "UomType_Stat")
            {
                return new UomModel()
                {
                    Qty = model.DefaultStatRate / decimal.Parse(srcItemUom["rate"].ToString()) * srcQty.Qty,
                    UomID = model.DefaultStatUomID
                };
            }

            if (srcItemUom["is_default"].ToString() != "1")
            {
                srcRate = decimal.Parse(srcItemUom["rate"].ToString()) / model.DefaultPackRate;
            }

            if (spec == null)
            {
                if (!string.IsNullOrWhiteSpace(itemID))
                {
                    DataRow itemRow = Item.Get(itemID);
                    if (!string.IsNullOrWhiteSpace(itemRow[i.Spec].ToString()))
                    {
                        spec = decimal.Parse("0" + itemRow[i.Spec]);
                    }
                }
                if (spec == null)
                {
                    spec = 1m;
                }
            }
            return new UomModel()
            {
                Qty = srcRate * model.Rate * srcQty.Qty * (decimal)spec,
                UomID = model.DefaultStatUomID
            };
        }

        public UomModel ConvertToDefaultPackUom(UomModel srcQty, decimal spec = 1m)
        {
            DataRow srcItemUom = Get(srcQty.UomID);

            if (srcItemUom["is_default"].ToString() == "1"
                //&& srcItemUom["uom_type"].ToString() == UomType.Pack)
                && srcItemUom["uom_type"].ToString() == UomType.UomType_Pack.ToString())
            {
                return srcQty;
            }

            string itemID = srcItemUom["item_id"].ToString().Trim();
            string itemCatalogID = srcItemUom["item_catalog_id"].ToString().Trim();
            string supplierID = srcItemUom["supplier_id"].ToString().Trim();

            //decimal destRate = 1m;
            UomConverterModel model = new UomConverterModel();
            decimal srcRate = 1m;

            if (!string.IsNullOrWhiteSpace(itemID))
            {
                if (!string.IsNullOrWhiteSpace(supplierID))
                {
                    model = GetItemSupplierUomConvertModel(itemID, supplierID);
                }
                else
                {
                    model = GetItemUomConvertModel(itemID);
                }
            }
            else if (!string.IsNullOrWhiteSpace(itemCatalogID))
            {
                model = GetItemCatalogUomConvertModel(itemCatalogID);
            }
            else
            {
                return srcQty;
            }

            //if (srcItemUom["uom_type"].ToString() == UomType.Pack)
            if (srcItemUom["uom_type"].ToString() == UomType.UomType_Pack.ToString())
            {
                return new UomModel()
                {
                    Qty = decimal.Parse(srcItemUom["rate"].ToString()) / model.DefaultPackRate * srcQty.Qty,
                    UomID = model.DefaultPackUomID
                };
            }

            if (srcItemUom["is_default"].ToString() != "1")
            {
                srcRate = decimal.Parse(srcItemUom["rate"].ToString()) / model.DefaultStatRate;
            }

            return new UomModel()
            {
                Qty = srcRate / model.Rate * srcQty.Qty / spec,
                UomID = model.DefaultPackUomID
            };
        }

        #endregion
    }



}
