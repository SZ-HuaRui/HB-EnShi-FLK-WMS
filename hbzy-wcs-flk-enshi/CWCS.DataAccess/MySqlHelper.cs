using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DataAccess
{
    public class MySqlHelper:IDbHelper<MySqlParameter>
    {
        public MySqlConnection conn { get; set; }
        public MySqlCommand com { get; set; }
        public MySqlDataAdapter adapter { get; set; }
        public string connStr { get; set; }
        public MySqlHelper(string str)
        {
            connStr = System.Configuration.ConfigurationManager.AppSettings[str].ToString();
            conn = new MySqlConnection(connStr);
            com = new MySqlCommand();
            com.Connection = conn;
            adapter = new MySqlDataAdapter();
            adapter.SelectCommand = com;
        }



        /// <summary>
        /// sql更新
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool ExecuteNonQueryBySql(string sql)
        {
            bool isSuccess = false;
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
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


        /// <summary>
        /// 存储过程,返回DataTable
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteByProcedure(string procedureName, MySqlParameter[] param)
        {
            DataSet ds = new DataSet();
            com.CommandType = CommandType.StoredProcedure;
            com.CommandText = procedureName;
            if (param != null)
            {
                if (com != null && com.Parameters.Count > 0)
                {
                    com.Parameters.Clear();
                }
                foreach (MySqlParameter parm in param)
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
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
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

        public DataTable ExecuteQueryBySqlParams(string sql, MySqlParameter[] param)
        {
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
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
                foreach (MySqlParameter parm in param)
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


        public bool ExecuteNonQueryBySqlParams(string sql, MySqlParameter[] param)
        {
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            bool isSuccess = false;
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
                foreach (MySqlParameter parm in param)
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

        public bool ExecuteManyNonQueryBySql(List<string> sqlList)
        {
            throw new NotImplementedException();
        }
    }
}
