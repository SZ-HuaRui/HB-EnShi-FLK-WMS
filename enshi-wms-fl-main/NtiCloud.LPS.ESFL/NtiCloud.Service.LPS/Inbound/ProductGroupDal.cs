using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS.Inbound;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static NtiCloud.Model.WMS.Inbound.GroupCPInput;

namespace NtiCloud.Service.WMS.Inbound
{
    public class ProductGroupDal : WmsObject
    {
        #region 成品组盘入库

        public void GroupProduct(GroupCPModel model)
        {
            bool hasBarCode = false;
            DataTable dtItem = null;

            try
            {
                #region 数据准备
                if (string.IsNullOrEmpty(model.ItemId))
                {
                    hasBarCode = true;

                    string code = model.BarcodeList[0];
                    //有码模式
                    if (code.Length < 8)
                    {
                        throw new Exception("件烟码格式错误，请检查！");
                    }

                    code = code.Substring(2, 6);

                    dtItem = GetItemByCodeOrId(code);
                    if (dtItem.Rows.Count == 0)
                    {
                        throw new Exception("件烟码无法解析成品牌号！牌号编码：" + code);
                    }

                    model.Qty = model.BarcodeList.Count;
                }
                else
                {
                    //无码
                    dtItem = GetItemByCodeOrId(model.ItemId);
                    if (dtItem.Rows.Count == 0)
                    {
                        throw new Exception("无法获取成品牌号！牌号id：" + model.ItemId);
                    }
                }

                DataRow row = dtItem.Rows[0];
                model.ItemId = row["T_ID"].ToString();
                model.ItemCode = row["ITEM_CODE"].ToString();
                model.ItemName = row["ITEM_NAME"].ToString();
                model.UomId = row["WEIGHT_UOM_ID"].ToString();
                #endregion

                #region 检查托盘是否重复
                DataTable dtCont = GetContainerBarCode(model.ContainerBarcode);
                if (dtCont.Rows.Count > 0)
                {
                    throw new Exception($"托盘 {model.ContainerBarcode} 条码已存在，请检查！");
                }
                #endregion

                string containerId = SnowflakeID.Guid().ToString(),
                    containerDetailId = SnowflakeID.Guid().ToString();

                //分配货位
                LPS.Inbound.Putaway putaway = new LPS.Inbound.Putaway();
                string zoneId = WmsConfig.GetOrDefault("Inbound:InboundZone", "1293844199577358336");
                string locationRow = putaway.AllocateOneLocationForPallet("1016319470013779968", "", model.ItemId, zoneId);
                if (string.IsNullOrEmpty(locationRow)) throw new Exception("无法分配货位行！");

                //根据行查询预分配货位
                string locationId = GetLocationByRow(locationRow);

                //创建容器主表
                CreateContainer(model, containerId, locationId);

                //创建容器明细
                CreateContainerDetail(model, containerId, containerDetailId);

                //创建件烟条码
                if (hasBarCode)
                {
                    CreateSN(model, containerId, containerDetailId);
                }

                //创建任务
                CreateTask(model, containerId, locationRow, out string taskId);

                //创建任务明细
                CreateTaskItem(model, taskId);

                //任务下发wcs
                List<string> _idList = new List<string> { taskId };
                new Interface.WCS.WorkTaskInterface().Send(_idList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataTable GetItemByCodeOrId(string codeOrId)
        {
            string sql = $@"select *
                              from &DataSource&wms_m_item
                             where warehouse_id='&WAREHOUSE_ID&'
                               and (item_code = @item_code_id
                                    or t_id = @item_code_id)";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "item_code_id", codeOrId }
            };
            DataTable dt = DBHelper.Select(sql, param, "", false);
            return dt;
        }

        private DataTable GetContainerBarCode(string containerCode)
        {
            string sql = $@"select *
                              from &DataSource&wms_b_container_h
                             where warehouse_id='&WAREHOUSE_ID&'
                               and container_barcode = @container_barcode and act_status='1'";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "container_barcode", containerCode }
            };
            DataTable dt = DBHelper.Select(sql, param, "", false);
            return dt;
        }

        private string GetLocationByRow(string row)
        {
            string sql = $@"select *
                              from &DataSource&wms_m_location
                             where warehouse_id='&WAREHOUSE_ID&'
                               and location_row = @location_row
                               and location_status = 'LocationStatus_Plan'
                             order by location_no desc";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "location_row", row.Trim() }
            };
            DataTable dt = DBHelper.Select(sql, param, $"通过行 {row} 获取预分配货位");
            return dt.Rows[0]["t_id"].ToString();
        }

        private void CreateContainer(GroupCPModel model, string containerId, string locationId)
        {
            //获取暂存区货位id
            string flag = model.IsFull.ToString();

            string sql = $@"insert into &DataSource&wms_b_container_h
                       (t_id,client_id,warehouse_id,location_id,container_barcode,is_full,container_type,
                        bom_id,first_in_date,create_date,created_by,act_status,is_in)
                     values
                       (@t_id,null,'&WAREHOUSE_ID&',@location_id,@barcode,@flag,@container_type,
                        @bom_id,now(),now(),@created_by,'1',0)";

            Dictionary<string, object> param1 = new Dictionary<string, object>()
            {
                { "t_id", containerId },
                { "location_id", locationId },
                { "barcode", model.ContainerBarcode },
                { "flag", flag },
                { "container_type",  ContainerType.ContainerType_Actual.ToString() },
                { "bom_id", model.ItemId },
                { "created_by", Context.UserInfo.UserName },
            };

            DBHelper.RunSql(sql, param1, "添加主容器信息");
        }

