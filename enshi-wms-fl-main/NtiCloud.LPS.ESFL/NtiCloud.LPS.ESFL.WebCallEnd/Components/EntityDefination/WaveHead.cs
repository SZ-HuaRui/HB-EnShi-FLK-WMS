using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class WaveHead
    {
        public String WaveNumber;
        public String TypeCode;

        public override string ToString()
        {
            return String.Format("波次号：{0} 类型：{1}", WaveNumber, TypeCode);
        }
    }

    [Serializable()]
    public class WaveHead_View
    {
        public String WaveId;
        public String WaveNumber;
        public DateTime ReleaseDate_DateTime;
        public String ReleaseUserId;
        public DateTime CreateDate_DateTime;
        public String CreateUserId;
        public String TypeCode;
        public String TypeCode_Text;
        public Decimal LeftTaskQty;
        public String StateCode;
        public String StateCode_Text;
    }
}
