using System;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class WCS_SUBTASKINFO
    {
           public WCS_SUBTASKINFO(){


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
           /// Desc:子任务ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string SUBTASKNO {get;set;}

           /// <summary>
           /// Desc:任务号
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int ? TASKNO {get;set;}

           /// <summary>
           /// Desc:设备类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string EQUIPMENTTYPE {get;set;}

           /// <summary>
           /// Desc:来源地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string FROMSTATION {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string MIDSTATION {get;set;}

           /// <summary>
           /// Desc:目标地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TOSTATION {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CURRENTSTATION {get;set;}

           /// <summary>
           /// Desc:0:未开始，1:完成，2:执行中，3:预约货位，4:货位预约成功，5:托盘加载失败反馈直接发送托盘信息，
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TASKSTATUS {get;set;}

           /// <summary>
           /// Desc:托盘条码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BARCODE {get;set;}

           /// <summary>
           /// Desc:任务状态
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RUNSTATUS {get;set;}

           /// <summary>
           /// Desc:设备编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string EQUIPMENTNUMBER {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string AGVIKEY {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string AGVINDEX {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string AGVMAGIC {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string AGVCARNO {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string AGVSTATUS {get;set;}

         
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ZONE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string USERID {get;set;}

           /// <summary>
           /// Desc:堆垛机完成时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? FINISHEDTIME {get;set;}


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
