using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WMSInterface.Entity50
{
    [Serializable()]
    public class TaskEntity
    {
        #region Model
        public String ITEMID;
        public decimal OPERATIONQTY;
        public int LGVEXIST;
        public String WAREHOUSEID;
        public String TASKTYPECODE;
        public String  TASKEXECUTORTYPECODE;
        public String  TASKNUMBER;
        public String  EXECUTIVEEQUIPMENTID;
        public String  ACTUALEXECUTIVEEQUIPMENTID = null;
        public String  GROUPNUMBER = "";
        public String  SEQUENCENUMBER = "";
        public int  PRIORITY;
        public String  CONTAINERID;
        public String  CONTAINERBARCODE;
        public String  LOTID = null;
        public String  LOTNUMBER = "";
        public String  BOMCODE = "";
        public String  SOURCEADDRESSID = null;
        public String  SOURCEBILLTYPE = "";
        public String  SOURCEADDRESSTYPE = "";
        public String  SOURCEADDRESSTEXT;
        public String  ACTUALSOURCEADDRESSID = null;
        public String  ACTUALSOURCEADDRESSTEXT = "";
        public String  TARGETADDRESSID = null;
        public String  TARGETADDRESSTYPE = "";
        public String  TARGETADDRESSTEXT;
        public String  ACTUALTARGETADDRESSID = null;
        public String  ACTUALTARGETADDRESSTEXT = "";
        public String  STATECODE;
        public DateTime  STARTDATE;
        public DateTime  FINISHDATE;
        public String  RECEIVINGLOTNUMBER;
        public String  SCHEMEID = null;
        public String  SCHEMENUMBER = "";
        public String  COMMONID = null;
        public String  COMMONNUMBER = "";
        public String  COMMONDETAILID = null;
        public String  COMMONDETAILNUMBER = "";
        public int  EXECUTABLE;
        public DateTime  PRIORITYCHANGETIME;
        public String  DEPENDENCETASKNUMBER = "";
        public String  LEDTEXT;
        public String  USERID;
        public DateTime  CREATETIME = DateTime.Now;
        public String  FIELD1;
        public String  FIELD2;
        public String  FIELD3;
        public String  FIELD4;
        public String  FIELD5;
        public String  VERSION;

        #endregion Model

    }

    [Serializable()]
    [KnownType(typeof(ManCallQueryNoCondition))]
    [KnownType(typeof(ManCallQueryByQueryConditions))]
    [KnownType(typeof(ManCallQueryByTaskNumber))]
    [DataContract]
    public abstract class ManCallQueryConditionBase
    {
        public abstract String CreateCondition();
        public abstract String CreateJoin();
    }

    [Serializable()]
    public class ManCallQueryNoCondition : ManCallQueryConditionBase
    {
        public override string CreateCondition()
        {
            return "";
        }
        public override string CreateJoin()
        {
            return "";
        }
    }

    [Serializable()]
    public class ManCallQueryByQueryConditions :ManCallQueryConditionBase
    {
        public String condition = null;

        public override string CreateCondition()
        {
            return condition.ToString();
        }

        public override string CreateJoin()
        {
            return "";
        }
    }

    [Serializable()]
    public class ManCallQueryByTaskNumber : ManCallQueryConditionBase
    {
        public String tasknumber = null;

        public override string CreateCondition()
        {
            return "and TaskNumber = '" + tasknumber.Replace("'", "''") + "'";
        }

        public override string CreateJoin()
        {
            return "";
        }
    }
    [Serializable()]
    [KnownType(typeof(ConfirmUsedQueryNoCondition))]
    [KnownType(typeof(ConfirmUsedByQueryConditions))]
    [KnownType(typeof(ConfirmUsedByConUsedMaterHeadID))]

    [DataContract]
    public abstract class ConfirmUsedQueryConditionBase
    {
        public abstract String CreateCondition();
        public abstract String CreateJoin();
    }
    [Serializable()]
    public class ConfirmUsedQueryNoCondition : ConfirmUsedQueryConditionBase
    {
        public override string CreateCondition()
        {
            return "";
        }
        public override string CreateJoin()
        {
            return "";
        }
    }
    [Serializable()]
    public class ConfirmUsedByQueryConditions : ConfirmUsedQueryConditionBase
    {
        public String condition = null;
        public override string CreateCondition()
        {
            return condition.ToString();
        }
        public override string CreateJoin()
        {
            return "";
        }
    }
    [Serializable()]
    public class ConfirmUsedByConUsedMaterHeadID : ConfirmUsedQueryConditionBase
    {
        public String ConUsedMaterHeadID = null;
        public override string CreateCondition()
        {
            return "and ConUsedMaterHeadID = '" + ConUsedMaterHeadID.Replace("'", "''") + "'";
        }
        public override string CreateJoin()
        {
            return "";
        }
    }
}
