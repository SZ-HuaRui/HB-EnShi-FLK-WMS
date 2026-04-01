using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSInterface.Entity50
{
    public class ContainerInventoryN
    {
        public String WarehouseID;//仓库号
        public String BomId;//配盘单ID

        public String OnHand;//库存
        public String OnScheme;//已分配
        public String OnReject;//损坏
        public String InTransmit;//变化
        public String OnLowSafe;//库存安全：低位
        public String OnHighSafe;//库存安全：高位
        //public String OrderPoint;//定货点
        //public String Field2;//最大存储时间
        ////public String OutSide;
        //public String Rate;//转换率
        //public String tempArea;//暂存区

    }
}
