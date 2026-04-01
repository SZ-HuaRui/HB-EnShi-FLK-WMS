using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public abstract class AbstractSqlServerOperation : IDbOperation<SqlParameter>
    {
        public abstract int ExecuteNoQueryBySql(string sql);
        public abstract DataTable ExecuteToTableByProcedure(string procedureName, SqlParameter[] param);
        public abstract DataTable ExecuteQueryBySql(string sql);
        public abstract string[] ExecuteToStrArrayByProcedure(string procedureName, SqlParameter[] param);

    }
}
