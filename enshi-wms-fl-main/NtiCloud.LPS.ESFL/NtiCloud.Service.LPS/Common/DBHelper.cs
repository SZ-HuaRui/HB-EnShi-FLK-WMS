using Newtonsoft.Json;
//using NtiCloud.Kernel;
using NtiCloud.Kernel.Core; 
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
//using kernelDataBase = NtiCloud.Kernel.KernelDataBase;

namespace NtiCloud.Service.WMS.Common
{
    public class DBHelper
    {
        public static IDBContextProvider kernelDataBase;

        public static void Init(IDBContextProvider dbProvider)
        {
            kernelDataBase = dbProvider;
        }

        public static void BeginTransaction()
        {
            kernelDataBase.BeginTransaction(Context.UserInfo);
        }

        public static void EndTransaction(bool commit = true)
        {
            kernelDataBase.EndTransaction(Context.UserInfo, commit);
        }

        public static string ConvertSql(string sql)
        {
            // 下列替换语句有局限性，只能指定小写
            string newSql = sql;
            newSql = sql.Replace("&CLIENT_ID&", Context.ClientID);
            newSql = newSql.Replace(Context.DATA_SOURCE, "wms.");//"wms."   Context.DataSource
            //newSql = newSql.Replace("=''", " is null");
            //newSql = KernelGloballyVariable.Globally_SQL(Context.UserInfo, newSql);
            //KernelGloballyVariable kgv = new KernelGloballyVariable();
            //newSql = Context.GlobalVariable.Globally_SQL(Context.User, newSql);

            //newSql = Context.GlobalVariable.Globally_SQL(Context.User, newSql).sql;

            GloballyDBChange globallyDB = Context.GlobalVariable.Globally_SQL(Context.User, newSql);
            newSql = globallyDB.sql;
            Context.User = globallyDB.userMode;
            //newSql = newSql.Replace("ifnull(", "nvl(");
            /*
            newSql = sql.Replace("ifnull(", CurrentUser.Info.UserDbString.Split(",")[2] + "(");
            newSql = newSql.Replace("now()", CurrentUser.Info.UserDbString.Split(",")[1]);
            newSql = newSql.Replace("@S", CurrentUser.Info.UserDbString.Split(",")[0]);
            newSql = newSql.Replace("&WarehouseID&", CurrentUser.WarehouseID);

            int b = newSql.IndexOf("date_format(");
            while (b >= 0 && CurrentUser.Info.UserDbType.Equals("Oracle"))
            {
                int e = newSql.IndexOf(")", b);
                StringBuilder date = new StringBuilder(newSql.Substring(b, e - b));
                date = date.Replace("date_format(", "to_date(");
                date = date.Replace("%Y", "yyyy");
                date = date.Replace("%m", "mm");
                date = date.Replace("%d", "dd");
                date = date.Replace("%h", "hh");
                date = date.Replace("%H", "hh24");
                date = date.Replace("%i", "mi");
                date = date.Replace("%s", "ss");
                newSql = newSql.Substring(0, b) + date + newSql.Substring(e);
                b = newSql.IndexOf("date_format(");
            }
            */
            return newSql;
        }

