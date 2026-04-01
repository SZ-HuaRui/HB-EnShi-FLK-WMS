using System;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class WCS_DEVICE
    {
           public WCS_DEVICE(){


           }
           /// <summary>
           /// Desc:主键ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string T_ID {get;set;}

           /// <summary>
           /// Desc:集团公司ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CLIENT_ID {get;set;}

           /// <summary>
           /// Desc:组织ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ORG_ID {get;set;}

           /// <summary>
           /// Desc:仓库ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WAREHOUSEID {get;set;}

           /// <summary>
           /// Desc:区域ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ZONEID {get;set;}

           /// <summary>
           /// Desc:设备ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DEVICEID {get;set;}

           /// <summary>
           /// Desc:设备编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DEVICECODE {get;set;}

           /// <summary>
           /// Desc:设备类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DEVICETYPE {get;set;}

           /// <summary>
           /// Desc:X坐标
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? COORDINATE_X {get;set;}

           /// <summary>
           /// Desc:Y坐标
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? COORDINATE_Y {get;set;}

           /// <summary>
           /// Desc:Z坐标
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? COORDINATE_Z {get;set;}

           /// <summary>
           /// Desc:长度
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? LENGTH {get;set;}

           /// <summary>
           /// Desc:宽度
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? WIDTH {get;set;}

           /// <summary>
           /// Desc:高度
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? HIGHT {get;set;}

           /// <summary>
           /// Desc:状态
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string STATUS {get;set;}

           /// <summary>
           /// Desc:大小
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? DEVICESIZE {get;set;}

           /// <summary>
           /// Desc:是否激活
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? ISACTIVE {get;set;}

           /// <summary>
           /// Desc:是否可用
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? ISENABLE {get;set;}

           /// <summary>
           /// Desc:转速
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? SPEED {get;set;}

           /// <summary>
           /// Desc:交互类型（0 默认，1 初始化空箱站台等等）
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? INTERACTETYPE {get;set;}

           /// <summary>
           /// Desc:显示方向(V 水平 T 垂直)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DIRECTION {get;set;}

           /// <summary>
           /// Desc:流向(L向左；R向右；U向上；D向下；TTW垂直双向；VTW 水平双向)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string FLOW {get;set;}

           /// <summary>
           /// Desc:设备规格
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string STANDARD {get;set;}

           /// <summary>
           /// Desc:最大任务数量
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? MAXGETTASKNUM {get;set;}

           /// <summary>
           /// Desc:交互区域
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string AREACODE {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CREATE_DATE {get;set;}

           /// <summary>
           /// Desc:创建人帐号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CREATED_BY {get;set;}

           /// <summary>
           /// Desc:操作人IP
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string MODIFY_IP {get;set;}

           /// <summary>
           /// Desc:编辑时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? MODIFY_DATE {get;set;}

           /// <summary>
           /// Desc:最后修改人帐号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string LAST_MODIFIED_BY {get;set;}

           /// <summary>
           /// Desc:状态
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ACT_STATUS {get;set;}

    }
}
