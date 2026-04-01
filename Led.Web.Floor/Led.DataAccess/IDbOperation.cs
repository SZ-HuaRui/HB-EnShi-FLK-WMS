using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IDbOperation<T>
    {
        int ExecuteNoQueryBySql(string sql);
        DataTable ExecuteToTableByProcedure(string procedureName, T[] param);
        DataTable ExecuteQueryBySql(string sql);
        string[] ExecuteToStrArrayByProcedure(string procedureName, T[] param);
    }
}
