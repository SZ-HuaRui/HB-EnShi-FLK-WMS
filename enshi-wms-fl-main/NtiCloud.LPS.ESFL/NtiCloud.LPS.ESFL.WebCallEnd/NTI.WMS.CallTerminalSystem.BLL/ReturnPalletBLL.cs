using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NTI.WMS.CallTerminalSystem.DAL;
using WMSInterface;
using System.Collections;
using WMSInterface.Entity50;
using WMSInterface.EntityDefination;

namespace NTI.WMS.CallTerminalSystem.BLL
{
    public class ReturnPalletBLL
    {
        string _equipmentNumber;
        string _userName;
        string _userPassword;
        string _warehouseID;

        public ReturnPalletBLL(string equipmentNumber, string userName, string userPassword, string warehouseId)
        {
            this._equipmentNumber = equipmentNumber;
            this._userName = userName;
            this._userPassword = userPassword;
            this._warehouseID = warehouseId;
        }

        /// <summary>
        /// 退废料
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machine"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string ReturnWasteMaterials(string url, string sessionKey, string warehouseId, string dockId, string hasLGV, string shiftId, string teamId, string productPlanOutId, string machine, string token)
        {
            return ReturnPalletDAL.ReturnWasteMaterials(url, sessionKey, warehouseId, dockId, hasLGV, shiftId, teamId, productPlanOutId, machine, token);
        }

        /// <summary>
        /// 退空托盘
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="dockId"></param>
        /// <param name="hasLGV"></param>
        /// <param name="shiftId"></param>
        /// <param name="teamId"></param>
        /// <param name="productPlanOutId"></param>
        /// <param name="machine"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string ClientReturnNullPallet(string url, string sessionKey, string warehouseId, string dockId, string hasLGV, string shiftId, string teamId, string productPlanOutId, string machine, string token)
        {
            return ReturnPalletDAL.ClientReturnNullPallet(url, sessionKey, warehouseId, dockId, hasLGV, shiftId, teamId, productPlanOutId, machine, token);
        }

        /// <summary>
        /// 退余料前检查站台当前是否存在未执行结束的任务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="dockId"></param>
        /// <param name="token"></param>
        /// <returns>true：存在；false：不存在</returns>
        public bool CheckDockCurTask(string url, string sessionKey, string warehouseId, string dockNumber, string token)
        {
            return ReturnPalletDAL.CheckDockCurTask(url, sessionKey, warehouseId, dockNumber, token);
        }

        /// <summary>
        /// 退坏料前检查站台当前是否存在未执行结束的任务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="dockId"></param>
        /// <param name="token"></param>
        /// <returns>true：存在；false：不存在</returns>
        public bool CheckAllowed_ReturnPallet(string url, string sessionKey, string warehouseId, string dockNumber, string token)
        {
            return ReturnPalletDAL.CheckAllowed_ReturnPallet(url, sessionKey, warehouseId, dockNumber, token);
        }

