using System;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class WCS_PLCMETATAG
    {
           public WCS_PLCMETATAG(){


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
           /// Desc:标签ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TAG_ID {get;set;}

           /// <summary>
           /// Desc:标签序号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? TAG_SEQ {get;set;}

           /// <summary>
           /// Desc:标签名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TAG_NAME {get;set;}

           /// <summary>
           /// Desc:标签编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TAG_CODE {get;set;}

           /// <summary>
           /// Desc:数据类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? TAG_DATATYPE {get;set;}

           /// <summary>
           /// Desc:偏移地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TAG_ADDRESS {get;set;}

           /// <summary>
           /// Desc:数据长度
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? TAG_DATASIZE {get;set;}

           /// <summary>
           /// Desc:标签组ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TAG_GROUPID {get;set;}

           /// <summary>
           /// Desc:设备编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string EQUIPMENTNUM {get;set;}

           /// <summary>
           /// Desc:是否可用
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? TAG_ISACTIVE {get;set;}

           /// <summary>
           /// Desc:是否归档
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? TAG_ARCHIVE {get;set;}

           /// <summary>
           /// Desc:默认值
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TAG_DEFAULTVALUE {get;set;}

           /// <summary>
           /// Desc:标签描述
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TAG_DESCRIPTION {get;set;}

           /// <summary>
           /// Desc:最大值
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? TAG_MAXIMUM {get;set;}

           /// <summary>
           /// Desc:最小值
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? TAG_MINIMUM {get;set;}

           /// <summary>
           /// Desc:循环次数
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? TAG_CYCLE {get;set;}

           /// <summary>
           /// Desc:版本
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? TAG_ROWVERSION {get;set;}

           /// <summary>
           /// Desc:读写模式
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? READWRITEMODE {get;set;}

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
