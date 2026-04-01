using CWCS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface
{
    public class WebClientProxy : WebClient
    {
        public int TimeOut { get; set; } = 5000;
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (StaticCommon.WcsParamConfigList.Exists(x => x.PARAM_CODE.Equals("WmsInterface")))
            {
                WCS_PARAMETER_CONFIG config = StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("WmsInterface")).ToList()[0];
                if (config != null)
                {
                    TimeOut = Convert.ToInt32(config.PARAM_VALUE);
                }
            }
            request.Timeout = TimeOut;
            return request;
        }
    }


}
