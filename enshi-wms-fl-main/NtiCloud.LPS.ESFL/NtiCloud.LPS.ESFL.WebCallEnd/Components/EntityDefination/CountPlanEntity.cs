using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace WMSInterface.EntityDefination
{
    /// <summary>
    /// QWH 2009-11-26
    /// </summary>
    [Serializable()]
    public class CountPlanEntity
    {
        public string warehouseID;
        public string zoneID;
        public string countPlanID = Guid.NewGuid().ToString("N");
        public string countPlanNumber;
        public string planName;
        public string planType;
        public int planCycle;
        public DateTime lastReminderDate;
        public DateTime lastReminderBillID;
        public string userID;
        public string stateCode;
        public string lastUpdate;
        public string memo;
        public DateTime createDate;

        public Collection<CountPlanDetailEntity> Details = new Collection<CountPlanDetailEntity>();
    }

    [Serializable()]
    public class CountPlanDetailEntity
    {
        public string warehouseID;
        public string zoneID;
        public string countPlanDetailID = Guid.NewGuid().ToString("N");
        public string countPlanID;
        public string itemID;
        public DateTime createDate;
    }
}
