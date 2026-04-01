using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSInterface
{
    [Serializable]
    public class UsedMaterialsDetail
    {
        private string _warehouseid;
        private string _usedmaterialsdetailid;
        private string _usedmaterialsid;     
        private string _modelcode;
        private string _itemcode;
        private string _itemid;
        private Decimal _callqty;
        private Decimal _returnqty;
        private Decimal _usedqty;
        private string _unit1id;
        private Decimal _rate;
        private Decimal _usedcount;
        private string _usedunitid;
        private string _shipperid;
        private DateTime _createtime;
        private string _field1;
        private string _field2;
        private string _field3;
        private string _field4;
        private string _field5;

        public string WAREHOUSEID
        {
            get { return _warehouseid; }
            set { _warehouseid = value; }
        }

        public string USEDMATERIALSDETAILID
        {
            get { return _usedmaterialsdetailid; }
            set { _usedmaterialsdetailid = value; }
        }

        public string USEDMATERIALSID
        {
            get { return _usedmaterialsid; }
            set { _usedmaterialsid = value; }
        }
        
        public string MODELCODE
        {
            get { return _modelcode; }
            set { _modelcode = value; }
        }
        public string ITEMCODE
        {
            get { return _itemcode; }
            set { _itemcode = value; }
        }
        public string ITEMID
        {
            get { return _itemid; }
            set { _itemid = value; }
        }
        public Decimal CALLQTY
        {
            get { return _callqty; }
            set { _callqty = value; }
        }
        public Decimal RETURNQTY
        {
            get { return _returnqty; }
            set { _returnqty = value; }
        }
        public Decimal USEDQTY
        {
            get { return _usedqty; }
            set { _usedqty = value; }
        }
        public string UNIT1ID
        {
            get { return _unit1id; }
            set { _unit1id = value; }
        }
        public Decimal RATE
        {
            get { return _rate; }
            set { _rate = value; }
        }
        public Decimal USEDCOUNT
        {
            get { return _usedcount; }
            set { _usedcount = value; }
        }
        public string USEDUNITID
        {
            get { return _usedunitid; }
            set { _usedunitid = value; }
        }
        public string SHIPPERID
        {
            get { return _shipperid; }
            set { _shipperid = value; }
        }
        public DateTime CREATETIME
        {
            get { return _createtime; }
            set { _createtime = value; }
        }
        public string FIELD1
        {
            get { return _field1; }
            set { _field1 = value; }
        }
        public string FIELD2
        {
            get { return _field2; }
            set { _field2 = value; }
        }
        public string FIELD3
        {
            get { return _field3; }
            set { _field3 = value; }
        }
        public string FIELD4
        {
            get { return _field4; }
            set { _field4 = value; }
        }
        public string FIELD5
        {
            get { return _field5; }
            set { _field5 = value; }
        }
    }
}
