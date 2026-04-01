using System;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class WCS_PLCMETAGROUP
    {
           public WCS_PLCMETAGROUP(){


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
           /// Desc:组ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PLC_GROUPID {get;set;}

           /// <summary>
           /// Desc:组序号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? PLC_GROUPSEQ {get;set;}

           /// <summary>
           /// Desc:驱动ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PLC_DRIVERID {get;set;}

           /// <summary>
           /// Desc:组名
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PLC_GROUPNAME {get;set;}

           /// <summary>
           /// Desc:更新频率
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? PLC_UPDATERATE {get;set;}

           /// <summary>
           /// Desc:死区
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? PLC_DEADBAND {get;set;}

           /// <summary>
           /// Desc:是否激活
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? PLC_ISACTIVE {get;set;}

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
