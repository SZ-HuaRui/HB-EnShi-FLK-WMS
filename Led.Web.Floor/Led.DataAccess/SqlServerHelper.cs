using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace DataAccess
{
    public class SqlServerHelper : AbstractSqlServerOperation
    {
        public SqlConnection conn { get; set; }
        public SqlCommand com { get; set; }
        public SqlDataAdapter adapter { get; set; }
        public string connStr { get; set; }
        public SqlServerHelper(string str)
        {
            connStr = System.Configuration.ConfigurationManager.AppSettings[str].ToString();
            conn = new SqlConnection(connStr);
            com = new SqlCommand();
            com.Connection = conn;
            adapter = new SqlDataAdapter();
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
            conn.Open();
            try
            {
                if (conn != null && com != null)
                {
                    return com.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return 2;
            }
            finally
            {
                conn.Close();
            }
            return -1;
        }

        /// <summary>
        /// 存储过程,返回DataTable
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns>DataTable</returns>
        public override DataTable ExecuteToTableByProcedure(string procedureName, SqlParameter[] param)
        {
            com.CommandType = CommandType.StoredProcedure;
            com.CommandText = procedureName;
            com.Parameters.Add(param);
            conn.Open();
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                if (conn != null && com != null && adapter != null)
                {
                    adapter.Fill(ds);
                    if (ds != null && ds.Tables != null)
                    {
                        dt = ds.Tables[0];
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                return dt;
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }
        /// <summary>
        /// 存储过程,返回string数组
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns>string[]</returns>
        public override string[] ExecuteToStrArrayByProcedure(string procedureName, SqlParameter[] param)
        {
            com.CommandType = CommandType.StoredProcedure;
            com.CommandText = procedureName;
            com.Parameters.Add(param);
            conn.Open();
            string[] strArray = new string[] { };
            try
            {
                com.ExecuteNonQuery();
                for (int i = 0; i < param.Length; i++)
                {
                    int j = 0;
                    if (param[i].Direction == ParameterDirection.Output)
                    {
                        strArray[j] = param[i].Value.ToString();
                        j++;
                    }
                }
                return strArray;
            }
            catch (Exception ex)
            {
                return strArray;
            }
            finally
            {
                conn.Close();
            }
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
            conn.Open();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                if (conn != null && com != null && adapter != null)
                {
                    adapter.Fill(ds);
                    if (ds != null && ds.Tables != null)
                    {
                        dt = ds.Tables[0];
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                return dt;
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }

    }
}
