using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSInterface
{
    [Serializable]
    public class UsedMaterialsHead
    {
        public UsedMaterialsHead() { }
        private string _usedmaterialsid;
        private string _warehouseid;
        private string _username;
        private string _machinenumber;
        private string _runsgroup;
        private DateTime _createtime;
        private string _field1;
        private string _field2;
        private string _field3;
        private string _field4;
        private string _field5;

        public string USEDMATERIALSID
        {
            get { return _usedmaterialsid; }
            set { _usedmaterialsid = value; }
        }
        public string WAREHOUSEID
        {
            get { return _warehouseid; }
            set { _warehouseid = value; }
        }
        public string USERNAME
        {
            get{return _username;}
            set{_username=value;}
        }
        public string MACHINENUMBER
        {
            get{return _machinenumber;}
            set{_machinenumber=value;}
        }
        public string RUNSGROUP
        {
            get { return _runsgroup; }
            set { _runsgroup = value; }
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
