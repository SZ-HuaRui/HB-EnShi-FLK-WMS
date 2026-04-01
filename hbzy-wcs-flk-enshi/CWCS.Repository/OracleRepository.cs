using CWCS.Common.Uitl;
using CWCS.Entity;
using CWCS.Entity.SRM;
using CWCS.Interface.Request.WMS;
using CWCS.Interface.Response.WMS;
using CWCS.Interface.WMSMicroService;
using DataAccess;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace CWCS.Repository
{
    public class OracleRepository
    {
        private OracleHelper helper;
        private OracleHelper wmsHelper;

        public string WareHouseID = System.Configuration.ConfigurationManager.AppSettings["WarehouseID"].ToString();

        public OracleRepository()
        {
            helper = OracleHelper.WcsInstance("DBContext");
            wmsHelper = OracleHelper.WmsInstance("WMSDBContext");
        }

        /// <summary>
        /// 获取设道信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetSetDockInfo()
        {
            string sql = $@"select e.equipment_number,e.description DOCKNO,e.station_no stationNO, i.item_code itemCode,i.item_name itemName,decode(i.each_pallet_fillqty,30,1,28,2,0) DockType,i.pallet_mode BoxType,
                            decode(canedit,0,'是','否') ISWCSCLEAN,decode(canedit,0,'是','否')ISCANCHANGEBRAND
                            from LANEWAYSELECT l
                            inner join swms_wi_item i on l.itemid = i.t_id
                            inner join swms_wi_equipment e on e.t_id = l.equipmentid
                            where l.canedit = 1
                            order by e.equipment_number asc";
            return helper.ExecuteQueryBySql(sql);
        }

        public DataTable GetAgvAlarm(int carNo)
        {
                AccessHelper accessHelper = new AccessHelper("AgvDbContext");
                string sql = $@"SELECT top 1  EventCode, EventString, SourceID, EventTime,
                                switch (
                                t.EventCode = 2011,1,
                                t.EventCode = 2012,2,
                                t.EventCode = 2013,3,
                                t.EventCode = 2014,4,
                                t.EventCode = 2350,5,
                                t.EventCode = 4444,6,
                                t.EventCode = 7777,7,
                                t.EventCode = 100, 0,
                                t.EventCode = 8001,9,
                                t.EventCode = 8002,10,
                                t.EventCode = 8003,11,
                                t.EventCode = 5010,12,
                                t.EventCode = 5011,13,
                                True,0
                                )as plcflag
                                FROM EventLog t
                             WHERE   SourceID = { carNo}
                             ORDER BY t.EventTime DESC"; ;
                return accessHelper.ExecuteQueryBySql(sql);
           
        }

        //从充电站到堆垛机
        public bool UpdateShuttleBoardInfo_CsToSc(string chargingStationNo, string belongEquipmentNo)
        {
            string sql = $@"update wcs_shuttleinfo t 
                            set t.current_status = 'In_Sc',
                            t.modify_time = sysdate
                            where t.current_status = 'In_ChargingStation' 
                            and t.belong_equipment_no = '{belongEquipmentNo}'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        //从堆垛机到充电站
        public bool UpdateShuttleBoardInfo_ScToCs(string chargingStationNo, string belongEquipmentNo)
        {
            string sql = $@"update wcs_shuttleinfo t 
                            set t.current_status = 'In_ChargingStation',
                            t.modify_time = sysdate
                            where t.current_status = 'In_Sc' 
                            and t.belong_equipment_no = '{belongEquipmentNo}'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        //更新进入或离开充电位时间
        public bool UpdateEnterOrLeaveChargingStationTime(string chargingStationNo, string belongEquipmentNo)
        {
            string sql = $@"update wcs_shuttleinfo t 
                            t.modify_time = sysdate,
                            t.enter_leave_cs_time = sysdate
                            where t.current_status = 'In_ChargingStation' 
                            and t.belong_equipment_no = '{belongEquipmentNo}'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        //从堆垛机到货架
        public bool UpdateShuttleBoardInfo_ScToRack(string belongEquipmentNo, string updateLine, string updateLayer, string updateRackSide)
        {
            string sql = $@"update wcs_shuttleinfo t 
                            set t.current_status = 'In_Line',
                            t.current_location_line = '{updateLine}',
                            t.current_location_layer = '{updateLayer}',
                            t.current_rack_side = '{updateRackSide}',
                            t.modify_time = sysdate,
                            t.enter_leave_cs_time = sysdate
                            where t.current_status = 'In_Sc' 
                            and t.belong_equipment_no = '{belongEquipmentNo}'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        //从货架到堆垛机
        public bool UpdateShuttleBoardInfo_RackToSc(string belongEquipmentNo, string currentLine, string currentLayer, string currentRackSide)
        {
            string sql = $@"update wcs_shuttleinfo t 
                            set t.current_status = 'In_Sc',
                            t.modify_time = sysdate,
                            t.enter_leave_cs_time = sysdate
                            where t.current_location_line = '{currentLine}',
                            and t.current_location_layer = '{currentLayer}',
                            and t.current_rack_side = '{currentRackSide}',
                            and t.belong_equipment_no = '{belongEquipmentNo}'
                            where t.current_status = 'In_Line'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        //更新穿梭板信息
        public bool UpdateShuttleInfo(WCS_SHUTTLEINFO ws)
        {
            string sql = $@" update wcs_shuttleinfo t
                             set t.current_location_line = '{ws.CURRENT_LOCATION_LINE}',
                                 t.current_location_layer = '{ws.CURRENT_LOCATION_LAYER}', 
                                 t.current_rack_side = '{ws.CURRENT_RACK_SIDE}',
                                 t.current_status = '{ws.CURRENT_STATUS}',
                                 t.enter_leave_cs_time = to_date('{ws.ENTER_LEAVE_CS_TIME}','yyyy/mm/dd hh24:mi:ss')
                                 where t.t_id = '{ws.T_ID}'"
                                 ;
            return helper.ExecuteNonQueryBySql(sql);
        }

        public bool UpdateAGVIndexById(string id, string index, string carNO)
        {
            string sql = $@"Update WCS_SUBTASKINFO set 
                            AGVINDEX= '{index}',
                            AGVCARNO= '{carNO}'
                            where t_id = '{id}'";
            return helper.ExecuteNonQueryBySql(sql);
        }


        public DataTable GetAllShuttleInfo(string belongEquipmentNo)
        {
            string sql = $@"select t.* from wcs_shuttleinfo t
                            where t.belong_equipment_no = '{belongEquipmentNo}'";
            return helper.ExecuteQueryBySql(sql);
        }

        public DataTable GetAllShuttleInfo()
        {
            string sql = $@"  
                             select t.*, case t.current_status when 'In_Sc' then '在堆垛机上' when 'In_Line' then '在货位中' else '在充电站' end current_status_des,
                             case t.current_rack_side when 'Left' then '左侧' when 'Right' then '右侧' else '置空' end current_rack_side_des
                             from wcs_shuttleinfo t";
            return helper.ExecuteQueryBySql(sql);
        }

        public DataTable GetScLineTask(string scNo, string line, string layer, string rackSide)
        {
            DataTable dt = new DataTable();
            return dt;
        }

        #region 插入一条穿梭板记录
        //public bool InsertShuttleInfoForShuttleBoard(Wcs_ShuttleInfo shuttleInfo)
        //{
        //    string sql = $@"insert into  WCS_SHUTTLEINFO
        //                    (
        //                      t_id,
        //                      warehouse_id,
        //                      equipment_no,
        //                      belong_equipment_no,
        //                      equipment_type ,
        //                      current_taskno,
        //                      current_location_line ,
        //                      current_relation_equipment_no ,
        //                      current_status,
        //                      equipment_status ,
        //                      create_time ,
        //                      modify_time,
        //                      current_location_layer
        //                    )values
        //                    (
        //                    '{shuttleInfo.t_id}',
        //                    '{shuttleInfo.warehouse_id}',
        //                    '{shuttleInfo.equipment_no}',
        //                    '{shuttleInfo.belong_equipment_no}',
        //                    '{shuttleInfo.equipment_type}',
        //                    '{shuttleInfo.current_taskno}',
        //                    '{shuttleInfo.current_location_line}',
        //                    '{shuttleInfo.current_relation_equipment_no}',
        //                    '{shuttleInfo.current_status}',
        //                    '{shuttleInfo.equipment_status}',
        //                    sysdate,
        //                    sysdate,
        //                     '{shuttleInfo.current_location_layer}'
        //                    )";
        //    return DataAccessFactory.Instance.ExecuteNonQuery(sql, CommandType.Text, null) > 0;
        //}
        #endregion

        public bool DeleteShuttleInfoForShuttelBoard(WCS_SHUTTLEINFO shuttleInfo)
        {
            string sql = $@"delete wcs_shuttleinfo t 
                            where t.belong_equipment_no = '{shuttleInfo.BELONG_EQUIPMENT_NO}'
                            and t.equipment_type = '{shuttleInfo.EQUIPMENT_TYPE}'
                            and t.current_location_line = '{shuttleInfo.CURRENT_LOCATION_LINE}'
                            and t.current_location_layer = '{shuttleInfo.CURRENT_LOCATION_LAYER}'";
            return helper.ExecuteNonQueryBySql(sql);
        }


        public bool UpdateAGVIndexByMagic(string index, string flag)
        {
            string sql = "update wcs_subtaskinfo set field5 = '" + flag + "' where warehouseid = '" + WareHouseID + "' and agvindex='" + index + "' and equipmentType = 'AGV' ";
            return helper.ExecuteNonQueryBySql(sql);
        }

        public DataTable GetNoExecuteSCTask()
        {
            string sql = $@"select ws.*,
                            wt.tasktype,
                            wt.wmstaskno,
                            decode(wt.tasktype,'TaskType_Move',0,'TaskType_OutBound',1,'TaskType_InBound',2) task_type_sequence
                            from wcs_subtaskinfo ws
                            inner join wcs_taskinfo wt on wt.taskno = ws.taskno
                            where ws.runstatus = '1'
                            and ws.taskstatus = '0'
                            and ws.equipmenttype = 'SC'
                            order by task_type_sequence asc,wt.wmstaskno asc";
            return helper.ExecuteQueryBySql(sql);
        }

        public bool IsExistSCPTask()
        {
            string sql = $@"select count(1) from wcs_subtaskinfo t
                    where t.tostation = 'R_201'
                    and t.equipmenttype = 'AGV'
                    and t.taskstatus = '2'";
            DataTable dt = helper.ExecuteQueryBySql(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }
            return Convert.ToInt32(dt.Rows[0][0].ToString()) > 0 ? true : false;
        }


        /// <summary>
        /// 是否存在更小货位的任务
        /// 11517011
        /// </summary>
        /// <returns></returns>
        public bool IsExistSmallerTask(string locationNumber)
        {
            string sql = $@"select count(1)
                            from wcs_subtaskinfo wt
                            where length(wt.tostation) = 8
                            and substr(wt.tostation,1,3) = '{locationNumber.Substring(0,3)}'
                            and to_number(substr(wt.tostation,4,2)) < to_number('{locationNumber.Substring(3,2)}')
                            and wt.equipmenttype = 'SC'";
            DataTable dt = helper.ExecuteQueryBySql(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }
            return Convert.ToInt32(dt.Rows[0][0].ToString()) > 0 ? true : false;
        }


        /// <summary>
        /// 获取出库目标地址
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutBoundTargetAddressForSC()
        {
            string sql = $@"
                            select distinct ws.tostation
                            from wcs_subtaskinfo ws
                            inner join wcs_taskinfo wt on wt.taskno = ws.taskno
                            where ws.runstatus = '1'
                            and ws.taskstatus = '0'
                            and ws.equipmenttype = 'SC'
                            and wt.tasktype = 'TaskType_OutBound'";
            return helper.ExecuteQueryBySql(sql);
        }


        /// <summary>
        /// 查询表数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public DataTable QueryTableData<T>(T model)
        {
            string sql = $@"select * from {model.GetType().Name} where 1 = 1";
            foreach (PropertyInfo item in model.GetType().GetProperties())
            {
                if (item.GetValue(model) != null )
                {
                    if (item.PropertyType.ToString().Contains("DateTime"))
                    {
                        string it = "";
                    }
                    sql += $@" and {item.Name} = '{item.GetValue(model)}'";
                }
            }
            return helper.ExecuteQueryBySql(sql);
        }

        /// <summary>
        /// 更新表数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereModel"></param>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        public bool UpdateTableData<T>(T whereModel, T updateModel)
        {
            string sql = $@"update {whereModel.GetType().Name} set ";
            foreach (PropertyInfo item in updateModel.GetType().GetProperties())
            {
                if (item.GetValue(updateModel) != null)
                {
                    if (item.PropertyType.FullName.Contains("DateTime"))
                    {
                        sql += $@" {item.Name} = to_date('{item.GetValue(updateModel)}','yyyy/mm/dd hh24:mi:ss'),";
                    }
                    else
                    {
                        sql += $@" {item.Name} = '{item.GetValue(updateModel)}',";
                    }
                }
            }
            sql = sql.TrimEnd(',');
            //条件
            sql += $@" where 1 = 1";
            foreach (PropertyInfo item in whereModel.GetType().GetProperties())
            {
                if (item.GetValue(whereModel) != null)
                {
                    if (item.PropertyType.FullName.Contains("DateTime"))
                    {
                        sql += $@" and {item.Name} = to_date('{item.GetValue(whereModel)}','yyyy/mm/dd hh24:mi:ss')";
                    }
                    else
                    {
                        sql += $@" and {item.Name} = '{item.GetValue(whereModel)}'";
                    }
                }
            }
            return helper.ExecuteNonQueryBySql(sql);
        }

        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertTableData<T>(T model)
        {
            string sql = $"insert into {model.GetType().Name} (";
            foreach (PropertyInfo item in model.GetType().GetProperties())
            {
                sql += $"{item.Name},";
            }
            sql = sql.Trim(',');
            sql += ") values (";
            foreach (PropertyInfo item in model.GetType().GetProperties())
            {
                if (item.PropertyType.FullName.Contains("DateTime"))
                {
                    sql += $"to_date('{item.GetValue(model)}','yyyy/mm/dd hh24:mi:ss'),";
                }
                else
                {
                    sql += $"'{item.GetValue(model)}',";
                }
            }
            sql = sql.Trim(',');
            sql += ")";
            return helper.ExecuteNonQueryBySql(sql);
        }

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteTableData<T>(T model)
        {
            string sql = $"delete from {model.GetType().Name} where 1 = 1";
            foreach (PropertyInfo item in model.GetType().GetProperties())
            {
                if (item.GetValue(model) != null)
                {
                    sql += $"and {item.Name} = '{item.GetValue(model)}'";
                }
            }
            return helper.ExecuteNonQueryBySql(sql);
        }

        public DataTable GetRecevieTask()
        {
            string sql = $@"select t.*,z.zone_code,z.zone_description
                            from wcs_receive_task t
                            left join wcs_zone z on t.zoneid = z.t_id";
            return helper.ExecuteQueryBySql(sql);
        }

        public DataTable GetSCInfo()
        {
            string sql = $@"select a.*,b.equipment_mode
                            from wcs_equipmentinfo a
                            inner join wcs_equipment b on a.equipmentnum = b.equipment_no
                            and equipmenttype = 'SC'";
            return helper.ExecuteQueryBySql(sql);
        }

        public bool ItemInventoryCompute()
        {
            try
            {
                OracleParameter[] param = new OracleParameter[] { };
                DataTable dt = wmsHelper.ExecuteByProcedure("P_AutoComputeInventory", param);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool PeiPanInventoryCompute()
        {
            try
            {
                OracleParameter[] param = new OracleParameter[] { };
                DataTable dt = wmsHelper.ExecuteByProcedure("P_AutoComputePeiPanInventory", param);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool UpdateAGVIndex(string key, string index, string taskStatus,string equipmentNumber)
        {
            string sql = $@"update wcs_subtaskinfo 
                            set agvindex = '{index}' 
                            where taskno = '{key}'
                            and equipmentType = 'AGV' 
                            and taskstatus = '{taskStatus}' 
                            and runstatus = '1'
                            and equipmentnumber = '{equipmentNumber}'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        public bool UpdateAGVCarNO(string index, string carNO, string taskStatus,string equipmentNumber)
        {
            string sql = $@"update wcs_subtaskinfo set agvcarno = '{carNO}' 
                            where agvindex= '{index}' 
                            and equipmentType = 'AGV' 
                            and taskstatus = '{taskStatus}' 
                            and runstatus = '1'
                            and equipmentnumber = '{equipmentNumber}'";
            return helper.ExecuteNonQueryBySql(sql);
        }
       

        /// <summary>
        /// 根据箱号获取货位信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public DataTable GetLocationInfo(string barcode)
        {
            string sql = $@"select z.zone_code,z.t_id zone_id, l.container_barcode,l.location_number, l.batch_no,i.item_code ,i.item_description,decode(l.location_state,'LocationState_Stored','已存储','LocationState_Out','待出库','LocationState_In','待入库') LocationStateDescription,l.inbound_date
                            from swms_wi_location l
                            inner join swms_wi_item i on i.t_id = l.item_id
                            inner join swms_wi_zone z on l.zone_id = z.t_id
                            and l.container_barcode is not null";
            if (!string.IsNullOrEmpty(barcode))
            {
                sql += $@" and container_barcode like '%{barcode}%'";
            }
            return helper.ExecuteQueryBySql(sql);
        }

        public string GetSystemErrorDescriptionByCode(string errorCode)
        {
            string sql = $@"select t.error_description 
                            from wcs_error_info t
                            where t.error_code like '%{errorCode}%'";
            DataTable dt = helper.ExecuteQueryBySql(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            return dt.Rows[0][0].ToString();
        }

        public string[] Wcs_LocationInfo_Update(string action_flag, string location_number, string item_code, string barcode, string batch_no, string firstin_time, string weight)
        {
            string[] result = new string[2];
            try
            {
                OracleParameter[] param = new OracleParameter[] {
                    new OracleParameter(@"i_warehouse_id", OracleDbType.Varchar2),
                    new OracleParameter(@"i_action_flag", OracleDbType.Varchar2,50),
                    new OracleParameter(@"i_item_code", OracleDbType.Varchar2, 50),
                    new OracleParameter(@"i_location_number", OracleDbType.Varchar2, 500),
                    new OracleParameter(@"i_barcode", OracleDbType.Varchar2, 50),
                    new OracleParameter(@"i_batch_no", OracleDbType.Varchar2, 500),
                    new OracleParameter(@"i_firtin_time", OracleDbType.Varchar2, 50),
                    new OracleParameter(@"i_weight", OracleDbType.Varchar2, 500),
                    new OracleParameter(@"o_Message", OracleDbType.Varchar2, 500),
                    new OracleParameter(@"o_Result", OracleDbType.Varchar2, 20),
            };
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = WareHouseID;
                param[1].Direction = ParameterDirection.Input;
                param[1].Value = action_flag;
                param[2].Direction = ParameterDirection.Input;
                param[2].Value = item_code;
                param[3].Direction = ParameterDirection.Input;
                param[3].Value = location_number;
                param[4].Direction = ParameterDirection.Input;
                param[4].Value = barcode;
                param[5].Direction = ParameterDirection.Input;
                param[5].Value = batch_no;
                param[6].Direction = ParameterDirection.Input;
                param[6].Value = firstin_time;
                param[7].Direction = ParameterDirection.Input;
                param[7].Value = weight;
                param[8].Direction = ParameterDirection.Output;
                param[8].Value = " ";
                param[9].Direction = ParameterDirection.Output;
                param[9].Value = 0;
                DataTable dt = helper.ExecuteByProcedure("WCS_Barcode_Update",param);
                result[0] = param[9].Value.ToString();
                result[1] = param[8].Value.ToString();
            }
            catch (Exception ex)
            {
                result[0] = "0";
                result[1] = ex.Message;
            }
            return result;

        }

        public DataTable GetAllLocationInfo(string location_number, string item_code, string barcode, string batch_no)
        {
            string sql = $@"select z.t_id zone_id,z.zone_code,z.zone_name,  l.location_number,l.container_barcode,i.item_code,i.item_name,l.batch_no, decode(l.location_state,'LocationState_Stored','已存储','LocationState_Out','待出库','LocationState_In','待入库','LocationState_Empty','空') location_state,decode(l.location_flag,'LocationFlag_Enabled','可用','禁用') location_flag,l.inbound_date
                            from swms_wi_location l 
                            left join swms_wi_item i on l.item_id = i.t_id
                            inner join swms_wi_zone z on z.t_id = l.zone_id
                            where 1=1 ";
            if (!string.IsNullOrEmpty(location_number)) sql += $" and l.location_number like '%{location_number}%'";
            if (!string.IsNullOrEmpty(item_code)) sql += $" and i.item_code like '%{item_code}%'";
            if (!string.IsNullOrEmpty(barcode)) sql += $" and l.container_barcode = '{barcode}'";
            if (!string.IsNullOrEmpty(batch_no)) sql += $" and l.batch_no like '%{batch_no}%'";
            return helper.ExecuteQueryBySql(sql);
        }



        public Dictionary<bool, string> UpdateLocationBarcode(string locationNumber, string zoneId, string barcode)
        {
            Dictionary<bool, string> dic = new Dictionary<bool, string>();
            try
            {
                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("i_WarehouseID",OracleDbType.Varchar2,100),
                new OracleParameter("i_ZoneID",OracleDbType.Varchar2,100),
                new OracleParameter("i_Barcode",OracleDbType.Varchar2,50),
                new OracleParameter("i_LocationNumber",OracleDbType.Varchar2,50),
                new OracleParameter("o_ReturnMessage",OracleDbType.Varchar2,500),
                new OracleParameter("o_Result",OracleDbType.Varchar2,10)
                };
                parameters[0].Direction = ParameterDirection.Input;
                parameters[0].Value = WareHouseID;
                parameters[1].Direction = ParameterDirection.Input;
                parameters[1].Value = zoneId;
                parameters[2].Direction = ParameterDirection.Input;
                parameters[2].Value = barcode;
                parameters[3].Direction = ParameterDirection.Input;
                parameters[3].Value = locationNumber;
                parameters[4].Direction = ParameterDirection.Output;
                parameters[4].Value = "";
                parameters[5].Direction = ParameterDirection.Output;
                parameters[5].Value = 0;
                DataTable dt = helper.ExecuteByProcedure("WCS_Barcode_Update", parameters); 
                if (parameters[5].Value.ToString() != "1")
                {
                    dic.Add(false, parameters[4].Value.ToString());
                    return dic;
                }
                dic.Add(true, "托盘号维护成功！");
                return dic;
            }
            catch (Exception ex)
            {
                dic.Add(false, ex.ToString());
                return dic;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] PickBoxCheck(string locationNumber)
        {
            string[] resultArray = new string[2];
            try
            {
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("i_WarehouseID",OracleDbType.Varchar2,50),
                    new OracleParameter("i_LocationNumber",OracleDbType.Varchar2,50),
                    new OracleParameter("o_Message",OracleDbType.Varchar2,1000),
                    new OracleParameter("o_Result",OracleDbType.Int32,50)
                };
                parameters[0].Direction = ParameterDirection.Input;
                parameters[0].Value = WareHouseID;
                parameters[1].Direction = ParameterDirection.Input;
                parameters[1].Value = locationNumber;
                parameters[2].Direction = ParameterDirection.Output;
                parameters[2].Value = "";
                parameters[3].Direction = ParameterDirection.Output;
                parameters[3].Value = 0;
                DataTable dt = helper.ExecuteByProcedure("WCS_PickBoxCheck", parameters);
                resultArray[0] = parameters[3].Value.ToString();
                resultArray[1] = parameters[2].Value.ToString();
                return resultArray;

            }
            catch (Exception ex)
            {
                resultArray[0] = "0";
                resultArray[1] = $"{ex.Message}{ex.StackTrace}";
                return resultArray;
            }
        }

        /// <summary>
        /// 入库分配最终地址
        /// </summary>
        /// <param name="locationNumber"></param>
        /// <returns></returns>
        public string[] AllocationFinalAddress_InBound(string agvCarNo, string agvIndex)
        {
            string[] resultArray = new string[3];
            try
            {
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("i_warehouse_id",OracleDbType.Varchar2,50),
                    new OracleParameter("i_agv_carno",OracleDbType.Varchar2,50),
                    new OracleParameter("i_agv_index",OracleDbType.Varchar2,1000),
                    new OracleParameter("o_result",OracleDbType.Varchar2,100),
                    new OracleParameter("o_message",OracleDbType.Varchar2,1000),
                    new OracleParameter("o_location_number",OracleDbType.Varchar2,100)
                };
                parameters[0].Direction = ParameterDirection.Input;
                parameters[0].Value = WareHouseID;

                parameters[1].Direction = ParameterDirection.Input;
                parameters[1].Value = agvCarNo;

                parameters[2].Direction = ParameterDirection.Input;
                parameters[2].Value = agvIndex;

                parameters[3].Direction = ParameterDirection.Output;
                parameters[3].Value = string.Empty;//result

                parameters[4].Direction = ParameterDirection.Output;
                parameters[4].Value = string.Empty;//message

                parameters[5].Direction = ParameterDirection.Output;
                parameters[5].Value = string.Empty;//location_number
                DataTable dt = helper.ExecuteByProcedure("wcs_inbound_finaladdress_update_floor02", parameters);
                resultArray[0] = parameters[3].Value.ToString();
                resultArray[1] = parameters[4].Value.ToString();
                resultArray[2] = parameters[5].Value.ToString();
                return resultArray;

            }
            catch (Exception ex)
            {
                resultArray[0] = "Failed";
                resultArray[1] = $"{ex.Message}{ex.StackTrace}";
                resultArray[2] = null;
                return resultArray;
            }
        }

        /// <summary>
        /// 出库分配最终地址
        /// </summary>
        /// <param name="locationNumber"></param>
        /// <returns></returns>
        public string[] AllocationFinalAddress_OutBound(string agvCarNo, string agvIndex)
        {
            string[] resultArray = new string[3];
            try
            {
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("i_warehouse_id",OracleDbType.Varchar2,50),
                    new OracleParameter("i_agv_carno",OracleDbType.Varchar2,50),
                    new OracleParameter("i_agv_index",OracleDbType.Varchar2,1000),
                    new OracleParameter("o_result",OracleDbType.Varchar2,100),
                    new OracleParameter("o_message",OracleDbType.Varchar2,1000),
                    new OracleParameter("o_location_number",OracleDbType.Varchar2,100)
                };
                parameters[0].Direction = ParameterDirection.Input;
                parameters[0].Value = WareHouseID;

                parameters[1].Direction = ParameterDirection.Input;
                parameters[1].Value = agvCarNo;

                parameters[2].Direction = ParameterDirection.Input;
                parameters[2].Value = agvIndex;

                parameters[3].Direction = ParameterDirection.Output;
                parameters[3].Value = string.Empty;//result

                parameters[4].Direction = ParameterDirection.Output;
                parameters[4].Value = string.Empty;//message

                parameters[5].Direction = ParameterDirection.Output;
                parameters[5].Value = string.Empty;//location_number
                DataTable dt = helper.ExecuteByProcedure("wcs_outbound_finaladdress_update_floor02", parameters);
                resultArray[0] = parameters[3].Value.ToString();
                resultArray[1] = parameters[4].Value.ToString();
                resultArray[2] = parameters[5].Value.ToString();
                return resultArray;

            }
            catch (Exception ex)
            {
                resultArray[0] = "Failed";
                resultArray[1] = $"{ex.Message}{ex.StackTrace}";
                resultArray[2] = null;
                return resultArray;
            }
        }


        /// <summary>
        /// 出库取货完成，清空货位
        /// </summary>
        /// <param name="locationNumber"></param>
        /// <returns></returns>
        public string[] Wcs_Clean_LocationInfo(string agvCarNo, string agvIndex)
        {
            string[] resultArray = new string[2];
            try
            {
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("i_warehouse_id",OracleDbType.Varchar2,50),
                    new OracleParameter("i_agv_carno",OracleDbType.Varchar2,50),
                    new OracleParameter("i_agv_index",OracleDbType.Varchar2,1000),
                    new OracleParameter("o_result",OracleDbType.Varchar2,100),
                    new OracleParameter("o_message",OracleDbType.Varchar2,1000),
                };
                parameters[0].Direction = ParameterDirection.Input;
                parameters[0].Value = WareHouseID;

                parameters[1].Direction = ParameterDirection.Input;
                parameters[1].Value = agvCarNo;

                parameters[2].Direction = ParameterDirection.Input;
                parameters[2].Value = agvIndex;

                parameters[3].Direction = ParameterDirection.Output;
                parameters[3].Value = string.Empty;//result

                parameters[4].Direction = ParameterDirection.Output;
                parameters[4].Value = string.Empty;//message
                DataTable dt = helper.ExecuteByProcedure("wcs_clean_agvpick", parameters);
                resultArray[0] = parameters[3].Value.ToString();
                resultArray[1] = parameters[4].Value.ToString();
                return resultArray;

            }
            catch (Exception ex)
            {
                resultArray[0] = "Failed";
                resultArray[1] = $"{ex.Message}{ex.StackTrace}";
                resultArray[2] = null;
                return resultArray;
            }
        }

        public bool UpdateAGVMagic(string index, string carNO, string magic)
        {
                string sql = $@"Update WCS_SUBTASKINFO
                                set agvmagic =  '{magic}'
                                where agvindex= '{index}'
                                and agvcarno= '{carNO}'
                                and taskstatus = '2'";
                return helper.ExecuteNonQueryBySql(sql);
        }




        public DataTable GetSubTaskInfo(string equipmentNumber)
        {
            string sql = $@"select 
                            ws.taskno,
                            case ws.taskstatus
                            when '1' then '已完成'
                            when '2' then '执行中'
                            when '0' then '未开始'
                            else ws.taskstatus end taskstatus,
                            case ws.runstatus 
                            when '0' then '不可执行'
                            when '1' then '可执行'
                            else ws.runstatus
                            end runstatus,
                            ws.fromstation,
                            ws.tostation,
                            wt.wmstaskno,
                            case wt.tasktype
                            when 'TaskType_OutBound' then
                            '出库任务'
                            when 'TaskType_InBound' then
                            '入库任务'
                            when 'TaskType_Move' then
                            '移库任务'
                            when 'Task_CallTerminal_ReturnBadPallet' then
                            '成品转移任务'
                            else
                            tasktype
                            end tasktype,
                            decode(ws.fromstation,'R_201','3','R_202','2','R_203','1',4)  rotation_flag,
                            ws.barcode,
                            ws.agvikey,
                            ws.agvmagic,
                            ws.agvcarno,
                            ws.agvindex,
                            ws.equipmentnumber，
                            ws.create_date,
                            ws.modify_date
                            from wcs_subtaskinfo ws
                            left join wcs_taskinfo wt on ws.taskno = wt.taskno
                            where ws.equipmenttype = 'AGV'
                            and ws.equipmentnumber = '{equipmentNumber}'
                           order  by rotation_flag asc,  wt.wmstaskno asc";
            return helper.ExecuteQueryBySql(sql);
        }

        public string GetAgvAddressByWcsAddress(string wcsAddress)
        {
            string sql = $@"select t.agv_address  from WCS_AGV_ADDRESS t
                            where t.wcs_address = '{wcsAddress}' ";
            DataTable dt = helper.ExecuteQueryBySql(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            return dt.Rows[0][0].ToString();
        }

        public string GetAddressTypeByAgvAddress(string agvAddress)
        {
            string sql = $@"select t.address_type  from WCS_AGV_ADDRESS t
                            where t.agv_address = '{agvAddress}' ";
            DataTable dt = helper.ExecuteQueryBySql(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            return dt.Rows[0][0].ToString();
        }


        /// <summary>
        /// 插入wms任务
        /// </summary>
        /// <returns></returns>
        public bool InsertReceiveTask(WCS_RECEIVE_TASK info)
        {
            string sql = $@"insert into wcs_receive_task(
                            T_ID,
                            WAREHOUSEID,
                            ZONEID,
                            TASKNUM,
                            TASKSTATUS,
                            TASKTYPE,
                            CONTAINERBARCODE,
                            WAVE_ID,
                            GROUPNUM,
                            SEQNUM,
                            SOURCEADDRESS,
                            TARGETADDRESS,
                            EQUIPMENTNUM,
                            LEDTEXT,
                            MIDDLESTATION,
                            LGVEXIST
                            ) values(
                            '{StringRandomUtil.Guid32Bit()}',
                            '{info.WAREHOUSEID}',
                            '{info.ZONEID}',
                            '{info.TASKNUM}',
                            '{info.TASKSTATUS}',
                            '{info.TASKTYPE}',
                            '{info.CONTAINERBARCODE}',
                            '{info.WAVE_ID}',
                            '{info.GROUPNUM}',
                            '{info.SEQNUM}',
                            '{info.SOURCEADDRESS}',
                            '{info.TARGETADDRESS}',
                            '{info.EQUIPMENTNUM}',
                            '{info.LEDTEXT}',
                            '{info.MIDDLESATION}',
                            '{info.LGVEXIST}'
                            )";
            return helper.ExecuteNonQueryBySql(sql);
        }

        /// <summary>
        /// 拆分WMS任务
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="zoneId"></param>
        /// <param name="taskNumber"></param>
        /// <returns></returns>
        public string[] SplitReceiveTask(string warehouseId,string zoneId, string taskNumber)
        {
            string[] result = new string[2];
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("i_warehouse_id",OracleDbType.Varchar2,100),
                new OracleParameter("i_zone_id",OracleDbType.Varchar2,100),
                new OracleParameter("i_task_number",OracleDbType.Varchar2,100),
                new OracleParameter("o_message",OracleDbType.NVarchar2,500),
                new OracleParameter("o_result",OracleDbType.Int32,2),
            };
            parameters[0].Value = warehouseId;
            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Value = zoneId;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Value = taskNumber;
            parameters[2].Direction = ParameterDirection.Input;
            parameters[3].Value = " ";
            parameters[3].Direction = ParameterDirection.Output;
            parameters[4].Value = 0;
            parameters[4].Direction = ParameterDirection.Output;
            helper.ExecuteByProcedure("wcs_split_task_new", parameters);
            result[0] = parameters[3].Value.ToString();
            result[1] = parameters[4].Value.ToString();
            return result;
        }
 

        public string[] GetInBoundZone(string warehouseId, string containerBarcode)
        {
            string[] result = new string[3];
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("i_warehouse_id",OracleDbType.Varchar2,100),
                new OracleParameter("i_container_barcode",OracleDbType.Varchar2,100),
                new OracleParameter("o_zone_code",OracleDbType.Varchar2,100),
                new OracleParameter("o_message",OracleDbType.NVarchar2,500),
                new OracleParameter("o_result",OracleDbType.Int32,2),
            };
            parameters[0].Value = warehouseId;
            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Value = containerBarcode;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Value = "0";
            parameters[2].Direction = ParameterDirection.Output;
            parameters[3].Value = "0";
            parameters[3].Direction = ParameterDirection.Output;
            parameters[4].Value = 0;
            parameters[4].Direction = ParameterDirection.Output;
            helper.ExecuteByProcedure("wcs_inbound_zone_select", parameters);
            result[0] = parameters[2].Value.ToString();//区域
            result[1] = parameters[3].Value.ToString();//描述
            result[2] = parameters[4].Value.ToString();//结果
            return result;
        }

        public void Interface_UpdateEquipmentStatus(UpdateEquipmentStatusModel model)
        {
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("i_warehouse_id",OracleDbType.Varchar2,100),
                new OracleParameter("i_zone_id",OracleDbType.Varchar2,100),
                new OracleParameter("i_equipment_no",OracleDbType.Varchar2,100),
                new OracleParameter("i_equipment_status",OracleDbType.Varchar2,100),
                new OracleParameter("i_error_code",OracleDbType.Varchar2,100),
                new OracleParameter("i_error_description",OracleDbType.NVarchar2,500),
                new OracleParameter("o_result",OracleDbType.Int32,2),
            };
            parameters[0].Value = model.WarehouseId;
            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Value = model.ZoneId;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Value = model.EquipmentNo;
            parameters[2].Direction = ParameterDirection.Input;
            parameters[3].Value = model.EquipmentStatus;
            parameters[3].Direction = ParameterDirection.Input;
            parameters[4].Value = model.ErrorCode;
            parameters[4].Direction = ParameterDirection.Input;
            parameters[5].Value = model.ErrorDescription;
            parameters[5].Direction = ParameterDirection.Input;
            parameters[6].Value = 0;
            parameters[6].Direction = ParameterDirection.Output;
            helper.ExecuteByProcedure("interface_update_equipment_status", parameters);
        }

        /// <summary>
        /// 得到子任务的目标地址
        /// </summary>
        /// <param name="taskNo"></param>
        /// <param name="equipmentType"></param>
        /// <param name="fromStation"></param>
        /// <returns></returns>
        public string GetToStation(string taskNo,string equipmentType,string fromStation)
        {
            string sql = $@"select tostation 
                            from wcs_subtaskinfo 
                            where taskno = {taskNo}
                            and equipmenttype = '{equipmentType}'
                            and fromstation = '{fromStation}'";
            DataTable dt = helper.ExecuteQueryBySql(sql);
            if (dt == null || dt.Rows.Count != 1)
            {
                return string.Empty;
            }
            return dt.Rows[0][0].ToString();
        }
        /// <summary>
        /// 根据WMS任务号得到WCS任务号
        /// </summary>
        /// <param name="wmsTaskNo"></param>
        /// <returns></returns>
        public string GetWcsTaskNoByWmsTaskNo(string wmsTaskNo)
        {
            string sql = $@"select t.taskno from wcs_taskinfo t
                            where t.wmstaskno = '{wmsTaskNo}'";
            DataTable dt = helper.ExecuteQueryBySql(sql);
            if (dt == null || dt.Rows.Count != 1)
            {
                return string.Empty;
            }
            return dt.Rows[0][0].ToString();
        }

        /// <summary>
        /// 根据WCS任务号得到WMS任务号
        /// </summary>
        /// <param name="wcsTaskNo"></param>
        /// <returns></returns>
        
        public string GetWmsTaskNoByWcsTaskNo(string wcsTaskNo)
        {
            string sql = $@"select t.wmstaskno from wcs_taskinfo t
                            where t.taskno = {wcsTaskNo}";
            DataTable dt = helper.ExecuteQueryBySql(sql);
            if (dt == null || dt.Rows.Count != 1)
            {
                return string.Empty;
            }
            return dt.Rows[0][0].ToString();
        }

        public DataTable GetTotalTask(WCS_SUBTASKINFO taskInfo)
        {
            string sql = $@"select 
                            wt.wmstaskno,
                            wt.taskno,
                            wt.tasktype,
                            wt.fromstation mainfromstation,
                            wt.tostation maintostation,
                            wt.barcode,
                            ws.fromstation subfromstation,
                            ws.tostation subfromstation,
                            ws.equipmenttype,
                            ws.equipmentnumber
                            from wcs_taskinfo wt
                            inner join wcs_subtaskinfo ws
                            on wt.taskno = ws.taskno 
                            where 1 = 1 ";
            foreach (PropertyInfo item in taskInfo.GetType().GetProperties())
            {
                if (item.GetValue(taskInfo) != null)
                {
                    sql += $@" and ws.{item.Name} = '{item.GetValue(taskInfo)}'";
                }
            }
            sql += $" order by wt.createtime desc";
            return helper.ExecuteQueryBySql(sql);
        }

        /// <summary>
        /// 创建WMS任务号
        /// </summary>
        /// <param name="billName"></param>
        /// <returns></returns>
        public string CreateWmsTaskNo(string billName)
        {
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("i_WarehouseID",OracleDbType.Varchar2,200),
                new OracleParameter("i_BillName",OracleDbType.Varchar2,200),
                new OracleParameter("o_BillNumber",OracleDbType.Varchar2,200),
            };
            parameters[0].Value = WareHouseID;
            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Value = billName;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Value = string.Empty;
            parameters[2].Direction = ParameterDirection.Output;
            helper.ExecuteByProcedure("M_GetBillNumber", parameters);
            return parameters[2].Value.ToString();
        }

       
        //获取基础物料信息
        public DataTable GetBaseItemInfo()
        {
            string sql = $@"select i.item_code,i.item_name  from swms_wi_item i ";
            return helper.ExecuteQueryBySql(sql);
        }


        public bool Wcs_DeleteItemInfo(string itemId)
        {
            string sql = $"delete from swms_wi_item i where i.t_id = '{itemId}'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        public bool Wcs_UpdateItemInfo(string itemId, string itemName, string exteriorCode, string boxType)
        {
            string sql = $@"update swms_wi_item i 
                            set i.item_name = '{itemName}',i.exterior_code = '{exteriorCode}',i.box_type = '{boxType}'
                            where i.t_id = '{itemId}'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        public DataTable GetItemInfo(string itemName)
        {
            string sql = $@"select i.t_id,
                            i.item_code,
                            i.exterior_code,
                            i.item_name,
                            i.box_type,
                            i.create_date 
                            from swms_wi_item  i
                            where 1=1 ";
            if (!string.IsNullOrEmpty(itemName)) sql += $" and i.item_name like '%{itemName}%'";
            sql += $" order  by i.item_name asc";
            return helper.ExecuteQueryBySql(sql);
        }

        public DataTable GetOneCodeInfo(string startTime, string endTime, string barcode, string oneCode, string itemCode)
        {
            string sql = $@"
                            select h.create_date, h.container_barcode,d.item_barcode, i.item_code,i.exterior_code,i.item_name,d.qty,h.source_text
                            from swms_m_container_h h
                            inner join swms_m_container_d d on h.t_id = d.container_id
                            left join swms_wi_item i on i.t_id = d.item_id
                            where 1=1";
            if (!string.IsNullOrEmpty(startTime)) sql += $@" and h.create_date >=  to_date('{startTime}','yyyy/mm/dd hh24:mi:ss')";
            if (!string.IsNullOrEmpty(endTime)) sql += $@" and h.create_date <=  to_date('{endTime}','yyyy/mm/dd hh24:mi:ss')";
            if (!string.IsNullOrEmpty(barcode)) sql += $@" and h.container_barcode like '%{barcode}%'";
            if (!string.IsNullOrEmpty(oneCode)) sql += $@" and d.item_barcode like '%{oneCode}%'";
            if (!string.IsNullOrEmpty(itemCode)) sql += $@" and i.item_code like '%{itemCode}%'";
            return helper.ExecuteQueryBySql(sql);
        }

        public DataTable GetWcsOneCodeToItemCode(string startTime, string endTime, string oneCode, bool isChecked)
        {
            string sql = $@"
                            select * from WCS_ONECODETOITEMCODE t
                            where 1=1";
            if (!string.IsNullOrEmpty(startTime)) sql += $@" and t.createtime >=  to_date('{startTime}','yyyy/mm/dd hh24:mi:ss')";
            if (!string.IsNullOrEmpty(endTime)) sql += $@" and t.createtime <=  to_date('{endTime}','yyyy/mm/dd hh24:mi:ss')";
            if (isChecked)
            {
                sql += $@" and (t.onecode is null or length(t.onecode) not in (27,32))";
            }
            else
            {
                if (!string.IsNullOrEmpty(oneCode)) sql += $@" and t.onecode like '%{oneCode}%'";
            }
            return helper.ExecuteQueryBySql(sql);
        }

        public DataTable GetInSortingReport(string startTime, string endTime, string scanCode, string plcDestination, string inSortingDescription)
        {
            string sql = $@"
                            select * from WCS_INSORTINGREPORT t 
                            where 1=1";
            if (!string.IsNullOrEmpty(startTime)) sql += $@" and t.create_time >=  to_date('{startTime}','yyyy/mm/dd hh24:mi:ss')";
            if (!string.IsNullOrEmpty(endTime)) sql += $@" and t.create_time <=  to_date('{endTime}','yyyy/mm/dd hh24:mi:ss')";
            if (!string.IsNullOrEmpty(scanCode)) sql += $@" and t.scan_code like '%{scanCode}%'";
            if (!string.IsNullOrEmpty(plcDestination)) sql += $@" and t.plc_destination = '{plcDestination}'";
            if (!string.IsNullOrEmpty(inSortingDescription)) sql += $@" and t.exception_description like '%{inSortingDescription}%'";
            return helper.ExecuteQueryBySql(sql);
        }

        public DataTable GetInBoundQty(string startTime, string endTime)
        {
            string sql = $@"
                            select i.item_code,i.exterior_code,i.item_name,count(1) Qty
                            from WCS_ONECODETOITEMCODE h
                            inner join swms_wi_item i on h.itemid = i.t_id 
                            where 1=1";
            if (!string.IsNullOrEmpty(startTime)) sql += $@" and h.createtime >=  to_date('{startTime}','yyyy/mm/dd hh24:mi:ss')";
            if (!string.IsNullOrEmpty(endTime)) sql += $@" and h.createtime <=  to_date('{endTime}','yyyy/mm/dd hh24:mi:ss')";
            sql += " group by i.item_code,i.exterior_code,i.item_name";
            return helper.ExecuteQueryBySql(sql);
        }




        public string GetDestinationDock(string scanCode, string itemCode, string fxNO, ref DataTable result)
        {
            string message = string.Empty;
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("i_Warehouse_Id",OracleDbType.Varchar2,50),
                new OracleParameter("i_Scan_Code",OracleDbType.Varchar2,200),
                new OracleParameter("i_Item_Code",OracleDbType.Varchar2,200),
                new OracleParameter("i_Fx_No",OracleDbType.Varchar2,200),
                new OracleParameter("o_Message",OracleDbType.Varchar2,500),
                new OracleParameter("o_Result",OracleDbType.RefCursor),
            };
            parameters[0].Value = WareHouseID;
            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Value = scanCode;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Value = itemCode;
            parameters[2].Direction = ParameterDirection.Input;
            parameters[3].Value = fxNO;
            parameters[3].Direction = ParameterDirection.Input;
            parameters[4].Value = "";
            parameters[4].Direction = ParameterDirection.Output;
            parameters[5].Direction = ParameterDirection.Output;
            result = helper.ExecuteByProcedure("WCS_GetInDock", parameters);
            message = parameters[4].Value.ToString();
            return message;
        }



        public bool Wcs_SavePalletInfo(string stationNO, string palletCode, string oneCodeCollection)
        {
            string message = string.Empty;
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("i_Warehouse_Id",OracleDbType.Varchar2,50),
                new OracleParameter("i_Station_No",OracleDbType.Varchar2,200),
                new OracleParameter("i_Pallet_Code",OracleDbType.Varchar2,200),
                new OracleParameter("i_OneCode_List",OracleDbType.Varchar2,2000),
                new OracleParameter("o_Result",OracleDbType.Int32,50),
            };
            parameters[0].Value = WareHouseID;
            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Value = stationNO;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Value = palletCode;
            parameters[2].Direction = ParameterDirection.Input;
            parameters[3].Value = oneCodeCollection;
            parameters[3].Direction = ParameterDirection.Input;
            parameters[4].Value = 0;
            parameters[4].Direction = ParameterDirection.Output;
            DataTable dt = helper.ExecuteByProcedure("WCS_SavePalletInfo", parameters);
            return parameters[4].Value.ToString().Equals("1");
        }

        /// <summary>
        /// 获取所有道口信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllDockInfo()
        {
                string sql = $@"select e.equipment_number DOCKNO,l.stationno, i.item_code itemCode,i.item_name itemName, i.exterior_code,i.box_type,to_char(i.each_pallet_fillqty) each_pallet_fillqty,
                            decode(canedit,0,'清道中',1,'设道完成',2,'清道完成') ISWCSCLEAN,decode(l.plcresponse,'0','未响应',1,'成功',2,'失败') plcresponse,l.floor 
                            from LANEWAYSELECT l
                            inner join swms_wi_item i on l.itemid = i.t_id
                            inner join swms_wi_equipment e on e.t_id = l.equipmentid
                            order by  to_number(e.equipment_number)  asc";
                return helper.ExecuteQueryBySql(sql);
            
        }



        public DataTable GetAllBrandInfo()
        {
                string sql = $@"select i.item_code,i.item_name
                            from swms_wi_item i
                            where i.item_code <> '99999999'";
                return helper.ExecuteQueryBySql(sql);
        }




        public DataTable Get_Wcs_JKInterface(string stationNO)
        {
                string sql = $@"select t.* from WCS_JKINTERFACE t where t.stationnumber = '{stationNO}'";
                return helper.ExecuteQueryBySql(sql);
        }

        /// <summary>
        /// 获取所有参数配置信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetWcsParamConfig(string paramName)
        {
            string sql = $@" select t.* from wcs_parameter_config t where 1 = 1";
            if (!string.IsNullOrEmpty(paramName))
            {
                sql += $"and t.param_name like '%{paramName}%'";
            }
            return helper.ExecuteQueryBySql(sql);
        }

        /// <summary>
        /// 根据参数编码，获取参数值
        /// </summary>
        /// <param name="paramCode"></param>
        /// <returns></returns>
        public string GetWcsParamConfigValueByCode(string paramCode)
        {
                string sql = $@" select t.param_value from wcs_parameter_config t where t.param_code = '{paramCode}'";
                return helper.ExecuteQueryBySql(sql).Rows?[0]["param_value"].ToString();
           
        }


        public bool UpdateWcsParamConfig(string paramCode, string paramValue)
        {
            string sql = $@"update wcs_parameter_config i 
                            set i.param_value = '{paramValue}'
                            where i.param_code = '{paramCode}'";
            return helper.ExecuteNonQueryBySql(sql);
        }


        public DataTable GetSomeBrandInfo(string itemCode)
        {
                string sql = $@"select i.item_code,i.item_name
                            from swms_wi_item i
                            where i.item_code <> '99999999'
                            and i.item_code like '%{itemCode}%'";
                return helper.ExecuteQueryBySql(sql);
        }
        public DataTable GetItemBoxTypeInfo(string itemCode)
        {
                string sql = $@"select i.*
                            from swms_wi_item i
                            where i.item_code <> '99999999'
                            and i.item_code = '{itemCode}'";
                return helper.ExecuteQueryBySql(sql);
        }

        public bool IsExistUser(string userID, string password)
        {
                string sql = $@"select count(1)  from wcs_user t
                           where t.user_id = '{userID}' and t.user_password = '{password}'";
                return Convert.ToInt32(helper.ExecuteQueryBySql(sql).Rows[0][0].ToString()) > 0;
        }

        public DataTable GetBrandInfoByStation(string stationNO)
        {
                string sql = $@"select i.item_code,i.item_name,l.stationno,e.equipment_number,l.plcresponse,l.canedit 
                                from lanewayselect l
                                inner join swms_wi_item i on l.itemid = i.t_id
                                left join swms_wi_equipment e on e.t_id = l.equipmentid
                                where l.stationno = '{stationNO}'";
                return helper.ExecuteQueryBySql(sql);
        }

        public DataTable GetBrandInfoByDock(string dockNO)
        {
                string sql = $@"select i.item_code,i.item_name,i.box_type,i.each_pallet_fillqty,l.stationno,e.equipment_number,l.plcresponse,l.canedit  
                                from lanewayselect l
                                inner join swms_wi_item i on l.itemid = i.t_id
                                inner join swms_wi_equipment e on e.t_id = l.equipmentid
                                where e.equipment_number = '{dockNO}'";
                return helper.ExecuteQueryBySql(sql);
        }

        public DataTable GetAllSetDockFloor()
        {
            
                string sql = $@"select distinct floor  from lanewayselect t
                                where t.canedit = 1";
                return helper.ExecuteQueryBySql(sql);
        }

        public string GetSwitchFloorValue()
        {
                string sql = $@"select t.wcs_equipment_mode  from swms_wi_equipment t
                                where t.equipment_number = 'FJX_IN'";
                return helper.ExecuteQueryBySql(sql)?.Rows[0][0].ToString();
        }

        public bool SetDockBrand(string dockNO, string itemCode)
        {
                string sql = $@"update lanewayselect t
                                set t.itemid = (select  t_id from swms_wi_item where item_code = '{itemCode}'),
                                t.itemcode = '{itemCode}',
                                t.canedit = 1,
                                t.plcresponse = '0',
                                t.changetime = sysdate
                                where equipmentid = (select t_id  from swms_wi_equipment 
                                where type_code = 'Equipment_InDock'
                                and equipment_number = '{dockNO}')";
                return helper.ExecuteNonQueryBySql(sql);
        }


        public bool SetCleanFlag(string dockNO, int flag)
        {
            string sql = $@"update lanewayselect l
                            set l.canedit = {flag}
                            where l.equipmentid = (select e.t_id from swms_wi_equipment e where e.equipment_number = '{dockNO}')";
            return helper.ExecuteNonQueryBySql(sql);
        }


        public bool IsExistsExceptionBrand(string itemCode)
        {
            string sql = $@"select  count(1)
                            from lanewayselect l
                            inner join swms_wi_item i on l.itemid = i.t_id
                            where i.exterior_code in (select exterior_code from swms_wi_item where item_code = '{itemCode}')
                            and l.canedit = '1'
                            and i.item_code <> '{itemCode}'";
            DataTable dt = helper.ExecuteQueryBySql(sql);
            if (dt == null || dt.Rows.Count == 0 || dt.Rows[0][0].ToString().Equals("0"))
            {
                return false;
            }
            return true;
        }

        public bool SetPLCFlag(string dockNO, int flag)
        {
            string sql = $@"update lanewayselect l
                            set l.plcresponse = '{flag.ToString()}'
                            where l.equipmentid = (select e.t_id from swms_wi_equipment e where e.equipment_number = '{dockNO}')";
            return helper.ExecuteNonQueryBySql(sql);
        }
        public bool SetInSortingFloor(string flag)
        {
            string sql = $@"update swms_wi_equipment t
                            set t.wcs_equipment_mode = '{flag}'
                            where t.equipment_number = 'FJX_IN'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        public void Wcs_SaveInSortingReport(string scanCode, string wcsDestination, string plcDestination, string exceptionDescription)
        {
                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("i_warehouse_id",OracleDbType.Varchar2,100),
                new OracleParameter("i_scan_code",OracleDbType.Varchar2,100),
                new OracleParameter("i_wcs_destination",OracleDbType.Varchar2,100),
                new OracleParameter("i_plc_destination",OracleDbType.Varchar2,100),
                new OracleParameter("i_exception_description",OracleDbType.Varchar2,100),
                new OracleParameter("o_Result",OracleDbType.Varchar2,100)
                };
                parameters[0].Direction = ParameterDirection.Input;
                parameters[0].Value = WareHouseID;
                parameters[1].Direction = ParameterDirection.Input;
                parameters[1].Value = scanCode;
                parameters[2].Direction = ParameterDirection.Input;
                parameters[2].Value = wcsDestination;
                parameters[3].Direction = ParameterDirection.Input;
                parameters[3].Value = plcDestination;
                parameters[4].Direction = ParameterDirection.Input;
                parameters[4].Value = exceptionDescription;
                parameters[5].Direction = ParameterDirection.Output;
                parameters[5].Value = "";
                DataTable dt = helper.ExecuteByProcedure("wcs_save_insorting_report", parameters);
        }

        public DataTable GetSystemLog(string startTime,string endTime,string keyword)
        {
            string sql = $@"select t.createtime,t.logtext,t.commonname
                            from wcs_eventlog t 
                            where ";
            if (startTime != null && !string.IsNullOrEmpty(startTime.Trim()))
            {
                sql += $"  t.createtime >= to_date('{startTime.Trim()}','yyyy/mm/dd hh24:mi:ss')";
            }
            if (endTime != null && !string.IsNullOrEmpty(endTime.Trim()))
            {
                sql += $" and t.createtime <= to_date('{endTime.Trim()}','yyyy/mm/dd hh24:mi:ss')";
            }
            if (keyword != null && !string.IsNullOrEmpty(keyword.Trim()))
            {
                sql += $" and t.logtext like '%{keyword.Trim()}%'";
            }
            //sql += $" order by t.createtime desc";
            return helper.ExecuteQueryBySql(sql);
        }
       
        public DataTable GetEventLog(string startTime, string endTime, string keyword, string containerBarcode, string taskNo, bool IsException)
        {

            string sql = $"select t.CREATETIME,t.logtext from wcs_eventlog t where 1 = 1 ";

            if (startTime != null && !string.IsNullOrEmpty(startTime.Trim()))
            {
                sql += $" and t.createtime >= to_date('{startTime.Trim()}','yyyy/mm/dd hh24:mi:ss')";
            }
            if (endTime != null && !string.IsNullOrEmpty(endTime.Trim()))
            {
                sql += $" and t.createtime <= to_date('{endTime.Trim()}','yyyy/mm/dd hh24:mi:ss')";
            }
            if (keyword != null && !string.IsNullOrEmpty(keyword.Trim()))
            {
                sql += $" and t.logtext like '%{keyword.Trim()}%'";
            }
            if (containerBarcode != null && !string.IsNullOrEmpty(containerBarcode.Trim()))
            {
                sql += $" and t.logtext like '%{containerBarcode.Trim()}%'";
            }
            if (taskNo != null && !string.IsNullOrEmpty(taskNo.Trim()))
            {
                sql += $" and t.logtext like '%{taskNo.Trim()}%'";
            }
            if (IsException)
            {
                sql += $" and t.logtext like '%失败%'";
            }
            sql += $"order by t.createtime desc";
            return helper.ExecuteQueryBySql(sql);
        }

        //判断是否有正在执行的任务
        public bool IsScExecuting(string scNO)
        {
            string sql = $@"select count(1)
                            from wcs_subtaskinfo t
                            where t.equipmenttype = 'SC'
                            and t.equipmentnumber = '{scNO}'
                            and t.taskstatus = '2'";
           return Convert.ToInt32(helper.ExecuteQueryBySql(sql).Rows[0][0]) > 0;
        }

        //拿到正在执行的任务
        public DataTable GetScExecuting(string scNO)
        {
            string sql = $@"select *
                            from wcs_subtaskinfo t
                            where t.equipmenttype = 'SC'
                            and t.equipmentnumber = '{scNO}'
                            and t.taskstatus = '2'";
            return helper.ExecuteQueryBySql(sql);
        }

        //拿到所有执行中的堆垛机任务
        public DataTable GetAllScExecuting()
        {
            string sql = $@"select *
                            from wcs_subtaskinfo t
                            where t.equipmenttype = 'SC'                            
                            and t.taskstatus = '2'";
            return helper.ExecuteQueryBySql(sql);
        }



        public bool UpdateTaskStatusByTaskno(string taskno, string fromstation, string tostation)
        {
            string sql = "update wcs_subtaskinfo set taskstatus='2' where warehouseid = '" + WareHouseID + "' and taskno='" + taskno + "' and equipmentType = 'AGV' and fromstation='" + fromstation + "' and tostation='" + tostation + "'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        public bool UpdateAGVIndex(string taskno, string index)
        {
            string sql = "update wcs_subtaskinfo set agvindex='" + index + "' where warehouseid = '" + WareHouseID + "' and taskno like '%" + taskno + "%' and equipmentType = 'AGV'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        public bool Update_Wcs_JKInterface(string stationNO, string flag)
        {
            string sql = $@"update wcs_jkinterface t
                            set t.status = '{flag}'
                            where t.stationnumber = '{stationNO}'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        public bool Update_Init_JKInterface(string stationNO)
        {
            string sql = $@"update wcs_jkinterface t
                            set t.status = '0',t.errornumber = '' ,t.palletcode = '',t.errorinfo = '' 
                            where t.stationnumber = '{stationNO}'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        public string[] Wcs_InsertItemInfo(string itemCode, string itemName, string boxType)
        {
            string message = string.Empty;
            string[] result = new string[2];
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("i_Warehouse_Id",OracleDbType.Varchar2,50),
                new OracleParameter("i_Item_Code",OracleDbType.Varchar2,200),
                new OracleParameter("i_item_Name",OracleDbType.Varchar2,500),
                new OracleParameter("i_Box_Type",OracleDbType.Varchar2,30),
                new OracleParameter("o_Message",OracleDbType.Varchar2,500),
                new OracleParameter("o_Result",OracleDbType.Int32,50),
            };
            parameters[0].Value = WareHouseID;
            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Value = itemCode;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Value = itemName;
            parameters[2].Direction = ParameterDirection.Input;
            parameters[3].Value = boxType;
            parameters[3].Direction = ParameterDirection.Input;
            parameters[4].Value = "";
            parameters[4].Direction = ParameterDirection.Output;
            parameters[5].Value = 0;
            parameters[5].Direction = ParameterDirection.Output;
            DataTable dt = helper.ExecuteByProcedure("test_insert_base_item",parameters);
            result[0] = parameters[5].Value.ToString().Trim();
            result[1] = parameters[4].Value.ToString().Trim();
            return result;
        }

        public void AddLog(string logtext)
        {
            OracleParameter[] parms = new OracleParameter[]
            {
                new OracleParameter(@"i_warehouseid", OracleDbType.Varchar2, 50),
                new OracleParameter(@"i_logtext", OracleDbType.Varchar2,1000)
            };
            parms[0].Direction = ParameterDirection.Input;
            parms[0].Value = WareHouseID;
            parms[1].Direction = ParameterDirection.Input;
            parms[1].Value = logtext;
            helper.ExecuteByProcedure("WCS_CREATEEVENTLOG_P", parms);
        }

        public bool UpdateAGVIndexByCarNo(string CarNo, string index)
        {
            string sql = "update wcs_subtaskinfo set agvindex='" + index + "' where warehouseid = '" + WareHouseID + "' and carno = '" + CarNo + "' and equipmentType = 'AGV' ";
            return helper.ExecuteNonQueryBySql(sql);
        }

        /// <summary>
        /// 放货时,检测货位状态
        /// </summary>
        /// <param name="locationNumber"></param>
        /// <returns></returns>
        public string[] PutBoxCheck(string locationNumber)
        {
            string[] resultArray = new string[2];
            try
            {
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("i_WarehouseID",OracleDbType.Varchar2,50),
                    new OracleParameter("i_LocationNumber",OracleDbType.Varchar2,50),
                    new OracleParameter("o_Message",OracleDbType.Varchar2,1000),
                    new OracleParameter("o_Result",OracleDbType.Int32,50)
                };
                parameters[0].Direction = ParameterDirection.Input;
                parameters[0].Value = WareHouseID;
                parameters[1].Direction = ParameterDirection.Input;
                parameters[1].Value = locationNumber;
                parameters[2].Direction = ParameterDirection.Output;
                parameters[2].Value = "";
                parameters[3].Direction = ParameterDirection.Output;
                parameters[3].Value = 0;
                DataTable dt = helper.ExecuteByProcedure("WCS_PutBoxCheck", parameters);
                resultArray[0] = parameters[3].Value.ToString();
                resultArray[1] = parameters[2].Value.ToString();
                return resultArray;

            }
            catch (Exception ex)
            {
                resultArray[0] = "0";
                resultArray[1] = $"{ex.Message}{ex.StackTrace}";
                return resultArray;
            }
        }

        //根据AGV任务查询输送机任务
        public DataTable GetSTATaskInfo(string index,string taskStauts,string runStatus,string equipmentNo)
        {
                string sql = $@"
                                select ws.* 
                                from wcs_subtaskinfo ws
                                inner join wcs_taskinfo wt on wt.taskno = ws.taskno
                                where ws.equipmenttype = 'STA'
                                and ws.taskno in (
                                select ws2.taskno 
                                from wcs_subtaskinfo ws2
                                where ws2.runstatus = '{runStatus}'
                                and ws2.taskstatus = '{taskStauts}'
                                and ws2.agvindex = '{index}' 
                                and ws2.equipmentnumber = '{equipmentNo}')
                                order by ws.create_date desc";
                return helper.ExecuteQueryBySql(sql);
        }

        //更新AGV任务
        public bool UpdateAGVTaskStatus(string index, string carNO, string runStatus, string taskStatus)
        {
            OracleParameter[] parms = new OracleParameter[]
            {
                new OracleParameter(@"i_warehouseid", OracleDbType.Varchar2, 50),
                new OracleParameter(@"i_Index", OracleDbType.Varchar2, 50),
                new OracleParameter(@"i_CarNO", OracleDbType.Varchar2, 10),
                new OracleParameter(@"i_RunStatus", OracleDbType.Varchar2),
                new OracleParameter(@"i_TaskStatus", OracleDbType.Varchar2, 10),
                new OracleParameter(@"o_Result", OracleDbType.Int32)
            };
            parms[0].Direction = ParameterDirection.Input;
            parms[0].Value = WareHouseID;
            parms[1].Direction = ParameterDirection.Input;
            parms[1].Value = index;
            parms[2].Direction = ParameterDirection.Input;
            parms[2].Value = carNO;
            parms[3].Direction = ParameterDirection.Input;
            parms[3].Value = runStatus;
            parms[4].Direction = ParameterDirection.Input;
            parms[4].Value = taskStatus;
            parms[5].Direction = ParameterDirection.Output;
            parms[5].Value = 0;
            helper.ExecuteByProcedure("WCS_UpdateAGVTaskStatus", parms);
            if (string.IsNullOrEmpty(parms[5].Value.ToString()) || parms[5].Value.ToString() == "0")
            {
                return false;
            }
            return true;
        }

        public DataTable GetOPCItem()
        {
            string sql = "select  * from wcs_opcitem where  zonecode ='FL' order by itemid";
            return helper.ExecuteQueryBySql(sql);
        }
        public DataTable GetBdInfo()
        {
            string sql = "select T.* from BDADDRESS T ORDER BY T.BDNO";
            return helper.ExecuteQueryBySql(sql);
        }

        public DataTable GetEquipmentInfo()
        {
            string sql = $@"SELECT W.*,E.Wcs_Equipment_Mode 
                            FROM WCS_EQUIPMENTINFO w
                            LEFT JOIN swms_wi_equipment E
                            ON W.EQUIPMENTNUM = E.Equipment_Number 
                            WHERE W.Factory = 'DAMBACH' 
                            ORDER BY W.Equipmentnum";
            return helper.ExecuteQueryBySql(sql);
        }

        public bool UpdateTaskStatus(string TaskNo, string TaskType, string TaskStatus, string RunStatus)
        {
            OracleParameter[] parms = new OracleParameter[]
            {
                new OracleParameter(@"i_WarehouseID", OracleDbType.Varchar2, 50),
                new OracleParameter(@"i_TaskNo", OracleDbType.Long),
                new OracleParameter(@"i_EquipmentType", OracleDbType.Varchar2, 10),
                new OracleParameter(@"i_TaskStatus", OracleDbType.Varchar2),
                new OracleParameter(@"i_RunStatus", OracleDbType.Varchar2, 10),
                new OracleParameter(@"o_result", OracleDbType.Int32)
            };
            parms[0].Direction = ParameterDirection.Input;
            parms[0].Value = WareHouseID;
            parms[1].Direction = ParameterDirection.Input;
            parms[1].Value = int.Parse(TaskNo);
            parms[2].Direction = ParameterDirection.Input;
            parms[2].Value = TaskType;
            parms[3].Direction = ParameterDirection.Input;
            parms[3].Value = TaskStatus;
            parms[4].Direction = ParameterDirection.Input;
            parms[4].Value = RunStatus;
            parms[5].Direction = ParameterDirection.Output;
            parms[5].Value = 0;
            helper.ExecuteByProcedure("WCS_UPDATETASKSTATUS_P",parms);
            return parms[5].Value.ToString().Equals("1");
        }

        /// <summary>
        /// 是否存在任务状态等于0,1,2的主任务
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>
        public bool IsExsitMainTaskEqualStatus(WCS_SUBTASKINFO taskInfo)
        {
            string sql = $@"select count(1) from wcs_taskinfo where taskno = {taskInfo.TASKNO}
                            and taskstatus = '{taskInfo.TASKSTATUS}'";
            DataTable dt = helper.ExecuteQueryBySql(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }
            return !dt.Rows[0][0].ToString().Equals("0");
        }


        /// <summary>
        /// 是否存在任务状态不等于0,1,2的子任务
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>
        public bool IsExsitSubTaskNotEqualStatus(WCS_SUBTASKINFO taskInfo)
        {
            string sql = $@"select count(1) from wcs_subtaskinfo where taskno = {taskInfo.TASKNO}
                            and taskstatus <> '{taskInfo.TASKSTATUS}'";
            DataTable dt = helper.ExecuteQueryBySql(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }
            return !dt.Rows[0][0].ToString().Equals("0");
        }

        /// <summary>
        /// 更新主任务状态
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>

        public bool UpdateMainTask(WCS_TASKINFO taskWhere, WCS_TASKINFO taskUpdate)
        {
            string sql = $@"update wcs_taskinfo wt
                            set wt.updatetime = to_date('{DateTime.Now}','yyyy/mm/dd hh24:mi:ss')";
            foreach (PropertyInfo item in taskUpdate.GetType().GetProperties())
            {
                if (item.GetValue(taskUpdate) != null)
                {
                    sql += $@" ,wt.{item.Name} = '{item.GetValue(taskUpdate)}'";
                }
            }
            //条件
            sql += $@" where 1 = 1";
            foreach (PropertyInfo item in taskWhere.GetType().GetProperties())
            {
                if (item.GetValue(taskWhere) != null)
                {
                    sql += $@" and wt.{item.Name} = '{item.GetValue(taskWhere)}'";
                }
            }
            return helper.ExecuteNonQueryBySql(sql);
        }

        /// <summary>
        /// 更新子任务状态
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>

        public bool UpdateSubTask(WCS_SUBTASKINFO taskWhere,WCS_SUBTASKINFO taskUpdate)
        {
            string sql = $@"update wcs_subtaskinfo ws 
                            set ws.modify_date = to_date('{DateTime.Now}','yyyy/mm/dd hh24:mi:ss')";
            foreach (PropertyInfo item in taskUpdate.GetType().GetProperties())
            {
                if (item.GetValue(taskUpdate) != null)
                {
                    sql += $@" ,ws.{item.Name} = '{item.GetValue(taskUpdate)}'";
                }
            }
            //条件
            sql += $@" where 1 = 1";
            foreach (PropertyInfo item in taskWhere.GetType().GetProperties())
            {
                if (item.GetValue(taskWhere) != null)
                {
                    sql += $@" and ws.{item.Name} = '{item.GetValue(taskWhere)}'";
                }
            }
            return helper.ExecuteNonQueryBySql(sql);
        }

        /// <summary>
        /// 转移WCS已完成任务
        /// </summary>
        /// <param name="taskNo"></param>
        /// <returns></returns>

        public bool TransferWcsFinishedTask(string taskNo)
        {
            List<string> sqlList = new List<string>() 
            {   $@"insert into wcs_taskinfo_history 
                   select* from wcs_taskinfo t where t.taskno = {taskNo}",
                $@"insert into wcs_subtaskinfo_history 
                   select* from wcs_subtaskinfo t where t.taskno = {taskNo}",
                $@"delete from wcs_taskinfo t where t.taskno = {taskNo}",
                $@"delete from wcs_subtaskinfo t where t.taskno = {taskNo}"
            };
            return helper.ExecuteManyNonQueryBySql(sqlList);
        }

        /// <summary>
        /// 转移上位下发任务
        /// </summary>
        /// <param name="taskNo"></param>
        /// <returns></returns>
        public bool TransferReceiveFinishedTask(string taskNo)
        {
            List<string> sqlList = new List<string>()
            {   $@" insert into wcs_receive_task_history
                    select wr.* from wcs_receive_task wr
                    where wr.tasknum in (
                    select wt.wmstaskno from wcs_taskinfo wt
                    where wt.taskno = {taskNo}
                    )",
                $@" delete  from wcs_receive_task wr
                    where wr.tasknum in (
                    select wt.wmstaskno from wcs_taskinfo wt
                    where wt.taskno = {taskNo}
                    )"
            };
            return helper.ExecuteManyNonQueryBySql(sqlList);
        }

        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="taskWhere"></param>
        /// <param name="taskUpdate"></param>
        /// <returns></returns>
        public bool DealTaskStatus(WCS_SUBTASKINFO taskWhere, WCS_SUBTASKINFO taskUpdate)
        {
            bool result = false;
            try
            {
                if (taskWhere.EQUIPMENTTYPE != null && taskWhere.EQUIPMENTTYPE.Equals("MAIN"))
                {
                    //修改主任务
                    return UpdateMainTask(
                           new WCS_TASKINFO() { TASKNO = taskWhere.TASKNO, },
                           new WCS_TASKINFO() { TASKSTATUS = taskUpdate.TASKSTATUS }
                           );
                }
                DataTable subTable = null;
                if (taskWhere.TASKNO == null)
                {
                    subTable = QueryTableData(taskWhere);
                    if (subTable != null && subTable.Rows.Count == 1)
                    {
                        taskWhere.TASKNO = Convert.ToInt32(subTable.Rows[0]["taskno"].ToString());
                    }
                    else
                    {
                        //如果条件找到多条任务,直接不处理
                        return false;
                    }
                }
                if (!UpdateSubTask(taskWhere, taskUpdate)) return false;
                bool isExsit = true;
               
                WCS_SUBTASKINFO taskWhereNew = ObjectTransformUtil.Clone(taskWhere);
                Type subTaskType = taskWhereNew.GetType();
                foreach (PropertyInfo item in taskUpdate.GetType().GetProperties())
                {
                    if (item.GetValue(taskUpdate) != null)
                    {
                        subTaskType.GetProperty(item.Name).SetValue(taskWhereNew, item.GetValue(taskUpdate));
                    }
                }

                if (taskUpdate.TASKSTATUS != null)
                {
                    //WMS任务状态处理
                    DealWmsTaskStatus(taskWhereNew, taskUpdate);

                    if (taskUpdate.TASKSTATUS.Equals("1"))
                    {
                        isExsit = IsExsitSubTaskNotEqualStatus(new WCS_SUBTASKINFO() { TASKNO = taskWhere.TASKNO, TASKSTATUS = taskUpdate.TASKSTATUS });
                        //子任务全部完成,则全部完成
                        if (!isExsit)
                        {
                            //完成主任务
                            UpdateMainTask(
                                new WCS_TASKINFO() { TASKNO = taskWhere.TASKNO, },
                                new WCS_TASKINFO() { TASKSTATUS = taskUpdate.TASKSTATUS }
                                );
                            //转移WMS已完成任务
                            TransferReceiveFinishedTask(taskWhere.TASKNO.ToString());
                            //转移WCS已完成任务
                            return TransferWcsFinishedTask(taskWhere.TASKNO.ToString());
                        }
                    }

                    if (taskUpdate.TASKSTATUS.Equals("1") || taskUpdate.TASKSTATUS.Equals("2"))
                    {
                        isExsit = IsExsitMainTaskEqualStatus(new WCS_SUBTASKINFO() { TASKNO = taskWhere.TASKNO, TASKSTATUS = "2" });
                        if (!isExsit)
                        {
                            return UpdateMainTask(
                                new WCS_TASKINFO() { TASKNO = taskWhere.TASKNO },
                                new WCS_TASKINFO() { TASKSTATUS = "2" }
                                );
                        }
                        result = true;
                    }

                    if (taskUpdate.TASKSTATUS.Equals("0"))
                    {
                        result = true;
                    }
                }
                else
                {
                    result = true;
                }
               
            }
            catch(Exception ex)
            {
                AddLog("DealTaskStatus" + ex.Message + ex.StackTrace);
                return false;
            }
            return result;
        }

        /// <summary>
        /// WMS任务状态处理
        /// </summary>
        /// <param name="taskModel"></param>
        public void DealWmsTaskStatus(WCS_SUBTASKINFO taskWhere,WCS_SUBTASKINFO taskUpdate)
        {
           try
           {
                DataTable totalTaskTable = GetTotalTask(taskWhere);
                DataTable subTaskTable = QueryTableData(taskWhere);
                string taskType = totalTaskTable.Rows[0]["tasktype"].ToString();
                WMSUpdateTaskStatusRequest request = new WMSUpdateTaskStatusRequest();
                ResultObject wmsServiceResult = null;
                WMSUpdateTaskStatusResponse response = null;
                request.Msg_Mark = $"WCS任务号:{taskWhere.TASKNO}";
                request.Msg_Params[0].WarehouseID = WareHouseID;
                request.Msg_Params[0].ZoneID = string.Empty;
                request.Msg_Params[0].TaskNum = totalTaskTable.Rows[0]["wmstaskno"].ToString();
                request.Msg_Params[0].TaskStatus = string.Empty;
                request.Msg_Method = "任务状态反馈";
                bool isExsit = true;


                #region 【废弃】任务反馈
                //if (taskUpdate.TASKSTATUS.Equals("1"))
                //{
                //    if (!string.IsNullOrEmpty(taskType))
                //    {
                //        if (taskType.Equals("Task_CallTerminal_ReturnBadPallet"))
                //        {
                //            request.Msg_Params[0].Old_Taskstatus = "Task_Executing";
                //            request.Msg_Params[0].TaskStatus = "Task_Executed";
                //        }
                //        if (taskType.Equals("Task_RFAssembly"))
                //        {
                //            if (taskWhere.EQUIPMENTTYPE.Equals("SC") || (taskWhere.EQUIPMENTTYPE.Equals("AGV") && toStation.Contains("D")))
                //            {
                //                request.Msg_Params[0].Old_Taskstatus = "Task_Executing";
                //                request.Msg_Params[0].TaskStatus = "Task_Executed";
                //            }
                //        }

                //        if (taskType.Equals("Task_CallTerminal_ReturnNullPallet"))
                //        {
                //            request.Msg_Params[0].Old_Taskstatus = "Task_Executing";
                //            request.Msg_Params[0].TaskStatus = "Task_Executed";
                //        }
                //        if (taskType.Equals("Task_CallTerminal_ReturnPallet"))
                //        {
                //            if (taskWhere.EQUIPMENTTYPE.Equals("AGV"))
                //            {
                //                if (toStation.Contains("D"))
                //                {
                //                    request.Msg_Params[0].Old_Taskstatus = "Task_Executing";
                //                    request.Msg_Params[0].TaskStatus = "Task_Executed";
                //                }
                //                else
                //                {
                //                    request.Msg_Params[0].Old_Taskstatus = "Task_Executing";
                //                    request.Msg_Params[0].TaskStatus = "Task_PartExecuted";
                //                }
                //            }
                //            if (taskWhere.EQUIPMENTTYPE.Equals("SC"))
                //            {
                //                request.Msg_Params[0].Old_Taskstatus = "Task_PartExecuted";
                //                request.Msg_Params[0].TaskStatus = "Task_Executed";
                //            }
                //        }
                //        if (taskType.Equals("Task_CallTerminal_Call"))
                //        {
                //            if (taskWhere.EQUIPMENTTYPE.Equals("AGV"))
                //            {
                //                if (fromStation.Contains("D"))
                //                {
                //                    request.Msg_Params[0].Old_Taskstatus = "Task_Executing";
                //                    request.Msg_Params[0].TaskStatus = "Task_PartExecuted";
                //                }
                //                else
                //                {
                //                    request.Msg_Params[0].Old_Taskstatus = "Task_PartExecuted";
                //                    request.Msg_Params[0].TaskStatus = "Task_Executed";
                //                }
                //            }
                //            if (taskWhere.EQUIPMENTTYPE.Equals("SC"))
                //            {
                //                request.Msg_Params[0].Old_Taskstatus = "Task_Executing";
                //                request.Msg_Params[0].TaskStatus = "Task_PartExecuted";
                //            }
                //        }
                //        if (taskType.Equals("Task_OutBound"))
                //        {
                //            if (taskWhere.EQUIPMENTTYPE.Equals("AGV"))
                //            {
                //                if (fromStation.Contains("D") && toStation.Equals("5722"))
                //                {
                //                    request.Msg_Params[0].Old_Taskstatus = "Task_Executing";
                //                    request.Msg_Params[0].TaskStatus = "Task_PartExecuted";
                //                }
                //                else
                //                {
                //                    request.Msg_Params[0].Old_Taskstatus = "Task_PartExecuted";
                //                    request.Msg_Params[0].TaskStatus = "Task_Executed";
                //                }
                //            }
                //            if (taskWhere.EQUIPMENTTYPE.Equals("SC"))
                //            {
                //                request.Msg_Params[0].Old_Taskstatus = "Task_Executing";
                //                request.Msg_Params[0].TaskStatus = "Task_PartExecuted";
                //            }
                //            if (taskWhere.EQUIPMENTTYPE.Equals("STA"))
                //            {
                //                request.Msg_Params[0].Old_Taskstatus = "Task_PartExecuted";
                //                request.Msg_Params[0].TaskStatus = "Task_Executed";
                //            }
                //        }


                //    }
                //}
                //if (taskUpdate.TASKSTATUS.Equals("2"))
                //{
                //    if (!string.IsNullOrEmpty(taskType))
                //    {
                //        if (taskType.Equals("Task_CallTerminal_ReturnBadPallet"))
                //        {
                //            request.Msg_Params[0].Old_Taskstatus = "Task_NotExecute";
                //            request.Msg_Params[0].TaskStatus = "Task_Executing";
                //        }

                //        if (taskType.Equals("Task_RFAssembly"))
                //        {
                //            if (taskWhere.EQUIPMENTTYPE.Equals("STA"))
                //            {
                //                request.Msg_Params[0].Old_Taskstatus = "Task_NotExecute";
                //                request.Msg_Params[0].TaskStatus = "Task_Executing";
                //            }
                //        }
                //        if (taskType.Equals("Task_InBound"))
                //        {
                //            if (taskWhere.EQUIPMENTTYPE.Equals("STA"))
                //            {
                //                request.Msg_Params[0].Old_Taskstatus = "Task_NotExecute";
                //                request.Msg_Params[0].TaskStatus = "Task_Executing";
                //            }
                //        }
                //        if (taskType.Equals("Task_CallTerminal_ReturnNullPallet"))
                //        {
                //            if (taskWhere.EQUIPMENTTYPE.Equals("AGV"))
                //            {
                //                request.Msg_Params[0].Old_Taskstatus = "Task_NotExecute";
                //                request.Msg_Params[0].TaskStatus = "Task_Executing";
                //            }
                //        }
                //        if (taskType.Equals("Task_CallTerminal_ReturnPallet"))
                //        {
                //            if (taskWhere.EQUIPMENTTYPE.Equals("AGV"))
                //            {
                //                request.Msg_Params[0].Old_Taskstatus = "Task_NotExecute";
                //                request.Msg_Params[0].TaskStatus = "Task_Executing";
                //            }
                //        }
                //        if (taskType.Equals("Task_CallTerminal_Call"))
                //        {
                //            if (taskWhere.EQUIPMENTTYPE.Equals("SC"))
                //            {
                //                request.Msg_Params[0].Old_Taskstatus = "Task_NotExecute";
                //                request.Msg_Params[0].TaskStatus = "Task_Executing";
                //            }
                //            //二楼第一次搬送任务
                //            if (taskWhere.EQUIPMENTTYPE.Equals("AGV") && fromStation.Contains("D"))
                //            {
                //                request.Msg_Params[0].Old_Taskstatus = "Task_NotExecute";
                //                request.Msg_Params[0].TaskStatus = "Task_Executing";
                //            }
                //        }
                //        if (taskType.Equals("Task_OutBound"))
                //        {
                //            if (taskWhere.EQUIPMENTTYPE.Equals("AGV") || taskWhere.EQUIPMENTTYPE.Equals("SC"))
                //            {
                //                request.Msg_Params[0].Old_Taskstatus = "Task_NotExecute";
                //                request.Msg_Params[0].TaskStatus = "Task_Executing";
                //            }
                //        }
                //    }
                //}

                #endregion
                //TaskStatus_NotExecute
                //TaskStatus_Executing
                //TaskStatus_WaitingExecute
                //TaskStatus_Finished
                //重写任务反馈
                if (!string.IsNullOrEmpty(taskUpdate.TASKSTATUS))
                {
                    if (taskUpdate.TASKSTATUS.Equals("1"))
                    {
                        //入库
                        if (taskType.Equals("TaskType_Inbound") || taskType.Equals("TaskType_InBound"))
                        {
                            //完成SC任务
                            if (taskWhere.EQUIPMENTTYPE.Equals("SC"))
                            {
                                //起始地址是5622
                                if (subTaskTable.Rows[0]["fromstation"].ToString().Equals("5622"))
                                {
                                    request.Msg_Params[0].TaskStatus = "1";
                                }
                                if (subTaskTable.Rows[0]["fromstation"].ToString().Equals("5647") || subTaskTable.Rows[0]["fromstation"].ToString().Equals("5637"))
                                {
                                    request.Msg_Params[0].TaskStatus = "1";
                                }
                            }
                            //完成AGV任务
                            if (taskWhere.EQUIPMENTTYPE.Equals("AGV"))
                            {
                                //一楼机台退空盘、尾料
                                if (subTaskTable.Rows[0]["tostation"].ToString().Equals("5658") && !subTaskTable.Rows[0]["fromstation"].ToString().Equals("5659"))
                                {
                                        request.Msg_Params[0].TaskStatus = "1";
                                }
                                //一楼紧急口托盘到5658
                                if (subTaskTable.Rows[0]["tostation"].ToString().Equals("5658") && subTaskTable.Rows[0]["fromstation"].ToString().Equals("5659"))
                                {
                                    request.Msg_Params[0].TaskStatus = string.Empty;
                                }
                                //二楼空盘码盘入库
                                if (subTaskTable.Rows[0]["fromstation"].ToString().Equals("5665"))
                                {
                                    request.Msg_Params[0].TaskStatus = "1";
                                }
                                //二楼托盘从5722入高架库
                                if (subTaskTable.Rows[0]["fromstation"].ToString().Equals("5722") || subTaskTable.Rows[0]["fromstation"].ToString().Equals("FSZ0011"))
                                {
                                    request.Msg_Params[0].TaskStatus = "1";
                                }
                                //二楼退料托盘放转向架
                                if (subTaskTable.Rows[0]["tostation"].ToString().Contains("R_"))
                                {
                                    request.Msg_Params[0].TaskStatus = string.Empty;
                                }
                                //二楼退料托盘从转向架入高架库
                                if (subTaskTable.Rows[0]["fromstation"].ToString().Contains("R_"))
                                {
                                    request.Msg_Params[0].TaskStatus = "1";
                                }
                                //二楼退料托盘从发射机入高架库
                                if (subTaskTable.Rows[0]["fromstation"].ToString().Contains("FS"))
                                {
                                    request.Msg_Params[0].TaskStatus = "1";
                                }
                            }
                            //完成STA任务
                            if (taskWhere.EQUIPMENTTYPE.Equals("STA"))
                            {
                                request.Msg_Params[0].TaskStatus = string.Empty;
                            }
                        }
                        //出库
                        if (taskType.Equals("TaskType_Outbound") || taskType.Equals("TaskType_OutBound"))
                        {
                            if (taskWhere.EQUIPMENTTYPE.Equals("SC"))
                            {
                                request.Msg_Params[0].TaskStatus = "3";
                            }
                            if (taskWhere.EQUIPMENTTYPE.Equals("AGV"))
                            {
                                //一楼从取货口到卷包机台
                                if (subTaskTable.Rows[0]["fromstation"].ToString().Equals("5650") || subTaskTable.Rows[0]["fromstation"].ToString().Equals("5653"))
                                {
                                    request.Msg_Params[0].TaskStatus = "1";
                                }
                                //二楼从货架到5722
                                if (subTaskTable.Rows[0]["tostation"].ToString().Equals("5722"))
                                {
                                    request.Msg_Params[0].TaskStatus = "3";
                                }
                                ////二楼从货架到转向架【不调用WMS接口，因为接口可能会超时，导致下发AGV任务延迟】
                                //if (subTaskTable.Rows[0]["tostation"].ToString().Contains("R_"))
                                //{
                                //    request.Msg_Params[0].TaskStatus = "3";
                                //}
                                //二楼从转向架到地面
                                if (subTaskTable.Rows[0]["fromstation"].ToString().Contains("R_"))
                                {
                                    request.Msg_Params[0].TaskStatus = "1";
                                }
                                //二楼从货架到发射机
                                if (subTaskTable.Rows[0]["tostation"].ToString().Contains("FS")
                                    || subTaskTable.Rows[0]["tostation"].ToString().Contains("7777")
                                    || subTaskTable.Rows[0]["tostation"].ToString().Equals("BCP501") 
                                    || subTaskTable.Rows[0]["tostation"].ToString().Equals("BCP601")
                                    || subTaskTable.Rows[0]["tostation"].ToString().Contains("7776"))
                                {
                                    request.Msg_Params[0].TaskStatus = "1";
                                }
                            }
                            if (taskWhere.EQUIPMENTTYPE.Equals("STA"))
                            {
                                isExsit = IsExsitSubTaskNotEqualStatus(new WCS_SUBTASKINFO()
                                {
                                    TASKNO = taskWhere.TASKNO,
                                    TASKSTATUS = "1"
                                });
                                //完成任务
                                if (!isExsit)
                                {
                                    request.Msg_Params[0].TaskStatus = "1";
                                }
                            }
                        }
                        //移库
                        if (taskType.Equals("TaskType_Move"))
                        {
                            request.Msg_Params[0].TaskStatus = "1";
                        }
                    }
                    #region 执行中不反馈
                    //if (taskUpdate.TASKSTATUS.Equals("2"))
                    //{
                    //    //入库
                    //    if (taskType.Equals("TaskType_Inbound") || taskType.Equals("TaskType_InBound"))
                    //    {
                    //        //执行SC任务
                    //        if (taskWhere.EQUIPMENTTYPE.Equals("SC"))
                    //        {
                    //            //起始地址是5622
                    //            if (subTaskTable.Rows[0]["fromstation"].ToString().Equals("5622"))
                    //            {
                    //                request.Msg_Params[0].TaskStatus = string.Empty;
                    //            }
                    //            if (subTaskTable.Rows[0]["fromstation"].ToString().Equals("5647") || subTaskTable.Rows[0]["fromstation"].ToString().Equals("5637"))
                    //            {
                    //                //空托盘组
                    //                if (subTaskTable.Rows[0]["barcode"].ToString().Equals("99999999"))
                    //                {
                    //                    request.Msg_Params[0].TaskStatus = "2";
                    //                }
                    //                else
                    //                {
                    //                    //退回的尾料
                    //                    request.Msg_Params[0].TaskStatus = string.Empty;
                    //                }
                    //            }

                    //        }
                    //        //执行AGV任务
                    //        if (taskWhere.EQUIPMENTTYPE.Equals("AGV"))
                    //        {
                    //            //一楼机台退空盘、尾料
                    //            if (subTaskTable.Rows[0]["tostation"].ToString().Equals("5658") && !subTaskTable.Rows[0]["fromstation"].ToString().Equals("5659"))
                    //            {
                    //                //一楼机台空托盘退回到5658
                    //                if (subTaskTable.Rows[0]["barcode"].ToString().Equals("99999999"))
                    //                {
                    //                    request.Msg_Params[0].TaskStatus = "2";
                    //                }
                    //                else
                    //                {
                    //                    //一楼机台退尾料到5658
                    //                    request.Msg_Params[0].TaskStatus = "2";
                    //                }
                    //            }
                    //            //一楼紧急口托盘到5658
                    //            if (subTaskTable.Rows[0]["tostation"].ToString().Equals("5658") && subTaskTable.Rows[0]["fromstation"].ToString().Equals("5659"))
                    //            {
                    //                request.Msg_Params[0].TaskStatus = string.Empty;
                    //            }
                    //            //二楼空盘码盘入库
                    //            if (subTaskTable.Rows[0]["fromstation"].ToString().Equals("5665"))
                    //            {
                    //                request.Msg_Params[0].TaskStatus = "2";
                    //            }
                    //            //二楼托盘从5722入高架库
                    //            if (subTaskTable.Rows[0]["fromstation"].ToString().Equals("5722"))
                    //            {
                    //                request.Msg_Params[0].TaskStatus = string.Empty;
                    //            }
                    //            //二楼退料托盘放转向架
                    //            if (subTaskTable.Rows[0]["tostation"].ToString().Contains("R_"))
                    //            {
                    //                request.Msg_Params[0].TaskStatus = "2";
                    //            }
                    //            //二楼退料托盘从转向架入高架库
                    //            if (subTaskTable.Rows[0]["fromstation"].ToString().Contains("R_"))
                    //            {
                    //                request.Msg_Params[0].TaskStatus = string.Empty;
                    //            }
                    //            //二楼退料托盘从发射机入高架库
                    //            if (subTaskTable.Rows[0]["fromstation"].ToString().Contains("FS"))
                    //            {
                    //                request.Msg_Params[0].TaskStatus = "2";
                    //            }
                    //        }
                    //        //执行STA任务
                    //        if (taskWhere.EQUIPMENTTYPE.Equals("STA"))
                    //        {
                    //            request.Msg_Params[0].TaskStatus = string.Empty;
                    //        }
                    //    }
                    //    //出库
                    //    if (taskType.Equals("TaskType_Outbound") || taskType.Equals("TaskType_OutBound"))
                    //    {
                    //        if (taskWhere.EQUIPMENTTYPE.Equals("SC"))
                    //        {
                    //            request.Msg_Params[0].TaskStatus = "2";
                    //        }
                    //        if (taskWhere.EQUIPMENTTYPE.Equals("AGV"))
                    //        {
                    //            //一楼从取货口到卷包机台
                    //            if (subTaskTable.Rows[0]["fromstation"].ToString().Equals("5650") || subTaskTable.Rows[0]["fromstation"].ToString().Equals("5653"))
                    //            {
                    //                request.Msg_Params[0].TaskStatus = string.Empty;
                    //            }
                    //            //二楼从货架到5722
                    //            if (subTaskTable.Rows[0]["tostation"].ToString().Equals("5722"))
                    //            {
                    //                request.Msg_Params[0].TaskStatus = "2";
                    //            }
                    //            //二楼从货架到转向架
                    //            if (subTaskTable.Rows[0]["tostation"].ToString().Contains("R_"))
                    //            {
                    //                request.Msg_Params[0].TaskStatus = "2";
                    //            }
                    //            //二楼从转向架到地面
                    //            if (subTaskTable.Rows[0]["fromstation"].ToString().Contains("R_"))
                    //            {
                    //                request.Msg_Params[0].TaskStatus = string.Empty;
                    //            }
                    //            //二楼从货架到发射机
                    //            if (subTaskTable.Rows[0]["tostation"].ToString().Contains("FS"))
                    //            {
                    //                request.Msg_Params[0].TaskStatus = "2";
                    //            }
                    //        }
                    //        if (taskWhere.EQUIPMENTTYPE.Equals("STA"))
                    //        {
                    //            request.Msg_Params[0].TaskStatus = string.Empty;
                    //        }
                    //    }
                    //    //移库
                    //    if (taskType.Equals("TaskType_Move"))
                    //    {
                    //        request.Msg_Params[0].TaskStatus = "2";
                    //    }
                    //}
                    #endregion
                    //调用部分完成
                    if (taskUpdate.TASKSTATUS.Equals("3"))
                    {
                        //入库
                        if (taskType.Equals("TaskType_Inbound") || taskType.Equals("TaskType_InBound"))
                        {
                            //部分完成AGV任务
                            if (taskWhere.EQUIPMENTTYPE.Equals("AGV"))
                            {
                                //一楼机台退空盘、尾料
                                if (subTaskTable.Rows[0]["tostation"].ToString().Equals("5658") 
                                    && !subTaskTable.Rows[0]["fromstation"].ToString().Equals("5659")
                                   )
                                {
                                     request.Msg_Params[0].TaskStatus = "3";
                                }
                                //二楼退料托盘从发射机入高架库
                                if (subTaskTable.Rows[0]["fromstation"].ToString().Contains("FS"))
                                {
                                    request.Msg_Params[0].TaskStatus = "3";
                                }
                            }
                        }
                        //出库
                        if (taskType.Equals("TaskType_Outbound") || taskType.Equals("TaskType_OutBound"))
                        {
                            if (taskWhere.EQUIPMENTTYPE.Equals("AGV"))
                            {
                                //二楼
                                if (subTaskTable.Rows[0]["fromstation"].ToString().Length == 7)
                                {
                                    request.Msg_Params[0].TaskStatus = "3";
                                }
                            }
                        }
                        //移库
                        if (taskType.Equals("TaskType_Move"))
                        {
                            request.Msg_Params[0].TaskStatus = "3";
                        }
                    }
                    if (!string.IsNullOrEmpty(request.Msg_Params[0].TaskStatus))
                    {
                        //调用WMS接口
                        wmsServiceResult = WMSMicroServiceUtil.RequestWMS(new WMSServicRequest() { PostData = request, DataUrl = "/api/tyjk/Task/FeedBackTaskStatus" });
                        if (!wmsServiceResult.IsSuccess)
                        {
                            AddLog($"WCS任务号:{taskWhere.TASKNO},WMS任务号:{request.Msg_Params[0].TaskNum},任务状态码:{request.Msg_Params[0].TaskStatus},调用WMS任务状态接口,反馈失败.详细信息:{wmsServiceResult.ErrorMessage}");
                            return;
                        }
                        response = JsonConvert.DeserializeObject<WMSUpdateTaskStatusResponse>(wmsServiceResult.ResultJson);
                        if (!response.isSucceed)
                        {
                            AddLog($"WCS任务号:{taskWhere.TASKNO},WMS任务号:{request.Msg_Params[0].TaskNum},任务状态码:{request.Msg_Params[0].TaskStatus},调用WMS任务状态接口,反馈失败.详细信息:{response.errmsg}");
                            return;
                        }
                        AddLog($"WCS任务号:{taskWhere.TASKNO},WMS任务号:{request.Msg_Params[0].TaskNum},任务状态码:{request.Msg_Params[0].TaskStatus},调用WMS任务状态接口,反馈成功");
                    }
                }
              
            }
            catch (Exception ex)
            {
                AddLog(ex.Message + ex.StackTrace);
            }

          
        }

        public DataTable GetAgvTaskInfoByIndex(string agvIndex,string agvCarNo,string runStatus)
        {
            string sql = $@"select ws.taskno,ws.fromstation,ws.tostation,ws.barcode
                            from wcs_subtaskinfo ws
                            where ws.agvindex = '{agvIndex}'
                            and ws.agvcarno = '{agvCarNo}'
                            and ws.runstatus = '{runStatus}'
                            and ws.equipmenttype = 'AGV'";
            return helper.ExecuteQueryBySql(sql);
        }

        public DataTable GetSRMTask(string scstatus)
        {
            OracleParameter[] parms = new OracleParameter[]
               {
                    new OracleParameter(@"i_warehouseid", OracleDbType.Varchar2, 50),
                    new OracleParameter(@"i_scstatus", OracleDbType.Varchar2, 50),
                    new OracleParameter(@"o_taskinfo", OracleDbType.RefCursor)
               };
            parms[0].Direction = ParameterDirection.Input;
            parms[0].Value = WareHouseID;
            parms[1].Direction = ParameterDirection.Input;
            parms[1].Value = scstatus;
            parms[2].Direction = ParameterDirection.Output;
            parms[2].Value = null;
            return helper.ExecuteByProcedure("WCS_SCGETTASK_P",parms);
        }

        //更新堆垛机故障
        public bool UpdateSCErrorInfo(string SCNO, string errorMessage)
        {
            string sql = $"update equipment t set t.errorinfo = '{errorMessage}' where t.equipmentnumber = '{SCNO}'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        public bool GetTask(string taskno, string barcode, string goodstype, string fromstation, string tostation, string emptyneed, ref string msg)
        {
            OracleParameter[] parms = new OracleParameter[]
            {
                new OracleParameter(@"i_warehouse_id", OracleDbType.Char,32),
                new OracleParameter(@"i_taskno", OracleDbType.Varchar2,10),
                new OracleParameter(@"i_barcode", OracleDbType.Varchar2,50),
                new OracleParameter(@"i_goods_type", OracleDbType.Varchar2,10),
                new OracleParameter(@"i_from_station", OracleDbType.Varchar2,10),
                new OracleParameter(@"i_to_station", OracleDbType.Varchar2,10),
                new OracleParameter(@"i_emptyneed", OracleDbType.Varchar2,10),
                new OracleParameter(@"o_result", OracleDbType.Varchar2,50),
                new OracleParameter(@"o_msg", OracleDbType.Char, 1000)
            };
            for (int i = 0; i < 11; i++)
            {
                parms[i].Direction = ParameterDirection.Input;
            }
            parms[11].Direction = ParameterDirection.Output;
            parms[12].Direction = ParameterDirection.Output;
            parms[0].Value = WareHouseID;
            parms[1].Value = taskno;
            parms[2].Value = barcode;
            parms[3].Value = goodstype;
            parms[4].Value = fromstation;
            parms[5].Value = tostation;
            parms[6].Value = emptyneed;
            parms[7].Value = "0";
            parms[8].Value = string.Empty;
            helper.ExecuteByProcedure("WCS_GETTASK_P", parms);
            msg = parms[8].Value.ToString();
            return parms[7].Value.ToString().Equals("1");
        }


        //任务管理结果集
        public DataTable GetTaskInfoView(string warehouseId,
                                         string wcsTaskType,
                                         string taskStatus,
                                         string startTime,
                                         string endTime,
                                         string wcsTaskNo,
                                         string wmsTaskNo,
                                         string fromAddress,
                                         string toAddress,
                                         string barcode)
        {
            OracleParameter[] param = new OracleParameter[]
            {
                new OracleParameter(@"i_warehouse_id", OracleDbType.Varchar2,50),
                new OracleParameter(@"i_wcs_task_type", OracleDbType.Varchar2,50),
                new OracleParameter(@"i_task_status", OracleDbType.Varchar2,100),
                new OracleParameter(@"i_start_time", OracleDbType.Varchar2,100),
                new OracleParameter(@"i_end_time", OracleDbType.Varchar2, 100),
                new OracleParameter(@"i_wcs_task_no", OracleDbType.Varchar2, 50),
                new OracleParameter(@"i_wms_task_no", OracleDbType.Varchar2, 50),
                new OracleParameter(@"i_from_address", OracleDbType.Varchar2, 50),
                new OracleParameter(@"i_to_address", OracleDbType.Varchar2, 50),
                new OracleParameter(@"i_barcode", OracleDbType.Varchar2, 50),
                new OracleParameter(@"o_result", OracleDbType.RefCursor),
            };
            for (int i = 0; i < 10; i++)
            {
                param[i].Direction = ParameterDirection.Input;
            }
            param[10].Direction = ParameterDirection.Output;
            param[0].Value = WareHouseID;
            param[1].Value = wcsTaskType;
            param[2].Value = taskStatus;
            param[3].Value = startTime;
            param[4].Value = endTime;
            param[5].Value = wcsTaskNo;
            param[6].Value = wmsTaskNo;
            param[7].Value = fromAddress;
            param[8].Value = toAddress;
            param[9].Value = barcode;
            param[10].Value = null;
            return helper.ExecuteByProcedure("wcs_taskview_p_z", param);
        }



        public DataTable GetTaskInfo(int tasktype, int taskstatus, string fromtime, string totime, string wmstaskno, string taskno, string fromstation, string tostation, string barcode, string zoneid)
        {
            OracleParameter[] param = new OracleParameter[] { new OracleParameter(@"i_warehouseid", OracleDbType.Varchar2, 50),
                    new OracleParameter(@"i_tasktype", OracleDbType.Int32),
                    new OracleParameter(@"i_taskstatus", OracleDbType.Int32),
                    new OracleParameter(@"i_fromtime", OracleDbType.Varchar2, 50),
                    new OracleParameter(@"i_totime", OracleDbType.Varchar2, 50),
                    new OracleParameter(@"i_wmstaskno", OracleDbType.Varchar2, 50),
                    new OracleParameter(@"i_fromstation", OracleDbType.Varchar2, 20),
                    new OracleParameter(@"i_tostation", OracleDbType.Varchar2, 20),
                    new OracleParameter(@"i_taskno", OracleDbType.Varchar2, 10),
                    new OracleParameter(@"i_barcode", OracleDbType.Varchar2, 20),
                    new OracleParameter(@"i_taskflag", OracleDbType.Varchar2, 20),
                    new OracleParameter(@"o_cursor", OracleDbType.RefCursor)
            };
            for (int i = 0; i < 11; i++)
            {
                param[i].Direction = ParameterDirection.Input;
            }
            param[11].Direction = ParameterDirection.Output;
            param[0].Value = WareHouseID;
            param[1].Value = tasktype;
            param[2].Value = taskstatus;
            param[3].Value = fromtime;
            param[4].Value = totime;
            param[5].Value = wmstaskno;
            param[6].Value = fromstation;
            param[7].Value = tostation;
            param[8].Value = taskno;
            param[9].Value = barcode;
            param[10].Value = "";
            param[11].Value = null;
            return helper.ExecuteByProcedure("wcs_taskview_p", param);
        }

        /// <summary>
        /// 经过分拣扫码器时,保存条码
        /// </summary>
        /// <param name="oneCode"></param>
        /// <param name="FXNO"></param>
        public void SaveOneCode(string oneCode, string FXNO)
        {
                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("i_WarehouseID",OracleDbType.Varchar2,50),
                new OracleParameter("i_OneCode",OracleDbType.Varchar2,100),
                new OracleParameter("i_FxNO",OracleDbType.Varchar2,100),
                new OracleParameter("o_Result",OracleDbType.Int32,50),
                };
                parameters[0].Direction = ParameterDirection.Input;
                parameters[0].Value = WareHouseID;
                parameters[1].Direction = ParameterDirection.Input;
                parameters[1].Value = oneCode;
                parameters[2].Direction = ParameterDirection.Input;
                parameters[2].Value = FXNO;
                parameters[3].Direction = ParameterDirection.Output;
                parameters[3].Value = 0;
                helper.ExecuteByProcedure("WCS_SaveOneCode", parameters);
        }

        public DataTable GetTaskInfo(string taskno)
        {
                OracleParameter[] param = new OracleParameter[2]{
                new OracleParameter("i_warehouseid",OracleDbType.Varchar2){Value = WareHouseID},
                new OracleParameter("taskno",OracleDbType.Varchar2){Value = taskno}};
                string sql_select = @"select fromstation,tostation,barcode,goodstype  from  wcs_taskinfo 
                                        where taskno = :taskno  and warehouseid = :i_warehouseid  ";
                return helper.ExecuteQueryBySqlParams(sql_select,param);
        }

        public bool Update_SCMode(string scno, SCTaskMode taskmode)
        {
            string sql = $@"update wcs_equipment t set t.equipment_mode = '{taskmode}' where t.equipment_no = '{scno}'";
            return helper.ExecuteNonQueryBySql(sql);
        }

        public string CheckSCTaksState(string taskno)
        {
                string sql_select = string.Format("select equipmentnumber from wcs_subtaskinfo where warehouseid = '{0}' and taskno = '{1}' and equipmenttype ='SC' and taskstatus = '0' ", WareHouseID, taskno);
                DataTable dt = helper.ExecuteQueryBySql(sql_select);
                return dt.Rows[0][0].ToString();
        }

        public bool Delete_WCSTaskInfo(string type, string taskno)
        {
                string sql1 = $"delete from wcs_taskinfo where taskno = '{taskno}'";
                string sql2 = $"delete from wcs_subtaskinfo where taskno = '{taskno}'";
                helper.ExecuteNonQueryBySql(sql1);
                return helper.ExecuteNonQueryBySql(sql2);
        }

        public bool AddTask(string fromstation, string tostation)
        {
            OracleParameter[] parms = new OracleParameter[]
            {
                new OracleParameter(@"i_WarehouseID", OracleDbType.Varchar2, 50),
                new OracleParameter(@"i_fromstation", OracleDbType.Varchar2, 20),
                new OracleParameter(@"i_tostation", OracleDbType.Varchar2, 20)
            };
            for (int i = 0; i < 3; i++)
            {
                parms[i].Direction = ParameterDirection.Input;
            }
            parms[0].Value = WareHouseID;
            parms[1].Value = fromstation;
            parms[2].Value = tostation;
            helper.ExecuteByProcedure("WCS_ADDTASKH_P",parms);
            return true;
        }

        public DataTable GetEventLog(string str, DateTime dts, DateTime dte, int ch)
        {
            string sql = "select * from (select eventlog,to_char(createtime,'yyyy-mm-dd hh24:mi:ss') createtime,case when eventlog like '%失败%' or eventlog like '%无法%' or eventlog like '%故障%' then '0' else '1' end logtype from wcs_eventlog where warehouseid='" + WareHouseID + "' and (createtime between to_date('" + dts + "','yyyy-mm-dd hh24:mi:ss')" + " and to_date('" + dte + "','yyyy-mm-dd hh24:mi:ss'))) where '1'='1'";
            if (!string.IsNullOrEmpty(str))
            {
                sql += " and eventlog like '%" + str + "%'";
            }
            if (ch == 1)
            {
                sql += " and logtype='0'";
            }
            sql += " order by createtime desc";
            return helper.ExecuteQueryBySql(sql);
        }
        public string GetItemCode(string taskNO)
        {
                string sql = $@"select i.item_code from wcs_taskinfo wt
                                inner join swms_wi_item i on wt.itemno = i.t_id
                                where wt.taskno = '{taskNO}'";
                return helper.ExecuteQueryBySql(sql)?.Rows[0]["item_code"].ToString();
        }
        public bool InsertEventlog(string text)
        {
            string sql = $@"insert into wcs_eventlog(warehouseid, logid, logtext, createtime)values('{WareHouseID}',sys_guid(),'{text}',sysdate)";
            return helper.ExecuteNonQueryBySql(sql);
        }

        public enum UpdateFlag
        {
            Add = 1,
            Minus = 2,
        }
        public enum STVCacheType
        {
            Cache_Large = 1,
            Cache_Small = 2
        }

        //更新剩余缓存数量
        public void UpdateSTVCache(UpdateFlag flag, STVCacheType cacheType, int count)
        {
            string sql = string.Empty;
            switch (flag)
            {
                case UpdateFlag.Add:
                    sql = $@"update stvcache t set t.usedcache = t.usedcache - {count},t.leftcache = t.leftcache + {count} where t.type = '{cacheType}'";
                    break;
                case UpdateFlag.Minus:
                    sql = $@"update stvcache t set t.usedcache = t.usedcache + {count},t.leftcache = t.leftcache - {count} where t.type = '{cacheType}'";
                    break;
            }
            helper.ExecuteNonQueryBySql(sql);
        }

        public bool UpdateSTVCacheInit(STVCacheType cacheType, int totalCount, int leftCount)
        {
            string sql = $@"update stvcache t set t.totalcache = {totalCount} ,t.leftcache = {leftCount} where t.type = '{cacheType}' ";
            return helper.ExecuteNonQueryBySql(sql);
        }


        public DataTable GetSTVCacheInfo(STVCacheType cacheType)
        {
            string sql = $@"select t.totalcache,t.leftcache from stvcache t where t.type = '{cacheType}'";
            return helper.ExecuteQueryBySql(sql);
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public DataTable GetEquipment(string type, string orderby)
        {
            string sql = $"select equipmentnumber, equipmentid from Equipment where typecode = '{type}'  order by {orderby}";
            return helper.ExecuteQueryBySql(sql);
        }

        /// <summary>
        /// 插入站台数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool InsertManyStation(List<WCS_OPCITEM> list)
        {
            List<string> listSql = new List<string>();
            string sql = string.Empty;
            foreach (WCS_OPCITEM item in list)
            {
                sql = $@"  
                        insert into wcs_opcitem
                        (
                            itemid,
                            itemname,
                            groupname,
                            objects,
                            datetype,
                            signaltype,
                            value,
                            stationno,
                            remark,
                            userid,
                            createtime,
                            warehouseid,
                            status,
                            stationtype,
                            locationx,
                            locationy,
                            width,
                            height,
                            belong,
                            direction,
                            zonecode,
                            areacode,
                            arrowdirection,
                            textdescription

                        )
                        values
                        (
                        '{item.Stationno}',
                        '{item.Stationno}',
                        '{item.Groupname}',
                        '{item.Objects}',
                        '{item.Datatype}',
                        '{item.Signaltype}',
                        '{item.Value}',
                        '{item.Stationno}',
                        '{item.Remark}',
                        '{item.Userid}',
                         sysdate,
                        '{item.Warehouseid}',
                        '{item.Status}',
                        '{item.Stationtype}',
                        '{item.Locationx}',
                        '{item.Locationy}',
                        '{item.Width}',
                        '{item.Height}',
                        '{item.Belong}',
                        '{item.Direction}',
                        '{item.ZoneCode}',
                        '{item.AreaCode}',
                        '{item.ArrowDirection}',
                        ''
                        )
                        ";
                listSql.Add(sql);
            }
            return helper.ExecuteManyNonQueryBySql(listSql);
        }

        public bool UpdateStationInfo(WCS_OPCITEM item)
        {
            string sql = $@"update wcs_opcitem t
                            set t.stationtype = '{item.Stationtype}',
                            t.width = '{item.Width}',
                            t.height = '{item.Height}',
                            t.locationx = '{item.Locationx}',
                            t.locationy = '{item.Locationy}',
                            t.direction = '{item.Direction}',
                            t.signaltype = '{item.Signaltype}',
                            t.value = '{item.Value}',
                            t.belong = '{item.Belong}',
                            t.arrowdirection = '{item.ArrowDirection}',
                            t.textdescription = '{item.TextDescription}',
                            t.status = '{item.Status}'
                            where t.stationno = '{item.Stationno}'";
            return helper.ExecuteNonQueryBySql(sql);
        }
    }


}
