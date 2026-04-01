using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text.RegularExpressions;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public class CommonFuctions
    {
        public static bool CheckQty(string qty, string odd1, string odd2, string odd3)
        {
            string formatQty1 = "^\\d+$";//"^([1-9][0-9]*)|(0)$"; //0 or 正整数
            string formatQty2 = "^(([1-9][0-9]*|(0))([.][0-9]{1,3})?)$"; //0  or 正小数
            string formatOdd = "^0([.][0-9]{1,3})?$";//0  or 0.?
            bool result = true;
            if (Properties.Settings.Default.AllowODD)//允许零头
            {
                if ((!Regex.IsMatch(qty, formatQty1)) || (!Regex.IsMatch(odd1, formatOdd)) || (!Regex.IsMatch(odd2, formatOdd)) || (!Regex.IsMatch(odd3, formatOdd)))
                    result = false;
            }
            else
            {
                if (!Regex.IsMatch(qty, formatQty2))
                    result = false;
            }
            return result;

        }
        /// <summary>
        /// 检查数量
        /// </summary>
        /// <returns></returns>
        public static bool CheckQty(string qty)
        {
            string formatQty1 = "^\\d+$";//"^([1-9][0-9]*)|(0)$";
            string formatQty2 = "^(([1-9][0-9]*|(0))([.][0-9]{1,3})?)$";
            bool result = false;
            if (Properties.Settings.Default.AllowODD)//允许零头
            {
                if (Regex.IsMatch(qty, formatQty2))
                    result = true;
            }
            else
            {
                if (Regex.IsMatch(qty, formatQty1))
                    result = true;
            }
            return result;
        }
        //验证文本框输入为数字
        public static bool GetNum(string str)
        {
            string r = "^\\d+$";　　//正整数     
            return Regex.IsMatch(str, r);
        }
        public static bool GetDecemal(string str)
        {
            string formatQty2 = "^(([1-9][0-9]*|(0))([.][0-9]{1,3})?)$";
            return Regex.IsMatch(str, formatQty2);
        }

        public static bool CheckDecemal(string str)
        {
            string formatQty2 = "^\\d+(\\.\\d+)?$";
            return Regex.IsMatch(str, formatQty2);
        }

        public static bool CheckDecemal1(string str)
        {
            string formatQty2 = "^(-?\\d+)(\\.\\d+)?$";
            return Regex.IsMatch(str, formatQty2);
        }

        public static DataTable RowFilterFromDataTable(DataTable dt, string FilterStr)
        {
            DataTable dtt = null;
            if (dt != null && dt.Rows.Count > 0 && !string.IsNullOrEmpty(FilterStr))
            {
                try
                {
                    DataView dv = new DataView(dt);
                    dv.RowFilter = FilterStr;
                    if (dv.Count > 0)
                    {
                        dtt = dv.ToTable();
                    }
                    dv.RowFilter = "";
                }
                catch
                {
                    dtt = null;
                }
            }

            return dtt;
        }

        public static DataTable RowFilterFromDataset(DataSet ds, string FilterStr)
        {
            DataTable dt = null;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && !string.IsNullOrEmpty(FilterStr))
            {
                try
                {
                    DataView dv = new DataView(ds.Tables[0]);
                    dv.RowFilter = FilterStr;
                    if (dv.Count > 0)
                    {
                        dt = dv.ToTable();
                    }
                    dv.RowFilter = "";
                }
                catch
                {
                    dt = null;
                }
            }

            return dt;
        }
        public static string GetHttpHostPath(string PathFileName)
        {
            return "http://" + HttpContext.Current.Request.ServerVariables["HTTP_HOST"] +
                HttpContext.Current.Request.ApplicationPath + PathFileName;
        }
    }
}