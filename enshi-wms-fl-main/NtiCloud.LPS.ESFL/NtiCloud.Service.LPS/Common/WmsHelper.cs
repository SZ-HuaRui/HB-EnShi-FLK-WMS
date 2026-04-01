using Microsoft.Extensions.DependencyInjection;
using NtiCloud.Interface.Core;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.Core;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace NtiCloud.Service.WMS.Common
{
    public static class WmsHelper
    {
        private static readonly Dictionary<string, object> seqDictName =
            new Dictionary<string, object>()
            {
                { "WaveNo", "Wave:WaveNo" },
                { "TaskNo", "Task:TaskNo" },
            };

        public static string GetSequence(string seqName)
        {
            if (seqDictName.ContainsKey(seqName))
            {
                string name = seqDictName[seqName].ToString();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    seqName = WmsConfig.GetOrDefault(name, seqName);
                }
            }
            //var service = ServiceLocator.GetService<ISequenceService>();
            //var taskNo = service.GetSequenceNo(Context.Info.SessionKey, "TaskNo").Result;
            //if (taskNo.Contains("错误"))
            //{
            //    throw new WmsException(-611, taskNo);
            //}

            var service = Context.ServiceProvider.GetService<ISequenceService>();
            //string no = service.GetSequenceNo(Context.UserInfo.SessionKey, seqName).Result;
            var no = service.GetSequenceNo(
                new SequenceInput.GetSequenceModel()
                //{ SeqNameOrId = "TaskNo" }).json.ToString();
                { SeqNameOrId = seqName }).json.ToString();
            if (string.IsNullOrWhiteSpace(no))
            {
                throw new WmsException(-701, $"无法获取{seqName}序列号！");
            }
            if (no.Contains("错误") || no.Contains("ErrMsg"))
            {
                throw new WmsException(-700, $"获取{seqName}序列号失败！{no}");
            }

            return no;
        }

        public static DataRow[] GetDictionary(string keyPath)
        {
            return GetDictionary(keyPath, true);
        }

        private static DataRow[] GetDictionary(string keyPath, bool first)
        {
            string key;
            DataRow[] parent = null;
            int pos = keyPath.LastIndexOf(":");
            if (pos > 0)
            {
                key = keyPath.Substring(pos + 1);
                parent = GetDictionary(keyPath.Substring(0, pos), false);
            }
            else
            {
                key = keyPath;
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "p_id", parent == null ? " " : parent[0]["t_id"].ToString() },
                { "dict_code", key }
            };
            string sql =
                "select t_id, client_id, org_id, p_id, dict_code, dict_name, sort_no, " +
                "       description, selected, create_date, created_by, modify_ip, modify_date, " +
                "       last_modified_by, act_status from prod_system_dictionary " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "    and ifnull(p_id, ' ') = @p_id and dict_code = @dict_code ";
            DataTable dt = DBHelper.Select(sql, param, $"数据字典查找[{key}]");
            DataRow[] rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rows, 0);

            return rows;
        }

        public static DataRow[] GetDictionaryBranch(string keyPath)
        {
            return GetDictionaryBranch(keyPath, true);
        }

        private static DataRow[] GetDictionaryBranch(string keyPath, bool first)
        {
            string key;
            DataRow[] parent = null;
            int pos = keyPath.LastIndexOf(":");
            if (pos > 0)
            {
                key = keyPath.Substring(pos + 1);
                parent = GetDictionary(keyPath.Substring(0, pos), false);
            }
            else
            {
                key = keyPath;
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "p_id", parent == null ? " " : parent[0]["t_id"].ToString() },
                { "dict_code", key }
            };
            string sql =
                "select t_id, client_id, org_id, p_id, dict_code, dict_name, sort_no, " +
                "       description, selected, create_date, created_by, modify_ip, modify_date, " +
                "       last_modified_by, act_status from prod_system_dictionary " +
                " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                "    and ifnull(p_id, ' ') = @p_id and dict_code = @dict_code ";
            DataTable dt = DBHelper.Select(sql, param, $"数据字典查找[{key}]");
            if (first && dt.Rows.Count > 0)
            {
                param = new Dictionary<string, object> {
                    { "p_id", dt.Rows[0]["t_id"].ToString() }
                };
                sql =
                    "select t_id, client_id, org_id, p_id, dict_code, dict_name, sort_no, " +
                    "       description, selected, create_date, created_by, modify_ip, modify_date, " +
                    "       last_modified_by, act_status from prod_system_dictionary " +
                    " where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' " +
                    "    and ifnull(p_id, ' ') = @p_id ";
                DataTable dt1 = DBHelper.Select(sql, param, $"数据字典查找[{key}]的下级节点");
                if (dt1.Rows.Count > 0)
                {
                    dt = dt1;
                }
            }
            DataRow[] rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rows, 0);

            return rows;
        }

        public static List<object> ConvertTableToObject<T>(DataTable dt, string className, T newt)
        {
            List<object> list = new List<object>();
            Type type = newt.GetType();

            //遍历DataTable中所有的数据行
            foreach (DataRow dr in dt.Rows)
            {
                var t = Activator.CreateInstance(type);

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

        public static WebJsonInfo ExcuteWithExceDo(Func<WebJsonInfo, WebJsonInfo> func)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                info.errcode = 0;
                info.errmsg = "";
                info.isSucceed = true;
                info.json = "";

                DBHelper.BeginTransaction();
                if (func != null)
                {
                    info = func.Invoke(info);
                }
                DBHelper.EndTransaction(info.errcode != 0 ? false : true);

                return info;
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);

                Model.UserLogModel logModel = new Model.UserLogModel();
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
                logModel.LogText = info.errmsg;
                info.errmsg = info.errmsg
                    + (string.IsNullOrEmpty(info.errmsg) ? "" : ":")
                    + ex.Message;

                logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
                UserLog.Add(logModel);
            }

            return info;
        }

        public static WebJsonInfo ExcuteWithExceDo(Func<WebJsonInfo, WebJsonInfo> func, Model.UserLogModel log)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = log;
            try
            {
                info.errcode = 0;
                info.errmsg = "";
                info.isSucceed = true;
                info.json = "";

                DBHelper.BeginTransaction();
                if (func != null)
                {
                    info = func.Invoke(info);
                }
                DBHelper.EndTransaction(info.errcode != 0 ? false : true);

                logModel.LogText = $"{logModel.LogText}:成功";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
                logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);

                return info;
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);

                info.errmsg = e.ErrorMsg;

                logModel.LogText =$"{logModel.LogText}:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
                logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            }

            UserLog.Add(logModel);

            return info;
        }

        public static WebJsonInfo Do(UserLogModel log, Action action)
        {
            WebJsonInfo info = new WebJsonInfo();

            log.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            //log.TypeCode = LogType.LogType_Operation.ToString();
            try
            {
                DBHelper.BeginTransaction();

                action.Invoke();

                DBHelper.EndTransaction();
                
                log.LogText += ", 执行成功";
                log.LogTextLong += ", 返回信息：成功";
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
                log.LogText += ", 执行失败";
                log.LogTextLong += $", 返回信息：{ex.Message}";
            }
            UserLog.Add(log);
            return info;
        }

        public static WebJsonInfo ResolveJsonResultFromWcs(string jsonResult)
        {
            WebJsonInfo retInfo = new WebJsonInfo();

            LogHelper.LogDebug(jsonResult);

            try
            {
                var je = JsonDocument.Parse(jsonResult).RootElement;
                var errCode = je.GetProperty("errcode").GetInt32();
                if (errCode != 0)
                {
                    retInfo.errcode = errCode;
                    retInfo.errmsg = je.GetProperty("errmsg").ToString();
                    retInfo.isSucceed = false;
                }
                retInfo.json = je.GetProperty("json").ToString();
            }
            catch (Exception ex)
            {
                string errMsg = $"解析WCS返回的结果发生错误：{ex.Message}, 返回结果：{jsonResult}";
                LogHelper.LogError(errMsg);
                retInfo.errcode = -101;
                retInfo.errmsg = errMsg;
                retInfo.isSucceed = false;
            }
            return retInfo;
        }
    }

    /// <summary>
    /// crud类型
    /// </summary>
    public enum SqlType
    {
        C,
        U,
        R,
        D
    }
}
