using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Entity.SRM
{
    public class SCStationInfo
    {
        public string StationNum;
        public string Line;  //堆垛机取放货行  与输送机对应
        public string Column;//堆垛机取放货列  与输送机对应
        public string Layer; //堆垛机取放货层  与输送机对应
        public string Deep;  //堆垛机取放货深  与输送机对应
    }
}
