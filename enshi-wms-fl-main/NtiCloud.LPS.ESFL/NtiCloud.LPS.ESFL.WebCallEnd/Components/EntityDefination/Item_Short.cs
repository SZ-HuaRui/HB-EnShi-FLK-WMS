using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class Item_Short
    {
        public String ItemId;
        public String Code;
        public String Name;
        public String Description;
        public Decimal UnitRate;
        public String UnitName;
    }

    [Serializable()]
    public class Item_View
    {
        public String ItemId;
        public String Code;
        public String Name;
        public String Description;
        public String SameCode;
        public String Exteriorcode;
        public Decimal Weight;
        public String ShipperId;
        public String ShipperName;
        public String Field1;
        public String Field2;
        public Decimal UnitRate;
        public String UnitName;
        public String UnitId;
    }

    [Serializable]
    public partial class ITEM
    {
        #region Model
        private string _warehouseid;
        private string _itemid;
        private string _code;
        private string _exteriorcode;
        private string _name;
        private string _description;
        private decimal? _needchecking = 0;
        private decimal? _receivingpercent = 0;
        private decimal? _countpercent = 0;
        private string _labelsample;
        private string _priceid;
        private string _unitid;
        private string _shipperid;
        private string _statecode;
        private string _samecode;
        private string _containeritemid = "NULL";
        private decimal? _digit = 0;
        private decimal? _round = 0;
        private decimal? _eachpalletfillqty = 1;
        private decimal? _palletmode = 1;
        private string _parentid;
        private decimal? _staytime = 0;
        private string _origin;
        private string _itemgrade;
        private decimal? _givenyear = 2009;
        private decimal? _length = 0;
        private decimal? _width = 0;
        private decimal? _height = 0;
        private decimal? _weight = 0;
        private string _cadno;
        private string _typecode;
        private string _zonecode;
        private string _userid;
        private DateTime _createtime = DateTime.Now;
        private string _field1;
        private string _field2;
        private string _field3;
        private string _field4;
        private string _field5;
        private decimal? _maxstaytime = 0;
        /// <summary>
        /// 
        /// </summary>
        public string WAREHOUSEID
        {
            set { _warehouseid = value; }
            get { return _warehouseid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ITEMID
        {
            set { _itemid = value; }
            get { return _itemid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CODE
        {
            set { _code = value; }
            get { return _code; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string EXTERIORCODE
        {
            set { _exteriorcode = value; }
            get { return _exteriorcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string NAME
        {
            set { _name = value; }
            get { return _name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DESCRIPTION
        {
            set { _description = value; }
            get { return _description; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? NEEDCHECKING
        {
            set { _needchecking = value; }
            get { return _needchecking; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? RECEIVINGPERCENT
        {
            set { _receivingpercent = value; }
            get { return _receivingpercent; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? COUNTPERCENT
        {
            set { _countpercent = value; }
            get { return _countpercent; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string LABELSAMPLE
        {
            set { _labelsample = value; }
            get { return _labelsample; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PRICEID
        {
            set { _priceid = value; }
            get { return _priceid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string UNITID
        {
            set { _unitid = value; }
            get { return _unitid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SHIPPERID
        {
            set { _shipperid = value; }
            get { return _shipperid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string STATECODE
        {
            set { _statecode = value; }
            get { return _statecode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SAMECODE
        {
            set { _samecode = value; }
            get { return _samecode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CONTAINERITEMID
        {
            set { _containeritemid = value; }
            get { return _containeritemid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? DIGIT
        {
            set { _digit = value; }
            get { return _digit; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? ROUND
        {
            set { _round = value; }
            get { return _round; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? EACHPALLETFILLQTY
        {
            set { _eachpalletfillqty = value; }
            get { return _eachpalletfillqty; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? PALLETMODE
        {
            set { _palletmode = value; }
            get { return _palletmode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PARENTID
        {
            set { _parentid = value; }
            get { return _parentid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? STAYTIME
        {
            set { _staytime = value; }
            get { return _staytime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ORIGIN
        {
            set { _origin = value; }
            get { return _origin; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ITEMGRADE
        {
            set { _itemgrade = value; }
            get { return _itemgrade; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? GIVENYEAR
        {
            set { _givenyear = value; }
            get { return _givenyear; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? LENGTH
        {
            set { _length = value; }
            get { return _length; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? WIDTH
        {
            set { _width = value; }
            get { return _width; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? HEIGHT
        {
            set { _height = value; }
            get { return _height; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? WEIGHT
        {
            set { _weight = value; }
            get { return _weight; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CADNO
        {
            set { _cadno = value; }
            get { return _cadno; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TYPECODE
        {
            set { _typecode = value; }
            get { return _typecode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ZONECODE
        {
            set { _zonecode = value; }
            get { return _zonecode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string USERID
        {
            set { _userid = value; }
            get { return _userid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CREATETIME
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FIELD1
        {
            set { _field1 = value; }
            get { return _field1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FIELD2
        {
            set { _field2 = value; }
            get { return _field2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FIELD3
        {
            set { _field3 = value; }
            get { return _field3; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FIELD4
        {
            set { _field4 = value; }
            get { return _field4; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FIELD5
        {
            set { _field5 = value; }
            get { return _field5; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? MAXSTAYTIME
        {
            set { _maxstaytime = value; }
            get { return _maxstaytime; }
        }
        #endregion Model

        public override string ToString()
        {
            return String.Format(@"物料ID：{0} 物料种类：{1} 物料编码：{2} 物料名称：{3} 物料描述：{4} 类型：{5} 状态：{6} 物料重量：{7} 
物料等级：{8} 产地：{9} 货主：{10}  ", ITEMID, PARENTID,CODE, NAME, DESCRIPTION, TYPECODE, STATECODE, WEIGHT,ITEMGRADE,ORIGIN,SHIPPERID);
        }

        [Serializable()]
        [KnownType(typeof(ITEMQueryNoCondition))]
        [KnownType(typeof(ITEMQueryByQueryConditions))]
        [KnownType(typeof(ITEMQueryByParentID))]
        [KnownType(typeof(ITEMQueryByItemID))]
        [DataContract]
        public abstract class ITEMQueryConditionBase
        {
            public abstract String CreateCondition();
            public abstract String CreateJoin();
        }

        [Serializable()]
        public class ITEMQueryNoCondition : ITEMQueryConditionBase
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
        public class ITEMQueryByQueryConditions : ITEMQueryConditionBase
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
        public class ITEMQueryByParentID : ITEMQueryConditionBase
        {
            public String ParentId = null;

            public override string CreateCondition()
            {
                return "and parentid = '" + ParentId.Replace("'", "''") + "'";
            }

            public override string CreateJoin()
            {
                return "";
            }
        }

        [Serializable()]
        public class ITEMQueryByItemID : ITEMQueryConditionBase
        {
            public String ItemId = null;

            public override string CreateCondition()
            {
                return "and itemid = '" + ItemId.Replace("'", "''") + "'";
            }

            public override string CreateJoin()
            {
                return "";
            }
        }
    }
}
