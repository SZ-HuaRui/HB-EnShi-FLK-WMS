using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NTI.WMS.CallTerminalSystem.DAL;

namespace NTI.WMS.CallTerminalSystem.BLL
{
    public class CommonBLL
    {
        string _equipmentNumber;
        string _userName;
        string _userPassword;
        string _warehouseID;

        public CommonBLL()
        {
        }

        public CommonBLL(string equipmentNumber, string userName, string userPassword, string warehouseId)
        {
            this._equipmentNumber = equipmentNumber;
            this._userName = userName;
            this._userPassword = userPassword;
            this._warehouseID = warehouseId;
        }


        //根据机台号、站台位号获取可供呼叫出库的配盘单明细        
        public DataTable GetBomDetailByMachineNumber(string url, string sessionKey, string warehouseId, string machineNumber, string dockNumber)
        {
            try
            {
                return CommonDAL.GetBomDetailByMachineNumber(url, sessionKey, warehouseId, machineNumber, dockNumber);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据站台“" + dockNumber + "”获取配盘明细信息错误:" + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 根据机台id、站台编码获取可供呼叫出库的配盘单明细
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineNumber"></param>
        /// <param name="dockcode">站台编号</param>
        /// <param name="schemeId">生产工单id</param>
        /// <param name="machineId">站台id</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public DataTable GetBomDetailByMachineNumber(string url, string sessionKey, string warehouseId, string machineNumber, string dockcode, string schemeId,string machineId, string token)
        {
            try
            {
                return CommonDAL.GetBomDetailByMachineNumber(url, sessionKey, warehouseId, machineNumber, dockcode, schemeId,machineId, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据站台“" + dockcode + "”获取配盘明细信息错误:" + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 根据机台id、站台编码获取可供呼叫出库的配盘单明细
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineNumber"></param>
        /// <param name="dockcode">站台编号</param>
        /// <param name="schemeId">生产工单id</param>
        /// <param name="machineId">站台id</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public DataTable GetBomDetailByMachineNumber1(string url, string sessionKey,string bomId, string warehouseId, string machineNumber, string dockcode, string schemeId, string machineId, string token)
        {
            try
            {
                return CommonDAL.GetBomDetailByMachineNumber1(url, sessionKey, bomId, warehouseId, machineNumber, dockcode, schemeId, machineId, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据站台“" + dockcode + "”获取配盘明细信息错误:" + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 根据机台id、站台编码获取Bom
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineNumber"></param>
        /// <param name="dockcode">站台编号</param>
        /// <param name="schemeId">生产工单id</param>
        /// <param name="machineId">站台id</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public DataTable GetBomByMachineNumber(string url, string sessionKey, string warehouseId, string machineNumber, string dockcode, string schemeId, string machineId, string token)
        {
            try
            {
                return CommonDAL.GetBomByMachineNumber(url, sessionKey, warehouseId, machineNumber, dockcode, schemeId, machineId, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据站台“" + dockcode + "”获取Bom信息错误:" + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 根据机台获取站台位信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineNumber"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable GetMachineDockInfo(string url, string sessionKey, string warehouseId, string machineNumber, string token=null)
        {
            try
            {
                return CommonDAL.GetMachineDockInfo(url, sessionKey, warehouseId, machineNumber, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据“" + machineNumber + "”获取站台信息出现错误:" + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 获取机台当前物料信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable GetMachineCurTaskByMachineNumber(string url, string sessionKey, string warehouseId, string machineId,string token)
        {
            try
            {
                return CommonDAL.GetMachineCurTaskByMachineNumber(url, sessionKey, warehouseId, machineId, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据“" + machineId + "”获取机台当前物料信息错误:" + ex.Message);
            }

            return null;
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
        /// 成品转移
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machine"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string ProductTransfer(string url, string sessionKey, string warehouseId, string dockId, string hasLGV, string shiftId, string teamId, string productPlanOutId, string machine, string token)
        {
            return ReturnPalletDAL.ProductTransfer(url, sessionKey, warehouseId, dockId, hasLGV, shiftId, teamId, productPlanOutId, machine, token);
        }

        /// <summary>
        /// 退空盘+要料
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="machine"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string ReturnNullAndCallPallet(string url, out string errcode, string sessionKey, string warehouseId, string dockId, string hasLGV, string shiftId, string teamId, string productPlanOutId, string machine, string token)
        {
            try
            {
                return ReturnPalletDAL.ReturnNullAndCallPallet(url, out errcode, sessionKey, warehouseId, dockId, hasLGV, shiftId, teamId, productPlanOutId, machine, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("退空托盘++要料出现错误:" + ex.Message);
                errcode = "1";
                return "连接服务器失败！";
            }
        }

        //获取机台当前站台托盘信息
        public DataTable GetMachineCurTaskByMachineNumber(string url, string sessionKey, string warehouseId, string machineNumber)
        {
            try
            {
                return CommonDAL.GetMachineCurTaskByMachineNumber(url, sessionKey, warehouseId, machineNumber);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据“" + machineNumber + "”获取站台机台当前站台托盘信息错误:" + ex.Message);
            }

            return null;
        }

        //根据机台号获取机台当前存在的任务
        public DataTable GetMachineTaskByMachineNumber(string url, string sessionKey, string warehouseId, string machineNumber, string taskTypes)
        {
            try
            {
                return CommonDAL.GetMachineTaskByMachineNumber(url, sessionKey, warehouseId, machineNumber, taskTypes);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据“" + machineNumber + "”获取站台当前任务出现错误:" + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 获取机台当前任务(除人工退料和人工领料任务)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="schemeId">生产工单id</param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="taskTypes">单据类型</param>
        /// <param name="token"></param>
        /// <param name="isContainsReturnWaste">是否包含退废料（1：包含；0：不包含）</param>
        /// <returns></returns>
        public DataTable GetMachineTaskByMachineNumber(string url, string sessionKey, string schemeId, string warehouseId, string machineId, string taskTypes, string token, string isContainsReturnWaste)
        {
            try
            {
                return CommonDAL.GetMachineTaskByMachineNumber(url, sessionKey, schemeId, warehouseId, machineId, taskTypes, token, isContainsReturnWaste);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据“" + machineId + "”获取站台当前任务出现错误:" + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 获取机台当前任务(除人工退料和人工领料任务)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="schemeId">生产工单id</param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="taskTypes">单据类型</param>
        /// <param name="token"></param>
        /// <param name="isContainsReturnWaste">是否包含退废料（1：包含；0：不包含）</param>
        /// <returns></returns>
        public DataTable GetMachineFinishTaskByMachineNumber(string url, string sessionKey, string schemeId, string warehouseId, string machineId, string taskTypes, string token, string startDatetime, string endDatetime)
        {
            try
            {
                return CommonDAL.GetMachineFinishTaskByMachineNumber(url, sessionKey, schemeId, warehouseId, machineId, taskTypes, token,  startDatetime, endDatetime);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据“" + machineId + "”获取站台当前任务出现错误:" + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 获取机台当前人工退料和人工领料任务任务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="schemeId">生产工单id</param>
        /// <param name="warehouseId"></param>
        /// <param name="machineId">机台id</param>
        /// <param name="taskTypes">单据类型</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataTable GetMachinePersonTaskByMachineNumber(string url, string sessionKey, string schemeId, string warehouseId, string machineId, string taskTypes, string token)
        {
            try
            {
                return CommonDAL.GetMachinePersonTaskByMachineNumber(url, sessionKey, schemeId, warehouseId, machineId, taskTypes, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs($"根据生产工单id：{schemeId}获取站台当前人工退料和人工领料任务任务出现错误:" + ex.Message);
            }

            return null;
        }

        //根据机台号获取机台当前生产计划
        public DataTable GetMachineCurPlanByMachineNumber(string url, string sessionKey, string machineId, string shift_id, string team_id, string token)
        {
            try
            {
                return CommonDAL.GetMachineCurPlanByMachineNumber(url, sessionKey, machineId, shift_id, team_id, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs($"根据机台id:{machineId}，班次id:{shift_id},班组id:{team_id}，获取生产牌号信息出现错误:{ex.Message}");
            }
            return null;
        }


        //根据车间类型获取机台数据
        public DataTable GetMachineDataByWorkShopType(string url, string userName, string sessionKey, string token, string warehouseId, string workShopType)
        {
            DataTable dt = null;
            try
            {
                dt = CommonDAL.GetMachineDataByWorkShopType(url, userName, sessionKey,token, warehouseId, workShopType);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据车间获取机台数据出现错误:" + ex.Message);
            }
            return dt;
        }

        //获取成品牌号
        public DataTable GetItemCP(string url, string sessionKey, string warehouseId)
        {
            DataTable dt = null;
            try
            {
                dt = CommonDAL.GetItemCP(url, sessionKey, warehouseId);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取成品牌号出现错误:" + ex.Message);
            }
            return dt;
        }

        //修改牌号
        public string UpdateMachineProductPlan(string url, string sessionKey, string machineNumber, string itemId)
        {
            try
            {
                return CommonDAL.UpdateMachineProductPlan(url, sessionKey, machineNumber, itemId);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("修改牌号出现错误:" + ex.Message);
            }
            return null;
        }

        #region Hide
        /// <summary>
        /// 检查站台位状态 
        /// </summary>
        /// <param name="MachineNumber"></param>
        /// <param name="DockNumber"></param>
        /// <returns>FALSE 正在使用；TRUE 未使用</returns>       
        public bool CheckDockState(string MachineNumber, string DockNumber)
        {
            bool flag = false;
            try
            {
                flag = CommonDAL.CheckDockState(_equipmentNumber, _userName, _userPassword, _warehouseID, MachineNumber, DockNumber);
            }
            catch { }

            return flag;
        }

        /// <summary>
        /// 检查成型机台的父站台对应的子站台是否已经退料完毕 peq 20110211
        /// </summary>
        /// <param name="Warehouseid">仓库id</param>
        /// <param name="MachineNumber">机台</param>
        /// <param name="DockNumber">站台</param>
        /// <returns></returns>
        public bool CheckCXMachineStatus(string Warehouseid, string MachineNumber, string DockNumber)
        {
            bool flag = false;
            try
            {
                flag = CommonDAL.CheckCXMachineStatus(Warehouseid, MachineNumber, DockNumber);
            }
            catch
            {
                return false;
            }
            return flag;
        }

        /// <summary>
        /// 检查成型机台是否存在父站台  PEQ 20110217
        /// </summary>
        /// <param name="Warehouseid">仓库id</param>
        /// <param name="MachineNumber">机台</param>
        /// <param name="DockNumber">站台</param>
        /// <returns></returns>
        public object CheckExistsFatherDockNumber(string Warehouseid, string MachineNumber, string DockNumber)
        {
            object obj = null;
            try
            {
                obj = CommonDAL.CheckExistsFatherDockNumber(Warehouseid, MachineNumber, DockNumber);
            }
            catch
            {
                return null;
            }
            return obj;
        }

        /// <summary>
        /// 检查站台位任务数据（仅用于要料）
        /// </summary>
        /// <param name="DockNumber"></param>
        /// <returns>FALSE 存在任务；TRUE 没有任务</returns>
        public bool CheckDockCurTaskByDockNumber(string DockNumber)
        {
            bool flag = false;
            try
            {
                flag = CommonDAL.CheckDockCurTaskByDockNumber(_equipmentNumber, _userName, _userPassword, _warehouseID, DockNumber);
            }
            catch { }

            return flag;
        }

        /// <summary>
        /// 检查站台位任务数据（仅用于退料）
        /// </summary>
        /// <param name="DockNumber"></param>
        /// <returns>FALSE 存在任务；TRUE 没有任务</returns>
        public bool CheckDockCurTaskByDockNumber_Return(string DockNumber)
        {
            bool flag = false;
            try
            {
                flag = CommonDAL.CheckDockCurTaskByDockNumber_Return(_equipmentNumber, _userName, _userPassword, _warehouseID, DockNumber);
            }
            catch { }

            return flag;
        }

        /// <summary>
        /// 检查站台位退料单数据（仅用于退料）
        /// </summary>
        /// <param name="EquipmentNumber"></param>
        /// <param name="UserName"></param>
        /// <param name="UserPassword"></param>
        /// <param name="WareHouseID"></param>
        /// <param name="DockNumber"></param>
        /// <param name="MachineNumber"></param>
        /// <returns>FALSE 存在未执行的退料单；TRUE 没有未执行的退料单</returns>
        public bool CheckDockReturnBill_Exists(string DockNumber, string MachineNumber)
        {
            bool flag = false;
            try
            {
                flag = CommonDAL.CheckDockReturnBill_Exists(_equipmentNumber, _userName, _userPassword, _warehouseID, DockNumber, MachineNumber);
            }
            catch { }

            return flag;
        }


        /// <summary>
        /// 获取车间信息
        /// </summary>
        public DataTable GetWorkShopZone(string url, string sessionKey, string warehouseId)
        {
            try
            {
                return CommonDAL.GetWorkShopZone(url, sessionKey, warehouseId);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取车间数据出现错误:" + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 根据MachineNumber获取生产区间信息 HJ 2010/09/02
        /// </summary>

        public DataSet GetZoneByMachineNumber(string MachineNumber)
        {
            DataSet ds = null;
            try
            {
                ds = CommonDAL.GetZoneByMachineNumber(_equipmentNumber, _userName, _userPassword, _warehouseID, MachineNumber);
                //if (ds != null && ds.Tables.Count > 0)
                //{
                //    DataTable dt = ds.Tables[0];
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        LogWriter.WriterSystemLogs("根据“" + MachineNumber + "”获取机台信息成功。");
                //    }
                //    else
                //    {
                //        LogWriter.WriterSystemLogs("根据“" + MachineNumber + "”获取机台信息失败！");
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据“" + MachineNumber + "”获取机台信息出现错误:" + ex.Message);
                ds = null;
            }

            return ds;
        }

        /// <summary>
        /// 根据MachineNumber获取机台id
        /// </summary>

        public DataSet GetEquimentid(string MachineNumber)
        {
            DataSet ds = null;
            try
            {
                ds = CommonDAL.GetEquimentid(_equipmentNumber, _userName, _userPassword, _warehouseID, MachineNumber);

            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据“" + MachineNumber + "”获取机台id出现错误:" + ex.Message);
                ds = null;
            }

            return ds;
        }


        /// <summary>
        /// 根据itemcode获取物料id
        /// </summary>

        public DataSet GetItemid(string MachineNumber)
        {
            DataSet ds = null;
            try
            {
                ds = CommonDAL.GetItemid(_equipmentNumber, _userName, _userPassword, _warehouseID, MachineNumber);

            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据“" + MachineNumber + "”获取机台id出现错误:" + ex.Message);
                ds = null;
            }

            return ds;
        }

        /// <summary>
        /// 获取班次信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable GetRunsCode(string url, string sessionKey, string token,out string msg)
        {
            msg = string.Empty;
            try
            {
                return CommonDAL.GetRunsCode(url, sessionKey, token);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogWriter.WriterErrorLogs("获取班次出现错误:" + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 获取班组信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable GetRunsGroup(string url, string sessionKey,string token)
        {
            try
            {
                return CommonDAL.GetRunsGroup(url, sessionKey, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("获取班组出现错误:" + ex.Message);
            }

            return null;
        }

        #region 贵阳新增接口

        //设置是否产生AGV
        public bool SetAgv(string warehouseId, string workshopId, string machineNumber, string hasAGV)
        {
            bool setIsAgv = false;
            try
            {
                setIsAgv = CommonDAL.SetAgv(warehouseId, workshopId, machineNumber, hasAGV);
            }
            catch
            {
                return false;
            }
            return setIsAgv;
        }

        //查询是否设置了产生AGV
        public string ShowAgv(string warehouseId, string machineNumber)
        {
            string displayAgv = string.Empty;
            try
            {
                displayAgv = CommonDAL.ShowAgv(warehouseId, machineNumber);
            }
            catch
            {
                return null;
            }
            return displayAgv;
        }

        //插入班次信息
        public bool InsertClassInfo(string WarehouseID, string RunID, string WorkShop, string TeamDate, string ClassState, string strMachineId, string strRemark, string autoRunID, string productDate, string preClass)
        {
            try
            {
                return CommonDAL.InsertClassInfo(WarehouseID, RunID, WorkShop, TeamDate, ClassState, strMachineId, strRemark, autoRunID, productDate, preClass);
            }
            catch { return false; }
        }

        //获取上一班次的信息
        public DataSet GetPreClassInfo(string strWarehouseId, string strMachineNumber, string strWorkShop, string strClassState)
        {
            try
            {
                return CommonDAL.GetPreClassInfo(strWarehouseId, strMachineNumber, strWorkShop, strClassState);
            }
            catch { return null; }
        }

        //修改班次的状态
        public int UpdateStatus(string WarehouseID, string WorkShop, string MachineNumber, string TeamGroup, string ClassState, string nextClass)
        {
            try
            {
                return CommonDAL.UpdateStatus(WarehouseID, WorkShop, MachineNumber, TeamGroup, ClassState, nextClass);
            }
            catch { return 0; }
        }
        #endregion


        #region 检查机台是否有从属站台  YYL
        public DataSet CheckDocknumber(string warehouseid, string MachineNumber, string Docknumber)
        {
            return null;//WCFService.CallEndWS.CheckDockNumber(warehouseid, MachineNumber, Docknumber);
        }
        #endregion


        public void TrackCallEndErrors(string EquipmentNumber, string UserName, string WareHouseID, int result, string Message, string RightMessages, string Errormessage)
        {
            try
            {
                CommonDAL.TrackCallEndErrors(EquipmentNumber, UserName, WareHouseID, result, Message, RightMessages, Errormessage);
            }
            catch { }
        }

        public DataTable GetMachineShipperSet(string machineNumber, string modelCode)
        {
            try
            {
                return CommonDAL.GetMachineShipperSet(_warehouseID, machineNumber, modelCode);
            }
            catch { }
            return null;
        }

        public bool UpdateMachineProductPlan(string machineNumber, string planNo, string state, out string msg)
        {
            try
            {
                return CommonDAL.UpdateMachineProductPlan(_warehouseID, machineNumber, planNo, state, out msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 根据机台号获取机台当前生产计划 QWH 2009/06/06
        /// </summary>
        /// <param name="MachineNumber">机台号</param>
        /// <returns></returns>
        public int UpdateMachineCurPlanBySchemeid(string machineNumber, string schemeid)
        {
            int flag = 0;
            try
            {
                flag = CommonDAL.UpdateMachineCurPlanBySchemeid(_equipmentNumber, _userName, _userPassword, _warehouseID, machineNumber, schemeid);
                //if (ds != null && ds.Tables.Count > 0)
                //{
                //    DataTable dt = ds.Tables[0];
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        LogWriter.WriterSystemLogs("根据“" + machineNumber + "”获取生产牌号信息成功。");
                //    }
                //    else
                //    {
                //        LogWriter.WriterSystemLogs("根据“" + machineNumber + "”获取生产牌号信息失败！");
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据“" + machineNumber + "”获取生产牌号信息出现错误:" + ex.Message);
                flag = 0;
            }

            return flag;
        }
        public int UpdateMachineCurPlanBySchemeid_accept(string machineNumber, string schemeid)
        {
            int flag = 0;
            try
            {
                flag = CommonDAL.UpdateMachineCurPlanBySchemeid_accept(_equipmentNumber, _userName, _userPassword, _warehouseID, machineNumber, schemeid);
                //if (ds != null && ds.Tables.Count > 0)
                //{
                //    DataTable dt = ds.Tables[0];
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        LogWriter.WriterSystemLogs("根据“" + machineNumber + "”获取生产牌号信息成功。");
                //    }
                //    else
                //    {
                //        LogWriter.WriterSystemLogs("根据“" + machineNumber + "”获取生产牌号信息失败！");
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据“" + machineNumber + "”获取生产牌号信息出现错误:" + ex.Message);
                flag = 0;
            }

            return flag;
        }
        //public DataSet GetMachineCurPlanByMachineNumber1(string machineNumber, string schemeid)
        //{
        //    DataSet ds = null;
        //    try
        //    {
        //        ds = CommonDAL.GetMachineCurPlanByMachineNumber1(_equipmentNumber, _userName, _userPassword, _warehouseID, machineNumber, schemeid);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriterErrorLogs("根据“" + machineNumber + "”获取生产牌号信息出现错误:" + ex.Message);
        //        ds = null;
        //    }

        //    return ds;
        //}
        #endregion
    }
}
