using Newtonsoft.Json;
using NtiCloud.Service.WMS;
using NtiCloud.Kernel;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    public class LocationAllocator : ILocationAllocator
    {
        public string AllocateInventory(string warehouseID, string itemID, string withBarcode)
        {
            string locationKey = "O_LOCATION_NO";
            string successKey = "O_Result";
            string msgKey = "o_msg";
            //batchNo = "789";

            List<Tuple<string, string, ParameterDirection>> param =
                new List<Tuple<string, string, ParameterDirection>>
            {
                        new Tuple<string, string, ParameterDirection>(
                            "I_WAREHOUSEID", warehouseID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_ITEMID", itemID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_BARCODE", withBarcode, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            //locationKey, string.Empty, ParameterDirection.Output),
                            locationKey, "", ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            //successKey, string.Empty, ParameterDirection.Output),
                            successKey, "", ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            //msgKey, string.Empty, ParameterDirection.Output),
                            msgKey, "", ParameterDirection.Output),
            };
            LogHelper.LogDebug(JsonConvert.SerializeObject(param));

            // 调用存储过程
            var outputs = DBHelper.RunProc(Context.DataSource + "LOC_Flat_OUT", param, "分配出库货位");

            var success = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == successKey)
                .FirstOrDefault()?.Value.ToString();
            var msg = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == msgKey)
                .FirstOrDefault()?.Value.ToString();
            var location = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == locationKey)
                .FirstOrDefault()?.Value.ToString();

            LogHelper.LogDebug($"success={success}");
            LogHelper.LogDebug($"msg={msg}");
            LogHelper.LogDebug($"location={location}");

            if (success == null)
            {
                throw new WmsException(-606, $"货位分配失败！可能是存储过程不存在或未编译！");
                //throw new WmsException(-606, msg);
            }
            if (!success.Equals("1"))
            {
                throw new WmsException(-606, msg);
                //throw new WmsException(-605, $"货位分配失败！请查看数据库日志");
            }

            return location;
        }

        public string AllocateInventory(InventoryAllocationParam param)
        {
            string locationKey = "O_LOCATION_NO";
            string successKey = "O_Result";
            string msgKey = "o_msg";
            //batchNo = "789";

            List<Tuple<string, string, ParameterDirection>> p =
                new List<Tuple<string, string, ParameterDirection>>
            {
                        new Tuple<string, string, ParameterDirection>(
                            "I_WAREHOUSEID", param.WarehouseID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_ITEMID", param.ItemID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_BARCODE", param.BarcodeFlag, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_batch_no", param.BatchNo, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "i_lastlocation_no", param.LastLocation, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "i_fraction", param.PartialFlag, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "i_Storage_Locgp", param.LocationGroupID2, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "i_expiry_date", param.SaftyPeriod.ToString(), ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            //locationKey, string.Empty, ParameterDirection.Output),
                            locationKey, "", ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            //successKey, string.Empty, ParameterDirection.Output),
                            successKey, "", ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            //msgKey, string.Empty, ParameterDirection.Output),
                            msgKey, "", ParameterDirection.Output),
            };
            LogHelper.LogDebug(JsonConvert.SerializeObject(param));

            // 调用存储过程
            var proc = WmsConfig.GetOrDefault("Outbound:OutboundAllocationProc", "LOC_Flat_OUT");
            proc = Context.DataSource + proc;
            var outputs = DBHelper.RunProc(proc, p, "分配出库货位");

            var success = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == successKey)
                .FirstOrDefault()?.Value.ToString();
            var msg = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == msgKey)
                .FirstOrDefault()?.Value.ToString();
            var location = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == locationKey)
                .FirstOrDefault()?.Value.ToString();

            LogHelper.LogDebug($"success={success}");
            LogHelper.LogDebug($"msg={msg}");
            LogHelper.LogDebug($"location={location}");

            if (success == null)
            {
                throw new WmsException(-606, $"货位分配失败！可能是存储过程不存在或未编译！");
                //throw new WmsException(-606, msg);
            }
            if (!success.Equals("1"))
            {
                throw new WmsException(-606, msg);
                //throw new WmsException(-605, $"货位分配失败！请查看数据库日志");
            }

            return location;
        }

        private string getResult(List<IDbDataParameter> outputs, string key)
        {
            string r = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == key)
                .FirstOrDefault()?.Value.ToString();

            LogHelper.LogDebug($"{key}={r}");

            return r;
        }

        private DataRow[] GetCodeRule(string ruleCode)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "rule_code", ruleCode },
            };
            string sql = @"
