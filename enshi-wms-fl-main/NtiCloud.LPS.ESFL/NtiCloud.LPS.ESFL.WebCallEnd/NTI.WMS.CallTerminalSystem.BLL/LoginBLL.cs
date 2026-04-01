using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTI.WMS.CallTerminalSystem.DAL;
using System.Data;

namespace NTI.WMS.CallTerminalSystem.BLL
{
    public class LoginBLL
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="url">微服务地址</param>
        /// <param name="UserName"></param>
        /// <param name="UserPassword"></param>
        /// <param name="sessionKey"></param>
        /// <returns>0表示远程服务器失败；1表示密码尝试超过限制，该用户已锁定；-1表示用户名或密码错误;2表示登录成功</returns>
        public int WorkShop_Loging(string url, string UserName, string UserPassword, string ip, out string sessionKey)
        {
            sessionKey = "";
            int result = 0;
            try
            {
                result = CommonDAL.WorkShop_Loging(url, UserName, UserPassword,ip, out sessionKey);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("验证登录出现错误:" + ex.Message);
            }
            finally { }

            return result;
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sessionKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool WorkShop_GetToken(string url, string sessionKey,string clientId, string warehouseID,string appCode, out string token)
        {
            token = "";
            bool result = false;
            try
            {
                result = CommonDAL.WorkShop_GetToken(url, sessionKey, clientId, warehouseID, appCode, out token);
                //result = CommonDAL.WorkShop_GetToken2(url, sessionKey, out token);
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("验证登录出现错误:" + ex.Message);
            }
            finally { }

            return result;
        }

        ///// <summary>
        ///// 获取token
        ///// </summary>
        ///// <param name="url"></param>
        ///// <param name="sessionKey"></param>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //public bool WorkShop_GetUserToken(string url, string sessionKey, out string token)
        //{
        //    token = "";
        //    bool result = false;
        //    try
        //    {
        //        result = CommonDAL.WorkShop_GetToken(url, sessionKey, out token);
        //        //result = CommonDAL.WorkShop_GetToken2(url, sessionKey, out token);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriterErrorLogs("验证登录出现错误:" + ex.Message);
        //    }
        //    finally { }

        //    return result;
        //}

        //自动登录验证用户
        public bool CheckAutoLoginUser(string warehouseId, string code, out string user)
        {
            try
            {
                DataTable dt = CommonDAL.CheckAutoLoginUser(warehouseId, code);
                if (dt != null && dt.Rows.Count > 0)
                {
                    user = dt.Rows[0]["userid"].ToString().Trim();
                    //LogWriter.WriterSystemLogs("编号为“" + code + "”的用户验证通过。");
                    return true;
                }
                LogWriter.WriterSystemLogs("根据工号“" + code + "”无法匹配到用户信息！");
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("根据工号“" + code + "”验证用户信息出现错误：" + ex.Message);
                throw ex;
            }

            user = "";
            return false;
        }

        public bool UpdateMachineProductPlan(string warehouseId, string machineNumber, string planNo, string state, out string msg)
        {
            return CommonDAL.UpdateMachineProductPlan(warehouseId, machineNumber, planNo, state, out msg);
        }

        public bool updatePlan(string warehouseId, string equitmentidwd, string equitmentnumberwd, string schemeidwd, string itemidwd, string itemcodewd, string statecodewd, string memowd, string useridwd, out string msg)
        {
            return CommonDAL.updatePlan(warehouseId, equitmentidwd, equitmentnumberwd, schemeidwd, itemidwd, itemcodewd, statecodewd, memowd, useridwd, out msg);
        }


        //add by wd updateSendTest
        public bool updateSendTest(string warehouseId, string equitmentidwd, string equitmentnumberwd, string schemeidwd, string itemidwd, string itemcodewd, string statecodewd, string memowd, string useridwd, out string msg)
        {
            return CommonDAL.updateSendTest(warehouseId, equitmentidwd, equitmentnumberwd, schemeidwd, itemidwd, itemcodewd, statecodewd, memowd, useridwd, out msg);
        }



        /// <summary>
        /// 获取权限 QWH 2009/06/08
        /// </summary>
        /// <param name="EquipmentNumber">设备号</param>
        /// <param name="UserName">用户ID</param>
        /// <param name="UserPassword">密码</param>
        /// <param name="WareHouseID">仓库ID</param>
        /// <returns></returns>
        public List<string> GetLogingUserPermission(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID)
        {
            List<string> list = null;
            try
            {
                list = CommonDAL.GetLogingUserPermission(EquipmentNumber, UserName, UserPassword, WareHouseID);
            }
            catch
            {
                list = null;
            }
            finally { }
            return list;
        }
    }
}
