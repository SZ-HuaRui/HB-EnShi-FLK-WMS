using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NTI.WMS.CallTerminalSystem.BLL
{
    public class RowFilterData
    {
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

    }
}
