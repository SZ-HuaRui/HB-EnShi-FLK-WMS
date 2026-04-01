using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class StatisticUnit
    {
        public String UnitId;
        public String ItemId;
        public String Description;
        public String StatisticUnitCode;
        public String TypeCode;
        public Int32 SequenceNumber;
        public String InterfaceCode;

        public override string ToString()
        {
            return String.Format("UnitId:{0} ItemId:{1} 描述:{2} 统计单位编号:{3} 类型:{4} 顺序号:{5} 接口编码:{6}",
                UnitId, ItemId, Description, StatisticUnitCode, TypeCode, SequenceNumber, InterfaceCode);
        }
    }

    [Serializable()]
    public class StatisticUnit_View
    {
        public String Id;
        public String UnitId;
        public String Unit_GroupId;
        public String Unit_CName;
        public String Unit_EName;
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
    [KnownType(typeof(StatisticUnitQueryNoCondition))]
    [KnownType(typeof(StatisticUnitQueryConditionList))]
    [KnownType(typeof(StatisticUnitQueryByItemIdList))]
    [KnownType(typeof(StatisticUnitQueryExistedOnly))]
    [KnownType(typeof(StatisticUnitQueryByCatalogId))]
    [DataContract]
    public abstract class StatisticUnitQueryConditionBase
    {
        public abstract String CreateCondition();
        public abstract String CreateJoin();
    }

    [Serializable()]
    public class StatisticUnitQueryNoCondition : StatisticUnitQueryConditionBase
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
    public class StatisticUnitQueryConditionList : StatisticUnitQueryConditionBase
    {
        public Collection<StatisticUnitQueryConditionBase> Conditions = new Collection<StatisticUnitQueryConditionBase>();

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
    public class StatisticUnitQueryByItemIdList : StatisticUnitQueryConditionBase
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
    public class StatisticUnitQueryExistedOnly : StatisticUnitQueryConditionBase
    {

        public override string CreateCondition()
        {
            return "and not statisticunit.id is null";
        }

        public override string CreateJoin()
        {
            return "";
        }
    }

    [Serializable()]
    public class StatisticUnitQueryByCatalogId : StatisticUnitQueryConditionBase
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
