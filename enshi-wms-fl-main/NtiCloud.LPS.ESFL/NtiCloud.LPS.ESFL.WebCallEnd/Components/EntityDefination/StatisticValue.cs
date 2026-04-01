using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class StatisticValue
    {
        public Decimal Value;
        public String ItemId;
        public String Description;
        public String StatisticUnitCode;
        public String TypeCode;
        public Int32 SequenceNumber;
        public String InterfaceCode;

        public override string ToString()
        {
            return String.Format("Value:{0} ItemId:{1} 描述:{2} 统计单位编号:{3} 类型:{4} 顺序号:{5} 接口编码:{6}",
                Value, ItemId, Description, StatisticUnitCode, TypeCode, SequenceNumber, InterfaceCode);
        }
    }

    [Serializable()]
    public class StatisticValue_View
    {
        public String Id;
        public Decimal? Value;
        public String ItemId;
        public String ItemId_Code;
        public String ItemId_Name;
        public String ItemId_Description;
        public String ItemId_UnitName;
        public String Description;
        public String StatisticUnitCode;
        public String UserId;
        public DateTime? CreateDate;
        public String TypeCode;
        public String TypeText;
        public Int32 SequenceNumber;
        public String InterfaceCode;
    }

    [Serializable()]
    [KnownType(typeof(StatisticValueQueryNoCondition))]
    [KnownType(typeof(StatisticValueQueryConditionList))]
    [KnownType(typeof(StatisticValueQueryByItemIdList))]
    [KnownType(typeof(StatisticValueQueryExistedOnly))]
    [KnownType(typeof(StatisticValueQueryByCatalogId))]
    [DataContract]
    public abstract class StatisticValueQueryConditionBase
    {
        public abstract String CreateCondition();
        public abstract String CreateJoin();
    }

    [Serializable()]
    public class StatisticValueQueryNoCondition : StatisticValueQueryConditionBase
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
    public class StatisticValueQueryConditionList : StatisticValueQueryConditionBase
    {
        public Collection<StatisticValueQueryConditionBase> Conditions = new Collection<StatisticValueQueryConditionBase>();

        public override string CreateCondition()
        {
            if (Conditions == null || Conditions.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            var e = Conditions.GetEnumerator();
            while (e.MoveNext())
            {
                sb.Append(e.Current.CreateCondition());
            }
            return sb.ToString();
        }

        public override string CreateJoin()
        {
            if (Conditions == null || Conditions.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            var e = Conditions.GetEnumerator();
            while (e.MoveNext())
            {
                sb.AppendLine(e.Current.CreateJoin());
            }
            return sb.ToString();
        }
    }

    [Serializable()]
    public class StatisticValueQueryByItemIdList : StatisticValueQueryConditionBase
    {
        public Collection<String> ItemIds = new Collection<String>();

        public override string CreateCondition()
        {
            if (ItemIds == null || ItemIds.Count == 0) return "";

            StringBuilder sb = new StringBuilder(" and (");
            var e = ItemIds.GetEnumerator();
            e.MoveNext();
            while (true)
            {
                sb.Append("item.itemid = '");
                sb.Append(e.Current.Replace("'", "''"));
                sb.Append("'");
                if (e.MoveNext())
                {
                    sb.Append(" or ");
                }
                else
                {
                    break;
                }
            }
            sb.Append(")");
            return sb.ToString();
        }

        public override string CreateJoin()
        {
            return "";
        }
    }

    [Serializable()]
    public class StatisticValueQueryExistedOnly : StatisticValueQueryConditionBase
    {

        public override string CreateCondition()
        {
            return "and not statisticvalue.id is null";
        }

        public override string CreateJoin()
        {
            return "";
        }
    }

    [Serializable()]
    public class StatisticValueQueryByCatalogId : StatisticValueQueryConditionBase
    {
        public String CatalogId = null;

        public override string CreateCondition()
        {
            //if (CatalogId == null)
            //    return "item.parentid is null";
            //else
            return "and item.parentid = '" + CatalogId.Replace("'", "''") + "'";
        }

        public override string CreateJoin()
        {
            return "";
        }
    }


}
