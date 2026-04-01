using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class CountHeadData
    {
        public String countheadid;
        public String countnumber;
        public String typecode;
        public String typedesc;
        public String statecode;
        public String statecodedesc;
        public String countway;
        public string countwaytypede;
        public String countfw;
        public String createtime;
        public String createuserid;
        public String finishedtime;
        public String finisheduserid;
        public String sourcenumber;
        public String destinnumber;
        public String reason;
        public String field1;
        public String field2;
        public String field3;
        public String field4;
        public String field5;
        public override string ToString()
        {
            return String.Format("盘点单ID：{0} 盘点单号：{1} 类型：{2}/{3} 状态：{4}/{5} 盘点方式：{6}/{7} 盘点条件：{8} 创建时间：{9} 创建人：{10} 结单时间：{11} 结单人：{12} 源单据：{13} 目的单据：{14} 备注：{15} 自定义字段1：{16} 自定义字段2：{17} 自定义字段3：{18} 自定义字段4：{19} 自定义字段5：{20}",
                countheadid, countnumber, typecode, typedesc, statecode, statecodedesc, countway, countwaytypede, countfw, createtime, createuserid, finishedtime, sourcenumber, destinnumber, reason, field1, field2, field3, field4, field5);
        }
    }
}
