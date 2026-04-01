using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMSInterface;
using System.Data;
using Newtonsoft.Json.Linq;

namespace NTI.WMS.CallTerminalSystem.DAL
{
    public class CallPalletDAL
    {
        //客户端发出要料呼叫         
        public static string ClientCallPallet(string url, string sessionKey, string warehouseId, string dockNumber,
            string hasLGV, string runsType, string token = null)
        {
            url += "api/wms/MachineMaterialService/CreateOutBoundByMachineDock";
            JObject jo = new JObject();
            jo.Add("machine", sessionKey);
            jo.Add("dock", warehouseId);
            jo.Add("productPlanOutId", dockNumber);
            jo.Add("shiftId", string.Empty);
            jo.Add("teamId", runsType);
            string result = NitCloudService.PostGateway(url, jo.ToString(), token);
            if (string.IsNullOrEmpty(result)) throw new Exception("要料调用微服务失败！");
            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                return entity["errmsg"].ToString();
            }
            return string.Empty;  //成功
        }

        /// <summary>
        /// 客户端发出要料呼叫
        /// </summary>
        /// <param name="url"></param>
        /// <param name="machineId"></param>
        /// <param name="hasLGV"></param>
        /// <param name="dockId"></param>
        /// <param name="productPlanOutId"></param>
        /// <param name="shiftId"></param>
        /// <param name="teamId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string ClientCallPallet(string url, string machineId, string hasLGV, string dockId, string productPlanOutId, string shiftId, string teamId, string token)
        {
            url += "api/wms/MachineMaterialService/CreateOutBoundByMachineDock";
            JObject jo = new JObject();
            jo.Add("machine", machineId);
            jo.Add("dock", dockId);
            jo.Add("productPlanOutId", productPlanOutId);
            jo.Add("hasAGV", hasLGV);
            jo.Add("shiftId", shiftId);
            jo.Add("teamId", teamId);
            string result = NitCloudService.PostGateway(url, jo.ToString(), token);
            if (string.IsNullOrEmpty(result)) throw new Exception("要料调用微服务失败！");
            JObject entity = JObject.Parse(result);
            bool isSucceed = Convert.ToBoolean(entity["isSucceed"].ToString());
            if (isSucceed == false)
            {
                return entity["errmsg"].ToString();
            }
            return string.Empty;  //成功
        }

        //检查呼叫的实托盘是否有库存
        public static bool CheckContainerInventory(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber)
        {
            //return WCFService.CallEndWS.CheckContainerInventory(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                bool res = srv.CheckContainerInventory(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Class_WCFService.CloseService(srv);
            }
        }

        //客户端发出退空托盘+要料呼叫         
        public static int ClientReturnAndCallPallet(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber, string Model, string hasLGV, string Runs)
        {
            int rs = 0;
            WMSInterface.CallEndWS srv = null;

            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                rs = srv.ClientReturnAndCallPallet(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber, Model, hasLGV, Runs);
            }
            catch
            { }
            finally { }

            if (rs <= 0)
            {
                WriteLog.Write(UserName + DateTime.Now.ToString() + "退空托盘+要料操作失败", "ClientReturnAndCallPallet");
            }

            return rs;
        }
    }
}

