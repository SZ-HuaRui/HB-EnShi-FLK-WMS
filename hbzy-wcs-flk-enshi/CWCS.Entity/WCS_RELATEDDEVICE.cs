using System;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class WCS_RELATEDDEVICE
    {
           public WCS_RELATEDDEVICE(){


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
           /// Desc:关联ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RELATEDID {get;set;}

           /// <summary>
           /// Desc:设备ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DEVICEID {get;set;}

           /// <summary>
           /// Desc:设备编码
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
           /// Desc:关联类型（例如：堆垛机取货站台 SCPUT）
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RELATETYPE {get;set;}

           /// <summary>
           /// Desc:关联序号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? RELATED_SEQ {get;set;}

           /// <summary>
           /// Desc:关联设备ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RELATEDEVICEID {get;set;}

           /// <summary>
           /// Desc:设备编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RELATEDEVICECODE {get;set;}

           /// <summary>
           /// Desc:备注
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RELATEDINFO {get;set;}

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
