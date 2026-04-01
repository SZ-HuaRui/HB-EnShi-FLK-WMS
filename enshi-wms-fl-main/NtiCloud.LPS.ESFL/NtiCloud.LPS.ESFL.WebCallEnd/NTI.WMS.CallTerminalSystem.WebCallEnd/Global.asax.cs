using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            Ajax.Utility.HandlerPath = "ajax";
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
            Session.Remove("Equipment");
            Session.Remove("UserName");
            Session.Remove("PassWord");
            Session.Remove("WarehouseID");

            Session.Remove("WorkShop");
            Session.Remove("MachineNumber");
            Session.Remove("HasLGV");
            Session.Remove("Run");
            Session.Remove("RunsGroup");
            Session.Remove("MachineName");
            Session.Remove("RememberPsw");//["RememberPsw"]
        }
        //protected void Application_Start(Object sender, EventArgs e)
        //{
        //    Ajax.Utility.HandlerPath = "ajax";
        //} 
    }
}
