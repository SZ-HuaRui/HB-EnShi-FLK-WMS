using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public abstract class Command
    {

        protected DockInfo _Info;
        protected User _User;
        public string TaskTypeCode;
        public ReturnResult Result = ReturnResult.None;

        private string taskNumber;

        public string TaskNumber
        {
            get
            {
                return taskNumber;
            }
            set
            {
                taskNumber = value;
            }
        }

        public User User
        {
            get { return _User; }
            set { _User = value; }
        }
        public DockInfo Info
        {
            get { return _Info; }
            set { _Info = value; }
        }


        public abstract string Execute();
        /// <summary>
        /// 要料
        /// </summary>
        /// <param name="url">微服务url</param>
        /// <param name="machineId">机台id</param>
        /// <param name="hasLGV">是否需要LGV</param>
        /// <param name="dockId">机台id</param>
        /// <param name="productPlanOutId">生产工单id</param>
        /// <param name="shiftId">班次id</param>
        /// <param name="teamId">班组id</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public abstract string ExecuteCallMaterials(string url, string machineId, string hasLGV, string dockId, string productPlanOutId, string shiftId, string teamId, string token);
        /// <summary>
        /// 
        /// </summary>
        /// <returns>true：存在；false：不存在</returns>
        public abstract bool Check();

        //退空托盘+要料提示需要准确
        public abstract int ExcuteCallAndReturn();
    }
}