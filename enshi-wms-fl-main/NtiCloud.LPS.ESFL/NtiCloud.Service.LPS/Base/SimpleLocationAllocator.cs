using Newtonsoft.Json;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    public class SimpleLocationAllocator : WmsObject, ILocationAllocator
    {
        public string AllocateLocation(string warehouseID,
            string itemID, string typeCode, string workFlowHeadID, string checkState)
        {
            DataRow row = AllocateEmptyLocatioin();
            return row["t_id"].ToString();
        }

        private DataRow AllocateEmptyLocatioin()
        {
            DataRow row = Location.GetEmptyLocation();

            //Location.UpdateLocationStockState(row["t_id"].ToString(),
            //    "P", row["stock_state"].ToString());

            return row;
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

        public string AllocateInventory(string warehouseID, string itemID, string withBarcode)
        {
            throw new NotImplementedException();
        }

        public string AllocateLocation(string warehouseID, string itemID, string batchNo)
        {
            throw new NotImplementedException();
        }

        public string AllocateLocation(string itemID, string batchNo, string packingType, string lastLocationNo)
        {
            throw new NotImplementedException();
        }

        public string AllocateInventory(InventoryAllocationParam param)
        {
            throw new NotImplementedException();
        }

        public string AllocateInventory(Dictionary<string, object> inputParams, string definedRule = null, string definedParam  = null)
        {
            throw new NotImplementedException();
        }

        public string AllocateLocation(Dictionary<string, object> inputParams, string definedRule = null, string definedParam  = null)
        {
            throw new NotImplementedException();
        }
    }
}
