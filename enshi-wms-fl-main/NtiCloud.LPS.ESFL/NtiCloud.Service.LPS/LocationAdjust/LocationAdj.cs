using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Model.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Inventory;
using NtiCloud.Service.WMS.Tasking;
using NtiCloud.Service.WMS.Model;
using static NtiCloud.Service.WMS.Common.DBHelper;

namespace NtiCloud.Service.WMS.LocationAdjust
{
    public class LocationAdj : WmsObject
    {

        private IDBContextProvider dbW;
        NtiCloud.Service.WMS.Base.CallEnd ce = new Base.CallEnd();
        public LocationAdj(IDBContextProvider _dbW)
        {
            dbW = _dbW;
        }
        public LocationAdj()
        {
        }
        /// <summary>
        /// 依货位调整主档ID获取主档记录
        /// </summary>
        /// <param name="locationAdjustid"></param>
        /// <returns></returns>
        public DataRow GetLocationAdjust(string locationAdjustid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "locationAdjustid", locationAdjustid }
            };
            string sql = $"select h.adjust_no,h.warehouse_id,h.zone_id,h.adjust_state,h.Exist_barcode from &DataSource&wms_b_locationadjust_h h where T_id=@locationAdjustid";
            DataTable dt = DBHelper.Select(sql, param, "");

