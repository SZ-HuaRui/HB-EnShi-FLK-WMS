using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    /*
     * Author:zpw
     * Description:Interface of database operation
     * 
     */
    public interface IDbHelper<T>
    {
        bool ExecuteNonQueryBySql(string sql);
        bool ExecuteNonQueryBySqlParams(string sql, T[] param);
        DataTable ExecuteQueryBySql(string sql);
        DataTable ExecuteQueryBySqlParams(string sql, T[] param);
        DataTable ExecuteByProcedure(string procedureName, T[] param);
        bool ExecuteManyNonQueryBySql(List<string> sqlList);

    }
}