        /// <summary>
        /// 退余料界面，展示的托盘数据。根据站台位获取当前托盘数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="dockNumber"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable GetContainerDetailByDockNumber(string url, string sessionKey, string warehouseId, string machineId, string dockNumber, string token)
        {
            try
            {
                return ReturnPalletDAL.GetContainerDetailByDockNumber(url, sessionKey, warehouseId, machineId, dockNumber, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取站台当前托盘数据出现错误:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据生产工单获取最新的机台要料/接班任务信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="schemeId">生产工单id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable GetTheNewItemNumByPlanOutId(string url, string isChangePersonReceive, string sessionKey, string warehouseId, string machineId, string schemeId, string token)
        {
            try
            {
                return ReturnPalletDAL.GetTheNewItemNumByPlanOutId(url, isChangePersonReceive, sessionKey, warehouseId, machineId, schemeId, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取站台当前托盘数据出现错误:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据站台获取最新的要料/接班任务信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="dockNumber">站台编号</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable GetTaskByDock(string url, string isChangePersonReceive, string sessionKey, string warehouseId, string machineId, string dockNumber, string token)
        {
            try
            {
                return ReturnPalletDAL.GetTaskByDock(url, isChangePersonReceive, sessionKey, warehouseId, machineId, dockNumber, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据站台获取最新的要料/接班任务信息出现错误:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 退坏料界面，展示的托盘数据。根据站台位获取当前托盘数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="dockNumber"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable BadMaterialGetContainerDetailByDockNumber(string url, string sessionKey, string warehouseId, string machineId, string dockNumber, string token)
        {
            try
            {
                return ReturnPalletDAL.BadMaterialGetContainerDetailByDockNumber(url, sessionKey, warehouseId, machineId, dockNumber, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取站台当前托盘数据出现错误:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 退坏料界面，展示的目的区域数据。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="dockNumber"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable GetTargetAreaOnBadMaterial(string url, string sessionKey, string warehouseId, string machineId, string dockNumber, string token)
        {
            try
            {
                return ReturnPalletDAL.GetTargetAreaOnBadMaterial(url, sessionKey, warehouseId, machineId, dockNumber, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取退坏料目的区域数据出现错误:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 调用退余料微服务，进行退余料
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isNeedCallPallet">是否需要再次要料（1:需要；0:不需要）</param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="dockId">站台id</param>
        /// <param name="productPlanOutId">生产工单id</param>
        /// <param name="shiftId">班次id</param>
        /// <param name="teamId">班组id</param>
        /// <param name="details">余料明细（json格式）</param>
        /// <param name="hasLGV">是否产生AGV任务</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public string ClientReturnPallet(string url, out string errcode, bool isNeedCallPallet,bool isCanBeCallOut, string sessionKey, string warehouseId, string machineId, string dockId, string productPlanOutId, string shiftId, string teamId, string details, string hasAGV, string token)
        {
            try
            {
                return ReturnPalletDAL.ClientReturnPallet(url,out errcode, isNeedCallPallet, isCanBeCallOut, sessionKey, warehouseId, machineId, dockId, productPlanOutId, shiftId, teamId, details, hasAGV, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("退余料出现错误:" + ex.Message);
                errcode = "1";
                return "连接服务器失败！";
            }
        }

        /// <summary>
        /// 调用退坏料微服务，进行退坏料
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="dockId">站台id</param>
        /// <param name="productPlanOutId">生产工单id</param>
        /// <param name="shiftId">班次id</param>
        /// <param name="teamId">班组id</param>
        /// <param name="details">坏料明细（json格式）</param>
        /// <param name="hasLGV">是否产生AGV任务</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public string ClientReturnBadPallet(string url, string sessionKey, string warehouseId, string machineId, string dockId, string productPlanOutId, string shiftId, string teamId, string details, string hasAGV, string token)
        {
            try
            {
                return ReturnPalletDAL.ClientReturnBadPallet(url, sessionKey, warehouseId, machineId, dockId, productPlanOutId, shiftId, teamId, details, hasAGV, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("退坏料出现错误:" + ex.Message);
                return "连接服务器失败！";
            }
        }

        /// <summary>
        /// 调用换牌微服务，进行退余料
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="productPlanOutId">生产工单id</param>
        /// <param name="shiftId">班次id</param>
        /// <param name="teamId">班组id</param>
        /// <param name="details">余料明细（json格式）</param>
        /// <param name="hasLGV">是否产生AGV任务</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public string ChangeCard(string url, string sessionKey,bool isCanBeCallOut, string warehouseId, string machineId, string productPlanOutId, string shiftId, string teamId, string details, string hasAGV, string token)
        {
            try
            {
                return ReturnPalletDAL.ChangeCard(url, sessionKey, isCanBeCallOut, warehouseId, machineId, productPlanOutId, shiftId, teamId, details, hasAGV, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("退余料出现错误:" + ex.Message);
                return "连接服务器失败！";
            }
        }

        /// <summary>
        /// 获取交接班时挂在站台上的物料信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="schemeId"></param>
        /// <param name="token"></param>
        /// <param name="isChangePersonReceive">是否交接人工领料数据（true:交；false:不交）</param>
        /// <returns></returns>
        public DataTable GetMachineCurItemsInfo(string url, string sessionKey, string warehouseId, string machineId, string schemeId, string token, bool isChangePersonReceive)
        {
            try
            {
                return ReturnPalletDAL.GetMachineCurItemsInfo(url, sessionKey, warehouseId, machineId, schemeId, token, isChangePersonReceive);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取交接班时挂在站台上的物料信息错误:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 接班时获取机台当前交班托盘信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="schemeId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable GetMachineCurItemsInfoWhenChangeClass(string url, string isChangePersonReceive, string sessionKey, string warehouseId, string machineId, string schemeId, string token)
        {
            try
            {
                return ReturnPalletDAL.GetMachineCurItemsInfoWhenChangeClass(url, isChangePersonReceive, sessionKey, warehouseId, machineId, schemeId, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取机台当前交班托盘信息错误:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 获取机台交接班站台
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="schemeId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable GetClassDockNumberByMachineNumber(string url, string sessionKey, string warehouseId, string machineId, string schemeId, string token)
        {
            try
            {
                return ReturnPalletDAL.GetClassDockNumberByMachineNumber(url, sessionKey, warehouseId, machineId, schemeId, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取机台交接班站台错误:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 交班
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineNumber"></param>
        /// <param name="details"></param>
        /// <param name="runsType"></param>
        /// <returns></returns>
        public string ClientReturnPallet_ToNext(string url, bool isChangePersonReceive, string sessionKey, string warehouseId, string machineId, string productPlanOutId, string shiftId, string teamId, string details, string hasAGV, string token)
        {
            return ReturnPalletDAL.ClientReturnPalletToNext(url, isChangePersonReceive, sessionKey, warehouseId, machineId, productPlanOutId, shiftId, teamId, details, hasAGV, token);
        }

        //客户确认接班
        public string ClientConfirmReturnBill(string url, string sessionKey, string warehouseId, string machineNumber,
             string dockNumber, string schemeId, string billId, string runsType)
        {
            try
            {
                return ReturnPalletDAL.ClientConfirmReturnBill(url, sessionKey, warehouseId, machineNumber, dockNumber,
                    schemeId, billId, runsType);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取机台交接班站台错误:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 确认接班
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="productPlanOutId"></param>
        /// <param name="shiftId"></param>
        /// <param name="teamId"></param>
        /// <param name="details"></param>
        /// <param name="hasAGV"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string ClientModifyReturnBill(string url, string isChangePersonReceive, string sessionKey, string warehouseId, string machineId, string productPlanOutId, string shiftId, string teamId, string details, string hasAGV, string token)
        {
            return ReturnPalletDAL.ClientModifyReturnBill(url, isChangePersonReceive, sessionKey, warehouseId, machineId, productPlanOutId, shiftId, teamId, details, hasAGV, token);

        }

        /// <summary>
        /// 修改交班时提交的物料数量
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="productPlanOutId"></param>
        /// <param name="shiftId"></param>
        /// <param name="teamId"></param>
        /// <param name="details"></param>
        /// <param name="hasAGV"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string EditReturnBill(string url, string sessionKey, string warehouseId, string machineId, string productPlanOutId, string shiftId, string teamId, string details, string hasAGV, string token)
        {
            return ReturnPalletDAL.EditReturnBill(url, sessionKey, warehouseId, machineId, productPlanOutId, shiftId, teamId, details, hasAGV, token);

        }


        /// <summary>
        /// 根据机台id获取机台所有站台位的容器信息(换牌退料)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable GetMachineCurContainInfo(string url, string sessionKey, string warehouseId, string machineId, string token)
        {
            try
            {
                return ReturnPalletDAL.GetMachineCurContainInfo(url, sessionKey, warehouseId, machineId, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取机台交接班站台错误:" + ex.Message);
            }
            return null;
        }


        #region old

        /// <summary>
        /// 修改机台为换牌后的首次呼叫状态 QWH 2009/07/10 
        /// </summary>
        /// <param name="MachineNumber">机台号</param>
        /// <returns>大于0，操作成功</returns>        
        public int UpdateMachineToFirstCallState(string MachineNumber)
        {
            int flag = 0;
            try
            {
                flag = ReturnPalletDAL.UpdateMachineToFirstCallState(_equipmentNumber, _userName, _userPassword, _warehouseID, MachineNumber);
            }
            catch
            { flag = 0; }
            finally { }

            return flag;
        }

        /// <summary>
        /// 获取退空托盘目的站台数据
        /// </summary>
        /// <returns>返回退空托盘目的地</returns>
        public DataSet GetReturnNullPalletDestination()
        {
            DataSet ds = null;
            try
            {
                ds = CommonDAL.SelectEquipmentWithTypeCode(_equipmentNumber, _userName, _userPassword, _warehouseID, "Equipment_Station_NullPallet");
            }
            catch
            {
                ds = null;
            }
            finally { }

            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0) ds = null;

            return ds;
        }

        /// <summary>
        /// 获取退料目的站台数据(包括退余料（真退）和退坏料)
        /// </summary>
        /// <returns>返回退料目的站台</returns>
        public DataSet GetReturnPalletDestination()
        {
            DataSet ds = null;
            try
            {
                ds = CommonDAL.SelectEquipmentWithTypeCode(_equipmentNumber, _userName, _userPassword, _warehouseID, "Equipment_StationIn");
            }
            catch
            {
                ds = null;
            }
            finally { }

            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0) ds = null;

            return ds;
        }

        /// <summary>
        /// 确认站台位是否允许作退料呼叫操作
        /// </summary>
        /// <param name="machineNunber">机台号</param>
        /// <param name="dockNumber">站台位号</param>
        /// <returns>TRUE:允许退料  FALSE：不允许退料</returns>
        public bool CheckAllowed_ReturnPallet(string machineNunber, string dockNumber)
        {
            CommonBLL cb = new CommonBLL(_equipmentNumber, _userName, _userPassword, _warehouseID);

            bool flag = false;
            //取消是否存在未完成的退料单的判断  PEQ 20110413
            if (!cb.CheckDockState(machineNunber, dockNumber) && cb.CheckDockCurTaskByDockNumber_Return(dockNumber))
            {
                flag = true;
            }
            return flag;

        }

        /// <summary>
        /// 如果是成型机台这检查退料位置是否为1站台和2站台，这两个站台不允许退料  PEQ 20110217
        /// </summary>
        /// <param name="machineNumber">机台</param>
        /// <param name="dockNumber">站台</param>
        /// <returns></returns>
        public bool CheckCXAllowedReturnPallet(string machineNumber, string dockNumber)
        {
            try
            {
                string dockNum = dockNumber;
                int lastChar = 0;

                if (!string.IsNullOrEmpty(dockNum))
                {
                    lastChar = Convert.ToInt32(dockNum.Substring(dockNum.Length - 1, 1));
                    if (lastChar == 1 || lastChar == 2)
                        return false;
                    else
                        return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool CheckDockReturnBill_Exists(string DockNumber, string MachineNumber)
        {
            bool Result = false;
            try
            {
                Result = ReturnPalletDAL.CheckDockReturnBill_Exists(_equipmentNumber, _userName, _userPassword, _warehouseID, DockNumber, MachineNumber);
            }
            catch
            {
                Result = false;
            }
            finally { }
            return Result;
        }

        #region 呼叫终端 退坏料、退余料、退空托盘、假退/交接班 等操作

        /// <summary>
        /// 客户端发出退坏料呼叫操作 QWH 2009/06/16
        /// </summary>
        /// <param name="machineNumber">机台号</param>
        /// <param name="dockNumber">站台位号</param>
        /// <param name="bomID">配盘单ID</param>
        /// <param name="containerEntity">包含退料数据的实体对象</param>
        /// <param name="returnBillType">退料类型（退坏料、退余料）</param>
        /// <param name="hasLGV">是否有LGV（1：表示有LGV，0：表示没有LGV）</param>
        /// <param name="destination">退料目的地</param>
        /// <returns>大于0:退余料操作成功</returns> 
        public int ClientReturnBadPallet(string machineNumber, string dockNumber, string bomID, ContainerEntity containerEntity, string returnBillType, string hasLGV, string destination, string Runs)
        {
            int flag = 0;
            try
            {
                flag = ReturnPalletDAL.ClientReturnBadPallet(_equipmentNumber, _userName, _userPassword, _warehouseID, machineNumber, dockNumber, bomID, containerEntity, returnBillType, hasLGV, destination, Runs);
            }
            catch
            { flag = 0; }
            finally { }

            return flag;
        }




        //根据条件查询已完成退料任务明细
        public DataSet CheckReturnRemnantTask(string MachineNumber, string WareHouseID, string StartTime, string EndTime)
        {
            DataSet ds = null;
            try
            {
                ds = ReturnPalletDAL.CheckReturnRemnantTask(MachineNumber, WareHouseID, StartTime, EndTime);
            }
            catch
            {
                ds = null;
            }
            finally { }

            if (ds == null) ds = null;

            return ds;
        }

        public bool ChangeReturnQty(string WarehouseID, DataTable change_DataTable)
        {
            try
            {
                return ReturnPalletDAL.ChangeReturnQty(WarehouseID, change_DataTable);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 退废料
        /// </summary>
        /// <param name="MachineNumber">机台号</param>
        /// <param name="DockNumber">站台位号</param>
        /// <returns>大于0:操作成功</returns>
        public int ClientReturnWaste(string MachineNumber, string DockNumber)
        {
            int flag = 0;
            try
            {
                flag = ReturnPalletDAL.ClientReturnWaste(_equipmentNumber, _userName, _userPassword, _warehouseID, MachineNumber, DockNumber);
            }
            catch
            { flag = 0; }
            finally { }

            return flag;
        }

        /// <summary>
        /// 退残烟
        /// </summary>
        /// <param name="MachineNumber">机台号</param>
        /// <param name="DockNumber">站台位号</param>
        /// <returns>大于0:操作成功</returns>
        public int ClientReturnReject(string MachineNumber, string DockNumber)
        {
            int flag = 0;
            try
            {
                flag = ReturnPalletDAL.ClientReturnReject(_equipmentNumber, _userName, _userPassword, _warehouseID, MachineNumber, DockNumber);
            }
            catch
            { flag = 0; }
            finally { }

            return flag;
        }
        #endregion


        /// <summary>
        /// 将某物料的退料数据与配盘BOM数量比较，确认输入的退料量是否符合BOM要求  QWH 2009/06/12
        /// </summary>
        /// <param name="itemID">物料ID</param>
        /// <param name="qty">数量</param>
        /// <param name="odd1">零头1</param>
        /// <param name="odd2">零头2</param>
        /// <param name="odd3">零头3</param>
        /// <param name="bomID">配盘BOMID</param>
        /// <returns>True:数量符合 False:数量超出BOM的数量</returns>
        public bool CompareItemQtyToBom(string itemID, decimal qty, decimal odd1, decimal odd2, decimal odd3, string bomID)
        {
            if ((qty + odd1 + odd2 + odd3) <= 0) return true;

            if (string.IsNullOrEmpty(itemID) || string.IsNullOrEmpty(bomID)) return false;
            DataSet bomDS = GetBOMDetailByBomID(bomID);
            if (bomDS == null) return false;

            bool flag = false;

            DataTable dt_Bom = RowFilterData.RowFilterFromDataset(bomDS, "ITEMID='" + itemID + "'");
            if (dt_Bom == null) return false;

            decimal percent = dt_Bom.Rows[0]["RECEIVINGPERCENT"] == null ? 0 : (((decimal)dt_Bom.Rows[0]["RECEIVINGPERCENT"]) / 100) + 1;
            decimal BomQty = dt_Bom.Rows[0]["QTY"] == null ? 0 : (decimal)dt_Bom.Rows[0]["QTY"] * percent;
            if (BomQty == 0) return false;

            if ((qty + odd1 + odd2 + odd3) <= BomQty)
            {
                flag = true;
            }

            return flag;
        }

        /// <summary>
        /// 根据BOMID获取配盘明细数据（含ITEM允差率） QWH 2009/06/08
        /// </summary>
        /// <param name="EquipmentNumber">设备号</param>
        /// <param name="UserName">用户名</param>
        /// <param name="UserPassword">密码</param>
        /// <param name="WareHouseID">仓库ID</param>
        /// <param name="BomID">BOMID</param>
        /// <returns></returns>
        public DataSet GetBOMDetailByBomID(string BomID)
        {
            DataSet ds = null;
            try
            {
                ds = ReturnPalletDAL.GetBOMDetailByBomID(_equipmentNumber, _userName, _userPassword, _warehouseID, BomID);
            }
            catch
            {
                ds = null;
            }
            finally { }

            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0) ds = null;

            return ds;
        }



        public DataTable CheckBomItemInventory(string itemId)
        {
            DataTable dt = null;
            try
            {
                dt = ReturnPalletDAL.CheckBomItemInventory(_warehouseID, itemId);
            }
            catch
            {
                dt = null;
            }
            finally { }
            if (dt == null || dt.Rows.Count <= 0) dt = null;
            return dt;
        }

        /// <summary>
        /// 根据机台号、班次获取物料消耗 HJ 2012/11/20
        /// </summary>
        /// <param name="MachineNumber"></param>
        /// <param name="Run"></param>
        /// <returns></returns>
        public DataSet GetUsedMaterialsByRuns(string MachineNumber, string StartTime, string EndTime)
        {
            DataSet ds = null;
            try
            {
                ds = ReturnPalletDAL.GetUsedMaterialsByRuns(_equipmentNumber, _userName, _userPassword, _warehouseID, MachineNumber, StartTime, EndTime);
            }
            catch
            {
                ds = null;
            }
            finally { }
            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0) ds = null;

            return ds;
        }
        //根据机台、站台信息获取人工领料信息
        public DataSet GetManPowerCallDetailByDockNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber)
        {
            DataSet ds = null;
            try
            {
                ds = ReturnPalletDAL.GetManPowerCallDetailByDockNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return ds;
        }
        //根据机台、站台信息获取最后人工领料信息
        public DataSet GetManPowerReturnDetailByDockNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber)
        {
            DataSet ds = null;
            try
            {
                ds = ReturnPalletDAL.GetManPowerReturnDetailByDockNumber(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return ds;
        }

        //创建人工退料信息
        public String CreateManPowerCallMaterTask(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, TaskEntity taskEntity, Container_View containerinfo, List<ContainerDetailN> details)
        {
            String tasknumber = null;
            try
            {
                tasknumber = ReturnPalletDAL.CreateManPowerCallMaterTask(EquipmentNumber, UserName, UserPassword, WareHouseID, taskEntity, containerinfo, details);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return tasknumber;
        }

        public String CreateManPowerReturnNullPallet(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber)
        {
            String tasknumber = null;
            try
            {
                tasknumber = ReturnPalletDAL.CreateManPowerReturnNullPallet(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, DockNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return tasknumber;
        }

        public DataSet LoadMachineProduct(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID)
        {
            DataSet ds = null;
            try
            {
                ds = ReturnPalletDAL.LoadMachineProduct(EquipmentNumber, UserName, UserPassword, WarehouseID);
            }
            catch { }

            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0) ds = null;
            return ds;
        }

        public DataSet LoadProduct(string EquipmentNumber, string UserID, string UserPassword, string WarehouseID, WMSInterface.EntityDefination.ITEM.ITEMQueryConditionBase Conditions)
        {
            DataSet ds = null;
            try
            {
                ds = ReturnPalletDAL.LoadProduct(EquipmentNumber, UserID, UserPassword, WarehouseID, Conditions);
            }
            catch { }

            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0) ds = null;
            return ds;
        }

        public int UpdateMachineProductPlan(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string MachineEquipmentID, string SchemeID, string ItemID, string MachineNumber, int Qty, string LotNumber, string Version)
        {
            try
            {
                return ReturnPalletDAL.UpdateMachineProductPlan(EquipmentNumber, UserName, UserPassword, WarehouseID, MachineEquipmentID, SchemeID, ItemID, MachineNumber, Qty, LotNumber, Version);
            }
            catch { return 0; }
        }

        public DataTable LoadMachinePalletType(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<String, String> Params)
        {
            DataTable dt = null;
            try
            {
                dt = ReturnPalletDAL.LoadMachinePalletType(loginInfo, Params);
            }
            catch { }

            if (dt == null || dt.Rows.Count <= 0) dt = null;
            return dt;
        }

        public WMSInterface.Entity50.Result UpdateMachineDockN(WMSInterface.Entity50.LoginInfo loginInfo, WMSInterface.Entity50.MachineDockN machineDock)
        {
            try
            {
                return ReturnPalletDAL.UpdateMachineDockN(loginInfo, machineDock);
            }
            catch { return null; }
        }

        public DataTable GetModelN(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<String, String> Params)
        {
            try
            {
                return ReturnPalletDAL.GetModelN(loginInfo, Params);
            }
            catch { return null; }
        }

        public String InsertUsedMaterials(WMSInterface.UsedMaterialsHead head, List<WMSInterface.UsedMaterialsDetail> useddetails)
        {
            try
            {
                return ReturnPalletDAL.InsertUsedMaterials(head, useddetails);
            }
            catch
            {
                return null;
            }
        }

        public bool CheckHavedEnsure(string WarehouseID, string MachineNumber, string RunsTypeCode, string StartTime, string EndTime)
        {
            try
            {
                return ReturnPalletDAL.CheckHavedEnsure(WarehouseID, MachineNumber, RunsTypeCode, StartTime, EndTime);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DataSet GetTwoProductName(string warehouseid, string machinenumber)
        {
            try
            {
                return ReturnPalletDAL.GetTwoProductName(warehouseid, machinenumber);
            }
            catch { return null; }
        }

        public static DataSet GetMaterialBillDetail(string WarehouseID, string MachineNumber, string StartTime, string EndTime, string Field4)
        {
            try
            {
                return ReturnPalletDAL.GetMaterialBillDetail(WarehouseID, MachineNumber, StartTime, EndTime, Field4);
            }
            catch { return null; }
        }
        public bool UpdateChangeClassItemQty(string warehouseId, string billId, DataTable dtChangeItmes, out string msg)
        {
            try
            {
                return ReturnPalletDAL.UpdateChangeClassItemQty(warehouseId, billId, dtChangeItmes, out msg);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs(ex.Message);
                msg = "连接服务器失败！";
            }
            return false;
        }
        #endregion
    }
}
