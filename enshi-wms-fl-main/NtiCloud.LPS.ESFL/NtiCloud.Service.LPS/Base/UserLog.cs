using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    public class UserLog : WmsObject
    {
        private static void Insert(UserLogModel model)
        {
            model.ID = model.ID ?? SnowflakeID.Guid().ToString();
            model.WarehouseID = model.WarehouseID ?? Context.WarehouseID;
            model.CreatedBy = model.CreatedBy ?? Context.UserInfo.UserName;
            model.ModifyIP = model.ModifyIP ?? Context.UserInfo.LoginIP;
            model.LastModifiedBy = model.LastModifiedBy ?? Context.UserInfo.UserName;
            model.ActStatus = "1";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "t_id", model.ID },
                { "warehouse_id", model.WarehouseID },
                { "log_text", model.LogText },
                { "log_text_long", model.LogTextLong },
                { "common_name", model.CommonName },
                { "critical_level", model.CriticalLevel.ToString() },
                { "client_number", model.ClientNumber },
                { "type_code", model.TypeCode },
                { "log_no", model.LogNo },
                { "parrent_log_no", model.ParrentLogNo },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into &DataSource&WMS_S_WORKUSERLOG ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       log_text, " +
                "       log_text_long, " +
                "       common_name, " +
                "       critical_level, " +
                "       client_number, " +
                "       type_code, " +
                "       log_no, " +
                "       parrent_log_no, " +
                "       create_date, " +
                "       created_by, " +
                "       modify_ip, " +
                "       modify_date, " +
                "       last_modified_by, " +
                "       act_status) " +
                "values ( " +
                "       @t_id, " +
                "       '&CLIENT_ID&', " +
                "       '&ORG_ID&', " +
                "       @warehouse_id, " +
                "       @log_text, " +
                "       @log_text_long, " +
                "       @common_name, " +
                "       @critical_level, " +
                "       @client_number, " +
                "       @type_code, " +
                "       @log_no, " +
                "       @parrent_log_no, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "系统日志");
        }

        public static void Add(UserLogModel model)
        {
            //DBHelper.BeginTransaction();

            Insert(model);

            //DBHelper.EndTransaction();
        }
    }
}
