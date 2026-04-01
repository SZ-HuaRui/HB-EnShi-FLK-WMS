using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;
using System.Collections;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public class Command_GetData
    {
        string _equipmentNumber;
        string _userName;
        string _password;
        string _warehouseID;
        private CommonBLL cb;

        public Command_GetData()
        {
            _equipmentNumber = LoginUser.EquipmentNumber;
            _userName = LoginUser.UserName;
            _password = LoginUser.Password;
            _warehouseID = LoginUser.WarehouseID;
        }

        //获取站台位
        public IList GetMachineDockInfo(string url, string sessionKey, string warehouseId, string machineNumber,string token)
        {
            IList list = null;
            try
            {
                cb = new CommonBLL();
                DataTable dtDock = cb.GetMachineDockInfo(url, sessionKey, warehouseId, machineNumber, token);

                if (dtDock != null && dtDock.Rows.Count > 0)
                {
                    DataView dv = new DataView(dtDock);
                    //dv.RowFilter = "SOURCE_FLAG = '1'";
                    if (dv.Count > 0)
                    {
                        list = BindDocks(list, dv);
                    }
                }
            }
            catch (Exception ex)
            {
                list = null;
            }
            return list;
        }

        public IList GetManMachineDockInfo(string url, string sessionKey, string warehouseId, string machineNumber)
        {
            IList list = null;
            try
            {
                cb = new CommonBLL(_equipmentNumber, _userName, _password, _warehouseID);
                DataTable dtDock = cb.GetMachineDockInfo(url, sessionKey, warehouseId, machineNumber);

                if (dtDock != null && dtDock.Rows.Count > 0)
                {
                    DataView dv = new DataView(dtDock);
                    dv.RowFilter = "SOURCE_FLAG = '0'";
                    if (dv.Count > 0)
                    {
                        list = BindDocks(list, dv);
                    }
                }
            }
            catch
            {
                list = null;
            }
            return list;
        }

        private IList BindDocks(IList list, DataView dv)
        {
            DataTable dt = dv.ToTable();
            string machineNumber = Check(dt.Rows[0]["machineCode"]);
            list = new ArrayList();
            foreach (DataRow row in dt.Rows)
            {
                DockInfo dock = new DockInfo();
                dock.MachineNumber = machineNumber;
                //dock.MachineType = Check(row["TYPE_CODE"]);
                dock.DockNumber = Check(row["dockcode"]);
                dock.MachineDockID = Check(row["dockId"]);
                dock.DockName = Check(row["dockname"]);
                dock.DockStatus = Check(row["dockstatus"]);
                dock.ModelCode = Check(row["sectionCode"]);
                //dock.PalletNumber = Check(row["CUR_PALLET"]);
                //dock.TaskNumber = Check(row["CUR_TASK_NUMBER"]);
                //dock.ItemSource = Check(row["SOURCE_FLAG"]);

                list.Add(dock);
            }
            return list;
        }

        public DataTable GetMachineDockInfom(string url, string sessionKey, string warehouseId, string machineNumber)
        {
            try
            {
                cb = new CommonBLL();
                return cb.GetMachineDockInfo(url, sessionKey, warehouseId, machineNumber);
            }
            catch
            {
                return null;
            }
        }

        public string Check(object obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString())) return null;
            return obj.ToString();
        }
    }
}