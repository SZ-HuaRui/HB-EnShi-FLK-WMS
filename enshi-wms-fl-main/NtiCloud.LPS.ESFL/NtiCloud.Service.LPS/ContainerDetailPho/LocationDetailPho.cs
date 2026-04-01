using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Common.Interface;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Tasking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NtiCloud.Service.WMS.Inventory;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using static NtiCloud.Model.WMS.CountContainerInput;
using System.Threading.Tasks;
using TaskStatus = NtiCloud.Service.WMS.Base.TaskStatus;
using NtiCloud.Service.LPS.Inbound;

namespace NtiCloud.Service.WMS.LocationDetailPho
{
    public class LocationDetailPho : WmsObject, ILocationDetailPho
    {
        private readonly IWorkTask _workTask;
        private readonly IWorkTaskItem _workTaskItem;
        private readonly IContainer _container;

        public LocationDetailPho(IWorkTask workTask, IWorkTaskItem workTaskItem, IContainer container)
        {
            _workTask = workTask;
            _workTaskItem = workTaskItem;
            _container = container;
        }

        public LocationDetailPho()
        {
            _workTask = new WorkTask();
            _workTaskItem = new WorkTaskItem();
            _container = new Container();
        }

        #region 创建快照
        /// <summary>
        /// 创建快照
        /// </summary>
        /// <returns></returns>
        public WebJsonInfo CreateDetailPho()
        {
            try
            {
                string curtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Clear();
                string sql = @"insert into &DataSource&wms_b_container_d_pho
                               select d.*,to_date(@curtime,'yyyy-mm-dd hh24:mi:ss')  BAKTIME,@BAKUSER BAKUSER from &DataSource&wms_b_container_d d
                        where warehouse_id = @warehouse_id ";
                
                param.Add("curtime", curtime);
                param.Add("BAKUSER",Context.UserInfo.UserName);
                param.Add("warehouse_id", Context.WarehouseID);
                int count = DBHelper.RunSql(sql, param, "查询货位是否存在",DBHelper.OperatedNumType.ZeroOrMore,null);
                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = null, json = null };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


    }
}
