using CWCS.DataAccess.CommUtil;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;

namespace CWCS.DataAccess.DataProvider
{
    /// <summary>
    /// OracleServer 数据处理
    /// </summary>
    public sealed class OracleMSHelper : AdoHelper
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public OracleMSHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 获取当前数据库类型的枚举
        /// </summary>
        public override DBMSType CurrentDBMSType
        {
            get { return DBMSType.OracleMS; }
        }

        /// <summary>
        /// 创建并且打开数据库连接
        /// </summary>
        /// <returns>数据库连接</returns>
        protected override IDbConnection GetConnection()
        {
            IDbConnection conn = base.GetConnection();
            if (conn == null)
            {
                conn = new OracleConnection(base.ConnectionString);
            }
            return conn;
        }

        /// <summary>
        /// 获取数据适配器实例
        /// </summary>
        /// <returns>数据适配器</returns>
        protected override IDbDataAdapter GetDataAdapter(IDbCommand command)
        {
            IDbDataAdapter ada = new OracleDataAdapter((OracleCommand)command);
            return ada;
        }

        /// <summary>
        /// 获取一个新参数对象
        /// </summary>
        /// <returns>特定于数据源的参数对象</returns>
        public override IDataParameter GetParameter()
        {
            return new OracleParameter();
        }

        /*
         * 
                //Oracle 处理自增
                //string seqName = entity.GetTableName() + "_" + entity.GetIdentityName() + "_SEQ";
                //CurrentDataBase.InsertKey = "select " + seqName + ".currval from dual;";
         */
        /// <summary>
        /// Oracle 不支持自增，请自己创建触发器和序列
        /// </summary>
        public override string InsertKey
        {
            get;
            set;
        }

        /// <summary>
        ///  获取一个新参数对象
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="dbType">参数数据类型</param>
        /// <param name="size">参数大小</param>
        /// <returns>特定于数据源的参数对象</returns>
        public override IDataParameter GetParameter(string paraName, System.Data.DbType dbType, int size)
        {
            OracleParameter para = new OracleParameter();
            para.ParameterName = paraName;
            if (size > 2000)
            {
                para.OracleType = OracleType.NClob;
                para.Size = size;
            }
            else
            {
                para.DbType = dbType;
                para.Size = size;
            }

            return para;
        }

        public override string GetNativeDbTypeName(IDataParameter para)
        {
            OracleParameter oraPara = (OracleParameter)para;
            OracleType oraType = oraPara.OracleType;
            if (oraType == OracleType.DateTime)
                return "Date";
            else if (oraType == OracleType.Int32)
                return "INT";
            else
                return oraType.ToString();

        }

        /// <summary>
        /// 返回此 OracleConnection 的数据源的架构信息。
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="restrictionValues">请求的架构的一组限制值</param>
        /// <returns>数据库架构信息表</returns>
        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            using (OracleConnection conn = (OracleConnection)this.GetConnection())
            {
                conn.Open();
                if (restrictionValues == null && string.IsNullOrEmpty(collectionName))
                    return conn.GetSchema();
                else if (restrictionValues == null && !string.IsNullOrEmpty(collectionName))
                    return conn.GetSchema(collectionName);
                else
                    return conn.GetSchema(collectionName, restrictionValues);
            }
        }

        /// <summary>
        /// 预处理SQL语句，语句中不能包含"["，"]"左右中括号，如果需要，请使用参数化查询。
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected override string PrepareSQL(string sql)
        {
            return sql.Replace("[", "\"").Replace("]", "\"").Replace("@", ":");
        }

        public override System.Data.Common.DbConnectionStringBuilder ConnectionStringBuilder
        {
            get { return new OracleConnectionStringBuilder(this.ConnectionString); }
        }

        public override string ConnectionUserID
        {
            get { return ((OracleConnectionStringBuilder)ConnectionStringBuilder).UserID; }
        }

        /// <summary>
        /// 获取Oracle数据库参数前缀资费
        /// </summary>
        public override string GetParameterChar
        {
            get
            {
                return ":";
            }
        }
    }
}
