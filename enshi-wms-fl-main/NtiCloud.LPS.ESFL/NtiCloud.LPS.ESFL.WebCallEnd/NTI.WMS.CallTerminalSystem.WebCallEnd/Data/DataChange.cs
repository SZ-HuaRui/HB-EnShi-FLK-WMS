using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Windows.Controls;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public class DataChange
    {
        public DataTable RowFilterFromDataset(DataSet ds, string filterStr)
        {
            DataTable dt = null;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                dt = RowFilterFromDataset(ds.Tables[0], filterStr);
            }
            return dt;
        }

        public DataTable RowFilterFromDataset(DataTable dt, string filterStr)
        {
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            if (string.IsNullOrEmpty(filterStr))
            {
                return dt;
            }
            try
            {
                DataView dv = new DataView(dt);
                dv.RowFilter = filterStr;
                if (dv != null)
                {
                    dt = dv.ToTable();
                }
                //if (dv.Count > 0)
                //{
                //    dt = dv.ToTable();
                //}
            }
            catch
            {
                dt = null;
            }
            return dt;
        }
    }

    class Class_CheckIsNumber
    {
        public static bool CheckNumber(char KeyValue, string Text)
        {
            if (KeyValue > 47 && KeyValue < 58)
            {
                Text = Text + KeyValue.ToString();
                return true;

            }
            else if (KeyValue == 8)
            {
                return true;

            }

            else if (KeyValue == 46)
            {
                if (string.IsNullOrEmpty(Text))
                {
                    return false;

                }
                if (Text.Contains("."))
                    return false;
                else
                    return true;

            }
            else
            {
                return false;
            }
        }
    }
}