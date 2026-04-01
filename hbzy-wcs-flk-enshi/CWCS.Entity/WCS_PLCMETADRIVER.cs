using System;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class WCS_PLCMETADRIVER
    {
           public WCS_PLCMETADRIVER(){


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
           /// Desc:驱动ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DRIVERID {get;set;}

           /// <summary>
           /// Desc:驱动类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? DRIVER_TYPE {get;set;}

           /// <summary>
           /// Desc:驱动名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DRIVER_NAME {get;set;}

           /// <summary>
           /// Desc:驱动描述
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DRIVER_DESC {get;set;}

           /// <summary>
           /// Desc:驱动序号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? DRIVER_SEQ {get;set;}

           /// <summary>
           /// Desc:设备类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DEVICETYPE {get;set;}

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
