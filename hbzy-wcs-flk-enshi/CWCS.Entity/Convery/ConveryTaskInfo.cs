using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Entity.Convery
{
   public class ConveryTaskInfo
    {
        public string StationNo { get; set; }  //设备编号
        public int TaskNumber { get; set; }   //任务号
        public string Barcode { get; set; }//托盘条码
        public int GoodsType { get; set; }  //货物类型
        public int FromStation { get; set; }  //开始地址
        public int ToStation { get; set; }   //目标地址
        public int CheckInfo { get; set; }   //输送机故障位
        public int Qty { get; set; }//数量
        public int BoxType { get; set; }//烟箱类型
        public int StationType { get; set; }
        public int RfidFlag { get; set; }//Rfid标记位

        public int SystemError;//WCS、WMS内部错误

    }
}
