using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Request.WMS
{
    //一楼上货口，申请入库站台
    public class InBoundRequestParams
    {
        public InBoundRequestParams()
        {
            //ItemProperties = new ItemProperties();
        }
        public string WarehouseID { get; set; }
        public string ZoneID { get; set; }
        public string ContainerBarCode { get; set; }
        public string StationNo { get; set; }
        public string TaskType { get; set; }
        public string Qty { get; set; } = "1";
        //目标地址类型:null,line,final
        //public string TargetAddressType { get; set; }
        //public ItemProperties ItemProperties { get; set; }
    }

    public class ItemProperties
    {
        public string WaterContent { get; set; }
        public string Weight { get; set; }
        public string Year { get; set; }
        public string Batch { get; set; }
    }

    public class TargetAddressType
    {
        public static string Null { get => "Null";}
        public static string Line { get => "Line"; }
        public static string Final { get => "Final"; }
    }
}
