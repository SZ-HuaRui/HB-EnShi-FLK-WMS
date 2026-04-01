using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public class LoginUser
    {
        public static string Url;
        public static string WarehouseID;
        public static string ClientId;
        public static string AppCode;
        /// <summary>
        /// 调用微服务api/platform/login/getlogin，返回的sessionKey
        /// </summary>
        public static string SessionKey
        {
            get
            {
                return GetSeesionKey("SessionKey").ToString();
            }
            set
            {
                HttpContext.Current.Session["SessionKey"] = value;
            }
        }
        /// <summary>
        /// 调用微服务api/oauth2/token，返回的token
        /// </summary>
        public static string Token
        {
            get
            {
                return GetSeesionKey("Token").ToString();
            }
            set
            {
                HttpContext.Current.Session["Token"] = value;
            }
        }

        public static string UserName
        {
            get
            {
                return GetSeesionKey("UserName").ToString();
            }
            set
            {
                HttpContext.Current.Session["UserName"] = value;
            }
        }
        public static string Password = "NTI";
        public static string EquipmentNumber;

        public static bool IsAdmin;

        /// <summary>
        /// 生产工单id
        /// </summary>
        public static string SchemeId
        {
            get
            {
                return GetSeesionKey("SchemeId").ToString();
            }
            set
            {
                HttpContext.Current.Session["SchemeId"] = value;
            }
        }

        /// <summary>
        /// 班次id
        /// </summary>
        public static string RunsCodeId
        {
            get
            {
                return GetSeesionKey("RunsCodeId").ToString();
            }
            set
            {
                HttpContext.Current.Session["RunsCodeId"] = value;
            }
        }

        /// <summary>
        /// 班组id
        /// </summary>
        public static string RunsGroupId
        {
            get
            {
                return GetSeesionKey("RunsGroupId").ToString();
            }
            set
            {
                HttpContext.Current.Session["RunsGroupId"] = value;
            }
        }

        /// <summary>
        /// 班次名称
        /// </summary>
        public static string RunsCode
        {
            get
            {
                return GetSeesionKey("RunsCode").ToString();
            }
            set
            {
                HttpContext.Current.Session["RunsCode"] = value;
            }
        }

        /// <summary>
        /// 班组名称
        /// </summary>
        public static string RunsGroup
        {
            get
            {
                return GetSeesionKey("RunsGroup").ToString();
            }
            set
            {
                HttpContext.Current.Session["RunsGroup"] = value;
            }
        }

        /// <summary>
        /// 班组-班次
        /// </summary>
        public static string RunsTypeCode
        {
            get
            {
                return GetSeesionKey("RunsTypeCode").ToString();
            }
            set
            {
                HttpContext.Current.Session["RunsTypeCode"] = value;
            }
        }

        /// <summary>
        /// 机台id
        /// </summary>
        public static string MachineId
        {
            get
            {
                return GetSeesionKey("MachineId").ToString();
            }
            set
            {
                HttpContext.Current.Session["MachineId"] = value;
            }
        }

        /// <summary>
        /// 机台编号
        /// </summary>
        public static string MachineNumber
        {
            get
            {
                return GetSeesionKey("MachineNumber").ToString();
            }
            set
            {
                HttpContext.Current.Session["MachineNumber"] = value;
            }
        }

        /// <summary>
        /// 机台名称
        /// </summary>
        public static string MachineName
        {
            get
            {
                return GetSeesionKey("MachineName").ToString();
            }
            set
            {
                HttpContext.Current.Session["MachineName"] = value;
            }
        }

        /// <summary>
        /// 站台编码
        /// </summary>
        public static string DockNumber
        {
            get
            {
                return GetSeesionKey("DockNumber").ToString();
            }
            set
            {
                HttpContext.Current.Session["DockNumber"] = value;
            }
        }

        public static string ItemID
        {
            get
            {
                return GetSeesionKey("ItemID").ToString();
            }
            set
            {
                HttpContext.Current.Session["ItemID"] = value;
            }
        }

        /// <summary>
        /// 生产牌号
        /// </summary>
        public static string ItemName
        {
            get
            {
                return GetSeesionKey("ItemName").ToString();
            }
            set
            {
                HttpContext.Current.Session["ItemName"] = value;
            }
        }

        /// <summary>
        /// 车间代码
        /// </summary>
        public static string WorkShop
        {
            get
            {
                return GetSeesionKey("WorkShop").ToString();
            }
            set
            {
                HttpContext.Current.Session["WorkShop"] = value;
            }
        }

        /// <summary>
        /// 车间名称
        /// </summary>
        public static string WorkShopName
        {
            get
            {
                return GetSeesionKey("WorkShopName").ToString();
            }
            set
            {
                HttpContext.Current.Session["WorkShopName"] = value;
            }
        }

        public static int HasLGV
        {
            get
            {
                string value = GetSeesionKey("HasLGV").ToString();
                return value == "" ? 1 : Convert.ToInt32(value);
            }
            set
            {
                HttpContext.Current.Session["HasLGV"] = value;
            }
        }

        public static bool Recycle
        {
            get
            {
                string value = GetSeesionKey("Recycle").ToString();
                return value == "" ? true : Convert.ToBoolean(value);
            }
            set
            {
                HttpContext.Current.Session["Recycle"] = value;
            }
        }

        public static string[] tempermission;   //权限
        public static List<string> PermissionList = new List<string>();

        private static object GetSeesionKey(string key)
        {
            if (HttpContext.Current.Session == null) return "";
            if (HttpContext.Current.Session[key] == null) return "";

            return HttpContext.Current.Session[key];
        }

        public static void LoadUserPermissions(string username)
        {
            LoginPermission per = new LoginPermission();
            PermissionList = per.GetUserPermission(username);
            if (PermissionList != null)
            {
                tempermission = new string[PermissionList.Count];
                PermissionList.CopyTo(tempermission);
            }
            else
                tempermission = null;
        }

        public static void ClearUserPermissions()
        {
            PermissionList.Clear();
        }

        public static bool IsAdministrator()
        {
            if (IsNTIAdmin()) return true;
            return PermissionList.Contains(PermissionConst.Admin);
        }

        public static bool IsNTIAdmin()
        {
            return (UserName.StartsWith("*"));
        }
    }
}