            return dt.Rows[0];
        }
        /// <summary>
        /// 有单据移库，检查移出库位的合法性
        /// </summary>
        public void CheckSourceLocation(string locationcode)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "location_code", locationcode }
            };
            string sql = $"select item_id,sum(qty)qty from wms_b_container_d where container_id in (" +
                        "select T_id from wms_b_container_H where location_id in(select distinct T_id from wms_m_location where location_code =@location_code))" +
                        "group by item_id having sum(qty)>0";
            DataTable dt = DBHelper.Select(sql, param, "");

            if (dt.Rows.Count == 0)
            {
                throw new WmsException(500, "未找到该货位的有效信息!");
            }

        }
        /// <summary>
        /// 有单据移库，检查容器的合法性
        /// </summary>
        /// <param name="Locationcode"></param>
        /// <param name="Container_no"></param>
        public void CheckContainer(string Locationcode, string Container_no)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "location_code", Locationcode },
                { "container_barcode",Container_no }

            };
            string sql = $"select item_id,sum(qty)qty from wms_b_container_d where container_id in (" +
                        "select T_id from wms_b_container_H where container_barcode=@container_barcode and " +
                        "location_id in(select distinct T_id from wms_m_location where location_code =@location_code))" +
                        "group by item_id having sum(qty)>0";
            DataTable dt = DBHelper.Select(sql, param, "");

            if (dt.Rows.Count == 0)
            {
                throw new WmsException(500, "未找到该容器的有效信息!");
            }
        }

        /// <summary>
        /// 检查货位调整增加明细数据的合法性
        /// </summary>
        /// <param name="LocAdjustd"></param>
        public void CheckLocAdustD(DataRow adr)
        {

            #region 判断输入参数
            if (string.IsNullOrWhiteSpace(adr["Location_out_id"].ToString()))
            {

                throw new WmsException(500, "移出库位不可为空");
            }
            if (string.IsNullOrWhiteSpace(adr["Location_in_id"].ToString()))
            {
                throw new WmsException(500, "移入库位不可为空");
            }
            if (string.IsNullOrWhiteSpace(adr["Item_id"].ToString()))
            {
                throw new WmsException(500, "移出物料不可为空");
            }
            if (!decimal.TryParse(adr["Qty"].ToString(), out decimal qty))
            {
                throw new WmsException(500, "移出数量必须为数字");

            }
            if (decimal.TryParse(adr["Qty"].ToString(), out qty))
            {
                if (qty <= 0)
                {
                    throw new WmsException(500, "移出数量必须大于0");
                }
            }
            Container container = new Container();
            DataTable Tcontainerd = container.getContainerDetail(adr["Location_out_id"].ToString(), adr["Item_id"].ToString(), adr["Wms_Lot_No"].ToString());

            DataRow RLocationAH = GetLocationAdjust(adr["Adjust_hid"].ToString());
            if (Tcontainerd.Rows[0]["exist_barcode"].ToString() != RLocationAH["exist_barcode"].ToString())
            {
                throw new WmsException(500, "明细货位调整标识必须与主档一致");
            }

            if (decimal.Parse(Tcontainerd.Rows[0]["qty"].ToString()) != decimal.Parse(adr["Qty"].ToString()))
            {
                throw new WmsException(500, "移出数量不能超过移出容器库存行数据!");
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "location_in_id", adr["Location_in_id"].ToString()}
            };
            string sql = $"select l.warehouse_id," +
                                   "l.t_id," +
                                   "l.location_code," +
                                   "nvl(l.container_limit, 0) as container_limit," +
                                   "nvl(l.qty_limit, 0) as qty_limit," +
                                   "nvl(l.weight_limit, 0) as weight_limit," +
                                   "count(h.container_barcode) as plts," +
                                   "nvl(sum(nvl(d.qty, 0) + nvl(in_qty, 0)), 0) as totalQty" +
                                   "nvl(max(u.uom_code), 'EA') as uom" +
                              "from &DataSource&wms_m_location l" +
                              "left join &DataSource&wms_b_container_h h " +
                              "on l.t_id = h.location_id" +
                              "left  join &DataSource&wms_b_container_d d" +
                              "on h.t_id = d.container_id" +
                              "left join &DataSource&wms_m_uom u" +
                              "on d.uom_qty_id = d.t_id" +
                              "where l.location_status <> 'StockStatus_Full'" +
                              "and I.t_id=@location_in_id" +
                              "and l.location_flag not in ('LocationFlag_Hold','LocationFlag_Forbidden','LocationFlag_OutOnly')" +
                              "group by l.warehouse_id,l.t_id,l.location_code, l.container_limit,l.qty_limit, l.weight_limit; ";
            DataRow dr = DBHelper.Select(sql, param, "").Rows[0];

            decimal qty_limit = decimal.Parse(dr["qty_limit"].ToString());
            decimal iQty = decimal.Parse(adr["totalQty"].ToString());
            if (iQty >= qty_limit)
            {
                throw new WmsException(500, "移入货位可用容量小于移出数量");
            }

            #endregion

        }
        /// <summary>
        /// 扫托盘直接移库，检查容器的合法性，并返回容器明细物料
        /// </summary>
        /// <param name="ContainerCode"></param>
        public DataTable GetContainerItem(string sLocationcode, string sContainerCode)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "Location_no", sLocationcode }

            };
            string sql;
            sql = "select count(1) as qty from &DataSource&wms_m_location where Location_no=@Location_no" +
                " and org_id='&ORG_ID&' ";
            DataTable dt = DBHelper.Select(sql, param, "查询库位的合法性");
            if (dt.Rows.Count == 0)
            {
                throw new WmsException(500, "未找到该库位，请输入正确库位编码");
            }

            param = new Dictionary<string, object> {
                { "container_barcode", sContainerCode }

            };

            sql = "select count(1) as qty from &DataSource&wms_b_container_h where container_barcode=@container_barcode" +
                " and org_id='&ORG_ID&' ";

            dt = DBHelper.Select(sql, param, "查询调容器的合法性");
            if (dt.Rows.Count == 0)
            {
                throw new WmsException(500, "未找到该容器，请输入正确容器");
            }
            sql = "select sum(In_qty) Inqty,Sum(Out_Qty) outqty from &DataSource&wms_b_container_d " +
                   "where container_id in (select T_id from &DataSource&wms_b_container_h where container_barcode =@container_barcode )" +
                   "and org_id='&ORG_ID&'";


            DataTable adt = DBHelper.Select(sql, param, "查询容器待入待出数量");
            if (decimal.Parse(adt.Rows[0]["Inqty"].ToString()) > 0)
            {
                throw new WmsException(500, "容器有待入数量，不允许移库");
            }
            if (decimal.Parse(adt.Rows[0]["outqty"].ToString()) > 0)
            {
                throw new WmsException(500, "货位有待出数量，不允许移库");
            }
            //取容器物料
            sql = "select distinct item_id ItemId,''ItemCode,item_name ItemName,Qty,uom_qty_id UomId," +
                    "Supplier_Id SupplierId,wms_lot_no lot,ratio Rate from &DataSource&wms_b_container_d " +
                  "where container_id in (select T_id from &DataSource&wms_b_container_h where container_barcode =@container_barcode)" +
                  "and org_id='&ORG_ID&' ";
            DataTable idt = DBHelper.Select(sql, param, "查询容器明细物料");

            return idt;
        }
        /// <summary>
        /// 取得平库库位上的物料
        /// </summary>
        /// <param name="sLocationcode"></param>
        /// <returns></returns>
        public DataTable GetLocationItem(string sLocationcode)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "container_barcode", sLocationcode }

            };
            string sql;

            sql = "select distinct * from &DataSource&wms_b_container_d " +
                  "where container_id in (select T_id from &DataSource&wms_b_container_h where container_barcode =@container_barcode)" +
                  "and org_id='&ORG_ID&' ";
            DataTable idt = DBHelper.Select(sql, param, "查询平库货位明细物料");

            return idt;
        }
        /// <summary>
        /// 依货位移动主档的ID得到明细记录
        /// </summary>
        /// <param name="sHid"></param>
        /// <returns></returns>
        public DataTable GetLocationAdjustD(string sHid)
        {

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "AdjustID", sHid }

            };
            string sql;
            sql = "select * from &DataSource&wms_b_locationadjust_D  " +
                          " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                          "    and adjust_HID = @AdjustID ";

            DataTable dt = DBHelper.Select(sql, param, "查询调整单明细");
            return dt;

        }
        /// <summary>
        /// 获取未执行，执行中的移库单号
        /// </summary>
        /// <param name="locationcode"></param>
        /// <returns></returns>
        public DataTable GetAdjustNo()
        {
            Dictionary<string, object> param = new Dictionary<string, object>
            {

            };
            string sql;
            sql = " select distinct adjust_no,client_id,org_id,T_id AdjustID,Warehouse_id," +
                "Zone_id,exist_barcode IsExistsBarcode from &DataSource&wms_b_locationadjust_h where adjust_state in('AdjustState_NotExecute','AdjustState_Executing')and client_id='&CLIENT_ID&'";

            DataTable dt = DBHelper.Select(sql, param, "查找库位调整单号");

            return dt;
        }
        /// <summary>
        /// 依据库单获取物料明累
        /// </summary>
        /// <param name="AdjustNo"></param>
        /// <returns></returns>
        public DataTable GetItembyAdjustNo(string AdjustNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "Adjust_No", AdjustNo }

            };
            string sql;
            sql = "select T_id,Item_code from &DataSource&wms_b_task_item " +
                "where task_id=(select T_id from &DataSource&wms_b_task where ref_doc_no=@Adjust_No) " +
                "and client_id='&CLIENT_ID&'";

            DataTable dt = DBHelper.Select(sql, param, "查找移库单下的物料");

            return dt;

        }
        /// <summary>
        /// 判断移入库位合法性，判定移入库位的容量
        /// </summary>
        /// <param name="sLocationNo"></param>
        public void CheckToInLocation(string sLocationNo)
        {

            Container acon = new Container();
            if (!acon.CheckLocationExis(sLocationNo))
            {
                throw new WmsException(500, "未找到该货位编号，请重新输入!");
            }

            string sWarehouse_id = Context.WarehouseID;
            string sql = @"select warehouse_id, t_id, location_code, container_limit, qty_limit, weight_limit, plts, totalqty, uom 
                            from &DataSource&view_location_storageinfo where  location_code =@location_code";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();

            lstParam.Add("Location_Code", sLocationNo);
            DataTable adt = DBHelper.Select(sql, lstParam);
            if (decimal.Parse(adt.Rows[0]["qty_limit"].ToString()) - decimal.Parse(adt.Rows[0]["totalqty"].ToString()) <= 0)
            {
                throw new WmsException(500, "该货位已无容量，请输入其它货位!");
            }

        }
        public void CheckToInContainer(string scontainerNo, string swms_lot, string sitemid)
        {
            Container acon = new Container();

            string sql = @"select * from wms_b_container_h where  container_barcode =@container_barcode";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();

            lstParam.Add("container_barcode", scontainerNo);
            DataTable adt = DBHelper.Select(sql, lstParam);
            if (adt.Rows.Count == 0)
            {
                throw new WmsException(500, "无此容器，请重新输入!");
            }
            ContainerModel containerh = acon.GetContainer(Context.WarehouseID, scontainerNo);
            bool isexist = acon.CheckContainerDetailExists(containerh.t_id, sitemid, swms_lot);
            if (!isexist)
            {
                throw new WmsException(500, "同一容器，不能混物料混批存放!");
            }
        }

        /// <summary>
        /// 插入货位调整明细记录
        /// </summary>
        /// <param name="LocAdjustd"></param>
        public bool InsertLocationAdjustD(LocationAdjustModel.LocationAdjustD LocAdjustd)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "t_id",SnowflakeID.Guid().ToString()},
                { "warehouse_id", Context.WarehouseID },
                { "adjust_hid",LocAdjustd.Adjust_hid},
                {"item_id",LocAdjustd.Item_id },
                { "lot_no",LocAdjustd.Lot_No},
                { "wms_lot_no",LocAdjustd.Wms_Lot_No},
                { "supplier_id",LocAdjustd.Supplier_id},
                {"location_in_id",LocAdjustd.Location_in_id },
                { "location_out_id",LocAdjustd.Location_out_id},
                { "qty",LocAdjustd.Qty.ToString()},
                {"uom_qty_id",LocAdjustd.Uom_qty_id },
                {"Sta_Qty", LocAdjustd.Sta_Qty.ToString()},
                {"Uom_stat_id", LocAdjustd.Uom_stat_id},
                { "Speci",LocAdjustd.Speci},
                { "created_by",Context.UserInfo.UserId}
            };

            string sql = @"INSERT INTO &DataSource&WMS_B_LOCATIONADJUST_D (CLIENT_ID,ORG_ID, t_id , warehouse_id , adjust_hid , item_id , 
                        lot_no,wms_lot_no,supplier_id,location_in_id , location_out_id , qty , uom_qty_id,stat_qty,uom_stat_id,speci, CREATED_BY,CREATE_DATE,ACT_STATUS) VALUES 
                        ('&CLIENT_ID&','&ORG_ID&',@t_id,@warehouse_id,@adjust_hid,@item_id,
                        @lot_no,@wms_lot_no,@supplier_id,@location_in_id,@location_out_id,@qty,@uom_qty_id,to_number(@qty)*to_number(@speci),@uom_stat_id,@speci,@created_by,now(),'1')";

            return DBHelper.RunSql(sql, param, "插入货位调整明细表") > 0;

        }
        /// <summary>
        /// 获取货位调整明细记录
        /// </summary>
        /// <param name="LocAdjustd"></param>
        public void UpdateLocationAdjustD(LocationAdjustModel.LocationAdjustD LocAdjustd)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                                { "t_id",LocAdjustd.Adjustdetailid},
                                { "modify_ip",Context.UserInfo.LoginIP},
                                { "qty",LocAdjustd.Qty.ToString()}

           };

            string sql = @"UPDATE &DataSource&WMS_B_LOCATIONADJUST_D
                           SET 
                               qty              = qty+@qty,      
                               MODIFY_IP        = @modify_ip,
                               MODIFY_DATE      = now()
                         WHERE t_id = @t_id";

            DBHelper.RunSql(sql, param, "修改货位调整明细表");

        }
        /// <summary>
        /// 更新货位调整单状态
        /// </summary>
        /// <param name="dbW"></param>
        /// <param name="user"></param>
        /// <param name="AdjustHID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateLocationAdjustStatus(IDBContextProvider dbW, UserLoginInfo user, string AdjustHID, AdjustState status)
        {
            string sql = @" update  &DataSource&wms_b_locationadjust_h set adjust_state = '" + status.ToString() + "',MODIFY_IP = '" + user.LoginIP + "',LAST_MODIFIED_BY = '" + user.UserId + "',MODIFY_DATE=sysdate where t_id = '" + AdjustHID + "' ";
            int rows;
            string msg = "";
            List<string> lstSql = new List<string>();
            lstSql.Add(sql);
            dbW.DBRunSql(user, lstSql, out rows, out msg);
            return rows > 0;
        }
        /// <summary>
        /// 移库单确认产生任务
        /// </summary>
        /// <param name="adjustno"></param>
        public void ConfirmLocationAdjust(string adjustno)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "adjust_no", adjustno }

            };
            string sqlH, sqlD;
            sqlH = "select * from &DataSource&wms_b_locationadjust_H  " +
                              "where client_id='&CLIENT_ID&'  " +
                              "and adjust_no = @adjust_no ";
            sqlD = "select a.t_id as AdjustdetailID, a.org_id, a.client_id, a.warehouse_id, a.adjust_hid," +
                            "a.item_id,(select Item_name from &DataSource&wms_m_item where T_id=a.item_id) as Item_name," +
                            "a.lot_no, a.owner_id, " +
                            "a.location_in_id,(select location_no from &DataSource&wms_m_location where T_id=a.location_in_id) as location_in_code," +
                            "a.location_out_id ,(select location_no from &DataSource&wms_m_location where T_id=a.location_out_id) as location_out_code, " +
                            "a.qty, a.uom_qtyid, a.stat_qty, a.uom_stat_id,  a.speci," +
                            "a.out_container_id,(select container_barcode from &DataSource&wms_b_container_h where T_id=a.out_container_id) as container_barcode," +
                            "a.container_d_id,a.wms_lot_no from &DataSource&WMS_B_LOCATIONADJUST_D a " +
                            "where client_id='&CLIENT_ID&'and " +
                            "adjust_HID=(select T_id from &DataSource&WMS_B_LOCATIONADJUST_H where adjust_no=@adjust_no) ";
            DataTable dtH = DBHelper.Select(sqlH, param, "查询调整单主表");
            DataTable dtD = DBHelper.Select(sqlD, param, "查询调整单明细");
            if (dtH.Rows.Count < 1)
            {
                throw new WmsException(500, "未找到移库单主档!");
            }
            if (dtH.Rows[0]["adjust_state"].ToString() != AdjustState.AdjustState_NotExecute.ToString())
            {
                throw new WmsException(500, "新建的货位调整单才能生成任务!");
            }

            List<LocationAdjustModel.LocationAdjustD> lstlocd = DBHelper.ConvertTableToObject<LocationAdjustModel.LocationAdjustD>(dtD);
            TaskService task = new TaskService();
            TaskHeaderModel tm = new TaskHeaderModel();

            UpdateLocationAdjustStatus(dbW, Context.UserInfo, dtH.Rows[0]["T_id"].ToString(), AdjustState.AdjustState_NotExecute);

            Container con = new Container();

            foreach (DataRow dr in dtD.Rows)
            {
                //检查明细数据是否符合要求 
                CheckLocAdustD(dr);
            }

            foreach (DataRow dr in dtD.Rows)
            {

                //增加目标库位待入数量 string ctnId, string itemid, string wmsLot
                //if (con.CheckContainerDetailExists())
                //{

                //}

                //增加来源库位待出数量

                tm.t_id = SnowflakeID.Guid().ToString();
                tm.task_no = WmsHelper.GetSequence("TaskNo");
                tm.client_id = dtH.Rows[0]["client_id"].ToString();
                tm.org_id = dtH.Rows[0]["org_id"].ToString();
                tm.warehouse_id = dtH.Rows[0]["warehouse_id"].ToString();
                tm.task_type = TaskType.TaskType_Move.ToString();
                tm.seq_no = 0;
                tm.priority = 2;
                tm.container_id = dr["Out_container_id"].ToString();
                tm.container_barcode = dr["container_barcode"].ToString();
                tm.ref_doc_type = "";
                tm.ref_doc_id = dtH.Rows[0]["T_id"].ToString();
                tm.ref_doc_no = dtH.Rows[0]["adjust_no"].ToString();
                tm.ref_doc_d_id = dr["t_id"].ToString();
                tm.dest_addr_id = dr["location_in_id"].ToString();
                tm.dest_addr_text = dr["location_in_code"].ToString();
                tm.dest_addr_type = "Location";
                tm.task_status = WMS.Base.TaskStatus.TaskStatus_Create.ToString();
                tm.is_executable = "0";
                tm.created_by = Context.UserInfo.UserId;
                tm.create_date = DateTime.Now;
                tm.modify_date = DateTime.Now;
                tm.act_status = "1";
                task.AddTask(dbW, Context.UserInfo, tm);

                TaskItemModel tim = new TaskItemModel();
                tim.t_id = SnowflakeID.Guid().ToString();
                tim.client_id = dtH.Rows[0]["client_id"].ToString();
                tim.org_id = dtH.Rows[0]["org_id"].ToString();
                tim.warehouse_id = dtH.Rows[0]["warehouse_id"].ToString();
                tim.task_id = tm.t_id;
                tim.item_id = dr["item_id"].ToString();
                tim.item_name = dr["item_name"].ToString();
                tim.plan_qty = decimal.Parse(dr["Qty"].ToString());
                tim.uom_qty_id = dr["uom_qtyid"].ToString();
                tim.uom_stat_id = dr["uom_stat_id"].ToString();
                tim.container_d_id = dr["container_d_id"].ToString();
                tim.ref_doc_type = "";
                tim.ref_doc_id = dtH.Rows[0]["T_id"].ToString(); ;
                tim.ref_doc_no = dtH.Rows[0]["adjust_no"].ToString();
                tim.ref_doc_d_id = dr["AdjustdetailID"].ToString();
                tim.process_status = ProcessStatus.ProcessStatus_New.ToString();
                tim.created_by = Context.UserInfo.UserId;
                tim.create_date = DateTime.Now;
                tim.modify_date = DateTime.Now;
                tim.act_status = "1";
                task.AddTaskItem(dbW, Context.UserInfo, tim);
            }

        }
        /// <summary>
        /// 移库完成，生成历史任务
        /// </summary>
        /// <param name="locationD"></param>
        public void CreateAdjustHisTask(LocationAdjustModel.LocationAdjustD locationD)
        {
            string sDoc = "", sDocID = "";
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "T_ID", locationD.Adjust_hid }

            };
            string sqlH, sqlD;
            sqlH = "select * from &DataSource&wms_b_locationadjust_H  " +
                              "where client_id='&CLIENT_ID&'  " +
                              "and T_id = @T_id ";
            DataTable dtH = DBHelper.Select(sqlH, param, "查询调整单主表", false);
            if (dtH.Rows.Count > 0)
            {
                sDoc = dtH.Rows[0]["adjust_no"].ToString();
                sDocID = dtH.Rows[0]["t_id"].ToString();
            }
            Container ctn = new Container();
            string sContainerID = ctn.getContainerIdByLocation(locationD.Location_in_code);
            string sContainerDetailID = ctn.getContainerDetailId(sContainerID, locationD.Item_id, locationD.Wms_Lot_No);

            TaskService task = new TaskService();
            TaskHeaderModel tm = new TaskHeaderModel();
            tm.t_id = SnowflakeID.Guid().ToString();
            tm.task_no = WmsHelper.GetSequence("TaskNo");
            tm.client_id = Context.ClientID;
            //tm.org_id = locationD.Org_id;
            tm.warehouse_id = Context.WarehouseID;
            tm.task_type = TaskType.TaskType_Move.ToString();
            tm.seq_no = 0;
            tm.priority = 2;
            tm.container_id = sContainerID;
            tm.container_barcode = locationD.Location_in_code;
            tm.ref_doc_type = DocType.DocType_Adjustment.ToString();
            tm.ref_doc_id = sDocID;
            tm.ref_doc_no = sDoc;
            tm.ref_doc_d_id = "";
            tm.dest_addr_id = locationD.Location_in_id;
            tm.dest_addr_text = "";
            tm.dest_addr_type = AddressType.AddressType_Location.ToString();
            tm.task_status = WMS.Base.TaskStatus.TaskStatus_Finished.ToString();
            tm.is_executable = "0";
            tm.created_by = Context.UserInfo.UserId;
            tm.create_date = DateTime.Now;
            tm.modify_date = DateTime.Now;
            tm.act_status = "1";
            task.AddTask(dbW, Context.UserInfo, tm);

            TaskItemModel tim = new TaskItemModel();
            tim.t_id = SnowflakeID.Guid().ToString();
            tim.client_id = Context.ClientID;
            //tim.org_id = locationD.Org_id;
            tim.warehouse_id = Context.WarehouseID;
            tim.task_id = tm.t_id;
            tim.item_id = locationD.Item_id;
            tim.item_name = "";
            tim.plan_qty = locationD.Qty;
            tim.uom_qty_id = locationD.Uom_qty_id;
            tim.uom_stat_id = locationD.Uom_stat_id;
            tim.container_d_id = sContainerDetailID;
            //tim.ref_doc_type = DocType.DocType_Adjustment.ToString();
            //tim.ref_doc_id = locationD.Adjust_hid;
            //tim.ref_doc_no = dtH.Rows[0]["adjust_no"].ToString();
            //tim.ref_doc_d_id = dr["AdjustdetailID"].ToString();
            tim.process_status = ProcessStatus.ProcessStatus_Done.ToString();
            tim.created_by = Context.UserInfo.UserId;
            tim.create_date = DateTime.Now;
            tim.modify_date = DateTime.Now;
            tim.act_status = "1";
            task.AddTaskItem(dbW, Context.UserInfo, tim);

            WorkTask atask = new WorkTask();
            atask.MoveToHistory(tm.t_id);
        }
        /// <summary>
        /// 有单据无码移库确认，保存数据，更新移库单明细，生成历史任务，转移库存
        /// </summary>
        /// <param name="locationD"></param>
        public void ConfirmAdjustDocRFNoneSN(LocationAdjustModel.LocationAdjustD locationD)
        {
            Location loct = new Location();

            LocationModel loctm = loct.GetLocationByCode(Context.WarehouseID, locationD.Location_out_code);
            locationD.Location_out_id = loctm.t_id;
            loctm = loct.GetLocationByCode(Context.WarehouseID, locationD.Location_in_code);
            locationD.Location_in_id = loctm.t_id;
            if (locationD.Wms_Lot_No == null)
            {
                locationD.Wms_Lot_No = "";
            }

            //判断有相同的调整单明细，则增加数量
            if (CheckAdjustDExist(ref locationD))
            {
                UpdateLocationAdjustD(locationD);
            }
            else//无相应的调整单明细，则增加调整单明细
            {
                InsertLocationAdjustD(locationD);
            }

            //库存信息转移
            UpdateContainerInfo(locationD);

            //产生任务，并转成历史任务
            CreateAdjustHisTask(locationD);
        }

        /// <summary>
        /// 无单据无码移库确认，保存数据，更新移库单明细，生成历史任务，转移库存
        /// </summary>
        /// <param name="locationD"></param>
        public void ConfirmAdjustNDocRFNoneSN(LocationAdjustModel.LocationAdjustD locationD)
        {
            Location loct = new Location();

            string sid = locationD.Container_d_id;
            //ContainerDModel contdm = ctn.getContainerDetailById(sid);

            LocationModel loctm = loct.GetLocationByCode(Context.WarehouseID, locationD.Location_out_code);
            locationD.Location_out_id = loctm.t_id;
            loctm = loct.GetLocationByCode(Context.WarehouseID, locationD.Location_in_code);
            locationD.Location_in_id = loctm.t_id;
            if (locationD.Wms_Lot_No == null)
            {
                locationD.Wms_Lot_No = "";
            }

            //库存信息转移
            UpdateContainerInfo(locationD);

            //产生任务，并转成历史任务
            CreateAdjustHisTask(locationD);
        }

        /// <summary>
        /// RF库位调整后，原库存减移库数量，增加新库存明细数量
        /// </summary>
        /// <param name="locationD"></param>
        public void UpdateContainerInfo(LocationAdjustModel.LocationAdjustD locationD)
        {
            string sid = locationD.Container_d_id;
            decimal actualQty = decimal.Parse("0" + locationD.Qty);
            string sH_ID;
            Container ctn = new Container();
            //Container tcon = new Container();
            Location locat = new Location();

            ContainerDModel contdm = ctn.getContainerDetailById(sid);

            //LocationModel locatm = locat.GetLocationById(locationD.Location_in_id);

            bool isExists = ctn.CheckContainerExisByLocation(locationD.Location_in_code);
            ctn.AddDetailQty(sid, -actualQty, contdm.qty);
            string tcontainerId = ctn.getContainerIdByLocation(locationD.Location_in_code);

            // List<ContainerDModel> lacond = DBHelper.ConvertTableToObject<ContainerDModel>(ctn.getContainerDetail(id, locationD.Item_id, locationD.Wms_Lot_No));
            //ContainerDetailModel tcond = new ContainerDetailModel();
            if (isExists)
            {

                if (ctn.CheckContainerDetailExists(tcontainerId, locationD.Item_id, locationD.Wms_Lot_No))
                {
                    DataTable dt = ctn.getContainerDetail(tcontainerId, locationD.Item_id, locationD.Wms_Lot_No);
                    ctn.AddDetailQty(dt.Rows[0]["T_ID"].ToString(), actualQty, 0);
                    //sH_ID = dt.Rows[0]["T_ID"].ToString();
                }
                //else
                //{
                //    Insertcontainer(locationD);
                //}
                sH_ID = ctn.getContainerIdByLocation(locationD.Location_in_code);
            }
            else
            {
                ContainerHeadModel ctnH = new ContainerHeadModel();
                sH_ID = SnowflakeID.Guid().ToString();
                ctnH.ID = sH_ID;
                ctnH.ClientID = Context.ClientID;
                ctnH.OrgID = "";
                ctnH.WarehouseID = Context.WarehouseID;
                ctnH.ActStatus = "1";
                ctnH.LocationID = locationD.Location_in_id;
                ctnH.ContainerBarcode = locationD.Location_in_code;
                ctnH.ContainerType = "ContainerType_Virtual";
                ctnH.FirstInDate = DateTime.Now;
                ctnH.OwnerID = "";
                ctnH.CreatedBy = Context.UserInfo.UserName;
                ctnH.ModifyDate = DateTime.Now;
                ctn.Insert(ctnH);
            }
            Insertcontainer(locationD, sH_ID);

        }
        public void Insertcontainer(LocationAdjustModel.LocationAdjustD locationD, string sHid)
        {
            ContainerDetailModel tcond = new ContainerDetailModel();
            Container ctn = new Container();

            Location locat = new Location();
            string scontainerId = ctn.getContainerIdByLocation(locationD.Location_out_code);
            List<ContainerDModel> lacond = DBHelper.ConvertTableToObject<ContainerDModel>(ctn.getContainerDetail(scontainerId, locationD.Item_id, locationD.Wms_Lot_No));
            //LocationModel locatm = locat.GetLocationById(locationD.Location_out_id);
            //string tcontainerId = ctn.getContainerIdByLocation(locationD.Location_out_code);
            tcond.ID = SnowflakeID.Guid().ToString();
            tcond.WarehouseID = Context.WarehouseID;
            tcond.ClientID = Context.ClientID;
            tcond.OrgID = "";
            tcond.ContainerID = sHid;
            tcond.ItemID = locationD.Item_id;
            tcond.ItemName = locationD.Item_name;
            tcond.LotNo = locationD.Lot_No;
            tcond.WmsLotNo = locationD.Wms_Lot_No;
            tcond.QCStatus = lacond[0].qc_status;
            tcond.Qty = locationD.Qty;
            tcond.StatQty = locationD.Sta_Qty;
            tcond.Spec = lacond[0].spec;
            tcond.UomQtyID = lacond[0].uom_qty_id;
            tcond.UomStatID = lacond[0].uom_stat_id;
            tcond.Ratio = lacond[0].ratio;
            tcond.FirstInDate = DateTime.Now;
            tcond.ProductionDate = lacond[0].production_date;
            tcond.ExpDate = lacond[0].exp_date;
            tcond.WarningDate = lacond[0].warning_date;
            tcond.FirstOutDate = lacond[0].first_out_date;
            tcond.OutEndDate = lacond[0].out_end_date;
            tcond.UseByDate = lacond[0].use_by_date;
            tcond.CreateDate = DateTime.Now;
            tcond.CreatedBy = Context.UserInfo.UserId;
            tcond.ActStatus = "1";
            ctn.Insert(tcond);
        }
        /// <summary>
        /// 判断新的移库信息能否加到已有的货位调整明细
        /// </summary>
        /// <param name="locationD"></param>
        /// <returns></returns>
        public bool CheckAdjustDExist(ref LocationAdjustModel.LocationAdjustD locationD)
        {
            string sqlD;
            sqlD = "select * from &DataSource&wms_b_locationadjust_D  " +
                              "where client_id='&CLIENT_ID&' and Location_out_id=@Location_out_id " +
                              "and Location_in_id=@Location_in_id and Item_id = @Item_id and (wms_lot_no=@wms_lot_no or @wms_lot_no='')";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "Location_out_id", locationD.Location_out_id },
                { "Location_in_id", locationD.Location_in_id },
                { "Item_id", locationD.Item_id },
                { "wms_lot_no", locationD.Wms_Lot_No }
            };
            try
            {
                DataTable dtd = DBHelper.Select(sqlD, param, "查询调整单物料明细");

                if (dtd.Rows.Count > 0)
                {
                    locationD.Adjustdetailid = dtd.Rows[0]["T_id"].ToString();
                    return true;
                }

                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        #region 库位调整
        /// <summary>
        /// 库位调整
        /// </summary>
        /// <param name="model"></param>
        public void LocationadJust(LocationAdjustModel.LocationadJustModel model)
        {
            DateTime dateTime = DateTime.Now;
            //调整单主档信息
            DataRow drLocJustHInfo = GetLocJustHInfo(model.LocationadJustHTid);
            string adjust_state = drLocJustHInfo["adjust_state"].ToString();//单据状态
            string equipment_id = drLocJustHInfo["equipment_id"].ToString();//执行设备id
            string zone_id = drLocJustHInfo["zone_id"].ToString();//区域id
            string adjust_no = drLocJustHInfo["adjust_no"].ToString();//单号
            string isalllineout = drLocJustHInfo["isalllineout"].ToString();//整行移出标志

            if (adjust_state != AdjustState.AdjustState_NotExecute.ToString())
            {
                throw new WmsException(500, "库位调整仅在单据状态为未执行状态下，才能进行调整！");
            }

            DataRow drTaskNum = SelectTaskNumByRefDocID(model.LocationadJustHTid);
            int _taskNum = Convert.ToInt32(drTaskNum["cnt"].ToString());
            if (_taskNum > 0)
            {
                throw new WmsException(500, "该库位调整已经产生调整任务，无需再次执行调整！");
            }
            //调整单明细信息
            DataTable dtLocJustDInfo = new DataTable();
            string sc01 = WmsConfig.GetOrDefault("Task:SC01", "1288091969666551808");//1号堆垛机t_id
            string sc02 = WmsConfig.GetOrDefault("Task:SC02", "1288092061295316992");//2号堆垛机t_id
            bool isDesc = false;
            if (string.Equals(equipment_id, sc01))
            {
                isDesc = false;
                dtLocJustDInfo = GetLocJustDInfo(model.LocationadJustHTid, false);
            }
            else if (string.Equals(equipment_id, sc02))
            {
                isDesc = true;
                dtLocJustDInfo = GetLocJustDInfo(model.LocationadJustHTid, true);
            }
            else
            {//二楼
                isDesc = true;
                dtLocJustDInfo = GetLocJustDInfo(model.LocationadJustHTid, true);
            }
            //保存移入货位所在行，与移出货位上的盘型之间的对应关系
            Dictionary<string, string> lineAndBom = new Dictionary<string, string>();
            //检查在调整单明细中移出货位是否存在相同的，如果存在，则不允许调整
            List<string> outLocId = new List<string>();
            for (int i = 0; i < dtLocJustDInfo.Rows.Count; i++)
            {
                string location_out_id = dtLocJustDInfo.Rows[i]["location_out_id"].ToString();//移出货位id
                string location_in_id = dtLocJustDInfo.Rows[i]["location_in_id"].ToString();//移入货位id
                string container_htid = dtLocJustDInfo.Rows[i]["out_container_id"].ToString();//移出货位容器主档id，这个容器id为创建单据时保存在明细中的容器id   
                DataRow drContainer = Container.GetContainerH(container_htid);//查询容器信息
                string locOutBom = drContainer["bom_id"].ToString();//容器的Bom
                LocationModel locInInfo = Location.GetLocationById(location_in_id);//移入货位信息
                string locInNo = locInInfo.location_no;//移入货位编码
                string locInLine = locInInfo.location_row.ToString();//移入货位所在行
                if (string.Equals(location_out_id, location_in_id))
                {
                    throw new WmsException(500, "库位调整当中存在移入和移出货位一样的明细，移入和移出货位不能相同！");
                }
                if (!outLocId.Contains(location_out_id))
                {
                    outLocId.Add(location_out_id);
                }
                if (lineAndBom.ContainsKey(locInLine))
                {
                    //已存在key
                    if (!string.Equals(lineAndBom[locInLine].ToString(), locOutBom))
                    {
                        throw new WmsException(500, "库位调整明细中存在移入货位属于同一行，对应的移出货位的盘型不一致的情况，无法进行库位调整！");
                    }
                }
                else
                {
                    lineAndBom.Add(locInLine, locOutBom);
                }
            }
            if (outLocId.Count != dtLocJustDInfo.Rows.Count)
            {
                throw new WmsException(500, "库位调整当中存在移出货位相同的明细，无法将同一个托盘移动到不同的货位上！");
            }
            //检查在调整单明细中移入货位是否存在相同的，如果存在，则不允许调整
            List<string> inLocId = new List<string>();
            for (int i = 0; i < dtLocJustDInfo.Rows.Count; i++)
            {
                string location_in_id = dtLocJustDInfo.Rows[i]["location_in_id"].ToString();//移入货位id
                if (!inLocId.Contains(location_in_id))
                {
                    inLocId.Add(location_in_id);
                }
            }
            if (inLocId.Count != dtLocJustDInfo.Rows.Count)
            {
                throw new WmsException(500, "库位调整当中存在移入货位相同的明细，无法将不同一个托盘移动到相同的货位上！");
            }

            for (int i = 0; i < outLocId.Count; i++)
            {
                LocationModel locOutInfo = Location.GetLocationById(outLocId[i]);//移出货位信息
                int? locOutRow = locOutInfo.location_row;//移出货位所在行
                string locOutNo = locOutInfo.location_no;//移出货位编码
                //LocationModel locInInfo = Location.GetLocationById(location_in_id);//移入货位信息
                //int? locInRow = locOutInfo.location_row;//移入货位所在行
                //string locInNo = locInInfo.location_no;//移入货位编码
                if (IsLocHaveTask(locOutRow.ToString()))
                {
                    throw new WmsException(500, $"移出库位{locOutNo}所在行存在除库位调整以外的任务，请等待任务完成后再执行调整！");
                }
            }

            for (int i = 0; i < inLocId.Count; i++)
            {
                LocationModel locInInfo = Location.GetLocationById(inLocId[i]);//移入货位信息
                int? locInRow = locInInfo.location_row;//移入货位所在行
                string locInNo = locInInfo.location_no;//移入货位编码
                if (IsLocHaveTask(locInRow.ToString()))
                {
                    throw new WmsException(500, $"移入库位{locInNo}所在行存在除库位调整以外的任务，请等待任务完成后再执行调整！");
                }
            }

            List<string> tasklist = new List<string>();
            //更新调整单主档状态
            UpdatelocJustHByTid(model.LocationadJustHTid, AdjustState.AdjustState_Executing.ToString());
            for (int i = 0; i < dtLocJustDInfo.Rows.Count; i++)
            {
                string adjustDTid = dtLocJustDInfo.Rows[i]["t_id"].ToString();//调整单明细id
                string container_htid = dtLocJustDInfo.Rows[i]["out_container_id"].ToString();//移出货位容器主档id，这个容器id为创建单据时保存在明细中的容器id                
                string location_out_id = dtLocJustDInfo.Rows[i]["location_out_id"].ToString();//移出货位id
                string location_in_id = dtLocJustDInfo.Rows[i]["location_in_id"].ToString();//移入货位id
                DataRow drContainer = Container.GetContainerH(container_htid);//查询容器信息
                string taskContainerBarcode = drContainer["container_barcode"].ToString();//容器的托盘条码
                LocationModel locOutInfo = Location.GetLocationById(location_out_id);//移出货位信息

                string locOutNo = locOutInfo.location_no;//移出货位编码
                string locOutStatus = locOutInfo.location_status.ToString();//移出行货位状态
                LocationModel locInInfo = Location.GetLocationById(location_in_id);//移入货位信息
                string locInNo = locInInfo.location_no;//移入货位编码
                string locInLine = locInInfo.location_row.ToString();//移入货位所在行
                string locInStatus = locInInfo.location_status.ToString();//移入行货位状态
                if (!string.Equals(locInStatus, LocationStatus.LocationStatus_Empty.ToString()))
                {
                    throw new WmsException(500, $"移入库位{locInNo}状态不为空,无法进行货位调整！");
                }
                if (!string.Equals(locOutStatus, LocationStatus.LocationStatus_Stored.ToString()))
                {
                    throw new WmsException(500, $"移出库位{locOutNo}状态不为已存储,无法进行货位调整！");
                }
                if (IsLocHaveContainer(location_out_id) == false)
                {
                    throw new WmsException(500, $"移出库位{locOutNo}上没有托盘,无法进行货位调整！");
                }
                DataRow[] drArrLocOutContainer = Container.GetHeadsByLocation(location_out_id);//查询移出货位对应的容器信息
                if (drArrLocOutContainer.Length > 1)
                {
                    throw new WmsException(500, $"移出库位{locOutNo}上的存在多个托盘，无法进行货位调整！");
                }

                if (drArrLocOutContainer.Length <= 0)
                {
                    throw new WmsException(500, $"移出库位{locOutNo}上没有托盘，无法进行货位调整！");
                }
                string container_barcode = drArrLocOutContainer[0]["container_barcode"].ToString();//移出货位上的托盘条码
                if (!string.Equals(container_barcode, taskContainerBarcode))
                {
                    throw new WmsException(500, $"移出库位{locOutNo}上的托盘条码与调整单中的托盘条码不一致，无法进行货位调整！");
                }
                if (IsLocHaveContainer(location_in_id) == true)
                {
                    throw new WmsException(500, $"移入库位{locInNo}上有托盘,无法进行货位调整！");
                }
                string outLocBomId = drArrLocOutContainer[0]["bom_id"].ToString();//移出货位上的BomId，盘型
                DataTable dtInLineNotEmptyLocs = ce.GetNotEmptyLocInfo(locInLine, isDesc);
                if (dtInLineNotEmptyLocs.Rows.Count > 0)
                {
                    string firstLocId = dtInLineNotEmptyLocs.Rows[0]["t_id"].ToString();//待移入的行中首个非空货位id
                    string firstLocStatus = dtInLineNotEmptyLocs.Rows[0]["location_status"].ToString();//待移入的行中首个非空货位状态
                    string inLineBomId = string.Empty;//移入货位所在行的BomId，盘型
                    if (string.Equals(firstLocStatus, LocationStatus.LocationStatus_In.ToString()))
                    {
                        DataRow _tempTaskInfo = GetTaskByDestAddrID(firstLocId);
                        string _tempContaierId = _tempTaskInfo["container_id"].ToString();
                        DataRow _drTempContainer = Container.GetContainerH(_tempContaierId);//查询容器信息
                        inLineBomId = _drTempContainer["bom_id"].ToString();
                    }
                    else if (string.Equals(firstLocStatus, LocationStatus.LocationStatus_Stored.ToString()))
                    {
                        DataTable dtFirstLoc = GetLocContainerInfo(firstLocId);
                        inLineBomId = dtFirstLoc.Rows[0]["bom_id"].ToString();//移入货位所在行的BomId，盘型
                    }
                    else
                    {
                        throw new WmsException(500, $"获取移入库位{locInNo}上的托盘对应的盘型，出现异常,请检查该行货位状态是否为已存储或者待入状态！");
                    }
                    string firstLocNo = dtInLineNotEmptyLocs.Rows[0]["location_no"].ToString();//待移入的行中首个非空货位编号


                    if (!string.IsNullOrEmpty(inLineBomId) && !string.Equals(outLocBomId, inLineBomId))
                    {
                        //throw new WmsException(500, $"移出库位{locOutNo}上的托盘对应的盘型，与移入货位所在的行{locInLine}上的盘型不一致(其中一个非空货位编码为{firstLocNo}),无法进行货位调整！");
                        throw new WmsException(500, $"移出库位{locOutNo}上的托盘对应的盘型，与移入货位所在的行上的盘型不一致,无法进行货位调整！");
                    }
                }
                //查询容器明细信息
                DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_htid);
                //更新调整单明细状态
                UpdatelocJustDByTid(adjustDTid, AdjustState.AdjustState_Executing.ToString());
                //更新移出货位状态为待出
                ce.UpdateLocStatusByLocationId(location_out_id, LocationStatus.LocationStatus_Out.ToString(), LocationStatus.LocationStatus_Stored.ToString(), "");
                //更新移入货位状态为待入
                ce.UpdateLocStatusByLocationId(location_in_id, LocationStatus.LocationStatus_In.ToString(), LocationStatus.LocationStatus_Empty.ToString(), "");

                #region 插入任务主档
                Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    ZoneID = zone_id,//区域ID
                    TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                    DependentTaskNo = "",//依赖任务号
                    TaskType = TaskType.TaskType_Move.ToString(),//移库任务
                    ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                    //ExecutedBy = "",//任务执行人
                    EquipmentID = equipment_id,//执行设备ID
                    //EquipmentType = "",//执行设备类型
                    //ActualEquipmentID = "",//实际执行设备ID
                    SeqNo = 1,//顺序号
                    Priority = 1,//优先级
                    ContainerID = container_htid,//容器ID
                    ContainerBarcode = container_barcode,//容器条码
                    RefDocType = DocType.DocType_Adjustment.ToString(),//来源单据类型
                    RefDocID = model.LocationadJustHTid,//来源单ID
                    RefDocDetailID = adjustDTid,//来源单明细ID,此处存放调整单明细id
                    RefDocNo = adjust_no,//来源单号
                    SrcAddrID = location_out_id,//来源地址ID
                    SrcAddrType = AddressType.AddressType_Location.ToString(),//来源地址类型 "location"
                    SrcAddrText = locOutNo,//来源地址
                    DestAddrID = location_in_id,//目标地址ID
                    DestAddrType = AddressType.AddressType_Location.ToString(),//目标地址类型"location"
                    DestAddrText = locInNo,//目标地址
                    //ActualDestAddrID = "",//实际目标地址ID
                    //ActualDestAddrText = "",//实际目标地址
                    TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                    StartDate = dateTime,//开始时间
                    //EndDate = dateTime,//结束时间
                    IsExecutable = "1",//任务是否可执行
                    //PriorityChangeTime = dateTime,//优先级修改时间        
                    LedText = "",//LED显示内容，此字段有其他用途
                    ExistLgv = "0",//是否存在LGV
                    //CreateDate = dateTime,//创建时间
                    //CreatedBy = "",//创建人帐号
                    //ModifyIP = "",//操作人IP 
                    //ModifyDate = dateTime,//编辑时间
                    //LastModifiedBy = "",//最后修改人帐号
                    ActStatus = "1"//状态
                };
                WorkTask.Insert(taskModel);
                #endregion

                for (int k = 0; k < dtrContainerDInfo.Length; k++)
                {
                    #region 插入任务物料信息
                    WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                    {
                        ID = SnowflakeID.Guid().ToString(),//主键ID
                        //ClientID = "",//集团ID
                        //OrgID = "",//组织ID
                        //WarehouseID = "",//仓库ID
                        TaskID = taskModel.ID,//主任务ID
                        ItemID = dtrContainerDInfo[k]["item_id"].ToString(),//物料ID
                        ItemName = dtrContainerDInfo[k]["item_name"].ToString(),//物料名称
                        PlanQty = Convert.ToDecimal(dtrContainerDInfo[k]["qty"].ToString()),//计划数量
                        //ProcessedQty = "",//已处理数量
                        //FinishedQty = "",//完成数量
                        UomQtyID = dtrContainerDInfo[k]["uom_qty_id"].ToString(),//数量单位
                        //FinishedStatQty = "",//完成统计数量
                        UomStatID = dtrContainerDInfo[k]["uom_stat_id"].ToString(),//统计数量单位
                        ContainerDetailID = dtrContainerDInfo[k]["t_id"].ToString(),//容器明细ID
                        RefDocType = DocType.DocType_Adjustment.ToString(),//来源单据类型  
                        RefDocID = model.LocationadJustHTid,//来源单ID
                        //RefDocDetailID = "",//来源单明细ID
                        RefDocNo = adjust_no,//来源单号
                        //StartDate = "",//开始时间
                        //EndDate = "",//结束时间
                        ProcessStatus = ProcessStatus.ProcessStatus_New.ToString()//工作状态
                        //CreateDate = "",//创建时间
                        //CreatedBy = "",//创建人帐号 
                        //ModifyIP = "",//操作人IP 
                        //ModifyDate = "",//编辑时间
                        //LastModifiedBy = "",//最后修改人帐号
                        //ActStatus = ""//状态
                    };
                    WorkTaskItem.Insert(taskItemModel);
                    #endregion
                }
                tasklist.Add(taskModel.ID);
            }
            WorkTaskInterface.Send(tasklist);
        }
        #endregion

        #region 创建整行库位调整明细
        public void CreateLocationadJustDetail(LocationAdjustModel.CreateLocationadJustDetail model)
        {
            DateTime dateTime = DateTime.Now;
            string locationadJustHTid = SnowflakeID.Guid().ToString();
            string equipment_id = model.Equipment_id;//执行设备id
            string zone_id = model.Zone_id;//区域id
            string adjust_no = model.Adjust_no;//单号
            string outline = model.Outline;//移出行
            string inline = model.Inline;//移入行
            if (IsLocHaveTask(outline))
            {
                throw new WmsException(500, $"移出行{outline}存在除库位调整以外的任务，请等待任务完成后再执行调整！");
            }

            if (IsLocHaveTask(inline))
            {
                throw new WmsException(500, $"移入行{inline}存在除库位调整以外的任务，请等待任务完成后再执行调整！");
            }

            DataTable dtLocJustDInfo = new DataTable();
            string sc01 = WmsConfig.GetOrDefault("Task:SC01", "1288091969666551808");//1号堆垛机t_id
            string sc02 = WmsConfig.GetOrDefault("Task:SC02", "1288092061295316992");//2号堆垛机t_id
            string msg = string.Empty;
            bool isDesc;
            if (string.Equals(equipment_id, sc01))
            {
                msg = "由1号堆垛机执行的";
                isDesc = false;
                //dtLocJustDInfo = GetLocJustDInfo(model.LocationadJustHTid, false);
            }
            else if (string.Equals(equipment_id, sc02))
            {
                msg = "由2号堆垛机执行的";
                isDesc = true;
                //dtLocJustDInfo = GetLocJustDInfo(model.LocationadJustHTid, true);
            }
            else
            {//二楼
                isDesc = true;
                //dtLocJustDInfo = GetLocJustDInfo(model.LocationadJustHTid, true);
            }

            DataTable storedLocInOutline = ce.GetStoredLocByLine(outline, isDesc);//移出行货位信息
            string _locOutlineZone = storedLocInOutline.Rows[0]["zone_id"].ToString();//移出货位所在区域
            DataTable containerByStoredLocInOutline = GetLocContainerInfo(storedLocInOutline.Rows[0]["t_id"].ToString());//移出行首个已存储货位对应的容器主档信息
            string outLocBomId = containerByStoredLocInOutline.Rows[0]["bom_id"].ToString();//移出行BomId
            DataTable allLocInInline = ce.GetAllLocByLine(inline, isDesc);//移入行所有货位信息
            string _locInlineZone = allLocInInline.Rows[0]["zone_id"].ToString();//移入货位所在区域
            if (!string.Equals(_locOutlineZone, zone_id))
            {
                throw new WmsException(500, $"移出行{outline}所在区域，与选择的区域不一致,无法进行货位调整！");
            }
            if (!string.Equals(_locInlineZone, zone_id))
            {
                throw new WmsException(500, $"移入行{inline}所在区域，与选择的区域不一致,无法进行货位调整！");
            }
            DataTable dtInLineNotEmptyLocs = ce.GetNotEmptyLocInfo(inline, isDesc);
            if (dtInLineNotEmptyLocs.Rows.Count > 0)
            {
                string firstLocId = dtInLineNotEmptyLocs.Rows[0]["t_id"].ToString();//待移入的行中首个非空货位id
                string firstLocStatus = dtInLineNotEmptyLocs.Rows[0]["location_status"].ToString();//待移入的行中首个非空货位状态
                string inLineBomId = string.Empty;//移入货位所在行的BomId，盘型
                if (string.Equals(firstLocStatus, LocationStatus.LocationStatus_In.ToString()))
                {
                    DataRow _tempTaskInfo = GetTaskByDestAddrID(firstLocId);
                    string _tempContaierId = _tempTaskInfo["container_id"].ToString();
                    DataRow _drTempContainer = Container.GetContainerH(_tempContaierId);//查询容器信息
                    inLineBomId = _drTempContainer["bom_id"].ToString();
                }
                else if (string.Equals(firstLocStatus, LocationStatus.LocationStatus_Stored.ToString()))
                {
                    DataTable dtFirstLoc = GetLocContainerInfo(firstLocId);
                    inLineBomId = dtFirstLoc.Rows[0]["bom_id"].ToString();//移入货位所在行的BomId，盘型
                }
                else
                {
                    throw new WmsException(500, $"获取移入行{inline}上的盘型，出现异常,请检查该行货位状态是否为已存储状态！");
                }
                string firstLocNo = dtInLineNotEmptyLocs.Rows[0]["location_no"].ToString();//待移入的行中首个非空货位编号


                if (!string.IsNullOrEmpty(inLineBomId) && !string.Equals(outLocBomId, inLineBomId))
                {
                    //throw new WmsException(500, $"移出库位{locOutNo}上的托盘对应的盘型，与移入货位所在的行{locInLine}上的盘型不一致(其中一个非空货位编码为{firstLocNo}),无法进行货位调整！");
                    throw new WmsException(500, $"移出行{outline}上的盘型，与移入行的盘型不一致,无法进行货位调整！");
                }
            }
            int lastEmptylocIndex = -1;
            for (int i = 0; i < allLocInInline.Rows.Count; i++)
            {
                string _locationStatus = allLocInInline.Rows[i]["location_status"].ToString();
                if (!string.Equals(_locationStatus, LocationStatus.LocationStatus_Stored.ToString()))
                {
                    lastEmptylocIndex++;
                }
                else
                {
                    break;
                }
            }
            //if (lastEmptylocIndex==-1)
            //{
            //    throw new WmsException(500, $"移入行{inline}中不存在{msg}可用空货位,无法进行货位调整！");
            //}
            if ((lastEmptylocIndex + 1) < storedLocInOutline.Rows.Count)
            {
                throw new WmsException(500, $"移入行{inline}中{msg}可用空货位数量不满足库位调整需要,无法进行货位调整！");
            }
            DateTime _createDt = DateTime.Now;

            Dictionary<string, object> insertHeadParam = new Dictionary<string, object>
                {
                    { "t_id", locationadJustHTid },
                    { "zone_id", zone_id },
                    { "adjust_no", adjust_no },
                    { "adjust_reason", model.Adjust_reason },
                    { "adjust_state", AdjustState.AdjustState_NotExecute.ToString() },
                    { "outline", model.Outline },
                    { "inline", model.Inline },
                    { "memo", model.Memo },
                    { "equipment_id", equipment_id },
                    { "create_date", _createDt.ToString() },
                    { "created_by", Context.UserInfo.UserName },
                    { "modify_ip", Context.UserInfo.LoginIP },
                    { "act_status", "1" }
                };
            string insertHeadSql = $" INSERT INTO &DataSource&WMS_B_LOCATIONADJUST_H (CLIENT_ID, ORG_ID, t_id, warehouse_id, zone_id, adjust_no, adjust_reason, adjust_state, outline, inline, ISALLLINEOUT, memo, EQUIPMENT_ID, CREATED_BY, MODIFY_IP, CREATE_DATE, ACT_STATUS) VALUES ('&CLIENT_ID&', '&ORG_ID&', @t_id, '&WAREHOUSE_ID&', @zone_id, @adjust_no, @adjust_reason, @adjust_state, @outline, @inline, '1', @memo, @equipment_id, @created_by, @modify_ip, date_format(@create_date, '%Y-%m-%d %H:%i:%s'), @act_status)";

            DBHelper.RunSql(insertHeadSql, insertHeadParam, "创建整行库位调整主档");

            for (int i = 0; i < storedLocInOutline.Rows.Count; i++)
            {
                string _tempInLoc = allLocInInline.Rows[lastEmptylocIndex]["t_id"].ToString();//待入货位
                string _tempOutLoc = storedLocInOutline.Rows[i]["t_id"].ToString();//待出货位                
                DataTable _tempContainerOutloc = GetLocContainerInfo(_tempOutLoc);//移出货位对应的容器主档信息
                Dictionary<string, object> param = new Dictionary<string, object>
                {
                    { "t_id", SnowflakeID.Guid().ToString() },
                    { "adjust_hid", locationadJustHTid },
                    { "location_in_id", _tempInLoc },
                    { "location_out_id", _tempOutLoc },
                    { "out_container_id", _tempContainerOutloc.Rows[0]["containerTid"].ToString() },
                    { "adjust_detail_state", AdjustState.AdjustState_NotExecute.ToString() },
                    { "create_date", _createDt.ToString() },
                    { "created_by", Context.UserInfo.UserName },
                    { "modify_ip", Context.UserInfo.LoginIP },
                    { "act_status", "1" }
                };
                string sql =
                    "insert into &DataSource&wms_b_locationadjust_d ( t_id, org_id, client_id, warehouse_id, adjust_hid, location_in_id, location_out_id, create_date, created_by, modify_ip, act_status, out_container_id, adjust_detail_state) "
                    +
                    "values (  @t_id,  '&CLIENT_ID&',   '&ORG_ID&',  '&WAREHOUSE_ID&', @adjust_hid,  @location_in_id, @location_out_id, date_format(@create_date, '%Y-%m-%d %H:%i:%s'), @created_by, @modify_ip, @act_status, @out_container_id, @adjust_detail_state) ";

                DBHelper.RunSql(sql, param, "创建整行库位调整明细");

                lastEmptylocIndex--;
            }
        }
        #endregion

        #region 编辑整行库位调整主档
        public void EditLocationadJustHead(LocationAdjustModel.EditLocationadJustHead model)
        {
            DateTime dateTime = DateTime.Now;
            //调整单主档信息
            DataRow drLocJustHInfo = GetLocJustHInfo(model.LocationadJustHTid);
            string adjust_state = drLocJustHInfo["adjust_state"].ToString();//单据状态
            string equipment_id = drLocJustHInfo["equipment_id"].ToString();//执行设备id
            string zone_id = drLocJustHInfo["zone_id"].ToString();//区域id
            string adjust_no = drLocJustHInfo["adjust_no"].ToString();//单号
            string isalllineout = drLocJustHInfo["isalllineout"].ToString();//整行移出标志
            string outline = drLocJustHInfo["outline"].ToString().Trim();//移出行
            string inline = drLocJustHInfo["inline"].ToString().Trim();//移入行
            if (adjust_state != AdjustState.AdjustState_NotExecute.ToString())
            {
                throw new WmsException(500, "库位调整仅在单据状态为未执行状态下，才能进行编辑！");
            }
            if (!string.Equals(isalllineout, "1"))
            {
                throw new WmsException(500, "该库位调整单据为整行库位调整单据，请走库位调整流程！");
            }
            if (string.IsNullOrEmpty(outline) || string.IsNullOrEmpty(inline))
            {
                throw new WmsException(500, "移入行或移出行号，未填写！");
            }
            //修改调整单主档
            UpdateReasonAndMemolocJustHByTid(model.LocationadJustHTid, model.Adjust_reason.Trim(), model.Memo.Trim(), model.Zone_id, model.Equipment_id, model.Outline, model.Inline);
            if (string.Equals(zone_id, model.Zone_id) && string.Equals(equipment_id, model.Equipment_id) && string.Equals(outline, model.Outline) && string.Equals(inline, model.Inline))
            {
                return;
            }
            //软删除调整明细
            UpdateActStatusJustDByTid(model.LocationadJustHTid, "0");
            if (IsLocHaveTask(model.Outline))
            {
                throw new WmsException(500, $"移出行{outline}存在除库位调整以外的任务，请等待任务完成后再执行调整！");
            }

            if (IsLocHaveTask(model.Inline))
            {
                throw new WmsException(500, $"移入行{inline}存在除库位调整以外的任务，请等待任务完成后再执行调整！");
            }
            DataTable dtLocJustDInfo = new DataTable();
            string sc01 = WmsConfig.GetOrDefault("Task:SC01", "1288091969666551808");//1号堆垛机t_id
            string sc02 = WmsConfig.GetOrDefault("Task:SC02", "1288092061295316992");//2号堆垛机t_id
            string msg = string.Empty;
            bool isDesc;
            if (string.Equals(equipment_id, sc01))
            {
                msg = "由1号堆垛机执行的";
                isDesc = false;
                //dtLocJustDInfo = GetLocJustDInfo(model.LocationadJustHTid, false);
            }
            else if (string.Equals(equipment_id, sc02))
            {
                msg = "由2号堆垛机执行的";
                isDesc = true;
                //dtLocJustDInfo = GetLocJustDInfo(model.LocationadJustHTid, true);
            }
            else
            {//二楼
                isDesc = true;
                //dtLocJustDInfo = GetLocJustDInfo(model.LocationadJustHTid, true);
            }
            DataTable storedLocInOutline = ce.GetStoredLocByLine(model.Outline, isDesc);//移出行货位信息
            DataTable containerByStoredLocInOutline = GetLocContainerInfo(storedLocInOutline.Rows[0]["t_id"].ToString());//移出行首个已存储货位对应的容器主档信息
            string outLocBomId = containerByStoredLocInOutline.Rows[0]["bom_id"].ToString();//移出行BomId
            DataTable allLocInInline = ce.GetAllLocByLine(model.Inline, isDesc);//移入行所有货位信息
            DataTable dtInLineNotEmptyLocs = ce.GetNotEmptyLocInfo(model.Inline, isDesc);
            if (dtInLineNotEmptyLocs.Rows.Count > 0)
            {
                string firstLocId = dtInLineNotEmptyLocs.Rows[0]["t_id"].ToString();//待移入的行中首个非空货位id
                string firstLocStatus = dtInLineNotEmptyLocs.Rows[0]["location_status"].ToString();//待移入的行中首个非空货位状态
                string inLineBomId = string.Empty;//移入货位所在行的BomId，盘型
                if (string.Equals(firstLocStatus, LocationStatus.LocationStatus_In.ToString()))
                {
                    DataRow _tempTaskInfo = GetTaskByDestAddrID(firstLocId);
                    string _tempContaierId = _tempTaskInfo["container_id"].ToString();
                    DataRow _drTempContainer = Container.GetContainerH(_tempContaierId);//查询容器信息
                    inLineBomId = _drTempContainer["bom_id"].ToString();
                }
                else if (string.Equals(firstLocStatus, LocationStatus.LocationStatus_Stored.ToString()))
                {
                    DataTable dtFirstLoc = GetLocContainerInfo(firstLocId);
                    inLineBomId = dtFirstLoc.Rows[0]["bom_id"].ToString();//移入货位所在行的BomId，盘型
                }
                else
                {
                    throw new WmsException(500, $"获取移入行{model.Inline}上的盘型，出现异常,请检查该行货位状态是否为已存储状态！");
                }
                string firstLocNo = dtInLineNotEmptyLocs.Rows[0]["location_no"].ToString();//待移入的行中首个非空货位编号


                if (!string.IsNullOrEmpty(inLineBomId) && !string.Equals(outLocBomId, inLineBomId))
                {
                    //throw new WmsException(500, $"移出库位{locOutNo}上的托盘对应的盘型，与移入货位所在的行{locInLine}上的盘型不一致(其中一个非空货位编码为{firstLocNo}),无法进行货位调整！");
                    throw new WmsException(500, $"移出行{outline}上的盘型，与移入行的盘型不一致,无法进行货位调整！");
                }
            }

            int lastEmptylocIndex = -1;
            for (int i = 0; i < allLocInInline.Rows.Count; i++)
            {
                string _locationStatus = allLocInInline.Rows[i]["location_status"].ToString();
                if (!string.Equals(_locationStatus, LocationStatus.LocationStatus_Stored.ToString()))
                {
                    lastEmptylocIndex++;
                }
                else
                {
                    break;
                }
            }
            //if (lastEmptylocIndex==-1)
            //{
            //    throw new WmsException(500, $"移入行{inline}中不存在{msg}可用空货位,无法进行货位调整！");
            //}
            if ((lastEmptylocIndex + 1) < storedLocInOutline.Rows.Count)
            {
                throw new WmsException(500, $"移入行{model.Inline}中{msg}可用空货位数量不满足库位调整需要,无法进行货位调整！");
            }
            DateTime _createDt = DateTime.Now;
            for (int i = 0; i < storedLocInOutline.Rows.Count; i++)
            {
                string _tempInLoc = allLocInInline.Rows[lastEmptylocIndex]["t_id"].ToString();//待入货位
                string _tempOutLoc = storedLocInOutline.Rows[i]["t_id"].ToString();//待出货位
                lastEmptylocIndex--;
                DataTable _tempContainerOutloc = GetLocContainerInfo(_tempOutLoc);//移出货位对应的容器主档信息
                Dictionary<string, object> param = new Dictionary<string, object>
                {
                    { "t_id", SnowflakeID.Guid().ToString() },
                    { "adjust_hid", model.LocationadJustHTid },
                    { "location_in_id", _tempInLoc },
                    { "location_out_id", _tempOutLoc },
                    { "out_container_id", _tempContainerOutloc.Rows[0]["containerTid"].ToString() },
                    { "adjust_detail_state", AdjustState.AdjustState_NotExecute.ToString() },
                    { "create_date", _createDt.ToString() },
                    { "created_by", Context.UserInfo.UserName },
                    { "modify_ip", Context.UserInfo.LoginIP },
                    { "act_status", "1" }
                };
                string sql =
                    "insert into &DataSource&wms_b_locationadjust_d ( t_id, org_id, client_id, warehouse_id, adjust_hid, location_in_id, location_out_id, create_date, created_by, modify_ip, act_status, out_container_id, adjust_detail_state) "
                    +
                    "values (  @t_id,  '&CLIENT_ID&',   '&ORG_ID&',  '&WAREHOUSE_ID&', @adjust_hid,  @location_in_id, @location_out_id, date_format(@create_date, '%Y-%m-%d %H:%i:%s'), @created_by, @modify_ip, @act_status, @out_container_id, @adjust_detail_state) ";

                DBHelper.RunSql(sql, param, "创建整行库位调整明细");
            }

        }
        #endregion

        #region 库位调整任务完成
        /// <summary>
        /// 库位调整任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void FinishLocationadJust(LocationAdjustModel.FinishLocationadJustModel model)
        {
            #region 1.获取库位调整任务信息
            //获取库位调整任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到库位调整任务！");
            }
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string location_out_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是移出货位id
            string location_in_id = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的地址id，此处指的是移入货位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            string adjustDTid = dtTaskInfo.Rows[0]["ref_doc_d_id"].ToString();//任务中，调整单明细id
            string locationadJustHTid = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//任务中，调整单主档id
            if (refDocType != DocType.DocType_Adjustment.ToString())
            {
                throw new WmsException(500, "该任务不是库位调整任务，无法完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "任务仅在处于已部分完成状态下，才能完成！");
            }
            #endregion
            #region 修改调整单明细状态为已完成
            UpdatelocJustDByTid(adjustDTid, AdjustState.AdjustState_Finished.ToString());
            #endregion
            //调整单明细的状态如果仅存在完成和取消，则将主档的状态改为完成
            if (IsHaveExecutingAdJustDByAdjustHTid(locationadJustHTid) == false)
            {
                UpdatelocJustHByTid(locationadJustHTid, AdjustState.AdjustState_Finished.ToString());
            }

            #region 5.更新任务状态，并将任务写入历史表
            //将任务更新为完成
            WorkTask.UpdateTaskComplete(model.TaskTid);
            WorkTask.MoveToHistory(model.TaskTid);
            #endregion
        }
        #endregion

        #region 库位调整任务部分完成
        /// <summary>
        /// 库位调整任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void PartionFinishLocationadJustTask(LocationAdjustModel.PartionFinishLocationadJustTaskModel model)
        {
            #region 1.获取库位调整任务信息
            //获取库位调整任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到库位调整任务！");
            }
            string zone_id = dtTaskInfo.Rows[0]["zone_id"].ToString();//区域
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string location_out_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是移出货位id
            string location_in_id = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的地址id，此处指的是移入货位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            string adjustDTid = dtTaskInfo.Rows[0]["ref_doc_d_id"].ToString();//任务中，调整单明细id
            if (refDocType != DocType.DocType_Adjustment.ToString())
            {
                throw new WmsException(500, "该任务不是库位调整任务，无法强制完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中状态下，才能部分完成！");
            }
            #endregion

            #region 2.将任务修改为部分完成
            WorkTask.UpdateTaskStatus(model.TaskTid, TaskStatus.TaskStatus_PartialFinished.ToString(), TaskStatus.TaskStatus_Executing.ToString());
            #endregion

            #region 3.修改移出货位状态，并将容器挂在移入货位上           
            //修改移出货位状态为空
            ce.UpdateLocStatusByLocationId(location_out_id, LocationStatus.LocationStatus_Empty.ToString(), LocationStatus.LocationStatus_Out.ToString(), "");
            //修改容器主档中货位字段数据为移入货位id
            Container.UpdateContainerLocation(container_id, location_in_id);
            //修改移入货位状态为空
            ce.UpdateLocStatusByLocationId(location_in_id, LocationStatus.LocationStatus_Stored.ToString(), LocationStatus.LocationStatus_In.ToString(), "");
            #endregion

            string agvZoneId = WmsConfig.GetOrDefault("Inbound:InboundZone", "1293844199577358336");
            if (string.Equals(agvZoneId, zone_id))//二楼，需要先判断行是否已经空了，如果空了，则把货位标识，修改成可用，用于允许下次入库
            {
                LocationModel locOutInfo = Location.GetLocationById(location_out_id);//移出货位信息
                string row = locOutInfo.location_row.ToString();
                bool isHavePlanLoc = ce.GetPlanLocNumByLine(row);
                if (isHavePlanLoc == false)
                {
                    ce.UpdatelocationflagByRow(row, LocationFlag.LocationFlag_Usable.ToString());
                }
            }
        }
        #endregion

        #region 库位调整任务强制完成
        /// <summary>
        /// 库位调整任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void ForceFinishLocationadJustTask(LocationAdjustModel.ForceFinishLocationadJustTaskModel model)
        {
            #region 1.获取库位调整任务信息
            //获取库位调整任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到库位调整任务！");
            }
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string location_out_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是移出货位id
            string location_in_id = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的地址id，此处指的是移入货位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            string adjustDTid = dtTaskInfo.Rows[0]["ref_doc_d_id"].ToString();//任务中，调整单明细id
            if (refDocType != DocType.DocType_Adjustment.ToString())
            {
                throw new WmsException(500, "该任务不是库位调整任务，无法强制完成！");
            }

            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "任务已完成，无需再次完成！");
            }
            #endregion

            //任务不是部分完成，则执行部分完成
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                LocationAdjustModel.PartionFinishLocationadJustTaskModel pft = new LocationAdjustModel.PartionFinishLocationadJustTaskModel();
                pft.TaskTid = model.TaskTid;
                PartionFinishLocationadJustTask(pft);
            }

            //执行完成任务
            LocationAdjustModel.FinishLocationadJustModel ft = new LocationAdjustModel.FinishLocationadJustModel();
            ft.TaskTid = model.TaskTid;
            FinishLocationadJust(ft);
        }
        #endregion

        #region 库位调整任务回收
        /// <summary>
        /// 库位调整任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void RecycleLocationadJustTask(LocationAdjustModel.RecycleLocationadJustTaskModel model)
        {
            #region 1.获取库位调整任务信息
            //获取库位调整任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到库位调整任务！");
            }
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string location_out_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是移出货位id
            string location_in_id = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的地址id，此处指的是移入货位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            string adjustDTid = dtTaskInfo.Rows[0]["ref_doc_d_id"].ToString();//任务中，调整单明细id
            string locationadJustHTid = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//任务中，调整单主档id
            if (refDocType != DocType.DocType_Adjustment.ToString())
            {
                throw new WmsException(500, "该任务不是库位调整任务，无法完成！");
            }
            if (taskStatus == TaskStatus.TaskStatus_PartialFinished.ToString() || taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "任务处于已部分完成或完成状态下，不能回收！");
            }
            #endregion           

            //调整单主档信息
            DataRow drLocJustHInfo = GetLocJustHInfo(locationadJustHTid);
            string adjust_state = drLocJustHInfo["adjust_state"].ToString();//单据状态
            string equipment_id = drLocJustHInfo["equipment_id"].ToString();//执行设备id
            string zone_id = drLocJustHInfo["zone_id"].ToString();//区域id
            string adjust_no = drLocJustHInfo["adjust_no"].ToString();//单号
            //调整单明细信息
            DataTable dtLocJustDInfo = new DataTable();
            string sc01 = WmsConfig.GetOrDefault("Task:SC01", "1288091969666551808");//1号堆垛机t_id
            string sc02 = WmsConfig.GetOrDefault("Task:SC02", "1288092061295316992");//2号堆垛机t_id
            if (string.Equals(equipment_id, sc01))
            {
                dtLocJustDInfo = GetLocJustDInfo(locationadJustHTid, false);
            }
            else if (string.Equals(equipment_id, sc02))
            {
                dtLocJustDInfo = GetLocJustDInfo(locationadJustHTid, true);
            }
            else
            {//二楼
                dtLocJustDInfo = GetLocJustDInfo(locationadJustHTid, true);
            }
            int _index = 0;
            for (int i = 0; i < dtLocJustDInfo.Rows.Count; i++)
            {
                if (string.Equals(adjustDTid, dtLocJustDInfo.Rows[i]["t_id"].ToString()))
                {
                    _index = i;
                    break;
                }
            }
            //修改任务软删除标记为“0”
            WorkTask.UpdateActStatus(model.TaskTid, "0");
            //更新调整单明细状态
            UpdatelocJustDByTid(adjustDTid, AdjustState.AdjustState_Cancel.ToString());
            //更新移出货位状态为已存储
            ce.UpdateLocStatusByLocationId(location_out_id, LocationStatus.LocationStatus_Stored.ToString(), LocationStatus.LocationStatus_Out.ToString(), "");
            //更新移入货位状态为空
            ce.UpdateLocStatusByLocationId(location_in_id, LocationStatus.LocationStatus_Empty.ToString(), LocationStatus.LocationStatus_In.ToString(), "");

            if (_index + 1 < dtLocJustDInfo.Rows.Count)
            {
                DataRow nextTask = GetTaskByRefDocDetailID(dtLocJustDInfo.Rows[_index + 1]["t_id"].ToString());
                WorkTask.RecycleTask(nextTask);
            }
            else
            {
                //调整单明细的状态如果仅存在完成和取消，则将主档的状态改为完成
                if (IsHaveExecutingAdJustDByAdjustHTid(locationadJustHTid) == false)
                {
                    UpdatelocJustHByTid(locationadJustHTid, AdjustState.AdjustState_Finished.ToString());
                }
            }
        }
        #endregion

        #region 自动库位调整
        /// <summary>
        /// 自动库位调整
        /// </summary>
        public void AutoLocationadJust(LocationAdjustModel.AutoLocationadJustModel model)
        {
            //自动库位调整
            //1.判断区域，一楼和二楼分开
            //2.获取库内存放的所有盘型，以及保存该盘型的行的数量
            //3.获取行的数量大于等于2，盘型的信息（包含区域、行号、每行已存储托盘数量）按照托盘数量升序排序
            //i)如果是一楼，判断空闲的堆垛机（没有空闲堆垛机，则返回），将托盘数量最小和最大的行号，分别作为起始行、目标行
            //  判断起始行的托盘数量，与堆垛机可达的空货位数量比较，如果前者大或者相等，则可以产生任务，否则继续判断下一允许自动库位调整的盘型
            //  (需要区分堆垛机，1号堆垛机与2号堆垛机，执行的行不一样，有些只能由1号堆垛机执行，有些只能由2号堆垛机执行)
            //ii)如果是二楼，将托盘数量最小和最大的行号，分别作为起始行、目标行
            //  判断起始行的托盘数量，与目标行（按照货位编码降序排序）的空货位（第一个已存储货位前面的）数量比较，
            //  如果前者大或者相等，则可以产生任务，否则继续判断下一允许自动库位调整的盘型

            string zoneId = WmsConfig.GetOrDefault("Outbound:FirstFloorZone", "1291999756888969216");//一楼区域
            if (SelectAutoAdjustTaskNumByZoneID(zoneId))
            {
                throw new WmsException(500, "目前有未完成的自动货位调整任务，请等待任务完成后在进行调整！");
            }
            if (string.Equals(zoneId, model.ZoneId))
            {
                ProduceOneFloor(model.ZoneId);
            }
            else
            {
                throw new WmsException(500, "不支持二楼自动货位调整！");
            }
        }

        #region 一楼自动库位调整
        /// <summary>
        /// 一楼自动库位调整
        /// </summary>
        /// <param name="zoneId"></param>
        public void ProduceOneFloor(string zoneId)
        {
            string sc01 = WmsConfig.GetOrDefault("Task:SC01", "1288091969666551808");//1号堆垛机t_id
            string sc02 = WmsConfig.GetOrDefault("Task:SC02", "1288092061295316992");//2号堆垛机t_id
            string oneFirst = WmsConfig.GetOrDefault("Task:AutoAdjustEquipment", "1");//默认1号堆垛机优先执行任务
            string isSpecifiedEquipmentBy = WmsConfig.GetOrDefault("Task:IsSpecifiedEquipmentBy", "1");//是否指定默认堆垛机执行任务
            string performAutoAdjustEquipment = WmsConfig.GetOrDefault("Task:PerformAutoAdjustEquipment", "1288091969666551808");//默认1号堆垛机执行任务
            int autoAdjustLineNum = Convert.ToInt32(WmsConfig.GetOrDefault("Task:AutoAdjustLineNum", "1"));//设置的，自动优化的行的数量
            int adjustMaxNum = Convert.ToInt32(WmsConfig.GetOrDefault("Task:AdjustMaxNum", "5"));//设置的，自动库位调整时起始行最多存放的托盘数量
            string actureEquipmentBy = string.Empty;//执行任务的堆垛机
            bool isDesc = false;
            int finishAutoAdjustNum = 0;
            List<string> tasklist = new List<string>();
            if (string.Equals(isSpecifiedEquipmentBy, "1"))//判断是否存在指定堆垛机执行任务
            {//存在
                if (string.Equals(performAutoAdjustEquipment, sc01))
                {
                    isDesc = false;
                    actureEquipmentBy = sc01;
                }
                else
                {
                    isDesc = true;
                    actureEquipmentBy = sc02;
                }
                List<string> _autoAdjustTask = CreateAutoAdjustTask(zoneId, actureEquipmentBy, isDesc, autoAdjustLineNum, out finishAutoAdjustNum);
                _autoAdjustTask.ForEach(a => tasklist.Add(a));
            }
            else
            {
                //DataTable dtDDJInfo = GetLocJustEquipmentInfo(sc01, sc02);
                //if (dtDDJInfo == null || dtDDJInfo.Rows.Count >= 2)
                //{
                //    throw new WmsException(500, "堆垛机都有正在执行的任务,请等待任务执行结束后在进行库位调整！");
                //}
                //if (dtDDJInfo.Rows.Count == 1)
                //{
                //    string eqTid = dtDDJInfo.Rows[0]["equipment_id"].ToString();//第一个存在任务的堆垛机id
                //    if (string.Equals(eqTid, sc01))
                //    {
                //        isDesc = true;
                //        actureEquipmentBy = sc02;
                //    }
                //    else
                //    {
                //        isDesc = false;
                //        actureEquipmentBy = sc01;
                //    }
                //}
                //else
                //{
                //    if (string.Equals(oneFirst, "1"))
                //    {
                //        isDesc = false;
                //        actureEquipmentBy = sc01;
                //    }
                //    else
                //    {
                //        isDesc = true;
                //        actureEquipmentBy = sc02;
                //    }
                //}
                //List<string> _autoAdjustTask = CreateAutoAdjustTask(zoneId, actureEquipmentBy, isDesc, autoAdjustLineNum, out finishAutoAdjustNum);
                //_autoAdjustTask.ForEach(a => tasklist.Add(a));

                List<string> _autoAdjustTask1 = CreateAutoAdjustTask(zoneId, sc01, false, autoAdjustLineNum, out finishAutoAdjustNum);//1号堆垛机执行
                _autoAdjustTask1.ForEach(a => tasklist.Add(a));
                autoAdjustLineNum = autoAdjustLineNum - finishAutoAdjustNum;//重新修改允许执行自动货位调整的行数的上线
                List<string> _autoAdjustTask2 = CreateAutoAdjustTask(zoneId, sc02, true, autoAdjustLineNum, out finishAutoAdjustNum);//2号堆垛机执行
                _autoAdjustTask2.ForEach(a => tasklist.Add(a));
            }
            if (tasklist.Count > 0)
            {
                WorkTaskInterface.Send(tasklist);
            }
            else
            {
                throw new WmsException(500, "未查询到可以进行库位调整的货位！");
            }
        }
        #endregion

        #region 创建自动调整货位任务
        /// <summary>
        /// 创建自动调整货位任务
        /// </summary>
        /// <param name="zoneId">区域</param>
        /// <param name="sc">执行任务的设备</param>
        /// <param name="isDesc">排序(true:降序;false:升序)</param>
        /// <param name="autoAdjustLineNum">执行自动库位调整的行的上线</param>
        /// <param name="finishAutoAdjustNum">完成库位调整行的数量</param>
        /// <returns></returns>
        public List<string> CreateAutoAdjustTask(string zoneId, string sc, bool isDesc, int autoAdjustLineNum, out int finishAutoAdjustNum)
        {
            finishAutoAdjustNum = 0;
            List<string> tasklist = new List<string>();
            string sc01 = WmsConfig.GetOrDefault("Task:SC01", "1288091969666551808");//1号堆垛机t_id
            string sc02 = WmsConfig.GetOrDefault("Task:SC02", "1288092061295316992");//2号堆垛机t_id

            int adjustMaxNum = Convert.ToInt32(WmsConfig.GetOrDefault("Task:AdjustMaxNum", "5"));//设置的，自动库位调整时起始行最多存放的托盘数量
            string actureEquipmentBy = sc;//执行任务的堆垛机
            DataTable dtBomInfo;//堆垛机允许执行的行的Bom信息
            if (string.Equals(sc01, sc))
            {//1号堆垛机
                dtBomInfo = GetBigZoneBomInfoByZoneId(zoneId);
            }
            else
            {//2号堆垛机
                dtBomInfo = GetBomInfoByZoneId(zoneId);
            }

            DataRow[] oneLineStoredBoms = dtBomInfo.Select("cnt = 1");//只有一行存放同一Bom，此处筛选这样的Bom信息
            List<string> finishAdjustLineOneBomStored = CreateAutoAdjustTaskOnOneBomStored(zoneId, sc, isDesc, oneLineStoredBoms, autoAdjustLineNum, out finishAutoAdjustNum);
            finishAdjustLineOneBomStored.ForEach(a => tasklist.Add(a));
            if (finishAutoAdjustNum >= autoAdjustLineNum)
            {
                return tasklist;
            }
            autoAdjustLineNum = autoAdjustLineNum - finishAutoAdjustNum;//重新修改允许执行自动货位调整的行数的上线

            DataRow[] _temp = dtBomInfo.Select("cnt > 1");//多行存放同一Bom，此处筛选这样的Bom信息
            if (_temp == null || _temp.Length <= 0)
            {
                return tasklist;
            }
            int _tempNum = 0;//记录调整的行数，用于判断是否已超过设定的调整行数的最大值
            for (int i = 0; i < _temp.Length; i++)
            {
                if (_tempNum >= autoAdjustLineNum)
                {
                    break;
                }
                string bomId = _temp[i]["bom_id"].ToString();
                DataTable dtBomLineInfos;
                if (string.Equals(sc01, sc))
                {//1号堆垛机
                    dtBomLineInfos = GetBigZoneBomInfoByZoneIdAndLine(zoneId, bomId);
                    if (dtBomLineInfos.Rows.Count<=0)
                    {
                        continue;
                    }
                }
                else
                {//2号堆垛机
                    dtBomLineInfos = GetBomInfoByZoneIdAndLine(zoneId, bomId);
                    if (dtBomLineInfos.Rows.Count <= 0)
                    {
                        continue;
                    }
                }
                string outLine = dtBomLineInfos.Rows[0]["LOCATION_ROW"].ToString();//移出行号
                int outLineStoredNum = Convert.ToInt32(dtBomLineInfos.Rows[0]["STOREDCNT"].ToString());//移出行的存储托盘数量
                if (adjustMaxNum < outLineStoredNum)
                {
                    continue;
                }
                if (IsLocHaveAutoAdjustmentTask(outLine))//移出行存在任务
                {
                    continue;
                }
                DataTable storedLocInOutline = ce.GetStoredLocByLine(outLine, isDesc);//移出行货位信息

                if (string.Equals(sc01, sc) && (string.Equals(outLine.Substring(1, 2), "04") || string.Equals(outLine.Substring(1, 2), "09") || string.Equals(outLine.Substring(1, 2), "16")))
                {
                    continue;
                }
                if (string.Equals(sc02, sc) && (string.Equals(outLine.Substring(1, 2), "28") || string.Equals(outLine.Substring(1, 2), "29") || string.Equals(outLine.Substring(1, 2), "30")))
                {
                    continue;
                }
                bool isMove = false;//是否可以自动调整库位
                int emptyLocNum = 0;//移入行中，堆垛机可达的空货位数量个数之和
                Dictionary<int, int> indexAndEmptyLocNun = new Dictionary<int, int>();//移入行索引，以及对应的堆垛机可达空货位的最大索引
                for (int z = 1; z < dtBomLineInfos.Rows.Count; z++)
                {
                    string inLine = dtBomLineInfos.Rows[z]["LOCATION_ROW"].ToString();//移入行号
                    if (IsLocHaveAutoAdjustmentTask(inLine))
                    {
                        isMove = false;
                        continue;
                    }
                    DataTable allLocInInline = ce.GetAllLocByLine(inLine, isDesc);//移入行所有货位信息
                    int lastEmptylocIndex = -1;//堆垛机可达空货位最大索引
                    //获取堆垛机可达空货位的最大索引
                    for (int j = 0; j < allLocInInline.Rows.Count; j++)
                    {
                        string _tempLocId = allLocInInline.Rows[0]["t_id"].ToString();
                        if (string.Equals(sc01, sc) && string.Equals(inLine.Substring(1, 2), "28") && j > 2)
                        {
                            break;
                        }
                        if (string.Equals(sc02, sc) && string.Equals(inLine.Substring(1, 2), "04") && j > 2)
                        {
                            break;
                        }
                        if (string.Equals(sc01, sc) && string.Equals(inLine.Substring(1, 2), "29") && j > 2)
                        {
                            break;
                        }
                        if (string.Equals(sc02, sc) && string.Equals(inLine.Substring(1, 2), "09") && j > 2)
                        {
                            break;
                        }
                        if (string.Equals(sc01, sc) && string.Equals(inLine.Substring(1, 2), "30") && j > 2)
                        {
                            break;
                        }
                        if (string.Equals(sc02, sc) && string.Equals(inLine.Substring(1, 2), "16") && j > 2)
                        {
                            break;
                        }
                        string _locationStatus = allLocInInline.Rows[j]["location_status"].ToString();
                        if (!string.Equals(_locationStatus, LocationStatus.LocationStatus_Stored.ToString()))
                        {
                            lastEmptylocIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (string.Equals(inLine.Substring(1, 2), "04") || string.Equals(inLine.Substring(1, 2), "09") || string.Equals(inLine.Substring(1, 2), "16"))
                    {
                        if (lastEmptylocIndex >= 2)
                        {
                            lastEmptylocIndex = 2;
                        }
                    }
                    if (string.Equals(inLine.Substring(1, 2), "28") || string.Equals(inLine.Substring(1, 2), "29") || string.Equals(inLine.Substring(1, 2), "30"))
                    {
                        if (lastEmptylocIndex >= 2)
                        {
                            lastEmptylocIndex = 2;
                        }
                    }
                    emptyLocNum += (lastEmptylocIndex + 1);
                    if (lastEmptylocIndex >= 0)
                    {
                        indexAndEmptyLocNun.Add(z, lastEmptylocIndex);
                    }

                    isMove = true;
                }
                if (isMove == false)
                {
                    continue;
                }
                if (emptyLocNum < outLineStoredNum)
                {//空货位个数小于移出行的托盘个数
                    continue;
                }

                DateTime dateTime = DateTime.Now;
                int moveNum = 0;//移出货位的个数
                foreach (KeyValuePair<int, int> kvp in indexAndEmptyLocNun)
                {
                    if (moveNum >= storedLocInOutline.Rows.Count)
                    {
                        break;
                    }
                    int dtIndex = kvp.Key;
                    int lastEmptylocIndex = kvp.Value;
                    string inLine = dtBomLineInfos.Rows[dtIndex]["LOCATION_ROW"].ToString();//移入行号
                    DataTable allLocInInline = ce.GetAllLocByLine(inLine, isDesc);//移入行所有货位信息
                    for (; moveNum < storedLocInOutline.Rows.Count;)
                    {
                        string location_out_id = storedLocInOutline.Rows[moveNum]["t_id"].ToString();//移出货位id
                        string locationcode = storedLocInOutline.Rows[moveNum]["LOCATION_NO"].ToString();//移出货位编码
                        DataRow[] _tempCon = Container.GetHeadsByLocation(location_out_id);
                        string container_htid = _tempCon[0]["t_id"].ToString();//容器id
                        DataRow drContainer = Container.GetContainerH(container_htid);//查询容器信息                    
                        DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_htid);//查询容器明细信息
                        string containerBarcode = drContainer["container_barcode"].ToString();//容器的托盘条码
                        string location_in_id = allLocInInline.Rows[lastEmptylocIndex]["t_id"].ToString();//待入货位
                        LocationModel locInInfo = Location.GetLocationById(location_in_id);//移入货位信息
                        string locInNo = locInInfo.location_no;//移入货位编码
                        string locInStatus = locInInfo.location_status.ToString();//移入行货位状态

                        #region 更新货位状态，并产生任务
                        //更新移出货位状态为待出
                        ce.UpdateLocStatusByLocationId(location_out_id, LocationStatus.LocationStatus_Out.ToString(), LocationStatus.LocationStatus_Stored.ToString(), "");
                        //更新移入货位状态为待入
                        ce.UpdateLocStatusByLocationId(location_in_id, LocationStatus.LocationStatus_In.ToString(), LocationStatus.LocationStatus_Empty.ToString(), "");

                        #region 插入任务主档
                        Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
                        {
                            ID = SnowflakeID.Guid().ToString(),//主键ID
                            ZoneID = zoneId,//区域ID
                            TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                            DependentTaskNo = "",//依赖任务号
                            TaskType = TaskType.TaskType_Move.ToString(),//移库任务
                            ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                            //ExecutedBy = "",//任务执行人
                            EquipmentID = actureEquipmentBy,//执行设备ID
                            //EquipmentType = "",//执行设备类型
                            //ActualEquipmentID = "",//实际执行设备ID
                            SeqNo = 1,//顺序号
                            Priority = 1,//优先级
                            ContainerID = container_htid,//容器ID
                            ContainerBarcode = containerBarcode,//容器条码
                            RefDocType = DocType.DocType_AutoAdjustment.ToString(),//来源单据类型
                            RefDocID = "",//来源单ID
                            RefDocDetailID = "",//来源单明细ID
                            RefDocNo = outLine,//来源单号
                            SrcAddrID = location_out_id,//来源地址ID
                            SrcAddrType = AddressType.AddressType_Location.ToString(),//来源地址类型 "location"
                            SrcAddrText = locationcode,//来源地址
                            DestAddrID = location_in_id,//目标地址ID
                            DestAddrType = AddressType.AddressType_Location.ToString(),//目标地址类型"location"
                            DestAddrText = locInNo,//目标地址
                            //ActualDestAddrID = "",//实际目标地址ID
                            //ActualDestAddrText = "",//实际目标地址
                            TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                            StartDate = dateTime,//开始时间
                            //EndDate = dateTime,//结束时间
                            IsExecutable = "1",//任务是否可执行
                            //PriorityChangeTime = dateTime,//优先级修改时间        
                            LedText = "",//LED显示内容，此字段有其他用途
                            ExistLgv = "0",//是否存在LGV
                            //CreateDate = dateTime,//创建时间
                            //CreatedBy = "",//创建人帐号
                            //ModifyIP = "",//操作人IP 
                            //ModifyDate = dateTime,//编辑时间
                            //LastModifiedBy = "",//最后修改人帐号
                            ActStatus = "1"//状态
                        };
                        WorkTask.Insert(taskModel);
                        #endregion

                        for (int k = 0; k < dtrContainerDInfo.Length; k++)
                        {
                            #region 插入任务物料信息
                            WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                            {
                                ID = SnowflakeID.Guid().ToString(),//主键ID
                                //ClientID = "",//集团ID
                                //OrgID = "",//组织ID
                                //WarehouseID = "",//仓库ID
                                TaskID = taskModel.ID,//主任务ID
                                ItemID = dtrContainerDInfo[k]["item_id"].ToString(),//物料ID
                                ItemName = dtrContainerDInfo[k]["item_name"].ToString(),//物料名称
                                PlanQty = Convert.ToDecimal(dtrContainerDInfo[k]["qty"].ToString()),//计划数量
                                //ProcessedQty = "",//已处理数量
                                //FinishedQty = "",//完成数量
                                UomQtyID = dtrContainerDInfo[k]["uom_qty_id"].ToString(),//数量单位
                                //FinishedStatQty = "",//完成统计数量
                                UomStatID = dtrContainerDInfo[k]["uom_stat_id"].ToString(),//统计数量单位
                                ContainerDetailID = dtrContainerDInfo[k]["t_id"].ToString(),//容器明细ID
                                RefDocType = DocType.DocType_AutoAdjustment.ToString(),//来源单据类型  
                                RefDocID = "",//来源单ID
                                //RefDocDetailID = "",//来源单明细ID
                                RefDocNo = "",//来源单号
                                //StartDate = "",//开始时间
                                //EndDate = "",//结束时间
                                ProcessStatus = ProcessStatus.ProcessStatus_New.ToString()//工作状态
                                //CreateDate = "",//创建时间
                                //CreatedBy = "",//创建人帐号 
                                //ModifyIP = "",//操作人IP 
                                //ModifyDate = "",//编辑时间
                                //LastModifiedBy = "",//最后修改人帐号
                                //ActStatus = ""//状态
                            };
                            WorkTaskItem.Insert(taskItemModel);
                            #endregion
                        }
                        tasklist.Add(taskModel.ID);
                        #endregion

                        lastEmptylocIndex--;
                        moveNum++;
                        if (lastEmptylocIndex < 0)
                        {
                            break;
                        }
                    }
                }
                _tempNum++;
                finishAutoAdjustNum++;
            }

            return tasklist;
        }
        #endregion


        #region 处理一个盘型仅存在一行的自动货位调整
        /// <summary>
        /// 处理一个盘型仅存在一行的自动货位调整
        /// </summary>
        /// <param name="zoneId">区域Id</param>
        /// <param name="sc">执行任务的设备</param>
        /// <param name="isDesc">排序(true:降序;false:升序)</param>
        /// <param name="oneLineStoredBoms">BomIds</param>
        /// <param name="autoAdjustLineNum">执行自动库位调整的行的上线</param>
        /// <param name="finishAutoAdjustNum">完成库位调整行的数量</param>
        /// <returns></returns>
        public List<string> CreateAutoAdjustTaskOnOneBomStored(string zoneId, string sc, bool isDesc, DataRow[] oneLineStoredBoms, int autoAdjustLineNum, out int finishAutoAdjustNum)
        {
            finishAutoAdjustNum = 0;
            List<string> tasklist = new List<string>();
            string sc01 = WmsConfig.GetOrDefault("Task:SC01", "1288091969666551808");//1号堆垛机t_id
            string sc02 = WmsConfig.GetOrDefault("Task:SC02", "1288092061295316992");//2号堆垛机t_id
            int adjustMaxNum = Convert.ToInt32(WmsConfig.GetOrDefault("Task:AdjustMaxNumOneBomStored", "3"));//设置的，自动库位调整时起始行最多存放的托盘数量
            string actureEquipmentBy = sc;//执行任务的堆垛机
            if (oneLineStoredBoms == null || oneLineStoredBoms.Length <= 0)
            {
                return tasklist;
            }
            int _tempNum = 0;//记录调整的行数，用于判断是否已超过设定的调整行数的最大值
            for (int i = 0; i < oneLineStoredBoms.Length; i++)
            {
                if (_tempNum >= autoAdjustLineNum)
                {
                    break;
                }
                string bomId = oneLineStoredBoms[i]["bom_id"].ToString();
                DataTable dtBomLineInfos;
                DataTable dtBomLineInLeftOrRight;
                if (string.Equals(sc01, sc))
                {//1号堆垛机
                    dtBomLineInfos = GetBigZoneBomInfoByZoneIdAndLine(zoneId, bomId);
                    if (dtBomLineInfos.Rows.Count <= 0)
                    {
                        continue;
                    }
                    dtBomLineInLeftOrRight = GetLeftLineByZoneIdAndBomId(zoneId, bomId);
                }
                else
                {//2号堆垛机
                    dtBomLineInfos = GetBomInfoByZoneIdAndLine(zoneId, bomId);
                    if (dtBomLineInfos.Rows.Count <= 0)
                    {
                        continue;
                    }
                    dtBomLineInLeftOrRight = GetRightLineByZoneIdAndBom(zoneId, bomId);
                }
                if (dtBomLineInLeftOrRight == null || dtBomLineInLeftOrRight.Rows.Count <= 0)
                {
                    continue;
                }
                string outLine = dtBomLineInfos.Rows[0]["LOCATION_ROW"].ToString();//移出行号
                int outLineStoredNum = Convert.ToInt32(dtBomLineInfos.Rows[0]["STOREDCNT"].ToString());//移出行的存储托盘数量
                if (adjustMaxNum < outLineStoredNum)
                {
                    continue;
                }
                if (IsLocHaveAutoAdjustmentTask(outLine))//移出行存在任务
                {
                    continue;
                }
                DataTable storedLocInOutline = ce.GetStoredLocByLine(outLine, isDesc);//移出行货位信息

                if (string.Equals(outLine.Substring(1, 2), "04") || string.Equals(outLine.Substring(1, 2), "09") || string.Equals(outLine.Substring(1, 2), "16") || string.Equals(outLine.Substring(1, 2), "28") || string.Equals(outLine.Substring(1, 2), "29") || string.Equals(outLine.Substring(1, 2), "30"))
                {
                    continue;
                }
                //if (string.Equals(sc01, sc) && (string.Equals(outLine.Substring(1, 2), "04") || string.Equals(outLine.Substring(1, 2), "09") || string.Equals(outLine.Substring(1, 2), "16")))
                //{
                //    continue;
                //}
                //if (string.Equals(sc02, sc) && (string.Equals(outLine.Substring(1, 2), "28") || string.Equals(outLine.Substring(1, 2), "29") || string.Equals(outLine.Substring(1, 2), "30")))
                //{
                //    continue;
                //}
                bool isMove = false;//是否可以自动调整库位
                int emptyLocNum = 0;//移入行中，堆垛机可达的空货位数量个数之和
                Dictionary<int, int> indexAndEmptyLocNun = new Dictionary<int, int>();//移入行索引，以及对应的堆垛机可达空货位的最大索引
                for (int z = 0; z < dtBomLineInLeftOrRight.Rows.Count; z++)
                {
                    string inLine = dtBomLineInLeftOrRight.Rows[z]["LOCATION_ROW"].ToString();//移入行号
                    if (IsLocHaveAutoAdjustmentTask(inLine))
                    {
                        isMove = false;
                        continue;
                    }
                    DataTable allLocInInline = ce.GetAllLocByLine(inLine, isDesc);//移入行所有货位信息
                    int lastEmptylocIndex = -1;//堆垛机可达空货位最大索引
                    //获取堆垛机可达空货位的最大索引
                    for (int j = 0; j < allLocInInline.Rows.Count; j++)
                    {
                        if (string.Equals(sc01, sc) && string.Equals(inLine.Substring(1, 2), "28") && j > 2)
                        {
                            break;
                        }
                        if (string.Equals(sc02, sc) && string.Equals(inLine.Substring(1, 2), "04") && j > 2)
                        {
                            break;
                        }
                        if (string.Equals(sc01, sc) && string.Equals(inLine.Substring(1, 2), "29") && j > 2)
                        {
                            break;
                        }
                        if (string.Equals(sc02, sc) && string.Equals(inLine.Substring(1, 2), "09") && j > 2)
                        {
                            break;
                        }
                        if (string.Equals(sc01, sc) && string.Equals(inLine.Substring(1, 2), "30") && j > 2)
                        {
                            break;
                        }
                        if (string.Equals(sc02, sc) && string.Equals(inLine.Substring(1, 2), "16") && j > 2)
                        {
                            break;
                        }
                        string _locationStatus = allLocInInline.Rows[j]["location_status"].ToString();
                        if (!string.Equals(_locationStatus, LocationStatus.LocationStatus_Stored.ToString()))
                        {
                            lastEmptylocIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (string.Equals(inLine.Substring(1, 2), "04") || string.Equals(inLine.Substring(1, 2), "09") || string.Equals(inLine.Substring(1, 2), "16"))
                    {
                        if (lastEmptylocIndex >= 2)
                        {
                            lastEmptylocIndex = 2;
                        }
                    }
                    if (string.Equals(inLine.Substring(1, 2), "28") || string.Equals(inLine.Substring(1, 2), "29") || string.Equals(inLine.Substring(1, 2), "30"))
                    {
                        if (lastEmptylocIndex >= 2)
                        {
                            lastEmptylocIndex = 2;
                        }
                    }
                    emptyLocNum += (lastEmptylocIndex + 1);
                    if (lastEmptylocIndex >= 0)
                    {
                        indexAndEmptyLocNun.Add(z, lastEmptylocIndex);
                    }

                    isMove = true;
                }
                if (isMove == false)
                {
                    continue;
                }
                if (emptyLocNum < outLineStoredNum)
                {//空货位个数小于移出行的托盘个数
                    continue;
                }

                DateTime dateTime = DateTime.Now;
                int moveNum = 0;//移出货位的个数
                foreach (KeyValuePair<int, int> kvp in indexAndEmptyLocNun)
                {
                    if (moveNum >= storedLocInOutline.Rows.Count)
                    {
                        break;
                    }
                    int dtIndex = kvp.Key;
                    int lastEmptylocIndex = kvp.Value;
                    string inLine = dtBomLineInLeftOrRight.Rows[dtIndex]["LOCATION_ROW"].ToString();//移入行号
                    DataTable allLocInInline = ce.GetAllLocByLine(inLine, isDesc);//移入行所有货位信息
                    for (; moveNum < storedLocInOutline.Rows.Count;)
                    {
                        string location_out_id = storedLocInOutline.Rows[moveNum]["t_id"].ToString();//移出货位id
                        string locationcode = storedLocInOutline.Rows[moveNum]["LOCATION_NO"].ToString();//移出货位编码
                        DataRow[] _tempCon = Container.GetHeadsByLocation(location_out_id);
                        string container_htid = _tempCon[0]["t_id"].ToString();//容器id
                        DataRow drContainer = Container.GetContainerH(container_htid);//查询容器信息                    
                        DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_htid);//查询容器明细信息
                        string containerBarcode = drContainer["container_barcode"].ToString();//容器的托盘条码
                        string location_in_id = allLocInInline.Rows[lastEmptylocIndex]["t_id"].ToString();//待入货位
                        LocationModel locInInfo = Location.GetLocationById(location_in_id);//移入货位信息
                        string locInNo = locInInfo.location_no;//移入货位编码
                        string locInStatus = locInInfo.location_status.ToString();//移入行货位状态

                        #region 更新货位状态，并产生任务
                        //更新移出货位状态为待出
                        ce.UpdateLocStatusByLocationId(location_out_id, LocationStatus.LocationStatus_Out.ToString(), LocationStatus.LocationStatus_Stored.ToString(), "");
                        //更新移入货位状态为待入
                        ce.UpdateLocStatusByLocationId(location_in_id, LocationStatus.LocationStatus_In.ToString(), LocationStatus.LocationStatus_Empty.ToString(), "");

                        #region 插入任务主档
                        Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
                        {
                            ID = SnowflakeID.Guid().ToString(),//主键ID
                            ZoneID = zoneId,//区域ID
                            TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                            DependentTaskNo = "",//依赖任务号
                            TaskType = TaskType.TaskType_Move.ToString(),//移库任务
                            ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                            //ExecutedBy = "",//任务执行人
                            EquipmentID = actureEquipmentBy,//执行设备ID
                            //EquipmentType = "",//执行设备类型
                            //ActualEquipmentID = "",//实际执行设备ID
                            SeqNo = 1,//顺序号
                            Priority = 1,//优先级
                            ContainerID = container_htid,//容器ID
                            ContainerBarcode = containerBarcode,//容器条码
                            RefDocType = DocType.DocType_AutoAdjustment.ToString(),//来源单据类型
                            RefDocID = "",//来源单ID
                            RefDocDetailID = "",//来源单明细ID
                            RefDocNo = outLine,//来源单号
                            SrcAddrID = location_out_id,//来源地址ID
                            SrcAddrType = AddressType.AddressType_Location.ToString(),//来源地址类型 "location"
                            SrcAddrText = locationcode,//来源地址
                            DestAddrID = location_in_id,//目标地址ID
                            DestAddrType = AddressType.AddressType_Location.ToString(),//目标地址类型"location"
                            DestAddrText = locInNo,//目标地址
                            //ActualDestAddrID = "",//实际目标地址ID
                            //ActualDestAddrText = "",//实际目标地址
                            TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                            StartDate = dateTime,//开始时间
                            //EndDate = dateTime,//结束时间
                            IsExecutable = "1",//任务是否可执行
                            //PriorityChangeTime = dateTime,//优先级修改时间        
                            LedText = "",//LED显示内容，此字段有其他用途
                            ExistLgv = "0",//是否存在LGV
                            //CreateDate = dateTime,//创建时间
                            //CreatedBy = "",//创建人帐号
                            //ModifyIP = "",//操作人IP 
                            //ModifyDate = dateTime,//编辑时间
                            //LastModifiedBy = "",//最后修改人帐号
                            ActStatus = "1"//状态
                        };
                        WorkTask.Insert(taskModel);
                        #endregion

                        for (int k = 0; k < dtrContainerDInfo.Length; k++)
                        {
                            #region 插入任务物料信息
                            WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                            {
                                ID = SnowflakeID.Guid().ToString(),//主键ID
                                //ClientID = "",//集团ID
                                //OrgID = "",//组织ID
                                //WarehouseID = "",//仓库ID
                                TaskID = taskModel.ID,//主任务ID
                                ItemID = dtrContainerDInfo[k]["item_id"].ToString(),//物料ID
                                ItemName = dtrContainerDInfo[k]["item_name"].ToString(),//物料名称
                                PlanQty = Convert.ToDecimal(dtrContainerDInfo[k]["qty"].ToString()),//计划数量
                                //ProcessedQty = "",//已处理数量
                                //FinishedQty = "",//完成数量
                                UomQtyID = dtrContainerDInfo[k]["uom_qty_id"].ToString(),//数量单位
                                //FinishedStatQty = "",//完成统计数量
                                UomStatID = dtrContainerDInfo[k]["uom_stat_id"].ToString(),//统计数量单位
                                ContainerDetailID = dtrContainerDInfo[k]["t_id"].ToString(),//容器明细ID
                                RefDocType = DocType.DocType_AutoAdjustment.ToString(),//来源单据类型  
                                RefDocID = "",//来源单ID
                                //RefDocDetailID = "",//来源单明细ID
                                RefDocNo = "",//来源单号
                                //StartDate = "",//开始时间
                                //EndDate = "",//结束时间
                                ProcessStatus = ProcessStatus.ProcessStatus_New.ToString()//工作状态
                                //CreateDate = "",//创建时间
                                //CreatedBy = "",//创建人帐号 
                                //ModifyIP = "",//操作人IP 
                                //ModifyDate = "",//编辑时间
                                //LastModifiedBy = "",//最后修改人帐号
                                //ActStatus = ""//状态
                            };
                            WorkTaskItem.Insert(taskItemModel);
                            #endregion
                        }
                        tasklist.Add(taskModel.ID);
                        #endregion

                        lastEmptylocIndex--;
                        moveNum++;
                        if (lastEmptylocIndex < 0)
                        {
                            break;
                        }
                    }
                }
                _tempNum++;
                finishAutoAdjustNum++;
            }

            return tasklist;
        } 
        #endregion


        #endregion

        #region 自动库位调整任务完成
        /// <summary>
        /// 自动库位调整任务完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishAutoLocationadJust(LocationAdjustModel.FinishAutoLocationadJustModel model)
        {
            #region 1.获取自动库位调整任务信息
            //获取自动库位调整任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到库位调整任务！");
            }
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string location_out_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是移出货位id
            string location_in_id = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的地址id，此处指的是移入货位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            string adjustDTid = dtTaskInfo.Rows[0]["ref_doc_d_id"].ToString();//任务中，调整单明细id
            string locationadJustHTid = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//任务中，调整单主档id
            if (refDocType != DocType.DocType_AutoAdjustment.ToString())
            {
                throw new WmsException(500, "该任务不是自动库位调整任务，无法完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "任务仅在处于已部分完成状态下，才能完成！");
            }
            #endregion

            #region 5.更新任务状态，并将任务写入历史表
            //将任务更新为完成
            WorkTask.UpdateTaskComplete(model.TaskTid);
            WorkTask.MoveToHistory(model.TaskTid);
            #endregion
        }
        #endregion

        #region 自动库位调整任务部分完成
        /// <summary>
        /// 自动库位调整任务部分完成
        /// </summary>
        /// <param name="model"></param>
        public void PartionFinishAutoLocationadJustTask(LocationAdjustModel.PartionFinishAutoLocationadJustTaskModel model)
        {
            #region 1.获取自动库位调整任务信息
            //获取自动库位调整任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到库位调整任务！");
            }
            string zone_id = dtTaskInfo.Rows[0]["zone_id"].ToString();//区域
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string location_out_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是移出货位id
            string location_in_id = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的地址id，此处指的是移入货位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            string adjustDTid = dtTaskInfo.Rows[0]["ref_doc_d_id"].ToString();//任务中，调整单明细id
            if (refDocType != DocType.DocType_AutoAdjustment.ToString())
            {
                throw new WmsException(500, "该任务不是自动库位调整任务，无法强制完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中状态下，才能部分完成！");
            }
            #endregion

            #region 2.将任务修改为部分完成
            WorkTask.UpdateTaskStatus(model.TaskTid, TaskStatus.TaskStatus_PartialFinished.ToString(), TaskStatus.TaskStatus_Executing.ToString());
            #endregion

            #region 3.修改移出货位状态，并将容器挂在移入货位上           
            //修改移出货位状态为空
            ce.UpdateLocStatusByLocationId(location_out_id, LocationStatus.LocationStatus_Empty.ToString(), LocationStatus.LocationStatus_Out.ToString(), "");
            //修改容器主档中货位字段数据为移入货位id
            Container.UpdateContainerLocation(container_id, location_in_id);
            //修改移入货位状态为空
            ce.UpdateLocStatusByLocationId(location_in_id, LocationStatus.LocationStatus_Stored.ToString(), LocationStatus.LocationStatus_In.ToString(), "");
            #endregion

            string agvZoneId = WmsConfig.GetOrDefault("Inbound:InboundZone", "1293844199577358336");
            if (string.Equals(agvZoneId, zone_id))//二楼，需要先判断行是否已经空了，如果空了，则把货位标识，修改成可用，用于允许下次入库
            {
                //LocationModel locOutInfo = Location.GetLocationById(location_out_id);//移出货位信息
                //string row = locOutInfo.location_row.ToString();
                //bool isHavePlanLoc = ce.GetPlanLocNumByLine(row);
                //if (isHavePlanLoc == false)
                //{
                //    ce.UpdatelocationflagByRow(row, LocationFlag.LocationFlag_Usable.ToString());
                //}
            }
        }
        #endregion

        #region 自动库位调整任务强制完成
        /// <summary>
        /// 自动库位调整任务强制完成
        /// </summary>
        /// <param name="model"></param>
        public void ForceFinishAutoLocationadJustTask(LocationAdjustModel.ForceFinishAutoLocationadJustTaskModel model)
        {
            #region 1.获取自动库位调整任务信息
            //获取自动库位调整任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到库位调整任务！");
            }
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string location_out_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是移出货位id
            string location_in_id = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的地址id，此处指的是移入货位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            string adjustDTid = dtTaskInfo.Rows[0]["ref_doc_d_id"].ToString();//任务中，调整单明细id
            if (refDocType != DocType.DocType_AutoAdjustment.ToString())
            {
                throw new WmsException(500, "该任务不是自动库位调整任务，无法强制完成！");
            }

            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "任务已完成，无需再次完成！");
            }
            #endregion

            //任务不是部分完成，则执行部分完成
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                LocationAdjustModel.PartionFinishAutoLocationadJustTaskModel pft = new LocationAdjustModel.PartionFinishAutoLocationadJustTaskModel();
                pft.TaskTid = model.TaskTid;
                PartionFinishAutoLocationadJustTask(pft);
            }

            //执行完成任务
            LocationAdjustModel.FinishAutoLocationadJustModel ft = new LocationAdjustModel.FinishAutoLocationadJustModel();
            ft.TaskTid = model.TaskTid;
            FinishAutoLocationadJust(ft);
        }
        #endregion

        #region 自动库位调整任务回收
        /// <summary>
        /// 自动库位调整任务回收
        /// </summary>
        /// <param name="model"></param>
        public void RecycleAutoLocationadJustTask(LocationAdjustModel.RecycleAutoLocationadJustTaskModel model)
        {
            #region 1.获取自动库位调整任务信息
            //获取自动库位调整任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到库位调整任务！");
            }
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string equipment_id = dtTaskInfo.Rows[0]["equipment_id"].ToString();//任务中，执行设备
            string ref_doc_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//任务中，来源单号，保存的是自动库位调整的行号
            string location_out_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是移出货位id
            string location_in_id = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的地址id，此处指的是移入货位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_AutoAdjustment.ToString())
            {
                throw new WmsException(500, "该任务不是自动库位调整任务，无法完成！");
            }
            if (taskStatus == TaskStatus.TaskStatus_PartialFinished.ToString() || taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "任务处于已部分完成或完成状态下，不能回收！");
            }
            #endregion           

            //调整单明细信息
            DataTable dtLocJustDInfo = new DataTable();
            string sc01 = WmsConfig.GetOrDefault("Task:SC01", "1288091969666551808");//1号堆垛机t_id
            string sc02 = WmsConfig.GetOrDefault("Task:SC02", "1288092061295316992");//2号堆垛机t_id
            if (string.Equals(equipment_id, sc01))
            {
                dtLocJustDInfo = SelectTaskNumByRefDocNo(ref_doc_no, false);
            }
            else if (string.Equals(equipment_id, sc02))
            {
                dtLocJustDInfo = SelectTaskNumByRefDocNo(ref_doc_no, true);
            }
            else
            {//二楼
                dtLocJustDInfo = SelectTaskNumByRefDocNo(ref_doc_no, true);
            }
            int _index = 0;
            for (int i = 0; i < dtLocJustDInfo.Rows.Count; i++)
            {
                if (string.Equals(model.TaskTid, dtLocJustDInfo.Rows[i]["t_id"].ToString()))
                {
                    _index = i;
                    break;
                }
            }
            //修改任务软删除标记为“0”
            WorkTask.UpdateActStatus(model.TaskTid, "0");
            //更新移出货位状态为已存储
            ce.UpdateLocStatusByLocationId(location_out_id, LocationStatus.LocationStatus_Stored.ToString(), LocationStatus.LocationStatus_Out.ToString(), "");
            //更新移入货位状态为空
            ce.UpdateLocStatusByLocationId(location_in_id, LocationStatus.LocationStatus_Empty.ToString(), LocationStatus.LocationStatus_In.ToString(), "");

            if (_index + 1 < dtLocJustDInfo.Rows.Count)
            {
                DataRow nextTask = WorkTask.GetWorkTask(dtLocJustDInfo.Rows[_index + 1]["t_id"].ToString());
                //DataRow nextTask = GetTaskByRefDocDetailID(dtLocJustDInfo.Rows[_index + 1]["t_id"].ToString());
                WorkTask.RecycleTask(nextTask);
            }
        }
        #endregion


        #region 公共方法

        #region 获取调整单主档信息
        /// <summary>
        /// 获取调整单主档信息
        /// </summary>
        /// <param name="locationadJustHTid">单据id</param>
        /// <returns></returns>
        public DataRow GetLocJustHInfo(string locationadJustHTid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "locationadJustHTid", locationadJustHTid }
            };
            string sql = $"select e.* from &DataSource&wms_b_locationadjust_h e where e.t_id = @locationadJustHTid and e.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "获取调整单主档信息");

            return dt.Rows[0];
        }
        #endregion

        #region 获取无任务的堆垛机
        /// <summary>
        /// 获取无任务的堆垛机
        /// </summary>
        /// <param name="sC01">1号堆垛机id</param>
        /// <param name="sC02">2号堆垛机id</param>
        /// <returns></returns>
        public DataTable GetLocJustEquipmentInfo(string sC01, string sC02)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "eqOne", sC01 },
                { "eqTwo", sC02 }
            };
            string sql = $"select distinct t.equipment_id from &DataSource&wms_b_task t where t.act_status='1' and t.equipment_id in('@eqOne','@eqTwo') order by t.equipment_id ";
            DataTable dt = DBHelper.Select(sql, param, "获取无任务的堆垛机", false);

            return dt;
        }
        #endregion

        #region 根据区域获取库内存放的所有盘型，以及保存该盘型的行的数量
        /// <summary>
        /// 根据区域获取库内存放的所有盘型，以及保存该盘型的行的数量
        /// </summary>
        /// <param name="zoneId">区域id</param>
        /// <returns></returns>
        public DataTable GetBomInfoByZoneId(string zoneId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "zoneId", zoneId }
            };
            //string sql = $"select x.zone_id,x.bom_id,count(1) as cnt from &DataSource&V_LINE_INFO x where x.zone_id=@zoneId and x.bom_id is not null group by x.zone_id,x.bom_id order by x.zone_id,x.bom_id";
            string sql = $"select x.zone_id,x.bom_id,count(1) as cnt from &DataSource&V_LINE_INFO x where x.zone_id=@zoneId and x.bom_id is not null  and x.bom_id<>'1285462015653056512' and substr(x.location_row, 2, 2) not in ('28', '29', '30')   group by x.zone_id,x.bom_id order by x.zone_id,x.bom_id";
            DataTable dt = DBHelper.Select(sql, param, "获取库内存放的所有盘型信息");

            return dt;
        }
        #endregion

        #region 根据区域获取大货位区域内存放的所有盘型，以及保存该盘型的行的数量
        /// <summary>
        /// 根据区域获取大货位区域内存放的所有盘型，以及保存该盘型的行的数量
        /// </summary>
        /// <param name="zoneId">区域id</param>
        /// <returns></returns>
        public DataTable GetBigZoneBomInfoByZoneId(string zoneId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "zoneId", zoneId }
            };
            //string sql = $"select x.zone_id,x.bom_id,count(1) as cnt from &DataSource&V_LINE_INFO x where x.zone_id=@zoneId  and substr(x.location_row, 2, 2)>=1 and substr(x.location_row, 2, 2)<=17 and x.bom_id is not null  group by x.zone_id,x.bom_id order by x.zone_id,x.bom_id";
            string sql = $"select x.zone_id,x.bom_id,count(1) as cnt from &DataSource&V_LINE_INFO x where x.zone_id=@zoneId  and (((substr(x.location_row, 2, 2) >= 1 and substr(x.location_row, 2, 2) <= 17) and substr(x.location_row, 2, 2) not in ('04', '09', '16')) or substr(x.location_row, 2, 2) in ('28', '29', '30')) and x.bom_id is not null and x.bom_id<>'1285462015653056512'  group by x.zone_id,x.bom_id order by x.zone_id,x.bom_id";
            DataTable dt = DBHelper.Select(sql, param, "获取库内存放的所有盘型信息");

            return dt;
        }
        #endregion

        #region 根据区域和行获取盘型，以及保存该盘型在行中存放的托盘数量
        /// <summary>
        /// 根据区域和行获取盘型，以及保存该盘型在行中存放的托盘数量
        /// </summary>
        /// <param name="zoneId">区域id</param>
        /// <returns></returns>
        public DataTable GetBomInfoByZoneIdAndLine(string zoneId, string bomId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "zoneId", zoneId },
                { "bomId", bomId }
            };
            //string sql = $"select x.* from &DataSource&V_LINE_INFO x where x.zone_id=@zoneId and x.bom_id=@bomId  and x.bom_id is not null and x.inCnt<=0 and x.outCnt<=0 and x.planCnt<=0 order by x.storedCnt asc";
            string sql = $"select x.* from &DataSource&V_LINE_INFO x where x.zone_id=@zoneId and x.bom_id=@bomId  and x.bom_id is not null and substr(x.location_row, 2, 2) not in ('28', '29', '30') and x.inCnt<=0  and x.bom_id<>'1285462015653056512' and x.outCnt<=0 and x.planCnt<=0 order by x.storedCnt asc";
            DataTable dt = DBHelper.Select(sql, param, "获取指定Bom的托盘信息",false);
            return dt;
        }
        #endregion

        #region 获取柱子右边的行
        /// <summary>
        /// 获取柱子右边的行
        /// </summary>
        /// <param name="zoneId">区域id</param>
        /// <param name="bomId">bomId</param>
        /// <returns></returns>
        public DataTable GetRightLineByZoneIdAndBom(string zoneId, string bomId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "zoneId", zoneId },
                { "bomId", bomId }
            };
            //string sql = $"select x.* from &DataSource&V_LINE_INFO x where x.zone_id=@zoneId and x.bom_id=@bomId  and x.bom_id is not null and x.inCnt<=0 and x.outCnt<=0 and x.planCnt<=0 order by x.storedCnt asc";
            string sql = $"select x.* from &DataSource&V_LINE_INFO x where x.zone_id=@zoneId and substr(x.location_row, 2, 2) in ('04', '09', '16') and x.inCnt<=0 and x.outCnt<=0 and x.planCnt<=0 and x.STOREDCNT<=0 order by x.location_row asc";
            DataTable dt = DBHelper.Select(sql, param, "获取柱子右边的行", false);
            return dt;
        }
        #endregion

        #region 根据区域和行获取大货位区中的盘型，以及保存该盘型在行中存放的托盘数量
        /// <summary>
        /// 根据区域和行获取大货位区中的盘型，以及保存该盘型在行中存放的托盘数量
        /// </summary>
        /// <param name="zoneId">区域id</param>
        /// <returns></returns>
        public DataTable GetBigZoneBomInfoByZoneIdAndLine(string zoneId, string bomId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "zoneId", zoneId },
                { "bomId", bomId }
            };
            //string sql = $"select x.* from &DataSource&V_LINE_INFO x where x.zone_id=@zoneId and x.bom_id=@bomId  and x.bom_id is not null  and substr(x.location_row, 2, 2)>=1 and substr(x.location_row, 2, 2)<=17 and x.inCnt<=0 and x.outCnt<=0 and x.planCnt<=0 order by x.storedCnt asc";
            string sql = $"select x.* from &DataSource&V_LINE_INFO x where x.zone_id=@zoneId and x.bom_id=@bomId  and x.bom_id is not null  and (((substr(x.location_row, 2, 2) >= 1 and substr(x.location_row, 2, 2) <= 17) and substr(x.location_row, 2, 2) not in ('04', '09', '16')) or substr(x.location_row, 2, 2) in ('28', '29', '30')) and x.inCnt<=0 and x.outCnt<=0 and x.planCnt<=0 order by x.storedCnt asc";
            DataTable dt = DBHelper.Select(sql, param, "获取指定Bom的托盘信息",false);
            return dt;
        }
        #endregion

        #region 根据柱子左边的行信息
        /// <summary>
        /// 根据柱子左边的行信息
        /// </summary>
        /// <param name="zoneId">区域id</param>
        /// <param name="bomId">bomId</param>
        /// <returns></returns>
        public DataTable GetLeftLineByZoneIdAndBomId(string zoneId, string bomId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "zoneId", zoneId },
                { "bomId", bomId }
            };
            //string sql = $"select x.* from &DataSource&V_LINE_INFO x where x.zone_id=@zoneId and x.bom_id=@bomId  and x.bom_id is not null  and substr(x.location_row, 2, 2)>=1 and substr(x.location_row, 2, 2)<=17 and x.inCnt<=0 and x.outCnt<=0 and x.planCnt<=0 order by x.storedCnt asc";
            string sql = $"select x.* from &DataSource&V_LINE_INFO x where x.zone_id=@zoneId and substr(x.location_row, 2, 2) in ('28', '29', '30') and x.inCnt<=0 and x.outCnt<=0 and x.planCnt<=0 and x.STOREDCNT<=0 order by x.location_row asc";
            DataTable dt = DBHelper.Select(sql, param, "根据柱子左边的行信息", false);
            return dt;
        }
        #endregion

        #region 获取调整单明细信息
        /// <summary>
        /// 获取调整单明细信息
        /// </summary>
        /// <param name="locationadJustHTid">单据id</param>
        /// <param name="isDesc">排序。true:降序；false:升序</param>
        /// <returns></returns>
        public DataTable GetLocJustDInfo(string locationadJustHTid, bool isDesc)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "locationadJustHTid", locationadJustHTid }
            };
            string sql = $"select distinct l.location_no,e.* from &DataSource&wms_b_locationadjust_d e,&DataSource&wms_m_location l where e.adjust_hid = @locationadJustHTid and l.t_id=e.location_out_id and e.act_status='1' and e.adjust_detail_state <>'{AdjustState.AdjustState_Cancel}'";
            if (isDesc == true)
            {
                sql += "  order by l.location_no desc";
            }
            else
            {
                sql += "  order by l.location_no";
            }
            DataTable dt = DBHelper.Select(sql, param, "获取调整单明细信息");

            return dt;
        }
        #endregion

        #region 根据来源单Id查询未完成的任务数量
        /// <summary>
        /// 根据来源单Id查询未完成的任务数量
        /// </summary>
        /// <param name="tId">来源单id</param>
        /// <returns></returns>
        public DataRow SelectTaskNumByRefDocID(string tId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "tId", tId }
            };
            string sql = $"select count(t.t_id) as cnt from &DataSource&wms_b_task t where t.ref_doc_id=@tId and t.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "查询调整单对应的任务数量");

            return dt.Rows[0];
        }
        #endregion

        #region 来源单号获取自动调整单任务信息
        /// <summary>
        /// 来源单号获取自动调整单任务信息
        /// </summary>
        /// <param name="refDocNo">来源单号</param>
        /// <param name="isDesc">排序。true:降序；false:升序</param>
        /// <returns></returns>
        public DataTable SelectTaskNumByRefDocNo(string refDocNo, bool isDesc)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "refDocNo", refDocNo }
            };
            string sql = $"select t.* from &DataSource&wms_b_task t where t.ref_doc_no=@refDocNo and t.act_status='1' and t.ref_doc_type='{DocType.DocType_AutoAdjustment}' and t.task_status not in('{TaskStatus.TaskStatus_Finished}','{TaskStatus.TaskStatus_PartialFinished}')";
            if (isDesc == true)
            {
                sql += "  order by t.src_addr_text desc";
            }
            else
            {
                sql += "  order by t.src_addr_text";
            }
            DataTable dt = DBHelper.Select(sql, param, "来源单号获取自动调整单任务信息");

            return dt;
        }
        #endregion

        #region 根据区域Id查询未完成的自动调整任务数量
        /// <summary>
        /// 根据区域Id查询未完成的自动调整任务数量
        /// </summary>
        /// <param name="zone_id">区域id</param>
        /// <returns>true:存在；false:不存在</returns>
        public bool SelectAutoAdjustTaskNumByZoneID(string zone_id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "zone_id", zone_id }
            };
            string sql = $"select count(t.t_id) as cnt from &DataSource&wms_b_task t where t.zone_id=@zone_id and t.act_status='1' and t.ref_doc_type='{DocType.DocType_AutoAdjustment}' and t.task_status not in('{TaskStatus.TaskStatus_Finished}','{TaskStatus.TaskStatus_PartialFinished}')";
            DataTable dt = DBHelper.Select(sql, param, "查询未完成的自动调整任务数量");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) > 0;
        }
        #endregion

        #region 根据来源单明细ID查询任务
        /// <summary>
        /// 根据来源单明细ID查询任务
        /// </summary>
        /// <param name="refDocDetailTid">来源单明细id</param>
        /// <returns></returns>
        public DataRow GetTaskByRefDocDetailID(string refDocDetailTid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "refDocDetailTid", refDocDetailTid }
            };
            string sql = $"select t.* from &DataSource&wms_b_task t where t.ref_doc_d_id=@refDocDetailTid and t.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "根据调整单明细查询任务");

            return dt.Rows[0];
        }
        #endregion

        #region 根据目的地址Id查询任务
        /// <summary>
        /// 根据目的地址Id查询任务
        /// </summary>
        /// <param name="refDocDetailTid">来源单明细id</param>
        /// <returns></returns>
        public DataRow GetTaskByDestAddrID(string destAddrIDTid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "destAddrIDTid", destAddrIDTid }
            };
            string sql = $"select t.* from &DataSource&wms_b_task t where t.dest_addr_id=@destAddrIDTid and t.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "根据目的地址获取任务信息");

            return dt.Rows[0];
        }
        #endregion

        #region 根据行号查询该行是否存在除库位调整以外的其他任务
        /// <summary>
        /// 根据行号查询该行是否存在除库位调整以外的其他任务
        /// </summary>
        /// <param name="row">行号</param>
        /// <returns>true:存在；false:不存在</returns>
        public bool IsLocHaveTask(string rowq)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "rowq", rowq }
            };
            //string sql = $"select count(l.t_id) as cnt from &DataSource&wms_m_location l where l.act_status='1' and l.location_row||'' = '@rowq'||'' and l.location_status not in('{LocationStatus.LocationStatus_Empty}') ";
            //string sql = $"select count(l.t_id) as cnt from &DataSource&wms_m_location l where l.act_status='1' and l.location_row = @rowq and l.location_status not in('{LocationStatus.LocationStatus_Empty}','{LocationStatus.LocationStatus_Stored}') ";
            string sql = $"select count(t.t_id) as cnt from &DataSource&wms_b_task t where (t.src_addr_id in( select l.t_id from &DataSource&wms_m_location l where l.act_status = '1' and l.location_row = @rowq) or t.dest_addr_id in(select l.t_id from &DataSource&wms_m_location l where l.act_status = '1' and l.location_row = @rowq)) and t.act_status='1' and t.ref_doc_type <>'{DocType.DocType_Adjustment}'";
            DataTable dt = DBHelper.Select(sql, param, "查询调整单明细中调整行是否存在除库位调整以外的任务");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) > 0;
        }
        #endregion

        #region 根据行号查询该行是否存在任务
        /// <summary>
        /// 根据行号查询该行是否存在任务
        /// </summary>
        /// <param name="row">行号</param>
        /// <returns>true:存在；false:不存在</returns>
        public bool IsLocHaveAutoAdjustmentTask(string rowq)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "rowq", rowq }
            };
            //string sql = $"select count(l.t_id) as cnt from &DataSource&wms_m_location l where l.act_status='1' and l.location_row||'' = '@rowq'||'' and l.location_status not in('{LocationStatus.LocationStatus_Empty}') ";
            //string sql = $"select count(l.t_id) as cnt from &DataSource&wms_m_location l where l.act_status='1' and l.location_row = @rowq and l.location_status not in('{LocationStatus.LocationStatus_Empty}','{LocationStatus.LocationStatus_Stored}') ";
            //string sql = $"select count(t.t_id) as cnt from &DataSource&wms_b_task t where (t.src_addr_id in( select l.t_id from &DataSource&wms_m_location l where l.act_status = '1' and l.location_row = @rowq) or t.dest_addr_id in(select l.t_id from &DataSource&wms_m_location l where l.act_status = '1' and l.location_row = @rowq)) and t.act_status='1' and t.ref_doc_type <>'{DocType.DocType_Adjustment}'";
            string sql = $"select count(t.t_id) as cnt from &DataSource&wms_b_task t where (t.src_addr_id in( select l.t_id from &DataSource&wms_m_location l where l.act_status = '1' and l.location_row = @rowq) or t.dest_addr_id in(select l.t_id from &DataSource&wms_m_location l where l.act_status = '1' and l.location_row = @rowq)) and t.act_status='1' ";
            DataTable dt = DBHelper.Select(sql, param, "查询调整单明细中调整行是否存在任务");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) > 0;
        }
        #endregion

        #region 根据货位id查询该货位上是否挂有容器
        /// <summary>
        /// 根据货位id查询该货位上是否挂有容器
        /// </summary>
        /// <param name="locId">货位id</param>
        /// <returns>true:有；false:没有</returns>
        public bool IsLocHaveContainer(string locId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "locId", locId }
            };
            string sql = $"select count(n.t_id) as cnt from &DataSource&wms_b_container_h n where n.location_id=@locId and n.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "查询货位上是否有托盘");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) > 0;
        }
        #endregion

        #region 修改调整单主档单据状态
        /// <summary>
        /// 修改调整单主档单据状态
        /// </summary>
        /// <param name="locJustHTid">单据主档id</param>
        /// <param name="adjust_state">单据主档状态</param>
        public void UpdatelocJustHByTid(string locJustHTid, string adjust_state)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "adjust_state", adjust_state },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "locJustHTid", locJustHTid }
            };
            string sql = "update &DataSource&wms_b_locationadjust_h l set l.adjust_state=@adjust_state,l.last_modified_by=@last_modified_by,l.modify_ip=@modify_ip,l.modify_date=sysdate where l.t_id=@locJustHTid  and l.act_status='1'";
            DBHelper.RunSql(sql, param, "修改调整单主档单据状态");
        }
        #endregion

        #region 修改调整单主档信息（移库原因和备注，区域、设备、移出行和移入行）
        /// <summary>
        /// 修改调整单主档信息（移库原因和备注，区域、设备、移出行和移入行）
        /// </summary>
        /// <param name="locJustHTid">单据主档id</param>
        /// <param name="reason">移库原因</param>
        /// <param name="memo">备注</param>
        /// <param name="zoneId">区域</param>
        /// <param name="equipmentId">设备</param>
        /// <param name="outLine">移出行</param>
        /// <param name="inLine">移入行</param>
        public void UpdateReasonAndMemolocJustHByTid(string locJustHTid, string reason, string memo, string zoneId, string equipmentId, string outLine, string inLine)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "reason", reason },
                { "memo", memo },
                { "zoneId", zoneId },
                { "equipmentId", equipmentId },
                { "outLine", outLine },
                { "inLine", inLine },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "locJustHTid", locJustHTid }
            };
            string sql = "update &DataSource&wms_b_locationadjust_h l set l.adjust_reason=@reason, l.zone_id=@zoneId, l.equipment_id=@equipmentId, l.outline=@outLine, l.inline=@inLine,l.memo=@memo,l.last_modified_by=@last_modified_by,l.modify_ip=@modify_ip,l.modify_date=sysdate where l.t_id=@locJustHTid  and l.act_status='1'";
            DBHelper.RunSql(sql, param, "修改调整单主档移库原因和备注");
        }
        #endregion

        #region 修改调整单明细单据状态
        /// <summary>
        /// 修改调整单明细单据状态
        /// </summary>
        /// <param name="locJustDTid">单据明细id</param>
        /// <param name="adjust_detail_state">单据明细状态</param>
        public void UpdatelocJustDByTid(string locJustDTid, string adjust_detail_state)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "adjust_detail_state", adjust_detail_state },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "locJustDTid", locJustDTid }
            };
            string sql = "update &DataSource&wms_b_locationadjust_d l set l.adjust_detail_state=@adjust_detail_state,l.last_modified_by=@last_modified_by,l.modify_ip=@modify_ip,l.modify_date=sysdate where l.t_id=@locJustDTid  and l.act_status='1'";
            DBHelper.RunSql(sql, param, "修改调整单明细单据状态");
        }
        #endregion

        #region 软删除调整单明细
        /// <summary>
        /// 软删除调整单明细
        /// </summary>
        /// <param name="locJustDTid">单据明细id</param>
        /// <param name="actStatus">软删除标记</param>
        public void UpdateActStatusJustDByTid(string locJustHTid, string actStatus)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "actStatus", actStatus },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "locJustHTid", locJustHTid }
            };
            string sql = "update &DataSource&wms_b_locationadjust_d l set l.act_status=@actStatus,l.last_modified_by=@last_modified_by,l.modify_ip=@modify_ip,l.modify_date=sysdate where l.adjust_hid=@locJustHTid  and l.act_status='1'";
            DBHelper.RunSql(sql, param, "软删除调整单明细", OperatedNumType.ZeroOrMore);
        }
        #endregion

        #region 查询是否存在执行中的调整单明细（调整单一旦开始执行，明细全部由未开始变成执行中），因此是否存在执行中的明细，就可以决定是否需要将主档变成完成
        /// <summary>
        /// 查询是否存在执行中的调整单明细
        /// </summary>
        /// <param name="adjustHTid">调整单主档id</param>
        /// <returns>true:存在；false:不存在</returns>
        public bool IsHaveExecutingAdJustDByAdjustHTid(string adjustHTid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "adjustHTid", adjustHTid }
            };
            string sql = $"select count(t.t_id) as cnt from &DataSource&wms_b_locationadjust_d t where t.adjust_hid=@adjustHTid and t.adjust_detail_state='{AdjustState.AdjustState_Executing}'  and t.act_status='1'";
            DataTable dt = DBHelper.Select(sql, param, "查询调整单执行中明细");

            return Convert.ToInt32(dt.Rows[0]["cnt"].ToString()) > 0;
        }
        #endregion

        #region 获取货位上绑定的容器信息
        /// <summary>
        /// 获取货位上绑定的容器信息
        /// </summary>
        /// <param name="locTid">货位id</param>
        /// <returns></returns>
        public DataTable GetLocContainerInfo(string locTid)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "locTid", locTid }
            };
            string sql = $"select distinct l.t_id,l.location_no,n.bom_id,n.t_id as containerTid from &DataSource&wms_m_location l left join &DataSource&wms_b_container_h n on l.t_id=n.location_id and l.act_status='1' and n.act_status='1' where   l.t_id=@locTid";
            DataTable dt = DBHelper.Select(sql, param, "货位上的托盘信息");

            return dt;
        }
        #endregion

        #endregion
    }
}