        private void CreateContainerDetail(GroupCPModel model, string containerId, string containerDetailId)
        {
            string sql = $@"insert into &DataSource&wms_b_container_d 
                                (t_id,client_id,warehouse_id,inbound_h_id,container_id,supplier_id,
                                 item_id,item_name,lot_no,exist_barcode,qty,uom_qty_id,qc_status,
                                 spec,first_in_date,create_date,created_by,act_status,uom_stat_id,stat_qty)
                            values
                                (@t_id,null,'&WAREHOUSE_ID&',null,@container_id,null,
                                 @item_id,@item_name,null,'1',@qty,@uom_id,@qc_status,
                                (select spec from &DataSource&wms_m_item where t_id =@item_id),now(),now(),@created_by,'1',null,null)";

            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "t_id", containerDetailId },
                { "container_id", containerId },
                { "item_id",  model.ItemId },
                { "item_name", model.ItemName },
                { "qty", model.Qty.ToString() },
                { "uom_id",  model.UomId },
                { "qc_status", QcStatus.QcStatus_CheckPassed.ToString() },
                { "created_by", Context.UserInfo.UserName },
            };
            DBHelper.RunSql(sql, param, "添加容器明细信息");
        }

        private void CreateSN(GroupCPModel model, string containerId, string containerDetailId)
        {
            string sql = @"
                insert into &DataSource&wms_b_sn_h
                  (t_id, client_id, org_id, warehouse_id, item_sn, item_id, item_name, container_id, 
                   container_no, container_d_id, position, create_date, created_by, act_status)
                values
                  (@t_id, null, '&ORG_ID&', '&WAREHOUSE_ID&', @item_sn, @item_id, @item_name, @container_id,
                   null, @container_d_id, @position, sysdate, @created_by, '1')";

            string itemId = model.ItemId, itemName = model.ItemName, user = Context.UserInfo.UserName;
            int count = model.BarcodeList.Count;

            for (int i = 0; i < count; i++)
            {
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    { "t_id", SnowflakeID.Guid().ToString() },
                    { "item_sn", model.BarcodeList[i] },
                    { "item_id", itemId },
                    { "item_name", itemName },
                    { "container_id", containerId },
                    { "container_d_id",  containerDetailId },
                    { "position",(i + 1).ToString("00") },
                    { "created_by",user },
                };
                DBHelper.RunSql(sql, param, "添加SN");
            }
        }

        private void CreateTask(GroupCPModel model, string containerId, string locationRow, out string id)
        {
            string t_id = id = SnowflakeID.Guid().ToString();

            string taskNo = WmsHelper.GetSequence("TaskNo");
            string sql = $@"insert into &DataSource&wms_b_task
                             (t_id,client_id,warehouse_id,zone_id,task_no,task_type,executor_type,container_id,container_barcode,src_addr_text,
                              dest_addr_text,ref_doc_type,task_status,start_date,end_date,create_date,created_by,act_status)
                           values
                             (@t_id,null,'&WAREHOUSE_ID&',@zone_id,@task_no,@task_type,@executor_type,@container_id,@barcode,@src_addr_text,
                              @dest_addr_text,@doc_type,@task_status,now(),now(),now(),@created_by,'1')";

            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "t_id", t_id },
                { "zone_id", "1293844199577358336" },
                { "executor_type",TaskExecutor.TaskExecutor_WCS.ToString()},
                { "task_no", taskNo },
                { "task_type", TaskType.TaskType_InBound.ToString() },
                { "doc_type", DocType.DocType_ReceiptGroup.ToString() },
                { "task_status", TaskStatus.TaskStatus_Executing.ToString() },
                { "container_id",  containerId },
                { "barcode", model.ContainerBarcode },
                { "src_addr_text", model.DockNumber },
                { "dest_addr_text", locationRow },
                { "created_by", Context.UserInfo.UserName },
            };
            DBHelper.RunSql(sql, param, "添加主任务");
        }

        private void CreateTaskItem(GroupCPModel model, string taskId)
        {
            string sql = $@"insert into &DataSource&wms_b_task_item
                                (t_id,client_id,warehouse_id,task_id,item_id,item_name,plan_qty,
                                 uom_qty_id,start_date,end_date,create_date,created_by,act_status)
                            values
                                (@t_id,null,'&WAREHOUSE_ID&',@task_id,@item_id,@item_name,@qty,
                                 @uom_id,now(),now(),now(),@created_by,'1')";

            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "t_id", SnowflakeID.Guid().ToString() },
                { "task_id", taskId },
                { "item_id", model.ItemId },
                { "item_name", model.ItemName },
                { "qty",  model.Qty.ToString() },
                { "uom_id", model.UomId },
                { "created_by", Context.UserInfo.UserName },
            };
            DBHelper.RunSql(sql, param, "添加任务明细信息");
        }
        #endregion

        //获取成品牌号
        public DataTable GetProductList()
        {
            string sql = $@"select t_id item_id, item_name
                              from &DataSource&wms_m_item
                             where warehouse_id='&WAREHOUSE_ID&'
                               and catalog_id = '1283282875881492480' and abc='1'";
            DataTable dt = DBHelper.Select(sql, null, "", false);
            return dt;
        }
    }
}
