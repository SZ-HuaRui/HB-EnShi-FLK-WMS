 
using NtiCloud.Kernel.Core; 
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Tasking
{
    public class TaskService
    { 
        public WebJsonInfo AddTask(IDBContextProvider dbW, UserLoginInfo user, TaskHeaderModel task)
        {
            WebJsonInfo re = new WebJsonInfo();

            if (string.IsNullOrEmpty(task.t_id))
            {
                task.t_id = Guid.NewGuid().ToString();
            }
            string sql = @"insert into "+ Context.DATA_SOURCE + @"wms_b_task
                    (t_id, client_id, org_id, warehouse_id, zone_id, task_no, dependent_task_no, task_type, executed_by, equipment_id, executor_type, actual_equipment_id, seq_no, priority, container_id, container_barcode, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, src_addr_id, src_addr_type, src_addr_text, dest_addr_id, dest_addr_type, dest_addr_text, actual_dest_addr_id, actual_dest_addr_text, task_status, start_date, end_date, is_executable, priority_change_time, led_text, exist_lgv, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status)
                    values
                    ('"+ task.t_id + "', '" + task.client_id + "', '" + task.org_id + "', '" + task.warehouse_id + "', '" + task.zone_id + "'" +
                    ", '" + task.task_no + "', '" + task.dependent_task_no + "', '" + task.task_type + "', '" + task.executed_by + "'" +
                    ", '" + task.equipment_id + "', '" + task.executor_type + "', '" + task.actual_equipment_id + "', '" + task.seq_no + "'" +
                    ", '" + task.priority + "', '" + task.container_id + "', '" + task.container_barcode + "', '" + task.ref_doc_type + "'" +
                    ", '" + task.ref_doc_id + "', '" + task.ref_doc_d_id + "', '" + task.ref_doc_no + "', '" + task.src_addr_id + "'" +
                    ", '" + task.src_addr_type + "', '" + task.src_addr_text + "', '" + task.dest_addr_id + "', '" + task.dest_addr_type + "'" +
                    ", '" + task.dest_addr_text + "', '" + task.actual_dest_addr_id + "', '" + task.actual_dest_addr_text + "', '" + task.task_status + "'" +
                    ", to_date('" + FormatDatetimeSql(task.start_date)   + "','yyyy-MM-dd HH24:mi:ss')"+
                    ", to_date('" + FormatDatetimeSql(task.end_date) + "','yyyy-MM-dd HH24:mi:ss')"+
                    " , '" + task.is_executable + "',"+
                    " to_date('" + FormatDatetimeSql(task.priority_change_time) + "','yyyy-MM-dd HH24:mi:ss')" +
                    ",'" + task.led_text + "', '" + task.exist_lgv + "', sysdate"+
                    ", '" + task.created_by + "', '" + task.modify_ip + "', sysdate, '" + task.last_modified_by + "', '1')";
            int rows;
            string msg = "";
            List<string> lstSql = new List<string>();
            lstSql.Add(DBHelper.ConvertSql(sql));
            dbW.DBRunSql(user, lstSql, out rows, out msg);
            re.isSucceed = rows > 0;
            re.errmsg = msg;
            re.json = task.t_id;
            return re;

        }

        public bool AddTask( TaskHeaderModel task)
        {
            WebJsonInfo re = new WebJsonInfo();

            if (string.IsNullOrEmpty(task.t_id))
            {
                task.t_id = Guid.NewGuid().ToString();
            }
            string sql = @"insert into &DataSource&wms_b_task
                          (t_id, client_id, org_id, warehouse_id, zone_id, task_no, dependent_task_no, task_type, executed_by, equipment_id, 
                            executor_type, actual_equipment_id, seq_no, priority, container_id, container_barcode, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, 
                            src_addr_id, src_addr_type, src_addr_text, dest_addr_id, dest_addr_type, dest_addr_text, actual_dest_addr_id, actual_dest_addr_text, task_status, start_date, 
                            end_date, is_executable, priority_change_time, led_text, exist_lgv, create_date, created_by, modify_ip, modify_date, last_modified_by, 
                            act_status)
                        values
                          (@t_id, @client_id, @org_id, @warehouse_id, @zone_id, @task_no, @dependent_task_no, @task_type, @executed_by, @equipment_id, 
                            @executor_type, @actual_equipment_id, @seq_no, @priority, @container_id, @container_barcode, @ref_doc_type, @ref_doc_id, @ref_doc_d_id, @ref_doc_no, 
                            @src_addr_id, @src_addr_type, @src_addr_text, @dest_addr_id, @dest_addr_type, @dest_addr_text, @actual_dest_addr_id, @actual_dest_addr_text, @task_status, to_date(@start_date,'yyyy-MM-dd HH24:mi:ss'), 
                            to_date(@end_date,'yyyy-MM-dd HH24:mi:ss'), @is_executable, null, @led_text, @exist_lgv, sysdate, @created_by, @modify_ip, sysdate, @last_modified_by, 
                            '1')";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", task.t_id);
            lstParam.Add("client_id", task.client_id);
            lstParam.Add("org_id", task.org_id);
            lstParam.Add("warehouse_id", task.warehouse_id);
            lstParam.Add("zone_id", task.zone_id);
            lstParam.Add("task_no", task.task_no);
            lstParam.Add("dependent_task_no", task.dependent_task_no);
            lstParam.Add("task_type", task.task_type);
            lstParam.Add("executed_by", task.executed_by);
            lstParam.Add("equipment_id", task.equipment_id);

            lstParam.Add("executor_type", task.executor_type);
            lstParam.Add("actual_equipment_id", task.actual_equipment_id);
            lstParam.Add("seq_no", task.seq_no.ToString());
            lstParam.Add("priority", task.priority.ToString());
            lstParam.Add("container_id", task.container_id);
            lstParam.Add("container_barcode", task.container_barcode);
            lstParam.Add("ref_doc_type", task.ref_doc_type);
            lstParam.Add("ref_doc_id", task.ref_doc_id);
            lstParam.Add("ref_doc_d_id", task.ref_doc_d_id);
            lstParam.Add("ref_doc_no", task.ref_doc_no);

            lstParam.Add("src_addr_id", task.src_addr_id);
            lstParam.Add("src_addr_type", task.src_addr_type);
            lstParam.Add("src_addr_text", task.src_addr_text);
            lstParam.Add("dest_addr_id", task.dest_addr_id);
            lstParam.Add("dest_addr_type", task.dest_addr_type);
            lstParam.Add("dest_addr_text", task.dest_addr_text); 
            lstParam.Add("actual_dest_addr_id", task.actual_dest_addr_id);
            lstParam.Add("actual_dest_addr_text", task.actual_dest_addr_text);
            lstParam.Add("task_status", task.task_status);
            lstParam.Add("start_date", FormatDatetimeSql( task.start_date));

            lstParam.Add("end_date", FormatDatetimeSql(task.end_date));
            lstParam.Add("is_executable", task.is_executable);
            lstParam.Add("led_text", task.led_text);
            lstParam.Add("exist_lgv", task.exist_lgv);
            lstParam.Add("created_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);

            return DBHelper.RunSql(sql,lstParam,"创建任务")>0;

        }

        public string FormatDatetimeSql(DateTime? dt) {
            if (dt == null)
            {
                return "";
            }
            else {
                return ((DateTime)dt).ToString("yyyy-MM-dd HH24:mm:ss");
            }
        }

        public WebJsonInfo AddTaskItem(IDBContextProvider dbW, UserLoginInfo user, TaskItemModel taskitem) {

            WebJsonInfo re = new WebJsonInfo();

            if (string.IsNullOrEmpty(taskitem.t_id))
            {
                taskitem.t_id = Guid.NewGuid().ToString();
            }
            string sql = @"insert into "+ Context.DATA_SOURCE + @"wms_b_task_item
  (t_id, client_id, org_id, warehouse_id, task_id, item_id, item_name, plan_qty, processed_qty, finished_qty, uom_qty_id, finished_stat_qty, uom_stat_id, container_d_id, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, start_date, end_date, process_status, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status)
values
  ('"+taskitem.t_id+ "', '" + taskitem.client_id + "', '" + taskitem.org_id + "', '" + taskitem.warehouse_id + "', '" + taskitem.task_id + "'"+
  ", '" + taskitem.item_id + "', '" + taskitem.item_name + "', '" + taskitem.plan_qty + "', '" + taskitem.processed_qty + "'"+
  ", '" + taskitem.finished_qty + "', '" + taskitem.uom_qty_id + "', '" + taskitem.finished_stat_qty + "', '" + taskitem.uom_stat_id + "'"+
  ", '" + taskitem.container_d_id + "', '" + taskitem.ref_doc_type + "', '" + taskitem.ref_doc_id + "', '" + taskitem.ref_doc_d_id + "'"+
  ", '" + taskitem.ref_doc_no + "'"+
  ", to_date('" + FormatDatetimeSql(taskitem.start_date)+ "','yyyy-MM-dd HH24:mi:ss')"+
  ", to_date('" + FormatDatetimeSql(taskitem.end_date) + "','yyyy-MM-dd HH24:mi:ss')" +
  ", '" + taskitem.process_status + "', sysdate,  '" + taskitem.created_by + "',  '" + taskitem.modify_ip + "', sysdate,  '" + taskitem.last_modified_by + "', '1')";

            int rows;
            string msg = "";
            List<string> lstSql = new List<string>();
            lstSql.Add(DBHelper.ConvertSql( sql));
            dbW.DBRunSql(user, lstSql, out rows, out msg);
            re.isSucceed = rows > 0;
            re.errmsg = msg;
            re.json = taskitem.t_id;
            return re;
        }

        public bool AddTaskItem(TaskItemModel taskitem)
        {

            WebJsonInfo re = new WebJsonInfo();

            if (string.IsNullOrEmpty(taskitem.t_id))
            {
                taskitem.t_id = Guid.NewGuid().ToString();
            }
            string sql = @" insert into &DataSource&wms_b_task_item
               (t_id, client_id, org_id, warehouse_id, task_id, item_id, item_name, plan_qty, processed_qty, finished_qty, 
                uom_qty_id, finished_stat_qty, uom_stat_id, container_d_id, ref_doc_type, ref_doc_id, ref_doc_d_id, ref_doc_no, start_date, end_date, 
                process_status, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status)
             values
               (@t_id, @client_id, @org_id, @warehouse_id, @task_id, @item_id, @item_name, @plan_qty, @processed_qty, @finished_qty,
                @uom_qty_id, @finished_stat_qty, @uom_stat_id, @container_d_id, @ref_doc_type, @ref_doc_id, @ref_doc_d_id, @ref_doc_no, to_date(@start_date,'yyyy-MM-dd HH24:mi:ss'), to_date(@end_date,'yyyy-MM-dd HH24:mi:ss') , 
                @process_status, sysdate, @created_by, @modify_ip, sysdate, @last_modified_by, '1')";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", taskitem.t_id);
            lstParam.Add("client_id", taskitem.client_id);
            lstParam.Add("org_id", taskitem.org_id);
            lstParam.Add("warehouse_id", taskitem.warehouse_id);
            lstParam.Add("task_id", taskitem.task_id);
            lstParam.Add("item_id", taskitem.item_id);
            lstParam.Add("item_name", taskitem.item_name);
            lstParam.Add("plan_qty", (taskitem.plan_qty??0).ToString());
            lstParam.Add("processed_qty", (taskitem.processed_qty??0).ToString());
            lstParam.Add("finished_qty", (taskitem.finished_qty??0).ToString());

            lstParam.Add("uom_qty_id", taskitem.uom_qty_id);
            lstParam.Add("finished_stat_qty", taskitem.finished_stat_qty.ToString());
            lstParam.Add("uom_stat_id", taskitem.uom_stat_id);
            lstParam.Add("container_d_id", taskitem.container_d_id);
            lstParam.Add("ref_doc_type", taskitem.ref_doc_type);
            lstParam.Add("ref_doc_id", taskitem.ref_doc_id);
            lstParam.Add("ref_doc_d_id", taskitem.ref_doc_d_id);
            lstParam.Add("ref_doc_no", taskitem.ref_doc_no);
            lstParam.Add("start_date",FormatDatetimeSql(taskitem.start_date));
            lstParam.Add("end_date", FormatDatetimeSql(taskitem.end_date));

            lstParam.Add("process_status", taskitem.process_status);
            lstParam.Add("created_by", Context.UserInfo.UserName);
            lstParam.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParam.Add("last_modified_by", Context.UserInfo.UserName);

            return DBHelper.RunSql(sql,lstParam,"新建任务明细")>0;
        }

    }
}
