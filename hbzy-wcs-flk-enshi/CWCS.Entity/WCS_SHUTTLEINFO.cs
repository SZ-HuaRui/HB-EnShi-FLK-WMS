using System;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class WCS_SHUTTLEINFO
    {
           public WCS_SHUTTLEINFO(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string T_ID {get;set;}

           /// <summary>
           /// Desc:仓库ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WAREHOUSE_ID {get;set;}

           /// <summary>
           /// Desc:设备号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string EQUIPMENT_NO {get;set;}

           /// <summary>
           /// Desc:所属设备号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BELONG_EQUIPMENT_NO {get;set;}

           /// <summary>
           /// Desc:设备类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string EQUIPMENT_TYPE {get;set;}

           /// <summary>
           /// Desc:当前任务号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CURRENT_TASKNO {get;set;}

           /// <summary>
           /// Desc:当前行
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CURRENT_LOCATION_LINE {get;set;}

           /// <summary>
           /// Desc:In_Line;In_Sc;In_ChargingStation
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CURRENT_STATUS {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CREATE_TIME {get;set;}

           /// <summary>
           /// Desc:修改时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? MODIFY_TIME {get;set;}

           /// <summary>
           /// Desc:当前层
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CURRENT_LOCATION_LAYER {get;set;}

           /// <summary>
           /// Desc:当前充电站编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CURRENT_CS_NO {get;set;}

           /// <summary>
           /// Desc:当前所在货架侧
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CURRENT_RACK_SIDE {get;set;}

           /// <summary>
           /// Desc:进入或离开充电站的时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? ENTER_LEAVE_CS_TIME {get;set;}

    }
}
