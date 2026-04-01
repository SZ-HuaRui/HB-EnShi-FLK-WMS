using System;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class WCS_ROUTE
    {
           public WCS_ROUTE(){


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
           /// Desc:路径ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ROUTEID {get;set;}

           /// <summary>
           /// Desc:堆垛机起始位置
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string FROMSCNO {get;set;}

           /// <summary>
           /// Desc:堆垛机目标地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TOSCNO {get;set;}

           /// <summary>
           /// Desc:输送机起始位置
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CONVERYFROMNO {get;set;}

           /// <summary>
           /// Desc:输送机目标位置
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CONVERYTONO {get;set;}

           /// <summary>
           /// Desc:AGV起始位置
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string AGVFROMNO {get;set;}

           /// <summary>
           /// Desc:AGV目标地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string AGVTONO {get;set;}

           /// <summary>
           /// Desc:任务类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TASKTYPE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PLATNO {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CHANGEWAREHOUSEFLAG {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string STVFROMNO {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string STVTONO {get;set;}

           /// <summary>
           /// Desc:仓库
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WAREHOUSEID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string USERID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string MIDSTATION {get;set;}

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
