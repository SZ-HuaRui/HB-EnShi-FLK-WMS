using CWCS.Interface.WMSMicroService;
using CWCS.UI.FL;
using CWCS.UI.FL.Tools;
using DevExpress.Data.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CWCS.UI
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var aa= BitConverter.ToInt32(new byte[] {0x00,0x01,0x01,0x00},0);
            var bb = BitConverter.GetBytes(99);
            //DevExpressCrack.Crack.Init();
            Process[] list = Process.GetProcessesByName("CWCS.UI");
            if (list != null && list.Length > 1)
            {
                MessageBox.Show("系统已经运行!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Environment.Exit(0);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());

        }
    }
}
