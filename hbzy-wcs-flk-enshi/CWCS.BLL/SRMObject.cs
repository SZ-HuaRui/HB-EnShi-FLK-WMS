using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWCS.Core.Library.SRMManager;

namespace CWCS.BLL
{
   public class SRMObject
    {
        public string SrmNo;
        public  SRMControl CSrm;
        public string IpAddress;
        public int Port;
        public int aislenumber;
        public int connectstatus;
        public bool ScMode;
        public bool checkout = true;//是否判断出库站台有货
    }
}
