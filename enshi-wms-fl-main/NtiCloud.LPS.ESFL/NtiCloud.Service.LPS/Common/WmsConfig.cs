using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public class WmsConfig
    {
        public static string GetOrDefault(string keyPath, string defaultValue)
        {
            string config = GetRecursively(keyPath, true, false);
            if (config.Length <= 0)
            {
                LogHelper.LogInformation($"未能找到配置[{keyPath}]");
                return defaultValue;
            }
            return config;
        }

        public static string Get(string keyPath)
        {
            return GetRecursively(keyPath, true);
        }

        private static string GetRecursively(string keyPath, bool first, bool errorWhenNotFound = true)
        {
            string key;
            string parent = "1";
            int pos = keyPath.LastIndexOf(":");
            if (pos > 0)
            {
                key = keyPath.Substring(pos + 1);
                parent = GetRecursively(keyPath.Substring(0, pos), false, errorWhenNotFound);
                if (parent.Length <= 0)
                {
                    return parent;
                }
            }
            else
            {
                key = keyPath;
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "parent", parent },
                { "key", key }
            };

            string selSql;

            selSql = "select t_id, client_id, org_id, warehouse_id, config_type, " +
                          "         config_key, config_value, config_name, config_desc, " +
                          "         create_date, created_by, modify_ip, modify_date, " +
                          "         last_modified_by, act_status, p_id from &DataSource&wms_s_config  " +
                          " where p_id = @parent and config_key = @key ";
            DataTable dt = DBHelper.Select(selSql, param,
                $"业务策略表查找[{key}]", errorWhenNotFound);

            if (dt.Rows.Count == 0)
            {
                return string.Empty;
            }
            if (first)
            {
                return dt.Rows[0]["config_value"].ToString();
            }
            else
            {
                return dt.Rows[0]["t_id"].ToString();
            }
        }

        #region 读Json配置用
        public static IConfigurationRoot GetConfig()
        {
            if (config == null)
            {
                lock (config)
                {
                    if (config == null)
                    {
                        var configBasePath = Path.Combine(Directory.GetCurrentDirectory(), "Configs");
                        //config = new ConfigurationBuilder()
                        //    .SetBasePath(configBasePath)
                        //    .Add(new JsonConfigurationSource { Path = "wms.json", ReloadOnChange = true })
                        //    .Build();
                        config = new ConfigurationBuilder()
                            .SetBasePath(configBasePath)
                            .Add(new JsonConfigurationSource { Path = "wms.json", ReloadOnChange = true })
                            .Build();
                    }
                }
            }
            return config;
        }

        private static IConfigurationRoot config = null;
        private static string locker = "Locker";
        /// <summary>
        /// json配置文件读取
        /// </summary>
        /// <param name="configFileName"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public static IConfigurationRoot GetJsonConfig(
           string configFileName = "wms.json",
           string basePath = "")
        {
            if (config == null)
            {
                lock (locker)
                {
                    if (config == null)
                    {
                        //basePath = string.IsNullOrWhiteSpace(basePath)
                        //    ? Path.Combine(Directory.GetCurrentDirectory(), "Configs")
                        //    : basePath;
                        if (string.IsNullOrWhiteSpace(basePath))
                        {
                            string filename = Path.Combine(
                                Directory.GetCurrentDirectory(), configFileName);
                            if (File.Exists(filename))
                            {
                                basePath = Directory.GetCurrentDirectory();
                            }
                            else
                            {
                                basePath = Path.Combine(Directory.GetCurrentDirectory(),
                                    "MicroservicesLibs", "Modules");
                                filename = Path.Combine(basePath, configFileName);
                                if (!File.Exists(filename))
                                {
                                    basePath = Path.Combine(
                                        Directory.GetCurrentDirectory(), "Configs");
                                    filename = Path.Combine(basePath, configFileName);
                                    if (!File.Exists(filename))
                                    {
                                        LogHelper.LogInformation("file {0} not exists", filename);
                                        return null;
                                    }
                                }
                            }
                        }

                        config = new ConfigurationBuilder()
                            .SetBasePath(basePath)
                            .AddJsonFile(configFileName)
                            .Build();
                    }
                }
            }
            return config;
        }
        #endregion
    }
}
