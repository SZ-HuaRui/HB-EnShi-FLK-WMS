using System;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class WCS_ITEMCODE_HISTORY
    {
           public WCS_ITEMCODE_HISTORY(){


           }
           /// <summary>
           /// Desc:物料ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ITEMCODEID {get;set;}

           /// <summary>
           /// Desc:一号工程码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ITEMCODE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? TASKNO {get;set;}

           /// <summary>
           /// Desc:tagid
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BARCODE {get;set;}

           /// <summary>
           /// Desc:来源地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BATCHNO {get;set;}

           /// <summary>
           /// Desc:入库标记
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TOBACCOTYPE {get;set;}

           /// <summary>
           /// Desc:规格：一箱10000只或者5000只
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? BINWEIGHT {get;set;}

           /// <summary>
           /// Desc:码垛状态
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? TOBACCOWEIGHT {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string EMPTYBINFLAG {get;set;}

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
           public string WAREHOUSEID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string USERID {get;set;}

           /// <summary>
           /// Desc:顺序号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? SEQUENCENO {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BINTYPE {get;set;}

           /// <summary>
           /// Desc:封箱号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BOMNO {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? INITBINWEIGHT {get;set;}

           /// <summary>
           /// Desc:站台
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RFIDNO {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BINDRESULT {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string FROMSTATION {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string VERSION {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string REWORKFALG {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ZONECODE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string GONGYI_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string T_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CLIENT_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ORG_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ACT_STATUS {get;set;}

    }
}
