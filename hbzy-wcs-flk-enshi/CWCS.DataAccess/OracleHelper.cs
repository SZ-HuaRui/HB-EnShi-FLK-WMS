using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    /// <summary>
    /// Author:zpw
    /// Description:Util class of accessing database 
    /// </summary>
    public class OracleHelper:IDbHelper<OracleParameter>
    {
        private string connStr { get; set; }

        private static object lockObject = new object();

        private static OracleHelper _instance = null;

        private static OracleHelper _wmsInstance = null;

        private OracleHelper(string str)
        {
                connStr = System.Configuration.ConfigurationManager.AppSettings[str].ToString();
        }



        public static OracleHelper WcsInstance(string connStr)
        {
            lock (lockObject)
            {
                if (_instance == null)
                {
                    _instance = new OracleHelper(connStr);
                }
                return _instance;
            }
        }

        public static OracleHelper WmsInstance(string connStr)
        {
            lock (lockObject)
            {
                if (_wmsInstance == null)
                {
                    _wmsInstance = new OracleHelper(connStr);
                }
                return _wmsInstance;
            }
        }


        /// <summary>
        /// sql更新
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool ExecuteNonQueryBySql(string sql)
        {
            bool isSuccess = false;
            OracleConnection conn = new OracleConnection(connStr);
            OracleCommand com = new OracleCommand();
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            com.Connection = conn;
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                if (com.ExecuteNonQuery() > 0) isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }
            finally
            {
                conn?.Close();
            }
            return isSuccess;
        }

        public DataTable ExecuteByProcedure(string procedureName, OracleParameter[] param)
        {
            DataSet ds = new DataSet();
            OracleConnection conn = new OracleConnection(connStr);
            OracleCommand com = new OracleCommand();
            com.CommandType = CommandType.StoredProcedure;
            com.CommandText = procedureName;
            com.CommandTimeout = 100;
            OracleDataAdapter adapter = new OracleDataAdapter();
            com.Connection = conn;
            adapter.SelectCommand = com;
            if (param != null)
            {
                if (com != null && com.Parameters.Count > 0)
                {
                    com.Parameters.Clear();
                }
                foreach (OracleParameter parm in param)
                    com.Parameters.Add(parm);
            }
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                conn?.Close();
                if (com != null && com.Parameters.Count > 0)
                {
                    com.Parameters.Clear();
                }
            }
            return ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        /// <summary>
        /// Sql查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecuteQueryBySql(string sql)
        {
            OracleConnection conn = new OracleConnection(connStr);
            OracleCommand com = new OracleCommand();
            OracleDataAdapter adapter = new OracleDataAdapter();
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            com.Connection = conn;
            adapter.SelectCommand = com;
            DataSet ds = new DataSet();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                conn?.Close();
            }
            return ds.Tables.Count > 0 ? ds.Tables[0] : null;
           
        }


        public DataTable ExecuteQueryBySqlParams(string sql, OracleParameter[] param)
        {
            OracleConnection conn = new OracleConnection(connStr);
            OracleCommand com = new OracleCommand();
            OracleDataAdapter adapter = new OracleDataAdapter();
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            com.Connection = conn;
            adapter.SelectCommand = com;
            DataSet ds = new DataSet();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                if (com != null && com.Parameters.Count > 0)
                {
                    com.Parameters.Clear();
                }
                foreach (OracleParameter parm in param)
                    com.Parameters.Add(parm);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                conn?.Close();
            }
            return ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }


        public bool ExecuteNonQueryBySqlParams(string sql, OracleParameter[] param)
        {
            bool isSuccess = false;
            OracleConnection conn = new OracleConnection(connStr);
            OracleCommand com = new OracleCommand();
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            com.Connection = conn;
            DataSet ds = new DataSet();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                if (com != null && com.Parameters.Count > 0)
                {
                    com.Parameters.Clear();
                }
                foreach (OracleParameter parm in param)
                    com.Parameters.Add(parm);
                if (com.ExecuteNonQuery() > 0) isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }
            finally
            {
                conn?.Close();
            }
            return isSuccess;
        }

        /// <summary>
        /// 执行多条语句
        /// </summary>
        /// <param name="sqlList"></param>
        /// <returns></returns>
        public bool ExecuteManyNonQueryBySql(List<string> sqlList)
        {
            bool isSuccess = false;
            OracleConnection conn = new OracleConnection(connStr);
            OracleCommand com = new OracleCommand();
            com.CommandType = CommandType.Text;
            com.Connection = conn;
            OracleTransaction transaction = null;
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                transaction = conn.BeginTransaction();
                foreach (string sql in sqlList)
                {
                    com.CommandText = sql;
                    if(com.ExecuteNonQuery() < 1)
                    {
                        throw new Exception($"执行语句:{sql}失败");
                    }
                }
                transaction.Commit();
                isSuccess = true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                isSuccess = false;
            }
            finally
            {
                conn?.Close();
            }
            return isSuccess;
        }
    }
}
