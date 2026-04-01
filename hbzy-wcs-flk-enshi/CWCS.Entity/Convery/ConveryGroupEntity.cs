using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Entity.Convery
{
    public class ConveryGroupEntity
    {
        public int value;
        public int length;
        public string key_ip;
        public int DBAddress;

        public bool cFlag;

        public List<ConveryEntity> lststation = new List<ConveryEntity>();

        public int perStationNo;
        public int currentStatioNo;

        public string Belong;

    }
}
