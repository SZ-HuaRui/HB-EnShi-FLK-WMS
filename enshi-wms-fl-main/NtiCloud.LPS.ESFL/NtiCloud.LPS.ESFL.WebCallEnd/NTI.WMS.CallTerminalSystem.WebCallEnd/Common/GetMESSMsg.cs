using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public class GetMESSMsg
    {
        bool userNameFocus = true;

        public static Call_Terminal_Log loginWindow;
        MessageBox messagebox = new MessageBox();
        string Equipment, Warehouseid, StrURl;
        string WareHouseID = string.Empty;
        string EquipmentNumber = string.Empty;
        string MachineNumber=string.Empty;
        string WorkShop=string.Empty;
        string MachineName=string.Empty;
        string UserName = string.Empty;
        string PassWord = string.Empty;
        private Dictionary<string, string> typeDictionary = new Dictionary<string, string>();
        private List<string> typeCode = new List<string>();

        public string Load()
        {
            LoginUser.WarehouseID=Properties.Settings.Default.WareHouseID;;
            LoginUser.EquipmentNumber=XmlReader.GetLocalEquipmentID();
            LoginUser.HasLGV=XmlReader.GetHasLGV();
            LoginUser.MachineNumber=XmlReader.GetMachineNumber();
            LoginUser.SelectWorkShop=WorkShop;
            LoginUser.MachineName=MachineName;
            LoginUser.Runs=XmlReader.GetClassGroup();//班组
            LoginUser.RunsTypeCode=XmlReader.GetClassTeam();//班次
            LoginUser.UserName=XmlReader.GetUserName();
            return "";
        }

       
        #region getWorkShop
        public void WorkShopDataBind()
        {
            try
            {
                CommonBLL bll = new CommonBLL(EquipmentNumber, UserName, PassWord, WareHouseID);
                DataSet ds = bll.GetZoneByMachineNumber(MachineNumber);
                DataTable datatable=ds.Tables[0];

                if (ds == null || ds.Tables.Count == 0)
                {
                   // ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('操作执行失败，无法访问服务器，或服务器未能返回有效信息！');</script>");
                    return ;
                }
                WorkShop=datatable.Rows[0]["CODE"].ToString();//获取车间代号
                MachineName=datatable.Rows[0]["DESCRIPTION"].ToString();
            }           
            catch
            {
                return;
            }
            finally { }
        }
        #endregion

        //插入当班接班信息 PEQ 20110118
        public bool CheckClassInfo()
        {
            bool operate = false;

            //自动生成班次
            DateTime dtNow = System.DateTime.Now;
            string autoRunId = string.Format("{0:yyyyMMddHHmmss}", dtNow);
            autoRunId = autoRunId + "-" + LoginUser.MachineNumber;

            //生产日期
            DateTime dateNow = new DateTime();
            dateNow = System.DateTime.Now.AddHours(-7);
            string strDateNow = string.Format("{0:u}", dateNow);
            int dateIndex = strDateNow.ToString().IndexOf(" ");
            string productNow = strDateNow.Substring(0, dateIndex).Trim();

            //BLL层
            CommonBLL commonBll = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

            string warehouseId = LoginUser.WarehouseID;
            string runId = autoRunId;
            string workShop = LoginUser.SelectWorkShop;
            string teamDate = LoginUser.RunsTypeCode;//班组
            string classState = "未交班";//状态
            string machineNumber = LoginUser.MachineNumber;
            string remark = "用户名";
            string autorunId = "";
            string productdate = productNow;

            //获取上一班已经交班的班次
            DataSet dsPreClass = commonBll.GetPreClassInfo(warehouseId, machineNumber, workShop, "未交班");
            string preClass = string.Empty;
            if (dsPreClass != null && dsPreClass.Tables.Count > 0 && dsPreClass.Tables[0].Rows.Count > 0)
                preClass = dsPreClass.Tables[0].Rows[0]["shiftcode"].ToString();

            //交接班的判断  PEQ 20110123


            try
            {
                //插入当前班的信息
                operate = commonBll.InsertClassInfo(warehouseId, runId, workShop, teamDate, classState, machineNumber, remark, autorunId, productdate, preClass);

                dsPreClass = commonBll.GetPreClassInfo(warehouseId, machineNumber, workShop, "已交班");
                if (dsPreClass != null && dsPreClass.Tables.Count > 0 && dsPreClass.Tables[0].Rows.Count > 0)
                    preClass = dsPreClass.Tables[0].Rows[0]["shiftcode"].ToString();
                //修改上一班的状态
                operate = this.UpdatePreClassStatus(warehouseId, workShop, machineNumber, preClass, "已完成", autorunId);

                return operate;
            }
            catch
            {
                return false;
            }
        }

        public DataSet GetCurClassInfo(string warehouseId, string machineNumber, string workShop, string classState)
        {
            CommonBLL commonBll = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            return commonBll.GetPreClassInfo(warehouseId, machineNumber, workShop, classState);
        }

        /// <summary>
        /// 修改上一班次的状态
        /// </summary>
        public bool UpdatePreClassStatus(string WarehouseID, string WorkShop, string MachineNumber, string RunID, string ClassState, string nextClass)
        {
            CommonBLL commonBll = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

            int updateClassStatus = commonBll.UpdateStatus(WarehouseID, WorkShop, MachineNumber, RunID, ClassState, nextClass);
            if (updateClassStatus > 0) return true;
            else
                return false;
        }
       
    }
}