using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class ReceivingHead
    {
        public String RECEIVINGID;
        public String RECEIVINGNUMBER;
        public String SCHEMEID;
        public String SCHEMENUMBER;
        public String SOURCEBILLNUMBER;
        public String COMMONID;
        public String COMMONNUMBER;
        public String RECEIVINGHEADMODE;
        public String LOTID;
        public String LOTNUMBER;
        public String BOMCODE;
        public String SHIPPERID;
        public String DELIVERYMAN;
        public String RECEIVINGDATE;
        public String STATECODE;
        public String USERID;
        public String SOURCENUMBER;
        public String DESTINNUMBER;
        public String DOCKID;
        public Int32 CANEDIT;
        public Int32 CREATEDATE;
        public Int32 LEFTTASKCOUNT;
        public String FIELD1;
        public String FIELD2;
        public String FIELD3;
        public String FIELD4;
        public String FIELD5;

        public override string ToString()
        {
            return String.Format(@"RECEIVINGID：{0} 收货单号：", RECEIVINGID, RECEIVINGNUMBER);
        }
    }

    [Serializable()]
    public class ReceivingWithDetails : ReceivingHead
    {
        public List<ReceivingDetail> Details = new List<ReceivingDetail>();
        public override string ToString()
        {
            return String.Format(@"RECEIVINGID：{0} 收货单号：", RECEIVINGID, RECEIVINGNUMBER);
        }
    }

    [Serializable()]
    public class ReceivingDetail
    {
        public String RECEIVINGID;
        public String DETAILID;
        public String ITEMID;
        public Decimal SCHEMEQTY;
        public Decimal ACTUALQTY;
        public Decimal PROCESSEDQTY;
        public Decimal CROSSEDQTY;
        public String MEMO; //
        public String PRICEID; //
        public Int32 HAVECHECKED;//
        public String RECEIVINGLOTNUMBER;
        public String ITEMSUPPLIER;//
        public String USERID;//
        public String FIELD1;
        public String FIELD2;
    }

    [Serializable()]
    public class ReceivingInfo
    {
        public String RECEIVINGID;
        public String RECEIVINGNUMBER;
        public String SCHEMEID;
        public String SCHEMENUMBER;
        public String SOURCEBILLNUMBER;
        public String COMMONID;
        public String COMMONNUMBER;
        public String RECEIVINGHEADMODE;
        public String LOTID;
        public String LOTNUMBER;
        public String BOMCODE;
        public String SHIPPERID;
        public String DELIVERYMAN;
        public String RECEIVINGDATE;
        public String STATECODE;
        public String USERID;
        public String SOURCENUMBER;
        public String DESTINNUMBER;
        public String DOCKID;
        public Int32 CANEDIT;
        public Int32 CREATEDATE;
        public Int32 LEFTTASKCOUNT;
        public String FIELD1;
        public String FIELD2;
        public String FIELD3;
        public String FIELD4;
        public String FIELD5; 
        public String DETAILID;
        public String ITEMID;
        public Decimal SCHEMEQTY;
        public Decimal ACTUALQTY;
        public Decimal PROCESSEDQTY;
        public Decimal CROSSEDQTY;
        public String MEMO; //    
        public Int32 HAVECHECKED;//
        public String RECEIVINGLOTNUMBER;//
        public String ITEMSUPPLIER;//     
        public String DetailField1 ;
        public String DetailField2;
    }

    [Serializable()]
    [KnownType(typeof(ReceivingQueryNoCondition))]
    [KnownType(typeof(ReceivingQueryByQueryConditions))]
    [KnownType(typeof(ReceivingQueryByReceivingId))]
    [DataContract]
    public abstract class ReceivingQueryConditionBase
    {
        public abstract String CreateCondition();
        public abstract String CreateJoin();
    }

    [Serializable()]
    public class ReceivingQueryNoCondition : ReceivingQueryConditionBase
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
    public class ReceivingQueryByQueryConditions : ReceivingQueryConditionBase
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
    public class ManPowerCallTaskQueryByQueryConditions : ReceivingQueryConditionBase
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
    public class ReceivingQueryByReceivingId : ReceivingQueryConditionBase
    {
        public String ReceivingId = null;

        public override string CreateCondition()
        {
            return "and Receivingid = '" + ReceivingId.Replace("'", "''") + "'";
        }

        public override string CreateJoin()
        {
            return "";
        }
    }
}
