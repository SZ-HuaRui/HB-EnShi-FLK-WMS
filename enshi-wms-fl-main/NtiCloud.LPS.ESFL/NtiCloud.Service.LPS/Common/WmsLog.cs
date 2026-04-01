using Microsoft.Extensions.DependencyInjection;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS.Model;
using NtiCloud.Service.LPS.Common;
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public class WmsLog
    {
        public static void Write(IServiceProvider provider,IDBContextProvider dbContext,UserMode userMode,Log log) 
        {
            string sql = $@"insert into &DataSource&wms_s_workuserlog
                              (t_id,
                              client_id,
                              warehouse_id,
                              log_text,
                              log_text_long,
                              common_name,
                              type_code,
                              created_by,
                              modify_ip,
                              create_date)
                            values
                              (@t_id,
                              '&CLIENT_ID&',
                              '&WAREHOUSE_ID&',
                              @message,
                              @messageDetail,
                              @action,
                              @type,
                              @created_by,
                              @modify_ip,
                              now())";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = SnowflakeID.Guid().ToString(),
                    ["message"] = log.Message,
                    ["messageDetail"] = $@"传入参数：{log.Parameter},输出结果：{log.Result}",
                    ["action"] = log.Action,
                    ["type"] = log.Type,
                    ["modify_ip"] = userMode.userLoginInfo.LoginIP,
                    ["created_by"] = userMode.userLoginInfo.UserName
                }
            };
            sql = provider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            dbContext.DBRunSql(userMode.userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param);
        }

        public static UserLogModel GetUserLog(Type type, string serviceName, string methodName, dynamic param)
        {
            NtiCloud.Kernel.Core.DescriptionAttribute attr = type.GetAttributeDescription(methodName);

            UserLogModel log = new UserLogModel
            {
                TypeCode = attr.Description,
                LogText=attr.Description,
                CommonName = $"/api/wms/{serviceName}/{methodName}",
                LogTextLong = $"参数:{Newtonsoft.Json.JsonConvert.SerializeObject(param)}"
            };

            return log;
        }
    }
}
