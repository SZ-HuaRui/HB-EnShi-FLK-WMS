using System;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class WCS_TASKINFO
    {
           public WCS_TASKINFO(){


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
           /// Desc:任务ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string TASKID {get;set;}

           /// <summary>
           /// Desc:任务号
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int? TASKNO {get;set;}

           /// <summary>
           /// Desc:WMS任务号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WMSTASKNO {get;set;}

           /// <summary>
           /// Desc:托盘条码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BARCODE {get;set;}

           /// <summary>
           /// Desc:起始地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string FROMSTATION {get;set;}

           /// <summary>
           /// Desc:目标地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TOSTATION {get;set;}

           /// <summary>
           /// Desc:优先级
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PRIORITY {get;set;}

           /// <summary>
           /// Desc:任务类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TASKTYPE {get;set;}

           /// <summary>
           /// Desc:任务状态
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TASKSTATUS {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string LEDTEXT {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WMSORDERNO {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? WMSORDERQTY {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WMSBATCHNO {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WMSGROUPNO {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WMSSEQUENCENO {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ITEMNO {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ITEMNAME {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? ITEMQTY {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CREATETIME {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? UPDATETIME {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string GOODSTYPE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WMSTASKFLAG {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string OUTSEQUENCEFLAG {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ZONE {get;set;}


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


    }
}
