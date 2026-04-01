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
    public class OracleHelper : AbstractOracleOperation
    {
        public OracleConnection conn { get; set; }
        public OracleCommand com { get; set; }
        public OracleDataAdapter adapter { get; set; }
        public string connStr { get; set; }
        public OracleHelper(string str)
        {
            connStr = System.Configuration.ConfigurationManager.AppSettings[str].ToString();
            conn = new OracleConnection(connStr);
            com = new OracleCommand();
            com.Connection = conn;
            adapter = new OracleDataAdapter();
            adapter.SelectCommand = com;
        }

       
        /// <summary>
        /// sql更新
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override int ExecuteNoQueryBySql(string sql)
        {
            
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            DbTransaction transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();
                if (conn != null && com != null)
                {
                    com.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
            catch (Exception)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                return 0;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return 1;
        }

        /// <summary>
        /// 存储过程,返回DataTable
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns>DataTable</returns>
        public override DataTable ExecuteToTableByProcedure(string procedureName, OracleParameter[] param)
        {
            com.CommandType = CommandType.StoredProcedure;
            com.CommandText = procedureName;
            if (param != null)
            {
                if (com != null && com.Parameters.Count > 0)
                {
                    com.Parameters.Clear();
                }
                foreach (OracleParameter parm in param)
                    com.Parameters.Add(parm);
            }
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                conn.Open();
                if (conn != null && com != null && adapter != null)
                {
                    adapter.Fill(ds);
                    if (ds != null && ds.Tables != null)
                    {
                        dt = ds.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
                if (com != null && com.Parameters.Count > 0)
                {
                    com.Parameters.Clear();
                }
            }
            return dt;
        }
        /// <summary>
        /// 存储过程,返回string数组
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns>string[]</returns>
        public override string[] ExecuteToStrArrayByProcedure(string procedureName, OracleParameter[] param)
        {
            com.CommandType = CommandType.StoredProcedure;
            com.CommandText = procedureName;
            if (param != null)
            {
                if (com != null && com.Parameters.Count > 0)
                {
                    com.Parameters.Clear();
                }
              
                foreach (OracleParameter parm in param)
                    com.Parameters.Add(parm);
            }
            string[] strArray = new string[param.Length];
            try
            {
                conn.Open();
                com.ExecuteNonQuery();
                int j = 0;
                for (int i = 0; i < param.Length; i++)
                {
                        strArray[j] = com.Parameters[i].Value.ToString();
                        j++;
                }
             
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
                if (com != null && com.Parameters.Count > 0)
                {
                    com.Parameters.Clear();
                }
            }
            return strArray;
        }

        /// <summary>
        /// Sql查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override DataTable ExecuteQueryBySql(string sql)
        {
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                conn.Open();
                if (conn != null && com != null && adapter != null)
                {
                    adapter.Fill(ds);
                    if (ds != null && ds.Tables != null)
                    {
                        dt = ds.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return dt;
        }

    }
}