select rh.rule_name, rh.rule_code, rd.* 
  from &DataSource&wms_sb_code_rule_d rd
  left join &DataSource&wms_sb_code_rule_h rh on rd.rule_h_id = rh.t_id
 where rd.client_id='&CLIENT_ID&' and rd.org_id='&ORG_ID&' 
   and rd.warehouse_id = @warehouse 
   and rh.rule_code = @rule_code
 order by end_position
";
            DataTable dt = DBHelper.Select(sql, param, "查询货位规则");

            return dt.AsEnumerable().ToArray();
        }


        private string BuildValueParam(string definedParam, Dictionary<string, object> inputParams)
        {
            DataRow[] parameters = GetCodeRule(definedParam);
            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in parameters)
            //foreach (KeyValuePair<string, string> t in inputParams)
            {
                string detailCode = row["ruledetailcode"].ToString().ToUpper();
                if (inputParams.ContainsKey(detailCode))
                {
                    sb.Append(inputParams[detailCode].ToString());
                }
                sb.Append(row["start_position"].ToString());  // delimiter
            }

            return sb.ToString();
        }

        public string AllocateInventory(Dictionary<string, object> inputParams, 
            string definedRule = null, string definedParam = null)
        {
            string locationKey = "o_locationid";
            string successKey = "o_ifsucess";
            string msgKey = "o_msg";

            //definedRule ??= WmsConfig.GetOrDefault("Outbound:AllocationRule", "work_task_4");
            definedParam ??= WmsConfig.GetOrDefault("Outbound:AllocationParams", "location_rules");

            List<Tuple<string, string, ParameterDirection>> p =
                new List<Tuple<string, string, ParameterDirection>>()
            {
                new Tuple<string, string, ParameterDirection>(
                    "i_warehouseid", inputParams["I_WAREHOUSEID"].ToString(), ParameterDirection.Input),
                new Tuple<string, string, ParameterDirection>(
                    "i_vale", BuildValueParam(definedParam, inputParams), ParameterDirection.Input),
                new Tuple<string, string, ParameterDirection>(
                    "i_typecode", definedRule, ParameterDirection.Input),
                new Tuple<string, string, ParameterDirection>(
                    locationKey, "", ParameterDirection.Output),
                new Tuple<string, string, ParameterDirection>(
                    successKey, "", ParameterDirection.Output),
                new Tuple<string, string, ParameterDirection>(
                    msgKey, "", ParameterDirection.Output),
            };
            LogHelper.LogDebug(JsonConvert.SerializeObject(p));

            // 调用存储过程
            var proc = Context.DataSource + WmsConfig.GetOrDefault("Outbound:OutboundAllocationProc", "loc_p_getlocation");//Context.DataSource  "wms."
            var outputs = DBHelper.RunProc(proc, p, "分配出库货位");

            var success = getResult(outputs, successKey);
            var msg = getResult(outputs, msgKey);
            var location = getResult(outputs, locationKey);

            if (success == null)
            {
                throw new WmsException(-606, $"货位分配失败！可能是存储过程不存在或未编译！");
                //throw new WmsException(-606, msg);
            }
            if (!success.Equals("1"))
            {
                throw new WmsException(-605, msg);
                //throw new WmsException(-605, $"货位分配失败！请查看数据库日志");
            }

            return location;
        }

        public string AllocateInventoryByStoreProc(Dictionary<string, object> inputParams)
        {
            string locationKey = "O_LOCATION_NO";
            string successKey = "O_Result";
            string msgKey = "o_msg";

            string definedParams = WmsConfig.GetOrDefault(
                "Outbound:AllocationParams", "I_WAREHOUSEID,I_ITEMID,I_BATCH_NO,I_SPEC");
            List<string> paramList = definedParams.Split(',')
                .Select(r => r.Trim().ToUpper()).ToList();

            List<Tuple<string, string, ParameterDirection>> p =
                new List<Tuple<string, string, ParameterDirection>>();

            foreach (string s in paramList)
            //foreach (KeyValuePair<string, string> t in inputParams)
            {
                if (!inputParams.ContainsKey(s))
                {
                    throw new WmsException(631, $"指定了无法获得的存储过程参数{s}");
                }

                string v = inputParams[s].ToString();
                p.Add(new Tuple<string, string, ParameterDirection>(
                    s, v, ParameterDirection.Input));
            }
            p.AddRange(new List<Tuple<string, string, ParameterDirection>>
            {
                new Tuple<string, string, ParameterDirection>(
                    locationKey, "", ParameterDirection.Output),
                new Tuple<string, string, ParameterDirection>(
                    successKey, "", ParameterDirection.Output),
                new Tuple<string, string, ParameterDirection>(
                    msgKey, "", ParameterDirection.Output),
            });
            LogHelper.LogDebug(JsonConvert.SerializeObject(inputParams));

            // 调用存储过程
            //var proc = Context.DataSource + WmsConfig.GetOrDefault("Outbound:OutboundAllocationProc", "LOC_Flat_OUT");
            var proc = Context.DataSource + WmsConfig.GetOrDefault("Outbound:OutboundAllocationProcTest", "LOC_Flat_OUT_Test");
            var outputs = DBHelper.RunProc(proc, p, "分配出库货位");

            var success = getResult(outputs, successKey);
            var msg = getResult(outputs, msgKey);
            var location = getResult(outputs, locationKey);

            if (success == null)
            {
                throw new WmsException(-606, $"货位分配失败！可能是存储过程不存在或未编译！");
                //throw new WmsException(-606, msg);
            }
            if (!success.Equals("1"))
            {
                throw new WmsException(-606, msg);
                //throw new WmsException(-605, $"货位分配失败！请查看数据库日志");
            }

            return location;
        }

        //public ResultInfo AllocateLocation(string warehouseID, string itemID,
        //    string typeCode, string workFlowHeadID, string checkState)
        //{
        //    ResultInfo info = new ResultInfo();

        //    string locationKey = "O_LOCATIONID";
        //    string successKey = "O_IFSUCESS";

        //    List<Tuple<string, string, ParameterDirection>> param =
        //        new List<Tuple<string, string, ParameterDirection>>
        //    {
        //                new Tuple<string, string, ParameterDirection>(
        //                    "I_WAREHOUSEID", warehouseID, ParameterDirection.Input),
        //                new Tuple<string, string, ParameterDirection>(
        //                    "I_ITEMID", itemID, ParameterDirection.Input),
        //                new Tuple<string, string, ParameterDirection>(
        //                    "I_TYPECODE", typeCode, ParameterDirection.Input),
        //                new Tuple<string, string, ParameterDirection>(
        //                    "I_WORKFLOWHEADID", workFlowHeadID, ParameterDirection.Input),
        //                new Tuple<string, string, ParameterDirection>(
        //                    "I_CHECKSTATE", checkState, ParameterDirection.Input),
        //                new Tuple<string, string, ParameterDirection>(
        //                    locationKey, string.Empty, ParameterDirection.Output),
        //                new Tuple<string, string, ParameterDirection>(
        //                    successKey, string.Empty, ParameterDirection.Output),
        //    };
        //    LogHelper.LogDebug(JsonConvert.SerializeObject(param));

        //    // 调用存储过程
        //    var outputs = KernelDataBase.DBRunProcedure(CurrentUser.Tran.Connection,
        //        CurrentUser.Tran, CurrentUser.Info.UserDbType, "loc_doublegetlocoutbound_p",
        //        out int resCount, out string resMsg, param);

        //    var success = outputs?
        //        .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == successKey)
        //        .FirstOrDefault()?.Value.ToString();
        //    var location = outputs?
        //        .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == locationKey)
        //        .FirstOrDefault()?.Value.ToString();

        //    LogHelper.LogDebug($"success={success}");
        //    LogHelper.LogDebug($"location={location}");

        //    if (success == null)
        //    {
        //        info.ErrorCode = -606;
        //        info.ErrMsg = $"货位分配失败！可能是存储过程不存在或未编译！";
        //        return info;
        //    }
        //    if (!success.Equals("1"))
        //    {
        //        info.ErrorCode = -605;
        //        info.ErrMsg = $"货位分配失败！";
        //        return info;
        //    }

        //    info.Result = location;

        //    return info;
        //}
        public string AllocateLocation(string warehouseID,
            string itemID, string batchNo)
        {
            string locationKey = "O_LOCATION_NO";
            string successKey = "O_Result";
            string msgKey = "o_msg";
            //batchNo = "789";

            List<Tuple<string, string, ParameterDirection>> param =
                new List<Tuple<string, string, ParameterDirection>>
            {
                        new Tuple<string, string, ParameterDirection>(
                            "I_WAREHOUSEID", warehouseID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_ITEMID", itemID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_batch_no", batchNo, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            //locationKey, string.Empty, ParameterDirection.Output),
                            locationKey, "", ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            //successKey, string.Empty, ParameterDirection.Output),
                            successKey, "", ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            //msgKey, string.Empty, ParameterDirection.Output),
                            msgKey, "", ParameterDirection.Output),
            };

            LogHelper.LogDebug(JsonConvert.SerializeObject(param));
            // 调用存储过程
            /*
            var outputs = KernelDataBase.DBRunProcedure(
                CurrentUser.Tran.Connection,
                CurrentUser.Tran, CurrentUser.Info.UserDbType, "loc_doublegetlocoutbound_p",
                out int resCount, out string resMsg, param);
                */
            var proc = WmsConfig.GetOrDefault("Inbound:InboundAllocationProc", "LOC_Flat_IN");
            var outputs = DBHelper.RunProc(proc, param, "分配入库货位");

            var success = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == successKey)
                .FirstOrDefault()?.Value.ToString();
            var msg = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == msgKey)
                .FirstOrDefault()?.Value.ToString();
            var location = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == locationKey)
                .FirstOrDefault()?.Value.ToString();

            LogHelper.LogDebug($"success={success}");
            LogHelper.LogDebug($"msg={msg}");
            LogHelper.LogDebug($"location={location}");

            if (success == null)
            {
                throw new WmsException(-606, $"货位分配失败！可能是存储过程不存在或未编译！");
                //throw new WmsException(-606, msg);
            }
            if (!success.Equals("1"))
            {
                throw new WmsException(-606, msg);
                //throw new WmsException(-605, $"货位分配失败！请查看数据库日志");
            }

            return location;
        }
        /// <summary>
        /// 返回LocationCode
        /// </summary>
        /// <param name="inputParams"></param>
        /// <returns></returns>
        public string AllocateLocation(Dictionary<string, object> inputParams,
            string definedRule = null, string definedParam = null)
        {
            string locationKey = "o_locationid";
            string successKey = "o_ifsucess";
            string msgKey = "o_msg";

            //definedRule ??= WmsConfig.GetOrDefault("Inbound:AllocationRule", "work_task_3");
            definedParam ??= WmsConfig.GetOrDefault("Inbound:AllocationParams", "location_rules");

            List<Tuple<string, string, ParameterDirection>> p =
                new List<Tuple<string, string, ParameterDirection>>()
            {
                new Tuple<string, string, ParameterDirection>(
                    "i_warehouseid", inputParams["I_WAREHOUSEID"].ToString(), ParameterDirection.Input),
                new Tuple<string, string, ParameterDirection>(
                    "i_vale", BuildValueParam(definedParam, inputParams), ParameterDirection.Input),
                new Tuple<string, string, ParameterDirection>(
                    "i_typecode", definedRule, ParameterDirection.Input),
                new Tuple<string, string, ParameterDirection>(
                    locationKey, "", ParameterDirection.Output),
                new Tuple<string, string, ParameterDirection>(
                    successKey, "", ParameterDirection.Output),
                new Tuple<string, string, ParameterDirection>(
                    msgKey, "", ParameterDirection.Output),
            };
            LogHelper.LogDebug(JsonConvert.SerializeObject(p));

            // 调用存储过程
            var proc = Context.DataSource + WmsConfig.GetOrDefault("Inbound:InboundAllocationProc", "loc_p_getlocation");//"wms."   Context.DataSource
            var outputs = DBHelper.RunProc(proc, p, "分配入库货位");

            var success = getResult(outputs, successKey);
            var msg = getResult(outputs, msgKey);
            var location = getResult(outputs, locationKey);

            if (success == null)
            {
                throw new WmsException(-606, $"货位分配失败！可能是存储过程不存在或未编译！");
                //throw new WmsException(-606, msg);
            }
            if (!success.Equals("1"))
            {
                throw new WmsException(-605, msg);
                //throw new WmsException(-605, $"货位分配失败！请查看数据库日志");
            }

            return location;
        }

        public string AllocateLocationV1(
            Dictionary<string, object> inputParams)
        {
            string locationKey = "O_LOCATION_NO";
            string successKey = "O_Result";
            string msgKey = "o_msg";

            string definedParams = WmsConfig.GetOrDefault(
                "Inbound:AllocationParams",
                "I_WAREHOUSEID,I_ITEMID,I_BATCH_NO,I_PACKING_TYPE,I_LASTLOCATION_NO");
            List<string> paramList = definedParams.Split(',')
                .Select(s => s.Trim().ToUpper()).ToList();

            List<Tuple<string, string, ParameterDirection>> p =
                new List<Tuple<string, string, ParameterDirection>>();

            foreach (string s in paramList)
            //foreach (KeyValuePair<string, string> t in inputParams)
            {
                if (!inputParams.ContainsKey(s))
                {
                    throw new WmsException(631, "指定了无法获得的存储过程参数" + s);
                }

                string v = inputParams[s].ToString();
                p.Add(new Tuple<string, string, ParameterDirection>(
                    s, v, ParameterDirection.Input));
            }
            p.AddRange(new List<Tuple<string, string, ParameterDirection>>
            {
                new Tuple<string, string, ParameterDirection>(
                    locationKey, "", ParameterDirection.Output),
                new Tuple<string, string, ParameterDirection>(
                    successKey, "", ParameterDirection.Output),
                new Tuple<string, string, ParameterDirection>(
                    msgKey, "", ParameterDirection.Output),
            });
            LogHelper.LogDebug(JsonConvert.SerializeObject(inputParams));

            // 调用存储过程
            var proc = WmsConfig.GetOrDefault("Inbound:InboundAllocationProc", "LOC_Flat_IN");
            var outputs = DBHelper.RunProc(proc, p, "分配入库货位");

            var success = getResult(outputs, successKey);
            var msg = getResult(outputs, msgKey);
            var location = getResult(outputs, locationKey);

            if (success == null)
            {
                throw new WmsException(-606, $"货位分配失败！可能是存储过程不存在或未编译！");
                //throw new WmsException(-606, msg);
            }
            if (!success.Equals("1"))
            {
                throw new WmsException(-606, msg);
                //throw new WmsException(-605, $"货位分配失败！请查看数据库日志");
            }

            return location;
        }

        public string AllocateLocation(
            string itemID, string batchNo, string packingType, string lastLocationNo)
        {
            string locationKey = "O_LOCATION_NO";
            string successKey = "O_Result";
            string msgKey = "o_msg";
            //batchNo = "789";

            List<Tuple<string, string, ParameterDirection>> param =
                new List<Tuple<string, string, ParameterDirection>>
            {
                        new Tuple<string, string, ParameterDirection>(
                            "I_WAREHOUSEID", Context.WarehouseID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_ITEMID", itemID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_batch_no", batchNo, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_packing_type", packingType, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_lastlocation_no", batchNo, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            //locationKey, string.Empty, ParameterDirection.Output),
                            locationKey, "", ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            //successKey, string.Empty, ParameterDirection.Output),
                            successKey, "", ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            //msgKey, string.Empty, ParameterDirection.Output),
                            msgKey, "", ParameterDirection.Output),
            };

            LogHelper.LogDebug(JsonConvert.SerializeObject(param));
            // 调用存储过程
            /*
            var outputs = KernelDataBase.DBRunProcedure(
                CurrentUser.Tran.Connection,
                CurrentUser.Tran, CurrentUser.Info.UserDbType, "loc_doublegetlocoutbound_p",
                out int resCount, out string resMsg, param);
                */
            var proc = WmsConfig.GetOrDefault("Inbound:InboundAllocationProc", "LOC_Flat_IN");
            var outputs = DBHelper.RunProc(proc, param, "分配入库货位");

            var success = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == successKey)
                .FirstOrDefault()?.Value.ToString();
            var msg = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == msgKey)
                .FirstOrDefault()?.Value.ToString();
            var location = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == locationKey)
                .FirstOrDefault()?.Value.ToString();

            LogHelper.LogDebug($"success={success}");
            LogHelper.LogDebug($"msg={msg}");
            LogHelper.LogDebug($"location={location}");

            if (success == null)
            {
                throw new WmsException(-606, $"货位分配失败！可能是存储过程不存在或未编译！");
                //throw new WmsException(-606, msg);
            }
            if (!success.Equals("1"))
            {
                throw new WmsException(-606, msg);
                //throw new WmsException(-605, $"货位分配失败！请查看数据库日志");
            }

            return location;
        }

        public string AllocateLocation(string warehouseID,
            string itemID, string typeCode, string workFlowHeadID, string checkState)
        {
            string locationKey = "O_LOCATIONID";
            string successKey = "O_IFSUCESS";

            List<Tuple<string, string, ParameterDirection>> param =
                new List<Tuple<string, string, ParameterDirection>>
            {
                        new Tuple<string, string, ParameterDirection>(
                            "I_WAREHOUSEID", warehouseID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_ITEMID", itemID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_TYPECODE", typeCode, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_WORKFLOWHEADID", workFlowHeadID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_CHECKSTATE", checkState, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            locationKey, string.Empty, ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            successKey, string.Empty, ParameterDirection.Output),
            };

            LogHelper.LogDebug(JsonConvert.SerializeObject(param));
            // 调用存储过程
            /*
            var outputs = KernelDataBase.DBRunProcedure(
                CurrentUser.Tran.Connection,
                CurrentUser.Tran, CurrentUser.Info.UserDbType, "loc_doublegetlocoutbound_p",
                out int resCount, out string resMsg, param);
                */
            var outputs = DBHelper.RunProc("", param, "分配入库货位");

            var success = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == successKey)
                .FirstOrDefault()?.Value.ToString();
            var location = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == locationKey)
                .FirstOrDefault()?.Value.ToString();

            LogHelper.LogDebug($"success={success}");
            LogHelper.LogDebug($"location={location}");

            if (success == null)
            {
                throw new WmsException(-606, $"货位分配失败！可能是存储过程不存在或未编译！");
            }
            if (!success.Equals("1"))
            {
                throw new WmsException(-605, $"货位分配失败！请查看数据库日志");
            }

            return location;
        }

        public string AllocateLocation(string warehouseID, string itemID,
            string typeCode, string workFlowHeadID, string checkState, string ownerID, string isFullPallet)
        {
            string locationKey = "O_LOCATIONID";
            string successKey = "O_IFSUCESS";

            List<Tuple<string, string, ParameterDirection>> param =
                new List<Tuple<string, string, ParameterDirection>>
            {
                        new Tuple<string, string, ParameterDirection>(
                            "I_WAREHOUSEID", warehouseID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_ITEMID", itemID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_TYPECODE", typeCode, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_WORKFLOWHEADID", workFlowHeadID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_CHECKSTATE", checkState, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_OWNER", ownerID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_FULLPALLET", isFullPallet, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            locationKey, string.Empty, ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            successKey, string.Empty, ParameterDirection.Output),
            };

            LogHelper.LogDebug(JsonConvert.SerializeObject(param));
            // 调用存储过程
            /*
            var outputs = KernelDataBase.DBRunProcedure(CurrentUser.Tran.Connection,
                CurrentUser.Tran, CurrentUser.Info.UserDbType, "loc_doublegetlocoutbound_cp",
                out int resCount, out string resMsg, param);
                */
            var outputs = DBHelper.RunProc("", param, "");

            var success = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == successKey)
                .FirstOrDefault()?.Value.ToString();
            var location = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == locationKey)
                .FirstOrDefault()?.Value.ToString();

            LogHelper.LogDebug($"success={success}");
            LogHelper.LogDebug($"location={location}");

            if (success == null)
            {
                throw new WmsException(-606, $"货位分配失败！可能是存储过程不存在或未编译！");
            }
            if (!success.Equals("1"))
            {
                throw new WmsException(-605, $"货位分配失败！请查看数据库日志");
            }

            return location;
        }

        public decimal GetTimeWindow(string warehouseID,
            string itemID, string typeCode, string workFlowHeadID)
        {
            string successKey = "O_IFSUCESS";
            string timeSortSpan = "O_TIMESORTSPAN";

            List<Tuple<string, string, ParameterDirection>> param =
                new List<Tuple<string, string, ParameterDirection>>
            {
                        new Tuple<string, string, ParameterDirection>(
                            "I_WAREHOUSEID", warehouseID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_ITEMID", itemID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_TYPECODE", typeCode, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_WORKFLOWHEADID", workFlowHeadID, ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_P_LOG_NO", " ", ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "I_LOG_NO", " ", ParameterDirection.Input),
                        new Tuple<string, string, ParameterDirection>(
                            "O_LOCATIONRULESID", string.Empty, ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            "O_ISEVERY", string.Empty, ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            "O_TIMESORTENABLED", string.Empty, ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            timeSortSpan, string.Empty, ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            successKey, string.Empty, ParameterDirection.Output),
                        new Tuple<string, string, ParameterDirection>(
                            "O_MSG", string.Empty, ParameterDirection.Output),
            };

            LogHelper.LogDebug(JsonConvert.SerializeObject(param));
            // 调用存储过程
            /*
            var outputs = KernelDataBase.DBRunProcedure(CurrentUser.Tran.Connection,
                CurrentUser.Tran, CurrentUser.Info.UserDbType, "LOC_ITEMMATCHING",
                out int resCount, out string resMsg, param);
                */
            var outputs = DBHelper.RunProc("", param, "");

            var success = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == successKey)
                .FirstOrDefault()?.Value.ToString();
            var timeWindow = outputs?
                .Where(v => v.ParameterName.Replace("@", "").Replace(":", "") == timeSortSpan)
                .FirstOrDefault()?.Value.ToString();

            LogHelper.LogDebug($"success={success}");
            LogHelper.LogDebug($"timeWindow={timeWindow}");

            if (success == null)
            {
                return -1;
            }
            if (!success.Equals("1"))
            {
                return -1;
            }

            return decimal.Parse("0" + timeWindow);
        }


    }
}
