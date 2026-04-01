using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public class AccessHelper
    {
        private OleDbConnection Connection;
        private OleDbCommand Command;
        private OleDbDataAdapter Adapter;
        public AccessHelper(string connectionText)
        {
            Connection = new OleDbConnection(System.Configuration.ConfigurationManager.AppSettings[connectionText].ToString());
            Command = new OleDbCommand();
            Adapter = new OleDbDataAdapter();
            Command.Connection = Connection;
            Adapter = new OleDbDataAdapter();
            Adapter.SelectCommand = Command;
        }

        public DataTable ExecuteQueryBySql(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                Command.CommandType = CommandType.Text;
                Command.CommandText = sql;
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }
                Adapter.Fill(dt);
            }
            catch (Exception ex)
            {

            }
            finally
            {
               Connection?.Close();
            }
            return dt;
        }

        ~AccessHelper()
        {
             Connection?.Close();
        }

    }
}
