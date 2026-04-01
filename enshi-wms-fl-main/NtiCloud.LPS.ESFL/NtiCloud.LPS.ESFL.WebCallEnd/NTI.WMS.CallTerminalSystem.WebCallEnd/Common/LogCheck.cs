using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTI.WMS.CallTerminalSystem.BLL;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    class DataDisposal
    {
        public static int check(string equipmentNumber, string username, string password, string warehouseID)
        {
            return 0;
        }

        public static int Login(string username, string password)
        {
            string equipmentNumber = LoginUser.EquipmentNumber;
            string warehouseID = LoginUser.WarehouseID;
            String runsTypeCode = LoginUser.RunsTypeCode;
            int isuser = check(equipmentNumber, username, password, warehouseID);
            if (isuser == 1)
            {
                LoginUser.WarehouseID = warehouseID;
                LoginUser.UserName = username;
                LoginUser.Password = password;
                LoginUser.EquipmentNumber = equipmentNumber;
                LoginUser.RunsTypeCode = runsTypeCode;
                LoginUser.LoadUserPermissions(username);
                LoginUser.IsAdmin = LoginUser.IsAdministrator();
                LoginUser.MachineNumber = XmlReader.GetMachineNumber();
                LoginUser.MachineName = XmlReader.GetMachineName();
                LoginUser.HasLGV = XmlReader.GetHasLGV();
            }
            else
            {
                LoginUser.UserName = "";
                LoginUser.Password = "";
                LoginUser.ClearUserPermissions();
            }
            return isuser;
        }

        public static bool CheckAutoLoginUser(string warehouseId, string code, out string user)
        {
            return new LoginBLL().CheckAutoLoginUser(warehouseId, code, out user);
        }

        public static bool UpdateMachineProductPlan(string warehouseId, string machineNumber, string planNo, string state, out string msg)
        {
            return new LoginBLL().UpdateMachineProductPlan(warehouseId, machineNumber, planNo, state, out msg);
        }

        public static bool updatePlan(string warehouseId, string equitmentidwd, string equitmentnumberwd, string schemeidwd, string itemidwd, string itemcodewd, string statecodewd, string memowd, string useridwd, out string msg) 
        {
            return new LoginBLL().updatePlan(warehouseId, equitmentidwd, equitmentnumberwd, schemeidwd, itemidwd, itemcodewd, statecodewd, memowd, useridwd, out msg);
        }

        //add by wd updateSendTest
        public static bool updateSendTest(string warehouseId, string equitmentidwd, string equitmentnumberwd, string schemeidwd, string itemidwd, string itemcodewd, string statecodewd, string memowd, string useridwd, out string msg)
        {
            return new LoginBLL().updateSendTest(warehouseId, equitmentidwd, equitmentnumberwd, schemeidwd, itemidwd, itemcodewd, statecodewd, memowd, useridwd, out msg);
        }



    }

    /// <summary>
    /// 用户权限
    /// </summary>
    public class LoginPermission
    {
        /// <summary>
        /// 获取用户权限
        /// </summary>
        /// <returns>List<string>权限</returns>
        public List<string> GetUserPermission(string username)
        {
            List<string> userPermission = new LoginBLL().GetLogingUserPermission(LoginUser.EquipmentNumber, username, LoginUser.Password, LoginUser.WarehouseID);
            return userPermission;
        }
    }
}