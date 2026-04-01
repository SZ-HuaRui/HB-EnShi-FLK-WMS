using System;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class WCS_RFID
    {
           public WCS_RFID(){


           }
           /// <summary>
           /// Desc:箱号
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string CASECODESR {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string GUIDSR {get;set;}

           /// <summary>
           /// Desc:烟箱材质
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? TYPEOFCASEIT {get;set;}

           /// <summary>
           /// Desc:约束条码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? RESTRICTIONCODEIT {get;set;}

           /// <summary>
           /// Desc:烟箱状态
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? STATUSIT {get;set;}

           /// <summary>
           /// Desc:初始化重量
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? TARERE {get;set;}

           /// <summary>
           /// Desc:空箱实际重量
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? DYNAMICTARERE {get;set;}

           /// <summary>
           /// Desc:使用场所
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? EXPECTEDLOCSR {get;set;}

           /// <summary>
           /// Desc:是否加盖
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? LIDPRESENCEBO {get;set;}

           /// <summary>
           /// Desc:批次
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BATCHIDSR {get;set;}

           /// <summary>
           /// Desc:牌号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BLENDCODESR {get;set;}

           /// <summary>
           /// Desc:版本
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string VERSIONSR {get;set;}

           /// <summary>
           /// Desc:顺序号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? BATCHSEQUENCENUMIT {get;set;}

           /// <summary>
           /// Desc:烟丝净重
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? NETRE {get;set;}

           /// <summary>
           /// Desc:水分
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? MOISTURERE {get;set;}

           /// <summary>
           /// Desc:时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? PTIME {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WAREHOUSEID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? TASKNO {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BOXTYPE {get;set;}

           /// <summary>
           /// Desc:处理模式
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RFIDMODEL {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CREATEDATE {get;set;}

           /// <summary>
           /// Desc:品牌编码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BRANDCODE {get;set;}

           /// <summary>
           /// Desc:品牌名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BRANDNAME {get;set;}

           /// <summary>
           /// Desc:工单号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ORDERNUMBER {get;set;}

    }
}
