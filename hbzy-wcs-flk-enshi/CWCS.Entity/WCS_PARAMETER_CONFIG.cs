using System;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class WCS_PARAMETER_CONFIG
    {
           public WCS_PARAMETER_CONFIG(){


           }
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
           public string PARAM_CODE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PARAM_NAME {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PARAM_VALUE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PARAM_TYPE {get;set;}

           public string CAN_EDIT { get; set; }

    }
}
