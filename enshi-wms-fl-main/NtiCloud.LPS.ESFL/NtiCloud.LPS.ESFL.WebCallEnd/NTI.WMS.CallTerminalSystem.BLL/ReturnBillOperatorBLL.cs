using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTI.WMS.CallTerminalSystem.DAL;
using System.Data;
using WMSInterface;

namespace NTI.WMS.CallTerminalSystem.BLL
{
    public class ReturnBillOperatorBLL
    {
        string _equipmentNumber;
        string _userName;
        string _userPassword;
        string _warehouseID;

        public ReturnBillOperatorBLL(string equipmentNumber, string userName, string userPassword, string warehouseId)
        {
            this._equipmentNumber = equipmentNumber;
            this._userName = userName;
            this._userPassword = userPassword;
            this._warehouseID = warehouseId;
        }

        //获取机台未完成的交接单信息
        public DataTable GetMachineReturnMaterialBill(string url, string sessionKey, string warehouseId, string machineNumber)
        {
            try
            {
                return ReturnPalletDAL.GetMachineReturnMaterialBill(url, sessionKey, warehouseId, machineNumber);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取机台未完成的交接单信息错误:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 获取机台未完成的交接单信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="dockId">交接班站台id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable GetMachineReturnMaterialBill(string url, string sessionKey, string warehouseId, string schemeId, string dockId, string token)
        {
            try
            {
                return ReturnPalletDAL.GetMachineReturnMaterialBill(url, sessionKey, warehouseId, schemeId, dockId, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取机台未完成的交接单信息错误:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 查看接班信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="schemeId">交班的生产工单id</param>
        /// <param name="dockId">交接班站台id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable GetMachineReturnMaterialBillWhenChangeClass(string url, string sessionKey, string warehouseId, string schemeId, string dockId, string token)
        {
            try
            {
                return ReturnPalletDAL.GetMachineReturnMaterialBillWhenChangeClass(url, sessionKey, warehouseId, schemeId, dockId, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取机台未完成的交接单信息错误:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 接班时获取机台未完成的交接单信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="schemeId">交班的生产工单id</param>
        /// <param name="dockId">交接班站台id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable GetChangeClassAcceptInfo(string url, string sessionKey, string warehouseId, string schemeId, string dockId, string token)
        {
            try
            {
                return ReturnPalletDAL.GetChangeClassAcceptInfo(url, sessionKey, warehouseId, schemeId, dockId, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取机台未完成的交接单信息错误:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 获取机台未完成的交接单信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <returns></returns>
        public DataTable GetNoFinishMachineReturnMaterialBill(string url, string sessionKey, string warehouseId, string machineId,string token)
        {
            try
            {
                return ReturnPalletDAL.GetNoFinishMachineReturnMaterialBill(url, sessionKey, warehouseId, machineId, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取机台未完成的交接单信息错误:" + ex.Message);
            }
            return null;
        }


        /// <summary>
        /// 仓库人员退回某退料单 PC 2009/07/31
        /// </summary>
        /// <param name="BillID">退料单ID</param>
        /// <param name="PalletNumber">托盘号</param>
        /// <param name="ContainerID"></param>
        /// <param name="MachineNumber"></param>
        /// <param name="DockNumber"></param>
        /// <returns>大于0，操作成功</returns>
        public int Return_MaterialBillByBillID(string BillID, string PalletNumber, string ContainerID, string MachineNumber, string DockNumber)
        {
            int flag = 0;
            try
            {
                flag = ReturnPalletDAL.Return_MaterialBillByBillID(_equipmentNumber, _userName, _userPassword, _warehouseID, BillID, PalletNumber, ContainerID, MachineNumber, DockNumber);
            }
            catch
            {
                flag = 0;
            }
            finally { }

            return flag;
        }

        /// <summary>
        /// 仓库人员确认接收退料单操作 QWH 2009/06/17  
        /// </summary>
        /// <param name="containerEntity">包含退料单数据的实体类</param>
        /// <param name="returnBillID">退料单ID</param>
        /// <returns>大于0，操作成功</returns>    
        public int ClientConfirmReturnBill_Warehouse(ContainerEntity containerEntity, string returnBillID)
        {
            int flag = 0;
            try
            {
                flag = ReturnPalletDAL.ClientConfirmReturnBill_Warehouse(_equipmentNumber, _userName, _userPassword, _warehouseID, containerEntity, returnBillID);
            }
            catch
            { flag = 0; }
            finally { }

            return flag;
        }

        /// <summary>
        /// 客户端确认接收交接班物料操作 QWH 2009/06/16
        /// </summary>
        /// <param name="machineNumber">机台号</param>
        /// <param name="returnBillID">交接单/退料单ID</param>
        /// <returns>大于0，操作成功</returns>        
        public int ClientConfirmReturnBill_WorkShop(string machineNumber, string returnBillID, string Runs)
        {
            int flag = 0;
            try
            {
                //flag = ReturnPalletDAL.ClientConfirmReturnBill(_equipmentNumber, _userName, _userPassword, _warehouseID, machineNumber, returnBillID, Runs);
            }
            catch
            { flag = 0; }
            finally { }

            return flag;
        }

        /// <summary>
        /// 根据ContainerDetailID 修改退料单或交接单中，某物料的退料量
        /// </summary>
        /// <param name="ContainerDetailID">容器明细ID</param>
        /// <param name="Qty">数量</param>
        /// <param name="odd1">零头1</param>
        /// <param name="odd2">零头2</param>
        /// <param name="odd3">零头3</param>
        /// <returns>大于0: 修改成功</returns>
        public int UpdateReturnItemQty(string containerDetailID, decimal qty, decimal odd1, decimal odd2, decimal odd3)
        {
            int flag = 0;
            try
            {
                flag = ReturnPalletDAL.UpdateReturnItemQty(_equipmentNumber, _userName, _userPassword, _warehouseID, containerDetailID, qty, odd1, odd2, odd3);
            }
            catch { flag = 0; }
            finally { }

            return flag;
        }

        public void UpdateSendHistory(string schemeID)
        {
            try
            {
                ReturnPalletDAL.UpdateSendHistory(_equipmentNumber, _userName, _userPassword, _warehouseID, schemeID);
            }
            catch
            { }
        }

        /// <summary>
        /// 获取所有未完成的退料单信息 QWH 2009/05/14
        /// </summary>
        /// <returns></returns>        
        public DataSet GetAllReturnMaterialBill()
        {
            DataSet ds = null;
            try
            {
                ds = ReturnPalletDAL.GetAllReturnMaterialBill(_equipmentNumber, _userName, _userPassword, _warehouseID);
            }
            catch
            { }
            finally { }

            return ds;
        }

        public DataSet SelectTypeByCondition(string SqlCondition)
        {
            DataSet ds = null;
            try
            {
                ds = ReturnPalletDAL.SelectTypeByCondition(_warehouseID, _equipmentNumber, _userName, _userPassword, SqlCondition);
            }
            catch { }
            return ds;
        }

        
        public DataSet GetMaterialBillDetail(string WarehouseID, string MachineNumber, string StartTime, string EndTime, string Field4)
        {
            DataSet ds = null;
            try
            {
                ds = ReturnPalletDAL.GetMaterialBillDetail(WarehouseID, MachineNumber, StartTime, EndTime, Field4);
            }
            catch { }
            return ds;
        }
    }
}
