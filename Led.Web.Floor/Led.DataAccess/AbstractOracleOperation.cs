using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public abstract class AbstractOracleOperation:IDbOperation<OracleParameter>
    {
        public abstract int ExecuteNoQueryBySql(string sql);
        public abstract DataTable ExecuteToTableByProcedure(string procedureName, OracleParameter[] param);
        public abstract DataTable ExecuteQueryBySql(string sql);
        public abstract string[] ExecuteToStrArrayByProcedure(string procedureName, OracleParameter[] param);
    }
}
