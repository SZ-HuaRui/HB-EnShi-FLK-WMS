using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTI.WMS.CallTerminalSystem.DAL;

namespace NTI.WMS.CallTerminalSystem.BLL
{
    public class CallPalletBLL
    {
        string _equipmentNumber;
        string _userName;
        string _userPassword;
        string _warehouseID;

        public CallPalletBLL()
        {

        }

        public CallPalletBLL(string equipmentNumber, string userName, string userPassword, string warehouseId)
        {
            this._equipmentNumber = equipmentNumber;
            this._userName = userName;
            this._userPassword = userPassword;
            this._warehouseID = warehouseId;
        }

        //客户端发出要料呼叫
        public string ClientCallPallet(string url, string sessionKey, string warehouseId, string dockNumber,
            string hasLGV, string runsType)
        {
            try
            {
                return CallPalletDAL.ClientCallPallet(url, sessionKey, warehouseId, dockNumber, hasLGV, runsType);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("客户端要料呼叫出现错误:" + ex.Message);
                return "连接服务器失败！";
            }
        }

        //客户端发出要料呼叫
        public string ClientCallPallet(string url, string machineId, string hasLGV, string dockId, string productPlanOutId, string shiftId, string teamId, string token)
        {
            return CallPalletDAL.ClientCallPallet(url, machineId, hasLGV, dockId, productPlanOutId, shiftId, teamId, token);
        }



        /// <summary>
        /// 客户端发出要料呼叫
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="warehouseId"></param>
        /// <param name="dockNumber"></param>
        /// <param name="hasLGV"></param>
        /// <param name="runsType"></param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public string ClientCallPallet(string url, string sessionKey, string warehouseId, string dockNumber,
            string hasLGV, string runsType, string token)
        {
            try
            {
                return CallPalletDAL.ClientCallPallet(url, sessionKey, warehouseId, dockNumber, hasLGV, runsType, token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("客户端要料呼叫出现错误:" + ex.Message);
                return "连接服务器失败！";
            }
        }

        /// <summary>
        /// 检查呼叫的实托盘是否有库存
        /// </summary>
        /// <param name="MachineNumber"></param>
        /// <param name="DockNumber"></param>
        /// <returns>TRUE: 有库存</returns>
        public bool CheckContainerInventory(string machineNumber, string dockNumber)
        {
            bool flag = false;
            try
            {
                flag = CallPalletDAL.CheckContainerInventory(_equipmentNumber, _userName, _userPassword, _warehouseID, machineNumber, dockNumber);
            }
            catch { }
            finally { }


            return flag;
        }


        /// <summary>
        /// 确认站台位是否允许作要料呼叫操作
        /// </summary>
        /// <param name="machineNunber">机台号</param>
        /// <param name="dockNumber">站台位号</param>
        /// <returns>TRUE:允许要料  FALSE：不允许要料</returns>
        public bool CheckAllowed_CallPallet(string machineNunber, string dockNumber)
        {
            CommonBLL cb = new CommonBLL(_equipmentNumber, _userName, _userPassword, _warehouseID);

            bool flag = false;
            //检查当前站台位置的状态(自己本身)
            if (cb.CheckDockState(machineNunber, dockNumber) && cb.CheckDockCurTaskByDockNumber(dockNumber))
            {
                flag = true;
            }

            //如果是成型机台检查成型机台的父站台对应的子站台是否已经退料完毕 PEQ 20110211
            //if (machineNunber.StartsWith("C"))
            //{
            //    //查询成型机台能要料的站台  PEQ 20110217
            //    //当使用位和备用位站台的信息都为空的时候，备用位置不能要料

            //    //检查是否是成型机台3号站台 PEQ 20110325
            //    flag = this.CheckCXAllowedCall(machineNunber, dockNumber);

            //    if (flag == true)
            //    {
            //        //如果不是检查是否存在父子站台要料关系
            //        if (!cb.CheckCXMachineStatus(_warehouseID, machineNunber, dockNumber))
            //        {
            //            flag = false;
            //        }
            //        else
            //        {
            //            flag = true;
            //        }
            //    }
            //}

            return flag;
        }

        //提示能操作的具体原因  add by pengenqi 2012/5/24
        public bool CheckDockState(string machineNunber, string dockNumber)
        {
            try
            {
                CommonBLL cb = new CommonBLL(_equipmentNumber, _userName, _userPassword, _warehouseID);
                return cb.CheckDockState(machineNunber, dockNumber);
            }
            catch { }
            return false;
        }

        //提示能操作的具体原因  add by pengenqi 2012/5/24
        public bool CheckDockCurTaskByDockNumber(string machineNunber, string dockNumber)
        {
            try
            {
                CommonBLL cb = new CommonBLL(_equipmentNumber, _userName, _userPassword, _warehouseID);
                return cb.CheckDockCurTaskByDockNumber(dockNumber);
            }
            catch { }
            return false;
        }

        public bool CheckCXAllowedCall(string machineNumber, string dockNumber)
        {
            try
            {
                string dockNum = dockNumber;
                int lastChar = 0;

                if (!string.IsNullOrEmpty(dockNum))
                {
                    lastChar = Convert.ToInt32(dockNum.Substring(dockNum.Length - 1, 1));

                    if (lastChar == 2)
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

        //

        /// <summary>
        /// 客户端发出退空托盘+要料呼叫
        /// </summary>
        /// <param name="machineNumber">要料机台号</param>
        /// <param name="dockNumber">要料站台位号</param>
        /// <param name="modelCode">区段号</param>
        /// <param name="hasLGV">是否有LGV（1：表示有LGV，0：表示没有LGV）</param>
        /// <returns>0:操作失败 1:退空托盘操作成功，要料失败 2:退空托盘+要料操作成功</returns>         
        public int ClientReturnAndCallPallet(string machineNumber, string dockNumber, string modelCode, string hasLGV, string Runs)
        {
            int flag = 0;
            try
            {
                flag = CallPalletDAL.ClientReturnAndCallPallet(_equipmentNumber, _userName, _userPassword, _warehouseID, machineNumber, dockNumber, modelCode, hasLGV, Runs);
            }
            catch
            { flag = 0; }
            finally { }

            return flag;
        }
    }
}
