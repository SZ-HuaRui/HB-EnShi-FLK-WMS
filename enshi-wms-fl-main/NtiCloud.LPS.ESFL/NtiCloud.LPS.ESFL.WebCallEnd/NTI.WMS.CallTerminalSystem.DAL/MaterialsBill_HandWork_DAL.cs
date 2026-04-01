using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMSInterface;
using System.Data;

namespace NTI.WMS.CallTerminalSystem.DAL
{
    /// <summary>
    /// 手工领、退、接收
    /// </summary>
    public class MaterialsBill_HandWork_DAL
    {
        /// <summary>
        /// 创建手工领料单
        public static int CreateHandWorkMaterialBill(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string machineNumber, string dockNumber, ContainerEntity containerEntity)
        {
            //return WCFService.CallEndWS.CreateHandWorkMaterialBill(EquipmentNumber, UserName, UserPassword, WareHouseID, machineNumber, dockNumber, containerEntity,"");

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                int res = srv.CreateHandWorkMaterialBill(EquipmentNumber, UserName, UserPassword, WareHouseID, machineNumber, dockNumber, containerEntity, "");
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

        public static DataSet GetFinishedMaterialsBill_HandWork(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID)
        {
            //return WCFService.CallEndWS.GetFinishedMaterialsBill_HandWork(EquipmentNumber, UserName, UserPassword, WareHouseID);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                DataSet ds = srv.GetFinishedMaterialsBill_HandWork(EquipmentNumber, UserName, UserPassword, WareHouseID);
                return ds;
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

        //修改手工领料单的已发料量
        public static int UpdateSentItemQty(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string ContainerDetailID, decimal Qty, decimal odd1, decimal odd2, decimal odd3)
        {
            //return WCFService.CallEndWS.UpdateTempZoneAndSentItemQty(EquipmentNumber, UserName, UserPassword, WareHouseID, ContainerDetailID, Qty, odd1, odd2, odd3);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                int res = srv.UpdateTempZoneAndSentItemQty(EquipmentNumber, UserName, UserPassword, WareHouseID, ContainerDetailID, Qty, odd1, odd2, odd3);
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

        public static DataSet GetBillState(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID)
        {
            //return WCFService.InitializeWS.ChoiceState(WareHouseID, EquipmentNumber, UserName, UserPassword, "PreOutBound");

            WMSInterface.InitializeWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.InitializeWS>();
                DataSet ds = srv.ChoiceState(WareHouseID, EquipmentNumber, UserName, UserPassword, "PreOutBound");
                return ds;
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

        //获取暂存区有库存的物料数据        
        public static DataSet GetItemDataByZoneID(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID)
        {
            //return WCFService.CallEndWS.GetItemDataByZoneID(EquipmentNumber, UserName, UserPassword, WareHouseID, "");

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                DataSet ds = srv.GetItemDataByZoneID(EquipmentNumber, UserName, UserPassword, WareHouseID, "");
                return ds;
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

        //车间人员获取手工领料单数据 QWH 2009/06/17         
        public static DataSet GetMaterialsBill_HandWork_Warehouse(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber)
        {
            //return WCFService.CallEndWS.GetMaterialsBill_HandWork_Warehouse(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                DataSet ds = srv.GetMaterialsBill_HandWork_Warehouse(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber);
                return ds;
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

        //仓库人员获取未完成的手工领料单数据 QWH 2009/06/17        
        public static DataSet GetMaterialsBill_HandWork(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID)
        {
            //return WCFService.CallEndWS.GetMaterialsBill_HandWork(EquipmentNumber, UserName, UserPassword, WareHouseID);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                DataSet ds = srv.GetMaterialsBill_HandWork(EquipmentNumber, UserName, UserPassword, WareHouseID);
                return ds;
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

        //车间人员接收物料（手工发料） QWH 2009/06/19        
        public static int Client_AcceptMaterials_HandWork(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber, string BillID, string Runs)
        {
            WorkFlowResult rs = null;
            WMSInterface.CallEndWS srv = null;

            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                rs = srv.Client_AcceptMaterials_HandWork(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber, BillID, Runs, "", "");
            }
            catch
            { }
            finally { }

            if (rs == null || rs.Result <= 0)
            {
                WriteLog.Write(rs, "Client_AcceptMaterials");
            }

            return rs.Result;
        }

        //仓库人员确认发料（手工领料单） QWH 2009/06/19        
        public static int Client_SentMaterials_HandWork(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string BillID, ContainerEntity containerEntity)
        {
            WorkFlowResult rs = null;
            WMSInterface.CallEndWS srv = null;

            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                rs = srv.Client_SentMaterials_HandWork(EquipmentNumber, UserName, UserPassword, WareHouseID, BillID, containerEntity);
            }
            catch
            { }
            finally { }

            if (rs == null || rs.Result <= 0)
            {
                WriteLog.Write(rs, "Client_SentMaterials");
            }

            return rs.Result;
        }
        public static bool FinishManPowerCallTask(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string tasknumber)
        {
            //return WCFService.CallEndWS.FinishManPowerCallTask(EquipmentNumber, UserName, UserPassword, WareHouseID, tasknumber);

            WMSInterface.CallEndWS srv = null;
            try
            {
                srv = Class_WCFService.GetFactory<WMSInterface.CallEndWS>();
                bool res = srv.FinishManPowerCallTask(EquipmentNumber, UserName, UserPassword, WareHouseID, tasknumber);
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
    }
}