        public static void LowerColumnName(DataTable dt)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.ToLower();
            }
        }

        public static readonly int NotFound = 100;
        public static readonly int ErrorOperation = -100;
        public static readonly int ErrorSQL = -102;

        public static DataTable Select(string sql, Dictionary<string, object> param,
            string errMsgPrefix = "", bool errorWhenNotFound = true)
        {
            //model.sql = serviceProvider.GetService<IKernelDESorAES>().DecryptByAES(model.sql);//对前端加密SQL进行解密操作
            //model.sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, model.sql);
            //sql = Context.GlobalVariable.Globally_SQL(Context.User, sql);
            //sql = Context.GlobalVariable.Globally_SQL(Context.User, sql).sql;
            //GloballyDBChange globallyDB = Context.GlobalVariable.Globally_SQL(Context.User, sql);
            //sql = globallyDB.sql;
            //Context.User = globallyDB.userMode;

            //sql = KernelGloballyVariable.Globally_SQL(Context.UserInfo, sql);
            sql = ConvertSql(sql);

            List<Dictionary<string, object>> list = null;
            if (param != null)
            {
                list = new List<Dictionary<string, object>>() { param };
            }
            //using KernelDataBase kernelDataBase = new KernelDataBase();
            DataTable dt = kernelDataBase.DBSelectAll(Context.UserInfo,
            sql, list).Tables[0];
            if (dt.Columns.Count == 1 && dt.Rows.Count == 1 && dt.Columns[0].ColumnName == "error")
            {
                LogHelper.LogDebug(sql);
                LogHelper.LogDebug(JsonConvert.SerializeObject(param));
                throw new WmsException(ErrorSQL, $"{dt.Rows[0][0].ToString()}!");
            }

            if (dt.Rows.Count == 0 && errorWhenNotFound)
            {
                LogHelper.LogDebug(sql);
                LogHelper.LogDebug(JsonConvert.SerializeObject(param));
                throw new WmsException(NotFound, $"{errMsgPrefix}记录未找到!");
            }
            /*
            var output = WmsConfig.GetJsonConfig()["OutputSelectedRows"];

            if (dt.Rows.Count > 0 && output.Contains("FirstRow,AllRows"))
            {
                foreach (DataRow row in dt.Rows)
                {
                    //LogHelper.LogDebug(JsonConvert.SerializeObject(dt.Rows[0].ItemArray));
                    foreach (DataColumn column in dt.Columns)
                    {
                        LogHelper.LogDebug($"{column.ColumnName}=[{row[column]}]");
                    }
                    if (output == "FirstRow") break;
                }
            }

            LowerColumnName(dt);
            */

            return dt;
        }

        public static string SelectOne(string sql, Dictionary<string, object> param,
            string errMsgPrefix = "", bool errorWhenNotFound = true)
        {
            try
            {
                DataTable dt = Select(sql, param, errMsgPrefix, errorWhenNotFound);
                if(dt!=null && dt.Rows.Count==1 && dt.Columns.Count==1 && dt.Columns[0].ColumnName!= "error")
                {
                    string res = dt.Rows[0][0].ToString();
                    return res;
                }else if (!errorWhenNotFound){
                    return "";
                }
                else
                {
                    LogHelper.LogDebug(sql);
                    LogHelper.LogDebug(JsonConvert.SerializeObject(param));
                    throw new WmsException(NotFound, $"{errMsgPrefix}记录未找到!");
                }
            }
            catch(Exception ex)
            {
                LogHelper.LogDebug(sql);
                LogHelper.LogDebug(JsonConvert.SerializeObject(param));
                throw new WmsException(NotFound, $"{errMsgPrefix}查询有误!" + ex.Message);
            }
        }

        public enum OperatedNumType { OnlyOne, OneOrMore, ZeroOrMore }
        public static int RunSql(string sql, Dictionary<string, object> param,
            string errMsgPrefix = "",
            OperatedNumType type = OperatedNumType.OnlyOne, IDbTransaction tran = null)
        {
            //sql = KernelGloballyVariable.Globally_SQL(Context.UserInfo, sql);
            sql = ConvertSql(sql);

            LogHelper.LogDebug(sql);
            LogHelper.LogDebug(JsonConvert.SerializeObject(param));

            tran = tran ?? Context.UserInfo.UserDbConnW.Transaction;

            //using KernelDataBase kernelDataBase = new KernelDataBase();
            kernelDataBase.DBRunSql(Context.UserInfo, new List<string>() { sql },
                out int resCount, out string resMsg, new List<Dictionary<string, object>>() { param });
            LogHelper.LogDebug(string.Format("记录数:{0}, 返回信息[{1}]", resCount, resMsg));

            if (!string.IsNullOrWhiteSpace(resMsg))
            {
                LogHelper.LogError(resMsg);
                throw new WmsException(ErrorSQL, resMsg);
            }
            if (resCount == 0 && type != OperatedNumType.ZeroOrMore)
            {
                //Console.WriteLine(resMsg);
                //Console.WriteLine(Util.GetStackTrace());
                LogHelper.LogError(resMsg);
                throw new WmsException(NotFound, $"{errMsgPrefix}出错了, 找不到要操作的数据" + resMsg);
            }
            if (resCount < 0)
            {
                throw new WmsException(ErrorOperation, $"{errMsgPrefix}出错了:{resMsg}");
            }
            if (resCount != 1 && type == OperatedNumType.OnlyOne)
            {
                throw new WmsException(ErrorOperation, $"{errMsgPrefix}出错了, 操作了过多的数据！");
            }

            return resCount;
        }

        public static int RunSqls(List<string> sqls, List<Dictionary<string, object>> parameters,
            string errMsgPrefix = "",
            OperatedNumType type = OperatedNumType.OnlyOne, IDbTransaction tran = null)
        {
            //sql = KernelGloballyVariable.Globally_SQL(Context.UserInfo, sql);
            List<string> conSqls = new List<string>();

            for (int i = 0; i < sqls.Count; i++)
            {
                conSqls.Add(ConvertSql(sqls[i]));
            }
            //LogHelper.LogDebug(sqls);
            //LogHelper.LogDebug(JsonConvert.SerializeObject(param));
            tran = tran ?? Context.UserInfo.UserDbConnW.Transaction;
            kernelDataBase.DBRunSql(Context.UserInfo, conSqls, out int resCount, out string resMsg, parameters);

            LogHelper.LogDebug(string.Format("记录数:{0}, 返回信息[{1}]", resCount, resMsg));


            if (!string.IsNullOrWhiteSpace(resMsg))
            {
                LogHelper.LogError(resMsg);
                throw new WmsException(ErrorSQL, resMsg);
            }
            if (resCount < 0)
            {
                throw new WmsException(ErrorOperation, $"{errMsgPrefix}出错了:{resMsg}");
            }

            return resCount;
        }

        public static List<IDbDataParameter> RunProc(string procName,
            List<Tuple<string, string, ParameterDirection>> param,
            string errMsgPrefix = "",
            OperatedNumType type = OperatedNumType.OnlyOne)
        {
            //using KernelDataBase kernelDataBase = new KernelDataBase();
            var outputs = kernelDataBase.DBRunProcedure(Context.UserInfo,
                procName, out int resCount, out string resMsg, param);
        
            if (!string.IsNullOrWhiteSpace(resMsg))
            {
                throw new WmsException(ErrorSQL, resMsg);
            }
            /*
            if (resCount < 0)
            {
                throw new WmsException(ErrorSQL, resMsg);
            }
            if (resCount != 1 && type == OperatedNumType.OnlyOne)
            {
                throw new WmsException(ErrorSQL, "存储过程返回过多结果!");
            }
            if (resCount == 0 && type != OperatedNumType.ZeroOrMore)
            {
                throw new WmsException(ErrorSQL, "存储过程没有返回结果!");
            }
            */

            return outputs;
        }

        public static DataSet  RunProcDataSet(string procName,
      List<Tuple<string, string, ParameterDirection>> param  )
        {
            //using KernelDataBase kernelDataBase = new KernelDataBase();
            DataSet ds = kernelDataBase.RunProcedure(Context.UserInfo,
                procName , param);  
            return ds;
        }
        public static string GetDateStr(DateTime date)
        {
            return date == DateTime.MinValue ? null : date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static DateTime? GetDateTime(object obj)
        {
            return (DateTime?)(obj == DBNull.Value ? null : obj);
        }

        public static decimal? GetDecimal(object obj)
        {
            return (decimal?)(obj == DBNull.Value ? null : obj);
        } 

        public static decimal? GetDecimal(string s)
        {
            return (decimal?)(string.IsNullOrWhiteSpace(s) ? (decimal?)null : decimal.Parse(s));
        } 
       
        public static List<T> ConvertTableToObject<T>(DataTable dt) where T:new()
        {
            List<T> list = new List<T>(); 
            //遍历DataTable中所有的数据行
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pro in propertys)
                {
                    //检查DataTable是否包含此列（列名==对象的属性名）  
                    if (dt.Columns.Contains(pro.Name) && dr[pro.Name] != null)
                    {
                        object value = dr[pro.Name];

                        Type tmpType = Nullable.GetUnderlyingType(pro.PropertyType) ?? pro.PropertyType;
                        try
                        {
                            object safeValue = (value == null) ? null : Convert.ChangeType(value, tmpType);

                            //如果非空，则赋给对象的属性  PropertyInfo
                            if (safeValue != DBNull.Value)
                            {
                                pro.SetValue(t, safeValue, null);
                            }
                        }
                        catch { }
                    }
                }
                //对象添加到泛型集合中
                list.Add(t);
            }
            return list;
        }
    }
}
