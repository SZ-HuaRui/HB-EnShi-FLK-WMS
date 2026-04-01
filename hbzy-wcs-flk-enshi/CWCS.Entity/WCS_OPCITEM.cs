using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
   public class WCS_OPCITEM
    {
        /// <summary>
        /// 获取或设置。
        /// </summary>

        public string Itemid { get; set; }

        /// <summary>
        /// 获取或设置。
        /// </summary>


        public string Itemname { get; set; }

        /// <summary>
        /// 获取或设置。
        /// </summary>


        public string Groupname { get; set; }

        /// <summary>
        /// 获取或设置。
        /// </summary>


        public string Objects { get; set; }

        /// <summary>
        /// 获取或设置。
        /// </summary>


        public string Datatype { get; set; }

        /// <summary>
        /// 获取或设置。
        /// </summary>


        public string Signaltype { get; set; }

        /// <summary>
        /// 获取或设置。
        /// </summary>


        public string Value { get; set; }

        /// <summary>
        /// 获取或设置。
        /// </summary>


        public string Stationno { get; set; }

        /// <summary>
        /// 获取或设置。
        /// </summary>


        public string Remark { get; set; }

        /// <summary>
        /// 获取或设置。
        /// </summary>


        public string Userid { get; set; }

        /// <summary>
        /// 获取或设置。
        /// </summary>


        public DateTime? Createtime { get; set; }

        


        public string Warehouseid { get; set; }

        /// <summary>
        /// 获取或设置。
        /// </summary>


        public string Status { get; set; }

        /// <summary>
        /// 获取或设置站台类型。
        /// </summary>


        public string Stationtype { get; set; }

        /// <summary>
        /// 获取或设置位置X。
        /// </summary>


        public string Locationx { get; set; }

        /// <summary>
        /// 获取或设置位置Y。
        /// </summary>


        public string Locationy { get; set; }

        /// <summary>
        /// 获取或设置宽。
        /// </summary>


        public string Width { get; set; }

        /// <summary>
        /// 获取或设置高。
        /// </summary>


        public string Height { get; set; }

        /// <summary>
        /// 获取或设置所属线程。
        /// </summary>


        public string Belong { get; set; }


       /// <summary>
       /// 站台方向
       /// </summary>
        public string Direction { get; set; }

       ///<summary>
       /// 库区
       /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string ZoneCode { get; set; }

        /// <summary>
        /// 箭头方向
        /// </summary>
        public string ArrowDirection { get; set; }

        /// <summary>
        /// 文字描述
        /// </summary>
        public string TextDescription { get; set; }
    }
}
