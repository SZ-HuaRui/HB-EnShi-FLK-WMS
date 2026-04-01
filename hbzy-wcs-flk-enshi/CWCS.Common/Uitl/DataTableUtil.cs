using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Common.Uitl
{
    public class DataTableUtil
    {
        public static DataTable ListRowToDataTable(List<DataRow> list)
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }
            DataTable dt = list[0].Table.Clone();
            foreach (DataRow dr in list)
            {
                dt.ImportRow(dr);
            }
            return dt;
        }
    }
}
