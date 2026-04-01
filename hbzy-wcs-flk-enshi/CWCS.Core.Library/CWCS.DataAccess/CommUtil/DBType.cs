using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.DataAccess.CommUtil
{
    /// <summary>
    /// 数据库管理系统枚举
    /// </summary>
    public enum DBMSType
    {
        Access,
        SqlServer,
        SqlServerCe,
        Oracle,
        OracleMS,
        DB2,
        Sysbase,
        MySql,
        SQLite,
        PostgreSQL,
        UNKNOWN = 999
    }

    /// <summary>
    /// 数据源类型
    /// </summary>
    public enum DataSourceType
    {
        OleDb,
        SqlServer,
        SqlServerCe,
        SQLite,
        Oracle,
        OracleMS,
        Odbc,
        TextFile,
        XML
    }
    public delegate TResult MyFunc<T, TResult>(T arg);

    public delegate TResult MyFunc<T1, T2, TResult>(T1 arg1, T2 arg2);

    public delegate void MyAction<T1, T2>(T1 arg1, T2 arg2);

    public delegate void MyAction<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
}
